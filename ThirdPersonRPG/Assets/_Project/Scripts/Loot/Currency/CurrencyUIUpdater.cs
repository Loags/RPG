using TMPro;
using UnityEngine;

namespace LB.Loot.Currency
{
    public class CurrencyUIUpdater : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI bronzeAmountDisplay;
        [SerializeField] private TextMeshProUGUI silverAmountDisplay;
        [SerializeField] private TextMeshProUGUI goldAmountDisplay;

        private void OnEnable()
        {
            CurrencyHandler.OnCurrencyUpdatedEvent += CurrencyConverter_OnCurrencyUpdatedEvent;
            RPGCharacterStats rpgCharacterStats = RPGCharacterController.Instance.rpgCharacterStats;
            CurrencyConverter_OnCurrencyUpdatedEvent(rpgCharacterStats.BronzeAmount, rpgCharacterStats.SilverAmount, rpgCharacterStats.GoldAmount);
        }

        private void OnDisable()
        {
            CurrencyHandler.OnCurrencyUpdatedEvent -= CurrencyConverter_OnCurrencyUpdatedEvent;
        }

        private void CurrencyConverter_OnCurrencyUpdatedEvent(CurrencyAmount bronzeAmount, CurrencyAmount silverAmount, CurrencyAmount goldAmount)
        {
            bronzeAmountDisplay.text = CurrencyHandler.GetCurrencyString(bronzeAmount);
            silverAmountDisplay.text = CurrencyHandler.GetCurrencyString(silverAmount);
            goldAmountDisplay.text = CurrencyHandler.GetCurrencyString(goldAmount);
        }
    }
}