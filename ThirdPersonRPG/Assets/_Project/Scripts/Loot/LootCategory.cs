using System;

namespace LB.Loot
{
    /// <summary>
    /// Defines the possible loot categories using a flag-based enum.
    /// Multiple categories can be selected at once.
    /// </summary>
    [Flags]
    public enum LootCategory
    {
        None = 0,
        MeleeWeapon = 1 << 0,
        Helmet = 1 << 3,
        Chest = 1 << 4,
        Gloves = 1 << 5,
        Legs = 1 << 6,
        Boots = 1 << 7,
        Necklace = 1 << 8,
        Earring = 1 << 9,
        Ring = 1 << 10,
        Consumable = 1 << 11,
        Default = 1 << 12,
        
        All = ~None
    }
}