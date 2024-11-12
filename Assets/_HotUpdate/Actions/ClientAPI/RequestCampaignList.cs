using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class RequestCampaignList : ActionTask<Blackboard>
{
    protected override string info { get { return "Request Campaign List"; } }

    protected override void OnExecute()
    {
        BagelCodeClientAPI.RequestCampaignList(
        (response) =>
        {
            BlackboardQueryUtils.UpdateInAppMessageList(response.inAppMessageList);
            BlackboardQueryUtils.LoadInAppMessageWebImages(CacheType.FileCache, true);
            BlackboardQueryUtils.UpdateSlotBannerList(response.slotBannerGroupList);
            IAMRouter.Instance.UpdateIAMInfo();

            if(agent != null)
                EndAction(true);
        },
        (error) =>
        {
            if(agent != null)
            {
                GlobalErrorHandler.GlobalError(error);
            }
        });
    }
}

}
