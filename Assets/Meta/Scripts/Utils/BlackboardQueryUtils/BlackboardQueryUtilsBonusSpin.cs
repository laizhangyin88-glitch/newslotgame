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
    private static readonly string BONUS_SPIN_LIST = "bonusSpinList";
    private static readonly string BONUS_SPIN_INFO = "BonusSpinInfo";
    private static readonly string END_TIMESTAMP = "endTimestamp";
    private static readonly string TOTAL_BONUS_SPIN_COUNT = "spinCount";

    public static List<Blackboard> GetBonusSpinList()
    {
        return MainBlackboard.Get().GetValue<List<Blackboard>>(BONUS_SPIN_LIST);
    }

    public static long GetBonusSpinEndTimestamp(int gameId)
    {
        var bonusSpinList = GetBonusSpinList();
        for (int i = 0; i < bonusSpinList.Count; ++i)
        {
            var bonusSpin = bonusSpinList[i];
            if (bonusSpin.GetValue<int>(GAME_ID) == gameId)
            {
                return bonusSpin.GetValue<long>(END_TIMESTAMP);
            }
        }
        return 0L;
    }

    public static int GetBonusSpinTotalCount(int gameId)
    {
        return GetBonusSpinTotalCount(gameId, GetBonusSpinList());
    }

    public static void UpdateBonusSpinCount(int gameId, int spinCount)
    {
        UpdateBonusSpinCount(gameId, spinCount, true, GetBonusSpinList());
    }

    private static int GetBonusSpinTotalCount(int gameId, List<Blackboard> gameSpinList)
    {
        int totalSpinCount = 0;

        for (int i = 0; i < gameSpinList.Count; ++i)
        {
            var gameSpin = gameSpinList[i];
            if (gameSpin.GetValue<int>(GAME_ID) == gameId)
                totalSpinCount += gameSpin.GetValue<int>(TOTAL_BONUS_SPIN_COUNT);
        }

        return totalSpinCount;
    }

    private static void UpdateBonusSpinCount(int gameId, int spinCount, bool reset, List<Blackboard> gameSpinList)
    {
        for (int i = 0; i < gameSpinList.Count; ++i)
        {
            var gameSpin = gameSpinList[i];
            if (gameSpin.GetValue<int>(GAME_ID) == gameId)
            {
                var spinCountVariable = gameSpin.GetVariable<int>(TOTAL_BONUS_SPIN_COUNT);
                spinCountVariable.value = reset ? spinCount : (spinCountVariable.value + spinCount);
                return;
            }
        }
        
        var newBB = BlackboardUtils.CreateBlackboard(BONUS_SPIN_INFO);
        newBB.AddVariable(GAME_ID, gameId);
        newBB.AddVariable(TOTAL_BONUS_SPIN_COUNT, spinCount);
        BlackboardUtils.AddToBlackboardList(MainBlackboard.Get(), BONUS_SPIN_LIST, newBB);
    }
}

}

