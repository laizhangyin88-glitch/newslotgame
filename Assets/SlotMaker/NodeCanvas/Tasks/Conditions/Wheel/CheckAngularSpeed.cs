using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Actions{

	[Category("GameObject")]
	[Description("Checks the current angular speed of the agent against a value based on it's Rigidbody velocity")]
	public class CheckAngularSpeed : ConditionTask<Rigidbody>{

		public CompareMethod checkType = CompareMethod.EqualTo;
		public BBParameter<float> value;

		[SliderField(0,0.1f)]
		public float differenceThreshold = 0.05f;

		protected override string info{
			get	{return "Angular Speed" + OperationTools.GetCompareString(checkType) + value;}
		}

		protected override bool OnCheck(){
			var speed = agent.angularVelocity.magnitude;
			return OperationTools.Compare((float)speed, (float)value.value, checkType, differenceThreshold);
		}
	}
}
