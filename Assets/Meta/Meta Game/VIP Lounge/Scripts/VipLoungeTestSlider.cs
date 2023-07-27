using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode.VipLounge
{
    public class VipLoungeTestSlider : MonoBehaviour
    {
        public Slider slider = null;
        public Transform moveTarget = null;

        public float extraAngle = -15.0f;

        public float height = 56.0f;
        public float halfHeight = 28.0f;

        public float lastValue = 0.0f;

        public void Start()
        {
        }

        public void Update()
        {
            if (lastValue != slider.value)
            {
                lastValue = slider.value;
                SetValueChange();
            }
        }

        public void SetValueChange()
        {
            float rad = lastValue * 180.0f + extraAngle;
            float calcHeight = (Mathf.Sin(rad * Mathf.Deg2Rad) * height) - halfHeight;
            Vector3 pos = moveTarget.localPosition;
            pos.y = calcHeight;
            moveTarget.localPosition = pos;
        }
    }
}