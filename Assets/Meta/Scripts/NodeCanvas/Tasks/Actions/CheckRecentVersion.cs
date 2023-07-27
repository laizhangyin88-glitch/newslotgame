using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Utils")]
    public class CheckRecentVersion : ActionTask
    {
        private const string LAST_RECENT_CLIENT_VERSION = "LAST_RECENT_CLIENT_VERSION";
        private const string LAST_RECENT_TIMESTAMP = "LAST_RECENT_SHOW_TIMESTAMP";

        public BBParameter<string> fromType;

        // DEV : Min, others : Days *** // not use recent cooltime.. ***
        private const long SAME_VERSION_RECENT_COOL_TIME_DAY = 0;

        protected override string info
        {
            get { return "Check Client Recent Version."; }
        }

        protected override void OnExecute()
        {
            int clientVersion = ApplicationSettings.GetClientVersionNumber();
            int recentVersion = BlackboardUtils.FindVariable<int>(null, "/recentClientNumberVersion").value;

            if (clientVersion < recentVersion)
            {
                int lastRecentVersion = PlayerPrefs.GetInt(LAST_RECENT_CLIENT_VERSION, 0);
                long lastTriggerTimestamp = PlayerPrefsUtils.GetInt64(LAST_RECENT_TIMESTAMP, 0L);
                long currentTimestamp = TimeUtils.GetTimeStamp();

                if (lastRecentVersion != clientVersion || currentTimestamp >= (lastTriggerTimestamp + GetCoolTime()))
                {
                    PlayerPrefs.SetInt(LAST_RECENT_CLIENT_VERSION, lastRecentVersion);
                    PlayerPrefsUtils.SetInt64(LAST_RECENT_TIMESTAMP, currentTimestamp);

                    string appDownloadURL = BlackboardUtils.FindVariable<string>(null, "/appDownloadUrl").value;
                    var rewardCoins = BlackboardUtils.FindVariable<long>(null, "/values/misc/VERSION_UPDATE_REWARD_CREDIT");

                    ErrorPopupInfo info = new ErrorPopupInfo();

                    info.type = ErrorPopupType.OkWithTitle;
                    info.title = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_DEPRECATED_VERSION_ERROR_WITH_TITLE");
                    info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_DEPRECATED_VERSION_ERROR_WITH_REWARD", rewardCoins.value);

                    info.useXButton = true;

                    info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OK");
                    info.buttonAutoClose1 = false;

                    info.callback1 = delegate
                    {
                        Application.OpenURL(appDownloadURL);
                    };

                    info.callbackX = delegate
                    {
                        EndAction();
                    };

                    ErrorPopupHandler.Instance.OpenError(info);

                    BICustomEvents.SendUpdateAppRecommandAE(recentVersion); // send bi event
                }
                else
                {
                    EndAction();
                }
            }
            else
            {
                EndAction();
            }
        }

        private long GetCoolTime()
        {
#if DEV
            return SAME_VERSION_RECENT_COOL_TIME_DAY * TimeUtils.ONE_MIN_MS;
#else
            return SAME_VERSION_RECENT_COOL_TIME_DAY * TimeUtils.ONE_DAY_MS;
#endif
        }
    }
}