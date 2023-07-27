using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SlotMaker;
using BagelCode.ClientModels;
using ParadoxNotion;
using NodeCanvas.Framework;

namespace BagelCode.BossRaiders
{
    public class BossRaidersSceneController : MonoBehaviour
    {
        public float effectMovementTime = 0.75f;
        public float increaseEffectTime = 1f;
        public float clubDamageWaitTime = 0.0f;
        public float attackWaitTime = 0.5f;
        public float hitWaitTime = 0.25f;
        public int betMultiplyIndex = 0;

        public bool isAutoSpin = false;
        public bool isReadyToAds = true;
        
        public string bundleName = "";
        public string bundleCommonName = "";
        public string bundleContentsSharedName = "";
        public string bundleCharacterName = "";

        private GameObject gs_managerObj;

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
        private ContextElement buttonClubTap;

        // battle
        private Animator battleAnimator;
        private ContextElement characterAreaElement;
        private ContextElement monsterAreaElement;
        private ContextElement userDamageTextElement;
        private ContextElement clubDamageTextElement;

        // characters
        private BossRaidersCharacterCommonController characterController = null;
        private BossRaidersMonsterCommonController monsterController = null;
        //private ContextElement characterStartPosElement;
        //private ContextElement characterEndPosElement;

        private ContextElement clubRankTextElement;
        private ContextElement clubTapTextElement;
        // club rank list
        private ContextElement[] clubCellRankTextElements;
        private ContextElement[] clubCellPointTextElements;
        private ContextElement[] clubCellNameTextElements;
        private ContextElement[] clubCellSymbolAreaElements;
        private ContextElement[] clubCellMyClubOnElements;
        private ContextElement[] clubCellMyClubOffElements;
        private ContextElement[] clubCellIconRankAreaElements;
        private ContextElement[] clubCellIconRankTextElements;
        private ContextElement[][] clubCellIconRankElements;
        private GameObject[] clubCellSymbolObject;
        private Text[] clubCellNameTexts;
        private BossRaidersSceneClubRankData[] clubCellRankDatas;

        // club reward
        private BossRaidersSceneClubLeadersController clubLeadersController;

        // wheel
        private ContextElement wheelContextElement;
        private BossRaidersWheelControllerBase wheelController;

        private RemainingTimerController timerController;

        private bool isInit = false;
        private bool isUrgentBGM = false;

        private const int RANK_LIST_COUNT = 3;
        private const int RANK_ICON_COUNT = 3;
        private const int INFO_PAGE_COUNT = 4;
        private const int USER_LEADERS_COUNT = 3;

        private const string INFO_SCENE_ASSET_NAME = "Popup Information Scene";
        private const string INFO_TEXT = "BOSS_RAIDERS_INFORMATION_TEXT_{0}";
        private const string INFO_PAGE_FORMAT = "Information Page {0:00}";

        private void InitProperty()
        {
            if (isInit) return;

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);
            rootBB = gameObject.GetComponent<Blackboard>();
            rootAnimator = gameObject.GetComponent<Animator>();

            InitBundles();

            if (gs_managerObj == null)
                gs_managerObj = MetaObjectUtils.MakePrefab(bundleCharacterName, "Boss Raiders Contents Sounds", transform);

            backgroundAnimator = ContextUtils.FindElement(rootElement, "Background", ContextSearchingType.ChildrenSearch)?.GetComponent<Animator>();

            // Timer
            eventTimerElement = ContextUtils.FindElement(rootElement, "Event Timer", ContextSearchingType.ChildrenSearch);
            remainingTimerElement = ContextUtils.FindElement(eventTimerElement, "Remaining Timer", ContextSearchingType.ChildrenSearch);

            // Button
            buttonCloseElement = ContextUtils.FindElement(rootElement, "Button Close", ContextSearchingType.ChildrenSearch);
            buttonInfoElement = ContextUtils.FindElement(rootElement, "Button Question", ContextSearchingType.ChildrenSearch);
            buttonClubTap = ContextUtils.FindElement(rootElement, "Button Primary", ContextSearchingType.ChildrenSearch);

            // Battle
            ContextElement battleElement = ContextUtils.FindElement(rootElement, "Battle Area", ContextSearchingType.ChildrenSearch);
            battleAnimator = battleElement.GetComponent<Animator>();
            characterAreaElement = ContextUtils.FindElement(battleElement, "Character Area", ContextSearchingType.ChildrenSearch);
            monsterAreaElement = ContextUtils.FindElement(battleElement, "Monster Area", ContextSearchingType.ChildrenSearch);

            ContextElement damageAreaElement = ContextUtils.FindElement(battleElement, "Damage Area", ContextSearchingType.ChildrenSearch);
            ContextElement userDamageAreaElement = ContextUtils.FindElement(damageAreaElement, "User Damage", ContextSearchingType.ChildrenSearch);
            userDamageTextElement = ContextUtils.FindElement(userDamageAreaElement, "Text", ContextSearchingType.ChildrenSearch);
            ContextElement clubDamageAreaElement = ContextUtils.FindElement(damageAreaElement, "User Club", ContextSearchingType.ChildrenSearch);
            clubDamageTextElement = ContextUtils.FindElement(clubDamageAreaElement, "Text", ContextSearchingType.ChildrenSearch);

