using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{
[Category("★ BagelCode/SlotMachine")]
public class PlayPivotSymbolAnimationByCell : ActionTask<BaseSlotMachine>
{
    public BBParameter<Cell>  cell;
    public BBParameter<string>  animationName;

    protected override string info
    {
        get { return string.Format("Play PivotSymbol {0} animation", animationName); }
    }

    protected override void OnExecute()
    {
        agent.GetPivotSymbol(cell.value.column, cell.value.row).Play(animationName.value);
        EndAction();
    }
}

}
