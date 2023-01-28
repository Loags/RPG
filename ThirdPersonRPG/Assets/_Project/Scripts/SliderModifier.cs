using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderModifier : MonoBehaviour
{
    [SerializeField] private TMP_Text sliderDisplayText;
    private Slider slider;
    private void Awake()
    {
        slider = GetComponent<Slider>();
    }
    private void Start()
    {
        UpdateSliderText(slider.value);
    }

    public void UpdateSliderText(float _value) // Called on SliderChanged 
    {
        float roundedVal = Mathf.Round(_value * 100f);
        sliderDisplayText.text = roundedVal + "%";
    }
}
