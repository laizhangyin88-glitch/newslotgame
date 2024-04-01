using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SlotMaker.TestSuite
{
	public class TestSuiteAppStore : MonoBehaviour
	{
		public List<Button> apps;

		public void OnClickApp(int buttonId)
		{
#if DEV && !NEW_NET
			var button = apps[buttonId];
			
			if (TestSuiteServer.HasApplication(button.gameObject.name))
				UninstallApp(button);
			else
				InstallApp(button);
#endif
        }

#if DEV && !NEW_NET
        private void Awake()
		{
			TestSuiteServer.GetApplicationList(Activates, null);
		}

		void Activates()
		{
			foreach (var button in apps)
			{
				button.interactable = true;
				button.GetComponent<Image>().color = TestSuiteServer.HasApplication(button.gameObject.name) ? TestSuiteManager.BUTTON2_COLOR : TestSuiteManager.BUTTON1_COLOR;
			}
		}

		private void InstallApp(Button button)
		{
			button.interactable = false;
			TestSuiteServer.InstallApplication(button.gameObject.name, 
				() => 
				{
					button.GetComponent<Image>().color = TestSuiteManager.BUTTON2_COLOR;
					button.interactable = true;
				}, null
			);
		}

		private void UninstallApp(Button button)
		{
			button.interactable = false;
			TestSuiteServer.UninstallApplication(button.gameObject.name, 
				() => 
				{
					button.GetComponent<Image>().color = TestSuiteManager.BUTTON1_COLOR;
					button.interactable = true;
				}, null
			);
		}
#endif

        public void Close()
		{
			GameObject.Destroy(gameObject);
		}
	}
}
