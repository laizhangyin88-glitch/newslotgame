using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Cards
{
    [Serializable]
    public class CardInfo : IEquatable<CardInfo>, ICloneable
    {
        public int index;
        public int number;
        public int suit;
        public SymbolAttribute mask;

        public bool Equals(CardInfo other)
        {
            return this.index == other.index;
        }

        public object Clone()
        {
            var newCard = new CardInfo();
            newCard.index = this.index;
            newCard.number = this.number;
            newCard.suit = this.suit;
            newCard.mask = this.mask;
            return newCard;
        }
    }
}
