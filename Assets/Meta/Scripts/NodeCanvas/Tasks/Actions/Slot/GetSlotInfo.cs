using System.Collections;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Task.Action
{

[Category("★ BagelCode/Utils")]
public class GetSlotInfo : ActionTask
{
    public BBParameter<int> gameId;

    [BlackboardOnly]
    public BBParameter<Blackboard> saveAs;
    public BBParameter<bool> saveAsIsEarlyAccessSlot;

    protected override string info
    {
        get {return string.Format("Get SlotInfo by {0} and save as {1}", gameId, saveAs);}
    }

    protected override void OnExecute()
    {
        Blackboard slotInfoBB = BlackboardQueryUtils.GetEarlyAccessSlotInfo(gameId.value);
        if(slotInfoBB == null)
        {
            saveAs.value = BlackboardQueryUtils.GetSlotInfoBB(gameId.value);
            saveAsIsEarlyAccessSlot.value = false;
        }
        else
        {
            saveAs.value = slotInfoBB;
            saveAsIsEarlyAccessSlot.value = true;
        }
        
        EndAction();
    }
}

}
