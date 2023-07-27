using System.Collections;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using BagelCode.BossRaiders;

namespace BagelCode.BossRaiders.Deal
{
    public partial class BossRaidersDealSceneController : MonoBehaviour
    {
        public float effectMovementTime = 0.75f;
        public float increaseEffectTime = 1f;
        public float clubDamageWaitTime = 0.0f;
        public float attackWaitTime = 0.5f;
        public float hitWaitTime = 0.25f;

        public bool isAutoSpin = false;

        public string bundleName = "";
        public string bundleCommonName = "";
        public string bundleContentsSharedName = "";
        public string bundleCharacterName = "";

        private GameObject gs_managerObj = null;

        private ContextElement rootElement;
        private Blackboard rootBB;
        private Animator rootAnimator;
        private Animator backgroundAnimator;
        private Animator scoreAnimator;

        private ContextElement topRewardTextElement;
        private ContextElement clearRewardTextElement;

        // battle
        private Animator battleAnimator;
        private ContextElement characterAreaElement;
        private ContextElement monsterAreaElement;
        private ContextElement userDamageTextElement;

        // characters
        private BossRaidersCharacterCommonController characterController = null;
        private BossRaidersMonsterCommonController monsterController = null;

        // wheel
        private ContextElement wheelContextElement;
        private BigWheel bigWheelComponent;
        private BossRaidersWheelControllerBase wheelController;

        private SceneLoadOperation sceneOperation;

        private Coroutine updateScoreEnumerator = null;

        private Variable<int> spinCount = null;

        private int themeId = 0;
        private bool isUrgentBGM = false;
        private string dealUuid = "";

        private bool isInit = false;

        public void OnInit()
        {
            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);
            rootBB = gameObject.GetComponent<Blackboard>();
            rootAnimator = gameObject.GetComponent<Animator>();

            InitBundles();
            InitProperty();
            InitWheelData();
            InitBlackboard();
            InitData();
        }

        private void InitBundles()
        {
            dealBB = BlackboardUtils.GetOrCreateVariable<Blackboard>(MainBlackboard.Get(), BossRaidersUtils.BOSS_RAIDERS_DEAL_INFO).value;

            themeId = rootBB.GetValue<int>("themeId");
            dealUuid = rootBB.GetValue<string>("dealUuid");
            spinCount = dealBB.GetVariable<int>("spinCount");

            bundleName = BossRaidersUtils.GetBossRaidersDealBundleName(themeId, true);
            bundleCommonName = BossRaidersUtils.GetBossRaidersDealBundleName(themeId, false);
            bundleContentsSharedName = BossRaidersUtils.GetBossRaidersSharedBundleName(true);
            bundleCharacterName = BossRaidersUtils.GetBossRaidersCharacterBundleName(themeId);
        }

