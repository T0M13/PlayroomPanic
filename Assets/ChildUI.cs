using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChildUI : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private AIAgent aI;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Slider interactionSlider;
    [SerializeField] private bool sliderActive = false;
    [Header("Settings")]
    [SerializeField] private bool iconSet = false;
    [SerializeField] private Image needIcon;
    [SerializeField] private Sprite defaultIcon;

    private void OnValidate()
    {
        GetReferences();
    }

    private void Awake()
    {
        GetReferences();
    }

    private void Start()
    {
        interactionSlider.gameObject.SetActive(sliderActive);
    }

    private void GetReferences()
    {
        if (aI == null)
        {
            aI = GetComponentInParent<AIAgent>();
        }

        if (canvas == null)
        {
            canvas = GetComponentInChildren<Canvas>();
        }

        if (interactionSlider == null)
        {
            interactionSlider = GetComponentInChildren<Slider>();
        }
    }

    public void SetSliderMaxValue(float maxValue)
    {
        if (sliderActive) return;

        interactionSlider.maxValue = maxValue;
        interactionSlider.value = 0;
        interactionSlider.gameObject.SetActive(true);
        sliderActive = true;
    }

    public void UpdateSliderValue(float currentValue)
    {
        interactionSlider.value = currentValue;
    }

    public void DeactivateSlider()
    {
        if (!sliderActive) return;

        interactionSlider.gameObject.SetActive(false);
        needIcon.sprite = defaultIcon;
        sliderActive = false;
    }

    public void SetNeedIcon(NeedIcon icon)
    {
        if (icon == null) return;
        if (iconSet) return;

        iconSet = true;
        needIcon.sprite = icon.icon;
        interactionSlider.gameObject.SetActive(true);
    }

    public void SetDefaultIconAndOff()
    {
        if (defaultIcon == null) return;
        if (!iconSet) return;

        interactionSlider.gameObject.SetActive(false);
        iconSet = false;
        needIcon.sprite = defaultIcon;
    }
}
