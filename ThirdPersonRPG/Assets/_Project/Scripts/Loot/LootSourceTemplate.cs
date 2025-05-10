using UnityEngine;
using LB.Utilities;
using static LB.Loot.LootDatabase;
using LB.Loot.Currency;
using LB.Loot.Experience;

namespace LB.Loot
{
    /// <summary>
    /// Template for defining loot properties that can be shared across multiple loot sources
    /// </summary>
    [CreateAssetMenu(fileName = "LootSourceTemplate", menuName = "RPG/Loot/LootSourceTemplate")]
    public class LootSourceTemplate : ScriptableObject
    {
        [SerializeField] private LootCategory allowedLootCategories;
        [SerializeField] private Vector2Int itemDropRange = new Vector2Int(1, 3);
        [SerializeField] private LootTier lootTier = LootTier.Normal;
        [SerializeField] private CurrencyDistribution currencyDistribution;
        [SerializeField] private ExperienceSource experienceSource;
        [SerializeField] private bool useCustomDropRates = false;
        [SerializeField] private DropRates customDropRates;

        /// <summary>
        /// Predefined loot tiers that can be used to quickly configure templates
        /// </summary>
        public enum LootTier
        {
            Poor,
            Normal,
            Good,
            Elite,
            Boss
        }

        public string TemplateName => name;
        public LootCategory AllowedLootCategories => allowedLootCategories;
        public Vector2Int ItemDropRange => itemDropRange;
        public CurrencyDistribution CurrencyDistribution => currencyDistribution;
        public LootTier Tier => lootTier;
        public bool UseCustomDropRates => useCustomDropRates;
        public ExperienceSource ExperienceSource => experienceSource;
        
        /// <summary>
        /// Gets the drop rates associated with this template
        /// </summary>
        public DropRates GetDropRates()
        {
            if (useCustomDropRates && customDropRates != null)
            {
                return customDropRates;
            }
            else
            {
                return GenerateDropRatesForTier(lootTier);
            }
        }

        /// <summary>
        /// Generates appropriate drop rates based on the loot tier
        /// </summary>
        private DropRates GenerateDropRatesForTier(LootTier tier)
        {
            DropRates rates = new DropRates();
            
            switch (tier)
            {
                case LootTier.Poor:
                    rates.AdjustRates(70f, 25f, 5f, 0f, 0f, 0f);
                    break;
                case LootTier.Normal:
                    rates.AdjustRates(55f, 30f, 10f, 4f, 1f, 0f);
                    break;
                case LootTier.Good:
                    rates.AdjustRates(40f, 35f, 15f, 7f, 2.5f, 0.5f);
                    break;
                case LootTier.Elite:
                    rates.AdjustRates(20f, 35f, 25f, 12f, 6f, 2f);
                    break;
                case LootTier.Boss:
                    rates.AdjustRates(10f, 25f, 30f, 20f, 10f, 5f);
                    break;
                default:
                    rates.AdjustRates(50f, 30f, 10f, 5f, 3f, 2f);
                    break;
            }
            
            return rates;
        }
    }
} 