using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
	[Category("★ BagelCode/Utils")]
	public class SetContentCount : ActionTask
	{
		public BBParameter<ContextElement> creditText;
		public BBParameter<long> targetCredit;

		protected override void OnExecute()
		{
			var turnCredit = ownerAgent.GetComponent<ContentCountAnimator>();
			turnCredit.SetCount(creditText.value as IContextText, targetCredit.value);

			EndAction();
		}
	}
}
