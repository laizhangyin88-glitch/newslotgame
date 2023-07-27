using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{
[Category("★ SlotMaker/String")]
public class AccessStringFromClipboard : ActionTask<Blackboard>
{
    public enum AccessClipboardType {
        Get,
        Set
    };
    public AccessClipboardType accessType;
    public BBParameter<string> valueA;

    protected override string info
    {
        get { 
            if (accessType == AccessClipboardType.Get) {
                return string.Format("{0} = Clipboard", valueA); 
            } else if (accessType == AccessClipboardType.Set) {
                return string.Format("Clipboard = {0}", valueA); 
            }
            return "Clipboard";
        }
    }

    protected override void OnExecute()
    {
        var variableA = BlackboardUtils.GetOrCreateVariable<string>(agent, valueA.value);
        if (variableA == null)
        {
            Debug.LogError("[Blackboard] Null variableA founded in " + valueA.value + "in " + agent.name);
            EndAction(false);
        } else {
            if (accessType == AccessClipboardType.Get) {
                variableA.value = GUIUtility.systemCopyBuffer;
            } else if (accessType == AccessClipboardType.Set) {
                GUIUtility.systemCopyBuffer = variableA.value;
            }
            EndAction();
        }
    }
}

}
