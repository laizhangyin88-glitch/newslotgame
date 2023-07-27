using UnityEngine;

namespace BagelCode
{
    public class GestureMouseScrollHandler : GestureMouseHandler
    {
        public override GestureManager.GestureType CheckGesture(MouseMotion motion, out GestureData gestureData)
        {
            gestureData = null;

            switch (motion.State)
            {
                case CursorState.NONE:
                    return GestureManager.GestureType.NONE;
                case CursorState.TAB:
                    return GestureManager.GestureType.SCROLL_TAB;
                case CursorState.HOLD:
                    {
                        Vector2 movement = motion.DeltaPos;
                        if (movement != Vector2.zero)
                        {
                            gestureData = new GestureScrollData()
                            { movement = movement };
                            return GestureManager.GestureType.SCROLL_MOVE;
                        }
                    }
                    break;
                case CursorState.UN_TAB:
                    return GestureManager.GestureType.SCROLL_UN_TAB;
            }

            return GestureManager.GestureType.NONE;
        }
    }
}
