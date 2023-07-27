using UnityEngine;
using System.Collections;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

    [Category("★ SlotMaker/SlotMachine/ReelMovement")]
    public class SetImpulse : ActionTask
    {
    	public BBParameter<Vector3> impulse;
    	public OperationMethod Operation = OperationMethod.Set;
    	public BBParameter<Vector3> saveAs;

    	protected override void OnExecute()
    	{
    		saveAs.value = OperationUtils.Operate(saveAs.value, impulse.value, Operation);
    		EndAction();
    	}
    }

}