            //characterStartPosElement = ContextUtils.FindElement(characterAreaElement, "Start Position", ContextSearchingType.ChildrenSearch);
            //characterEndPosElement = ContextUtils.FindElement(characterAreaElement, "End Position", ContextSearchingType.ChildrenSearch);

            // Club Cell
            InitClubCells();

            // Club Text element
            ContextElement myClubElement = ContextUtils.FindElement(rootElement, "My Club", ContextSearchingType.ChildrenSearch);
            clubRankTextElement = ContextUtils.FindElement(myClubElement, "Text Rank", ContextSearchingType.ChildrenSearch);
            clubTapTextElement = ContextUtils.FindElement(buttonClubTap, "Text", ContextSearchingType.ChildrenSearch);

            clubLeadersController = new BossRaidersSceneClubLeadersController();
            clubLeadersController.OnInit(rootElement);

            // Wheel
            ContextElement wheelAreaElement = ContextUtils.FindElement(rootElement, "Wheel Area", ContextSearchingType.ChildrenSearch);
            wheelContextElement = ContextUtils.FindElement(wheelAreaElement, "Wheel", ContextSearchingType.ChildrenSearch);
            wheelController = wheelAreaElement.GetComponent<BossRaidersWheelController>();

            MetaContextElementUtils.SimpleSetTextGlobal(buttonInfoElement, "Text", "BOSS_RAIDERS_INFORMATION_BUTTON_TEXT", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetTextGlobal(clubRankTextElement, "BOSS_RAIDERS_CLUB_RANK", ContextSearchingType.ChildrenSearch, 0, 0);
            MetaContextElementUtils.SetTextGlobal(clubTapTextElement, "BOSS_RAIDERS_CLUB_TAP", ContextSearchingType.ChildrenSearch);

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

            MetaContextElementUtils.SetClickable(
                buttonClubTap,
                "OnClickClubDetails",
                rootElement,
                null
            );

            BlackboardUtils.SetOrCreateValue(remainingTimerElement.GetComponent<Blackboard>(), "caller", gameObject);

            //GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_START).Play();

            CreateCharacter();
            CreateMonster();

            isInit = true;
        }

        private void InitBundles()
        {
            EventInfo eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo(false);
            bundleName = BlackboardQueryUtils.GetMetaBundleName(eventInfo, true);
            bundleCommonName = BlackboardQueryUtils.GetMetaBundleName(eventInfo, false);
            bundleContentsSharedName = BlackboardQueryUtils.GetSharedMetaBundleName(eventInfo, true);
            List<string> extraBundleNames = BlackboardQueryUtils.GetMetaExtraBundlesName(eventInfo, true);
            if (extraBundleNames != null && extraBundleNames.Count > 0)
                bundleCharacterName = extraBundleNames[0];
        }

        private void InitBlackboard()
        {
            BlackboardUtils.SetOrCreateValue(rootBB, "wheelAnimator", wheelController.GetAnimator());
            BlackboardUtils.SetOrCreateValue(rootBB, "wheelContextElement", wheelContextElement);
            BlackboardUtils.SetOrCreateValue(rootBB, "highlightElement", wheelController.GetHighlightElement());

            Blackboard dataBB = BossRaidersMonsterData.Instance.GetComponent<Blackboard>();
            if (dataBB != null)
            {
                attackWaitTime = dataBB.GetVariable<float>("attackToHitWaitTime")?.value ?? attackWaitTime;
                hitWaitTime = dataBB.GetVariable<float>("hitToDamageWaitTime")?.value ?? hitWaitTime;
            }

        }

        public void OnInit()
        {
            InitProperty();
            InitWheelData();
            InitBlackboard();

            UpdateTimer();
            UpdateClubText();
            UpdateClubLeaders();
        }

        private void InitClubCells()
        {
            clubCellRankTextElements = new ContextElement[RANK_LIST_COUNT];
            clubCellPointTextElements = new ContextElement[RANK_LIST_COUNT];
            clubCellNameTextElements = new ContextElement[RANK_LIST_COUNT];
            clubCellSymbolAreaElements = new ContextElement[RANK_LIST_COUNT];
            clubCellMyClubOnElements = new ContextElement[RANK_LIST_COUNT];
            clubCellMyClubOffElements = new ContextElement[RANK_LIST_COUNT];
            clubCellSymbolObject = new GameObject[RANK_LIST_COUNT];
            clubCellIconRankAreaElements = new ContextElement[RANK_LIST_COUNT];
            clubCellIconRankTextElements = new ContextElement[RANK_LIST_COUNT];
            clubCellIconRankElements = new ContextElement[RANK_LIST_COUNT][];
            clubCellNameTexts = new Text[RANK_LIST_COUNT];
            clubCellRankDatas = new BossRaidersSceneClubRankData[RANK_LIST_COUNT];

            ContextElement[] contextElements = new ContextElement[RANK_LIST_COUNT];
            for (int i = 0; i < RANK_LIST_COUNT; ++i)
            {
                contextElements[i] = ContextUtils.FindElement(rootElement, string.Format("Club Cell {0}", i + 1), ContextSearchingType.ChildrenSearch);
                clubCellRankTextElements[i] = ContextUtils.FindElement(contextElements[i], "Text Rank", ContextSearchingType.ChildrenSearch);
                clubCellPointTextElements[i] = ContextUtils.FindElement(contextElements[i], "Text Point", ContextSearchingType.ChildrenSearch);
                clubCellNameTextElements[i] = ContextUtils.FindElement(contextElements[i], "Text Club Name", ContextSearchingType.ChildrenSearch);
                clubCellSymbolAreaElements[i] = ContextUtils.FindElement(contextElements[i], "Club Symbol Area", ContextSearchingType.ChildrenSearch);
                clubCellMyClubOnElements[i] = ContextUtils.FindElement(contextElements[i], "Base My Club", ContextSearchingType.ChildrenSearch);
                clubCellMyClubOffElements[i] = ContextUtils.FindElement(contextElements[i], "Base Common", ContextSearchingType.ChildrenSearch);
                clubCellIconRankAreaElements[i] = ContextUtils.FindElement(contextElements[i], "Icon Rank Area", ContextSearchingType.ChildrenSearch);
                clubCellIconRankTextElements[i] = ContextUtils.FindElement(clubCellIconRankAreaElements[i], "Text Icon Rank", ContextSearchingType.ChildrenSearch);
                clubCellNameTexts[i] = clubCellNameTextElements[i].GetComponent<Text>();
                clubCellIconRankElements[i] = new ContextElement[RANK_ICON_COUNT];
                for (int j = 0; j < RANK_ICON_COUNT; ++j)
                    clubCellIconRankElements[i][j] = ContextUtils.FindElement(clubCellIconRankAreaElements[i], string.Format("Icon Rank {0}", j + 1), ContextSearchingType.ChildrenSearch);

                clubCellRankDatas[i] = new BossRaidersSceneClubRankData();
                clubCellRankDatas[i].OnInit(contextElements[i]);
                clubCellRankDatas[i].SetCaller(rootElement);
            }
        }

