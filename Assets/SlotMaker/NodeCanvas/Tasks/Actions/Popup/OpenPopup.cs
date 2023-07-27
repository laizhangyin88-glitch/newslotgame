using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Task.Actions
{

[Category("★ SlotMaker/Popup")]
public class OpenPopup : ActionTask 
{
	public BBParameter<GameObject> popup;

	protected override string info
    {
        get { return "Open " + popup; }
    }

	protected override void OnExecute()
    {
    	PopupManager.Instance.Open(popup.value);
    	EndAction();
    }
}

}
