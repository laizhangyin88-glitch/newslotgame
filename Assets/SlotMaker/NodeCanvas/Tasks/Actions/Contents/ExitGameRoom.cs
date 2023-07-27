using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class ExitGameRoom : ActionTask
{
    protected override string info
    {
        get { return string.Format("ExitGameRoom"); }
    }

    protected override void OnExecute()
    {
        EndAction(true);
    }
}

}
