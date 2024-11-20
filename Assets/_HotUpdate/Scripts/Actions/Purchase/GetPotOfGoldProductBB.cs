using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Task.Actions
{

[Category("★ BagelCode/Purchase")]
public class GetPotOfGoldProductBB : ActionTask<Blackboard> 
{
    [BlackboardOnly]
    public BBParameter<Blackboard> productBB;

    public BBParameter<int> eventID;

    protected override string info
    {
        get { return string.Format("{0} = Get POG BB with Passive Sale", productBB); }
    }

    protected override void OnExecute()
    {
        EventInfo eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.PIGGY_BANK_SALE);

        if(eventInfo != null)
        {
            int productIndex = PassiveEventManager.Instance.GetEventInfoIndex(eventInfo);

            eventID.value = eventInfo.id;
            productBB.value = BlackboardQueryUtils.GetPotOfGoldProduct(productIndex);

            // BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), "potOfGoldSaleEventID", eventInfo.id);
        }
        else
        {
            eventID.value = 0;
            productBB.value = BlackboardQueryUtils.GetPotOfGoldProduct(0);

            // BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), "potOfGoldSaleEventID", 0);
        }

        EndAction();
    }
}

}
