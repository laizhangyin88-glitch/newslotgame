using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using ParadoxNotion;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;

namespace SlotMaker.Cards
{
    public class VideoPoker : MonoBehaviour 
    {
        public enum GameStatus
        {
            Ready,
            Reset,
            Deal,
            Draw,
            Drawing
        };
        public GameStatus gameStatus = GameStatus.Ready;
        public List<HandsGroup> handsGroups;

        public List<int> winColors;

        [Range(0.1f, 16f)]
        public float animationSpeed;
        public bool ignoreCardOpenDelay;

        public float masterCardOpenDelay;
        public float slaveCardOpenDelay;
        public float closeCardsDelay;
        
        public float handWinBoardDelay;

        public float masterHandWaitTime;
        public float slaveHandWaitTime;

        public float masterResultTime;
        public float slaveResultTime;

        public float safetyWaitTime;

        public UnityEvent onStartedGame;
        public UnityEvent onEndedGame;
        public UnityIntEvent onChangedHandsGroup;

        // Sound Usage.
        public UnityEvent onSingleWin;
        public UnityEvent onMasterCardHold;
        public UnityEvent onMasterCardOpen;
        public UnityEvent onMasterCardOpenAll;
        public UnityEvent onSubHandCardOpenAll;
        public UnityEvent onShowCurrentMultiplier;
        // End of sound usage

        int lastGroup = -1;
        List<List<CardInfo>> decks;

        List<FSMOwner> results;
        List<Blackboard> resultBBs;

        public Sprite[] multiplierSprites;

        const int CARDS_COUNT = 5;

        public bool CanDraw()
        {
            return gameStatus == GameStatus.Draw;
        }

        public bool IsReady()
        {
            return gameStatus == GameStatus.Ready;
        }

        public void InitializeHandsGroup(int group)
        {
            lastGroup = group;

            var handsGroup = handsGroups[lastGroup];
            handsGroup.gameObject.SetActive(true);
            handsGroup.InitializeHands(OnClick, CARDS_COUNT);
            
            InitializeResults();
        }

        void InitializeResults()
        {
            results = new List<FSMOwner>();
            resultBBs = new List<Blackboard>();

            var handsGroup = handsGroups[lastGroup].hands;
            int handsCount = handsGroup.Count;
            for (int i = 0; i < handsCount; ++i)
            {
                var result = handsGroup[i].GetComponent<Blackboard>().GetValue<GameObject>("result").GetComponent<FSMOwner>();
                if (!result.isRunning)
                    result.StartBehaviour(false, null);
                results.Add(result);
                resultBBs.Add((Blackboard)result.blackboard);
            }
        }

        public void ReleaseHandsGroup()
        {
            if (lastGroup < 0)
                return;

            var handsGroup = handsGroups[lastGroup];
            handsGroup.ReleaseHands();
            handsGroup.gameObject.SetActive(false);
        }

        public void UpdateDecks(List<List<int>> decks)
        {
            this.decks = new List<List<CardInfo>>();

            int decksCount = decks.Count;
            for (int i = 0; i < decksCount; ++i)
            {
                var deck = new List<CardInfo>();
                int deckCount = decks[i].Count;
                for (int j = 0; j < deckCount; ++j)
                {
                    deck.Add((CardInfo)CustomCardData.Instance.cardAssets.cards[decks[i][j]].Clone());
                }
                this.decks.Add(deck);
            }
        }

        public void UpdateJackpot(List<Blackboard> jackpots)
        {
            var handsGroup = handsGroups[lastGroup].hands;
            int handsCount = handsGroup.Count;

            for (int i = 0; i < handsCount; ++i)
            {
                Blackboard bb = resultBBs[i];
                bb.SetValue("isJackpot", false);
                bb.SetValue("jackpotIndex", -1);
                bb.SetValue("jackpotAwardAmount", 0L);
            }

            for (int i = 0; i < jackpots.Count; ++i)
            {
                int handIndex = jackpots[i].GetValue<int>("handIndex");
                bool isJackpot = jackpots[i].GetValue<bool>("isJackpot");
                int jackpotIndex = jackpots[i].GetValue<int>("jackpotIndex");
                long jackpotAwardAmount = jackpots[i].GetValue<long>("jackpotAwardAmount");

                Blackboard bb = resultBBs[handIndex];
                bb.SetValue("isJackpot", isJackpot);
                bb.SetValue("jackpotIndex", jackpotIndex);
                bb.SetValue("jackpotAwardAmount", jackpotAwardAmount);
            }
        }

