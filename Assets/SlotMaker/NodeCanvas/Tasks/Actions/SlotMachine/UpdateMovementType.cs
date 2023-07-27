using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/SlotMachine")]
public class UpdateMovementType : ActionTask
{
    public BBParameter<int> movementType;
    protected override void OnExecute()
    {
        // TODO
        // Boost mode 개발 시 아래 코드를 수정하여 적용
        if (BlackboardUtils.FindVariable<bool>(null, "./autoSpin").value ||
            BlackboardUtils.FindVariable<Blackboard>(null, "./bonus") != null)
        {
            movementType.value = 1;    
        }
        else 
        {
            movementType.value = 0;
        }

        EndAction();
    }
}

}
