using UnityEngine;
using System.Collections.Generic;
using LB.Utilities;
using LB.Loot.Currency;
using LB.Loot.Experience;

namespace LB.Loot
{
    /// <summary>
    /// Centralized database for all loot configurations in the game
    /// </summary>
    [CreateAssetMenu(fileName = "LootDatabase", menuName = "RPG/Loot/LootDatabase")]
    public class LootDatabase : ScriptableObject
    {
        [System.Serializable]
        public class LootConfiguration
        {
            [SerializeField] private EnemyType enemyType;
            [SerializeField] private LootSourceTemplate lootTemplate;

            public EnemyType EnemyType => enemyType;
            public LootCategory AllowedLootCategories => lootTemplate?.AllowedLootCategories ?? LootCategory.None;
            public Vector2Int ItemDropRange => lootTemplate?.ItemDropRange ?? new Vector2Int(1, 1);
            public CurrencyDistribution CurrencyDistribution => lootTemplate?.CurrencyDistribution;
            public DropRates DropRates => lootTemplate?.GetDropRates() ?? new DropRates();
            public LootSourceTemplate LootTemplate => lootTemplate;
            public bool HasValidTemplate => lootTemplate != null;
            public ExperienceSource ExperienceSource => lootTemplate?.ExperienceSource;
        }

        [System.Serializable]
        public class DefaultTemplateRule
        {
            [SerializeField] private EnemyType enemyType;
            [SerializeField] private int minLevel;
            [SerializeField] private int maxLevel;
            [SerializeField] private LootSourceTemplate template;
            [SerializeField] private float weight = 1f;

            public EnemyType EnemyType => enemyType;
            public int MinLevel => minLevel;
            public int MaxLevel => maxLevel;
            public LootSourceTemplate Template => template;
            public float Weight => weight;

            public bool Matches(EnemyType type, int level)
            {
                return enemyType == type && level >= minLevel && level <= maxLevel;
            }
        }

        [System.Serializable]
        public class DropRates
        {
            [SerializeField] private float commonDropRate = 50f;
            [SerializeField] private float uncommonDropRate = 30f;
            [SerializeField] private float rareDropRate = 10f;
            [SerializeField] private float epicDropRate = 5f;
            [SerializeField] private float legendaryDropRate = 3f;
            [SerializeField] private float mythicalDropRate = 2f;

            public float CommonDropRate => commonDropRate;
            public float UncommonDropRate => uncommonDropRate;
            public float RareDropRate => rareDropRate;
            public float EpicDropRate => epicDropRate;
            public float LegendaryDropRate => legendaryDropRate;
            public float MythicalDropRate => mythicalDropRate;

            /// <summary>
            /// Adjust all drop rates at once
            /// </summary>
            public void AdjustRates(float common, float uncommon, float rare, float epic, float legendary, float mythical)
            {
                commonDropRate = common;
                uncommonDropRate = uncommon;
                rareDropRate = rare;
                epicDropRate = epic;
                legendaryDropRate = legendary;
                mythicalDropRate = mythical;
            }

            /// <summary>
            /// Creates a deep copy of the drop rates
            /// </summary>
            public DropRates Clone()
            {
                return new DropRates
                {
                    commonDropRate = this.commonDropRate,
                    uncommonDropRate = this.uncommonDropRate,
                    rareDropRate = this.rareDropRate,
                    epicDropRate = this.epicDropRate,
                    legendaryDropRate = this.legendaryDropRate,
                    mythicalDropRate = this.mythicalDropRate
                };
            }

            public Dictionary<LootRarity, float> GetDropRateMappings()
            {
                return new Dictionary<LootRarity, float>
                {
                    { LootRarity.Common, commonDropRate },
                    { LootRarity.Uncommon, uncommonDropRate },
                    { LootRarity.Rare, rareDropRate },
                    { LootRarity.Epic, epicDropRate },
                    { LootRarity.Legendary, legendaryDropRate },
                    { LootRarity.Mythical, mythicalDropRate }
                };
            }
        }

        [SerializeField] private List<LootConfiguration> lootConfigurations = new();
        [SerializeField] private List<DefaultTemplateRule> defaultTemplateRules = new();

        private static LootDatabase _instance;
        public static LootDatabase Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<LootDatabase>("LootDatabase");
                    if (_instance == null)
                    {
                        Debug.LogError("Failed to load LootDatabase from Resources!");
                    }
                }
                return _instance;
            }
        }

        public LootConfiguration GetLootConfiguration(EnemyType enemyId)
        {
            return lootConfigurations.Find(config => config.EnemyType == enemyId);
        }

        /// <summary>
        /// Gets a default template for the specified enemy type and level
        /// </summary>
        public LootSourceTemplate GetDefaultTemplateForEnemyType(EnemyType enemyType, int enemyLevel = 1)
        {
            // First, find all rules that match the enemy type and level
            var matchingRules = defaultTemplateRules.FindAll(rule => rule.Matches(enemyType, enemyLevel));
            
            if (matchingRules.Count == 0)
            {
                Debug.LogWarning($"No default template rules found for {enemyType} (Level {enemyLevel})");
                return null;
            }

            // If only one rule matches, use it
            if (matchingRules.Count == 1)
            {
                return matchingRules[0].Template;
            }

            // If multiple rules match, use weighted random selection
            float totalWeight = 0f;
            foreach (var rule in matchingRules)
            {
                totalWeight += rule.Weight;
            }

            float randomPoint = Random.value * totalWeight;
            float cumulativeWeight = 0f;

            foreach (var rule in matchingRules)
            {
                cumulativeWeight += rule.Weight;
                if (randomPoint < cumulativeWeight)
                {
                    return rule.Template;
                }
            }

            // Fallback to the last matching rule
            return matchingRules[matchingRules.Count - 1].Template;
        }

        /// <summary>
        /// Updates all non-custom drop rates to their default values
        /// </summary>
        [ContextMenu("Update Default Drop Rates")]
        public void UpdateDefaultDropRates()
        {
            int updatedCount = 0;
            foreach (LootConfiguration config in lootConfigurations)
            {
                if (config.HasValidTemplate)
                {
                    // Do nothing, will use template rates
                }
                else
                {
                    Debug.LogWarning($"Configuration for {config.EnemyType} has no valid template!");
                }
                updatedCount++;
            }
            Debug.Log($"Checked {updatedCount} configurations");
        }
    }
}