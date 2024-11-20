using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_email_connect : ActionTask<Blackboard>
{
    protected override void OnExecute()
	{
        AdjustManager.Instance.SendEvent("email_connect");

		EndAction();
	}	
}

}
