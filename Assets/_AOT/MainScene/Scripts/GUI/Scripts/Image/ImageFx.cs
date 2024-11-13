using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace SlotMaker
{
    public class ImageFx : MonoBehaviour
    {
        [BoxGroup("Color")]
        public bool useColor;
        [BoxGroup("Color")]
        [EnableIf("useColor")]
        public Color color = Color.white;
        [BoxGroup("Color")]
        [EnableIf("useColor")]
        [SerializeField] 
        private float colorFrequency = 1f;
        [BoxGroup("Color")]
        [EnableIf("useColor")]
        [SerializeField] 
        private float colorDamping = 1f;
        private Color colorVelocity = Color.clear;

        [BoxGroup("Fill Amount")]
        public bool useFillAmount;
        [BoxGroup("Fill Amount")]
        [EnableIf("useFillAmount")]
        [Range(0f, 1f)] 
        public float fillAmount = 0f;
        [BoxGroup("Fill Amount")]
        [EnableIf("useFillAmount")]
        [SerializeField] 
        private float fillAmountFrequency = 1f;
        [BoxGroup("Fill Amount")]
        [EnableIf("useFillAmount")]
        [SerializeField] 
        private float fillAmountDamping = 1f;
        private float fillAmountVelocity = 0f;

        private Image _image;
        private Image image { get { return _image ?? (_image = GetComponent<Image>()); } }

        private void Update()
        {
            if (useColor)
            {
                image.color += PIDUtils.CalcDisplacement(
                    image.color, color, ref colorVelocity, Color.clear, colorFrequency, colorDamping, Time.deltaTime);
            }

            if (useFillAmount)
            {
                image.fillAmount += PIDUtils.CalcDisplacement(
                    image.fillAmount, fillAmount, ref fillAmountVelocity, 0f, fillAmountFrequency, fillAmountDamping, Time.deltaTime);
            }
        }
    }
}
