using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using BagelCode;
using ParadoxNotion;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Scene")]
public class GotoFreeGame : ActionTask
{
    public BBParameter<string> id;
    //[BlackboardOnly]
    //public BBParameter<GameObject> saveAs;

    protected override string info
    {
        get 
        {
                return "goto last free game";
        }
    }

    protected override void OnExecute()
    {
            Variable<int> lastFreeGameID = BlackboardUtils.GetOrCreateVariable<int>(null, id.value);
            BlackboardQueryUtils.SetEnterGameInfo(lastFreeGameID.value, "EnterGame", "default", null, 0, null, false);
            var e = new EventData("OnEnterGame", 0);
            Graph.SendGlobalEvent(e, this);
            EndAction();
    }

}

}
