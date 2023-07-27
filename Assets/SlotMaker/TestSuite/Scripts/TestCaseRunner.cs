using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.TestSuite
{

public class TestCaseRunner : MonoBehaviour
{
#if DEV
	public TestCase testCase { get; set; }

	private int suiteIndex;
    private int testIndex;
	private int repeat;
	private TestSuiteRunner testSuiteRunner;

	public void Run(TestCase testCase)
	{
		this.testCase = testCase;
        Time.timeScale = Convert.ToInt32(testCase.customData["timeScale"]);
		repeat = Convert.ToInt32(testCase.customData["repeat"]);

		suiteIndex = 0;
		RunTestSuite();
	}

	public void Stop()
	{
        TestSuiteManager.Instance.tcDescription.text = string.Empty;
        TestSuiteManager.Instance.tsDescription.text = string.Empty;

        Time.timeScale = 1;
		testSuiteRunner.Stop();
        GameObject.Destroy(gameObject);
	}

	public void Complete(TestSuite testSuite)
	{
		Debug.Log("[TestSuite] Completed " + testSuite.suiteId);

		++suiteIndex;
		RunTestSuite();
	}

	protected void RunTestSuite()
	{
		if (suiteIndex < testCase.testSuites.Count)
		{
			string suiteId = testCase.testSuites[suiteIndex];

			var prefab = AssetBundleManager.LoadAsset<GameObject>("testsuite", suiteId);
			var go = GameObject.Instantiate(prefab) as GameObject;
			go.name = suiteId;
			go.transform.SetParent(transform, false);

			testSuiteRunner = go.GetComponent<TestSuiteRunner>();
			var testSuite = TestSuiteManager.Instance.GetTestSuite(suiteId);

            TestSuiteManager.Instance.tcDescription.text = string.Format("{0}/{1}", (testIndex + 1), repeat);
            TestSuiteManager.Instance.tsDescription.text = suiteId.Replace("SC_", string.Empty);

            testSuiteRunner.Run(this, testSuite);
		}
		else if (++testIndex < repeat)
		{
			suiteIndex = 0;
			RunTestSuite();
		}
		else
		{
            Debug.Log("[TestSuite] Done: " + testCase.caseId);

            Stop();

            string quit = TestSuiteServer.GetUserData("quit");
            if (!string.IsNullOrEmpty(quit) && Convert.ToBoolean(quit))
                Application.Quit();
		}
	}
#endif
}

}
