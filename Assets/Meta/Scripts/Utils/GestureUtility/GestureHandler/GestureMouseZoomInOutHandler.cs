using UnityEngine;

namespace BagelCode
{
    public class GestureMouseZoomInOutHandler : GestureMouseHandler
    {
        // Settings
        private const float ZOOM_SCALE = 1f;
        //

        public override GestureManager.GestureType CheckGesture(MouseMotion motion, out GestureData gestureData)
        {
            gestureData = null;

            Vector2 zoomPoint = motion.CurrentPos;

            switch (motion.State)
            {
                case CursorState.NONE:
                    return GestureManager.GestureType.NONE;
                case CursorState.WHEEL_UP: // Zoom In
                    gestureData = new GestureZoomInOutData()
                    { zoomScale = -ZOOM_SCALE, zoomPoint = zoomPoint };
                    return GestureManager.GestureType.ZOOM_IN;
                case CursorState.WHEEL_DOWN: // Zoom Out
                    gestureData = new GestureZoomInOutData()
                    { zoomScale = ZOOM_SCALE, zoomPoint = zoomPoint };
                    return GestureManager.GestureType.ZOOM_OUT;
            }

            return GestureManager.GestureType.NONE;
        }
    }
}
