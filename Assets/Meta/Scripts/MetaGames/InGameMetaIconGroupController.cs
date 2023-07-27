using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using ParadoxNotion;
using BagelCode.ClientModels;
using System.Linq;
using NodeCanvas.Framework;

namespace BagelCode
{
    public class InGameMetaIconGroupController : EventMonoSingleton<InGameMetaIconGroupController>
    {
        //
        public bool isItemEarning = false;
        //

        private const int MAX_BUTTON_COUNT = 7;
        private const long GROUP_INTERACTING_COOLDOWN_MS = 300L;

        private ContextElement root;
        private Animator anim;

        private ContextElement openButtonElement;
        private ContextElement closeButtonElement;
        private ContextElement horizontalGroupElement;

        private ContextElement[] buttonAreaElements = new ContextElement[MAX_BUTTON_COUNT];

        private bool isOpen = true;

        [SerializeField]
        private List<EventButtonInfo> eventButtonInfoList = new List<EventButtonInfo>();

        private ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;

        private Coroutine earnMetaGameItemCoroutine = null;

        private bool isReady = false;
        private long lastGroupInteractingTimestamp = 0L;

        private bool IS_GROUP_INTERACTABLE => TimeUtils.GetTimeStamp() - lastGroupInteractingTimestamp > GROUP_INTERACTING_COOLDOWN_MS;

        [System.Serializable]
        private class EventButtonInfo
        {
            public GameObject eventButtonObj;
            public MetaGameType type;
            public bool isEnabled;

            public bool isBetEnough = true;
            public bool isUnlockedLevel = true;
            public EventInfo eventInfo = null;
            public GameObject inGameButtonObj = null;
            public long minEligibleBet = -1L;
            public int internalPriority = 0; // 작을수록 앞
        }

        private enum CheckMetaGameResultState
        {
            NONE = 0,
            REMOVED, // Removed & Make New = Removed
            MAKE_NEW,
        }

#if DEV
        private int eligibleIndex = 0;

        [Sirenix.OdinInspector.Button]
        public void TestUpdateButtonOrder()
        {
            UpdateButtonOrder();
        }

        [Sirenix.OdinInspector.Button]
        public void TestAddButton()
        {
            eventButtonInfoList.Add(new EventButtonInfo()
            {
                type = MetaGameType.LEVEL_UP_DASH,
                isEnabled = true,
            });
            UpdateButtonOrder();
        }

        [Sirenix.OdinInspector.Button]
        public void TestEarnMetaGameItem(int index)
        {
            var info = eventButtonInfoList[index];
            earnMetaGameItemCoroutine = StartCoroutine(EarnMetaGameItemCoroutine(info));
        }
#endif

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            root = GetComponent<ContextElement>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            openButtonElement = ContextUtils.FindElement(root, "Open Button", CHILDREN);
            closeButtonElement = ContextUtils.FindElement(root, "Close Button", CHILDREN);
            horizontalGroupElement = ContextUtils.FindElement(root, "Event Button Horizontal Group", CHILDREN);

            for(int i = 0; i < MAX_BUTTON_COUNT; ++i)
            {
                string elementName = string.Format("Event Button Area {0}", i + 1);
                buttonAreaElements[i] = ContextUtils.FindElement(horizontalGroupElement, elementName, CHILDREN);
            }

            MetaContextElementUtils.SetClickable(openButtonElement, OnClickOpenButton);
            MetaContextElementUtils.SetClickable(closeButtonElement, OnClickCloseButton);

            // Position
            if (OrientationUtils.Instance.PossibleChangeOrientation() && BlackboardQueryUtils.GetOrientation() == Orientation.PORTRAIT)
            {
                var rectTransform = GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(60f, -200f);
            }

            // Update All
            CheckEventMetaGame();
            CheckStaticMetaGame(); // hog, vip, level dash, epic pass always
            UpdateButtonOrder();
            OpenGroup(true);

            // Init Events
            RegisterHandleEventType(MetaEventDefine.ON_PASSIVE_EVENT);
            RegisterHandleEventType(MetaEventDefine.ON_CREDIT_EVENT);
            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);
            RegisterHandleEventType(ContentEvent.ON_CONTENT_EVENT);

            Register(MetaEventDefine.ON_PASSIVE_EVENT, MetaEventDefine.START_PASSIVE_EVENT, OnUpdatePassiveEvent);
            Register(MetaEventDefine.ON_PASSIVE_EVENT, MetaEventDefine.REFRESH_PASSIVE_EVENT, OnUpdatePassiveEvent);

            Register(MetaEventDefine.ON_CREDIT_EVENT, "UpdatedTotalBetCredit", OnChangeBet);

            Register(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_BEGIN_META_GAME, OnBeginMetaGame);
            Register(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_END_TURN_META, OnEndTurnMetaGame);
            Register(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_LEVEL_UP, OnLevelUp);

            Register(ContentEvent.ON_CONTENT_EVENT, ContentEvent.ON_END_TURN_EVENT, OnEndTurn);
            Register(ContentEvent.ON_CONTENT_EVENT, ContentEvent.ON_BEGIN_TURN_EVENT, OnBeginTurn);

