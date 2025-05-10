using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace LB.Inventory
{
    public class EquipmentStatsDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text healthText;
        [SerializeField] private TMP_Text staminaText;
        [SerializeField] private TMP_Text armorText;
        [SerializeField] private TMP_Text strengthText;
        [SerializeField] private TMP_Text agilityText;
        [SerializeField] private TMP_Text intellectText;
        
        private RPGCharacterStats rpgCharacterStats;

        public void Start()
        {
            rpgCharacterStats = GameObject.FindGameObjectWithTag("Player").GetComponent<RPGCharacterStats>();
            rpgCharacterStats.onModifierChanged += UpdateStats;
            UpdateStats();
        }

        public void UpdateStats()
        {
            healthText.text = rpgCharacterStats.GetMaxValueOfAttributeType(Attributes.Health).ToString();
            staminaText.text = rpgCharacterStats.GetMaxValueOfAttributeType(Attributes.Stamina).ToString();
            armorText.text = rpgCharacterStats.GetMaxValueOfAttributeType(Attributes.Armor).ToString();
            strengthText.text = rpgCharacterStats.GetMaxValueOfAttributeType(Attributes.Strength).ToString();
            agilityText.text = rpgCharacterStats.GetMaxValueOfAttributeType(Attributes.Agility).ToString();
            intellectText.text = rpgCharacterStats.GetMaxValueOfAttributeType(Attributes.Intellect).ToString();
        }


        private void OnDestroy()
        {
            rpgCharacterStats.onModifierChanged -= UpdateStats;
        }
    }
}