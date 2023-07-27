using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SlotMaker.TestSuite
{
    public class TestSuiteConsolePopup : MonoBehaviour
    {
    	public TextMeshProUGUI id;

    	private void Awake()
    	{
#if DEV
    		id.text = TestSuiteServer.GetUserId();
#endif
    	}

        public void LogSystemInfo()
        {
            TestSuiteManager.Instance.LoadGameObject("SystemInfo");
            Close();
        }

        public void OpenCustomReport()
        {
            TestSuiteManager.Instance.LoadGameObject("CustomReport");
            Close();
        }

    	public void Automations()
    	{
    		TestSuiteManager.Instance.LoadGameObject("Automations");
    		Close();
    	}

    	public void AppStore()
    	{
    		TestSuiteManager.Instance.LoadGameObject("App Store");
    		Close();
    	}

    	public void Settings()
    	{
    		TestSuiteManager.Instance.LoadGameObject("Settings Popup");
    		Close();
    	}

    	public void Logout()
    	{
#if DEV
    		TestSuiteServer.Logout();
    		Close();
#endif
    	}

    	public void Close()
    	{
    		GameObject.Destroy(gameObject);
    	}
    }
}
