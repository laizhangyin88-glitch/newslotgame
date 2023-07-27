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
    public const int LUCKY_FIVE_DECK_COUNT = 5;
    private const string LUCKY_FIVE_INFO = "luckyFiveInfo";

    // public static IBlackboard InitLuckyFive(LuckyFiveWinInfo winInfo)
    // {
    //     if(winInfo == null) return null;

    //     var winInfoBB = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), LUCKY_FIVE_INFO);

    //     ClientAPI2Blackboard.Serialize(winInfoBB, winInfo);

    //     BlackboardUtils.SetOrCreateValue<bool>(winInfoBB, "readyDeck", winInfo.cardList.Count == LUCKY_FIVE_DECK_COUNT);

    //     InitMetaGame();

    //     return winInfoBB;
    // }

    // public static void UpdateLuckyFive(LuckyFiveWinInfo winInfo)
    // {
    //     var winInfoBB = InitLuckyFive(winInfo);
    //     if(winInfoBB != null)
    //     {
    //         // Add Card.
    //         SetMetaGameSpinInterrupt();
    //         BlackboardUtils.SetOrCreateValue<bool>(winInfoBB, "readyDeck", winInfo.cardList.Count == LUCKY_FIVE_DECK_COUNT);
    //     }
    // }

    public static List<int> GetLuckyFiveDeck()
    {
        // var winInfoBB = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), LUCKY_FIVE_INFO);
        var winInfoBB = GetMetaGameEnterInfo();

        if(winInfoBB != null)
        {
            var cardList = winInfoBB.GetValue<List<int>>("cardList");

            while(cardList.Count < LUCKY_FIVE_DECK_COUNT)
                cardList.Insert(0, -1);

            return cardList;
        }

        return new List<int>() {-1, -1, -1, -1, -1};
    }

    public static List<bool> GetLuckyFiveHitList()
    {
        // var winInfoBB = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), LUCKY_FIVE_INFO);
        var winInfoBB = GetMetaGameEnterInfo();

        if(winInfoBB != null)
        {
            var hitList = winInfoBB.GetValue<List<bool>>("hitList");

            while(hitList.Count < LUCKY_FIVE_DECK_COUNT)
                hitList.Insert(0, false);

            return hitList;
        }

        return new List<bool>() {false, false, false, false, false};
    }

    public static void LuckyFiveAddCardComplete()
    {
        ClearMetaGameSpinInterrupt();
    }

    public static LuckyFiveRule GetLuckyFiveRule()
    {
        // var winInfoBB = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), LUCKY_FIVE_INFO);
        var winInfoBB = GetMetaGameEnterInfo();

        if(winInfoBB != null)
        {
            var rule = winInfoBB.GetValue<LuckyFiveRule>("rule");

            if(rule != LuckyFiveRule.UNKNOWN)
                return rule;
        }

        return LuckyFiveRule.HIGH_CARD;
    }

    public static long GetLuckyFiveAverageBet()
    {
        // var winInfoBB = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), LUCKY_FIVE_INFO);
        var winInfoBB = GetMetaGameEnterInfo();

        if(winInfoBB != null)
            return winInfoBB.GetValue<long>("averageBet");

        return 0L;
    }

    public static long GetLuckyFiveEarnCoins()
    {
        // var winInfoBB = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), LUCKY_FIVE_INFO);
        var winInfoBB = GetMetaGameEnterInfo();

        if(winInfoBB != null)
            return winInfoBB.GetValue<long>("earnCredit");
        
        return 0L;
    }

    public static bool IsLuckyFiveClaimable()
    {
        // var winInfoBB = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), LUCKY_FIVE_INFO);
        var winInfoBB = GetMetaGameEnterInfo();

        if(winInfoBB != null)
            return winInfoBB.GetValue<bool>("isClaimable");

        return false;
    }

    public static bool ExistLuckyFiveRule()
    {
        var deckList = GetLuckyFiveDeck();
        if(deckList[0] == -1) return false;

        return GetLuckyFiveRule() != LuckyFiveRule.HIGH_CARD;
    }

    public static bool LuckyFiveReadyDeck()
    {
        var deckList = GetLuckyFiveDeck();
        if(deckList[0] == -1) return false;

        return true;
    }

    public static void LuckyFiveClaim()
    {
        // var winInfoBB = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), LUCKY_FIVE_INFO);
        var winInfoBB = GetMetaGameEnterInfo();

        if(winInfoBB != null)
            winInfoBB.SetValue("isClaimable", false);
    }
}

}

