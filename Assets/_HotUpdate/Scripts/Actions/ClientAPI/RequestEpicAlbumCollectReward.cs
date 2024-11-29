using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using BagelCode.ClientModels;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class RequestEpicAlbumCollectReward : ActionTask <Blackboard>
{
    public BBParameter<int> category;
    public BBParameter<List<Blackboard>> saveAsRewardResultList;
    public BBParameter<int> saveAsRewardStage;
    public BBParameter<bool> saveAsSuccess;
    
    protected override string info
    { 
        get 
        { 
            return "Request Epic Album Collect Reward";
        } 
    }
    protected override void OnExecute()
    {
        saveAsSuccess.value = false;

        var eaInfoBB = BlackboardQueryUtils.GetEpicAlbumInfo((CategoryType)category.value);
        if(eaInfoBB != null)
        {
            saveAsRewardStage.value = eaInfoBB.GetValue<int>("collectedRewardStage");

            BagelCodeClientAPI.RequestEpicAlbumCollectReward(category.value, saveAsRewardStage.value + 1,
            (response) =>
            {
                saveAsSuccess.value = true;

                var epicAlbumInfoBB = BlackboardQueryUtils.GetEpicAlbumInfo((CategoryType)category.value);
                
                if(epicAlbumInfoBB != null)
                {
                    epicAlbumInfoBB.SetValue("collectedRewardStage", response.rewardStage);
                    epicAlbumInfoBB.SetValue("nextRequiredStarCount", response.nextRequiredStarCount);

                    var enabledEpicAlbumRewardCount = BlackboardUtils.GetOrCreateVariable<int>( MainBlackboard.Get(), "enabledEpicAlbumRewardCount");
                    --enabledEpicAlbumRewardCount.value;
                }

                if(agent != null)
                {
                    ClientAPI2Blackboard.Serialize(agent, response);
                    saveAsRewardResultList.value = BlackboardUtils.GetOrCreateBlackboardList(agent, "rewardResultList");
                    EndAction();
                }
            },
            (error) =>
            {
                // Debug.LogError(error.errorCode);
                switch(error.errorCode)
                {
                    case ClientModels.Error.INVALID_EPIC_ALBUM_COLLECT_REWARD_ERROR:
                    case ClientModels.Error.INVALID_REWARD_STAGE_ERROR:
                    {
                        if(agent != null)
                            EndAction(false);
                    }
                        break;
                    default:
                        GlobalErrorHandler.GlobalError(error);
                        break;
                }
            });
        }
        else
        {
            EndAction(false);
        }
    }
}

}
