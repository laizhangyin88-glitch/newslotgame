using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Task.Action
{

[Category("★ BagelCode/Meta/Sounds")]
public class RemoveAllMetaSoundsEvent : ActionTask<Blackboard>
{
    private const string ON_META_UI_EVENT = "OnMetaUIEvent";

    protected override string info
    {
        get
        {
            return "Send Global Meta Event (Remove All Meta Sounds";
        }
    }

    protected override void OnExecute()
    {
        MessageDispatcher.Dispatch(ON_META_UI_EVENT, new EventData("OnRemoveAllSounds"));

        EndAction();
    }
}

}
