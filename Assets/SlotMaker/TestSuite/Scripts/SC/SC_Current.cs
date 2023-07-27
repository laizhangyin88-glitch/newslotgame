using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using SlotMaker.Json;

namespace SlotMaker.TestSuite
{
    public class SC_Current : TestSuiteRunner 
    {
#if DEV
        public override void Run(TestCaseRunner testCaseRunner, SlotMaker.TestSuite.TestSuite testSuite)
        {
            base.Run(testCaseRunner, testSuite);

            string gameTitle = ContentBlackboard.Get().GetValue<Blackboard>("game").GetValue<string>("gameTitle");
            TextAsset textAsset = AssetBundleManager.LoadAsset<TextAsset>("testsuite", gameTitle);
            testCase.customData["testInfo"] = SlotSimpleJson.DeserializeObject<ContentTestInfo>(textAsset.text);

            testCaseRunner.Complete(testSuite);
            Stop();
        }
#endif
    }
}
