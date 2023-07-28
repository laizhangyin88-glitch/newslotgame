using System;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion;
using UnityEngine;
using SlotMaker;

namespace GameStudio.Slot.NDS
{
    public class NDSSymbolReadyNudgeController : FeatureModule
    {
        private const string ON_SLOT_EVENT = "OnSlotEvent";
        protected const string ON_NUDGE_START_EVENT = "PlayWildReadyNudge";
        protected const string ON_SUPER_BONUS_NUDGE_START_EVENT = "PlayWildReadySuperBonusNudge";
        private const int SUPER_BONUS_WILD = 8;
        private const int DOWN = 3;
        private const int UP = 4;

        public SlotMachine slotMachine;
        protected override void OnEnable()
        {
            base.OnEnable();
            MessageDispatcher.Register(ON_SLOT_EVENT, OnSlotEvent);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            MessageDispatcher.UnRegister(ON_SLOT_EVENT, OnSlotEvent);
        }
        protected virtual void OnSlotEvent(EventData receivedEvent)
        {
            if (receivedEvent.id != slotMachine.slotIndex) return;
            if (receivedEvent.name.Equals(ON_NUDGE_START_EVENT, StringComparison.Ordinal))
            {
                OnReadyNudge();
            }
            else if (receivedEvent.name.Equals(ON_SUPER_BONUS_NUDGE_START_EVENT, StringComparison.Ordinal))
            {
                OnReadySuperBonusNudge(((EventData<List<int>>)receivedEvent).value);
            }
        }
        public void OnReadyNudge()
        {
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
                        BaseSymbol symbol = slotMachine.GetSymbol(reelIndex, rowIndex);
                        symbol.Play("ReadyNudge");
                    }
                }
            }
        }
        public void OnReadySuperBonusNudge(List<int> nudgeDirList)
        {
            int slotIndex = slotMachine.slotIndex;
            var slotData = ContentCustomData.GetSlotData(slotIndex);
            var deck = slotData.deck;
            for (int reelIndex = 0; reelIndex < 3; reelIndex++)
            {
                if (nudgeDirList[reelIndex] == 0) continue;
                int nudgeDirection = nudgeDirList[reelIndex];
                BaseSymbol symbol = null;
                for (int rowIndex = 2; rowIndex < 5; rowIndex++)
                {
                    int symbolIndex = deck.GetDeckSymbol(reelIndex, rowIndex).symbol;
                    if (symbolIndex == SUPER_BONUS_WILD)
                    {
                        symbol = slotMachine.GetSymbol(reelIndex, rowIndex);
                        if (nudgeDirection == UP)
                        {
                            break;
                        }
                    }
                }
                if (nudgeDirection == DOWN)
                {
                    symbol.Play("ReadyNudgeDown");
                }
                else if (nudgeDirection == UP)
                {
                    symbol.Play("ReadyNudgeUp");
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
    }
}
