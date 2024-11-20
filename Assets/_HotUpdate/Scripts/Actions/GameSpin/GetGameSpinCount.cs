using System.Collections;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Task.Action
{

[Category("★ BagelCode/GameSpin")]
public class GetGameSpinCount : ActionTask<Blackboard>
{
    public BBParameter<string> gameId;

    [BlackboardOnly]
    public BBParameter<int> gameSpinCount;

    protected override string info
    {
        get
        {
            return string.Format("{0} = Get Game Spin Count({1})", gameSpinCount, gameId);
        }
    }

    protected override void OnExecute()
    {
        int gameID = BlackboardUtils.FindVariable<int>(agent, gameId.value).value;

        gameSpinCount.value = BlackboardQueryUtils.GetGameSpinTotalCount(gameID);
            
        EndAction();
    }
}

}
