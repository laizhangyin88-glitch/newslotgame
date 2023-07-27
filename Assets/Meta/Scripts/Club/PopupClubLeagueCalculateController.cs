using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using UnityEngine;
using NodeCanvas.Framework;
using BagelCode.OSA_Scroll;
using BagelCode.ClientModels;
using ParadoxNotion;

namespace BagelCode
{
    public class PopupClubLeagueCalculateController : MonoBehaviour
    {
        private ContextElement rootElement;
        private Blackboard rootBB;
        private Animator anim;

        private ContextElement titleTextElement;

        private ContextElement clubLeagueScrollElement;
        private ContextElement clubCellScrollElement;

        private ContextElement floatingAreaElement;
        private ContextElement floatingClubCellElement;

        private ContextElement cellLoadingElement;

        private OSA_PopupAllClubLeagueTier osaClubLeagueController;
        private OSA_PopupAllClubCell osaClubCellController;

        private List<Blackboard> clubListBB;

        private bool isClubber = false;
        private bool isFirstItem = true;
        public bool isListNotReady = false;
        public int currentLeagueTier = 0;

        public bool requestSuccess = false;
        public int myClubLeagueTier = 0;
        public long startIndex = 0L;
        public long lastUpdateTimestamp = 0L;
        public long clubCellVelocity => (long)osaClubCellController?.Velocity.y;

        private Blackboard myClubBB = null;

        private readonly int PREFETCH_ITEMS_COUNT = 50;
        private readonly int CELL_ITEMS_COUNT = 30;
        private readonly int PAGE_CHECK_COUNT = 5;

        private long currentStartIndex
        {
            get { return BlackboardUtils.GetOrCreateVariable<long>(clubListBB[currentLeagueTier], "startIndex")?.value ?? 0L; }
            set { BlackboardUtils.SetOrCreateValue(clubListBB[currentLeagueTier], "startIndex", value); }
        }

        private long currentEndIndex
        {
            get { return BlackboardUtils.GetOrCreateVariable<long>(clubListBB[currentLeagueTier], "endIndex")?.value ?? 0L; }
            set { BlackboardUtils.SetOrCreateValue(clubListBB[currentLeagueTier], "endIndex", value); }
        }

        void Update()
        {
            if (clubCellVelocity != 0L)
            {
                UpdateFloatingArea();
            }
        }

        public void OnInit()
        {
            InitProperty();
        }

        private void InitProperty()
        {
            rootElement = GetComponent<ContextElement>();
            rootBB = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();

            rootElement.UpdateContext(false);

            ContextElement closeButtonElement = ContextUtils.FindElement(rootElement, "Button Close", ContextSearchingType.ChildrenSearch);

            ContextElement titleAreaElement = ContextUtils.FindElement(rootElement, "Title Area", ContextSearchingType.ChildrenSearch);
            titleTextElement = ContextUtils.FindElement(titleAreaElement, "Text", ContextSearchingType.ChildrenSearch);

            ContextElement contentsElement = ContextUtils.FindElement(rootElement, "Contents", ContextSearchingType.ChildrenSearch);
            clubLeagueScrollElement = ContextUtils.FindElement(contentsElement, "Club League Layout/Scroll Rect", ContextSearchingType.FullNameSearch);
            clubCellScrollElement = ContextUtils.FindElement(contentsElement, "Club Cell Layout", ContextSearchingType.ChildrenSearch);

            floatingAreaElement = ContextUtils.FindElement(clubCellScrollElement, "Floating Area", ContextSearchingType.ChildrenSearch);
            floatingClubCellElement = ContextUtils.FindElement(floatingAreaElement, "Cell Area/Club League Calculate Cell", ContextSearchingType.FullNameSearch);

            cellLoadingElement = ContextUtils.FindElement(contentsElement, "Loading", ContextSearchingType.ChildrenSearch);

            osaClubLeagueController = clubLeagueScrollElement?.GetComponent<OSA_PopupAllClubLeagueTier>() ?? null;
            osaClubCellController = clubCellScrollElement?.GetComponent<OSA_PopupAllClubCell>() ?? null;

            osaClubLeagueController.SetCaller(rootElement);

            osaClubCellController.SetCaller(rootElement);
            osaClubCellController.SubscribeEndDragEvent(OnEndDragEventCallback);

            MetaContextElementUtils.SetClickable(closeButtonElement, OnClickedClose);

            isClubber = GetMyClubId() > 0L;
        }

