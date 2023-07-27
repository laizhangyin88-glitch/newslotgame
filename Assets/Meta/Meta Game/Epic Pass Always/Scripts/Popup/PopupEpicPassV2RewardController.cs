using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using NodeCanvas.Framework;
using SlotMaker;
using BagelCode.ClientModels;
using BagelCode.OSA_Scroll;

namespace BagelCode.EpicPass
{
    public class PopupEpicPassV2RewardController : MonoBehaviour
    {
        private ContextElement rootElement;
        private Blackboard rootBB;
        private Animator rootAnim;

        private ContextElement titleTextElement;
        private ContextElement buttonCollectElement;
        private ContextElement buttonUnlockElement;
        private ContextElement freePassContentsAreaElement;
        private ContextElement epicPassContentsAreaElement;
        private ContextElement epicPassTitleTextElement;
        private ContextElement levelUpItemElement;
        private ContextElement iconRewardAreaElement;
        private ContextElement webImageElement;

        private ContextElement arrowLeftElement;
        private ContextElement arrowRightElement;

        private OSA_PopupEpicPassV2Rewards osaFreeController;
        private OSA_PopupEpicPassV2Rewards osaEpicController;
        private PopupEpicPassV2RewardCellController levelUpItemController;
        private List<Blackboard> rewardResultList = null;
        private List<Blackboard> freeResultList = null;
        private List<Blackboard> paidResultList = null;
        private List<Blackboard> availableRewardList = null;    // RewardInfo

        private bool isInit = false;
        private bool isLevelUp = false;
        private bool isEpicScrollCheck = false;
        private bool isPaidUser = false;

        private float epicScrollCheckTime = 0.0f;
        private float epicScrollDuration = 0.2f;

        private long epicRewardsScrollVelocity => (long)osaEpicController?.Velocity.x;

        private string contextID = "";

        private readonly float scrollWaitTime = 0.5f;
        private readonly int FREE_PASS_MAX_COUNT = 7;
        private readonly int EPIC_PASS_MAX_COUNT = 4;

        private void OnDestroy()
        {
            if (MetaSystem.Instance != null)
                MetaSystem.UnSubscribeBackButton(gameObject.GetHashCode());
        }

        void Update()
        {
            if (isEpicScrollCheck)
                CheckEpicPassScroll();
        }

        public void OnInit()
        {
            InitProperty();

            UpdateData();
            UpdateText();
            UpdateButtom();
            UpdateFreePass();
            UpdateEpicPass();
        }

        private void InitProperty()
        {
            if (isInit) return;

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(true);
            rootBB = gameObject.GetComponent<Blackboard>();
            rootAnim = gameObject.GetComponent<Animator>();

            ContextElement contentAreaElement = ContextUtils.FindElement(rootElement, "Contents Area", ContextSearchingType.ChildrenSearch);

            titleTextElement = ContextUtils.FindElement(contentAreaElement, "Text Title", ContextSearchingType.ChildrenSearch);

            freePassContentsAreaElement = ContextUtils.FindElement(contentAreaElement, "Free Pass Contents Area", ContextSearchingType.ChildrenSearch);
            osaFreeController = freePassContentsAreaElement?.GetComponent<OSA_PopupEpicPassV2Rewards>();
            epicPassContentsAreaElement = ContextUtils.FindElement(contentAreaElement, "Epic Pass Contents Area", ContextSearchingType.ChildrenSearch);
            osaEpicController = epicPassContentsAreaElement?.GetComponent<OSA_PopupEpicPassV2Rewards>();

            levelUpItemElement = ContextUtils.FindElement(freePassContentsAreaElement, "Scroll Rect/Contents/Cell 01", ContextSearchingType.FullNameSearch);
            levelUpItemController = levelUpItemElement?.GetComponent<PopupEpicPassV2RewardCellController>();

            arrowLeftElement = ContextUtils.FindElement(epicPassContentsAreaElement, "Arrow L", ContextSearchingType.ChildrenSearch);
            arrowRightElement = ContextUtils.FindElement(epicPassContentsAreaElement, "Arrow R", ContextSearchingType.ChildrenSearch);
            epicPassTitleTextElement = ContextUtils.FindElement(epicPassContentsAreaElement, "Text Sub Title", ContextSearchingType.ChildrenSearch);

            buttonCollectElement = ContextUtils.FindElement(contentAreaElement, "Button Collect", ContextSearchingType.ChildrenSearch);
            buttonUnlockElement = ContextUtils.FindElement(contentAreaElement, "Button Unlock", ContextSearchingType.ChildrenSearch);

            iconRewardAreaElement = ContextUtils.FindElement(contentAreaElement, "Icon Reward Area", ContextSearchingType.ChildrenSearch);
            webImageElement = ContextUtils.FindElement(iconRewardAreaElement, "Epic Pass Always Reward Web Image/Image", ContextSearchingType.FullNameSearch);
            iconRewardAreaElement.gameObject.SetActive(false);
            // Back Button Event.
            MetaSystem.SubscribeBackButton(gameObject.GetHashCode(), () => { OnClose(); });

            isInit = true;
        }

