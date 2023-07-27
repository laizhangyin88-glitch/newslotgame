using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using Sirenix.OdinInspector;
using ParadoxNotion.Services;

namespace BagelCode
{
    public class PopupVIPInvitationController : MonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        public void InitProperty()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();

            root.UpdateContext(false);

            InitContext();
        }

        private void InitContext()
        {
            var reward = BlackboardUtils.GetOrCreateVariable<Blackboard>(bb, "_rewardInfo");
            var targetTier = reward.value.GetValue<int>("targetTier");
            var targetTierGroup = TierUtils.GetTierGroup(targetTier);

            // Tier Text
            MetaContextElementUtils.SimpleSetText(root, "Text Tier After", TextDecoUtils.ConvertTierStyleText(targetTier), CHILDREN);

            // After Tier
            var targetTierElement = ContextUtils.FindElement(root, "Tier After Area/Image Tier", FULL);
            MetaContextElementUtils.SetIntProperty(targetTierElement, targetTierGroup);

            // Buttons
            var inviteButtonElement = ContextUtils.FindElement(root, "Button Ok", CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(inviteButtonElement, "Text", "VIP_INVITATION_INVITE_BUTTON", CHILDREN, reward.value.GetValue<long>("snsRewardCredit"));
            MetaContextElementUtils.SetClickable(inviteButtonElement,
                () => EventSender.SendEvent(gameObject, "OnClickInvite"));

            var closeButtonElement = ContextUtils.FindElement(root, "Button Close", CHILDREN);
            MetaContextElementUtils.SetClickable(closeButtonElement,
                () => EventSender.SendEvent(gameObject, "OnClose"));
        }
    }
}