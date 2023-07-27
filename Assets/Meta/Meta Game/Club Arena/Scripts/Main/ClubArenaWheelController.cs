using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using NodeCanvas.Framework;
using BagelCode.ClientModels;

namespace BagelCode.ClubArena
{
    public class ClubArenaWheelController : MonoBehaviour
    {
        public float effectMovementTime = 0.75f;

        private ContextElement rootElement;
        private Animator rootAnimator;

        private ContextElement callerElement;

        private ContextElement wheelContextElement;

        private ContextElement[] wheelElements;
        private ContextElement highlightElement;
        private ContextElement betButtonElement;
        private ContextElement betSpeechBalloonTextElement;
        private ContextElement betPopupTextElement;
        private ContextElement spinButtonElement;
        private ContextElement spinButtonTextElement;

        private ContextElement spinButtonInfoElement;
        private ContextElement spinButtonAdElement;

        private ContextElement adSppeechBallonElement;
        private ContextElement adSppeechBallonTextElement;
        private ContextElement wheelInactiveElement;

        private ContextElement wheelItemPoolElement;

        private ClubArenaEnergyController energyController;
        private ClubArenaWheelRewardItemController rewardItemController = null;

        private bool isInit = false;

        private const int WHEEL_ITEM_COUNT = 10;

        private ClubArenaSpinButton spinButtonController;
        private RemainingTimerController timerController;

        private UnityAction ownerAdsTimerCallback;
        private UnityAction<ClubArenaSpinResultType> ownerCompletedRewardItem;
        private Coroutine betUpgradeEffectEnumerator = null;
        private Coroutine speechBalloonEnumerator = null;

        private const string WHEEL_SPIN = "Spin";
        private const string WHEEL_WIN = "Win";
        private const string WHEEL_BET_UPGRADE = "BetUpgrade";
        private const string WHEEL_BET_POPUP = "BetPopup";
        private const string WHEEL_BET_SPEECH_BALLOON = "BetSpeechBalloon";

        private void OnDisable()
        {
            if (betUpgradeEffectEnumerator != null)
                StopCoroutine(betUpgradeEffectEnumerator);
            if (speechBalloonEnumerator != null)
                StopCoroutine(speechBalloonEnumerator);
        }

        public void OnInit(ContextElement caller)
        {
            if (isInit) return;

            callerElement = caller;

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);
            rootAnimator = gameObject.GetComponent<Animator>();

            wheelContextElement = ContextUtils.FindElement(rootElement, "Wheel", ContextSearchingType.ChildrenSearch);

            wheelElements = new ContextElement[WHEEL_ITEM_COUNT];
            ContextElement baseElement = ContextUtils.FindElement(wheelContextElement, "Base", ContextSearchingType.ChildrenSearch);
            for (int i = 0; i < WHEEL_ITEM_COUNT; ++i)
                wheelElements[i] = ContextUtils.FindElement(baseElement, string.Format("{0:00}", i + 1), ContextSearchingType.ChildrenSearch);

            highlightElement = ContextUtils.FindElement(wheelContextElement, "Highlight", ContextSearchingType.ChildrenSearch);
            betButtonElement = ContextUtils.FindElement(rootElement, "Bet Button", ContextSearchingType.ChildrenSearch);

            ContextElement betSpeechBalloonElement = ContextUtils.FindElement(rootElement, "Bet Speech Balloon", ContextSearchingType.ChildrenSearch);
            betSpeechBalloonTextElement = ContextUtils.FindElement(betSpeechBalloonElement, "Text", ContextSearchingType.ChildrenSearch);
            ContextElement betPopupElement = ContextUtils.FindElement(rootElement, "Bet Popup", ContextSearchingType.ChildrenSearch);
            betPopupTextElement = ContextUtils.FindElement(betPopupElement, "Text", ContextSearchingType.ChildrenSearch);

            spinButtonElement = ContextUtils.FindElement(rootElement, "Button Spin", ContextSearchingType.ChildrenSearch);
            spinButtonInfoElement = ContextUtils.FindElement(spinButtonElement, "Info", ContextSearchingType.ChildrenSearch);
            spinButtonTextElement = ContextUtils.FindElement(spinButtonInfoElement, "Text Spin Energy", ContextSearchingType.ChildrenSearch);
            spinButtonAdElement = ContextUtils.FindElement(spinButtonElement, "Ad", ContextSearchingType.ChildrenSearch);

            spinButtonController = spinButtonElement.GetComponent<ClubArenaSpinButton>();

