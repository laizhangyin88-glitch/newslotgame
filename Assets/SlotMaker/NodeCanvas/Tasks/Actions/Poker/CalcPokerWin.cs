using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Cards.Tasks.Actions
{
    [Category("★ SlotMaker/Cards")]
    public class CalcPokerWin : ActionTask
    {
        public BBParameter<long> betCredit;
        public BBParameter<long> baseWager;
        public BBParameter<int> handCount;
        public BBParameter<List<long>> multipliers;
        public BBParameter<List<long>> payouts;
        public BBParameter<List<Blackboard>> decks;

        public BBParameter<List<PokerWin>> saveAs;
        public bool apply;

        private static readonly List<bool> HIT_ALL = new List<bool>{ true, true, true, true, true };

        protected override void OnExecute()
        {
            long totalEarnCredit = 0L;
            var winList = new List<PokerWin>();

            long betPerHand = (betCredit.value / handCount.value) / baseWager.value;

            for (int i = 0; i < handCount.value; ++i)
            {
                var cardList = decks.value[i].GetValue<List<int>>("cardList");

                List<bool> hitmap;
                int paytableIndex = CalcWin(
                    CustomCardData.Instance.cardAssets.cards, 
                    ((PokerPayTable)CustomCardData.Instance.payTable).ranks,
                    cardList, out hitmap
                );

                if (paytableIndex >= 0)
                {
                    long earnCredit = payouts.value[paytableIndex] * betPerHand * multipliers.value[i];
                    totalEarnCredit += earnCredit;

                    var pokerWin = new PokerWin
                    {
                        owner = i,
                        paytableIndex = paytableIndex,
                        hitmap = hitmap,
                        earnCredit = earnCredit,
                        multiplier = multipliers.value[i]
                    };
                    winList.Add(pokerWin);
                }
            }

            if (apply)
            {
                var spin = ContentBlackboard.Get().GetValue<Blackboard>("spin");
                BlackboardUtils.SetOrCreateValue<List<PokerWin>>(spin, "winList", winList);
                ContentBlackboardUtils.AddEarnCredit(spin, totalEarnCredit);
            }

            saveAs.value = winList;

            EndAction();
        }

        int CalcWin(List<CardInfo> refCards, List<PokerPayTable.RankComparer> ranks, List<int> cards, out List<bool> hitmap)
        {
            hitmap = null;

            List<int> wilds = new List<int>();
            List<int>[] pairs = new List<int>[14];
            List<int>[] suits = new List<int>[4];
            List<int>[] numbers = new List<int>[14];

            PreCalc(refCards, cards, wilds, pairs, suits, numbers);
            int flush = CalcFlush(suits, wilds);
            int straight = CalcStraight(numbers, wilds);
            
            int ranksCount = ranks.Count;
            for (int i = 0; i < ranksCount; ++i)
            {
                var rank = ranks[i];
                if (rank.pair.enabled)
                {
                    int firstNumber, secondNumber, withNumber;
                    if (CalcPair(pairs, wilds, cards.Count,
                        rank.pair.kind, rank.pair.include, rank.pair.with, 
                        out firstNumber, out secondNumber, out withNumber, out hitmap))
                    {
                        return i;
                    } 
                }
                else
                {
                    if (rank.flush && flush < 0)
                        continue;

                    if (rank.straight.enabled)
                    {
                        if (straight < 0) continue;
                        if (rank.straight.royal && straight < 14) continue;
                        if (rank.straight.natural && wilds.Count > 0) continue;
                    }

                    hitmap = HIT_ALL;
                    return i;
                }
            }
            return -1;
        }

        void PreCalc(List<CardInfo> refCards, List<int> cards, List<int> wilds, List<int>[] pairs, List<int>[] suits, List<int>[] numbers)
        {
            int cardCount = cards.Count;
            for (int handIndex = 0; handIndex < cardCount; ++handIndex)
            {
                var cardInfo = refCards[ cards[handIndex] ];

                if (SymbolMask.HasAttribute(cardInfo.mask, SymbolAttribute.Wild))
                {
                    wilds.Add(handIndex);
                    continue;
                }

                SafetyAdd(pairs, cardInfo.number - 1, handIndex);
                SafetyAdd(suits, cardInfo.suit, handIndex);
                SafetyAdd(numbers, cardInfo.number - 1, handIndex);
            }

            pairs[13] = pairs[0];
            numbers[13] = numbers[0];
        }

        int CalcFlush(List<int>[] suits, List<int> wilds)
        {
            int wildCount = wilds.Count;
            for (int suit = 0; suit < 4; ++suit)
            {
                int suitCount = SafetyCount(suits, suit);
                if (suitCount + wildCount >= 5)
                    return suit;
            }
            return -1;
        }

        int CalcStraight(List<int>[] numbers, List<int> wilds)
        {
            int wildCount = wilds.Count;
            for (int biggestNumber = 14; biggestNumber > 4; --biggestNumber)
            {
                int requiredWild = 0;
                for (int i = 0; i < 5; ++i)
                {
                    int number = biggestNumber - i;
                    if (numbers[number - 1] == null)
                        ++requiredWild;
                }
                if (requiredWild <= wildCount)
                    return biggestNumber;
            }
            return -1;
        }

        bool CalcPair(List<int>[] pairs, List<int> wilds, int cardCount, int[] kind, int[] include, int[] with, 
            out int firstNumber, out int secondNumber, out int withNumber, out List<bool> hitmap)
        {
            firstNumber  = -1;
            secondNumber = -1;
            withNumber   = -1;
            hitmap = null;

            int wildCount = wilds.Count;
            for (firstNumber = 14; firstNumber > 1; --firstNumber)
            {
                int firstPairCount = SafetyCount(pairs, firstNumber - 1);
                if (firstPairCount + wildCount >= kind[0])
                {
                    bool passedInclude = !(wildCount < kind[0] && include.Length > 0);
                    if (!passedInclude)
                    {
                        for (int includeIndex = 0; includeIndex < include.Length; ++includeIndex)
                        {
                            int includeNumber = include[includeIndex];
                            if (firstNumber == includeNumber || (firstNumber - 13) == includeNumber)
                            {
                                passedInclude = true;
                                break;
                            }
                        }
                    }

                    if (passedInclude)
                    {
                        bool passedSecondKind = kind.Length < 2;
                        secondNumber = -1;
                        if (!passedSecondKind)
                        {
                            for (secondNumber = 14; secondNumber > 1; --secondNumber)
                            {
                                int secondPairCount = SafetyCount(pairs, secondNumber - 1);
                                if (secondNumber != firstNumber && secondPairCount >= kind[1])
                                {
                                    passedSecondKind = true;
                                    break;
                                }
                            }
                        }

                        if (passedSecondKind)
                        {
                            bool passedWith = with.Length == 0;
                            withNumber = -1;
                            if (!passedWith)
                            {
                                for (int withIndex = 0; withIndex < with.Length; ++withIndex)
                                {
                                    withNumber = with[withIndex];
                                    int withPairCount = SafetyCount(pairs, withNumber - 1);
                                    if (withNumber != firstNumber && withNumber != secondNumber && 
                                        withPairCount > 0)
                                    {
                                        passedWith = true;
                                        break;
                                    }
                                }
                            }
                            if (passedWith)
                            {
                                hitmap = new List<bool>(new bool[cardCount]);
                                foreach (int i in wilds)
                                {
                                    hitmap[i] = true;
                                }
                                foreach (int i in pairs[firstNumber - 1])
                                {
                                    hitmap[i] = true;
                                }
                                if (secondNumber > 0)
                                {
                                    foreach (int i in pairs[secondNumber - 1])
                                    {
                                        hitmap[i] = true;
                                    }
                                }
                                if (withNumber > 0)
                                {
                                    foreach (int i in pairs[withNumber - 1])
                                    {
                                        hitmap[i] = true;
                                    }
                                }
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        void SafetyAdd(List<int>[] list, int key, int value)
        {
            if (list[key] == null)
                list[key] = new List<int>();
            list[key].Add(value);
        }

        int SafetyCount(List<int>[] list, int key)
        {
            if (list[key] == null)
                return 0;
            return list[key].Count;
        }
    }
}
