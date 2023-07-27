using System.Collections.Generic;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;
using BagelCode.ClientModels;
using BagelCode.OSA_Scroll;
using System.Collections;

using static BagelCode.InboxEvent;
using ParadoxNotion;

namespace BagelCode
{
    public class InboxController : MonoBehaviour
    {
        public static bool IsAcceptable
        {
            get => isAcceptable;
            set
            {
                isAcceptable = value;
                instance.rootBB.SetValue("isAcceptable", isAcceptable);
            }
        }
        private static bool isAcceptable = false;

        public static InboxController instance;

        public InboxRectController rectController;

        private ContextElement root;
        private Blackboard rootBB;
        private Animator anim;

        // Context
        private ContextElement inboxEmptyElement;
        private ContextElement buttonAreaElement;
        private ContextElement collectAllBaseGrayElement;
        private ContextElement collectAllBaseGreenElement;
        private ContextElement collectAllCreditElement;
        private ContextElement collectAllInformation;
        private ContextElement scrollElement;

        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;
        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;

        private const string IS_LOADING = "IsLoading";

        private bool isInit = false;

        //

        private void OnDestroy()
        {
            MetaSystem.UnSubscribeBackButton(gameObject.GetHashCode());
        }

        public IEnumerator InitCoroutine()
        {
            instance = this;

            InitProperty();

            rootBB.AddVariable("isAcceptable", false);
            IsAcceptable = false;

            yield return StartCoroutine(RequestInboxListCoroutine());

            anim.SetBool(IS_LOADING, false);

            MetaContextElementUtils.SetActive(scrollElement, true);

            RefreshInbox();

            InitEvents();
        }

        public IEnumerator ReloadInboxCoroutine()
        {
            yield return new WaitUntilTrigger(new WaitUntilConditionTrigger(() => IsAcceptable));

            anim.SetBool(IS_LOADING, true);

            yield return StartCoroutine(RequestInboxListCoroutine());

            scrollElement.GetComponent<OSA_InboxItems>().Refresh();

            RefreshInbox();

            anim.SetBool(IS_LOADING, false);
        }

        public IEnumerator InboxCollectAllCoroutine()
        {
            if (!IsAcceptable) yield break;
            IsAcceptable = false;

            var collectAllEnabled = BlackboardUtils.FindVariable<bool>(rootBB, "/collectAllEnabled");
            if (collectAllEnabled != null && collectAllEnabled.value)
            {
                var collectAllCredit = BlackboardUtils.FindVariable<long>(rootBB, "/collectAllCredit");
                if (collectAllCredit != null && collectAllCredit.value > 0)
                {

                    anim.SetBool(IS_LOADING, true);

                    yield return StartCoroutine(RequestCollectAllCoroutine());

                    yield return StartCoroutine(RequestInboxListCoroutine());

                    scrollElement.GetComponent<OSA_InboxItems>().CreateItemList();

                    GSManager.Instance.GetHandler("UI_Coin_Add").Play();
                    ContextElement collectAllEffectElement = ContextUtils.FindElement(root, "Button Area/Button Collect All/Particle Collect", FULL);
                    MetaContextElementUtils.SetActive(collectAllEffectElement, true);
                    anim.SetBool(IS_LOADING, false);

                    yield return new WaitForSeconds(0.6f);
                }

                RefreshInbox();
            }
            else
            {
                Animator buttonAnim = collectAllInformation.GetComponent<Animator>();
                buttonAnim.SetTrigger("Appear");
            }

            IsAcceptable = true;
        }

