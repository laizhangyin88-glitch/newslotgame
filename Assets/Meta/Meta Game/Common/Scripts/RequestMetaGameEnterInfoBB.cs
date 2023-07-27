using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Meta Games/Common")]
public class RequestMetaGameEnterInfoBB : ActionTask<Blackboard>
{
    public BBParameter<List<string>>    saveAsWebImageUrlList;
    public BBParameter<bool>            isOtherMetaGame;

    protected override string info
    {
        get{ return string.Format("Request Meta Game Enter Info"); }
    }

    protected override void OnExecute ()
    {
        EventInfo eventInfo = (!isOtherMetaGame.value) ? BlackboardQueryUtils.GetMetaGameEventInfo() : BlackboardQueryUtils.GetOtherMetaGameEventInfo();

        saveAsWebImageUrlList.value = null;

        if(eventInfo != null)
        {
            var metaEnterInfoBB = BlackboardQueryUtils.GetMetaGameEnterInfo();

            if(metaEnterInfoBB == null || metaEnterInfoBB.GetValue<EventInfoType>("type") != eventInfo.type)
            {
                BagelCodeClientAPI.RequestMetaEnterGameInfo(BlackboardQueryUtils.GetIngameID(),
                (response) =>
                {
                    if(agent != null)
                    {
                        if (!isOtherMetaGame.value) BlackboardQueryUtils.UpdateMetaGameEnterInfo(response.metaGameEnterInfo);
                        else BlackboardQueryUtils.UpdateOtherMetaGameEnterInfo(response.metaGameEnterInfo);
                        saveAsWebImageUrlList.value = BlackboardQueryUtils.GetMetaGameCommonWebImageList(eventInfo);
                        EndAction();
                    }
                },
                (error) =>
                {
                    if(agent != null)
                    {
                        GlobalErrorHandler.GlobalError(error);
                        EndAction(false);
                    }
                });
            }
            else
            {
                saveAsWebImageUrlList.value = BlackboardQueryUtils.GetMetaGameCommonWebImageList(eventInfo);
                EndAction(true);
            }
        }
        else
        {
            EndAction(true);
        }
    }
}

}
