using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Task.Actions
{

[Category("★ BagelCode/Ingame/")]
public class GetWinShareNudgeInfo : ActionTask<Blackboard>
{
    public BBParameter<int> saveAsWinType;
    public BBParameter<string> saveAsFilePath;
    public BBParameter<long> saveAsWinCoins;
    public BBParameter<long> saveAsBetCoins;
    public BBParameter<int> saveAsGameID;

    protected override string info
    {
        get { return "Get Win Share Nudge Info"; }
    }

    protected override void OnExecute()
    {
        var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "winShareNudgeInfo");

        if(bb != null)
        {
            saveAsWinType.value  = BlackboardUtils.GetOrCreateVariable<int>(bb, "winType").value;
            saveAsFilePath.value = BlackboardUtils.GetOrCreateVariable<string>(bb, "filePath").value;
            saveAsWinCoins.value = BlackboardUtils.GetOrCreateVariable<long>(bb, "winCoins").value;
            saveAsBetCoins.value = BlackboardUtils.GetOrCreateVariable<long>(bb, "betCoins").value;
            saveAsGameID.value   = BlackboardUtils.GetOrCreateVariable<int>(bb, "gameID").value;
        }

        EndAction();
    }
}

}
