using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using NodeCanvas.StateMachines;

namespace BagelCode.LuckyFive
{
    public class LuckyFiveCardInstance : MonoBehaviour
    {
        public FSMOwner fsmOwner;

        public LuckyFiveCardInfo cardInfo;
        public bool hit;

        [Serializable]
        public class UnityLuckyFiveCardInstanceEvent : UnityEvent<LuckyFiveCardInstance> {}
        // public UnityCardInstanceEvent onClick;

        public int Index { get { return cardInfo.index; } }
        
        public int Number { get { return cardInfo.number; } }
        
        public int Suit { get { return cardInfo.suit; } }

        public bool UseFullSuit { get { return cardInfo.useFullSuit; } }
    }
}
