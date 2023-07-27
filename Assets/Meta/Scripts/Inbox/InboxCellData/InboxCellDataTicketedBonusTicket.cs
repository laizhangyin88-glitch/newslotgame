using SlotMaker;
using NodeCanvas.Framework;
using System.Collections;

namespace BagelCode
{
    public class InboxCellDataTicketedBonusTicket : InboxCellData
    {
        public InboxCellDataTicketedBonusTicket(InboxCellController _owner, Blackboard _inboxInfo)
            : base(_owner, _inboxInfo)
        {
            gameId = inboxInfo.GetVariable<int>("gameId")?.value ?? -1;
        }

        public override IEnumerator InboxItemCheckExtraCoroutine()
        {
            gameId = inboxInfo.GetVariable<int>("gameId")?.value ?? -1;

            yield break;
        }

        public override IEnumerator OnAcceptSuccessCoroutine()
        {
            string fromType = "inbox";
            int ticketId = cell.responseInfo.GetValue<int>("ticketId");

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
    }
}
