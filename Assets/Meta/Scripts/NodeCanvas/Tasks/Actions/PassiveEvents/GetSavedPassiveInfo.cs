using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions 
{

[Category("★ BagelCode/PassiveEvents")]
public class GetSavedPassiveInfo : ActionTask<Blackboard> 
{
    public BBParameter<Blackboard> saveAs;

    private const string SAVE_EVENT_INFO = "saveEventInfo";

    protected override string info
    {
        get{ return string.Format("{0} = Get Saved Passive Info", saveAs); }
    }

    protected override void OnExecute () 
    {
        Variable variable = MainBlackboard.Get().GetVariable(SAVE_EVENT_INFO, typeof(Blackboard));
        if (variable != null)
        {
            saveAs.value = MainBlackboard.Get().GetValue<Blackboard>(SAVE_EVENT_INFO);
        }
        

        EndAction(true);
    }
}

}
