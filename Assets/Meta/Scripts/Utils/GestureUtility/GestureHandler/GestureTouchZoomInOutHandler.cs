using UnityEngine;

namespace BagelCode
{
    public class GestureTouchZoomInOutHandler : GestureTouchHandler
    {
        // Settings
        public const float ZOOM_SENSITIVITY = 1.5f;
        public const float DIFFERENCE_THRESHOLD = 0.05f;
        //

        public override GestureManager.GestureType CheckGesture(TouchMotion[] motions, out GestureData gestureData)
        {
            gestureData = null;

            var touch0 = motions[0];
            var touch1 = motions[1];

            if (touch0.State == CursorState.NONE ||
                touch1.State == CursorState.NONE)
                return GestureManager.GestureType.NONE;

            if (touch0.State == CursorState.UN_TAB ||
                touch1.State == CursorState.UN_TAB)
                return GestureManager.GestureType.ZOOM_UN_TAB;

            var prevDistance = Vector2.Distance(touch0.PrevPos, touch1.PrevPos);
            var currentDistance = Vector2.Distance(touch0.CurrentPos, touch1.CurrentPos);

            // Calc Center
            var zoomPoint = (touch0.CurrentPos + touch1.CurrentPos) * 0.5f;

            float zoomScale = (currentDistance - prevDistance) * ZOOM_SENSITIVITY;
            if (zoomScale > DIFFERENCE_THRESHOLD) // Zoom In
            {
                gestureData = new GestureZoomInOutData()
                { zoomScale = zoomScale, zoomPoint = zoomPoint };
                return GestureManager.GestureType.ZOOM_IN;
            }
            else if (zoomScale < -DIFFERENCE_THRESHOLD) // Zoom Out
            {
                gestureData = new GestureZoomInOutData()
                { zoomScale = zoomScale, zoomPoint = zoomPoint };
                return GestureManager.GestureType.ZOOM_OUT;
            }
            else
            {
                return GestureManager.GestureType.NONE;
            }
        }
    }
}
