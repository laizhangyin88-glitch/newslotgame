using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using SlotMaker;

namespace BagelCode
{
    public class ReferrerDetailInfo
    {
        public string referrerUrl = null;

        // utm_source=??/utm_medium=??/utm_medium=??/utm_term=??/utm_content=?? etc...
        private Dictionary<string, string> splitData = null;

        public ReferrerDetailInfo(string url)
        {
            if(string.IsNullOrEmpty(url)) return;

            referrerUrl = url;

            splitData = new Dictionary<string, string>();

            string[] datas = referrerUrl.Split('&');
            foreach(string data in datas)
            {
                string[] infos = data.Split('=');

                if(infos.Length == 2)
                {
                    if(!splitData.ContainsKey(infos[0]))
                    {
                        splitData[infos[0]] = infos[1];
                    }
                }
            }
#if DEV
            TestLog();
#endif
        }

        public string GetData(string key)
        {
            if(splitData == null) return null;

            if(!splitData.ContainsKey(key)) return null;

            return splitData[key];
        }

#if DEV
        public void TestLog()
        {
            if (!ApplicationSettings.LogTest()) return;

            Debug.LogError(referrerUrl);

            foreach( KeyValuePair<string, string> data in splitData)
            {
                Debug.LogError( string.Format("{0} : {1}", data.Key, data.Value) );
            }
        }
#endif
    }

    public static partial class BiEventUtils
    {
        public static void SendClientInstall(string referrerUrl)
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();

#if UNITY_ANDROID && !PLATFORM_AMAZON && !UNITY_EDITOR
            ReferrerDetailInfo info = new ReferrerDetailInfo(referrerUrl);

            customData["utm_source"]    = info.GetData("utm_source");
            customData["utm_medium"]    = info.GetData("utm_medium");
            customData["utm_term"]      = info.GetData("utm_term");
            customData["utm_content"]   = info.GetData("utm_content");
            customData["utm_campaign"]  = info.GetData("utm_campaign");
            customData["gclid"]         = info.GetData("gclid");
#else
            customData["utm_source"] = null;
            customData["utm_medium"] = null;
            customData["utm_term"] = null;
            customData["utm_content"] = null;
            customData["utm_campaign"] = null;
            customData["gclid"] = null;
#endif

            Analytics.CustomEvent("client_install", customData);
        }
    }
}
