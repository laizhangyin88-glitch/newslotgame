using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

namespace SlotMaker
{
    public class ContextSlider : ContextCompositor, IContextFloatProperty
    {
        public Slider slider;

        public bool isUseRestrictRange = false;
        public float minRangeValue = 0f;
        public float maxRangeValue = 1f;

        private void Start()
        {
            if(slider != null)
            {
                slider.onValueChanged.RemoveAllListeners();
                slider.onValueChanged.AddListener(ChangedSliderValue);
            }
        }

        public virtual void SetFloatProperty(float value)
        {
            slider.value = value;
        }

        public float GetFloatProperty()
        {
            return slider.value;
        }

        private void ChangedSliderValue(float value)
        {
            if(isUseRestrictRange)
            {
                if(minRangeValue > value) slider.value = minRangeValue;
                else if(maxRangeValue < value) slider.value = maxRangeValue;
            }
        }

        public void SetSliderRangeParameter(bool useRange, float minValue, float maxValue)
        {
            isUseRestrictRange = useRange;
            minRangeValue = minValue;
            maxRangeValue = maxValue;
        }
    }
}
