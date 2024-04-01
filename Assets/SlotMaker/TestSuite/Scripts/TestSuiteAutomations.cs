using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SlotMaker.TestSuite
{
    public class TestSuiteAutomations : MonoBehaviour
    {
        public Transform testCaseTransform;
    	public GameObject buttonPrefab;
        public GameObject keyValuePairPrefab;

#if DEV && !NEW_NET

        private void Awake()
    	{
    		foreach (var pair in TestSuiteManager.Instance.testCases)
    		{
    			GameObject go = GameObject.Instantiate(buttonPrefab) as GameObject;
    			go.transform.SetParent(testCaseTransform, false);

    			go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = string.Format("{0} - {1}", pair.Value.caseId, pair.Value.description);
    			go.GetComponent<Button>().onClick.AddListener(
    				() =>
    				{
    					OnClickTestCase(pair.Key);
    				}
    			);
    		}
    	}

    	private void OnClickTestCase(string caseId)
    	{
            transform.gameObject.DestroyChildren();
            
            GameObject go = GameObject.Instantiate(buttonPrefab) as GameObject;
            go.transform.SetParent(transform, false);
            go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Run";
            go.GetComponent<Button>().onClick.AddListener(
                () =>
                {
                    OnClickRun(caseId);
                }
            );
            
            var testCase = TestSuiteManager.Instance.GetTestCase(caseId);
            foreach (var pair in testCase.customData)
            {
                go = GameObject.Instantiate(keyValuePairPrefab) as GameObject;
                go.transform.SetParent(transform, false);
                go.GetComponent<TestSuiteKeyValuePair>().Bind(testCase, pair.Key);
            }

            go = GameObject.Instantiate(buttonPrefab) as GameObject;
            go.transform.SetParent(transform, false);
            go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Close";
            go.GetComponent<Button>().onClick.AddListener(Close);
    	}
        
        private void OnClickRun(string caseId)
        {
            TestSuiteManager.Instance.Run(caseId);
            Close();
        }
#endif

        public void Close()
    	{
    		GameObject.Destroy(gameObject);
    	}
    }
}
