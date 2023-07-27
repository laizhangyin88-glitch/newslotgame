using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/NativeHelper")]
public class QuitApplication : ActionTask
{
    protected override string info { get { return "Quit Application"; } }

    protected override void OnExecute()
    {
        NativeHelper.Instance.OnApplicationExit();
        
        EndAction();
    }
}

}
