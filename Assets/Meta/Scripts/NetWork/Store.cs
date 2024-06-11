using SimpleJSON;
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

    private static readonly List<int> newGameId = new List<int>() { 3000, 3001 };

    public static bool IsNewGame(int gameId)
    {
        return newGameId.Contains(gameId);
    }

    private static readonly Dictionary<int, string> gameTitle = new Dictionary<int, string>()
    {
        [3000] = "bst",
        [3001] = "fruitparty",
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
}
