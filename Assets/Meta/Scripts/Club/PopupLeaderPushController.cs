using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using System.Collections.Generic;
using System.Collections;

namespace BagelCode
{
    public class PopupLeaderPushController : EventMonoBehaviour
    {
        private const int MESSAGE_LIST_COUNT = 3;

        private ContextElement root;
        private Blackboard bb;

        private int select;
        private bool isFromIam;

        private Blackboard leaderPushInfo;

        private ContextElement[] messageDefaultElements = new ContextElement[MESSAGE_LIST_COUNT];
        private ContextElement[] messageSelectElements = new ContextElement[MESSAGE_LIST_COUNT];

        private bool isStreamingAssetLoaded = false;

        private const string SEND_LEADER_PUSH_EVENT = "SendLeaderPush";
        private const string OPEN_INFORMATION_EVENT = "OpenInformation";

        private const string CAPTAINSCALL_STREAMING_ASSET_NAME = "captainscall";

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private void Start()
        {
            InitProperty();
        }

        private void OnDestroy()
        {
            MetaSystem.UnSubscribeBackButton(this.GetHashCode());
        }

        private void InitProperty()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();

            root.UpdateContext(false);

            // Question
            MetaContextElementUtils.SimpleSetClickable(root, "Button Question",
                gameObject, MetaEventDefine.ON_META_UI_EVENT, OPEN_INFORMATION_EVENT);

            // Close
            MetaContextElementUtils.SimpleSetClickable(root, "Button Close", ClosePopup);
            MetaSystem.SubscribeBackButton(this.GetHashCode(), ClosePopup);

            // Title
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Title Area/Text", "POPUP_LEADER_PUSH_TITLE", FULL);

            // Messages
            List<string> pushMessageList = GetPushMessageList();
            if (pushMessageList == null) return;

            var pushListElement = ContextUtils.FindElement(root, "Captain's Call Push List", FULL);
            MetaContextElementUtils.SimpleSetTextGlobal(pushListElement, "Info Text", "POPUP_LEADER_PUSH_INFO", CHILDREN);
            ContextElement[] pushTextAreas = new ContextElement[3];
            for (int i = 0; i < MESSAGE_LIST_COUNT; ++i)
            {
                pushTextAreas[i] = ContextUtils.FindElement(
                    pushListElement,
                    string.Format("Push Text Area {0}", i + 1),
                    CHILDREN);

                messageDefaultElements[i] = ContextUtils.FindElement(pushTextAreas[i], "Button Push/Base", FULL);
                MetaContextElementUtils.SimpleSetText(messageDefaultElements[i], "Text", pushMessageList[i], CHILDREN);

                messageSelectElements[i] = ContextUtils.FindElement(pushTextAreas[i], "Button Push/Select", FULL);
                MetaContextElementUtils.SimpleSetText(messageSelectElements[i], "Text", pushMessageList[i], CHILDREN);

                int _select = i;
                MetaContextElementUtils.SimpleSetClickable(
                    pushTextAreas[i],
                    "Button Push",
                    () => Select(_select));
            }

            Select(0);

            // Send
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Button Send/Text", "POPUP_LEADER_PUSH_SEND", FULL);
            MetaContextElementUtils.SimpleSetClickable(
                root, "Button Send",
                gameObject, MetaEventDefine.ON_META_UI_EVENT, SEND_LEADER_PUSH_EVENT);

            // Info
            MetaContextElementUtils.SimpleSetTextGlobal(
                pushListElement, "Button Question Area/Button Question/Text", "BUTTON_COMMON_INFORMATION", FULL);
            MetaContextElementUtils.SimpleSetClickable(
                pushListElement,
                "Button Question Area/Button Question",
                gameObject, MetaEventDefine.ON_META_UI_EVENT, OPEN_INFORMATION_EVENT,
                false, true, FULL);

            InitEvents();

#if UNITY_EDITOR
            isStreamingAssetLoaded = true;
#else
            StartCoroutine(LoadStreamingAsset());
#endif
        }

        private void InitEvents()
        {
            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);

