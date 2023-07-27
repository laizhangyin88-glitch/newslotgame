using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Popup/Common Reward")]
    public class UpdateCommonRewardItem : ActionTask<ContextElement>
    {
        public BBParameter<Blackboard> rewardResponseInfoBB;
        public BBParameter<bool> isMultiline;
        
        protected override string info
        {
            get { return string.Format("Update Common Reward Item"); }
        }

        protected override void OnExecute()
        {
            var isSentToInbox = BlackboardUtils.FindVariable<bool>(rewardResponseInfoBB.value, "isSentToInbox");
            bool fromLevelUpDash = BlackboardUtils.FindVariable<bool>(rewardResponseInfoBB.value, "fromLevelUpDash")?.value ?? false;

            bool isInbox = isSentToInbox != null && isSentToInbox.value;
            bool isReward = isInbox || fromLevelUpDash;

            Blackboard rewardInfo = isReward ? rewardResponseInfoBB.value.GetValue<Blackboard>("rewardInfo") : rewardResponseInfoBB.value;

            ContextElement imageAreaElement = ContextUtils.FindElement(agent, "Image Area", ContextSearchingType.ChildrenSearch);
            ContextElement textElement = ContextUtils.FindElement(agent, "Text", ContextSearchingType.ChildrenSearch);

            var rewardType = isReward ? rewardInfo.GetValue<RewardType>("type") : rewardInfo.GetValue<RewardType>("rewardType");
            var checkScene = BlackboardUtils.FindVariable<RewardCheckScene>(rewardResponseInfoBB.value, "rewardCheckScene")?.value ?? RewardCheckScene.DEFAULT;

            MetaCommonRewardUtils.CommonRewardResultSetter(imageAreaElement, textElement, rewardInfo, rewardType, isInbox, isMultiline.value, rewardResponseInfoBB.value, checkScene);

            if (checkScene != RewardCheckScene.LEVEL_UP_DASH)
            {
                switch (rewardType)
                {
                    case RewardType.TICKETED_BONUS_TICKET:
                    case RewardType.TICKETED_BONUS_TICKET_FOR_BOOST:
                        {
                            var tag = BlackboardUtils.FindVariable<BonusTag>(rewardInfo, "tag");
                            if (tag != null)
                            {
                                if (tag.value == BonusTag.BUY_A_BONUS)
                                {
                                    MetaContextElementUtils.SimpleSetActive(agent, "Buy A Bonus", true);
                                }
                                else if (tag.value == BonusTag.SUPER_BONUS)
                                {
                                    MetaContextElementUtils.SimpleSetActive(agent, "Super Bonus", true);
                                }
                                else if (tag.value == BonusTag.INSTANT_BONUS)
                                {
                                    MetaContextElementUtils.SimpleSetActive(agent, "Instant Bonus", true);
                                }
                            }
                        }
                        break;
                }
            }
            
            var isFromBingo = BlackboardUtils.FindVariable<bool>(rewardResponseInfoBB.value, "isFromBingo");
            
            if (isFromBingo != null && isFromBingo.value) 
                MetaContextElementUtils.SimpleSetActive(agent, "Bingo Reward", true);
                
            MetaContextElementUtils.SimpleSetActive(agent, "Check", isInbox);
            
            EndAction();
        }
    }
}
