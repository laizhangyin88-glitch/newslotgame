using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace BSS.Utils {
    public static class UIUtility 
    {
        public static RectTransform SetCanvas(this RectTransform rectTr, Canvas canvas) {
            rectTr.SetParent(canvas.transform, false);
            return rectTr;
        }

        public static RectTransform SetFullSize(this RectTransform rectTr) {
            rectTr.sizeDelta = new Vector2(0.0f, 0.0f);
            rectTr.anchorMin = new Vector2(0.0f, 0.0f);
            rectTr.anchorMax = new Vector2(1.0f, 1.0f);
            rectTr.pivot = new Vector2(0.5f, 0.5f);
            rectTr.offsetMin = Vector2.zero;
            rectTr.offsetMax = Vector2.zero;
            return rectTr;
        }

        public static RectTransform SetSize(this RectTransform rectTr, Vector2 newSize) {
            var pivot = rectTr.pivot;
            var dist = newSize - rectTr.rect.size;
            rectTr.offsetMin = rectTr.offsetMin - new Vector2(dist.x * pivot.x, dist.y * pivot.y);
            rectTr.offsetMax = rectTr.offsetMax + new Vector2(dist.x * (1f - pivot.x), dist.y * (1f - pivot.y));
            return rectTr;
        }
        public static void SetAnchorsFix(this RectTransform rectTr, float x, float y) {
            rectTr.anchorMin = new Vector2(x, y);
            rectTr.anchorMax = new Vector2(x, y);
        }
        public static void SetAnchorsStretch(this RectTransform rectTr, float min, float max) {
            rectTr.anchorMin = new Vector2(min, min);
            rectTr.anchorMax = new Vector2(max, max);
        }
        public static void SetOffset(this RectTransform rectTr, float left, float right, float top, float bottom) {
            rectTr.offsetMin = new Vector2(left, top);
            rectTr.offsetMax = new Vector2(-right, -bottom);
        }

        public static Rect GetWorldRect(this RectTransform rectTr) {
            var rect = new Rect();
            var corners = new Vector3[4];
            rectTr.GetWorldCorners(corners);
            rect.min = corners[0];
            rect.max = corners[2];
            return rect;
        }
    }
}
