using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace LB.Loot.Currency
{
    /// <summary>
    /// Handles the distribution of multiple currencies with configurable probabilities and amounts
    /// </summary>
    [CreateAssetMenu(fileName = "CurrencyDistribution", menuName = "RPG/Currency/CurrencyDistribution")]
    public class CurrencyDistribution : ScriptableObject
    {
        [System.Serializable]
        public class CurrencyDrop
        {
            [SerializeField] private CurrencyType type;
            [SerializeField] private Vector2Int amountRange;
            [SerializeField] [Range(0f, 1f)] private float dropChance = 1f;

            public CurrencyType Type => type;
            public Vector2Int AmountRange => amountRange;
            public float DropChance => dropChance;

            public int GetRandomAmount()
            {
                return Random.Range(amountRange.x, amountRange.y + 1);
            }
        }

        [SerializeField] private List<CurrencyDrop> currencyDrops = new List<CurrencyDrop>();
        [SerializeField] private bool distributeAllCurrencyTypes = false;

        /// <summary>
        /// Gets the name of this currency distribution (same as the ScriptableObject name)
        /// </summary>
        public string DistributionName => name;

        /// <summary>
        /// Gets a random distribution of currencies based on configured probabilities
        /// </summary>
        /// <returns>List of currency amounts that were successfully rolled</returns>
        public List<CurrencyAmount> GetRandomDistribution()
        {
            List<CurrencyAmount> distribution = new List<CurrencyAmount>();
            Dictionary<CurrencyType, int> currencyTotals = new Dictionary<CurrencyType, int>();

            // First pass: Roll for each currency and collect amounts
            foreach (CurrencyDrop drop in currencyDrops)
            {
                if (Random.value <= drop.DropChance)
                {
                    int amount = drop.GetRandomAmount();
                    
                    // Add to running total for this currency type
                    if (!currencyTotals.ContainsKey(drop.Type))
                    {
                        currencyTotals[drop.Type] = 0;
                    }
                    currencyTotals[drop.Type] += amount;
                }
            }

            // If we want all currencies to be represented, ensure at least minimum amounts
            if (distributeAllCurrencyTypes)
            {
                foreach (CurrencyDrop drop in currencyDrops)
                {
                    if (!currencyTotals.ContainsKey(drop.Type))
                    {
                        currencyTotals[drop.Type] = drop.AmountRange.x;
                    }
                }
            }

            // Normalize currencies using the game's exchange rates
            if (currencyTotals.Count > 0)
            {
                currencyTotals = NormalizeCurrencies(currencyTotals);
            }

            // Create final distribution
            foreach (var currency in currencyTotals)
            {
                if (currency.Value > 0)
                {
                    distribution.Add(new CurrencyAmount(currency.Key, currency.Value));
                }
            }

            return distribution;
        }

        /// <summary>
        /// Normalizes currencies using the game's CurrencyHandler
        /// </summary>
        private Dictionary<CurrencyType, int> NormalizeCurrencies(Dictionary<CurrencyType, int> currencyTotals)
        {
            // If we have a single currency already, just return it
            if (currencyTotals.Count == 1)
            {
                return currencyTotals;
            }

            // Convert everything to bronze first
            int totalBronze = 0;
            foreach (var currency in currencyTotals)
            {
                totalBronze += CurrencyHandler.ToBronze(new CurrencyAmount(currency.Key, currency.Value));
            }

            // Then use the CurrencyHandler to split into denominations
            Dictionary<CurrencyType, int> normalizedTotals = 
                CurrencyHandler.SplitIntoDenominations(new CurrencyAmount(CurrencyType.Bronze, totalBronze));

            return normalizedTotals;
        }

        /// <summary>
        /// Gets the guaranteed minimum amount of a specific currency type
        /// </summary>
        public int GetMinimumAmount(CurrencyType type)
        {
            CurrencyDrop drop = currencyDrops.FirstOrDefault(d => d.Type == type);
            return drop != null ? drop.AmountRange.x : 0;
        }

        /// <summary>
        /// Gets the maximum possible amount of a specific currency type
        /// </summary>
        public int GetMaximumAmount(CurrencyType type)
        {
            CurrencyDrop drop = currencyDrops.FirstOrDefault(d => d.Type == type);
            return drop != null ? drop.AmountRange.y : 0;
        }
    }
} 