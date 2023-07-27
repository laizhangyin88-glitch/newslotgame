using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class RectTransformMirror : MonoBehaviour
    {
        public RectTransform self;

        public RectTransform targetRect;

        public bool inverseWH;

        public bool useForcePortraitMode;

        private float cavasWidth;
        private float canvasHeight;

        private void Awake()
        {
            self = GetComponent<RectTransform>();
            self.anchorMin = new Vector2(0.5f, 0.5f);
            self.anchorMax = new Vector2(0.5f, 0.5f);
        }

        private void Start()
        {

        }

        private void Update()
        {
            if(targetRect == null) return;

            if( targetRect.rect.width != cavasWidth
              ||targetRect.rect.height != canvasHeight)
            {
                cavasWidth = targetRect.rect.width;
                canvasHeight = targetRect.rect.height;

                if(inverseWH)
                {
                    self.sizeDelta = new Vector2(canvasHeight, cavasWidth);
                    self.localScale = Vector3.one;
                }
                else
                {
                    if(useForcePortraitMode)
                    {
                        // Height aspect ratio fix scale fop canvas,UWP
                        float ratio = canvasHeight / cavasWidth;
                        Debug.LogError(ratio);
                        self.sizeDelta = new Vector2(canvasHeight, cavasWidth);
                        self.localScale = new Vector3(ratio, ratio, 1f);
                    }
                    else
                    {
                        self.sizeDelta = new Vector2(cavasWidth, canvasHeight);
                        self.localScale = Vector3.one;
                    }
                }
            }
        }
    }
}
