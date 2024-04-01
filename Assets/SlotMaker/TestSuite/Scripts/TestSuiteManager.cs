using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using ParadoxNotion;
using NodeCanvas.Framework;
using SlotMaker.Json;
using TMPro;

namespace SlotMaker.TestSuite
{
    public class TestSuiteManager : MonoSingleton<TestSuiteManager>
    {
        public Transform panel;
        public GameObject guide;
        public TextMeshProUGUI version;

        public TextMeshProUGUI tcDescription;
        public TextMeshProUGUI tsDescription;
        public TextMeshProUGUI errorLogDisplay;

        public static readonly Color PANEL_COLOR = new Color(0.22f, 0.22f, 0.22f, 0.498f);
        public static readonly Color BUTTON1_COLOR = new Color(0.298f, 0.298f, 0.298f, 1f);
        public static readonly Color BUTTON2_COLOR = new Color(0.176f, 0.176f, 0.176f, 1f);

#if DEV
        public string id { get; set; }
        private int versionType { get; set; }

        private Dictionary<string, MessageDispatcher.EventDelegate> delegates { get; set; }= new Dictionary<string, MessageDispatcher.EventDelegate>();

        public Dictionary<string, ContentInfo> contentInfos { get; set; }= new Dictionary<string, ContentInfo>();

        public DebugSpinParam debugDataList { get; set; }
        public Dictionary<int, List<DebugSpin>> debugSpinInfoDict { get; set; } = new Dictionary<int, List<DebugSpin>>();

        public Dictionary<string, TestCase> testCases { get; set; }
        public Dictionary<string, TestSuite> testSuites { get; set; }

        public TestCaseRunner testCaseRunner { get; set; }
        public TestSuiteDebugSpinRunner DebugSpinRunner { get; set; }

        private bool enterGame;
        private GameObject currentPopup;

        private List<string> logs = new List<string>();
        private string lastLog = string.Empty;
        private static readonly StringBuilder Sb = new StringBuilder();

        private bool showLogOnScreen = false;

        private string _debugParam = string.Empty;
        public string DebugParam
        {
            get { return _debugParam; }
            set { _debugParam = value ?? string.Empty; }
        }

        public static string[] TEST_REPORT_STATISTICS_NAME;
        public static long WORKSPACE = 11140323218804;
        public static List<long> PROJECTS_BUG_REPORT_QA = new List<long>{ 923605408955908 };
        public static List<long> PROJECTS_BUG_REPORT_DEV = new List<long>{ 923605408955910 };
        public static string[] CUSTOM_FIELDS_CONTENT_BUG_REPORT = new string[]{
            "678443159158136", // Content
            "307862012999305", // Report In,
            "694094213986789", // Device Model
            "694094213986791", // OS
            "308614635924716", // Type
        };
        public static Dictionary<string, string> REPORT_TYPE = new Dictionary<string, string>{
            { "bug", "308614635924717" },
            { "en",  "308614635924718" }
        };

