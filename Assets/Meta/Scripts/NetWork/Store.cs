using SimpleJSON;

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

    //public static int test_is_free_spin = 0;

   // public static int[] test_spin_tab = new int[] { };
    //public static string lastFreeSpinContents = "";
}
