using System.Collections;
using UnityEngine;
using NodeCanvas;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion.Design;


namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Symbol")]
public class SymbolGetCustomData<T> : ActionTask<BaseSymbol>
{
    public BBParameter<string> key = "";
    [BlackboardOnly]
    public BBParameter<T> saveAs;

    protected override string info
	{
		get { return string.Format("{0} = Symbol Get CustomData({1})", saveAs, key); }
	}

    protected override void OnExecute()
    {
        if (agent.symbolInfo.customData.ContainsKey(key.value))
        {
            saveAs.value = (T)agent.symbolInfo.customData[key.value];
            EndAction();
        }
        else
        {
            Debug.LogError("[Symbol](" + agent.name + ") symbol customData dosen't have the key: \"" + key.value + "\"");
            EndAction(false);
        }
    }
}

}