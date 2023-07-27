using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/GameObject")]
public class SetActive : ActionTask<Component>
{
	public enum SetActiveMode
	{
		Deactivate = 0,
		Activate   = 1,
		Toggle     = 2
	}
	public SetActiveMode setTo = SetActiveMode.Toggle;

	protected override string info
	{
		get	{ return string.Format("{0}.{1}", agentInfo, setTo); }
	}

	protected override void OnExecute()
	{
		bool value;

		if (setTo == SetActiveMode.Toggle)
			value = !agent.gameObject.activeSelf;
		else
			value = (int)setTo == 1;

		agent.gameObject.SetActive(value);
		EndAction();
	}
}

}
