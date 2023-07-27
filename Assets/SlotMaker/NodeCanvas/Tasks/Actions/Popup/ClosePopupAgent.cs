using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Task.Actions
{

[Category("★ SlotMaker/Popup")]
public class ClosePopupAgent : ActionTask<Transform>
{
    protected override void OnExecute()
    {
        if(agent != null)
        {
            PopupManager.Instance.Close(agent.gameObject);
        }
        else
        {
            PopupManager.Instance.Close();
        }
        
        EndAction();
    }
}

}
