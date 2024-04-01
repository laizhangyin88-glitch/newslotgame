using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using ParadoxNotion;
using NodeCanvas.Framework;
using SlotMaker.Json;
using TMPro;

namespace SlotMaker.TestSuite
{
    public class TestSuiteEnterGameApp : MonoBehaviour, ITaskCallback
    {
        public Transform testCaseTransform;
        public Transform lastGameTransform;
        public Transform scrollViewTransform;
        public Transform contentTransform;
        public GameObject buttonPrefab;

        public InputField inputFilter;

        public UnityEngine.CanvasGroup headerGroup;
        public TextMeshProUGUI textVersion;
        public TextMeshProUGUI[] textTotal;
        public Slider[] sliderTotal;

        private Dictionary<string, Blackboard> gameInfoDict = new Dictionary<string, Blackboard>();
        public System.Action endAction { get; set; }

        private Coroutine autoTest = null;
        private string caseId;

        private bool initialized;
        private List<ContentInfo> contentInfos = new List<ContentInfo>();
        private Dictionary<string, GameObject> contentGameObjects = new Dictionary<string, GameObject>();
        private Dictionary<string, int[]> statValues = new Dictionary<string, int[]>();
        private Color[] colorStatus = new Color[3]
        {
            Color.white,
            Color.green,
            Color.red
        };

        private MessageDelegates delegates;

        public void StopAutoTest()
        {
#if DEV && !NEW_NET
            if (autoTest != null)
            {
                StopCoroutine(autoTest);
                autoTest = null;

                testCaseTransform.GetChild(0).GetComponent<TextMeshProUGUI>().text = caseId;
            }
            else
            {
                TestSuiteManager.Instance.Run(caseId);

                endAction = null;
                Close();
            }
#endif
        }

#if DEV && !NEW_NET
        private void Awake()
        {
            string testCase = TestSuiteServer.GetUserData("testCase");
            if (!string.IsNullOrEmpty(testCase))
            {
                testCaseTransform.gameObject.SetActive(true);
                autoTest = StartCoroutine(RunTestCase(testCase));
            }

            textVersion.text = string.Format("{0}{1}", ApplicationSettings.GetApplicationStage()[0], ApplicationSettings.Instance.clientVersion);

            string lastGame = TestSuiteServer.GetUserData("lastGame");
            if (!string.IsNullOrEmpty(lastGame))
            {
                lastGameTransform.gameObject.SetActive(true);
                lastGameTransform.GetChild(0).GetComponent<TextMeshProUGUI>().text = lastGame;
                lastGameTransform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "LAST GAME";
                lastGameTransform.GetComponent<Button>().onClick.AddListener(
                    () =>
                    {
                        OnClickContent(lastGame);
                    }
                );
            }

            var gameInfoList = BlackboardUtils.FindVariable<List<Blackboard>>(null, "/gameInfoList").value;
            foreach (var gameInfo in gameInfoList)
            {
                gameInfoDict[gameInfo.GetValue<string>("gameTitle")] = gameInfo;
            }

            var json = AssetBundleManager.LoadAsset<TextAsset>("testsuite", "Contents");
            contentInfos = SlotSimpleJson.DeserializeObject<List<ContentInfo>>(json.text);
            contentInfos.Reverse(); 

            foreach (var contentInfo in contentInfos)
            {
                GameObject go = GameObject.Instantiate(buttonPrefab) as GameObject;
                go.transform.SetParent(contentTransform, false);

                go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = contentInfo.gameTitle;
                go.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = contentInfo.gameTitleName;
                go.GetComponent<Button>().onClick.AddListener(
                    () =>
                    {
                        OnClickContent(contentInfo.gameTitle);
                    }
                );
                var toggle = go.transform.GetChild(2).GetComponent<Toggle>();
                toggle.onValueChanged.AddListener(
                    (isOn) =>
                    {
                        OnClickWeight(contentInfo.gameTitle, isOn);
                    }
                );
                contentGameObjects[contentInfo.gameTitle] = go;

                statValues[contentInfo.gameTitle] = new int[7];
            }

            inputFilter.text = PlayerPrefs.GetString("TestSuite.ContentsFilter", "");
            OnFilterChanged(inputFilter.text);

            delegates = new MessageDelegates(new Dictionary<string, MessageDispatcher.EventDelegate>{
                { "RefreshContentReport", RefreshContentReport }
            });
        }

