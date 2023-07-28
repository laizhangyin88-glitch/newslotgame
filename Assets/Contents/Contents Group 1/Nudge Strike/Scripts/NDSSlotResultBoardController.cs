using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using ParadoxNotion;
using NodeCanvas.Framework;

namespace GameStudio.Slot.NDS
{
    public class NDSSlotResultBoardController : FeatureModule
    {
        public SlotMachine slotMachine;
        public ContextTextMeshProUGUI text;
        protected string ON_FEATURE_BEGIN_EVENT { get => "SlotCreditUpdate"; }
        protected const string ON_NUDGE_START_EVENT = "ReadyNudgeFeature";
        protected const string ON_SUPER_BONUS_NUDGE_START_EVENT = "ReadySuperBonusNudgeFeature";
        protected const string CHECK_NUDGE_EVENT = "CheckNudgeFeature";
        protected const string SUPER_BONUS_CHECK_NUDGE_EVENT = "CheckSuperBonusNudgeFeature";
        protected const string CHECK_SLOT_FRAME_NUDGE_RESET_EVENT = "SlotFrameNudgeReset";
        protected string RESET_FEATURE { get => "ResetSlotCreditUpdate"; }
        public bool isNudge = false;
        public bool isWin = false;
        protected override void OnEnable()
        {
            base.OnEnable();
            RegisterEvent(ON_FEATURE_BEGIN_EVENT, (EventData eventData) => StartCoroutine(Play(eventData)));
            RegisterEvent(RESET_FEATURE, (EventData eventData) => DisappearBoard());
            RegisterEvent(CHECK_NUDGE_EVENT, (EventData eventData) => OnCheckNudge());
            RegisterEvent(SUPER_BONUS_CHECK_NUDGE_EVENT, (EventData eventData) => OnCheckSuperBonusNudge());
            RegisterEvent(ON_NUDGE_START_EVENT, (EventData eventData) => OnReadyNudge());
            RegisterEvent(ON_SUPER_BONUS_NUDGE_START_EVENT, (EventData eventData) => OnReadyNudge());
            RegisterEvent(CHECK_SLOT_FRAME_NUDGE_RESET_EVENT, (EventData eventData) => OnSlotFrameReset());
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterEvent(ON_FEATURE_BEGIN_EVENT);
            UnRegisterEvent(RESET_FEATURE);
            UnRegisterEvent(CHECK_NUDGE_EVENT);
            UnRegisterEvent(SUPER_BONUS_CHECK_NUDGE_EVENT);
            UnRegisterEvent(ON_NUDGE_START_EVENT);
            UnRegisterEvent(ON_SUPER_BONUS_NUDGE_START_EVENT);
            UnRegisterEvent(CHECK_SLOT_FRAME_NUDGE_RESET_EVENT);
        }

        private IEnumerator Play(EventData eventData)
        {
            int slotIndex = slotMachine.slotIndex;
            long credit = ((List<long>)eventData.value)[slotIndex];
            yield return StartCoroutine(OnPlayCoroutine(credit));
        }
        private void DisappearBoard()
        {
            GetComponent<Animator>().SetBool("Win", false);
        }
        protected IEnumerator OnPlayCoroutine(long credit)
        {
            isWin = false;
            if (credit > 0)
            {
                text.SetText(FormatUtility.CommaNumberFormat(credit));
                GetComponent<Animator>().SetBool("Win", true);
                isWin = true;
            }
            yield break;
        }
        private void OnCheckNudge()
        {
            isNudge = false;
            int slotIndex = slotMachine.slotIndex;
            var slotData = ContentCustomData.GetSlotData(slotIndex);
            var deck = slotData.deck;
            for (int reelIndex = 0; reelIndex < 3; reelIndex++)
            {
                for (int rowIndex = 2; rowIndex < 5; rowIndex++)
                {
                    int symbolIndex = deck.GetDeckSymbol(reelIndex, rowIndex).symbol;
                    if (CheckNudge(symbolIndex, rowIndex))
                    {
                        isNudge = true;
                        return ;
                    }
                }
            }
        }
        private void OnCheckSuperBonusNudge()
        {
            isNudge = false;
            int slotIndex = slotMachine.slotIndex;
            var slotData = ContentCustomData.GetSlotData(slotIndex);
            var deck = slotData.deck;
            for (int reelIndex = 0; reelIndex < 3; reelIndex++)
            {
                bool isAppearWild = false;
                int nudgeCount = 0;
                for (int rowIndex = 2; rowIndex < 5; rowIndex++)
                {
                    var symbolInfo = deck.GetSymbol(reelIndex, rowIndex);
                    int symbolIndex = symbolInfo.symbol;

                    if (symbolIndex != 8)
                    {
                        nudgeCount++;
                    }
                    else
                    {
                        isAppearWild = true;
                    }
                }
                if (isAppearWild && nudgeCount > 0)
                {
                    isNudge = true;
                    return ;
                }
            }
        }
        private bool CheckNudge(int symbolIndex, int row)
        {
            if (symbolIndex != 0 && symbolIndex != 1)
            {
                return false;
            }

            if (symbolIndex == 0)
            {
                return row < 4;
            }
            else
            {
                return row > 2;
            }
        }
        private void OnReadyNudge()
        {
            FrameReset();
            if (isNudge)
            {
                GetComponent<Animator>().SetBool("Nudge", true);
            }
            else
            {
                GetComponent<Animator>().SetBool("Not Nudge", true);
            }
        }
        public void OnEndNudge()
        {
            GetComponent<Animator>().SetBool("Nudge", false);
        }
        private void OnSlotFrameReset()
        {
            FrameReset();
        }
        private void FrameReset()
        {
            GetComponent<Animator>().SetBool("Not Nudge", false);
            GetComponent<Animator>().SetBool("Nudge", false);
        }
    }
}
