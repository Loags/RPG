using LB.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace LB.Utilities.DeveloperConsole.Commands
{
    [CreateAssetMenu(menuName = "Utilities/Developer/Commands/SpawnItemCommand")]
    public class SpawnItemCommand : ConsoleCommand
    {
        public void GeneratePredefinedCommands()
        {
            possibleCommands.Clear();
            ItemDatabaseObject db = ItemDatabaseObject.Instance;

            // Dictionary mapping ItemType to corresponding processing function
            Dictionary<ItemType, Action<string>> itemProcessors = new Dictionary<ItemType, Action<string>>
            {
                { ItemType.Equipment, baseCommand => ProcessEquipment(db, baseCommand) },
                { ItemType.Consumable, baseCommand => ProcessConsumables(db, baseCommand) }
            };

            // Process all item types using the dictionary
            foreach (ItemType itemType in Enum.GetValues(typeof(ItemType)))
            {
                if (itemType == ItemType.NONE || !itemProcessors.ContainsKey(itemType)) continue;
                string baseCommand = itemType.ToString();
                itemProcessors[itemType].Invoke(baseCommand);
            }
        }

        /// <summary>
        /// Processes all equipment types (Weapon, Armor, Accessory).
        /// </summary>
        private void ProcessEquipment(ItemDatabaseObject db, string baseCommand)
        {
            Dictionary<Equipment, Action<string>> equipmentProcessors = new Dictionary<Equipment, Action<string>>
            {
                { Equipment.Weapon, equipmentCommand => ProcessWeapons(db, equipmentCommand) },
                { Equipment.Armor, equipmentCommand => ProcessArmors(db, equipmentCommand) },
                { Equipment.Accessory, equipmentCommand => ProcessAccessories(db, equipmentCommand) }
            };

            foreach (Equipment equipmentType in Enum.GetValues(typeof(Equipment)))
            {
                if (equipmentType == Equipment.NONE || !equipmentProcessors.ContainsKey(equipmentType)) continue;
                string equipmentCommand = $"{baseCommand}_{equipmentType}";
                equipmentProcessors[equipmentType].Invoke(equipmentCommand);
            }
        }

        /// <summary>
        /// Processes weapons with their IDs and tiers.
        /// </summary>
        private void ProcessWeapons(ItemDatabaseObject db, string equipmentCommand)
        {
            foreach (WeaponObject weaponItem in db.WeaponItemObjects.Where(w =>
                         w.weaponType != Weapon.NONE && w.weaponType != Weapon.Unarmed))
            {
                string weaponCommand = $"{equipmentCommand}_{weaponItem.weaponType}_{weaponItem.data.Id}";
                AddCommandsWithTiers(weaponCommand);
            }
        }

        /// <summary>
        /// Processes armors with their IDs and tiers.
        /// </summary>
        private void ProcessArmors(ItemDatabaseObject db, string equipmentCommand)
        {
            foreach (ArmorObject armorItem in db.ArmorItemObjects.Where(a => a.armorType != Armor.NONE))
            {
                string armorCommand = $"{equipmentCommand}_{armorItem.armorType}_{armorItem.data.Id}";
                AddCommandsWithTiers(armorCommand);
            }
        }

        /// <summary>
        /// Processes accessories with their IDs and tiers.
        /// </summary>
        private void ProcessAccessories(ItemDatabaseObject db, string equipmentCommand)
        {
            foreach (AccessoryObject accessoryItem in db.AccessoryItemObjects.Where(a =>
                         a.accessoryType != Accessory.NONE))
            {
                string accessoryCommand = $"{equipmentCommand}_{accessoryItem.accessoryType}_{accessoryItem.data.Id}";
                AddCommandsWithTiers(accessoryCommand);
            }
        }

        /// <summary>
        /// Processes consumables.
        /// </summary>
        private void ProcessConsumables(ItemDatabaseObject db, string baseCommand)
        {
            foreach (Consumable consumableType in Enum.GetValues(typeof(Consumable)))
            {
                if (consumableType == Consumable.NONE) continue;
                string consumableCommand = $"{baseCommand}_{consumableType}";
                possibleCommands.Add(consumableCommand);
            }
        }

        /// <summary>
        /// Adds item tiers to the given base command.
        /// </summary>
        private void AddCommandsWithTiers(string baseCommand)
        {
            foreach (ItemTier tier in Enum.GetValues(typeof(ItemTier)))
            {
                if (tier == ItemTier.NONE) continue;
                possibleCommands.Add($"{baseCommand}_{tier}");
            }
        }


        /// <summary>
        /// Processes the /spawn_item command.
        /// </summary>
        public override bool Process(string[] args)
        {
            return false;
        }
    }
}