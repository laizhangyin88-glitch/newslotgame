using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using SlotMaker;
using BagelCode.ClientModels;
using Sirenix.OdinInspector;
using ParadoxNotion;

namespace BagelCode.LevelUpDash
{
    public class LevelUpDashMainController : EventMonoBehaviour
    {
        private const float DASH_AUTO_CLOSE_TIME_SEC = 7f;
        private const int MAX_CELL_COUNT = 3;
        private const float REWARD_DISPLAY_TIME = 7f;
        private const float LEVEL_CELL_UPDATE_TIME = 0.5f;

        //

        private ContextElement root;
        private Blackboard bb;

        private List<Blackboard> stageList;
        private int StageCount => stageList?.Count ?? 0;

        private Animator[] cellAnims = new Animator[MAX_CELL_COUNT];
        private ContextElement[] cellElements = new ContextElement[MAX_CELL_COUNT];
        private ContextElement[] cellProgressElements = new ContextElement[MAX_CELL_COUNT];
        private ContextElement[] cellProgressTextElements = new ContextElement[MAX_CELL_COUNT];
        private ContextElement[] cellRewardAreaElements = new ContextElement[MAX_CELL_COUNT];

        private float[] remainingRewardDisplays = new float[MAX_CELL_COUNT];
        private int[] targetLevels = new int[MAX_CELL_COUNT];

        private bool isAutoClose = false;
        private bool isAutoClosed = false;
        private float remainingAutoClose = 0f;
        private bool isInGame = false;

        private GameObject rewardPopupObj = null;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        public void Init()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();

            root.UpdateContext(false);

            // AE
            string enterType = BlackboardUtils.FindVariable<string>(bb, "enterType")?.value;
            string metaGroupContextId = BlackboardUtils.GetOrCreateVariable<string>(bb, "metaGroupContextId")?.value;
            BiEventUtils.SendBiEventEnter(enterType, "level_up_dash", metaGroupContextId);

            // Make Background
            bool isVertical = false;
#if !UNITY_WEBGL && !UNITY_WSA
            Orientation orientation = BlackboardQueryUtils.GetOrientation();
            isVertical = orientation == Orientation.PORTRAIT;
#endif
            var backgroundAreaElement = ContextUtils.FindElement(root, "Background Area", CHILDREN);
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = isVertical ? "Level Up Dash Background Vertical" : "Level Up Dash Background";
            Transform parent = backgroundAreaElement.transform;

            MetaObjectUtils.MakePrefab(bundle, asset, parent);

            // Enter Meta
            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, "OnEnterMetaGame");

            string playerPrefsKey = string.Format(LevelUpDash.Defines.PLAYER_PREFS_IS_FIRST_ENTER_FORMAT, LevelUpDash.Utils.EventID);
            bool isFirst = PlayerPrefs.GetInt(playerPrefsKey, 1) == 1;
            BlackboardUtils.SetOrCreateValue(bb, "isFirst", isFirst);

            var profileElement = ContextUtils.FindElement(root, "Profile", CHILDREN);
            var profileBB = profileElement.GetComponent<Blackboard>();
            BlackboardUtils.SetOrCreateValue(profileBB, "hideTags", true);
            BlackboardUtils.SetOrCreateValue(profileBB, "disableClick", true);

            MetaContextElementUtils.SimpleSetClickable(
                root, "Close", gameObject, EventSender.ON_CUSTOM_EVENT, "OnClose");

