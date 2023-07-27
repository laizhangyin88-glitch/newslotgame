using BagelCode.ClientModels;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;

namespace BagelCode.InAppMessage
{
    public class IAMBonusController : MonoBehaviour
    {
        private ContextElement agent;

        private Blackboard blackboard;

        private ContextElement betTextElement;
        private ContextElement previousBetTextElement;
        private ContextElement saleElement;
        private ContextElement saleTextElement;
        private ContextElement multiplyElement;
        private ContextElement multiplyTextElement;
        private ContextElement gemTextElement;
        private ContextElement previousGemTextElement;

        private static string ON_CLICK_BUTTON_EVENT = "OnClickButton";

        public void Init(GameObject owner, Blackboard componentBB, bool madeComponentBB, bool interactable)
        {
            agent = GetComponent<ContextElement>();
            agent.UpdateContext();

            blackboard = GetComponent<Blackboard>();
            BlackboardUtils.SetOrCreateValue<GameObject>(blackboard, "owner", owner);
            BlackboardUtils.SetOrCreateValue<Blackboard>(blackboard, "componentBB", componentBB);
            if (madeComponentBB)
                componentBB.transform.SetParent(blackboard.transform);

            betTextElement = ContextUtils.FindElement(agent, "Bet Text", ContextSearchingType.ChildrenSearch);
            previousBetTextElement = ContextUtils.FindElement(agent, "Previous Bet Text", ContextSearchingType.ChildrenSearch);
            saleElement = ContextUtils.FindElement(agent, "Sale", ContextSearchingType.ChildrenSearch);
            saleTextElement = ContextUtils.FindElement(saleElement, "Sale Text", ContextSearchingType.ChildrenSearch);
            multiplyElement = ContextUtils.FindElement(agent, "Multiply", ContextSearchingType.ChildrenSearch);
            multiplyTextElement = ContextUtils.FindElement(multiplyElement, "Multiply Text", ContextSearchingType.ChildrenSearch);

            ContextElement buttonElement = ContextUtils.FindElement(agent, "IAM Purchase Button", ContextSearchingType.ChildrenSearch);
            gemTextElement = ContextUtils.FindElement(buttonElement, "Gem Text", ContextSearchingType.ChildrenSearch);
            previousGemTextElement = ContextUtils.FindElement(buttonElement, "Previous Gem Text", ContextSearchingType.ChildrenSearch);

            if (interactable)
                MetaContextElementUtils.SetClickable<ContextElement>(buttonElement, ON_CLICK_BUTTON_EVENT, agent, owner.GetComponent<ContextElement>(), null);

            Refresh();
        }

        public void Refresh(Blackboard componentBB)
        {
            Transform previousComponentBB = blackboard.transform.Find("componentBB");
            if (previousComponentBB != null)
                Destroy(previousComponentBB.gameObject);

            BlackboardUtils.SetOrCreateValue<Blackboard>(blackboard, "componentBB", componentBB);
            componentBB.transform.SetParent(blackboard.transform);
            Refresh();
        }

