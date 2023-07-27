using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;
using SlotMaker;
using System.Collections.Generic;
using System.Linq;
using BagelCode.ClientModels;
using Sirenix.OdinInspector;
using ParadoxNotion;

using static BagelCode.BottomState;

namespace BagelCode
{
    public partial class ClubContentsController : EventMonoBehaviour
    {
        public BottomState BottomState => bb.GetValue<BottomState>("bottomState");

        public GameObject PageObj => pageDataDict[BottomState].obj;
        public ContextElement PageElement => pageDataDict[BottomState].root;
        public Animator PageAnim => pageDataDict[BottomState].anim;

        private Blackboard bb;
        private ContextElement root;

        private Dictionary<BottomState, ClubPageData> pageDataDict = new Dictionary<BottomState, ClubPageData>();

        private Blackboard clubInfoBB;
        private List<Blackboard> memberBBList;
        private int clubLevel;
        private long clubId;

        private bool isMyClub;
        private bool isPopup;
        private bool isLeagueResult;
        private bool isLeaguePopup;

        public Blackboard leaderPushInfoBB;
        public Blackboard clubArenaRewardInfoBB;
        public Blackboard bossRaidersRewardInfoBB;

        public bool LeaderPushExist => leaderPushInfoBB.GetValue<bool>("exist");
        public bool LeaderPushAvailable => leaderPushInfoBB.GetValue<bool>("available");

        private bool isInit = false;

        private Coroutine repeatUpdateCoroutine;

        private ContextElement joinButtonAreaElement;
        private ContextElement joinButtonElement;
        private ContextElement joinButtonTextElement;
        private ContextElement cooltimeButtonElement;
        private ContextElement cooltimeButtonTextElement;
        private ContextElement joinRestrictionTextElement;
        private ContextElement tabMetaButtonElement;
        private ContextElement metaButtonAreaElement;
        private bool updateJoinCooltime = false;

        // Join Button State
        const string JOINABLE = "joinable";
        const string RESTRICTED = "restricted";
        const string COOLTIME = "cooltime";

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private const StringTable.StringTableType GLOBAL = StringTable.StringTableType.Global;

        public void InitProperty()
        {
            if (!isInit)
            {
                bb = GetComponent<Blackboard>();
                root = GetComponent<ContextElement>();

                root.UpdateContext(false);

                isLeaguePopup = bb.GetVariable<bool>("isLeaguePopup")?.value ?? false;

                InitPageData();

                InitClubBaseContextElements();

                InitEvents();

                isInit = true;
            }

            InitPage();
        }

        private void InitClubBaseContextElements()
        {
            // Join Button
            joinButtonAreaElement = ContextUtils.FindElement(root, "Button Area", CHILDREN);
            MetaContextElementUtils.SetActive(joinButtonAreaElement, !isLeaguePopup);
            if (!isLeaguePopup)
            {
                joinButtonElement = ContextUtils.FindElement(joinButtonAreaElement, "Button Join", CHILDREN);
                joinButtonTextElement = ContextUtils.FindElement(joinButtonElement, "Text", CHILDREN);

                joinRestrictionTextElement = ContextUtils.FindElement(joinButtonAreaElement, "Text Full", CHILDREN);

                cooltimeButtonElement = ContextUtils.FindElement(joinButtonAreaElement, "Button Cool Time", CHILDREN);
                cooltimeButtonTextElement = ContextUtils.FindElement(cooltimeButtonElement, "Text", CHILDREN);
                cooltimeButtonElement.GetComponent<PIDButton>().interactable = false;

                MetaContextElementUtils.SetClickable(joinButtonElement, gameObject, "OnJoin", false);
            }
            // Tab Meta Button
            ContextElement bottomTabElement = ContextUtils.FindElement(root, "Bottom Tab", CHILDREN);
            tabMetaButtonElement = ContextUtils.FindElement(bottomTabElement, "Tab Meta", CHILDREN);
            metaButtonAreaElement = ContextUtils.FindElement(tabMetaButtonElement, "Meta Button Area", CHILDREN);
        }

        public void SetBottomState(BottomState _state)
        {
            bb.AddVariable("bottomState", _state);
        }

        public void SetPageObjLoading(bool animState)
        {
            PageAnim.SetBool("IsLoading", animState);
        }

