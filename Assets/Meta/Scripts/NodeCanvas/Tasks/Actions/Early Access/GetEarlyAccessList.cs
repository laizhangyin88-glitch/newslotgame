using System;
using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/EarlyAccess")]

public class GetEarlyAccessList : ActionTask<Blackboard>
{
    public BBParameter<List<Blackboard>> slotInfoList;
    public BBParameter<List<Blackboard>> gameInfoList;

    protected override string info
    {
        get { return "Get Early Access Slot,Game Info List"; }
    }

    protected override void OnExecute()
    {
        slotInfoList.value = BlackboardQueryUtils.GetEarlyAccessSlotList();
        gameInfoList.value = BlackboardQueryUtils.GetEarlyAccessGameInfoList();

        if(slotInfoList.value == null || slotInfoList.value.Count == 0 ||
           gameInfoList.value == null || gameInfoList.value.Count == 0 )
        {
            slotInfoList.value = new List<Blackboard>();
            gameInfoList.value = new List<Blackboard>();
        }

        EndAction();
    }
}

}
