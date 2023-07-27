using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using NodeCanvas.StateMachines;

namespace SlotMaker.Cards
{
    public class CardInstance : MonoBehaviour
    {
        public FSMOwner fsmOwner;

        public CardInfo cardInfo;
        public bool opened;
        public bool held;
        public int order;
        public bool clickable;
        
        public UnityIntEvent onChangedOrder;
        public UnityBoolEvent onChangedClickable;

        [Serializable]
        public class UnityCardInstanceEvent : UnityEvent<CardInstance> {}
        public UnityCardInstanceEvent onClick;

        public int Index { get { return cardInfo.index; } }
        
        public int Number { get { return cardInfo.number; } }
        
        public int Suit { get { return cardInfo.suit; } }
        
        public SymbolAttribute Mask { get { return cardInfo.mask; } }

        public void ChangeOrder(int value)
        {
            order = value;
            
            onChangedOrder.Invoke(value);
        }

        public void SetClickable(bool value)
        {
            clickable = value;

            onChangedClickable.Invoke(value);
        }

        public void OnClick()
        {
            onClick.Invoke(this);
        }
    }
}
