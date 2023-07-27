using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BlackboardQuery
{

[Category("★ BagelCode/UserSyncInfo")]
public class ApplyUserSyncInfo : ActionTask
{
    protected override string info
    {
        get { return "Apply User Sync Info"; }
    }

    protected override void OnExecute()
    {
        MetaSystem.ApplyUserSyncInfo(true);

        EndAction();
    }
}

}
