using System.Collections.Generic;
using SlotMaker;

namespace BagelCode
{
    public class TotalResultPopupControllerDailySpin : TotalResultPopupController
    {
        public List<int> winTypeList = new List<int>();
        public List<bool> isJackpotList = new List<bool>();

        protected override string GetTitleText()
            => StringTableUtils.GetString(GLOBAL, "POPUP_DAILY_SPINS_TOTAL_RESULT_TITLE");

        protected override string GetInfoText()
            => StringTableUtils.GetString(GLOBAL, "POPUP_DAILY_SPINS_TOTAL_RESULT_INFO", prizeList.Count);

        protected override string GetTypeText(int i)
            => StringTableUtils.GetString(GLOBAL, "POPUP_DAILY_SPINS_TOTAL_RESULT_TYPE");

        protected override string GetWinText(int i)
        {
            bool isJackpot = isJackpotList[i];
            if (isJackpot)
            {
                return StringTableUtils.GetString(GLOBAL, "POPUP_DAILY_SPINS_TOTAL_RESULT_JACKPOT_WIN");
            }
            return "";
        }

        protected override string GetPrizeText(int i)
        {
            long prize = prizeList[i];
            return StringTableUtils.GetString(GLOBAL, "COMMA_STYLE_COIN", prize);
        }

        protected override string GetCollectButtonText()
            => StringTableUtils.GetString(GLOBAL, "POPUP_DAILY_SPINS_TOTAL_RESULT_COLLECT");

        protected override string GetOkayButtonText()
            => StringTableUtils.GetString(GLOBAL, "POPUP_DAILY_SPINS_TOTAL_RESULT_OKAY");

        protected override string GetTotalText()
            => StringTableUtils.GetString(GLOBAL, "POPUP_DAILY_SPINS_TOTAL_RESULT_TOTAL");

        protected override void OnClose()
        {
            EventSender.SendCalleeCallback(gameObject);
        }

        protected override void GetVariables()
        {
            prizeList = bb.GetValue<List<long>>("_prizeList");
            winTypeList = bb.GetVariable<List<int>>("_winTypeList")?.value;
            isJackpotList = bb.GetVariable<List<bool>>("_isJackpotList")?.value;
        }
    }
}
