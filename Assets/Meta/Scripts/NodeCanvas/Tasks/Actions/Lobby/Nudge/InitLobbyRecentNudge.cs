using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Lobby")]
public class InitLobbyRecentNudge : ActionTask 
{
    private GameObject recentObj = null;

    protected override string info
    {
        get { return "Init Lobby Recent Version Nudge"; }
    }

    protected override void OnExecute()
    {
        int clientVersion = ApplicationSettings.GetClientVersionNumber();
        int recentVersion = BlackboardUtils.FindVariable<int>(null, "/recentClientNumberVersion").value;

        if(recentObj == null && clientVersion < recentVersion)
        {
            recentObj = MetaObjectUtils.MakeScene(MetaStringDefine.LOBBY_BUNDLE_NAME, "Lobby Recent Version Update Nudge Scene", agent.transform, "" );

            // BICustomEvents.UpdateAppRecommand(recentVersion); // send bi event
        }

        EndAction();
    }
}

}
