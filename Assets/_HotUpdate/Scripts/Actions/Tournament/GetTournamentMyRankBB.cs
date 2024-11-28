using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Task.Actions
{

[Category("★ BagelCode/Tournament")]
public class GetTournamentMyRankBB : ActionTask<Blackboard>
{
    public BBParameter<string>  pathBB;

    public BBParameter<int> saveAs;
    public BBParameter<long> saveAsScore;

    protected override string info
    {
        get { return string.Format("{0} = GetTournamentMyRank", saveAs); }
    }

    protected override void OnExecute()
    {
        saveAs.value = -1;
        saveAsScore.value = 0;

        var roundBB = BlackboardUtils.GetOrCreateVariable<Blackboard>(agent, pathBB.value);
        var status = BlackboardUtils.GetOrCreateVariable<TournamentStatus>(roundBB.value, "status");

        if(status.value == TournamentStatus.PLAYING)
        {
            var meID = BlackboardUtils.GetOrCreateVariable<string>(MainBlackboard.Get(), "me/userId");
            var myRankList = BlackboardUtils.GetOrCreateVariable<List<Blackboard>>(roundBB.value, "myRankList");

            for(int i=0; i<myRankList.value.Count; ++i)
            {
                var userId = BlackboardUtils.GetOrCreateVariable<string>(myRankList.value[i], "profile/userId");

                if(userId.value == meID.value)
                {
                    saveAs.value = BlackboardUtils.GetOrCreateVariable<int>(myRankList.value[i], "rank").value;
                    saveAsScore.value = BlackboardUtils.GetOrCreateVariable<long>(myRankList.value[i], "score").value;
                    break;
                }
            }
        }

        EndAction();
    }
}

}
