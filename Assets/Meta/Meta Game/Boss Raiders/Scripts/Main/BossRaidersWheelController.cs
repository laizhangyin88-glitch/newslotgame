using UnityEngine;
using System.Collections;
using SlotMaker;
using BagelCode.MetaGame;

namespace BagelCode.BossRaiders
{
    public class BossRaidersWheelController : BossRaidersWheelControllerBase
    {
        private ContextElement betButtonElement;
        private ContextElement betSpeechBalloonTextElement;
        private ContextElement betPopupTextElement;
        private ContextElement spinButtonElement;
        private ContextElement spinButtonTextElement;

        private ContextElement spinButtonInfoElement;
        private ContextElement spinButtonAdElement;

        private ContextElement adSppeechBallonElement;
        private ContextElement adSppeechBallonTextElement;

        private bool isInit = false;

        private const int WHEEL_ITEM_COUNT = 12;

        private RemainingTimerController timerController;

        private Coroutine betUpgradeEffectEnumerator = null;
        private Coroutine speechBalloonEnumerator = null;

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

        public override void OnInit(ContextElement caller)
        {
            if (isInit) return;

            base.OnInit(caller);

            OpenSpeechBalloon();
            isInit = true;
        }

        protected override void InitProperty()
        {
            betButtonElement = ContextUtils.FindElement(rootElement, "Bet Button", ContextSearchingType.ChildrenSearch);

            ContextElement betSpeechBalloonElement = ContextUtils.FindElement(rootElement, "Bet Speech Balloon", ContextSearchingType.ChildrenSearch);
            betSpeechBalloonTextElement = ContextUtils.FindElement(betSpeechBalloonElement, "Text", ContextSearchingType.ChildrenSearch);
            ContextElement betPopupElement = ContextUtils.FindElement(rootElement, "Bet Popup", ContextSearchingType.ChildrenSearch);
            betPopupTextElement = ContextUtils.FindElement(betPopupElement, "Text", ContextSearchingType.ChildrenSearch);

            spinButtonElement = ContextUtils.FindElement(rootElement, "Button Spin", ContextSearchingType.ChildrenSearch);
            spinButtonInfoElement = ContextUtils.FindElement(spinButtonElement, "Info", ContextSearchingType.ChildrenSearch);
            spinButtonTextElement = ContextUtils.FindElement(spinButtonInfoElement, "Text Spin Energy", ContextSearchingType.ChildrenSearch);
            spinButtonAdElement = ContextUtils.FindElement(spinButtonElement, "Ad", ContextSearchingType.ChildrenSearch);

            adSppeechBallonElement = ContextUtils.FindElement(spinButtonAdElement, "Ad Speech Balloon", ContextSearchingType.ChildrenSearch);
            adSppeechBallonTextElement = ContextUtils.FindElement(adSppeechBallonElement, "Text Time", ContextSearchingType.ChildrenSearch);

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
        }

        protected override void InitWheelController()
        {
            ContextElement wheelElement = ContextUtils.FindElement(rootElement, "Wheel", ContextSearchingType.ChildrenSearch);

            wheelController = new BossRaidersWheel();
            wheelController.OnInit(wheelElement, rootAnimator, WHEEL_ITEM_COUNT);
            wheelController.InitInactiveElement(ContextUtils.FindElement(rootElement, "Wheel Inactive", ContextSearchingType.ChildrenSearch));
        }

        protected override void InitSpinButtonController()
        {
            spinButtonController = spinButtonElement.GetComponent<MetaGameSpinButton>();
        }

        protected override void InitWheelData()
        {
            SetWheelData(BossRaidersUtils.GetDefaultBetMultiplyNumerator());
            SetBetText();
        }

