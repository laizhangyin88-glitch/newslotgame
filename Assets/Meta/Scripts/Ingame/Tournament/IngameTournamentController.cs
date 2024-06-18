using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class IngameTournamentController : MonoBehaviour
    {
        private Blackboard rootBlackboard;
        private Animator rootAnimator;
        private Animator prizePoolAnimator;

        private ContextElement animatorElement;
        private ContextElement prizeCellAreaElement;
        private ContextElement prizePoolTitleElement;
        private ContextElement prizePoolCoinTextElement;
        private ContextElement prizePoolMultiplierTextElement;
        private ContextElement prizePoolResultLoseRankTextElement;
        private ContextElement prizePoolResultLoseRoundTextElement;
        private ContextElement rankTextElement;
        private ContextElement moreOpenButtonElement;
        private ContextElement moreCloseButtonElement;
        private ContextElement moreCoverElement;
        private ContextElement moreContentsAreaElement;
        private ContextElement remainingTimerTextElement;
        private ContextElement remainingTimerGaugeElement;

        private ContextElement roundElement;
        private ContextElement roundStartElement;
        private ContextElement roundInfoElement;
        private ContextElement roundResultElement;
        private Animator roundAnimator;

        private ContextElement moreInfoElement;
        private IngameTournamentInfoController moreInfoController;
        private ContextElement moreInfoOpenButtonElement;
        private ContextElement moreInfoCloseButtonElement;
        private ContextElement moreInfoCoverElement;
        private ContextElement moreInfoOutsideAreaElement;

        private List<ContextElement> userProfileList;

        private string trophyActiveFormat = "";
        private string trophyInactiveFormat = "";

        private bool isInit = false;
        private bool isShow = false;
        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

        private TournamentStatus tournamentStatus = TournamentStatus.UNKNOWN;

        private RemainingTimerGaugeController timerGaugeController;
        private RemainingTimerController timerController;

        private bool isFold = false;
        private bool isPlayingEffect = false;

        private float waitTIme = 1.0f;

        private string tournamentID = "";
        private int tournamentRound = -1;
        private int tournamentRank = -1;

        private Blackboard _tournamentBB;
        public Blackboard tournamentBB
        {
            get
            {
                if(_tournamentBB == null)
                {
                    var info = BlackboardUtils.GetOrCreateVariable<Blackboard>(ContentBlackboard.Get(), "tournamentInfo");
                    if(info != null)
                        _tournamentBB = info.value;
                }

                return _tournamentBB;
            }
        }

        private const float UPDATE_CHECK_TIME = 0.6f;

        private const string PLAYERPREFS_TOURNAMENT_FOLD_KEY = "Tournament_Fold";

        private const string TOURNAMENT_ROUND_TEXT = "TOURNAMENT_ROUND_TEXT";
        private const string TOURNAMENT_TROPHY_RANK_TEXT = "TOURNAMENT_TROPHY_RANK_TEXT";
        private const string TOURNAMENT_TROPHY_RANK_EMPTY_TEXT = "TOURNAMENT_TROPHY_RANK_EMPTY_TEXT";
        private const string TOURNAMENT_PRIZE_POOL_TEXT = "TOURNAMENT_PRIZE_POOL_TEXT";
        private const string TOURNAMENT_PRIZE_MULTIPLIER_TEXT = "TOURNAMENT_PRIZE_MULTIPLIER_TEXT";
        private const string TOURNAMENT_START_TITLE = "TOURNAMENT_START_TITLE";
        private const string TOURNAMENT_START_TEXT = "TOURNAMENT_START_TEXT";
        private const string TOURNAMENT_END_LOSE_RANK_TEXT = "TOURNAMENT_END_LOSE_RANK_TEXT";
        private const string TOURNAMENT_END_LOSE_ROUND_TEXT = "TOURNAMENT_END_LOSE_ROUND_TEXT";
        private const string TOURNAMENT_END_COVER_ROUND_TEXT = "TOURNAMENT_END_COVER_ROUND_TEXT";
        private const string TOURNAMENT_END_COVER_PRIZE_TEXT = "TOURNAMENT_END_COVER_PRIZE_TEXT";
        private const string TOURNAMENT_RESULT_WIN_TEXT = "TOURNAMENT_RESULT_WIN_TEXT";
        private const string TOURNAMENT_RESULT_WIN_DESC_TEXT = "TOURNAMENT_RESULT_WIN_DESC_TEXT";
        private const string TOURNAMENT_RESULT_LOSE_TEXT = "TOURNAMENT_RESULT_LOSE_TEXT";
        private const string TOURNAMENT_RESULT_LOSE_DESC_TEXT = "TOURNAMENT_RESULT_LOSE_DESC_TEXT";

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private void Update()
        {
            if(isShow)
            {
                waitTIme -= Time.deltaTime;

                if(waitTIme < 0f)
                {
                    UpdateTournamentStatus(false);
                    // moreInfoController.OnUpdateTournamentInfo();
                    waitTIme += UPDATE_CHECK_TIME;
                }
            }
        }

        public void OnSpinButton()
        {
            if(!isInit) return;
            if(!isShow) return;

            OnTournamentInfoClose();
        }

        public void OnInitTournament()
        {
            if(isInit) return;

            rootBlackboard = gameObject.GetComponent<Blackboard>();

            // Init context.
            var rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            animatorElement = ContextUtils.FindElement(rootElement, "Animator", ContextSearchingType.ChildrenSearch);
            rootAnimator = animatorElement.gameObject.GetComponent<Animator>();

            prizeCellAreaElement = ContextUtils.FindElement(rootElement, "Animator/Prize Cell Area", ContextSearchingType.FullNameSearch);

            var prizePoolAnimatorElement = ContextUtils.FindElement(rootElement, "Animator/Prize Pool", ContextSearchingType.FullNameSearch);
            prizePoolAnimator = prizePoolAnimatorElement.gameObject.GetComponent<Animator>();

            prizePoolTitleElement = ContextUtils.FindElement(prizePoolAnimatorElement, "Text Round", ContextSearchingType.ChildrenSearch);
            prizePoolCoinTextElement = ContextUtils.FindElement(prizePoolAnimatorElement, "Text Pool Coin", ContextSearchingType.ChildrenSearch);
            prizePoolMultiplierTextElement = ContextUtils.FindElement(prizePoolAnimatorElement, "Text Pool Multiplier", ContextSearchingType.ChildrenSearch);
            prizePoolResultLoseRankTextElement = ContextUtils.FindElement(prizePoolAnimatorElement, "Text Result Rank", ContextSearchingType.ChildrenSearch);
            prizePoolResultLoseRoundTextElement = ContextUtils.FindElement(prizePoolAnimatorElement, "Text Result Round", ContextSearchingType.ChildrenSearch);
            rankTextElement = ContextUtils.FindElement(prizePoolAnimatorElement, "Text Rank", ContextSearchingType.ChildrenSearch);

            moreOpenButtonElement = ContextUtils.FindElement(rootElement, "Animator/Button More Open", ContextSearchingType.FullNameSearch);
            moreCloseButtonElement = ContextUtils.FindElement(rootElement, "Animator/Button More Close", ContextSearchingType.FullNameSearch);
            moreCoverElement = ContextUtils.FindElement(rootElement, "Animator/Button More Cover", ContextSearchingType.FullNameSearch);
            moreContentsAreaElement = ContextUtils.FindElement(rootElement, "Animator/Button More Contents Area", ContextSearchingType.FullNameSearch);

            remainingTimerGaugeElement = ContextUtils.FindElement(rootElement, "Animator/Gauge", ContextSearchingType.FullNameSearch);
            remainingTimerTextElement  = ContextUtils.FindElement(rootElement, "Animator/Gauge/Text Time", ContextSearchingType.FullNameSearch);

            timerGaugeController = remainingTimerGaugeElement.gameObject.AddComponent<RemainingTimerGaugeController>();
            timerController = remainingTimerTextElement.gameObject.AddComponent<RemainingTimerController>();

            timerGaugeController.Init(remainingTimerGaugeElement, rootAnimator, true, "Time", null);
            timerController.Init(remainingTimerTextElement, "TIME_FORMAT_MMSS", "TOURNAMENT_TIMER_NORMAL_TEXT", "", "00:00", false, OnTimerCallback);

            userProfileList = new List<ContextElement>();

            for(int i=0; i<3; ++i)
            {
                var profileElement = ContextUtils.FindElement(rootElement, string.Format("Animator/Cell {0:00}", i+1), ContextSearchingType.FullNameSearch);
                userProfileList.Add(profileElement);
            }

            isFold = (PlayerPrefs.GetInt(PLAYERPREFS_TOURNAMENT_FOLD_KEY, (false ? 1 : 0)) > 0) ? true : false;
            rootAnimator.SetBool("IsFold", isFold);

            var currentStatus = BlackboardUtils.FindVariable<TournamentStatus>(ContentBlackboard.Get(), "tournamentInfo/status").value;
            rootAnimator.SetBool("Active", false);

            // Make Prize Cell
            GameObject prizeCellObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "In Game Tournament Prize Cell", prizeCellAreaElement.transform, "");
            roundElement = prizeCellObj.GetComponent<ContextElement>();
            roundElement.UpdateContext(false);

            roundStartElement  = ContextUtils.FindElement(roundElement, "Start", ContextSearchingType.ChildrenSearch);
            roundInfoElement   = ContextUtils.FindElement(roundElement, "Round", ContextSearchingType.ChildrenSearch);
            roundResultElement = ContextUtils.FindElement(roundElement, "Result", ContextSearchingType.ChildrenSearch);

            roundAnimator = prizeCellObj.GetComponent<Animator>();
            roundAnimator.SetBool("Active", false);

            // Make More Cell
            GameObject moreCellObj = null;
#if UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR
            if(Screen.width < Screen.height)
                moreCellObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "In Game Tournament Tab Vertical Mode", moreContentsAreaElement.transform, "");
            else
