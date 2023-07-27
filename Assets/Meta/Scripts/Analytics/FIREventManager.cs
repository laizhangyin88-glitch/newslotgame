using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Services;
using SlotMaker;
using SlotMaker.Json;

namespace BagelCode
{
    public class FIREventManager : MonoWeakSingleton<FIREventManager>
    {
        private static Dictionary<string, string> eventTokenDict = new Dictionary<string, string>()
        {
            {"level_up:30", "level_up_30"},
            {"level_up:40", "level_up_40"},
            {"level_up:50", "level_up_50"}
        };

        public void SendEvent(string eventID)
        {
            if(eventTokenDict.ContainsKey(eventID))
                NativeHelper.Instance.SendFIREvent(eventTokenDict[eventID]);
        }
    }
}
