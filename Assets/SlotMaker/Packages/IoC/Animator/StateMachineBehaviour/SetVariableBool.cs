using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;

namespace SlotMaker.IoC.AnimatorBehaviour
{
	public class SetVariableBool : StateMachineBehaviour
	{
	    public bool atExit;
	    public VariableBool boolValue;
	    public BoolSetModes setTo;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (!atExit) boolValue.value = OperationUtils.Operate(boolValue.value, setTo);
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (atExit) boolValue.value = OperationUtils.Operate(boolValue.value, setTo);
		}
	}
}