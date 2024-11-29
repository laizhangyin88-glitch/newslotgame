using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_rate_us_collect : ActionTask<Blackboard>
{
    public BBParameter<int> rating;
    
	protected override void OnExecute()
	{
		var earnCredit = BlackboardUtils.FindVariable<long>(agent, "earnCredit");
        
        Analytics.CustomEvent("client_rate_us_collect", new Dictionary<string, object>
		{
			{ "earn_coin", earnCredit.value },
            { "rating", rating.value }
		});

		EndAction();
	} 
}

}
