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
#if DEV && !NEW_NET
    		id.text = TestSuiteServer.GetUserId();
#endif
        }

        public void LogSystemInfo()
        {
#if DEV && !NEW_NET
            TestSuiteManager.Instance.LoadGameObject("SystemInfo");
#endif
            Close();
        }

        public void OpenCustomReport()
        {
#if DEV && !NEW_NET
            TestSuiteManager.Instance.LoadGameObject("CustomReport");
#endif
            Close();
        }

    	public void Automations()
    	{
#if DEV && !NEW_NET
            TestSuiteManager.Instance.LoadGameObject("Automations");
#endif
    		Close();
    	}

    	public void AppStore()
    	{
#if DEV && !NEW_NET
            TestSuiteManager.Instance.LoadGameObject("App Store");
#endif
    		Close();
    	}

    	public void Settings()
    	{
#if DEV && !NEW_NET
            TestSuiteManager.Instance.LoadGameObject("Settings Popup");
#endif
    		Close();
    	}

    	public void Logout()
    	{
#if DEV && !NEW_NET
            TestSuiteServer.Logout();
#endif
    		Close();
    	}

    	public void Close()
    	{
    		GameObject.Destroy(gameObject);
    	}
    }
}
