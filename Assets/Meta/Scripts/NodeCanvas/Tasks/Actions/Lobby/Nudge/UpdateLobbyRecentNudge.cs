using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Lobby")]
public class UpdateLobbyRecentNudge : ActionTask<ContextElement>
{
    public BBParameter<string> saveAsAppDownloadUrl;
    public BBParameter<bool> saveAsFold;

    private const StringTable.StringTableType tableType = StringTable.StringTableType.Global;

    private const string LAST_RECENT_NUDGE_VERSION_KEY = "LAST_RECENT_NUDGE_VERSION";

    protected override string info
    {
        get { return "Update Lobby Recent Version Nudge"; }
    }

    protected override void OnExecute()
    {
        int recentVersion = BlackboardUtils.FindVariable<int>(null, "/recentClientNumberVersion").value;
        int lastRecentVersion = PlayerPrefs.GetInt(LAST_RECENT_NUDGE_VERSION_KEY, 0);

        saveAsFold.value = lastRecentVersion >= recentVersion;
        if(lastRecentVersion < recentVersion)
        {
            PlayerPrefs.SetInt(LAST_RECENT_NUDGE_VERSION_KEY, recentVersion);
        }

        saveAsAppDownloadUrl.value = BlackboardUtils.FindVariable<string>(null, "/appDownloadUrl").value;

        var rewardCoins = BlackboardUtils.FindVariable<long>(null, "/values/misc/VERSION_UPDATE_REWARD_CREDIT");

        agent.UpdateContext(false);
        ContextElement downloadButton = ContextUtils.FindElement(agent, "Button Blue", ContextSearchingType.ChildrenSearch);
        MetaContextElementUtils.SimpleSetText(downloadButton, "Text", StringTableUtils.GetString(tableType, "COMMA_STYLE_COIN", rewardCoins.value));
        MetaContextElementUtils.SetClickable(
            downloadButton,
            "OnAppDownload",
            false,
            false,
            SendEvent,
            ownerSystem
        );

        ContextElement toggleButton = ContextUtils.FindElement(agent, "Toggle Button", ContextSearchingType.ChildrenSearch);
        MetaContextElementUtils.SetClickable(
            toggleButton,
            "OnToggle",
            false,
            false,
            SendEvent,
            ownerSystem
        );

        EndAction();
    }
}

}
