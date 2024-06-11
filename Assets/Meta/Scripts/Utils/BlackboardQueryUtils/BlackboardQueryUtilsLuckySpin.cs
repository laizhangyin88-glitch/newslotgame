using BagelCode.ClientModels;
using SlotMaker;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public static partial class BlackboardQueryUtils
    {
        public static void CreateLuckySpinReelStripsBB()
        {
            // Lucky Spin Reel Setting.
            List<int> gameIdReel = BlackboardUtils.GetOrCreateVariable<List<int>>(MainBlackboard.Get(), "gameSpinReel/gameIdReel").value;
            List<int> spinCountReel = BlackboardUtils.GetOrCreateVariable<List<int>>(MainBlackboard.Get(), "gameSpinReel/spinCountReel").value;
            List<int> betScaleReel = BlackboardUtils.GetOrCreateVariable<List<int>>(MainBlackboard.Get(), "gameSpinReel/betScaleReel").value;
            List<int> freebieBetScaleReel = CalculateFreebieLuckySpinMultiplierBet(betScaleReel);

            var gameIDReelBB = BlackboardUtils.CreateBlackboard("gameIdReel");
            var spinCountReelBB = BlackboardUtils.CreateBlackboard("spinCountReel");
            var betScaleReelBB = BlackboardUtils.CreateBlackboard("betScaleReel");

            // Convert Blackboard Info
            BlackboardUtils.SetOrCreateValue<List<int>>(gameIDReelBB, "value", gameIdReel);
            BlackboardUtils.SetOrCreateValue<List<int>>(spinCountReelBB, "value", spinCountReel);
            BlackboardUtils.SetOrCreateValue<List<int>>(betScaleReelBB, "value", freebieBetScaleReel);
            Debug.LogError("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@!!!!!!!!!!!!!!!");
            // Convert Blackboard Info
            var reelSequenceListBB = BlackboardUtils.CreateBlackboard("reelSequenceList");
            BlackboardUtils.GetOrCreateBlackboardList(reelSequenceListBB, "reelSequenceList");
            BlackboardUtils.AddToBlackboardList(reelSequenceListBB, "reelSequenceList", gameIDReelBB);
            BlackboardUtils.AddToBlackboardList(reelSequenceListBB, "reelSequenceList", spinCountReelBB);
            BlackboardUtils.AddToBlackboardList(reelSequenceListBB, "reelSequenceList", betScaleReelBB);

            // Contents Blackboard Setting.
            var contentsBB = BlackboardUtils.GetOrCreateBlackboard(ContentBlackboard.Get(), "game");

            BlackboardUtils.SetOrCreateValue<GameType>(contentsBB, "gameType", GameType.SLOT_MACHINE);
            BlackboardUtils.GetOrCreateBlackboardList(contentsBB, "reelSetList");

            BlackboardUtils.AddToBlackboardList(contentsBB, "reelSetList", reelSequenceListBB);
        }

        public static int GetHighlightSpinCountThreshold()
        {
            return BlackboardUtils.GetOrCreateVariable<int>(MainBlackboard.Get(), "values/gameSpin/highlightSpinCountThreshold").value;
        }

        private static List<int> CalculateFreebieLuckySpinMultiplierBet(List<int> baseBetList)
        {
            List<int> freebieBetList = new List<int>();
            if (baseBetList != null && baseBetList.Count > 0)
            {
                for (int i = 0; i < baseBetList.Count; ++i)
                {
                    long betNumeratorValue = FreebieLevelUtils.GetLevelMultiplierNumeratorValue(baseBetList[i], FreebieLevelUtils.FreebieType.LUCKY_SPIN);
                    freebieBetList.Add((int)betNumeratorValue);
                }
            }
            return freebieBetList;
        }
    }
}
