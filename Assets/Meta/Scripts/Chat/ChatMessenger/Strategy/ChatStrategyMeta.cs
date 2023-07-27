using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;
using UnityEngine;

namespace BagelCode.Chat
{
    public class ChatStrategyMeta : IChatStrategy
    {
        private ChatMessenger owner => ChatMessenger.Instance;
        private ChatPollComp chatPollComp = new ChatPollComp();

        public class ChatPollComp : IComparer<ChatMessageData>
        {
            public int Compare(ChatMessageData chatA, ChatMessageData chatB)
            {
                // Sucess or Wait
                if(chatA.isResponse && chatB.isResponse)
                {
                    if(chatA.isSuccess && !chatB.isSuccess) return -1;
                    if(!chatA.isSuccess && chatB.isSuccess) return 1;
                }
                else if(chatA.isResponse && !chatB.isResponse)
                {
                    if(!chatA.isSuccess) return 1;
                }
                else if(!chatA.isResponse && chatB.isResponse)
                {
                    if(!chatB.isSuccess) return -1;
                }

                // if((chatA.isResponse == chatA.isSuccess) && (chatB.isResponse == chatB.isSuccess))
                // else if((chatA.isResponse == chatA.isSuccess) && (chatB.isResponse == chatB.isSuccess))
                // {
                //     if(chatA.chatPoll.id < chatB.chatPoll.id) return -1;
                //     else if(chatA.chatPoll.id > chatB.chatPoll.id) return 1;
                // }
                // else if((chatA.isResponse == !chatA.isSuccess) && (chatB.isResponse == !chatB.isSuccess))
                // {
                //     // Failed vs Failed
                //     if(chatA.chatPoll.id < chatB.chatPoll.id) return -1;
                //     else if(chatA.chatPoll.id > chatB.chatPoll.id) return 1;
                // }
                // else if(chatA.isSuccess && !chatB.isSuccess) return -1;
                // else if(!chatA.isSuccess && chatB.isSuccess) return 1;

                // else if (chatA.isResponse && chatB.isResponse)
                // {
                //     if (chatA.isSuccess && !chatB.isSuccess) return -1;
                //     if (!chatA.isSuccess && chatB.isSuccess) return 1;
                // }

                // if(chatA.isResponse != chatB.isResponse)
                // {
                //     if( (chatA.isResponse && !chatA.isSuccess) && !chatB.isResponse ) return 1;
                //     if( (chatB.isResponse && !chatB.isSuccess) && !chatA.isResponse ) return -1;
                // }
                if (chatA.chatPoll.id == chatB.chatPoll.id) return 0;

                if (chatA.chatPoll.id > chatB.chatPoll.id)
                    return 1;
                else
                    return -1;
            }
        }

        public IComparer<ChatMessageData> GetChatComparer()
        {
            return chatPollComp;
        }

        public bool IsVisible(ChatPoll chatPoll)
        {
            if (chatPoll.type == ChatType.UNKNOWN) return false;
            if (owner.IsReported(chatPoll.id)) return false;
            if (owner.IsMute(chatPoll.userId)) return false;
            // if (chatPoll.type == ChatType.COLLECTING_GAME_REQUEST)
            // {
            //     if (MetaGameUtils.IsPlayingMetaGame() && MetaGameUtils.IsPlayingCollectingGame())
            //     {
            //         var chatEventId = (chatPoll.data as ChatDataCollectingGameRequest).metaGameEventId;
            //         return chatEventId == MetaGameUtils.GetEventID();
            //     }
            //     else
            //     {
            //         return false;
            //     }
            // }
            
            return true;
        }

        public bool IsVisible(ChatMessageData chatData)
        {
            return IsVisible(chatData.chatPoll);
        }
        
        public bool IsInterest(ChatPoll chatPoll)
        {
            if (!IsVisible(chatPoll)) return false;
            if (chatPoll.userId == BlackboardQueryUtils.GetMyUserId()) return false;
            return true;
        }

        public bool IsInterest(ChatMessageData chatData)
        {
            return IsInterest(chatData.chatPoll);
        }
    }
}
