using System;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.LuckyFive;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/Lucky Five")]
    public class UpdateLuckyFiveResultStaticValues : ActionTask<Blackboard>
    {
        public BBParameter<List<GameObject>> cardObjectList;
        public BBParameter<Blackboard> responseBB;

        private ContextElement agentElement;
        private bool notEnoughCards;

        protected override string info
        {
            get { return "Update Lucky Five Result Static Values"; }
        }

        protected override void OnExecute()
        {
            agentElement = agent.gameObject.GetComponent<ContextElement>();

            var recentCardList = responseBB.value.GetValue<List<Blackboard>>("recentCardList");

            notEnoughCards = recentCardList.Count < BlackboardQueryUtils.LUCKY_FIVE_DECK_COUNT;

            if(LuckyFiveCustomCardData.Instance == null)
            {
                MetaObjectUtils.MakePrefab("mglucky5common0", "LuckyFiveCardAssets", agent.transform, "", "CardDataAssets");
            }

            UpdatePayTable();
            UpdateCards();
            UpdateRewards();
            UpdateWins();

            EndAction();
        }

        private void UpdatePayTable()
        {
            var payTable = responseBB.value.GetValue<List<double>>("paytable");
            var rule = responseBB.value.GetValue<LuckyFiveRule>("rule");

            for(int i=0; i<10; ++i)
            {
                string elementPath = string.Format("Win Table {0}/Payout", i + 1);
                ContextElement payOutElement = ContextUtils.FindElement(agentElement, elementPath, ContextSearchingType.FullNameSearch);

                SetTextElementText(payOutElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "LUCKY_FIVE_RESULT_PAY_TEXT", payTable[i]));

                if((int)rule == i && !notEnoughCards)
                {
                    string highlightElementPath = string.Format("Win Table {0}/Highlight Area {0}", i + 1);
                    ContextElement highlightElement = ContextUtils.FindElement(agentElement, highlightElementPath, ContextSearchingType.FullNameSearch);
                    highlightElement.gameObject.SetActive(true);
                }
            }
        }

        private void UpdateCards()
        {
            var recentCardList = responseBB.value.GetValue<List<Blackboard>>("recentCardList");

            while(recentCardList.Count < BlackboardQueryUtils.LUCKY_FIVE_DECK_COUNT)
                recentCardList.Insert(0, null);

            var hitList = responseBB.value.GetValue<List<bool>>("hitList");
            while(hitList.Count < BlackboardQueryUtils.LUCKY_FIVE_DECK_COUNT)
                hitList.Insert(0, false);

            var prefab = AssetBundleManager.LoadAsset<GameObject>("mglucky5common0", "Lucky 5 Card");

            cardObjectList.value = new List<GameObject>();

            for(int i=0; i<BlackboardQueryUtils.LUCKY_FIVE_DECK_COUNT; ++i)
            {
                GameObject go = GameObject.Instantiate(prefab) as GameObject;
                go.name = string.Format("Card {0}", i);
                var cardInstance = go.GetComponent<LuckyFiveCardInstance>();

                cardInstance.fsmOwner.StopBehaviour();
                cardInstance.fsmOwner.StartBehaviour();

                string betTextElementPath = string.Format("Bet Text {0}", i + 1);
                string betTextBGElementPath = string.Format("Background {0}", i + 1);
                ContextElement betTextElement = ContextUtils.FindElement(agentElement, betTextElementPath, ContextSearchingType.ChildrenSearch);
                ContextElement betBGTextElement = ContextUtils.FindElement(agentElement, betTextBGElementPath, ContextSearchingType.ChildrenSearch);

                cardInstance.cardInfo = null;
                if(recentCardList[i] != null)
                {
                    cardInstance.cardInfo = LuckyFiveCustomCardData.Instance.cardAssets.cards[recentCardList[i].GetValue<int>("number")];
                    SetTextElementText(betTextElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "SIMPLE_CREDIT_COLOR", recentCardList[i].GetValue<long>("bet")));
                    betTextElement.gameObject.SetActive(true);
                    betBGTextElement.gameObject.SetActive(true);

                    cardInstance.hit = notEnoughCards ? true : hitList[i];
                }
                else
                {
                    SetTextElementText(betTextElement, "0");
                    betTextElement.gameObject.SetActive(false);
                    betBGTextElement.gameObject.SetActive(false);
                }

                if(cardInstance.cardInfo != null)
                {
                    if (!cardInstance.fsmOwner.isRunning)
                        cardInstance.fsmOwner.graph.isRunning = true;
                    cardInstance.fsmOwner.TriggerState("Set");
                }

                Transform parent = agent.transform.Find( string.Format("Anchor/Recents Cards/Win Card Area/Card Area {0}", i+1) );
                go.transform.SetParent(parent, false);

                cardObjectList.value.Add(go);
            }
        }

        private void UpdateRewards()
        {
            ContextElement avgTextElement = ContextUtils.FindElement(agentElement, "Result Area/Text Bet", ContextSearchingType.FullNameSearch);
            ContextElement ruleTextElement = ContextUtils.FindElement(agentElement, "Result Area/Text Win", ContextSearchingType.FullNameSearch);

            SetTextElementText(avgTextElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "LUCKY_FIVE_RESULT_AVG_BET_TEXT", responseBB.value.GetValue<long>("averageBet")));

            var rule = responseBB.value.GetValue<LuckyFiveRule>("rule");

            if(notEnoughCards)
            {
                SetTextElementText(ruleTextElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "LUCKY_FIVE_RESULT_NOT_ENOUGH_CARD_TEXT"));
            }
            else
            {
                if(rule == LuckyFiveRule.HIGH_CARD)
                {
                    SetTextElementText(ruleTextElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "LUCKY_FIVE_RESULT_NO_WIN_TEXT"));
                }
                else
                {
                    var earnCoins = responseBB.value.GetValue<long>("earnCredit");

                    string ruleText = StringTableUtils.GetString(StringTable.StringTableType.Global, "LUCKY_FIVE_RULE_FORMAT", rule);
                    ruleText = StringTableUtils.GetString(StringTable.StringTableType.Global, ruleText);

                    SetTextElementText(ruleTextElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "LUCKY_FIVE_RESULT_TOTAL_WIN_TEXT", ruleText, earnCoins));
                }
            }
        }

        private void UpdateWins()
        {
            ContextElement totalWinTextElement = ContextUtils.FindElement(agentElement, "Total Wins Chart/Text Right", ContextSearchingType.FullNameSearch);
            SetTextElementText(totalWinTextElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "LUCKY_FIVE_RESULT_COIN_TEXT", responseBB.value.GetValue<long>("totalWin")));
        }

        private void SetTextElementText(ContextElement element, string text)
        {
            IContextText textElement = element as IContextText;
            if(textElement != null)
                textElement.SetText(text);
        }
    }
}
