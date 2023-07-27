using UnityEngine;
using SlotMaker;

#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace BagelCode.EpicPass
{
    public class EpicPassV2BetUpSpeechBalloonController : MonoBehaviour
    {
        public float displayTime = 3f;

        private bool isAnimActive;
        private float passedTime = 0.0f;
        private float waitTime = 0.1f;
        private float gaugeTime = 0.0f;
        private float gauge = 0.0f;
        private float targetGauge;
        private float startGauge;

        private Animator rootAnimator;
        private ContextElement rootElement;

        private ContextElement bottomTextElement;
        private ContextElement webImageElement;
        private ContextSlider betProgressBarSlider;

        private bool isInit = false;
        private bool isReady = false;

        private long totalBet;
        private long qualifiedBetAmount;

        public void OnReadyGame()
        {
            isReady = true;
            if (!isInit) return;

            var totalBetCredit = BlackboardUtils.FindVariable<long>("./totalBetCredit");
            if (totalBetCredit != null)
                UpdateTotalBet(totalBetCredit.value);
        }

        public void UpdateTotalBet(long totalBetCredit)
        {
            totalBet = totalBetCredit;

            if (!isInit) return;

            Display(true);
        }

        public void OnEnterTurn()
        {
            if (!isInit) return;

            Display(false);
        }

        public void InitProperty()
        {
            if (isInit) return;

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);
            rootAnimator = gameObject.GetComponent<Animator>();

            bottomTextElement = ContextUtils.FindElement(rootElement, "Bottom Text", ContextSearchingType.ChildrenSearch);

            ContextElement infoProgressBarAreaElement = ContextUtils.FindElement(rootElement, "Info Progress Bar Area", ContextSearchingType.ChildrenSearch);
            webImageElement = ContextUtils.FindElement(infoProgressBarAreaElement, "Epic Pass Always Point Web Image", ContextSearchingType.ChildrenSearch);
            ContextElement progressBarElement = ContextUtils.FindElement(infoProgressBarAreaElement, "Bet Progress/Progress Bar", ContextSearchingType.FullNameSearch);
            betProgressBarSlider = progressBarElement.GetComponent<ContextSlider>();

            qualifiedBetAmount = BlackboardUtils.FindValue<long>("./metaEligibleBetThreshold/epicPassV2BetAmount");

            MetaContextElementUtils.SetTextGlobal(bottomTextElement, "EPIC_PASS_ALWAYS_IN_GAME_ACTIVE_TEXT");

            isInit = true;
        }

        //

        public void Display(bool isActive)
        {
            isAnimActive = isActive;
            if (isActive)
            {
                passedTime = 0f;
                gaugeTime = 0.0f;
                UpdateProgressBar();
                isAnimActive = !MetaGameUtils.IsMetaGameLevelLocked() && targetGauge > 0.0f && totalBet >= qualifiedBetAmount;
            }

            if (rootAnimator != null)
                rootAnimator.SetBool("IsActive", isAnimActive);
        }

        private void Start()
        {
            InitProperty();

            if (isReady)
            {
                // Update & Show Display
                var totalBetCredit = BlackboardUtils.FindVariable<long>("./totalBetCredit");
                if (totalBetCredit != null)
                    UpdateTotalBet(totalBetCredit.value);
            }
        }

        private void Update()
        {
            if (isAnimActive && isInit)
            {
                if (passedTime < displayTime)
                    passedTime += Time.deltaTime;
                else
                    Display(false);

                gaugeTime += Time.deltaTime;
                gauge = startGauge + gaugeTime / waitTime * (targetGauge - startGauge);
                betProgressBarSlider.SetFloatProperty(gauge);

                if (gaugeTime >= waitTime)
                {
                    gauge = targetGauge;
                    betProgressBarSlider.SetFloatProperty(gauge);
                }
            }
        }

        private void UpdateProgressBar()
        {
            targetGauge = EpicPassUtilsV2.GetCurrentGaugeAsFloat(totalBet);
            if (isAnimActive)
                startGauge = gauge;
            else
            {
                betProgressBarSlider.SetFloatProperty(targetGauge);
                startGauge = targetGauge;
                gauge = targetGauge;
            }
        }

#if UNITY_EDITOR
        [Button]
        public void TestOnUpdateBet()
        {
            var totalBetCredit = BlackboardUtils.FindVariable<long>("./totalBetCredit");
            if (totalBetCredit != null)
                UpdateTotalBet(totalBetCredit.value);
        }
#endif
    }
}