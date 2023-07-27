using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;

namespace BagelCode
{
    public class PopupBBBRewardController : MonoBehaviour
    {
        private ContextElement root;
        private Blackboard rootBB;
        private Animator rootAnim;

        private ContextElement buttonElement;
        private ContextElement rewardTextElement;
        private ContextElement infoTextElement;

        public void OnInit()
        {
            InitProperty();
            InitText();
            rootAnim?.SetBool("Active", true);
        }

        private void InitProperty()
        {
            root = GetComponent<ContextElement>();
            rootBB = GetComponent<Blackboard>();
            rootAnim = GetComponent<Animator>();

            root.UpdateContext();

            buttonElement = ContextUtils.FindElement(root, "Button Purchase Circle Big", ContextSearchingType.ChildrenSearch);

            rewardTextElement = ContextUtils.FindElement(root, "Text Reward", ContextSearchingType.ChildrenSearch);
            infoTextElement = ContextUtils.FindElement(root, "Text Info", ContextSearchingType.ChildrenSearch);

            MetaContextElementUtils.SetClickable(buttonElement, "OnCollect", root, null);
            MetaSystem.SubscribeBackButton(GetHashCode(), () => EventSender.SendEvent(gameObject, "OnCollect"));
        }

        private void InitText()
        {
            MetaContextElementUtils.SetText(rewardTextElement, GetRewardText());
            MetaContextElementUtils.SetTextGlobal(infoTextElement, "BAD_BEAT_BONUS_INFO");
            MetaContextElementUtils.SimpleSetTextGlobal(buttonElement, "Text", "BUTTON_COLLECT", ContextSearchingType.ChildrenSearch);
        }

        private string GetRewardText()
        {
            string rewardText = "";

            Blackboard rewardBB = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "bbbRewardInfo")?.value;
            if (rewardBB != null)
            {
                long credit = rewardBB.GetValue<long>("credit");
                rewardText = StringTableUtils.GetString(StringTable.StringTableType.Global, "COMMA_STYLE_COIN", credit);
            }
            return rewardText;
        }

        public void OnClose()
        {
            MetaSystem.UnSubscribeBackButton(GetHashCode());
            EventSender.SendGlobalEvent("FinishedBBB");
            rootAnim?.SetTrigger("Close");
        }
    }
}