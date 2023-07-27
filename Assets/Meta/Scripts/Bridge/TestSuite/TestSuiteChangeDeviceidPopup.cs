using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using TMPro;
using ParadoxNotion;

namespace BagelCode
{
    public class TestSuiteChangeDeviceidPopup : MonoBehaviour
    {
        public TMP_InputField nowInputFilter;
        public TMP_InputField changeInputFilter;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            string deviceID = PlayerPrefs.GetString("DEBUG_DEVICE_ID", "");
            if (string.IsNullOrEmpty(deviceID))
                deviceID = NativeHelper.Instance.GetDeviceID();

            SetNowInputText(deviceID);
        }

        public void Apply()
        {
            string changeValue = GetChangeText();
            if (!string.IsNullOrEmpty(changeValue))
            {
                if (changeValue.Length == 36)
                {
                    PlayerPrefs.SetString("DEBUG_DEVICE_ID", changeValue);

                    MessageDispatcher.Dispatch("OnSystemEvent", new EventData("SystemReset"));

                    Close();
                }
            }
        }

        public void Close() 
        {
            GameObject.Destroy(gameObject);
        }

        private void SetNowInputText(string text)
        {
            if (nowInputFilter != null)
                nowInputFilter.text = text;
        }

        private string GetChangeText()
        {
            if (changeInputFilter != null) return changeInputFilter.text;
            return "";
        }
    }
}