        public void RefreshInbox()
        {
            root.UpdateContext(false);

            var inboxList = BlackboardUtils.FindValue<List<Blackboard>>("/inboxList");
            var inboxBannerList = BlackboardUtils.FindValue<List<Blackboard>>("/inboxBannerList");
            List<Blackboard> bucksGiftList = BlackboardQueryUtils.GetBucksGiftList();

            // Banner
            BlackboardQueryUtils.RemoveWatchedInboxItem(inboxBannerList);
            UpdateBanner();

            // Empty Object
            bool isActiveEmpty = inboxList.IsEmpty() && inboxBannerList.IsEmpty() && (bucksGiftList == null || bucksGiftList.Count == 0);
            MetaContextElementUtils.SetActive(inboxEmptyElement, isActiveEmpty);

            // Collect All Credit
            var collectAllCredit = BlackboardUtils.FindVariable<long>(rootBB, "/collectAllCredit")?.value ?? 0;
            Animator buttonAnim = buttonAreaElement.GetComponent<Animator>();
            if (collectAllCredit > 0)
            {
                var collectAllEnabled = BlackboardUtils.FindVariable<bool>(rootBB, "/collectAllEnabled")?.value ?? false;
                MetaContextElementUtils.SetActive(collectAllBaseGreenElement, collectAllEnabled);
                MetaContextElementUtils.SetActive(collectAllBaseGrayElement, !collectAllEnabled);

                IContextText collectAllCreditText = collectAllCreditElement as IContextText;
                if (collectAllEnabled)
                {
                    collectAllCreditText.SetGlobalText("BUTTON_COLLECT_ALL_COIN_COLLECT", collectAllCredit);
                }
                else
                {
                    collectAllCreditText.SetGlobalText("BUTTON_COLLECT_ALL_COIN_LOCKED", collectAllCredit);
                }
                buttonAnim.SetBool("IsCollectAll", true);
            }
            else
            {
                buttonAnim.SetBool("IsCollectAll", false);
            }

            IsAcceptable = true;
        }

        public void CloseInbox()
        {
            BlackboardUtils.DestroyBlackboardList(MainBlackboard.Get(), "inboxResponse");
            anim.SetTrigger("Close");
        }

        public IEnumerator AcceptNextInboxItemCoroutine(InboxCellController cellController, bool isAuto = false)
        {
            // Wait Inbox Acceptable
            var inboxAcceptableTrigger = new EventTrigger(cellController, MetaEventDefine.ON_META_UI_EVENT, ON_INBOX_ACCEPTABLE);
            yield return new WaitUntilTrigger(inboxAcceptableTrigger);

            EventSender.SendEvent(cellController.gameObject, new EventData<bool>(ON_ACCEPT, isAuto));
        }

