using UnityEngine;
using SlotMaker;
using BagelCode.ClientModels;
using NodeCanvas.Framework;

#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace BagelCode
{
    public class EpicPassInfoSpeechBalloonController : MonoBehaviour
    {
        public float displayTime = 3f;

        private bool isAnimActive;
        private float passedTime;

        private Animator rootAnimator;
        private ContextElement rootElement;

        // private ContextElement topIconElement;

        private ContextElement winPointElement;
        private ContextElement bigWinPointElement;
        private ContextElement superBigWinPointElement;
        private ContextElement megaWinPointElement;
        private ContextElement superMegaWinPointElement;
        private ContextElement epicWinPointElement;

        private bool isInit = false;
        private bool isReady = false;

        private long totalBet;

        private const string ELIGIBLA_POIN_TEXT_FORMAT = "EPIC_PASS_SPEECH_BALLOON_POINT_TEXT";

        //

        public void OnReadyGame()
        {
            isReady = true;
            if (!isInit) return;

            var totalBetCredit = BlackboardUtils.FindVariable<long>("./totalBetCredit");
            if(totalBetCredit != null)
                UpdateTotalBet(totalBetCredit.value);
        }

        public void UpdateTotalBet(long totalBetCredit)
        {
            totalBet = totalBetCredit;

            if (!isInit) return;

            UpdateWinPointElement();
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

            // topIconElement = ContextUtils.FindElement(rootElement, "Top Icon", ContextSearchingType.ChildrenSearch);
            // MetaContextElementUtils.SetWebImage(topIconElement, EpicPassUtils.PointImageUrl);

            var infoBaseAreaElement = ContextUtils.FindElement(rootElement, "Info Base Area", ContextSearchingType.ChildrenSearch);

            winPointElement = ContextUtils.FindElement(infoBaseAreaElement, "Info Base Win/Text", ContextSearchingType.FullNameSearch);
            bigWinPointElement = ContextUtils.FindElement(infoBaseAreaElement, "Info Base Big Win/Text", ContextSearchingType.FullNameSearch);
            superBigWinPointElement = ContextUtils.FindElement(infoBaseAreaElement, "Info Base Super Big Win/Text", ContextSearchingType.FullNameSearch);
            megaWinPointElement = ContextUtils.FindElement(infoBaseAreaElement, "Info Base Mega Win/Text", ContextSearchingType.FullNameSearch);
            superMegaWinPointElement = ContextUtils.FindElement(infoBaseAreaElement, "Info Base Super Mega Win/Text", ContextSearchingType.FullNameSearch);
            epicWinPointElement = ContextUtils.FindElement(infoBaseAreaElement, "Info Base Epic Win/Text", ContextSearchingType.FullNameSearch);

            var winTextElement = ContextUtils.FindElement(infoBaseAreaElement, "Info Base Win/Title Text", ContextSearchingType.FullNameSearch);
            var bigWinTextElement = ContextUtils.FindElement(infoBaseAreaElement, "Info Base Big Win/Title Text", ContextSearchingType.FullNameSearch);
            var superBigWinTextElement = ContextUtils.FindElement(infoBaseAreaElement, "Info Base Super Big Win/Title Text", ContextSearchingType.FullNameSearch);
            var megaWinTextElement = ContextUtils.FindElement(infoBaseAreaElement, "Info Base Mega Win/Title Text", ContextSearchingType.FullNameSearch);
            var superMegaWinTextElement = ContextUtils.FindElement(infoBaseAreaElement, "Info Base Super Mega Win/Title Text", ContextSearchingType.FullNameSearch);
            var epicWinTextElement = ContextUtils.FindElement(infoBaseAreaElement, "Info Base Epic Win/Title Text", ContextSearchingType.FullNameSearch);

            MetaContextElementUtils.SetTextGlobal(winTextElement, "EPIC_PASS_SPEECH_BALLOON_WIN_TEXT_00");
            MetaContextElementUtils.SetTextGlobal(bigWinTextElement, "EPIC_PASS_SPEECH_BALLOON_WIN_TEXT_01");
            MetaContextElementUtils.SetTextGlobal(superBigWinTextElement, "EPIC_PASS_SPEECH_BALLOON_WIN_TEXT_02");
            MetaContextElementUtils.SetTextGlobal(megaWinTextElement, "EPIC_PASS_SPEECH_BALLOON_WIN_TEXT_03");
            MetaContextElementUtils.SetTextGlobal(superMegaWinTextElement, "EPIC_PASS_SPEECH_BALLOON_WIN_TEXT_04");
            MetaContextElementUtils.SetTextGlobal(epicWinTextElement, "EPIC_PASS_SPEECH_BALLOON_WIN_TEXT_05");

            isInit = true;
        }

        //

        public void Display(bool isActive)
        {
            if(isActive)
            {
                passedTime = 0f;
                isAnimActive = true;
                rootAnimator.SetBool("IsActive", true);
            }
            else
            {
                passedTime = displayTime;
            }
        }

        private void Start()
        {
            InitProperty();

            if(isReady)
            {
                // Update & Show Display
                var totalBetCredit = BlackboardUtils.FindVariable<long>("./totalBetCredit");
                if(totalBetCredit != null)
                    UpdateTotalBet(totalBetCredit.value);
            }
        }

        private void Update()
        {
            if(isAnimActive)
            {
                if (passedTime < displayTime)
                {
                    passedTime += Time.deltaTime;
                }
                else
                {
                    passedTime = 0f;
                    isAnimActive = false;
                    rootAnimator.SetBool("IsActive", false);
                }
            }
        }

        private void UpdateWinPointElement()
        {
            // long eligiblePoint = EpicPassUtils.GetEligiblePoint(totalBet);
            Blackboard eligibleBetInfoBB = EpicPassUtils.GetEligibleBetInfoBB(totalBet);
            bool isEligible = eligibleBetInfoBB != null;

            if(isEligible)
            {
                MetaContextElementUtils.SetTextGlobal(winPointElement, ELIGIBLA_POIN_TEXT_FORMAT, eligibleBetInfoBB.GetValue<long>("point"));
                MetaContextElementUtils.SetTextGlobal(bigWinPointElement, ELIGIBLA_POIN_TEXT_FORMAT, EpicPassUtils.GetWinTypePoint(eligibleBetInfoBB, WinType.BIG) );
                MetaContextElementUtils.SetTextGlobal(superBigWinPointElement, ELIGIBLA_POIN_TEXT_FORMAT, EpicPassUtils.GetWinTypePoint(eligibleBetInfoBB, WinType.SUPER_BIG) );
                MetaContextElementUtils.SetTextGlobal(megaWinPointElement, ELIGIBLA_POIN_TEXT_FORMAT, EpicPassUtils.GetWinTypePoint(eligibleBetInfoBB, WinType.MEGA) );
                MetaContextElementUtils.SetTextGlobal(superMegaWinPointElement, ELIGIBLA_POIN_TEXT_FORMAT, EpicPassUtils.GetWinTypePoint(eligibleBetInfoBB, WinType.SUPER_MEGA) );
                MetaContextElementUtils.SetTextGlobal(epicWinPointElement, ELIGIBLA_POIN_TEXT_FORMAT, EpicPassUtils.GetWinTypePoint(eligibleBetInfoBB, WinType.EPIC) );
            }

            Display(isEligible && !MetaGameUtils.IsMetaGameLevelLocked());
        }

#if UNITY_EDITOR
        [Button]
        public void TestOnUpdateBet()
        {
            var totalBetCredit = BlackboardUtils.FindVariable<long>("./totalBetCredit");
            if(totalBetCredit != null)
                UpdateTotalBet(totalBetCredit.value);
        }
#endif
    }
}