        public void OnFilterChanged(string filter)
        {
            int version = -1;

            string[] tokens = filter.Split(' ');
            List<Regex> regs = new List<Regex>();
            foreach (var token in tokens)
            {
                string[] options = token.Split(':');
                if (options.Length > 1)
                {
                    if (string.Equals(options[0], "v"))
                    {
                        int v;
                        if (int.TryParse(options[1], out v))
                            version = v;
                    }
                }
                else
                {
                    regs.Add(new Regex(token, RegexOptions.IgnoreCase | RegexOptions.Compiled));
                }
            }

            foreach (var contentInfo in contentInfos)
            {
                var go = contentGameObjects[contentInfo.gameTitle];
                go.SetActive(false);

                if (version > 0 && version != contentInfo.version)
                    continue;

                if (regs.Count == 0)
                {
                    go.SetActive(true);
                    continue;
                }

                foreach (var reg in regs)
                {
                    if (reg.IsMatch(contentInfo.gameTitle) || reg.IsMatch(contentInfo.gameTitleName))
                    {
                        go.SetActive(true);
                        break;
                    }
                }
            }

            PlayerPrefs.SetString("TestSuite.ContentsFilter", filter);
        }

        private void Start()
        {
            TestSuiteManager.Instance.RefreshContentReport();
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

            DateTime now = DateTime.UtcNow;
            DateTime minutes = (DateTime)result[TestSuiteManager.TEST_REPORT_STATISTICS_NAME[0]].NextReset;
            DateTime hours = (DateTime)result[TestSuiteManager.TEST_REPORT_STATISTICS_NAME[2]].NextReset;
            DateTime days = (DateTime)result[TestSuiteManager.TEST_REPORT_STATISTICS_NAME[4]].NextReset;
            sliderTotal[0].value = 60 - (minutes - now).Minutes;
            sliderTotal[1].value = 24 - (hours - now).Hours;
            sliderTotal[2].value = 7 - (days - now).Days;

            for (int i = 0; i < 7; ++i)
            {
                var results = result[TestSuiteManager.TEST_REPORT_STATISTICS_NAME[i]].Results;
                foreach (var entry in results)
                {
                    if (statValues.ContainsKey(entry.ReportName))
                    {
                        statValues[entry.ReportName][i] = entry.StatValue;
                    }
                }
            }

            int[] totalResult = new int[6];
            foreach (var pair in contentGameObjects)
            {
                var bb = pair.Value.GetComponent<Blackboard>();
                Image[] images = new Image[3];
                images[0] = bb.GetValue<GameObject>("hourly").GetComponent<Image>();
                images[1] = bb.GetValue<GameObject>("daily").GetComponent<Image>();
                images[2] = bb.GetValue<GameObject>("weekly").GetComponent<Image>();

                var statValue = statValues[pair.Key];
                for (int i = 0; i < 3; ++i)
                {
                    if (statValue[i * 2] < statValue[i * 2 + 1])
                        statValue[i * 2] = statValue[i * 2 + 1];

                    if (statValue[i * 2] == 0)
                        images[i].color = colorStatus[0];
                    else if (statValue[i * 2] != statValue[i * 2 + 1])
                        images[i].color = colorStatus[2];
                    else
                        images[i].color = colorStatus[1];

                    totalResult[i * 2] += statValue[i * 2];
                    totalResult[i * 2 + 1] += statValue[i * 2 + 1];
                }

                pair.Value.transform.GetChild(2).GetComponent<Toggle>().isOn = (statValue[6] > 0) ? true : false;
            }

            for (int i = 0; i < 3; ++i)
            {
                Color color;
                if (totalResult[i * 2] == 0)
                    color = colorStatus[0];
                else if (totalResult[i * 2] == totalResult[i * 2 + 1])
                    color = colorStatus[1];
                else
                    color = colorStatus[2];
                sliderTotal[i].fillRect.GetComponent<Image>().color = color;
                textTotal[i].text = string.Format("{0}/{1}", totalResult[i * 2 + 1], totalResult[i * 2]);
            }
            headerGroup.interactable = true;

            initialized = true;
        }

        private IEnumerator RunTestCase(string id)
        {
            caseId = id;

            for (int count = 5; count >= 0; --count)
            {
                testCaseTransform.GetChild(0).GetComponent<TextMeshProUGUI>().text = string.Format("{0} - Run after {1} sec", caseId, count);

                yield return new WaitForSeconds(1f);
            }

            TestSuiteManager.Instance.Run(caseId);

            endAction = null;
            Close();
        }

        private void OnClickContent(string gameTitle)
        {
            Blackboard gameInfo;
            if (!gameInfoDict.TryGetValue(gameTitle, out gameInfo))
                return;

            MetaSystem.SelectGame(gameInfo.GetValue<int>("gameId"));
            MetaSystem.EnterGame();

            endAction = null;
            Close();
        }

        private void OnClickWeight(string gameTitle, bool toggle)
        {
            var textAsset = AssetBundleManager.LoadAsset<TextAsset>("testsuite", "ReportId");
            var reportIdDict = SlotSimpleJson.DeserializeObject<Dictionary<string, string>>(textAsset.text);

            var parameter = new Dictionary<string, object>();
            parameter["reportId"] = reportIdDict[gameTitle];
            parameter["weight"]   = toggle ? 1 : 0;
            TestSuiteManager.Instance.ContentWeight(parameter);
        }
#endif

        public void Close()
        {
            if (endAction != null)
                endAction();

            GameObject.Destroy(gameObject);
        }
    }
}