            var sendTrigger = new EventTrigger(this, MetaEventDefine.ON_META_UI_EVENT, SEND_LEADER_PUSH_EVENT);
            var informationTrigger = new EventTrigger(this, MetaEventDefine.ON_META_UI_EVENT, OPEN_INFORMATION_EVENT);
            RegisterHandlingEvent(sendTrigger, SendCoroutine);
            RegisterHandlingEvent(informationTrigger, OpenInfomationPopupCoroutine);
        }

        private void Select(int _select)
        {
            select = _select;
            for (int i = 0; i < MESSAGE_LIST_COUNT; ++i)
            {
                MetaContextElementUtils.SetActive(messageDefaultElements[i], i != select);
                MetaContextElementUtils.SetActive(messageSelectElements[i], i == select);
            }
        }

        private IEnumerator LoadStreamingAsset()
        {
            string bundle = CAPTAINSCALL_STREAMING_ASSET_NAME;

            var bundleList = BlackboardQueryUtils.GetUsingMetaAssetBundles();
            if (bundleList != null && bundleList.Contains(bundle))
            {
                isStreamingAssetLoaded = true;
                yield break;
            }

            var loadOperation = AssetBundleManager.LoadAssetBundleInternal(bundle);
            yield return new WaitUntil(() => loadOperation != null && loadOperation.IsDone());

            BlackboardQueryUtils.AddUsingMetaAssetBundle(bundle);

            isStreamingAssetLoaded = true;
        }

        private IEnumerator SendCoroutine()
        {
            bool success = false;
            bool failure = false;

            // Loading
            GameObject loadingPopupObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenLoadingPopupCoroutine(
                (GameObject popupObj) => loadingPopupObj = popupObj));

            MetaObjectUtils.SetCalleeCaller(loadingPopupObj, gameObject);

            BagelCodeClientAPI.RequestClubLeaderPush(
                select,
                BiEventUtils.GenerateContextID(),
                (response) =>
                {
                    leaderPushInfo.AddVariable("lastPushTimestamp", TimeUtils.GetTimeStamp());
                    EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_CLUB_FEED_REFRESH);
                    EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.UPDATE_LEADER_PUSH_STATE_EVENT);

                    success = true;
                },
                (error) =>
                {
                    switch(error.errorCode)
                    {
                        case ClientModels.Error.NOT_ENOUGH_CLUB_LEVEL_ERROR:
                            GlobalErrorHandler.OpenAlertPopup("ERROR_SOMETHING_WRONG");
                            break;
                        default:
                            GlobalErrorHandler.GlobalError(error);
                            break;
                    }


                    failure = true;
                });

            yield return new WaitUntil(() => success || failure);

            MetaPopupUtils.ClosePopup(loadingPopupObj);

            ClosePopup();
        }

        private IEnumerator OpenInfomationPopupCoroutine()
        {
            yield return new WaitUntil(() => isStreamingAssetLoaded);

            int INFO_PAGE_COUNT = 2;
            yield return StartCoroutine(MetaPopupUtils.OpenCommonInformationPopupCoroutine(
                this,
                INFO_PAGE_COUNT,
                "Captain's Call Information Page {0:00}",
                "POPUP_LEADER_PUSH_INFORMATION_TEXT_{0}",
                CAPTAINSCALL_STREAMING_ASSET_NAME));
        }

        private void ClosePopup()
        {
            if (isFromIam)
            {
                EventSender.SendCalleeCallback(gameObject, IAMUtils.ON_IAM_CALLBACK_EVENT);
            }
            else
            {
                EventSender.SendCalleeCallback(gameObject);
            }

            MetaPopupUtils.ClosePopup(gameObject);
        }

        private List<string> GetPushMessageList()
        {
            isFromIam = bb.GetVariable<bool>("fromIam")?.value ?? false;
            if (isFromIam) // from IAM
            {
                // Get caller from InAppMessageManager(IAMRouter)
                var iamBB = IAMRouter.Instance.bb;
                var caller = iamBB.GetValue<GameObject>("caller");
                MetaObjectUtils.SetCalleeCaller(gameObject, caller);
                Blackboard callerBB = caller.GetComponent<Blackboard>();

                leaderPushInfo = BlackboardUtils.FindVariable<Blackboard>(callerBB, "clubInfoResponse/leaderPushInfo")?.value;
            }
            else // from Send Button (member, news feed)
            {
                leaderPushInfo = bb.GetVariable<Blackboard>("leaderPushInfo")?.value;
            }

            if(leaderPushInfo == null)
            {
                Debug.LogWarning("PopupLeaderPushController initializing failue. leaderPushInfo is null.");
                ClosePopup();
                return null;
            }

            return leaderPushInfo.GetValue<List<string>>("messageList");
        }
    }
}
