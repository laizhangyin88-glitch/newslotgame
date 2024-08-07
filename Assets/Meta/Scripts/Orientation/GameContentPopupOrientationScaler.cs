using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SlotMaker;

namespace BagelCode
{
    public class GameContentPopupOrientationScaler : MonoBehaviour
    {
        public RectTransform pivotRect;
        public Vector2 vertical = new Vector2(640, 1136);
        public Vector2 verticalLandscape = new Vector2(640, 1024);

        private RectTransform rect;

        private Vector2 prevSizeDelta;

        private void Awake()
        {
            rect = GetComponent<RectTransform>();
            if(pivotRect == null)
            {
                pivotRect = transform.parent.gameObject.GetComponent<RectTransform>();
                // prevSizeDelta = pivotRect.sizeDelta;
            }

            prevSizeDelta = Vector2.zero;
        }

        private void LateUpdate()
        {
            if(pivotRect == null) return;

            if(prevSizeDelta != pivotRect.sizeDelta)
            {
                UpdateScaleFactor();
            }
        }

        public void ChangeOrientation(ScreenOrientation targetOrientation)
        {
            //UpdateScaleFactor();
        }

        private void UpdateScaleFactor()
        {
            float scaleFactor = 1f;

            if(OrientationUtils.Instance.contentOrientation == ScreenOrientation.LandscapeLeft)
            {
                //transform.localScale = Vector3.one;
            }
            else
            {
                var targetSize = OrientationUtils.Instance.PossibleChangeOrientation() ? vertical : verticalLandscape;

                scaleFactor = Mathf.Min(pivotRect.sizeDelta.x / targetSize.x, pivotRect.sizeDelta.y / targetSize.y);

                transform.localScale = Vector3.one * scaleFactor;
            }

            if(pivotRect != null)
            {
                if(scaleFactor != 1f)
                    rect.sizeDelta = pivotRect.sizeDelta * (1f/scaleFactor);
                else
                    rect.sizeDelta = pivotRect.sizeDelta;

                prevSizeDelta = pivotRect.sizeDelta;
            }
        }
    }
}
