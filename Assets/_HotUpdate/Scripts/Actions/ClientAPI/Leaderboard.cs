using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class Leaderboard : ActionTask
{
    public BBParameter<int> priodType;
    public BBParameter<int> sortType;

    protected override string info { get { return "Reqeust Leaderboard"; } }

    protected override void OnExecute()
    {
        BagelCodeClientAPI.Leaderboard(
        (PeriodTypes)priodType.value,
        (SortTypes)sortType.value,
        (response) =>
        {
            var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "leaderboardResponse"); 
            
            bb.AddVariable("sortType", typeof(int));
            bb.SetValue("sortType", sortType.value);

            bb.AddVariable("priodType", typeof(int));
            bb.SetValue("priodType", priodType.value);

            string bbName = string.Format("{0}_{1}", sortType.value, priodType.value);

            BlackboardUtils.DestroyBlackboard(bb, bbName);
            var responseBB = BlackboardUtils.GetOrCreateBlackboard(bb, bbName); 

            ClientAPI2Blackboard.Serialize(responseBB, response);
            EndAction(true);
            
        },
        (error) =>
        {
            // EndAction(false);
        });
    }
}

}
