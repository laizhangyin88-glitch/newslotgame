using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;
using UnityEngine;

namespace BagelCode.Chat
{
    /// <summary>
    /// Chat Strategy Pattern
    /// </summary>
    public interface IChatStrategy 
    {
        bool IsVisible(ChatPoll chatPoll);
        bool IsVisible(ChatMessageData chatData);// Chatting Text Balloon Visible?
        bool IsInterest(ChatPoll chatPoll);
        bool IsInterest(ChatMessageData chatData);// BageCount Addiable?
        IComparer<ChatMessageData> GetChatComparer();// Chatting Text Balloon Order
    }
}
