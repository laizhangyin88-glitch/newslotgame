using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
    [AddComponentMenu("UI/Toggle Animator")]
    [RequireComponent(typeof(RectTransform))]
    public class ToggleAnimator : MonoBehaviour
    {
        public Toggle toggle;
        public Animator animator;
        public string activeState = "Active";
        // public string inActiveTrigger = "InActive";

        private void Awake()
        {
            if(toggle == null)
                toggle = GetComponent<Toggle>();
            if(animator == null)
                animator = GetComponent<Animator>();

            InitListener();
        }

        private void InitListener()
        {
            if(toggle == null) return;

            toggle.onValueChanged.AddListener(SetOnOff);
        }

        private void OnEnable()
        {
            if(toggle == null) return;

            SetOnOff(toggle.isOn);
        }

        private void SetOnOff(bool toggle)
        {
            animator.SetBool( activeState, toggle);
        }
    }
}
