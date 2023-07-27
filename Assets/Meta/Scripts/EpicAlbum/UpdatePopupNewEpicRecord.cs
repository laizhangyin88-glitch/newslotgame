using System;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.EpicAlbum
{
    [Category("★ BagelCode/EpicAlbum")]
    public class UpdatePopupNewEpicRecord : ActionTask<ContextElement>
    {
        public BBParameter<long> earnCredit;

        private ContextElement gaugeElement;
        private int prevStarCount;

        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;
        
        protected override string info
        {
            get { return "Update Popup New Epic Record"; }
        }

        protected override void OnExecute()
        {
            var agentElement = agent.gameObject.GetComponent<ContextElement>();
            agentElement.UpdateContext();

            var gameId = BlackboardUtils.GetOrCreateVariable<int>(null, "./game/gameId");
            bool newEpicRecord = BlackboardQueryUtils.CheckIfNewEpicRecord(gameId.value, earnCredit.value);

            var titleText = StringTableUtils.GetString(tableType, newEpicRecord ? "POPUP_EPIC_RECORD_NEW_TITLE_TEXT" : "POPUP_EPIC_RECORD_DEFAULT_TITLE_TEXT");
            MetaContextElementUtils.SimpleSetText(agentElement, "Title Area/Title Text", titleText, ContextSearchingType.FullNameSearch);
            MetaContextElementUtils.SimpleSetText(agentElement, "Title Area/Text", StringTableUtils.GetString(tableType, "POPUP_EPIC_RECORD_WIN_COUNT_TEXT"), ContextSearchingType.FullNameSearch);
            MetaContextElementUtils.SimpleSetText(agentElement, "Sub Text", StringTableUtils.GetString(tableType, "POPUP_EPIC_RECORD_BOTTOM_TEXT"), ContextSearchingType.ChildrenSearch);
            

            var maxCoin = BlackboardUtils.FindVariable<long>(null, "/values/misc/FACEBOOK_SHARE_REWARD_MAX_CREDIT");
            var buttonText = StringTableUtils.GetString(tableType, "BUTTON_SHARE_GET_MYSTERY_NEW", maxCoin.value);
            MetaContextElementUtils.SimpleSetText(agentElement, "Button Share/Text", buttonText, ContextSearchingType.FullNameSearch);

            DateTime dateTime = TimeUtils.GetCurrentDateTime();
            string dateFormat = StringTableUtils.GetString(StringTable.StringTableType.Global, "EPIC_ALBUM_DETAIL_POPUP_DATE_FORMAT");
            string dateString = string.Format("{0}", dateTime.ToString(dateFormat));
            MetaContextElementUtils.SimpleSetText(agentElement, "Day Tag Base/Text", dateString, ContextSearchingType.FullNameSearch);

            Blackboard woeInfo = gameId.value != -1 ? BlackboardQueryUtils.GetWOEInfo(gameId.value) : null;
            if(woeInfo != null)
            {
                Debug.LogError(woeInfo.GetValue<int>("epicWinCount"));
                Debug.LogError(woeInfo.GetValue<int>("starCount"));
                Debug.LogError(woeInfo.GetValue<int>("nextRequiredEpicWinCount"));
                prevStarCount = woeInfo.GetValue<int>("starCount");
            }

            gaugeElement = ContextUtils.FindElement(agentElement, "Title Area/Gauge", ContextSearchingType.FullNameSearch);
            MetaContextElementUtils.SetFloatProperty(gaugeElement, 1f);

            EndAction();
        }
    }
}