using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using System.Collections;

namespace BagelCode
{
    public class ClubMemberCellController : EventMonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;

        private GameObject caller;

        private Blackboard leaderPushInfoBB;

        private ContextElement leaderMenuElement;

        private ContextElement leaderPushElement;
        private ContextElement leaderPushSendElement;
        private ContextElement leaderPushCoolTimeElement;
        private ContextElement leaderPushCoolTimeTextElement;

        private Coroutine updateCoroutine;

        private bool isInit = false;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        public void InitProperty()
        {
            if (isInit) return;

            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();

            root.UpdateContext(false);

            caller = bb.GetValue<GameObject>("caller");

            InitEvents();

            // Profile Button
            MetaContextElementUtils.SetClickable(root, gameObject, "OnOpenProfile", false);

            // Leader Menu
            leaderMenuElement = ContextUtils.FindElement(root, "Button Leader Only Area/Leader Menu", FULL);
            MetaContextElementUtils.SetActive(leaderMenuElement, false);
            bb.AddVariable("_leaderMenuElement", leaderMenuElement);

            bool isActiveMenuButton = bb.GetVariable<bool>("_isActiveMenuButton")?.value ?? false;
            var leaderOnlyAreaElement = ContextUtils.FindElement(root, "Button Leader Only Area", CHILDREN);
            MetaContextElementUtils.SetActive(leaderOnlyAreaElement, isActiveMenuButton);

            var leaderOnlyButtonElement = ContextUtils.FindElement(leaderOnlyAreaElement, "Button Leader Only", CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(leaderOnlyButtonElement, "Text", "CLUB_LEADER_MENU_BUTTON_TEXT", CHILDREN);
            MetaContextElementUtils.SetClickable(leaderOnlyButtonElement, gameObject, "OnOpenLeaderMenu", false);

            // Chat
            MetaContextElementUtils.SimpleSetActive(root, "Button Private Chat Area", false, CHILDREN);

            // Leader Push
            leaderPushElement = ContextUtils.FindElement(root, "Leader Push", CHILDREN);
            leaderPushSendElement = ContextUtils.FindElement(leaderPushElement, "Button Send", CHILDREN);
            leaderPushCoolTimeElement = ContextUtils.FindElement(leaderPushElement, "Button Send Cool Time", CHILDREN);

            // Leader Push Send Button
            MetaContextElementUtils.SetClickable(
                leaderPushSendElement,
                () => {
                    EventSender.SendEvent(caller, MetaEventDefine.ON_META_UI_EVENT,
                new ParadoxNotion.EventData<string>(MetaEventDefine.ON_CLICK_LEADER_PUSH_OPEN, "members"));
                });

            // Leader Push Send Button
            MetaContextElementUtils.SetClickable(
                leaderPushCoolTimeElement,
                () => {
                    EventSender.SendEvent(caller, MetaEventDefine.ON_META_UI_EVENT,
                new ParadoxNotion.EventData<string>(MetaEventDefine.ON_CLICK_LEADER_PUSH_OPEN, "members"));
                });

            MetaContextElementUtils.SimpleSetTextGlobal(
                leaderPushSendElement, "Text", "BUTTON_MEMBER_LEADER_PUSH_SEND", CHILDREN);

            leaderPushCoolTimeTextElement = ContextUtils.FindElement(leaderPushCoolTimeElement, "Text", CHILDREN);
            // leaderPushCoolTimeElement.GetComponent<PIDButton>().interactable = false;

            isInit = true;
        }

        public void UpdateCellData()
        {
            if (bb == null) return;
            bool isMyCell = bb.GetVariable<bool>("_isMe")?.value ?? false;
            if (isMyCell && caller != null)
            {
                var callerBB = caller.GetComponent<Blackboard>();
                leaderPushInfoBB = BlackboardUtils.FindVariable<Blackboard>(callerBB, "clubInfoResponse/leaderPushInfo")?.value;
            }
            else
            {
                leaderPushInfoBB = null;
            }

            if (updateCoroutine != null)
                StopCoroutine(updateCoroutine);

            updateCoroutine = StartCoroutine(RepeatUpdateCoroutine());

            UpdateLeaderPushButton();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            root = GetComponent<ContextElement>();
            root.UpdateContext(false);
            MetaContextElementUtils.SimpleSetActive(root, "Leader Push", false);
        }

        //

        private void InitEvents()
        {
            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);

            Register(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.UPDATE_LEADER_PUSH_BUTTON_EVENT, UpdateCellData);
        }

        private IEnumerator RepeatUpdateCoroutine() // todo remaining timer 쓰도록 수정
        {
            float INTERVAL = 1f;

            while(true)
            {
                if (!enabled) continue;

                UpdateLeaderPushButton();

                yield return new WaitForSeconds(INTERVAL);
            }
        }

        private void UpdateLeaderPushButton()
        {
            bool exist = false;
            bool available = false;

            if (leaderPushInfoBB != null)
            {
                exist = leaderPushInfoBB.GetValue<bool>("exist");
                available = leaderPushInfoBB.GetValue<bool>("available");
            }

            MetaContextElementUtils.SetActive(leaderPushElement, exist);

            if (exist)
            {
                MetaContextElementUtils.SetActive(leaderPushSendElement, available);
                MetaContextElementUtils.SetActive(leaderPushCoolTimeElement, !available);
                if (!available) // cool time
                {
                    long remaining = (long)ClubUtils.GetLeaderPushSendCoolTimeRemaining(leaderPushInfoBB);
                    MetaContextElementUtils.SetTextGlobal(
                        leaderPushCoolTimeTextElement,
                        "BUTTON_MEMBER_LEADER_PUSH_COOL_TIME",
                        remaining);
                }
            }
        }
    }
}
