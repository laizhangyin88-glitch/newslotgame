

namespace BagelCode
{
    public static class ClubDefine
    {
#if DEV
        // 1 min
        public const long LEAGUE_POPUP_COOL_TIME_MS = 60000L;
#else
        // 20 hour
        public const long LEAGUE_POPUP_COOL_TIME_MS = 72000000L;
#endif
        public const int LEADER_PUSH_UNLOCK_CLICK_MAX = 10;
        public const int LEADER_PUSH_REQUIRED_CLUB_LEVEL = 4;

        public const string PLAYER_PREFS_LAST_CLUB_CHECKED_MS = "LAST_CLUB_CHECKED";
        public const string PLAYER_PREFS_CLUB_LEVEL = "CLUB_LEVEL";
        public const string PLAYER_PREFS_UNLOCK_LEADER_PUSH_POPUP_SHOWN = "IS_UNLOCK_LEADER_PUSH_POPUP_SHOWN";
        public const string PLAYER_PREFS_LEADER_PUSH_CLICK_COUNT = "LEADER_PUSH_CLICK_COUNT";
        public const string PLAYER_PREFS_LAST_META_GAME_CHECKED = "LAST_META_GAME_CHECKED";
        public const string PLAYER_PREFS_LEAGUE_RESULT_POPUP = "CLUB_LEAGUE_RESULT_WEEK";
    }
}
