using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using UnityEngine;
using BagelCode.ClientModels;

namespace BagelCode.Chat
{
    public class ChatGlobalTopbar : MonoBehaviour, IChatMetaListener
    {
        private ContextElement mContext => GetComponent<ContextElement>();
        private ContextElement contentTextContext;

        private const char ellipsis = (char)8230;
        private const int maxLength = 60;

        private long lastMessageID = -1;

        private void Start()
        {
            mContext.UpdateContext();

            contentTextContext  = ContextUtils.FindElement(mContext, "Chatting Text", ContextSearchingType.FullNameSearch);

            ChatMetaManager.Instance.AddListener(this);

            MetaContextElementUtils.SetText(contentTextContext, "-");

            UpdateText();
        }

        void OnDestroy()
        {
            if (ChatMetaManager.Instance != null)
                ChatMetaManager.Instance.RemoveListener(this);
        }

        private void UpdateText()
        {
            var globalChannelID = ChatMetaManager.Instance.GetChannelID(ChannelType.Global);
            var lastChatID = ChatMessenger.Instance.GetGlobalTopbarChatID(globalChannelID);
            if (lastChatID == -1) return;

            var chatData = ChatMessenger.Instance.FindChatData(lastChatID);
            if (chatData == null) return;

            var chatType = chatData.chatPoll.GetChatType();
            string message = "";

            if(lastMessageID >= lastChatID)
                return;
                
            lastMessageID = lastChatID;

            if (chatType == ChatType.MESSAGE)
            {
                var data = chatData.chatPoll.data as ChatDataMessage;
                message = data.message;
            }
            else if (chatType == ChatType.INSTANT)
            {
                var data = chatData.chatPoll.data as ChatDataInstant;
                message = data.instant.ConvertString();
            }
            else
            {
                return;
            }

            // elipsis message
            if(message.Length > maxLength)
            {
                message = message.Substring(0, maxLength);
                message += ellipsis;
            }

            string userName = chatData.chatPoll.profile == null ? "UNKNOWN" : chatData.chatPoll.profile.name;

            MetaContextElementUtils.SetText(contentTextContext, StringTableUtils.GetString(StringTable.StringTableType.Global, "CHAT_GLOBAL_TOPBAR_TEXT", userName, StringTable.BadWordFilter(message)));
        }

        

        public void OnAddChatMessages(ChannelType channelType, string channelId,List<ChatMessageData> chatDataList)
        {
            if (channelType == ChannelType.Global)
            {
                UpdateText();
            }
        }

        public void OnAddRecentChatMessages(ChannelType channelType, string channelId,List<ChatMessageData> chatDataList)
        {
        }

        public void OnRemoveRequestChatMessages(ChannelType channelType, string channelId,List<ChatMessageData> chatDataList)
        {
        }


        public void OnGlobalChannelChanged(string preGlobalId, string curGlobalId) {}
        public void OnClubLeaved(string preClubChannelId) {}
    }
}
