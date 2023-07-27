using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using SlotMaker;
using SlotMaker.Json;
using SlotMaker.TestSuite;

namespace BagelCode
{
    public class SC_CurrentNew : TestSuiteRunner 
    {
#if DEV
        public override void Run(TestCaseRunner testCaseRunner, SlotMaker.TestSuite.TestSuite testSuite)
        {
            base.Run(testCaseRunner, testSuite);

            StartCoroutine(LoadCurrentGameData());
        }

        private IEnumerator LoadCurrentGameData()
        {
            string gameTitle = ContentBlackboard.Get().GetValue<Blackboard>("game").GetValue<string>("gameTitle");
            int gameId = ContentBlackboard.Get().GetValue<Blackboard>("game").GetValue<int>("gameId");

            List<DebugSpin> temp = null;
            yield return StartCoroutine(TestSuiteManager.Instance.GetDebugSpins(gameId, (ret) => temp = ret));

            var textAsset = AssetBundleManager.LoadAsset<TextAsset>("testsuite", "ReportId");
            var reportIdDict = SlotSimpleJson.DeserializeObject<Dictionary<string, string>>(textAsset.text);
            string reportId;
            reportIdDict.TryGetValue(gameTitle, out reportId);
            testCase.customData["testInfo"] = new ContentTestInfo() { reportId = reportId, debugSpins = temp };

            testCaseRunner.Complete(testSuite);
            Stop();
        }
#endif
    }
}
