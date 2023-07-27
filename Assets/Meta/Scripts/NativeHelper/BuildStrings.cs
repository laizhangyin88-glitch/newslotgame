#if (UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX) && !UNITY_EDITOR

public class BuildStrings
{
    #if BUILD_PROD
        static public string serverBaseUrl = "https://logic-prod.slots1.bagelgames.com/api";
        static public string appDownloadUrl = "https://apps.facebook.com/1881581215396162";
        static public string chattingUrl = "https://chat-prod.slots1.bagelgames.com";
    #elif BUILD_ST
        static public string serverBaseUrl = "https://logic-st.slots1.bagelcode.com:20000/api";
        static public string appDownloadUrl = "https://apps.facebook.com/1881581215396162";
        static public string chattingUrl = "https://chat-st.slots1.bagelgames.com";
    #elif BUILD_QA
        static public string serverBaseUrl = "https://logic-qa.slots1.bagelgames.com/api";
        static public string appDownloadUrl = "https://apps.facebook.com/131332947544414";
        static public string chattingUrl = "https://chat-qa.slots1.bagelgames.com";
    #elif BUILD_TEAM
        static public string serverBaseUrl = "https://logic-team.slots1.bagelgames.com/api";
        static public string appDownloadUrl = "https://apps.facebook.com/1898338550387095";
        static public string chattingUrl = "https://chat-team.slots1.bagelgames.com";
    #elif BUILD_TEAM_CONTENTS
        static public string serverBaseUrl = "https://logic-team-gs.slots1.bagelgames.com/api";
        static public string appDownloadUrl = "https://apps.facebook.com/751808465251537";
        static public string chattingUrl = "https://chat-team-gs.slots1.bagelgames.com";
    #else
        // get default serverBaseUrl from ApplicationSettings
        static public string serverBaseUrl = "";
        static public string appDownloadUrl = "";
        static public string chattingUrl = "";
    #endif
}

#endif
