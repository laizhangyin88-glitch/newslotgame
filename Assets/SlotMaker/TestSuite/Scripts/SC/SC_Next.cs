using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using NodeCanvas.Framework;
using SlotMaker.Json;

namespace SlotMaker.TestSuite
{
	public class SC_Next : TestSuiteRunner
	{
#if DEV
		public override void Run(TestCaseRunner testCaseRunner, SlotMaker.TestSuite.TestSuite testSuite)
		{
            base.Run(testCaseRunner, testSuite);

			var contentInfos = ContentsManifest.GetContentInfos();
			ContentInfo nextGame = null;

            var gameTitle = BlackboardUtils.FindVariable<string>(null, "./game/gameTitle");
            if (gameTitle == null)
            {
            	nextGame = contentInfos[0];
            }
            else
            {
            	for (int i = 0; i < contentInfos.Count; ++i)
            	{
            		if (string.Equals(gameTitle.value, contentInfos[i].gameTitle))
            		{
            			if (i > (contentInfos.Count - 1))
            				nextGame = contentInfos[0];
        				else
        					nextGame = contentInfos[i];
    					break;
            		}
            	}
            }

            testCase.customData["contentInfo"] = nextGame;
            testCase.customData["debugSpin"] = true;

			StartCoroutine(LoadTestInfo(nextGame));
		}

        private IEnumerator LoadTestInfo(ContentInfo contentInfo)
        {
            string gameTitle = contentInfo.gameTitle;
            int gameId = contentInfo.gameId;

            List<DebugSpin> temp = null;
            yield return StartCoroutine(TestSuiteManager.Instance.GetDebugSpins(gameId, (ret) => temp = ret));

            var textAsset = AssetBundleManager.LoadAsset<TextAsset>("testsuite", "ReportId");
            var reportIdDict = SlotSimpleJson.DeserializeObject<Dictionary<string, string>>(textAsset.text);
            testCase.customData["testInfo"] = new ContentTestInfo() { reportId = reportIdDict[gameTitle], debugSpins = temp };

            testCaseRunner.Complete(testSuite);
            Stop();
        }
#endif
	}
}