        public void UpdateWin(List<PokerWin> winList)
        {
            var handsGroup = handsGroups[lastGroup].hands;
            int handsCount = handsGroup.Count;

            for (int i = 0; i < handsCount; ++i)
            {
                Blackboard bb = resultBBs[i];
                bb.SetValue("paytableIndex", -1);
                bb.SetValue("hitmap", null);
                bb.SetValue("title", string.Empty);
                bb.SetValue("earnCredit", 0L);
                bb.SetValue("earnCreditWithoutMultiplier", 0L);
                bb.SetValue("multiplier", 1L);
            }

            for (int i = 0; i < winList.Count; ++i)
            {
                var win = winList[i];
                var bb = resultBBs[win.owner];
                bb.SetValue("paytableIndex", win.paytableIndex);
                bb.SetValue("hitmap", win.hitmap);
                bb.SetValue("title", GetPayTableName(win.paytableIndex));
                bb.SetValue("earnCredit", win.earnCredit);
                bb.SetValue("earnCreditWithoutMultiplier", win.earnCredit / win.multiplier);
                bb.SetValue("multiplier", win.multiplier);
                bb.SetValue("winColor", winColors[win.paytableIndex]);
            }
        }

        public void Reset()
        {
            StartCoroutine(ResetAnimation());
        }

        public void Deal()
        {
            PreUpdateHands();
            StartCoroutine(DealAnimation());
        }

        public void Draw(List<long> multipliers)
        {
            SetClickable(false);
            PostUpdateHands();

            StartCoroutine(DrawAnimation(multipliers));
        }

        public List<bool> GetHelds()
        {
            var helds = new List<bool>();
            var hand = handsGroups[lastGroup].hands[0];
            for (int i = 0; i < CARDS_COUNT; ++i)
            {
                var card = hand.cards[i];
                helds.Add(card.held);
            }
            return helds;
        }

        public HandInstance GetHand()
        {
            return handsGroups[lastGroup].hands[0];
        }

        void PreUpdateHands()
        {
            int handIndex = 0;
            var handsGroup = handsGroups[lastGroup].hands;
            var hand = handsGroup[handIndex];
            var deck = hand.GetComponent<CardDeckInstance>().deck;
            deck.Clear();
            for (int j = 0; j < CARDS_COUNT; ++j)
            {
                var card = hand.cards[j];
                card.cardInfo = (CardInfo)decks[handIndex][j].Clone();
                deck.PushBack((CardInfo)decks[handIndex][j]);
            }
        }

        void PostUpdateHands()
        {
            var handsGroup = handsGroups[lastGroup].hands;
            int handsCount = handsGroup.Count;

            for (int i = 0; i < handsCount; ++i)
            {
                var hand = handsGroup[i];
                var deck = hand.GetComponent<CardDeckInstance>().deck;

                if (i == 0)
                    deck.Clear();
                
                for (int j = 0; j < CARDS_COUNT; ++j)
                {
                    var card = hand.cards[j];
                    if (!card.held)
                    {
                        card.cardInfo = (CardInfo)decks[i][j];
                    }
                }
            }
        }

        public void ToggleCard(CardInstance masterCard, bool held, int handsCount)
        {
            int order = masterCard.order;
            var handsGroup = handsGroups[lastGroup].hands;
            
            masterCard.held = held;
            masterCard.fsmOwner.TriggerState("Hold");

            for (int i = 1; i < handsCount; ++i)
            {
                var slaveCard = handsGroup[i].cards[order];
                slaveCard.held = held;

                if (held)
                {
                    slaveCard.cardInfo = (CardInfo)masterCard.cardInfo.Clone();
                    slaveCard.fsmOwner.TriggerState("Set");
                }
                else 
                {
                    slaveCard.fsmOwner.TriggerState("Close");
                }
            }
        }

        public void ToggleCards(List<bool> helds, int handsCount)
        {
            var handsGroup = handsGroups[lastGroup].hands;
            var cards = handsGroup[0].cards;

            for (int i = 0; i < helds.Count; ++i)
            {
                ToggleCard(cards[i], helds[i], handsCount);
            }
        }

