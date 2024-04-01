using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlotMaker.TestSuite
{
    public class TestSuiteSettingsPopup : MonoBehaviour
    {
        public List<Button> buttons;
        public List<Button> logFilters;

        private bool dirty;

        private string testCase;
        private string forceGuestMode;
        private string quit;

        private LogFilter lastLogFilter;

        private void Awake()
        {
            lastLogFilter = ApplicationSettings.Instance.logFilter;
            LogFilter logFilter = lastLogFilter;

            for (int i = 0; i < logFilters.Count; ++i)
            {
                LogFilter mask = (LogFilter)(1 << i);
                logFilters[i].GetComponent<Image>().color = ((logFilter & mask) == mask) ? TestSuiteManager.BUTTON2_COLOR : TestSuiteManager.BUTTON1_COLOR;
            }
#if DEV && !NEW_NET
            testCase = TestSuiteServer.GetUserData("testCase");
            forceGuestMode = TestSuiteServer.GetUserData("forceGuestMode");
            quit = TestSuiteServer.GetUserData("quit");
#endif

            buttons[0].GetComponent<Image>().color = !string.IsNullOrEmpty(testCase) ? TestSuiteManager.BUTTON2_COLOR : TestSuiteManager.BUTTON1_COLOR;
            buttons[1].GetComponent<Image>().color = UserDataToBoolean(forceGuestMode) ? TestSuiteManager.BUTTON2_COLOR : TestSuiteManager.BUTTON1_COLOR;
            buttons[2].GetComponent<Image>().color = UserDataToBoolean(quit) ? TestSuiteManager.BUTTON2_COLOR : TestSuiteManager.BUTTON1_COLOR;
        }

        public void OnClickTestButton(int index)
        {
            bool toggle = false;
            switch (index)
            {
            case 0:
                toggle = string.IsNullOrEmpty(testCase);
                testCase = toggle ? "CC01" : null;
                break;
            case 1:
                toggle = !UserDataToBoolean(forceGuestMode);
                forceGuestMode = toggle.ToString();
                break;
            case 2:
                toggle = !UserDataToBoolean(quit);
                quit = toggle.ToString();
                break;
            }
            buttons[index].GetComponent<Image>().color = toggle ? TestSuiteManager.BUTTON2_COLOR : TestSuiteManager.BUTTON1_COLOR;

            dirty = true;
        }

        public void OnClickLogFilter(int index)
        {
            LogFilter mask = (LogFilter)(1 << index);
            LogFilter logFilter = ApplicationSettings.Instance.logFilter;

            if ((logFilter & mask) == mask)
            {
                ApplicationSettings.Instance.logFilter &= ~mask;
                logFilters[index].GetComponent<Image>().color = TestSuiteManager.BUTTON1_COLOR;
            }
            else
            {
                ApplicationSettings.Instance.logFilter |= mask;
                logFilters[index].GetComponent<Image>().color = TestSuiteManager.BUTTON2_COLOR;
            }
        }

        private bool UserDataToBoolean(string userData)
        {
            if (string.IsNullOrEmpty(userData))
                return false;

            return Convert.ToBoolean(userData);
        }

        public void Close()
        {
#if DEV
            GetComponent<CanvasGroup>().interactable = false;

            LogFilter logFilter = ApplicationSettings.Instance.logFilter;
            if (!dirty && logFilter == lastLogFilter)
            {
                GameObject.Destroy(gameObject);
                return;
            }

            //TestSuiteServer.UpdateUserData(
            //    new Dictionary<string, string>{ 
            //        { "testCase", testCase }, 
            //        { "forceGuestMode", forceGuestMode },
            //        { "quit", quit },
            //        { "logFilter", ((int)ApplicationSettings.Instance.logFilter).ToString() }
            //    },
            //    () => { GameObject.Destroy(gameObject); },
            //    () => { Debug.LogError("[TestSuite] UpdateUserData failed"); }
            //);
#endif
        }
    }
}
