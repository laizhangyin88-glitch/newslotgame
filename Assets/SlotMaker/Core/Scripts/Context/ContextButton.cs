using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;

namespace SlotMaker
{
    public class ContextButton : ContextCompositor, IContextClickable, IContextBooleanProperty
    {
    	public Button button;

        private UnityAction<ContextElement> callBack;

        public override void UpdateContext(bool forceUpdate = false)
        {
            base.UpdateContext(forceUpdate);
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

        public void SetBooleanProperty(bool value)
        {
            button.interactable = value;
        }

        public bool GetBooleanProperty()
        {
            return button.interactable;
        }
    }
}
