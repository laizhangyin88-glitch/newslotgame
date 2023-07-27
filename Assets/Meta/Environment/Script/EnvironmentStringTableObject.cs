using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BagelCode;
using SlotMaker;
using System.Text.RegularExpressions;

namespace BagelCode
{
    [CreateAssetMenu(fileName="New StringTable", menuName="SlotMaker/ScriptableObject/EnvironmentStringTableObject")]
    public class EnvironmentStringTableObject : StringTableObject
    {
        public string productName;
        public string vipLoungeName;
        public string vipBadgeName;
        public string vipMetaGameName;
        public string vipBadgeTMPName;

#if UNITY_EDITOR

        public override void CompleteImport()
        {
            Debug.Log("EnvironmentStringTableObject : Post process");

            foreach (var pair in stringTable)
            {
                string changedText = pair.value;

                // Check contains app name
                if( !string.IsNullOrEmpty(pair.value)
                 && (!pair.key.Contains("GAME_MULTILINE_TITLE") && !pair.key.Contains("GAME_TITLE")))
                {
                    string checker = changedText.Replace(" ", "").ToLower();
                    if(checker.Contains("clubvegas") || checker.Contains("cashbillionaire"))
                    {
                        Debug.LogError( string.Format("You should check {0} key. Contains the app name.", pair.key) );
                    }
                }

                if( !string.IsNullOrEmpty(pair.value)
                    && (pair.value.Contains("{app_name}") || pair.value.Contains("{app_name_upper}")) )
                {
                    changedText = Regex.Replace(changedText, "{app_name}", productName, RegexOptions.None);
                    changedText = Regex.Replace(changedText, "{app_name_upper}", productName.ToUpper(), RegexOptions.None);
                    pair.value = changedText;
                }

                changedText = pair.value;
                if (!string.IsNullOrEmpty(pair.value)
                    && (pair.value.Contains("{vip_lounge_name}")
                     || pair.value.Contains("{vip_lounge_name_upper}")
                     ))
                {
                    changedText = Regex.Replace(changedText, "{vip_lounge_name}", vipLoungeName, RegexOptions.None);
                    changedText = Regex.Replace(changedText, "{vip_lounge_name_upper}", vipLoungeName.ToUpper(), RegexOptions.None);
                    pair.value = changedText;
                }

                changedText = pair.value;
                if (!string.IsNullOrEmpty(pair.value)
                    && (pair.value.Contains("{vip_lounge_badge_name}")
                     || pair.value.Contains("{vip_lounge_badge_name_lower}")
                     || pair.value.Contains("{vip_lounge_badge_name_upper}")
                     || pair.value.Contains("{vip_lounge_badge_name_plural}")
                     || pair.value.Contains("{vip_lounge_badge_name_plural_lower}")
                     || pair.value.Contains("{vip_lounge_badge_name_plural_upper}")
                     ))
                {
                    changedText = Regex.Replace(changedText, "{vip_lounge_badge_name}", vipBadgeName, RegexOptions.None);
                    changedText = Regex.Replace(changedText, "{vip_lounge_badge_name_lower}", vipBadgeName.ToLower(), RegexOptions.None);
                    changedText = Regex.Replace(changedText, "{vip_lounge_badge_name_upper}", vipBadgeName.ToUpper(), RegexOptions.None);
                    changedText = Regex.Replace(changedText, "{vip_lounge_badge_name_plural}", $"{vipBadgeName}s", RegexOptions.None);
                    changedText = Regex.Replace(changedText, "{vip_lounge_badge_name_plural_lower}", $"{vipBadgeName.ToLower()}s", RegexOptions.None);
                    changedText = Regex.Replace(changedText, "{vip_lounge_badge_name_plural_upper}", $"{vipBadgeName.ToUpper()}S", RegexOptions.None);
                    pair.value = changedText;
                }

                changedText = pair.value;
                if (!string.IsNullOrEmpty(pair.value)
                    && (pair.value.Contains("{vip_lounge_meta_game_name}")
                     || pair.value.Contains("{vip_lounge_meta_game_name_lower}")
                     || pair.value.Contains("{vip_lounge_meta_game_name_upper}")
                     ))
                {
                    changedText = Regex.Replace(changedText, "{vip_lounge_meta_game_name}", vipMetaGameName, RegexOptions.None);
                    changedText = Regex.Replace(changedText, "{vip_lounge_meta_game_name_lower}", vipMetaGameName.ToLower(), RegexOptions.None);
                    changedText = Regex.Replace(changedText, "{vip_lounge_meta_game_name_upper}", vipMetaGameName.ToUpper(), RegexOptions.None);
                    pair.value = changedText;
                }

                changedText = pair.value;
                if (!string.IsNullOrEmpty(pair.value)
                    && (pair.value.Contains("{vip_lounge_badge_tmp_name}")
                     ))
                {
                    changedText = Regex.Replace(changedText, "{vip_lounge_badge_tmp_name}", vipBadgeTMPName, RegexOptions.None);
                    pair.value = changedText;
                }
            }

            Debug.Log("EnvironmentStringTableObject : Finish prod environment table refine");
        }
#endif
    }
}
