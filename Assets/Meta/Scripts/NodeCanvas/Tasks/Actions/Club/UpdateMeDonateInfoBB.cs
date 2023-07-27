using System;
using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Club")]

public class UpdateMeDonateInfoBB : ActionTask<Blackboard>
{
    public BBParameter<string>  valueA;
    public BBParameter<long> donateValue;

    protected override string info
    {
        get { return "Update Me Donate Info BB"; }
    }

    protected override void OnExecute()
    {
        var memberListInfoBB = BlackboardUtils.FindVariable<List<Blackboard>>(agent, valueA.value);

        if(memberListInfoBB != null)
        {
            var meID = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "me/userId");

            for(int i=0; i<memberListInfoBB.value.Count; ++i)
            {
                var memberID = memberListInfoBB.value[i].GetValue<string>("userId");
                if(memberID == meID.value)
                {
                    var donation = BlackboardUtils.FindVariable<long>(memberListInfoBB.value[i], "donation");
                    donation.value += donateValue.value;
                    break;
                }
            }
        }

        EndAction();
    }
}

}
