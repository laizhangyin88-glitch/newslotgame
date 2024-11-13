using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Task.Action
{

[Category("★ BagelCode/Feature Lock")]
public class GetUnlockSlotList : ActionTask
{
    public BBParameter<int> beforeLevel;
    public BBParameter<int> currentLevel;

    [BlackboardOnly]
    public BBParameter<List<Blackboard>> saveAsList;

    protected override string info
    {
        get {return string.Format("{0} = Get Unlock Slot List ", saveAsList);}
    }

    protected override void OnExecute()
    {
        saveAsList.value = BlackboardQueryUtils.GetUnlockSlotInfoList(beforeLevel.value, currentLevel.value);
        
        EndAction();
    }
}

}
