using UnityEngine;
using System.Collections;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/SlotMachine/ReelMovement")]
public class SetVelocity : ActionTask
{
	public BBParameter<Vector3> velocity;
	public OperationMethod Operation = OperationMethod.Set;
	public BBParameter<Vector3> saveAs;

	protected override void OnExecute()
	{
		saveAs.value = OperationUtils.Operate(saveAs.value, velocity.value, Operation);
        EndAction();
	}
}

}
