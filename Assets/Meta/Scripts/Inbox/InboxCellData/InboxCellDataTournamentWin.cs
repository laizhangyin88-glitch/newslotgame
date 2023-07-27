using SlotMaker;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using UnityEngine;
using System.Collections;

namespace BagelCode
{
    public class InboxCellDataTournamentWin : InboxCellData
    {
        public InboxCellDataTournamentWin(InboxCellController _owner, Blackboard _inboxInfo)
            : base(_owner, _inboxInfo)
        {
            iconText = inboxInfo.GetValue<int>("rank").ToString();
            disappearType = 1;
        }

        private string iconText;

        public override void InboxItemCheckType()
        {
            if(iconText != "0")
            {
                MetaContextElementUtils.SimpleSetText(
                    ownerRootElement, "Icon Area/Inbox Icon Tournament/Text Rank", iconText, FULL);
            }
        }

        public override IEnumerator OnAcceptSuccessCoroutine()
        {
            GSManager.Instance.GetHandler("UI_Coin_Add").Play();

            yield break;
        }

        protected override string GetInternalText()
        {
            int rank = inboxInfo.GetValue<int>("rank");
            int round = inboxInfo.GetValue<int>("serialWinCount");
            long actualCredit = inboxInfo.GetValue<long>("actualWinCredit");
            double serialWinBonus = inboxInfo.GetValue<double>("serialWinBonus");

            return StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_TOURNAMENT_WIN",
                TextDecoUtils.NumberToOrdinal(rank), (round + 1), serialWinBonus, actualCredit);
        }

        protected override string GetButtonText()
        {
            return StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_BUTTON_ACCEPT");
        }

        protected override IconType GetIconType()
        {
            return IconType.TOURNAMENT_WIN;
        }
    }
}
