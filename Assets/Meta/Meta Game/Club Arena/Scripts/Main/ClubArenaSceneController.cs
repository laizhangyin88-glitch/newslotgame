using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;
using ParadoxNotion;
using NodeCanvas.Framework;
using Sirenix.OdinInspector;

namespace BagelCode.ClubArena
{
    public class ClubArenaSceneController : EventMonoBehaviour
    {
        public float effectMovementTime = 0.75f;
        public float increaseEffectTime = 1f;
        public float wheelResulWaitTime = 0.5f;
        public int betMultiplyIndex = 0;
        public int revengeIndex = -1;

        public string leaveType = "close";

        public bool isAutoSpin = false;
        public bool isReadyToAds = true;
        public bool isRemovedFromClub = false;

        private SceneLoadOperation sceneOperation;

        private ContextElement rootElement;
        private Blackboard rootBB;
        private Animator rootAnimator;
        private Animator backgroundAnimator;

        private ContextElement eventTimerElement;
        private ContextElement remainingTimerElement;

        // button
        private ContextElement buttonCloseElement;
        private ContextElement buttonInfoElement;

        // battle
        private ClubArenaBattleController battleController;

        // wheel
        private ClubArenaWheelController wheelController;

        private RemainingTimerController timerController;

        // club
        private ClubArenaSceneClubRankController clubRankController;
        private ClubArenaSceneClubLeadersController clubLeadersController;

        private Transform popupAreaTransform = null;

        private bool isInit = false;

        private const int INFO_PAGE_COUNT = 4;
        private const int USER_LEADERS_COUNT = 3;

        private const string CLUB_ARENA_COMMON_BUNDLE = "mgclubarenacommon";
        private const string CLUB_ARENA_CONTENT_BUNDLE = "mgclubarenacontents";

        private const string INFO_SCENE_ASSET_NAME = "Popup Information Scene";
        private const string INFO_TEXT = "CLUB_ARENA_INFORMATION_TEXT_{0}";
        private const string INFO_PAGE_FORMAT = "Information Page {0:00}";

        private void InitProperty()
        {
            if (isInit) return;

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);
            rootBB = gameObject.GetComponent<Blackboard>();
            rootAnimator = gameObject.GetComponent<Animator>();

            backgroundAnimator = ContextUtils.FindElement(rootElement, "Background", ContextSearchingType.ChildrenSearch)?.GetComponent<Animator>();

            // Timer
            eventTimerElement = ContextUtils.FindElement(rootElement, "Event Timer", ContextSearchingType.ChildrenSearch);
            remainingTimerElement = ContextUtils.FindElement(eventTimerElement, "Remaining Timer", ContextSearchingType.ChildrenSearch);
            ContextUtils.FindElement(remainingTimerElement, "Text", ContextSearchingType.ChildrenSearch).gameObject.SetActive(false);

            // Button
            buttonCloseElement = ContextUtils.FindElement(rootElement, "Button Close", ContextSearchingType.ChildrenSearch);
            buttonInfoElement = ContextUtils.FindElement(rootElement, "Button Question", ContextSearchingType.ChildrenSearch);

            // Battle
            ContextElement battleElement = ContextUtils.FindElement(rootElement, "Battle Area", ContextSearchingType.ChildrenSearch);
            battleController = battleElement.GetComponent<ClubArenaBattleController>();

            // Club Cell
            //InitClubCells();
            //InitClubReward();

            // Wheel
            ContextElement wheelAreaElement = ContextUtils.FindElement(rootElement, "Wheel Area", ContextSearchingType.ChildrenSearch);
            wheelController = wheelAreaElement.GetComponent<ClubArenaWheelController>();

            MetaContextElementUtils.SimpleSetTextGlobal(buttonInfoElement, "Text", "CLUB_ARENA_INFORMATION_BUTTON_TEXT", ContextSearchingType.ChildrenSearch);

            // Button Clickable Event
            MetaContextElementUtils.SetClickable(
                buttonCloseElement,
                "OnClose",
                rootElement,
                null
            );

            MetaContextElementUtils.SetClickable(
                buttonInfoElement,
                "OnClickInfo",
                rootElement,
                null
            );

            BlackboardUtils.SetOrCreateValue(remainingTimerElement.GetComponent<Blackboard>(), "caller", gameObject);

            popupAreaTransform = GameObject.Find("Popup Manager/Area").transform;

            GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_MAIN_BGM).Play();

