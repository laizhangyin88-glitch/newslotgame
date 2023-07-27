using UnityEngine;
using System;
using System.Collections.Generic;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion;
using ParadoxNotion.Design;

#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Blackboard")]
public class SetEnum : ActionTask<Blackboard>
{
    public BBParameter<string> valueA;
    public BBObjectParameter valueB = new BBObjectParameter(typeof(System.Enum));

    protected override string info
    {
        get { return valueA + " = " + valueB; }
    }

    protected override void OnExecute()
    {
        var variableA = BlackboardUtils.FindVariable(agent, valueA.value);
        if (variableA == null)
        {
            Debug.LogError("[Blackboard](" + agent.name + ") Null variable founded in " + valueA.value);
        }
        else 
        {
            variableA.value = (int)valueB.value;
        }

        EndAction();
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