        private void UpdateData()
        {
            if (rootBB == null) return;

            rewardResultList = EpicPassUtilsV2.RewardResultList;
            isLevelUp = BlackboardUtils.GetOrCreateVariable<bool>(rootBB, "isLevelUp")?.value ?? false;
            isPaidUser = BlackboardUtils.FindVariable<bool>(rootBB, "isPaidUser")?.value ?? false;
            availableRewardList = BlackboardUtils.FindVariable<List<Blackboard>>(rootBB, "availableRewardOnPurchaseList")?.value ?? null;

            EventInfo eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.SEASON_PASS_V2);
            BlackboardUtils.SetOrCreateValue<int>(rootBB, "_seasonPassEventID", eventInfo?.id ?? 0);

            contextID = BiEventUtils.GetPopupContextId(gameObject);
            BlackboardUtils.SetOrCreateValue<string>(rootBB, "_biContextID", contextID);
        }

        private void UpdateText()
        {
            string titleText = "";
            string collectButton = "";
            if (rewardResultList != null)
            {
                titleText = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_EPIC_PASS_ALWAYS_REWARD_TEXT");
                collectButton = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_EPIC_PASS_ALWAYS_REWARD_COLLECT_BUTTON");
                // Is epic pass unlock check & set text(POPUP_EPIC_PASS_ALWAYS_REWARD_PAID_UNLOCK_TEXT / POPUP_EPIC_PASS_ALWAYS_REWARD_PAID_REWARDS_TEXT)
                MetaContextElementUtils.SetTextGlobal(epicPassTitleTextElement, isPaidUser ? "POPUP_EPIC_PASS_ALWAYS_REWARD_PAID_REWARDS_TEXT" : "POPUP_EPIC_PASS_ALWAYS_REWARD_PAID_UNLOCK_TEXT");
                GSManager.Instance.GetHandler(EpicPassUtilsV2.Sounds.EPIC_PASS_REWARD).Play();
            }
            else
            {
                // Level Up
                titleText = StringTableUtils.GetString(StringTable.StringTableType.Global, "EPIC_PASS_LEVEL_UP_TITLE_TEXT", EpicPassUtilsV2.Level);
                collectButton = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_EPIC_PASS_ALWAYS_LEVEL_UP_CLOSE_BUTTON");

                GSManager.Instance.GetHandler(EpicPassUtilsV2.Sounds.EPIC_PASS_LEVEL_UP_POPUP).Play();
            }

            MetaContextElementUtils.SetText(titleTextElement, titleText);
            MetaContextElementUtils.SimpleSetText(buttonCollectElement, "Text", collectButton, ContextSearchingType.ChildrenSearch);

            Blackboard productBB = EpicPassUtilsV2.EpicPassInfo.GetValue<Blackboard>("epicPassProduct");
            if (productBB != null)
            {
                BlackboardUtils.SetOrCreateValue(rootBB, "product", productBB);
                MetaContextElementUtils.SimpleSetTextGlobal(buttonUnlockElement, "Text", "POPUP_EPIC_PASS_ALWAYS_REWARD_PURCHASE_BUTTON", ContextSearchingType.ChildrenSearch, productBB.GetValue<double>("price"));
            }
        }