            // Cells
            stageList = LevelUpDash.Utils.StageList;
            for (int i = 0; i < MAX_CELL_COUNT; ++i)
            {
                string cellName = string.Format("Level Up Dash Gauge Cell {0}", (i + 1));
                cellElements[i] = ContextUtils.FindElement(root, cellName, CHILDREN);
                cellAnims[i] = cellElements[i].GetComponent<Animator>();

                bool isCellActive = stageList.IsValidIndex(i);
                MetaContextElementUtils.SetActive(cellElements[i], isCellActive);

                if (isCellActive)
                {
                    cellProgressElements[i] = ContextUtils.FindElement(cellElements[i], "Progress Bar", CHILDREN);
                    cellProgressTextElements[i] = ContextUtils.FindElement(cellElements[i], "Text Progress", CHILDREN);
                    cellRewardAreaElements[i] = ContextUtils.FindElement(cellElements[i], "Reward Cell Area", CHILDREN);

                    // Level
                    Blackboard stageBB = stageList[i];
                    targetLevels[i] = BlackboardUtils.GetOrCreateVariable<int>(stageBB, "targetLevel")?.value ?? 0;

                    int startLevel = LevelUpDash.Utils.GetMissionStartLevel();
                    UpdateLevelCellProgress(i, startLevel, 0L);

                    MetaContextElementUtils.SimpleSetTextGlobal(cellElements[i],
                        "Text Reach Level", "LEVEL_UP_DASH_REACH_LEVEL_FORMAT", CHILDREN, targetLevels[i]);

                    // Rewards
                    int targetIndex = i;
                    MetaContextElementUtils.SimpleSetClickable(cellElements[i], "Level Up Dash Chest",
                        () =>
                        {
                            isAutoClose = false;
                            ShowReward(targetIndex);
                        });

                    MetaContextElementUtils.SetClickable(cellRewardAreaElements[i],
                        () =>
                        {
                            isAutoClose = false;
                            remainingRewardDisplays[targetIndex] = 0f;
                        });

                    MakeRewardCells(stageBB, i);
                }
            }

            isInGame = BlackboardQueryUtils.IsIngame();
            if (!isInGame)
                GSManager.Instance.GetAudioMixerSnapshot(MetaStringDefine.SNAPSHOT_LOBBY_MAIN).TransitionTo(0);

            // Play BGM
            GSManager.Instance.GetHandler(LevelUpDash.Defines.SOUND_BGM).Play();

            // Auto Spin -> Auto Close  
            isAutoClose = BlackboardUtils.FindVariable<bool>(bb, "prevAutoSpinState")?.value ?? false;
            remainingAutoClose = DASH_AUTO_CLOSE_TIME_SEC;

            // Update All
            UpdatePurchaseBoostState(true);
            UpdateTimer();

