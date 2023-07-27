using BagelCode.ClientModels;
using NodeCanvas.Framework;
using SlotMaker;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public partial class ClubContentsController
    {
        private ContextElement newsFeedLeaderPushButtonElement;

        private ContextElement feedInputElement;

        private ContextElement requestTabButtonElement;
        private ContextElement buttonLayoutAreaElement;
        private ContextElement requestButtonElement;
        private ContextElement requestButtonIconAreaElement;

        private ContextElement sendButtonElement;
        private ContextElement sendButtonTextElement;
        private ContextElement newsFeedCoolTimeButtonElement;
        private ContextElement newsFeedCoolTimeButtonTextElement;

        private bool isInitNewsFeed = false;

        public void OnTabNewsFeed(string fromType)
        {
            var authority = bb.GetValue<ClubAuthority>("_authority");

            Dictionary<string, object> customData = new Dictionary<string, object>();

            customData["type"] = fromType;
            customData["user_authority"] = (long)authority;
            customData["is_leader_push_enabled"] = leaderPushInfoBB.GetVariable<bool>("available")?.value ?? false;

            Analytics.CustomEvent("client_click_club_newsfeed", customData);
        }

        public void LoadNewsFeed()
        {
            SetPageObjActivate();

            MetaContextElementUtils.SimpleSetIntProperty(root, "Bottom Tab", 2, CHILDREN);

            bb.AddVariable("_fromFeedID", 0L);
            bb.AddVariable("_biContextID", BiEventUtils.GenerateContextID());

            // Get Meta Game Info
            BlackboardQueryUtils.GetMetaGamePassiveEvent(false, true, false, false,
                out string bundleName, out _, out _, out EventInfoType eventType, out int eventId,
                out long endTimestamp, out bool enableShare, out _, out _);
            bb.AddVariable("_mgBundleName", bundleName);
            bb.AddVariable("_mgEventType", eventType);
            bb.AddVariable("_mgEventID", eventId);
            bb.AddVariable("_mgEndTimestamp", endTimestamp);
            bb.AddVariable("_isEnableShare", enableShare);

            // Init Properties
            InitNewsFeedProperty();

            // Active Feed Input
            MetaContextElementUtils.SetActive(feedInputElement, true);

            // Update Request Button
            UpdateRequestButton(enableShare, bundleName, eventType, eventId);

            // Set Creator Data
            var dynamicScroll = PageObj.GetComponent<DynamicScrollClubNewsFeedCreator>();
            var clubInfo = bb.GetValue<Blackboard>("clubInfo");
            var authority = bb.GetValue<ClubAuthority>("_authority");
            dynamicScroll.SetClubInfo(clubInfo, authority, bundleName, eventType, eventId);

            // Active Particle
            MetaContextElementUtils.SimpleSetActive(
                root, "News Feed/Button Area/Button Collect All/Particle Collect", false, FULL);

            // Set News Feed Tab 0
            bb.AddVariable("collectCredit", false);
            bb.AddVariable("newsfeedTabIndex", 0);
            var newsFeedElement = pageDataDict[BottomState.NEWS_FEED].root;
            MetaContextElementUtils.SetIntProperty(newsFeedElement, 0);

            UpdateLeaderPushButtonNewsFeed();
        }

        private void InitNewsFeedProperty()
        {
            if (isInitNewsFeed) return;

            feedInputElement = ContextUtils.FindElement(root, "Post Input", CHILDREN);
            var newsFeedElement = ContextUtils.FindElement(root, "News Feed", CHILDREN);

            // Request
            var collectAllButtonElement = ContextUtils.FindElement(newsFeedElement, "Button Area/Button Collect All", FULL);
            bb.AddVariable("_collectAllButton", collectAllButtonElement);
            var newsfeedTabElement = ContextUtils.FindElement(newsFeedElement, "Filter", CHILDREN);
            bb.AddVariable("_newsfeedTabElement", newsfeedTabElement);
            requestTabButtonElement = ContextUtils.FindElement(newsfeedTabElement, "Tab Requests", CHILDREN);
            buttonLayoutAreaElement = ContextUtils.FindElement(feedInputElement, "Button Layout", CHILDREN);
            requestButtonElement = ContextUtils.FindElement(feedInputElement, "Button Layout/Button Request", FULL);
            requestButtonIconAreaElement = ContextUtils.FindElement(requestButtonElement, "Meta Game Item Area", CHILDREN);

            MetaContextElementUtils.SetClickable(
                collectAllButtonElement,
                gameObject,
                "OnClubFeedCollectAll",
                false);

            MetaContextElementUtils.SetClickable(
                newsfeedTabElement,
                gameObject,
                "OnChangeNewsfeedTab",
                false);

            MetaContextElementUtils.SetClickable(
                requestButtonElement,
                gameObject,
                "OnRequestClubShare",
                false);

            var collectAllButtonTopText01Element = ContextUtils.FindElement(collectAllButtonElement, "Text 01", FULL);
            MetaContextElementUtils.SetTextGlobal(collectAllButtonTopText01Element, "BUTTON_COLLECT_ALL_COIN");
            var collectAllButtonTopText02Element = ContextUtils.FindElement(collectAllButtonElement, "Text 02", FULL);
            MetaContextElementUtils.SetText(collectAllButtonTopText02Element, "0");

            var requestButtonActiveTextElement = ContextUtils.FindElement(requestButtonElement, "Active/Text", CHILDREN);
            var requestButtonInactiveTextElement = ContextUtils.FindElement(requestButtonElement, "Inactive/Text", CHILDREN);
            MetaContextElementUtils.SetTextGlobal(requestButtonActiveTextElement, "BUTTON_REQUEST_SHARE_ITEM");
            MetaContextElementUtils.SetTextGlobal(requestButtonInactiveTextElement, "BUTTON_REQUEST_SHARE_ITEM");

            // Leader Push
            newsFeedLeaderPushButtonElement = ContextUtils.FindElement(feedInputElement, "Button Layout 2", FULL);

            var openLeaderPushPopupButtonElement = ContextUtils.FindElement(newsFeedLeaderPushButtonElement, "Button Send", CHILDREN);
            MetaContextElementUtils.SetClickable(
                openLeaderPushPopupButtonElement,
                () => {
                    EventSender.SendEvent(gameObject, MetaEventDefine.ON_META_UI_EVENT,
                new ParadoxNotion.EventData<string>(MetaEventDefine.ON_CLICK_LEADER_PUSH_OPEN, "newsfeed"));
                });

            sendButtonElement = ContextUtils.FindElement(newsFeedLeaderPushButtonElement, "Button Send", CHILDREN);
            sendButtonTextElement = ContextUtils.FindElement(sendButtonElement, "Text", CHILDREN);
            newsFeedCoolTimeButtonElement = ContextUtils.FindElement(newsFeedLeaderPushButtonElement, "Button Send Cool Time", CHILDREN);
            newsFeedCoolTimeButtonTextElement = ContextUtils.FindElement(newsFeedCoolTimeButtonElement, "Text", CHILDREN);
            // newsFeedCoolTimeButtonElement.GetComponent<PIDButton>().interactable = false;

            MetaContextElementUtils.SetClickable(
                newsFeedCoolTimeButtonElement,
                () => {
                    EventSender.SendEvent(gameObject, MetaEventDefine.ON_META_UI_EVENT,
                new ParadoxNotion.EventData<string>(MetaEventDefine.ON_CLICK_LEADER_PUSH_OPEN, "newsfeed"));
                });

            isInitNewsFeed = true;
        }

        private void UpdateRequestButton(bool enableShare, string bundleName, EventInfoType eventType, int _eventId)
        {
            bool enableRequest = enableShare;
            GameObject dataObj = null;
            int eventId = _eventId;

            if (enableRequest)
            {
#if USE_ASSETBUNDLE
                var bundle = AssetBundleManager.GetLoadedAssetBundle(bundleName);
                enableRequest = bundle != null;
#endif
                if (enableRequest)
                {
                    switch (eventType)
                    {
                        case EventInfoType.COLLECTING_GAME:
                            if (dataObj == null)
                                dataObj = MetaObjectUtils.MakePrefab(bundleName, "Data", root.transform, null, null);
                            break;
#if UNITY_EDITOR
                        default:
                            Debug.LogError("Make Data Prefab failure.");
                            break;
#endif
                    }

                    if (dataObj == null)
                    {
                        eventId = 0;
                        enableRequest = false;
                    }
                }
                else
                {
                    eventId = 0;
                    enableRequest = false;
                }
            }

            buttonLayoutAreaElement.gameObject.SetActive(enableRequest);

            bool isRequestTabEnabled = BlackboardQueryUtils.IsHiddenObjectsEnabled() || enableRequest;
            requestTabButtonElement.gameObject.SetActive(isRequestTabEnabled);

            if (enableRequest)
            {
                MetaObjectUtils.MakePrefab(bundleName, "Share Icon", requestButtonIconAreaElement.transform, null, null);
                MetaContextElementUtils.SetBooleanProperty(requestButtonElement, false);
                MetaContextElementUtils.SimpleSetActive(requestButtonElement, "Loading Area", true);
            }

            bb.AddVariable("_mgEventID", eventId);
            bb.AddVariable("isEnableRequest", enableRequest);
        }

        private void UpdateLeaderPushButtonNewsFeed() // called every seconds
        {
            if (BottomState != BottomState.NEWS_FEED) return;

            if (leaderPushInfoBB == null) return;

            bool exist = LeaderPushExist;
            bool available = LeaderPushAvailable;

            MetaContextElementUtils.SetActive(newsFeedLeaderPushButtonElement, exist);
            if (exist)
            {
                MetaContextElementUtils.SetActive(sendButtonElement, available);
                MetaContextElementUtils.SetActive(newsFeedCoolTimeButtonElement, !available);

                if (available)
                {
                    string textKey = "BUTTON_NEWS_FEED_LEADER_PUSH_SEND";
                    MetaContextElementUtils.SetTextGlobal(sendButtonTextElement, textKey);
                }
                else
                {
                    long remaining = (long)ClubUtils.GetLeaderPushSendCoolTimeRemaining(leaderPushInfoBB);
                    string text = StringTableUtils.GetString(GLOBAL, "BUTTON_NEWS_FEED_LEADER_PUSH_COOL_TIME", remaining);
                    MetaContextElementUtils.SetText(newsFeedCoolTimeButtonTextElement, text);
                }
            }
        }
    }
}
