using System.Collections;
using UnityEngine;
using SlotMaker;
using ParadoxNotion;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using System.Collections.Generic;
using Sirenix.OdinInspector;

using static BagelCode.HiddenObjects.HiddenObjects.Defines;

namespace BagelCode.HiddenObjects
{
    public class HiddenObjectsMainController : EventMonoBehaviour
    {
        // todo shk
        // origin 오브젝트 validate 하는 ta 지원 기능 추가~~

        private ContextElement root;
        private Blackboard bb;
        private Animator anim;

        private bool isInit = false;

        private string contextId;

        private Blackboard hogInfo;

        private ContextElement[] stageCellElements = new ContextElement[STAGE_CELL_COUNT];
        private ContextElement[] chapterCellElements = new ContextElement[CHAPTER_CELL_COUNT];

        private ContextElement textInfoElement;
        private ContextElement textRewardElement;
        private ContextElement textCompleteElement;
        private ContextElement collectButtonElement;

        private ContextElement finderBonusAreaElement;
        private ContextElement finderBonusAreaAvtivedElement;
        private ContextElement finderBonusAreaInactivedElement;
        private ContextElement finderBonusCooltimeTimerElement;

        private ContextElement requestButtonElement;
        private ContextElement requestButtonActiveElement;
        private ContextElement requestButtonInactiveElement;
        private ContextElement requestButtonRemainingTimerElement;

        private int chapterPageIndex = 0;
        private Variable<int> targetChapterIndexVar; // for all chapter

        private int visibleChapterCount = 0;
        private int chapterCount = 0;
        private int chapterPageCount = 0;
        private int userLevel = 0;

        private bool isPlayChapterUnlockAnim = false;
        private bool isPlayStageUnlockAnim = false;

        // Shop
        private bool isFinderShopEnabled;
        private bool isBundleShopEnabled;
        private bool isBundleShopShowCoinFirst;

        // Ads
        private bool isAdsEnabled;
        private bool isInHouseAds;
        private string placementKey = "";
        private int finderBonusCount;

        [SerializeField]
        private bool[] chapterUnlocks;
        private HiddenUniverseChapterInfo[] chapterInfos;

        private StageState[,] chapterStageUnlocks; // [chapter, stage]
        private HiddenUniverseStageInfo[,] chapterStageInfoes;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private const StringTable.StringTableType GLOBAL = StringTable.StringTableType.Global;

        private enum StageState
        {
            NONE = 0,
            LOCK,
            UNLOCK,
            NEW_UNLOCK,
        }

        //

        private void OnDestroy()
        {
            MetaSystem.UnSubscribeBackButton(gameObject.GetHashCode());
        }

        public void InitProperty()
        {
            if (isInit) return;

            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            contextId = BiEventUtils.GenerateContextID();

            // Disable Finder Gift Kudo
            KudoEventManager.Instance.InactiveTargetKudo(PollType.HIDDEN_UNIVERSE_FINDER_GIFT);

            // Play Anim
            anim.SetBool("Active", true);
            anim.SetTrigger("Change");

            hogInfo = HiddenObjects.Utils.HiddenObjectsInfo;
            targetChapterIndexVar = bb.GetVariable<int>("targetChapterIndex");
            userLevel = BlackboardQueryUtils.GetMyLevel();

            // Init Elements, Chapter/Stage Datas
            InitContents();
            InitChapterData(out bool unlockNewChapter);
            InitStageData(unlockNewChapter);

            // Events
            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);
            RegisterHandleEventType(MetaEventDefine.ON_CONTENT_EVENT);
            Register(HiddenObjects.Events.ON_END_FINDER_REQUEST_COOLTIME, UpdateFinderRequestButton);
            Register(HiddenObjects.Events.ON_END_FINDER_BONUS_COOLTIME, UpdateFinderBonusButton);
            Register(HiddenObjects.Events.ON_CLICK_CHAPTER, OnSelectChapter);
            Register(MetaEventDefine.ON_META_UI_EVENT, HiddenObjects.Events.ON_UPDATE_FINDER_COUNT, OnUpdateFinderCount);

            Register(MetaEventDefine.ON_META_UI_EVENT, HiddenObjects.Events.ON_START_STAGE_UNLOCK_ANIM,
                () => isPlayStageUnlockAnim = true);
            Register(MetaEventDefine.ON_META_UI_EVENT, HiddenObjects.Events.ON_FINISH_STAGE_UNLOCK_ANIM,
                () => isPlayStageUnlockAnim = false);
            Register(MetaEventDefine.ON_META_UI_EVENT, HiddenObjects.Events.ON_START_CHAPTER_UNLOCK_ANIM,
                () => isPlayChapterUnlockAnim = true);
            Register(MetaEventDefine.ON_META_UI_EVENT, HiddenObjects.Events.ON_FINISH_CHAPTER_UNLOCK_ANIM,
                () => isPlayChapterUnlockAnim = false);

            // Update All
            InitChapterCells();
            UpdateChapterCells(false, true, unlockNewChapter);

            InitStageCells();
            UpdateStageCells(true);

            OnUpdateFinderCount();
            UpdateFinderBonusButton();

            UpdateChapterReward();

            isInit = true;
        }