        public void Refresh()
        {
            Blackboard action = BlackboardUtils.GetOrCreateVariable<Blackboard>(blackboard, "componentBB/action").value;

            int gameId = action.GetValue<int>("gameId");
            int ticketedBonusId = action.GetValue<int>("ticketedBonusId");
            long winxNumerator = BlackboardQueryUtils.GetWinXNumeratorFromTicketedBonusID(ticketedBonusId);

            long bet = action.GetValue<long>("bet");
            long extraBet = action.GetValue<long>("extraBet");

            long gem = IAMUtils.CalculateGem(bet, extraBet, winxNumerator);

            EventInfo bonusEventInfo = PassiveEventManager.Instance.GetBonusEventInfo(gameId);

            MetaContextElementUtils.SetActive(saleElement, bonusEventInfo != null && bonusEventInfo.type == EventInfoType.BONUS_SALE);
            MetaContextElementUtils.SetActive(previousGemTextElement, bonusEventInfo != null && bonusEventInfo.type == EventInfoType.BONUS_SALE);
            MetaContextElementUtils.SetActive(multiplyElement, bonusEventInfo != null && bonusEventInfo.type == EventInfoType.BONUS_MULTIPLY);
            MetaContextElementUtils.SetActive(previousBetTextElement, bonusEventInfo != null && bonusEventInfo.type == EventInfoType.BONUS_MULTIPLY);

            string betText = "", gemText = "";
            if (bonusEventInfo != null)
            {
                if (bonusEventInfo.type == EventInfoType.BONUS_SALE)
                {
                    string previousGemText = StringTableUtils.GetString(StringTable.StringTableType.Global, "IAM_BONUS_PREVIOUS_GEM_TEXT", gem);
                    MetaContextElementUtils.SetText(previousGemTextElement, previousGemText);

                    long saleNumerator = PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(bonusEventInfo);

                    gemText = StringTableUtils.GetString(StringTable.StringTableType.Global, "IAM_BONUS_GEM_TEXT", IAMUtils.RemoveUnitOfDigit(gem * (100 - saleNumerator) / NumberUtils.GetGlobalDenominator()));
                    gem = IAMUtils.RemoveUnitOfDigit(gem * (100 - saleNumerator) / NumberUtils.GetGlobalDenominator());

                    string saleText = StringTableUtils.GetString(StringTable.StringTableType.Global, "IAM_BONUS_SALE_TEXT", saleNumerator);
                    MetaContextElementUtils.SetText(saleTextElement, saleText);

                    betText = StringTableUtils.GetString(StringTable.StringTableType.Global, "IAM_BONUS_BET_TEXT", bet + extraBet);
                }
                else if (bonusEventInfo.type == EventInfoType.BONUS_MULTIPLY)
                {
                    string previousBetText = StringTableUtils.GetString(StringTable.StringTableType.Global, "IAM_BONUS_PREVIOUS_BET_TEXT", bet + extraBet);
                    MetaContextElementUtils.SetText(previousBetTextElement, previousBetText);

                    long multiplierNumerator = PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(bonusEventInfo);
                    betText = StringTableUtils.GetString(StringTable.StringTableType.Global, "IAM_BONUS_BET_TEXT", IAMUtils.RemoveUnitOfDigit((bet + extraBet) * multiplierNumerator / NumberUtils.GetGlobalDenominator()));

                    string multiplierText = null;
                    if(BlackboardQueryUtils.IsBuyABonusEventPercentText())
                    {
                        long viewAddPercent = NumberUtils.GetAdditionalPercent(multiplierNumerator);
                        multiplierText = StringTableUtils.GetString(StringTable.StringTableType.Global, "IAM_BONUS_PERCENT_TEXT", viewAddPercent);
                    }
                    else
                    {
                        multiplierText = StringTableUtils.GetString(StringTable.StringTableType.Global, "IAM_BONUS_MULTIPLY_TEXT", NumberUtils.GetMultiplierFromNumerator(multiplierNumerator));
                    }
                    MetaContextElementUtils.SetText(multiplyTextElement, multiplierText);

                    gemText = StringTableUtils.GetString(StringTable.StringTableType.Global, "IAM_BONUS_GEM_TEXT", gem);
                }
            }
            else
            {
                betText = StringTableUtils.GetString(StringTable.StringTableType.Global, "IAM_BONUS_BET_TEXT", bet + extraBet);
                gemText = StringTableUtils.GetString(StringTable.StringTableType.Global, "IAM_BONUS_GEM_TEXT", gem);
            }

            BlackboardUtils.SetOrCreateValue(action.GetValue<Blackboard>("product"), "gemPrice", gem);

            MetaContextElementUtils.SetText(betTextElement, betText);
            MetaContextElementUtils.SetText(gemTextElement, gemText);
        }
    }
}
