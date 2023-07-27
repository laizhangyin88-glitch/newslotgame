using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using SlotMaker;
using SlotMaker.Json;
using BagelCode.Chat;

namespace BagelCode
{
    public static partial class BiEventUtils
    {
        public static string chatEnterContextID = null;

        public static void ClickChatButton(string place)
        {
            chatEnterContextID = GenerateContextID();

            Dictionary<string, object> customData = new Dictionary<string, object>();

            customData["type"] = place;

            Analytics.CustomEvent("client_click_chat", customData);
        }

        public static void ChatEnter(string chatType, string language, string channelID)
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();

            customData["chat_type"] = chatType;
            customData["language"] = language;
            customData["chat_room_id"] = channelID;
            customData["remain_speaker_count"] =  BlackboardQueryUtils.GetGlobalChatSpeakerCount();
            customData["target_user_id"] = null;
            customData["context_id"] = chatEnterContextID;

            Analytics.CustomEvent("client_chat_enter", customData);
        }

        public static void SendChat(string chatType,
                                    string language,
                                    string channelID,
                                    ChatMessageData chatData,
                                    bool isSpeakerUsed,
                                    bool isSuccess
                                    )
        {
            // ContextID is Unique ID. Generate from chatdata class.

            Dictionary<string, object> customData = new Dictionary<string, object>();

            string messageJson = BlackboardJson.SerializeObject(chatData.chatPoll.data);

            customData["chat_type"] = chatType;
            customData["language"] = language;
            customData["chat_room_id"] = channelID;
            customData["remain_speaker_count"] = BlackboardQueryUtils.GetGlobalChatSpeakerCount();
            customData["is_speaker_used_chat"] = isSpeakerUsed;
            customData["message"] = messageJson;
            customData["target_user_id"] = null;
            customData["context_id"] = chatData.contextID;
            customData["chat_message_type"]= chatData.chatPoll.type.ToString().ToLower();
            customData["status"] = isSuccess ? "SUCCESS" : "FAIL";

            Analytics.CustomEvent("client_chat", customData);
        }

        public static void ClickReportInformation()
        {
            Analytics.CustomEvent("client_click_chat_report_information", new Dictionary<string, object>(){});
        }

        public static void SendSystemChat(string chatType, string language, string channelID, ChatMessageData chatData, string name, string message = null)
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();

            string messageJson = string.IsNullOrEmpty(message) ? BlackboardJson.SerializeObject(chatData.chatPoll.data) : message;

            customData["chat_type"] = chatType;
            customData["language"] = language;
            customData["chat_room_id"] = channelID;
            customData["remain_speaker_count"] = BlackboardQueryUtils.GetGlobalChatSpeakerCount();
            customData["is_speaker_used_chat"] = false;
            customData["message"] = messageJson;
            customData["context_id"] = chatData.contextID;
            customData["chat_message_type"] = chatData.chatPoll.type.ToString();
            customData["name"] = name;

            Analytics.CustomEvent("client_system_chat", customData);
        }
    }
}
