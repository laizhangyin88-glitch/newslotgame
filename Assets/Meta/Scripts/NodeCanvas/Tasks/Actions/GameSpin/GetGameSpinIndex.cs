using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Task.Action
{

[Category("★ BagelCode/GameSpin")]
public class GetGameSpinIndex : ActionTask<Blackboard>
{
    public bool fromLast;

    [BlackboardOnly]
    public BBParameter<int> saveAsIndex;
    public BBParameter<int> saveAsSpinCount;

    protected override string info
    {
        get
        {
            return string.Format("{0} = Get Game Spin Index {1}", saveAsIndex, fromLast ? "fromLast" : "");
        }
    }

    protected override void OnExecute()
    {
        saveAsIndex.value = -1;
        saveAsSpinCount.value = 0;

        var gameSpinList = BlackboardUtils.FindVariable<List<int>>(null, "./gameSpinList");

        if(gameSpinList != null && gameSpinList.value.Count > 0)
        {
            for(int i=0; i<gameSpinList.value.Count; ++i)
            {
                if(gameSpinList.value[i] > 0)
                {
                    saveAsIndex.value = i;
                    saveAsSpinCount.value = gameSpinList.value[i];
                    
                    if(!fromLast)
                        break;
                }
            }
        }

        EndAction();
    }
}

}
