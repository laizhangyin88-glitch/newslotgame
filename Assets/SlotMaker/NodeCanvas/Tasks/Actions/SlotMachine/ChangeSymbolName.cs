using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/SlotMachine")]
public class ChangeSymbolName : ActionTask
{
    public BBParameter<int> symbolIndex;
    public BBParameter<string> symbolName;

    protected override string info { get { return string.Format("ChangeSymbolName({0}, {1})", symbolIndex, symbolName); } }

    protected override void OnExecute()
    {
        ContentCustomData.Instance.symbolName[symbolIndex.value] = symbolName.value;

        EndAction();
    }
}

}
