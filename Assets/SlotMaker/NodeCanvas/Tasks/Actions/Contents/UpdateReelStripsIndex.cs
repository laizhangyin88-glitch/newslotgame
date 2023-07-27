using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class UpdateReelStripsIndex : ActionTask<Blackboard>
{
    public BBParameter<string> key;

    protected override void OnExecute()
    {
        var src = BlackboardUtils.FindVariable<Blackboard>(agent, key.value).value;
        var dst = BlackboardUtils.FindVariable<Blackboard>(null, "./game/reelSetIndex").value;
        dst.SetValue("currentIndex", src.GetValue<int>("currentIndex"));
        dst.SetValue("nextIndex", src.GetValue<int>("nextIndex"));

        EndAction();
    }
}

}
