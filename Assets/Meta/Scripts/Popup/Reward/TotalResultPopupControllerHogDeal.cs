using System.Collections.Generic;
using BagelCode.ClientModels;
using SlotMaker;

namespace BagelCode
{
    public class TotalResultPopupControllerHogDeal : TotalResultPopupController
    {
        private List<HogDealJackpotType> jackpotTypeList = new List<HogDealJackpotType>();

        protected override string GetTitleText() =>
            StringTableUtils.GetString(GLOBAL, "POPUP_HOG_DEAL_TOTAL_RESULT_TITLE");

        protected override string GetInfoText() => 
            StringTableUtils.GetString(GLOBAL, "POPUP_HOG_DEAL_TOTAL_RESULT_INFO", prizeList.Count);

        protected override string GetTypeText(int i)
            => StringTableUtils.GetString(GLOBAL, "POPUP_HOG_DEAL_TOTAL_RESULT_ROUND", i + 1);

        protected override string GetWinText(int i)
        {
            HogDealJackpotType jackpotType = jackpotTypeList[i];
            if (jackpotType != HogDealJackpotType.UNKNOWN && jackpotType != HogDealJackpotType.NONE)
            {
                return StringTableUtils.GetString(GLOBAL, string.Format("POPUP_HOG_DEAL_TOTAL_RESULT_{0}_WIN", jackpotType.ToString()));
            }
            return "";
        }

        protected override string GetPrizeText(int i)
        {
            long prize = prizeList[i];
            return StringTableUtils.GetString(GLOBAL, "COMMA_STYLE_COIN", prize);
        }

        protected override string GetCollectButtonText()
            => StringTableUtils.GetString(GLOBAL, "POPUP_HOG_DEAL_TOTAL_RESULT_COLLECT");

        protected override string GetOkayButtonText()
            => StringTableUtils.GetString(GLOBAL, "POPUP_HOG_DEAL_TOTAL_RESULT_OKAY");

        protected override string GetTotalText()
            => StringTableUtils.GetString(GLOBAL, "POPUP_HOG_DEAL_TOTAL_RESULT_TOTAL");

        protected override void OnClose()
        {
            EventSender.SendCalleeCallback(gameObject);
        }

        protected override void GetVariables()
        {
            prizeList = bb.GetValue<List<long>>("prizeList");
            jackpotTypeList = bb.GetVariable<List<HogDealJackpotType>>("jackpotTypeList")?.value;
        }
    }
}