        private void InitProperty()
        {
            if (isInit) return;

            if (gs_managerObj == null)
                gs_managerObj = MetaObjectUtils.MakePrefab(bundleCharacterName, "Boss Raiders Contents Sounds", transform);

            var backgroundElement = ContextUtils.FindElement(rootElement, "Background", ContextSearchingType.ChildrenSearch);
            backgroundAnimator = backgroundElement?.GetComponent<Animator>();
            MetaContextElementUtils.SetClickable(backgroundElement, () => EventSender.SendEvent(gameObject, "OnClickStartSpin"));

            ContextElement scoreElement = ContextUtils.FindElement(rootElement, "Score Area", ContextSearchingType.ChildrenSearch);
            scoreAnimator = scoreElement.GetComponent<Animator>();
            topRewardTextElement = ContextUtils.FindElement(scoreElement, "Text Reward", ContextSearchingType.ChildrenSearch);
            ContextElement rewardElement = ContextUtils.FindElement(rootElement, "Reward Area", ContextSearchingType.ChildrenSearch);
            clearRewardTextElement = ContextUtils.FindElement(rewardElement, "Text Reward", ContextSearchingType.ChildrenSearch);

            // Battle
            ContextElement battleElement = ContextUtils.FindElement(rootElement, "Battle Area", ContextSearchingType.ChildrenSearch);
            battleAnimator = battleElement.GetComponent<Animator>();
            characterAreaElement = ContextUtils.FindElement(battleElement, "Character Area", ContextSearchingType.ChildrenSearch);
            monsterAreaElement = ContextUtils.FindElement(battleElement, "Monster Area", ContextSearchingType.ChildrenSearch);

            ContextElement damageAreaElement = ContextUtils.FindElement(battleElement, "Damage Area", ContextSearchingType.ChildrenSearch);
            ContextElement userDamageAreaElement = ContextUtils.FindElement(damageAreaElement, "User Damage", ContextSearchingType.ChildrenSearch);
            userDamageTextElement = ContextUtils.FindElement(userDamageAreaElement, "Text", ContextSearchingType.ChildrenSearch);

            // Wheel
            ContextElement wheelAreaElement = ContextUtils.FindElement(rootElement, "Wheel Area", ContextSearchingType.ChildrenSearch);
            wheelContextElement = ContextUtils.FindElement(wheelAreaElement, "Wheel", ContextSearchingType.ChildrenSearch);
            bigWheelComponent = wheelContextElement.GetComponent<BigWheel>();
            wheelController = wheelAreaElement.GetComponent<BossRaidersWheelControllerBase>();

            CreateCharacter();
            CreateMonster();

            isInit = true;
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

        private void InitData()
        {
            UpdateClearBonusText();
            UpdateScoreText();
        }

        private void InitWheelData()
        {
            InitWheelCandidate();
            wheelController?.OnInit(rootElement);
        }

        private bool CreateCharacter()
        {
            if (characterController == null)
            {
                GameObject obj = MetaObjectUtils.MakePrefab(bundleContentsSharedName, "Boss Raiders Deal Character Base", characterAreaElement.transform, null, "Character");
                if (obj != null)
                {
                    characterController = obj.GetComponent<BossRaidersCharacterCommonController>();
                    if (characterController == null)
                    {
                        Destroy(obj);
                        return false;
                    }
                }

                if (characterController != null)
                {
                    if (characterController.InitData(new BossRaidersDealCharacterBarController(), bundleCharacterName, false) == false)
                        return false;
                    UpdateCharacterData();
                    return true;
                }
            }
            return false;
        }

        public bool CreateMonster(bool isNewBoss = false)
        {
            if (spinCount.value == 0) return false;

            ResetMonster();
            if (BossRaidersMonsterData.Instance.utils != null)
            {
                MonsterData monsterData = BossRaidersMonsterData.Instance.data.GetData(GetBossIndex());
                string assetName = BossRaidersMonsterData.Instance.utils.GetMonsterPrefabName(monsterData.type, monsterData);
                if (string.IsNullOrEmpty(assetName))
                    return false;

                GameObject obj = MetaObjectUtils.MakePrefab(bundleContentsSharedName, "Boss Raiders Deal Monster Base", monsterAreaElement.transform, null, "Monster Boss");
                if (obj != null)
                {
                    monsterController = obj.GetComponent<BossRaidersMonsterCommonController>();
                    if (monsterController == null)
                    {
                        Destroy(obj);
                        return false;
                    }

                    monsterController.OnInit(bundleCharacterName, assetName, monsterData, false);
                    monsterController.SetAnimatorCallback(SetBackgroundAnimator);
                    monsterController.SetBossLevel(GetCurrentBossRound());
                    SetBossRaidersCreateBGM(monsterController.SetBossHP(GetCurrentBossHP()), isNewBoss);
                }
                if (isNewBoss) monsterController.CreateMonsterSound();
            }

            return false;
        }

        public void UpdateCharacterData()
        {
            characterController.SetEnergy(GetExpectedDamage());
        }

        private void ResetMonster()
        {
            if (monsterController != null)
            {
                Destroy(monsterController.gameObject);
                monsterController = null;
            }
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

        private void SetUserDamageText(int damage)
        {
            if (userDamageTextElement != null)
                MetaContextElementUtils.SetTextGlobal(userDamageTextElement, "TEXT_COMMA_NUMBER", damage);
        }

        public void OnCheckWheelStateToEvent()
        {
            if (CheckSpinCount())
                MetaContextElementUtils.SendEvent(rootElement, "OnWheelSpinEvent", null, null);
            else
            {
                wheelController?.SetAutoSpin(false);
                MetaContextElementUtils.SendEvent(rootElement, "OnWheelNotSpinEvent", null, null);
            }
        }

        public void OnCloseToLoading()
        {
            if (isUrgentBGM == false) GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_BGM).Stop();
            else GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_URGENT_BGM).Stop();
        }

