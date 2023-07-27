using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;
using UnityEngine;
using SlotMaker.Json;
using SlotMaker;
using System.Linq;

namespace BagelCode.Chat {
    /// <summary>
    /// [ChatPoll Id]
    /// >Negative: Requesting Messages
    /// >1~100: System Messages
    /// >10000~: Normal Messages
    /// </summary>
    public static class ChatPollExtension {
        public static ChatType GetChatType(this ChatPoll chatPoll) {
            if (chatPoll.data == null) return ChatType.UNKNOWN;

            if (chatPoll.data is ChatDataMessage) return ChatType.MESSAGE;
            else if (chatPoll.data is ChatDataInstant) return ChatType.INSTANT;
            else if (chatPoll.data is ChatDataEmoji) return ChatType.EMOJI;
            else if (chatPoll.data is ChatDataClubPr) return ChatType.CLUB_PR;
            else if (chatPoll.data is ChatDataBossRaidersBossKill) return ChatType.BOSS_RAIDERS_BOSS_KILL;
            else if (chatPoll.data is ChatDataBossRaidersRankingUp) return ChatType.BOSS_RAIDERS_RANKING_UP;
            else if (chatPoll.data is ChatDataBossRaidersLeadingAttacker) return ChatType.BOSS_RAIDERS_LEADING_ATTACKER;
            else if (chatPoll.data is ChatDataClubArenaClubRankingUp) return ChatType.CLUB_ARENA_CLUB_RANKING_UP;
            else if (chatPoll.data is ChatDataClubArenaLeadingClubMember) return ChatType.CLUB_ARENA_LEADING_CLUB_MEMBER;
            // else if (chatPoll.data is ChatDataCollectingGameRequest) return ChatType.COLLECTING_GAME_REQUEST;

            return ChatType.UNKNOWN;
        }

        public static bool IsGlobalTopbarType(this ChatPoll chatPoll)
        {
            switch(chatPoll.type)
            {
                case ChatType.MESSAGE:
                case ChatType.INSTANT:
                    return true;
            }

            return false;
        }

        public static void ApplyData(this ChatPoll chatPoll,string message)
        {
            var data = new ChatDataMessage();
            data.message = message;
            chatPoll.type = ChatType.MESSAGE;
            chatPoll.data = data;
        }

        public static void ApplyData(this ChatPoll chatPoll, EmojiChat emojiChat)
        {
            var data = new ChatDataEmoji();
            data.emoji = emojiChat;
            chatPoll.type = ChatType.EMOJI;
            chatPoll.data = data;
        }

        public static void ApplyData(this ChatPoll chatPoll, InstantChat instantChat)
        {
            var data = new ChatDataInstant();
            data.instant = instantChat;
            chatPoll.type = ChatType.INSTANT;
            chatPoll.data = data;
        }

        public static void ApplyData(this ChatPoll chatPoll, long clubId,string clubName,string clubSymbol)
        {
            var data = new ChatDataClubPr();
            data.clubId = clubId;
            data.clubName=clubName;
            data.clubSymbol = clubSymbol;
            chatPoll.type = ChatType.CLUB_PR;
            chatPoll.data = data;
        }

        // public static void ApplyData(this ChatPoll chatPoll, int requirement,int pieceId,int possessions) {
        //     var data = new ChatDataCollectingGameRequest();
        //     data.requirement = requirement;
        //     data.pieceId = pieceId;
        //     data.possessions = possessions;
        //     chatPoll.type = ChatType.COLLECTING_GAME_REQUEST;
        //     chatPoll.data = data;
        // }

        public static string ConvertString(this InstantChat instantChat) {
            string key = instantChat.ToString().Replace("INSTANT", "CONTENT");
            if (string.IsNullOrEmpty(key)) {
                return "";
            }
            return StringTableUtils.GetString(StringTable.StringTableType.Global, key);
        }
        public static string GetString(this ChatPoll chatPoll)
        {
            if (chatPoll.type == ChatType.MESSAGE)
            {
                var data = (chatPoll.data as ChatDataMessage);
                return data.message;
            }
            else if (chatPoll.type == ChatType.INSTANT)
            {
                var data = (chatPoll.data as ChatDataInstant);
                return data.instant.ConvertString();
            }

            return "";
        }

        public static long GetMinID(this List<ChatMessageData> chatDataList)
        {
            if (chatDataList.Count == 0)
                return -1;

            return chatDataList.Where(x => x.chatPoll.id > 0 && x.isResponse && x.isSuccess).Select(x => x.chatPoll.id).Min();
        }
    }
}