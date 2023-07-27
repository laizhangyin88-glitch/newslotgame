using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SlotMaker;

namespace BagelCode
{
    public class PopupScaleConverter : MonoBehaviour
    {
        public RectTransform pivotRect;
        public Vector2 vertical = new Vector2(1136, 640);

        private RectTransform rect;

        private Vector2 prevSizeDelta;

        private void Start()
        {
            Init();
            UpdateScaleFactor();
        }

        private void Init()
        {
            rect = GetComponent<RectTransform>();
            if (pivotRect == null)
                pivotRect = transform.root.GetComponent<RectTransform>();

            prevSizeDelta = Vector2.zero;
        }

        private void UpdateScaleFactor()
        {
            float scaleFactor = Mathf.Min(pivotRect.sizeDelta.x / vertical.x, pivotRect.sizeDelta.y / vertical.y);
            transform.localScale = Vector3.one * scaleFactor;
        }
    }
}