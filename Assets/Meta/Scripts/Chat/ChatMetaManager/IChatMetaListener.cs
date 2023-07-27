using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;
using UnityEngine;

namespace BagelCode.Chat
{
    public interface IChatMetaListener
    {
        void OnGlobalChannelChanged(string preGlobalID, string curGlobalID);

        void OnAddChatMessages(ChannelType channelType, string channelID, List<ChatMessageData> chatDataList);
        void OnAddRecentChatMessages(ChannelType channelType, string channelID,List<ChatMessageData> chatDataList);
        
        void OnRemoveRequestChatMessages(ChannelType channelType, string channelID,List<ChatMessageData> chatDataList);
        
        void OnClubLeaved(string preClubChannelID);
    }
}