        void OnClick(CardInstance masterCard)
        {
            var handsGroup = handsGroups[lastGroup].hands;
            int handsCount = handsGroup.Count;

            ToggleCard(masterCard, !masterCard.held, handsCount);

            results[0].TriggerState("Idle");

            if (masterCard.held)
                onMasterCardHold.Invoke();
        }

        IEnumerator ResetAnimation()
        {
            gameStatus = GameStatus.Reset;

            if (CloseCards() > 0)
            {
                yield return new WaitForSeconds(closeCardsDelay / animationSpeed);
                yield return new WaitForEndOfFrame();
            }

            gameStatus = GameStatus.Deal;
        }

        IEnumerator DealAnimation()
        {
            while (gameStatus != GameStatus.Deal)
                yield return null;

            var hand = handsGroups[lastGroup].hands[0];
            for (int i = 0; i < CARDS_COUNT; ++i)
            {
                var card = hand.cards[i];
                card.fsmOwner.TriggerState("Open");
            }
            
            if (!ignoreCardOpenDelay)
                yield return new WaitForSeconds(masterCardOpenDelay / animationSpeed);

            if (resultBBs[0].GetValue<int>("paytableIndex") >= 0)
            {
                yield return new WaitForSeconds(handWinBoardDelay);
                results[0].TriggerState("PreWin");
            }
            
            SetClickable(true);

            gameStatus = GameStatus.Draw;
            onStartedGame.Invoke();
        }

        IEnumerator DrawAnimation(List<long> multipliers)
        {
            long betCredit = 0;

            results[0].TriggerState("Idle");

            gameStatus = GameStatus.Drawing;

            yield return new WaitForSeconds(closeCardsDelay / animationSpeed);
            yield return new WaitForEndOfFrame();
            
            var handsGroup = handsGroups[lastGroup].hands;
            int handsCount = handsGroup.Count;

            for (int i = 0; i < handsCount; ++i)
            {
                var hand = handsGroup[i];
                int notHoldCount = 0;
                bool isMasterHand = i == 0;

                if (!isMasterHand)
                {
                    onSubHandCardOpenAll.Invoke();
                }

                for (int j = 0; j < CARDS_COUNT; ++j)
                {
                    var card = hand.cards[j];
                    if (!card.held)
                    {
                        ++notHoldCount;
                        card.fsmOwner.TriggerState(i == 0 ? "ReOpen" : "Open");
                        if (!isMasterHand && !ignoreCardOpenDelay)
                            yield return new WaitForSeconds(slaveCardOpenDelay / animationSpeed);
                    }
                    else
                    {
                        if (isMasterHand)
                        {
                            // ToggleCard(card, false, 1);
                        }
                    }
                }

                if (isMasterHand)
                {
                    if (notHoldCount > 0)
                    {
                        if (notHoldCount >= CARDS_COUNT)
                            onMasterCardOpenAll.Invoke();
                        else
                            onMasterCardOpen.Invoke();
                    }

                    if (!ignoreCardOpenDelay)
                        yield return new WaitForSeconds(masterCardOpenDelay / animationSpeed);
                }

                var hitmap = resultBBs[i].GetValue<List<bool>>("hitmap");
                StartCoroutine(ShowResult(i, hand, results[i], resultBBs[i], multipliers, hitmap));

                if (i < handsCount - 1)
                {
                    float handWaitTime = (i == 0) ? masterHandWaitTime : slaveHandWaitTime;
                    yield return new WaitForSeconds(handWaitTime / animationSpeed);
                }
            }

            yield return new WaitForSeconds(safetyWaitTime);

            gameStatus = GameStatus.Ready;
            onEndedGame.Invoke();
        }

        IEnumerator ShowResult(int handIndex, HandInstance hand, FSMOwner resultOwner, Blackboard resultBB, List<long> multipliers, List<bool> hitmap)
        {
            float handWaitTime = (handIndex == 0) ? masterResultTime : slaveResultTime;
            yield return new WaitForSeconds(handWaitTime);
            
            for (int i = 0; i < CARDS_COUNT; ++i)
            {
                var card = hand.cards[i];
                card.GetComponent<Animator>().SetBool("Disabled", (hitmap != null) ? !hitmap[i] : true);
            }

            if (resultBB.GetValue<int>("paytableIndex") >= 0) 
            {
                resultOwner.TriggerState("Win");
                if (multipliers != null && multipliers.Count > 0 && multipliers[handIndex] > 1L)
                    ShowNextMultiplier(handIndex, multipliers[handIndex]);

                PokerWin win = new PokerWin();
                win.owner = handIndex;
                win.paytableIndex = resultBB.GetValue<int>("paytableIndex");
                win.earnCredit = resultBB.GetValue<long>("earnCredit");
                win.multiplier = resultBB.GetValue<long>("multiplier");

                MessageDispatcher.Dispatch("OnWinEvent", new EventData<PokerWin>("SingleWin", win));
                onSingleWin.Invoke();
            }
        }

