using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Conditions
{

[Category("★ SlotMaker/Symbol")]
public class SymbolCheckCustomDataKey : ConditionTask<BaseSymbol>
{
    public BBParameter<string> key = "";

    protected override string info 
    { 
        get { return string.Format("symbolInfo.customData[{0}] exists", key); } 
    }

    protected override bool OnCheck()
    {
        return (agent.symbolInfo.customData == null) ? false : agent.symbolInfo.customData.ContainsKey(key.value);
    }
}

}