            isInit = true;
        }

        private void InitBattleData()
        {
            battleController.OnInit(rootElement);
            battleController.OnCreateMyChest();
            battleController.SetCallback(GetBattleCallBack);
        }

        private void InitWheelData()
        {
            long defaultMultiply = ClubArenaUtils.GetDefaultBetMultiplyNumerator();
            ClubArenaUtils.CurrentBetMultiplyNumerator = defaultMultiply;

            GetCalcBetMultiplyIndex(defaultMultiply);

            wheelController?.OnInit(rootElement);
            wheelController?.SetAdsTimerCallback(OnAdsTimerCallback);
            wheelController?.SetCompletedRewardItem(GetRewardItemCompleted);
        }

        private void InitClubController()
        {
            clubRankController = new ClubArenaSceneClubRankController();
            clubRankController.OnInit(rootElement);

            clubLeadersController = new ClubArenaSceneClubLeadersController();
            clubLeadersController.OnInit(rootElement);
        }

        private void InitBlackboard()
        {
            BlackboardUtils.SetOrCreateValue(rootBB, "wheelAnimator", wheelController.GetAnimator());
            BlackboardUtils.SetOrCreateValue(rootBB, "wheelContextElement", wheelController.GetWheelElement());
            BlackboardUtils.SetOrCreateValue(rootBB, "highlightElement", wheelController.GetHighlightElement());
        }

        public void OnInit()
        {
            InitProperty();
            InitBattleData();
            InitWheelData();
            InitClubController();
            InitBlackboard();

            UpdateTimer();
            OnUpdateData();
        }

        public void OnCloseRewardPopup()
        {
            EventData eventData = new EventData(ClubArenaUtils.ON_REFRESH_EVENT);
            MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, eventData);
        }

        public void OnCloseToLoading()
        {
            GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_MAIN_BGM).Stop();
        }

        public void OnOpponentCreate()
        {
            SetMatchingAnimation(false);
            battleController.OnCreateOpponentChest();
        }

        public float OnOpponentMatching()
        {
            SetMatchingAnimation(true);
            return 1.0f;
        }

        public void OnSelectTarget(bool isActive)
        {
            if (isActive)
            {
                GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_REVENGE_KUDO).Play();
                MetaFeedUtils.SendClubArenaRevenge(StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_ARENA_KUDO_REVENGE_TEXT"));
            }
            SetTargetAnimation(isActive);
        }

        private void OnLoadInfoPopup(SceneLoadOperation _sceneOperation)
        {
            sceneOperation = _sceneOperation;

            GameObject popupGO = sceneOperation.GetScene();
            var infoBB = popupGO.GetComponent<Blackboard>();

            MetaPopupUtils.SetInformationPopupData(infoBB, INFO_PAGE_COUNT, CLUB_ARENA_CONTENT_BUNDLE, "Information Dots", INFO_PAGE_FORMAT, INFO_TEXT);

            string popupContextID = BiEventUtils.GenerateContextID();
            BlackboardUtils.SetOrCreateValue(infoBB, "caller", gameObject);
            BlackboardUtils.SetOrCreateValue(infoBB, "_biContextID", popupContextID);

            popupGO.SetActive(true);
            PopupManager.Instance.Open(popupGO);
            ClubArenaUtils.BIClientClubArenaPopup("meta_information", popupContextID);
        }

        private void OnLoadClubLeadersPopup(SceneLoadOperation _sceneOperation)
        {
            string popupContextID = BiEventUtils.GenerateContextID();
            GameObject popupGO = OnLoadPopup(_sceneOperation, popupContextID);

            ClubArenaPopupLeaders popupClubLeaders = popupGO.GetComponent<ClubArenaPopupLeaders>();
            popupClubLeaders.OnInit();
            popupClubLeaders.SetActive(true);

            ClubArenaUtils.BIClientClubArenaPopup("leaders", popupContextID);
        }

        private void OnLoadClubRankPopup(SceneLoadOperation _sceneOperation)
        {
            string popupContextID = BiEventUtils.GenerateContextID();
            GameObject popupGO = OnLoadPopup(_sceneOperation, popupContextID);

            ClubArenaPopupClubRanking popupClubRank = popupGO.GetComponent<ClubArenaPopupClubRanking>();
            popupClubRank.OnInit();
            popupClubRank.SetActive(true);

            ClubArenaUtils.BIClientClubArenaPopup("rewards_information", popupContextID);
        }

        private void OnLoadPointTakenPopup(SceneLoadOperation _sceneOperation)
        {
            string popupContextID = BiEventUtils.GenerateContextID();
            GameObject popupGO = OnLoadPopup(_sceneOperation, popupContextID);

            ClubArenaPopupAttackedController popupHelp = popupGO.GetComponent<ClubArenaPopupAttackedController>();
            popupHelp.OnInit(rootBB.GetValue<bool>("_isShowHelp"), popupContextID);
            popupHelp.SetActive(true);
        }

        private void OnLoadRevengeListPopup(SceneLoadOperation _sceneOperation)
        {
            string popupContextID = BiEventUtils.GenerateContextID();
            GameObject popupGO = OnLoadPopup(_sceneOperation, popupContextID);

            ClubArenaPopupRevengeController popupRevenge = popupGO.GetComponent<ClubArenaPopupRevengeController>();
            popupRevenge.OnInit();
            popupRevenge.SetActive(true);

            rootBB.SetValue("_revengeContextID", "");

            ClubArenaUtils.BIClientClubArenaPopup("revenge_list", popupContextID, popupRevenge.GetTargetUserIdList());
        }

        private void OnLoadClosePopup(SceneLoadOperation _sceneOperation)
        {
            sceneOperation = _sceneOperation;

            GameObject popupGO = sceneOperation.GetScene();
            Blackboard bb = popupGO.GetComponent<Blackboard>();

            bb.SetValue("owner", gameObject.transform);

            bb.SetValue("eventButtonYes", "OnCloseToLeave");
            bb.SetValue("eventButtonX", "OnCloseCancel");
            bb.SetValue("title", StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_ARENA_POPUP_LEAVE_TEXT"));
            bb.SetValue("buttonYesText", StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY"));
            bb.SetValue("autoCloseYes", true);
            bb.SetValue("autoCloseX", true);
            bb.SetValue("useCloseButton", true);

            PopupManager.Instance.Open(popupGO);
            popupGO.gameObject.SetActive(true);
        }

        private GameObject OnLoadPopup(SceneLoadOperation _sceneOperation, string popupContextID)
        {
            sceneOperation = _sceneOperation;

            GameObject popupGO = sceneOperation.GetScene();
            var popupBB = popupGO.GetComponent<Blackboard>();

            BlackboardUtils.SetOrCreateValue(popupBB, "caller", gameObject);
            BlackboardUtils.SetOrCreateValue(popupBB, "_biContextID", popupContextID);

            popupGO.SetActive(true);
            PopupManager.Instance.Open(popupGO);

            return popupGO;
        }

        public void OnClickOpponentTarget()
        {
            MetaContextElementUtils.SendEvent(rootElement, "OnClubArenaTargetOpponent", null, null);
            EventSender.SendGlobalEvent("OnSkip");
        }

        public void OnClickWheelBetButton()
        {
            // Bet change check
            List<long> betMultiplyNumeratorList = ClubArenaUtils.BetMultiplyNumeratorList;
            if (betMultiplyNumeratorList == null || betMultiplyNumeratorList.Count == 0)
            {
                betMultiplyIndex = 0;
                return;
            }

            if (betMultiplyIndex + 1 < betMultiplyNumeratorList.Count)
                ++betMultiplyIndex;
            else
                betMultiplyIndex = 0;
            ClubArenaUtils.PreviousBetMultiplyNumerator = ClubArenaUtils.CurrentBetMultiplyNumerator;
            ClubArenaUtils.CurrentBetMultiplyNumerator = betMultiplyNumeratorList[betMultiplyIndex];

            wheelController.ChangeMultiplier(ClubArenaUtils.CurrentBetMultiplyNumerator);
            GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_BET_BUTTON).Play();
        }

        public void OnUpdateData()
        {
            OnUpdateEnergyData();
            UpdateReadyToAds();
            ClubArenaUtils.UpdateBetMultiplyNumerator();
            wheelController?.UpdateWheelData(isReadyToAds);
        }

        public void SpinWheel(bool isSpin)
        {
            if (wheelController != null)
                wheelController.SpinWheel(isSpin);
        }

        public void UpdateAutoSpin(bool value)
        {
            isAutoSpin = value;
        }

        private void UpdateTimer()
        {
            EventInfo eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo();
            if (eventInfo != null)
                MetaGameUtils.UpdateMetaGameRemainingTimer(eventTimerElement, remainingTimerElement, eventInfo.endTimestamp);
        }

        private int GetCalcBetMultiplyIndex(long multi)
        {
            //betMultiplyIndex
            List<long> betMultiplyNumeratorList = ClubArenaUtils.BetMultiplyNumeratorList;
            if (betMultiplyNumeratorList == null || betMultiplyNumeratorList.Count == 0)
            {
                betMultiplyIndex = 0;
            }
            else
            {
                for (int i = 0; i < betMultiplyNumeratorList.Count; ++i)
                {
                    if (betMultiplyNumeratorList[i] == multi)
                    {
                        betMultiplyIndex = i;
                        break;
                    }
                }
            }
            return betMultiplyIndex;
        }

        public bool GetCheckResultAttack()
        {
            ClubArenaDebugSpinResultType resultType = ClubArenaUtils.GetResultDebugSpinType();
            return resultType == ClubArenaDebugSpinResultType.ATTACK || resultType == ClubArenaDebugSpinResultType.STEAL;
        }

        public bool GetCheckResultType()
        {
            bool isBonus = false;
            Blackboard bb = ClubArenaUtils.WheelResultInfo;
            if (bb != null)
            {
                ClubArenaSpinResultType resultType = bb.GetValue<ClubArenaSpinResultType>("type");
                isBonus = resultType == ClubArenaSpinResultType.BONUS;
                PlaySoundWheelResultType(resultType);
            }

            return isBonus;
        }

        public bool GetFirstVisit()
        {
            return PlayerPrefs.GetInt(ClubArenaUtils.CLUB_ARENA_FIRST_VISIT, 0) > 0;
        }

        public float GetResultAngle()
        {
            return wheelController.GetResultAngle();
        }

        public float GetWheelCurrentAngle(float addAngle = 0.0f)
        {
            return wheelController.GetWheelCurrentAngle(addAngle);
        }

        public int GetWheelCurrentAngleInteger(float currentAngle)
        {
            return wheelController.GetWheelCurrentAngleInteger(currentAngle);
        }

        private void GetRewardItemCompleted(ClubArenaSpinResultType resultType)
        {
            switch(resultType)
            {
                case ClubArenaSpinResultType.POINT_10:
                case ClubArenaSpinResultType.POINT_50:
                    GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_POINT_GET).Play();
                    battleController.OnUpdateMyReward(ClubArenaDebugSpinResultType.POINT);
                    OnUpdateMyChestLevel();
                    break;
                case ClubArenaSpinResultType.SHIELD:
                    GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_SHIELD_GET).Play();
                    battleController.OnUpdateMyReward(ClubArenaDebugSpinResultType.SHIELD);
                    Blackboard bb = ClubArenaUtils.WheelResultInfo;
                    if (bb.GetValue<long>("energy") > 0)
                        battleController.CreateRewardEnergyItem(CLUB_ARENA_COMMON_BUNDLE, "In Game Club Arena Energy", wheelController.GetWheelItemPoolTransform());
                    else
                        SetWaitTimeAndSendWheelResultEvent(wheelResulWaitTime);
                    break;
                case ClubArenaSpinResultType.ATTACK_10:
                case ClubArenaSpinResultType.ATTACK_20:
                case ClubArenaSpinResultType.ATTACK_50:
                case ClubArenaSpinResultType.ATTACK_100:
                    GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_POINT_GET).Play();
                    OnBattleOpponentAttackResult(ClubArenaUtils.GetWheelBaseCandidateData(ClubArenaUtils.GetResultSpinType()));
                    break;
                case ClubArenaSpinResultType.STEAL:
                    GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_POINT_GET).Play();
                    battleController.OnUpdateMyReward(ClubArenaDebugSpinResultType.STEAL);
                    OnBattleOpponentDamageNextStep();
                    break;
            }
        }

        private void GetBattleCallBack(string key)
        {
            // key value check
            if (string.IsNullOrEmpty(key)) return;
            if (ApplicationSettings.LogTest())
                Debug.Log("Main Scene : GetBattleCallBack - key = " + key);
            if (string.Equals(key, ClubArenaUtils.BATTLE_ANIMATION_REWARD_ENERGY))
            {
                // RewardEnergy : Energy chest -> spin button move completed
                OnUpdateEnergyData(true);
                SetWaitTimeAndSendWheelResultEvent(wheelResulWaitTime);
            }
            else if (string.Equals(key, ClubArenaUtils.BATTLE_ANIMATION_CHANGED_CHEST))
            {
                // ChestChanged : Chest changed completed
                SetWaitTimeAndSendWheelResultEvent(wheelResulWaitTime);
            }
            else if (string.Equals(key, ClubArenaUtils.BATTLE_ANIMATION_MY_CLUB))
            {
                if (ClubArenaUtils.OpponentHasShield && ClubArenaUtils.GetResultDebugSpinType() == ClubArenaDebugSpinResultType.ATTACK)
                {
                    GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_OPPONENT_WARRIOR).Play();
                    battleController.SetAnimation("opponentClub");
                }
                else
                    WheelResultAttackOrStealAction();
            }
            else if (string.Equals(key, ClubArenaUtils.BATTLE_ANIMATION_OPPONENT_CLUB))
            {
                WheelResultAttackOrStealAction();
            }
            else if (string.Equals(key, ClubArenaUtils.BATTLE_ANIMATION_DAMAGE))
            {
                if (!ClubArenaUtils.OpponentHasShield || ClubArenaUtils.GetResultDebugSpinType() == ClubArenaDebugSpinResultType.STEAL)
                {
                    battleController.SetAnimation("clubDamage");
                    battleController.UpdateOpponentChestInfo();
                }
            }
            else if (string.Equals(key, ClubArenaUtils.BATTLE_ANIMATION_STEAL))
            {
                // update chest -> check point (LocalFeed)
                WheelResultLocalFeed();
            }
            else if (string.Equals(key, ClubArenaUtils.BATTLE_ANIMATION_SHIELD))
            {
                // opponent shield animation end
                WheelResultLocalFeed();
                //long baseBet = ClubArenaUtils.GetWheelBaseCandidateData(ClubArenaUtils.GetResultSpinType());
                //long opponentShield = ClubArenaUtils.OpponentState.GetValue<long>("shield");
                //if (opponentShield - baseBet < 0)
                //    battleController.BattleOpponentHit();
                //else
                //    WheelResultLocalFeed();
            }
            else if (string.Equals(key, ClubArenaUtils.BATTLE_ANIMATION_HIT))
            {
                // opponent attack animation end -> LocalFeed Call
                WheelResultLocalFeed();
            }
            else if (string.Equals(key, ClubArenaUtils.BATTLE_ANIMATION_DEAD))
            {
                // opponent dead animation end
                OnCheckNewOpponent();
            }
            else if (string.Equals(key, ClubArenaUtils.OPPONENT_ANIMATION_APPEAR))
            {
                SetBackgroundAnimation();
            }
        }

        private void OnAdsTimerCallback()
        {
            wheelController.UpdateWheelData(isReadyToAds);
        }

        private void OnBattleOpponentAttackResult(long damage)
        {
            battleController.OnUpdateMyReward(ClubArenaDebugSpinResultType.ATTACK);
            OnBattleOpponentDamageNextStep();
        }

        private void OnBattleOpponentDamageNextStep()
        {
            if (OnCheckOpponentAlive())
                OnCheckNewOpponent();
            else
                battleController.BattleOpponentDead();
        }

        private void OnChangeOpponent()
        {
            battleController.SetChestActive(false, false);
            SetMatchingAnimation(true);
            StartCoroutine(ChangedOpponentMatching());
        }

        public void OnChangeRevengeOpponent()
        {
            ClubArenaUtils.RemoveRevengeList(revengeIndex);
            revengeIndex = -1;
            OnChangeOpponent();
            battleController.SetRevenge(true);
        }

        private void OnCheckNewOpponent()
        {
            var newOpponent = ClubArenaUtils.ClubArenaInfo.GetVariable<Blackboard>("newOpponentState");
            if (newOpponent == null || newOpponent.value == null)
            {
                SetWaitTimeAndSendWheelResultEvent(wheelResulWaitTime);
                return;
            }

            ClubArenaUtils.OpponentState = newOpponent.value;
            BlackboardUtils.SetOrCreateValue<long>(ClubArenaUtils.OpponentState, "basePoint", 0);
            BlackboardUtils.DestroyBlackboard(ClubArenaUtils.ClubArenaInfo, "newOpponentState");
            OnChangeOpponent();
        }

        private IEnumerator ChangedOpponentMatching()
        {
            yield return new WaitForSeconds(wheelResulWaitTime);
            SetMatchingAnimation(false);
            battleController.SetChestActive(false, true);
            OnUpdateMyChestLevel();
        }

        private bool OnCheckOpponentAlive()
        {
            return battleController.GetOpponentAlive();
        }

        public void OnCheckPointTaken()
        {
            Blackboard bb = ClubArenaUtils.PointTaken;
            string eventName = "OnClosePointTaken";
            int takenUsers = bb.GetValue<int>("takenUserCount");
            ClubArenaUtils.PointTakenCount();
            if (takenUsers > 0)
            {
                if (takenUsers < ClubArenaUtils.CLUB_ARENA_MAX_HELP_SHOW_USER)
                    eventName = "OnPopupNonHelp";
                else if (ClubArenaUtils.PointTakenCount() < ClubArenaUtils.CLUB_ARENA_MAX_HELP_COUNT)
                {
                    if (ClubArenaUtils.MyState.GetValue<int>("leftHelpPopupCount") > 0)
                        eventName = "OnPopupHelp";
                    else
                        eventName = "OnPopupNonHelp";
                }
                else
                    eventName = "OnPopupNonHelp";
            }
            MetaContextElementUtils.SendEvent(rootElement, eventName, null, null);
        }

        public void OnCheckWheelStateToEvent()
        {
            long currentEnergy = ClubArenaUtils.CurrentEnergy;
            string eventName = "";
            if (CheckSpinEnergy())
                eventName = "OnWheelSpinEvent";
            else if (currentEnergy >= ClubArenaUtils.RequiredEnergy)
            {
                wheelController?.SetAutoSpin(false);
                eventName = "OnWheelEnableEvent";
            }
            else
            {
                SetAdsValues();
                long endTimestamp = GetCheckTimeAddCoolTime(ClubArenaUtils.LastVideoAdsClaimTimestamp, ClubArenaUtils.ClubArenaCooltime);
                if (endTimestamp < 0)
                {
                    if (isAutoSpin)
                        eventName = "OnWheelAdsPopupEvent";
                    else if (isReadyToAds)
                        eventName = "OnWheelAdsEvent";
                    else
                        eventName = "OnWheelEnableEvent";
                }
                else
                    eventName = "OnWheelEnableEvent";

                wheelController?.SetAutoSpin(false);
            }
            MetaContextElementUtils.SendEvent(rootElement, eventName, null, null);
        }

        public void OnCheckLoadingScene(GameObject obj)
        {
            if (obj != null)
            {
                PopupManager.Instance.Close(obj);
                Destroy(obj);
            }
        }

        public void OnUpdateEnergyData(bool addBonusEnergy = false)
        {
            long updateEnergy = ClubArenaUtils.CurrentEnergy;
            if (addBonusEnergy)
            {
                long addEnergy = CheckRewardBonusEnergy();
                updateEnergy += addEnergy;
                ClubArenaUtils.CurrentEnergy = updateEnergy;
                wheelController?.SetAppearEnergy(addEnergy);
            }
            wheelController?.SetEnergy(updateEnergy);
        }

        private void OnUpdateMyChestLevel()
        {
            if (battleController.CheckChangedMyChestLevel())
                battleController.ChangedMyChest();  // My chest changed action -> finish GetBattleCallBack(ClubArenaUtils.BATTLE_ANIMATION_CHANGED_CHEST) Call
            else
                SetWaitTimeAndSendWheelResultEvent(wheelResulWaitTime);
        }

        public void OpenBonusPopup(GameObject obj)
        {
            ClubArenaPopupBonusController popupBonusController = obj.GetComponent<ClubArenaPopupBonusController>();
            popupBonusController.OnInit();
            obj.SetActive(true);
            popupBonusController.SetActiveTrigger();
        }

        public void OpenInfoPopup()
        {
            MetaPopupUtils.OpenPopupAsync(this, MetaStringDefine.LOBBY_BUNDLE_NAME, INFO_SCENE_ASSET_NAME, popupAreaTransform, OnLoadInfoPopup);
        }

        public void OpenClubLeadersPopup()
        {
            MetaPopupUtils.OpenPopupAsync(this, CLUB_ARENA_CONTENT_BUNDLE, "Popup Club Arena Leaders Scene", popupAreaTransform, OnLoadClubLeadersPopup);
        }

        public void OpenClubRankPopup()
        {
            MetaPopupUtils.OpenPopupAsync(this, CLUB_ARENA_CONTENT_BUNDLE, "Popup Club Arena Club Ranking Scene", popupAreaTransform, OnLoadClubRankPopup);
        }

        public void OpenPointTakenPopup()
        {
            MetaPopupUtils.OpenPopupAsync(this, CLUB_ARENA_CONTENT_BUNDLE, "Popup Club Arena Attacked Scene", popupAreaTransform, OnLoadPointTakenPopup);
        }

        public void OpenRevengeListPopup()
        {
            MetaPopupUtils.OpenPopupAsync(this, CLUB_ARENA_CONTENT_BUNDLE, "Popup Club Arena Revenge List Scene", popupAreaTransform, OnLoadRevengeListPopup);
        }

        public void OpenClubInfoPopup(long clubId)
        {
            var clubMemberObj = MetaObjectUtils.MakeScene("Popup Club Member Scene", PopupManager.Instance.transform.Find("Area"));
            var clubMemberScene = clubMemberObj.GetComponent<Blackboard>();
            PopupManager.Instance.Open(clubMemberObj);
            clubMemberScene.AddVariable("caller", gameObject);
            clubMemberScene.AddVariable("clubID", clubId);
        }

        public void OpenClubProfilePopup(string userId)
        {
            var profileObj = MetaObjectUtils.MakeScene("Profile Popup Scene", PopupManager.Instance.transform.Find("Area"));
            var profilePopupBB = profileObj.GetComponent<Blackboard>();
            profileObj.SetActive(false);

            profilePopupBB.AddVariable("caller", gameObject);
            profilePopupBB.SetValue("_userId", userId);
            profilePopupBB.SetValue("bi_fromType", "metaGame");

            if (userId == BlackboardQueryUtils.GetMyUserId())
                profilePopupBB.SetValue("isMe", true);

            PopupManager.Instance.Open(profileObj);
            profileObj.SetActive(true);
        }

        public void OpenClosePopup()
        {
            MetaPopupUtils.OpenPopupAsync(this, MetaStringDefine.LOBBY_BUNDLE_NAME, "Popup Common OK Scene", popupAreaTransform, OnLoadClosePopup);
        }

        public void OpenSpeechBalloon()
        {
            wheelController.OpenSpeechBalloon();
        }

        public bool CheckBonusEnergy()
        {
            long rewardAmount = CheckRewardBonusEnergy();
            if (rewardAmount > 0)
            {
                return true;
            }
            return false;
        }

        public bool CheckSpinEnergy()
        {
            long trySpinEnergy = NumberUtils.GetMultiplierNumeratorValue(ClubArenaUtils.RequiredEnergy, ClubArenaUtils.CurrentBetMultiplyNumerator);
            return ClubArenaUtils.CurrentEnergy >= trySpinEnergy;
        }

        public bool CheckRevengeList()
        {
            List<Blackboard> revengeList = ClubArenaUtils.RevengeList;
            if (revengeList == null || revengeList.Count < 1)
                return false;
            else
                return true;
        }

        private long CheckRewardBonusEnergy()
        {
            long resultAmount = 0;

            Blackboard bb = ClubArenaUtils.WheelResultInfo;
            if (bb == null) return resultAmount;

            ClubArenaDebugSpinResultType resultType = ClubArenaUtils.GetResultDebugSpinType();
            if (resultType == ClubArenaDebugSpinResultType.BONUS_ENERGY)
            {
                List<Blackboard> bonusInfoList = bb.GetValue<List<Blackboard>>("bonusInfoList");
                int bonusIndex = bb.GetValue<int>("bonusIndex");
                resultAmount = NumberUtils.GetMultiplierNumeratorValue(bonusInfoList[bonusIndex].GetValue<long>("amount"), ClubArenaUtils.CurrentBetMultiplyNumerator);
            }
            else if (resultType == ClubArenaDebugSpinResultType.SHIELD)
                resultAmount = (long)(bb.GetValue<long>("energy"));

            return resultAmount;
        }

        public float CloseBonusReward()
        {
            if (CheckBonusEnergy())
            {
                StartCoroutine(UpdateBonusEnergy());
                return 0.2f;
            }
            return 0.0f;
        }

        private IEnumerator UpdateBonusEnergy()
        {
            yield return new WaitForSeconds(0.2f);
            OnUpdateEnergyData(true);
        }

        private long GetCheckTimeAddCoolTime(long checkTimeStamp, long coolTimeStamp)
        {
            long returnValue = (checkTimeStamp + coolTimeStamp) - TimeUtils.GetTimeStamp();
            return returnValue;
        }

        private void SendEventCompletedWheelResult()
        {
            MetaContextElementUtils.SendEvent(rootElement, "OnCompletedWheelResult", null, null);
        }

        private void UpdateReadyToAds()
        {
            bool isReady = false;
            bool isInhouse = BlackboardUtils.FindValue<bool>(MainBlackboard.Get(), "values/misc/INHOUSE_ADS_ENABLED");
            bool isVideo = BlackboardUtils.FindValue<bool>(MainBlackboard.Get(), "values/misc/VIDEO_ADS_ENABLED");
            if (isInhouse)
            {
                isReady = IAMRouter.Instance.CheckTriggerIAM(InAppMessageTriggerType.INHOUSE_ADS_FOR_CLUB_ARENA);
            }
            else if (isVideo)
            {
                string placement = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "videoAdsPlacementNames/clubArena").value;
                isReady = !string.IsNullOrEmpty(placement) && VideoAdsController.Instance.IsVideoAdsAvailable(placement);
            }

            isReadyToAds = isReady;
        }

        private void SetAdsValues()
        {
            bool isInhouse = BlackboardUtils.FindValue<bool>(MainBlackboard.Get(), "values/misc/INHOUSE_ADS_ENABLED");
            bool isVideo = BlackboardUtils.FindValue<bool>(MainBlackboard.Get(), "values/misc/VIDEO_ADS_ENABLED");

            if (isInhouse)
            {
                if (IAMRouter.Instance.CheckTriggerIAM(InAppMessageTriggerType.INHOUSE_ADS_FOR_CLUB_ARENA))
                    rootBB.SetValue("placementKey", "");
            }
            else if (isVideo)
            {
                string placement = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "videoAdsPlacementNames/clubArena").value;
                if (!string.IsNullOrEmpty(placement) && VideoAdsController.Instance.IsVideoAdsAvailable(placement))
                    rootBB.SetValue("placementKey", placement);
            }
            else
                rootBB.SetValue("placementKey", "");
        }

        public void SetFirstVisit(bool isValue)
        {
            PlayerPrefs.SetInt(ClubArenaUtils.CLUB_ARENA_FIRST_VISIT, isValue ? 1 : 0);
        }

        private void SetMatchingAnimation(bool isActive)
        {
            if (rootAnimator != null)
                rootAnimator.SetBool("isMatching", isActive);
            if (isActive)
                GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_MATCH_START).Play();
        }

        private void SetTargetAnimation(bool isActive)
        {
            if (rootAnimator != null)
                rootAnimator.SetBool("isTarget", isActive);
        }

        private void SetBackgroundAnimation()
        {
            if (backgroundAnimator != null)
                backgroundAnimator.SetTrigger("isActive");
        }

        private void SetWaitTime(float waitTime, bool isAdd = false)
        {
            float time = waitTime;
            if (isAdd)
            {
                var waitBB = rootBB.GetVariable<float>("_waitTime");
                time += (waitBB != null) ? waitBB.value : 0.0f;
            }
            BlackboardUtils.SetOrCreateValue(rootBB, "_waitTime", time);
        }

        public void SetKudoActive(bool isActive)
        {
            ClubArenaUtils.SetKudoActive(isActive);
        }

        public void SetPopupOutOfEnergy(GameObject obj, string biType)
        {
            ClubArenaPopupOutOfEnergy popupController = obj.GetComponent<ClubArenaPopupOutOfEnergy>();
            if (popupController != null)
            {
                string popupContextID = BiEventUtils.GenerateContextID();
                popupController.OnInit();
                popupController.SetMessageText(StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_ARENA_POPUP_OUT_ENERGY_TEXT"));
                popupController.SetPurchaseButton(StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_ARENA_POPUP_OUT_ENERGY_SPIN_BUTTON_TEXT"));
                popupController.SetBIType(biType);
                popupController.SetPopupContextID(popupContextID);
                popupController.SetCloseActive(true);
                popupController.SetActive(true);

                ClubArenaUtils.BIClientClubArenaPopup(biType, popupContextID);
            }
        }

        public void SetPopupOutOfEnergyAds(GameObject obj, string biType)
        {
            ClubArenaPopupOutOfEnergy popupController = obj.GetComponent<ClubArenaPopupOutOfEnergy>();
            if (popupController != null)
            {
                string popupContextID = BiEventUtils.GenerateContextID();
                popupController.OnInit();
                popupController.SetMessageText(StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_ARENA_POPUP_OUT_ENERGY_TEXT"));
                popupController.SetPurchaseButton(StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_ARENA_POPUP_OUT_ENERGY_SPIN_BUTTON_TEXT"));
                popupController.SetAdButton(StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_ARENA_POPUP_OUT_ENERGY_AD_BUTTON_TEXT", ClubArenaUtils.RequiredEnergy));
                popupController.SetFreeEnergyText(StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_ARENA_POPUP_OUT_ENERGY_AD_FREE_ENERGY_TEXT", ClubArenaUtils.RequiredEnergy));
                popupController.SetBIType(biType);
                popupController.SetPopupContextID(popupContextID);
                popupController.SetCloseActive(true);
                popupController.SetActive(true);

                ClubArenaUtils.BIClientClubArenaPopup(biType, popupContextID);
            }
        }

        private void SetWaitTimeAndSendWheelResultEvent(float waitTime, bool isAdd = false)
        {
            SetWaitTime(waitTime, isAdd);
            SendEventCompletedWheelResult();
        }

        private void PlaySoundWheelResultType(ClubArenaSpinResultType resultType)
        {
            switch (resultType)
            {
                case ClubArenaSpinResultType.POINT_10:
                case ClubArenaSpinResultType.POINT_50:
                    //GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_GET_POINTS).Play();
                    break;
                case ClubArenaSpinResultType.ATTACK_10:
                case ClubArenaSpinResultType.ATTACK_20:
                case ClubArenaSpinResultType.ATTACK_50:
                case ClubArenaSpinResultType.ATTACK_100:
                    //GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_WHEEL_ATTACK).Play();
                    break;
                case ClubArenaSpinResultType.SHIELD:
                    //GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_WHEEL_SHIELD).Play();
                    break;
                case ClubArenaSpinResultType.STEAL:
                    //GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_WHEEL_STEAL).Play();
                    break;

            }
        }

        public void PlaySoundWheelSpin(bool isSpin)
        {
            // Big Wheel Segments tick Sound Playing
            if (isSpin) GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_WHEEL_SPIN).Play();
            else GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_WHEEL_STOP).Play();
        }

        public void PlaySoundWheelStart()
        {
            GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_WHEEL_START).Play();
        }

        private void UpdateClubText()
        {
            clubRankController.UpdateClubText();
        }

        public void WheelResultAttackAction()
        {
            switch (ClubArenaUtils.GetResultDebugSpinType())
            {
                case ClubArenaDebugSpinResultType.ATTACK:
                case ClubArenaDebugSpinResultType.STEAL:
                    GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_MY_WARRIOR).Play();
                    battleController.SetAnimation("myClub");
                    break;
            }
        }

        private void WheelResultAttackOrStealAction()
        {
            ClubArenaSpinResultType resultType = ClubArenaUtils.GetResultSpinType();
            switch (resultType)
            {
                case ClubArenaSpinResultType.ATTACK_10:
                case ClubArenaSpinResultType.ATTACK_20:
                case ClubArenaSpinResultType.ATTACK_50:
                case ClubArenaSpinResultType.ATTACK_100:
                    ClubArenaUtils.SetOpponentAttackDamage(ClubArenaUtils.GetWheelBaseCandidateData(ClubArenaUtils.GetResultSpinType()));
                    battleController.BattleAttack(ClubArenaUtils.GetBattleAnimationIndex());
                    break;
                case ClubArenaSpinResultType.STEAL:
                    ClubArenaUtils.SetOpponentStealDamage();
                    battleController.BattleSteal(ClubArenaUtils.GetBattleAnimationIndex());
                    break;
                default:
                    // todo : Action node end
                    break;
            }
        }

        public void WheelResultRewardItemAction(Transform t)
        {
            ClubArenaSpinResultType type = ClubArenaUtils.GetResultSpinType();
            switch (type)
            {
                case ClubArenaSpinResultType.POINT_10:
                    wheelController.CreateRewardItem(CLUB_ARENA_CONTENT_BUNDLE, "Wheel Club Arena Point Step 2", type,
                                                     wheelController.GetWheelItemPoolTransform(), battleController.GetMyChestTransform(), wheelController.GetWheelItemPoolTransform(), 5);
                    break;
                case ClubArenaSpinResultType.POINT_50:
                    wheelController.CreateRewardItem(CLUB_ARENA_CONTENT_BUNDLE, "Wheel Club Arena Point Step 3", type,
                                                     wheelController.GetWheelItemPoolTransform(), battleController.GetMyChestTransform(), wheelController.GetWheelItemPoolTransform(), 5);
                    break;
                case ClubArenaSpinResultType.SHIELD:
                    wheelController.CreateRewardItem(CLUB_ARENA_CONTENT_BUNDLE, "Wheel Club Arena Shield Step 3", type,
                                                     wheelController.GetWheelItemPoolTransform(), battleController.GetShieldTransform(), wheelController.GetWheelItemPoolTransform(), 5);
                    break;
                case ClubArenaSpinResultType.ATTACK_10:
                case ClubArenaSpinResultType.ATTACK_20:
                case ClubArenaSpinResultType.ATTACK_50:
                case ClubArenaSpinResultType.ATTACK_100:
                case ClubArenaSpinResultType.STEAL:
                    wheelController.CreateRewardItem(CLUB_ARENA_CONTENT_BUNDLE, "Wheel Club Arena Point Step 3", type,
                                                     t, battleController.GetMyChestTransform(), t.parent.parent, 9);
                    break;
            }
        }

        public void WheelResultLocalFeed()
        {
            switch(ClubArenaUtils.GetResultSpinType())
            {
                case ClubArenaSpinResultType.POINT_10:
                case ClubArenaSpinResultType.POINT_50:
                case ClubArenaSpinResultType.SHIELD:
                    MetaFeedUtils.SendClubArenaReward();
                    break;
                case ClubArenaSpinResultType.ATTACK_10:
                case ClubArenaSpinResultType.ATTACK_20:
                case ClubArenaSpinResultType.ATTACK_50:
                case ClubArenaSpinResultType.ATTACK_100:
                case ClubArenaSpinResultType.STEAL:
                    //battleController.UpdateOpponentChestInfo();
                    battleController.SetRevenge(false);
                    MetaFeedUtils.SendClubArenaReward();
                    break;
            }
        }

        public void WheelResultLocalFeedCallback(GameObject feedObject)
        {
            switch(ClubArenaUtils.GetResultDebugSpinType())
            {
                case ClubArenaDebugSpinResultType.ATTACK:
                case ClubArenaDebugSpinResultType.STEAL:
                    WheelResultRewardItemAction(feedObject.transform);
                    break;
            }
        }
        // AE 104~105
        public void BIClientClickClubArenaBetChange()
        {
            long prevBetMultiplier = (long)NumberUtils.GetMultiplierFromNumerator(ClubArenaUtils.PreviousBetMultiplyNumerator);
            long currentBetMultiplier = (long)NumberUtils.GetMultiplierFromNumerator(ClubArenaUtils.CurrentBetMultiplyNumerator);
            long requiredEnergy = ClubArenaUtils.RequiredEnergy;

            Dictionary<string, object> customData = new Dictionary<string, object>();
            customData["match_context_id"] = ClubArenaUtils.MatchContextID;
            customData["previous_bet_multiplier"] = prevBetMultiplier;
            customData["bet_multiplier"] = currentBetMultiplier;
            customData["previous_bet"] = prevBetMultiplier * requiredEnergy;
            customData["bet"] = currentBetMultiplier * requiredEnergy;
            Analytics.CustomEvent("client_click_club_arena_bet_change", customData);
        }

        public void BIClientClubArenaLeave(string biContextID)
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();
            customData["match_context_id"] = ClubArenaUtils.MatchContextID;
            customData["context_id"] = biContextID;
            customData["type"] = leaveType;
            Analytics.CustomEvent("client_club_arena_leave", customData);
        }

        public void BIClientClickClubArenaPopup(string popupType, string contextID)
        {
            ClubArenaUtils.BIClientClickClubArenaPopup(popupType, contextID);
        }
#if UNITY_EDITOR
        [Button]
        private void TestHelpPopup()
        {
            ClubArenaPointTaken clubArenaPointTaken = new ClubArenaPointTaken();
            clubArenaPointTaken.takenAmountSum = 100000;
            clubArenaPointTaken.takenUserCount = 5;
            clubArenaPointTaken.mostTakenUserId = ClubArenaUtils.MyUserId;
            clubArenaPointTaken.mostTakenUserName = "Test";
            clubArenaPointTaken.mostTakenUserProfileUrl = BlackboardUtils.FindVariable<string>(null, "/me/profileUrl").value;
            clubArenaPointTaken.mostTakenAmount = 10000;

            ClientAPI2Blackboard.Serialize(BlackboardUtils.GetOrCreateBlackboard(ClubArenaUtils.ClubArenaInfo, "pointTaken"), clubArenaPointTaken);
            BlackboardUtils.SetOrCreateValue<bool>(rootBB, "_isShowHelp", false);
            OpenPointTakenPopup();
        }
#endif
    }
}
