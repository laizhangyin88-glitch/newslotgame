using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ParadoxNotion;
using NodeCanvas.Framework;
using SlotMaker;
using SlotMaker.TestSuite;

namespace BagelCode
{
    public class SC_SpinNew : TestSuiteRunner
    {
#if DEV && !NEW_NET
        public DebugSpin currentDebug { get; set; }

        private ContentTestInfo testInfo;

        public int spinIndex { get; set; }
        public int spinCount { get; set; }
        public int seqIndex { get; set; }
        public int seqCount { get; set; }
        private int waitingTime;
        private int timeout;
        private bool debugSpin;
        private string location;

        private const int SPIN_BUTTON_INTERVAL = 1;
        private const int EVENT_SOLVING_INTERVAL = 2;
        private const int TIMEOUT_INTERVAL = 10;

        private const string NORMAL_SPIN = "spin";
        private const string FREE_SPIN_TRIGGER = "free_spin_trigger";
        private const string FREE_SPIN = "free_spin";

        private Variable<bool> autoSpin;

        private MessageDelegates delegates;

        private void Awake()
        {
            delegates = new MessageDelegates
            (
                new Dictionary<string, MessageDispatcher.EventDelegate>
                {
                    { "EndGame",    EndGame    },
                    { "BeginTurn",  BeginTurn  },
                    { "EndTurn",    EndTurn    },
                    { "BeginSpin",  BeginSpin  },
                    { "EndSpin",    EndSpin    },
                    { "BeginBonus", BeginBonus }
                }
            );

            autoSpin = BlackboardUtils.FindVariable<bool>("./autoSpin");
        }

        private void OnEnable()
        {
            MessageDispatcher.Register("OnContentEvent", delegates.Delegate);
        }

        private void OnDisable()
        {
            MessageDispatcher.UnRegister("OnContentEvent", delegates.Delegate);
        }

        public override void Run(TestCaseRunner testCaseRunner, SlotMaker.TestSuite.TestSuite testSuite)
        {
            base.Run(testCaseRunner, testSuite);

            testInfo = (ContentTestInfo)testCase.customData["testInfo"];
            debugSpin = (bool)testCase.customData["debugSpin"];
            if (!debugSpin || testInfo.debugSpins.Count == 0)
            {
                object value;
                spinCount = (testCase.customData.TryGetValue("spinCount", out value)) ? Convert.ToInt32(value) : 1;
                seqCount = 1;
                TestSuiteManager.Instance.DebugParam = string.Empty;
                debugSpin = false;
            }
            else
            {
                spinCount = testInfo.debugSpins.Count;
                seqCount = testInfo.debugSpins[spinIndex].DebugSequenceList.Count;
                currentDebug = testInfo.debugSpins[0];
                TestSuiteManager.Instance.DebugParam = testInfo.debugSpins[0].DebugSequenceList[0].debugParam;
            }
            timeout = Convert.ToInt32(testCase.customData["timeout"]);
            if (Convert.ToBoolean(testCase.customData["minbet"]))
                MessageDispatcher.Dispatch("OnCreditEvent", new EventData<int>("UpdateBetIndex", 0));
            if (Convert.ToBoolean(testCase.customData["autospin"]))
                autoSpin.value = true;
            else
                StartCoroutine(SpinButtonEventTrigger());

            StartCoroutine(EventSolver());
            StartCoroutine(TimeoutChecker());

            ShowLog();
        }

        public void CompleteSequence()
        {
            seqIndex = 0;

            if (++spinIndex >= spinCount)
            {
                testCaseRunner.Complete(testSuite);
                Stop();
                return;
            }

            if (debugSpin)
            {
                seqCount = testInfo.debugSpins[spinIndex].DebugSequenceList.Count;
                currentDebug = testInfo.debugSpins[spinIndex];
                TestSuiteManager.Instance.DebugParam = currentDebug.DebugSequenceList[seqIndex].debugParam;
            }
            ShowLog();
        }

        public override void Stop()
        {
            autoSpin.value = false;

            base.Stop();
        }

