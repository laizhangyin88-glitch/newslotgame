using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_fb_connect : ActionTask<Blackboard>
{
    protected override void OnExecute()
    {
        AdjustManager.Instance.SendEvent("fb_connect");

        EndAction();
    }
}

}
