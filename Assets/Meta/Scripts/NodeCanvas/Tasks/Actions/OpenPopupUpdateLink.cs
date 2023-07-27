using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using System.Collections;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Utils")]
    public class OpenPopupUpdateLink : ActionTask<Blackboard>
    {
        private const StringTable.StringTableType GLOBAL = StringTable.StringTableType.Global;

        public BBParameter<bool> isPersonal;
        public BBParameter<string> fromType;
        public BBParameter<int> gameId;
        public BBParameter<string> missionId;

        protected override string info
        {
            get { return "Open Popup Update Link"; }
        }

        protected override void OnExecute()
        {
            StartCoroutine(OpenOkayPopupCoroutine());
        }

        private IEnumerator OpenOkayPopupCoroutine()
        {
            var popupObj = MetaPopupUtils.OpenOKPopup();

            string text = StringTableUtils.GetString(GLOBAL, "POPUP_COMMON_NEED_TO_UPDATE");
            string buttonText = StringTableUtils.GetString(GLOBAL, "BUTTON_OK");
            MetaPopupUtils.SetCommonPopupData(popupObj, agent.transform,
                text, "", "OnOkay", buttonText,
                "", "", "OnClose", "", true, true, true, true, true);

            MetaObjectUtils.SetCalleeCaller(popupObj, agent.gameObject);

            var okayTrigger = new EventTrigger(agent.gameObject, "OnOkay");
            var closeTrigger = new EventTrigger(agent.gameObject, "OnClose");
            yield return new WaitUntilTrigger(okayTrigger, closeTrigger);

            if (okayTrigger.IsTrigger)
            {
                string appDownloadUrl = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "/appDownloadUrl").value;

                // Clicked Update
                if (appDownloadUrl != null)
                {
#if UNITY_WEBGL && !UNITY_EDITOR
                    NativeHelper.Instance.OpenUrl(appDownloadUrl);
#else
                    Application.OpenURL(appDownloadUrl);
#endif
                    EndAction();
                }
                else
                {
                    EndAction(false);
                }
            }
            else
            {
                EndAction();
            }

            var gameInfo = BlackboardQueryUtils.GetGameInfo(gameId.value);
            int minVersion = 0;
            if (gameInfo != null)
                minVersion = BlackboardUtils.FindVariable<int>(gameInfo, "minClientVersion")?.value ?? 0;

            int recentVersion = BlackboardUtils.FindVariable<int>(null, "/recentClientNumberVersion").value;
            if (fromType?.value == "challenge")
            {
                if (isPersonal?.value ?? false)
                {
                    long id = BlackboardUtils.FindVariable<long>(agent, missionId.value)?.value ?? 0L;
                    BICustomEvents.SendUpdateAppRecommandAEChallenge(recentVersion, minVersion, gameId.value, id);
                }
                else
                {
                    string id = BlackboardUtils.FindVariable<string>(agent, missionId.value)?.value;
                    BICustomEvents.SendUpdateAppRecommandAEClubChallenge(recentVersion, minVersion, gameId.value, id);
                }
            }
            else if(fromType?.value == "iam")
            {
                // iam
                var iamInfo = BlackboardUtils.FindVariable<Blackboard>(agent, "_iamInfo")?.value;
                if (iamInfo != null)
                {
                    int id = BlackboardUtils.FindVariable<int>(iamInfo, "id")?.value ?? 0;
                    BICustomEvents.SendUpdateAppRecommandAEIam(recentVersion, minVersion, gameId.value, id);
                    yield break;
                }

                // notice
                var noticeInfo = BlackboardUtils.FindVariable<Blackboard>(agent, "noticeInfo")?.value;
                if (noticeInfo != null)
                {
                    int id = BlackboardUtils.FindVariable<int>(noticeInfo, "id")?.value ?? 0;
                    BICustomEvents.SendUpdateAppRecommandAEIam(recentVersion, minVersion, gameId.value, id);
                    yield break;
                }

                // banner
                var bannerInfo = BlackboardUtils.FindVariable<Blackboard>(agent, "bannerInfo")?.value;
                if (bannerInfo != null)
                {
                    string id = BlackboardUtils.FindVariable<string>(bannerInfo, "id")?.value ?? "0";
                    int iid = int.Parse(id);
                    BICustomEvents.SendUpdateAppRecommandAEIam(recentVersion, minVersion, gameId.value, iid);
                    yield break;
                }
            }
            else
            {
                BICustomEvents.SendUpdateAppRecommandAE(recentVersion); // send bi event
            }
        }
    }
}
