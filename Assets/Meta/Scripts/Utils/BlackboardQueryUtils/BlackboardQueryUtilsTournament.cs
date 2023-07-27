using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode
{

public static partial class BlackboardQueryUtils
{
    static public void UpdateTournament(TournamentInfoForMetaInfo tourInfo)
    {
        if(tourInfo == null) return;

        var tournamentInfoBB = BlackboardUtils.GetOrCreateVariable<Blackboard>(ContentBlackboard.Get(), "tournamentInfo");

        var tournamentId = BlackboardUtils.GetOrCreateVariable<string>(tournamentInfoBB.value, "tournamentId");
        if(tournamentId.value == tourInfo.tournamentId)
        {
            var revision = BlackboardUtils.GetOrCreateVariable<double>(tournamentInfoBB.value, "revision");
            if(revision.value > tourInfo.revision) return;
        }

        ClientAPI2Blackboard.Serialize(tournamentInfoBB.value, tourInfo);

        var isChanged = BlackboardUtils.GetOrCreateVariable<bool>(tournamentInfoBB.value, "isChanged");
        isChanged.value = true;
    }

    static public int GetTournamentPeriodMin()
    {
        var tournamentInfoBB = BlackboardUtils.GetOrCreateVariable<Blackboard>(ContentBlackboard.Get(), "tournamentInfo");

        var startTime = BlackboardUtils.GetOrCreateVariable<long>(tournamentInfoBB.value, "startTimestamp");
        var endTime = BlackboardUtils.GetOrCreateVariable<long>(tournamentInfoBB.value, "endByTimestamp");

        TimeSpan leftTime = TimeUtils.ParseTimestampToDateTime(endTime.value) - TimeUtils.ParseTimestampToDateTime(startTime.value);

        return (int)leftTime.TotalMinutes;
    }
}

}

