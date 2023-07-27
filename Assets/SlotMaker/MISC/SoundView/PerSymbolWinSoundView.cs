using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using NodeCanvas.Framework;
using ParadoxNotion.Services;
using Sirenix.OdinInspector;

namespace SlotMaker
{
    public class PerSymbolWinSoundView : MonoBehaviour
    {
        protected const string ON_WIN_EVENT = "OnWinEvent";
        protected const string ON_TOTAL_WIN_EVENT = "TotalWin";
        protected const string ON_SINGLE_WIN_EVENT = "SingleWin";
        protected const string ON_SKIP_WIN_EVENT = "SkipWin";

        public MessageRouter router;

        public string totalWinSound;
        public List<string> symbolWinSounds;
        public bool playOncePerSymbol;
        [ShowIf("playOncePerSymbol", true)]
        public List<bool> playedOncePerSymbolList;
        private List<int> playedSlotIndexList;

        private void Awake()
        {
            MessageDispatcher.Register(ON_WIN_EVENT, OnWinEvent);

            if (playOncePerSymbol)
                playedSlotIndexList = new List<int>(new int[playedOncePerSymbolList.Count]);
        }

        protected virtual void OnDestroy()
        {
            MessageDispatcher.UnRegister(ON_WIN_EVENT, OnWinEvent);
        }

        protected void OnWinEvent(EventData receivedEvent)
        {
            if (receivedEvent.name.Equals(ON_SKIP_WIN_EVENT, StringComparison.Ordinal))
                OnSkipWin(receivedEvent.id);
            else if (receivedEvent.name.Equals(ON_SINGLE_WIN_EVENT, StringComparison.Ordinal))
                OnWin((SymbolWin)receivedEvent.value, receivedEvent.id);
            else if (receivedEvent.name.Equals(ON_TOTAL_WIN_EVENT, StringComparison.Ordinal))
                OnTotalWin((List<SymbolWin>)receivedEvent.value);
        }

        protected virtual void OnTotalWin(List<SymbolWin> winList)
        {
            if (!string.IsNullOrEmpty(totalWinSound))
                GSManager.Instance.GetHandler(totalWinSound).Play();
        }

        protected virtual void OnWin(SymbolWin win, int slotIndex)
        {
            int symbolIndex = win.symbolIndex;
            string symbolWinSound = symbolWinSounds[symbolIndex];

            if (playOncePerSymbol)
            {
                if (playedOncePerSymbolList == null || playedOncePerSymbolList.Count == 0)
                {
                    Debug.LogError("playedOncePerSymbolList should be set");
                    return;
                }
                else if (!playedOncePerSymbolList[symbolIndex] && !string.IsNullOrEmpty(symbolWinSound))
                {
                    playedOncePerSymbolList[symbolIndex] = true;
                    GSManager.Instance.GetHandler(symbolWinSound).Play();
                    playedSlotIndexList[symbolIndex] = slotIndex;
                }
            }
            else 
            {
                var LoopCount = BlackboardUtils.FindVariable<int>(null, "./LoopCount");
                if (LoopCount.value == 0 && !string.IsNullOrEmpty(symbolWinSound))
                    GSManager.Instance.GetHandler(symbolWinSound).Play();
            }
        }

        protected virtual void OnSkipWin(int slotIndex)
        {
            for (int i = 0; i < symbolWinSounds.Count; ++i)
            {
                if (IsSoundStoppable(i, slotIndex))
            	    GSManager.Instance.GetHandler(symbolWinSounds[i]).Stop();
            }

            if (!string.IsNullOrEmpty(totalWinSound))
                GSManager.Instance.GetHandler(totalWinSound).Stop();
        }

        private bool IsSoundStoppable(int symbolIndex, int slotIndex)
        {
            if (string.IsNullOrEmpty(symbolWinSounds[symbolIndex]))
                return false;

            if (playOncePerSymbol && playedSlotIndexList[symbolIndex] != slotIndex)
                return false;

            return true;
        }
    }
}