        private void InitClubBlackboard(int maxLeagueTier)
        {
            GameObject listObject = new GameObject();
            listObject.name = "clubList";
            listObject.transform.parent = rootBB.propertiesBindTarget.transform;

            clubListBB = new List<Blackboard>();
            for (int i = 0; i <= maxLeagueTier; ++i)
            {
                GameObject go = new GameObject();
                go.name = i.ToString();
                go.transform.parent = listObject.transform;

                clubListBB.Add(go.AddComponent<Blackboard>());
                var tierClubList = clubListBB[i].AddVariable("tierClubList", typeof(List<Blackboard>));
                tierClubList.value = new List<Blackboard>();
                clubListBB[i].AddVariable("startIndex", -1L);
                clubListBB[i].AddVariable("endIndex", 0L);
            }

            rootBB.AddVariable("clubList", clubListBB);
            currentStartIndex = startIndex;
        }

        public void InitScrollLeagueTier()
        {
            Blackboard clubRankingListPrefetchBB = rootBB.GetValue<Blackboard>("clubRankingListPrefetchResponse");
            int maxLeagueTier = clubRankingListPrefetchBB.GetValue<int>("maxClubLeagueTier");

            InitClubBlackboard(maxLeagueTier);
            SetTitleText(currentLeagueTier);

            osaClubLeagueController.SetMyLeagueTier(myClubLeagueTier);
            osaClubLeagueController.SetSelectTier(currentLeagueTier);
            osaClubLeagueController.CreateItemList(maxLeagueTier);
        }

        public void InitScrollClubCell()
        {
            Blackboard clubRankingListBB = rootBB.GetValue<Blackboard>("clubRankingListResponse");
            List<Blackboard> clubList = GetTierClubList(currentLeagueTier);

            InitFloatingData();
            osaClubCellController.CreateItemList(clubList);

            Variable<SceneState> currentSceneState = BlackboardUtils.GetOrCreateVariable<SceneState>("/currentSceneState");
            if (currentSceneState != null && currentSceneState.value == SceneState.LOBBY)
            {
                long clubLeagueCurrentWeek = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "/clubLeagueCurrentWeek")?.value ?? 0L;
                if (PlayerPrefsUtils.GetInt64(ClubDefine.PLAYER_PREFS_LEAGUE_RESULT_POPUP) != clubLeagueCurrentWeek)
                    PlayerPrefsUtils.SetInt64(ClubDefine.PLAYER_PREFS_LEAGUE_RESULT_POPUP, clubLeagueCurrentWeek);
            }
        }

        private void InitFloatingData()
        {
            Blackboard myInitClubBB = GetMyClubData();
            if (isClubber == false || myInitClubBB == null) return;

            if (floatingClubCellElement != null)
                floatingClubCellElement.GetComponent<PopupAllClubCellItem>()?.UpdateVariables(myInitClubBB, rootElement);
        }

        private void ResetClubBlackboard()
        {
            if (clubListBB == null) return;
            for (int i = 0; i < clubListBB.Count; ++i)
            {
                clubListBB[i].AddVariable("startIndex", -1L);
                clubListBB[i].AddVariable("endIndex", 0L);

                List<Blackboard> tierClubList = GetTierClubList(i);
                if (tierClubList.Count == 0)
                    continue;

                for (int j = tierClubList.Count - 1; j >= 0; --j)
                    BlackboardUtils.RemoveAtBlackboardList(clubListBB[i], "tierClubList", j);
            }
        }

        public void RefreshScrollLeagueTier()
        {
            SetTitleText(currentLeagueTier);

            osaClubLeagueController.SetMyLeagueTier(myClubLeagueTier);
            osaClubLeagueController.RefreshLeagueTier(currentLeagueTier);
        }

