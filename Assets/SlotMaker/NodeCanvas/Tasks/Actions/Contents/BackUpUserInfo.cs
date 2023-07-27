using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BlackboardQuery
{

[Category("★ BagelCode/Me")]
public class BackUpUserInfo : ActionTask
{
    protected override string info
    {
        get { return "Backup User Info"; }
    }

    protected override void OnExecute()
    {
        MetaSystem.BackupUserSyncInfo();

        EndAction();
    }
}

}
