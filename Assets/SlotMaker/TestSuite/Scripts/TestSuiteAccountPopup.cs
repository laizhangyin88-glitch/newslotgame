using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlotMaker.TestSuite
{
    public class TestSuiteAccountPopup : MonoBehaviour
    {
        private const string DEBUG_DEVICE_ID = "DEBUG_DEVICE_ID";

        private void Awake()
        {
#if DEV && !NEW_NET
            TestSuiteManager.Instance.ClosePopup();
#endif
        }

        public void NewGuest()
        {
            PlayerPrefs.SetString(DEBUG_DEVICE_ID, Guid.NewGuid().ToString());
            GameObject.Destroy(gameObject);
        }

        public void Reset()
        {
            PlayerPrefs.SetString(DEBUG_DEVICE_ID, "");
            GameObject.Destroy(gameObject);
        }

        public void Close()
        {
            GetComponent<CanvasGroup>().interactable = false;
            GameObject.Destroy(gameObject);
        }
    }
}
