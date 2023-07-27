using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ParadoxNotion;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode
{
    public class ChattingInputField : MonoBehaviour
    {
        public InputField inputField;

        private bool activateChatting;

        private const string ON_META_UI_EVENT = "OnMetaUIEvent";
        private const string ACTIVATE_CHATTING_EVENT = "OnChatToggle";

        private const string ON_CHAT_SEND_EVENT = "OnChatSend";
        private const string ON_CLICK_CHAT = "OnClickChat";

        private void OnEnable()
        {
            MessageDispatcher.Register(ON_META_UI_EVENT, OnMetaUIEvent);
        }

        private void OnDisable()
        {
            MessageDispatcher.UnRegister(ON_META_UI_EVENT, OnMetaUIEvent);
        }

        private void OnMetaUIEvent(EventData eventData)
        {
            if (eventData.name.Equals(ACTIVATE_CHATTING_EVENT, StringComparison.Ordinal))
            {
                activateChatting = ((EventData<bool>)eventData).value;
                if (activateChatting)
                    inputField.ActivateInputField();
                else 
                    inputField.DeactivateInputField();
            }
        }

        public void OnEndEdit()
        {
            if (activateChatting && !inputField.wasCanceled)
            {
                // if(!string.IsNullOrEmpty(inputField.text))
                // {
                //     string trimMessage = inputField.text.Trim();

                //     if(!string.IsNullOrEmpty(trimMessage))
                //     {
                //         MessageDispatcher.Dispatch(ON_META_UI_EVENT, new EventData<string>(ON_CHAT_SEND_EVENT, trimMessage));
                //         inputField.text = "";
                //         inputField.ActivateInputField();
                //     }
                //     else
                //     {
                //         inputField.text = "";
                //         inputField.DeactivateInputField();
                //     }
                // }
                // else
                // {
                //     inputField.DeactivateInputField();
                //     GraphOwner.SendGlobalEvent(ON_CLICK_CHAT);
                // }
            }
        }

        private void Update()
        {
            if(activateChatting)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    SendInputMessage();
                }
            }
        }

        private void SendInputMessage()
        {
            if (activateChatting && !inputField.wasCanceled)
            {
                if(!string.IsNullOrEmpty(inputField.text))
                {
                    string trimMessage = inputField.text.Trim();

                    if(!string.IsNullOrEmpty(trimMessage))
                    {
                        MessageDispatcher.Dispatch(ON_META_UI_EVENT, new EventData<string>(ON_CHAT_SEND_EVENT, trimMessage));
                        inputField.text = "";
                        inputField.ActivateInputField();
                    }
                    else
                    {
                        inputField.text = "";
                        inputField.DeactivateInputField();
                    }
                }
                else
                {
                    inputField.DeactivateInputField();
                    EventSender.SendGlobalEvent(ON_CLICK_CHAT);
                }
            }
        }
    }
}
