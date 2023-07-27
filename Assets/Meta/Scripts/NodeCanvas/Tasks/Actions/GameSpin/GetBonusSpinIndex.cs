using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Task.Action
{

[Category("★ BagelCode/EarlyAccess")]
public class GetBonusSpinIndex : ActionTask<Blackboard>
{
    public bool fromLast;

    [BlackboardOnly]
    public BBParameter<int> saveAsIndex;
    public BBParameter<int> saveAsSpinCount;

    protected override string info
    {
        get
        {
            return string.Format("{0} = Get Bonus Spin Index {1}", saveAsIndex, fromLast ? "fromLast" : "");
        }
    }

    protected override void OnExecute()
    {
        saveAsIndex.value = -1;
        saveAsSpinCount.value = 0;

        var bonusSpinList = BlackboardUtils.FindVariable<List<int>>(null, "./bonusSpinList");

        if(bonusSpinList != null && bonusSpinList.value.Count > 0)
        {
            for(int i=0; i<bonusSpinList.value.Count; ++i)
            {
                if(bonusSpinList.value[i] > 0)
                {
                    saveAsIndex.value = i;
                    saveAsSpinCount.value = bonusSpinList.value[i];
                    
                    if(!fromLast)
                        break;
                }
            }
        }

        EndAction();
    }
}

}
