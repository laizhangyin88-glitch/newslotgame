using System.Collections.Generic;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode
{
    public static partial class BlackboardQueryUtils
    {
        private static readonly string GAME_SPIN_LIST = "totalGameSpinCountList";
        private static readonly string GAME_SPIN_INFO = "GameSpinInfo";
        private static readonly string GAME_ID = "gameId";
        private static readonly string TOTAL_GAME_SPIN_COUNT = "totalSpinCountOfGame";
        private static readonly string ENABLE_GAME_SPIN = "values/misc/ENABLE_GAME_SPIN";

        public static bool GetGameSpinEnabled()
        {
            return BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), ENABLE_GAME_SPIN).value;
        }

        public static List<Blackboard> GetGameSpinList()
        {
            return MainBlackboard.Get().GetValue<List<Blackboard>>(GAME_SPIN_LIST);
        }

        public static int GetGameSpinTotalCount(int gameId)
        {
            return GetGameSpinTotalCount(gameId, GetGameSpinList());
        }

        public static void UpdateGameSpinCount(int gameId, int spinCount)
        {
            UpdateGameSpinCount(gameId, spinCount, true, GetGameSpinList());
        }

        public static void AddGameSpinCount(int gameId, int spinCount)
        {
            UpdateGameSpinCount(gameId, spinCount, false, GetGameSpinList());
        }

        private static int GetGameSpinTotalCount(int gameId, List<Blackboard> gameSpinList)
        {
            int totalSpinCount = 0;

            for (int i = 0; i < gameSpinList.Count; ++i)
            {
                var gameSpin = gameSpinList[i];
                if (gameSpin.GetValue<int>(GAME_ID) == gameId)
                {
                    totalSpinCount += gameSpin.GetValue<int>(TOTAL_GAME_SPIN_COUNT);
                }
            }

            return totalSpinCount;
        }

        private static void UpdateGameSpinCount(int gameId, int spinCount, bool reset, List<Blackboard> gameSpinList)
        {
            for (int i = 0; i < gameSpinList.Count; ++i)
            {
                var gameSpin = gameSpinList[i];
                if (gameSpin.GetValue<int>(GAME_ID) == gameId)
                {
                    var spinCountVariable = gameSpin.GetVariable<int>(TOTAL_GAME_SPIN_COUNT);
                    spinCountVariable.value = reset ? spinCount : (spinCountVariable.value + spinCount);
                    return;
                }
            }

            var newBB = BlackboardUtils.CreateBlackboard(GAME_SPIN_INFO);
            newBB.AddVariable(GAME_ID, gameId);
            newBB.AddVariable(TOTAL_GAME_SPIN_COUNT, spinCount);
            BlackboardUtils.AddToBlackboardList(MainBlackboard.Get(), GAME_SPIN_LIST, newBB);
        }
    }
}
