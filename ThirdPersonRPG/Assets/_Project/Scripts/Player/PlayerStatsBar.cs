using LB.Inventory;
using UnityEngine;

namespace LB
{
    public class PlayerStatsBar : SliderModifier
    {
        [SerializeField] private bool isHealthBar;

        private RPGCharacterStats rpgCharacterStats;

        public override void Awake()
        {
            base.Awake();
            rpgCharacterStats = RPGCharacterController.Instance.rpgCharacterStats;
        }

        public override void Start()
        {
            UpdateMax();
            UpdateCurrent();
            rpgCharacterStats.onModifierChanged += UpdateMax;
            rpgCharacterStats.onModifierChanged += UpdateCurrent;
            base.Start();
        }

        public void UpdateMax()
        {
            slider.maxValue = isHealthBar
                ? rpgCharacterStats.GetMaxValueOfAttributeType(Attributes.Health)
                : rpgCharacterStats.GetMaxValueOfAttributeType(Attributes.Stamina);
        }

        public void UpdateCurrent()
        {
            slider.value = isHealthBar ? rpgCharacterStats.CurrentHealth : rpgCharacterStats.CurrentStamina;
        }

        public override void UpdateSliderText(float _value)
        {
            if (isHealthBar)
                sliderDisplayText.text = rpgCharacterStats.CurrentHealth + " / " +
                                         rpgCharacterStats.GetMaxValueOfAttributeType(Attributes.Health);
            else
                sliderDisplayText.text = rpgCharacterStats.CurrentStamina + " / " +
                                         rpgCharacterStats.GetMaxValueOfAttributeType(Attributes.Stamina);
        }

        private void OnDestroy()
        {
            rpgCharacterStats.onModifierChanged -= UpdateMax;
            rpgCharacterStats.onModifierChanged -= UpdateCurrent;
        }
    }
}