        private void InitWheelData()
        {
            //long maxBet = BossRaidersUtils.MaxBetMultiplyNumerator;
            //long saveBet = (long)BossRaidersUtils.SaveBetMultiplyNumerator;
            //if (maxBet < saveBet)
            //{
            //    saveBet = BossRaidersUtils.BaseBetMultiplyNumerator;
            //    BossRaidersUtils.SaveBetMultiplyNumerator = (int)saveBet;
            //}
            int defaultMultiply = BossRaidersUtils.GetDefaultBetMultiplyNumerator();
            BossRaidersUtils.CurrentBetMultiplyNumerator = defaultMultiply;
            GetCalcBetMultiplyIndex(defaultMultiply);

            wheelController?.OnInit(rootElement);
            wheelController?.SetAdsTimerCallback(OnAdsTimerCallback);
        }

        public bool CheckBonusEnergy()
        {
            long rewardAmount = CheckRewardBonusEnergy();
            if (rewardAmount > 0)
            {
                characterController.SetAppearEnergy((int)rewardAmount);
                characterController.SetEnergyAnimator(true);
                return true;
            }
            return false;
        }

        public bool CheckNextBossStatus()
        {
            // true : boss alive / false : boss dead
            Blackboard bb = BossRaidersUtils.WheelResultInfo;
            if (bb != null)
                return bb.GetValue<int>("hp") > 0;
            return false;
        }

        private long CheckRewardBonusEnergy()
        {
            long resultAmount = 0;

            Blackboard bb = BossRaidersUtils.WheelResultInfo;
            if (bb != null && bb.GetValue<BossRaidersWheelType>("type") == BossRaidersWheelType.BONUS)
            {
                List<Blackboard> bonusInfoList = bb.GetValue<List<Blackboard>>("bonusInfoList");
                int bonusIndex = bb.GetValue<int>("bonusIndex");

                if (bonusInfoList != null && bonusInfoList.Count > 0)
                {
                    RewardType rewardType = bonusInfoList[bonusIndex].GetValue<RewardType>("type");
                    if (rewardType == RewardType.BOSS_RAIDERS_ENERGY)
                        resultAmount = bonusInfoList[bonusIndex].GetValue<long>("amount");
                }
            }
            return resultAmount;
        }

        public bool CheckSpinEnergy()
        {
            long trySpinEnergy = NumberUtils.GetMultiplierNumeratorValue(BossRaidersUtils.RequiredEnergy, BossRaidersUtils.CurrentBetMultiplyNumerator);
            return BossRaidersUtils.CurrentEnergy >= trySpinEnergy;
        }

        private void ClearClubCellSymbols()
        {
            if (clubCellSymbolObject == null) return;

            for (int i = 0; i < RANK_LIST_COUNT; ++i)
            {
                if (clubCellSymbolObject[i] != null)
                {
                    Destroy(clubCellSymbolObject[i]);
                    clubCellSymbolObject[i] = null;
                }
            }
        }

        private bool CreateCharacter()
        {
            if (characterController == null)
            {
                GameObject obj = MetaObjectUtils.MakePrefab(bundleContentsSharedName, "Boss Raiders Character Base", characterAreaElement.transform, null, "Character");
                if (obj != null)
                {
                    characterController = obj.GetComponent<BossRaidersCharacterCommonController>();
                    if (characterController == null)
                    {
                        Destroy(obj);
                        return false;
                    }
                }
            }

            if (characterController != null)
            {
                if (characterController.InitData(new BossRaidersCharacterBarController(), bundleCharacterName) == false)
                    return false;
                //characterController.startPos = characterStartPosElement.transform.position;
                //characterController.endPos = characterEndPosElement.transform.position;
                UpdateCharacterData();
                return true;
            }
            return false;
        }

