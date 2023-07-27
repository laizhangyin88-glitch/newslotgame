using BagelCode.ClientModels;
using UnityEngine;
using SlotMaker;
using UnityEngine.UI;
using NodeCanvas.Framework;
using System.Collections.Generic;

namespace BagelCode
{
    public class ImageCommonRewardController : MonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;
        private Animator anim;

        private ContextElement badgeElement;

        private RewardType rewardType;
        private RewardCheckScene checkScene;
        private Blackboard rewardInfo;
        private bool isInbox;

        private void OnEnable()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            rewardInfo = BlackboardUtils.FindVariable<Blackboard>(bb, "rewardInfo")?.value;
            rewardType = BlackboardUtils.FindVariable<RewardType>(bb, "rewardType")?.value ?? RewardType.UNKNOWN;
            checkScene = BlackboardUtils.FindVariable<RewardCheckScene>(bb, "rewardCheckScene")?.value ?? RewardCheckScene.DEFAULT;
            isInbox = BlackboardUtils.FindVariable<bool>(bb, "isInbox")?.value ?? false;

            long amount = MetaCommonRewardUtils.GetRewardAmount(rewardInfo, isInbox, rewardType, checkScene);

            switch (rewardType)
            {
                case RewardType.TIER_UPGRADE:
                    {
                        var targetTier = BlackboardUtils.FindVariable<int>(rewardInfo, "targetTier")?.value ?? 0;
                        var tierGroup = TierUtils.GetTierGroup(targetTier);

                        anim.SetInteger("Tier", tierGroup);
                    }
                    break;
                case RewardType.SOCIAL_CREDIT:
                    {
                        var friendList = BlackboardUtils.FindValue<List<Blackboard>>(rewardInfo, "friendList");

                        if (friendList.Count > 0)
                        {
                            for (int i = 0; i < (friendList.Count > 5 ? 5 : friendList.Count); i++)
                            {
                                var profileAreaElement = ContextUtils.FindElement(root, string.Format("Profile Area {0}", (i + 1)), ContextSearchingType.ChildrenSearch);
                                var profileElement = ContextUtils.FindElement(profileAreaElement, "Profile", ContextSearchingType.ChildrenSearch);

                                int tier = TierUtils.GetTierGroup(friendList[i].GetValue<int>("tier"));
                                profileElement.GetComponent<Animator>().SetInteger("Tier", tier);
                            }
                        }
                    }
                    break;
                case RewardType.CLUB_CREDIT:
                    {
                        var clubMemberList = BlackboardUtils.FindValue<List<Blackboard>>(rewardInfo, "clubMemberProfileList");

                        if (clubMemberList.Count > 0)
                        {
                            for (int i = 0; i < (clubMemberList.Count > 5 ? 5 : clubMemberList.Count); i++)
                            {
                                var profileAreaElement = ContextUtils.FindElement(root, "Profile Area " + (i + 1), ContextSearchingType.ChildrenSearch);
                                var profileElement = ContextUtils.FindElement(profileAreaElement, "Profile", ContextSearchingType.ChildrenSearch);

                                int tier = TierUtils.GetTierGroup(clubMemberList[i].GetValue<int>("tier"));
                                profileElement.GetComponent<Animator>().SetInteger("Tier", tier);
                            }
                        }
                    }
                    break;
                case RewardType.SCRATCHER:
                case RewardType.SCRATCHER_FOR_INBOX:
                    {
                        var imageElement = ContextUtils.FindElement(root, "Image", ContextSearchingType.ChildrenSearch);
                        Image image = imageElement.GetComponent<Image>();
                        var imageAnim = imageElement.GetComponent<Animator>();

                        if (image.sprite != null)
                        {
                            imageAnim.SetBool("Active", true);
                        }

                        badgeElement = ContextUtils.FindElement(imageElement, "Badge", ContextSearchingType.ChildrenSearch);
                    }
                    break;
            }

            if(badgeElement != null && bb != null)
            {
                var badgeAnim = badgeElement.GetComponent<Animator>();

                badgeAnim.SetInteger("value", (int)amount);

                if (amount > 99L)
                    MetaContextElementUtils.SimpleSetText(badgeElement, "Text", "999+");
                else
                    MetaContextElementUtils.SimpleSetText(badgeElement, "Text", amount.ToString());
            }
        }
    }
}
