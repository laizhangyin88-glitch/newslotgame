using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class GDPRConsent : ActionTask
{
	protected override string info { get { return "Request GDPR Consent"; } }

	protected override void OnExecute()
	{
		var version = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "/values/misc/POLICY_VERSION");

		if (version == null)
		{
			EndAction(false);
			return;
		}

		BagelCodeClientAPI.Consent(version.value, 
			(response) => 
			{
				ClientAPI2Blackboard.Serialize(MainBlackboard.Get(), response);
				EndAction();
			},
			(error) =>
			{
                GlobalErrorHandler.GlobalError(error);
				EndAction(false);
			}
		);
	}
}
	
}