        public override void ChangeWheelData(long multi)
        {
            SetBetText();

            if (betUpgradeEffectEnumerator != null)
                StopCoroutine(betUpgradeEffectEnumerator);

            betUpgradeEffectEnumerator = StartCoroutine(BetUpgradeEnumerator(multi));

            OpenSpeechBalloon();

            if (BossRaidersUtils.CurrentBetMultiplyNumerator == BossRaidersUtils.MaxBetMultiplyNumerator && BossRaidersUtils.MaxBetMultiplyNumerator != BossRaidersUtils.BaseBetMultiplyNumerator)
                SetTriggerAnimation(WHEEL_BET_POPUP);
        }

        private void SetBetText()
        {
            double multiValue = NumberUtils.GetMultiplierFromNumerator(BossRaidersUtils.CurrentBetMultiplyNumerator);
            double maxValue = NumberUtils.GetMultiplierFromNumerator(BossRaidersUtils.MaxBetMultiplyNumerator);
            long requiredEnergy = BossRaidersUtils.RequiredEnergy;
            if (betButtonElement != null)
                MetaContextElementUtils.SimpleSetTextGlobal(betButtonElement, "Text", "BOSS_RAIDERS_WHEEL_BET_BUTTON", ContextSearchingType.ChildrenSearch, multiValue);
            if (betSpeechBalloonTextElement != null)
                MetaContextElementUtils.SetTextGlobal(betSpeechBalloonTextElement, "BOSS_RAIDERS_WHEEL_BET_SPEECH_BALLOON", multiValue);
            if (betPopupTextElement != null)
                MetaContextElementUtils.SetTextGlobal(betPopupTextElement, "BOSS_RAIDERS_WHEEL_BET_POPUP", maxValue);
            if (spinButtonTextElement != null)
                MetaContextElementUtils.SetTextGlobal(spinButtonTextElement, "BOSS_RAIDERS_WHEEL_SPIN_BUTTON", multiValue * requiredEnergy);
        }

        private void SetActiveBetButton(bool isActive)
        {
            betButtonElement?.gameObject.SetActive(isActive);
        }

        private bool GetActiveAnimation(string key)
        {
            return (rootAnimator != null) ? rootAnimator.GetBool(key) : false;
        }

        public override void SetActiveSpinButton(bool isSpin, bool isReadyToAds)
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

        private void OnAdsTimerCallback()
        {
            if (ownerAdsTimerCallback != null)
                ownerAdsTimerCallback.Invoke();
        }

        public override void OpenSpeechBalloon()
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
            SetWheelData(multi);
        }

        private void UpdateAds(bool isActive, bool isReadyToAds)
        {
            if (isActive)
            {
                adSppeechBallonElement.gameObject.SetActive(false);
                wheelController?.SetInactiveObject(false);
                return;
            }

            timerController.StopTimer();

            long nextAdsTime = BossRaidersUtils.LastVideoAdsClaimTimestamp + BossRaidersUtils.BossRaidersCooltime;
            long checkTime = nextAdsTime - TimeUtils.GetTimeStamp();
            if (checkTime < 0)
            {
                // coolTime
                adSppeechBallonElement.gameObject.SetActive(false);
                wheelController?.SetInactiveObject(!isReadyToAds);
            }
            else
            {
                adSppeechBallonElement.gameObject.SetActive(true);
                wheelController?.SetInactiveObject(true);
                timerController.StartTimer(nextAdsTime, 0);
            }
        }

        public override void UpdateWheelData(bool isReadyToAds)
        {
            bool isSpin = BossRaidersUtils.CurrentEnergy >= BossRaidersUtils.RequiredEnergy;
            SetActiveSpinButton(isSpin, isReadyToAds);
            SetActiveBetButton(GetBetButtonActive());
            UpdateAds(isSpin, isReadyToAds);
        }

        private bool GetBetButtonActive()
        {
            long maxBet = BossRaidersUtils.MaxBetMultiplyNumerator;
            long currentBet = BossRaidersUtils.CurrentBetMultiplyNumerator;
            long baseBet = BossRaidersUtils.BaseBetMultiplyNumerator;

            return (maxBet > baseBet || currentBet > baseBet);
        }
    }
}