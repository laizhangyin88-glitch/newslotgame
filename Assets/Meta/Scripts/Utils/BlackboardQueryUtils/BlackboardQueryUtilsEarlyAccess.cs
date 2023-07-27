using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode
{

public static partial class BlackboardQueryUtils
{
    public static void UpdateEarlyAccessGrade(int targetGrade)
    {
        var isAvailable = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "earlyAccess/isAvailable");
        var grade = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "earlyAccess/grade");
        isAvailable.value = true;
        grade.value = targetGrade;

    }

    public static bool IsEarlyAccessAvailable()
    {
        var isAvailable = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "earlyAccess/isAvailable");

        if(isAvailable != null)
            return isAvailable.value;

        return false;
    }

    public static int GetEarlyAccessGrade()
    {
        var grade = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "earlyAccess/grade");

        if(grade != null)
            return grade.value;

        return -1;
    }

    public static Blackboard GetEarlyAccessSlotInfo(int gameID)
    {
        var slotList = GetEarlyAccessSlotList();

        for(int i=0; i<slotList.Count; ++i)
        {
            var gameId = BlackboardUtils.FindVariable<int>(slotList[i], "gameId");
            if(gameId.value == gameID)
            {
                return slotList[i];
            }
        }

        return null;
    }

    public static List<Blackboard> GetEarlyAccessSlotList()
    {
        var slotList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), "vipSlotList");

        if(slotList != null)
            return slotList.value;

        return null;
    }
    
    public static List<Blackboard> GetEarlyAccessGameInfoList()
    {
        List<Blackboard> earlyAccessList = GetEarlyAccessSlotList();

        if(earlyAccessList != null)
        {
            List<Blackboard> gameInfoList = new List<Blackboard>();

            for(int i=0; i<earlyAccessList.Count; ++i)
            {
                var gameID = BlackboardUtils.FindVariable<int>(earlyAccessList[i], "gameId");
                var gameInfoBB = BlackboardQueryUtils.GetGameInfo(gameID.value);
                if(gameInfoBB != null)
                {
                    gameInfoList.Add(gameInfoBB);
                }
            }

            return gameInfoList;
        }

        return null;
    }

    public static int GetEarlyAccessRemainTotalSpinCount()
    {
        int totalSpinCount = 0;
        var slotInfoList = GetEarlyAccessSlotList();

        if(slotInfoList != null && slotInfoList.Count > 0)
        {
            for(int i=0; i<slotInfoList.Count; ++i)
            {
                var status = BlackboardUtils.FindVariable<int>(slotInfoList[i], "flags/status");

                if(status != null && (status.value == 0 || status.value == 4) )
                {
                    var gameID = BlackboardUtils.FindVariable<int>(slotInfoList[i], "gameId");
                    totalSpinCount += GetBonusSpinTotalCount(gameID.value);
                }
            }
        }

        return totalSpinCount;
    }

    public static int GetEarlyAccessGameID()
    {
        int gameID = 0;
        var slotInfoList = GetEarlyAccessSlotList();

        if(slotInfoList != null && slotInfoList.Count > 0)
        {
            for(int i=0; i<slotInfoList.Count; ++i)
            {
                var status = BlackboardUtils.FindVariable<int>(slotInfoList[i], "flags/status");

                if(status != null && (status.value == 0 || status.value == 4) )
                {
                    gameID = BlackboardUtils.FindVariable<int>(slotInfoList[i], "gameId").value;
                    break;
                }
            }
        }

        return gameID;
    }

    public static Blackboard GetEarlyAccessGradeInfo(int index)
    {
        var earlyAccessInfoList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), "values/earlyAccess");

        if(earlyAccessInfoList != null && earlyAccessInfoList.value.Count > index)
        {
            return earlyAccessInfoList.value[index];
        }

        return null;
    }

    public static int GetEarlyAccessMaxCount(bool isMaxGrade = false)
    {
        int maxGrade = 0;

        if(isMaxGrade)
        {
            var earlyAccessInfoList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), "values/earlyAccess");
            if(earlyAccessInfoList != null)
            {
                maxGrade = earlyAccessInfoList.value.Count - 1;
            }
        }

        return GetEarlyAccessMaxCount(maxGrade);
    }

    public static int GetEarlyAccessMaxCount(int grade)
    {
        var gradeInfo = GetEarlyAccessGradeInfo(grade);

        if(gradeInfo != null)
        {
            var bonusSpinCount = BlackboardUtils.FindVariable<int>(gradeInfo, "bonusSpinCount");
            return bonusSpinCount.value;
        }

        return 0;
    }

    public static int RefillEarlyAccessBonusSpins(int grade)
    {
        var gradeInfo = GetEarlyAccessGradeInfo(grade);

        if(gradeInfo != null)
        {
            var slotInfoList = GetEarlyAccessSlotList();

            if(slotInfoList != null)
            {
                var refillSpinCount = BlackboardUtils.FindVariable<int>(gradeInfo, "bonusSpinCount");

                for(int i=0; i<slotInfoList.Count; ++i)
                {
                    var status = BlackboardUtils.FindVariable<int>(slotInfoList[i], "flags/status");

                    if(status != null && status.value != 1 && status.value != 2)
                    {
                        var gameID = BlackboardUtils.FindVariable<int>(slotInfoList[i], "gameId");
                        UpdateBonusSpinCount(gameID.value, refillSpinCount.value);
                    }
                }

                return refillSpinCount.value;
            }
        }

        return 0;
    }
}

}

