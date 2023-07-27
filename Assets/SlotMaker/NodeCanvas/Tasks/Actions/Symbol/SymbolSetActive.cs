using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Symbol")]
public class SymbolSetActive : ActionTask
{
    public BBParameter<GameObject> gameObject;
    public enum SetActiveMode
	{
		Deactivate = 0,
		Activate   = 1,
		Toggle     = 2
	}
	public SetActiveMode setTo = SetActiveMode.Toggle;

    protected override string info { get { return string.Format("{0}.{1}", gameObject, setTo); } }

    protected override void OnExecute()
    {
        bool value;

		if (setTo == SetActiveMode.Toggle)
			value = !gameObject.value.activeSelf;
		else
			value = (int)setTo == 1;

		gameObject.value.SetActive(value);

        EndAction();
    }
}

}