        public bool CreateMonster(bool isNewBoss = false)
        {
            ResetMonster();

            if (BossRaidersMonsterData.Instance.utils != null)
            {
                BossRaidersBossType bossType = BossRaidersUtils.CurrentBossType;
                MonsterData monsterData = BossRaidersMonsterData.Instance.data.GetData(BossRaidersUtils.CurrentBossIndex);
                if (monsterData == null)
                {
                    monsterData = new MonsterData();
                    monsterData.type = bossType;
                    monsterData.colorType = BossRaidersUtils.CurrentBossColorType;
                    monsterData.scale = (float)BossRaidersUtils.CurrentBossScale;
                }

                string assetName = BossRaidersMonsterData.Instance.utils.GetMonsterPrefabName(bossType, monsterData);
                if (string.IsNullOrEmpty(assetName))
                    return false;

                GameObject obj = MetaObjectUtils.MakePrefab(bundleContentsSharedName, "Boss Raiders Monster Base", monsterAreaElement.transform, null, "Monster Boss");
                if (obj != null)
                {
                    monsterController = obj.GetComponent<BossRaidersMonsterCommonController>();
                    if (monsterController == null)
                    {
                        Destroy(obj);
                        return false;
                    }

                    monsterController.OnInit(bundleCharacterName, assetName, monsterData, true);
                    monsterController.SetAnimatorCallback(SetBackgroundAnimator);
                    monsterController.SetBossLevel(BossRaidersUtils.CurrentRound);
                    SetBossRaidersCreateBGM(monsterController.SetBossHP(BossRaidersUtils.CurrentBossHP), isNewBoss);
                }
                if (isNewBoss) monsterController.CreateMonsterSound();
            }

            return false;
        }

        private int GetCalcBetMultiplyIndex(int multi)
        {
            //betMultiplyIndex
            List<int> betMultiplyNumeratorList = BossRaidersUtils.BetMultiplyNumeratorList;
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

        private long GetCheckTimeAddCoolTime(long checkTimeStamp, long coolTimeStamp)
        {
            long returnValue = (checkTimeStamp + coolTimeStamp) - TimeUtils.GetTimeStamp();
            return returnValue;
        }

        public bool GetCheckResultType()
        {
            bool isBonus = false;
            Blackboard bb = BossRaidersUtils.WheelResultInfo;
            if (bb != null)
            {
                isBonus = bb.GetValue<BossRaidersWheelType>("type") == BossRaidersWheelType.BONUS;
                if (isBonus) characterController.SetEnergyAnimator(false);
                else
                {
                    // Attack hitType Check & Play animation
                    switch (BossRaidersUtils.GetHitType())
                    {
                        case BossRaidersHitType.BIG:
                            GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_BIG_HIT).Play();
                            SetBattleAnimator("BigHit");
                            break;
                        case BossRaidersHitType.MEGA:
                            GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_MEGA_HIT).Play();
                            SetBattleAnimator("MegaHit");
                            break;
                        case BossRaidersHitType.EPIC:
                            GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_EPIC_HIT).Play();
                            SetBattleAnimator("EpicHit");
                            break;
                    }
                }
            }

