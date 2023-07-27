using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Services;

namespace SlotMaker
{
    public class ClassicPayLineController : MonoBehaviour
    {
        public MessageRouter  router;
        public List<Animator> payLines;

        public List<int> anchorPolicies;

        protected const string ON_WIN_EVENT = "OnWinEvent";
        protected const string ON_TOTAL_WIN_EVENT = "TotalWin";
        protected const string ON_SINGLE_WIN_EVENT = "SingleWin";
        protected const string ON_SKIP_WIN_EVENT = "SkipWin";

        private void Awake()
        {
            MessageDispatcher.Register(ON_WIN_EVENT, OnWinEvent);
        }

        private void OnDestroy()
        {
            MessageDispatcher.UnRegister(ON_WIN_EVENT, OnWinEvent);
        }

        protected void OnWinEvent(EventData receivedEvent)
        {
            if (receivedEvent.name.Equals(ON_SKIP_WIN_EVENT, StringComparison.Ordinal))
                OnSkipWin();
            else if (receivedEvent.name.Equals(ON_SINGLE_WIN_EVENT, StringComparison.Ordinal))
                OnWin((SymbolWin)receivedEvent.value);
            else if (receivedEvent.name.Equals(ON_TOTAL_WIN_EVENT, StringComparison.Ordinal))
                OnTotalWin((List<SymbolWin>)receivedEvent.value);
        }

        private void OnWin(SymbolWin win)
        {
            if (win.lineIndex <= 0) return;

            var payLinesBB = BlackboardUtils.FindVariable<List<Blackboard>>(null, "./game/payLines").value;
            int column = BlackboardUtils.FindVariable<int>(null, "./customData/slotDataList/0/column").value;
            int row    = BlackboardUtils.FindVariable<int>(null, "./customData/slotDataList/0/row").value;

            int index = 0;
            int lineIndex = (int)win.lineIndex-1;
            List<Blackboard> payLineBBList = payLinesBB[0].GetValue<List<Blackboard>>("value");
            List<int> payLine = payLineBBList[lineIndex].GetValue<List<int>>("value");

            int count = column + 1;
            for (int j = 0; j < count; ++j)
            {
                int anchoredPolicy = anchorPolicies[j];
                index = payLine[anchoredPolicy] + row * j;
                payLines[index].SetBool("enable", true);
            }
        }

        private void OnTotalWin(List<SymbolWin> winList)
        {
            for (int i = 0; i < winList.Count; ++i)
            {
                OnWin(winList[i]);
            }
        }

        private void OnSkipWin()
        {
            for (int i = 0; i < payLines.Count; ++i)
            {
                payLines[i].SetBool("enable", false);
            }
        }
    }
}
