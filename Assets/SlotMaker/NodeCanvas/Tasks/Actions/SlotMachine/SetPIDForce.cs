using UnityEngine;
using System.Collections;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

    [Category("★ SlotMaker/SlotMachine/ReelMovement")]
    public class SetPIDForce : ActionTask
    {
    	public BBParameter<Vector3> position;
    	public BBParameter<Vector3> desiredPosition;
    	public BBParameter<Vector3> velocity;
    	public BBParameter<Vector3> desiredVelocity;
    	public BBParameter<float> frequency;
    	public BBParameter<float> damping;
    	public BBParameter<float> epsilon;
    	public OperationMethod Operation = OperationMethod.Set;
    	public BBParameter<Vector3> saveAs;

    	protected override void OnUpdate()
    	{
    		if ((Vector3.Distance(position.value, desiredPosition.value) < epsilon.value) &&
    			(Vector3.Distance(velocity.value, desiredVelocity.value) < epsilon.value))
    		{
    			EndAction();
    			return;
    		}

    		float ksg, kdg;
            PIDUtils.CalcCoefficient(frequency.value, damping.value, Time.fixedDeltaTime, out ksg, out kdg);
    		Vector3 force = PIDUtils.CalcForce(position.value, desiredPosition.value, velocity.value, desiredVelocity.value, ksg, kdg);
    		saveAs.value = OperationUtils.Operate(saveAs.value, force, Operation);
    	}
    }

}
