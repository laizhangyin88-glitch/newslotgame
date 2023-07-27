using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;

namespace SlotMaker
{
    public class ContextToggle : ContextCompositor, IContextClickable, IContextBooleanProperty
    {
        public Toggle toggle;
        public bool detectUnSelect = false;
        
        private UnityAction<ContextElement> callBack;
        
        public void AddListenerOnClick(UnityAction<ContextElement> action)
        {
            callBack += action;

            toggle.onValueChanged.RemoveListener( OnClickContext );
            toggle.onValueChanged.AddListener( OnClickContext );
        }

        public void RemoveAllListener()
        {
            callBack = null;
        }

        public void DoClick()
        {
            toggle.isOn = true;
        }


        public void SetBooleanProperty(bool value)
        {
            toggle.isOn = value;
        }

        public bool GetBooleanProperty()
        {
            return toggle.isOn;
        }

        private void OnClickContext(bool isSelect)
        {
            if (callBack != null)
            {
                if (isSelect || detectUnSelect)
                    callBack(this);
            }
        }
    }
}
