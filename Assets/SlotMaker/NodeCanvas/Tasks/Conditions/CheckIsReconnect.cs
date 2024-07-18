using NodeCanvas.Framework;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Tasks.Condition
{
    public class CheckIsReconnect : ConditionTask
    {
        protected override bool OnCheck()
        {
            var bb = ContentBlackboard.Get();
            var gameNew = BlackboardUtils.GetOrCreateBlackboard(bb, "gameNew");
            var temp = gameNew.GetVariable<JSONNode>("ConnectData");
            if(temp != null)
            {
                if(temp.value != null)
                {
                    Debug.LogError("enter reconnect............................");
                    return true;
                }
            }
            return false;
        }
    }
}

