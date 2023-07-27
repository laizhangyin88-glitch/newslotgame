using System.Collections.Generic;
using BagelCode.ClientModels;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Chat
{
    public class TextBalloonMetaGame : TextBalloonBase
    {
        public enum MetaGameType
        {
            Unknown = -666,
            BossRaiders = 0,
            ClubArena,
        }

        public List<Sprite> chatBallonImages = new List<Sprite>();

        private MetaGameType metaGameType = MetaGameType.Unknown;

        protected ContextElement metaIconElement;
        protected ContextElement textBalloonElement;
        protected ContextElement decoElement;
        protected ContextElement chattingIconElement;
        protected ContextElement chattingTextElement;

        public override void Init(ChattingController _owner)
        {
            base.Init(_owner);

            metaIconElement = ContextUtils.FindElement(mContext, "Profile Area", ContextSearchingType.ChildrenSearch);
            textBalloonElement = ContextUtils.FindElement(mContext, "Text Balloon", ContextSearchingType.ChildrenSearch);
            decoElement = ContextUtils.FindElement(textBalloonElement, "Deco Area", ContextSearchingType.ChildrenSearch);
            chattingIconElement = ContextUtils.FindElement(textBalloonElement, "Icon Area", ContextSearchingType.ChildrenSearch);
            chattingTextElement = ContextUtils.FindElement(textBalloonElement, "Chatting Text", ContextSearchingType.ChildrenSearch);
        }

        public override void Refresh(ChatMessageData data)
        {
            base.Refresh(data);
            switch (chatData.chatPoll.type)
            {
                case ChatType.BOSS_RAIDERS_BOSS_KILL:
                case ChatType.BOSS_RAIDERS_RANKING_UP:
                case ChatType.BOSS_RAIDERS_LEADING_ATTACKER:
                    metaGameType = MetaGameType.BossRaiders;
                    break;
                case ChatType.CLUB_ARENA_CLUB_RANKING_UP:
                case ChatType.CLUB_ARENA_LEADING_CLUB_MEMBER:
                    metaGameType = MetaGameType.ClubArena;
                    break;
            }

            SetSpriteTextBalloon();
        }

        private void SetSpriteTextBalloon()
        {
            if (metaGameType != MetaGameType.Unknown && textBalloonElement != null && chatBallonImages.IsValidIndex((int)metaGameType))
                MetaContextElementUtils.SetSprite(textBalloonElement, chatBallonImages[(int)metaGameType]);
        }

        protected void SetChattingText(string text)
        {
            if (chattingTextElement != null)
                MetaContextElementUtils.SetText(chattingTextElement, text);
        }

        protected string GetBossRaidersChatIcon(int themeId)
        {
            return string.Format("Icon Meta Boss Raiders_{0}", themeId);
        }
    }
}