        private void Awake()
        {
            TextAsset textAsset;
#if UNITY_EDITOR
            textAsset = AssetBundleManager.LoadAsset<TextAsset>("testsuite", "EditorSettings");
            if (textAsset != null)
                id = SlotSimpleJson.DeserializeObject<TestSuiteEditorSettings>(textAsset.text).id;
#endif
            var contentInfoList = ContentsManifest.GetContentInfos();
            foreach (var info in contentInfoList)
            {
                contentInfos[info.gameTitle] = info;
            }

            var stage = GetStage();
            TEST_REPORT_STATISTICS_NAME = new string[7];
            TEST_REPORT_STATISTICS_NAME[0] = "TestContentHourly" + stage;
            TEST_REPORT_STATISTICS_NAME[1] = "TestContentHourlyFailure" + stage;
            TEST_REPORT_STATISTICS_NAME[2] = "TestContentDaily" + stage;
            TEST_REPORT_STATISTICS_NAME[3] = "TestContentDailyFailure" + stage;
            TEST_REPORT_STATISTICS_NAME[4] = "TestContentWeekly" + stage;
            TEST_REPORT_STATISTICS_NAME[5] = "TestContentWeeklyFailure" + stage;
            TEST_REPORT_STATISTICS_NAME[6] = "TestContentWeight" + stage;

            delegates[ContentEvent.ON_BEGIN_GAME_EVENT] = OnBeginGame;
            delegates[ContentEvent.ON_END_GAME_EVENT] = OnEndGame;
            delegates["OpenFirstLogin"] = OpenFirstLogin;
            delegates["OpenConsole"] = OpenConsole;
            delegates["OpenEnterGame"] = OpenEnterGame;
            delegates["OpenDebugSpin"] = OpenDebugSpin;
            delegates["OpenCustomReport"] = OpenCustomReport;

            MessageDispatcher.Register("OnMetaUIEvent", OnMetaUIEvent);
            MessageDispatcher.Register(ContentEvent.ON_CONTENT_EVENT, OnContentEvent);
            MessageDispatcher.Register("OnTestSuite", OnTestSuite);

            Application.logMessageReceived += HandleLog;

            ToggleVersionInfo();
            ToggleErrorLogInfo();
            StartCoroutine(FPS());
        }

        protected override void OnDestroy()
        {
            MessageDispatcher.UnRegister("OnMetaUIEvent", OnMetaUIEvent);
            MessageDispatcher.UnRegister(ContentEvent.ON_CONTENT_EVENT, OnContentEvent);
            MessageDispatcher.UnRegister("OnTestSuite", OnTestSuite);

            Application.logMessageReceived -= HandleLog;

            base.OnDestroy();
        }

        public string GetStage()
        {
#if BUILD_QA
            return string.Empty;
#else
            return "DEV";
#endif
        }

        private void OnMetaUIEvent(EventData eventData)
        {
            MessageDispatcher.EventDelegate del;
            if (delegates.TryGetValue(eventData.name, out del))
                del.Invoke(eventData);
        }

        private void OnContentEvent(EventData eventData)
        {
            MessageDispatcher.EventDelegate del;
            if (delegates.TryGetValue(eventData.name, out del))
                del.Invoke(eventData);
        }

        private void OnTestSuite(EventData eventData)
        {
            MessageDispatcher.EventDelegate del;
            if (delegates.TryGetValue(eventData.name, out del))
                del.Invoke(eventData);
        }

        private void OpenFirstLogin(EventData eventData)
        {
            string forceGuestMode = TestSuiteServer.GetUserData("forceGuestMode");
            if (!string.IsNullOrEmpty(forceGuestMode) && Convert.ToBoolean(forceGuestMode))
                GraphOwner.SendGlobalEvent("OnClickGuest");
        }

        private void OnBeginGame(EventData eventData)
        {
            var game = ((EventData<Blackboard>)eventData).value;
            //TestSuiteServer.UpdateUserData(new Dictionary<string, string>
            //{
            //    { "lastGame", game.GetValue<string>("gameTitle") }
            //}, null, null);

            //TestSuiteManager.Instance.DebugParam = string.Empty;
        }

        private void OnEndGame(EventData eventData)
        {

        }

        public void OpenConsole(EventData eventData)
        {
#if !NEW_NET
            if (testCaseRunner != null)
                testCaseRunner.Stop();
#endif
            GameObject.Destroy(currentPopup);
            currentPopup = LoadGameObject("Console Popup");
            LoadGameObject("Custom Editor", currentPopup.transform);
        }

        public void OpenEnterGame(EventData eventData)
        {
            OpenEnterGameApp(null);
        }

        public void OpenDebugSpin(EventData eventData)
        {
#if !NEW_NET
            if (testCaseRunner != null)
                testCaseRunner.Stop();

            if (DebugSpinRunner != null)
                DebugSpinRunner.Stop();
#endif
            GameObject.Destroy(currentPopup);
            currentPopup = LoadGameObject("DebugSpin");
        }

