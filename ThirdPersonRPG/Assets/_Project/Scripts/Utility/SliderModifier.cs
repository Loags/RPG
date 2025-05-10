using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderModifier : MonoBehaviour
{
    [SerializeField] protected TMP_Text sliderDisplayText;

    protected Slider slider;

    public virtual void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public virtual void Start()
    {
        UpdateSliderText(slider.value);
    }

    public virtual void UpdateSliderText(float _value) // Called on SliderChanged 
    {
        float roundedVal = Mathf.Round(_value * 100f);
        sliderDisplayText.text = roundedVal + "%";
    }
}
