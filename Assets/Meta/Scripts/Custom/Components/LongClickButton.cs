
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BagelCode {
    public class LongClickButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
        private bool pointerDown;
        private float pointerDownTimer;

        public float requiredHoldTime;

        public UnityEvent onLongClick = new UnityEvent();

        public void OnPointerDown(PointerEventData eventData) {
            pointerDown = true;
        }

        public void OnPointerUp(PointerEventData eventData) {
            Reset();
        }



        private void Update() {
            if (pointerDown) {
                pointerDownTimer += Time.deltaTime;
                if (pointerDownTimer >= requiredHoldTime) {
                    if (onLongClick != null)
                        onLongClick.Invoke();

                    Reset();
                }
            }
        }

        private void Reset() {
            pointerDown = false;
            pointerDownTimer = 0;
        }

    }
}