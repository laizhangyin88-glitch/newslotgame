using UnityEngine;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion;
using ParadoxNotion.Design;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SlotMaker.Tasks.Condition
{

[Category("★ SlotMaker/Blackboard")]
public class CheckEnum : ConditionTask<Blackboard> 
{	
	public BBParameter<string> valueA;
	public BBObjectParameter valueB = new BBObjectParameter(typeof(System.Enum));

	protected override string info
	{
		get { return valueA + " == " + valueB; }
	}

	protected override bool OnCheck()
	{
		var variableA = BlackboardUtils.FindVariable(agent, valueA.value);
        if (variableA == null)
        {
            Debug.LogError("[Blackboard](" + agent.name + ") Null variable founded in " + valueA.value);
            return false;
        }
        else 
        {
            return (int)variableA.value == (int)valueB.value;
        }
	}

	////////////////////////////////////////
	///////////GUI AND EDITOR STUFF/////////
	////////////////////////////////////////
	#if UNITY_EDITOR
	
	protected override void OnTaskInspectorGUI()
	{
		DrawDefaultInspector();
		if (GUILayout.Button("Select Type"))
		{
			EditorUtils.ShowPreferedTypesSelectionMenu(typeof(System.Enum), (t) => { valueB.SetType(t); });
		}
	}

	#endif
}

}