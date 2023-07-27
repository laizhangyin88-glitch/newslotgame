using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BagelCode.Chat
{
    [RequireComponent(typeof(InputField))]
    public class ChatInputField : MonoBehaviour
    {
        public ChattingController owner;
        public ChannelType channelType;
        private InputField mInputField => GetComponent<InputField>();

        private void Start()
        {
            mInputField.onEndEdit.AddListener((message) =>
            {
                if (string.IsNullOrWhiteSpace(message)) return;

                owner.SendChatMessage(message);
                mInputField.text = "";

                if (TouchScreenKeyboard.isSupported)
                {
                     mInputField.DeactivateInputField();
                }
                else
                {
                     mInputField.ActivateInputField();
                     mInputField.Select();
                 }
             });


            gameObject.SetActive(true);
            if (IsTourchScreen())
            {
                GetComponent<CanvasGroup>().alpha = 0f;
                GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
            else
            {
                GetComponent<CanvasGroup>().alpha = 1f;
                GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
        }

        private bool IsTourchScreen()
        {
            if (TouchScreenKeyboard.isSupported && SystemInfo.deviceType != DeviceType.Desktop)
                return true;
            return false;
        }
    }
}