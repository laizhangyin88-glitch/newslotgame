using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Symbol")]
public class SymbolSetActiveCachingObject : ActionTask<BaseSymbol>
{
    public BBParameter<int> cachingId;
    public enum SetActiveMode
	{
		Deactivate = 0,
		Activate   = 1,
		Toggle     = 2
	}
	public SetActiveMode setTo = SetActiveMode.Toggle;

    protected override string info { get { return string.Format("Cache({0}).{1}", cachingId, setTo); } }

    protected override void OnExecute()
    {
        var gameObject = agent.GetComponent<SymbolController>().cachingObjects[cachingId.value];
        if (gameObject == null)
        {
            EndAction();
            return;
        }

        bool value;

		if (setTo == SetActiveMode.Toggle)
			value = !gameObject.activeSelf;
		else
			value = (int)setTo == 1;

		gameObject.SetActive(value);

        EndAction();
    }
}

}
