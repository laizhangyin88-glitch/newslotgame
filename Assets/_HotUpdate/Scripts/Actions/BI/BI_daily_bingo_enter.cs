using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_daily_bingo_enter : ActionTask<Blackboard>
{
    protected override void OnExecute()
    {
        Analytics.CustomEvent("client_bingo_of_the_month_enter", new Dictionary<string, object>());
        
        EndAction();
    }
}

}
