using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
#if UNITY_EDITOR
using System.Text.RegularExpressions;
#endif

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Utils")]
    public class ClientMigration : ActionTask
    {
        private const string LAST_CLIENT_VERSION_KEY = "LAST_CLIENT_VERSION";

        protected override string info
        {
            get
            {
                return "Client Migration Progress";
            }
        }

        protected override void OnExecute()
        {
            int clientVersion = ApplicationSettings.GetClientVersionNumber();
            int lastClientVersion = PlayerPrefs.GetInt(LAST_CLIENT_VERSION_KEY, clientVersion);

            if(lastClientVersion < clientVersion)
            {
                DoMigration(clientVersion, lastClientVersion);
            }

            PlayerPrefs.SetInt(LAST_CLIENT_VERSION_KEY, clientVersion);
            PlayerPrefs.Save();

#if UNITY_EDITOR
            UpdateAppName();
#endif

            EndAction();
        }

        private void DoMigration(int currentVersion, int prevVersion)
        {
            // Debug.LogError( string.Format("Test Migration {0} -> {1}", prevVersion, currentVersion));
#if UNITY_IOS || UNITY_ANDROID
            PlayerPrefs.SetInt("ShowFriendInviteLinkBadge", 1);
#endif
        }

#if UNITY_EDITOR
        private void UpdateAppName()
        {
            var bb = StringTable.Get(StringTable.StringTableType.Global);
            bool existKeyName = false;

            foreach (var pair in bb.variables)
            {
                string orgText = bb.GetValue<string>(pair.Key);
                if(orgText != null && (orgText.Contains("{app_name}") || orgText.Contains("{app_name_upper}")) )
                {
                    orgText = Regex.Replace(orgText, "{app_name}", "(AppName)", RegexOptions.None);
                    orgText = Regex.Replace(orgText, "{app_name_upper}", "(APPNAME)", RegexOptions.None);
                    bb.SetValue(pair.Key, orgText);
                    Debug.Log(orgText);
                    existKeyName = true;
                }
            }

            if(existKeyName)
                Debug.LogError("You must verify that {app_name}, {app_name_up} is in the en_global file.");
        }
#endif
    }
}
