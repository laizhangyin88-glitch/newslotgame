using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace SlotMaker.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
public class GetSymbolPosition : ActionTask<BaseSlotMachine>
{
    public BBParameter<Cell> cell;
    public BBParameter<int> column;
    public BBParameter<int> row;
    public bool isWorld = true;

    public BBParameter<Vector3> saveAs;

    protected override string info
    {
        get { return string.Format("Get {0},{1} Position",column,row); }
    }

    protected override void OnExecute()
    {
        var reel = agent.GetReel(column.value);

        Vector3 symbolPosition = Vector3.zero;

        if (cell.isNone)
        {
            symbolPosition = reel.CalcSymbolPosition(reel.beginColumn, reel.beginRow, column.value, row.value);
        }
        else
        {
            symbolPosition = reel.CalcSymbolPosition(reel.beginColumn, reel.beginRow, cell.value.column, cell.value.row);
        }

        if (isWorld)
        {
            symbolPosition = reel.rectTransform.TransformPoint(symbolPosition);
        }

        saveAs.value = symbolPosition;
        EndAction();
    }
}

}
