using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
[Description("Save symbolnames in customData(Blackboard) as 'symbolNames'")]
public class SetSymbolNames : ActionTask
{
    protected override void OnExecute()
    {
        int symbolCount = ContentCustomData.Instance.symbolCount;
        List<string> symbolNames = new List<string>();
        bool isError = true;
        for (int i = 0; i < symbolCount; ++i)
        {
            string key = string.Format("SYMBOL_NAME_{0}",i);
            string symbolName = StringTableUtils.GetString(
                StringTable.StringTableType.Content, key, out isError);
            symbolNames.Add(symbolName);
            if (isError) break;
        }

        Blackboard bb = ContentBlackboard.Get();
        Blackboard customBB = bb.GetValue<Blackboard>("customData");
        customBB.AddVariable("symbolNames", symbolNames);

        EndAction();
    }
}

}
