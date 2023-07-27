using BagelCode.ClientModels;
using SlotMaker;

namespace BagelCode.Chat
{
    public class TextBalloonMetaGameClubArenaRankingUp : TextBalloonMetaGame
    {
        public override void Init(ChattingController _owner)
        {
            base.Init(_owner);

            MetaObjectUtils.MakePrefab("Icon Meta Club Arena", metaIconElement.transform);
            MetaObjectUtils.MakePrefab("Club Arena Deco", decoElement.transform);
            MetaObjectUtils.MakePrefab("Icon Rank Up", chattingIconElement.transform);
        }

        public override void Refresh(ChatMessageData data)
        {
            base.Refresh(data);

            SetUserName(StringTableUtils.GetString(StringTable.StringTableType.Global, "CHAT_CLUB_ARENA_NOTICE_TITLE"));
            ChatDataClubArenaClubRankingUp chatClubRankingUp = (ChatDataClubArenaClubRankingUp)chatData.chatPoll.data;
            SetChattingText(StringTableUtils.GetString(StringTable.StringTableType.Global, "CHAT_CLUB_ARENA_CLUB_RANKING_UP_TEXT",
                chatClubRankingUp.clubRank, chatClubRankingUp.clubRank));
        }
    }
}