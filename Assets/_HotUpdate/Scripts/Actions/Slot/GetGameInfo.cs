using System.Collections;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Task.Action
{

[Category("★ BagelCode/Utils")]
public class GetGameInfo : ActionTask
{
    public BBParameter<int> gameId;

    [BlackboardOnly]
    public BBParameter<Blackboard> saveAs;
    public BBParameter<long> saveAsMinBet;
    public BBParameter<long> saveAsMaxBet;
    public BBParameter<int>  saveAsLevelRestriction;
    public BBParameter<GameUnlockStatus> saveAsGameUnlockStatus;

    protected override string info
    {
        get {return string.Format("Get GameInfo by {0} and save as {1}", gameId, saveAs);}
    }

    protected override void OnExecute()
    {
        saveAs.value = BlackboardQueryUtils.GetGameInfo(gameId.value);

        if(saveAs.value != null)
        {
            saveAsMinBet.value = saveAs.value.GetValue<long>("minBet");
            saveAsMaxBet.value = saveAs.value.GetValue<long>("maxBet");
            saveAsLevelRestriction.value = saveAs.value.GetValue<int>("minLevel");
            saveAsGameUnlockStatus.value = saveAs.value.GetValue<GameUnlockStatus>("unlockStatus");
        }
        else
        {
            saveAsMinBet.value = 0L;
            saveAsMaxBet.value = 0L;
            saveAsLevelRestriction.value = 0;
            saveAsGameUnlockStatus.value = GameUnlockStatus.UNKNOWN;
        }

        EndAction();
    }
}

}
