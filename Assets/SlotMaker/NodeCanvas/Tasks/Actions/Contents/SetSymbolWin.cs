using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class SetSymbolWin : ActionTask<Blackboard>
{
    public BBParameter<SymbolWin> symbolWin;

    // Save As
    [BlackboardOnly]
    public BBParameter<long> earnCredit;
    [BlackboardOnly]
    public BBParameter<long> multiplier;
    [BlackboardOnly]
    public BBParameter<int>  lineIndex;
    [BlackboardOnly]
    public BBParameter<int>  symbolIndex;
    [BlackboardOnly]
    public BBParameter<List<Cell>> cells;
    [BlackboardOnly]
    public BBParameter<int> hitCount;
    [BlackboardOnly]
    public BBParameter<int> wayCount;
    [BlackboardOnly]
    public BBParameter<int> direction;

    protected override void OnExecute()
    {
        earnCredit.value  = symbolWin.value.earnCredit;
        multiplier.value  = symbolWin.value.multiplier;
        lineIndex.value   = symbolWin.value.lineIndex == null ? 0 : (int)symbolWin.value.lineIndex;
        symbolIndex.value = symbolWin.value.symbolIndex;
        cells.value       = symbolWin.value.cells;
        hitCount.value    = symbolWin.value.hitCount;
        wayCount.value    = symbolWin.value.wayCount == null ? 0 : (int)symbolWin.value.wayCount;
        direction.value   = symbolWin.value.direction;

        EndAction();
    }
}

[Category("★ BagelCode/Contents")]
[Description("param1 = earnCredit / wayCount / multiplier\nparam2 = earnCredit / multiplier\nparam3 = earnCredit / wayCount")]
public class SetSymbolWinForSignboard : ActionTask<Blackboard>
{
    public BBParameter<SymbolWin> symbolWin;

    protected override void OnExecute()
    {
        if (symbolWin.value.earnCredit <= 0)
        {
            EndAction(false);
            return;
        }
        
        IBlackboard bb = agent.GetComponent<IBlackboard>();

        int wayCount = symbolWin.value.wayCount == null ? 1 : (int)symbolWin.value.wayCount;

        BlackboardUtils.SetOrCreateValue<long>(bb, "param1", (long)(symbolWin.value.earnCredit / wayCount / symbolWin.value.multiplier));
        BlackboardUtils.SetOrCreateValue<long>(bb, "param2", (long)(symbolWin.value.earnCredit / symbolWin.value.multiplier));
        BlackboardUtils.SetOrCreateValue<long>(bb, "param3", (long)(symbolWin.value.earnCredit / wayCount));
        EndAction();
    }
}

[Category("★ BagelCode/Contents")]
public class SetSymbolWinFromCells : ActionTask<Blackboard>
{
    public BBParameter<long> earnCredit;
    public BBParameter<long> multiplier;
    public BBParameter<int> lineIndex;
    public BBParameter<int> symbolIndex;
    public BBParameter<List<Cell>> cells;
    public BBParameter<int> wayCount;
    public BBParameter<int> direction;

    public BBParameter<SymbolWin> saveAs;

    protected override void OnExecute()
    {
            
        var symbolWin = new SymbolWin();

        symbolWin.earnCredit = earnCredit == null ? 0 : earnCredit.value;
        symbolWin.multiplier = multiplier == null ? 1 : multiplier.value;
        symbolWin.lineIndex = lineIndex == null ? 0 : lineIndex.value;
        symbolWin.symbolIndex = symbolIndex == null ? 0 : symbolIndex.value;
        symbolWin.cells = cells == null ? new List<Cell>() : cells.value;
        symbolWin.hitCount = cells == null ? 0 : cells.value.Count;
        symbolWin.wayCount = wayCount == null ? 0 : wayCount.value;
        symbolWin.direction = direction == null ? 0 : direction.value;

        saveAs.value = symbolWin;
        EndAction();
    }
}

}
