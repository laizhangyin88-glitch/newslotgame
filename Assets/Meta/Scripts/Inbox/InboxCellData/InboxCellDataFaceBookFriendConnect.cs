using SlotMaker;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using UnityEngine;
using System.Collections;

namespace BagelCode
{
    public class InboxCellDataFaceBookFriendConnect : InboxCellData
    {
        public InboxCellDataFaceBookFriendConnect(InboxCellController _owner, Blackboard _inboxInfo)
            : base(_owner, _inboxInfo)
        {
            disappearType = 1;
        }

        public override IEnumerator OnAcceptSuccessCoroutine()
        {
            GSManager.Instance.GetHandler("UI_Coin_Add").Play();

            return base.OnAcceptSuccessCoroutine();
        }

        protected override string GetInternalText()
        {
            long coin = inboxInfo.GetValue<long>("credit");
            string userName = inboxInfo.GetValue<string>("userName");
            return StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_FACEBOOK_FRIEND", userName, coin);
        }

        protected override string GetButtonText()
        {
            return StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_BUTTON_CLAIM");
        }

        protected override IconType GetIconType()
        {
            return IconType.FACEBOOK_FRIEND_CONNECT;
        }
    }
}
