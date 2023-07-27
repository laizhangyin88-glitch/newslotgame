using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SlotMaker;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BagelCode
{
    [CreateAssetMenu(fileName="AdjustEventToken", menuName="Meta/Environment/AdjustEventToken")]
    public class AdjustEventToken : ScriptableObjectSingleton<AdjustEventToken>
    {
        [ShowInInspector]
        public Dictionary_string_string eventTokenDict = new Dictionary_string_string();

        public Dictionary<string, string> GetAdjustTokens()
        {
            return eventTokenDict;
        }
    }
}