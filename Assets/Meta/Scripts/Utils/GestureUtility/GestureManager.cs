using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using ParadoxNotion;

namespace BagelCode
{
    public class GestureManager : MonoWeakSingleton<GestureManager>
    {
        public bool printGestureEvent = true;

        public const string ON_GESTURE_EVENT = "OnGestureEvent";

        //

        private TouchMotion[] touchMovements = new TouchMotion[MAX_TOUCH_COUNT];
        private MouseMotion mouseMovement = new MouseMotion();

        private Dictionary<GestureHandlerType, GestureHandler> gestureHandlerDict = new Dictionary<GestureHandlerType, GestureHandler>();

        private const int MAX_TOUCH_COUNT = 2;

        public enum GestureHandlerType
        {
            NONE = 0,

            ZOOM_IN_OUT,
            SCROLL,
            SWIPE,
            CORRECT_CLICK,
        }

        public enum GestureType
        {
            NONE = 0,

            // Zoom
            ZOOM_UN_TAB,
            ZOOM_IN,
            ZOOM_OUT,

            // Scroll
            SCROLL_TAB,
            SCROLL_UN_TAB,
            SCROLL_MOVE,

            // Swipe
            SWIPE_LEFT,
            SWIPE_RIGHT,

            // Click
            CORRECT_CLICK,
        };

        //

        private void Awake()
        {
            // Init
            for(int i = 0; i < MAX_TOUCH_COUNT; ++i)
            {
                touchMovements[i] = new TouchMotion();
            }
        }

        private void Update()
        {
            bool existGesture = gestureHandlerDict.Count > 0;
            if (existGesture)
            {
#if (UNITY_EDITOR && !TEST_MOBILE) || UNITY_WSA || UNITY_WEBGL // Mouse
                UpdateMouseState();
#elif UNITY_ANDROID || UNITY_IPHONE // Touch
                UpdateTouchState();
#endif
            }

            // Check Gesture
            foreach (var handler in gestureHandlerDict.Values)
            {
                if (!handler.Enabled) continue;

                if (handler is GestureTouchHandler touchHandler)
                {
                    var gestureType = touchHandler.CheckGesture(touchMovements, out GestureData gestureData);
                    SendGestureEvent(gestureType, gestureData);
                }
                else if(handler is GestureMouseHandler mouseHandler)
                {
                    var gestureType = mouseHandler.CheckGesture(mouseMovement, out GestureData gestureData);
                    SendGestureEvent(gestureType, gestureData);
                }
            }
        }

        // public method

        public void EnableGestureHandler(GestureHandlerType type, bool enableMultiTouch = true)
        {
            Input.multiTouchEnabled = enableMultiTouch;

            SetGestureHandlerState(type, true);
        }

        public void DisableGestureHandler(GestureHandlerType type)
        {
            SetGestureHandlerState(type, false);

            // todo multi touch 여러곳에서 쓰면 상황에 따라 원치않게 disable 되는 경우 있을지도?
#if !DEV
            Input.multiTouchEnabled = false;
#endif
        }

        //

        private void SetGestureHandlerState(GestureHandlerType type, bool state)
        {
            var handler = ValidateGestureHandler(type);
            if (handler != null)
                ValidateGestureHandler(type).Enabled = state;
        }

        // 새로운 핸들러 타입이 추가될 때 타입 바인딩 필요
        private GestureHandler ValidateGestureHandler(GestureHandlerType type)
        {
            if (gestureHandlerDict.ContainsKey(type))
                return gestureHandlerDict[type];

            GestureHandler newHandler;
            switch (type)
            {
#if (UNITY_EDITOR && !TEST_MOBILE) || UNITY_WSA || UNITY_WEBGL // Mouse Only
                case GestureHandlerType.ZOOM_IN_OUT:
                    newHandler = new GestureMouseZoomInOutHandler();
                    break;
                case GestureHandlerType.SCROLL:
                    newHandler = new GestureMouseScrollHandler();
                    break;
                case GestureHandlerType.CORRECT_CLICK:
                    newHandler = new GestureMouseCorrectClickHandler();
                    break;
                case GestureHandlerType.SWIPE:
                    newHandler = new GestureMouseSwipeHandler();
                    break;
#elif UNITY_ANDROID || UNITY_IPHONE // Touch Only
                case GestureHandlerType.ZOOM_IN_OUT:
                    newHandler = new GestureTouchZoomInOutHandler();
                    break;
                case GestureHandlerType.SCROLL:
                    newHandler = new GestureTouchScrollHandler();
                    break;
                case GestureHandlerType.CORRECT_CLICK:
                    newHandler = new GestureTouchCorrectClickHandler();
                    break;
                case GestureHandlerType.SWIPE:
                    newHandler = new GestureTouchSwipeHandler();
                    break;
#endif
                default:
                    Debug.LogError("GestureManager.ValidateGestureHandler failure. "
                        + type + " is undefined GestureHandlerType.");
                    return null;
            }

            gestureHandlerDict.Add(type, newHandler);
            return newHandler;
        }

        private void SendGestureEvent(GestureType type, GestureData gestureData)
        {
            if (type == GestureType.NONE)
                return;

            if (gestureData is null)
            {
                MessageDispatcher.Dispatch(ON_GESTURE_EVENT, new EventData(type.ToString()));
            }
            else
            {
                MessageDispatcher.Dispatch(ON_GESTURE_EVENT, new EventData<object>(type.ToString(), gestureData));
            }
        }

        private void UpdateMouseState()
        {
            var mousePos = Camera.main?.ScreenToWorldPoint(Input.mousePosition) ?? new Vector3();
            bool isButton = true;
            if(Input.GetMouseButtonDown(0))
            {
                mouseMovement.Tab(mousePos);
            }
            else if(Input.GetMouseButton(0))
            {
                mouseMovement.Update(mousePos);
            }
            else if(Input.GetMouseButtonUp(0))
            {
                mouseMovement.UnTab();
            }
            else
            {
                mouseMovement.UpdateMousePosition(mousePos);
                isButton = false;
            }

            bool isWheel = true;
            float wheelInput = Input.GetAxis("Mouse ScrollWheel");
            if (wheelInput > 0)
            {
                mouseMovement.WheelDown();
            }
            else if (wheelInput < 0)
            {
                mouseMovement.WheelUp();
            }
            else
            {
                isWheel = false;
            }

            if (!isButton && !isWheel)
                mouseMovement.InitState();
        }

        private void UpdateTouchState()
        {
            bool[] touches = new bool[MAX_TOUCH_COUNT];

            for(int i = 0; i < Input.touchCount; ++i)
            {
                Touch touch = Input.GetTouch(i);
                int fingerId = touch.fingerId;

                if(fingerId >= MAX_TOUCH_COUNT) continue;

                touches[fingerId] = true;

                var touchPos = Camera.main?.ScreenToWorldPoint(touch.position) ?? new Vector3();

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        touchMovements[fingerId]?.Tab(touchPos);
                        break;

                    case TouchPhase.Stationary:
                    case TouchPhase.Moved:
                        touchMovements[fingerId]?.Update(touchPos);
                        break;

                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        touchMovements[fingerId]?.UnTab();
                        break;
                }
            }

            for(int i = 0; i < MAX_TOUCH_COUNT; ++i)
            {
                if (!touches[i])
                    touchMovements[i].InitState();
            }
        }
    }
}
