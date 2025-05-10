using UnityEngine;

namespace LB.Loot.Currency
{
    /// <summary>
    /// Represents a specific amount of a currency type
    /// </summary>
    [System.Serializable]
    public struct CurrencyAmount
    {
        [SerializeField] private CurrencyType type;
        [SerializeField] private int amount;

        public CurrencyType Type => type;
        public int Amount => amount;

        public CurrencyAmount(CurrencyType type, int amount)
        {
            this.type = type;
            this.amount = amount;
        }

        public override string ToString()
        {
            return $"{amount} {type}";
        }
    }
} 