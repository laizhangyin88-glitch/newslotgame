using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

    [Category("★ SlotMaker/SlotMachine/ReelMovement")]
    public class WaitChangedDirection : ActionTask
    {
    	public BBParameter<Vector3> direction;
        public BBParameter<Vector3> waitDirection = Vector3.down;

    	protected override void OnUpdate()
    	{
            if (Vector3.Dot(direction.value.normalized, waitDirection.value) < 0f)
    		{
    			EndAction();
    		}
    	}
    }

}