            return isBonus;
        }

        public int GetCompletedRound()
        {
            return BossRaidersUtils.UpdatedTotalCompletedRound;
        }

        public bool GetFirstVisit()
        {
            return PlayerPrefs.GetInt(BossRaidersUtils.BOSS_RAIDERS_FIRST_VISIT, 0) > 0;
        }

        public bool GetHitTypeIsDefault()
        {
            bool isDefaultHit = true;

            Blackboard bb = BossRaidersUtils.WheelResultInfo;
            if (bb != null && bb.GetValue<BossRaidersWheelType>("type") == BossRaidersWheelType.ATTACK)
                isDefaultHit = bb.GetValue<BossRaidersHitType>("hitType") == BossRaidersHitType.DEFAULT;

            return isDefaultHit;
        }

        public float GetResultAngle()
        {
            int resultAngle = BossRaidersUtils.WheelResultIndex;
            resultAngle *= 30;
            return (float)resultAngle;
        }

        public long GetRoundResultGem()
        {
            return BossRaidersUtils.GetResultGem(BossRaidersUtils.CurrentReward);
        }

        public long GetPrevRoundResultGem()
        {
            return BossRaidersUtils.GetResultGem(BossRaidersUtils.PrevReward);
        }

        public float GetWheelCurrentAngle(float addAngle = 0.0f)
        {
            float currentAngle = ((IContextFloatProperty)wheelContextElement).GetFloatProperty();
            currentAngle += 360.0f + addAngle;
            currentAngle /= 30.0f;
            return currentAngle;
        }

        public int GetWheelCurrentAngleInteger(float currentAngle)
        {
            int currentAngleInteger = ((int)currentAngle + 1) % 12;
            ++currentAngleInteger;
            return currentAngleInteger;
        }

        public void MoveToCharacter(bool isGo)
        {
            //Vector3 from = isGo ? characterController.startPos : characterController.endPos;
            //Vector3 to = isGo ? characterController.endPos : characterController.startPos;

            //AsyncActionUtils.ApplyMovement(this, characterController.transform, from, to, effectMovementTime, TweenUtils.TweenCollectMove,
            //    0f, () => { MetaContextElementUtils.SendEvent(rootElement, "OnFinishCharacterMove", null, null); });
        }

        public void OpenInfoPopup()
        {
            Transform rootTransform = GameObject.Find("Popup Manager/Area").transform;
            MetaPopupUtils.OpenPopupAsync(this, MetaStringDefine.LOBBY_BUNDLE_NAME, INFO_SCENE_ASSET_NAME, rootTransform, OnLoadInfoPopup);
        }

        public void OpenSpeechBalloon()
        {
            wheelController.OpenSpeechBalloon();
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

        public void OpenClubInfoPopup(long clubId)
        {
            var clubMemberObj = MetaObjectUtils.MakeScene("Popup Club Member Scene", PopupManager.Instance.transform.Find("Area"));
            var clubMemberScene = clubMemberObj.GetComponent<Blackboard>();
            PopupManager.Instance.Open(clubMemberObj);
            clubMemberScene.AddVariable("caller", gameObject);
            clubMemberScene.AddVariable("clubID", clubId);
        }

        private void OnAdsTimerCallback()
        {
            wheelController.UpdateWheelData(isReadyToAds);
        }

        public void OnClickUserProfile(Blackboard bb)
        {
            if (bb != null)
            {
                string userId = bb.GetVariable<string>("_userID")?.value ?? "";
                if (!string.IsNullOrEmpty(userId))
                    MetaContextElementUtils.SendEvent(rootElement, "OnClickClubProfile", userId, null, null);
            }
        }

        public void OnClickWheelBetButton()
        {
            // Bet change check
            List<int> betMultiplyNumeratorList = BossRaidersUtils.BetMultiplyNumeratorList;
            if (betMultiplyNumeratorList == null || betMultiplyNumeratorList.Count == 0)
            {
                betMultiplyIndex = 0;
                return;
            }

            if (betMultiplyIndex + 1 < betMultiplyNumeratorList.Count)
                ++betMultiplyIndex;
            else
                betMultiplyIndex = 0;
            BossRaidersUtils.PreviousBetMultiplyNumerator = BossRaidersUtils.CurrentBetMultiplyNumerator;
            BossRaidersUtils.CurrentBetMultiplyNumerator = betMultiplyNumeratorList[betMultiplyIndex];
            //BossRaidersUtils.SaveBetMultiplyNumerator = betMultiplyNumeratorList[betMultiplyIndex];

            wheelController.ChangeWheelData(BossRaidersUtils.CurrentBetMultiplyNumerator);
            GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_MULTI).Play();
        }

        public void OnCloseRewardPopup()
        {
            EventData eventData = new EventData(BossRaidersUtils.ON_REFRESH_EVENT);
            MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, eventData);
        }

        public void OnCloseToLoading()
        {
            if (isUrgentBGM == false) GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_BGM).Stop();
            else GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_URGENT_BGM).Stop();
        }

        private void OnLoadInfoPopup(SceneLoadOperation _sceneOperation)
        {
            sceneOperation = _sceneOperation;

            GameObject popupGO = sceneOperation.GetScene();
            var infoBB = popupGO.GetComponent<Blackboard>();

            MetaPopupUtils.SetInformationPopupData(infoBB, INFO_PAGE_COUNT, bundleName, "Information Dots", INFO_PAGE_FORMAT, INFO_TEXT);

            BlackboardUtils.SetOrCreateValue(infoBB, "caller", gameObject);

            popupGO.SetActive(true);
            PopupManager.Instance.Open(popupGO);
        }

        public void OnCheckWheelStateToEvent()
        {
            long currentEnergy = BossRaidersUtils.CurrentEnergy;
            string eventName = "OnWheelEnableEvent";
            if (CheckSpinEnergy())
                eventName = "OnWheelSpinEvent";
            else if (currentEnergy >= BossRaidersUtils.RequiredEnergy)
            {
                wheelController?.SetAutoSpin(false);
                eventName = "OnWheelEnableEvent";
            }
            else
            {
                SetAdsValues();
                long endTimestamp = GetCheckTimeAddCoolTime(BossRaidersUtils.LastVideoAdsClaimTimestamp, BossRaidersUtils.BossRaidersCooltime);
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

        public void OnUpdateClubData()
        {
            UpdateClubText();
            UpdateClubLeaders();
        }

        public void OnUpdateData()
        {
            UpdateCharacterData();
            UpdateReadyToAds();
            BossRaidersUtils.UpdateBetMultiplyNumerator();
            wheelController?.UpdateWheelData(isReadyToAds);
        }

        public void PlaySoundWheelSpin(bool isSpin)
        {
            if (isSpin) GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_WHEEL_SPIN).Play();
            else GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_WHEEL_STOP).Play();
        }

        public void PlaySoundWheelStart()
        {
            GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_WHEEL_START).Play();
        }

        private void ResetMonster()
        {
            if (monsterController != null)
            {
                Destroy(monsterController.gameObject);
                monsterController = null;
            }
        }

        private void UpdateReadyToAds()
        {
            bool isReady = false;
            bool isInhouse = BlackboardUtils.FindValue<bool>(MainBlackboard.Get(), "values/misc/INHOUSE_ADS_ENABLED");
            bool isVideo = BlackboardUtils.FindValue<bool>(MainBlackboard.Get(), "values/misc/VIDEO_ADS_ENABLED");
            if (isInhouse)
                isReady = IAMRouter.Instance.CheckTriggerIAM(InAppMessageTriggerType.INHOUSE_ADS_FOR_BOSS_RAIDERS);
            else if (isVideo)
            {
                string placement = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "videoAdsPlacementNames/bossRaiders").value;
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
                if (IAMRouter.Instance.CheckTriggerIAM(InAppMessageTriggerType.INHOUSE_ADS_FOR_BOSS_RAIDERS))
                    rootBB.SetValue("placementKey", "");
            }
            else if (isVideo)
            {
                string placement = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "videoAdsPlacementNames/bossRaiders").value;
                if (!string.IsNullOrEmpty(placement) && VideoAdsController.Instance.IsVideoAdsAvailable(placement))
                    rootBB.SetValue("placementKey", placement);
            }
            else
                rootBB.SetValue("placementKey", "");
        }

        public void SetBackgroundAnimator(string key)
        {
            if (backgroundAnimator != null)
                backgroundAnimator.SetTrigger(key);
        }

        private void SetBattleAnimator(string key, float waitTime = 0.0f, string soundKey = "")
        {
            if (waitTime > 0.0f)
                StartCoroutine(SetBattleAnimatorEnumerator(key, waitTime, soundKey));
            else
                battleAnimator?.SetTrigger(key);
        }

        private IEnumerator SetBattleAnimatorEnumerator(string key, float waitTime, string soundKey)
        {
            yield return new WaitForSeconds(waitTime);
            battleAnimator?.SetTrigger(key);
            if (!string.IsNullOrEmpty(soundKey))
                GSManager.Instance.GetHandler(soundKey).Play();
        }

        private void SetBossRaidersCreateBGM(float hpPercentile, bool isNewBoss)
        {
            if (isNewBoss)
                SetBossRaidersHitBGM(hpPercentile);
            else
            {
                // Init Call
                if (hpPercentile > 0.2f)
                {
                    GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_BGM).Play();
                    isUrgentBGM = false;
                }
                else
                {
                    GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_URGENT_BGM).Play();
                    isUrgentBGM = true;
                }
            }
        }

        private void SetBossRaidersHitBGM(float hpPercentile)
        {
            if (hpPercentile > 0.2f)
            {
                if (isUrgentBGM == true)
                {
                    GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_URGENT_BGM).Stop();
                    GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_BGM).Play();
                    isUrgentBGM = false;
                }
            }
            else
            {
                if (isUrgentBGM == false)
                {
                    GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_BGM).Stop();
                    GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_URGENT_BGM).Play();
                    isUrgentBGM = true;
                }
            }
        }

        public void SetCharacterAnimator(string key, bool isActive)
        {
            characterController?.SetAnimator(key, isActive);
        }

        public void SetFirstVisit(bool isValue)
        {
            PlayerPrefs.SetInt(BossRaidersUtils.BOSS_RAIDERS_FIRST_VISIT, isValue ? 1 : 0);
        }

        public void SetMonsterAnimator(string key, bool isActive)
        {
            monsterController?.SetAnimator(key);
        }

        public void SetPopupBossComplet(GameObject obj, string biType)
        {
            BossRaidersPopupCommonController popupController = obj.GetComponent<BossRaidersPopupCommonController>();
            if (popupController != null)
            {
                popupController.SetMessageText(StringTableUtils.GetString(StringTable.StringTableType.Global, "BOSS_RAIDERS_POPUP_COMMON_COMPLETED_TEXT", BossRaidersUtils.PrevRound, GetPrevRoundResultGem(), BossRaidersUtils.PrevLeaguePoint));
                popupController.SetPurchaseButton(StringTableUtils.GetString(StringTable.StringTableType.Global, "BOSS_RAIDERS_POPUP_COMMON_COMPLETED_BUTTON_TEXT"));
                popupController.SetBIType(biType);
                popupController.SetIsMeta(true);
                GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_POPUP).Play();
            }
        }

        public void SetPopupCompletedRound(GameObject obj, string biType)
        {
            BossRaidersPopupCommonController popupController = obj.GetComponent<BossRaidersPopupCommonController>();
            if (popupController != null)
            {
                popupController.SetMessageText(StringTableUtils.GetString(StringTable.StringTableType.Global, "BOSS_RAIDERS_POPUP_COMMON_GONE_TEXT", BossRaidersUtils.UpdatedTotalCompletedRound));
                popupController.SetPurchaseButton(StringTableUtils.GetString(StringTable.StringTableType.Global, "BOSS_RAIDERS_POPUP_COMMON_GONE_BUTTON_TEXT"));
                popupController.SetBIType(biType);
                popupController.SetIsMeta(true);
                GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_RESULTS).Play();
            }
        }

        public void SetPopupOutOfEnergy(GameObject obj, string biType)
        {
            BossRaidersPopupCommonController popupController = obj.GetComponent<BossRaidersPopupCommonController>();
            if (popupController != null)
            {
                popupController.SetMessageText(StringTableUtils.GetString(StringTable.StringTableType.Global, "BOSS_RAIDERS_POPUP_OUT_ENERGY_TEXT"));
                popupController.SetPurchaseButton(StringTableUtils.GetString(StringTable.StringTableType.Global, "BOSS_RAIDERS_POPUP_OUT_ENERGY_SPIN_BUTTON_TEXT"));
                popupController.SetBIType(biType);
                popupController.SetIsMeta(true);
                popupController.SetCloseActive(true);
                GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_POPUP_OUT_OF_ENERGY).Play();
            }
        }

        public void SetPopupOutOfEnergyAds(GameObject obj, string biType)
        {
            BossRaidersPopupCommonController popupController = obj.GetComponent<BossRaidersPopupCommonController>();
            if (popupController != null)
            {
                popupController.SetMessageText(StringTableUtils.GetString(StringTable.StringTableType.Global, "BOSS_RAIDERS_POPUP_OUT_ENERGY_TEXT"));
                popupController.SetPurchaseButton(StringTableUtils.GetString(StringTable.StringTableType.Global, "BOSS_RAIDERS_POPUP_OUT_ENERGY_SPIN_BUTTON_TEXT"));
                popupController.SetAdButton(StringTableUtils.GetString(StringTable.StringTableType.Global, "BOSS_RAIDERS_POPUP_OUT_ENERGY_AD_BUTTON_TEXT"));
                popupController.SetBIType(biType);
                popupController.SetIsMeta(true);
                popupController.SetCloseActive(true);
                GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_POPUP_OUT_OF_ENERGY).Play();
            }
        }

        private void SetClubDamageText(int damage)
        {
            if (clubDamageTextElement != null)
                MetaContextElementUtils.SetTextGlobal(clubDamageTextElement, "TEXT_COMMA_NUMBER", damage);
        }

        private void SetUserDamageText(int damage)
        {
            if (userDamageTextElement != null)
                MetaContextElementUtils.SetTextGlobal(userDamageTextElement, "TEXT_COMMA_NUMBER", damage);
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

        public void UpdateBossData()
        {
            // boss HP / info data update (RoundBossInfo)
            Blackboard bb = BossRaidersUtils.WheelResultInfo;
            if (bb != null)
            {
                int resultBossHP = bb.GetValue<int>("hp");
                BossRaidersUtils.CurrentBossHP = resultBossHP;

                if (resultBossHP == 0)
                {
                    // next boss data update
                    BossRaidersUtils.RoundBalanceInfo = bb.GetValue<Blackboard>("nextRoundBalanceInfo");
                    BossRaidersUtils.RoundBossInfo = bb.GetValue<Blackboard>("nextRoundBossInfo");
                }
            }
        }

        public void UpdateBossRewardPoint()
        {
            if (clubLeadersController != null)
                clubLeadersController.UpdateBossRewardPoint();
        }

        public void UpdateCharacterData(bool addBonusEnergy = false)
        {
            long updateEnergy = BossRaidersUtils.CurrentEnergy;
            if (addBonusEnergy)
            {
                updateEnergy += (int)CheckRewardBonusEnergy();
                BossRaidersUtils.CurrentEnergy = updateEnergy;
            }
            characterController.SetEnergy(updateEnergy);
        }

        private void UpdateClubLeaders()
        {
            if (clubLeadersController != null)
                clubLeadersController.UpdateClubLeaders();
        }

        private void UpdateClubText()
        {
            MetaContextElementUtils.SetTextGlobal(clubRankTextElement, "BOSS_RAIDERS_CLUB_RANK", BossRaidersUtils.GetClubRank(), BossRaidersUtils.GetPercentile());

            List<Blackboard> clubRankInfoList = BossRaidersUtils.ClubRankInfoList;
            long myClubId = BlackboardUtils.FindValue<long>(null, "/me/clubId");

            ClearClubCellSymbols();
            for (int i = 0; i < RANK_LIST_COUNT; ++i)
            {
                if (clubRankInfoList != null && clubRankInfoList.Count > i)
                {
                    int clubRank = clubRankInfoList[i].GetValue<int>("rank");
                    if (clubRank > 3)
                    {
                        clubCellIconRankAreaElements[i].gameObject.SetActive(false);
                        clubCellRankTextElements[i].gameObject.SetActive(true);
                        MetaContextElementUtils.SetText(clubCellRankTextElements[i], clubRank.ToString());
                    }
                    else
                    {
                        clubCellIconRankAreaElements[i].gameObject.SetActive(true);
                        clubCellRankTextElements[i].gameObject.SetActive(false);
                        for (int j = 0; j < RANK_ICON_COUNT; ++j)
                            clubCellIconRankElements[i][j].gameObject.SetActive(j + 1 == clubRank);
                        MetaContextElementUtils.SetText(clubCellIconRankTextElements[i], clubRank.ToString());
                    }

                    MetaContextElementUtils.SetText(clubCellNameTextElements[i], clubRankInfoList[i].GetValue<string>("clubName"));
                    MetaContextElementUtils.SetTextGlobal(clubCellPointTextElements[i], "TEXT_COMMA_NUMBER", clubRankInfoList[i].GetValue<int>("totalCompletedRound"));
                    string symbolName = clubRankInfoList[i].GetValue<string>("clubSymbol");
                    if (!string.IsNullOrEmpty(symbolName))
                        clubCellSymbolObject[i] = MetaIconUtils.MakeClubSymbolIconObject(symbolName, clubCellSymbolAreaElements[i].transform, null);

                    long clubId = clubRankInfoList[i].GetValue<long>("clubId");
                    bool isMyClub = clubId == myClubId;
                    clubCellMyClubOnElements[i].gameObject.SetActive(isMyClub);
                    clubCellMyClubOffElements[i].gameObject.SetActive(!isMyClub);
                    clubCellNameTexts[i].color = isMyClub ? Color.white : Color.black;
                    clubCellRankDatas[i].clubId = clubId;
                }
                else
                {
                    MetaContextElementUtils.SetText(clubCellRankTextElements[i], "-");
                    MetaContextElementUtils.SetText(clubCellNameTextElements[i], "");
                    MetaContextElementUtils.SetTextGlobal(clubCellPointTextElements[i], "TEXT_COMMA_NUMBER", 0);
                    clubCellIconRankAreaElements[i].gameObject.SetActive(false);
                    clubCellRankTextElements[i].gameObject.SetActive(true);
                    clubCellMyClubOnElements[i].gameObject.SetActive(false);
                    clubCellMyClubOffElements[i].gameObject.SetActive(true);
                    clubCellNameTexts[i].color = Color.black;
                    clubCellRankDatas[i].clubId = 0;
                }
            }
        }

        public void UpdateNewBossDataInfo()
        {
            // Temp Code (boss clear -> Club Rank List aata update)
            // Request Boss Raiders Info Call !
            List<Blackboard> clubRankInfoList = BossRaidersUtils.ClubRankInfoList;
            long myClubId = BlackboardUtils.FindValue<long>(null, "/me/clubId");

            if (clubRankInfoList != null)
            {
                if (clubRankInfoList.Count == 0)
                {
                    // New dummy BB
                    Blackboard bb = (Blackboard)BlackboardUtils.CreateBlackboard("BossRaidersClubRankInfo");
                    BlackboardUtils.SetOrCreateValue<long>(bb, "clubId", myClubId);
                    BlackboardUtils.SetOrCreateValue<string>(bb, "clubName", BlackboardUtils.FindValue<string>(null, "/clubInfo/name"));
                    BlackboardUtils.SetOrCreateValue<string>(bb, "clubSymbol", BlackboardUtils.FindValue<string>(null, "/clubInfo/symbol"));
                    BlackboardUtils.SetOrCreateValue<int>(bb, "totalCompletedRound", 1);

                    BlackboardUtils.AddToBlackboardList(BossRaidersUtils.BossRaidersInfo, "clubRankInfoList", bb);
                }
                else
                {
                    for (int i = 0; i < clubRankInfoList.Count; ++i)
                    {
                        if (clubRankInfoList[i].GetValue<long>("clubId") == myClubId)
                        {
                            int totalCompletedRound = clubRankInfoList[i].GetValue<int>("totalCompletedRound");
                            clubRankInfoList[i].SetValue("totalCompletedRound", totalCompletedRound + 1);
                        }
                    }
                }
            }

            UpdateClubText();
        }

        private void UpdateTimer()
        {
            EventInfo eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo();
            if (eventInfo != null)
                MetaGameUtils.UpdateMetaGameRemainingTimer(eventTimerElement, remainingTimerElement, eventInfo.endTimestamp);
        }

        public void WheelAttack(bool isAttack)
        {
            characterController?.Attack(isAttack);
        }

        public void WheelMonsterDamageEffect()
        {
            Blackboard bb = BossRaidersUtils.WheelResultInfo;
            int clubDamage = 0;

            int currentBossHP = BossRaidersUtils.CurrentBossHP;
            if (bb != null)
            {
                int userAttack = bb.GetValue<int>("attack");
                int resultBossHP = bb.GetValue<int>("hp");
                if (currentBossHP - userAttack > resultBossHP)
                {
                    clubDamage = currentBossHP - userAttack - resultBossHP;
                    SetClubDamageText(clubDamage);
                }
                SetUserDamageText(userAttack);

                if (monsterController != null) SetBossRaidersHitBGM(monsterController.SetBossHP(resultBossHP));
            }

            SetBattleAnimator("DamageNormal");
            if (clubDamage > 0)
                SetBattleAnimator("DamageClub", clubDamageWaitTime, BossRaidersUtils.Sounds.BOSS_RAIDERS_BATTLE_INJURED_MULTI);
        }

        public void WheelMonsterHit(bool isAlive)
        {
            monsterController?.HitMonster(isAlive);
        }

        // BI Event
        public void BIClientClickBossRaidersMaxBet()
        {
            long prevBetMultiplier = BossRaidersUtils.PreviousBetMultiplyNumerator;
            long currentBetMultiplier = BossRaidersUtils.CurrentBetMultiplyNumerator;
            long requiredEnergy = BossRaidersUtils.RequiredEnergy;

            Dictionary<string, object> customData = new Dictionary<string, object>();
            customData["bet"] = (long)(NumberUtils.GetMultiplierFromNumerator(currentBetMultiplier) * requiredEnergy);
            customData["previous_bet"] = (long)(NumberUtils.GetMultiplierFromNumerator(prevBetMultiplier) * requiredEnergy);
            customData["bet_multiplier"] = currentBetMultiplier;
            customData["previous_bet_multiplier"] = prevBetMultiplier;
            customData["theme_id"] = BossRaidersUtils.ThemeId;
            Analytics.CustomEvent("client_click_boss_raiders_max_bet", customData);
        }

        public void BIClientBossRaidersPopup(string contextId, string type)
        {
            BossRaidersUtils.BIClientBossRaidersPopup(contextId, type);
        }

        public void BIClientClickBossRaidersPopup(string contextId, string type)
        {
            BossRaidersUtils.BIClientClickBossRaidersPopup(contextId, type);
        }
    }
}
