using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LB;
using LB.Utilities;

public class RPGCharacterExperienceBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider experienceSlider;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI experienceText;
    [SerializeField][ReadOnly] private RPGCharacterStats playerStats;

    public void Awake()
    {
        if (playerStats == null)
        {
            playerStats = RPGCharacterController.Instance.rpgCharacterStats;
        }
    }

    private void OnEnable()
    {
        if (playerStats != null)
        {
            playerStats.OnExperienceGained += UpdateExperienceUI;
            playerStats.OnLevelUp += UpdateLevelUI;
            UpdateExperienceUI(0);
            UpdateLevelUI(playerStats.GetCurrentLevel);
        }
    }

    private void OnDisable()
    {
        if (playerStats != null)
        {
            playerStats.OnExperienceGained -= UpdateExperienceUI;
            playerStats.OnLevelUp -= UpdateLevelUI;
        }
    }

    private void UpdateExperienceUI(float _)
    {
        if (playerStats == null) return;

        float currentExp = playerStats.GetCurrentExperience;
        float expToNextLevel = playerStats.GetExperienceToNextLevel;
        float fillAmount = currentExp / expToNextLevel;

        experienceSlider.value = fillAmount;
        experienceText.text = $"Exp: {currentExp:F0} / {expToNextLevel:F0}";
    }

    private void UpdateLevelUI(int newLevel)
    {
        if (levelText != null)
        {
            levelText.text = $"Level: {newLevel}";
        }
    }
}