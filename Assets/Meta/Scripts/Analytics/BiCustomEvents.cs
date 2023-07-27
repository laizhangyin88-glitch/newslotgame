using System.Collections.Generic;
using SlotMaker;

namespace BagelCode
{

    public static class BICustomEvents
    {
        public static void StorageCheck(long freeDiskSpace, long minRequiredSpace)
        {
            Analytics.CustomEvent("client_storage_check", new Dictionary<string, object>
        {
            { "available_storage", freeDiskSpace },
            { "min_required_storage", minRequiredSpace }
        });
        }

        public static void FAS(bool pauseStatus)
        {
            if (!ApplicationReady()) return;

            Analytics.CustomEvent("client_fas", new Dictionary<string, object>
        {
            { "type", pauseStatus == true ? "pause" : "resume" }
        });
        }

        public static void EarlyAccessEnter(string enterType)
        {
            Analytics.CustomEvent("client_early_access_enter", new Dictionary<string, object>
        {
            { "enter_type", enterType }
        });
        }

        public static void SendUpdateAppRecommandAE(int version)
        {
            AEUtils.SendAE("client_update_popup_recommend",
                ("type", null),
                ("recent_client_number_version", version));
        }

        public static void SendUpdateAppRecommandAEChallenge(int version, int minVersion, int gameId, long missionId)
        {
            AEUtils.SendAE("client_update_popup_recommend",
                ("type", "challenge"),
                ("recent_client_number_version", version),
                ("min_client_number_version", minVersion),
                ("game_id", gameId),
                ("mission_id", missionId));
        }

        public static void SendUpdateAppRecommandAEClubChallenge(int version, int minVersion, int gameId, string missionId)
        {
            AEUtils.SendAE("client_update_popup_recommend",
                ("type", "club_challenge"),
                ("recent_client_number_version", version),
                ("min_client_number_version", minVersion),
                ("game_id", gameId),
                ("club_mission_id", missionId));
        }

        public static void SendUpdateAppRecommandAEIam(int version, int minVersion, int gameId, int iamId)
        {
            AEUtils.SendAE("client_update_popup_recommend",
                ("type", "iam"),
                ("recent_client_number_version", version),
                ("min_client_number_version", minVersion),
                ("game_id", gameId),
                ("iam_id", iamId));
        }

        ///
        /// Optional Function
        ///

        private static bool ApplicationReady()
        {
            return MainBlackboard.Get().GetVariable("me") != null;
        }
    }
}
