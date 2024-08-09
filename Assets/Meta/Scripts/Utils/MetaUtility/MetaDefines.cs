using System.Collections.Generic;

namespace BagelCode
{
    public enum MetaGameType
    {
        NONE = 0,

        // Collecting Game Type
        // number: collecting game id
        // 타입 누락되었을 때 에러 노출 필요
        // todo shk
        // 어디선가 validate 해줄것
        VEGAS_NIGHT = 1,
        WORLD_TOUR = 2,
        JOKERS_TABLE = 3,
        PAWSOME_FURTUNE = 4,
        BILLIONAIRE_BINGO = 5,
        CLUB_KENO = 6,
        SPORTS_LEAGUE = 7,
        COOK = 8,
        ROLLER_GHOSTER = 9,

        // Passive Meta Games
        // GetMetaGameEventInfo 시 먼저 정의된 것 체크
        // 먼저 정의된 것이 우선순위 높음
        LUCKY_FIVE = 100,
        SEASON_PASS, // 101 ..
        BOSS_RAIDERS,
        CLUB_ARENA,
        VIP_LOUNGE,
        SEASON_PASS_V2,

        // Other Meta Games
        GOLDEN_TOWER,
        GEM_JACKPOT,
        LEVEL_UP_DASH,
        BUILD_DREAM_SEASON,
        HIDDEN_OBJECTS,
    }

    public enum MetaChallengeType
    {
        NONE = 0,
        DAILY,
        EXPERT,
        MASTER,
        CLUB,
        EVENT_PERSONAL,
        EVENT_CLUB,
        EPIC_PASS,
        NORMAL, // Daily Challenge Tab
    }

    public enum MetaChallengeMainTabType
    {
        NONE = 0,
        NORMAL,     // Normal(Daily) Challenge - Daily Expert Master / Club / Event - Personal, Club
        EPIC_PASS,
    }

    public enum MetaChallengeDailyTabType
    {
        NONE = 0,
        DAILY,
        EXPERT,
        MASTER,
        CLUB,
        EVENT_PERSONAL,
        EVENT_CLUB,
    }

    public enum SceneState
    {
        LOGIN = 0,

        LOBBY,
        EARLY_ACCESS,

        DAILY_BONUS,
        BINGO,
        NOTICE,

        DAILY_SPIN,
        INBOX,
        FRIENDS,
        WALL_OF_EPIC,
        CLUB,
        EPIC_ALBUM,
        LEADERBOARD,
        FRIEND,

        INGAME,
        PAYTABLE,

        EVENT_META_GAME,

        LOADING,

        MAX
    }

    public enum MysteryGiftType
    {
        NONE = 0,
        LEVEL_UP,
        LEADER_PUSH,
    }

    public enum BottomState
    {
        NONE = -666,
        LEAGUE = 0,
        MEMBER = 1,
        NEWS_FEED = 2,
        CHALLENGE = 3,
        METAGAME = 4,
    }

    public enum RewardCheckScene
    {
        DEFAULT = 0,
        LEVEL_UP_DASH,
        EPIC_PASS_V2,
    }

    public static class MetaStringDefine
    {
        private static string lobbyBundleName = null;

        public static string TEST_SUITE = "testsuite";

        public static string LOBBY_BUNDLE_NAME
        {
            get
            {
                if (string.IsNullOrEmpty(lobbyBundleName))
                {
                    lobbyBundleName = SlotMaker.ApplicationSettings.MakeApplicationBundleName("lobby");
                }

                return lobbyBundleName;
            }
        }

        private static string lobbySoundBundleName = null;
        public static string LOBBY_SOUND_BUNDLE_NAME
        {
            get
            {
                if (string.IsNullOrEmpty(lobbySoundBundleName))
                {
                    lobbySoundBundleName = SlotMaker.ApplicationSettings.MakeApplicationBundleName("lobbysnd");
                }

                return lobbySoundBundleName;
            }
        }

        public const int CLUB_CHALLENGE_STAGE_MAX = 1;

        public const string TIME_FORMAT_HHMMSS_TOTALHOUR = "TIME_FORMAT_HHMMSS_TOTALHOUR";

