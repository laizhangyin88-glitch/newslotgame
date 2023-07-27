using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class TotalResultPopupControllerScratcher : TotalResultPopupController
    {
        public List<string> scratcherNameList = new List<string>();

        public List<int> winTypeList = new List<int>();
        public List<bool> isJackpotList = new List<bool>();

        protected override string GetTitleText()
            => StringTableUtils.GetString(GLOBAL, "COLLECTING_GAME_TOTAL_RESULT_TITLE");

        protected override string GetInfoText()
            => StringTableUtils.GetString(GLOBAL, "COLLECTING_GAME_TOTAL_RESULT_INFO", prizeList.Count);

        protected override string GetTypeText(int i)
            => StringTableUtils.GetString(GLOBAL, "COLLECTING_GAME_TOTAL_RESULT_NAME", scratcherNameList[i]);

        protected override string GetWinText(int i)
        {
            int winType = winTypeList[i];
            if ((ScratcherBigWinType)winType >= ScratcherBigWinType.BIG)
            {
                return StringTableUtils.GetString(GLOBAL, string.Format("COLLECTING_GAME_RESULT_{0}_WIN", ((ScratcherBigWinType)winTypeList[i]).ToString()));
            }
            return "";
        }

        protected override string GetPrizeText(int i)
        {
            long prize = prizeList[i];
            if(prize == 0L) return StringTableUtils.GetString(GLOBAL, "COLLECTING_GAME_LOSE_TEXT");
            else return StringTableUtils.GetString(GLOBAL, "COMMA_STYLE_COIN", prize);
        }

        protected override string GetCollectButtonText()
            => StringTableUtils.GetString(GLOBAL, "COLLECTING_GAME_COLLECT_BUTTON_TEXT");

        protected override string GetOkayButtonText()
            => StringTableUtils.GetString(GLOBAL, "COLLECTING_GAME_OKAY_BUTTON_TEXT");

        protected override string GetTotalText()
            => StringTableUtils.GetString(GLOBAL, "COLLECTING_GAME_TOTAL_RESULT_TOTAL");

        protected override void OnClose()
        {
            var caller = bb.GetValue<GameObject>("caller");
            EventSender.SendEvent(caller, "OnRefresh");
        }

        protected override void GetVariables()
        {
            prizeList = bb.GetValue<List<long>>("_prizeList");
            scratcherNameList = bb.GetVariable<List<string>>("_scratcherNameList")?.value;
            winTypeList = bb.GetVariable<List<int>>("_winTypeList")?.value;
        }
    }
}
