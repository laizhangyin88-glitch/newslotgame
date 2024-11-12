using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
	[Category("★ BagelCode/Utils")]
	public class UpdateContentCount : ActionTask
	{
		public BBParameter<ContextElement> creditText;
		public BBParameter<long> unitCount;
		public BBParameter<long> targetCount;

		protected override void OnExecute()
		{
			var turnCredit = ownerAgent.GetComponent<ContentCountAnimator>();
			turnCredit.UpdateCount(creditText.value as IContextText, unitCount.value, targetCount.value);

			EndAction();
		}
	}
}