            // Events
            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);
            Register(LevelUpDash.Events.ON_END_PURCHASE_BOOSTER, () => UpdatePurchaseBoostState(false));
            Register(MetaEventDefine.ON_META_UI_EVENT, LevelUpDash.Events.ON_UPDATE_LEVEL_UP_DASH, () => UpdatePurchaseBoostState(false));
        }

        private void UpdateLevelCellProgress(int i, int level, long exp)
        {
            float progress = GetLevelCellProgress(i, level, exp);
            UpdateLevelCellProgress(i, progress);
        }

        private float GetLevelCellProgress(int i, int level, long exp)
        {
            long currentExp, targetExp;
            float progress;
            if (i == 0)
            {
                int startLevel = LevelUpDash.Utils.GetMissionStartLevel();
                currentExp = BlackboardQueryUtils.GetRequiredExp(startLevel, level);

                if (level >= startLevel)
                    currentExp += exp;

                targetExp = BlackboardQueryUtils.GetRequiredExp(startLevel, targetLevels[i]);
            }
            else // i > 0
            {
                currentExp = BlackboardQueryUtils.GetRequiredExp(targetLevels[i - 1], level);

                if (level >= targetLevels[i - 1])
                    currentExp += exp;

                targetExp = BlackboardQueryUtils.GetRequiredExp(targetLevels[i - 1], targetLevels[i]);
            }

            progress = targetExp > 0 ? (currentExp / (float)targetExp) : 0f;
            progress = Mathf.Clamp01(progress);

            return progress;
        }

        private void UpdateLevelCellProgress(int i, float progress)
        {
            MetaContextElementUtils.SimpleSetActive(cellProgressElements[i], "Bottom", progress > 0f);
            MetaContextElementUtils.SimpleSetActive(cellProgressElements[i], "Handle", progress > 0f);

            string progressText = TextDecoUtils.RatioToPercentage(progress);
            MetaContextElementUtils.SetText(cellProgressTextElements[i], progressText);
            MetaContextElementUtils.SetSliderValue(cellProgressElements[i], progress);
        }

        private void Update()
        {
            for(int i = 0; i < StageCount; ++i)
            {
                if(remainingRewardDisplays[i] > 0f)
                {
                    remainingRewardDisplays[i] -= Time.deltaTime;
                }
                else if(remainingRewardDisplays[i] != float.MinValue)
                {
                    remainingRewardDisplays[i] = float.MinValue;
                    cellAnims[i].SetBool("isRewardAppear", false);
                }
            }

            if (isAutoClose)
            {
                remainingAutoClose -= Time.deltaTime;
                if(remainingAutoClose <= 0f)
                {
                    isAutoClosed = true;
                    isAutoClose = false;
                    EventSender.SendEvent(gameObject, "OnAutoClose");
                }
            }
        }

        private void ShowReward(int i)
        {
            if (cellAnims.IsValidIndex(i))
            {
                cellAnims[i].SetBool("isRewardAppear", true);
                remainingRewardDisplays[i] = REWARD_DISPLAY_TIME;
            }
        }

        private void MakeRewardCells(Blackboard stageBB, int index)
        {
            var rewardBBList = BlackboardUtils.GetOrCreateVariable<List<Blackboard>>(stageBB, "rewardBundle")?.value;
            if (rewardBBList != null && rewardBBList.Count > 0)
            {
                for (int i = 0; i < rewardBBList.Count; ++i)
                {
                    var rewardBB = rewardBBList[i];
                    RewardType type = BlackboardUtils.GetOrCreateVariable<RewardType>(rewardBB, "type")?.value ?? RewardType.UNKNOWN;
                    string rewardAssetName = MetaCommonRewardUtils.GetRewardImageAssetName(type, null);

                    if (string.IsNullOrEmpty(rewardAssetName)) continue;

                    // Make Reward Cell
                    string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
                    string asset = "Level Up Dash Reward Cell";
                    Transform parent = cellRewardAreaElements[index].transform;

                    var cellObj = MetaObjectUtils.MakePrefab(bundle, asset, parent);
                    var rewardCellElement = cellObj.GetComponent<ContextElement>();

                    rewardCellElement.UpdateContext(true);

                    // Set Reward Image
                    var imageAreaElement = ContextUtils.FindElement(rewardCellElement, "Reward Area", CHILDREN);
                    MetaCommonRewardUtils.CommonRewardResultSetter(imageAreaElement, null, rewardBB, type, true, false, null, RewardCheckScene.LEVEL_UP_DASH);
                    string countText = MetaCommonRewardUtils.GetCommonRewardAmountSimpleText(rewardBB, true, type, RewardCheckScene.LEVEL_UP_DASH);

                    MetaContextElementUtils.SimpleSetText(rewardCellElement, "Text Count", countText);
                }
            }
        }

        private void UpdateTimer()
        {
            var timerElement = ContextUtils.FindElement(root, "Timer", CHILDREN);
            var textElement = ContextUtils.FindElement(timerElement, "Text Time", CHILDREN);
            var timerController = timerElement.GetComponent<RemainingTimerController>();
            timerController.Init(
                textElement,
                MetaStringDefine.TIME_FORMAT_HHMMSS_TOTALHOUR,
                "",
                "",
                "Ended",
                true,
                null);

            timerController.StartTimer(LevelUpDash.Utils.EndTimestamp, 0L);
        }

        private void UpdatePurchaseBoostState(bool isInitialize)
        {
            bool isEnable = LevelUpDash.Utils.IsEnabledPurchaseBooster();
            bool isActive = LevelUpDash.Utils.IsActivePurchaseBooster();

            var boostAreaElement = ContextUtils.FindElement(root, "Level Up Exp Area", CHILDREN);
            var boostButtonElement = ContextUtils.FindElement(boostAreaElement, "Button", CHILDREN);
            var tagAreaElement = ContextUtils.FindElement(boostAreaElement, "Event Tag Area", CHILDREN);

            MetaContextElementUtils.SetActive(boostAreaElement, isEnable);
            MetaContextElementUtils.SetActive(tagAreaElement, isActive);

            if (isInitialize)
            {
                MetaContextElementUtils.SetClickable(boostButtonElement,
                    () =>
                    {
                        isAutoClose = false;
                        EventSender.SendEvent(gameObject, "OnClickBoost");
                    }, false);
            }

            if (isEnable)
            {
                long multiplyNumerator = LevelUpDash.Utils.PurchaseBoosterMultiplierNumerator;
                double multiplier = NumberUtils.GetMultiplierFromNumerator(multiplyNumerator);
                MetaContextElementUtils.SimpleSetTextGlobal(
                    boostButtonElement, "Text Exp Multi", "LEVEL_UP_DASH_MAIN_SCENE_BOOSTER_MULTIPLY", CHILDREN, multiplier);
            }

            if (isActive)
            {
                var remainingTimerElement = ContextUtils.FindElement(tagAreaElement, "Event Tag Without Text/Remaining Timer", FULL);

                MetaContextElementUtils.SetCommonRemainingTimer(
                    remainingTimerElement,
                    LevelUpDash.Utils.PurchaseBoosterEndTimestamp,
                    0L,
                    MetaStringDefine.TIME_FORMAT_HHMMSS_TOTALHOUR,
                    "",
                    "",
                    "Ended",
                    true,
                    gameObject,
                    LevelUpDash.Events.ON_END_PURCHASE_BOOSTER);
            }
        }

        public IEnumerator UpdateLevelProgressCoroutine()
        {
            if (stageList == null) yield break;

            bool meetFirstActived = false;
            bool isExpIncreased = false;
            for (int i = 0; i < StageCount; ++i)
            {
                var stageBB = stageList[i];
                bool isClaimed = BlackboardUtils.GetOrCreateVariable<bool>(stageBB, "isClaimedReward")?.value ?? false;

                if (isClaimed)
                {
                    cellAnims[i].SetTrigger("isCollected");

                    UpdateLevelCellProgress(i, 1f);
                }
                else if (!meetFirstActived)
                {
                    meetFirstActived = true;
                    cellAnims[i].SetTrigger("isActive");

                    int startLevel = LevelUpDash.Utils.GetMissionStartLevel();
                    int level = BlackboardQueryUtils.GetMyLevel();
                    int targetLevel = LevelUpDash.Utils.GetTargetMissionLevel(i);
                    level = Mathf.Min(level, targetLevel);

                    long exp = BlackboardQueryUtils.GetCurrentExp();
                    if (level >= targetLevel) exp = 0L;

                    if (level > startLevel || exp > 0L)
                    {
                        isExpIncreased = true;
                        StartCoroutine(UpdateCellProgressSmoothCoroutine(i, startLevel, 0L, level, exp));
                    }
                }
                else
                {
                    cellAnims[i].SetTrigger("isLocked");

                    UpdateLevelCellProgress(i, 0f);
                }
            }

            if (isExpIncreased)
            {
                yield return new WaitForSeconds(LEVEL_CELL_UPDATE_TIME);
            }
        }

        public IEnumerator CheckRewardCoroutine()
        {
            if (stageList == null) yield break;

            for (int i = 0; i < StageCount; ++i)
            {
                int targetLevel = LevelUpDash.Utils.GetTargetMissionLevel(i);
                int level = BlackboardQueryUtils.GetMyLevel();

                if (level < targetLevel || targetLevel < 0) continue;

                // level >= targetLevel
                var stageBB = stageList[i];
                bool isClaimed = BlackboardUtils.GetOrCreateVariable<bool>(stageBB, "isClaimedReward")?.value ?? false;

                if (!isClaimed)
                {
                    // Request Clear
                    var rewardResultList = BlackboardUtils.FindVariable<List<Blackboard>>(bb, "rewardResultList")?.value;
                    if (rewardResultList != null && rewardResultList.Count > 0)
                    {
                        BlackboardUtils.SetOrCreateValue(stageBB, "isClaimedReward", true);

                        foreach (var rewardResult in rewardResultList)
                            BlackboardUtils.SetOrCreateValue(rewardResult, "fromLevelUpDash", true);

                        // Change Snapshot
                        if (!isInGame)
                            GSManager.Instance.GetAudioMixerSnapshot(MetaStringDefine.SNAPSHOT_CONTENT_POPUP).TransitionTo(0);

                        // Open Reward Popup
                        bool isFinal = i == (stageList.Count - 1);

                        rewardPopupObj = MetaCommonRewardUtils.OpenCommonRewardResultPopup(gameObject, rewardResultList, true, isFinal, RewardCheckScene.LEVEL_UP_DASH);

                        var popupBB = rewardPopupObj.GetComponent<Blackboard>();
                        BlackboardUtils.SetOrCreateValue(popupBB, "playSound", false);

                        // Complete Sound
                        if (isFinal)
                            GSManager.Instance.GetHandler(LevelUpDash.Defines.SOUND_COLLECT_FINAL_REWARD).Play();
                        else
                            GSManager.Instance.GetHandler(LevelUpDash.Defines.SOUND_COLLECT_REWARD).Play();

                        // Until Close Reward Popup
                        var callbackTrigger = new EventTrigger(gameObject, EventSender.ON_CALLEE_CALLBACK);
                        yield return new WaitUntilTrigger(callbackTrigger);

                        // callback = interrupted
                        isAutoClose = false;

                        // Rollback Snapshot
                        if (!isInGame)
                            GSManager.Instance.GetAudioMixerSnapshot(MetaStringDefine.SNAPSHOT_LOBBY_MAIN).TransitionTo(0);

                        // Collecting
                        GSManager.Instance.GetHandler(LevelUpDash.Defines.SOUND_CHECK_REWARD_BOX).Play();
                        cellAnims[i].SetTrigger("Collecting");

                        BlackboardUtils.SetOrCreateValue(bb, "isFinish", isFinal);
                        BlackboardUtils.SetOrCreateValue(bb, "showNextReward", false);

                        yield return new WaitForSeconds(1f);

                        if (i + 1 < StageCount) // unlock next
                        {
                            cellAnims[i + 1].SetTrigger("Unlock");
                            GSManager.Instance.GetHandler(LevelUpDash.Defines.SOUND_UNLOCK).Play();
                            BlackboardUtils.SetOrCreateValue(bb, "showNextReward", true);

                            yield return new WaitForSeconds(1f);
                        }
                    }
                }
            }
        }

        public void ShowRemainingRewards()
        {
            int index = LevelUpDash.Utils.GetCurrentStageIndex();
            if (index >= 0)
            {
                for (int i = index; i < StageCount; ++i)
                {
                    ShowReward(i);
                }
            }
            string playerPrefsKey = string.Format(LevelUpDash.Defines.PLAYER_PREFS_IS_FIRST_ENTER_FORMAT, LevelUpDash.Utils.EventID);
            PlayerPrefs.SetInt(playerPrefsKey, 0);
        }

        public void ShowCurrentReward()
        {
            int index = LevelUpDash.Utils.GetCurrentStageIndex();
            if (stageList.IsValidIndex(index))
                ShowReward(index);
        }

        public void OnClose(bool byEnterGame)
        {
            // Stop BGM
            GSManager.Instance.GetHandler(LevelUpDash.Defines.SOUND_BGM).Stop();

            // by auto close
            if(rewardPopupObj != null)
                MetaPopupUtils.ClosePopup(rewardPopupObj);

            // todo
            // Iam 에서 Vip Welcome 활성화 하는 경우 대응
            if (!byEnterGame)
            {
                EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, "OnLeaveMetaGame");
            }

            // Finish Current Stage
            LevelUpDash.Utils.ClearMissionRewardResultList();

            // Check Level Dash Finished
            bool isActive = BlackboardQueryUtils.IsActiveMetaGameEvent(EventInfoType.LEVEL_UP_DASH_MISSION);
            if (!isActive) LevelUpDash.Utils.FinishLevelUpDash();

            var eventData = new EventData<bool>("OnCalleeCallback", !isAutoClosed);
            EventSender.SendCalleeCallback(gameObject, eventData);

            MetaPopupUtils.ClosePopup(gameObject);
        }

        private IEnumerator UpdateCellProgressSmoothCoroutine(int i, int fromLevel, long fromExp, int toLevel, long toExp)
        {
            float from = GetLevelCellProgress(i, fromLevel, fromExp);
            float to = GetLevelCellProgress(i, toLevel, toExp);
            float scale = to - from;

            if (scale > 0.01f)
                GSManager.Instance.GetHandler(LevelUpDash.Defines.SOUND_GAUGE_INCREASE).Play();

            yield return StartCoroutine(
                AsyncActionUtils.ProgressiveActionCoroutine(
                    LEVEL_CELL_UPDATE_TIME,
                    null,
                    (t) =>
                    {
                        float progress = t * scale + from;
                        UpdateLevelCellProgress(i, progress);
                    },
                    null));
        }

