using SlotMaker;
using NodeCanvas.Framework;

namespace BagelCode
{
    public class InboxCellDataSales : InboxCellData
    {
        public InboxCellDataSales(InboxCellController _owner, Blackboard _inboxInfo)
            : base(_owner, _inboxInfo)
        {
            buttonType = InboxCellController.InboxButtonType.REDEEM;
        }

        protected override string GetInternalText()
        {
            string resultText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_MESSAGE", title, message);

            string userName = BlackboardUtils.FindValue<string>(MainBlackboard.Get(), "me/name");

            string gameName = "{game_name}";
            var gameIdVar = inboxInfo.GetVariable<int>("gameId");
            if (gameIdVar != null && gameIdVar.value > 0)
            {
                gameId = gameIdVar.value;
                gameName = BlackboardQueryUtils.GetMetaGameTitle(gameIdVar.value);
            }

            TextDecoUtils.ConvertStringFormat(ref resultText, "{name}", userName);
            TextDecoUtils.ConvertStringFormat(ref resultText, "{game_name}", gameName);

            return resultText;
        }

        protected override string GetButtonText()
        {
            return StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_BUTTON_SHOW_ME");
        }

        protected override IconType GetIconType()
        {
            var gameId = inboxInfo.GetVariable<int>("gameId");
            if (gameId != null && gameId.value > 0)
                return IconType.GAME_THUMBNAIL;

            var iconUrlVar = inboxInfo.GetVariable<string>("imageUrl");
            if (iconUrlVar != null && !string.IsNullOrEmpty(iconUrlVar.value))
            {
                iconUrl = iconUrlVar.value;
                return IconType.WEB_IMAGE;
            }

            return IconType.DEFAULT;
        }
    }
}
