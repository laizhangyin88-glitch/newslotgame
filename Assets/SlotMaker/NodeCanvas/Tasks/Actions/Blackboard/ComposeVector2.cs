using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace SlotMaker.Tasks.Actions{

[Category("★ SlotMaker/Blackboard")]
public class ComposeVector2 : ActionTask 
{
	public BBParameter<float> x;
	public BBParameter<float> y;
	[BlackboardOnly]
	public BBParameter<Vector2> saveAs;

	protected override string info
    {
		get {return "New Vector as " + saveAs;}
	}

	protected override void OnExecute()
    {
		saveAs.value = new Vector2(x.value, y.value);
		EndAction();
	}
}
}
