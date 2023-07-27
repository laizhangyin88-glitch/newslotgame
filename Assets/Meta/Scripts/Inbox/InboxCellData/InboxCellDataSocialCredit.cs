using SlotMaker;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using UnityEngine;
using System.Collections;

namespace BagelCode
{
    public class InboxCellDataSocialCredit : InboxCellData
    {
        public InboxCellDataSocialCredit(InboxCellController _owner, Blackboard _inboxInfo)
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
            return StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_COIN", title, message, FreebieLevelUtils.GetLevelMultiplierNumeratorValue(coin, FreebieLevelUtils.FreebieType.FRIENDS_DEAL_BONUS));
        }

        protected override string GetButtonText()
        {
            return StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_BUTTON_ACCEPT");
        }

        protected override IconType GetIconType()
        {
            var iconUrlVar = inboxInfo.GetVariable<string>("profileUrl");
            if (iconUrlVar != null && !string.IsNullOrEmpty(iconUrlVar.value))
            {
                iconUrl = iconUrlVar.value;
                return IconType.WEB_IMAGE;
            }

            return IconType.EMPTY;
        }
    }
}
