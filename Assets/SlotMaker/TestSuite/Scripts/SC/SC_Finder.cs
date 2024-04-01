using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using NodeCanvas.Framework;
using SlotMaker.Json;

namespace SlotMaker.TestSuite
{
    public class SC_Finder : TestSuiteRunner
    {
#if DEV && !NEW_NET
        private MessageDelegates delegates;
        private class ContentSortingInfo
        {
            public ContentInfo contentInfo;
            public int[] statValues = new int[3];
        }

#if BUILD_QA
        private const float WEIGHT_TEST_FREQEUNCY = 0.3f;
#else
        private const float WEIGHT_TEST_FREQEUNCY = 0.5f;
#endif

        private void Awake()
        {
            delegates = new MessageDelegates
            (
                new Dictionary<string, MessageDispatcher.EventDelegate>
                {
                    { "RefreshContentReport", RefreshContentReport }
                }
            );
        }

        private void Start()
        {
            TestSuiteManager.Instance.RefreshContentReport(false);
        }

        private void OnEnable()
        {
            MessageDispatcher.Register("OnTestSuite", delegates.Delegate);
        }

        private void OnDisable()
        {
            MessageDispatcher.UnRegister("OnTestSuite", delegates.Delegate);
        }

        private void RefreshContentReport(EventData eventData)
        {
            var result = ((EventData<Dictionary<string, TestSuiteReportResult>>)eventData).value;

            var contentInfos = ContentsManifest.GetContentInfos();

            var dict = new Dictionary<string, ContentSortingInfo>();
            foreach (var contentInfo in contentInfos)
            {
                if (!contentInfo.ignore)
                {
                    dict[contentInfo.gameTitle] = new ContentSortingInfo{ contentInfo = contentInfo };
                }
            }

            var statisticNames = new string[3];
            statisticNames[0] = TestSuiteManager.TEST_REPORT_STATISTICS_NAME[0];
            statisticNames[1] = TestSuiteManager.TEST_REPORT_STATISTICS_NAME[2];
            statisticNames[2] = TestSuiteManager.TEST_REPORT_STATISTICS_NAME[6];

            for (int i = 0; i < 3; ++i)
            {
                var results = result[statisticNames[i]].Results;
                foreach (var entry in results)
                {
                    if (dict.ContainsKey(entry.ReportName))
                    {
                        dict[entry.ReportName].statValues[i] = entry.StatValue;
                    }
                }
            }

            var otherList = new List<ContentSortingInfo>();
            var firstList = new List<ContentSortingInfo>();
            var weightList = new List<ContentSortingInfo>();
            bool weightFirst = (Random.value < WEIGHT_TEST_FREQEUNCY);
            foreach (var pair in dict)
            {
                if (pair.Value.statValues[0] == 0)
                    firstList.Add(pair.Value);
                else if (weightFirst && pair.Value.statValues[2] > 0)
                    weightList.Add(pair.Value);
                else
                    otherList.Add(pair.Value);
            }

            if (firstList.Count > 0)
            {
                Found(firstList[UnityEngine.Random.Range(0, firstList.Count - 1)]);
                return;
            }

            if (weightList.Count > 0)
            {
                Found(weightList[UnityEngine.Random.Range(0, weightList.Count - 1)]);
                return;
            }

            otherList.Sort((lhv, rhv) => lhv.statValues[1].CompareTo(rhv.statValues[1]));
            Found(otherList[0]);
        }

        private void Found(ContentSortingInfo contentSortingInfo)
        {
            testCase.customData["contentInfo"] = contentSortingInfo.contentInfo;
            testCase.customData["debugSpin"] = (contentSortingInfo.statValues[0] == 0);

            StartCoroutine(LoadTestInfo(contentSortingInfo.contentInfo));
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
