using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;

namespace BagelCode
{
    public class PopupMysteryGiftController : EventMonoBehaviour
    {
        public float AUTO_DISPLAY_TIME = 5f;

        //

        private ContextElement root;
        private Blackboard bb;
        private Animator anim;

        private Blackboard giftInfo;

        private bool autoClose = false;

        private MysteryGiftType giftType = MysteryGiftType.LEVEL_UP;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private const StringTable.StringTableType GLOBAL = StringTable.StringTableType.Global;

        private Coroutine autoCloseCoroutine = null;
        private Coroutine breakCoroutine = null;

        private void Start()
        {
            InitProperty();
        }

        private void InitProperty()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            giftInfo = bb.GetVariable<Blackboard>("_mysteryGiftInfo")?.value;

            var giftTypeVar = bb.GetVariable<MysteryGiftType>("giftType");
            if (giftTypeVar != null)
                giftType = giftTypeVar.value;

            // Gift Type
            switch(giftType)
            {
                case MysteryGiftType.LEVEL_UP:
                    InitLevelUpGift();
                    break;
                case MysteryGiftType.LEADER_PUSH:
                    InitLeaderPushReward();
                    break;
                default:
                    {
                        Debug.LogWarning(string.Format("PopupMysteryGiftController initializing failure. {0} is undefined gift type.", giftType));
                        MetaPopupUtils.ClosePopup(gameObject);
                    }
                    break;
            }

            // Close
            MetaSystem.SubscribeBackButton(this.GetHashCode(), Close);
        }

        private void Close()
        {
            if(autoCloseCoroutine != null)
            {
                StopCoroutine(autoCloseCoroutine);
                autoCloseCoroutine = null;
            }

            if (breakCoroutine == null)
            {
                breakCoroutine = StartCoroutine(OpenCoroutine());
            }
        }

        private IEnumerator OpenCoroutine()
        {
            // todo TestSuiteContentsEventSolver Weight 1

            // Make Reward Popup
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Popup Common Reward Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject rewardPopupObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(bundle, asset, parent,
                (SceneLoadOperation sceneLoadOperation) => rewardPopupObj = sceneLoadOperation.GetScene()));

            var popupBB = rewardPopupObj.GetComponent<Blackboard>();

            string title = "";
            var rewardList = new List<Blackboard>();
            switch (giftType)
            {
                case MysteryGiftType.LEVEL_UP:
                    title = StringTableUtils.GetString(GLOBAL, "POPUP_MYSTERY_GIFT_TITLE");
                    rewardList = bb.GetValue<List<Blackboard>>("_mysteryGiftList");
                    break;
                case MysteryGiftType.LEADER_PUSH:
                    title = StringTableUtils.GetString(GLOBAL, "POPUP_COMMON_REWARD_RESULT_LEADER_PUSH_TITLE");
                    rewardList = bb.GetValue<List<Blackboard>>("_mysteryGiftList");
                    break;
            }

            var caller = bb.GetVariable<GameObject>("caller")?.value;
            MetaObjectUtils.SetCalleeCaller(rewardPopupObj, caller);

            BlackboardUtils.SetOrCreateValue(popupBB, "title", title);
            BlackboardUtils.SetOrCreateValue(popupBB, "autoClose", autoClose);
            BlackboardUtils.SetOrCreateValue(popupBB, "rewardList", rewardList);

            // Play Break
            anim.SetBool("Collect", true);
            var breakTrigger = new EventTrigger(this, "OnEndBreak");
            yield return new WaitUntilTrigger(breakTrigger);

            // Open Reward Popup
            MetaPopupUtils.OpenPopup(rewardPopupObj);

            // Close
            MetaSystem.UnSubscribeBackButton(this.GetHashCode());
            anim.SetTrigger("Close");
            MetaPopupUtils.ClosePopup(gameObject);
        }

        private IEnumerator AutoCloseCoroutine()
        {
            yield return new WaitForSeconds(AUTO_DISPLAY_TIME);

            Close();
        }

        private void InitLevelUpGift()
        {
            // Title
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Title Area/Text", "POPUP_MYSTERY_GIFT_TITLE", FULL);

            // Text
            int level = giftInfo.GetValue<int>("level");
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Text", "POPUP_MYSTERY_GIFT_TEXT", CHILDREN, level);

            // Open
            var openButtonElement = ContextUtils.FindElement(root, "Button Open", CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(openButtonElement, "Text", "BUTTON_OPEN", CHILDREN);
            MetaContextElementUtils.SetClickable(openButtonElement, Close);

            bool isAutoSpin = BlackboardUtils.FindVariable<bool>("./autoSpin")?.value ?? false;
            if (isAutoSpin)
            {
                autoClose = true;
                autoCloseCoroutine = StartCoroutine(AutoCloseCoroutine());
            }
        }

        private void InitLeaderPushReward()
        {
            // Title
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Title Area/Text", "POPUP_LEADER_PUSH_REWARD_TITLE", FULL);

            // Text
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Text", "POPUP_LEADER_PUSH_REWARD_TEXT", CHILDREN);

            // Open
            var openButtonElement = ContextUtils.FindElement(root, "Button Open", CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(openButtonElement, "Text", "BUTTON_OPEN", CHILDREN);
            MetaContextElementUtils.SetClickable(openButtonElement, Close);
        }
    }
}
