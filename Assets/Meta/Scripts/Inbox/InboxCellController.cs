using System.Collections.Generic;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;
using System.Collections;
using BagelCode.ClientModels;
using System.Linq;
using ParadoxNotion.Services;
using BagelCode.OSA_Scroll;

using static BagelCode.InboxEvent;

namespace BagelCode
{
    public class InboxCellController : EventMonoBehaviour
    {
        public InboxController ownerInboxController;
        public InboxRectController rectController;

        private List<Blackboard> inboxInfoGroup;

        [SerializeField]
        public int InboxInfoGroupCount => inboxInfoGroup.Count;
        public int InboxId => InboxInfo.GetVariable<int>("id")?.value ?? 0;
        public int BucksGiftId => InboxInfo.GetVariable<int>("giftId")?.value ?? 0;

        [SerializeField]
        private Blackboard inboxInfo;
        public Blackboard InboxInfo
        {
            get
            {
                inboxInfo = inboxInfoGroup.FirstOrDefault();
                return inboxInfo;
            }
        }

        public Blackboard responseInfo;

        public InboxCellData CellData { get; private set; }

        private ContextElement root;
        private Blackboard bb;
        private Animator anim;

        private GameObject earlyAccessObj;
        private GameObject slotThumbnailObj;
        private GameObject iconObj;

        // Context
        private ContextElement messageElement;
        private ContextElement acceptButtonElement;
        private ContextElement redeemButtonElement;
        private ContextElement cellBaseElement;
        private ContextElement cellGradientElement;

        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;
        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;

        private const StringTable.StringTableType GLOBAL = StringTable.StringTableType.Global;

        private bool isInit = false;

        private Coroutine updateCoroutine;

        // Color
        [SerializeField]
        private Color DEFAULT_COLOR, DEFAULT_GRADIENT_COLOR, WARNING_COLOR, WARNING_GRADIENT_COLOR;

        public enum InboxButtonType
        {
            REDEEM = 0,
            ACCEPT = 1,
        }

        //

        public void InitProperty()
        {
            if (isInit) return;

            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            gameObject.name += " " + transform.GetSiblingIndex();

            ownerInboxController = InboxController.instance;
            GameObject caller = ownerInboxController.gameObject;
            bb.AddVariable("caller", caller);

            rectController = ownerInboxController.rectController;

            messageElement = ContextUtils.FindElement(root, "Text Message", CHILDREN);
            acceptButtonElement = ContextUtils.FindElement(root, "Button Accept", CHILDREN);
            redeemButtonElement = ContextUtils.FindElement(root, "Button Redeem", CHILDREN);
            cellBaseElement = ContextUtils.FindElement(root, "Cell Base", CHILDREN);
            cellGradientElement = ContextUtils.FindElement(root, "Cell Gradient", CHILDREN);

            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);

            isInit = true;
        }

        public IEnumerator OnAcceptCoroutine()
        {
            if (!CellData.isBanner)
            {
                yield return StartCoroutine(OnAcceptInboxItemCoroutine());
            }
            else
            {
                yield return StartCoroutine(OnAcceptInboxBannerCoroutine());
            }
        }

        public void UpdateInboxInfoGroup(List<Blackboard> _inboxInfoGroup)
        {
            InitProperty();

            inboxInfoGroup = _inboxInfoGroup;
            bb.SetValue("inboxInfo", InboxInfo);

            if(updateCoroutine != null)
            {
                StopCoroutine(updateCoroutine);
                updateCoroutine = null;
            }
            updateCoroutine = StartCoroutine(UpdateInboxCellCoroutine());
        }

        public IEnumerator UpdateInboxCellCoroutine()
        {
            if (!(earlyAccessObj is null)) Destroy(earlyAccessObj);
            if (!(slotThumbnailObj is null)) Destroy(slotThumbnailObj);

            UpdateCellData();

            if (CellData != null)
            {
                yield return StartCoroutine(MakeInboxIconCoroutine(CellData.iconType, true));

                CellData.InboxItemCheckType();

                yield return StartCoroutine(UpdateWithGameIdCoroutine());
            }

            EventSender.SendEvent(gameObject, ON_READY);
        }

        public void UpdateText(string text)
        {
            MetaContextElementUtils.SetText(messageElement, text);
        }

        //

