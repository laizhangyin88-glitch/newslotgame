using System.Text;
using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;
using UnityEngine;
using SlotMaker;

namespace BagelCode.Chat
{
    public class ChatMessageData
    {
        public bool isResponse = false;
        public bool isSuccess = false;

        public ChatPoll chatPoll;

        public TextBalloonBase ownerBalloon;
        public ChatMessengerChannel ownerChannel;

        public bool isSystem = false;

        public string assetName;
        public string contextID; // use to bi.

        public bool isVisible;

        public bool changeSize;

        public bool isMe;

        public string filterMessage;

        public void InitData(ChatPoll poll)
        {
            isResponse = false;
            isSuccess = false;
            assetName = null;
            chatPoll = poll;
            ownerBalloon = null;
            ownerChannel = null;

            isSystem = false;

            isMe = chatPoll.userId == BlackboardQueryUtils.GetMyUserId();

            contextID = BiEventUtils.GenerateContextID();

            changeSize = true;
        }

        public void SetSuccessData()
        {
            isResponse = true;
            isSuccess = true;
        }

        public void Post()
        {
            isResponse = false;
            isSuccess = false;
        }

        public void SendSuccess()
        {
            isResponse = true;
            isSuccess = true;
            // ownerChannel.OnRemapChatData(this);
        }

        public void SendFailed()
        {
            isResponse = true;
            isSuccess = false;

            if(ownerBalloon != null)
                ownerBalloon.SendFailed(this);
        }

        public string GetAssetName()
        {
            if(chatPoll != null)
            {
                if(string.IsNullOrEmpty(assetName))
                {
                    if (isSystem)
                    {
                        assetName = "Text Balloon System";
                    }
                    else
                    {
                        // bool isMe = (chatPoll.userId == BlackboardQueryUtils.GetMyUserId());
                    
                        StringBuilder sb = new StringBuilder("Text Balloon");

                        if (isMe)
                        {
                            sb.Append(" Me");
                        }
                        else
                        {
                            sb.Append(" Other");
                        }

                        switch (chatPoll.type)
                        {
                            case ChatType.MESSAGE:
                            case ChatType.INSTANT:
                                sb.Append(" Message");
                                break;
                            case ChatType.EMOJI:
                                sb.Append(" Emoticon");
                                break;
                            case ChatType.CLUB_PR:
                                sb.Append(" Club");
                                break;
                            case ChatType.BOSS_RAIDERS_BOSS_KILL:
                                return "Text Balloon Meta Boss Kill";
                            case ChatType.BOSS_RAIDERS_RANKING_UP:
                                return "Text Balloon Meta Boss Ranking Up";
                            case ChatType.BOSS_RAIDERS_LEADING_ATTACKER:
                                return "Text Balloon Meta Boss Leaders Up";
                            case ChatType.CLUB_ARENA_CLUB_RANKING_UP:
                                return "Text Balloon Meta Club Ranking Up";
                            case ChatType.CLUB_ARENA_LEADING_CLUB_MEMBER:
                                return "Text Balloon Meta Club Leaders Up";
                        }

                        assetName = sb.ToString();
                    }
                }
            }

            // if(!isMe)
            // {
                bool isFirst = ownerChannel.IsUserFirstChatPoll(this);

                if (isFirst && !isSystem)
                    return assetName + " First";
            // }
            

            return assetName;
        }

        public string GetMessage()
        {
            if(chatPoll.GetChatType() == ChatType.MESSAGE)
            {
                if(string.IsNullOrEmpty(filterMessage))
                    filterMessage = StringTable.BadWordFilter(chatPoll.GetString());

                return filterMessage;
            }

            return chatPoll.GetString();
        }

    }
}