        public void OpenCustomReport(EventData eventData)
        {
            GameObject.Destroy(currentPopup);
            currentPopup = LoadGameObject("CustomReport");
        }

        public void LoginResult()
        {
            ApplicationSettings.Instance.logFilter = TestSuiteServer.GetLogFilter();
            enterGame = TestSuiteServer.HasApplication("EnterGame");

            {
                var textAsset = AssetBundleManager.LoadAsset<TextAsset>("testsuite", "TestCases");
                testCases = SlotSimpleJson.DeserializeObject<Dictionary<string, TestCase>>(textAsset.text);
            }

            {
                var textAsset = AssetBundleManager.LoadAsset<TextAsset>("testsuite", "TestSuites");
                testSuites = SlotSimpleJson.DeserializeObject<Dictionary<string, TestSuite>>(textAsset.text);
            }
        }

        public void OpenEnterGameApp(Action endAction)
        {
            if (enterGame || endAction == null)
            {
                GameObject.Destroy(currentPopup);
                currentPopup = LoadGameObject("EnterGame");
                currentPopup.GetComponent<ITaskCallback>().endAction = endAction;
            }
            else
            {
                endAction();
            }
        }

        public void Run(List<DebugSequence> DebugSequenceList)
        {
#if !NEW_NET
            var go = LoadGameObject("DebugSpin Runner");
            DebugSpinRunner = go.GetComponent<TestSuiteDebugSpinRunner>();
            DebugSpinRunner.Run(DebugSequenceList);
#endif
        }

        public void Run(string caseId)
        {
#if !NEW_NET
            var go = LoadGameObject("TestCase Runner");
            testCaseRunner = go.GetComponent<TestCaseRunner>();
            testCaseRunner.Run(GetTestCase(caseId));
#endif
        }

        public TestCase GetTestCase(string caseId)
        {
            TestCase tc;
            if (testCases.TryGetValue(caseId, out tc))
                return tc;

            Debug.LogError("[TestSuite] Cound not be found " + caseId);
            return null;
        }

        public TestSuite GetTestSuite(string suiteId)
        {
            TestSuite ts;
            if (testSuites.TryGetValue(suiteId, out ts))
                return ts;

            Debug.LogError("[TestSuite] Could not be found " + suiteId);
            return null;
        }

        public void Report(Dictionary<string, object> report)
        {
#if !NEW_NET
            if (testCaseRunner != null)
                testCaseRunner.Stop();

            StartCoroutine(ReportCo(report));
#endif
        }

        private IEnumerator LoadDebugSpinParamList(int gameId)
        {
            string baseUrl = "https://test-manager.bagelgames.com/api/debugSpin/list/";
            var www = UnityWebRequest.Get(baseUrl + gameId.ToString());

            yield return www.SendWebRequest();

            if (string.IsNullOrEmpty(www.error))
            {
                debugDataList = SlotSimpleJson.DeserializeObject<DebugSpinParam>(www.downloadHandler.text);

                List<DebugSpin> temp = null;
                if (debugDataList.data != null)
                {
                    temp = debugDataList.data.Select(
                                debug =>
                                {
                                    return new DebugSpin()
                                    {
                                        code = debug.id,
                                        title = debug.name,
                                        tag = debug.tag,
                                        description = debug.description,
                                        DebugSequenceList = debug.info.sequenceList,
                                    };
                                }
                            ).ToList();

                    if (debugSpinInfoDict.ContainsKey(gameId))
                        debugSpinInfoDict.Remove(gameId);
                }

                if (!debugSpinInfoDict.ContainsKey(gameId))
                    debugSpinInfoDict.Add(gameId, temp);
            }
            else
            {
                Debug.LogError("[Testsuite - Error!] Failed to load debug spin info list");
            }

        }