        private IEnumerator OnAcceptInboxItemCoroutine()
        {
            // Start Accept
            rectController.OnStartAccept();

            anim.SetInteger("DisappearType", CellData.disappearType);

            CellData.gameId = -1;

            // Parse Event Data
            var acceptEventData = BlackboardUtils.GetOrCreateVariable<object>(bb, "acceptEventData")?.value;
            CellData.AcceptEventData = acceptEventData;

            // Check View Ads
            yield return StartCoroutine(CellData.InboxItemCheckViewAdCoroutine());

            // Check Voucher, Ticketed Bonus Ticket etc
            yield return StartCoroutine(CellData.InboxItemCheckExtraCoroutine());

            // Cancel Purchase Voucher
            if (InboxController.IsAcceptable)
            {
                rectController.OnFinishAccept();
                yield break;
            }

            // Check Enter Game
            yield return StartCoroutine(CellData.InboxCellCheckGameCoroutine());

            // Loading
            GameObject loadingObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenLoadingPopupCoroutine((obj) => loadingObj = obj));

            // Request Inbox Accept
            InboxUtils.InboxAcceptRequest(bb, InboxInfo);

            var successTrigger = new EventTrigger(gameObject, ON_SUCCESS_ACCEPT_INBOX);
            var failTrigger = new EventTrigger(gameObject, ON_FAIL_ACCEPT_INBOX);
            var cancelTrigger = new EventTrigger(gameObject, ON_CANCEL_ACCEPT_INBOX);
            var successBucksTrigger = new EventTrigger(gameObject, ON_SUCCESS_ACCEPT_GIFT_BUCKS);
            yield return new WaitUntilTrigger(successTrigger, failTrigger, cancelTrigger, successBucksTrigger);

            MetaPopupUtils.ClosePopup(loadingObj);

            if (successTrigger.IsTrigger)
            {
                Blackboard inboxResponse = BlackboardUtils.FindValue<Blackboard>("/inboxResponse");
                responseInfo = inboxResponse.GetValue<Blackboard>(string.Format("ID_{0}", InboxId));
                bb.AddVariable("response", responseInfo);

                RewardCauseType rewardCauseType = InboxInfo.GetValue<RewardCauseType>("rewardCauseType");
                BiEventUtils.BiEventInboxAccept(InboxInfo, rewardCauseType);

                var acceptNextItemTrigger = new EventTrigger(gameObject, ACCEPT_NEXT_INBOX_ITEM);
                var onOpenCommonRewardPopupTrigger = new EventTrigger(gameObject, MetaEventDefine.ON_OPEN_COMMON_REWARD_POPUP);

                if (ApplicationSettings.LogTest())
                    Debug.Log("Wait Accept Success");

                yield return StartCoroutine(CellData.OnAcceptSuccessCoroutine());

                if (ApplicationSettings.LogTest())
                    Debug.Log("Accept Success");

                // Accept next item
                if (acceptNextItemTrigger.IsTrigger && inboxInfoGroup.Count > 0)
                {
                    CellData.isFirstReward = false;
                    CellData.isAcceptNext = true;

                    bool isAuto = (bool)acceptNextItemTrigger.EventData?.value;
                    StartCoroutine(ownerInboxController.AcceptNextInboxItemCoroutine(this, isAuto));
                }
                else
                {
                    CellData.isFirstReward = true;
                    CellData.isAcceptNext = false;
                }

                // Remove Self
                var items = rectController.GetComponent<OSA_InboxItems>();
                InboxUtils.RemoveAcceptInboxItem(items, InboxId);

                CellData.OnRemoveInboxItem();

                if (onOpenCommonRewardPopupTrigger.IsTrigger)
                {
                    EventSender.SendEvent(ownerInboxController.gameObject, RELOAD_INBOX);
                }
            }
            else if (failTrigger.IsTrigger)
            {
                Disappear();

                // Remove Self
                var items = rectController.GetComponent<OSA_InboxItems>();
                InboxUtils.RemoveAcceptInboxItem(items, InboxId);
            }
            else if (successBucksTrigger.IsTrigger)
            {
                Blackboard inboxResponse = BlackboardUtils.FindValue<Blackboard>("/inboxResponse");
                responseInfo = inboxResponse.GetValue<Blackboard>(string.Format("GIFT_ID_{0}", BucksGiftId));
                bb.AddVariable("response", responseInfo);

                // Remove Self
                var items = rectController.GetComponent<OSA_InboxItems>();
                InboxUtils.RemoveAcceptVegasBucksItem(items, BucksGiftId);

                CellData.OnRemoveInboxItem();
            }
            else if (cancelTrigger.IsTrigger)
            {
                Debug.Log("Inbox accept canceled. inboxId: " + InboxId);
            }

            CellData.AcceptEventData = null;

