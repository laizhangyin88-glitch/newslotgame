using UnityEngine;
using System.Collections.Generic;
using BagelCode.ClientModels;
using SlotMaker;

namespace BagelCode.Chat
{
    public class TextBalloonMetaGameClubArenaLeadersUp : TextBalloonMetaGame
    {
        private List<GameObject> rankingIcons = new List<GameObject>();

        public override void Init(ChattingController _owner)
        {
            base.Init(_owner);

            MetaObjectUtils.MakePrefab("Club Arena Deco", decoElement.transform);
            rankingIcons.Add(MetaObjectUtils.MakePrefab("Icon Rank 1st", chattingIconElement.transform));
            rankingIcons.Add(MetaObjectUtils.MakePrefab("Icon Rank 2nd", chattingIconElement.transform));
            rankingIcons.Add(MetaObjectUtils.MakePrefab("Icon Rank 3rd", chattingIconElement.transform));
            SetActiveRankingIcons(0);
        }

        public override void Refresh(ChatMessageData data)
        {
            base.Refresh(data);

            ChatDataClubArenaLeadingClubMember chatClubLeading = (ChatDataClubArenaLeadingClubMember)chatData.chatPoll.data;
            SetActiveRankingIcons(chatClubLeading.clubMemberRank);
            SetChattingText(StringTableUtils.GetString(StringTable.StringTableType.Global, "CHAT_META_GAME_LEADING_ATTACKER_TEXT",
                chatClubLeading.clubMemberRank, chatClubLeading.clubMemberRank, StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_ARENA_NAME")));
        }

        private void SetActiveRankingIcons(int memberRank)
        {
            for (int i = 0; i < rankingIcons.Count; ++i)
                rankingIcons[i].SetActive(memberRank - 1 == i);
        }
    }
}