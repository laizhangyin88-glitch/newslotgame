using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/GameObject")]
public class DestroyGameObject : ActionTask 
{
	public BBParameter<GameObject> target;

	protected override string info
	{
		get { return string.Format("Destroy {0}", target); }
	}

	protected override void OnExecute()
	{
		Object.Destroy(target.value);
		EndAction();
	}
}

}