        public void SetPageObjActivate()
        {
            pageDataDict.Values.ToList().ForEach(p => p.obj.SetActive(false));
            PageObj.SetActive(true);
        }

        public void OnUpdateClubInfo()
        {
            leaderPushInfoBB = BlackboardUtils.FindVariable<Blackboard>(bb, "clubInfoResponse/leaderPushInfo")?.value;
            clubInfoBB = bb.GetValue<Blackboard>("clubInfo");
            memberBBList = BlackboardUtils.FindVariable<List<Blackboard>>(bb, "clubInfoResponse/clubMemberList")?.value;
            clubLevel = clubInfoBB.GetValue<int>("level");
            clubId = clubInfoBB.GetValue<long>("id");
            clubArenaRewardInfoBB = BlackboardUtils.FindVariable<Blackboard>(bb, "clubInfoResponse/clubArenaRewardPopup")?.value;
            bossRaidersRewardInfoBB = BlackboardUtils.FindVariable<Blackboard>(bb, "clubInfoResponse/bossRaidersRewardPopup")?.value;

            isMyClub = bb.GetValue<bool>("_isMyClub");
            isPopup = bb.GetVariable<bool>("isPopup")?.value ?? false;
            isLeagueResult = bb.GetValue<bool>("_isLeagueResult");

            var clubJoinType = clubInfoBB.GetValue<ClubJoinType>("joinType");
            bb.AddVariable("_clubJoinType", clubJoinType);

            InitEvents();

            UpdateLeaderPushState();

            UpdateJoinButton();

            if (repeatUpdateCoroutine != null)
                StopCoroutine(repeatUpdateCoroutine);

            repeatUpdateCoroutine = StartCoroutine(RepeatUpdateCoroutine());
        }

        public void OnRefreshClubInfo(float delay)
        {
            StartCoroutine(OnRefreshClubInfoCoroutine(delay));
        }

