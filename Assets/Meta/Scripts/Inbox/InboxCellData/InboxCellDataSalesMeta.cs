using SlotMaker;
using NodeCanvas.Framework;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class InboxCellDataSalesMeta : InboxCellData
    {
        public InboxCellDataSalesMeta(InboxCellController _ownerCellController, Blackboard _inboxInfo)
            : base(_ownerCellController, _inboxInfo)
        {
            buttonType = InboxCellController.InboxButtonType.REDEEM;
        }

        protected override string GetInternalText()
        {
            string resultText;

            var rewardType = GetRewardType();
            switch (rewardType)
            {
                default: // hog deal
                    resultText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_MESSAGE", title, message);
                    break;
            }

            return resultText;
        }

        protected override string GetButtonText()
        {
            return StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_BUTTON_SHOW_ME");
        }

        protected override IconType GetIconType()
        {
            var rewardType = GetRewardType();
            switch (rewardType)
            {
                case RewardType.HOG_DEAL:
                    return IconType.HOG_DEAL;
            }

            return IconType.DEFAULT;
        }

        private RewardType GetRewardType()
        {
            // todo shk
            var rewardType = inboxInfo.GetVariable<RewardType>("rewardType")?.value ?? RewardType.UNKNOWN;
            return rewardType;
        }
    }
}
