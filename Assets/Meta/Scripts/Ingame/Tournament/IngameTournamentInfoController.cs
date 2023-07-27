using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class IngameTournamentInfoController : MonoBehaviour
    {
        private Blackboard     rootBlackboard;
        private Animator       rootAnimator;
        private ContextElement rootElement;

        private ContextElement bottomInfoElement;
        private ContextElement infoElement;
        private ContextElement prizesElement;
        private ContextElement topUsersElement;
        private ContextElement loadingElement;

        private ContextElement infoRoundTextElement;
        private ContextElement infoPrizeTextElement;
        private ContextElement infoTopTextElement;
        private ContextElement infoRoundInfosElement;

        private List<ContextElement> topUserCellElementList;
        private int topUserActiveCount;

        private bool isInit = false;
        private bool isActive = false;
        private bool isReady = false;
        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

        private static int lastSelectTab = 0;

        private Blackboard _tournamentBB;
        public Blackboard tournamentBB
        {
            get
            {
                if(_tournamentBB == null)
                    _tournamentBB = BlackboardUtils.GetOrCreateVariable<Blackboard>(MainBlackboard.Get(), "responseTournamentInfo").value;

                return _tournamentBB;
            }
        }

        public List<Blackboard> topRankList
        {
            get
            {
                var mainTournamentBB  = BlackboardUtils.GetOrCreateVariable<Blackboard>(ContentBlackboard.Get(), "tournamentInfo").value;
                return mainTournamentBB.GetValue<List<Blackboard>>("topRankList");
            }
        }

        private Variable<bool> autoSpin;
        public bool AutoSpin { get { return autoSpin != null && autoSpin.value; } }

        private void Awake()
        {
            autoSpin = BlackboardUtils.FindVariable<bool>("./autoSpin");
        }
        
        private void OnDisable()
        {
            StopAllCoroutines();
        }

        public void OnInitTournament()
        {
            if(isInit) return;

            rootBlackboard = gameObject.GetComponent<Blackboard>();
            rootAnimator = gameObject.GetComponent<Animator>();

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            bottomInfoElement = ContextUtils.FindElement(rootElement, "Bottom Info", ContextSearchingType.ChildrenSearch);
            infoElement       = ContextUtils.FindElement(rootElement, "Info", ContextSearchingType.ChildrenSearch);
            prizesElement     = ContextUtils.FindElement(rootElement, "Prizes", ContextSearchingType.ChildrenSearch);
            topUsersElement   = ContextUtils.FindElement(rootElement, "Top 3", ContextSearchingType.ChildrenSearch);
            loadingElement    = ContextUtils.FindElement(rootElement, "Loading", ContextSearchingType.ChildrenSearch);

            // Infos
            infoRoundTextElement  = ContextUtils.FindElement(infoElement, "Current Round Info/Text Round", ContextSearchingType.FullNameSearch);
            infoPrizeTextElement  = ContextUtils.FindElement(infoElement, "Current Round Info/Text Prize", ContextSearchingType.FullNameSearch);
            infoTopTextElement    = ContextUtils.FindElement(infoElement, "Top Text", ContextSearchingType.ChildrenSearch);
            infoRoundInfosElement = ContextUtils.FindElement(infoElement, "Round Info Anchor", ContextSearchingType.ChildrenSearch);

            // Users
            topUserCellElementList = new List<ContextElement>();
            for(int i=0; i < 3; ++i)
            {
                var userElement = ContextUtils.FindElement(topUsersElement, string.Format("Cell {0}", i+1), ContextSearchingType.ChildrenSearch);
                topUserCellElementList.Add(userElement);
                GameObject userProfileObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Profile Picture Small", userElement.transform, "Profile Area", "Profile");
                userElement.UpdateContext(true);
            }

            GameObject refreshButtonObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Button Secondary", topUsersElement.transform, "Button Refresh Area", "Refresh Button");
            var topUserRefreshButtonElement = refreshButtonObj.GetComponent<ContextElement>();
            topUserRefreshButtonElement.UpdateContext(true);
            MetaContextElementUtils.SimpleSetText(topUserRefreshButtonElement, "Text", StringTableUtils.GetString(tableType, "TOURNAMENT_TOP_USER_REFRESH_BUTTON_TEXT"));

            IContextClickable refreshClickableElement = topUserRefreshButtonElement as IContextClickable;
            if (refreshClickableElement != null)
            {
                refreshClickableElement.RemoveAllListener();
                refreshClickableElement.AddListenerOnClick( (ContextElement s) =>
                {
                    OnUpdateTournamentInfo();
                });
            }

            IContextClickable tabClickableElement = rootElement as IContextClickable;
            if (tabClickableElement != null)
            {
                tabClickableElement.RemoveAllListener();
                tabClickableElement.AddListenerOnClick( (ContextElement s) =>
                {
                    OnSelectTab();
                });
            }

            isInit = true;
        }

        public void OnSetActive(bool isActive)
        {
            this.isActive = isActive;

            rootAnimator.SetBool("Active", isActive);
            rootAnimator.SetBool("TabContentsOpen", isActive);

            isReady = false;
            
            if(isActive)
            {
                UpdateTab(-1);
                UpdateTopUsers();
                MetaContextElementUtils.SetIntProperty(rootElement, lastSelectTab);

                BagelCodeClientAPI.TournamentInfo(
                (response) =>
                {
                    var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "responseTournamentInfo");
                    ClientAPI2Blackboard.Serialize(bb, response.info);
                    OnResponse();
                },
                (error) =>
                {
                    GlobalErrorHandler.GlobalError(error);
                });
            }
            else
            {
                // StopCoroutine("ReqeustTournamentInfo");

                // Destroy BB
            }
        }

        public void OnUpdateTournamentInfo()
        {
            if(isActive)
            {
                UpdateTopUsers();
                UpdateToUsersPrize();
            }
        }

        private void OnResponse()
        {
            if(!isActive) return;

            UpdateInfos();
            UpdateAllPrizes();
            UpdateToUsersPrize();
            UpdateTab(lastSelectTab);

            isReady = true;
        }

        private void OnSelectTab()
        {
            lastSelectTab = MetaContextElementUtils.GetIntProperty(rootElement);

            switch(lastSelectTab)
            {
                case 0:
                    BI_client_click_tournament_ui("info");
                    break;
                case 1:
                    BI_client_click_tournament_ui("prize");
                    break;
                case 2:
                    BI_client_click_tournament_ui("top3");
                    break;
            }
            

            if(isReady)
            {
                UpdateTab(lastSelectTab);
            }
        }

        private void UpdateTab(int index)
        {
            bottomInfoElement.gameObject.SetActive(index == 0 || index == 1);
            infoElement.gameObject.SetActive(index == 0);
            prizesElement.gameObject.SetActive(index == 1);
            topUsersElement.gameObject.SetActive(index == 2);
            loadingElement.gameObject.SetActive(index == -1);
        }

        private void UpdateInfos()
        {
            int round = tournamentBB.GetValue<int>("serialWinCount");
            double prizeMultiplier = tournamentBB.GetValue<double>("serialWinBonus");
            int maxRankToAdvance = tournamentBB.GetValue<int>("maxRankToAdvance");

            MetaContextElementUtils.SetText(infoRoundTextElement, StringTableUtils.GetString(tableType, "TOURNAMENT_INFO_ROUND_TEXT", round + 1));
            MetaContextElementUtils.SetText(infoPrizeTextElement, StringTableUtils.GetString(tableType, "TOURNAMENT_INFO_MULTIPLIER_TEXT", prizeMultiplier));
            MetaContextElementUtils.SetText(infoTopTextElement, StringTableUtils.GetString(tableType, "TOURNAMENT_INFO_DESCRIPTION_TEXT", maxRankToAdvance));

            List<double> prizeMultiplierList = tournamentBB.GetValue<List<double>>("serialWinBonusList");

            for(int i=0; i < 10; ++i)
            {
                string cellName = string.Format("Cell {0}", i+1);
                ContextElement cellElement = ContextUtils.FindElement(infoRoundInfosElement, cellName, ContextSearchingType.ChildrenSearch);
                if(cellElement != null)
                {
                    if(prizeMultiplierList.Count > i)
                    {
                        string textRound = StringTableUtils.GetString(tableType, "TOURNAMENT_INFO_CELL_ROUND_TEXT", i + 1);
                        if(prizeMultiplierList.Count == i + 1)
                            textRound += "+";

                        MetaContextElementUtils.SimpleSetText(cellElement, "Text Round", textRound);
                        MetaContextElementUtils.SimpleSetText(cellElement, "Text Multiplier", StringTableUtils.GetString(tableType, "TOURNAMENT_INFO_CELL_MULTIPLIER_TEXT", prizeMultiplierList[i]));
                    }

                    cellElement.gameObject.SetActive(prizeMultiplierList.Count > i);
                }
            }
        }

        private void UpdateAllPrizes()
        {
            List<Blackboard> prizeList = tournamentBB.GetValue<List<Blackboard>>("prizeList");

            for(int i=0; i < prizeList.Count; ++i)
            {
                int fromRank = prizeList[i].GetValue<int>("fromRank");
                int toRank = prizeList[i].GetValue<int>("toRank");
                long actualPrize = prizeList[i].GetValue<long>("actualPrize");

                if(fromRank == toRank)
                    MetaContextElementUtils.SimpleSetText(prizesElement, string.Format("Cell {0} Text Rank", i+1), StringTableUtils.GetString(tableType, "TOURNAMENT_PRIZE_RANK_SINGLE", toRank));
                else
                    MetaContextElementUtils.SimpleSetText(prizesElement, string.Format("Cell {0} Text Rank", i+1), StringTableUtils.GetString(tableType, "TOURNAMENT_PRIZE_RANK_DOUBLE", fromRank, toRank));

                MetaContextElementUtils.SimpleSetText(prizesElement, string.Format("Cell {0} Text Prizes", i+1), StringTableUtils.GetString(tableType, "TOURNAMENT_PRIZE_RANK_PRIZE", actualPrize));
            }
        }

        private void UpdateTopUsers()
        {
            List<Blackboard> userBBList = topRankList;

            topUserActiveCount = 0;

            for(int i=0; i<topUserCellElementList.Count; ++i)
            {
                var pictureElement = ContextUtils.FindElement(topUserCellElementList[i], "Profile Area/Profile", ContextSearchingType.FullNameSearch);
                var imageElement = ContextUtils.FindElement(pictureElement, "Image", ContextSearchingType.ChildrenSearch);
                if(userBBList != null && userBBList.Count > i)
                {
                    long score       = userBBList[i].GetValue<long>("score");
                    var  userProfile = userBBList[i].GetValue<Blackboard>("profile");
                    int  tierGroup   = TierUtils.GetTierGroup(userProfile.GetValue<int>("tier"));
                    string userId    = userProfile.GetValue<string>("userId");
                    string userName  = userProfile.GetValue<string>("name");
                    string profileUrl= userProfile.GetValue<string>("profileUrl");
                    int  reportCount = userProfile.GetValue<int>("reportCount");

                    bool isIgnored = MetaSystem.IsIgnoredUser(userId, reportCount);
                    if(string.IsNullOrEmpty(profileUrl) || isIgnored)
                    {
                        imageElement.gameObject.SetActive(false);
                    }
                    else
                    {
                        imageElement.gameObject.SetActive(true);
                        MetaContextElementUtils.SetWebImage(imageElement, profileUrl, CacheType.MemCache, true, null);
                    }

                    MetaContextElementUtils.SetBlackboardValue<int>(pictureElement, "tierGroup", tierGroup);
                    MetaContextElementUtils.SimpleSetText(topUserCellElementList[i], "Text Name", StringTableUtils.GetString(tableType, "TOURNAMENT_TOP_USER_NAME_TEXT", userName));
                    MetaContextElementUtils.SimpleSetText(topUserCellElementList[i], "Text Score", StringTableUtils.GetString(tableType, "TOURNAMENT_TOP_USER_SCORE_TEXT", score));

                    MetaContextElementUtils.SetBlackboardValue<string>(topUserCellElementList[i], "userID", userId);

                    ++topUserActiveCount;
                }
                else
                {
                    // Empty
                    imageElement.gameObject.SetActive(false);
                    MetaContextElementUtils.SetBlackboardValue<int>(pictureElement, "tierGroup", 0);
                    MetaContextElementUtils.SetBlackboardValue<string>(topUserCellElementList[i], "userID", "");

                    MetaContextElementUtils.SimpleSetIntProperty(topUserCellElementList[i], "Profile Area/Profile", 0, ContextSearchingType.FullNameSearch);
                    MetaContextElementUtils.SimpleSetText(topUserCellElementList[i], "Text Name", StringTableUtils.GetString(tableType, "TOURNAMENT_TOP_USER_NAME_TEXT", "-"));
                    MetaContextElementUtils.SimpleSetText(topUserCellElementList[i], "Text Score", StringTableUtils.GetString(tableType, "TOURNAMENT_TOP_USER_SCORE_EMPTY_TEXT"));
                }
            }
        }

        private void UpdateToUsersPrize()
        {
            for(int i=0; i<topUserCellElementList.Count; ++i)
            {
                if(topUserActiveCount > i)
                {
                    long prize = GetRankToPrize(i+1);
                    MetaContextElementUtils.SimpleSetText(topUserCellElementList[i], "Text Prize", StringTableUtils.GetString(tableType, "TOURNAMENT_TOP_USER_PRIZE_TEXT", prize));
                }
                else
                {
                    MetaContextElementUtils.SimpleSetText(topUserCellElementList[i], "Text Prize", StringTableUtils.GetString(tableType, "TOURNAMENT_TOP_USER_PRIZE_EMPTY_TEXT"));
                }
            }
        }

        private long GetRankToPrize(int rank)
        {
            List<Blackboard> prizeList = tournamentBB.GetValue<List<Blackboard>>("prizeList");

            for(int i=0; i<prizeList.Count; ++i)
            {
                int fromRank = prizeList[i].GetValue<int>("fromRank");
                int toRank = prizeList[i].GetValue<int>("toRank");

                if(fromRank <= rank && rank <= toRank)
                    return prizeList[i].GetValue<long>("actualPrize");
            }

            return 0;
        }

        public void OnSpinEvent(bool isSpin)
        {
            if(topUserCellElementList == null || topUserCellElementList.Count == 0) return;
            if(!isSpin && AutoSpin) return;

            for(int i=0; i<topUserCellElementList.Count; ++i)
            {
                MetaContextElementUtils.SetBlackboardValue<bool>(topUserCellElementList[i], "isSpin", isSpin);
            }
        }

        private void BI_client_click_tournament_ui(string type)
        {
            Analytics.CustomEvent("client_click_tournament_ui", new Dictionary<string, object>
            {
                { "type", type }
            });
        }
    }
}