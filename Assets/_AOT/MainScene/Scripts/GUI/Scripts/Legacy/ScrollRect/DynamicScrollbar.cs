using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlotMaker
{

public class DynamicScrollbar : MonoBehaviour
{
    public Scrollbar scrollbar;
    public DynamicScrollRect scrollRect;

    private float scrollbarValue;
    private float scrollRectValue;
    private bool mutex;

    private void OnEnable()
    {
        // scrollRect.draggable = false;
    }

    private void OnDisable()
    {
        // scrollRect.draggable = true;
    }

    public void OnValueChanged(float value)
    {
        if (!mutex)
            scrollbarValue = value;
    }

    public void OnDisplacementChanged(float value)
    {
        scrollRectValue = value;

        if (!mutex)
        {
            mutex = true;
            scrollbar.value = scrollbarValue = value;
            mutex = false;
        }
    }

    public void PrevPage()
    {
        if (scrollbar.numberOfSteps != 0)
            scrollbar.value -= 1f / scrollbar.numberOfSteps;
    }

    public void NextPage()
    {
        if (scrollbar.numberOfSteps != 0)
            scrollbar.value += 1f / scrollbar.numberOfSteps;
    }

    private void Update()
    {
        if (scrollbarValue != scrollRectValue)
        {
            mutex = true;
            scrollRect.normalizedDisplacement = scrollbarValue = scrollbar.value;
            Debug.Log("Normal: " + scrollRect.normalizedDisplacement);
        }
        else 
        {
            mutex = false;
        }
    }
}

}
