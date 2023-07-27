using UnityEngine;
using System.Collections;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{
    [Category("★ SlotMaker/SlotMachine/ReelMovement")]
    public class SetForce : ActionTask
    {
    	public BBParameter<Vector3> force;
    	public BBParameter<float> time;
    	public OperationMethod Operation = OperationMethod.Set;
    	public BBParameter<Vector3> saveAs;

    	private float remaningTime;

    	protected override void OnExecute()
    	{
    		remaningTime = time.value;
    	}

    	protected override void OnUpdate()
    	{
    		saveAs.value = OperationUtils.Operate(saveAs.value, force.value, Operation);

            remaningTime -= Time.fixedDeltaTime;
    		if (remaningTime <= 0f)
    			EndAction();
    	}
    }
}