            adSppeechBallonElement = ContextUtils.FindElement(spinButtonAdElement, "Ad Speech Balloon", ContextSearchingType.ChildrenSearch);
            adSppeechBallonTextElement = ContextUtils.FindElement(adSppeechBallonElement, "Text Time", ContextSearchingType.ChildrenSearch);

            wheelInactiveElement = ContextUtils.FindElement(rootElement, "Wheel Inactive", ContextSearchingType.ChildrenSearch);

            ContextElement wheelPoolAreaElement = ContextUtils.FindElement(rootElement, "Wheel Item Pool Area", ContextSearchingType.ChildrenSearch);
            wheelItemPoolElement = ContextUtils.FindElement(wheelPoolAreaElement, "Anchor", ContextSearchingType.ChildrenSearch);

            energyController = gameObject.GetComponent<ClubArenaEnergyController>();
            energyController?.OnInit(rootElement);

            // Button Clickable Event
            MetaContextElementUtils.SetClickable(
                betButtonElement,
                "OnClickWheelBet",
                callerElement,
                null
            );

            timerController = adSppeechBallonTextElement.gameObject.GetComponent<RemainingTimerController>();
            if (timerController == null)
                timerController = adSppeechBallonTextElement.gameObject.AddComponent<RemainingTimerController>();

            timerController.Init(
                adSppeechBallonTextElement,
                "TIME_FORMAT_HHMMSS_TOTALHOUR",
                "TEXT_NORMAL",
                "",
                "00:00",
                false,
                OnAdsTimerCallback
            );

            InitWheelData();
            OpenSpeechBalloon();
            isInit = true;
        }

        private void InitWheelData()
        {
            SetWheelData(ClubArenaUtils.GetDefaultBetMultiplyNumerator());
            SetBetText();
        }

        public void ChangeMultiplier(long multi)
        {
            SetBetText();

            if (betUpgradeEffectEnumerator != null)
                StopCoroutine(betUpgradeEffectEnumerator);

            betUpgradeEffectEnumerator = StartCoroutine(BetUpgradeEnumerator(multi));

            OpenSpeechBalloon();

            if (ClubArenaUtils.CurrentBetMultiplyNumerator == ClubArenaUtils.MaxBetMultiplyNumerator && ClubArenaUtils.MaxBetMultiplyNumerator != ClubArenaUtils.BaseBetMultiplyNumerator)
                SetTriggerAnimation(WHEEL_BET_POPUP);
        }

        private void SetWheelData(long multi)
        {
            List<Blackboard> WheelCandidateList = ClubArenaUtils.WheelCandidateList;
            if (WheelCandidateList == null || WheelCandidateList.Count != WHEEL_ITEM_COUNT) return;

            for (int i = 0; i < WHEEL_ITEM_COUNT; ++i)
            {
                ClubArenaSpinResultType type = WheelCandidateList[i].GetValue<ClubArenaSpinResultType>("type");
                if (type == ClubArenaSpinResultType.BONUS || type == ClubArenaSpinResultType.STEAL || type == ClubArenaSpinResultType.SHIELD)
                    continue;

                long baseValue = 0;
                switch (type)
                {
                    case ClubArenaSpinResultType.POINT_10:
                    case ClubArenaSpinResultType.POINT_50:
                        baseValue = WheelCandidateList[i].GetValue<long>("point");
                        break;
                    //case ClubArenaSpinResultType.SHIELD_10:
                    //case ClubArenaSpinResultType.SHIELD_50:
                    //    baseValue = WheelCandidateList[i].GetValue<long>("shield");
                    //    break;
                    case ClubArenaSpinResultType.ATTACK_10:
                    case ClubArenaSpinResultType.ATTACK_20:
                    case ClubArenaSpinResultType.ATTACK_50:
                    case ClubArenaSpinResultType.ATTACK_100:
                        baseValue = WheelCandidateList[i].GetValue<long>("attack");
                        break;
                }

                MetaContextElementUtils.SimpleSetTextGlobal(wheelElements[i], "Text", "TEXT_COMMA_NUMBER", ContextSearchingType.ChildrenSearch, baseValue);
            }
        }

