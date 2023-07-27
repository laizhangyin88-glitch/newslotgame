using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using SlotMaker;

namespace BagelCode
{
    public class PlatformInputField : UnityEngine.UI.InputField
    {
        [SerializeField]
        private UnityStringEvent onEndInput;

        public bool isSequenceChat;
        public bool isClearEndEdit;

        public void OnPlatformEndEdit()
        {
    #if !UNITY_WSA && !UNITY_STANDALONE_WIN && !UNITY_STANDALONE_OSX && !UNITY_WEBGL && !UNITY_EDITOR
            onEndInput.Invoke(this.text);
    #endif
        }

    #if UNITY_WSA || (UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX) || UNITY_EDITOR
        private bool activeForcus;

        private void OnDisable()
        {
            activeForcus = false;
            this.DeactivateInputField();
        }

        private void Update()
        {
            if(activeForcus)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    OnKeyboardReturn();
                }
            }

            activeForcus = this.isFocused;
        }

        private void OnKeyboardReturn()
        {
            if(!string.IsNullOrEmpty(this.text))
            {
                string trimMessage = this.text.Trim();

                if(!string.IsNullOrEmpty(trimMessage))
                {
                    onEndInput.Invoke(trimMessage);

                    if(isClearEndEdit)
                        this.text = "";

                    if(isSequenceChat)
                    {
                        this.ActivateInputField();
                    }
                    else
                    {
                        activeForcus = false;
                        this.DeactivateInputField();
                    }
                }
                else
                {
                    this.text = "";
                    activeForcus = false;
                    this.DeactivateInputField();
                }
            }
            else
            {
                activeForcus = false;
                this.DeactivateInputField();
            }
        }
    #endif

    #if UNITY_WEBGL && !UNITY_EDITOR 

        public override void OnSelect(BaseEventData eventData)
        {
            this.text = WebNativeDialog.OpenNativeStringDialog("Input Text", this.text);
            onEndInput.Invoke(this.text);
            StartCoroutine(this.DelayInputDeactive());
        }
        private IEnumerator DelayInputDeactive()
        {
            yield return new WaitForEndOfFrame();
            
            this.DeactivateInputField();
            EventSystem.current.SetSelectedGameObject(null);
        }

        private IEnumerator OverlayHtmlCoroutine()
        {
            yield return new WaitForEndOfFrame();
            this.DeactivateInputField();
            EventSystem.current.SetSelectedGameObject(null);
            WebGLInput.captureAllKeyboardInput = false;
            while (WebNativeDialog.IsOverlayDialogActive())
            {
                yield return null;
            }
            WebGLInput.captureAllKeyboardInput = true;

            if (!WebNativeDialog.IsOverlayDialogCanceled())
            {
                this.text = WebNativeDialog.GetOverlayDialogValue();
            }
        }
        
    #endif
    }
}
