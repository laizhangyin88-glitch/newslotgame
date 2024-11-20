using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginMaskController : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;

    private Slider slider;

    private float currentValue;

    private float totalValue;
    private void Start()
    {
        slider = transform.Find("Anchor/Slider").GetComponentInChildren<Slider>();
        slider.value = 0;
        totalValue = 0;
        if (_canvasGroup == null)
            return;

        _canvasGroup.blocksRaycasts = !TestDisplayManager.Instance.IsGM;

    }

    public void SetSliderTotal(float total)
    {
        if (totalValue <= 0)
        {
            totalValue = total;
            currentValue = 0;
        }
    }

    public void AddSliderValue()
    {
        currentValue++;
        slider.value = currentValue / totalValue;
    }
}
