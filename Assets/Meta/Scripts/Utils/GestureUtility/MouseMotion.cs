using UnityEngine;

namespace BagelCode
{
    public class MouseMotion : Motion
    {
        public void WheelUp()
        {
            state = CursorState.WHEEL_UP;
        }

        public void WheelDown()
        {
            state = CursorState.WHEEL_DOWN;
        }

        public void UpdateMousePosition(Vector2 mousePos)
        {
            currentPos = mousePos;
        }
    }
}
