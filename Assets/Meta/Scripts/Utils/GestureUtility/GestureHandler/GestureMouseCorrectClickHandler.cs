
namespace BagelCode
{
    public class GestureMouseCorrectClickHandler : GestureMouseHandler
    {
        // Settings
        public const float DIFFERENCE_THRESHOLD = 0.2f;
        //

        private bool progress = false;

        public override GestureManager.GestureType CheckGesture(MouseMotion motion, out GestureData gestureData)
        {
            gestureData = null;

            switch (motion.State)
            {
                case CursorState.NONE:
                    progress = false;
                    return GestureManager.GestureType.NONE;
                case CursorState.TAB:
                    progress = true;
                    return GestureManager.GestureType.NONE;
                case CursorState.HOLD:
                    float delta = (motion.BeganPos - motion.CurrentPos).magnitude;
                    if (delta > DIFFERENCE_THRESHOLD)
                    {
                        progress = false;
                    }
                    return GestureManager.GestureType.NONE;
                case CursorState.UN_TAB:
                    if (progress)
                    {
                        gestureData = new GestureCorrectClickData()
                        {
                            clickPos = motion.CurrentPos,
                            isPointerOverGameObject = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(),
                            clickedGameObject = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject,
                        };

                        return GestureManager.GestureType.CORRECT_CLICK;
                    }
                    break;
            }

            return GestureManager.GestureType.NONE;
        }
    }
}
