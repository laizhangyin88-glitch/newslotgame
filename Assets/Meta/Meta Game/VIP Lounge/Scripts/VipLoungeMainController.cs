using System.Collections;
using UnityEngine;
using SlotMaker;
using ParadoxNotion;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using System.Collections.Generic;

using static BagelCode.VipLounge.VipLounge.Defines;

namespace BagelCode.VipLounge
{
    public class VipLoungeMainController : EventMonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;
        private Animator anim;

        private bool isInit = false;
        private bool isPassive = false;

        private Blackboard vipLoungeInfo;

        private ChaseTypeLong jackpotChase = null;
        private ContextElement jackpotScoreTextElement = null;
        private ContextElement gaugeElement = null;
        private ContextElement gaugeTextElement = null;
        private ContextElement metaGameBadgeElement = null;

        private GameObject gs_managerObj = null;

        private string loungeOpenCooltime;
        private string loungeJackpotCooltime;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        //private Animator infoSpeechAnimator;
        //private Coroutine infoSpeechBalloonEnumerator = null;

        private Dictionary<string, (Coroutine, Animator)> badgeCellSpeech = new Dictionary<string, (Coroutine, Animator)>();

        private const StringTable.StringTableType GLOBAL = StringTable.StringTableType.Global;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                CloseBadgesSpeechBallon();
            }
        }

        public void InitProperty()
        {
            if (isInit) return;

            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            if (gs_managerObj == null)
                gs_managerObj = MetaObjectUtils.MakePrefab(CONTENTS_BUNDLE, "VIP Lounge Contents Sounds", transform);

            StartBGM();

            // Play Anim
            anim.SetBool("Active", true);

            vipLoungeInfo = VipLounge.Utils.VipLoungeInfo;

            // Init Elements
            InitSkeleton();
            InitBottomArea();
            InitCellArea();
            InitGrandJackpot();
            InitTimer();
            InitData();


            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);
            RegisterHandleEventType(MetaEventDefine.ON_CONTENT_EVENT);

            isInit = true;
            BlackboardUtils.SetOrCreateValue(bb, "_isInit", isInit);
            BlackboardUtils.SetOrCreateValue(bb, "_isPassive", isPassive);
        }

        // The skeleton is one-time initialize contexts like buttons, close event, etc... No need to initialize more than two times.
        private void InitSkeleton()
        {
            // Information
            var informationButtonElement = ContextUtils.FindElement(root, "Button Question", CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(informationButtonElement, "Text", "BUTTON_COMMON_INFORMATION", CHILDREN);
            MetaContextElementUtils.SetClickable(informationButtonElement,
                () => EventSender.SendEvent(gameObject, VipLounge.Events.ON_CLICK_INFORMATION));
#if DEV
            // todo : Test case only - Lounge Jackpot Open
            var loungeJackpotElement = ContextUtils.FindElement(root, "Badge Big Lounge Jackpot", CHILDREN);
            MetaContextElementUtils.SetClickable(loungeJackpotElement,
                () =>
                {
                    if (!VipLounge.Utils.IsEnded)
                        EventSender.SendEvent(gameObject, VipLounge.Events.ON_CLICK_JACKPOT);
                });
#endif
            // Earning Point Button
            var earningPointButtonElement = ContextUtils.FindElement(root, "Button Earning Point", CHILDREN);
            MetaContextElementUtils.SetClickable(earningPointButtonElement,
                () => EventSender.SendEvent(gameObject, VipLounge.Events.ON_CLICK_EARNING_POINT));

            // Close
            var closeButtonElement = ContextUtils.FindElement(root, "Button Close", CHILDREN);
            MetaContextElementUtils.SetClickable(closeButtonElement,
                () => EventSender.SendEvent(gameObject, VipLounge.Events.ON_CLOSE));

            // Back Button
            MetaSystem.SubscribeBackButton(gameObject.GetHashCode(),
                () => EventSender.SendEvent(gameObject, VipLounge.Events.ON_BACK_BUTTON));

            // Refresh logic when badge count decrease from 2 to 1
            if (MAX_BADGE_COUNT == VipLounge.Utils.BadgeCount)
            {
                var remainTime = VipLounge.Utils.BenefitEndTimestamp - TimeUtils.GetTimeStamp();
                var firstBadgeExpireTime = remainTime - VipLounge.Utils.LoungeOpenTimeMillisec;
                Invoke(nameof(Refresh), firstBadgeExpireTime / 1000f + 1); // Add 1 sec for safety expire refresh
            }
        }

        public void InitCellArea()
        {
            bool isEnded = VipLounge.Utils.IsEnded;

            // Big Badge Cell - Vegas Dreams Special
            ContextElement metaGameCell = ContextUtils.FindElement(root, "Badge Big 01 Meta Game", CHILDREN);
            Animator metaGameCellAnimator = metaGameCell.GetComponent<Animator>();
            metaGameCellAnimator.keepAnimatorControllerStateOnDisable = true;
            if (!BlackboardQueryUtils.IsVegasDreamsActive())
            {
                metaGameCellAnimator.SetBool("isNotOpen", true);
                metaGameCellAnimator.SetBool("isLocked", false);
            }
            else
            {
                metaGameCellAnimator.SetBool("isNotOpen", false);
                metaGameCellAnimator.SetBool("isLocked", isEnded);
            }
            MetaContextElementUtils.SetClickable(metaGameCell,
                () =>
                {
                    if (!VipLounge.Utils.IsEnded && BlackboardQueryUtils.IsVegasDreamsActive())
                        EventSender.SendEvent(gameObject, VipLounge.Events.ON_CLICK_BIG_META_GAME);
                });
            if (metaGameBadgeElement == null)
            {
                ContextElement metaGameBadgeAreaElement = ContextUtils.FindElement(metaGameCell, "Badge Area", CHILDREN);
                GameObject badgeObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Badge Big", metaGameBadgeAreaElement.transform);
                metaGameBadgeAreaElement.UpdateContext();
                metaGameBadgeElement = badgeObj.GetComponent<ContextElement>();
            }
            UpdateBadgeCount(metaGameBadgeElement, VegasDreams.VegasDreams.Utils.TotalDepotCount);

            // Small Badge Cell
            ContextElement smallBadgeAreaElement = ContextUtils.FindElement(root, "Small Badge Area", CHILDREN);
            // Level Up Faster
            ContextElement levelUpCell = ContextUtils.FindElement(smallBadgeAreaElement, "Badge Small Level Up Faster", CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(levelUpCell, "Base Active/Text", "VIP_LOUNGE_BADGE_BALLOON_TEXT_2", FULL);
            MetaContextElementUtils.SimpleSetTextGlobal(levelUpCell, "Base Locked/Text", "VIP_LOUNGE_BADGE_BALLOON_TEXT_1", FULL);
            Animator levelUpCellAnimator = levelUpCell.GetComponent<Animator>();
            levelUpCellAnimator.keepAnimatorControllerStateOnDisable = true;
            levelUpCellAnimator.SetBool("isLocked", isEnded);
            MetaContextElementUtils.SetClickable(levelUpCell,
                () => ActiveBadgeSpeechBalloon(LEVEL_UP));
            // Club Bonus Boosted
            ContextElement clubBonusCell = ContextUtils.FindElement(smallBadgeAreaElement, "Badge Small Club Bonus Boosted", CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(clubBonusCell, "Base Active/Text", "VIP_LOUNGE_BADGE_BALLOON_TEXT_6", FULL);
            MetaContextElementUtils.SimpleSetTextGlobal(clubBonusCell, "Base Locked/Text", "VIP_LOUNGE_BADGE_BALLOON_TEXT_1", FULL);
            Animator clubBonusCellAnimator = clubBonusCell.GetComponent<Animator>();
            clubBonusCellAnimator.keepAnimatorControllerStateOnDisable = true;
            clubBonusCellAnimator.SetBool("isLocked", isEnded);
            MetaContextElementUtils.SetClickable(clubBonusCell,
                () => ActiveBadgeSpeechBalloon(CLUB_BONUS_BOOSTED));
            // High Roller
            ContextElement highRollerCell = ContextUtils.FindElement(smallBadgeAreaElement, "Badge Small High Roller", CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(highRollerCell, "Base Active/Text", "VIP_LOUNGE_BADGE_BALLOON_TEXT_5", FULL);
            MetaContextElementUtils.SimpleSetTextGlobal(highRollerCell, "Base Locked/Text", "VIP_LOUNGE_BADGE_BALLOON_TEXT_1", FULL);
            Animator highRollerCellAnimator = highRollerCell.GetComponent<Animator>();
            highRollerCellAnimator.keepAnimatorControllerStateOnDisable = true;
            highRollerCellAnimator.SetBool("isLocked", isEnded);
            MetaContextElementUtils.SetClickable(highRollerCell,
                () => ActiveBadgeSpeechBalloon(HIGH_ROLLER));
            // Club Vegas Reward
            ContextElement rewardCell = ContextUtils.FindElement(smallBadgeAreaElement, "Badge Small Club Vegas Reward", CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(rewardCell, "Base Active/Text", "VIP_LOUNGE_BADGE_BALLOON_TEXT_4", FULL);
            MetaContextElementUtils.SimpleSetTextGlobal(rewardCell, "Base Locked/Text", "VIP_LOUNGE_BADGE_BALLOON_TEXT_1", FULL);
            Animator rewardCellAnimator = rewardCell.GetComponent<Animator>();
            rewardCellAnimator.keepAnimatorControllerStateOnDisable = true;
            rewardCellAnimator.SetBool("isLocked", isEnded);
            MetaContextElementUtils.SetClickable(rewardCell,
                () => ActiveBadgeSpeechBalloon(REWARD));
            // Lounge Jackpot
            ContextElement loungeJackpotCell = ContextUtils.FindElement(root, "Badge Big Lounge Jackpot", CHILDREN);
            Animator loungeJackpotAnimator = loungeJackpotCell.GetComponent<Animator>();
            loungeJackpotAnimator.keepAnimatorControllerStateOnDisable = true;
            UpdateJackpotIcon(loungeJackpotAnimator);
            MetaContextElementUtils.SimpleSetTextGlobal(loungeJackpotCell, "Text Subinfo", isEnded || VipLounge.Utils.IsLockLoungeJackpot ? "VIP_LOUNGE_JACKPOT_SUBINFO_LOCKED" : "VIP_LOUNGE_JACKPOT_SUBINFO_ACTIVE", CHILDREN);
            ContextElement loungeJackpotInfo = ContextUtils.FindElement(loungeJackpotCell, "Button Info", CHILDREN);
            Animator loungeJackpotInfoAnimator = loungeJackpotInfo.GetComponent<Animator>();
            if (!VipLounge.Utils.IsEnded)
            {
                MetaContextElementUtils.SimpleSetTextGlobal(loungeJackpotInfo, "Text", GetBalloonTableKey("VIP_LOUNGE_BADGE_BALLOON_TEXT_7"), CHILDREN);

                MetaContextElementUtils.SetClickable(loungeJackpotInfo,
                    () =>
                    {
                        if (!VipLounge.Utils.IsEnded)
                            ActiveBadgeSpeechBalloon(LOUNGE_JACKPOT_INFO);
                        else
                            loungeJackpotInfo.gameObject.SetActive(false);
                    });
                loungeJackpotInfo.gameObject.SetActive(true);
            }
            else
                loungeJackpotInfo.gameObject.SetActive(false);

            // Init Cell Speech Coroutine Controller
            badgeCellSpeech[META_GAME] = (null, metaGameCellAnimator);
            badgeCellSpeech[CLUB_BONUS_BOOSTED] = (null, clubBonusCellAnimator);
            badgeCellSpeech[LEVEL_UP] = (null, levelUpCellAnimator);
            badgeCellSpeech[REWARD] = (null, rewardCellAnimator);
            badgeCellSpeech[HIGH_ROLLER] = (null, highRollerCellAnimator);
            badgeCellSpeech[LOUNGE_JACKPOT_INFO] = (null, loungeJackpotInfoAnimator);
        }

        public void InitGrandJackpot()
        {
            ContextElement loungeJackpotCell = ContextUtils.FindElement(root, "Badge Big Lounge Jackpot", CHILDREN);
            jackpotScoreTextElement = ContextUtils.FindElement(loungeJackpotCell, "Text Score", CHILDREN);
            jackpotChase = jackpotScoreTextElement.GetComponent<ChaseTypeLong>();
            if (jackpotChase == null)
                jackpotChase = jackpotScoreTextElement.gameObject.AddComponent<ChaseTypeLong>();
            Blackboard jackpotInfo = GetGrandJackpotInfoBB();
            jackpotChase.SetNonstopChase(
                jackpotScoreTextElement,
                jackpotInfo.GetValue<long>("prev"),
                jackpotInfo.GetValue<long>("current"),
                jackpotInfo.GetValue<int>("deltaMs"),
                null,
                StringTable.StringTableType.Global,
                false,
                NumberUtils.GetGlobalDenominator(),
                BlackboardUtils.GetOrCreateVariable<long>(jackpotInfo, "progress"));
        }

        // Bottom Gauge Areaa
        public void InitBottomArea()
        {
            // Gauge
            gaugeElement = ContextUtils.FindElement(root, "Gage Area/Progress Bar", FULL);
            gaugeTextElement = ContextUtils.FindElement(root, "Gage Area/Progress Bar/Fill Area/Text Gage Info", FULL);
            UpdateLoungePointGauge();

            // Max Badge Credit Change
            var gagueAnimator = ContextUtils.FindElement(root, "Gage Area", CHILDREN).GetComponent<Animator>();
            UpdateExtraLoungePoint();
            gagueAnimator.SetBool("isFullInfo", true);  // vip lounge 1.5 => all active

            // Badge Init
            for (int i = 1; i <= MAX_BADGE_COUNT; i++)
            {
                var badge = ContextUtils.FindElement(root, $"Gage Area/Diamond {i:00}", FULL);
                MetaContextElementUtils.SetActive(badge, false);
            }

            // Badge Active
            for (int i = 1; i <= VipLounge.Utils.BadgeCount; i++)
            {
                var badge = ContextUtils.FindElement(root, $"Gage Area/Diamond {i:00}", FULL);
                MetaContextElementUtils.SetActive(badge, true);
            }

            var timerElement = ContextUtils.FindElement(root, "Gage Area/Timer Base", FULL);
            MetaContextElementUtils.SetActive(timerElement, !VipLounge.Utils.IsEnded);

            // Speech Balloon
            var infoReadyBalloon = ContextUtils.FindElement(root, "Button Info/Speech Balloon 01", FULL);
            var infoActiveBalloon = ContextUtils.FindElement(root, "Button Info/Speech Balloon 02", FULL);
            MetaContextElementUtils.SetActive(infoReadyBalloon, VipLounge.Utils.IsEnded);
            MetaContextElementUtils.SetActive(infoActiveBalloon, !VipLounge.Utils.IsEnded);

            var infoReadyTextBalloon = ContextUtils.FindElement(root, "Button Info/Speech Balloon 01/Text", FULL);
            var infoActiveTextBalloon = ContextUtils.FindElement(root, "Button Info/Speech Balloon 02/Text", FULL);
            MetaContextElementUtils.SetTextGlobal(infoReadyTextBalloon, "VIP_LOUNGE_GAUGE_INFO_BUBBLE_READY_TEXT", VipLounge.Utils.MaxLoungePoint);
            MetaContextElementUtils.SetTextGlobal(infoActiveTextBalloon, "VIP_LOUNGE_GAUGE_INFO_BUBBLE_ACTIVE_TEXT");
        }

        private void InitTimer()
        {
            if (VipLounge.Utils.IsEnded)
                return;

            // Gauge Timer
            var badgeTimerElement = ContextUtils.FindElement(root, "Gage Area/Timer Base/Text Remain Time", FULL);
            var badgeTimer = GetRemainingTimerController(badgeTimerElement);
            badgeTimer.Init(badgeTimerElement, "TIME_FORMAT_HHMMSS", "TEXT_NORMAL", "", "Ended", true, Refresh);
            badgeTimer.StartTimer(VipLounge.Utils.BenefitEndTimestamp, 0);

            // Lounge Jackpot Timer
            var loungeJackpotElement = ContextUtils.FindElement(root, "Badge Big Lounge Jackpot", CHILDREN);
            var loungeJackpotTimerElement = ContextUtils.FindElement(loungeJackpotElement, "Remaining Timer", CHILDREN);
            var loungeJackpotTimer = GetRemainingTimerController(loungeJackpotTimerElement);
            var loungeJackpotTimerTextElement = ContextUtils.FindElement(loungeJackpotTimerElement, "Text", CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(loungeJackpotTimerElement, "Text Timer Title", "VIP_LOUNGE_JACKPOT_TIMER_TITLE", CHILDREN);
            loungeJackpotTimer.Init(loungeJackpotTimerTextElement, "TIME_FORMAT_HHMMSS", "TEXT_NORMAL", "", "Ended", true, () =>
            {
                EventSender.SendEvent(gameObject, VipLounge.Events.ON_JACKPOT_END_TIME);
            });
            loungeJackpotTimer.StartTimer(VipLounge.Utils.LoungeJackpotActiveTimestamp, 0);
            //// Info Timer
            //var infoTimerElement = ContextUtils.FindElement(root, "Button Info/Speech Balloon 02/Remaining Timer/Text", FULL);
            //var infoTimer = infoTimerElement.gameObject.AddComponent<RemainingTimerController>();
            //infoTimer.Init(infoTimerElement, "TIME_FORMAT_HHMMSS_TOTALHOUR", "TEXT_NORMAL", "", "Ended", false, null);
            //infoTimer.StartTimer(VipLounge.Utils.BenefitEndTimestamp, 0);
        }

        private void InitData()
        {
            loungeOpenCooltime = BlackboardQueryUtils.GetTimeStampToTimeString(VipLounge.Utils.LoungeOpenTimeMillisec);
            loungeJackpotCooltime = BlackboardQueryUtils.GetTimeStampToTimeString(VipLounge.Utils.LoungeJackpotTimeMillisec);

            EventInfo eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.VIP_LOUNGE);
            isPassive = eventInfo != null;
        }

        public void UpdateMetaData()
        {
            InitBottomArea();
            InitCellArea();
            InitTimer();
        }

        private void UpdateBadgeCount(ContextElement badgeElement, int count)
        {
            if (badgeElement == null)
                return;

            var depotAnimatorMaxValue = Mathf.Min(100, count);
            var depotBadgeTextString = Mathf.Min(99, count).ToString();
            if (count > 99) depotBadgeTextString += "+";

            MetaContextElementUtils.SetIntProperty(badgeElement, depotAnimatorMaxValue);
            MetaContextElementUtils.SimpleSetText(badgeElement, "Text", depotBadgeTextString, FULL);
        }

        private void UpdateLoungePointGauge()
        {
            if (MAX_BADGE_COUNT == VipLounge.Utils.BadgeCount)
            {
                MetaContextElementUtils.SetSliderValue(gaugeElement, 1f);
                MetaContextElementUtils.SetTextGlobal(gaugeTextElement, "VIP_LOUNGE_GAUGE_PROGRESS_JACKPOT_ACTIVATED");
            }
            else
            {
                var maxLoungePoint = VipLounge.Utils.MaxLoungePoint;
                var loungePoint = VipLounge.Utils.LoungePoint;
                float ratio = (float)loungePoint / maxLoungePoint;
                MetaContextElementUtils.SetSliderValue(gaugeElement, ratio);
                MetaContextElementUtils.SetTextGlobal(gaugeTextElement, "A_PER_B", loungePoint, maxLoungePoint);
            }
        }

        private void UpdateExtraLoungePoint()
        {
            //var multiplier = LevelUtils.GetLevelMultiplierNumeratorFromType("coin");
            //var exchangeCredit = NumberUtils.GetMultiplierNumeratorValue(excessLoungePoint, VipLounge.Utils.LoungePointCreditValue);
            //exchangeCredit = NumberUtils.GetMultiplierNumeratorValue(exchangeCredit, multiplier);
            var gagueFullInfoTextElement = ContextUtils.FindElement(root, "Gage Area/Text Full Info", FULL);
            MetaContextElementUtils.SetTextGlobal(gagueFullInfoTextElement, "VIP_LOUNGE_GAUGE_FULL_INFO_EXCHANGE_TEXT", VipLounge.Utils.ExcessLoungePoint);
        }

        private void UpdateJackpotIcon(Animator loungeJackpotAnimator)
        {
            loungeJackpotAnimator?.SetBool("isLocked", VipLounge.Utils.IsEnded || VipLounge.Utils.IsLockLoungeJackpot);
            loungeJackpotAnimator?.SetBool("Locked without Icon", VipLounge.Utils.IsLockLoungeJackpot);
        }

        private RemainingTimerController GetRemainingTimerController(ContextElement contextElement)
        {
            if (contextElement == null) return null;
            var remainingTimer = contextElement.gameObject.GetComponent<RemainingTimerController>();
            if (remainingTimer == null)
                remainingTimer = contextElement.gameObject.AddComponent<RemainingTimerController>();
            return remainingTimer;
        }

        private void Refresh()
        {
            Debug.Log("start [RequestVipLoungeEnter]");
            BagelCodeClientAPI.RequestVipLoungeEnter(
                (response) =>
                {
                    Debug.Log("succeed [RequestVipLoungeEnter]");

                    var bb = MainBlackboard.Get();
                    ClientAPI2Blackboard.Serialize(bb, response);
                    UpdateMetaData();
                },
                (error) =>
                {
                    Debug.LogError(error.errorCode);
                    GlobalErrorHandler.GlobalError(error);
                });
        }

        public IEnumerator CheckFirstEnterCoroutine()
        {
            // First Enter?
            bool isFirstEnter = PlayerPrefs.GetInt(PLAYER_PREFS_IS_FIRST_ENTER, 1) == 1;
            if (isFirstEnter)
            {
                PlayerPrefs.SetInt(PLAYER_PREFS_IS_FIRST_ENTER, 0);
                yield return StartCoroutine(OpenInformationPopupCoroutine(true));
            }
        }

        public IEnumerator OpenInformationPopupCoroutine(bool isAuto = false)
        {
            string lobbyBundle = CONTENTS_BUNDLE;

            string asset = "Popup VIP Epic Lounge Information Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject popupObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(lobbyBundle, asset, parent,
                (SceneLoadOperation sceneLoadOperation) => popupObj = sceneLoadOperation.GetScene()));

            var popupBB = popupObj.GetComponent<Blackboard>();

            string contextID = BlackboardUtils.GetOrCreateVariable<string>(bb, "contextID")?.value;
            BlackboardUtils.SetOrCreateValue(popupBB, "contextID", contextID);
            BlackboardUtils.SetOrCreateValue(popupBB, "openCooltime", loungeOpenCooltime);
            BlackboardUtils.SetOrCreateValue(popupBB, "jackpotCooltime", loungeJackpotCooltime);

            MetaObjectUtils.SetCalleeCaller(popupObj, gameObject);
            MetaPopupUtils.OpenPopup(popupObj);

            var callbackTrigger = new EventTrigger(gameObject, EventSender.ON_CALLEE_CALLBACK);
            yield return new WaitUntilTrigger(callbackTrigger);
        }

        public IEnumerator OpenEarningPointPopupCoroutine()
        {
            string lobbyBundle = CONTENTS_BUNDLE;

            string asset = "VIP Lounge Popup Earning Point Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject popupObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(lobbyBundle, asset, parent,
                (SceneLoadOperation sceneLoadOperation) => popupObj = sceneLoadOperation.GetScene()));

            var popupBB = popupObj.GetComponent<Blackboard>();

            string contextID = BlackboardUtils.GetOrCreateVariable<string>(bb, "contextID")?.value;
            BlackboardUtils.SetOrCreateValue(popupBB, "contextID", contextID);
            BlackboardUtils.SetOrCreateValue(popupBB, "mainBB", bb);

            MetaObjectUtils.SetCalleeCaller(popupObj, gameObject);
            MetaPopupUtils.OpenPopup(popupObj);
        }

        public IEnumerator OpenSpinRewardPopupCoroutine()
        {
            string lobbyBundle = CONTENTS_BUNDLE;

            string asset = "Popup VIP Lounge Spin Rewards Shop Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject popupObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(lobbyBundle, asset, parent,
                (SceneLoadOperation sceneLoadOperation) => popupObj = sceneLoadOperation.GetScene()));

            MetaObjectUtils.SetCalleeCaller(popupObj, gameObject);
            MetaPopupUtils.OpenPopup(popupObj);
            var callbackTrigger = new EventTrigger(gameObject, EventSender.ON_CALLEE_CALLBACK);
            yield return new WaitUntilTrigger(callbackTrigger);
        }

        public IEnumerator OpenJackpotPopupCoroutine()
        {
            string jackpotReadyAsset = "Popup Vip Lounge Jackpot Badge Ready Scene";
            string jackpotMainAsset = "Popup VIP Lounge Jackpot Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject popupObj = null;
            // Ready
            yield return StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(CONTENTS_BUNDLE, jackpotReadyAsset, parent,
                (SceneLoadOperation sceneLoadOperation) => popupObj = sceneLoadOperation.GetScene()));
            MetaPopupUtils.OpenPopup(popupObj);
            StopBGM();
            GSManager.Instance.GetHandler(LOUNGE_JACKPOT_ENTER).Play();
