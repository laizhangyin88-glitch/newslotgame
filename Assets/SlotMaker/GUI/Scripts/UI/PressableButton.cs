using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace SlotMaker
{
    public class PressableButton : PIDButton
    {
        [FormerlySerializedAs("onPress")]
        [SerializeField]
        private ButtonPressedEvent m_OnPress = new ButtonPressedEvent();

        public ButtonPressedEvent onPress
        {
            get
            {
                return this.m_OnPress;
            }
            set
            {
                this.m_OnPress = value;
            }
        }

        private void Press()
        {
            if (!this.IsActive() || !this.IsInteractable())
                return;
            UISystemProfilerApi.AddMarker("Button.onPress", (UnityEngine.Object) this);
            this.m_OnPress.Invoke();
        }
        
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            this.Press();
        }
        
        [Serializable]
        public class ButtonPressedEvent : UnityEvent
        {
        }
    }
}