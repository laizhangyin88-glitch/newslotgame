using SimpleJSON;
using SlotMaker;
using System.Collections.Generic;

public class globalStore
{
    public static GameState gameState = GameState.None;

    public static string gToken = null;

    public static int nowGameID = -1;

    //public static List<object> gameInfoList;
    public static JSONNode gameInfoList;

    /// <summary>单局游戏结束</summary>
    public static bool isPlay = false;

    public static long newCredit = 0;

    private static readonly List<int> newGameId = new List<int>() { 3000, 3001, 3002, 3003 };

    public static bool IsNewGame(int gameId)
    {
        return newGameId.Contains(gameId);
    }
    /// <summary>
    /// 当前的游戏是不是在新添加的游戏中
    /// </summary>
    /// <returns></returns>
    public static bool IsInNewGame()
    {
        int gameId = BlackboardUtils.GetOrCreateVariable<int>(null, "./game/gameId").value;
        return IsNewGame(gameId);
    }

    private static readonly Dictionary<int, string> gameTitle = new Dictionary<int, string>()
    {
        [3000] = "bst",
        [3001] = "fruitparty",
        [3002] = "pyramid",
        [3003] = "ftv",
    };

    public static readonly Dictionary<int, int> bonusID = new Dictionary<int, int>()
    {
        [3001] = 300102,
    };

    public static string GetGameTitle(int gameId)
    {
        if (gameTitle.TryGetValue(gameId, out var title))
            return title;
        return string.Empty;
    }

    //public static int test_is_free_spin = 0;

    // public static int[] test_spin_tab = new int[] { };
    //public static string lastFreeSpinContents = "";

    /// <summary>
    /// 每个游戏对应的滚轮表的名字
    /// </summary>
    public static Dictionary<int, Dictionary<string, int>> ReelDictDict = new Dictionary<int, Dictionary<string, int>>()
    {
        {
            3001,
            new Dictionary<string, int>()
            {
                { "regular_game_reel", 0 },
                { "free_game_reel", 1 }
            }
        }
    };


    public static List<List<int>> reelSetList1 = new List<List<int>>();
    public static List<List<int>> reelSetList2 = new List<List<int>>();
}
