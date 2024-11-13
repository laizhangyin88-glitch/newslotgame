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
public class SetWinShareNudgeInfo : ActionTask<Blackboard>
{
    public BBParameter<string> winTypeValue;
    public BBParameter<string> filePathValue;
    public BBParameter<string> winCoinsValue;
    public BBParameter<string> betCoinsValue;
    public BBParameter<string> gameIdValue;

    protected override string info
    {
        get { return "Set Win Share Nudge Info"; }
    }

    protected override void OnExecute()
    {
        var winType     = BlackboardUtils.FindVariable<int>(agent,    winTypeValue.value);
        var filePath    = BlackboardUtils.FindVariable<string>(agent, filePathValue.value);
        var winCoins    = BlackboardUtils.FindVariable<long>(agent,   winCoinsValue.value);
        var betCoins    = BlackboardUtils.FindVariable<long>(agent,   betCoinsValue.value);
        var gameID      = BlackboardUtils.FindVariable<int>(agent,    gameIdValue.value);

        BlackboardUtils.DestroyBlackboard(MainBlackboard.Get(), "winShareNudgeInfo");

        var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "winShareNudgeInfo");

        BlackboardUtils.SetOrCreateValue(bb, "winType", winType.value);
        BlackboardUtils.SetOrCreateValue(bb, "filePath", filePath.value);
        BlackboardUtils.SetOrCreateValue(bb, "winCoins", winCoins.value);
        BlackboardUtils.SetOrCreateValue(bb, "betCoins", betCoins.value);
        BlackboardUtils.SetOrCreateValue(bb, "gameID", gameID.value);

        EndAction();
    }
}

}