        public IEnumerator GetDebugSpins(int gameId, Action<List<DebugSpin>> callback)
        {
            List<DebugSpin> ret = null;

            do {
                debugSpinInfoDict.TryGetValue(gameId, out ret);

                if (ret == null)
                    yield return StartCoroutine(LoadDebugSpinParamList(gameId));
            }
            while(ret == null);

            callback(ret);
        }

        public IEnumerator ReportCo(Dictionary<string, object> report)
        {
            if (report["screenshot"] == null)
                yield return StartCoroutine(ScreenShotCo(report));

            yield return StartCoroutine(AsanaHttp.Instance.Tasks(GetAccessToken(), report));

            string quit = TestSuiteServer.GetUserData("quit");
            if (!string.IsNullOrEmpty(quit) && Convert.ToBoolean(quit))
                Application.Quit();
        }

        public IEnumerator ScreenShotCo(Dictionary<string, object> report)
        {
            report["screenshot"] = "screenshot.png";
            yield return StartCoroutine(ScreenCapture.Instance.CaptureCo(new ScreenCapture.CaptureParams
                {
                    path = Application.temporaryCachePath + "/screenshot.png",
                    quality = 0,
                    scaleType = ScreenCapture.CaptureParams.ScaleType.FixedHeight,
                    scaleFactor = 320f,
                    destroyTexture = true
                }, null));
        }

        public bool IsIgnoreReport(string gameTitle)
        {
            ContentInfo info;
            if (contentInfos.TryGetValue(gameTitle, out info))
                return info.ignore;
            return true;
        }

        public void UpdateReportHeader(Dictionary<string, object> report)
        {
#if !NEW_NET
            report["workspace"] = WORKSPACE;
            if (!report.ContainsKey("projects"))
                report["projects"] =  string.IsNullOrEmpty(GetStage()) ? PROJECTS_BUG_REPORT_QA : PROJECTS_BUG_REPORT_DEV; // qa, dev

            var custom_fields = new Dictionary<string, string>();
            custom_fields[CUSTOM_FIELDS_CONTENT_BUG_REPORT[0]] = (string)report["content"];
            custom_fields[CUSTOM_FIELDS_CONTENT_BUG_REPORT[1]] = ApplicationSettings.Instance.clientVersion;
            custom_fields[CUSTOM_FIELDS_CONTENT_BUG_REPORT[2]] = SystemInfo.deviceModel;
            custom_fields[CUSTOM_FIELDS_CONTENT_BUG_REPORT[3]] = SystemInfo.operatingSystem;
            custom_fields[CUSTOM_FIELDS_CONTENT_BUG_REPORT[4]] = REPORT_TYPE[(string)report["type"]];
            report["custom_fields"] = custom_fields;

            if (testCaseRunner != null) {
                testCaseRunner.Stop();
                Debug.LogError("TestCaseRunner stopped since bug is detected");
            }

            Dictionary<string, object> reportDetails = (Dictionary<string, object>)report["details"];
            reportDetails["errorLogs"] = logs;
#endif
        }

        public void RefreshContentReport(bool fullReport = true)
        {
            var result = new Dictionary<string, TestSuiteReportResult>();
            StartCoroutine(RefreshContentReportCo(result, fullReport));
        }

        private IEnumerator RefreshContentReportCo(Dictionary<string, TestSuiteReportResult> result, bool fullReport = true)
        {
            int request = 0;
            if (fullReport)
            {
                for (int i = 0; i < 7; ++i)
                {
                    UpdateReportStatistics(result, TEST_REPORT_STATISTICS_NAME[i]);
                    ++request;
                }
            }
            else
            {
                UpdateReportStatistics(result, TEST_REPORT_STATISTICS_NAME[0]);
                UpdateReportStatistics(result, TEST_REPORT_STATISTICS_NAME[2]);
                UpdateReportStatistics(result, TEST_REPORT_STATISTICS_NAME[6]);
                request = 3;
            }

            while (result.Count < request)
            {
                yield return null;
            }

            MessageDispatcher.Dispatch("OnTestSuite", new EventData<Dictionary<string, TestSuiteReportResult>>("RefreshContentReport", result));
        }

