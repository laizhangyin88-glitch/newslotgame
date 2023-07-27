using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ParadoxNotion;
using NodeCanvas.Framework;

namespace SlotMaker.TestSuite
{
    public class TestSuiteKeyboardController : MonoBehaviour
    {
#if DEV
        private Vector2 touchBeginPosition0;
        private Vector2 touchBeginPosition1;

        private const string ON_CONTENT_UI_EVENT = "OnContentUIEvent";
        private const string ON_SHOW_UI_EVENT = "ShowUI";
        private const string ON_HIDE_UI_EVENT = "HideUI";
        private const string ON_SPINBUTTON_EVENT = "OnSpinButtonEvent";

        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetKeyDown(KeyCode.D))
                    MessageDispatcher.Dispatch("OnTestSuite", new EventData("OpenDebugSpin"));
                if (Input.GetKeyDown(KeyCode.S))
                    MessageDispatcher.Dispatch(ON_CONTENT_UI_EVENT, new EventData<bool>(ON_SHOW_UI_EVENT, true));
                if (Input.GetKeyDown(KeyCode.H))
                    MessageDispatcher.Dispatch(ON_CONTENT_UI_EVENT, new EventData<bool>(ON_HIDE_UI_EVENT, true));
                if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8))
                    MessageDispatcher.Dispatch("OnTestSuite", new EventData("OpenCustomReport"));
                if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9))
                    MessageDispatcher.Dispatch("OnTestSuite", new EventData("OpenEnterGame"));
                if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Period))
                    MessageDispatcher.Dispatch("OnTestSuite", new EventData("OpenConsole"));

                // Time control added 
                if (Input.GetKeyDown(KeyCode.Alpha1))
                    Time.timeScale = 1f;
                if (Input.GetKeyDown(KeyCode.Alpha2))
                    Time.timeScale = 2f;
                if (Input.GetKeyDown(KeyCode.Alpha3))
                    Time.timeScale = 4f;
                if (Input.GetKeyDown(KeyCode.Alpha4))
                    Time.timeScale = 0.5f;
            }

            if (Input.GetKey(KeyCode.F8))
                MessageDispatcher.Dispatch("OnTestSuite", new EventData("OpenDebugSpin"));
            if (Input.GetKey(KeyCode.F9))
                MessageDispatcher.Dispatch("OnTestSuite", new EventData("OpenEnterGame"));
            if (Input.GetKey(KeyCode.F10))
                MessageDispatcher.Dispatch("OnTestSuite", new EventData("OpenConsole"));

            touchesBegan.Clear();
            touchesEnded.Clear();

            ProcessTouches();

            gestureRecognizer.ProcessTouchesBegan(touchesBegan);
            gestureRecognizer.ProcessTouchesEnded(touchesEnded);
            var gestureType = gestureRecognizer.RegonizeGesture();
            switch (gestureType)
            {
                case GestureRecognizer.GestureType.UnKnown:
                    break;
                case GestureRecognizer.GestureType.Tab:
                    bool ingame = MainBlackboard.Get().GetValue<bool>("inGame");
                    if(ingame)
                        MessageDispatcher.Dispatch(ON_SPINBUTTON_EVENT, new EventData(ON_SPINBUTTON_EVENT));
                    break;
                case GestureRecognizer.GestureType.SwipeLeft2Right:
                    MessageDispatcher.Dispatch("OnTestSuite", new EventData("OpenCustomReport"));
                    break;
                case GestureRecognizer.GestureType.SwipeRight2Left:
                    MessageDispatcher.Dispatch("OnTestSuite", new EventData("OpenEnterGame"));
                    break;
                case GestureRecognizer.GestureType.SwipeTop2Bottom:
                    MessageDispatcher.Dispatch("OnTestSuite", new EventData("OpenConsole"));
                    break;
                case GestureRecognizer.GestureType.SwipeBottom2Top:
                    MessageDispatcher.Dispatch("OnTestSuite", new EventData("OpenDebugSpin"));
                    break;
                case GestureRecognizer.GestureType.PinchOpen:
                    MessageDispatcher.Dispatch(ON_CONTENT_UI_EVENT, new EventData<bool>(ON_HIDE_UI_EVENT, true));
                    break;
                case GestureRecognizer.GestureType.PinchClosed:
                    MessageDispatcher.Dispatch(ON_CONTENT_UI_EVENT, new EventData<bool>(ON_SHOW_UI_EVENT, true));
                    break;
            }
        }

        private readonly HashSet<int> isTouchDown = new HashSet<int>();

        private struct GestureTouch
        {
            public int fingerId;
            public Vector2 position;
        }
        private List<GestureTouch> touchesBegan = new List<GestureTouch>();
        private List<GestureTouch> touchesEnded = new List<GestureTouch>();

        private class GestureRecognizer
        {
            public enum GestureType
            {
                UnKnown,
                Tab,
                SwipeLeft2Right,
                SwipeRight2Left,
                SwipeTop2Bottom,
                SwipeBottom2Top,
                PinchOpen,
                PinchClosed
            };
            private List<GestureTouch> touch0 = new List<GestureTouch>();
            private List<GestureTouch> touch1 = new List<GestureTouch>();

            public void ProcessTouchesBegan(List<GestureTouch> touchesBegan)
            {
                for (int i = 0; i < touchesBegan.Count; ++i)
                {
                    var touchBegan = touchesBegan[i];
                    if (touchBegan.fingerId == 0)
                    {
                        touch0.Clear();
                        touch0.Add(touchBegan);
                    }
                    else if (touchBegan.fingerId == 1)
                    {
                        touch1.Clear();
                        touch1.Add(touchBegan);
                    }
                }
            }

            public void ProcessTouchesEnded(List<GestureTouch> touchesEnded)
            {
                for (int i = 0; i < touchesEnded.Count; ++i)
                {
                    var touchEnded = touchesEnded[i];
                    if (touchEnded.fingerId == 0)
                    {
                        touch0.Add(touchEnded);
                    }
                    else if (touchEnded.fingerId == 1)
                    {
                        touch1.Add(touchEnded);
                    }
                }
            }

            public GestureType RegonizeGesture()
            {
                if (touch0.Count == 2 && touch1.Count == 2)
                {
                    var delta0 = touch0[1].position - touch0[0].position;
                    var delta1 = touch1[1].position - touch1[0].position;
                    var dot = Vector2.Dot(delta0.normalized, delta1.normalized);

                    var beginLength = (touch0[0].position - touch1[0].position).sqrMagnitude;
                    var endLength = (touch0[1].position - touch1[1].position).sqrMagnitude;
                    var deltaLength = endLength - beginLength;

                    touch0.Clear();
                    touch1.Clear();

                    if (dot == 0f)
                    {
                        return GestureType.Tab;
                    }
                    else if (dot > 0f)
                    {
                        if (Mathf.Abs(delta0.x) > Mathf.Abs(delta0.y))
                        {
                            if (delta0.x > 0f)
                                return GestureType.SwipeLeft2Right;
                            else if (delta0.x < 0f)
                                return GestureType.SwipeRight2Left;
                        }
                        else
                        {
                            if (delta0.y > 0f)
                                return GestureType.SwipeBottom2Top;
                            else if (delta0.y < 0f)
                                return GestureType.SwipeTop2Bottom;
                        }
                    }
                    else
                    {
                        if (deltaLength > 0f)
                            return GestureType.PinchOpen;
                        else if (deltaLength < 0f)
                            return GestureType.PinchClosed;
                    }
                }

                return GestureType.UnKnown;
            }
        }
        private GestureRecognizer gestureRecognizer = new GestureRecognizer();

        /// Ref: Fingers
        private void ProcessTouches()
        {
            for (int i = 0; i < Input.touchCount; ++i)
            {
                ProcessTouch(Input.GetTouch(i));
            }
        }

        private void ProcessTouch(Touch t)
        {
            GestureTouch g;
            g.fingerId = t.fingerId;
            g.position = t.position;

            if (isTouchDown.Contains(t.fingerId))
            {
                if (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary)
                {
                }
                else if (t.phase != TouchPhase.Began)
                {
                    touchesEnded.Add(g);
                    isTouchDown.Remove(t.fingerId);
                }
                else
                {
                    Debug.LogError("Invalid state, touch " + t.fingerId + " began but isTouchDown was already true for this touch");
                }
            }
            else if (t.phase != TouchPhase.Ended && t.phase != TouchPhase.Canceled)
            {
                touchesBegan.Add(g);
                isTouchDown.Add(t.fingerId);
            }
        }
#endif
    }
}