        private void InitContents()
        {
            // Information
            var informationButtonElement = ContextUtils.FindElement(root, "Button Question", CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(informationButtonElement, "Text", "BUTTON_COMMON_INFORMATION", CHILDREN);
            MetaContextElementUtils.SetClickable(informationButtonElement,
                () => EventSender.SendEvent(gameObject, HiddenObjects.Events.ON_CLICK_INFORMATION));

            // Request Finder
            requestButtonElement = ContextUtils.FindElement(root, "Button Request", CHILDREN);
            requestButtonActiveElement = ContextUtils.FindElement(requestButtonElement, "Active", CHILDREN);
            requestButtonInactiveElement = ContextUtils.FindElement(requestButtonElement, "Inactive", CHILDREN);
            requestButtonRemainingTimerElement = ContextUtils.FindElement(requestButtonInactiveElement, "Remaining Timer", CHILDREN);

            MetaContextElementUtils.SetClickable(requestButtonElement, gameObject,
                HiddenObjects.Events.ON_CLICK_FINDER_REQUEST, false);
            MetaContextElementUtils.SimpleSetTextGlobal(requestButtonActiveElement, "Text",
                "HIDDEN_OBJECTS_MAIN_SCENE_REQUEST_TEXT", CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(requestButtonInactiveElement, "Text",
                "HIDDEN_OBJECTS_FINDER_REQUEST_COOLTIME_TEXT", CHILDREN);

            // Bonus (Ads)
            finderBonusAreaElement = ContextUtils.FindElement(root, "Finder Bonus Area", CHILDREN);
            finderBonusAreaAvtivedElement = ContextUtils.FindElement(finderBonusAreaElement, "Bonus On Area", CHILDREN);
            finderBonusAreaInactivedElement = ContextUtils.FindElement(finderBonusAreaElement, "Bonus Off Area", CHILDREN);
            finderBonusCooltimeTimerElement = ContextUtils.FindElement(finderBonusAreaInactivedElement, "Button/Event Timer Area/Remaining Timer", FULL);

            MetaContextElementUtils.SimpleSetClickable(finderBonusAreaAvtivedElement, "Button",
                gameObject, EventSender.ON_CUSTOM_EVENT, HiddenObjects.Events.ON_CLICK_FINDER_BONUS);

            finderBonusCount = hogInfo.GetValue<int>("videoAdsFinderCount");
            MetaContextElementUtils.SimpleSetTextGlobal(finderBonusAreaAvtivedElement,
                "Button/Desc Base/Text", "HIDDEN_OBJECTS_MAIN_SCENE_FINDER_BONUS_TEXT", FULL, finderBonusCount);

            // Close
            var closeButtonElement = ContextUtils.FindElement(root, "Close", CHILDREN);
            MetaContextElementUtils.SetClickable(closeButtonElement,
                () => EventSender.SendEvent(gameObject, HiddenObjects.Events.ON_CLOSE));

            // Back Button
            MetaSystem.SubscribeBackButton(gameObject.GetHashCode(),
                () => EventSender.SendEvent(gameObject, HiddenObjects.Events.ON_BACK_BUTTON));

            var finderShopButtonElement = ContextUtils.FindElement(root, "Finder Buy", CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(finderShopButtonElement, "Text", "HIDDEN_OBJECTS_MAIN_SCENE_FINDER_SHOP_BUTTON", CHILDREN);
            // ㄴHide the shop button when product not existing
            isFinderShopEnabled = BlackboardQueryUtils.IsFinderShopEnabled();
            isBundleShopEnabled = BlackboardQueryUtils.IsFinderBundleShopEnabled(out bool isShowCoinFirst);
            isBundleShopShowCoinFirst = isShowCoinFirst;
            bool isShopEnabled = isFinderShopEnabled || isBundleShopEnabled;
            BlackboardUtils.SetOrCreateValue(bb, "isFinderShopEnabled", isShopEnabled);
            if (isShopEnabled)
            {
                MetaContextElementUtils.SetClickable(finderShopButtonElement, () => EventSender.SendEvent(
                    gameObject, HiddenObjects.Events.ON_CLICK_FINDER_SHOP));
            }
            MetaContextElementUtils.SetActive(finderShopButtonElement, isShopEnabled);
        }

        private void InitChapterData(out bool unlockNewChapter)
        {
            unlockNewChapter = false;

            // Chapter Element
            var chapterAreaElement = ContextUtils.FindElement(root, "Chapter Area", CHILDREN);
            for (int i = 0; i < CHAPTER_CELL_COUNT; ++i)
            {
                string chapterCellName = string.Format("Chapter {0:00}", i + 1);
                chapterCellElements[i] = ContextUtils.FindElement(chapterAreaElement, chapterCellName, CHILDREN);
            }

            MetaContextElementUtils.SimpleSetClickable(root, "Chapter Area/Button Next",
                () => MoveChapterPage(true), false, FULL);
            MetaContextElementUtils.SimpleSetClickable(root, "Chapter Area/Button Previous",
                () => MoveChapterPage(false), false, FULL);

            // Reward Element
            textInfoElement = ContextUtils.FindElement(root, "Text Information", CHILDREN);
            textCompleteElement = ContextUtils.FindElement(root, "Text Complete", CHILDREN);
            textRewardElement = ContextUtils.FindElement(root, "Text Reward", CHILDREN);
            collectButtonElement = ContextUtils.FindElement(root, "Collect Button Area/Button Collect", FULL);

            MetaContextElementUtils.SetClickable(collectButtonElement, gameObject, HiddenObjects.Events.ON_COLLECT_CHAPTER_REWARD, false);

            // Info BB
            var chapterInfoBBLIst = hogInfo.GetValue<List<Blackboard>>("chapterInfoList");
            var chapterUnlockBBList = hogInfo.GetValue<List<Blackboard>>("chapterUnlockList");
            var hiddenUniverseSymbolList = hogInfo.GetValue<List<string>>("hiddenUniverseSymbolList");

            chapterCount = hiddenUniverseSymbolList.Count;
            visibleChapterCount = 0;

            chapterInfos = new HiddenUniverseChapterInfo[chapterCount];

            for (int i = 0; i < chapterCount; ++i)
            {
                var chapterInfo = BlackboardQueryUtils.DeserializeHiddenUniverseChapterRewardInfo(chapterInfoBBLIst[i]);
                chapterInfos[i] = chapterInfo;

                if (HiddenObjects.Utils.IsValidChapterIndex(i + 1, out _))
                {
                    ++visibleChapterCount;
                }
                else
                {
                    Debug.LogWarning("Chapter information parsing error. Check the <HiddenObjectsChapterData>'s chapterSymbolList.");
                    break;
                }
            }

            // No Chapter
            if (visibleChapterCount == 0)
            {
                Debug.LogError("HiddenObjectsMainController initializing failure. chapterCount is zero.");
                EventSender.SendEvent(gameObject, "OnSystemEvent", "SystemReset");
                return;
            }

            chapterPageCount = (visibleChapterCount - 1) / CHAPTER_CELL_COUNT + 1;

            // Chapter Unlocks
            chapterUnlocks = new bool[chapterCount];
            for (int i = 0; i < chapterCount; ++i)
            {
                var chapterUnlockBB = chapterUnlockBBList[i];
                bool isUnlock = chapterUnlockBB.GetValue<bool>("unlock");
                chapterUnlocks[i] = isUnlock;

                if (isUnlock)
                {
                    chapterPageIndex = i / CHAPTER_CELL_COUNT;
                    targetChapterIndexVar.value = i; // select last
                }
            }

            // Check unlock by last chapter
            int lastUnlockedChapter = PlayerPrefs.GetInt(PLAYER_PREFS_LAST_UNLOCKED_CHAPTER_INDEX, 0);
            int currentUnlockedLastChapter = targetChapterIndexVar.value;
            if (currentUnlockedLastChapter > lastUnlockedChapter)
            {
                unlockNewChapter = true;
                PlayerPrefs.SetInt(PLAYER_PREFS_LAST_UNLOCKED_CHAPTER_INDEX, currentUnlockedLastChapter);
            }
        }

        private void InitStageData(bool unlockNewChapter)
        {
            // Elements
            var stageAreaElement = ContextUtils.FindElement(root, "Stage Area", CHILDREN);
            for (int i = 0; i < STAGE_CELL_COUNT; ++i)
            {
                string stageCellName = string.Format("Stage {0:00}", i + 1);
                stageCellElements[i] = ContextUtils.FindElement(stageAreaElement, stageCellName, CHILDREN);
            }

            // Set Stage Infos
            chapterStageUnlocks = new StageState[chapterCount, STAGE_CELL_COUNT];
            chapterStageInfoes = new HiddenUniverseStageInfo[chapterCount, STAGE_CELL_COUNT];
            var chapterStageUnlockBBList = hogInfo.GetValue<List<Blackboard>>("stageUnlockList");
            var chapterStageInfoBBList = hogInfo.GetValue<List<Blackboard>>("stageInfoList");
            for (int i = 0; i < chapterCount; ++i)
            {
                int chapter = i + 1;
                var stageUnlocks = chapterStageUnlockBBList.FindAll(_bb => _bb.GetValue<int>("chapter") == chapter).ToArray();
                var stageInfoes = chapterStageInfoBBList.FindAll(_bb => _bb.GetValue<int>("chapter") == chapter).ToArray();
                for (int j = 0; j < STAGE_CELL_COUNT; ++j)
                {
                    // Stage Count가 5 미만인 경우 NONE
                    bool isEnableStage = stageUnlocks.IsValidIndex(j);
                    if (isEnableStage)
                    {
                        bool isUnlock = stageUnlocks[j].GetValue<bool>("unlock");
                        chapterStageUnlocks[i, j] = isUnlock ? StageState.UNLOCK : StageState.LOCK;
                    }
                    else
                    {
                        chapterStageUnlocks[i, j] = StageState.NONE; // NONE 일 때 처리는 현재 미구현
                    }

                    // Stage Info
                    chapterStageInfoes[i, j] = BlackboardQueryUtils.DeserializeHiddenUniverseStageInfo(stageInfoes[j]);
                }
            }

            // Unlock Last Stage
            bool isBreak = false;
            if (unlockNewChapter)
            {
                for (int i = visibleChapterCount - 1; i >= 0; --i)
                {
                    for (int j = STAGE_CELL_COUNT - 1; j >= 0; --j)
                    {
                        // Search Last Unlock Stage
                        if (chapterStageUnlocks[i, j] == StageState.UNLOCK)
                        {
                            chapterStageUnlocks[i, j] = StageState.NEW_UNLOCK;
                            isBreak = true;
                            break;
                        }
                    }
                    if (isBreak) break;
                }
            }
        }

        public IEnumerator IntroCoroutine()
        {
            InitProperty();

            StartBGM();

            // Send BI
            bool isInGame = BlackboardQueryUtils.IsIngame();
            string metaGroupContextId = BlackboardUtils.GetOrCreateVariable<string>(bb, "metaGroupContextId")?.value;
            BiEventUtils.SendBiEventEnter(isInGame ? "in_game" : "lobby_event_button", "hog", metaGroupContextId);

            // Wait FSM
            yield return new WaitForEndOfFrame();

            // Cut Scene
            var eventData = new EventData<bool>(HiddenObjects.Events.CHECK_CUT_SCENE, false);
            EventSender.SendEvent(gameObject, eventData);
            var onFinishCutSceneTrigger = new EventTrigger(gameObject, MetaEventDefine.ON_META_UI_EVENT, HiddenObjects.Events.ON_FINISH_CUT_SCENE);
            yield return new WaitUntilTrigger(onFinishCutSceneTrigger);

            // First Enter?
            bool isFirstEnter = PlayerPrefs.GetInt(PLAYER_PREFS_IS_FIRST_ENTER, 1) == 1;
            if (isFirstEnter)
            {
                PlayerPrefs.SetInt(PLAYER_PREFS_IS_FIRST_ENTER, 0);
                yield return StartCoroutine(OpenInformationPopupCoroutine(true));
            }

            // Gifted Finder
            int giftedFinder = hogInfo.GetValue<int>("giftedFinder");
            if (giftedFinder > 0)
            {
                yield return StartCoroutine(OpenFinderReceivedPopupCoroutine());
            }
        }

        public void CheckCutScene()
        {
            int chapter = targetChapterIndexVar.value + 1;
            int stage = bb.GetValue<int>("targetStageIndex") + 1;

            bool shown;

            bool fromInGame = bb.GetValue<bool>("fromInGame");
            if (fromInGame)
            {
                var stageInfo = chapterStageInfoes[chapter - 1, stage - 1];
                bool isCleared = stageInfo.completedStarCount != 0 || stageInfo.ongoingStarPercentile != 0;
                shown = isCleared;
            }
            else
            {
                string symbol = HiddenObjects.Utils.ChapterNumberToSymbol(chapter);
                string playerPrefsKey = string.Format(
                    PLAYER_PREFS_SHOWN_CUT_SCENE_FORMAT, symbol);

                shown = PlayerPrefs.GetInt(playerPrefsKey, 0) == 1;
                PlayerPrefs.SetInt(playerPrefsKey, 1);
            }

            EventSender.SendEvent(gameObject, shown ?
                HiddenObjects.Events.CANCEL_CUT_SCENE : // Cancel
                HiddenObjects.Events.PLAY_CUT_SCENE); // Play
        }

        public IEnumerator CollectChapterRewardCoroutine()
        {
            // Request
            long earnCredit = 0L;
            long earnGem = 0L;

            bool isSuccess = false;
            bool isFail = false;
            int chapter = targetChapterIndexVar.value + 1;
            BagelCodeClientAPI.RequestHiddenObjectsCollectChapterReward(
                chapter,
                (response) =>
                {
                    isSuccess = true;

                    earnCredit = response.earnCredit;
                    earnGem = response.earnGem;

                    // AE
                    Dictionary<string, object> customData = new Dictionary<string, object>();
                    customData["chapter"] = chapter;
                    customData["chapter_star_count"] = GetChapterTotalStarCount();
                    customData["earn_coin"] = earnCredit;
                    customData["earn_free_gem"] = earnGem;
                    Analytics.CustomEvent("client_hog_chapter_reward_collect", customData);

                    BlackboardQueryUtils.AddCoins(earnCredit);
                    BlackboardQueryUtils.AddGems(earnGem);
                },
                (error) =>
                {
                    isFail = true;

                    Debug.LogError(error.errorCode);
                });

            MetaSystem.BackupUserSyncInfo();

            yield return new WaitUntil(() => isSuccess || isFail);

            if (earnCredit == 0L && earnGem == 0L) yield break;

            // Make Popup
            string bundle = CONTENTS_BUNDLE;
            string asset = "Popup Hidden Objects Chapter Reward Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject rewardPopupObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(bundle, asset, parent,
                  (GameObject popupObj) => rewardPopupObj = popupObj));

            GSManager.Instance.GetHandler("UI_Purchase_Complete_Appear").Play();

            var popupBB = rewardPopupObj.GetComponent<Blackboard>();

            BlackboardUtils.SetOrCreateValue(popupBB, "starCount", GetChapterTotalStarCount());
            BlackboardUtils.SetOrCreateValue(popupBB, "chapterNumber", targetChapterIndexVar.value + 1);
            BlackboardUtils.SetOrCreateValue(popupBB, "coin", earnCredit);
            BlackboardUtils.SetOrCreateValue(popupBB, "gem", earnGem);

            MetaObjectUtils.SetCalleeCaller(rewardPopupObj, gameObject);

            MetaPopupUtils.OpenPopup(rewardPopupObj);

            var callbackTrigger = new EventTrigger(gameObject, EventSender.ON_CALLEE_CALLBACK);
            yield return new WaitUntilTrigger(callbackTrigger);

            GSManager.Instance.GetHandler("UI_Coin_Add").Play();

            chapterInfos[targetChapterIndexVar.value].isRewarded = true;

            UpdateChapterReward();
        }

        public IEnumerator PlayCutSceneCoroutine()
        {
            bool fromInGame = bb.GetValue<bool>("fromInGame");

            int chapter = targetChapterIndexVar.value + 1;
            int stage = fromInGame ? bb.GetValue<int>("targetStageIndex") + 1 : 0;
            if (HiddenObjects.Utils.GetCutSceneData(chapter, stage, out List<CutSceneData> sceneDataList))
            {
                // Make Cut Scene Popup
                string bundle = CONTENTS_BUNDLE;
                string asset = "Popup Hidden Objects Story Scene";
                Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

                GameObject cutSceneObj = null;
                yield return StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(bundle, asset, parent,
                    (SceneLoadOperation sceneLoadOperation) => cutSceneObj = sceneLoadOperation.GetScene()));

                // Set Cut Scene BB
                var cutSceneBB = cutSceneObj.GetComponent<Blackboard>();
                BlackboardUtils.SetOrCreateValue(cutSceneBB, "fromInGame", fromInGame);
                BlackboardUtils.SetOrCreateValue(cutSceneBB, "sceneDataList", sceneDataList);

                MetaObjectUtils.SetCalleeCaller(cutSceneObj, gameObject);

                MetaPopupUtils.OpenPopup(cutSceneObj);

                var callbackTrigger = new EventTrigger(gameObject, EventSender.ON_CALLEE_CALLBACK);
                yield return new WaitUntilTrigger(callbackTrigger);
            }
        }

