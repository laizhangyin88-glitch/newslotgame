using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    public class MirrorableRectTransform : MonoBehaviour
    {
        public RectTransform targetRect;

        private RectTransform _rectTransform;
        public RectTransform rectTransform { get { return _rectTransform ?? (_rectTransform = GetComponent<RectTransform>()); } }

        private void Update()
        {
            if (targetRect != null)
            {
                rectTransform.position = targetRect.position;
                rectTransform.sizeDelta = new Vector2(targetRect.rect.width, targetRect.rect.height);
            }
        }
    }
}