        private void UpdateReportStatistics(Dictionary<string, TestSuiteReportResult> result, string statisticsName)
        {
            TestSuiteServer.GetTestSuiteReport(statisticsName,
            (response) =>
            {
                result[statisticsName] = response;
            }, null);
        }

        public void ContentBegin(Dictionary<string, object> parameter, Action callback)
        {
            parameter["stage"] = GetStage();

            TestSuiteServer.BeginGame(parameter, callback, null);
        }

        public void ContentEnd(Dictionary<string, object> parameter, Action callback)
        {
            parameter["stage"] = GetStage();

            TestSuiteServer.EndGame(parameter, callback, null);
        }

        public void ContentWeight(Dictionary<string, object> parameter)
        {
            parameter["stage"] = GetStage();

            TestSuiteServer.UpdateGameWeight(parameter, null, null);
        }

        public string GetAccessToken()
        {
            return "0/7f6e5a550dac90757215a3ea7430258b";
        }

        public string GetPlatform()
        {
            switch (Application.platform)
            {
            case RuntimePlatform.IPhonePlayer:
                return "236199784943687";
            case RuntimePlatform.Android:
                return "236199784943688";
            }

            return "247732496605193";//UnityEditor
        }

        void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception)
            {
                Sb.Length = 0;
                Sb.Append("[").Append(DateTime.Now.ToLongTimeString()).Append("] ")
                    .Append(type).Append(": ").Append(logString).Append("\n");
#if !BUILD_QA
                lastLog = Sb.ToString();

                if (showLogOnScreen)
                    errorLogDisplay.text = lastLog;
#endif
                Sb.Append(stackTrace).Append(StackTraceUtility.ExtractStackTrace());
                logString = Sb.ToString();
                logs.Add(logString);
                if (logs.Count > 20)
                {
                    logs.RemoveAt(0);
                }
            }
        }

        IEnumerator FPS()
        {
            while (true)
            {
                int lastFrameCount = Time.frameCount;
                float lastTime = Time.realtimeSinceStartup;
                yield return new WaitForSeconds(0.5f);
                float timeSpan = Time.realtimeSinceStartup - lastTime;
                int frameCount = Time.frameCount - lastFrameCount;

                if (versionType == 2)
                    version.text = Mathf.RoundToInt(frameCount / timeSpan).ToString();
            }
        }
#endif
            public void ClosePopup()
        {
#if DEV
            GameObject.Destroy(currentPopup);
#endif
        }

        public void ToggleVersionInfo()
        {
#if DEV
            if (++versionType == 3)
                versionType = 0;

            if (versionType == 1)
                version.text = string.Format("{0}{1}", ApplicationSettings.GetApplicationStage()[0], ApplicationSettings.Instance.clientVersion);
            else
                version.text = string.Empty;
#endif
        }

        public void ToggleErrorLogInfo()
        {
#if (DEV && !BUILD_QA)
            showLogOnScreen = !showLogOnScreen;

            if (showLogOnScreen)
                errorLogDisplay.text = lastLog;
            else
                errorLogDisplay.text = string.Empty;
#endif
        }

        public void ToggleGuide()
        {
            guide.SetActive(!guide.activeSelf);
        }

        public GameObject LoadGameObject(string assetName, Transform target = null)
        {
            var prefab = AssetBundleManager.LoadAsset<GameObject>("testsuite", assetName);
            if (prefab == null)
                return null;

            var go = GameObject.Instantiate(prefab) as GameObject;
            go.name = assetName;
            go.transform.SetParent(target != null ? target : panel, false);
            return go;
        }
    }
}
