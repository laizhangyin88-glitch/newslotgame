using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion;
using UnityEngine;
using SlotMaker;

namespace BagelCode
{
    public abstract class BuyABonusButtonBase : MonoBehaviour
    {
        private MessageDelegates creditDelegates;

        private bool isInit = false;

        protected virtual void OnEnable()
        {
            MessageDispatcher.Register("OnCreditEvent", creditDelegates.Delegate);
        }

        protected virtual void OnDisable()
        {
            MessageDispatcher.UnRegister("OnCreditEvent", creditDelegates.Delegate);
        }

        protected virtual void Awake()
        {
            creditDelegates = new MessageDelegates
            (
                new Dictionary<string, MessageDispatcher.EventDelegate>
                {
                    { "UpdatedTotalBetCredit", BetChanged }
                }
            );
        }

        public void OnInitGame()
        {
            InitProperty();
            UpdateValues();
            RefreshButton();
        }

        private void BetChanged(EventData eventData)
        {
            RefreshButton();
        }

        protected abstract void InitProperty();
        public abstract void UpdateValues();
        public abstract void RefreshButton();
    }
}
