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
    public class NDSDividerController : FeatureModule
    {
        private const string ON_SLOT_EVENT = "OnSlotEvent";
        protected const string ON_SLOT_SPIN_EVENT = "SpinSlot";
        protected const string ON_SLOT_WIN_EVENT = "SlotWin";
        protected const string ON_FORCE_SLOT_WIN_EVENT = "ForceSlotWin";
        protected const string ON_SLOT_DIVIDER_SET_IDLE = "SlotDividerIdle";
        protected const string ON_NUDGE_START_EVENT = "ReadyNudgeFeature";
        protected const string END_NUDGE_START_EVENT = "EndReadyNudgeFeature";
        protected const string ON_SUPER_BONUS_NUDGE_START_EVENT = "ReadySuperBonusNudgeFeature";
        protected const string END_SUPER_BONUS_NUDGE_START_EVENT = "EndReadySuperBonusNudgeFeature";
        protected const string CHECK_NUDGE_EVENT = "CheckNudgeFeature";
        protected const string SUPER_BONUS_CHECK_NUDGE_EVENT = "CheckSuperBonusNudgeFeature";
        protected const string SLOT_RESET_EVENT = "SlotReset";

        
        public List<GameObject> dividerList;
        public SlotMachine slotMachine;
        private Deck deck;
        public List<int> nudgeDirectionList = new List<int>(new int[] {0, 0, 0, 0});
        private bool isWinPlayed = false;
        protected override void OnEnable()
        {
            base.OnEnable();
            RegisterEvent(ON_SLOT_SPIN_EVENT, (EventData eventData) => OnSlotSpin());
            RegisterEvent(CHECK_NUDGE_EVENT, (EventData eventData) => OnCheckNudge());
            RegisterEvent(SUPER_BONUS_CHECK_NUDGE_EVENT, (EventData eventData) => OnSuperBonusCheckNudge());
            RegisterEvent(ON_NUDGE_START_EVENT, (EventData eventData) => StartCoroutine(OnReadyNudge()));
            RegisterEvent(ON_SUPER_BONUS_NUDGE_START_EVENT, (EventData eventData) => StartCoroutine(OnReadySuperBonusNudge()));
            RegisterEvent(SLOT_RESET_EVENT, (EventData eventData) => OnReset());
            RegisterEvent(ON_SLOT_DIVIDER_SET_IDLE, (EventData eventData) => OnReset());
            MessageDispatcher.Register(ON_SLOT_EVENT, OnSlotEvent);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterEvent(ON_SLOT_SPIN_EVENT);
            UnRegisterEvent(CHECK_NUDGE_EVENT);
            UnRegisterEvent(SUPER_BONUS_CHECK_NUDGE_EVENT);
            UnRegisterEvent(ON_NUDGE_START_EVENT);
            UnRegisterEvent(ON_SUPER_BONUS_NUDGE_START_EVENT);
            UnRegisterEvent(SLOT_RESET_EVENT);
            UnRegisterEvent(ON_SLOT_DIVIDER_SET_IDLE);
            MessageDispatcher.UnRegister(ON_SLOT_EVENT, OnSlotEvent);
        }

        public void OnPrepareStop(BaseReel reel)
        {
            if (reel.reelIndex == 0)
            {
                dividerList[0].GetComponent<Animator>().SetTrigger("Stop");
                dividerList[1].GetComponent<Animator>().SetTrigger("Stop");
            }
            else if (reel.reelIndex == 1)
            {
                dividerList[2].GetComponent<Animator>().SetTrigger("Stop");
            }
            else
            {
                dividerList[3].GetComponent<Animator>().SetTrigger("Stop");
            }
        }

        public void OnSlotSpin()
        {
            isWinPlayed = false;
            for (int i = 0; i < dividerList.Count; i++)
            {
                dividerList[i].GetComponent<Animator>().SetTrigger("Spin");
            }
        }
        private IEnumerator OnReadyNudge()
        {
            yield return StartCoroutine(ReadyNudge());

            ContentEvent.SendEvent(END_NUDGE_START_EVENT);
        }
        private IEnumerator OnReadySuperBonusNudge()
        {
            yield return StartCoroutine(ReadyNudge());

            ContentEvent.SendEvent(END_SUPER_BONUS_NUDGE_START_EVENT);
        }
        private IEnumerator ReadyNudge()
        {
            for (int i = 0; i < dividerList.Count; i++)
            {
                if (nudgeDirectionList[i] != 0)
                {
                    dividerList[i].GetComponent<Animator>().SetTrigger("ReadyNudge");
                }
                else
                {
                    dividerList[i].GetComponent<Animator>().SetTrigger("Idle");
                }
                dividerList[i].GetComponent<Animator>().SetInteger("Direction", nudgeDirectionList[i]);
            }
            isWinPlayed = false;
            yield break;
        }
        private void OnCheckNudge()
        {
            int slotIndex = slotMachine.slotIndex;
            var slotData = ContentCustomData.GetSlotData(slotIndex);
            var deck = slotData.deck;
            nudgeDirectionList = new List<int>(new int[] {0, 0, 0, 0});
            for (int reelIndex = 0; reelIndex < 3; reelIndex++)
            {
                for (int rowIndex = 2; rowIndex < 5; rowIndex++)
                {
                    int symbolIndex = deck.GetDeckSymbol(reelIndex, rowIndex).symbol;
                    if (CheckNudge(symbolIndex, rowIndex))
                    {
                        if (nudgeDirectionList[reelIndex] == 0)
                        {
                            nudgeDirectionList[reelIndex] = symbolIndex == 0 ? -1 : 1;
                        }
                        nudgeDirectionList[reelIndex + 1] = symbolIndex == 0 ? -1 : 1;
                    }
                }
            }
        }
        private void OnSuperBonusCheckNudge()
        {
            int slotIndex = slotMachine.slotIndex;
            var slotData = ContentCustomData.GetSlotData(slotIndex);
            var deck = slotData.deck;
            nudgeDirectionList = new List<int>(new int[] {0, 0, 0, 0});
            for (int reelIndex = 0; reelIndex < 3; reelIndex++)
            {
                int normalSymbolCount = 0;
                int nudgeDirection = 0;
                for (int rowIndex = 2; rowIndex < 5; rowIndex++)
                {
                    int symbolIndex = deck.GetDeckSymbol(reelIndex, rowIndex).symbol;
                    if (symbolIndex == 8)
                    {
                        if (normalSymbolCount == 0)
                        {
                            nudgeDirection = -1;
                        }
                        else
                        {
                            nudgeDirection = 1;
                        }
                    }
                    else
                    {
                        normalSymbolCount++;
                    }
                }
                if (normalSymbolCount > 0 && nudgeDirection != 0)
                {
                    if (nudgeDirectionList[reelIndex] == 0)
                    {
                        nudgeDirectionList[reelIndex] = nudgeDirection;
                    }
                    nudgeDirectionList[reelIndex + 1] = nudgeDirection;
                }
            }
        }
        public void Nudge()
        {
            for (int i = 0; i < dividerList.Count; i++)
            {
                if (nudgeDirectionList[i] != 0)
                {
                    dividerList[i].GetComponent<Animator>().SetTrigger("Nudge");
                }
            }
        }

        public void OnSlotWin()
        {
            for (int i = 0; i < dividerList.Count; i++)
            {
                dividerList[i].GetComponent<Animator>().SetTrigger("Win");
            }
        }
        protected virtual void OnSlotEvent(EventData receivedEvent)
        {
            if (receivedEvent.id != slotMachine.slotIndex) return;
            if (receivedEvent.name.Equals(ON_SLOT_WIN_EVENT, StringComparison.Ordinal))
            {
                if (!isWinPlayed)
                {
                    isWinPlayed = true;
                    OnSlotWin();
                }
            }
            if (receivedEvent.name.Equals(ON_FORCE_SLOT_WIN_EVENT, StringComparison.Ordinal))
            {
                isWinPlayed = true;
                OnSlotWin();
            }
        }
        private void OnReset()
        {
            for (int i = 0; i < dividerList.Count; i++)
            {
                dividerList[i].GetComponent<Animator>().SetTrigger("Idle");
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
