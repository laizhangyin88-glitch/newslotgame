using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/NativeHelper")]
public class CopyClipboard : ActionTask
{
    public BBParameter<string> str;

    protected override string info { get { return string.Format("copy {0} to Clipboard", str); } }

    protected override void OnExecute()
    {
        NativeHelper.Instance.CopyClipboard(str.value);
        
        EndAction();
    }
}

}
