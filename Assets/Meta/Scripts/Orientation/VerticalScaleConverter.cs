using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SlotMaker;

namespace BagelCode
{
    public class VerticalScaleConverter : MonoBehaviour
    {
        public RectTransform pivotRect;
        public Vector2 vertical = new Vector2(640, 1136);

        private RectTransform rect;

        private Vector2 prevSizeDelta;

        private bool isInit = false;


        public void ChangeOrientation(ScreenOrientation targetOrientation)
        {
            if(OrientationUtils.Instance.PossibleChangeOrientation()) return;

            UpdateScaleFactor();
        }

        private void Start()
        {
            if(OrientationUtils.Instance.PossibleChangeOrientation()) return;

            Init();

            UpdateScaleFactor();
        }

        private void Init()
        {
            if(OrientationUtils.Instance.PossibleChangeOrientation()) return;

            rect = GetComponent<RectTransform>();
            if(pivotRect == null)
                pivotRect = transform.root.GetComponent<RectTransform>();

            prevSizeDelta = Vector2.zero;
            isInit = true;
        }

        private void LateUpdate()
        {
            if(OrientationUtils.Instance.PossibleChangeOrientation()) return;

            if(pivotRect == null) return;

            if(prevSizeDelta != pivotRect.sizeDelta)
            {
                UpdateScaleFactor();
            }
        }

        private void UpdateScaleFactor()
        {
            if(OrientationUtils.Instance.PossibleChangeOrientation()) return;

            if(!isInit) return;
            float scaleFactor = 1f;

            var currentOrientation = BlackboardUtils.FindVariable<ClientModels.Orientation>(MainBlackboard.Get(), "currentOrientation");

            if(currentOrientation == null || currentOrientation.value == ClientModels.Orientation.LANDSCAPE)
            {
                transform.localScale = Vector3.one;
            }
            else
            {
                switch(Screen.orientation)
                {
                    default:
                        {
                            if(Screen.autorotateToPortrait || Screen.autorotateToPortraitUpsideDown)
                            {
                                scaleFactor = Mathf.Min(pivotRect.sizeDelta.x / vertical.x, pivotRect.sizeDelta.y / vertical.y);

                                transform.localScale = Vector3.one * scaleFactor;
                            }
                            else
                            {
                                transform.localScale = Vector3.one;
                            }
                        }
                        break;
                }
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