        public void OnUpdateData()
        {
            UpdateWheelSpinData();
        }

        public bool CheckSpinCount()
        {
            return spinCount.value > 0;
        }

        public void UpdateAutoSpin(bool value)
        {
            isAutoSpin = value;
        }

        public void UpdateClearBonusText()
        {
            // clearRewardTextElement
            Blackboard roundInfoBB = GetRoundBalanceInfoBB();
            MetaContextElementUtils.SetText(clearRewardTextElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "COMMA_COIN", roundInfoBB?.GetValue<long>("clearBonus") ?? 0));
        }

        private void UpdateScoreText()
        {
            long credit = BlackboardUtils.GetOrCreateVariable<long>(dealBB, "credit")?.value ?? 0L;
            long gem = BlackboardUtils.GetOrCreateVariable<long>(dealBB, "gem")?.value ?? 0L;
            long prevCredit = BlackboardUtils.GetOrCreateVariable<long>(dealBB, "prevCredit")?.value ?? 0L;
            long prevGem = BlackboardUtils.GetOrCreateVariable<long>(dealBB, "prevGem")?.value ?? 0L;

            if (updateScoreEnumerator != null)
                StopCoroutine(updateScoreEnumerator);
            updateScoreEnumerator = StartCoroutine(UpdateScoreTextCoroutine(prevCredit, credit, prevGem, gem, 0.5f));
        }

        private IEnumerator UpdateScoreTextCoroutine(long prevCredit, long credit, long prevGem, long gem, float runTime)
        {
            if (runTime > 0.0f && (prevCredit != credit || prevGem != gem))
            {
                float elapsedTime = 0.0f;
                long creditInterval = credit - prevCredit;
                long gemInterval = gem - prevGem;
                while (elapsedTime < runTime)
                {
                    yield return null;

                    elapsedTime += Time.deltaTime;

                    float rate = elapsedTime / runTime;
                    long nextCredit, nextGem;

                    if (rate > 1.0f)
                    {
                        nextCredit = credit;
                        nextGem = gem;
                    }
                    else
                    {
                        nextCredit = prevCredit + (long)(creditInterval * rate);
                        nextGem = prevGem + (long)(gemInterval * rate);
                    }

                    MetaContextElementUtils.SetText(topRewardTextElement, GetScoreText(nextCredit, nextGem));
                }
            }

            MetaContextElementUtils.SetText(topRewardTextElement, GetScoreText(credit, gem));
        }

        private string GetScoreText(long credit, long gem)
        {
            string scoreText = StringTableUtils.GetString(StringTable.StringTableType.Global, "COMMA_COIN", credit);
            if (gem > 0)
                scoreText += " " + StringTableUtils.GetString(StringTable.StringTableType.Global, "COMMA_GEM", gem);
            return scoreText;
        }

