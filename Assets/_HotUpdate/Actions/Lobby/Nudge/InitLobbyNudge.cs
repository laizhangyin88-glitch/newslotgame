using System;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Lobby")]

public class InitLobbyNudge : ActionTask <Blackboard>
{
    public BBParameter<string> onShowNudgePopupName;

#if DEV
    private static int[] facebookNudgeCoolTimeDayList = { 3,  7, 30}; // Test Min. 
    private static int[] emailNudgeCoolTimeDayList    = {30, 30, 30}; // Test Min. 
#else
    private static int[] facebookNudgeCoolTimeDayList = { 3,  7, 30};
    private static int[] emailNudgeCoolTimeDayList    = {30, 30, 30};
#endif

    // .for facebook
    private const string LOBBY_NUDGE_FACEBOOK_LAST_SHOW_TIMESTAMP = "LOBBY_NUDGE_FACEBOOK_LAST_SHOW_TIMESTAMP";
    private const string LOBBY_NUDGE_FACEBOOK_COOLTIME_INDEX      = "LOBBY_NUDGE_FACEBOOK_COOLTIME_INDEX";

    // .for email
    private const string LOBBY_NUDGE_EMAIL_LAST_SHOW_TIMESTAMP    = "LOBBY_NUDGE_EMAIL_LAST_SHOW_TIMESTAMP";
    private const string LOBBY_NUDGE_EMAIL_COOLTIME_INDEX         = "LOBBY_NUDGE_EMAIL_COOLTIME_INDEX";

    private const int FB_NUDGE_LEVEL_RESTRICTION = 12;
    private const int EMAIL_NUDGE_LEVEL_RESTRICTION = 15;
    
    protected override string info
    { 
        get 
        { 
            return string.Format("Init Lobby Nudge(Facebook, Email)");
        } 
    }

    protected override void OnExecute()
    {
        onShowNudgePopupName.value = "";

        var loginCount     = BlackboardUtils.FindVariable<int>(null, "/me/loginCount");
        var ssoAccountInfo = BlackboardUtils.FindVariable<Blackboard>(null, "/ssoAccountInfo");
        
        if (loginCount != null && loginCount.value > 5)
        {
            if (ssoAccountInfo == null)
            {
                ShowFacebookNudge();
            }
            else
            {
                var facebookId = BlackboardUtils.FindVariable<string>(ssoAccountInfo.value, "facebookId");
                var email      = BlackboardUtils.FindVariable<string>(ssoAccountInfo.value, "email");

                if (facebookId == null || string.IsNullOrEmpty(facebookId.value))
                {
                    ShowFacebookNudge();
                }
                else if ( email == null || string.IsNullOrEmpty(email.value))
                {
                    ShowEmailNudge();
                }
            }
        }
        
        EndAction(true);
    }

    private void ShowFacebookNudge()
    {
        var meLevel = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "me/level");
        
        if(meLevel.value < FB_NUDGE_LEVEL_RESTRICTION) return;

        var lastTimestamp = PlayerPrefsUtils.GetOrCreateInt64(LOBBY_NUDGE_FACEBOOK_LAST_SHOW_TIMESTAMP);

        if(lastTimestamp == 0L)
        {
            OpenFacebookNudgePopup();

            PlayerPrefsUtils.SetInt64(LOBBY_NUDGE_FACEBOOK_LAST_SHOW_TIMESTAMP, TimeUtils.GetTimeStamp());
        }
        else
        {
            int coolTimeIndex = PlayerPrefs.GetInt(LOBBY_NUDGE_FACEBOOK_COOLTIME_INDEX, 0);
            if(coolTimeIndex >= facebookNudgeCoolTimeDayList.Length)
                coolTimeIndex = facebookNudgeCoolTimeDayList.Length-1;

            long currentTimestamp = TimeUtils.GetTimeStamp();
            long targetTimestamp = GetCoolTimestamp(lastTimestamp, facebookNudgeCoolTimeDayList, coolTimeIndex);

            if (currentTimestamp >= targetTimestamp)
            {
                OpenFacebookNudgePopup();
                PlayerPrefs.SetInt(LOBBY_NUDGE_FACEBOOK_COOLTIME_INDEX, ++coolTimeIndex);
                PlayerPrefsUtils.SetInt64(LOBBY_NUDGE_FACEBOOK_LAST_SHOW_TIMESTAMP, currentTimestamp);
            }
        }
    }

    private void ShowEmailNudge()
    {
        var meLevel = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "me/level");
        
        if(meLevel.value < EMAIL_NUDGE_LEVEL_RESTRICTION) return;

        var lastTimestamp = PlayerPrefsUtils.GetOrCreateInt64(LOBBY_NUDGE_EMAIL_LAST_SHOW_TIMESTAMP);

        if(lastTimestamp == 0L)
        {
            OpenEmailNudgePopup();

            PlayerPrefsUtils.SetInt64(LOBBY_NUDGE_EMAIL_LAST_SHOW_TIMESTAMP, TimeUtils.GetTimeStamp());
        }
        else
        {
            int coolTimeIndex = PlayerPrefs.GetInt(LOBBY_NUDGE_EMAIL_COOLTIME_INDEX, 0);
            if(coolTimeIndex >= emailNudgeCoolTimeDayList.Length)
                coolTimeIndex = emailNudgeCoolTimeDayList.Length-1;

            long currentTimestamp = TimeUtils.GetTimeStamp();
            long targetTimestamp = GetCoolTimestamp(lastTimestamp, emailNudgeCoolTimeDayList, coolTimeIndex);

            if (currentTimestamp >= targetTimestamp)
            {
                OpenEmailNudgePopup();
                PlayerPrefs.SetInt(LOBBY_NUDGE_EMAIL_COOLTIME_INDEX, ++coolTimeIndex);
                PlayerPrefsUtils.SetInt64(LOBBY_NUDGE_EMAIL_LAST_SHOW_TIMESTAMP, currentTimestamp);
            }    
        }
    }

    private void OpenFacebookNudgePopup()
    {
        onShowNudgePopupName.value = "Popup Facebook Connect Scene";
    }

    private void OpenEmailNudgePopup()
    {
        onShowNudgePopupName.value = "Popup Email Connect Scene";
    }

    private long GetCoolTimestamp(long timestamp, int[] coolTimes, int index)
    {
        if(index >= coolTimes.Length)
            index = coolTimes.Length-1;

#if DEV
        return timestamp + coolTimes[index] * TimeUtils.ONE_MIN_MS;
#else
        return timestamp + coolTimes[index] * TimeUtils.ONE_DAY_MS;
#endif
    }
}

}
