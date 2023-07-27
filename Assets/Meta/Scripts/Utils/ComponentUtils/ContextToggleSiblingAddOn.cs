using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using SlotMaker;

namespace BagelCode
{
    public class ContextToggleSiblingAddOn : MonoBehaviour
    {
        public Toggle toggle;
        public bool isLastSibling = true;

        private void Start()
        {
            if(toggle == null)
                toggle = GetComponent<Toggle>();

            toggle.onValueChanged.RemoveListener( OnSelectContext );
            toggle.onValueChanged.AddListener( OnSelectContext );

            OnSelectContext(toggle.isOn);
        }

        private void OnEnable()
        {
            if(toggle != null)
            {
                OnSelectContext(toggle.isOn);
            }
        }

        public void OnSelectContext(bool isSelect)
        {
            if (isSelect)
            {
                if(isLastSibling)
                    transform.SetAsLastSibling();
                else
                    transform.SetAsFirstSibling();
            }
        }
    }
}
