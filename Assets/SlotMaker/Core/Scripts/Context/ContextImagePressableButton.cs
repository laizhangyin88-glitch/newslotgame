using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace SlotMaker
{
    public class ContextImagePressableButton : ContextCompositor, IContextImage, IContextClickable, IContextPressable
    {
        public Image image;
        public PressableButton button;
        public UnityEvent onChangedSprite;
        private UnityAction<ContextElement> callBack;

        public void SetHash(string hashCode)
        {
        }

        public bool CheckHash(string hashCode)
        {
            return true;
        }

        public void SetSprite(Sprite sprite)
        {
            if (image == null) return;
            image.sprite = sprite;

            if (onChangedSprite != null)
                onChangedSprite.Invoke();
        }

        // public void AddListenerOnChangedSprite(UnityAction<ContextElement> action)
        // {
        // 	onChangedSprite.AddListener(() => { action(this); });
        // }

        public void SetColor(Color color)
        {
            if(image != null)
            {
                image.color = color;
            }
        }

        public void AddListenerOnClick(UnityAction<ContextElement> action)
        {
            callBack += action;
            // button.onClick.AddListener(() => { action(this); });

            button.onClick.RemoveListener( OnClickContext );
            button.onClick.AddListener( OnClickContext );
        }

        public void RemoveAllListener()
        {
            callBack = null;
        }

        public void DoClick()
        {
            button.onClick.Invoke();
        }

        private void OnClickContext()
        {
            if (callBack != null)
                callBack(this);
        }

        public void AddListenerOnPress(UnityAction<ContextElement> action)
        {
            callBack += action;
            
            button.onPress.RemoveListener( OnClickContext );
            button.onPress.AddListener( OnClickContext );
        }

        public void DoPress()
        {
            button.onPress.Invoke();
        }
    }
}