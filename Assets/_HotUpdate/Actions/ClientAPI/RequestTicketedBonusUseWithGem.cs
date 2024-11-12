using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

    [Category("★ BagelCode/ClientAPI")]
    public class RequestTicketedBonusUseWithGem : ActionTask<Blackboard>
    {
        public BBParameter<Blackboard> actionBB;
        public BBParameter<Blackboard> productBB;
        public BBParameter<string> contextId;

        public BBParameter<bool> isPurchaseSuccess;


        protected override string info { get { return "Request Ticketed Bonus Use With Gem"; } }

        protected override void OnExecute()
        {
            int gameId = actionBB.value.GetValue<int>("gameId");
            int ticketedBonusId = actionBB.value.GetValue<int>("ticketedBonusId");
            long bet = actionBB.value.GetValue<long>("bet");
            long extraBet = actionBB.value.GetValue<long>("extraBet");
            var iamIDVariable = BlackboardUtils.FindVariable<int>(agent, "iamId");
            int iamID = iamIDVariable == null ? 0 : iamIDVariable.value;
            var iamTriggerTypeVaraible = BlackboardUtils.FindVariable<InAppMessageTriggerType>(agent, "triggerType");
            string iamTriggerType = iamTriggerTypeVaraible == null ? "" : iamTriggerTypeVaraible.value.ToString();

            bool isEvent = false;

            int bonusSaleEventId = 0;
            int bonusMultiplyEventId = 0;
            EventInfo bonusEventInfo = PassiveEventManager.Instance.GetBonusEventInfo(gameId);
            if (bonusEventInfo != null)
            {
                if (bonusEventInfo.type == EventInfoType.BONUS_SALE)
                {
                    isEvent = true;
                    bonusSaleEventId = bonusEventInfo.id;
                }
                else if (bonusEventInfo.type == EventInfoType.BONUS_MULTIPLY)
                {
                    isEvent = true;
                    bonusMultiplyEventId = bonusEventInfo.id;
                }
            }

            BagelCodeClientAPI.TicketedBonusUseWithGem(gameId, ticketedBonusId, bet, extraBet, bonusSaleEventId, bonusMultiplyEventId, iamID, iamTriggerType,
                (response) =>
                {
                    ClientItemAcquired(isEvent);

                    var useResponse = agent.GetComponent<Blackboard>();
                    ClientAPI2Blackboard.Serialize(useResponse, response);

                    BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
                    BlackboardQueryUtils.ApplyUserSyncInfo();

                    isPurchaseSuccess.value = true;
                    EndAction();
                },
                (error) =>
                {
                    switch(error.errorCode)
                    {
                        case ClientModels.Error.NOT_ENOUGH_GEM_ERROR:
                        {
                            if (agent != null)
                            {
                                isPurchaseSuccess.value = false;
                                EndAction();
                            }
                        }
                            break;
                        default:
                            GlobalErrorHandler.GlobalError(error);
                            break;
                    }
                });
        }

        private void ClientItemAcquired(bool isEvent)
        {
            if(agent == null) return;

            BiEventUtils.ItemAcquired(productBB.value, contextId.value, isEvent);
        }
    }

}