        public const string SNAPSHOT_LOBBY_MAIN = "Lobby_Main";
        public const string SNAPSHOT_LOBBY_POPUP = "Lobby_Popup";
        public const string SNAPSHOT_CONTENT_MUTE = "Content_Mute";
        public const string SNAPSHOT_CONTENT_MAIN = "Content_Main";
        public const string SNAPSHOT_CONTENT_POPUP = "Content_Popup";
        public const string SNAPSHOT_LOADING = "Loading";

        public const string PLATFORM_ANDROID = "ANDROID";
        public const string PLATFORM_IOS = "IOS";
        public const string PLATFORM_CANVAS = "Canvas";
        public const string PLATFORM_WSA = "WINDOWS";
        public const string PLATFORM_StandaloneWindows = "GAMEROOM";
        public const string PLATFORM_StandaloneOSX = "OSX_STANDALONE";
    }

    public static class MetaEventDefine
    {
        // General
        public const string ON_ENTER_META_GAME = "OnEnterMetaGame";
        public const string ON_LEAVE_META_GAME = "OnLeaveMetaGame";
        public const string ANY_EVENT_NAME = "ANY_EVENT_NAME";
        public const string ACTIVE_META_UI = "ActiveMetaUI";
        public const string INACTIVE_META_UI = "InActiveMetaUI";
        public const string ON_HOME_BUTTON = "OnHomeButton";

        // Main
        public const string ON_CONTENT_UI_EVENT = "OnContentUIEvent";
        public const string ON_META_UI_EVENT = "OnMetaUIEvent";
        public const string ON_SYSTEM_EVENT = "OnSystemEvent";
        public const string ON_LONG_POLL_EVENT = "OnLongPollEvent";
        public const string ON_LOCAL_FEED_EVENT = "OnLocalFeedEvent";
        public const string ON_CREDIT_EVENT = "OnCreditEvent";
        public const string ON_PASSIVE_EVENT = "OnPassiveEvent";
        public const string ON_CONTENT_EVENT = "OnContentEvent";

        // PassiveEvent
        public const string START_PASSIVE_EVENT = "StartPassive";
        public const string REFRESH_PASSIVE_EVENT = "RefreshPassive";

        /// OnMetaUIEvent
        public const string ON_MAKE_RESULT_POPUP = "OnMakeResultPopup";
        public const string ON_LEVEL_UP = "OnLevelUp";
        public const string ON_LOAD_SUCCESS_BUNDLE = "OnLoadSuccessBundle";
        public const string ON_OPEN_META_EVENT_GROUP = "OnOpenMetaEventGroup";
        public const string ON_CLOSE_META_EVENT_GROUP = "OnCloseMetaEventGroup";
        public const string ON_BEGIN_META_GAME = "OnBeginMetaGame";
        public const string ON_END_TURN_META = "OnEndTurnMeta";
        public const string ON_END_META_GAME = "OnEndMetaGame";
        public const string ON_FINISH_GETTING_META_GAME_ITEM = "OnFinishGettingMetaGameItem";
        public const string ON_BLOCK = "OnBlock";
        public const string ON_UNBLOCK = "OnUnblock";
        public const string ON_BLOCKED_USER_CHANGED = "OnBlockedUserChanged";

        public const string ON_ENTER_ONLINE_PLAYERS = "OnEnterOnlinePlayers";
        public const string ON_LEAVE_ONLINE_PLAYERS = "OnLeaveOnlinePlayers";
        public const string ON_ENTER_LEADERBOARD = "OnEnterLeaderboard";
        public const string ON_LEAVE_LEADERBOARD = "OnLeaveLeaderboard";
        public const string ON_ENTER_WOE = "OnEnterWallOfEpic";
        public const string ON_LEAVE_WOE = "OnLeaveWallOfEpic";
        public const string ON_ENTER_COUPON = "OnEnterCoupon";
        public const string ON_LEAVE_COUPON = "OnLeaveCoupon";
        public const string ON_ENTER_CUSTOMER_SUPPORT = "OnEnterCustomerSupport";
        public const string ON_ENTER_REFUND_DIALOG = "OnEnterRefundDialog";
        public const string ON_LEAVE_REFUND_DIALOG = "OnLeaveRefundDialog";
        public const string ON_ENTER_JACKPOT_DIALOG = "OnEnterNewJackpotDialog";
        public const string ON_LEAVE_JACKPOT_DIALOG = "OnLeaveNewJackpotDialog";
        public const string ON_ENTER_JACKPOT_RECORD = "OnEnterJackpotRecord";
        public const string ON_LEAVE_JACKPOT_RECORD = "OnLeaveJackpotRecord";