#endif
                moreCellObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "In Game Tournament Tab", moreContentsAreaElement.transform, "");

            moreInfoController = moreCellObj.GetComponent<IngameTournamentInfoController>();
            moreInfoController.OnInitTournament();

            var moreInfoElement = moreCellObj.GetComponent<ContextElement>();
            moreInfoElement.UpdateContext(false);

            moreInfoOpenButtonElement = ContextUtils.FindElement(animatorElement, "Button More Open", ContextSearchingType.ChildrenSearch);
            moreInfoCloseButtonElement = ContextUtils.FindElement(animatorElement, "Button More Close", ContextSearchingType.ChildrenSearch);
            moreInfoCoverElement = ContextUtils.FindElement(animatorElement, "Button More Cover", ContextSearchingType.ChildrenSearch);

            moreInfoOutsideAreaElement = ContextUtils.FindElement(moreInfoElement, "Button More Outside Area", ContextSearchingType.ChildrenSearch);

            trophyActiveFormat = StringTableUtils.GetString(tableType, "TOURNAMENT_TROPHY_ACTIVE");
            trophyInactiveFormat = StringTableUtils.GetString(tableType, "TOURNAMENT_TROPHY_INACTIVE");

            var unfoldElement = ContextUtils.FindElement(animatorElement, "Button Open", ContextSearchingType.ChildrenSearch);
            IContextClickable unfoldClickableElement = unfoldElement as IContextClickable;
            if (unfoldClickableElement != null)
            {
                unfoldClickableElement.RemoveAllListener();
                unfoldClickableElement.AddListenerOnClick( (ContextElement s) =>
                {
                    isFold = false;
                    OnSetFold(false);
                    BI_client_click_tournament_ui(isFold ? "collapse" : "expand");
                });
            }

            var foldElement = ContextUtils.FindElement(animatorElement, "Button Close", ContextSearchingType.ChildrenSearch);
            IContextClickable foldClickableElement = foldElement as IContextClickable;
            if (foldClickableElement != null)
            {
                foldClickableElement.RemoveAllListener();
                foldClickableElement.AddListenerOnClick( (ContextElement s) =>
                {
                    isFold = true;
                    OnSetFold(true);
                    BI_client_click_tournament_ui(isFold ? "collapse" : "expand");
                });
            }

            IContextClickable foldToggleClickableElement = prizePoolAnimatorElement as IContextClickable;
            if (foldToggleClickableElement != null)
            {
                foldToggleClickableElement.RemoveAllListener();
                foldToggleClickableElement.AddListenerOnClick( (ContextElement s) =>
                {
                    isFold = !isFold;
                    OnSetFold(isFold);
                    BI_client_click_tournament_ui(isFold ? "collapse" : "expand");
                });
            }

            IContextClickable moreInfoOpenClickableElement = moreInfoOpenButtonElement as IContextClickable;
            if (moreInfoOpenClickableElement != null)
            {
                moreInfoOpenClickableElement.RemoveAllListener();
                moreInfoOpenClickableElement.AddListenerOnClick( (ContextElement s) =>
                {
                    OnTournamentInfoOpen();
                    BI_client_click_tournament_ui("more_open");
                });
            }

            IContextClickable moreInfoCloseClickableElement = moreInfoOutsideAreaElement as IContextClickable;
            if (moreInfoCloseClickableElement != null)
            {
                moreInfoCloseClickableElement.RemoveAllListener();
                moreInfoCloseClickableElement.AddListenerOnClick( (ContextElement s) =>
                {
                    OnTournamentInfoClose();
                    BI_client_click_tournament_ui("more_close");
                });
            }

            isInit = true;
        }

        public void OnUpdateTournamentVariables()
        {
            TournamentStatus targetStatus = tournamentBB.GetValue<TournamentStatus>("status");
            string currentTournamentID = tournamentBB.GetValue<string>("tournamentId");

            tournamentStatus = targetStatus;
            tournamentID = currentTournamentID;

            UpdateTimer(targetStatus);
            UpdateVariables();
        }

        public void OnShowTournament()
        {
            isShow = true;
            UpdateTournamentStatus(true);
            OnTournamentInfoClose();

            switch(tournamentStatus)
            {
                case TournamentStatus.BREAKING:
                    UpdateRoundBreak();
                    break;
            }
        }

        private void UpdateTournamentStatus(bool isForce)
        {
            if(tournamentBB == null) return;

            if(!isForce)
            {
                var isChanged = BlackboardUtils.GetOrCreateVariable<bool>(tournamentBB, "isChanged");

                if(isChanged.value == false) return;
                isChanged.value = false;
            }

            TournamentStatus targetStatus = tournamentBB.GetValue<TournamentStatus>("status");
            string currentTournamentID = tournamentBB.GetValue<string>("tournamentId");
            int currentRound = tournamentBB.GetValue<int>("serialWinCount");

            if(targetStatus == TournamentStatus.WAITING && isForce)
            {
                StartCoroutine("RoundEffectTournamentEnter");
            }

            if(targetStatus != tournamentStatus || tournamentID != currentTournamentID)
            {
                if(targetStatus == TournamentStatus.WAITING)
                {
                    UpdateRoundWait();
                    // ALL Start round effect.
                    if(tournamentRound > currentRound )
                        StartCoroutine("RoundEffectBackToBegin");
                }
                else if((tournamentStatus == TournamentStatus.WAITING || tournamentStatus == TournamentStatus.BREAKING)
                        && targetStatus == TournamentStatus.PLAYING)
                {
                    BI_client_tournament_start();

                    // Start Round Effect.
                    StartCoroutine("RoundEffectTournamentStart");
                }
                else if(tournamentStatus == TournamentStatus.PLAYING && targetStatus == TournamentStatus.BREAKING)
                {
                    // BI_client_tournament_break(tournamentID);

                    StartCoroutine("RoundEffectTournamentEnd");
                }
            }

            if(targetStatus != tournamentStatus || tournamentID != currentTournamentID)
                UpdateTimer(targetStatus);

            UpdateVariables();

            switch(targetStatus)
            {
                case TournamentStatus.WAITING:
                case TournamentStatus.PLAYING:
                    UpdateUserCell();
                    break;
            }

            tournamentStatus = targetStatus;
            tournamentID = currentTournamentID;
            tournamentRound = currentRound;
        }

        private void UpdateVariables()
        {
            if(tournamentBB == null) return;

            TournamentStatus currentStatus = tournamentBB.GetValue<TournamentStatus>("status");
            string currentTournamentID = tournamentBB.GetValue<string>("tournamentId");

            int round = tournamentBB.GetValue<int>("serialWinCount");
            int maxRankToAdvance = tournamentBB.GetValue<int>("maxRankToAdvance");
            double roundMultiplier = tournamentBB.GetValue<double>("serialWinBonus");
            long basePrize = tournamentBB.GetValue<long>("baseTotalPrize");
            long totalPrize = System.Convert.ToInt64(System.Convert.ToDouble(basePrize) * roundMultiplier);

            prizePoolAnimator.SetInteger("Round", round + 1);
            MetaContextElementUtils.SetText(prizePoolTitleElement, StringTableUtils.GetString(tableType, TOURNAMENT_ROUND_TEXT, round + 1));

            MetaContextElementUtils.SetText(prizePoolCoinTextElement, StringTableUtils.GetString(tableType, TOURNAMENT_PRIZE_POOL_TEXT, totalPrize));
            MetaContextElementUtils.SetText(prizePoolMultiplierTextElement, StringTableUtils.GetString(tableType, TOURNAMENT_PRIZE_MULTIPLIER_TEXT, roundMultiplier));

            int myRank = -1;

            if(currentStatus == TournamentStatus.WAITING)
            {
                MetaContextElementUtils.SetText(rankTextElement, StringTableUtils.GetString(tableType, TOURNAMENT_TROPHY_RANK_EMPTY_TEXT));
            }
            else
            {
                if(currentStatus == TournamentStatus.PLAYING)
                {
                    var meID = BlackboardUtils.GetOrCreateVariable<string>(MainBlackboard.Get(), "me/userId");
                    List<Blackboard> myRankList = tournamentBB.GetValue<List<Blackboard>>("myRankList");

                    for(int i=0; i < myRankList.Count; ++i)
                    {
                        var userId = BlackboardUtils.GetOrCreateVariable<string>(myRankList[i], "profile/userId");

                        if(userId.value == meID.value)
                        {
                            myRank = myRankList[i].GetValue<int>("rank");
                            break;
                        }
                    }

                    if(tournamentID == currentTournamentID && tournamentRank != -1)
                    {
                        if(myRank <= maxRankToAdvance && tournamentRank > maxRankToAdvance)
                        {
                            // Up
                            rootAnimator.SetBool("IsRankchange", true);
                            rootAnimator.SetTrigger("RankUp");
                        }
                        else if(myRank > maxRankToAdvance && tournamentRank <= maxRankToAdvance)
                        {
                            // Down.
                            rootAnimator.SetBool("IsRankchange", true);
                            rootAnimator.SetTrigger("RankDown");
                        }
                    }
                }

                if(myRank < 0)
                {
                    MetaContextElementUtils.SetText(rankTextElement, StringTableUtils.GetString(tableType, TOURNAMENT_TROPHY_RANK_EMPTY_TEXT));
                }
                else
                {
                    MetaContextElementUtils.SetText(rankTextElement, StringTableUtils.GetString(tableType, TOURNAMENT_TROPHY_RANK_TEXT, myRank <= maxRankToAdvance ? trophyActiveFormat : trophyInactiveFormat, myRank));
                }
            }

            tournamentRank = myRank;
        }

        private void UpdateRoundWait()
        {
            roundStartElement.gameObject.SetActive(false);
            roundInfoElement.gameObject.SetActive(false);
            roundResultElement.gameObject.SetActive(false);

            // Active
            rootAnimator.SetBool("IsContextCover", false);
            roundAnimator.SetBool("Active", false);
            prizePoolAnimator.SetBool("IsBreak", false);
            prizePoolAnimator.SetBool("IsResult", false);
        }

        private void UpdateRoundBreak()
        {
            int nextRound = tournamentBB.GetValue<int>("serialWinCount");
            double nextRoundMultiplier = tournamentBB.GetValue<double>("serialWinBonus");
            MetaContextElementUtils.SimpleSetText(roundInfoElement, "Text Title", StringTableUtils.GetString(tableType, TOURNAMENT_START_TITLE, nextRound + 1));

            // cover
            MetaContextElementUtils.SimpleSetText(animatorElement, "Text Title", StringTableUtils.GetString(tableType, TOURNAMENT_END_COVER_ROUND_TEXT, nextRound + 1));
            MetaContextElementUtils.SimpleSetText(animatorElement, "Text Context", StringTableUtils.GetString(tableType, TOURNAMENT_END_COVER_PRIZE_TEXT, nextRoundMultiplier));

            rootAnimator.SetBool("IsContextCover", true);
            prizePoolAnimator.SetBool("IsBreak", true);
        }

        private IEnumerator RoundEffectBackToBegin()
        {
            while(isPlayingEffect) yield return new WaitForSeconds(0.33f);
            isPlayingEffect = true;

            roundStartElement.gameObject.SetActive(false);
            roundInfoElement.gameObject.SetActive(false);
            roundResultElement.gameObject.SetActive(false);

            int round = tournamentBB.GetValue<int>("serialWinCount");
            MetaContextElementUtils.SetText(prizePoolResultLoseRankTextElement, "-");
            MetaContextElementUtils.SetText(prizePoolResultLoseRoundTextElement, StringTableUtils.GetString(tableType, TOURNAMENT_END_LOSE_ROUND_TEXT, round + 1));
            prizePoolAnimator.SetBool("IsResult", true);

            yield return new WaitForSeconds(3f);

            prizePoolAnimator.SetBool("IsResult", false);

            isPlayingEffect = false;
        }

        private IEnumerator RoundEffectTournamentEnter()
        {
            while(isPlayingEffect) yield return new WaitForSeconds(0.33f);
            isPlayingEffect = true;

            roundStartElement.gameObject.SetActive(true);
            roundInfoElement.gameObject.SetActive(false);
            roundResultElement.gameObject.SetActive(false);

            // Active
            rootAnimator.SetBool("IsContextCover", false);
            roundAnimator.SetBool("Active", true);
            prizePoolAnimator.SetBool("IsBreak", false);
            prizePoolAnimator.SetBool("IsResult", false);

            yield return new WaitForSeconds(3f);
            // Deactive

            roundAnimator.SetBool("Active", false);

            isPlayingEffect = false;
        }

        private IEnumerator RoundEffectTournamentStart()
        {
            while(isPlayingEffect) yield return new WaitForSeconds(0.33f);
            isPlayingEffect = true;

            tournamentRank = -1;

            roundStartElement.gameObject.SetActive(false);
            roundInfoElement.gameObject.SetActive(true);
            roundResultElement.gameObject.SetActive(false);

            rootAnimator.SetBool("IsContextCover", false);
            prizePoolAnimator.SetBool("IsBreak", false);
            prizePoolAnimator.SetBool("IsResult", false);

            int round = tournamentBB.GetValue<int>("serialWinCount");
            int maxRankToAdvance = tournamentBB.GetValue<int>("maxRankToAdvance");
            MetaContextElementUtils.SimpleSetText(roundInfoElement, "Text Title", StringTableUtils.GetString(tableType, TOURNAMENT_START_TITLE, round + 1));
            MetaContextElementUtils.SimpleSetText(roundInfoElement, "Text Sub", StringTableUtils.GetString(tableType, TOURNAMENT_START_TEXT, maxRankToAdvance));

            // Active
            roundAnimator.SetBool("Active", true);

            yield return new WaitForSeconds(3f);
            // Deactive

            roundAnimator.SetBool("Active", false);

            isPlayingEffect = false;
        }

        private IEnumerator RoundEffectTournamentEnd()
        {
            while(isPlayingEffect) yield return new WaitForSeconds(0.33f);
            isPlayingEffect = true;

            roundStartElement.gameObject.SetActive(false);
            roundInfoElement.gameObject.SetActive(false);
            roundResultElement.gameObject.SetActive(true);

            int nextRound = tournamentBB.GetValue<int>("serialWinCount");
            double nextRoundMultiplier = tournamentBB.GetValue<double>("serialWinBonus");
            MetaContextElementUtils.SimpleSetText(roundInfoElement, "Text Title", StringTableUtils.GetString(tableType, TOURNAMENT_START_TITLE, nextRound + 1));

            // cover
            MetaContextElementUtils.SimpleSetText(animatorElement, "Text Title", StringTableUtils.GetString(tableType, TOURNAMENT_END_COVER_ROUND_TEXT, nextRound + 1));
            MetaContextElementUtils.SimpleSetText(animatorElement, "Text Context", StringTableUtils.GetString(tableType, TOURNAMENT_END_COVER_PRIZE_TEXT, nextRoundMultiplier));

            var prevTournamentResult = tournamentBB.GetValue<Blackboard>("prevTournamentResult");
            int rank = prevTournamentResult.GetValue<int>("rank");
            long actualWinCredit = prevTournamentResult.GetValue<long>("actualWinCredit");
            if(actualWinCredit > 0)
            {
                MetaContextElementUtils.SimpleSetText(roundResultElement, "Rank", StringTableUtils.GetString(tableType, TOURNAMENT_TROPHY_RANK_TEXT, trophyActiveFormat, rank));
                MetaContextElementUtils.SimpleSetText(roundResultElement, "Text Sub", StringTableUtils.GetString(tableType, TOURNAMENT_RESULT_WIN_DESC_TEXT));
                MetaContextElementUtils.SimpleSetText(roundResultElement, "Text Title", StringTableUtils.GetString(tableType, TOURNAMENT_RESULT_WIN_TEXT));
            }
            else
            {
                MetaContextElementUtils.SimpleSetText(roundResultElement, "Rank", StringTableUtils.GetString(tableType, TOURNAMENT_TROPHY_RANK_TEXT, trophyInactiveFormat, rank));
                MetaContextElementUtils.SimpleSetText(roundResultElement, "Text Sub", StringTableUtils.GetString(tableType, TOURNAMENT_RESULT_LOSE_DESC_TEXT, nextRound + 1));
                MetaContextElementUtils.SimpleSetText(roundResultElement, "Text Title", StringTableUtils.GetString(tableType, TOURNAMENT_RESULT_LOSE_TEXT));
            }

            // Active
            roundAnimator.SetBool("Active", true);

            yield return new WaitForSeconds(3f);

            // Deactive
            roundAnimator.SetBool("Active", false);

            if(actualWinCredit == 0)
            {
                MetaContextElementUtils.SetText(prizePoolResultLoseRankTextElement, StringTableUtils.GetString(tableType, TOURNAMENT_END_LOSE_RANK_TEXT, rank));
                MetaContextElementUtils.SetText(prizePoolResultLoseRoundTextElement, StringTableUtils.GetString(tableType, TOURNAMENT_END_LOSE_ROUND_TEXT, 1));
                prizePoolAnimator.SetBool("IsResult", true);

                yield return new WaitForSeconds(3f);
            }

            rootAnimator.SetBool("IsContextCover", true);

            prizePoolAnimator.SetBool("IsResult", false);
            prizePoolAnimator.SetBool("IsBreak", true);

            isPlayingEffect = false;
        }

        private void UpdateTimer(TournamentStatus status)
        {
            // Update Timer.
            if(status == TournamentStatus.BREAKING)
            {
                long startTimestamp = tournamentBB.GetValue<long>("endByTimestamp");
                long endTimestamp = tournamentBB.GetValue<long>("breakUntilTimestamp");
                timerGaugeController.StopTimer();
                timerGaugeController.StartTimer(startTimestamp, endTimestamp);

                timerController.StopTimer();
                timerController.StartTimer(endTimestamp, 0);
            }
            else
            {
                long startTimestamp = tournamentBB.GetValue<long>("startTimestamp");
                long endTimestamp = tournamentBB.GetValue<long>("endByTimestamp");
                timerGaugeController.StopTimer();
                timerGaugeController.StartTimer(startTimestamp, endTimestamp);

                timerController.StopTimer();
                timerController.StartTimer(endTimestamp, 0);
            }
        }

        private void UpdateUserCell()
        {
            if(isFold) return;

            TournamentStatus currentStatus = tournamentBB.GetValue<TournamentStatus>("status");
            List<Blackboard> myRankList = tournamentBB.GetValue<List<Blackboard>>("myRankList");

            if(currentStatus == TournamentStatus.PLAYING && myRankList.Count == 0) return;

            for(int i=0; i<3; ++i)
            {
                Blackboard rankInfo = null;

                if(currentStatus == TournamentStatus.WAITING)
                {
                    MetaContextElementUtils.SetBlackboardValue<bool>(userProfileList[i], "spinToRank", i == 2 ? true : false);
                }
                else
                {
                    if(myRankList.Count > i)
                        rankInfo = myRankList[i];

                    MetaContextElementUtils.SetBlackboardValue<bool>(userProfileList[i], "spinToRank", false);
                }

                MetaContextElementUtils.SetBlackboardValue<Blackboard>(userProfileList[i], "rankInfo", rankInfo);
                MetaContextElementUtils.SetBlackboardValue<int>(userProfileList[i], "cellType", 0);
                MetaContextElementUtils.SetBlackboardValue<bool>(userProfileList[i], "Update", true);

            }
            // Status PLAY == Up, Down Effect.
            // ./tournamentInfo/myRankList
        }

        private void OnTimerCallback()
        {
            if(tournamentBB == null) return;

            TournamentStatus currentStatus = tournamentBB.GetValue<TournamentStatus>("status");
            if(currentStatus == TournamentStatus.PLAYING || currentStatus == TournamentStatus.BREAKING)
            {
#if NEW_NET

            NetManager.Instance.Post(RPCName.metaInfo, new Dictionary<string,object>(),
                (res) =>
                {
                    string resStr = res.ToString();

                    /* var roomBB =  BlackboardUtils.FindVariable<Blackboard>(ContentBlackboard.Get(), "room");

                     if(roomBB != null && roomBB.value != null)
                     {
                         BlackboardQueryUtils.UpdateTournament(response.tournamentInfo);
                     }*/
                },
                (error) =>
                {
                }
            );
            return;
#endif


                // Request Meta
                var roomID = BlackboardUtils.FindVariable<string>(ContentBlackboard.Get(), "room/roomId");

                if(roomID != null || !string.IsNullOrEmpty(roomID.value))
                {
                    BagelCodeClientAPI.RoomMetaInfo(roomID.value,
                    (response) =>
                    {
                        var roomBB =  BlackboardUtils.FindVariable<Blackboard>(ContentBlackboard.Get(), "room");

                        if(roomBB != null && roomBB.value != null)
                        {
                            BlackboardQueryUtils.UpdateTournament(response.tournamentInfo);
                        }
                    },
                    (error) =>
                    {
                    });



                }
            }
        }

        private void OnSetFold(bool isFold)
        {
            if(rootAnimator == null) return;

            PlayerPrefs.SetInt(PLAYERPREFS_TOURNAMENT_FOLD_KEY, isFold ? 1 : 0);

            rootAnimator.SetBool("IsFold", isFold);

            UpdateUserCell();

            if(isFold)
                OnTournamentInfoClose();
        }

        private void OnTournamentInfoOpen()
        {
            moreInfoOpenButtonElement.gameObject.SetActive(false);
            moreInfoCloseButtonElement.gameObject.SetActive(true);
            moreInfoCoverElement.gameObject.SetActive(true);

            moreInfoController.OnSetActive(true);
        }


        private void OnTournamentInfoClose()
        {
            moreInfoOpenButtonElement.gameObject.SetActive(true);
            moreInfoCloseButtonElement.gameObject.SetActive(false);
            moreInfoCoverElement.gameObject.SetActive(false);

            moreInfoController.OnSetActive(false);
        }

        private void BI_client_tournament_start()
        {
            string id = tournamentBB.GetValue<string>("tournamentId");
            int round = tournamentBB.GetValue<int>("serialWinCount");
            double roundMultiplier = tournamentBB.GetValue<double>("serialWinBonus");
            round += 1;

            Analytics.CustomEvent("client_tournament", new Dictionary<string, object>
            {
                { "type", "play" },
                { "tournament_id", id },
                { "round", round },
                { "prize_multiplier", roundMultiplier}
            });
        }

        // private void BI_client_tournament_break(string prevTournamentID)
        // {
        //     var prevTournamentResult = tournamentBB.GetValue<Blackboard>("prevTournamentResult");
        //     int round = prevTournamentResult.GetValue<int>("tournamentSerialWinCount");
        //     int rank = prevTournamentResult.GetValue<int>("rank");
        //     long actualWinCredit = prevTournamentResult.GetValue<long>("actualWinCredit");
        //     round += 1;

        //     Analytics.CustomEvent("client_tournament", new Dictionary<string, object>
        //     {
        //         { "type", "break" },
        //         { "tournament_id", prevTournamentID },
        //         { "round", round },
        //         { "rank", rank },
        //         { "earn_coin", actualWinCredit }
        //     });
        // }

        private void BI_client_click_tournament_ui(string type)
        {
            Analytics.CustomEvent("client_click_tournament_ui", new Dictionary<string, object>
            {
                { "type", type }
            });
        }
    }
}