        private void SetBetText()
        {
            double multiValue = NumberUtils.GetMultiplierFromNumerator(ClubArenaUtils.CurrentBetMultiplyNumerator);
            double maxValue = NumberUtils.GetMultiplierFromNumerator(ClubArenaUtils.MaxBetMultiplyNumerator);
            long requiredEnergy = ClubArenaUtils.RequiredEnergy;
            if (betButtonElement != null)
                MetaContextElementUtils.SimpleSetTextGlobal(betButtonElement, "Text", "CLUB_ARENA_WHEEL_BET_BUTTON", ContextSearchingType.ChildrenSearch, multiValue);
            if (betSpeechBalloonTextElement != null)
                MetaContextElementUtils.SetTextGlobal(betSpeechBalloonTextElement, "CLUB_ARENA_WHEEL_BET_SPEECH_BALLOON", multiValue);
            if (betPopupTextElement != null)
                MetaContextElementUtils.SetTextGlobal(betPopupTextElement, "CLUB_ARENA_WHEEL_BET_POPUP", maxValue);
            if (spinButtonTextElement != null)
                MetaContextElementUtils.SetTextGlobal(spinButtonTextElement, "CLUB_ARENA_WHEEL_SPIN_BUTTON", multiValue * requiredEnergy);
        }

        private void SetActiveAnimation(string key, bool isActive)
        {
            if (rootAnimator != null)
                rootAnimator.SetBool(key, isActive);
        }

        private void SetActiveBetButton(bool isActive)
        {
            betButtonElement?.gameObject.SetActive(isActive);
        }

        public void SetEnergy(long energy)
        {
            energyController.SetEnergy(energy);
        }

        public void SetAppearEnergy(long addEnergy = 0)
        {
            energyController.SetAppearEnergy(addEnergy);
        }

        private bool GetActiveAnimation(string key)
        {
            return (rootAnimator != null) ? rootAnimator.GetBool(key) : false;
        }

        public void SetActiveSpinButton(bool isSpin, bool isReadyToAds)
        {
            if (isReadyToAds)
            {
                spinButtonInfoElement.gameObject.SetActive(isSpin);
                spinButtonAdElement.gameObject.SetActive(!isSpin);
            }
            else
            {
                spinButtonInfoElement.gameObject.SetActive(true);
                spinButtonAdElement.gameObject.SetActive(false);
            }
        }

        public void SetAdsTimerCallback(UnityAction callback)
        {
            ownerAdsTimerCallback = callback;
        }

        public void SetCompletedRewardItem(UnityAction<ClubArenaSpinResultType> callback)
        {
            ownerCompletedRewardItem = callback;
        }

        public void SetTriggerAnimation(string key)
        {
            rootAnimator?.SetTrigger(key);
        }

        public Animator GetAnimator()
        {
            return rootAnimator;
        }

        public ContextElement GetHighlightElement()
        {
            return highlightElement;
        }

        public float GetResultAngle()
        {
            int resultAngle = ClubArenaUtils.WheelResultIndex;
            resultAngle *= 36;
            return (float)resultAngle;
        }

        public long GetWheelBaseCandidateData(ClubArenaSpinResultType type)
        {
            Dictionary<ClubArenaSpinResultType, long> wheelBaseCandidateList = ClubArenaUtils.WheelBaseCandidateList;
            if (wheelBaseCandidateList == null || !wheelBaseCandidateList.ContainsKey(type)) return 0;
            return wheelBaseCandidateList[type];
        }

        public float GetWheelCurrentAngle(float addAngle = 0.0f)
        {
            float currentAngle = ((IContextFloatProperty)wheelContextElement).GetFloatProperty();
            currentAngle += 360.0f + addAngle;
            currentAngle /= 36.0f;
            return currentAngle;
        }

        public int GetWheelCurrentAngleInteger(float currentAngle)
        {
            int addInteger = 1;
            int currentAngleInteger = ((int)currentAngle + addInteger) % WHEEL_ITEM_COUNT;
            currentAngleInteger += addInteger;
            return currentAngleInteger;
        }

        public ContextElement GetWheelElement()
        {
            return wheelContextElement;
        }

        public int GetWheelItemCount()
        {
            return WHEEL_ITEM_COUNT;
        }

        public Transform GetWheelItemPoolTransform()
        {
            return wheelItemPoolElement.transform;
        }

        private void OnAdsTimerCallback()
        {
            if (ownerAdsTimerCallback != null)
                ownerAdsTimerCallback.Invoke();
        }

        public void OpenSpeechBalloon()
        {
            if (speechBalloonEnumerator != null)
                StopCoroutine(speechBalloonEnumerator);

            if (GetBetButtonActive())
                speechBalloonEnumerator = StartCoroutine(OpenSpeechBalloonEnumerator());
        }

