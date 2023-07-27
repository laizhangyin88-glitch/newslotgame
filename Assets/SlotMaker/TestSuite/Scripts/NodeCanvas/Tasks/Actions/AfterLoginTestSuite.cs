using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.TestSuite.Tasks.Actions
{
	[Category("★ SlotMaker/TestSuite")]
	public class AfterLoginTestSuite : ActionTask
	{
		protected override void OnExecute()
		{
	#if DEV
			TestSuiteManager.Instance.OpenEnterGameApp(EndAction);
	#else
			EndAction();
	#endif
		}
	}
}