        private void SetTitleText(int clubTier)
        {
            if (titleTextElement != null)
                MetaContextElementUtils.SetTextGlobal(titleTextElement, "POPUP_CLUB_ALL_LEAGUE_TITLE", clubTier);
        }

        private void SetCellLoading(bool isActive)
        {
            cellLoadingElement?.gameObject.SetActive(isActive);
        }

        private void SetActiveClubCell(bool isActive)
        {
            clubCellScrollElement?.gameObject.SetActive(isActive);
        }

        private void SetActiveFloatingArea(bool isActive)
        {
            bool isBeforeActive = floatingAreaElement.gameObject.activeSelf;

            if (isActive)
                floatingAreaElement.gameObject.SetActive(IsActiveFloatingArea());
            else
                floatingAreaElement.gameObject.SetActive(false);

            if (floatingAreaElement.gameObject.activeSelf != isBeforeActive)
                osaClubCellController.RebuildLayout();
        }

        private void SetRefresh(bool isRefresh)
        {
            BlackboardUtils.SetOrCreateValue<bool>(rootBB, "isRefresh", isRefresh);
        }

        private Blackboard GetMyClubData()
        {
            long myClubId = GetMyClubId();
            if (myClubId == 0) return null;

            if (myClubBB != null) return myClubBB;

            List<Blackboard> tierClubList = GetTierClubList(myClubLeagueTier);

            if (tierClubList != null && tierClubList.Count > 0)
            {
                for (int i = 0; i < tierClubList.Count; ++i)
                {
                    if (myClubId == tierClubList[i].GetValue<long>("id"))
                    {
                        myClubBB = tierClubList[i];
                        break;
                    }
                }
            }
            // prefetch my club data
            if (myClubBB == null)
            {
                // my club is not in list (lp 0)
                Blackboard prefetchResponse = BlackboardUtils.FindVariable<Blackboard>(rootBB, "clubRankingListPrefetchResponse")?.value ?? null;
                if (prefetchResponse != null)
                    myClubBB = BlackboardUtils.FindVariable<Blackboard>(prefetchResponse, "clubInfo")?.value ?? null;
            }
            return myClubBB;
        }

        private long GetMyClubId()
        {
            return BlackboardUtils.FindVariable<long>(null, "/me/clubId")?.value ?? 0L;
        }

        private bool IsMyClubInList(List<Blackboard> listBB)
        {
            long myClubId = GetMyClubData()?.GetValue<long>("id") ?? 0L;
            if (myClubId == 0L || listBB == null) return false;
            return GetClubInList(myClubId, listBB) != null;
        }

        private Blackboard GetClubInList(long clubId, List<Blackboard> listBB)
        {
            if (listBB != null && listBB.Count > 0)
            {
                for (int i = 0; i < listBB.Count; ++i)
                {
                    if (clubId == listBB[i].GetValue<long>("id"))
                        return listBB[i];
                }
            }
            return null;
        }

        private bool IsActiveFloatingArea()
        {
            return isClubber && currentLeagueTier == myClubLeagueTier && GetMyClubData() != null;
        }

        public void OnClickedClose()
        {
            PopupManager.Instance.Close(gameObject);
            osaClubCellController.UnSubscribeEndDragEvent(OnEndDragEventCallback);
            anim.SetTrigger("Close");
            Destroy(gameObject);
        }

