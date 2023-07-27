using SlotMaker;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using UnityEngine;
using System.Collections;

namespace BagelCode
{
    public class InboxCellDataGameCompensation : InboxCellData
    {
        public InboxCellDataGameCompensation(InboxCellController _owner, Blackboard _inboxInfo)
            : base(_owner, _inboxInfo)
        {
            disappearType = 1;
            gameId = BlackboardUtils.FindVariable<int>(inboxInfo, "gameId")?.value ?? -1;
        }

        public override IEnumerator OnAcceptSuccessCoroutine()
        {
            GSManager.Instance.GetHandler("UI_Coin_Add").Play();

            return base.OnAcceptSuccessCoroutine();
        }

        protected override string GetInternalText()
        {
            long coin = inboxInfo.GetValue<long>("credit");
            return StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_COMPENSATION", title, message, coin);
        }

        protected override string GetButtonText()
        {
            return StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_BUTTON_ACCEPT");
        }

        protected override IconType GetIconType()
        {
            return IconType.GAME_THUMBNAIL;
        }
    }
}
