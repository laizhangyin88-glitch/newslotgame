using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

namespace SlotMaker
{
    public class ContextImageSlider : ContextCompositor, IContextFloatProperty
    {
        public Image image;

        public float frequency;
        public float damping;

        public float targetValue;
        private float velocity;

        public virtual void SetFloatProperty(float value)
        {
            targetValue = value;
        }

        public float GetFloatProperty()
        {
            return targetValue;
        }

        private void Update()
        {
            float ksg, kdg;
            PIDUtils.CalcCoefficient(frequency, damping, Time.deltaTime, out ksg, out kdg);

            float force = PIDUtils.CalcForce(image.fillAmount, targetValue, velocity, 0f, ksg, kdg);
            velocity += force * Time.deltaTime;

            float displacement = velocity * Time.deltaTime;
            image.fillAmount += displacement;
        }
    }
}
