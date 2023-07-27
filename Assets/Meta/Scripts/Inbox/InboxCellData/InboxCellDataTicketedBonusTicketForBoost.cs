using SlotMaker;
using NodeCanvas.Framework;
using System.Collections;
using BagelCode;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class InboxCellDataTicketedBonusTicketForBoost : InboxCellData
    {
        public InboxCellDataTicketedBonusTicketForBoost(InboxCellController _owner, Blackboard _inboxInfo)
            : base(_owner, _inboxInfo)
        {
            gameId = BlackboardUtils.FindVariable<int>(inboxInfo, "reward/gameId")?.value ?? -1;
        }

        public override IEnumerator InboxItemCheckExtraCoroutine()
        {
            gameId = BlackboardUtils.FindVariable<int>(inboxInfo, "reward/gameId")?.value ?? -1;

            yield break;
        }

        public override IEnumerator OnAcceptSuccessCoroutine()
        {
            string fromType = "inbox";
            int ticketId = inboxInfo.GetValue<int>("ticketId");

            BlackboardQueryUtils.SetEnterGameInfo(
                gameId,
                "EnterGame",
                fromType,
                "",
                0,
                "",
                false,
                ticketId,
                null);

            EventSender.SendGlobalEvent("OnEnterGame");

            yield break;
        }

        protected override string GetButtonText()
        {
            return StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_BUTTON_PLAY");
        }

        protected override IconType GetIconType()
        {
            return IconType.GAME_THUMBNAIL;
        }

        protected override string GetInternalText()
        {
            //RewardCauseType rewardCauseType = inboxInfo.GetValue<RewardCauseType>("rewardCauseType"); // buy : TICKETED_BONUS_TICKET_DEAL
            bonusTag = BlackboardUtils.FindValue<BonusTag>(inboxInfo, "reward/tag");
            gameId = BlackboardUtils.FindValue<int>(inboxInfo, "reward/gameId");
            long baseBet = BlackboardUtils.FindValue<long>(inboxInfo, "reward/baseBet");
            long extraBet = BlackboardUtils.FindValue<long>(inboxInfo, "reward/extraBet");

            string gameTitleFormat = string.Format("GAME_TITLE_{0}", gameId.ToString());
            string gameTitle = StringTableUtils.GetString(GLOBAL, gameTitleFormat);

            string rewardText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_MESSAGE", title, message);
            TextDecoUtils.ConvertStringFormat(ref rewardText, "{bet}", TextDecoUtils.ConvertCoinStyleText(baseBet + extraBet));
            TextDecoUtils.ConvertStringFormat(ref rewardText, "{game_name}", gameTitle);

            var userName = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "me/name");
            if (userName != null && !string.IsNullOrEmpty(userName.value))
                TextDecoUtils.ConvertStringFormat(ref rewardText, "{name}", userName.value);

            return rewardText;
        }
    }
}
