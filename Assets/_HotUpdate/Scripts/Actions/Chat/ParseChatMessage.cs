using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Task.Actions
{

[Category("★ BagelCode/Poll")]
public class ParseChatMessage : ActionTask<Blackboard>
{
    public BBParameter<Blackboard> bb;

    private const string constType = "type";
    private const string constMessage = "message";

    private static string chatType = "";
    private static string chatMessage = "";

    protected override string info
    {
        get { return string.Format("Parse Chat message"); }
    }

    protected override void OnExecute()
    {
        // var chatListBB = BlackboardUtils.GetOrCreateBlackboardList(MainBlackboard.Get(), "chatInfo");

        // var polltype = bb.value.GetVariable<PollType>("__event__");

        // bool isAddChatAlarm = false;
        // chatType = "";

        // if (polltype != null) // from polltype
        // {
        //     bool error = false;
        //     switch(polltype.value)
        //     {
        //         case PollType.BONUS_TRIGGER:
        //             // string bonusUserName = bb.value.GetValue<string>("userName");
        //             // int bonusId   = bb.value.GetValue<int>("bonusId");
        //             // string userId = bb.value.GetValue<string>("userId");
        //             break;
        //         case PollType.WIN:
        //             WinType winType = bb.value.GetValue<WinType>("winType");
        //             long winCredit  = bb.value.GetValue<long>("winCredit");
        //             string winName     = bb.value.GetValue<string>("name");

        //             // chatType = "win";
        //             // if (winType == WinType.BIG)
        //             // {
        //             //     chatMessage = StringTableUtils.GetString(StringTable.StringTableType.Global, "CONTENT_CHAT_BIGWIN", out error, winName, winCredit);
        //             // }
        //             // else if (winType == WinType.SUPER_BIG)
        //             // {
        //             //     chatMessage = StringTableUtils.GetString(StringTable.StringTableType.Global, "CONTENT_CHAT_SUPERBIGWIN", out error, winName, winCredit);
        //             // }
        //             // else if (winType == WinType.MEGA)
        //             // {
        //             //     chatMessage = StringTableUtils.GetString(StringTable.StringTableType.Global, "CONTENT_CHAT_MEGAWIN", out error, winName, winCredit);
        //             // }
        //             // else if (winType == WinType.SUPER_MEGA)
        //             // {
        //             //     chatMessage = StringTableUtils.GetString(StringTable.StringTableType.Global, "CONTENT_CHAT_SUPERMEGAWIN", out error, winName, winCredit);
        //             // }
        //             // else
        //             if (winType == WinType.EPIC)
        //             {
        //                 chatType = "win";
        //                 chatMessage = StringTableUtils.GetString(StringTable.StringTableType.Global, "CONTENT_CHAT_EPICWIN", out error, winName, winCredit);
        //             }

        //             break;
        //         case PollType.JACKPOT_WIN:
        //             // string winnerName    = bb.value.GetValue<string>("name");
        //             // long jacpotCredit    = bb.value.GetValue<long>("winCredit");
        //             // JackpotKudoType jackpotType = bb.value.GetValue<JackpotKudoType>("jackpotKudoType");

        //             // chatType = "system";
        //             // if (jackpotType == JackpotKudoType.MINI)
        //             // {
        //             //     chatMessage = StringTableUtils.GetString(StringTable.StringTableType.Global, "CONTENT_CHAT_MINIJACKPOT", out error, winnerName, jacpotCredit);
        //             // }
        //             // else if (jackpotType == JackpotKudoType.MINOR)
        //             // {
        //             //     chatMessage = StringTableUtils.GetString(StringTable.StringTableType.Global, "CONTENT_CHAT_MINORJACKPOT", out error, winnerName, jacpotCredit);
        //             // }
        //             // else if (jackpotType == JackpotKudoType.MAJOR)
        //             // {
        //             //     chatMessage = StringTableUtils.GetString(StringTable.StringTableType.Global, "CONTENT_CHAT_MAJORJACKPOT", out error, winnerName, jacpotCredit);
        //             // }
        //             // else if (jackpotType == JackpotKudoType.MEGA)
        //             // {
        //             //     chatMessage = StringTableUtils.GetString(StringTable.StringTableType.Global, "CONTENT_CHAT_MEGAJACKPOT", out error, winnerName, jacpotCredit);
        //             // }
        //             // else if (jackpotType == JackpotKudoType.GRAND)
        //             // {
        //             //     chatMessage = StringTableUtils.GetString(StringTable.StringTableType.Global, "CONTENT_CHAT_GRANDJACKPOT", out error, winnerName, jacpotCredit);
        //             // }

        //             break;
        //         case PollType.FRIEND_CONNECT:
        //             // string friendName = bb.value.GetValue<string>("name");
                    
        //             // chatType = "system";
        //             // chatMessage = StringTableUtils.GetString(StringTable.StringTableType.Global, "CONTENT_CHAT_FRIEND_LOGIN", out error, friendName);
                    
        //             break;
        //         case PollType.NOTICE:
        //             string notice = bb.value.GetValue<string>("message");
        //             chatType = "notice";
                    
        //             chatMessage = StringTableUtils.GetString(StringTable.StringTableType.Global, "CONTENT_CAHT_NOTICE_NORMAL", out error, notice);
        //             break;
        //     }
        // }
        // else
        // {
        //     chatType = bb.value.GetValue<string>("type");
        //     bool error = true;
        //     Debug.LogError(chatType);
        //     if (chatType == "myChat")
        //     {
        //         chatType = "meChat";
        //         string meName    = BlackboardUtils.FindValue(MainBlackboard.Get(), "/me/name") as string;
        //         string chat = bb.value.GetValue<string>("message");

        //         chatMessage = StringTableUtils.GetString(StringTable.StringTableType.Global, "CONTENT_CHAT_ME_MESSAGE", out error, meName, chat);
        //     }
        //     else if (chatType == "enterRoom_Other")
        //     {
        //         chatType = "system";
        //         string name  = bb.value.GetValue<string>("name");
        //         chatMessage = StringTableUtils.GetString(StringTable.StringTableType.Global, "CONTENT_CHAT_USER_JOIN", out error, name);
        //     }
        //     else if (chatType == "enterRoom_Friend")
        //     {
        //         chatType = "system";
        //         string name  = bb.value.GetValue<string>("name");
        //         chatMessage = StringTableUtils.GetString(StringTable.StringTableType.Global, "CONTENT_CHAT_FRIEND_JOIN", out error, name);
        //     }
        //     else if (chatType == "enterRoom_Me")
        //     {
        //         chatType = "system";
        //         // SlotBetType betTag = bb.value.GetValue<SlotBetType>("betTag");
        //         int gameId = bb.value.GetValue<int>("gameId");
        //         chatMessage = StringTableUtils.GetString(StringTable.StringTableType.Global, "CONTENT_CHAT_ME_JOIN_FREEBET", out error, gameId);
                
        //     }

        // }

        // if(!string.IsNullOrEmpty(chatType))
        // {
        //     IBlackboard chatBB = BlackboardUtils.CreateBlackboard("chatBB");       

        //     chatBB.AddVariable(constMessage, chatMessage);
        //     chatBB.AddVariable(constType, chatType);

        //     BlackboardQueryUtils.UpdateChat(chatBB, isAddChatAlarm);
        // }
        
        EndAction();
    }
}

}