        public IEnumerator RequestFinderBonusCoroutine()
        {
            if (!isAdsEnabled) yield break;

            // send bi todo shk

            if (isInHouseAds)
            {
                yield return StartCoroutine(ShowInHouseAdsCoroutine());
            }
            else
            {
                yield return StartCoroutine(ShowVideoAdsCoroutine());
            }

            UpdateFinderBonusButton();
        }

        public IEnumerator OnFinderRequestCoroutine()
        {
            bool isClubber = ClubUtils.IsClubber();

            string resultMessageKey = "";
            if (!isClubber)
            {
                resultMessageKey = "HIDDEN_OBJECTS_POPUP_NEED_JOIN_CLUB_MESSAGE";
            }
            else
            {
                var onSuccessTrigger = new EventTrigger(gameObject, "OnSuccess");
                var onFailTrigger = new EventTrigger(gameObject, "OnFail");
                yield return StartCoroutine(RequestFinderRequestCoroutine());

                if (onSuccessTrigger.IsTrigger)
                {
                    resultMessageKey = "HIDDEN_OBJECTS_POPUP_REQUEST_POSTED_MESSAGE";
                }
                else if (onFailTrigger.IsTrigger)
                {
                    resultMessageKey = "HIDDEN_OBJECTS_POPUP_TRY_AGAIN_MESSAGE";
                }
            }

            if (!string.IsNullOrEmpty(resultMessageKey))
                yield return StartCoroutine(OpenCommonMessagePopupCoroutine(resultMessageKey));
        }

