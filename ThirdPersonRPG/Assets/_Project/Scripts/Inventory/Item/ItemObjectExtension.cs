using UnityEngine;

namespace LB.Inventory
{
    public enum ItemType
    {
        Equipment,
        Consumable,
        Default,
        NONE
    }

    public enum ItemTier
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary,
        Mythical,
        NONE
    }

    public enum Equipment
    {
        Weapon,
        Armor,
        Accessory,
        NONE
    }

    public enum Weapon
    {
        Unarmed,
        MeleeWeapon,
        NONE
    }

    public enum Armor
    {
        Helmet,
        Chest,
        Gloves,
        Legs,
        Boots,
        NONE
    }

    public enum Accessory
    {
        Ring,
        Earring,
        Necklace,
        NONE
    }

    public enum Consumable
    {
        Food,
        NONE
    }

    public enum Attributes
    {
        Agility,
        Intellect,
        Stamina,
        Strength,
        Health,
        Armor,
        RegenerationHealth,
        RegenerationStamina,
        Dexterity,
        Mana,
        CriticalHitChance,
        CriticalHitDamage,
        Luck,
        Evasion,
        Experience,
        Level,
        ExperienceMultiplier
    }

    public static class ItemObjectExtension
    {
        public static T As<T>(this ItemObject itemObject) where T : ItemObject
        {
            if (itemObject is T specificItemObject)
            {
                return specificItemObject;
            }
            else
            {
                Debug.LogError($"ItemObject cannot be cast to type {typeof(T)}.");
                return null;
            }
        }

        public static class Colors
        {
            public static readonly Color DefaultBackground = new Color(0.6f, 0.6f, 0.6f, 0.6f);
            public static readonly Color Common = new Color(1f, 1f, 1f, 0.6f); // White
            public static readonly Color Uncommon = new Color(0f, 1f, 0f, 0.6f); // Green
            public static readonly Color Rare = new Color(0f, 0f, 1f, 0.6f); // Blue
            public static readonly Color Epic = new Color(0.5f, 0f, 0.5f, 0.6f); // Purple
            public static readonly Color Legendary = new Color(1f, 0.65f, 0f, 0.6f); // Orange
            public static readonly Color Mythical = new Color(1f, 0f, 0f, 0.6f); // Red

            public static Color GetColorForTier(ItemTier tier)
            {
                return tier switch
                {
                    ItemTier.Common => Common,
                    ItemTier.Uncommon => Uncommon,
                    ItemTier.Rare => Rare,
                    ItemTier.Epic => Epic,
                    ItemTier.Legendary => Legendary,
                    ItemTier.Mythical => Mythical,
                    _ => DefaultBackground,
                };
            }
        }
    }
}