        public const string REDEEM_COUPON = "RedeemCoupon";
        public const string ON_COUPON_REDEEM_SUCCESS = "OnRedeemSuccess";
        public const string ON_COUPON_REDEEM_FAILURE = "OnRedeemFailure";

        public const string ON_OPEN_COMMON_REWARD_POPUP = "OnOpenCommonRewardPopup";

        public const string ON_ENABLE_BACK_BUTTON = "OnEnableBackButton";
        public const string ON_DISABLE_BACK_BUTTON = "OnDisableBackButton";
        ///

        // SystemEvent
        public const string SYSTEM_RESET = "SystemReset";

        // Meta Game
        public const string SHOW_BET_UP_BALLOON = "ShowBetUpBalloon";
        public const string ON_EARN_META_GAME_ITEM = "OnEarnMetaGameItem";
        public const string UPDATE_META_GAME_ITEM_EARNING_INSTANTLY = "UpdateMetaGameItemEarningInstantly";

        // Club
        public const string ON_CLUB_JOIN_SUCCESS = "OnJoinReqestClubSuccess";
        public const string ON_CLUB_EDIT = "OnClubEdit";
        public const string ON_CLUB_LEAVE = "LeaveClub";
        public const string ON_CLUB_REQUEST_ACCEPTED = "OnClubRequestAccepted";

        public const string ON_ARRIVE_EARN_EPIC_PASS_POINT_EFFECT = "OnArriveEarnEpicPassPointEffect";
        public const string ON_CLICK_CLOSE_PURCHASE_REWARD_POPUP = "OnCloseRewardPopup";

        public const string UPDATE_LEADER_PUSH_STATE_EVENT = "UpdateLeaderPushState";
        public const string UPDATE_LEADER_PUSH_BUTTON_EVENT = "UpdateLeaderPushButton";
        public const string ON_CLICK_LEADER_PUSH_OPEN = "OnClickLeaderPushOpen";

        public const string ON_CLUB_FEED_REFRESH = "OnClubFeedRefresh";
        public const string ON_REFRESH_CLUB_INFO = "OnRefreshClubInfo";

        // VIP Lounge
        public const string ON_VIP_LOUNGE_REWARD_EVENT = "OnVIPLoungeReward";
        public const string ON_VIP_LOUNGE_REWARD_SUCCESS = "OnVIPLoungeRewardSuccess";
        public const string ON_VIP_LOUNGE_WELCOME_JOIN = "OnVIPLoungeWelcomeJoin";

        // Common
        public const string ON_CANCEL = "OnCancel";

        //Machine
        public const string ON_MACHINE = "OnMachine";
    }

    public static class SystemEventDefine
    {
        public const string ON_SYSTEM_EVENT = "OnSystemEvent";
        public const string ON_SYSTEM_RESET_EVENT = "SystemReset";
        public const string ON_FINISHED_LOGIN_EVENT = "FinishedLogin";
    }

    public static class MachineEventDefine
    {
        public const string ON_KEY_UP = "OnKeyUp";
        public const string ON_KEY_DOWN = "OnKeyDown";
        public const string ON_KEY_RIGHT = "OnKeyRight";
        public const string ON_KEY_LEFT = "OnKeyLeft";
        public const string ON_KEY_START = "OnKeyStart";
        public const string ON_KEY_BET5 = "OnKeyTab";
        public const string ON_KEY_MAX_BET = "OnKeyMaxBet";
        public const string ON_LIGHT_CHANGE = "OnLightChange";
    }
}