        private void UpdateButtom()
        {
            buttonUnlockElement.gameObject.SetActive(!EpicPassUtilsV2.Paid && rewardResultList != null);

            MetaContextElementUtils.SetClickable(
                buttonUnlockElement,
                "OnPurchase",
                rootElement,
                null);

            MetaContextElementUtils.SetClickable(
                buttonCollectElement,
                "OnCollect",
                rootElement,
                null);
        }

        private void UpdateFreePass()
        {
            if (osaFreeController == null) return;
            freeResultList = EpicPassUtilsV2.RewardFreeResultList;

            if (freeResultList == null || freeResultList.Count == 0)
            {
                freePassContentsAreaElement.gameObject.SetActive(false);
                if (isLevelUp)
                {
                    MetaContextElementUtils.SetWebImage(webImageElement, EpicPassUtilsV2.BigIconImageUrl);
                    iconRewardAreaElement.gameObject.SetActive(true);
                }
            }
            else
            {
                var simpleList = MetaCommonRewardUtils.CreateSimpleRewardResultOrInfoList(
                    rootBB, "simpleFreeRewardResultList", EpicPassUtilsV2.RewardFreeResultList, RewardCheckScene.EPIC_PASS_V2);

                osaFreeController.InitData(simpleList);

                osaFreeController.enabled = true;
                levelUpItemElement.gameObject.SetActive(false);
            }

            if (freeResultList == null || freeResultList.Count <= FREE_PASS_MAX_COUNT)
                osaFreeController.SetDragEnabled(false);
        }

        private void UpdateEpicPass()
        {
            if (osaEpicController == null) return;
            paidResultList = EpicPassUtilsV2.RewardPaidResultList;

            if (paidResultList == null || paidResultList.Count == 0)
            {
                if (!isPaidUser && availableRewardList != null && availableRewardList.Count > 0)
                {
                    var simpleList = MetaCommonRewardUtils.CreateSimpleRewardResultOrInfoList(
                        rootBB, "simpleAvailableRewardList", availableRewardList, RewardCheckScene.EPIC_PASS_V2);

                    paidResultList = simpleList;
                    osaEpicController.InitData(paidResultList, true);

                    epicPassContentsAreaElement.gameObject.SetActive(true);

                    ActiveEpicPassScroll(paidResultList.Count);
                }
                else
                {
                    epicPassContentsAreaElement.gameObject.SetActive(false);
                    isEpicScrollCheck = false;
                }
            }
            else
            {
                var simpleList = MetaCommonRewardUtils.CreateSimpleRewardResultOrInfoList(
                    rootBB, "simpleEpicRewardResultList", EpicPassUtilsV2.RewardPaidResultList, RewardCheckScene.EPIC_PASS_V2);

                paidResultList = simpleList;
                osaEpicController.InitData(paidResultList);

                epicPassContentsAreaElement.gameObject.SetActive(true);
                ActiveEpicPassScroll(paidResultList.Count);
            }
        }

        private void ActiveEpicPassScroll(int itemCount)
        {
            if (itemCount > EPIC_PASS_MAX_COUNT)
            {
                arrowLeftElement.gameObject.SetActive(true);
                arrowRightElement.gameObject.SetActive(true);
                isEpicScrollCheck = true;
            }
            else
            {
                arrowLeftElement.gameObject.SetActive(false);
                arrowRightElement.gameObject.SetActive(false);
                osaEpicController.SetDragEnabled(false);
                isEpicScrollCheck = false;
            }
        }