        public void OnClickedLeague(int selectLeague)
        {
            if (currentLeagueTier == selectLeague)
            {
                requestSuccess = true;
                return;
            }

            osaClubLeagueController.SetTierCover(selectLeague);

            // Todo : Test Button : MINI / MINOR
            //if (selectLeague == 0)
            //{
            //    List<ClubListInfo> dummyList = DummyClubListInfo();
            //    InsertClubList(currentLeagueTier, dummyList, false);
            //    List<Blackboard> tierClubList = GetTierClubList(currentLeagueTier);
            //    osaClubCellController.ResetParams(tierClubList);
            //    osaClubCellController.InsertToFirstItems(dummyList.Count, false, true);

            //    requestSuccess = true;
            //}
            //else if (selectLeague == 1)
            //{
            //    List<ClubListInfo> dummyList = DummyClubListInfo();
            //    int insertIndex = osaClubCellController.GetItemsCount();
            //    InsertClubList(currentLeagueTier, dummyList, true);
            //    List<Blackboard> tierClubList = GetTierClubList(currentLeagueTier);
            //    osaClubCellController.ResetParams(tierClubList);
            //    osaClubCellController.InsertItems(insertIndex, dummyList.Count, false, true);

            //    requestSuccess = true;
            //}
            osaClubCellController.ResetParams();
            osaClubCellController.ResetItems(0);
            currentLeagueTier = selectLeague;

            SetTitleText(selectLeague);

            SetActiveFloatingArea(false);

            List<Blackboard> currentTierClubList = GetTierClubList(selectLeague);
            if (currentTierClubList.Count > 0)
            {
                osaClubCellController.CreateItemList(currentTierClubList);

                CheckActiveFloatingArea();
                requestSuccess = true;
            }
            else
            {
                startIndex = 0;
                SetActiveClubCell(false);
                RequestClubLeagueRankingList(selectLeague, (int)startIndex, CELL_ITEMS_COUNT);
            }
        }

        public void OnClickedClubId(string contextId, long selectClubId)
        {
            var clubMemberObj = MetaObjectUtils.MakeScene("Popup Club Member Scene", PopupManager.Instance.transform.Find("Area"));
            var clubMemberScene = clubMemberObj.GetComponent<Blackboard>();
            PopupManager.Instance.Open(clubMemberObj);
            clubMemberScene.AddVariable("caller", gameObject);
            clubMemberScene.AddVariable("clubID", selectClubId);
            clubMemberScene.AddVariable("joinContextID", contextId);
        }

        public void OnEndDragEventCallback(double delta)
        {
            // delta 0 > value = last / 1 < value = first
            if (delta < 0.0 || delta >= 1.0)
            {
                if (requestSuccess == true)
                {
                    isFirstItem = delta >= 1.0 && PAGE_CHECK_COUNT < GetTierClubList(currentLeagueTier).Count;
                    int startIdx = 0;
                    int listCnt = CELL_ITEMS_COUNT;
                    if (isFirstItem)
                    {
                        if (currentStartIndex == 0L)
                        {
                            UpdateFloatingArea();
                            return;
                        }
                        if (currentStartIndex < 0L) currentStartIndex = 0L;

                        startIdx = (int)currentStartIndex - listCnt;
                        if (startIdx < 0)
                        {
                            listCnt += startIdx;
                            startIdx = 0;
                        }
                    }
                    else
                    {
                        startIdx = (int)currentEndIndex;
                    }
                    RequestClubLeagueRankingList(currentLeagueTier, startIdx, listCnt, isFirstItem);
                }
            }

            UpdateFloatingArea();
        }

        public void CheckActiveFloatingArea()
        {
            List<Blackboard> osaActiveListBB = osaClubCellController.GetDataBB();
            if (osaActiveListBB != null)
                SetActiveFloatingArea(!IsMyClubInList(osaActiveListBB));
        }

        private List<ClubListInfo> ConvertClubListInfo(List<Blackboard> clubListBB)
        {
            List<ClubListInfo> clubListInfo = new List<ClubListInfo>();
            if (clubListBB == null) return clubListInfo;

            for (int i = 0; i < clubListBB.Count; ++i)
            {
                ClubListInfo clubInfo = new ClubListInfo
                {
                    id = clubListBB[i].GetValue<long>("id"),
                    name = clubListBB[i].GetValue<string>("name"),
                    symbol = clubListBB[i].GetValue<string>("symbol"),
                    motd = clubListBB[i].GetValue<string>("motd"),
                    members = clubListBB[i].GetValue<int>("members"),
                    level = clubListBB[i].GetValue<int>("level"),
                    friends = clubListBB[i].GetValue<int>("friends"),
                    leagueTier = clubListBB[i].GetValue<int>("leagueTier"),
                    leaguePoint = clubListBB[i].GetValue<long>("leaguePoint"),
                    minPlayerLevel = clubListBB[i].GetValue<int>("minPlayerLevel"),
                    joinType = clubListBB[i].GetValue<ClubJoinType>("joinType"),
                    rank = clubListBB[i].GetValue<int>("rank")
                };
                clubListInfo.Add(clubInfo);
            }

            return clubListInfo;
        }

