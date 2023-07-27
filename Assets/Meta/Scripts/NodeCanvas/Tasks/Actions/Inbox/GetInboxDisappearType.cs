using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using BagelCode.ClientModels;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Inbox")]
    public class GetInboxDisappearType : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<Blackboard>  inboxInfoBB;

        [BlackboardOnly]
        public BBParameter<int> saveAs;

        protected override string info
        {
            get { return string.Format("{0} = Get Inbox DisappearType", saveAs); }
        }

        protected override void OnExecute()
        {
            saveAs.value = 0;

            InboxTypes inboxType = BlackboardUtils.FindVariable<InboxTypes>(inboxInfoBB.value, "type").value;

            switch(inboxType)
            {
                case InboxTypes.REWARD:
                    {
                        RewardType rewardType = BlackboardUtils.FindVariable<RewardType>(inboxInfoBB.value, "reward/type").value;

                        switch(rewardType)
                        {
                            case RewardType.CREDIT:
                            case RewardType.CREDIT_WITH_MULTIPLIER:
                                saveAs.value = 1;
                                break;
                            case RewardType.RP:
                                saveAs.value = 2;
                                break;
                            case RewardType.DAILY_BOOST:
                                saveAs.value = 1;
                                break;
                        }
                    }
                    break;
                // case InboxTypes.MESSAGE:
                //     break;
                // case InboxTypes.MESSAGE_WARNING:
                //     break;
                case InboxTypes.GAME_COMPENSATION:
                    saveAs.value = 1;
                    break;
                case InboxTypes.FACEBOOK_FRIEND_CONNECT:
                    saveAs.value = 1;
                    break;
                case InboxTypes.FACEBOOK_SHARE:
                    saveAs.value = 1;
                    break;
                case InboxTypes.TOURNAMENT_WIN:
                    saveAs.value = 1;
                    break;
                case InboxTypes.SOCIAL_CREDIT:
                    saveAs.value = 1;
                    break;
            }

            EndAction();
        }
    }
}

