using NodeCanvas.Framework;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Tasks.Actions.Contents
{
    public class ClearConnectData : ActionTask
    {
        protected override void OnExecute()
        {
            var bb = ContentBlackboard.Get();
            var gameNew = BlackboardUtils.GetOrCreateBlackboard(bb, "gameNew"); 
            BlackboardUtils.SetOrCreateValue<JSONNode>(gameNew, "ConnectData", null);
            EndAction();
        }
    }
}
