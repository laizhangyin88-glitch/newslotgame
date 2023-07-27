using UnityEngine;

namespace BagelCode
{
    public class GestureMouseSwipeHandler : GestureMouseHandler
    {
        // Settings
        private const float MAX_SWIPE_TIME = 0.2f;
        //

        private float remainingSwipeTime = 0f;

        public override GestureManager.GestureType CheckGesture(MouseMotion motion, out GestureData gestureData)
        {
            gestureData = null;

            switch (motion.State)
            {
                case CursorState.NONE:
                    return GestureManager.GestureType.NONE;
                case CursorState.TAB:
                    remainingSwipeTime = MAX_SWIPE_TIME;
                    return GestureManager.GestureType.NONE;
                case CursorState.HOLD:
                    {
                        if(motion.DeltaPos == Vector2.zero)
                        {
                            remainingSwipeTime = MAX_SWIPE_TIME;
                        }
                        else
                        {
                            remainingSwipeTime -= Time.deltaTime;
                        }
                    }
                    return GestureManager.GestureType.NONE;
                case CursorState.UN_TAB:
                    if(remainingSwipeTime > 0f)
                    {
                        Vector2 movement = motion.DeltaPos;
                        float scale = movement.magnitude;

                        bool isRight = Vector2.Dot(movement, Vector2.right) > 0.5f;
                        bool isLeft = Vector2.Dot(movement, -Vector2.right) > 0.5f;

                        if (isRight)
                        {
                            gestureData = new GestureSwipeData()
                            { swipeScale = scale };
                            return GestureManager.GestureType.SWIPE_RIGHT;
                        }
                        else if (isLeft)
                        {
                            gestureData = new GestureSwipeData()
                            { swipeScale = scale };
                            return GestureManager.GestureType.SWIPE_LEFT;
                        }
                    }
                    return GestureManager.GestureType.NONE;
            }

            return GestureManager.GestureType.NONE;
        }
    }
}
