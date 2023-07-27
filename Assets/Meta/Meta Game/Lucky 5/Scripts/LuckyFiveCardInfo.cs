using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode.LuckyFive
{
    [Serializable]
    public class LuckyFiveCardInfo : IEquatable<LuckyFiveCardInfo>, ICloneable
    {
        public int index;
        public int number;
        public int suit;
        public bool useFullSuit;

        public bool Equals(LuckyFiveCardInfo other)
        {
            return this.index == other.index;
        }

        public object Clone()
        {
            var newCard = new LuckyFiveCardInfo();
            newCard.index = this.index;
            newCard.number = this.number;
            newCard.suit = this.suit;
            newCard.useFullSuit = this.useFullSuit;
            return newCard;
        }
    }
}
