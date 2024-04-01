using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.TestSuite.Tasks.Actions
{
    [Category("★ SlotMaker/TestSuite")]
    public class LoginTestSuite : ActionTask
    {
    	protected override void OnExecute()
    	{
#if DEV && !NEW_NET
            TestSuiteServer.Login(EndAction, EndAction);
#else
            EndAction();
#endif
    	}
    }
}
