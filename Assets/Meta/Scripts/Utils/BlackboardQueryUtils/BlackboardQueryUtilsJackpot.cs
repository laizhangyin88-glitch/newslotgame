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
    public static void InitJackpotInfo(Blackboard jackpotInfo)
    {
        var progress = BlackboardUtils.FindVariable(jackpotInfo, "progress");

        if(progress == null || progress.value == null)
        {
            BlackboardUtils.SetOrCreateValue<long>(jackpotInfo, "progress", 0L);
        }
    }

    public static void ResetJackpotInfo(Blackboard jackpotInfo)
    {
        if(jackpotInfo == null) return;

        BlackboardUtils.SetOrCreateValue<long>(jackpotInfo, "current", jackpotInfo.GetValue<long>("min"));
        BlackboardUtils.SetOrCreateValue<long>(jackpotInfo, "prev", jackpotInfo.GetValue<long>("min"));
        BlackboardUtils.SetOrCreateValue<bool>(jackpotInfo, "alarm", false);

        var progress = BlackboardUtils.FindVariable(jackpotInfo, "progress");

        if(progress == null || progress.value == null)
        {
            jackpotInfo.RemoveVariable("progress");
        }
    }

    public static Blackboard GetJackpotBlackboardForLobby(List<Blackboard> bb, int gameID)
    {
        if (bb != null)
        {
            for(int i=0; i<bb.Count; ++i)
            {
                Variable<int> currentGameID = BlackboardUtils.FindVariable<int>(bb[i], "gameId");

                if(currentGameID.value == gameID)
                {
                    var jackpotBB = BlackboardUtils.FindVariable<List<Blackboard>>( bb[i], "jackpots" );

                    if(jackpotBB != null)
                        return bb[i];
                }
            }
        }
        return null;
    }

    public static List<Blackboard> GetJackpotListForLobby(List<Blackboard> bb, int gameID)
    {
        if (bb != null)
        {
            for(int i=0; i<bb.Count; ++i)
            {
                Variable<int> currentGameID = BlackboardUtils.FindVariable<int>(bb[i], "gameId");

                if(currentGameID.value == gameID)
                {
                    var jackpotListBB = BlackboardUtils.FindVariable<List<Blackboard>>( bb[i], "jackpots" );

                    if(jackpotListBB != null)
                        return jackpotListBB.value;
                }
            }
        }
        return null;
    }

    public static Blackboard GetMetaJackpotInfo(MetaJackpotType type)
    {
        if(MetaJackpotType.UNKNOWN == type) return null;

        var metaJackpotInfoBB = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "metaJackpotInfoDict");

        if(metaJackpotInfoBB != null && metaJackpotInfoBB.value != null)
        {
            return BlackboardUtils.FindVariable<Blackboard>(metaJackpotInfoBB.value, type.ToString()).value;
        }

        return null;
    }

    public static void UpdateMetaJackpotInfo(MetaJackpotType type, List<JackpotInfo> updateInfo, long serverTimestamp)
    {
        if(MetaJackpotType.UNKNOWN == type) return;

        Blackboard metaJackpotInfoBB = GetMetaJackpotInfo(type);

        if(metaJackpotInfoBB != null)
        {
            var timestamp = metaJackpotInfoBB.GetVariable<long>("serverTime");

            if(timestamp == null || timestamp.value < serverTimestamp)
            {
                var jackpotInfoList = BlackboardUtils.FindVariable<List<Blackboard>>(metaJackpotInfoBB, "value");

                long progressValue = 0L;
                var progress = BlackboardUtils.FindVariable<long>(jackpotInfoList.value[0], "progress");
                if(progress != null)
                {
                    progressValue = progress.value;
                }

                BlackboardUtils.SetOrCreateList(metaJackpotInfoBB, "value", updateInfo, BagelCode.ClientAPI2Blackboard.Serialize);

                jackpotInfoList = BlackboardUtils.FindVariable<List<Blackboard>>(metaJackpotInfoBB, "value");
                BlackboardUtils.SetOrCreateValue<long>(jackpotInfoList.value[0], "progress", progressValue);

                BlackboardUtils.SetOrCreateValue( metaJackpotInfoBB, "serverTime", serverTimestamp);
            }
        }
    }

    public static void CreateDummyJackpotInfo(MetaJackpotType type)
    {
        if(MetaJackpotType.UNKNOWN == type) return;

        var metaJackpotInfoBB = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "metaJackpotInfoDict");

        var jackpotBoard = BlackboardUtils.GetOrCreateBlackboard(metaJackpotInfoBB.value, type.ToString());
        // BlackboardUtils.GetOrCreateBlackboardList(jackpotBoard, "value");

        // todo. scalable
        long jackpotAmount = BlackboardUtils.FindValue<long>(MainBlackboard.Get(), "values/dailyMegaWheel/jackpotAmount");

        List<JackpotInfo> infoList = new List<JackpotInfo>();

        JackpotInfo info = new JackpotInfo();
        info.current = jackpotAmount;
        info.prev = jackpotAmount;
        info.deltaMs = 300000;
        info.min = jackpotAmount;
        info.max = jackpotAmount;
        info.alarm = false;

        infoList.Add(info);

        BlackboardUtils.SetOrCreateList<JackpotInfo>(jackpotBoard, "value", infoList, ClientAPI2Blackboard.Serialize);
    }

    // public static List<Blackboard> GetMetaJackpotInfoList(MetaJackpotType type)
    // {
    //     if(MetaJackpotType.UNKNOWN == type) return null;

    //     Blackboard jackpotInfoBB = GetMetaJackpotInfoBB(type);

    //     if(jackpotInfoBB != null)
    //     {
    //         return BlackboardUtils.FindVariable<List<Blackboard>>(jackpotInfoBB, "jackpotList").value;
    //     }

    //     return null;
    // }

    // public static Blackboard GetMetaJackpotInfo(MetaJackpotType type, int index)
    // {
    //     if(MetaJackpotType.UNKNOWN == type || index < 0) return null;

    //     List<Blackboard> jackpotList = GetMetaJackpotInfoList(type);

    //     if(jackpotList.Count > index)
    //     {
    //         return jackpotList[index];
    //     }

    //     return null;
    // }
}

}

