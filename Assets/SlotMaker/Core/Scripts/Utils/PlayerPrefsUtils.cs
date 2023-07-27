using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    public static class PlayerPrefsUtils
    {
        public static long GetOrCreateInt64(string key, long defaultValue = 0L)
        {
            if (PlayerPrefs.HasKey(key))
            {
                return Convert.ToInt64(PlayerPrefs.GetString(key));
            }
            else 
            {
                PlayerPrefs.SetString(key, defaultValue.ToString());
                return defaultValue;
            }
        }

        public static long GetInt64(string key, long defaultValue = 0L)
        {
            return Convert.ToInt64(PlayerPrefs.GetString(key, defaultValue.ToString()));
        }

        public static void SetInt64(string key, long value)
        {
            PlayerPrefs.SetString(key, value.ToString());
        }
    }
}
