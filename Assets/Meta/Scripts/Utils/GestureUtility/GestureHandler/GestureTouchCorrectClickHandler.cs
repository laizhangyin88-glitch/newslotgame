using UnityEngine;

namespace BagelCode
{
    public class GestureTouchCorrectClickHandler : GestureTouchHandler
    {
        // Settings
        public const float DIFFERENCE_THRESHOLD = 0.2f;
        //

        private bool progress = false;
        private bool isPointerOverGameObject = false;
        private GameObject clickedGameObject = null;

        public override GestureManager.GestureType CheckGesture(TouchMotion[] motions, out GestureData gestureData)
        {
            gestureData = null;

            var touch0 = motions[0];
            var touch1 = motions[1];

            if(touch1.State != CursorState.NONE)
            {
                progress = false;
                return GestureManager.GestureType.NONE;
            }

            switch(touch0.State)
            {
                case CursorState.NONE:
                    progress = false;
                    isPointerOverGameObject = false;
                    clickedGameObject = null;
                    return GestureManager.GestureType.NONE;
                case CursorState.TAB:
                    progress = true;
                    isPointerOverGameObject = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId);
                    clickedGameObject = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
                    return GestureManager.GestureType.NONE;
                case CursorState.HOLD:
                    if (touch0.DeltaPos.magnitude > DIFFERENCE_THRESHOLD)
                    {
                        progress = false;
                    }
                    return GestureManager.GestureType.NONE;
                case CursorState.UN_TAB:
                    if (progress)
                    {
                        gestureData = new GestureCorrectClickData()
                        {
                            clickPos = touch0.CurrentPos,
                            isPointerOverGameObject = isPointerOverGameObject,
                            clickedGameObject = clickedGameObject,
                        };
                        return GestureManager.GestureType.CORRECT_CLICK;
                    }
                    break;
            }

            return GestureManager.GestureType.NONE;
        }
    }
}