        private IEnumerator OnRefreshClubInfoCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT,
                new ParadoxNotion.EventData(MetaEventDefine.ON_REFRESH_CLUB_INFO));
        }

        //

        private void InitEvents()
        {
            // Clear Registered Events
            UnRegisterAll();

            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);

            Register(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.UPDATE_LEADER_PUSH_STATE_EVENT, UpdateLeaderPushState);
            Register(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.UPDATE_LEADER_PUSH_BUTTON_EVENT, UpdateLeaderPushButtonNewsFeed);

            Register(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_CLICK_LEADER_PUSH_OPEN, OnClickLeaderPushOpen);
        }

        private IEnumerator RepeatUpdateCoroutine() // todo remaining timer 쓰도록 수정
        {
            float INTERVAL = 1f;
            while (true)
            {
                if (!enabled) continue;

                yield return new WaitForSeconds(INTERVAL);

                CheckLeaderPushState();

                UpdateLeaderPushButtonNewsFeed();

                if (updateJoinCooltime)
                    UpdateJoinButton();
            }
        }

        //

        private void InitPage()
        {
            var _state = MEMBER;

            if (isLeaguePopup) _state = LEAGUE;

            SetBottomState(_state);
            SetPageObjActivate();
            SetPageObjLoading(true);
        }

        private void InitPageData()
        {
            var leagueObj = bb.GetValue<GameObject>("leagueView");
            var memberObj = bb.GetValue<GameObject>("scrollView");
            var newsFeedObj = bb.GetValue<GameObject>("newsFeedScrollView");
            var challengeObj = bb.GetValue<GameObject>("challengeView");

            pageDataDict.Add(LEAGUE, new ClubPageDataLeague(leagueObj));
            pageDataDict.Add(MEMBER, new ClubPageDataMember(memberObj));
            pageDataDict.Add(NEWS_FEED, new ClubPageDataNewsFeed(newsFeedObj));
            pageDataDict.Add(CHALLENGE, new ClubPageDataChallenge(challengeObj));
        }

        private void UpdateJoinButton()
        {
            MetaContextElementUtils.SetActive(joinButtonAreaElement, !isMyClub);

            if (isLeaguePopup && isMyClub) return;

            updateJoinCooltime = false;

            bb.AddVariable("uiJoinType", "");

            // Is Clubber?
            long meClubId = BlackboardUtils.FindVariable<long>("/me/clubId")?.value ?? 0L;
            if (meClubId > 0L)
            {
                SetJoinButtonState("", "");
                return;
            }

            // Check Cooltime
            string fromType = bb.GetVariable<string>("buttonFromType")?.value ?? "";
            var clubJoinType = clubInfoBB.GetValue<ClubJoinType>("joinType");
            long cooltimeMs = BlackboardUtils.FindValue<long>("/values/misc/CLUB_REJOIN_COOLTIME_MS");
            long lastLeaveTimestamp = BlackboardUtils.FindValue<long>("/lastClubLeaveTimestamp");
            long elapsed = TimeUtils.GetTimeStamp() - lastLeaveTimestamp;
            long remaining = System.Math.Max(cooltimeMs - elapsed, 0L) / 1000L;
            if (remaining > 0L)
            {
                string key;
                if (fromType == "FROM_INVITE")
                    key = "BUTTON_ACCEPT_COOL_TIME";
                else if (clubJoinType == ClubJoinType.PRIVATE)
                    key = "BUTTON_REQUEST_TO_JOIN_COOL_TIME";
                else
                    key = "BUTTON_JOIN_COOL_TIME";

                string text = StringTableUtils.GetString(GLOBAL, key, remaining);

                updateJoinCooltime = true;

                SetJoinButtonState(COOLTIME, text);
                return;
            }

            // Feature Locked?
            bool isLocked = BlackboardQueryUtils.IsLockedFeature(LockedFeatureType.CLUB);
            if (isLocked)
            {
                int minLevel = BlackboardQueryUtils.GetFeatureMinLevel(LockedFeatureType.CLUB);
                string text = StringTableUtils.GetString(GLOBAL, "CLUB_INFO_LEVEL_RESTRICTION", minLevel);
                SetJoinButtonState(RESTRICTED, text);
                return;
            }

            // Is Already Requested?
            var userClubRequestedId = BlackboardUtils.FindValue<long>("/userClubStateInfo/requestedClubId");
            var userClubState = BlackboardUtils.FindValue<UserClubState>("/userClubStateInfo/userClubState");
            if (userClubState == UserClubState.REQUESTED && clubId == userClubRequestedId)
            {
                string text = StringTableUtils.GetString(GLOBAL, "CLUB_INFO_REQUESTED");
                SetJoinButtonState(RESTRICTED, text);
                return;
            }

            // Check From Type
            if (fromType == "FROM_INVITE")
            {
                bb.AddVariable("uiJoinType", "invite");
                string text = StringTableUtils.GetString(GLOBAL, "BUTTON_ACCEPT");
                SetJoinButtonState(JOINABLE, text);
                return;
            }

            // Check Min Level
            int clubMinLevel = clubInfoBB.GetValue<int>("minPlayerLevel");
            int playerLevel = BlackboardUtils.FindValue<int>("/me/level");
            if (clubMinLevel > playerLevel)
            {
                string text = StringTableUtils.GetString(GLOBAL, "CLUB_INFO_LEVEL_RESTRICTION", clubMinLevel);
                SetJoinButtonState(RESTRICTED, text);
                return;
            }

            // Private Club?
            if (clubJoinType == ClubJoinType.PRIVATE)
            {
                bb.AddVariable("uiJoinType", "request");
                string text = StringTableUtils.GetString(GLOBAL, "BUTTON_REQUEST_TO_JOIN");
                SetJoinButtonState(JOINABLE, text);
                return;
            }
            else
            {
                // Club Member Full?
                int maxMemberCount = ClubUtils.GetClubMaxMemberCount(clubLevel);
                int memberCount = memberBBList == null ? 0 : memberBBList.Count;
                if (memberCount >= maxMemberCount)
                {
                    // Request reservation.
                    bb.AddVariable("uiJoinType", "request");
                    string text = StringTableUtils.GetString(GLOBAL, "BUTTON_REQUEST_TO_JOIN");
                    SetJoinButtonState(JOINABLE, text);
                    return;
                    // string text = StringTableUtils.GetString(GLOBAL, "CLUB_INFO_MEMBER_FULL");
                    // SetJoinButtonState(RESTRICTED, text);
                    // return;
                }
            }

            // Default
            {
                bb.AddVariable("uiJoinType", "default");
                string text = StringTableUtils.GetString(GLOBAL, "BUTTON_JOIN");
                SetJoinButtonState(JOINABLE, text);
                return;
            }
        }

        private void SetJoinButtonState(string state, string text)
        {
            // Button Active
            MetaContextElementUtils.SetActive(joinButtonElement, state == JOINABLE);
            MetaContextElementUtils.SetActive(joinRestrictionTextElement, state == RESTRICTED);
            MetaContextElementUtils.SetActive(cooltimeButtonElement, state == COOLTIME);

            switch (state)
            {
                case JOINABLE:
                    MetaContextElementUtils.SetText(joinButtonTextElement, text);
                    break;
                case RESTRICTED:
                    MetaContextElementUtils.SetText(joinRestrictionTextElement, text);
                    break;
                case COOLTIME:
                    MetaContextElementUtils.SetText(cooltimeButtonTextElement, text);
                    break;
                default:
                    MetaContextElementUtils.SetActive(joinButtonAreaElement, false);
                    break;
            }
        }

        private void UpdateLeaderPushState()
        {
            if (ApplicationSettings.LogTest())
                Debug.Log("UpdateLeaderPushState");

            if (leaderPushInfoBB == null) return;

            bool Exist()
            {
                var authority = bb.GetVariable<ClubAuthority>("_authority")?.value ?? ClubAuthority.UNKNOWN;
                if (authority != ClubAuthority.LEADER) return false;

                if (clubLevel < ClubDefine.LEADER_PUSH_REQUIRED_CLUB_LEVEL)
                    return false;

                if (!bb.GetValue<bool>("_isMyClub"))
                    return false;

                return true;
            }

            leaderPushInfoBB.AddVariable("exist", Exist());

            bool Available()
            {
                if (!LeaderPushExist)
                    return false;

                long lastPushTimestamp = leaderPushInfoBB.GetValue<long>("lastPushTimestamp");
                long elapsed = TimeUtils.GetTimeStamp() - lastPushTimestamp;

                // Cool Time
                long cooltimeMs = leaderPushInfoBB.GetValue<long>("cooltimeMs");
                if (elapsed < cooltimeMs) return false;

                return true;
            }

            leaderPushInfoBB.AddVariable("available", Available());

            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.UPDATE_LEADER_PUSH_BUTTON_EVENT);
        }

        private void CheckLeaderPushState()
        {
            if (leaderPushInfoBB != null)
            {
                if (!LeaderPushAvailable && LeaderPushExist)
                {
                    long lastPushTimestamp = leaderPushInfoBB.GetValue<long>("lastPushTimestamp");
                    long elapsed = TimeUtils.GetTimeStamp() - lastPushTimestamp;

                    // Cool Time
                    long cooltimeMs = leaderPushInfoBB.GetValue<long>("cooltimeMs");
                    if (elapsed >= cooltimeMs)
                    {
                        UpdateLeaderPushState();
                    }
                }
            }
        }

        public void CheckActiveBottomMetaIcon()
        {
            EventInfo eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo();
            bool isLockedFeature = BlackboardQueryUtils.IsLockedFeature(LockedFeatureType.META_GAME);
            bool isActive = false;

            if (eventInfo != null && !isLockedFeature)
            {
                isActive = CheckActiveClubMetaGame(eventInfo.type);
                if (isActive == true)
                {
                    string assetName = GetAssetNameClubMetaGame(eventInfo);
                    if (!string.IsNullOrEmpty(assetName))
                        isActive = CreateBottomMetaIcon(assetName, eventInfo);
                }
            }

            tabMetaButtonElement.gameObject.SetActive(isActive);
        }

        private string GetContextId()
        {
            var biContextId = BlackboardUtils.GetOrCreateVariable<string>(bb, "_biContextID");
            if (string.IsNullOrEmpty(biContextId.value))
            {
                biContextId.value = BiEventUtils.GenerateContextID();
                bb.SetValue("_biContextID", biContextId.value);
            }
            return biContextId.value;
        }

        #region Popups

        public IEnumerator OpenClubEntryPopupsCoroutine()
        {
            // isPopup=true: profile, league popup
            if (!isPopup)
            {
                // League Result
                if (IsLeagueResult())
                    yield return StartCoroutine(OpenLeagueResultPopupCoroutine());

                // League Popup
                if (IsLeaguePopupEnabled())
                    yield return StartCoroutine(OpenClubLeaguePopupCoroutine());

                // Level Up
                if (IsLevelUp())
                    yield return StartCoroutine(OpenLevelUpPopupCoroutine());

                // Unlock Leader Push IAM
                if (CheckLeaderPushUnlocked())
                    yield return StartCoroutine(TriggerUnlockLeaderPushIamCoroutine());

                // Boss Raiders End Notice Popup
                if (IsMetaGameNoticeEnabled(ClubUtils.ResultPopupMetaGameName.BOSS_RAIDERS, bossRaidersRewardInfoBB))
                    yield return StartCoroutine(OpenMetaGameNoticeMyPopupCoroutine(ClubUtils.ResultPopupMetaGameName.BOSS_RAIDERS, bossRaidersRewardInfoBB, "Popup Boss Raiders End Notice My Scene"));

                // Club Arena End Notice Popup
                if (IsMetaGameNoticeEnabled(ClubUtils.ResultPopupMetaGameName.CLUB_ARENA, clubArenaRewardInfoBB))
                    yield return StartCoroutine(OpenMetaGameNoticeMyPopupCoroutine(ClubUtils.ResultPopupMetaGameName.CLUB_ARENA, clubArenaRewardInfoBB, "Popup Club Arena End Notice My Scene"));

                // Lobby To Club IAM
                TriggerLobbyToClubIam();
            }
        }

        private void TriggerLobbyToClubIam()
        {
            if (ApplicationSettings.LogTest())
                Debug.Log("ClubContentsController.TriggerLobbyToClubIamCoroutine");

            IAMRouter.Instance.TriggerIAM(InAppMessageTriggerType.ENTER_CLUB_FROM_LOBBY, gameObject, null);
        }

        private IEnumerator OpenLeagueResultPopupCoroutine()
        {
            if (ApplicationSettings.LogTest())
                Debug.Log("ClubContentsController.OpenLeagueResultPopupCoroutine");

            Blackboard clubInfoResponse = bb.GetVariable<Blackboard>("clubInfoResponse").value;
            Blackboard leagueResultInfo = clubInfoResponse.GetVariable<Blackboard>("leagueRewardPopup").value;

            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Popup Club League Result Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject leagueResultPopupObj = null;
            yield return StartCoroutine(MetaObjectUtils.MakeSceneCoroutine(bundle, asset, parent,
                (GameObject popupObj) => leagueResultPopupObj = popupObj));

            Blackboard popupBB = leagueResultPopupObj.GetComponent<Blackboard>();
            popupBB.AddVariable("leagueResultInfo", leagueResultInfo);

            MetaObjectUtils.SetCalleeCaller(leagueResultPopupObj, gameObject);

            MetaPopupUtils.OpenPopup(leagueResultPopupObj);

            var callbackTrigger = new EventTrigger(this, EventSender.ON_CALLEE_CALLBACK);
            yield return new WaitUntilTrigger(callbackTrigger);

            bb.SetValue("_isLeagueResult", false);
            isLeagueResult = false;
        }

        private IEnumerator OpenClubLeaguePopupCoroutine()
        {
            if (ApplicationSettings.LogTest())
                Debug.Log("ClubContentsController.OpenClubLeaguePopupCoroutine");

            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Popup Club League Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject clubLeaguePopupObj = null;
            yield return StartCoroutine(MetaObjectUtils.MakeSceneCoroutine(bundle, asset, parent,
                (GameObject popupObj) => clubLeaguePopupObj = popupObj));

            MetaObjectUtils.SetCalleeCaller(clubLeaguePopupObj, gameObject);

            var popupElement = clubLeaguePopupObj.GetComponent<ContextElement>();
            popupElement.UpdateContext(false);
            var popupContentsElement = ContextUtils.FindElement(popupElement, "Club Contents", FULL);
            var contentsBB = popupContentsElement.GetComponent<Blackboard>();

            var popupBB = clubLeaguePopupObj.GetComponent<Blackboard>();
            var clubInfoResponse = BlackboardUtils.FindVariable<Blackboard>(bb, "clubInfoResponse");
            popupBB.AddVariable("clubID", clubId);
            contentsBB.AddVariable("clubID", clubId);
            contentsBB.AddVariable("clubInfoResponse", clubInfoResponse.value);
            contentsBB.AddVariable("isLeaguePopup", true);

            MetaPopupUtils.OpenPopup(clubLeaguePopupObj);

            PlayerPrefsUtils.SetInt64(ClubDefine.PLAYER_PREFS_LAST_CLUB_CHECKED_MS, TimeUtils.GetTimeStamp());

            var callbackTrigger = new EventTrigger(this, EventSender.ON_CALLEE_CALLBACK);
            yield return new WaitUntilTrigger(callbackTrigger);
        }

        private bool IsLeagueResult()
        {
            if (!isMyClub) return false;

            return isLeagueResult;
        }

        private bool IsLeaguePopupEnabled()
        {
            if (!isMyClub) return false;

            if (PlayerPrefs.HasKey(ClubDefine.PLAYER_PREFS_LAST_CLUB_CHECKED_MS))
            {
                long currentTime = TimeUtils.GetTimeStamp();
                long lastChecked = PlayerPrefsUtils.GetInt64(ClubDefine.PLAYER_PREFS_LAST_CLUB_CHECKED_MS, currentTime);
                long elapsed = currentTime - lastChecked;

                return elapsed > ClubDefine.LEAGUE_POPUP_COOL_TIME_MS;
            }
            else
            {
#if DEV
                PlayerPrefsUtils.SetInt64(ClubDefine.PLAYER_PREFS_LAST_CLUB_CHECKED_MS, 0L);
#else
                PlayerPrefsUtils.SetInt64(ClubDefine.PLAYER_PREFS_LAST_CLUB_CHECKED_MS, TimeUtils.GetTimeStamp());
#endif
            }

            return false;
        }

        private IEnumerator OpenMetaGameNoticeMyPopupCoroutine(ClubUtils.ResultPopupMetaGameName metaGameType, Blackboard infoBB, string assetName)
        {
            if (ApplicationSettings.LogTest())
                Debug.Log("ClubContentsController.OpenClubArenaNoticePopupMyCoroutine");

            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = assetName;
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject popupObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(bundle, asset, parent,
                (SceneLoadOperation sceneLoadOperation) => popupObj = sceneLoadOperation.GetScene()));

            var popupBB = popupObj.GetComponent<Blackboard>();
            popupBB.AddVariable("rewardInfo", infoBB);
            popupBB.AddVariable("clubInfo", clubInfoBB);
            popupBB.AddVariable("metaGameType", metaGameType);

            MetaObjectUtils.SetCalleeCaller(popupObj, gameObject);

            MetaPopupUtils.OpenPopup(popupObj);

            ClubUtils.SetClubMetaGamePlayerPrefs(metaGameType, infoBB.GetValue<int>("eventId"));

            var callbackTrigger = new EventTrigger(this, EventSender.ON_CALLEE_CALLBACK);
            yield return new WaitUntilTrigger(callbackTrigger);
        }

        private IEnumerator OpenLevelUpPopupCoroutine()
        {
            if (ApplicationSettings.LogTest())
                Debug.Log("ClubContentsController.OpenLevelUpPopupCoroutine");

            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Popup Club Level Up New Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject popupObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(bundle, asset, parent,
                (SceneLoadOperation sceneLoadOperation) => popupObj = sceneLoadOperation.GetScene()));

            var popupBB = popupObj.GetComponent<Blackboard>();
            popupBB.AddVariable("clubInfo", clubInfoBB);

            MetaObjectUtils.SetCalleeCaller(popupObj, gameObject);

            MetaPopupUtils.OpenPopup(popupObj);

            var callbackTrigger = new EventTrigger(this, EventSender.ON_CALLEE_CALLBACK);
            yield return new WaitUntilTrigger(callbackTrigger);
        }

        private bool IsLevelUp()
        {
            if (!isMyClub) return false;

            int prevLevel = ClubUtils.GetClubPlayerPrefs(ClubDefine.PLAYER_PREFS_CLUB_LEVEL, clubId, 0);
            ClubUtils.SetClubPlayerPrefs(ClubDefine.PLAYER_PREFS_CLUB_LEVEL, clubId, clubLevel);

            if (prevLevel > 0 && clubLevel > 1)
            {
                return clubLevel > prevLevel;
            }

            return false;
        }

        private IEnumerator TriggerUnlockLeaderPushIamCoroutine()
        {
            if (ApplicationSettings.LogTest())
                Debug.Log("ClubContentsController.TriggerUnlockLeaderPushIamCoroutine");

            bool isTriggered = IAMRouter.Instance.TriggerIAM(
                InAppMessageTriggerType.ENTER_CLUB_FROM_LOBBY_WITH_LEADER_PUSH_UNLOCKED, gameObject, null);

            if (isTriggered)
            {
                ClubUtils.SetClubPlayerPrefs(ClubDefine.PLAYER_PREFS_UNLOCK_LEADER_PUSH_POPUP_SHOWN, clubId, 1);
                var iamCallbackTrigger = new EventTrigger(this, IAMUtils.ON_IAM_CALLBACK_EVENT);
                yield return new WaitUntilTrigger(iamCallbackTrigger);
            }
        }

        private bool CheckLeaderPushUnlocked()
        {
            if (!isMyClub) return false;

            bool isUnlocked = ClubUtils.GetClubPlayerPrefs(ClubDefine.PLAYER_PREFS_UNLOCK_LEADER_PUSH_POPUP_SHOWN, clubId) == 0;
            return isUnlocked && leaderPushInfoBB != null && leaderPushInfoBB.GetValue<bool>("available");
        }

        private void OnClickLeaderPushOpen(EventData eventData)
        {
            int clickCount = ClubUtils.GetClubPlayerPrefs(ClubDefine.PLAYER_PREFS_LEADER_PUSH_CLICK_COUNT, clubId);
            int maxClickCount = ClubDefine.LEADER_PUSH_UNLOCK_CLICK_MAX;

            bool available = LeaderPushAvailable;
            if (LeaderPushAvailable)
            {
                if (ApplicationSettings.LogTest())
                    Debug.Log("client_click_club_leader_push_icon bi event sent.");

                // Send BI
                var customData = new Dictionary<string, object>();
                customData["context_id"] = BiEventUtils.GenerateContextID();
                customData["cooltime_finish"] = 0L;
                customData["last_push_ts"] = 0L;
                customData["status"] = available ? "unlock" : "lock";
                customData["leader_push_icon_result"] = "succeed";
                customData["retry_count"] = clickCount;
                customData["trigger_type"] = (string)eventData.value;
                Analytics.CustomEvent("client_click_club_leader_push_icon", customData);

                ClubUtils.SetClubPlayerPrefs(ClubDefine.PLAYER_PREFS_LEADER_PUSH_CLICK_COUNT, clubId, 0);

                MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData("OpenLeaderPushPopup"));
            }
            else
            {
                if (clickCount < maxClickCount)
                {
                    if (ApplicationSettings.LogTest())
                        Debug.Log("client_click_club_leader_push_icon bi event sent.");

                    // Send BI
                    long remaining = ClubUtils.GetLeaderPushSendCoolTimeRemainingLong(leaderPushInfoBB);
                    long lastPushTimestamp = leaderPushInfoBB.GetValue<long>("lastPushTimestamp");

                    var customData = new Dictionary<string, object>();
                    customData["context_id"] = BiEventUtils.GenerateContextID();
                    customData["cooltime_finish"] = remaining;
                    customData["last_push_ts"] = lastPushTimestamp;
                    customData["status"] = available ? "unlock" : "lock";
                    customData["leader_push_icon_result"] = "failed";
                    customData["retry_count"] = clickCount;
                    customData["trigger_type"] = eventData.value;
                    Analytics.CustomEvent("client_click_club_leader_push_icon", customData);

                    ClubUtils.SetClubPlayerPrefs(ClubDefine.PLAYER_PREFS_LEADER_PUSH_CLICK_COUNT, clubId, clickCount + 1);
                }
            }
        }

        public IEnumerator OpenLeaderPushPopupCoroutine()
        {
            if (ApplicationSettings.LogTest())
                Debug.Log("ClubContentsController.OpenLeaderPushPopup");

            if (leaderPushInfoBB == null ||
                !LeaderPushExist ||
                !LeaderPushAvailable) yield break;

            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Popup Captain's Call Push Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject popupObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(bundle, asset, parent,
                (SceneLoadOperation sceneLoadOperation) => popupObj = sceneLoadOperation.GetScene()));

            var popupBB = popupObj.GetComponent<Blackboard>();
            popupBB.AddVariable("leaderPushInfo", leaderPushInfoBB);

            MetaObjectUtils.SetCalleeCaller(popupObj, gameObject);

            MetaPopupUtils.OpenPopup(popupObj);

            var callbackTrigger = new EventTrigger(this, EventSender.ON_CALLEE_CALLBACK);
            yield return new WaitUntilTrigger(callbackTrigger);

            UpdateLeaderPushState();
        }

        private bool IsMetaGameNoticeEnabled(ClubUtils.ResultPopupMetaGameName metaGameType, Blackboard infoBB)
        {
            if (!isMyClub) return false;
            if (infoBB == null) return false;

            int eventId = infoBB.GetVariable<int>("eventId")?.value ?? 0;

            if (PlayerPrefs.HasKey(ClubUtils.GetClubMetaGamePlayerPrefsName(metaGameType)))
            {
                int lastEventId = ClubUtils.GetClubMetaGamePlayerPrefs(metaGameType, eventId);
                return eventId != lastEventId;
            }
            else
            {
#if DEV
                ClubUtils.SetClubMetaGamePlayerPrefs(metaGameType, 0);
#else
                ClubUtils.SetClubMetaGamePlayerPrefs(metaGameType, eventId);
#endif
            }

            return false;
        }

        #endregion

