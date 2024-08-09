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
    /// <summary>更新玩家缓存</summary>
    public const string updateUserCache = "update_user_cache";

    /// <summary>HAPPY DOLLARS</summary>
    public const string claimBonus = "claim_bonus";


    /// <summary>免费游戏历史记录</summary>
    public const string freeSpinHistory = "slot_spin_history";

    /// <summary>顶号</summary>
    public const string serverClose = "server_close";
    /// <summary>心跳</summary>
    public const string ping = "ping";

    /// <summary>上分</summary><remarks>实体机专用协议，Editor环境下会报err:12</remarks>
    /// {"balance", 1000}, //上分1000
    public const string addCredit = "agent_incredit_exchange";
    /// <summary>下分</summary>
    public const string decreaseCredit = "agent_outcredit_exchange";


    /// <summary>查询是否可以进币</summary>
    public const string creatAddCoinOrder = "agent_create_incredit_coin_order";

    /// <summary>投币</summary>
    /// {"device_id":100,"count":100 }
    public const string confirmAddCoinOrder = "agent_confirm_incredit_coin_order";


    /// <summary>查询是否可以充美刀</summary>
    public const string creatAddMoneyOrder = "agent_create_incredit_inbanknote_order";
    /// <summary>充美元</summary>
    /// {"money", 100}, //充100美元
    public const string confirmAddMoneyOrder = "agent_confirm_incredit_inbanknote_order";


    /// <summary>查询是否可以退币</summary>
    public const string createCoinOutOrder = "agent_create_outcredit_ticket_order";
    /// <summary>退币</summary>
    /// {"money", 100}, //退票 100 
    public const string confirmCoinOutOrder = "agent_confirm_outcredit_ticket_order";


    /// <summary>创建“打印订单”</summary>
    /// {"money", 100}, 
    public const string createPrintOrder = "agent_create_outcredit_print_order";
    /// <summary>确认“打印订单”</summary>
    /// {"money", 100},
    public const string confirmPrintOrder = "agent_confirm_outcredit_print_order";

    /// <summary> 赢得大厅彩金 </summary>
    public const string winGameBonus = "win_game_bonus";

    /// <summary> 押注彩金返回 </summary>
    public const string gameBonusResult = "game_bonus_result";

    /// <summary> 更改名称 </summary>
    public const string resetNickName = "reset_nick_name";
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
