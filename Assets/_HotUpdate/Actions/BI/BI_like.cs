using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

public enum BILikeType
{
	INGAME,
	PROFILE,
	WALLOFEPIC
}

[Category("★ BagelCode/BI")]
public class BI_like : ActionTask
{
    public BBParameter<BILikeType> type;
    public BBParameter<string> userId;
    private string[] BILikeTypeString = {"in_game", "profile", "woe"};

    protected override void OnExecute()
    {
        Analytics.CustomEvent("client_like", new Dictionary<string, object>
        {
            { "type", BILikeTypeString[(int)type.value] },
            { "target_user_id", userId.value },
            { "slot_enter_context_id", BiEventUtils.GetSlotEnterContextID() }
        });

        EndAction();
    }
}

}
