

namespace BagelCode
{
    public static class HogDeal
    {
        public static class Defines
        {
            public const string COMMON_BUNDLE = "mghogdealcommon";
            public const string CONTENTS_BUNDLE = "mghogdealcontents";

            // Player Prefs
            public const string PLAYER_PREFS_JACKPOT = "HOG_DEAL_JACKPOT";
            public const string PLAYER_PREFS_WIN = "HOG_DEAL_WIN";
        }

        public static class Events
        {
            public const string ON_TAP = "OnTap";

            public const string ON_COLLECT = "OnCollect";
            public const string ON_NEXT = "OnNext";
            public const string ON_CLOSE = "OnClose";
        }
    }
}
