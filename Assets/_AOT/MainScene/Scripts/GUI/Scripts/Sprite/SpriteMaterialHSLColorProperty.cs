using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    [ExecuteInEditMode]
    [AddComponentMenu("SlotMaker/UI/Sprite/Property/HSLColor")]
    [RequireComponent(typeof(SpritePanel))]
    public class SpriteMaterialHSLColorProperty : MonoBehaviour
    {
        private bool isDirty = false;

        private SpritePanel _panel;
        private SpritePanel panel
        {
            get
            {
                if (_panel == null)
                    _panel = GetComponent<SpritePanel>();
                return _panel;
            }
        }

        [Range(-180.0f, 180.0f)]
        public float hue;
        private float _lastHue;

        [Range(-100.0f, 100.0f)]
        public float saturation;
        private float _lastSaturation;

        [Range(-100.0f, 100.0f)]
        public float lightness;
        private float _lastLightness;

        private Color hslDisplacement;

        private void Awake()
        {
            UpdateHSLColor();
        }
        
        private void Update()
        {
            isDirty = (hue != _lastHue) || (saturation != _lastSaturation) || (lightness != _lastLightness);

            if (isDirty)
            {
                UpdateHSLColor();
            }
        }

        private void UpdateHSLColor()
        {
            hslDisplacement = CalculateHSLDisplacement();
            panel.color = hslDisplacement;

            _lastHue = hue;
            _lastSaturation = saturation;
            _lastLightness = lightness;

            isDirty = false;
        }

        private Color CalculateHSLDisplacement()
        {
            Color displacement = new Color(.0f, .0f, .0f);

            displacement.r = 0.5f + (Mathf.Clamp(hue, -180.0f, 180.0f) / 720.0f);
            displacement.g = 0.5f + (Mathf.Clamp(saturation, -100.0f, 100.0f) / 200.0f);
            displacement.b = 0.5f + (Mathf.Clamp(lightness, -100.0f, 100.0f) / 200.0f);

            return displacement;
        }
    }
}