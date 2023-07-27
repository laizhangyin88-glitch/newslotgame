using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ParadoxNotion;

namespace SlotMaker
{
    public class InGameCreditBehaviour : MonoBehaviour
    {
        public UnityIntEvent onUpdateBetIndex;
        public UnityIntEvent onUpdateExtraBetRatioIndex;
        public UnityLongEvent onUpdateBetCredit;
        public UnityLongEvent onUpdatedTotalBetCredit;
        public UnityLongEvent onBuyABonus;
        public UnityEvent onBetMax;
        public UnityEvent onBetDown;
        public UnityEvent onBetUp;
        
        private MessageDelegates creditDelegates;

        protected virtual void Awake()
        {
            creditDelegates = new MessageDelegates
            (
                new Dictionary<string, MessageDispatcher.EventDelegate>
                {
                    { "UpdateBetIndex",             UpdateBetIndex           },
                    { "UpdateExtraBetRatioIndex",   UpdateExtraBetRatioIndex },
                    { "UpdateBetCredit",            UpdateBetCredit          },
                    { "UpdatedTotalBetCredit",      UpdatedTotalBetCredit    },
                    { "BuyABonus",                  BuyABonus                },
                    { "BetMax",                     BetMax                   },
                    { "BetDown",                    BetDown                  },
                    { "BetUp",                      BetUp                    }
                }
            );
        }

        protected virtual void OnEnable()
        {
            MessageDispatcher.Register("OnCreditEvent", creditDelegates.Delegate);
        }

        protected virtual void OnDisable()
        {
            MessageDispatcher.UnRegister("OnCreditEvent", creditDelegates.Delegate);
        }

        private void UpdateBetIndex(EventData eventData)
        {
            onUpdateBetIndex.Invoke(((EventData<int>)eventData).value);
        }

        private void UpdateExtraBetRatioIndex(EventData eventData)
        {
            onUpdateExtraBetRatioIndex.Invoke(((EventData<int>)eventData).value);
        }

        private void UpdateBetCredit(EventData eventData)
        {
            onUpdateBetCredit.Invoke(((EventData<long>)eventData).value);
        }

        private void UpdatedTotalBetCredit(EventData eventData)
        {
            onUpdatedTotalBetCredit.Invoke(((EventData<long>)eventData).value);
        }

        private void BuyABonus(EventData eventData)
        {
            onBuyABonus.Invoke(((EventData<long>)eventData).value);
        }

        private void BetMax(EventData eventData)
        {
            onBetMax.Invoke();
        }

        private void BetDown(EventData eventData)
        {
            onBetDown.Invoke();
        }

        private void BetUp(EventData eventData)
        {
            onBetUp.Invoke();
        }
    }
}
