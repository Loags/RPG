using UnityEngine;

namespace LB.Loot
{
    /// <summary>
    /// Defines the possible item rarities using a flag-based enum.
    /// Multiple rarities can be allowed at once.
    /// </summary>
    [System.Flags]
    public enum LootRarity
    {
        None = 0,
        Common = 1 << 0,
        Uncommon = 1 << 1,
        Rare = 1 << 2,
        Epic = 1 << 3,
        Legendary = 1 << 4,
        Mythical = 1 << 5
    }
}