using System;
using UnityEngine;

namespace LB.Inventory
{
    public enum SlotType
    {
        Helmet,
        Chest,
        Gloves,
        Legs,
        Boots,
        Necklace,
        Ring,
        Earring,
        Weapon,
        Shield,
        NONE
    }

    [CreateAssetMenu(fileName = "New Allowed Item", menuName = "Inventory System/Allowed Item")]
    [Serializable]
    public class AllowedItem : ScriptableObject
    {
        public SlotType slotType;
        public ItemType itemType;

        public Equipment equipmentType;
        public Weapon weaponType;
        public Armor armorType;
        public Accessory accessoryType;
    }
}