        private List<ClubListInfo> DummyClubListInfo()
        {
            Blackboard clubTopTierBB = rootBB.GetValue<Blackboard>("clubRankingListResponse");
            List<Blackboard> clubList = clubTopTierBB.GetValue<List<Blackboard>>("clubList");

            return ConvertClubListInfo(clubList);
        }

        private void InsertClubList(int clubTier, List<ClubListInfo> clubListInfo, bool isFirst = false)
        {
            if (clubListInfo == null || clubListInfo.Count <= 0)
                return;

            List<Blackboard> tierClubList = GetTierClubList(clubTier);

            for (int i = 0; i < clubListInfo.Count; ++i)
            {
                GameObject go = new GameObject();
                go.name = "clubListInfo";
                go.transform.parent = clubListBB[clubTier].transform;

                Blackboard newBB = go.AddComponent<Blackboard>();
                ClientAPI2Blackboard.Serialize(newBB, clubListInfo[i]);
                if (isFirst)
                    tierClubList.Insert(i, newBB);
                else
                    tierClubList.Add(newBB);
            }
        }

        private List<Blackboard> GetTierClubList(int clubTier)
        {
            var clubBB = clubListBB[clubTier];
            if(clubBB != null)
            {
                return BlackboardUtils.FindVariable<List<Blackboard>>(clubBB, "tierClubList")?.value ?? new List<Blackboard>();
            }
            return new List<Blackboard>();
        }

        public void OpenCommonPopup()
        {
            MetaPopupUtils.OpenPopupAsync(this, MetaStringDefine.LOBBY_BUNDLE_NAME, "Popup Common OK Scene", GameObject.Find("Popup Manager/Area").transform, OnLoadCommonPopup);
        }

