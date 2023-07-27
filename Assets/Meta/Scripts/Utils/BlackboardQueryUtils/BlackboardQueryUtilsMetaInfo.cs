using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;

namespace BagelCode
{

    public static partial class BlackboardQueryUtils
    {
        public static void UpdateMysteryGiftInfo(int nextMysteryGiftLevel, MysteryGiftInfo mysteryGiftInfo, long serverTimestamp)
        {
            if(mysteryGiftInfo != null)
            {
                var mysteryGiftInfoBB = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "mysteryGiftInfo");
                ClientAPI2Blackboard.Serialize(mysteryGiftInfoBB, mysteryGiftInfo);
            }

            var mysterGiftLevel = BlackboardUtils.GetOrCreateVariable<int>(MainBlackboard.Get(), "nextMysteryGiftLevel");
            if(mysterGiftLevel.value < nextMysteryGiftLevel)
            {
                mysterGiftLevel.value = nextMysteryGiftLevel;
            }
        }

        public static Blackboard GetMysteryGiftInfo()
        {
            var mysteryGiftInfoBB = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "mysteryGiftInfo");
            return mysteryGiftInfoBB?.value ?? null;
        }

        public static void ClearMysteryGiftInfo()
        {
            BlackboardUtils.DestroyBlackboard(MainBlackboard.Get(), "mysteryGiftInfo");
        }
    }

}

