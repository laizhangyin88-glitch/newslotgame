using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NodeCanvas.Framework;

namespace SlotMaker.Cards
{
    public class HandsGroup : MonoBehaviour
    {
        public List<HandInstance> hands;

        public void InitializeHands(UnityAction<CardInstance> onClick, int cardCount)
        {
            int handCount = hands.Count;
            for (int i = 0; i < handCount; ++i)
            {
                var hand = hands[i];
                var pool = hand.GetComponent<CardDeckInstance>().pool;
                for (int j = 0; j < cardCount; ++j)
                {
                    var card = pool.GetObject().GetComponent<CardInstance>();
                    if (!card.fsmOwner.isRunning)
                    {
                        card.fsmOwner.StartBehaviour(false, null);
                        card.onClick.AddListener(onClick);
                    }
                    else
                    {
                        card.fsmOwner.TriggerState("Close");
                    }
                    card.ChangeOrder(j);
                    card.transform.SetParent(hand.anchors[j], false);
                    hand.cards.Add(card);
                }
            }    
        }

        public void ReleaseHands()
        {
            int handCount = hands.Count;
            for (int i = 0; i < handCount; ++i)
            {
                var hand = hands[i];
                int cardCount = hand.cards.Count;
                for (int j = 0; j < cardCount; ++j)
                {
                    hand.cards[j].GetComponent<PooledObject>().ReturnToPool();
                }
                hand.cards.Clear();
            }
        }
    }
}
