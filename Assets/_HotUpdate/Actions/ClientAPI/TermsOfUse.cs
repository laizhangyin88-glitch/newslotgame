using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class TermsOfUse : ActionTask
{
	public BBParameter<string> contextID;

	protected override string info { get { return "Request TOU"; } }

	protected override void OnExecute()
	{
		var termsOfUseType = BlackboardUtils.FindVariable<ClientModels.TermsOfUseType>(MainBlackboard.Get(), "termsOfUseType");
		var version = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "/values/misc/TERMS_OF_USE_VERSION");

		if (version == null || termsOfUseType == null)
		{
			EndAction(false);
			return;
		}


		BagelCodeClientAPI.TermsOfUse(version.value, contextID.value, termsOfUseType.value,
			(response) =>
			{
                if(agent != null)
                {
                    BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "/values/misc/ENABLE_TERMS_OF_USE").value = false;
                    ClientAPI2Blackboard.Serialize(MainBlackboard.Get(), response);
                    EndAction();
                }

			},
			(error) =>
			{
                if(agent != null)
                {
                    GlobalErrorHandler.GlobalError(error);
                    EndAction(false);
                }
			}
		);
	}
}

}