        int CloseCards()
        {
            int closeCount = 0;
            var handsGroup = handsGroups[lastGroup].hands;
            int handsCount = handsGroup.Count;
            for (int i = 0; i < handsCount; ++i)
            {
                var hand = handsGroup[i];
                for (int j = 0; j < CARDS_COUNT; ++j)
                {
                    var card = hand.cards[j];
                    if (card.opened)
                    {
                        card.fsmOwner.TriggerState("Close");
                        ++closeCount;
                    }

                    //
                    card.GetComponent<Animator>().SetBool("Disabled", false);
                }
                
                results[i].TriggerState("Idle");
            }
            return closeCount;
        }

        int CloseNotHeldCards()
        {
            int closeCount = 0;
            var hand = handsGroups[lastGroup].hands[0];
            for (int i = 0; i < CARDS_COUNT; ++i)
            {
                var card = hand.cards[i];
                if (!card.held)
                {
                    // card.fsmOwner.TriggerState("Close");
                    ++closeCount;
                }
            }
            return closeCount;
        }

        void SetClickable(bool clickable)
        {
            var hand = handsGroups[lastGroup].hands[0];
            for (int i = 0; i < CARDS_COUNT; ++i)
            {
                var card = hand.cards[i];
                card.SetClickable(clickable);
            }
        }

        string GetPayTableName(int paytableIndex)
        {
            return ((PokerPayTable)CustomCardData.Instance.payTable).ranks[paytableIndex].name;
        }

        Sprite GetMultiplierSprite(long multiplier)
        {
            if (multiplier == 2)
                return multiplierSprites[0];
            else if (multiplier == 3)
                return multiplierSprites[1];
            else if (multiplier == 4)
                return multiplierSprites[2];
            else if (multiplier == 5)
                return multiplierSprites[3];
            else if (multiplier == 6)
                return multiplierSprites[4];
            else if (multiplier == 7)
                return multiplierSprites[5];
            else if (multiplier == 11)
                return multiplierSprites[6];
            else if (multiplier == 12)
                return multiplierSprites[7];
            return null;
        }

        public void HideCurrMultiplier(int hand)
        {
            Blackboard bb = handsGroups[lastGroup].hands[hand].GetComponent<Blackboard>();
            bb = bb.GetValue<GameObject>("multiplier").GetComponent<Blackboard>();

            bb.GetComponent<Animator>().SetBool("Current", false);
        }

        public void ShowCurrMultiplier(int hand, long multiplier)
        {
            Blackboard bb = handsGroups[lastGroup].hands[hand].GetComponent<Blackboard>();
            bb = bb.GetValue<GameObject>("multiplier").GetComponent<Blackboard>();
            var currTextImg = bb.GetValue<IContextImage>("currTextImage");
            var currFakeText = bb.GetValue<IContextText>("currText");

            currFakeText.SetText(multiplier + "X");
            currTextImg.SetSprite(GetMultiplierSprite(multiplier));

            bb.GetComponent<Animator>().SetBool("Current", true);
            bb.GetComponent<Animator>().SetTrigger("Appear");
            onShowCurrentMultiplier.Invoke();
        }

        public void HideNextMultiplier(int hand)
        {
            Blackboard bb = handsGroups[lastGroup].hands[hand].GetComponent<Blackboard>();
            bb = bb.GetValue<GameObject>("multiplier").GetComponent<Blackboard>();

            bb.GetComponent<Animator>().SetBool("Next", false);
        }

        public void ShowNextMultiplier(int hand, long multiplier)
        {
            Blackboard bb = handsGroups[lastGroup].hands[hand].GetComponent<Blackboard>();
            bb = bb.GetValue<GameObject>("multiplier").GetComponent<Blackboard>();

            bb.GetValue<IContextText>("nextText").SetText(multiplier + "X");
            bb.GetComponent<Animator>().SetBool("Next", true);
        }
    }
}
