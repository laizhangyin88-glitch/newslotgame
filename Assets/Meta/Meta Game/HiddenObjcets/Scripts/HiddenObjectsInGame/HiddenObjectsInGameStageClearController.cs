using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using System.Collections;
using BagelCode.ClientModels;
using UnityEngine.UI;

namespace BagelCode.HiddenObjects
{
    public class HiddenObjectsInGameStageClearController : EventMonoBehaviour
    {
        private const float SCORE_INCREASING_TIME = 0.25f;

        private const int STAR_COUNT = 5;
        private const int BONUS_COUNT = 4;

        private ContextElement root;
        private Blackboard bb;
        private Animator anim;

        private HiddenUniverseStageInfo currentProgress;

        private EventTrigger skipTrigger;

        private long currentScore = 0L;

        private long totalScore;
        private long[] bonusScores = new long[BONUS_COUNT];
        private long earnCredit;
        private long earnGem;
        private int needFinderCount;

        private HiddenUniverseLeaderboard leaderboard;
        private HiddenUniverseStageInfo prevStageInfo;
        private HiddenUniverseStageInfo stageInfo;

        private ContextElement totalScoreTextElement;
        private ContextElement[] starElements = new ContextElement[STAR_COUNT];
        private ContextElement[] starSliderElements = new ContextElement[STAR_COUNT];
        private ContextElement[] percentageBaseElements = new ContextElement[STAR_COUNT];

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnClickAny();
            }
        }

        private void OnDestroy()
        {
            MetaSystem.UnSubscribeBackButton(this.GetHashCode());
            BlurManager.SetBlur(false);
        }

        public void InitProperty()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            anim.SetBool("Active", true);

            totalScore = bb.GetValue<long>("totalScore");
#if DEV
            if (PlayerPrefs.GetInt(HiddenObjects.Defines.PLAYER_PREFS_TOTAL_SCORE_ZERO, 0) == 1)
            {
                totalScore = 0L;
            }
