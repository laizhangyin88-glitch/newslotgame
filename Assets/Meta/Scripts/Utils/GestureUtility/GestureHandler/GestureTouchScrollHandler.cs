using UnityEngine;

namespace BagelCode
{
    public class GestureTouchScrollHandler : GestureTouchHandler
    {
        // Settings
        public const float SCROLL_SENSITIVITY = 1.3f;
        public const float DIFFERENCE_THRESHOLD = 0.05f;
        //

        public override GestureManager.GestureType CheckGesture(TouchMotion[] motions, out GestureData gestureData)
        {
            gestureData = null;

            var touch0 = motions[0];
            var touch1 = motions[1];

            if (touch0.State == CursorState.HOLD || touch1.State == CursorState.HOLD)
            {
                Vector2 movement0 = touch0.DeltaPos;
                Vector2 movement1 = touch1.DeltaPos;

                if (touch0.State != CursorState.HOLD)
                    movement0 = movement1;
                else if (touch1.State != CursorState.HOLD)
                    movement1 = movement0;

                float dot = Vector2.Dot(movement0.normalized, movement1.normalized);
                if (dot <= 0f) // 두 손가락의 방향이 다름
                    return GestureManager.GestureType.NONE;

                Vector2 movement = (movement0 + movement1) * 0.5f;

                if (movement != Vector2.zero)
                {
                    var movementScale = movement.magnitude * SCROLL_SENSITIVITY;
                    if (movementScale > DIFFERENCE_THRESHOLD)
                    {
                        gestureData = new GestureScrollData()
                        { movement = movement };
                        return GestureManager.GestureType.SCROLL_MOVE;
                    }
                }
            }
            else if (touch0.State == CursorState.TAB || touch1.State == CursorState.TAB)
            {
                return GestureManager.GestureType.SCROLL_TAB;
            }
            else if (touch0.State == CursorState.UN_TAB || touch1.State == CursorState.UN_TAB)
            {
                return GestureManager.GestureType.SCROLL_UN_TAB;
            }

            return GestureManager.GestureType.NONE;
        }
    }
}