#if DEV
            // todo : Test case only - Lounge Jackpot Open
            if (!(bb.GetVariable<bool>("_isTest")?.value ?? false))
            {
                UpdateJackpotIcon(ContextUtils.FindElement(root, "Badge Big Lounge Jackpot", CHILDREN)?.GetComponent<Animator>());
            }
#else
            UpdateJackpotIcon(ContextUtils.FindElement(root, "Badge Big Lounge Jackpot", CHILDREN)?.GetComponent<Animator>());
#endif
            yield return new WaitForSeconds(3.0f);
            MetaPopupUtils.ClosePopup(popupObj);
            popupObj = null;
            // Jackpot Main
            yield return StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(CONTENTS_BUNDLE, jackpotMainAsset, parent,
                (SceneLoadOperation sceneLoadOperation) => popupObj = sceneLoadOperation.GetScene()));

            Blackboard popupBB = popupObj.GetComponent<Blackboard>();
            MetaObjectUtils.SetCalleeCaller(popupObj, gameObject);
            BlackboardUtils.SetOrCreateValue<Blackboard>(popupBB, "mainBB", bb);
            MetaPopupUtils.OpenPopup(popupObj);
            GSManager.Instance.GetHandler(LOUNGE_JACKPOT_MAIN_BGM).Play();

            PopupVipLoungeJackpotController popupController = popupObj.GetComponent<PopupVipLoungeJackpotController>();
            if (popupController != null)
            {
                var callbackTrigger = new EventTrigger(gameObject, EventSender.ON_CALLEE_CALLBACK);
                yield return new WaitUntilTrigger(callbackTrigger);
                UpdateExtraLoungePoint();
                yield return StartCoroutine(RequestVipLoungeJackpotInfo());
            }
            GSManager.Instance.GetHandler(LOUNGE_JACKPOT_MAIN_BGM).Stop();
            StartBGM();
            MetaPopupUtils.ClosePopup(popupObj);
        }

        public IEnumerator EnterVDSGame()
        {
            string contextID = BlackboardUtils.GetOrCreateVariable<string>(bb, "contextID")?.value;
            //MetaAssetBundleUtils.SendMetaGameLoadingBIEvent(contextID, "ui_click");

            string bundle = VegasDreams.VegasDreams.Defines.COMMON_BUNDLE;
            string asset = "Vegas Dreams Loading Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject loadingObj = null;
            yield return StartCoroutine(MetaObjectUtils.MakeSceneCoroutine(
                bundle, asset, parent, (GameObject sceneObj) => loadingObj = sceneObj));

            MetaPopupUtils.OpenPopup(loadingObj);

            Blackboard loadingBB = loadingObj.GetComponent<Blackboard>();
            loadingBB.AddVariable("_biContextID", contextID);
            loadingBB.AddVariable("isEnter", true);
            loadingBB.AddVariable("isMetaInGame", true);
            loadingBB.AddVariable("enter_type", "meta_icon");

            var prevOrientation = BlackboardUtils.FindVariable<Orientation>(bb, "prevOrientation");
            if (prevOrientation != null)
                BlackboardUtils.SetOrCreateValue(loadingBB, "prevOrientation", prevOrientation.value);

            StopBGM();

            anim.SetBool("Active", false);
        }

        public IEnumerator CloseCoroutine()
        {
            // Make Loading Scene
            string bundle = COMMON_BUNDLE;
            string asset = "VIP Epic Lounge Loading Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject loadingObj = null;
            yield return StartCoroutine(MetaObjectUtils.MakeSceneCoroutine(
                bundle, asset, parent, (GameObject sceneObj) => loadingObj = sceneObj));

            MetaPopupUtils.OpenPopup(loadingObj);

            Blackboard loadingBB = loadingObj.GetComponent<Blackboard>();
            BlackboardUtils.SetOrCreateValue(loadingBB, "isEnter", false);
            BlackboardUtils.SetOrCreateValue(loadingBB, "isMetaInGame", false);

            var prevOrientation = BlackboardUtils.FindVariable<Orientation>(bb, "prevOrientation");
            if (prevOrientation != null)
                BlackboardUtils.SetOrCreateValue(loadingBB, "prevOrientation", prevOrientation.value);

            StopBGM();
        }

        public IEnumerator OnBackButtonCoroutine()
        {
            var okButtonTrigger = new EventTrigger(gameObject, VipLounge.Events.ON_OK_BUTTON);

            // OK Popup
            yield return StartCoroutine(OpenCommonMessagePopupCoroutine("VIP_LOUNGE_POPUP_LEAVE", true));

            EventSender.SendEvent(gameObject,
                okButtonTrigger.IsTrigger ?
                VipLounge.Events.ON_CLOSE :
                VipLounge.Events.ON_RETURN);

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame(); // wait for exit action in fsm
        }

        public void Close(bool instantly)
        {
            MetaSystem.UnSubscribeBackButton(gameObject.GetHashCode());
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

        public void StartBGM()
        {
            // Play BGM
            GSManager.Instance.GetHandler(VIP_LOUNGE_MAIN_BGM).Play();
        }

        public void StopBGM()
        {
            // Stop BGM
            GSManager.Instance.GetHandler(VIP_LOUNGE_MAIN_BGM).Stop();
        }


        private IEnumerator OpenCommonMessagePopupCoroutine(string messageKey, bool isBack = false)
        {
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject messageObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenOKPopupCoroutine(parent,
                (GameObject popupObj) => messageObj = popupObj));

            string message = StringTableUtils.GetString(GLOBAL, messageKey);
            string ok = StringTableUtils.GetString(GLOBAL, "BUTTON_OKAY");

            string okEvent = isBack ? VipLounge.Events.ON_OK_BUTTON : "";

            MetaPopupUtils.SetCommonPopupData(messageObj, transform, message, "", okEvent, ok, "", "", "", "",
                true, false, true, true, true);

            MetaObjectUtils.SetCalleeCaller(messageObj, gameObject);

            MetaPopupUtils.OpenPopup(messageObj);

            var callbackTrigger = new EventTrigger(gameObject, EventSender.ON_CALLEE_CALLBACK);
            yield return new WaitUntilTrigger(callbackTrigger);
        }

        public void ActiveInfoSpeechBalloon()
        {
            //if (infoSpeechBalloonEnumerator != null)
            //    StopCoroutine(infoSpeechBalloonEnumerator);
            //infoSpeechBalloonEnumerator = StartCoroutine(InfoSpeechBalloonEnumerator());
        }

        private void CloseBadgesSpeechBallon()
        {
            foreach (var cell in badgeCellSpeech)
            {
                var (cellCoroutine, cellAnimator) = cell.Value;

                if (cellCoroutine != null)
                {
                    cellAnimator.SetBool("isSpeechBalloon", false);
                    StopCoroutine(cellCoroutine);
                }
            }
        }

        public void ActiveBadgeSpeechBalloon(string key)
        {
            CloseBadgesSpeechBallon();

            var (coroutine, animator) = badgeCellSpeech[key];

            coroutine = StartCoroutine(nameof(BadgeCellSpeechBalloonEnumerator), animator);
            badgeCellSpeech[key] = (coroutine, animator);
        }

        private string GetBalloonTableKey(string key)
        {
            if (VipLounge.Utils.IsEnded)
                return "VIP_LOUNGE_BADGE_BALLOON_TEXT_1";
            else
                return key;
        }

        //private IEnumerator InfoSpeechBalloonEnumerator()
        //{
        //    infoSpeechAnimator.SetBool("isSpeechBalloon", true);
        //    yield return new WaitForSeconds(3.0f);
        //    infoSpeechAnimator.SetBool("isSpeechBalloon", false);
        //}

        private IEnumerator BadgeCellSpeechBalloonEnumerator(Animator animator)
        {
            ContextElement animElement = animator.GetComponent<ContextElement>();
            ContextElement activeElement = null, lockedElement = null;
            bool isEnded = VipLounge.Utils.IsEnded;
            if (animElement != null)
            {
                activeElement = ContextUtils.FindElement(animElement, "Base Active", CHILDREN);
                lockedElement = ContextUtils.FindElement(animElement, "Base Locked", CHILDREN);
            }
            activeElement?.gameObject.SetActive(!isEnded);
            lockedElement?.gameObject.SetActive(isEnded);

            animator.SetBool("isSpeechBalloon", true);
            yield return new WaitForSeconds(3.0f);
            animator.SetBool("isSpeechBalloon", false);

            activeElement?.gameObject.SetActive(false);
            lockedElement?.gameObject.SetActive(false);
        }
        // Lounge Jackpot
        public bool CheckPopupLoungeJackpot()
        {
            return VipLounge.Utils.IsActiveJackpotPopup;
        }

        private Blackboard GetGrandJackpotInfoBB()
        {
            Blackboard grandJackpotInfoBB = bb.GetVariable<Blackboard>("grandJackpotInfo")?.value ?? null;
            if (grandJackpotInfoBB == null)
            {
                grandJackpotInfoBB = BlackboardUtils.GetOrCreateBlackboard(bb, "grandJackpotInfo") as Blackboard;
                SetIncreaseTime();
                UpdateGrandJackpotInfoValue(grandJackpotInfoBB);
            }
            return grandJackpotInfoBB;
        }

        private void UpdateGrandJackpotInfoValue(Blackboard grandJackpotInfoBB)
        {
            if (grandJackpotInfoBB == null)
                return;

            Blackboard loungeJackpotPresentationValues = GetLoungeJackpotPresentationValuesBB();
            List<Blackboard> jackpotTableList = VipLounge.Utils.LoungeJackpotTableList(bb);
            Blackboard grandJackpotTableBB = jackpotTableList.Find(b => b.GetValue<LoungeJackpotWinType>("winType") == LoungeJackpotWinType.GRAND);

            long baseWinCredit = VipLounge.Utils.LoungeJackpotBaseWinCredit(bb);
            long minMultiplierNumerator = grandJackpotTableBB.GetValue<long>("minMultiplierNumerator");
            long maxMultiplierNumerator = grandJackpotTableBB.GetValue<long>("maxMultiplierNumerator");
            long minValue = grandJackpotTableBB.GetValue<long>("minValue");
            long maxValue = NumberUtils.GetMultiplierNumeratorValue(minValue, (long)((double)maxMultiplierNumerator / minMultiplierNumerator * NumberUtils.GetGlobalDenominator()));
            long grandJackpotCommunityCredit = VipLounge.Utils.LoungeJackpotGrandJackpotCommunityCredit(bb);
            long minGrandJackpotValue = minValue + grandJackpotCommunityCredit;
            if (minGrandJackpotValue > maxValue)
                minGrandJackpotValue = maxValue;

            long minBaseCreditValue = VipLounge.Utils.GetLoungeJackpotMultiplierValue(baseWinCredit, minMultiplierNumerator);
            long currentCalcValue = minBaseCreditValue + grandJackpotCommunityCredit;
            long maxCapCredit = VipLounge.Utils.GetLoungeJackpotMultiplierValue(baseWinCredit, maxMultiplierNumerator);
            if (currentCalcValue > maxCapCredit)
                currentCalcValue = maxCapCredit;

            BlackboardUtils.SetOrCreateValue(bb, "_refreshTime", loungeJackpotPresentationValues.GetValue<long>("AFTER_ANIMATION_REFRESH_TIME_MILLISEC") / 1000);

            long current = System.Math.Max(currentCalcValue, minGrandJackpotValue);
            long prev = GetIncreaseTime() ? VipLounge.Utils.GetLoungeJackpotMultiplierValue(current, loungeJackpotPresentationValues.GetValue<long>("INCREASE_START_PERCENT")) : current;
            int deltaMs = GetLoungeJackpotChaseDeltaMS(prev, current, loungeJackpotPresentationValues.GetValue<long>("INCREASE_CREDIT_PER_SECOND"));

            BlackboardUtils.SetOrCreateValue(grandJackpotInfoBB, "prev", prev);
            BlackboardUtils.SetOrCreateValue(grandJackpotInfoBB, "current", current);
            BlackboardUtils.SetOrCreateValue(grandJackpotInfoBB, "deltaMs", deltaMs);
            BlackboardUtils.SetOrCreateValue(grandJackpotInfoBB, "min", System.Math.Max(minBaseCreditValue, minValue));
            BlackboardUtils.SetOrCreateValue(grandJackpotInfoBB, "max", System.Math.Max(maxCapCredit, maxValue));
            BlackboardUtils.SetOrCreateValue(grandJackpotInfoBB, "baseCredit", baseWinCredit);
        }

        private void SetIncreaseTime()
        {
            Blackboard loungeJackpotPresentationValues = GetLoungeJackpotPresentationValuesBB();

            long increaseResetCooltime = loungeJackpotPresentationValues.GetValue<long>("INCREASE_ANIMATION_RESET_TIME_MILLISEC");
            long increaseTime = PlayerPrefsUtils.GetInt64(PLAYER_PREFS_JACKPOT_INCREASE_TIME);
            long nowTimeStamp = TimeUtils.GetTimeStamp();

            bool isIncreaseTime = increaseTime + increaseResetCooltime < nowTimeStamp;
            if (isIncreaseTime == true)
                PlayerPrefsUtils.SetInt64(PLAYER_PREFS_JACKPOT_INCREASE_TIME, nowTimeStamp);

            BlackboardUtils.SetOrCreateValue(bb, "_isIncreaseTime", isIncreaseTime);
        }

        private bool GetIncreaseTime()
        {
            return bb.GetVariable<bool>("_isIncreaseTime")?.value ?? false;
        }

        private Blackboard GetLoungeJackpotPresentationValuesBB()
        {
            return BlackboardUtils.FindValue<Blackboard>(bb, "loungeJackpotPresentationValues");
        }

        private int GetLoungeJackpotChaseDeltaMS(long prev, long current, long increaseCreditPerSecond)
        {
            long calcDeltaMS = (current - prev) * 1000 / increaseCreditPerSecond;
            return ((long)int.MaxValue < calcDeltaMS) ? int.MaxValue : (int)calcDeltaMS;
        }

        public void CheckVipLoungeActive()
        {
            if (!BlackboardQueryUtils.IsVipLoungeEnabled())
            {
                EventSender.SendEvent(gameObject, VipLounge.Events.ON_ENDED_TIME);
            }
        }

        public void CheckVipLoungeEndTime()
        {
            if (BlackboardQueryUtils.IsVipLoungeEndedTimestamp())
            {
                EventSender.SendEvent(gameObject, VipLounge.Events.ON_ENDED_TIME);
            }
        }

        public void OnCloseDailySpin(GameObject popupObject)
        {
            if (popupObject != null)
            {
                if (popupObject.GetComponent<GraphOwner>() != null)
                    MetaSystem.UnSubscribeBackButton(popupObject.GetComponent<GraphOwner>().GetHashCode());
                popupObject.GetComponent<Animator>()?.SetTrigger("Close");
            }
        }

        public IEnumerator OnEndedTimeCoroutine()
        {
            // OK Popup
            yield return StartCoroutine(OpenCommonMessagePopupCoroutine("VIP_LOUNGE_POPUP_ENDED", true));
        }

        public IEnumerator OnTriggerIAM(InAppMessageTriggerType triggerType)
        {
            if (IAMRouter.Instance.TriggerIAM(triggerType, gameObject, BlackboardUtils.GetOrCreateVariable<string>(bb, "contextID")?.value))
            {
                // OnIAMCallback
                var iamCallbacklTrigger = new EventTrigger(gameObject, "OnIAMCallback");
                yield return new WaitUntilTrigger(iamCallbacklTrigger);
            }
        }

        // API Update (RequestVipLoungeJackpotInfo)
        private void UpdateGrandJackpotInfoBB()
        {
            if (bb == null)
                return;

            bool isIncreaseTime = GetIncreaseTime();
            Blackboard grandJackpotInfoBB = GetGrandJackpotInfoBB();
            long oldPrev = grandJackpotInfoBB.GetVariable<long>("prev")?.value ?? 0L;
            long oldCurrent = grandJackpotInfoBB.GetVariable<long>("current")?.value ?? 0L;
            long oldBaseCredit = grandJackpotInfoBB.GetVariable<long>("baseCredit")?.value ?? 0L;

            UpdateGrandJackpotInfoValue(grandJackpotInfoBB);

            long nowCurrent = grandJackpotInfoBB.GetVariable<long>("current")?.value ?? 0L;
            if (oldCurrent != nowCurrent)
            {
                int deltaMs = GetLoungeJackpotChaseDeltaMS(oldPrev, nowCurrent, GetLoungeJackpotPresentationValuesBB()?.GetValue<long>("INCREASE_CREDIT_PER_SECOND") ?? 50);
                long nowPrev = grandJackpotInfoBB.GetVariable<long>("prev")?.value ?? 0L;
                Variable<long> progress = BlackboardUtils.GetOrCreateVariable<long>(grandJackpotInfoBB, "progress");
                if (progress.value < 0L)
                    progress.value = 0L;

                if (oldCurrent > nowCurrent)
                {
                    oldPrev = nowPrev;
                    progress.value = 0L;
                }
                else if (nowPrev > progress.value && oldBaseCredit != grandJackpotInfoBB.GetVariable<long>("baseCredit").value)
                    progress.value = nowPrev;

                jackpotChase.SetNonstopChase(
                    jackpotScoreTextElement,
                    oldPrev,
                    nowCurrent,
                    deltaMs,
                    null,
                    StringTable.StringTableType.Global,
                    false,
                    NumberUtils.GetGlobalDenominator(),
                    progress);
            }
            if (isIncreaseTime == false)
                MetaContextElementUtils.SetTextGlobal(jackpotScoreTextElement, "TEXT_COMMA_NUMBER", nowCurrent);
        }

        public IEnumerator RequestVipLoungeJackpotSpin()
        {
            bool success = false;
            bool fail = false;
            string contextId = BlackboardUtils.GetOrCreateVariable<string>(bb, "contextID")?.value;
#if DEV
            LoungeJackpotWinType winType = LoungeJackpotWinType.UNKNOWN;
            if (BlackboardUtils.FindVariable<LoungeJackpotWinType>(MainBlackboard.Get(), LOUNGE_JACKPOT_DEBUG_SPIN) != null)
                winType = BlackboardUtils.GetOrCreateVariable<LoungeJackpotWinType>(MainBlackboard.Get(), LOUNGE_JACKPOT_DEBUG_SPIN)?.value ?? LoungeJackpotWinType.UNKNOWN;
            BlackboardUtils.SetOrCreateValue(MainBlackboard.Get(), LOUNGE_JACKPOT_DEBUG_SPIN, LoungeJackpotWinType.UNKNOWN);
            BagelCodeClientAPI.RequestVipLoungeJackpotDebugSpin(winType, contextId,
#else
            BagelCodeClientAPI.RequestVipLoungeJackpotSpin(contextId,
#endif
                (response) =>
                {
                    success = true;
                    BlackboardQueryUtils.UpdateResponseVIPLounge(response.error, response.common, response.serverTime, response.vipLoungeInfo);
                    ClientAPI2Blackboard.Serialize(bb, response);
                },
                (error) =>
                {
                    fail = true;
                    GlobalErrorHandler.GlobalError(error);
                });
            yield return new WaitUntil(() => (success || fail));
            if (success)
                yield return StartCoroutine(OpenJackpotPopupCoroutine());
        }

        public IEnumerator RequestVipLoungeJackpotInfo()
        {
            bool success = false;
            bool fail = false;

            BagelCodeClientAPI.RequestVipLoungeJackpotInfo(
                (response) =>
                {
                    ClientAPI2Blackboard.Serialize(bb, response);
                    UpdateGrandJackpotInfoBB();
                    success = true;
                },
                (error) =>
                {
                    fail = true;
                    GlobalErrorHandler.GlobalError(error);
                });

            yield return new WaitUntil(() => (success || fail));
        }
    }
}
