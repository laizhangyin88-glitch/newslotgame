using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/Lucky Spin")]

public class InitLuckySpin : ActionTask <Blackboard> 
{
    protected override string info
    { 
        get 
        { 
            return string.Format("Init Lucky Spin");
        } 
    }
    protected override void OnExecute()
    {
        BlackboardQueryUtils.CreateLuckySpinReelStripsBB();
        
        EndAction();
    }
}

}
