using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private PlayerReferences playerReferences;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Slider interactionSlider;
    [SerializeField] private bool sliderActive = false;

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
        if (playerReferences == null)
        {
            playerReferences = GetComponentInParent<PlayerReferences>();
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
        sliderActive = false;
    }

}
