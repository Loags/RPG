using LB.Interactable;
using LB.Inventory;
using LB.Loot.Currency;
using LB.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LB.Loot
{
    public class LootManager : PersistantSingleton<LootManager>
    {
        private readonly List<ItemObject> possibleLoot = new();

        /// <summary>
        /// Spawns loot for the given enemy at the specified position
        /// </summary>
        public void SpawnLoot(EnemyType enemyId, Vector3 position)
        {
            LootDatabase.LootConfiguration config = GetOrCreateConfiguration(enemyId);
            if (config == null)
            {
                Debug.LogError($"No loot configuration found for enemy: {enemyId} and couldn't create one");
                return;
            }

            SpawnItems(config, position);
        }

        /// <summary>
        /// Gets existing configuration or creates a temporary one based on templates
        /// </summary>
        private LootDatabase.LootConfiguration GetOrCreateConfiguration(EnemyType enemyId)
        {
            // Try to get existing configuration
            LootDatabase.LootConfiguration config = LootDatabase.Instance.GetLootConfiguration(enemyId);
            
            // If configuration doesn't exist but we have default templates, create a temporary configuration
            if (config == null)
            {
                LootSourceTemplate template = LootDatabase.Instance.GetDefaultTemplateForEnemyType(enemyId);
                if (template != null)
                {
                    Debug.Log($"Creating temporary loot configuration for {enemyId} using template {template.TemplateName}");
                    
                    // Create temporary configuration object
                    // Note: This won't be saved to the database
                    var tempConfig = new LootDatabase.LootConfiguration();
                    
                    // Use reflection to set template and enemyId
                    var field = typeof(LootDatabase.LootConfiguration).GetField("enemyType", 
                        System.Reflection.BindingFlags.NonPublic | 
                        System.Reflection.BindingFlags.Instance);
                    field?.SetValue(tempConfig, enemyId);
                    
                    field = typeof(LootDatabase.LootConfiguration).GetField("lootTemplate", 
                        System.Reflection.BindingFlags.NonPublic | 
                        System.Reflection.BindingFlags.Instance);
                    field?.SetValue(tempConfig, template);
                    
                    return tempConfig;
                }
            }
            
            return config;
        }

        /// <summary>
        /// Distributes currency from the enemy to the player
        /// </summary>
        public void DistributeCurrency(EnemyType enemyId)
        {
            LootDatabase.LootConfiguration config = GetOrCreateConfiguration(enemyId);

            if (config == null)
            {
                Debug.LogError($"No loot configuration found for enemy: {enemyId}");
                return;
            }

            if (config.CurrencyDistribution == null)
            {
                Debug.LogError($"No loot currencyDistribution found for enemy: {enemyId}");
                return;
            }

            // Get currency distribution from the config
            List<CurrencyAmount> currencyAmounts = config.CurrencyDistribution.GetRandomDistribution();
            
            if (currencyAmounts.Count == 0)
            {
                return; // No currency to distribute
            }
            
            // Get existing player currencies
            var stats = RPGCharacterController.Instance.rpgCharacterStats;
            
            // Add each currency amount to the player
            foreach (CurrencyAmount currency in currencyAmounts)
            {
                stats.AddCurrency(currency);
            }
            
            // Log the received currency
            string currencyText = currencyAmounts.Count == 1 
                ? CurrencyHandler.GetCurrencyString(currencyAmounts[0])
                : string.Join(", ", currencyAmounts.Select(c => $"{c.Amount} {c.Type}"));
            
            Debug.Log($"Received currency: {currencyText}");
        }

        private void SpawnItems(LootDatabase.LootConfiguration config, Vector3 position)
        {
            if (config == null)
            {
                Debug.LogError("Cannot spawn items: Configuration is null");
                return;
            }

            if (config.LootTemplate == null)
            {
                Debug.LogError($"Cannot spawn items: No template assigned for {config.EnemyType}");
                return;
            }

            // Get possible loot based on allowed categories
            var lootMappings = GetLootMappings();
            possibleLoot.Clear();

            foreach (var category in config.AllowedLootCategories.GetFlags())
            {
                if (lootMappings.TryGetValue(category, out var items))
                {
                    possibleLoot.AddRange(items);
                }
            }

            if (possibleLoot.Count == 0)
            {
                Debug.LogWarning($"No possible loot items found for categories: {config.AllowedLootCategories}");
                return;
            }

            // Get random loot based on configuration
            List<ItemObject> loots = GetRandomLoot(config);
            if (loots == null || loots.Count == 0)
            {
                return;
            }

            // Spawn each item
            foreach (ItemObject loot in loots)
            {
                Item item = loot.type switch
                {
                    ItemType.Equipment => loot.CreateItemWithTier(GetRandomTier(config)),
                    ItemType.Consumable => loot.CreateItem(),
                    ItemType.Default => loot.CreateItem(),
                    _ => null
                };

                if (item == null) continue;

                ItemPickup itemPickup = ObjectPoolerManager.Instance.SpawnFromPool<ItemPickup>(
                    ObjectPoolIDs.INTERACTABLE_ITEM_PICKUP,
                    position,
                    Quaternion.identity);

                if (itemPickup != null)
                {
                    itemPickup.AddItem(item);
                }
                else
                {
                    Debug.LogError("Failed to spawn ItemPickup from the object pool!");
                }
            }

            ItemPickUpManager.Instance.CombineClosestItemPickUps();
        }

        private List<ItemObject> GetRandomLoot(LootDatabase.LootConfiguration config)
        {
            if (possibleLoot.Count == 0)
            {
                Debug.LogError($"No possible loot items found for configuration!");
                return null;
            }

            List<ItemObject> loot = new();
            int amount = LBMath.GetRandomClampedCount(possibleLoot,
                config.ItemDropRange.x,
                config.ItemDropRange.y);

            for (int i = 0; i < amount; i++)
            {
                loot.Add(possibleLoot.TakeRandom());
            }

            return loot;
        }

        private ItemTier GetRandomTier(LootDatabase.LootConfiguration config)
        {
            Dictionary<LootRarity, float> dropRateMappings = config.DropRates.GetDropRateMappings();
            float totalWeight = dropRateMappings.Values.Sum();

            if (totalWeight <= 0)
            {
                Debug.LogError("No valid rarities allowed! Defaulting to Common.");
                return ItemTier.Common;
            }

            float randomPoint = Random.value * totalWeight;
            float cumulativeWeight = 0f;

            foreach (KeyValuePair<LootRarity, float> entry in dropRateMappings)
            {
                cumulativeWeight += entry.Value;
                if (randomPoint < cumulativeWeight)
                {
                    return ConvertLootRarityToTier(entry.Key);
                }
            }

            return ItemTier.Common;
        }

        private Dictionary<LootCategory, IReadOnlyList<ItemObject>> GetLootMappings()
        {
            ItemDatabaseObject database = ItemDatabaseObject.Instance;

            return new Dictionary<LootCategory, IReadOnlyList<ItemObject>>{
                { LootCategory.MeleeWeapon, database.MeleeWeaponItemObjects },
                { LootCategory.Helmet, database.HelmetItemObjects },
                { LootCategory.Chest, database.ChestItemObjects },
                { LootCategory.Gloves, database.GlovesItemObjects },
                { LootCategory.Legs, database.LegsItemObjects },
                { LootCategory.Boots, database.BootsItemObjects },
                { LootCategory.Necklace, database.NecklaceItemObjects },
                { LootCategory.Earring, database.EarringItemObjects },
                { LootCategory.Ring, database.RingItemObjects },
                { LootCategory.Consumable, database.ConsumableItemObjects },
                { LootCategory.Default, database.DefaultItemObjects }};
        }

        private ItemTier ConvertLootRarityToTier(LootRarity rarity)
        {
            return rarity switch
            {
                LootRarity.Common => ItemTier.Common,
                LootRarity.Uncommon => ItemTier.Uncommon,
                LootRarity.Rare => ItemTier.Rare,
                LootRarity.Epic => ItemTier.Epic,
                LootRarity.Legendary => ItemTier.Legendary,
                LootRarity.Mythical => ItemTier.Mythical,
                _ => ItemTier.Common
            };
        }
    }
}