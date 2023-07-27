using System.Collections.Generic;
using BagelCode.ClientModels;
using BagelCode.Tasks.Actions.Contents;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;

namespace BagelCode.InAppMessage
{
    public class IAMBonusRangeController : MonoBehaviour
    {
        private ContextElement agent;

        private Blackboard blackboard;

        private List<IAMUtils.BonusInfo> bonusInfoList;

        private int gameId;
        private int ticketedBonusId;
        private long winxNumerator;
        private bool closeWhenSucceeded;

        private int bonusIndex;

        private PIDButton arrowButtonRight;
        private PIDButton arrowButtonLeft;

        private static string ON_CLICK_RIGHT_EVENT = "OnClickRight";
        private static string ON_CLICK_LEFT_EVENT = "OnClickLeft";

        private List<IAMBonusController> iamBonusList;

        public void Init(GameObject owner, Blackboard componentBB, bool interactable)
        {
            agent = GetComponent<ContextElement>();
            agent.UpdateContext();

            blackboard = GetComponent<Blackboard>();
            BlackboardUtils.SetOrCreateValue<Blackboard>(blackboard, "componentBB", componentBB);
            closeWhenSucceeded = componentBB.GetValue<bool>("closeWhenSucceeded");

            gameId = componentBB.GetValue<int>("gameId");
            ticketedBonusId = componentBB.GetValue<int>("ticketedBonusId");
            winxNumerator = BlackboardQueryUtils.GetWinXNumeratorFromTicketedBonusID(ticketedBonusId);

            long minGem = componentBB.GetValue<long>("minGem");
            long maxGem = componentBB.GetValue<long>("maxGem");

            bonusInfoList = IAMUtils.GetBonusInfoList(minGem, maxGem, winxNumerator, gameId);

            ContextElement arrowButtonRightElement = ContextUtils.FindElement(agent, "Arrow Button Right", ContextSearchingType.ChildrenSearch);
            arrowButtonRight = arrowButtonRightElement.GetComponent<PIDButton>();
            ContextElement arrowButtonLeftElement = ContextUtils.FindElement(agent, "Arrow Button Left", ContextSearchingType.ChildrenSearch);
            arrowButtonLeft = arrowButtonLeftElement.GetComponent<PIDButton>();

            ContextElement iamBonusAreaElement = ContextUtils.FindElement(agent, "IAM Bonus Area", ContextSearchingType.ChildrenSearch);

            if (bonusInfoList.Count > 3)
            {
                if(interactable)
                {
                    MetaContextElementUtils.SetClickable(arrowButtonRightElement, ON_CLICK_RIGHT_EVENT, agent, null);
                    MetaContextElementUtils.SetClickable(arrowButtonLeftElement, ON_CLICK_LEFT_EVENT, agent, null);
                }

                long betCredit = BlackboardUtils.FindValue<long>(null, "./betCredit");
                bonusIndex = 0;
                bool betFound = false;
                for (int i = 0; i < bonusInfoList.Count; i++)
                {
                    if (bonusInfoList[i].bet == betCredit)
                    {
                        bonusIndex = i;
                        betFound = true;
                    }
                }

                if (!betFound && bonusInfoList.Count > 0)
                {
                    if (betCredit >= bonusInfoList[bonusInfoList.Count - 1].bet)
                        bonusIndex = bonusInfoList.Count - 1;
                    else if (betCredit <= bonusInfoList[0].bet)
                        bonusIndex = 0;
                }

                if (bonusInfoList.Count - bonusIndex < 3)
                    bonusIndex = bonusInfoList.Count - 3;

                iamBonusList = new List<IAMBonusController>();

                for (int i = bonusIndex; i < bonusIndex + 3; i++)
                {
                    GameObject iamBonus = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "IAM Bonus", iamBonusAreaElement.transform);
                    IAMBonusController iamBonusController = iamBonus.GetComponent<IAMBonusController>();

                    iamBonusController.Init(owner, MakeBlackboardForBuyBonus(bonusInfoList[i].bet, bonusInfoList[i].extraBet), true, interactable);
                    iamBonusList.Add(iamBonusController);
                }
            }
            else
            {
                MetaContextElementUtils.SetActive(arrowButtonRightElement, false);
                MetaContextElementUtils.SetActive(arrowButtonLeftElement, false);

                for (int i = 0; i < bonusInfoList.Count; i++)
                {
                    GameObject iamBonus = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "IAM Bonus", iamBonusAreaElement.transform);
                    IAMBonusController iamBonusController = iamBonus.GetComponent<IAMBonusController>();

                    iamBonusController.Init(owner, MakeBlackboardForBuyBonus(bonusInfoList[i].bet, bonusInfoList[i].extraBet), true, interactable);
                }
            }

            RefreshArrow();
        }

        public void OnClickRight()
        {
            bonusIndex += 3;

            if (bonusInfoList.Count - bonusIndex < 3)
                bonusIndex = bonusInfoList.Count - 3;

            Refresh();
        }

        public void OnClickLeft()
        {
            bonusIndex -= 3;

            if (bonusIndex < 0)
                bonusIndex = 0;

            Refresh();
        }

        private void Refresh()
        {
            RefreshArrow();

            for (int i = bonusIndex; i < bonusIndex + 3; i++)
                iamBonusList[i - bonusIndex].Refresh(MakeBlackboardForBuyBonus(bonusInfoList[i].bet, bonusInfoList[i].extraBet));
        }

        private void RefreshArrow()
        {
            arrowButtonRight.interactable = bonusIndex != bonusInfoList.Count - 3;
            arrowButtonLeft.interactable = bonusIndex != 0;
        }

        private Blackboard MakeBlackboardForBuyBonus(long bet, long extraBet)
        {
            Blackboard bonusComponentBB = (Blackboard)BlackboardUtils.CreateBlackboard("componentBB");

            Blackboard action = (Blackboard)BlackboardUtils.CreateBlackboard("action");
            action.transform.SetParent(bonusComponentBB.transform);
            BlackboardUtils.SetOrCreateValue<int>(action, "gameId", gameId);
            BlackboardUtils.SetOrCreateValue<int>(action, "ticketedBonusId", ticketedBonusId);
            BlackboardUtils.SetOrCreateValue<long>(action, "bet", bet);
            BlackboardUtils.SetOrCreateValue<long>(action, "extraBet", extraBet);
            BlackboardUtils.SetOrCreateValue<ActionType>(action, "type", ActionType.BUY_BONUS);
            BlackboardUtils.SetOrCreateValue<Blackboard>(bonusComponentBB, "action", action);

            ProductUtils.MakeBonusProduct(action, "buy_bonus");
            BlackboardUtils.SetOrCreateValue<bool>(bonusComponentBB, "closeWhenSucceeded", closeWhenSucceeded);
            BlackboardUtils.SetOrCreateValue<InAppMessageComponentType>(bonusComponentBB, "type", InAppMessageComponentType.SINGLE_BONUS_BUY_BUTTON);

            return bonusComponentBB;
        }
    }
}
