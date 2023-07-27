using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]

public class ApplyLuckySpinCollectGameSpin : ActionTask <Blackboard> 
{
    protected override string info
    { 
        get 
        { 
            return string.Format("Apply Lucky Spin. Collect Game Spin");
        } 
    }
    protected override void OnExecute()
    {
        var gameID = BlackboardUtils.FindVariable<int>(ContentBlackboard.Get(), "gameSpinResult/gameId");
        var addedSpinCount = BlackboardUtils.FindVariable<int>(ContentBlackboard.Get(), "gameSpinResult/addedSpinCount");

        BlackboardQueryUtils.AddGameSpinCount( gameID.value, addedSpinCount.value );
        
        EndAction();
    }
}

}