#if UNITY_EDITOR
        #region Test

        [Button]
        private void TestClearPlayerPrefs()
        {
            PlayerPrefsUtils.SetInt64(ClubDefine.PLAYER_PREFS_LAST_CLUB_CHECKED_MS, 0L);
            ClubUtils.SetClubPlayerPrefs(ClubDefine.PLAYER_PREFS_CLUB_LEVEL, clubId, -1);
            ClubUtils.SetClubPlayerPrefs(ClubDefine.PLAYER_PREFS_UNLOCK_LEADER_PUSH_POPUP_SHOWN, clubId, 0);
            ClubUtils.ResetClubMetaGamePlayerPrefs();
        }

        [Button]
        private void TestOpenEntryPopups()
        {
            StartCoroutine(OpenClubEntryPopupsCoroutine());
        }

        [Button]
        private void TestOpenClubLeaguePopup()
        {
            StartCoroutine(OpenClubLeaguePopupCoroutine());
        }

        [Button]
        private void TestOpenLevelUpPopup()
        {
            StartCoroutine(OpenLevelUpPopupCoroutine());
        }

        [Button]
        private IEnumerator TestOpenLeaderPushPopupCoroutine()
        {
            if (ApplicationSettings.LogTest())
                Debug.Log("ClubContentsController.OpenLeaderPushPopup");

            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Popup Captain's Call Push Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject popupObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(bundle, asset, parent,
                (SceneLoadOperation sceneLoadOperation) => popupObj = sceneLoadOperation.GetScene()));

            var popupBB = popupObj.GetComponent<Blackboard>();
            popupBB.AddVariable("leaderPushInfo", leaderPushInfoBB);

            MetaPopupUtils.OpenPopup(popupObj);
        }

        [Button]
        private void TestOpenClubArenaNoticeMyPopup()
        {
            var responseBB = BlackboardUtils.GetOrCreateBlackboard(bb, "clubInfoResponse");
            clubArenaRewardInfoBB = (Blackboard)BlackboardUtils.GetOrCreateBlackboard(responseBB, "clubArenaRewardPopup");
            BlackboardUtils.SetOrCreateValue<long>(clubArenaRewardInfoBB, "point", 1234567890);
            BlackboardUtils.SetOrCreateValue<long>(clubArenaRewardInfoBB, "gem", 1000);
            BlackboardUtils.SetOrCreateValue<int>(clubArenaRewardInfoBB, "rank", 11);
            BlackboardUtils.SetOrCreateValue<int>(clubArenaRewardInfoBB, "percentile", 5);
            BlackboardUtils.SetOrCreateValue<string>(clubArenaRewardInfoBB, "backgroundImageUrl", "https://cdn.bagelgames.com/SLOTS1/images/club_arena/tinify/Club Arena End Notice My.png");
            StartCoroutine(OpenMetaGameNoticeMyPopupCoroutine(ClubUtils.ResultPopupMetaGameName.CLUB_ARENA, clubArenaRewardInfoBB, "Popup Club Arena End Notice My Scene"));
        }
#endregion
#endif
    }
}