        public IEnumerator LoadInGameCoroutine()
        {
            int chapter = targetChapterIndexVar.value;
            int stage = bb.GetValue<int>("targetStageIndex");

            if (ApplicationSettings.LogTest())
                Debug.Log(string.Format("LoadInGameCoroutine. chapter:{0}, stage:{1}", chapter, stage));

            StopBGM();

            // Load InGame
            yield return StartCoroutine(MainToInGameTransitionCoroutine(false));
        }

        public IEnumerator InGameCoroutine()
        {
            var onFinishStageTrigger = new EventTrigger(gameObject, HiddenObjects.Events.ON_FINISH_STAGE);
            while (true)
            {
                onFinishStageTrigger.Reset();
                yield return new WaitUntilTrigger(onFinishStageTrigger);

                var clearStageInfo = BlackboardUtils.GetOrCreateVariable<Blackboard>(bb, "clearStageInfo")?.value;
                if (clearStageInfo == null) // Quit
                {
                    break;
                }
                else // Clear
                {
                    bool playAgain = clearStageInfo.GetValue<bool>("playAgain");
                    if (playAgain) // Play Again
                    {
                        // Backup Clear Info
                        BackupClearInfo();

                        yield return StartCoroutine(MainToInGameTransitionCoroutine(true));
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            // Load Main
            yield return StartCoroutine(InGameToMainTransitionCoroutine());

            OnClearStage();

            StartBGM();
        }

        public IEnumerator OnBackButtonCoroutine()
        {
            var okButtonTrigger = new EventTrigger(gameObject, HiddenObjects.Events.ON_OK_BUTTON);

            // OK Popup
            yield return StartCoroutine(OpenCommonMessagePopupCoroutine("HIDDEN_OBJECTS_POPUP_LEAVE", true));

            EventSender.SendEvent(gameObject,
                okButtonTrigger.IsTrigger ?
                HiddenObjects.Events.ON_CLOSE :
                HiddenObjects.Events.ON_RETURN);

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame(); // wait for exit action in fsm
        }

        //

        public IEnumerator OpenFinderReceivedPopupCoroutine()
        {
            int earnFinder = hogInfo.GetValue<int>("giftedFinder");

            // Reward Popup
            string bundle = COMMON_BUNDLE;
            string asset = "Popup Hidden Objects Finder Received Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject popupObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(bundle, asset, parent,
                (SceneLoadOperation sceneLoadOperation) => popupObj = sceneLoadOperation.GetScene()));

            var popupBB = popupObj.GetComponent<Blackboard>();
            BlackboardUtils.SetOrCreateValue(popupBB, "earnFinder", earnFinder);
            BlackboardUtils.SetOrCreateValue(popupBB, "isPurchaseResult", false);
            MetaObjectUtils.SetCalleeCaller(popupObj, gameObject);

            MetaPopupUtils.OpenPopup(popupObj);

            var callbackTrigger = new EventTrigger(gameObject, EventSender.ON_CALLEE_CALLBACK);
            yield return new WaitUntilTrigger(callbackTrigger);
        }

        public IEnumerator OpenFinderShopPopupCoroutine()
        {
            bool isPurchased = false;

            if (isFinderShopEnabled || isBundleShopEnabled)
            {
                bool triggered = IAMRouter.Instance.TriggerIAM(InAppMessageTriggerType.ENTER_HIDDEN_UNIVERSE_SHOP, gameObject, contextId);
                if (triggered)
                {
                    var callbackTrigger = new EventTrigger(gameObject, IAMUtils.ON_IAM_CALLBACK_EVENT);
                    yield return new WaitUntilTrigger(callbackTrigger);
                }
            }

            if (isFinderShopEnabled)
            {
                string bundle = CONTENTS_BUNDLE;
                string asset = "Popup Hidden Objects Shop Scene";
                Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

                GameObject shopObj = null;
                yield return StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(bundle, asset, parent,
                    (SceneLoadOperation sceneLoadOperation) => shopObj = sceneLoadOperation.GetScene()));

                MetaPopupUtils.OpenPopup(shopObj);

                var purchaseTrigger = new EventTrigger(gameObject, HiddenObjects.Events.ON_SUCCESS_PURCHASE);
                var cancelTrigger = new EventTrigger(gameObject, HiddenObjects.Events.ON_CANCEL_PURCHASE);
                yield return new WaitUntilTrigger(purchaseTrigger, cancelTrigger);

                isPurchased = purchaseTrigger.IsTrigger;
            }

            if (!isPurchased && isBundleShopEnabled)
            {
                GameObject loadingObj = null;
                yield return StartCoroutine(MetaPopupUtils.OpenLoadingPopupCoroutine(
                    (GameObject popupObj) => loadingObj = popupObj));

                string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
                string asset = "Shop Scene";
                Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

                GameObject shopObj = null;
                yield return StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(bundle, asset, parent,
                    (SceneLoadOperation sceneLoadOperation) => shopObj = sceneLoadOperation.GetScene()));

                var shopBB = shopObj.GetComponent<Blackboard>();
                BlackboardUtils.SetOrCreateValue(shopBB, "coinShopType", ShopType.COIN_WITH_HIDDEN_UNIVERSE);
                BlackboardUtils.SetOrCreateValue(shopBB, "gemShopType", ShopType.GEM_WITH_HIDDEN_UNIVERSE);
                BlackboardUtils.SetOrCreateValue(shopBB, "_openTabIndex", isBundleShopShowCoinFirst ? 0 : 1);

                MetaPopupUtils.OpenPopup(shopObj);

                MetaObjectUtils.SetCalleeCaller(shopObj, gameObject);

                MetaPopupUtils.ClosePopup(loadingObj);

                var callbackTrigger = new EventTrigger(gameObject, EventSender.ON_CALLEE_CALLBACK);
                yield return new WaitUntilTrigger(callbackTrigger);

                EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, HiddenObjects.Events.ON_UPDATE_FINDER_COUNT);
            }
        }

        public IEnumerator OpenInformationPopupCoroutine(bool isAuto = false)
        {
            // Send BI
            Analytics.CustomEvent("client_hog_info_popup", new Dictionary<string, object>
            {
                { "type", isAuto ? "automatic" : "clicked" },
            });

            string bundle = CONTENTS_BUNDLE;
            int INFO_PAGE_COUNT = 5;

            yield return StartCoroutine(MetaPopupUtils.OpenCommonInformationPopupCoroutine(
                this,
                INFO_PAGE_COUNT,
                "Information Page {0:00}",
                "POPUP_HIDDEN_OBJECTS_INFORMATION_TEXT_{0}",
                bundle));

            var callbackTrigger = new EventTrigger(gameObject, EventSender.ON_CALLEE_CALLBACK);
            yield return new WaitUntilTrigger(callbackTrigger);
        }

        public IEnumerator OpenCommonMessagePopupCoroutine(string messageKey, bool isBack = false)
        {
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject messageObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenOKPopupCoroutine(parent,
                (GameObject popupObj) => messageObj = popupObj));

            string message = StringTableUtils.GetString(GLOBAL, messageKey);
            string ok = StringTableUtils.GetString(GLOBAL, "BUTTON_OKAY");

            string okEvent = isBack ? HiddenObjects.Events.ON_OK_BUTTON : "";

            MetaPopupUtils.SetCommonPopupData(messageObj, transform, message, "", okEvent, ok, "", "", "", "",
                true, false, true, true, true);

            MetaObjectUtils.SetCalleeCaller(messageObj, gameObject);

            MetaPopupUtils.OpenPopup(messageObj);

            var callbackTrigger = new EventTrigger(gameObject, EventSender.ON_CALLEE_CALLBACK);
            yield return new WaitUntilTrigger(callbackTrigger);
        }

        public IEnumerator CloseCoroutine()
        {
            // Make Loading Scene
            string bundle = COMMON_BUNDLE;
            string asset = "Hidden Objects Loading Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject loadingObj = null;
            yield return StartCoroutine(MetaObjectUtils.MakeSceneCoroutine(
                bundle, asset, parent, (GameObject sceneObj) => loadingObj = sceneObj));

            MetaPopupUtils.OpenPopup(loadingObj);

            Blackboard loadingBB = loadingObj.GetComponent<Blackboard>();
            BlackboardUtils.SetOrCreateValue(loadingBB, "isEnter", false);
            BlackboardUtils.SetOrCreateValue(loadingBB, "isMetaInGame", false);
        }

        public void Close(bool instantly)
        {
            // Enable Finder Gift Kudo
            KudoEventManager.Instance.ActiveTargetKudo(PollType.HIDDEN_UNIVERSE_FINDER_GIFT);

            if (instantly)
            {
                Destroy(gameObject);
            }
            else
            {
                anim.SetTrigger("Close");
            }
        }

        //

        private void StartBGM()
        {
            // Play BGM
            GSManager.Instance.GetHandler(SOUNDS_BACKGROUND).Play();
        }

        private void StopBGM()
        {
            // Stop BGM
            GSManager.Instance.GetHandler(SOUNDS_BACKGROUND).Stop();
        }

        private IEnumerator ShowInHouseAdsCoroutine()
        {
            GameObject loadingObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenLoadingPopupCoroutine(
                (GameObject popupObj) => loadingObj = popupObj));

            MetaSystem.BackupUserSyncInfo();
            System.GC.Collect();

            // Send BI
            SendAdsBIEvent("click", false);

            bool triggered = IAMRouter.Instance.TriggerIAM(InAppMessageTriggerType.INHOUSE_ADS_FOR_HIDDEN_UNIVERSE, gameObject, contextId);
            if (triggered)
            {
                var callbackTrigger = new EventTrigger(gameObject, IAMUtils.ON_IAM_CALLBACK_EVENT);
                yield return new WaitUntilTrigger(callbackTrigger);

                yield return StartCoroutine(RequestFinderBonusClaimCoroutine());

                // Send BI
                SendAdsBIEvent("complete", true);
            }

            MetaPopupUtils.ClosePopup(loadingObj);
        }

        private IEnumerator ShowVideoAdsCoroutine()
        {
            yield return new WaitForSeconds(0.2f);

            System.GC.Collect();

            // Send BI
            SendAdsBIEvent("click", false);

            bool result = VideoAdsController.Instance.ShowVideoAds(placementKey, gameObject);
            if (!result) yield break;

            var onVideoRewardedCallback = new EventTrigger(gameObject, VideoAdsController.ON_VIDEO_ADS_REWARDED);
            yield return new WaitUntilTrigger(onVideoRewardedCallback);

            GameObject loadingObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenLoadingPopupCoroutine(
                (GameObject popupObj) => loadingObj = popupObj));

            yield return StartCoroutine(RequestFinderBonusClaimCoroutine());

            // Send BI
            SendAdsBIEvent("complete", true);

            MetaPopupUtils.ClosePopup(loadingObj);
        }

        private IEnumerator RequestFinderBonusClaimCoroutine()
        {
            bool isSuccess = false;
            bool isFail = false;
            BagelCodeClientAPI.RequestHiddenObjectsVideoAdsClaim(contextId,
                (response) =>
                {
                    isSuccess = true;

                    BlackboardUtils.SetOrCreateValue(hogInfo, "lastVideoAdsClaimTimestamp", response.lastVideoAdsClaimTimestamp);
                    BlackboardUtils.SetOrCreateValue(hogInfo, "videoAdsCooltime", response.hiddenUniverseVideoAdsCooltime);

                    HiddenObjects.Utils.UpdateFinderCount(response.finder);
                },
                (error) =>
                {
                    isFail = true;
                    Debug.LogError(error.errorCode);
                });

            yield return new WaitUntil(() => isSuccess || isFail);
        }

        private IEnumerator InGameToMainTransitionCoroutine()
        {
            // Make Loading Scene
            string bundle = COMMON_BUNDLE;
            string asset = "Hidden Objects Loading Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject loadingObj = null;
            yield return StartCoroutine(MetaObjectUtils.MakeSceneCoroutine(
                bundle, asset, parent, (GameObject sceneObj) => loadingObj = sceneObj));

            // Notify loading popup has opened for closing in game
            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, HiddenObjects.Events.ON_OPEN_LOADING);

            Blackboard loadingBB = loadingObj.GetComponent<Blackboard>();
            BlackboardUtils.SetOrCreateValue(loadingBB, "isEnter", false);
            BlackboardUtils.SetOrCreateValue(loadingBB, "isMetaInGame", true);

            int chapter = targetChapterIndexVar.value;
            int stage = bb.GetValue<int>("targetStageIndex");
            BlackboardUtils.SetOrCreateValue(loadingBB, "currentChapter", chapter);
            BlackboardUtils.SetOrCreateValue(loadingBB, "currentStage", stage);

            MetaObjectUtils.SetCalleeCaller(loadingObj, gameObject);

            MetaPopupUtils.OpenPopup(loadingObj);

            var callbackTrigger = new EventTrigger(gameObject, EventSender.ON_CALLEE_CALLBACK);
            yield return new WaitUntilTrigger(callbackTrigger);
        }

        private IEnumerator MainToInGameTransitionCoroutine(bool playAgain)
        {
            // Make Loading Scene
            string bundle = COMMON_BUNDLE;
            string asset = "Hidden Objects Loading Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject loadingObj = null;
            yield return StartCoroutine(MetaObjectUtils.MakeSceneCoroutine(
                bundle, asset, parent, (GameObject sceneObj) => loadingObj = sceneObj));

            // Notify loading popup has opened for closing in game
            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, HiddenObjects.Events.ON_OPEN_LOADING);

            int chapterIndex = targetChapterIndexVar.value;
            int stageIndex = bb.GetValue<int>("targetStageIndex");
            var chapterStageInfo = new HiddenUniverseStageInfo();

            if (playAgain) // Update Chapter Stage Info
            {
                chapterStageInfo.chapter = chapterStageInfoes[chapterIndex, stageIndex].chapter;
                chapterStageInfo.stage = chapterStageInfoes[chapterIndex, stageIndex].stage;
                chapterStageInfo.needFinderCount = chapterStageInfoes[chapterIndex, stageIndex].needFinderCount;

                var prevStageInfo = bb.GetValue<Blackboard>("clearStageInfo");
                chapterStageInfo.completedStarCount = prevStageInfo.GetValue<int>("completedStarCount");
                chapterStageInfo.ongoingStarPercentile = prevStageInfo.GetValue<int>("ongoingStarPercentile");
            }
            else
            {
                chapterStageInfo.chapter = chapterStageInfoes[chapterIndex, stageIndex].chapter;
                chapterStageInfo.stage = chapterStageInfoes[chapterIndex, stageIndex].stage;
                chapterStageInfo.completedStarCount = chapterStageInfoes[chapterIndex, stageIndex].completedStarCount;
                chapterStageInfo.ongoingStarPercentile = chapterStageInfoes[chapterIndex, stageIndex].ongoingStarPercentile;
                chapterStageInfo.needFinderCount = chapterStageInfoes[chapterIndex, stageIndex].needFinderCount;
            }

            Blackboard loadingBB = loadingObj.GetComponent<Blackboard>();
            int chapterTotalStarCount = GetChapterTotalStarCount();
            int stageStarCount = chapterStageInfoes[chapterIndex, stageIndex].completedStarCount;
            BlackboardUtils.SetOrCreateValue(loadingBB, "targetChapter", chapterIndex + 1);
            BlackboardUtils.SetOrCreateValue(loadingBB, "targetStage", stageIndex + 1);
            BlackboardUtils.SetOrCreateValue(loadingBB, "stageInfo", chapterStageInfo);
            BlackboardUtils.SetOrCreateValue(loadingBB, "isEnter", true);
            BlackboardUtils.SetOrCreateValue(loadingBB, "isMetaInGame", true);
            BlackboardUtils.SetOrCreateValue(loadingBB, "playAgain", playAgain);
            BlackboardUtils.SetOrCreateValue(loadingBB, "chapterTotalStarCount", chapterTotalStarCount);
            BlackboardUtils.SetOrCreateValue(loadingBB, "stageStarCount", stageStarCount);

            MetaObjectUtils.SetCalleeCaller(loadingObj, gameObject);

            MetaPopupUtils.OpenPopup(loadingObj);

            // Enable Meta Interactables
            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ACTIVE_META_UI);

            anim.SetBool("Active", false);
        }

        private IEnumerator RequestFinderRequestCoroutine()
        {
            bool isSuccess = false;
            bool isFail = false;

            GameObject loadingObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenLoadingPopupCoroutine(
                (GameObject popupObj) => loadingObj = popupObj));

            long clubId = BlackboardUtils.FindValue<long>("/me/clubId");
            BagelCodeClientAPI.RequestHiddenObjectsFinderRequest(clubId,
                (response) =>
                {
                    isSuccess = true;
                    BlackboardUtils.SetOrCreateValue(hogInfo, "lastFinderRequestTimestamp", response.lastFinderRequestTimestamp);
                    BlackboardUtils.SetOrCreateValue(hogInfo, "finderRequestCooltime", response.hiddenUniverseFinderRequestCooltime);
                },
                (error) =>
                {
                    isFail = true;
                    // Error.HIDDEN_UNIVERSE_ALREADY_MAX_FINDER_ERROR
                    Debug.LogError(error.errorCode);
                });

            yield return new WaitUntil(() => isSuccess || isFail);

            UpdateFinderRequestButton();

            MetaPopupUtils.ClosePopup(loadingObj);

            EventSender.SendEvent(gameObject, isSuccess ? HiddenObjects.Events.ON_SUCCESS : HiddenObjects.Events.ON_FAIL);
        }

        private void SendAdsBIEvent(string step, bool withReward)
        {
            int reward = withReward ? finderBonusCount : 0;
            Analytics.CustomEvent("client_video_ad", new Dictionary<string, object>
            {
                { "action", step },
                { "type_of_reward", reward > 0 ? "finder" : "" },
                { "amount_of_reward", reward },
                { "placement", placementKey },
                { "type_of_ad",  isInHouseAds ? "inhouse" : "ironsource" }
            });
        }

        private void OnClearStage()
        {
            anim.SetBool("Active", true);
            anim.SetTrigger("Change");

            // Backup Clear Info
            bool newChapterUnlocked = BackupClearInfo();

            // Update all
            UpdateChapterReward();
            UpdateFinderRequestButton();
            UpdateFinderBonusButton();
            UpdateChapterCells(newChapterUnlocked);
            UpdateStageCells();

            // BGM On
            StartBGM();
        }

        private bool BackupClearInfo() // returning is new chapter unlocked
        {
            bool newChapterUnlocked = false;

            var clearStageInfo = BlackboardUtils.GetOrCreateVariable<Blackboard>(bb, "clearStageInfo")?.value;
            if (clearStageInfo != null)
            {
                int chapterIndex = clearStageInfo.GetValue<int>("chapter") - 1;
                int stageIndex = clearStageInfo.GetValue<int>("stage") - 1;

                var chapterStageInfo = chapterStageInfoes[chapterIndex, stageIndex];

                int prevStarCount = chapterStageInfo.completedStarCount;

                int starCount = clearStageInfo.GetValue<int>("completedStarCount");
                int starProgress = clearStageInfo.GetValue<int>("ongoingStarPercentile");

                chapterStageInfo.completedStarCount = starCount;
                chapterStageInfo.ongoingStarPercentile = starProgress;

                int needStarForStageUnlock = HiddenObjects.Utils.NeedStarForStageUnlock;
                int needStarForChapterUnlock = HiddenObjects.Utils.NeedStarForChapterUnlock;
#if DEV
                if (PlayerPrefs.GetInt(PLAYER_PREFS_CHAPTER_UNLOCK_EASY) == 1)
                {
                    chapterStageUnlocks[chapterIndex + 1, 0] = StageState.NEW_UNLOCK;
                    PlayerPrefs.SetInt(PLAYER_PREFS_LAST_UNLOCKED_CHAPTER_INDEX, chapterIndex + 1);

                    // Move Chapter Index
                    if ((chapterIndex + 1) % 4 == 0)
                        MoveChapterPage(true, false);
                }
#endif

                if (starCount > prevStarCount)
                {
                    // Stage Unlock
                    if (prevStarCount < needStarForStageUnlock &&
                        starCount >= needStarForStageUnlock && // Need Star
                        stageIndex < STAGE_CELL_COUNT - 1) // Not Last
                    {
                        chapterStageUnlocks[chapterIndex, stageIndex + 1] = StageState.NEW_UNLOCK;
                    }

                    // Chapter Unlock
                    if (chapterUnlocks.IsValidIndex(chapterIndex + 1) &&
                        !chapterUnlocks[chapterIndex + 1] && // Next Locked
                        GetChapterTotalStarCount(chapterIndex) >= needStarForChapterUnlock && // Need Star
                        IsAllStageUnlocked(chapterIndex) && // All Stage Unlock
                        chapterInfos[chapterIndex + 1].unlockLevel <= userLevel) // Level
                    {
                        chapterStageUnlocks[chapterIndex + 1, 0] = StageState.NEW_UNLOCK;
                        PlayerPrefs.SetInt(PLAYER_PREFS_LAST_UNLOCKED_CHAPTER_INDEX, chapterIndex + 1);

                        // Move Chapter Index
                        if ((chapterIndex + 1) % 4 == 0)
                            MoveChapterPage(true, false);
                    }
                }
            }

            // Check New Unlock
            int currentChapterIndex = targetChapterIndexVar.value;
            if (chapterUnlocks.IsValidIndex(currentChapterIndex + 1) &&
                chapterStageUnlocks[currentChapterIndex + 1, 0] == StageState.NEW_UNLOCK)
            {
                newChapterUnlocked = true;
            }

            return newChapterUnlocked;
        }

        private void OnSelectChapter(EventData eventData)
        {
            int targetIndex = (int)eventData.value;
            if (targetIndex == targetChapterIndexVar.value)
                return;

            if (isPlayChapterUnlockAnim || isPlayStageUnlockAnim)
                return;

            targetChapterIndexVar.value = targetIndex;

            UpdateChapterReward();
            UpdateChapterCells(false, true);
            UpdateStageCells(true);

            // Play Sound
            GSManager.Instance.GetHandler(SOUNDS_SELECT_CHAPTER).Play();

            anim.SetTrigger("Change");

            // skip 1 frame for chapter, stage cell
            EventSender.SendEvent(gameObject, HiddenObjects.Events.ON_CHANGE_CHAPTER);
        }

        private void OnUpdateFinderCount()
        {
            UpdateFinderRequestButton();
        }

        private void UpdateFinderRequestButton()
        {
            long lastFinderRequestTimestamp = hogInfo.GetValue<long>("lastFinderRequestTimestamp");
            long finderRequestCooltime = hogInfo.GetValue<long>("finderRequestCooltime");
            long currentTimestamp = TimeUtils.GetTimeStamp();
            long targetTimestamp = lastFinderRequestTimestamp + finderRequestCooltime;

            bool isCooltime = currentTimestamp < targetTimestamp;
            MetaContextElementUtils.SetActive(requestButtonActiveElement, !isCooltime);
            MetaContextElementUtils.SetActive(requestButtonInactiveElement, isCooltime);

            int currentFinder = HiddenObjects.Utils.Finder;
            int maxFinder = HiddenObjects.Utils.MaxFinder;
            bool isAvailable = !isCooltime && currentFinder < maxFinder;
            requestButtonElement.GetComponent<PIDButton>().interactable = isAvailable;

            if (isCooltime)
            {
                MetaContextElementUtils.SetCommonRemainingTimer(requestButtonRemainingTimerElement, targetTimestamp, 0L,
                    "TIME_FORMAT_HHMMSS", "", "", "", true, gameObject,
                    HiddenObjects.Events.ON_END_FINDER_REQUEST_COOLTIME);
            }
        }

        private void UpdateChapterReward()
        {
            // Rewards
            bool isCollected = false;
            bool isCompleted = IsAllStageCompleted(targetChapterIndexVar.value);
            long chapterRewardCoin = 0L;
            long chapterRewardGem = 0L;

            if (chapterInfos.IsValidIndex(targetChapterIndexVar.value))
            {
                isCollected = chapterInfos[targetChapterIndexVar.value].isRewarded;
                chapterRewardCoin = chapterInfos[targetChapterIndexVar.value].completeRewardCoin;
                chapterRewardGem = chapterInfos[targetChapterIndexVar.value].completeRewardGem;
            }

            MetaContextElementUtils.SetActive(collectButtonElement, !isCollected && isCompleted);
            MetaContextElementUtils.SetActive(textRewardElement, !isCollected);
            MetaContextElementUtils.SetActive(textCompleteElement, isCollected);

            int currentStar = GetChapterTotalStarCount();
            int maxStar = STAGE_CELL_COUNT * MAX_STAR_COUNT;
            if (isCollected)
            {
                MetaContextElementUtils.SetTextGlobal(textInfoElement,
                    "HIDDEN_OBJECTS_MAIN_SCENE_CHAPTER_REWARD_COLLECTED", maxStar, maxStar);
            }
            else
            {
                if (chapterRewardCoin > 0L && chapterRewardGem > 0L)
                {
                    MetaContextElementUtils.SetTextGlobal(textRewardElement, "HIDDEN_OBJECTS_MAIN_SCENE_CHAPTER_REWARD_COIN_GEM",
                        chapterRewardCoin, chapterRewardGem);
                }
                else if (chapterRewardCoin > 0L)
                {
                    MetaContextElementUtils.SetTextGlobal(textRewardElement, "HIDDEN_OBJECTS_MAIN_SCENE_CHAPTER_REWARD_COIN",
                        chapterRewardCoin);
                }
                else if (chapterRewardGem > 0L)
                {
                    MetaContextElementUtils.SetTextGlobal(textRewardElement, "HIDDEN_OBJECTS_MAIN_SCENE_CHAPTER_REWARD_GEM",
                        chapterRewardGem);
                }
                else
                {
                    MetaContextElementUtils.SetText(textRewardElement, "");
                }

                if (isCompleted) // collectable
                {
                    MetaContextElementUtils.SetTextGlobal(textInfoElement, "HIDDEN_OBJECTS_MAIN_SCENE_CHAPTER_REWARD_COMPLETED",
                    (targetChapterIndexVar.value + 1).ToString());

                    MetaContextElementUtils.SimpleSetTextGlobal(collectButtonElement, "Text",
                        "BUTTON_COLLECT", CHILDREN);
                }
                else // default
                {
                    MetaContextElementUtils.SetTextGlobal(textInfoElement,
                        "HIDDEN_OBJECTS_MAIN_SCENE_CHAPTER_REWARD_INFO", currentStar, maxStar);
                }
            }
        }

        private void UpdateFinderBonusButton()
        {
            isAdsEnabled = false;

            if (finderBonusCount > 0)
            {
                // Check Ads
                var inhouseAdsEnabled = BlackboardUtils.FindVariable<bool>("/values/misc/INHOUSE_ADS_ENABLED");
                var videoAdsEnabled = BlackboardUtils.FindVariable<bool>("/values/misc/VIDEO_ADS_ENABLED");
                if (inhouseAdsEnabled.value)
                {
                    placementKey = "";
                    if (IAMRouter.Instance.CheckTriggerIAM(InAppMessageTriggerType.INHOUSE_ADS_FOR_HIDDEN_UNIVERSE))
                    {
                        isAdsEnabled = true;
                        isInHouseAds = true;
                    }
                }
                else if (videoAdsEnabled.value)
                {
                    placementKey = BlackboardUtils.FindVariable<string>("/videoAdsPlacementNames/hiddenUniverse").value;
                    if (!string.IsNullOrEmpty(placementKey) && VideoAdsController.Instance.IsVideoAdsAvailable(placementKey))
                    {
                        isAdsEnabled = true;
                        isInHouseAds = false;
                    }
                }
            }

            MetaContextElementUtils.SetActive(finderBonusAreaElement, isAdsEnabled);

            // Show Bonus Button
            if (isAdsEnabled)
            {
                long lastVideoAdsClaimTimestamp = hogInfo.GetValue<long>("lastVideoAdsClaimTimestamp");
                long videoAdsCooltime = hogInfo.GetValue<long>("videoAdsCooltime");
                long currentTimestamp = TimeUtils.GetTimeStamp();
                long targetTimestamp = lastVideoAdsClaimTimestamp + videoAdsCooltime;

                bool isCooltime = currentTimestamp < targetTimestamp;
                MetaContextElementUtils.SetActive(finderBonusAreaAvtivedElement, !isCooltime);
                MetaContextElementUtils.SetActive(finderBonusAreaInactivedElement, isCooltime);

                if (isCooltime)
                {
                    MetaContextElementUtils.SetCommonRemainingTimer(finderBonusCooltimeTimerElement, targetTimestamp, 0L,
                        "TIME_FORMAT_HHMMSS", null, null, null, true, gameObject,
                        HiddenObjects.Events.ON_END_FINDER_BONUS_COOLTIME);
                }
            }
        }

        private void InitStageCells()
        {
            for (int i = 0; i < STAGE_CELL_COUNT; ++i)
            {
                var cellElement = stageCellElements[i];
                var cellBB = cellElement.GetComponent<Blackboard>();
                BlackboardUtils.SetOrCreateValue(cellBB, "caller", gameObject);
                BlackboardUtils.SetOrCreateValue(cellBB, "stage", i + 1);

                MetaContextElementUtils.SetActive(cellElement, true);
            }
        }

        private void UpdateStageCells(bool byChapterChanged = false)
        {
            int chapterIndex = targetChapterIndexVar.value;

            bool meetFirstLockedCell = false;
            int needStar;
            for (int i = 0; i < STAGE_CELL_COUNT; ++i)
            {
                var stageInfo = chapterStageInfoes[chapterIndex, i];
                int completedStarCount = stageInfo.completedStarCount;

                if (i == 0)
                {
                    needStar = 0;
                }
                else
                {
                    var prevStageInfo = chapterStageInfoes[chapterIndex, i - 1];
                    needStar = HiddenObjects.Utils.NeedStarForStageUnlock - prevStageInfo.completedStarCount;
                }

                bool isMastered = completedStarCount == MAX_STAR_COUNT;

                bool showLockInfo = false;
                bool isLocked = chapterStageUnlocks[chapterIndex, i] == StageState.LOCK;
                if (!meetFirstLockedCell && isLocked && i > 0)
                {
                    meetFirstLockedCell = true;
                    showLockInfo = true;

                    var prevCellBB = stageCellElements[i - 1].GetComponent<Blackboard>();
                    BlackboardUtils.SetOrCreateValue(prevCellBB, "isButton", true);
                }

                bool isNewUnlocked = false;
                if (chapterStageUnlocks[chapterIndex, i] == StageState.NEW_UNLOCK)
                {
                    isNewUnlocked = true;
                    chapterStageUnlocks[chapterIndex, i] = StageState.UNLOCK;
                }

                var cellBB = stageCellElements[i].GetComponent<Blackboard>();
                BlackboardUtils.SetOrCreateValue(cellBB, "isUpdated", true);
                BlackboardUtils.SetOrCreateValue(cellBB, "chapter", chapterIndex + 1);
                BlackboardUtils.SetOrCreateValue(cellBB, "isLocked", isLocked);
                BlackboardUtils.SetOrCreateValue(cellBB, "showLockInfo", showLockInfo);
                BlackboardUtils.SetOrCreateValue(cellBB, "isMastered", isMastered);
                BlackboardUtils.SetOrCreateValue(cellBB, "isNewUnlocked", isNewUnlocked);
                BlackboardUtils.SetOrCreateValue(cellBB, "isButton", false);

                BlackboardUtils.SetOrCreateValue(cellBB, "completedStarCount", stageInfo.completedStarCount);
                BlackboardUtils.SetOrCreateValue(cellBB, "ongoingStarPercentile", stageInfo.ongoingStarPercentile);
                BlackboardUtils.SetOrCreateValue(cellBB, "needFinderCount", stageInfo.needFinderCount);
                BlackboardUtils.SetOrCreateValue(cellBB, "needStar", needStar);

                bool isStoryEnabled = HiddenObjects.Utils.IsStoryEnabled(chapterIndex + 1, 0);
                BlackboardUtils.SetOrCreateValue(cellBB, "isWaitStory", byChapterChanged && isStoryEnabled);
            }
        }

        private void InitChapterCells()
        {
            for (int i = 0; i < CHAPTER_CELL_COUNT; ++i)
            {
                var cellElement = chapterCellElements[i];
                var cellBB = cellElement.GetComponent<Blackboard>();
                BlackboardUtils.SetOrCreateValue(cellBB, "caller", gameObject);

                MetaContextElementUtils.SetActive(cellElement, true);
            }
        }

        private int GetUnlockedChapterCount()
        {
            int result = 0;
            for (int i = 0; i < visibleChapterCount; ++i)
                result += chapterUnlocks[i] ? 1 : 0;
            return result;
        }

        private bool IsAllStageUnlocked(int chapterIndex)
        {
            for (int i = 0; i < STAGE_CELL_COUNT - 1; ++i)
            {
                var stageInfo = chapterStageInfoes[chapterIndex, i];
                if (stageInfo.completedStarCount < HiddenObjects.Utils.NeedStarForStageUnlock)
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsAllStageCompleted(int chapterIndex)
        {
            for (int i = 0; i < STAGE_CELL_COUNT; ++i)
            {
                var stageInfo = chapterStageInfoes[chapterIndex, i];
                if (stageInfo.completedStarCount < MAX_STAR_COUNT)
                {
                    return false;
                }
            }

            return true;
        }

        private void UpdateChapterCells(bool unlockNewChapter = false, bool byChapterChanged = false, bool unlockCurrentChapter = false)
        {
            bool isPrevChapterUnlock = true;
            bool isPrevChapterStageAllUnlock = true;
            int prevChapterStarCompletedCount = 0;

            int targetChapterIndex = targetChapterIndexVar.value;

            int unlockedChapterCount = GetUnlockedChapterCount();

            // Including Coming Soon
            int allChapterCount = chapterPageCount * CHAPTER_CELL_COUNT;

            for (int i = 0; i < allChapterCount; ++i)
            {
                int chapterNumber = i + 1;
                int chapterIndex = i;

                bool isLocked = false;
                bool isComingSoon = chapterCount <= i;

                // Unlock Last
                bool isNewUnlocked =
                    (unlockNewChapter && i == unlockedChapterCount) ||
                    (unlockCurrentChapter && i == unlockedChapterCount - 1);

                if (i < chapterCount &&
                    chapterStageUnlocks[i, 0] == StageState.NEW_UNLOCK)
                {
                    chapterStageUnlocks[i, 0] = StageState.UNLOCK;
                }

                if (isNewUnlocked && !isComingSoon) chapterUnlocks[i] = true;

                int totalCompletedStarCount = 0;
                bool isAllStageCompleted = !isComingSoon;
                bool isAllStageUnlocked = !isComingSoon;
                bool isLevelLock = !isComingSoon;
                int minLevel = 0;

                if (!isComingSoon)
                {
                    isLocked = !chapterUnlocks[i] && !isNewUnlocked; // new unlocked -> locked
                    isAllStageUnlocked = IsAllStageUnlocked(i);
                    isAllStageCompleted = IsAllStageCompleted(i);

                    for (int j = 0; j < STAGE_CELL_COUNT; ++j)
                    {
                        var stageInfo = chapterStageInfoes[i, j];
                        totalCompletedStarCount += stageInfo.completedStarCount;
                    }

                    minLevel = chapterInfos[chapterIndex].unlockLevel;
                    isLevelLock = userLevel < minLevel;
                }

                bool isDisplaying =
                    chapterPageIndex * CHAPTER_CELL_COUNT <= i &&
                    i < (chapterPageIndex + 1) * CHAPTER_CELL_COUNT;

                if (isDisplaying)
                {
                    int cellIndex = i - chapterPageIndex * CHAPTER_CELL_COUNT;
                    var cellBB = chapterCellElements[cellIndex].GetComponent<Blackboard>();

                    BlackboardUtils.SetOrCreateValue(cellBB, "isUpdated", true);
                    BlackboardUtils.SetOrCreateValue(cellBB, "isSelected", i == targetChapterIndex);
                    BlackboardUtils.SetOrCreateValue(cellBB, "chapter", chapterNumber);
                    BlackboardUtils.SetOrCreateValue(cellBB, "isLocked", isLocked);
                    BlackboardUtils.SetOrCreateValue(cellBB, "isNewUnlocked", isNewUnlocked);
                    BlackboardUtils.SetOrCreateValue(cellBB, "isComingSoon", isComingSoon);
                    BlackboardUtils.SetOrCreateValue(cellBB, "isMastered", isAllStageCompleted);

                    // For Unlock Info
                    BlackboardUtils.SetOrCreateValue(cellBB, "isPrevChapterUnlock", isPrevChapterUnlock);
                    BlackboardUtils.SetOrCreateValue(cellBB, "isPrevChapterStageAllUnlock", isPrevChapterStageAllUnlock);
                    BlackboardUtils.SetOrCreateValue(cellBB, "prevChapterCompltetedStarCount", prevChapterStarCompletedCount);
                    BlackboardUtils.SetOrCreateValue(cellBB, "isLevelLock", isLevelLock);
                    BlackboardUtils.SetOrCreateValue(cellBB, "minLevel", minLevel);

                    bool isStoryEnabled = HiddenObjects.Utils.IsStoryEnabled(chapterNumber, 0);
                    BlackboardUtils.SetOrCreateValue(cellBB, "isWaitStory", byChapterChanged && isStoryEnabled);
                }

                isPrevChapterUnlock = !isLocked;
                isPrevChapterStageAllUnlock = isAllStageUnlocked;
                prevChapterStarCompletedCount = totalCompletedStarCount;
            }
        }

        private int GetChapterTotalStarCount(int chapterIndex = -1)
        {
            if (chapterIndex == -1)
            {
                chapterIndex = targetChapterIndexVar.value;
            }

            int result = 0;
            for (int j = 0; j < STAGE_CELL_COUNT; ++j)
                result += chapterStageInfoes[chapterIndex, j].completedStarCount;
            return result;
        }

        private void MoveChapterPage(bool isRight, bool update = true)
        {
            if (isPlayChapterUnlockAnim || isPlayStageUnlockAnim)
                return;

            SetChapterPage(chapterPageIndex + (isRight ? 1 : -1), update);
        }

        private void SetChapterPage(int targetIndex, bool update)
        {
            int prevIndex = chapterPageIndex;
            chapterPageIndex = targetIndex;

            MathUtils.NormalizeArrayIndex(ref chapterPageIndex, chapterPageCount);

            if (chapterPageIndex == prevIndex) return;

            if (update)
            {
                UpdateChapterCells();
            }
        }
    }
}
