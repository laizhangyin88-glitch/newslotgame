using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Chat
{
    public class TextBalloonSystem : TextBalloonBase
    {
        public ContextText nameContext;
        public ContextText contentContext;

        public override void Init(ChattingController _owner)
        {
            base.Init(_owner);
            
            nameContext.SetGlobalText("CHAT_GLOBAL_SYSTEM_WELCOME_NAME_TEXT");
            contentContext.SetGlobalText("CHAT_GLOBAL_SYSTEM_WELCOME_NAME_CONTENT", BlackboardQueryUtils.GetMyProfile().name);
        }

        public override void Refresh(ChatMessageData _chatData)
        {
            base.Refresh(_chatData);
        }
    }

}