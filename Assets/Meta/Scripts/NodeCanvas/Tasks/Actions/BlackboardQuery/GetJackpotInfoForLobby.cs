using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BlackboardQuery
{

[Category("★ BagelCode/Blackboard Query")]
public class GetJackpotInfoForLobby : ActionTask<Blackboard>
{
    public BBParameter<string>  pathValue;

    public BBParameter<string>  idValue;

    [BlackboardOnly]
    public BBParameter<Blackboard> saveAs;

    [BlackboardOnly]
    public BBParameter<List<Blackboard>> saveJackpotList;

    protected override string info
    {
        get { return string.Format("{0} Get Jackpot Info For Lobby {1}", saveAs, idValue); }
    }

    protected override void OnExecute()
    {
        var jackpotInfoList = BlackboardUtils.FindVariable<List<Blackboard>>(agent, pathValue.value);

        Variable<int> findGameID = BlackboardUtils.FindVariable<int>(agent, idValue.value);

        saveAs.value = BlackboardQueryUtils.GetJackpotBlackboardForLobby(jackpotInfoList.value, findGameID.value);
        saveJackpotList.value = BlackboardQueryUtils.GetJackpotListForLobby(jackpotInfoList.value, findGameID.value);

        EndAction();
    }
}

}
