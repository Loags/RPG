using UnityEngine;
using System.Collections.Generic;
using LB.Loot.Currency;

namespace LB
{
    public class RPGCharacterStats : CharacterStats
    {
        [Header("Currency")]
        [SerializeField] private CurrencyAmount bronzeAmount = new(CurrencyType.Bronze, 0);
        [SerializeField] private CurrencyAmount silverAmount = new(CurrencyType.Silver, 0);
        [SerializeField] private CurrencyAmount goldAmount = new(CurrencyType.Gold, 0);

        public CurrencyAmount BronzeAmount => bronzeAmount;
        public CurrencyAmount SilverAmount => silverAmount;
        public CurrencyAmount GoldAmount => goldAmount;
        
        public void SetBronzeAmount(CurrencyAmount amount) => bronzeAmount = amount;
        public void SetSilverAmount(CurrencyAmount amount) => silverAmount = amount;
        public void SetGoldAmount(CurrencyAmount amount) => goldAmount = amount;

        private RPGCharacterEquipmentController rpgCharacterEquipmentController;

        public override CharacterFaction Faction => CharacterFaction.PlayerFaction;

        private void Awake()
        {
            rpgCharacterEquipmentController = GetComponent<RPGCharacterEquipmentController>();
        }

        public override void SetUpAttributes()
        {
            for (int i = 0; i < attributes.Length; i++)
            {
                attributes[i].SetParent(characterStats);
            }

            for (int i = 0; i < rpgCharacterEquipmentController.equipment.GetSlots.Length; i++)
            {
                rpgCharacterEquipmentController.equipment.GetSlots[i].OnBeforeUpdate +=
                    rpgCharacterEquipmentController.OnRemoveEquipmentItem;
                rpgCharacterEquipmentController.equipment.GetSlots[i].OnAfterUpdate +=
                    rpgCharacterEquipmentController.OnAddEquipmentItem;
            }
        }

        public void AddCurrency(CurrencyAmount amount)
        {
            Dictionary<CurrencyType, int> denominations = CurrencyHandler.SplitIntoDenominations(amount);

            foreach (KeyValuePair<CurrencyType, int> denomination in denominations)
            {
                switch (denomination.Key)
                {
                    case CurrencyType.Bronze:
                        SetBronzeAmount(new CurrencyAmount(CurrencyType.Bronze, bronzeAmount.Amount + denomination.Value));
                        break;
                    case CurrencyType.Silver:
                        SetSilverAmount(new CurrencyAmount(CurrencyType.Silver, silverAmount.Amount + denomination.Value));
                        break;
                    case CurrencyType.Gold:
                        SetGoldAmount(new CurrencyAmount(CurrencyType.Gold, goldAmount.Amount + denomination.Value));
                        break;
                }
            }

            CurrencyHandler.NormalizeCurrency(ref bronzeAmount, ref silverAmount, ref goldAmount);
        }

        public void RemoveCurrency(CurrencyAmount amount)
        {
            int totalBronze = CurrencyHandler.GetTotalBronze(bronzeAmount, silverAmount, goldAmount);
            int amountBronze = CurrencyHandler.ToBronze(amount);

            if (totalBronze < amountBronze)
            {
                Debug.LogError("Not enough currency!");
                return;
            }

            SetBronzeAmount(new CurrencyAmount(CurrencyType.Bronze, totalBronze - amountBronze));
            SetSilverAmount(new CurrencyAmount(CurrencyType.Silver, 0));
            SetGoldAmount(new CurrencyAmount(CurrencyType.Gold, 0));
            CurrencyHandler.NormalizeCurrency(ref bronzeAmount, ref silverAmount, ref goldAmount);
        }
    }
}