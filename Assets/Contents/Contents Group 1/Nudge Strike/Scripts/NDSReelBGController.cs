using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using ParadoxNotion;
using ParadoxNotion.Services;
using NodeCanvas.Framework;

namespace GameStudio.Slot.NDS
{
    public class NDSReelBGController : FeatureModule
    {
        protected const string ON_WIN_EVENT = "OnWinEvent";
        protected const string ON_TOTAL_WIN_EVENT = "TotalWin";
        protected const string ON_TOTAL_WIN_LINE_EVENT = "TotalWinLine";
        protected const string ON_JACKPOT_WIN_EVENT = "JackpotWin";
        protected const string ON_SINGLE_WIN_EVENT = "SingleWin";
        protected const string ON_SKIP_WIN_EVENT = "SkipWin";

        protected const string ON_SLOT_SPIN_EVENT = "SpinSlot";
        protected const string ON_END_NUDGE = "NudgeEnd";
        protected const string ON_NUDGE_START_EVENT = "ReadyNudgeFeature";
        protected const string ON_SUPER_BONUS_NUDGE_START_EVENT = "ReadySuperBonusNudgeFeature";
        protected const string SLOT_RESET_EVENT = "SlotReset";

        private bool isWinPlayed = false;

        public bool isNeedWildStop = false;

        public SlotMachine slotMachine;
        public int reelIndex;

        public void OnPrepareStop()
        {
            GetComponent<Animator>().SetBool("WildStop", isNeedWildStop);
            GetComponent<Animator>().SetTrigger("Stop");
        }
        public void OnEndNudge()
        {
            GetComponent<Animator>().SetBool("Nudge", false);
        }
        public void ReadyNudge()
        {
            int slotIndex = slotMachine.slotIndex;
            var slotData = ContentCustomData.GetSlotData(slotIndex);
            var deck = slotData.deck;

            isNeedWildStop = false;
            for (int rowIndex = 2; rowIndex < 5; rowIndex++)
            {
                int symbolIndex = deck.GetDeckSymbol(reelIndex, rowIndex).symbol;
                if (CheckNudge(symbolIndex, rowIndex))
                {
                    isNeedWildStop = true;
                }
            }
            GetComponent<Animator>().SetBool("Nudge", isNeedWildStop);
        }
        public void ReadySuperBonusNudge()
        {
            int slotIndex = slotMachine.slotIndex;
            var slotData = ContentCustomData.GetSlotData(slotIndex);
            var deck = slotData.deck;

            isNeedWildStop = false;
            
            int nudgeCount = 0;
            bool isAppearWild = false;
            for (int rowIndex = 2; rowIndex < 5; rowIndex++)
            {
                int symbolIndex = deck.GetDeckSymbol(reelIndex, rowIndex).symbol;
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
                isNeedWildStop = true;
            }
            GetComponent<Animator>().SetBool("Nudge", isNeedWildStop);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            MessageDispatcher.Register(ON_WIN_EVENT, OnWinEvent);
            RegisterEvent(ON_SLOT_SPIN_EVENT, (EventData eventData) => OnSlotSpin());
            RegisterEvent(ON_NUDGE_START_EVENT, (EventData eventData) => ReadyNudge());
            RegisterEvent(ON_SUPER_BONUS_NUDGE_START_EVENT, (EventData eventData) => ReadySuperBonusNudge());
            RegisterEvent(SLOT_RESET_EVENT, (EventData eventData) => OnSlotReset());
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            MessageDispatcher.UnRegister(ON_WIN_EVENT, OnWinEvent);
            UnRegisterEvent(ON_SLOT_SPIN_EVENT);
            UnRegisterEvent(ON_NUDGE_START_EVENT);
            UnRegisterEvent(ON_SUPER_BONUS_NUDGE_START_EVENT);
            UnRegisterEvent(SLOT_RESET_EVENT);
        }

        protected virtual void OnWinEvent(EventData receivedEvent)
        {
            if (receivedEvent.id != slotMachine.slotIndex) return;

            if (receivedEvent.name.Equals(ON_TOTAL_WIN_EVENT, StringComparison.Ordinal) || receivedEvent.name.Equals(ON_TOTAL_WIN_LINE_EVENT, StringComparison.Ordinal) || receivedEvent.name.Equals(ON_JACKPOT_WIN_EVENT, StringComparison.Ordinal))
            {
                if (!isWinPlayed)
                {
                    isWinPlayed = true;
                    OnWin();
                }
            }
        }

        void OnWin()
        {
            GetComponent<Animator>().SetTrigger("Win");
        }

        void OnSlotSpin()
        {
            isWinPlayed = false;
            GetComponent<Animator>().SetTrigger("Spin");
        }

        private void OnSlotReset()
        {
            GetComponent<Animator>().SetTrigger("Idle");
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
    }

}
