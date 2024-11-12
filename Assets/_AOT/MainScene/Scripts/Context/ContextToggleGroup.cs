using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SlotMaker
{
    public class ContextToggleGroup : ContextList, IContextIntProperty, IContextClickable
    {
        public ToggleGroup toggleGroup;

        private List<ContextToggle> toggleElements = new List<ContextToggle>();

        public override void AddContextElement(ContextElement element)
        {
            ContextToggle toggleElement = element as ContextToggle;
            if (toggleElement != null)
            {
                toggleElement.toggle.group = toggleGroup;
                
                if (!toggleElements.Contains(toggleElement))
                    toggleElements.Add(toggleElement);
            }

            base.AddContextElement(element);
        }

        public override void RemoveContextElement(ContextElement element)
        {
            ContextToggle toggleElement = element as ContextToggle;

            if (toggleElements.Contains(toggleElement))
                toggleElements.Remove(toggleElement);

            base.RemoveContextElement(element); 
        }

        public override void UpdateContext(bool forceUpdate = false)
        {
            if (IsDirty() || forceUpdate)
            {
                if (forceUpdate)
                {
                    toggleElements.Clear();
                    elementList.Clear();
                }

                UpdateContext(this, transform, forceUpdate);
                dirty = false;
            }
        }

        public void SetIntProperty(int index)
        {
            toggleElements[index].toggle.isOn = true;
        }

        public int GetIntProperty()
        {
            int count = toggleElements.Count;
            for (int i = 0; i < count; ++i)
            {
                var toggleElement = toggleElements[i];
                if (toggleElement.toggle.isOn)
                    return i;
            }
            
            return -1;
        }

        public void AddListenerOnClick(UnityAction<ContextElement> action)
        {
            int count = toggleElements.Count;;
            for (int i = 0; i < count; ++i)
            {
                var toggleElement = toggleElements[i];
                toggleElement.AddListenerOnClick(action);
            }
        }

        public void RemoveAllListener()
        {
            int count = toggleElements.Count;;
            for (int i = 0; i < count; ++i)
            {
                var toggleElement = toggleElements[i];
                toggleElement.RemoveAllListener();
            }
        }

        public void DoClick() {}
    }
}
