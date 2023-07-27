using BagelCode.ClientModels;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Chat
{
    public class TextBalloonMetaGameBossRaidersKill : TextBalloonMetaGame
    {
        private GameObject iconObject = null;

        public override void Init(ChattingController _owner)
        {
            base.Init(_owner);

            //MetaObjectUtils.MakePrefab("Boss Raiders Deco", decoElement.transform);
            MetaObjectUtils.MakePrefab("Icon Defeated", chattingIconElement.transform);
        }

        public override void Refresh(ChatMessageData data)
        {
            base.Refresh(data);

            ChatDataBossRaidersBossKill chatBossKill = (ChatDataBossRaidersBossKill)chatData.chatPoll.data;
            if (iconObject == null)
                MetaObjectUtils.MakePrefab(GetBossRaidersChatIcon(chatBossKill.themeId), metaIconElement.transform);
            SetUserName(StringTableUtils.GetString(StringTable.StringTableType.Global, "CHAT_BOSS_RAIDERS_NOTICE_TITLE"));
            SetChattingText(StringTableUtils.GetString(StringTable.StringTableType.Global, "CHAT_BOSS_RAIDERS_KILL_TEXT", chatBossKill.round));
        }
    }
}