        private void CheckEpicPassScroll()
        {
            if (paidResultList == null || paidResultList.Count == 0)
                return;

            if (epicRewardsScrollVelocity != 0)
                epicScrollCheckTime = 0.0f;
            else if (epicScrollCheckTime < scrollWaitTime)
                epicScrollCheckTime += Time.deltaTime;
            else
                return;

            int scrollIndex = osaEpicController.GetScrollItemIndex();
            SetEpicPassArrowActive(scrollIndex);
        }

        private void SetEpicPassArrowActive(int index)
        {
            arrowLeftElement.gameObject.SetActive(index > 0);
            arrowRightElement.gameObject.SetActive(index + EPIC_PASS_MAX_COUNT < paidResultList.Count);
        }

        public void OnClose()
        {
            EventData eventData = new EventData(MetaEventDefine.ON_CLICK_CLOSE_PURCHASE_REWARD_POPUP);
            MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, eventData);
            MessageDispatcher.Dispatch("OnCreditEvent", new EventData<bool>("UpdateNaviCredit", false));

            EpicPassUtilsV2.ClearRewardResultList();

            PopupManager.Instance.Close(gameObject);
            rootAnim.SetTrigger("Close");
        }

        public void OnClickCollect()
        {
            OnClose();
        }

        public void OnClickUnlock()
        {
            // Send AE
            BIClientClickButtonEpicPass("SEASON_PASS_UPGRADE");

            EventInfo eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.SEASON_PASS_V2);
            if (eventInfo != null && eventInfo.constraints != null)
            {
                EventDataSeasonPassV2 epicPassEventInfo = eventInfo.constraints as EventDataSeasonPassV2;
                if (epicPassEventInfo != null)
                {
                    Dictionary<string, object> customData = new Dictionary<string, object>();

                    customData["passive_event_id"] = (long)eventInfo.id;
                    customData["season_pass_setting_id"] = (long)epicPassEventInfo.seasonPassSettingV2Id;
                    customData["season_pass_setting_name"] = epicPassEventInfo.seasonPassSettingV2Name;
                    customData["context_id"] = contextID;
                    customData["pass_level"] = EpicPassUtilsV2.Level;
                    customData["required_pass_point"] = EpicPassUtilsV2.RequiredPoint;
                    customData["own_pass_point"] = EpicPassUtilsV2.Point;

                    Analytics.CustomEvent("client_click_season_pass_upgrade", customData);
                }
            }
        }

        public void OnClickLeftArrow()
        {
            MetaContextElementUtils.SendEvent(rootElement, "OnClickLeft", null, null);
        }

        public void OnClickRightArrow()
        {
            MetaContextElementUtils.SendEvent(rootElement, "OnClickRight", null, null);
        }

        public void OnMoveEpicRewardLeft()
        {
            int scrollIndex = osaEpicController.GetScrollItemIndex();
            if (scrollIndex > 0)
            {
                osaEpicController.SmoothScrollTo(scrollIndex - 1, epicScrollDuration);
                SetEpicPassArrowActive(scrollIndex - 1);
            }
        }

        public void OnMoveEpicRewardRight()
        {
            int scrollIndex = osaEpicController.GetScrollItemIndex();
            if (scrollIndex + EPIC_PASS_MAX_COUNT < paidResultList.Count)
            {
                osaEpicController.SmoothScrollTo(scrollIndex + 1, epicScrollDuration);
                SetEpicPassArrowActive(scrollIndex + 1);
            }
        }
        // VIP Lounge Welcome Popup
        public void CheckWelcomeVIPLoungePopup(GameObject objRewardPopup)
        {
            if (BlackboardQueryUtils.IsVipLoungeEnabled())
                EpicPassUtilsV2.CheckWelcomePopup = true;
        }
        // 154 AE
        public void BIClientClickButtonEpicPass(string buttonName)
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();
            customData["button_name"] = buttonName;
            customData["context_id"] = contextID;
            Analytics.CustomEvent("client_click_button", customData);
        }
    }
}