        private IEnumerator OpenSpeechBalloonEnumerator()
        {
            if (GetActiveAnimation(WHEEL_BET_SPEECH_BALLOON))
            {
                SetActiveAnimation(WHEEL_BET_SPEECH_BALLOON, false);
                yield return new WaitForFixedUpdate();
            }

            SetActiveAnimation(WHEEL_BET_SPEECH_BALLOON, true);
            yield return new WaitForSeconds(1.5f);
            SetActiveAnimation(WHEEL_BET_SPEECH_BALLOON, false);
        }

        private IEnumerator BetUpgradeEnumerator(long multi)
        {
            SetActiveAnimation(WHEEL_BET_UPGRADE, false);
            yield return new WaitForFixedUpdate();
            SetActiveAnimation(WHEEL_BET_UPGRADE, true);
            yield return new WaitForSeconds(0.1f);
            //SetWheelData(multi);
        }

        public void SetAutoSpin(bool isAutoSpin)
        {
            if (spinButtonController != null)
                spinButtonController.AutoSpin = isAutoSpin;
        }

        public void SpinWheel(bool isSpin)
        {
            if (spinButtonController != null)
            {
                if (isSpin)
                    spinButtonController.SpinSlotMachine();
                else
                    spinButtonController.StoppedSlotMachine();
            }
        }

        private void UpdateAds(bool isActive, bool isReadyToAds)
        {
            if (isActive)
            {
                adSppeechBallonElement.gameObject.SetActive(false);
                wheelInactiveElement.gameObject.SetActive(false);
                return;
            }

            timerController.StopTimer();

            long nextAdsTime = ClubArenaUtils.LastVideoAdsClaimTimestamp + ClubArenaUtils.ClubArenaCooltime;
            long checkTime = nextAdsTime - TimeUtils.GetTimeStamp();
            if (checkTime < 0)
            {
                // coolTime
                adSppeechBallonElement.gameObject.SetActive(false);
                wheelInactiveElement.gameObject.SetActive(!isReadyToAds);
            }
            else
            {
                adSppeechBallonElement.gameObject.SetActive(true);
                wheelInactiveElement.gameObject.SetActive(true);
                timerController.StartTimer(nextAdsTime, 0);
            }
        }

        public void UpdateWheelData(bool isReadyToAds)
        {
            bool isSpin = ClubArenaUtils.CurrentEnergy >= ClubArenaUtils.RequiredEnergy;
            SetActiveSpinButton(isSpin, isReadyToAds);
            SetActiveBetButton(GetBetButtonActive());
            UpdateAds(isSpin, isReadyToAds);
        }

        private bool GetBetButtonActive()
        {
            long maxBet = ClubArenaUtils.MaxBetMultiplyNumerator;
            long currentBet = ClubArenaUtils.CurrentBetMultiplyNumerator;
            long baseBet = ClubArenaUtils.BaseBetMultiplyNumerator;

            return (maxBet > baseBet || currentBet > baseBet);
        }

        private void ClearRewardItem()
        {
            if (rewardItemController != null)
            {
                Destroy(rewardItemController);
                rewardItemController = null;
            }
        }

        private void CompletedRewardItem()
        {
            if (ownerCompletedRewardItem != null)
                ownerCompletedRewardItem.Invoke(rewardItemController.Type);

            ClearRewardItem();
        }

        public void CreateRewardItem(string bundleName, string assetName, ClubArenaSpinResultType type, Transform fromTransform, Transform toTransform, Transform rootTransform, int layerIndex)
        {
            ClearRewardItem();

            GameObject effectGO = MetaObjectUtils.MakePrefab(bundleName, assetName, rootTransform);
            Vector3 from = fromTransform.position;
            Vector3 to = toTransform.position;
            ChangeLayer(effectGO.transform, layerIndex);
            rewardItemController = effectGO.GetComponent<ClubArenaWheelRewardItemController>();
            rewardItemController?.OnInit(type);

            AsyncActionUtils.ApplyMovement(this, effectGO.transform, from, to, effectMovementTime, TweenUtils.VectorTweenCollectMove,
                0f, CompletedRewardItem);
        }

        private void ChangeLayer(Transform t, int layerIndex)
        {
            t.gameObject.layer = layerIndex;
            foreach (Transform child in t)
            {
                child.gameObject.layer = layerIndex;
                if (child.childCount > 0)
                    ChangeLayer(child.transform, layerIndex);
            }
        }
    }
}