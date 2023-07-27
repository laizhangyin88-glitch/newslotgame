using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]

public class ClaimClubFeed : ActionTask <Blackboard> 
{
    public BBParameter<string> feedIDValue;

    public BBParameter<int> bonusType;
    public BBParameter<long> bonusCredit;
    public BBParameter<string> analyticContextID;

    protected override string info
    { 
        get 
        { 
            return "Claim Club Feed(Like)";
        } 
    }
    protected override void OnExecute()
    {
        var feedID = BlackboardUtils.FindVariable<long>(agent, feedIDValue.value);

        BagelCodeClientAPI.SendNewsFeedClaim(feedID.value,
        (response) =>
        {
            long winBonusCoins = 0;
            if(response.claimReward != null)
            {
                switch(response.claimReward.rewardType)
                {
                    case RewardType.CREDIT:
                        {
                            RewardResultCredit creditResult = response.claimReward.rewardResult as RewardResultCredit;
                            if(creditResult != null)
                            {
                                winBonusCoins = creditResult.credit;
                                BlackboardQueryUtils.AddCoins(creditResult.credit);
                            }
                        }
                        break;
                    case RewardType.CREDIT_WITH_MULTIPLIER:
                        {
                            RewardResultCreditWithMultiplier creditResult = response.claimReward.rewardResult as RewardResultCreditWithMultiplier;
                            if(creditResult != null)
                            {
                                winBonusCoins = creditResult.credit;
                                BlackboardQueryUtils.AddCoins(creditResult.credit);
                            }
                        }
                        break;
                    case RewardType.GEM:
                        {
                            RewardResultGem gemResult = response.claimReward.rewardResult as RewardResultGem;
                            if (gemResult != null)
                            {
                                BlackboardQueryUtils.AddGems(gemResult.gem);
                            }
                        }
                        break;
                    case RewardType.CLUB_ARENA_ENERGY:
                        {
                            RewardResultClubArenaEnergy energyResult = response.claimReward.rewardResult as RewardResultClubArenaEnergy;
                            if (energyResult != null)
                                winBonusCoins = energyResult.energy;
                        }
                        break;
                }
            }

            analyticContextID.value = response.analyticContextId;

            if(agent != null)
            {
                if (winBonusCoins > 0)
                {
                    bonusType.value = 1;
                    bonusCredit.value = winBonusCoins;
                }
                else
                    bonusType.value = 0;
                
                EndAction(true);
            }
        },
        (error) =>
        {
            switch(error.errorCode)
            {
                case ClientModels.Error.SOMEONE_UPDATING_CLUB_ERROR:
                    GlobalErrorHandler.OpenAlertPopup("ERROR_SOMETHING_WRONG");
                    if(agent != null)
                        EndAction(true);
                    break;
                case ClientModels.Error.NOT_IN_CLUB_ERROR:
                    {
                        if(agent != null)
                            EndAction(false);
                        var meClubID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubId");
                        if(meClubID.value > 0)
                        {
                            bool stringError = false;
                            ErrorPopupInfo info = new ErrorPopupInfo();
                            info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CLUB_REMOVED_ALERT", out stringError);
                            info.type = ErrorPopupType.OK;
                            info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OK", out stringError);
                            
                            ErrorPopupHandler.Instance.OpenError(info);
                        }

                        BlackboardQueryUtils.SetMyClubId(0);
                        MessageDispatcher.Dispatch("OnMetaUIEvent", new EventData("OnRemovedClub"));
                    }
                    break;
                default:
                    GlobalErrorHandler.GlobalError(error);
                    break;
            }
        });
    }
}

}
