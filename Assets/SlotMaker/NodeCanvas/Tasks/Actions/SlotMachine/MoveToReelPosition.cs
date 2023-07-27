using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/SlotMachine")]
public class MoveToReelPosition : ActionTask<Transform> 
{
    public BBParameter<Transform> slotMachine;
    public BBParameter<int> reelIndex;

    protected override string info { get { return string.Format("Move({0})", reelIndex); } }

    protected override void OnExecute()
    {
        var targetPosition = slotMachine.value.GetComponent<BaseSlotMachine>().GetReel(reelIndex.value).rectTransform.anchoredPosition;
        agent.GetComponent<RectTransform>().anchoredPosition = targetPosition;
        EndAction();
    }
}

}