            // Finish Accept
            rectController.OnFinishAccept();
        }

        private IEnumerator OnAcceptInboxBannerCoroutine()
        {
            var iamId = InboxInfo.GetVariable<int>("inAppMessageId");
            if (iamId is null)
                yield break;

            // Start Accept
            rectController.OnStartAccept();

            // Get IAM Info
            Blackboard iamInfo = BlackboardQueryUtils.GetIAMBlackboard(iamId.value);
            if (iamInfo is null)
            {
                GameObject loadingObj = null;
                yield return StartCoroutine(MetaPopupUtils.OpenLoadingPopupCoroutine((obj) => loadingObj = obj));

                yield return StartCoroutine(IAMUtils.IAMInfoRequestCoroutine(iamId.value, (_iamInfo, iamTriggerType) => iamInfo = _iamInfo));

                MetaPopupUtils.ClosePopup(loadingObj);

                bool isSuccessPreloadIAMImages = false;
                yield return StartCoroutine(IAMUtils.PreloadIAMImagesCoroutine(iamId.value, (isSuccess) => isSuccessPreloadIAMImages = isSuccess));

                if (!isSuccessPreloadIAMImages)
                    yield break;
            }

            // Send BI
            string biContextId = InboxUtils.GetInboxEnterContextID();
            if (!(iamInfo is null))
            {
                var iamType = iamInfo.GetValue<InAppMessageType>("type");

                Analytics.CustomEvent("client_click_inbox_iam", new Dictionary<string, object>()
                {
                    ["iam_id"] = iamId.value,
                    ["iam_type"] = iamType.ToString(),
                    ["context_id"] = biContextId
                });
            }

            //Trigger IAM
            bool isTriggered = false;
            isTriggered = IAMRouter.Instance.TriggerIAMByID(InAppMessageTriggerType.UNKNOWN, iamId.value, gameObject, biContextId, false, false, false);

            if(isTriggered)
            {
                // IAM callback event is received through router.
                var router = GetComponent<MessageRouter>();
                var iamCallbackTrigger = new EventTrigger(router, ON_IAM_CALLBACK);
                yield return new WaitUntilTrigger(iamCallbackTrigger);

                var bannerType = CellData.bannerType;
                if (bannerType == InboxBannerTypes.NEWS)
                {
                    Disappear();

                    BlackboardQueryUtils.RemoveInboxBannerItem(InboxId);

                    // Remove Self
                    var items = rectController.GetComponent<OSA_InboxItems>();
                    items.RemoveItemFromID(InboxId);
                }
            }
            else
            {
                // Cancel Callback
                EventSender.SendEvent(gameObject, ON_IAM_CALLBACK);

                yield return new WaitForSeconds(0.05f);
            }

            // Finish Accept
            rectController.OnFinishBannerAccept();
        }

        private IEnumerator UpdateWithGameIdCoroutine()
        {
            if (CellData.gameId == -1)
                yield break;

            ContextElement thumbnailElement = ContextUtils.FindElement(root, "Icon Area", FULL);
            Transform thumbnailTransform = thumbnailElement.transform;
            MakeSlotThumbnailIcon(thumbnailTransform);

            switch (CellData.bonusTag)
            {
                case BonusTag.BUY_A_BONUS:
                    yield return StartCoroutine(MakeInboxIconCoroutine(InboxCellData.IconType.BUY_A_BONUS, false));
                    break;
                case BonusTag.SUPER_BONUS:
                    yield return StartCoroutine(MakeInboxIconCoroutine(InboxCellData.IconType.SUPER_BONUS, false));
                    break;
                case BonusTag.INSTANT_BONUS:
                    yield return StartCoroutine(MakeInboxIconCoroutine(InboxCellData.IconType.INSTANT_BONUS, false));
                    break;
            }

            if( CellData.inboxType == InboxTypes.SPIN_DEAL || CellData.inboxType == InboxTypes.SPIN_DEAL_V2 )
            {
                bool boosted = CellData.inboxInfo.GetVariable<bool>("isBoosted")?.value ?? false;
                if (boosted)
                    yield return StartCoroutine(MakeInboxIconCoroutine(InboxCellData.IconType.SPIN_BOOST, false));
            }

            Blackboard slotInfoBB = BlackboardQueryUtils.GetEarlyAccessSlotInfo(CellData.gameId);
            if (!(slotInfoBB is null))
            {
                string bundle = ApplicationSettings.MakeApplicationBundleName("lobby");
                MetaObjectUtils.MakePrefabAsync(bundle, "Badge Early Access", thumbnailTransform,
                    (_earlyAccessObj) => earlyAccessObj = _earlyAccessObj);

                yield return new WaitUntil(() => !(earlyAccessObj is null));
            }
        }

        private void UpdateCellData()
        {
            if (CellData?.inboxInfo == InboxInfo)
                return;

            CellData = InboxCellData.Create(this, InboxInfo);

            bb.AddVariable("eventId", 0);
            if (CellData is InboxCellDataReward rewardCellData)
            {
                EventInfo eventInfo = rewardCellData.eventInfo;
                if(eventInfo != null)
                {
                    bb.SetValue("eventId", eventInfo.id);
                }
            }

            // Color
            if (CellData.useWarningColor)
            {
                MetaContextElementUtils.SetColor(cellBaseElement, WARNING_COLOR);
                MetaContextElementUtils.SetColor(cellGradientElement, WARNING_GRADIENT_COLOR);
            }
            else
            {
                MetaContextElementUtils.SetColor(cellBaseElement, DEFAULT_COLOR);
                MetaContextElementUtils.SetColor(cellGradientElement, DEFAULT_GRADIENT_COLOR);
            }

            // Text
            UpdateText(CellData.text);

            // Button
            InboxButtonType buttonType = CellData.buttonType;
            acceptButtonElement.gameObject.SetActive(buttonType == InboxButtonType.ACCEPT);
            redeemButtonElement.gameObject.SetActive(buttonType == InboxButtonType.REDEEM);

            ContextElement targetElement = buttonType == InboxButtonType.ACCEPT ? acceptButtonElement : redeemButtonElement;
            MetaContextElementUtils.SimpleSetText(targetElement, "Text", CellData.buttonText, CHILDREN);
            MetaContextElementUtils.SetClickable(targetElement, () => EventSender.SendEvent(gameObject,ON_ACCEPT));

            // Date
            long expireTimestamp = InboxInfo.GetValue<long>("expireTimestamp");
            string expireText = " ";
            if (expireTimestamp > 0)
                expireText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_EXPIRE_DATE", expireTimestamp - MetaSystem.GetTimeStamp());

            MetaContextElementUtils.SimpleSetTextGlobal(root, "Text Date", "INBOX_ITEM_DATE", CHILDREN, expireText);
        }

        private void MakeSlotThumbnailIcon(Transform parent)
        {
            if (!(slotThumbnailObj is null)) GameObject.Destroy(slotThumbnailObj);

            var gameInfo = BlackboardQueryUtils.GetGameInfo(CellData.gameId);
            string gameTitle = gameInfo.GetVariable<string>("gameTitle")?.value;

            if (gameTitle != null)
            {
                slotThumbnailObj = MetaIconUtils.MakeSlotThumbnailIconObjectFromGameTitle(gameTitle, parent, "");
            }
            else
            {
                slotThumbnailObj = null;
            }
        }

        private IEnumerator MakeInboxIconCoroutine(InboxCellData.IconType iconType, bool useIsLoaded)
        {
            if (!(iconObj is null)) GameObject.Destroy(iconObj);

            // Skip
            if (iconType == InboxCellData.IconType.EMPTY || iconType == InboxCellData.IconType.GAME_THUMBNAIL)
                yield break;

            // Area
            string iconArea;
            if (iconType == InboxCellData.IconType.PURCHASE_COUPON)
                iconArea = "Coupon Area";
            else if (iconType == InboxCellData.IconType.TIER_UPGRADE)
                iconArea = "Image Tier Area";
            else
                iconArea = "Icon Area";

            // Make Icon
            var iconAreaElement = ContextUtils.FindElement(root, iconArea, FULL);
            var parent = iconAreaElement.GetComponent<Transform>();
            string iconName = InboxUtils.GetInboxIconPrefabName(iconType, inboxInfo);
            iconObj = MetaIconUtils.MakeInboxIconObjectFromName(iconName, parent, "");

            iconAreaElement.UpdateContext(false);

            // Post Effect
            if(iconType == InboxCellData.IconType.LEVEL_UP_EXP_BOOST)
            {
                var iconElement = iconObj.GetComponent<ContextElement>();
                var textElement = ContextUtils.FindElement(iconElement, "Text Exp Multi", ContextSearchingType.ChildrenSearch);

                long multiplyNumerator = LevelUpDash.LevelUpDash.Utils.PurchaseBoosterMultiplierNumerator;
                double multiplier = NumberUtils.GetMultiplierFromNumerator(multiplyNumerator);
                string multiText = StringTableUtils.GetString(GLOBAL, "LEVEL_UP_DASH_MAIN_SCENE_BOOSTER_MULTIPLY", multiplier);
                MetaContextElementUtils.SetText(textElement, multiText);
            }
            else if(iconType == InboxCellData.IconType.SPIN_BOOST)
            {
                var iconElement = iconObj.GetComponent<ContextElement>();
                MetaContextElementUtils.SimpleSetTextGlobal(iconElement, "Text", "INBOX_ITEM_SPIN_BOOST_LABEL", CHILDREN);
            }
            else if (iconType == InboxCellData.IconType.WEB_IMAGE && useIsLoaded)
            {
                bool isLoaded = false;
                MetaContextElementUtils.SimpleSetWebImage(
                    root,
                    "Icon Area/Inbox Icon Web Image",
                    CellData.iconUrl,
                    () => isLoaded = true,
                    null,
                    FULL);

                yield return new WaitUntil(() => isLoaded);
            }
        }

        private void Disappear()
        {
            anim.SetTrigger("Disappear");
        }
    }
}
