using UnityEngine;
using SlotMaker;

#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace BagelCode.EpicPass
{
    public class EpicPassV2InfoSpeechBalloonController : MonoBehaviour
    {
        public float displayTime = 3f;

        private bool isAnimActive;
        private float passedTime = 0.0f;

        private Animator rootAnimator;
        private ContextElement rootElement;

        private ContextElement bottomTextElement;

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

            ContextElement topTextElement = ContextUtils.FindElement(rootElement, "Top Text", ContextSearchingType.ChildrenSearch);
            bottomTextElement = ContextUtils.FindElement(rootElement, "Bottom Text", ContextSearchingType.ChildrenSearch);

            qualifiedBetAmount = BlackboardUtils.FindValue<long>("./metaEligibleBetThreshold/epicPassV2BetAmount");

            MetaContextElementUtils.SetTextGlobal(topTextElement, "EPIC_PASS_ALWAYS_IN_GAME_INACTIVE_TITLE");
            MetaContextElementUtils.SetTextGlobal(bottomTextElement, "EPIC_PASS_ALWAYS_IN_GAME_INACTIVE_TEXT", qualifiedBetAmount);

            isInit = true;
        }

        public void Display(bool isActive)
        {
            isAnimActive = isActive;
            if (isActive)
            {
                passedTime = 0f;
                isAnimActive = !MetaGameUtils.IsMetaGameLevelLocked() && qualifiedBetAmount > totalBet;
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