using UnityEngine;
using System.Collections.Generic;

namespace LB.Loot.Currency
{
    public static class CurrencyHandler
    {
        public static event System.Action<CurrencyAmount, CurrencyAmount, CurrencyAmount> OnCurrencyUpdatedEvent;

        private static readonly Dictionary<CurrencyType, int> CurrencyToBronze = new()
        {
            { CurrencyType.Bronze, 1 },
            { CurrencyType.Silver, 1000 },
            { CurrencyType.Gold, 100000 }
        };

        /// <summary>
        /// Converts a currency amount to its equivalent in bronze coins
        /// </summary>
        public static int ToBronze(CurrencyAmount amount)
        {
            return amount.Amount * CurrencyToBronze[amount.Type];
        }

        /// <summary>
        /// Converts a bronze amount to the specified currency type
        /// </summary>
        public static CurrencyAmount FromBronze(int bronzeAmount, CurrencyType targetType)
        {
            int convertedAmount = bronzeAmount / CurrencyToBronze[targetType];
            return new CurrencyAmount(targetType, convertedAmount);
        }

        /// <summary>
        /// Converts a currency amount to another currency type
        /// </summary>
        public static CurrencyAmount Convert(CurrencyAmount amount, CurrencyType targetType)
        {
            int bronzeAmount = ToBronze(amount);
            return FromBronze(bronzeAmount, targetType);
        }

        /// <summary>
        /// Normalizes a currency amount to the highest possible denomination
        /// </summary>
        public static CurrencyAmount Normalize(CurrencyAmount amount)
        {
            int bronzeAmount = ToBronze(amount);

            // Try to convert to the highest denomination first
            if (bronzeAmount >= CurrencyToBronze[CurrencyType.Gold])
            {
                return FromBronze(bronzeAmount, CurrencyType.Gold);
            }
            else if (bronzeAmount >= CurrencyToBronze[CurrencyType.Silver])
            {
                return FromBronze(bronzeAmount, CurrencyType.Silver);
            }

            return amount; // Return as bronze if it's the highest possible denomination
        }

        /// <summary>
        /// Splits a currency amount into its constituent denominations
        /// </summary>
        public static Dictionary<CurrencyType, int> SplitIntoDenominations(CurrencyAmount amount)
        {
            int bronzeAmount = ToBronze(amount);
            Dictionary<CurrencyType, int> denominations = new Dictionary<CurrencyType, int>();

            foreach (CurrencyType currency in new[] { CurrencyType.Gold, CurrencyType.Silver, CurrencyType.Bronze })
            {
                int value = CurrencyToBronze[currency];
                int count = bronzeAmount / value;
                if (count > 0)
                {
                    denominations[currency] = count;
                    bronzeAmount %= value;
                }
            }

            return denominations;
        }

        public static void NormalizeCurrency(ref CurrencyAmount bronzeAmount, ref CurrencyAmount silverAmount, ref CurrencyAmount goldAmount)
        {
            int totalBronze = GetTotalBronze(bronzeAmount, silverAmount, goldAmount);
            Dictionary<CurrencyType, int> denominations = SplitIntoDenominations(new CurrencyAmount(CurrencyType.Bronze, totalBronze));

            bronzeAmount = new CurrencyAmount(CurrencyType.Bronze, denominations.GetValueOrDefault(CurrencyType.Bronze, 0));
            silverAmount = new CurrencyAmount(CurrencyType.Silver, denominations.GetValueOrDefault(CurrencyType.Silver, 0));
            goldAmount = new CurrencyAmount(CurrencyType.Gold, denominations.GetValueOrDefault(CurrencyType.Gold, 0));
            OnCurrencyUpdatedEvent?.Invoke(bronzeAmount, silverAmount, goldAmount);
            Debug.Log($"Player Currency updated: {GetCurrencyString(bronzeAmount, silverAmount, goldAmount)}");
        }

        public static int GetTotalBronze(CurrencyAmount bronzeAmount, CurrencyAmount silverAmount, CurrencyAmount goldAmount)
        {
            return ToBronze(bronzeAmount) + ToBronze(silverAmount) + ToBronze(goldAmount);
        }

        public static string GetCurrencyString(CurrencyAmount currencyAmount1, CurrencyAmount currencyAmount2, CurrencyAmount currencyAmount3)
        {
            return string.Concat(currencyAmount1.Amount.ToString(), " | ", currencyAmount2.Amount.ToString(), " | ", currencyAmount3.Amount.ToString(), " | ");
        }

        public static string GetCurrencyString(CurrencyAmount currencyAmount1, CurrencyAmount currencyAmount2)
        {
            return string.Concat(currencyAmount1.Amount.ToString(), " | ", currencyAmount2.Amount.ToString());
        }

        public static string GetCurrencyString(CurrencyAmount currencyAmount)
        {
            return currencyAmount.Amount.ToString();
        }

        public static bool HasEnoughCurrency(CurrencyAmount bronzeAmount, CurrencyAmount silverAmount, CurrencyAmount goldAmount, CurrencyAmount amount)
        {
            return GetTotalBronze(bronzeAmount, silverAmount, goldAmount) >= CurrencyHandler.ToBronze(amount);
        }
    }
}