            Register(MetaEventDefine.ON_META_UI_EVENT, "OnBeginTutorial", HideGroup);
            Register(MetaEventDefine.ON_META_UI_EVENT, "OnEndTutorial", ShowGroup);
            Register(MetaEventDefine.ON_META_UI_EVENT, "FeatureUnlocked", OnFeatureUnlocked);
            Register(MetaEventDefine.ON_META_UI_EVENT, "OnClickInGameMetaGameIcon", OnClickInGameMetaGameIcon);

            Register(MetaEventDefine.ON_META_UI_EVENT, LevelUpDash.LevelUpDash.Events.ON_UPDATE_LEVEL_UP_DASH, UpdateLevelUpDashState);
            Register(MetaEventDefine.ON_META_UI_EVENT, VipLounge.VipLounge.Events.ON_UPDATE_VIP_LOUNGE_INFO, OnUpdateVipLoungeState);

            Register("OnMakeEventButton", OnMakeEventButton);

            Register(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ACTIVE_META_UI, ActiveMetaUI);
        }

        private void OnUpdateVipLoungeState()
        {
            UpdateButtonOrder();
        }

        private void OnLevelUp()
        {
            CheckEventMetaGame();
            CheckStaticMetaGame();
            UpdateLevelLocked();
            UpdateButtonOrder();
        }

        private void OnBeginMetaGame()
        {
            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_END_META_GAME);
            return;
        }

        private void OnMakeEventButton(EventData eventData)
        {
            var eventButtonObj = (GameObject)eventData.value;
            var eventButtonBB = eventButtonObj.GetComponent<Blackboard>();
            var eventButtonCaller = BlackboardUtils.GetOrCreateVariable<GameObject>(eventButtonBB, "caller")?.value;

            // Search
            for (int i = 0; i < eventButtonInfoList.Count; ++i)
            {
                var info = eventButtonInfoList[i];
                if (info.eventButtonObj == eventButtonCaller)
                {
                    info.inGameButtonObj = eventButtonObj;
                    info.minEligibleBet = BlackboardQueryUtils.GetMinEligibleBet(info.type);
                }
            }

            // meta game data가 필요한 요소는 In Game Button이 생성된 후 업데이트
            UpdateLevelLocked();
            UpdateButtonBetEnough();
            UpdateButtonOrder();
        }

        private void OpenGroup(bool _isOpen, bool isManually = false)
        {
            if (isOpen == _isOpen && IS_GROUP_INTERACTABLE) return;

            isOpen = _isOpen;
            lastGroupInteractingTimestamp = TimeUtils.GetTimeStamp();

            ForceStopEarningEffects();
            ApplyEarningItemInstantly();

            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT,
                isOpen ? MetaEventDefine.ON_OPEN_META_EVENT_GROUP : MetaEventDefine.ON_CLOSE_META_EVENT_GROUP);

            // Update Active
            UpdateButtonActive();
            anim.SetBool("isActive", isOpen);

            // Update blockraycast
            for (int i = 0; i < buttonAreaElements.Length; ++i)
            {
                buttonAreaElements[i].GetComponent<UnityEngine.CanvasGroup>().blocksRaycasts = _isOpen || i == 0;
            }

            // AE
            if (isManually)
            {
                var customData = new Dictionary<string, object>();
                customData["meta_list"] = GetMetaList();
                customData["type"] = isOpen ? "open" : "close";
                customData["target"] = null;
                customData["context_id"] = null;
                Analytics.CustomEvent("client_click_meta_collection", customData);
            }
        }

        private void OnClickInGameMetaGameIcon(EventData eventData)
        {
            if (eventData == null || eventData.value == null) return;

            var inGameObj = eventData.value as GameObject;
            StartCoroutine(SendAECoroutine(inGameObj));
        }

        private IEnumerator SendAECoroutine(GameObject inGameObj)
        {
            var inGameBB = inGameObj.GetComponent<Blackboard>();
            var info = eventButtonInfoList.Find(l => l.inGameButtonObj == inGameObj);
            if (info == null) yield break;

            MetaGameType type = info.type;

            string contextId = "";
            if(type == MetaGameType.BOSS_RAIDERS ||
                type == MetaGameType.CLUB_ARENA ||
                type == MetaGameType.SEASON_PASS)
            {
                // Icon 에서 만든 contextId 사용
                Variable<string> _biContextID = BlackboardUtils.GetOrCreateVariable<string>(inGameBB, "_biContextID");
                yield return new WaitUntil(() =>
                {
                    return _biContextID != null && !string.IsNullOrEmpty(_biContextID.value);
                });
                contextId = _biContextID.value;
            }
            else
            {
                contextId = BiEventUtils.GenerateContextID();
            }

            if(type == MetaGameType.LEVEL_UP_DASH)
            {
                var levelDashMainBB = LevelUpDash.MetaSystemLevelUpDashController.Instance.GetComponent<Blackboard>();
                BlackboardUtils.SetOrCreateValue(levelDashMainBB, "metaGroupContextId", contextId);
            }

            BlackboardUtils.SetOrCreateValue(inGameBB, "metaGroupContextId", contextId);

            // Send AE
            var customData = new Dictionary<string, object>();
            customData["meta_list"] = GetMetaList();
            customData["type"] = "click";
            customData["target"] = GetMetaName(type);
            customData["context_id"] = contextId;
            Analytics.CustomEvent("client_click_meta_collection", customData);
        }

        private string GetMetaList()
        {
            var metaListDict = new Dictionary<string, string>();
            int counter = 1;
            for(int i = 0; i < eventButtonInfoList.Count;++i)
            {
                var info = eventButtonInfoList[i];
                if (info.isEnabled)
                {
                    string typeString = GetMetaName(info.type);
                    metaListDict.Add(counter.ToString(), typeString);

                    counter++;
                }
            }

            return SlotMaker.Json.SlotSimpleJson.SerializeObject(metaListDict);
        }

        private string GetMetaName(MetaGameType type)
        {
            return TextDecoUtils.EnumTypeToText<MetaGameType>((int)type, TextDecoUtils.TextFormat.LOWER);
        }

        private void HideGroup()
        {
            anim.SetBool("isShow", false);
        }

        private void ShowGroup()
        {
            if (!BlackboardQueryUtils.IsActiveTutorial() && IsExistEnabled())
                anim.SetBool("isShow", true);
        }

        private bool IsExistEnabled()
        {
            return eventButtonInfoList.Any(i => i.isEnabled);
        }

        private void OnFeatureUnlocked(EventData eventData)
        {
            if(eventData.value is LockedFeatureType lockedFeatureType &&
                lockedFeatureType == LockedFeatureType.META_GAME)
            {
                for(int i = 0; i < eventButtonInfoList.Count;++i)
                {
                    var info = eventButtonInfoList[i];

                    if (info.isEnabled)
                    {
                        var eventButtonObj = info.eventButtonObj;
                        EventSender.SendEvent(eventButtonObj, "OnUnlocked");
                    }
                }
            }
        }

        private CheckMetaGameResultState CheckLevelUpDash()
        {
            bool isActive = LevelUpDash.LevelUpDash.Utils.IsActiveLevelUpDash();
            if (isActive)
            {
                var levelDashInfo = eventButtonInfoList.FirstOrDefault(i => i.type == MetaGameType.LEVEL_UP_DASH);
                if (levelDashInfo == null)
                {
                    eventButtonInfoList.Add(new EventButtonInfo()
                    {
                        type = MetaGameType.LEVEL_UP_DASH,
                        isEnabled = true,
                    });
                    return CheckMetaGameResultState.MAKE_NEW;
                }
            }
            else if(RemoveEventButton(MetaGameType.LEVEL_UP_DASH))
            {
                return CheckMetaGameResultState.REMOVED;
            }

            return CheckMetaGameResultState.NONE;
        }

        private CheckMetaGameResultState CheckStaticMetaGame()
        {
            CheckMetaGameResultState result = CheckMetaGameResultState.NONE;

            // vip
            bool isVipLoungeEnabled = BlackboardQueryUtils.IsVipLoungeEnabled();
            if (isVipLoungeEnabled)
            {
                int level = BlackboardQueryUtils.GetMyLevel();
                int minLevel = BlackboardQueryUtils.GetFeatureMinLevel(LockedFeatureType.VIP_LOUNGE);

                bool isUnlockedLevel = level >= minLevel;

                var vdsInfo = eventButtonInfoList.FirstOrDefault(i => i.type == MetaGameType.BUILD_DREAM_SEASON);
                if (vdsInfo == null)
                {
                    if (isUnlockedLevel)
                    {
                        // Vegas Dream
                        eventButtonInfoList.Add(new EventButtonInfo()
                        {
                            type = MetaGameType.BUILD_DREAM_SEASON,
                            isEnabled = BlackboardQueryUtils.IsVegasDreamsActive(),
                            isUnlockedLevel = isUnlockedLevel,
                        });
                        BlackboardQueryUtils.SetVipLoungeEnabled(true);

                        result = CheckMetaGameResultState.MAKE_NEW;
                    }
                }
                else
                {
                    vdsInfo.isEnabled = BlackboardQueryUtils.IsVegasDreamsActive();
                    vdsInfo.isUnlockedLevel = isUnlockedLevel;
                }
            }

            // epic pass always
            bool isEpicPassAlwaysActived = BlackboardQueryUtils.IsEpicPassAlwaysActive();
            if (isEpicPassAlwaysActived)
            {
                int level = BlackboardQueryUtils.GetMyLevel();
                int minLevel = BlackboardQueryUtils.GetFeatureMinLevel(LockedFeatureType.META_GAME);
                bool enoughLevel = level >= minLevel;

                var epicPassAlwaysInfo = eventButtonInfoList.FirstOrDefault(i => i.type == MetaGameType.SEASON_PASS_V2);
                if (epicPassAlwaysInfo == null)
                {
                    if (!BlackboardQueryUtils.IsLockedFeature(LockedFeatureType.META_GAME) || MetaGameUtils.IsMetaEventLevelLock())
                    {
                        eventButtonInfoList.Add(new EventButtonInfo()
                        {
                            type = MetaGameType.SEASON_PASS_V2,
                            isEnabled = true,
                            isUnlockedLevel = enoughLevel,
                        });
                        result = CheckMetaGameResultState.MAKE_NEW;
                    }
                }
                else
                {
                    epicPassAlwaysInfo.isUnlockedLevel = enoughLevel;
                }
            }

            // hog
            bool isHogActived = BlackboardQueryUtils.IsHiddenObjectsActive();
            if (isHogActived)
            {
                int level = BlackboardQueryUtils.GetMyLevel();
                int minLevel = BlackboardQueryUtils.GetFeatureMinLevel(LockedFeatureType.HIDDEN_UNIVERSE);
                bool enoughLevel = level >= minLevel;

                var hogInfo = eventButtonInfoList.FirstOrDefault(i => i.type == MetaGameType.HIDDEN_OBJECTS);
                if (hogInfo == null)
                {
                    eventButtonInfoList.Add(new EventButtonInfo()
                    {
                        type = MetaGameType.HIDDEN_OBJECTS,
                        isEnabled = true,
                        isUnlockedLevel = enoughLevel,
                    });

                    result = CheckMetaGameResultState.MAKE_NEW;
                }
                else
                {
                    hogInfo.isUnlockedLevel = enoughLevel;
                }
            }

            // level dash
            var levelDashState = CheckLevelUpDash();
            result = GetState(result, levelDashState);

            return result;
        }

        private CheckMetaGameResultState GetState(CheckMetaGameResultState a, CheckMetaGameResultState b)
        {
            if (a == CheckMetaGameResultState.REMOVED || b == CheckMetaGameResultState.REMOVED)
                return CheckMetaGameResultState.REMOVED;

            if (a == CheckMetaGameResultState.MAKE_NEW || b == CheckMetaGameResultState.MAKE_NEW)
                return CheckMetaGameResultState.MAKE_NEW;

            return CheckMetaGameResultState.NONE;
        }

        private void OnChangeBet()
        {
            OpenGroup(false);

            UpdateButtonBetEnough();
            UpdateButtonOrder();
        }

        private void UpdateLevelUpDashState()
        {
            var result = CheckLevelUpDash();
            if (result != CheckMetaGameResultState.NONE)
            {
                OpenGroup(false);
                UpdateButtonOrder();
            }
        }

        private void UpdateLevelLocked()
        {
            for(int i = 0; i < eventButtonInfoList.Count;++i)
            {
                var info = eventButtonInfoList[i];
                if(info.isEnabled && info.inGameButtonObj != null)
                {
                    info.isUnlockedLevel = BlackboardQueryUtils.IsMetaGameUnlocked(info.type);
                }
            }
        }

        private void UpdateButtonBetEnough()
        {
            long totalBet = BlackboardQueryUtils.GetTotalBet();
            for (int i = 0; i < eventButtonInfoList.Count; ++i)
            {
                var info = eventButtonInfoList[i];
                if (info.isEnabled && info.inGameButtonObj != null)
                {
                    long minBet = info.minEligibleBet;
                    info.isBetEnough = minBet > -1L && totalBet >= minBet;
                }
            }
        }

        private bool IsButtonUnlocked(EventButtonInfo info)
        {
            if(info != null && info.type != MetaGameType.NONE)
            {
                bool forceLocked = BlackboardQueryUtils.IsMetaButtonForceLocked(info.type);

                return !forceLocked && info.isBetEnough && info.isUnlockedLevel;
            }

            return false;
        }

        private string GetMetaGameButtonPrefabName(MetaGameType type)
        {
            bool isPassiveMetaGame = BlackboardQueryUtils.IsPassiveMetaGame(type, out bool isOtherMetaGame);
            if (type == MetaGameType.HIDDEN_OBJECTS)
            {
                return "Hidden Objects Event Button In Game";
            }
            else if (type == MetaGameType.BUILD_DREAM_SEASON)
            {
                return "Vegas Dreams Event Button In Game";
            }
            else if(type == MetaGameType.GEM_JACKPOT || type == MetaGameType.GOLDEN_TOWER)
            {
                return "";
            }
            else if(type == MetaGameType.LEVEL_UP_DASH)
            {
                return "Level Up Dash Event Button In Game";
            }
            else if (type == MetaGameType.SEASON_PASS_V2)
            {
                return "Epic Pass Always Event Button In Game";
            }
            else if (isOtherMetaGame)
            {
                return "Other Event Button In Game"; // todo
            }
            else if (isPassiveMetaGame)
            {
                return "Event Button In Game";
            }
            // todo vip icon..

            Debug.LogError("InGameMetaIconGroupController.GetMetaGameButtonPrefabName failure.");
            return "";
        }

        private void UpdateButtonActive()
        {
            bool meetFirstBetLess = false;
            for (int i = 0; i < eventButtonInfoList.Count; ++i)
            {
                var info = eventButtonInfoList[i];
                var buttonObj = info.eventButtonObj;

                if (i >= MAX_BUTTON_COUNT) // over 7
                {
                    if (buttonObj != null)
                        buttonObj.SetActive(false);
                    continue;
                }

                // Active
                bool isActive;
                if (isOpen)
                {
                    isActive = info.isEnabled;
                }
                else
                {
                    isActive = i == 0;
                }

                if (buttonObj != null)
                {
                    buttonObj.SetActive(isActive);

                    // Active Anim
                    var buttonElement = buttonObj.GetComponent<ContextElement>();
                    MetaContextElementUtils.SetPropertySafty(buttonElement, isActive);
                }

                // Bet Up Balloon
                if (isOpen &&
                    info.isEnabled && info.inGameButtonObj != null &&
                    !info.isBetEnough && !meetFirstBetLess)
                {
                    meetFirstBetLess = true;
                    EventSender.SendEvent(info.inGameButtonObj, MetaEventDefine.SHOW_BET_UP_BALLOON);
                }
            }
        }

        private void UpdateButtonOrder()
        {
            // Calc Priority
            foreach(var info in eventButtonInfoList)
            {
                bool enabled = info.isEnabled;
                bool isPassiveMeta = BlackboardQueryUtils.IsPassiveMetaGame(info.type, out _);
                bool isUnlocked = IsButtonUnlocked(info);

                // bigger comes back
                int priority = (int)info.type;
                priority += enabled ? 0 : 100000;
                priority += isUnlocked ? 0 : 10000;
                priority += isPassiveMeta ? 0 : 1000;

                info.internalPriority = priority;
            }

            eventButtonInfoList.Sort((a, b) => a.internalPriority - b.internalPriority);

            // Set Event Button Parents, Active
            for(int i = 0; i < eventButtonInfoList.Count; ++i)
            {
                var info = eventButtonInfoList[i];
                var buttonObj = info.eventButtonObj;

                if (i >= MAX_BUTTON_COUNT) // over 7
                {
                    if(buttonObj != null)
                    {
                        buttonObj.SetActive(false);
                    }
                    continue;
                }

                if (!info.isEnabled) continue;

                var buttonArea = buttonAreaElements[i];
                if (buttonObj == null)
                {
                    // Make Button
                    var gameType = info.type;
                    string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
                    string asset = GetMetaGameButtonPrefabName(gameType);
                    Transform parent = buttonArea.transform;

                    buttonObj = MetaObjectUtils.MakePrefab(bundle, asset, parent);
                    MetaObjectUtils.SetCalleeCaller(buttonObj, gameObject);

                    bool isPassiveMetaGame = BlackboardQueryUtils.IsPassiveMetaGame(gameType, out bool isOtherMetaGame);
                    if (isPassiveMetaGame && !isOtherMetaGame)
                    {
                        var buttonBB = buttonObj.GetComponent<Blackboard>();
                        var passiveEventType = BlackboardQueryUtils.MetaGameTypeToEventInfoType(gameType, out _);
                        BlackboardUtils.SetOrCreateValue(buttonBB, "eventType", passiveEventType);
                    }

                    info.eventButtonObj = buttonObj;
                }
                else
                {
                    info.eventButtonObj.transform.SetParent(buttonArea.transform);
                    info.eventButtonObj.transform.localPosition = new Vector3();
                    info.eventButtonObj.transform.localScale = new Vector3(1f, 1f, 1f);
                }
            }

            int buttonCount = GetButtonCount();
            if (buttonCount <= 0)
            {
                HideGroup();
            }
            else
            {
                ShowGroup();

                // Anim
                anim.SetInteger("MetaCount", buttonCount);

                if(buttonCount == 1)
                {
                    OpenGroup(false);
                }

                // Update
                UpdateButtonActive();
            }
        }

        private int GetButtonCount()
        {
            return eventButtonInfoList.Count(l => l.isEnabled);
        }

        private CheckMetaGameResultState CheckEventMetaGame()
        {
            CheckMetaGameResultState result = CheckMetaGameResultState.NONE;

            var eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo();

            var gameType = BlackboardQueryUtils.EventInfoTypeToMetaGameType(eventInfo?.type ?? EventInfoType.UNKNOWN, out _);
            bool exist = false;

            for (int i = eventButtonInfoList.Count - 1; i >= 0; --i)
            {
                var info = eventButtonInfoList[i];
                if (BlackboardQueryUtils.IsPassiveMetaGame(info.type, out _))
                {
                    // Exist
                    if (gameType == info.type)
                    {
                        exist = true;
                        info.isEnabled = true;
                    }
                    // Disable
                    else
                    {
                        result = CheckMetaGameResultState.REMOVED;

                        if (info.eventButtonObj != null)
                            Destroy(info.eventButtonObj);
                        eventButtonInfoList.Remove(info);
                    }
                }
            }

            // Make Button
            bool isLockedFeature = BlackboardQueryUtils.IsLockedFeature(LockedFeatureType.META_GAME);
            if (!exist && eventInfo != null &&
                (!isLockedFeature || MetaGameUtils.IsMetaEventLevelLock()))
            {
                result = CheckMetaGameResultState.MAKE_NEW;

                bool isActive = BlackboardQueryUtils.IsActiveMetaGameEvent(eventInfo.type);

                eventButtonInfoList.Add(new EventButtonInfo()
                {
                    type = gameType,
                    isEnabled = isActive,
                    eventInfo = eventInfo,
                });
            }

            return result;
        }

        private CheckMetaGameResultState CheckRefreshPassiveModeMetaEvent(EventData eventData)
        {
            // check vip, vds inactive
            if (eventData.name.Equals("RefreshPassive"))
            {
                int eventId = (int)eventData.value;
                var eventInfo = PassiveEventManager.Instance.GetEventInfoFromID(eventId, true);

                if (eventInfo != null)
                {
                    if (eventInfo.type == EventInfoType.BUILD_DREAM_SEASON)
                    {
                        bool isActive = BlackboardQueryUtils.IsActiveMetaGameEvent(EventInfoType.BUILD_DREAM_SEASON);
                        if (!isActive)
                        {
                            RemoveEventButton(MetaGameType.BUILD_DREAM_SEASON);
                            BlackboardQueryUtils.SetVegasDreamsActive(false);

                            return CheckMetaGameResultState.REMOVED;
                        }
                    }
                    else if (eventInfo.type == EventInfoType.VIP_LOUNGE)
                    {
                        bool isActive = BlackboardQueryUtils.IsActiveMetaGameEvent(EventInfoType.VIP_LOUNGE);
                        if (!isActive)
                        {
                            // RemoveEventButton(MetaGameType.VIP_LOUNGE);
                            // vip lounge 는 인게임 메타 버튼 없음
                            // vip 종료 시 vds 종료
                            RemoveEventButton(MetaGameType.BUILD_DREAM_SEASON);
                            BlackboardQueryUtils.SetVipLoungeEnabled(false);

                            return CheckMetaGameResultState.REMOVED;
                        }
                    }
                    else if(eventInfo.type == EventInfoType.LEVEL_UP_DASH_MISSION)
                    {
                        bool isActive = BlackboardQueryUtils.IsActiveMetaGameEvent(EventInfoType.LEVEL_UP_DASH_MISSION);
                        if (!isActive)
                        {
                            RemoveEventButton(MetaGameType.LEVEL_UP_DASH);
                            LevelUpDash.LevelUpDash.Utils.FinishLevelUpDash();

                            return CheckMetaGameResultState.REMOVED;
                        }
                    }
                    else if(eventInfo.type == EventInfoType.SEASON_PASS_V2)
                    {
                        bool isActive = BlackboardQueryUtils.IsActiveMetaGameEvent(EventInfoType.SEASON_PASS_V2);
                        if (!isActive)
                        {
                            RemoveEventButton(MetaGameType.SEASON_PASS_V2);

                            return CheckMetaGameResultState.REMOVED;
                        }
                    }
                }
            }

            return CheckMetaGameResultState.NONE;
        }

        private CheckMetaGameResultState CheckStartPassiveModeMetaEvent(EventData eventData)
        {
            // check vip, vds active
            if (eventData.name.Equals("StartPassive"))
            {
                EventInfoType eventInfoType = ((EventData<EventInfoType>)eventData).value;

                if (eventInfoType == EventInfoType.BUILD_DREAM_SEASON)
                {
                    bool isActive = BlackboardQueryUtils.IsActiveMetaGameEvent(EventInfoType.BUILD_DREAM_SEASON);
                    if (isActive)
                    {
                        BlackboardQueryUtils.SetVegasDreamsActive(true);
                        return CheckMetaGameResultState.MAKE_NEW;
                    }
                }
                else if (eventInfoType == EventInfoType.VIP_LOUNGE)
                {
                    bool isActive = BlackboardQueryUtils.IsActiveMetaGameEvent(EventInfoType.VIP_LOUNGE);
                    if (isActive)
                    {
                        BlackboardQueryUtils.SetVipLoungeEnabled(true);
                        return CheckMetaGameResultState.MAKE_NEW;
                    }
                }
            }

            return CheckMetaGameResultState.NONE;
        }

        private bool RemoveEventButton(MetaGameType type)
        {
            var info = eventButtonInfoList.Find(i => i.type == type);
            if (info != null)
            {
                if (info.eventButtonObj != null) Destroy(info.eventButtonObj);
                eventButtonInfoList.Remove(info);
                return true;
            }

            return false;
        }

        private void OnUpdatePassiveEvent(EventData eventData)
        {
            CheckMetaGameResultState result = CheckMetaGameResultState.NONE;

            // 메타게임 상태 확인해서 결과(생성/삭제)에 따라 그룹 상태 갱신(열기/닫기)
            result = GetState(result, CheckStartPassiveModeMetaEvent(eventData));
            result = GetState(result, CheckRefreshPassiveModeMetaEvent(eventData));
            result = GetState(result, CheckEventMetaGame());
            result = GetState(result, CheckStaticMetaGame());

            UpdateButtonOrder();

            if(result == CheckMetaGameResultState.MAKE_NEW)
            {
                if (isOpen) OpenGroup(false);
                else OpenGroup(true);
            }
            else if(result == CheckMetaGameResultState.REMOVED)
            {
                ForceStopEarningEffects();
                ApplyEarningItemInstantly();
                if (isOpen) OpenGroup(false);
            }
        }

        private void OnClickOpenButton()
        {
            OpenGroup(true, true);
        }

        private void OnClickCloseButton()
        {
            OpenGroup(false, true);
        }

        private void GetFirstEarningItem(out EventButtonInfo targetInfo, out List<EventButtonInfo> anotherInfos)
        {
            targetInfo = null;
            anotherInfos = null;

#if DEV
            if (PlayerPrefs.GetInt("DEBUG_META_ITEM") == 1)
            {
                // rounding enable meta games
                int max = eventButtonInfoList.Count;
                int i = 0;
                while (i++ < max)
                {
                    targetInfo = eventButtonInfoList.CircularIndexing(eligibleIndex++);
                    if (targetInfo.isEnabled) break;
                }

                Debug.Log("Get Item: " + targetInfo.type);

                BlackboardQueryUtils.ClearEarnMetaGameItem();
                BlackboardQueryUtils.SetMetaItemDebug(targetInfo.type);

                return;
            }
#endif

            anotherInfos = new List<EventButtonInfo>();
            for (int i = 0; i < eventButtonInfoList.Count; ++i)
            {
                var info = eventButtonInfoList[i];
                if (targetInfo == null)
                {
                    if (BlackboardQueryUtils.CheckEarnMetaGameItem(info.type))
                    {
                        targetInfo = info;
                    }
                }
                else
                {
                    anotherInfos.Add(info);
                }
            }
        }

        private void OnEndTurn()
        {
            if (isReady)
                MetaInterruption();
        }

        // todo
        // end spin 안오는 상황 대응 (free spin? 특정 slot?)
        private void ActiveMetaUI(EventData eventData)
        {
            if (isReady)
                MetaInterruption();
        }

        private void MetaInterruption()
        {
            bool isGameSpin = BlackboardUtils.GetOrCreateVariable<bool>(ContentBlackboard.Get(), "isGameSpin")?.value ?? false;
            if (isGameSpin) return;

            // Level Up으로 Slot Unlock, Play 시 등 예외 상황 확인
            int metaInterruptingCount = BlackboardUtils.GetOrCreateVariable<int>(MainBlackboard.Get(), "metaInterruptingCount")?.value ?? 0;
            if (metaInterruptingCount > 0) return;

            bool isMetaInterrupted = false;

            bool vipEnabled = BlackboardQueryUtils.IsVipLoungeEnabled();
            //var vipInfo = eventButtonInfoList.FirstOrDefault(i => i.type == MetaGameType.VIP_LOUNGE);
            if (vipEnabled)
            {
                // Stop Auto Spin
                // end turn 에서 하면 auto spin false 되기 전에 한번 더 스핀됨.
                bool isOpen = BlackboardQueryUtils.IsVipLoungeBadgeEarned(false);
                if (isOpen)
                {
                    isMetaInterrupted = true;
                    BlackboardUtils.SetOrCreateValue(ContentBlackboard.Get(), "autoSpin", false);
                }

                // Check Welcome
                var eventData = new EventData<GameObject>(VipLounge.VipLounge.Events.CHECK_VIP_LOUNGE_OPEN, gameObject);
                EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, eventData);
            }
            if (isMetaInterrupted) return;

            bool levelDashActived = LevelUpDash.LevelUpDash.Utils.IsActiveLevelUpDash();
            if (levelDashActived)
            {
                var infoBB = LevelUpDash.LevelUpDash.Utils.InfoBB;
                bool isNew = BlackboardUtils.FindVariable<bool>(infoBB, "isNewOpen")?.value ?? false;
                if (isNew)
                {
                    isMetaInterrupted = true;
                    bool prevAutoSpinState = BlackboardQueryUtils.IsAutoSpin();
                    BlackboardUtils.SetOrCreateValue(ContentBlackboard.Get(), "autoSpin", false);

                    BlackboardUtils.SetOrCreateValue(infoBB, "isNewOpen", false);

                    var levelDashMain = LevelUpDash.MetaSystemLevelUpDashController.Instance;
                    var levelDashMainBB = levelDashMain.GetComponent<Blackboard>();
                    BlackboardUtils.SetOrCreateValue(levelDashMainBB, "enterType", "trigger_level_dash");
                    BlackboardUtils.SetOrCreateValue(levelDashMainBB, "prevAutoSpinState", prevAutoSpinState);

                    EventSender.SendGlobalMetaEvent(LevelUpDash.LevelUpDash.Events.OPEN_LEVEL_UP_DASH);
                }
            }
            if (isMetaInterrupted) return;
        }

        private void CheckMetaGameActiveInEndTurn()
        {
            UpdateLevelUpDashState();
        }

        private void OnEndTurnMetaGame() // After End Turn
        {
            // Check Active
            CheckMetaGameActiveInEndTurn();

            if (!isOpen && isItemEarning) // skip earning effects
            {
                ApplyEarningItemInstantly();
                return;
            }

            // Check Earning
            GetFirstEarningItem(out EventButtonInfo info, out List<EventButtonInfo> anotherInfoList);

            if (info == null) return; // no earning

            // Update Anothers
            foreach (var _info in anotherInfoList)
            {
                if (_info.inGameButtonObj != null)
                    EventSender.SendEvent(_info.inGameButtonObj, MetaEventDefine.UPDATE_META_GAME_ITEM_EARNING_INSTANTLY);
            }

            earnMetaGameItemCoroutine = StartCoroutine(EarnMetaGameItemCoroutine(info));
        }

        private void OnBeginTurn()
        {
            isReady = true;
        }

        private void ForceStopEarningEffects()
        {
            if (isItemEarning)
            {
                if (earnMetaGameItemCoroutine != null) StopCoroutine(earnMetaGameItemCoroutine);
                earnMetaGameItemCoroutine = null;

                anim.SetBool("isChange", false);

                UpdateButtonOrder();

                isItemEarning = false;
            }
        }

        private void ApplyEarningItemInstantly()
        {
            foreach (var _info in eventButtonInfoList)
            {
                if (_info.inGameButtonObj != null)
                    EventSender.SendEvent(_info.inGameButtonObj, MetaEventDefine.UPDATE_META_GAME_ITEM_EARNING_INSTANTLY);
            }

            BlackboardQueryUtils.ClearEarnMetaGameItem();
        }

        // Epic Pass Point 를 End Turn Response에서 얻어서 이 때 처리
        private IEnumerator EarnMetaGameItemCoroutine(EventButtonInfo info)
        {
            if (info == null || info.eventButtonObj == null) yield break;

            isItemEarning = true;

            var timerTrigger = new TimerTrigger(2.5f); // max waiting time..
            var gettingMetaGameItemTrigger = new EventTrigger(gameObject, MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_FINISH_GETTING_META_GAME_ITEM);
            var isCloseTrigger = new WaitUntilConditionTrigger(() => !isOpen); // close group

            var firstButtonObj = eventButtonInfoList.First().eventButtonObj;

            if (isOpen || firstButtonObj == info.eventButtonObj)
            {
                EventSender.SendEvent(info.inGameButtonObj, MetaEventDefine.ON_EARN_META_GAME_ITEM);
                if (isOpen)
                {
                    yield return new WaitUntilTrigger(timerTrigger, gettingMetaGameItemTrigger, isCloseTrigger);
                }
                else
                {
                    yield return new WaitUntilTrigger(timerTrigger, gettingMetaGameItemTrigger);
                }
            }
            else // group closed && target is not first
            {
                yield return new WaitUntil(() => IS_GROUP_INTERACTABLE); // wait group closing

                int targetIndex = eventButtonInfoList.IndexOf(info);
                var firstArea = buttonAreaElements[0];
                var targetArea = buttonAreaElements[targetIndex];

                var targetObj = info.eventButtonObj;

                // hide first
                anim.SetBool("isChange", true);
                yield return new WaitForSeconds(0.1f);

                // Swap
                if (firstButtonObj != null)
                {
                    firstButtonObj.transform.SetParent(targetArea.transform);
                    firstButtonObj.SetActive(false);
                }
                if(targetObj != null)
                {
                    targetObj.transform.SetParent(firstArea.transform);
                    targetObj.SetActive(true);
                }

                // show target
                anim.SetBool("isChange", false);
                yield return new WaitForSeconds(0.2f);

                yield return new WaitUntil(() => info.inGameButtonObj == null || info.inGameButtonObj.activeSelf);

                EventSender.SendEvent(info.inGameButtonObj, MetaEventDefine.ON_EARN_META_GAME_ITEM);

                yield return new WaitUntilTrigger(timerTrigger, gettingMetaGameItemTrigger);

                // hide target
                anim.SetBool("isChange", true);
                yield return new WaitForSeconds(0.1f);

                // Swap
                if (firstButtonObj != null)
                {
                    firstButtonObj.transform.SetParent(firstArea.transform);
                    firstButtonObj.SetActive(true);
                }
                if (targetObj != null)
                {
                    targetObj.transform.SetParent(targetArea.transform);
                    targetObj.SetActive(false);
                }

                // show first
                anim.SetBool("isChange", false);
                yield return new WaitForSeconds(0.2f);
            }

            BlackboardQueryUtils.ClearEarnMetaGameItem();

            UpdateButtonOrder();

            isItemEarning = false;
        }
    }
}
