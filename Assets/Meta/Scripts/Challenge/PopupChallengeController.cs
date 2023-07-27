using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using System.Collections;
using System.Linq;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class PopupChallengeController : EventMonoBehaviour
    {
        public ContextElement root;
        public Blackboard rootBB;
        public Animator rootAnim;

        private List<MetaChallengeType> challengeTypeList;
        private Dictionary<MetaChallengeType, ChallengePopupData> popupDataDict = new Dictionary<MetaChallengeType, ChallengePopupData>();

        private MetaChallengeType currentTabType = MetaChallengeType.NONE;
        private bool isLoadComplete = false;
        private bool ignoreEventChallenge = false;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        public enum TabState
        {
            NONE = 0,
            TAB,
            COVER,
            COMPLETE,
        }

        //

        private void OnDestroy()
        {
            MetaSystem.UnSubscribeBackButton(gameObject.GetHashCode());
        }

        public void OnRefreshPassiveEvent()
        {
            EventSender.SendEvent(gameObject, ChallengeEventManager.ON_UPDATE_CHALLENGE_POPUP);
        }

        public void InitProperty()
        {
            root = GetComponent<ContextElement>();
            rootBB = gameObject.GetComponent<Blackboard>();
            rootAnim = gameObject.GetComponent<Animator>();

            root.UpdateContext();

            ignoreEventChallenge = rootBB.GetVariable<bool>("ignoreEventChallenge")?.value ?? false;
        }

        public IEnumerator OpenCoroutine()
        {
            rootAnim.SetBool("IsLoading", true);

            EventSender.SendGlobalEvent(ChallengeEventManager.ON_OPEN_CHALLENGE_POPUP);

            yield return StartCoroutine(ChallengeUtils.RequestChallengeInfo(rootBB));

            var challengeResponse = (Blackboard)BlackboardUtils.GetOrCreateBlackboard(rootBB, "challengeInfoResponse");
            ChallengeEventManager.InitChallengeTypeList(challengeResponse);
            challengeTypeList = ChallengeEventManager.ChallengeTypeList;

            if (MetaGameUtils.IsEpicPassAlwayslLock())
                yield return StartCoroutine(ChallengeUtils.RequestChallengeEpicPassInfo());

            InitTabElements();

            foreach (var popupData in popupDataDict.Values)
                popupData.InitProperty();

            UpdateCurrentTabType();

            if (currentTabType != MetaChallengeType.EPIC_PASS)
                yield return StartCoroutine(ClaimCompletedChallengeReward());

            OnUpdateChallengeInfo();

            // Select Tab
            var forceSelectTabType = rootBB.GetVariable<MetaChallengeType>("forceSelectTabType")?.value ?? MetaChallengeType.NONE;

            if (forceSelectTabType != MetaChallengeType.NONE)
            {
                foreach (var challengeType in popupDataDict.Keys)
                {
                    if (forceSelectTabType == challengeType || (forceSelectTabType != MetaChallengeType.EPIC_PASS && challengeType == MetaChallengeType.NORMAL))
                    {
                        popupDataDict[challengeType].isAuto = true;
                        popupDataDict[challengeType].OnSelectTab(forceSelectTabType);
                    }
                }
            }

            rootAnim.SetBool("IsLoading", false);

            // Set Challenge Check Time
            ChallengeUtils.SetAllChallengeCheckTimeCurrent();

            InitEvents();

            isLoadComplete = true;
        }

        public void ClosePopup()
        {
            EpicPassUtilsV2.RequestRefreshInbox();
            EventSender.SendCalleeCallback(gameObject);
            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, ChallengeEventManager.CLOSE_CHALLENGE_POPUP);

            rootAnim.SetTrigger("Close");

            MetaPopupUtils.ClosePopup(gameObject);
        }

        public void OnEndTimer()
        {
            if (currentTabType == MetaChallengeType.EVENT_PERSONAL || currentTabType == MetaChallengeType.EVENT_CLUB)
            {
                SelectTab(MetaChallengeType.DAILY);
                OnRefreshPassiveEvent();
            }
        }

        public IEnumerator OpenLeadersPopupCoroutine(string missionId)
        {
            // Loading
            GameObject loadingObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenLoadingPopupCoroutine((obj) => loadingObj = obj));

            bool isEventClubChallenge = currentTabType == MetaChallengeType.EVENT_CLUB;

            Blackboard challengeInfo;
            if (isEventClubChallenge)
            {
                var clubData = popupDataDict[MetaChallengeType.EVENT_CLUB] as ChallengePopupDataEvent;
                challengeInfo = clubData.challengeInfo;
            }
            else
            {
                var clubData = popupDataDict[MetaChallengeType.CLUB] as ChallengePopupDataClub;
                challengeInfo = clubData.challengeInfo;
            }

            int date = challengeInfo.GetValue<int>("date");

            var onRemovedClubTrigger = new EventTrigger(this, MetaEventDefine.ON_META_UI_EVENT, ChallengeEventManager.ON_REMOVED_CLUB);
            yield return StartCoroutine(ChallengeUtils.RequestClubMissionInfoCoroutine(rootBB, date, missionId));

            if (onRemovedClubTrigger.IsTrigger)
            {
                SelectTab(isEventClubChallenge ? MetaChallengeType.EVENT_CLUB : MetaChallengeType.CLUB);
                yield break;
            }

            GameObject leaderPopupObj = null;

            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Popup Club Challenge Leaders Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;
            yield return StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(bundle, asset, parent,
                (scene) => leaderPopupObj = scene.GetScene()));

            Blackboard leaderPopupBB = leaderPopupObj.GetComponent<Blackboard>();
            var response = rootBB.GetVariable<Blackboard>("clubMissionResponse")?.value;
            leaderPopupBB.AddVariable("clubMissionResponse", response);

            MetaObjectUtils.SetCalleeCaller(leaderPopupObj, gameObject);

            MetaPopupUtils.OpenPopup(leaderPopupObj);
            leaderPopupObj.GetComponent<GraphOwner>().StopBehaviour();
            leaderPopupObj.GetComponent<GraphOwner>().StartBehaviour();

            MetaPopupUtils.ClosePopup(loadingObj);

            var callbackTrigger = new EventTrigger(gameObject, EventSender.ON_CALLEE_CALLBACK);
            yield return new WaitUntilTrigger(callbackTrigger);
        }

        private void InitEvents()
        {
            var closeButtonElement = ContextUtils.FindElement(root, "Button Close", CHILDREN);

            MetaContextElementUtils.SetClickable(
                closeButtonElement,
                () => EventSender.SendEvent(gameObject, ChallengeEventManager.ON_CLOSE));

            MetaSystem.SubscribeBackButton(
                gameObject.GetHashCode(),
                () => EventSender.SendEvent(gameObject, ChallengeEventManager.ON_CLOSE));
        }

        private void UpdateCurrentTabType()
        {
            var forceSelectTabType = rootBB.GetVariable<MetaChallengeType>("forceSelectTabType")?.value ?? MetaChallengeType.NONE;
            currentTabType = forceSelectTabType;

            // Apply default TabType
            if (!challengeTypeList.Contains(forceSelectTabType))
            {
                currentTabType = GetDefaultTabType();
                rootBB.AddVariable("forceSelectTabType", currentTabType);
            }
        }

        private void OnUpdateChallengeInfo()
        {
            foreach (var popupData in popupDataDict.Values)
                popupData.OnUpdateChallengeInfo();
        }

        private IEnumerator ClaimCompletedChallengeReward()
        {
            // Event is First
            var simpleChallengeInfoList = MainBlackboard.Get().GetValue<List<Blackboard>>("challengeInfoList");
            var copyInfoList = new List<Blackboard>(simpleChallengeInfoList);

            var eventInfo = copyInfoList.FirstOrDefault(s =>
                (s.GetVariable<ChallengeType>("challengeType")?.value ?? ChallengeType.UNKNOWN) == ChallengeType.EVENT);

            if(copyInfoList.Remove(eventInfo))
                copyInfoList.Insert(0, eventInfo);

            //var eventInfo = simpleChallengeInfoList.FirstOrDefault(s =>
            //    (s.GetVariable<ChallengeType>("challengeType")?.value ?? ChallengeType.UNKNOWN) == ChallengeType.EVENT);

            //int eventInfoIndex = eventInfo == null ? -1 : simpleChallengeInfoList.IndexOf(eventInfo);

            //var indexingList = new List<int>();
            //int i = 0;
            //foreach (var info in simpleChallengeInfoList) indexingList.Add(i++);
            //if (indexingList.Remove(eventInfoIndex))
            //    indexingList.Insert(0, eventInfoIndex);

            //for( int j = 0; j < indexingList.Count; ++j)
            //{
            //    int index = indexingList[j];
            //    var simpleInfo = simpleChallengeInfoList[index];

            for(int i = 0; i < copyInfoList.Count; ++i)
            {
                var simpleInfo = copyInfoList[i];
                bool isDone = simpleInfo.GetValue<bool>("done");
                bool isClaimed = simpleInfo.GetValue<bool>("claimed");
                if (!isDone || isClaimed) continue;

                var challengeType = BlackboardQueryUtils.GetChallengeSimpleInfoChallengeType(simpleInfo);
                var challengeResponse = BlackboardUtils.GetOrCreateBlackboard(rootBB, "challengeInfoResponse");
                var challengeInfo = BlackboardQueryUtils.GetChallengeInfoBB((Blackboard)challengeResponse, challengeType);

                // Request Claim
                yield return StartCoroutine(ChallengeUtils.RequestChallengeClaim(challengeInfo, rootBB));

                Blackboard response = rootBB.GetVariable<Blackboard>("claimResponse")?.value;
                if (response == null) continue;

                // Send BI Event
                BiEventUtils.SendBiChallengeClaim(response, simpleInfo);

                // Open Reward Popup
                string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
                string asset = "Popup Reward Challenge Scene";
                Transform transform = MetaPopupUtils.PopupManagerAreaTransform;

                GameObject rewardPopupObj = null;
                yield return StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(bundle, asset, transform,
                    (SceneLoadOperation scene) => rewardPopupObj = scene.GetScene()));

                if (rewardPopupObj == null) continue;

                Blackboard rewardPopupBB = rewardPopupObj.GetComponent<Blackboard>();
                rewardPopupBB.AddVariable("challengeInfo", simpleInfo);
                rewardPopupBB.AddVariable("claimResponse", response);
                rewardPopupBB.AddVariable("caller", gameObject);

                MetaPopupUtils.OpenPopup(rewardPopupObj);

                // Wait Close Reward Popup
                var waitCallbackTrigger = new EventTrigger(this, EventSender.ON_CALLEE_CALLBACK);
                yield return new WaitUntilTrigger(waitCallbackTrigger);
            }
        }

        //

        private void SelectTab(MetaChallengeType type)
        {
            if(!isLoadComplete) return;
            currentTabType = type;
            GSManager.Instance.GetHandler("UI_Button_Tab").Play();
            StartCoroutine(OnSelectTab());
        }

        private IEnumerator OnSelectTab()
        {
            if (currentTabType == MetaChallengeType.NORMAL)
            {
                MetaChallengeType defaultType = GetDefaultTabType();
                currentTabType = defaultType;
            }

            if (currentTabType == MetaChallengeType.DAILY ||
                currentTabType == MetaChallengeType.EXPERT ||
                currentTabType == MetaChallengeType.MASTER ||
                currentTabType == MetaChallengeType.CLUB ||
                currentTabType == MetaChallengeType.EVENT_PERSONAL)
            {
                if (popupDataDict.ContainsKey(MetaChallengeType.NORMAL) && popupDataDict[MetaChallengeType.NORMAL].IsBadge)
                {
                    rootAnim.SetBool("EpicPass", false);
                    rootAnim.SetBool("IsLoading", true);
                    yield return StartCoroutine(ClaimCompletedChallengeReward());
                    rootAnim.SetBool("IsLoading", false);
                }
            }

            foreach (var popupData in popupDataDict.Values)
                popupData?.OnSelectTab(currentTabType);
        }

        private MetaChallengeType GetDefaultTabType()
        {
            foreach(var type in challengeTypeList)
            {
                if (!popupDataDict.ContainsKey(type)) continue;
                if ((type == MetaChallengeType.EVENT_CLUB || type == MetaChallengeType.EVENT_PERSONAL) && ignoreEventChallenge) continue;
                if (type == MetaChallengeType.EPIC_PASS) continue;

                if(!popupDataDict[type].isUpdated)
                    popupDataDict[type].UpdateChallengeInfo();

                if (!popupDataDict[type].IsComplete(type))
                    return type;
            }

            return MetaChallengeType.MASTER;
        }

        private ChallengePopupData GenerateChallengePopupData(MetaChallengeType type)
        {
            switch (type)
            {
                case MetaChallengeType.DAILY:
                case MetaChallengeType.EXPERT:
                case MetaChallengeType.MASTER:
                    {
                        foreach (var t in popupDataDict.Keys)
                            if (t == MetaChallengeType.DAILY || t == MetaChallengeType.EXPERT || t == MetaChallengeType.MASTER)
                                return popupDataDict[t];

                        return new ChallengePopupDataNormal(gameObject);
                    }
                case MetaChallengeType.CLUB:
                    return new ChallengePopupDataClub(gameObject);
                case MetaChallengeType.EVENT_PERSONAL:
                case MetaChallengeType.EVENT_CLUB:
                    {
                        foreach (var t in popupDataDict.Keys)
                            if (t == MetaChallengeType.EVENT_PERSONAL || t == MetaChallengeType.EVENT_CLUB)
                                return popupDataDict[t];

                        return new ChallengePopupDataEvent(gameObject, type == MetaChallengeType.EVENT_PERSONAL);
                    }
                case MetaChallengeType.EPIC_PASS:
                    return new ChallengePopupDataEpicPass(gameObject);
                case MetaChallengeType.NORMAL:
                    return new ChallengePopupDataDailyTab(gameObject);
            }

            return null;
        }

        private void InitTabElements()
        {
            foreach(var data in popupDataDict.Values)
                data.ClearPopupData();

            ContextElement mainTabElement = ContextUtils.FindElement(root, "Left Tab Base", CHILDREN);
            ContextElement dailyTabElement = ContextUtils.FindElement(root, "Top Tab Base", CHILDREN);
            // Set Main Tab
            var mainTabChallengeType = System.Enum.GetValues(typeof(MetaChallengeMainTabType)) as MetaChallengeMainTabType[];
            foreach(MetaChallengeMainTabType type in mainTabChallengeType)
            {
                if (type == MetaChallengeMainTabType.NONE) continue;
                var tabElement = ContextUtils.FindElement(mainTabElement, ChallengeTypeToMainTabName(type), CHILDREN);
                tabElement.gameObject.SetActive(false);
            }
            // Set Daily Tab
            var dailyTabChallengeType = System.Enum.GetValues(typeof(MetaChallengeDailyTabType)) as MetaChallengeDailyTabType[];
            foreach (MetaChallengeDailyTabType type in dailyTabChallengeType)
            {
                if (type == MetaChallengeDailyTabType.NONE) continue;
                var tabElement = ContextUtils.FindElement(dailyTabElement, ChallengeTypeToDailyTabName(type), CHILDREN);
                tabElement.gameObject.SetActive(false);
            }

            popupDataDict.Clear();
            bool enableEpicPassV2 = MetaGameUtils.IsEpicPassAlwayslLock();

            foreach (var type in challengeTypeList)
            {
                popupDataDict.Add(type, GenerateChallengePopupData(type));

                bool isMainTab = IsMainTab(type);
                ContextElement tabElement = ContextUtils.FindElement(isMainTab ? mainTabElement : dailyTabElement, ChallengeTypeToName(type), CHILDREN);

                ChallengePopupData.tabElementDict[type] = tabElement;
                ChallengePopupData.coverElementDict[type] = ContextUtils.FindElement(tabElement, "Cover", CHILDREN);
                ChallengePopupData.badgeElementDict[type] = ContextUtils.FindElement(tabElement, "Badge Area", CHILDREN);
                GameObject badgeObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Badge Outline Gold", ChallengePopupData.badgeElementDict[type].transform, null, "Badge");
                Animator badgeAnim = badgeObj.GetComponent<Animator>();
                if (badgeAnim != null)
                    badgeAnim.keepAnimatorControllerStateOnDisable = true;
                ChallengePopupData.badgeAnimatorDict[type] = badgeAnim;

                if (!isMainTab)
                    ChallengePopupData.completeElementDict[type] = ContextUtils.FindElement(tabElement, "Complete", CHILDREN);

                tabElement.gameObject.SetActive(isMainTab);

                MetaContextElementUtils.SetClickable(
                    tabElement,
                    () => EventSender.SendEvent(gameObject, ChallengeTypeToEvent(type)));
            }

            // Select Tab
            foreach (var type in challengeTypeList)
            {
                string eventName = ChallengeTypeToEvent(type);
                Register(eventName, () => SelectTab(type));
            }
        }

        private string ChallengeTypeToName(MetaChallengeType type)
        {
            return "Tab " + TextDecoUtils.EnumTypeToText<MetaChallengeType>(
                (int)type, TextDecoUtils.TextFormat.PASCAL_CASE, " ");
        }

        private string ChallengeTypeToEvent(MetaChallengeType type)
        {
            return "On" + TextDecoUtils.EnumTypeToText<MetaChallengeType>(
                (int)type, TextDecoUtils.TextFormat.PASCAL_CASE, "");
        }

        private string ChallengeTypeToMainTabName(MetaChallengeMainTabType type)
        {
            return "Tab " + TextDecoUtils.EnumTypeToText<MetaChallengeMainTabType>(
                (int)type, TextDecoUtils.TextFormat.PASCAL_CASE, " ");
        }

        private string ChallengeTypeToDailyTabName(MetaChallengeDailyTabType type)
        {
            return "Tab " + TextDecoUtils.EnumTypeToText<MetaChallengeDailyTabType>(
                (int)type, TextDecoUtils.TextFormat.PASCAL_CASE, " ");
        }

        private bool IsMainTab(MetaChallengeType type)
        {
            switch (type)
            {
                case MetaChallengeType.NONE:
                case MetaChallengeType.NORMAL:
                case MetaChallengeType.EPIC_PASS:
                    return true;
                default:
                    return false;
            }
        }
    }
}
