using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using NodeCanvas.Framework;

namespace SlotMaker.TestSuite
{
    public class TestSuiteContentCoinMismatchDetector : MonoBehaviour 
    {
        public string message;

#if DEV && !NEW_NET
        enum ClaimType
        {
            UNKNOWN = -666,
            ALWAYS = 1,
            OPTIONAL = 2,
            NONE = 3
        }

        private MessageDelegates delegates;

        private void Awake()
        {
            delegates = new MessageDelegates 
            (
                new Dictionary<string, MessageDispatcher.EventDelegate>
                {
                    { "EndSpin",  EndSpin  },
                    { "EndBonus", EndBonus }
                }
            );
        }

        private void OnEnable()
        {
            MessageDispatcher.Register("OnContentEvent", delegates.Delegate);
        }

        private void OnDisable()
        {
            MessageDispatcher.UnRegister("OnContentEvent", delegates.Delegate);
        }

        private void EndSpin(EventData eventData)
        {
            var spin = ((EventData<Blackboard>)eventData).value;
            var response = spin.GetVariable<Blackboard>("response");
            if (response != null)
            {
                var serverResult = response.value.GetVariable<Blackboard>("result");
                if (serverResult == null) return;

                long localCredit = spin.GetValue<long>("singleCredit");
                long serverCredit = serverResult.value.GetValue<long>("earnCredit");
                if (localCredit != serverCredit)
                {
                    ReportCreditMismatch(
                        ContentBlackboard.Get().GetValue<Blackboard>("game").GetValue<string>("gameTitle"),
                        "EndSpin", localCredit, serverCredit, spin
                    );
                }
            }
        }

        private void EndBonus(EventData eventData)
        {
            var bonus = ((EventData<Blackboard>)eventData).value;
            var response = bonus.GetVariable<Blackboard>("response");
            if (response != null)
            {
                long serverCredit = response.value.GetValue<long>("earnCredit");
                long localCredit = bonus.GetValue<long>("singleCredit");

                var initialSpinCount = BlackboardUtils.FindVariable<int>(bonus, "initialSpinCount");
                if (initialSpinCount != null)
                {
                    var claimType = (ClaimType)(int)response.value.GetVariable("claimType").value;
                    if (claimType == ClaimType.NONE && serverCredit > 0 ||
                        claimType == ClaimType.ALWAYS && localCredit == 0)
                        return;
                }

                if (localCredit != serverCredit)
                {
                    ReportCreditMismatch(
                        ContentBlackboard.Get().GetValue<Blackboard>("game").GetValue<string>("gameTitle"),
                        "EndBonus", localCredit, serverCredit
                    );
                }
            }
        }

        private void ReportCreditMismatch(string content, string where, long localCredit, long serverCredit, Blackboard spin = null)
        {
            var spinResponse = (spin != null) ? spin.GetValue<Blackboard>("response") : BlackboardUtils.FindValue<Blackboard>("./spin/response");
            var debugSpin = BlackboardUtils.FindVariable(spinResponse, "debugSpin");
            string debugSpinInfo = (debugSpin != null) ? string.Format("(DebugSpin: {0})", debugSpin.value) : "";
            message = string.Format("{0}: Local credit({1}) != server credit({2}) {3}", where, localCredit, serverCredit, debugSpinInfo);

            MessageDispatcher.Dispatch("OnMetaUIEvent", new EventData<Blackboard>("OnInGameChatEvent", GetComponent<Blackboard>()));
            MessageDispatcher.Dispatch("OnMetaUIEvent", new EventData<string>("OnChatSend", message));

            if (TestSuiteManager.Instance.IsIgnoreReport(content))
                return;

            var report = new Dictionary<string, object>();
            report["name"] = "Credit Mismatch";
            report["notes"] = message;
            report["screenshot"] = null;
            report["content"] = content;
            report["type"] = "bug";

            var reportDetails = new Dictionary<string, object>();
            reportDetails["gameTitle"] = content;
            reportDetails["message"] = message;
            reportDetails["location"] = where;
            if (spin != null)
            {
                if (spin.GetVariable("deck") != null)
                {
                    reportDetails["deck"] = spin.GetValue<Deck>("deck");
                    reportDetails["winList"] = spin.GetValue<List<SymbolWin>>("winList");
                }
            }
            reportDetails["dump"] = BlackboardSerializer.Deserialize(ContentBlackboard.Get());
            report["details"] = reportDetails;

            TestSuiteManager.Instance.UpdateReportHeader(report);
            TestSuiteManager.Instance.Report(report);
        }
#endif
    }
}
