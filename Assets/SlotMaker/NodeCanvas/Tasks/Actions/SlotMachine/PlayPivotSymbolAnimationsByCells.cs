using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{
[Category("★ BagelCode/SlotMachine")]
public class PlayPivotSymbolAnimationsByCells : ActionTask<BaseSlotMachine>
{
    public BBParameter<List<Cell>> cells;
    public BBParameter<string>    animationName;

    protected override string info
    {
        get { return string.Format("Play PivotSymbol {0} animation", animationName); }
    }

    protected override void OnExecute()
    {
        for (int i = 0; i < cells.value.Count; ++i)
        {
            var cell = cells.value[i];
            agent.GetPivotSymbol(cell.column, cell.row).Play(animationName.value);
        }
        EndAction();
    }
}

}
