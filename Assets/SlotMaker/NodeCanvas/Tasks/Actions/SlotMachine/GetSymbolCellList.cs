using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/SlotMachine")]
public class GetSymbolCellList : ActionTask<Blackboard>
{
    public BBParameter<SymbolWin> symbolWin;

    public BBParameter<List<Cell>> saveAs;

    protected override string info
    {
        get { return string.Format("Get Cell List from SymbolWin({0}) save as {1}", symbolWin, saveAs); }
    }

    protected override void OnExecute()
    {
        saveAs.value = symbolWin.value.cells;
        EndAction();
    }
}

}