        private void InitProperty()
        {
            if (isInit) return;

            root = GetComponent<ContextElement>();
            rootBB = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();

            anim.SetBool("Active", true);
            anim.SetBool(IS_LOADING, true);

            root.UpdateContext(true);

            inboxEmptyElement = ContextUtils.FindElement(root, "Inbox Empty", CHILDREN);
            buttonAreaElement = ContextUtils.FindElement(root, "Button Area", CHILDREN);
            collectAllBaseGrayElement = ContextUtils.FindElement(root, "Button Area/Button Collect All/Base Gray", FULL);
            collectAllBaseGreenElement = ContextUtils.FindElement(root, "Button Area/Button Collect All/Base Green", FULL);
            collectAllCreditElement = ContextUtils.FindElement(root, "Button Area/Button Collect All/Text 02", FULL);
            collectAllInformation = ContextUtils.FindElement(root, "Button Area/Button Collect All/Information", FULL);
            scrollElement = ContextUtils.FindElement(root, "Inbox Scroll Rect", CHILDREN);

            rectController = scrollElement.GetComponent<InboxRectController>();

            // Tab
            MetaContextElementUtils.SimpleSetIntProperty(root, "Tab Area", 0, CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Tab Area/Tab Inbox/Tab Text", "INBOX_TAB_TEXT", FULL);

            // Collect All
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Button Area/Button Collect All/Text 01", "BUTTON_COLLECT_ALL_COIN", FULL);
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Button Area/Button Collect All/Text 02", "BUTTON_COLLECT_ALL_COIN_COLLECT", FULL, 0);
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Button Area/Button Collect All/Information/Text", "INBOX_COLLECT_ALL_BALLOON", FULL, 0);

            isInit = true;
        }

        private void InitEvents()
        {
            // Close
            MetaContextElementUtils.SimpleSetClickable(root, "Button Close", () => EventSender.SendEvent(gameObject, CLOSE_INBOX));

            MetaSystem.SubscribeBackButton(gameObject.GetHashCode(), () => EventSender.SendEvent(gameObject, CLOSE_INBOX));

            // Collect All
            MetaContextElementUtils.SimpleSetClickable(root, "Button Area/Button Collect All",
                () => EventSender.SendEvent(gameObject, ON_INBOX_COLLECT_ALL), true, FULL);
        }

        public void ActionRefreshInbox()
        {
            RefreshInbox();
            IsAcceptable = true;
        }

        //

        private IEnumerator RequestInboxListCoroutine()
        {
            bool isSuccessOrFailure = false;
            BagelCodeClientAPI.InboxList(
            (response) =>
            {
                BlackboardUtils.DestroyBlackboardList(MainBlackboard.Get(), "inboxList");
                BlackboardUtils.DestroyBlackboardList(MainBlackboard.Get(), "inboxBannerList");
                ClientAPI2Blackboard.Serialize(MainBlackboard.Get(), response);
                BlackboardQueryUtils.UpdateCollectAllCredit();

                isSuccessOrFailure = true;
            },
            (error) =>
            {
                Debug.Log("Error : " + error.errorCode.ToString());
                GlobalErrorHandler.GlobalError(error);

                isSuccessOrFailure = true;
            });

            yield return new WaitUntil(() => isSuccessOrFailure);
        }

        private IEnumerator RequestCollectAllCoroutine()
        {
            int maxId = BlackboardQueryUtils.GetMaxInboxId();

            bool isSuccessOrFailure = false;
            BagelCodeClientAPI.InboxCollectAllRequest(maxId,
                (response) =>
                {
                    BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
                    BlackboardQueryUtils.ApplyUserSyncInfo();

                    isSuccessOrFailure = true;
                },
                (error) =>
                {
                    Debug.Log("Error : " + error.errorCode.ToString());
                    GlobalErrorHandler.GlobalError(error);

                    isSuccessOrFailure = true;
                });

            yield return new WaitUntil(() => isSuccessOrFailure);
        }

        public void UpdateBanner()
        {
            List<Blackboard> bannerList = new List<Blackboard>();

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            if(!BlackboardQueryUtils.UserOptionsPushNotification() && !NativeHelper.Instance.GetPushNotificationSubscribed())
            {
                bannerList = BlackboardQueryUtils.GetNoticeListFromType(NoticeTypes.INBOX_PUSH_OFF);
            }
            else if (!NativeHelper.Instance.GetPushNotificationSubscribed())
            {
                bannerList = BlackboardQueryUtils.GetNoticeListFromType(NoticeTypes.INBOX_PUSH_DEVICE_OFF);
            }
            else if (!BlackboardQueryUtils.UserOptionsPushNotification())
            {
                bannerList = BlackboardQueryUtils.GetNoticeListFromType(NoticeTypes.INBOX_PUSH_SETTING_OFF);
            }
#endif
            bannerList.AddRange( BlackboardQueryUtils.GetNoticeListFromType(NoticeTypes.INBOX) );

            ContextElement bannerElement = ContextUtils.FindElement(root, "Inbox Banner", CHILDREN);

            if (bannerList.Count >= 1)
            {
                Blackboard noticeInfo = GetBannerNoticeInfo(bannerList);
                if (noticeInfo != null)
                {
                    var bannerBB = bannerElement.GetComponent<Blackboard>();
                    bannerBB.AddVariable("noticeInfo", noticeInfo);
                    bannerBB.AddVariable("caller", gameObject);

                    BlackboardQueryUtils.SetCooltime(noticeInfo);

                    bannerElement.gameObject.SetActive(true);
                }
                else
                    bannerElement.gameObject.SetActive(false);
            }
            else
            {
                bannerElement.gameObject.SetActive(false);
            }
        }

        private Blackboard GetBannerNoticeInfo(List<Blackboard> bannerList)
        {
            if (bannerList == null || bannerList.Count == 0)
                return null;

            for (int i = 0; i < bannerList.Count; ++i)
            {
                Blackboard noticeInfo = bannerList[i];
                int maxExposureCount = BlackboardUtils.FindValue<int>(noticeInfo, "constraints/maxExposureCount");
                if (maxExposureCount > 0)
                {
                    int noticeId = BlackboardUtils.FindValue<int>(noticeInfo, "id");
                    string triggeredMaxExposureKey = string.Format(BlackboardQueryUtils.noticeExposureCountKey, noticeId);
                    int prefsMaxExposureCount = PlayerPrefs.GetInt(triggeredMaxExposureKey, 0);

                    if (prefsMaxExposureCount < maxExposureCount)
                    {
                        // check endTimestamp
                        long endTimestamp = BlackboardQueryUtils.GetNoticeEndTimestamp(noticeInfo);
                        if (endTimestamp == 0 || endTimestamp > TimeUtils.GetTimeStamp())
                        {
                            PlayerPrefs.SetInt(triggeredMaxExposureKey, prefsMaxExposureCount + 1);
                            return noticeInfo;
                        }
                        else
                            continue;
                    }
                    else
                        continue;
                }
                else
                    return noticeInfo;
            }

            return null;
        }
    }
}
