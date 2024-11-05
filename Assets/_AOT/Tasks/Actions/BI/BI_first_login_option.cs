using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

public enum BIFirstLoginType
{
	GUEST,
	FACEBOOK,
    APPLE
}

[Category("★ BagelCode/BI")]
public class BI_first_login_option : ActionTask<Blackboard>
{
	public BBParameter<BIFirstLoginType> loginOption;
	private string[] loginOptionList = {"guest", "fb_connect", "apple_connect"};

    protected override string info
    {
        get
        {
            return string.Format("BI_first_login_option({0})", loginOption);
        }
    }

    protected override void OnExecute()
    {
        string contextId = PlayerPrefs.GetString("VIP_CLUB_FUNNEL_CONTEXT_ID", "");

        Analytics.CustomEvent("client_first_login_option", new Dictionary<string, object>
        {
            { "selected_option", loginOptionList[(int)loginOption.value] },
            { "context_id", contextId }
        });
        EndAction();
    }
}

}