        private void OnLoadCommonPopup(SceneLoadOperation _sceneOperation)
        {
            GameObject popupGO = _sceneOperation.GetScene();
            Blackboard bb = popupGO.GetComponent<Blackboard>();

            bb.SetValue("owner", gameObject.transform);

            bb.SetValue("eventButtonYes", "OnCloseToRefresh");
            bb.SetValue("title", StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CLUB_ALL_LEAGUE_TIME_OUT"));
            bb.SetValue("buttonYesText", StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY"));
            bb.SetValue("autoCloseYes", true);
            bb.SetValue("autoCloseX", true);
            bb.SetValue("useCloseButton", false);

            PopupManager.Instance.Open(popupGO);
            popupGO.gameObject.SetActive(true);
        }

        public void ClearList()
        {
            SetCellLoading(true);
            SetActiveClubCell(false);
            // club cell list remove
            osaClubCellController.ResetParams();
            osaClubCellController.ResetItems(0);
            // club list clear
            ResetClubBlackboard();

            BlackboardUtils.DestroyBlackboard(rootBB, "clubRankingListPrefetchResponse");
            BlackboardUtils.DestroyBlackboard(rootBB, "clubRankingListResponse");
        }

        public void UpdateClubInfo(long joinClubId)
        {
            isClubber = GetMyClubId() > 0L;
            myClubLeagueTier = currentLeagueTier;
            RefreshScrollLeagueTier();
            InitFloatingData();
        }

        private void UpdateFloatingArea()
        {
            if (IsActiveFloatingArea())
                CheckActiveFloatingArea();
        }

        private void PopupListNotReady()
        {
            MetaPopupUtils.OpenPopupAsync(this, MetaStringDefine.LOBBY_BUNDLE_NAME, "Popup Common OK Scene", GameObject.Find("Popup Manager/Area").transform, OnLoadListNotReadyPopup);
        }

        private void OnLoadListNotReadyPopup(SceneLoadOperation _sceneOperation)
        {
            GameObject popupGO = _sceneOperation.GetScene();
            Blackboard bb = popupGO.GetComponent<Blackboard>();

            bb.SetValue("owner", gameObject.transform);

            bb.SetValue("eventButtonYes", "OnClosePopup");
            bb.SetValue("title", StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CLUB_ALL_LEAGUE_LIST_NOT_READY"));
            bb.SetValue("buttonYesText", StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY"));
            bb.SetValue("autoCloseYes", true);
            bb.SetValue("autoCloseX", true);
            bb.SetValue("useCloseButton", false);

            PopupManager.Instance.Open(popupGO);
            popupGO.gameObject.SetActive(true);
        }

        public void BIClientClubRankingPopup(string contextId, string enterType)
        {
            Blackboard myBiClubBB = GetMyClubData();

            Dictionary<string, object> customData = new Dictionary<string, object>();
            int userMyClubRank = BlackboardUtils.FindVariable<int>(myBiClubBB, "rank")?.value ?? 0;

            customData["is_clubber"] = isClubber;
            customData["user_club_tier"] = isClubber ? myClubLeagueTier : (int?)null;
            customData["user_club_rank"] = isClubber ? (userMyClubRank == 0 ? (int?)null : userMyClubRank) : (int?)null;
            customData["entry_flow"] = enterType;
            customData["context_id"] = contextId;

            Analytics.CustomEvent("client_club_ranking_popup", customData);
        }

        public void BIClientClubRankingPopupClubProfile(string contextId, long clubId)
        {
            int clubRank = 0;
            int leagueTier = 0;

            Blackboard clubBB = GetClubInList(clubId, osaClubCellController.GetDataBB());
            if (clubBB != null)
            {
                clubRank = clubBB.GetValue<int>("rank");
                leagueTier = clubBB.GetValue<int>("leagueTier");
            }

            if (clubRank == 0) // hoxy?
            {
                clubBB = GetClubInList(clubId, GetTierClubList(currentLeagueTier));
                if (clubBB != null)
                {
                    clubRank = clubBB.GetValue<int>("rank");
                    leagueTier = clubBB.GetValue<int>("leagueTier");
                }
            }

            Dictionary<string, object> customData = new Dictionary<string, object>();

            customData["club_id"] = clubId;
            customData["club_tier"] = leagueTier;
            customData["club_rank"] = clubRank;
            customData["context_id"] = contextId;

            Analytics.CustomEvent("client_club_ranking_popup_club_profile", customData);
        }

        #region Client API
        public void RequestClubLeagueRankingListPrefetch()
        {
            BagelCodeClientAPI.RequestClubLeagueRankingListPrefetch(RequestClubRankingListPrefetch, ErrorCallback);
        }

        private void RequestClubRankingListPrefetch(ClubLeagueRankingListPrefetchResponse response)
        {
            if (rootBB == null) return;
            Blackboard bb = (Blackboard)BlackboardUtils.GetOrCreateBlackboard(rootBB, "clubRankingListPrefetchResponse");

            BlackboardUtils.ClearBlackboard(bb);
            ClientAPI2Blackboard.Serialize(bb, response);

            myClubLeagueTier = currentLeagueTier = response.userClubLeagueTier;
            lastUpdateTimestamp = response.lastUpdatedTimestamp;
            isListNotReady = response.listNotReady;

            if (isClubber == false) myClubLeagueTier = response.maxClubLeagueTier + 1;

            long userClubLeagueIndex = response.userClubLeagueIndex;
            if (userClubLeagueIndex < (PREFETCH_ITEMS_COUNT / 2))
                startIndex = 0;
            else
                startIndex = response.userClubLeagueIndex;

            if (isListNotReady == true)
                PopupListNotReady();
        }

        private void ErrorCallback(BagelCodeHTTPError error)
        {
            switch (error.errorCode)
            {
                case ClientModels.Error.SOMEONE_UPDATING_CLUB_ERROR:
                    GlobalErrorHandler.OpenAlertPopup("ERROR_SOMETHING_WRONG");
                    break;
                case ClientModels.Error.NOT_IN_CLUB_ERROR:
                    {
                        var meClubID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubId");
                        if (meClubID.value > 0)
                        {
                            bool stringError = false;
                            ErrorPopupInfo info = new ErrorPopupInfo();
                            info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CLUB_REMOVED_ALERT", out stringError);
                            info.type = ErrorPopupType.OK;
                            info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);

                            ErrorPopupHandler.Instance.OpenError(info);
                        }

                        BlackboardQueryUtils.SetMyClubId(0);
                        MessageDispatcher.Dispatch("OnMetaUIEvent", new EventData("OnRemovedClub"));
                    }
                    break;
                default:
                    GlobalErrorHandler.GlobalError(error);
                    break;
            }
        }

        public void RequestClubLeagueRankingList(int clubTier, int _startIndex, int count, bool isFirst = false)
        {
            requestSuccess = false;
            isFirstItem = isFirst;

            SetCellLoading(true);

            startIndex = _startIndex;

            BagelCodeClientAPI.RequestClubLeagueRankingList(clubTier, _startIndex, count, RequestClubRankingList, ErrorCallback);
        }

        private void RequestClubRankingList(ClubLeagueRankingListResponse response)
        {
            if (rootBB == null) return;
            Blackboard bb = (Blackboard)BlackboardUtils.GetOrCreateBlackboard(rootBB, "clubRankingListResponse");

            BlackboardUtils.ClearBlackboard(bb);
            ClientAPI2Blackboard.Serialize(bb, response);

            if (lastUpdateTimestamp == 0L) lastUpdateTimestamp = response.lastUpdatedTimestamp;

            if (response.clubList.Count > 0 && lastUpdateTimestamp == response.lastUpdatedTimestamp)
            {
                if (isFirstItem)
                {
                    InsertClubList(currentLeagueTier, response.clubList, true);
                    List<Blackboard> tierClubList = GetTierClubList(currentLeagueTier);

                    osaClubCellController.ResetParams(tierClubList);
                    osaClubCellController.InsertToFirstItems(response.clubList.Count, false, true);
                    if (currentEndIndex == 0L) currentEndIndex = currentEndIndex + response.clubList.Count;
                    if (currentStartIndex < 0L || currentStartIndex > startIndex) currentStartIndex = startIndex;
                }
                else
                {
                    InsertClubList(currentLeagueTier, response.clubList, false);
                    List<Blackboard> tierClubList = GetTierClubList(currentLeagueTier);

                    int insertIndex = osaClubCellController.GetItemsCount();
                    osaClubCellController.ResetParams(tierClubList);
                    osaClubCellController.InsertItems(insertIndex, response.clubList.Count, false, true);
                    currentEndIndex = startIndex + response.clubList.Count;
                    if (currentStartIndex < 0L) currentStartIndex = startIndex;
                }
            }

            requestSuccess = true;
            SetCellLoading(false);
            SetActiveClubCell(true);
            CheckActiveFloatingArea();

            if (lastUpdateTimestamp < response.lastUpdatedTimestamp)
                SetRefresh(true);

            lastUpdateTimestamp = response.lastUpdatedTimestamp >= lastUpdateTimestamp ? response.lastUpdatedTimestamp : lastUpdateTimestamp;
        }

        private List<ClubListInfo> DummyRequestClubRaankingList()
        {
            List<ClubListInfo> clubListInfo = new List<ClubListInfo>();

            Blackboard myClubInfo = BlackboardUtils.FindValue<Blackboard>(null, "/clubInfo");
            for (int i = 0; i < 30; ++i)
            {
                ClubListInfo clubInfo = new ClubListInfo
                {
                    id = myClubInfo.GetValue<long>("id"),
                    name = myClubInfo.GetValue<string>("name"),
                    symbol = myClubInfo.GetValue<string>("symbol"),
                    motd = "Dummy" + (i + startIndex).ToString() + " : " + myClubInfo.GetValue<string>("motd"),
                    members = 0,
                    level = myClubInfo.GetValue<int>("level"),
                    friends = 0,
                    leagueTier = 0,
                    leaguePoint = 0L,
                    minPlayerLevel = myClubInfo.GetValue<int>("minPlayerLevel"),
                    joinType = myClubInfo.GetValue<ClubJoinType>("joinType"),
                    rank = i + (int)startIndex
                };
                clubListInfo.Add(clubInfo);
            }
            return clubListInfo;
        }
        #endregion
    }
}