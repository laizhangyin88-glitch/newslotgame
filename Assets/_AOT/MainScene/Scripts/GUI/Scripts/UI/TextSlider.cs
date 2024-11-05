using UnityEngine;
using UnityEngine.UI;

namespace SlotMaker
{
    public class TextSlider : MonoBehaviour 
    {
        public RectTransform parentRect;
        public Text text;
        public float interval = 0.5f;
        public float waitDelay = 1f;

        public RectTransform textRect;
        private Vector2 newPosition = new Vector2(0f, 0f);
        public float range = 0f;
        private int direction = 1;
        private float waitDelta = 0f;


        void OnEnable()
        {
            if (text != null)
            {
                text.RegisterDirtyLayoutCallback(layoutCallback);
                textRect = text.GetComponent<RectTransform>();
            }
        }

        void OnDisable()
        {
            if (text != null)
            {
                text.UnregisterDirtyLayoutCallback(layoutCallback);
                textRect = text.GetComponent<RectTransform>();
            }
        }

        void Update () 
        {
            if (range > 0f) // if (text.preferredWidth > parentRect.rect.size)
            {
                if (waitDelta > 0f)
                {
                    waitDelta -= Time.deltaTime;
                }
                else
                {
                    if (textRect.anchoredPosition.x > range || textRect.anchoredPosition.x < -range)
                    {
                        direction *= -1;
                        waitDelta = waitDelay;
                    }

                    newPosition = textRect.anchoredPosition;
                    newPosition.x += interval * direction;

                    textRect.anchoredPosition = newPosition;
                }
            }
        }   

        private void layoutCallback()
        {
            range = (text.preferredWidth - parentRect.rect.width) / 2f;

            if (range < 5f) // ignore small move like buggy
            {
                range = 0f;
            }
        } 
    }
}