#region SPIN
        private void WheelActiveAnimation(string key, bool isActive)
        {
            wheelController?.SetActiveAnimation(key, isActive);
        }

        public void WheelStart(Vector3 torque, ForceMode mode)
        {
            WheelActiveAnimation("Spin", true);
            wheelContextElement.GetComponent<Rigidbody>().AddRelativeTorque(torque, mode);
            GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_WHEEL_START).Play();
        }

        public void UpdateWheelSpinData(bool addBonusSpin = false)
        {
            int wheelCount = spinCount.value; // Current spin count get;
            if (addBonusSpin)
            {
                wheelCount += CheckBonusResultSpin();
                spinCount.value = wheelCount;
            }
            wheelController?.ChangeWheelData(spinCount.value);
        }

        public void SpinWheel(bool isSpin)
        {
            if (wheelController != null)
                wheelController.SpinWheel(isSpin);
        }

        public void CheckCurrentWheelSpin(float addAngle = 0.0f)
        {
            Variable<string> lastCellName = BlackboardUtils.GetOrCreateVariable<string>(rootBB, "lastCellName");
            Variable<string> currentCellName = BlackboardUtils.GetOrCreateVariable<string>(rootBB, "currentCellName");
            ContextElement highlightElement = wheelController.GetHighlightElement();

            currentCellName.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "TEXT_ZERO_2", GetWheelCurrentAngleInteger(GetWheelCurrentAngle(addAngle)));
            if (!currentCellName.value.Equals(lastCellName.value))
            {
                wheelController.GetHighlightAnimator(lastCellName.value)?.SetBool("Highlight", false);
                PlaySoundWheelSpin(true);
            }
            Animator currentHighlightAnimator = wheelController.GetHighlightAnimator(currentCellName.value);
            currentHighlightAnimator?.SetBool("Highlight", true);
            currentHighlightAnimator?.SetBool("Win", false);

            lastCellName.value = currentCellName.value;
        }

        public void SetWheelSkip(float addAngle = 0.0f)
        {
            BigWheelSkip();
            CheckCurrentWheelSpin(addAngle);
        }

        public void SetResultAngleBigWheelSpin(float initialTorque, int additionalRotationCount)
        {
            BigWheelSpin(initialTorque, GetResultAngle(), additionalRotationCount);
        }

        private void BigWheelSpin(float initialTorque, float desiredAngle, int additionalRotationCount)
        {
            bigWheelComponent?.Simulation(initialTorque, desiredAngle, additionalRotationCount);
        }

        private void BigWheelSkip()
        {
            bigWheelComponent?.SkipSimulation();
        }

        private float GetResultAngle()
        {
            int resultAngle = GetSpinResultAngle();
            resultAngle *= 30;
            return (float)resultAngle;
        }

        private float GetWheelCurrentAngle(float addAngle = 0.0f)
        {
            float currentAngle = ((IContextFloatProperty)wheelContextElement).GetFloatProperty();
            currentAngle += 360.0f + addAngle;
            currentAngle /= 30.0f;
            return currentAngle;
        }

        private int GetWheelCurrentAngleInteger(float currentAngle)
        {
            int currentAngleInteger = ((int)currentAngle + 1) % 12;
            ++currentAngleInteger;
            return currentAngleInteger;
        }

        public void PlaySoundWheelSpin(bool isSpin)
        {
            if (isSpin) GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_WHEEL_SPIN).Play();
            else GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_WHEEL_STOP).Play();
        }
        // Spin Result
        public IEnumerator SpinWheelResultCoroutine()
        {
            Variable<string> lastCellName = BlackboardUtils.GetOrCreateVariable<string>(rootBB, "lastCellName");
            Animator lastHighlightAnimator = wheelController.GetHighlightAnimator(lastCellName.value);

            PlaySoundWheelSpin(false);
            WheelActiveAnimation("Spin", false);
            WheelActiveAnimation("Win", true);

            lastHighlightAnimator?.SetBool("Highlight", false);
            lastHighlightAnimator?.SetBool("Win", true);

            yield return new WaitForSeconds(0.1f);

            lastHighlightAnimator?.SetBool("Win", false);
            WheelActiveAnimation("Win", false);
        }

        public bool GetCheckResultType()
        {
            bool isBonus = GetSpinResultIsBonus();
            if (!isBonus)
            {
                switch (GetSpinResultHitType())
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
            return isBonus;
        }

        private bool GetHitTypeIsDefault()
        {
            return GetSpinResultHitType() == BossRaidersHitType.DEFAULT;
        }

        // Bonus Popup
        public void OpenBonusPopup()
        {
            MetaPopupUtils.OpenPopupAsync(this, bundleContentsSharedName, "Popup Boss Raiders Deal Bonus Scene", MetaPopupUtils.PopupManagerAreaTransform, OnLoadBonusPopup);
        }

        private void OnLoadBonusPopup(SceneLoadOperation _sceneOperation)
        {
            sceneOperation = _sceneOperation;
            GameObject popupGO = sceneOperation.GetScene();
            var popupBB = popupGO.GetComponent<Blackboard>();

            BlackboardUtils.SetOrCreateValue(popupBB, "caller", gameObject);
            BlackboardUtils.SetOrCreateValue(popupBB, "isMeta", false);

            PopupManager.Instance.Open(popupGO);
            popupGO.SetActive(true);
        }

        public void CloseBonusPopup()
        {
            if (sceneOperation != null)
            {
                GameObject popupGO = sceneOperation.GetScene();
                PopupManager.Instance.Close(popupGO);
                Destroy(popupGO);
            }
            wheelController?.SetSpinButtonActiveCover(false);
            sceneOperation = null;
        }

        public IEnumerator ApplyBonusResult()
        {
            int rewardAmount = CheckBonusResultSpin();
            if (rewardAmount > 0)
            {
                yield return new WaitForSeconds(0.2f);
                // SPIN ADD / Appear animation?
                UpdateWheelSpinData(true);
            }
            else
            {
                // Update Score
                UpdateBonusReward();
                RewardType bonusRewardType = GetBonusResultType();
                scoreAnimator?.SetTrigger((bonusRewardType == RewardType.GEM) ? "isGem" : "isCoin");
                if (bonusRewardType == RewardType.GEM)
                    GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_DEAL_SCORE_GEM).Play();
                else
                    GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_DEAL_SCORE_COIN).Play();
                UpdateScoreText();
            }
        }
        // Attack
        public IEnumerator SpinResultAttack(bool isBossAlive)
        {
            BossRaidersHitType hitType = GetSpinResultHitType();
            if (hitType != BossRaidersHitType.DEFAULT)
                yield return new WaitForSeconds(1.0f);

            WheelCharacterAttack();

            yield return new WaitForSeconds(attackWaitTime);

            WheelMonsterHit(isBossAlive);

            yield return new WaitForSeconds(hitWaitTime);

            WheelMonsterDamageEffect();
            yield return new WaitForSeconds(0.5f);
            UpdateBossData(isBossAlive);
        }

        public void UpdateClearBonusScore()
        {
            UpdateClearBonus();
            OnUpdateRewardAnimation();
        }

        private void WheelCharacterAttack()
        {
            characterController?.Attack(true);
        }

        private void WheelMonsterHit(bool isAlive)
        {
            monsterController?.HitMonster(isAlive);
        }

        public bool CheckBossAlive()
        {
            // true : boss alive / false : boss dead
            Blackboard resultBB = GetSpinResultInfoBB();
            if (resultBB != null)
                return resultBB.GetValue<long>("remainingHp") > 0L;
            return false;
        }

        private void WheelMonsterDamageEffect()
        {
            Blackboard resultBB = GetSpinResultInfoBB();
            if (resultBB != null)
            {
                Blackboard wheelIndexBB = GetWheelCandidate(GetSpinResultAngle());
                int userAttack = 0;
                if (wheelIndexBB.GetValue<BossRaidersWheelType>("type") == BossRaidersWheelType.ATTACK)
                    userAttack = wheelIndexBB.GetValue<int>("baseAttackHp");
                long resultBossHP = resultBB.GetValue<long>("remainingHp");
                SetUserDamageText(userAttack);
                if (monsterController != null) SetBossRaidersHitBGM(monsterController.SetBossHP(resultBossHP));
            }

            SetBattleAnimator("DamageNormal");
        }

        public IEnumerator RequestBossRaidersDealSpin()
        {
            if (spinCount.value > 0)
                --spinCount.value;

            UpdateWheelSpinData();

            bool requestSuccess = false;
            bool requestFail = false;
#if DEV
            Variable<BossRaidersDebugSpinType> spinType = BlackboardUtils.GetOrCreateVariable<BossRaidersDebugSpinType>(dealBB, BossRaidersUtils.BOSS_RAIDERS_DEBUG_SPIN_TYPE);
            BagelCodeClientAPI.RequestBossRaidersDealDebugSpin(dealUuid, spinType?.value ?? BossRaidersDebugSpinType.NONE, isAutoSpin,
#else
            BagelCodeClientAPI.RequestBossRaidersDealSpin(dealUuid, isAutoSpin,
#endif
                (response) =>
                {
                    BackupDealInfo();
                    ClientAPI2Blackboard.Serialize(dealBB, response);
                    if (response.userSyncInfo != null)
                    {
                        BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
                        BlackboardQueryUtils.ApplyUserSyncInfo();
                    }
#if DEV
                    spinType.value = BossRaidersDebugSpinType.NONE;
#endif
                    requestSuccess = true;
                },
                (error) =>
                {
                    switch (error.errorCode)
                    {
                        case Error.BOSS_RAIDERS_DEAL_UUID_MISMATCH_ERROR:
                        case Error.BOSS_RAIDERS_DEAL_NOT_EXIST_ERROR:
                            GlobalErrorHandler.GlobalError(error);
                            break;
                        default:
                            GlobalErrorHandler.GlobalError(error);
                            break;
                    }
                    
                    requestFail = true;
                });

            yield return new WaitUntil(() => requestSuccess || requestFail);
        }

#endregion
#region POPUP
        public void SetPopupBossComplet(GameObject obj, string biType)
        {
            BossRaidersPopupCommonController popupController = obj.GetComponent<BossRaidersPopupCommonController>();
            if (popupController != null)
            {
                popupController.SetMessageText(StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_BOSS_RAIDERS_DEAL_COMMON_COMPLETED_TEXT", dealBB?.GetValue<int>("prevRound"), dealBB?.GetValue<long>("prevClearBonus")));
                popupController.SetPurchaseButton(StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_BOSS_RAIDERS_DEAL_COMMON_COMPLETED_BUTTON_TEXT"));
                popupController.SetBIType(biType);
                popupController.SetIsMeta(false);
                GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_POPUP).Play();
            }
        }

        public void OpenDealTotalResultPopup()
        {
            MetaPopupUtils.OpenPopupAsync(this, bundleContentsSharedName, "Popup Boss Raiders Deal Result Scene", MetaPopupUtils.PopupManagerAreaTransform, OnLoadTotalResultPopup);
        }

        private void OnLoadTotalResultPopup(SceneLoadOperation _sceneOperation)
        {
            sceneOperation = _sceneOperation;
            GameObject popupGO = sceneOperation.GetScene();
            var popupBB = popupGO.GetComponent<Blackboard>();

            BlackboardUtils.SetOrCreateValue(popupBB, "caller", gameObject);
            BlackboardUtils.SetOrCreateValue(popupBB, "endInfo", dealBB.GetValue<Blackboard>("endInfo"));

            PopupManager.Instance.Open(popupGO);
            popupGO.SetActive(true);

            BossRaidersDealPopupTotalResultController resultController = popupGO.GetComponent<BossRaidersDealPopupTotalResultController>();
            if (resultController != null)
                resultController.OnInit();
        }
#endregion
        public IEnumerator OnBossRaidersDealLoadingCoroutine()
        {
            GameObject loadingSceneObj = null;
            yield return StartCoroutine(MetaObjectUtils.MakeSceneCoroutine(bundleCommonName, "Boss Raiders Deal Loading Scene", MetaPopupUtils.PopupManagerAreaTransform,
                (GameObject popupObj) => loadingSceneObj = popupObj));

            var dealLoadingBB = loadingSceneObj.GetComponent<Blackboard>();
            BlackboardUtils.SetOrCreateValue(dealLoadingBB, "caller", rootBB.GetValue<GameObject>("rootCaller"));
            BlackboardUtils.SetOrCreateValue(dealLoadingBB, "isEnter", false);
            BlackboardUtils.SetOrCreateValue(dealLoadingBB, "_biContextID", rootBB.GetValue<string>("_biContextID"));
            BlackboardUtils.SetOrCreateValue(dealLoadingBB, "themeId", themeId);

            PlayerPrefs.SetString(BossRaidersUtils.NOT_FINISHED_BOSS_RAIDERS_DEAL, "");

            loadingSceneObj.SetActive(true);
        }

        public void OnUpdateRewardAnimation()
        {
            scoreAnimator?.SetTrigger("isCoin");
            GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_DEAL_SCORE_COIN).Play();
            UpdateScoreText();
        }
    }
}