#if DEV
        [Button]
        private void DebugSetProgressFull(int targetCellIndex)
        {
            StartCoroutine(DebugSetProgressFullCoroutine(targetCellIndex));
        }

        private IEnumerator DebugSetProgressFullCoroutine(int targetCellIndex)
        {
            int i = targetCellIndex;
            if (stageList.IsValidIndex(i))
            {
                cellAnims[i].SetTrigger("isActive");

                int startLevel = LevelUpDash.Utils.GetMissionStartLevel();
                StartCoroutine(UpdateCellProgressSmoothCoroutine(i, startLevel, 0L, targetLevels[i], 0));

                yield return new WaitForSeconds(LEVEL_CELL_UPDATE_TIME);

                // Change Snapshot
                if (!isInGame)
                    GSManager.Instance.GetAudioMixerSnapshot(MetaStringDefine.SNAPSHOT_CONTENT_POPUP).TransitionTo(0);

                // Open Reward Popup
                bool isFinal = i == (stageList.Count - 1);
                string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
                string asset = isFinal ? "Popup Common Reward Title Tag Scene" : "Popup Common Reward Scene";
                Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

                var popupObj = MetaObjectUtils.MakeScene(bundle, asset, parent);
                var popupBB = popupObj.GetComponent<Blackboard>();

                string title = isFinal ?
                    StringTableUtils.GetString(StringTable.StringTableType.Global, "LEVEL_UP_DASH_MAIN_SCENE_REWARD_CONGRATULATIONS") :
                    StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_PURCHASE_REWARD_TITLE");

                BlackboardUtils.SetOrCreateValue(popupBB, "rewardList", new List<Blackboard>());
                BlackboardUtils.SetOrCreateValue(popupBB, "title", title);
                BlackboardUtils.SetOrCreateValue(popupBB, "applyEarnValues", false);
                MetaObjectUtils.SetCalleeCaller(popupObj, gameObject);

                MetaPopupUtils.OpenPopup(popupObj);

                BlackboardUtils.SetOrCreateValue(popupBB, "playSound", false);

                // Complete Sound
                if (isFinal)
                    GSManager.Instance.GetHandler(LevelUpDash.Defines.SOUND_COLLECT_FINAL_REWARD).Play();
                else
                    GSManager.Instance.GetHandler(LevelUpDash.Defines.SOUND_COLLECT_REWARD).Play();

                // Until Close Reward Popup
                var callbackTrigger = new EventTrigger(gameObject, EventSender.ON_CALLEE_CALLBACK);
                yield return new WaitUntilTrigger(callbackTrigger);

                // Rollback Snapshot
                if (!isInGame)
                    GSManager.Instance.GetAudioMixerSnapshot(MetaStringDefine.SNAPSHOT_LOBBY_MAIN).TransitionTo(0);

                // Collecting
                GSManager.Instance.GetHandler(LevelUpDash.Defines.SOUND_CHECK_REWARD_BOX).Play();
                cellAnims[i].SetTrigger("Collecting");

                yield return new WaitForSeconds(1f);

                if (i + 1 < StageCount) // unlock next
                {
                    cellAnims[i + 1].SetTrigger("Unlock");
                    GSManager.Instance.GetHandler(LevelUpDash.Defines.SOUND_UNLOCK).Play();

                    yield return new WaitForSeconds(1f);
                }
            }
        }
#endif
    }
}
