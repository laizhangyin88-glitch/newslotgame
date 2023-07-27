using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SlotMaker.TestSuite
{
    public class TestSuiteAutoClicker : StandaloneInputModule
    {
#if DEV
        private bool active = false;
        public float interval = 0.1f;
        private Coroutine coroutine;
        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.LeftBracket))
            {
                AutoClicker();
            }
        }
        private void AutoClicker()
        {
            if (active)
            {
                StopCoroutine(coroutine);
                active = false;
            }
            else
            {
                coroutine = StartCoroutine(Click());
                active = true;
            }
        }
        private IEnumerator Click()
        {
            while (true)
            {
                Vector3 mousePos = Input.mousePosition;
                Vector2 pos = new Vector2(mousePos.x, mousePos.y);
                Input.simulateMouseWithTouches = true;
                var pointerData = GetTouchPointerEventData(new Touch()
                {
                    position = pos,
                }, out bool b, out bool bb);

                ProcessTouchPress(pointerData, true, true);
                yield return new WaitForSeconds(interval);
            }
            // ProcessTouchPress(pointerData, false, false);
            // ProcessMousePress()
        }
        // Start is called before the first frame update
#endif
    }
}