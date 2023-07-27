using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.IoC.Strategy
{
    [CreateAssetMenu(fileName = "New Page Position Property", menuName = "SlotMaker2/IoC/Transform/Property/Page Position")]
    public class PagePositionPropertyStrategy : PropertyStrategy<RectTransform>
    {
        public RectTransform.Axis axis = RectTransform.Axis.Horizontal;

        public override Vector3 GetVector3(Component comp)
        {
            RectTransform rectTransform = Get(comp);
            int childCount = rectTransform.childCount;
            Vector3 pagePosition = rectTransform.anchoredPosition;
            Vector2 sizeDelta = rectTransform.sizeDelta;

            if (childCount > 0)
            {
                switch (axis)
                {
                case RectTransform.Axis.Vertical:
                    pagePosition.y = ConvertToPagePosition(pagePosition.y, sizeDelta.y, childCount);
                    break;
                case RectTransform.Axis.Horizontal:
                    pagePosition.x = ConvertToPagePosition(pagePosition.x, sizeDelta.x, childCount);
                    break;
                }
            }

            return pagePosition;
        }

        public override void SetVector3(Component comp, Vector3 value)
        {
            RectTransform rectTransform = Get(comp);
            int childCount = rectTransform.childCount;
            Vector2 sizeDelta = rectTransform.sizeDelta;

            if (childCount > 0)
            {
                switch (axis)
                {
                case RectTransform.Axis.Vertical:
                    value.y = ConvertToAnchoredPosition(value.y, sizeDelta.y, childCount);
                    break;
                case RectTransform.Axis.Horizontal:
                    value.x = ConvertToAnchoredPosition(value.x, sizeDelta.x, childCount);
                    break;
                }
            }

            rectTransform.anchoredPosition = value;
        }

        private float ConvertToPagePosition(float position, float size, int pageCount)
        {
            float pageSize = size / pageCount;
            float pageOffset = pageSize != 0f ? (position / pageSize) : 0f;
            return -(pageOffset - (float)(pageCount - 1) * 0.5f);
        }

        private float ConvertToAnchoredPosition(float pagePosition, float size, int pageCount)
        {
            float pageSize = size / pageCount;
            return ((float)(pageCount - 1) * 0.5f - pagePosition) * pageSize;
        }
    }
}