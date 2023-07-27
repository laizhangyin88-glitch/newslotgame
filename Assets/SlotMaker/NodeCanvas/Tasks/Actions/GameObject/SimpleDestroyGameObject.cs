using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/GameObject")]
public class SimpleDestroyGameObject : ActionTask
{
	public BBParameter<string> target;

	protected override string info
	{
		get { return string.Format("Destroy {0}", target); }
	}

	protected override void OnExecute()
	{
		GameObject.Destroy(GameObject.Find(target.value));
		EndAction();
	}
}

}
