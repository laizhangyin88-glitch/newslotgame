using BagelCode.Tasks.Actions.ClientAPI;
using RPCLogin;
using SlotMaker;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


/// <summary>
/// Converts a IDictionary&lt;string,object> / IList&lt;object> object into a JSON string
/// </summary>
/// <param name="json">A IDictionary&lt;string,object> / IList&lt;object></param>
/// <param name="jsonSerializerStrategy">Serializer strategy to use</param>
/// <returns>A JSON encoded string, or null if object 'json' is not serializable</returns>
///


public class RPCName
{
    /// <summary>登录</summary>
    public const string login = "login";
    /// <summary>进入大厅</summary>
    public const string lobby = "lobby";
    /// <summary>进入子游戏</summary>
    public const string enterGame = "enter_game";
    /// <summary>房间 玩家数据</summary>
    public const string metaInfo = "meta_info";
    /// <summary>keno 开玩</summary>
    public const string kenoSpin = "keno_spin";
    /// <summary>jacks 开牌</summary>
    public const string jacksDeal = "jacks_deal";
    /// <summary>jacks 换牌</summary>
    public const string jacksDraw = "jacks_draw";
    /// <summary>jacks </summary>
    public const string jacksGambleStart = "gamble_start"; // /v2/gamble/start
    /// <summary>jacks </summary>
    public const string jacksGambleDeal = "gamble_deal"; // /v2/gamble/deal
    /// <summary>jacks </summary>
    public const string jacksGambleTake = "gamble_take"; // /v2/gamble/take
    /// <summary>拉霸机 开玩</summary>
    public const string slotSpin = "slot_spin";
    /// <summary>顶号</summary>
    public const string serverClose = "server_close";
    /// <summary>心跳</summary>
    public const string ping = "ping";
    /// <summary>上分</summary>
    public const string agentRechargeToDeviceUser = "agent_recharge_to_device_user";
    /// <summary>下分</summary>
    public const string decreaseDeviceCredit = "decrease_device_credit";

}


namespace RPCBase
{
    [System.Serializable]
    public class ReqBase{
        public string protocol_key;
        public Dictionary<string, object> data;
    }
    [System.Serializable]
    public class ResBase
    {
        public string protocol_key;
        public Dictionary<string, object> data;
    }
}


namespace RPCLogin
{
    [System.Serializable]
    public struct ResLogin
    {
        public int err;
        public string msg;
        public long balance;
        public List<_GameInfoList> gameInfoList;
    }

    [System.Serializable]
    public struct _GameInfoList
    {
        public string shortImageUrl;
        public string longImageUrl;
        public long gameId;
        public int gameFilter;
        public string gameTitle;
    }
}

namespace RPCHall
{
    [System.Serializable]
    public struct ReqEnterGame
    {
        /// <summary>游戏id</summary>
        public int game_id;
    }

}

namespace RPCKenoClassic
{

    [System.Serializable]
    public struct ResEnterGame
    {
        /// <summary>游戏id</summary>
        public int game_id;
    }

    public class RPCReqKenoSpin
    {
        public static readonly string protocol_key = "keno_spin";
        public ReqKenoSpin data;
    }


    /// <summary>
    /// ReqKenoSpin 
    /// </summary>
    [System.Serializable]
    public class ReqKenoSpin
    {
        /// <summary>下注金额</summary>
        public uint betPerTicket;
        /// <summary>下注号码</summary>
        public List<List<int>> pickInfoList;
    }


    /*
    [System.Serializable]
    public partial class KenoPlayRequest
    {
        public uint blockseq = 0;
        public int ackMask = 0;
        public string contents = "";
        public int gameId = 0;
        public bool isGamePlay = false;
        public bool isAutoPlay = false;
        public bool isBonusPlay = false;
        public int metaGameEventId = 0;
        public int collectingGameChestDropRateMultiplyEventId = 0;
        public bool isAutoChange = false;
        public List<int> expEventIdList = new List<int>();
        public bool isHighRollerBet = false;
        public int seasonPassEventId = 0;
    }*/


    [System.Serializable]
    public struct ResKenoSpin
    {
        public int err;
        public string token_id;
    }
}

public struct AccountLoginRespone
{
    public int err;
    public string token_id;
    public string logic_ip;
}
