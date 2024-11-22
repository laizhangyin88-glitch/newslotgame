using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Task.Actions
{

[Category("★ BagelCode/Purchase")]
public class CheckPurchaseProhibitedRegion : ActionTask<Blackboard> 
{
	protected override void OnExecute()
    {
    	EndAction(!BlackboardQueryUtils.CheckPurchaseProhibitedRegion());
    }
}

}
