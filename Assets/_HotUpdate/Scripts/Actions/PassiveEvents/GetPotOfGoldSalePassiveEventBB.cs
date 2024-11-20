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

[Category("★ BagelCode/PassiveEvents")]
public class GetPotOfGoldSalePassiveEventBB : ActionTask<Blackboard> 
{
    public BBParameter<Blackboard> productBB;

    public BBParameter<int> id;
    public BBParameter<int> productIndex;
    public BBParameter<long> endTimestamp;
    public BBParameter<int> salePercent;

    protected override string info
    {
        get{ return string.Format("Pot of gold Sale Event"); }
    }

    protected override void OnExecute () 
    {
        EventInfo eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.PIGGY_BANK_SALE);

        if(eventInfo != null)
        {
            id.value = eventInfo.id;
            productIndex.value = PassiveEventManager.Instance.GetEventInfoIndex(eventInfo);
            endTimestamp.value = eventInfo.endTimestamp;
            salePercent.value = BlackboardQueryUtils.GetPotOfGoldSalePercent(productIndex.value);
            
            // BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), "potOfGoldSaleEventID", eventInfo.id);
        }
        else
        {
            id.value = 0;
            productIndex.value = 0;
            endTimestamp.value = 0;
            salePercent.value = 0;

            // BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), "potOfGoldSaleEventID", 0);
        }

        EndAction(true);
    }
}

}
