using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace SlotMaker.Tasks.Actions
{

[Category("★ BagelCode/Popup")]
public class SimpleOpenContentPopup : ActionTask
{
    public BBParameter<GameObject> popup;

	protected override string info
    {
        get { return "Open " + popup; }
    }

	protected override void OnExecute()
    {
        var bb = popup.value.GetComponent<Blackboard>();
        if (bb != null)
        {
            var variable = BlackboardUtils.GetOrCreateVariable<GameObject>(bb, "caller");
            variable.value = agent.gameObject;
        }

    	PopupManager.Instance.Open(popup.value);
    	EndAction();
    }
}

}
