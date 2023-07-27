using BagelCode.ClientModels;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Chat
{
    public class TextBalloonMetaGameBossRaidersRankingUp : TextBalloonMetaGame
    {
        private GameObject iconObject = null;

        public override void Init(ChattingController _owner)
        {
            base.Init(_owner);

            //MetaObjectUtils.MakePrefab("Boss Raiders Deco", decoElement.transform);
            MetaObjectUtils.MakePrefab("Icon Rank Up", chattingIconElement.transform);
        }

        public override void Refresh(ChatMessageData data)
        {
            base.Refresh(data);

            ChatDataBossRaidersRankingUp chatBossRankingUp = (ChatDataBossRaidersRankingUp)chatData.chatPoll.data;
            if (iconObject == null)
                MetaObjectUtils.MakePrefab(GetBossRaidersChatIcon(chatBossRankingUp.themeId), metaIconElement.transform);
            SetUserName(StringTableUtils.GetString(StringTable.StringTableType.Global, "CHAT_BOSS_RAIDERS_NOTICE_TITLE"));
            SetChattingText(StringTableUtils.GetString(StringTable.StringTableType.Global, "CHAT_BOSS_RAIDERS_CLUB_RANKING_UP_TEXT",
                chatBossRankingUp.clubRank, chatBossRankingUp.clubRank));
        }
    }
}