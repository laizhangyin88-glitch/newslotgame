using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Cards
{
    [Serializable]
    public class CardDeck
    {
        public List<CardInfo> deck;

        public CardInfo PopBack()
        {
            int index = deck.Count - 1;
            var cardInfo = deck[index];
            deck.RemoveAt(index);
            return cardInfo;
        }

        public CardInfo PopFront()
        {
            var cardInfo = deck[0];
            deck.RemoveAt(0);
            return cardInfo;
        }

        public void PushBack(CardInfo cardInfo)
        {
            deck.Add(cardInfo);
        }

        public void PushFront(CardInfo cardInfo)
        {
            deck.Insert(0, cardInfo);
        }

        public void Clear()
        {
            deck.Clear();
        }
    }
}
