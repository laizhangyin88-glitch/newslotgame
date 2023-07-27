using System;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using SlotMaker.Json;
using NodeCanvas.Framework;

namespace BagelCode
{
    public class UrlBuildUtils
    {
        public static string GetHelpCenterUrl(string supportPageUrl)
        {
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { "ClientVersion"      , ProductSettings.GetProductVersionNumber().ToString()                               },
                { "Device"             , ApplicationSettings.GetDeviceModel().Replace(",", "").Replace("|", "").Replace("\"", "") },
                { "OS"                 , ApplicationSettings.GetOperatingSystem()                                                 },
                { "Platform"           , GetPlatformName()                                                                        },
                { "UtcTimeOffset"      , TimeUtils.GetTimeZoneOffset()                                                            },
                { "Application"        , GetOrEmptyStringValue("appName").Replace(" ", "_")                                       },
                { "Country"            , GetOrEmptyStringValue("clientCountry")                                                   },
                { "TotalGameDepositUSD", (int)(GetOrZeroDoubleValue("me/lifetimeSpend") * 100)                                    },
                { "Email"              , GetOrEmptyStringValue("ssoAccountInfo/email")                                            },
                { "DivisionProfileId"  , GetOrEmptyStringValue("me/friendCode")                                                   }
            };

            string dataJson = SlotSimpleJson.SerializeObject(data); //= SlotSimpleJson
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(dataJson);
            string payload = System.Convert.ToBase64String(bytes);

            return supportPageUrl + "/?payload=" + payload;
        }

        // for apply form with Zendesk
        private static string GetPlatformName()
        {
            string platformName = "";

            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    platformName = "Android";
                    break;
                case RuntimePlatform.IPhonePlayer:
                    platformName = "Apple";
                    break;
                case RuntimePlatform.WebGLPlayer:
                    platformName = "Facebook";
                    break;
            }

            return platformName;
        }

        public static string GetOrEmptyStringValue(string name)
        {
            Variable variable = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), name);

            if (variable != null && variable.value != null)
            {
                return variable.value as string;
            }
            else
            {
                return string.Empty;
            }
        }

        public static double GetOrZeroDoubleValue(string name)
        {
            var variable = BlackboardUtils.FindVariable<double>(MainBlackboard.Get(), name);

            if (variable != null && variable.value != null)
            {
                return variable.value;
            }
            else
            {
                return 0d;
            }
        }
    }
}