        private void EndGame(EventData eventData)
        {
            testCaseRunner.Stop();
        }

        private void BeginTurn(EventData eventData)
        {
            waitingTime = 0;
            location = "Turn";
        }

        private void EndTurn(EventData eventData)
        {
            if (seqIndex + 1 >= seqCount)
                CompleteSequence();
            else if (debugSpin && String.Equals(currentDebug.DebugSequenceList[seqIndex].type, NORMAL_SPIN))
                TestSuiteManager.Instance.DebugParam = currentDebug.DebugSequenceList[++seqIndex].debugParam;

            if (spinIndex < spinCount)
                ShowLog();
        }

        private void BeginSpin(EventData eventData)
        {
            waitingTime = 0;
            location = "Spin";
        }

        private void EndSpin(EventData eventData)
        {
            if (seqIndex + 1 >= seqCount)
            {
                // Do not complet sequence on end spin event
            }
            else if (debugSpin && String.Equals(currentDebug.DebugSequenceList[seqIndex].type, FREE_SPIN))
                TestSuiteManager.Instance.DebugParam = currentDebug.DebugSequenceList[++seqIndex].debugParam;

            if (spinIndex < spinCount)
                ShowLog();
        }

        private void BeginBonus(EventData eventData)
        {
            waitingTime = 0;
            location = "Bonus";

            if (seqIndex + 1 >= seqCount)
            {
                // Do not complet sequence on end spin event
            }
            else if (debugSpin && String.Equals(currentDebug.DebugSequenceList[seqIndex].type, FREE_SPIN_TRIGGER))
                TestSuiteManager.Instance.DebugParam = currentDebug.DebugSequenceList[++seqIndex].debugParam;

            if (spinIndex < spinCount)
                ShowLog();
        }

        private IEnumerator SpinButtonEventTrigger()
        {
            while (true)
            {
                yield return new WaitForSeconds(SPIN_BUTTON_INTERVAL);

                MessageDispatcher.Dispatch("OnSpinButtonEvent", new EventData("OnSpinButtonEvent"));
            }
        }

        private IEnumerator EventSolver()
        {
            while (true)
            {
                yield return new WaitForSeconds(EVENT_SOLVING_INTERVAL);

#if NEW_NET
                yield return null;
#else
                if (TestSuiteEventSolver.HasSolver("Meta"))
                {
                    TestSuiteEventSolver.Solve("Meta");
                    waitingTime = 0;
                }
                else if (TestSuiteEventSolver.HasSolver("Contents"))
                {
                    TestSuiteEventSolver.Solve("Contents");
                    waitingTime = 0;
                }
#endif
            }
        }

        private IEnumerator TimeoutChecker()
        {
            while (waitingTime < timeout)
            {
                yield return new WaitForSeconds(TIMEOUT_INTERVAL);

                waitingTime += TIMEOUT_INTERVAL;
            }

            string content = ContentBlackboard.Get().GetValue<Blackboard>("game").GetValue<string>("gameTitle");
            string message = string.Format("Timeout{0}: {1}/{2}", (debugSpin ? "(DebugSpin)" : ""), seqIndex, seqCount);

            var report = new Dictionary<string, object>();
            report["name"] = "Timeout";
            report["notes"] = message;
            report["screenshot"] = null;
            report["content"] = content;
            report["type"] = "bug";

            var reportDetails = new Dictionary<string, object>();
            reportDetails["gameTitle"] = content;
            reportDetails["message"] = message;
            reportDetails["location"] = location;
            reportDetails["dump"] = BlackboardSerializer.Deserialize(ContentBlackboard.Get());
            report["details"] = reportDetails;

            TestSuiteManager.Instance.UpdateReportHeader(report);
            TestSuiteManager.Instance.Report(report);
        }

        private void ShowLog()
        {
            if (!debugSpin)
                TestSuiteManager.Instance.tsDescription.text = string.Format("{0}/{1}", (spinIndex + 1), spinCount);
            else
                TestSuiteManager.Instance.tsDescription.text = testInfo.debugSpins[spinIndex].code.ToString();
        }
#endif
    }
}
