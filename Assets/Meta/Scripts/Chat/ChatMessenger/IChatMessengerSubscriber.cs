using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;

namespace BagelCode.Chat
{
    public interface IChatMessengerSubscriber
    {
        /// <summary>
        /// When channel is connected.
        /// </summary>
        void OnConnectedChannel(string channelID);
        /// <summary>
        /// When channel is disconnected.
        /// </summary>
        void OnDisconnectedChannel(string channelID);
        /// <summary>
        /// When messages is received. Not call in connected.
        /// </summary>
        void OnAddChatMessages(string channelID, List<ChatMessageData> chatDataList);
        void OnAddRecentChatMessages(string channelID, List<ChatMessageData> chatDataList);

        void OnRemoveRequestChatMessages(string channelID, List<ChatMessageData> chatDataList);
    }

}
