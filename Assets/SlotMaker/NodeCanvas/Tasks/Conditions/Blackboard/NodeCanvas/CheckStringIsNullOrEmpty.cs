using NodeCanvas.Framework;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Conditions{

	[Category("✫ Blackboard")]
	public class CheckStringIsNullOrEmpty : ConditionTask {

		[BlackboardOnly]
		public BBParameter<string> valueA;

		protected override string info{
			get {return valueA + " == null or empty";}
		}

		protected override bool OnCheck(){
			return string.IsNullOrEmpty(valueA.value);
		}
	}
}