#endif

            bonusScores[0] = bb.GetValue<long>("baseScore");
            bonusScores[1] = bb.GetValue<long>("accuracyBonusScore");
            bonusScores[2] = bb.GetValue<long>("timeBonusScore");
            bonusScores[3] = bb.GetValue<long>("hintBonusScore");
            earnCredit = bb.GetValue<long>("earnCredit");
            earnGem = bb.GetValue<long>("earnGem");

            var leaderboardBB = bb.GetValue<Blackboard>("leaderboard");
            leaderboard = BlackboardQueryUtils.DeserializeHiddenUniverseLeaderboard(leaderboardBB);

            prevStageInfo = bb.GetValue<HiddenUniverseStageInfo>("prevStageInfo");

            var stageInfoBB = bb.GetValue<Blackboard>("stageInfo");
            stageInfo = BlackboardQueryUtils.DeserializeHiddenUniverseStageInfo(stageInfoBB);

            // Check Skip
            skipTrigger = new EventTrigger(gameObject, HiddenObjects.Events.ON_SKIP);

            // Back Button
            MetaContextElementUtils.SimpleSetClickable(root, "Button Close", gameObject,
                EventSender.ON_CUSTOM_EVENT, HiddenObjects.Events.ON_BACK_TO_MAIN);
            MetaSystem.SubscribeBackButton(this.GetHashCode(), () =>
            {
                EventSender.SendEvent(gameObject, HiddenObjects.Events.ON_BACK_TO_MAIN);
                OnClickAny();
            });

            // Enable Blur
            BlurManager.SetBlur(true);

            // Stage Cleared
            bool isPerfect = bb.GetValue<bool>("isPerfect");
            bool isTimeUp = bb.GetValue<bool>("isTimeUp");
            if (isPerfect)
            {
                GSManager.Instance.GetHandler(HiddenObjects.Defines.SOUNDS_STAGE_PERFECT_CLEAR).Play();

                MetaContextElementUtils.SimpleSetActive(root, "Stage Clear/Text Stage Clear", false, FULL);
                MetaContextElementUtils.SimpleSetActive(root, "Stage Clear/Perfect Game", true, FULL);
            }
            else if(isTimeUp)
            {
                GSManager.Instance.GetHandler(HiddenObjects.Defines.SOUNDS_STAGE_TIME_UP).Play();

                MetaContextElementUtils.SimpleSetTextGlobal(root, "Stage Clear/Text Stage Clear", "HIDDEN_OBJECTS_IN_GAME_CLEAR_TIME_UP", FULL);
            }
            else
            {
                GSManager.Instance.GetHandler(HiddenObjects.Defines.SOUNDS_STAGE_CLEAR).Play();

                MetaContextElementUtils.SimpleSetTextGlobal(root, "Stage Clear/Text Stage Clear", "HIDDEN_OBJECTS_IN_GAME_CLEAR_CLEARED", FULL);
            }

            // Continue
            MetaContextElementUtils.SimpleSetActive(root, "Bottom Text Area/Text", false, FULL);
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Bottom Text Area/Text", "HIDDEN_OBJECTS_IN_GAME_CLEAR_TAP", FULL);

            // Star
            var starScoreAreaElement = ContextUtils.FindElement(root, "Star Score Area", CHILDREN);
            for (int i = 0; i < STAR_COUNT; ++i)
            {
                starElements[i] = ContextUtils.FindElement(starScoreAreaElement, "Star " + (i + 1).ToString(), CHILDREN);
                starSliderElements[i] = ContextUtils.FindElement(starElements[i], "Base", CHILDREN);
                percentageBaseElements[i] = ContextUtils.FindElement(starElements[i], "Percentage Base", CHILDREN);
            }

            // Score
            totalScoreTextElement = ContextUtils.FindElement(root, "Total Score Area/Text Total Score", FULL);

            var bottomArea = ContextUtils.FindElement(root, "Bottom Area", CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(bottomArea, "Text Base Score Title", "HIDDEN_OBJECTS_IN_GAME_CLEAR_BASE_SCORE", CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(bottomArea, "Text Accuracy Bonus Title", "HIDDEN_OBJECTS_IN_GAME_CLEAR_ACCURACY_BONUS", CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(bottomArea, "Text Time Bonus Title", "HIDDEN_OBJECTS_IN_GAME_CLEAR_TIME_BONUS", CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(bottomArea, "Text Hint Bonus Title", "HIDDEN_OBJECTS_IN_GAME_CLEAR_HINT_BONUS", CHILDREN);

            MetaContextElementUtils.SimpleSetTextGlobal(bottomArea, "Text Base Score", "TEXT_COMMA_NUMBER", CHILDREN, bonusScores[0]);
            MetaContextElementUtils.SimpleSetTextGlobal(bottomArea, "Text Accuracy Bonus", "TEXT_COMMA_NUMBER", CHILDREN, bonusScores[1]);
            MetaContextElementUtils.SimpleSetTextGlobal(bottomArea, "Text Time Bonus", "TEXT_COMMA_NUMBER", CHILDREN, bonusScores[2]);
            MetaContextElementUtils.SimpleSetTextGlobal(bottomArea, "Text Hint Bonus", "TEXT_COMMA_NUMBER", CHILDREN, bonusScores[3]);

            UpdateStarProgress(0f);
            UpdateTotalScore(0L);

            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);
            Register(MetaEventDefine.ON_META_UI_EVENT, HiddenObjects.Events.ON_UPDATE_FINDER_COUNT,
                () => BlackboardUtils.SetOrCreateValue(bb, "finder", HiddenObjects.Utils.Finder));

            StartCoroutine(ShowSkipTextCoroutine(3.6f));
        }

        public void OnClose(bool playAgain)
        {
            if (playAgain)
            {
                StartCoroutine(PlayAgainCoroutine());
            }
            else
            {
                EventSender.SendCalleeCallback(gameObject, HiddenObjects.Events.ON_BACK_TO_MAIN);

                anim.SetTrigger("Close");
                PopupManager.Instance.Close(gameObject);
            }
        }

        private IEnumerator PlayAgainCoroutine()
        {
            // Play Finder Consume Anim
            anim.SetTrigger("isStart");

            // Disable Meta Interactables
            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.INACTIVE_META_UI);

            // Update Finder Gauge
            int finder = HiddenObjects.Utils.Finder;
            HiddenObjects.Utils.UpdateFinderCount(finder - needFinderCount);

            var onFinderConsumedTrigger = new EventTrigger(gameObject, HiddenObjects.Events.ON_FINDER_CONSUMED);
            yield return new WaitUntilTrigger(onFinderConsumedTrigger);

            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ACTIVE_META_UI);

            EventSender.SendCalleeCallback(gameObject, HiddenObjects.Events.ON_PLAY_AGAIN);

            anim.SetTrigger("Close");
            PopupManager.Instance.Close(gameObject);
        }

        public void SetTotalScoreContents()
        {
            // Effect
            MetaContextElementUtils.SimpleSetActive(root, "Background Effect Anchor", true);
        }

        public void PlayTotalScoreIncreaseAnim(int bonusIndex)
        {
            StartCoroutine(PlayScoreIncreasingAnimCoroutine(bonusIndex));
        }

        public void PlayTotalScoreStarAnim()
        {
            if (totalScore > 0L)
            {
                StartCoroutine(TotalScoreAnimCoroutine());
            }
            else
            {
                Skip();
            }
        }

        private IEnumerator TotalScoreAnimCoroutine()
        {
            // Star Movement
            yield return StartCoroutine(PlayStarMovementAnimCoroutine());

            // Delay
            yield return new WaitForSeconds(1.25f);

            // Star Progress
            yield return StartCoroutine(PlayStarProgressIncreaseAnimCoroutine());

            EventSender.SendEvent(gameObject, HiddenObjects.Events.ON_SKIP);
        }

        public void SetResultContents()
        {
            UpdateStarProgress(1f);
            UpdateTotalScore(totalScore);

            // Header
            bool isHighsScore = bb.GetValue<bool>("isHighScore");
            MetaContextElementUtils.SimpleSetActive(root, "Total Score Area/Text New Record", isHighsScore, FULL);
            if (isHighsScore)
            {
                MetaContextElementUtils.SimpleSetTextGlobal(root, "Total Score Area/Text New Record", "HIDDEN_OBJECTS_IN_GAME_CLEAR_NEW_RECORD", FULL);
            }

            // Results
            MetaContextElementUtils.SimpleSetActive(root, "Result Base", true);
            anim.SetTrigger("Buttons Appear");

            var leaderboardAreaElement = ContextUtils.FindElement(root, "Leaderboard Area", CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(leaderboardAreaElement, "Text Leaderboard",
                "HIDDEN_OBJECTS_IN_GAME_CLEAR_LEADERBOARD", CHILDREN);

            // Rank
            int myRank = leaderboard.nowMyRank.rank;
            int MAX_RANKING_COUNT = 7;
            for (int i = 0; i < MAX_RANKING_COUNT; ++i)
            {
                if(i == 0)
                {
                    InitRankCell(leaderboardAreaElement, myRank == 1, i, leaderboard.firstRank);
                }
                else
                {
                    if(leaderboard.shownRankList.IsValidIndex(i - 1))
                    {
                        var rank = leaderboard.shownRankList[i - 1];
                        InitRankCell(leaderboardAreaElement, rank.rank == myRank, i, rank);
                    }
                    else
                    {
                        InitRankCell(leaderboardAreaElement, false, i, null);
                    }
                }
            }

            // Rewards
            var resultBaseElement = ContextUtils.FindElement(root, "Result Base", CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(resultBaseElement, "Text Rewards",
                "HIDDEN_OBJECTS_IN_GAME_CLEAR_REWARDS", CHILDREN);

            MetaContextElementUtils.SimpleSetTextGlobal(resultBaseElement, "Text Coin Rewards",
                "TEXT_COMMA_NUMBER", CHILDREN, earnCredit);

            bool gemEarned = earnGem > 0;
            MetaContextElementUtils.SimpleSetActive(resultBaseElement, "Image Gem", gemEarned);
            MetaContextElementUtils.SimpleSetActive(resultBaseElement, "Text Gem Rewards", gemEarned);
            if (gemEarned)
            {
                MetaContextElementUtils.SimpleSetTextGlobal(resultBaseElement, "Text Gem Rewards",
                    "TEXT_COMMA_NUMBER", CHILDREN, earnGem);
            }

            // Back Button
            var backButtonElement = ContextUtils.FindElement(resultBaseElement, "Button Back", CHILDREN);
            MetaContextElementUtils.SetClickable(backButtonElement, gameObject, HiddenObjects.Events.ON_BACK_TO_MAIN, false);

            MetaContextElementUtils.SimpleSetTextGlobal(backButtonElement, "Text",
                "HIDDEN_OBJECTS_IN_GAME_CLEAR_BACK_BUTTON", CHILDREN);

            // Play Button
            var playButtonElement = ContextUtils.FindElement(resultBaseElement, "Button Play", CHILDREN);
            MetaContextElementUtils.SetClickable(playButtonElement, gameObject, HiddenObjects.Events.ON_PLAY_AGAIN, false);

            needFinderCount = bb.GetValue<int>("needFinderCount");
            MetaContextElementUtils.SimpleSetTextGlobal(playButtonElement, "Text",
                "HIDDEN_OBJECTS_IN_GAME_CLEAR_PLAY_BUTTON", CHILDREN, needFinderCount);

            MetaContextElementUtils.SimpleSetTextGlobal(resultBaseElement, "Text Finder Use",
                "HIDDEN_OBJECTS_MAIN_STAGE_CONSUME_FINDER_TEXT", CHILDREN, needFinderCount);
        }

        private IEnumerator ShowSkipTextCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (!skipTrigger.IsTrigger)
            {
                MetaContextElementUtils.SimpleSetActive(root, "Bottom Text Area/Text", true, FULL);
            }
        }

        private IEnumerator OpenProfilePopupCoroutine(string userId)
        {
            GameObject loadingObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenLoadingPopupCoroutine(
                (GameObject popupObj) => loadingObj = popupObj));

            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Profile Popup Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject profileObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(bundle, asset, parent,
                (SceneLoadOperation sceneLoadOperation) => profileObj = sceneLoadOperation.GetScene()));

            var profileBB = profileObj.GetComponent<Blackboard>();

            MetaObjectUtils.SetCalleeCaller(profileObj, gameObject);
            BlackboardUtils.SetOrCreateValue(profileBB, "bi_fromType", "hidden_universe_clear");
            BlackboardUtils.SetOrCreateValue(profileBB, "_userId", userId);
            BlackboardUtils.SetOrCreateValue(profileBB, "isInRoom", false);

            MetaPopupUtils.OpenPopup(profileObj);

            MetaPopupUtils.ClosePopup(loadingObj);

            var callbackTrigger = new EventTrigger(gameObject, EventSender.ON_CALLEE_CALLBACK);
            yield return new WaitUntilTrigger(callbackTrigger);
        }

        private void OnClickProfileButton(string userId)
        {
            StartCoroutine(OpenProfilePopupCoroutine(userId));
        }

        private void InitRankCell(ContextElement leaderboardAreaElement, bool isMe, int index, HiddenUniverseRank rankInfo)
        {
            var rankingCellElement = ContextUtils.FindElement(leaderboardAreaElement,
                "Rank Cell " + (index + 1).ToString(), CHILDREN);

            MetaContextElementUtils.SetActive(rankingCellElement, rankInfo != null);

            if (rankInfo != null)
            {
                // Profile Popup
                string userId = rankInfo.userId;
                if(!string.IsNullOrEmpty(userId))
                {
                    MetaContextElementUtils.SimpleSetClickable(rankingCellElement, "Profile Button",
                        () => OnClickProfileButton(userId));
                }

                // Rank
                MetaContextElementUtils.SimpleSetText(rankingCellElement, "Text Rank", rankInfo.rank.ToString());

                // User Name
                MetaContextElementUtils.SimpleSetText(rankingCellElement, "Text Username", rankInfo.userName);

                // Score
                MetaContextElementUtils.SimpleSetTextGlobal(rankingCellElement, "Text Userscore",
                    "TEXT_COMMA_NUMBER", CHILDREN, rankInfo.score);

                // Tier
                var profileElement = ContextUtils.FindElement(rankingCellElement, "Profile Picture Small", CHILDREN);
                MetaContextElementUtils.SetPropertySafty(profileElement, TierUtils.GetTierGroup(rankInfo.tier));

                // Profile
                var imageElement = ContextUtils.FindElement(profileElement, "Image", CHILDREN);
                MetaContextElementUtils.SetWebImage(imageElement, rankInfo.profileUrl, CacheType.FileCache, true, null);

                // Is Me
                MetaContextElementUtils.SimpleSetActive(rankingCellElement, "My Rank Base", isMe, CHILDREN);

                if (isMe)
                {
                    int prevRank = leaderboard.prevMyRank.rank;
                    int nowRank = leaderboard.nowMyRank.rank;
                    if ((prevRank > 0) && nowRank < prevRank) // Rank Up
                    {
                        var rankUpAreaElement = ContextUtils.FindElement(rankingCellElement, "Rank Up Anchor", CHILDREN);

                        string bundle = HiddenObjects.Defines.CONTENTS_BUNDLE;
                        string asset = "Leaderboard Rank Cell My Rank Up";
                        Transform parent = rankUpAreaElement.transform;
                        GameObject rankUpObj = MetaObjectUtils.MakePrefab(bundle, asset, parent);

                        if (rankUpObj != null)
                        {
                            var rankUpElement = rankUpObj.GetComponent<ContextElement>();
                            rankUpElement.UpdateContext(true);

                            int increasedRank = prevRank - nowRank;
                            MetaContextElementUtils.SimpleSetText(rankUpElement, "Rank Up Text", increasedRank.ToString());
                        }
                    }
                }
            }
        }

        //

        private IEnumerator PlayScoreIncreasingAnimCoroutine(int bonusIndex)
        {
            // Delay
            var timerTrigger = new TimerTrigger(0.8f);
            yield return new WaitUntilTrigger(timerTrigger, skipTrigger);

            // Play Sounds
            if (!skipTrigger.IsTrigger)
            {
                GSManager.Instance.GetHandler(HiddenObjects.Defines.SOUNDS_SCORE_ADDITION).Play();
            }

            // Delay 2
            var timerTrigger2 = new TimerTrigger(0.4f);
            yield return new WaitUntilTrigger(timerTrigger2, skipTrigger);

            long originScore = currentScore;
            long targetScore = currentScore;

            targetScore += bonusScores[bonusIndex];

            // Increasing Score
            float remaining = SCORE_INCREASING_TIME;
            while (remaining > 0f && !skipTrigger.IsTrigger)
            {
                float progress = remaining / SCORE_INCREASING_TIME;
                remaining -= Time.deltaTime;

                long t = (long)((targetScore - originScore) * (1f - progress));
                currentScore = originScore + t;

                UpdateTotalScore(currentScore);

                yield return new WaitForEndOfFrame();
            }

            // Skip
            if (skipTrigger.IsTrigger)
            {
                UpdateTotalScore(totalScore);
            }
            else
            {
                currentScore = targetScore;
                UpdateTotalScore(targetScore);
            }
        }

        private IEnumerator PlayStarProgressIncreaseAnimCoroutine()
        {
            if (prevStageInfo.completedStarCount == STAR_COUNT)
                yield break;

            const float DURATION = 1.5f;
            float remaining = DURATION;

            string bundle = HiddenObjects.Defines.CONTENTS_BUNDLE;
            string asset = "Hidden Objects Stage Clear Star Gage Effect";
            Transform parent = starSliderElements[0].transform;
            var gaugeEffectObj = MetaObjectUtils.MakePrefab(bundle, asset, parent);
            if (gaugeEffectObj != null)
            {
                var slider = starSliderElements[0].transform.GetComponent<Slider>();
                slider.handleRect = gaugeEffectObj.GetComponent<RectTransform>();
            }

            // Gauge Progress
            int ongoingStarIndex = UpdateStarProgress(0f, gaugeEffectObj);
            int prevOngoingStarIndex = ongoingStarIndex;
            while (remaining > 0f && !skipTrigger.IsTrigger)
            {
                remaining -= Time.deltaTime;

                float t = 1f - remaining / DURATION;
                ongoingStarIndex = UpdateStarProgress(t, gaugeEffectObj);

                if(ongoingStarIndex > prevOngoingStarIndex &&
                    starElements.IsValidIndex(prevOngoingStarIndex))
                {
                    // Play Star Complete Effect
                    if (earnGem > 0)
                    {
                        GSManager.Instance.GetHandler(HiddenObjects.Defines.SOUNDS_GEM_EARNED).Play();
                        MetaContextElementUtils.SimpleSetActive(starElements[prevOngoingStarIndex], "Gem Get Effect", true);
                    }

                    prevOngoingStarIndex = ongoingStarIndex;
                    MetaContextElementUtils.SimpleSetActive(starElements[prevOngoingStarIndex], "Full Star Effect", true);
                }

                yield return new WaitForEndOfFrame();
            }

            UpdateStarProgress(1f);
            Destroy(gaugeEffectObj);
        }

        private IEnumerator PlayStarMovementAnimCoroutine()
        {
            if (prevStageInfo.completedStarCount == STAR_COUNT)
                yield break;

            string bundle = HiddenObjects.Defines.CONTENTS_BUNDLE;
            string asset = "Hidden Objects Stage Clear Flying Star";
            Transform parent = totalScoreTextElement.transform;

            ContextElement targetStarElement = null;

            const float MOVEMENT_TIME = 0.3f;
            const int SHOOT_COUNT = 5;
            for (int i = 0; i < SHOOT_COUNT; ++i)
            {
                int completedStarCount = currentProgress.completedStarCount;
                if (completedStarCount < STAR_COUNT)
                {
                    targetStarElement = starSliderElements[currentProgress.completedStarCount];
                }

                var starObj = MetaObjectUtils.MakePrefab(bundle, asset, parent);
                var controller = starObj.GetComponent<DirectionalWeightPositionController>();
                controller.from = totalScoreTextElement.transform;
                controller.to = targetStarElement.transform;
                controller.width = 0.5f;

                // Movement
                var timerTrigger = new TimerTrigger(MOVEMENT_TIME);
                yield return new WaitUntilTrigger(timerTrigger, skipTrigger);
                if (skipTrigger.IsTrigger)
                {
                    Destroy(starObj);
                    yield break;
                }

                // Play Sounds
                GSManager.Instance.GetHandler(HiddenObjects.Defines.SOUNDS_STAR_ADDITION).Play();
            }
        }

        private void OnClickAny()
        {
            Skip();
        }

        private void Skip()
        {
            anim.SetTrigger("Skip");
            EventSender.SendEvent(gameObject, HiddenObjects.Events.ON_SKIP);
        }


        private HiddenUniverseStageInfo LerpStageProgress(HiddenUniverseStageInfo a, HiddenUniverseStageInfo b, float t)
        {
            float aProgress = GetStageProgress(a);
            float bProgress = GetStageProgress(b);

            float fResult = Mathf.Lerp(aProgress, bProgress, t);

            return new HiddenUniverseStageInfo()
            {
                completedStarCount = (int)(fResult * 100f * STAR_COUNT) / 100,
                ongoingStarPercentile = (int)(fResult * 100f * STAR_COUNT) % 100,
            };
        }

        private float GetStageProgress(HiddenUniverseStageInfo info)
        {
            return (info.completedStarCount + info.ongoingStarPercentile * 0.01f) / STAR_COUNT;
        }

        private int UpdateStarProgress(float t, GameObject gaugeEffectObj = null)
        {
            int ongoingStarIndex = STAR_COUNT - 1;
            currentProgress = LerpStageProgress(prevStageInfo, stageInfo, t);

            bool meetOngoingStar = false;
            for(int i = 0; i < STAR_COUNT; ++i)
            {
                if(i < currentProgress.completedStarCount)
                {
                    MetaContextElementUtils.SetSliderValue(starSliderElements[i], 1f);
                    MetaContextElementUtils.SetActive(percentageBaseElements[i], false);
                }
                else
                {
                    if (!meetOngoingStar)
                    {
                        meetOngoingStar = true;
                        ongoingStarIndex = i;

                        float progress = currentProgress.ongoingStarPercentile / 100f;
                        MetaContextElementUtils.SetSliderValue(starSliderElements[i], progress);

                        MetaContextElementUtils.SetActive(percentageBaseElements[i], true);
                        MetaContextElementUtils.SimpleSetTextGlobal(percentageBaseElements[i], "Text",
                            "HIDDEN_OBJECTS_IN_GAME_CLEAR_STAR_PROGRESS", CHILDREN, currentProgress.ongoingStarPercentile);

                        // Update Gauge
                        if (gaugeEffectObj != null)
                        {
                            if (gaugeEffectObj.transform.parent != starSliderElements[i].transform)
                            {
                                gaugeEffectObj.transform.SetParent(starSliderElements[i].transform);

                                ClearStarHandles();
                                var slider = starSliderElements[i].transform.GetComponent<Slider>();
                                slider.handleRect = gaugeEffectObj.GetComponent<RectTransform>();

                                var gaugeRect = gaugeEffectObj.GetComponent<RectTransform>();
                                gaugeRect.anchoredPosition = new Vector2();
                            }
                        }
                    }
                    else
                    {
                        MetaContextElementUtils.SetSliderValue(starSliderElements[i], 0f);
                        MetaContextElementUtils.SetActive(percentageBaseElements[i], false);
                    }
                }
            }

            return ongoingStarIndex;
        }

        private void ClearStarHandles()
        {
            for(int i = 0; i < STAR_COUNT; ++i)
            {
                var slider = starSliderElements[i].GetComponent<Slider>();
                slider.handleRect = null;
            }
        }

        private void UpdateTotalScore(long score)
        {
            MetaContextElementUtils.SetTextGlobal(totalScoreTextElement, "TEXT_COMMA_NUMBER", score);
        }
    }
}
