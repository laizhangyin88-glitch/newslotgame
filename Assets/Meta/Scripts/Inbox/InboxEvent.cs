namespace BagelCode
{
    public static class InboxEvent
    {
        // Main
        public const string REFRESH_INBOX_ITEM = "RefreshInboxItem";
        public const string ON_CONNECT_COMPLETE = "OnConnectComplete";
        public const string CLOSE_INBOX = "CloseInbox";
        public const string ON_ENTER_DAILY_SPIN = "OnEnterDailySpin";
        public const string ON_LEAVE_INBOX = "OnLeaveInbox";
        public const string ON_LEAVE_LOBBY = "OnLeaveLobby";
        public const string ON_COLLECT_COMMON_REWARDS = "OnCollectCommonRewards";
        public const string ON_INBOX_COLLECT_ALL = "OnInboxCollectAll";
        public const string INBOX_TO_DAILY_SPIN = "InboxToDailySpin";
        public const string SYSTEM_RESET = "SystemReset";
        public const string RELOAD_INBOX = "ReloadInbox";

        // Banner
        public const string ON_CLICK_BANNER = "OnClickBanner";
        public const string ON_INVITE_ERROR = "OnInviteError";
        public const string ON_SUCCESS_FACEBOOK_INVITE = "OnSuccessFacebookInvite";

        // Cell
        public const string ON_ACCEPT = "OnAccept";
        public const string ON_SUCCESS_ACCEPT_INBOX = "OnSuccessAcceptInbox";
        public const string ON_FAIL_ACCEPT_INBOX = "OnFailAcceptInbox";
        public const string ON_CANCEL_ACCEPT_INBOX = "OnCancelAcceptInbox";
        public const string ON_IAM_CALLBACK = "OnIAMCallback";
        public const string REFRESH_PASSIVE = "RefreshPassive";
        public const string START_PASSIVE = "StartPassive";
        public const string ON_READY = "OnReady";
        public const string UPDATE_NAVI_CREDIT = "UpdateNaviCredit";
        public const string UPDATE_INBOX_ITEM = "UpdateInboxItem";
        public const string ON_INBOX_ACCEPTABLE = "OnInboxAcceptable";
        public const string ACCEPT_NEXT_INBOX_ITEM = "AcceptNextInboxItem";
        public const string ON_SUCCESS_ACCEPT_GIFT_BUCKS = "OnSuccessAcceptGiftBucks";
    }
}
