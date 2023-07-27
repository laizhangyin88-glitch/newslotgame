using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Symbol")]
public class SymbolSetScale : ActionTask<BaseSymbol>
{

    protected override string info { get { return string.Format("SymbolSetScale"); } }

    protected override void OnExecute()
    {
        float multiplier = (float)agent.symbolInfo.link.rowCount;
        if (agent.isPivot && !agent.unitSymbol)
        {
            agent.anchor.localScale = new Vector3(multiplier, multiplier, 1);
            agent.anchor.localPosition = CalcPivotPosition(multiplier);
        }
        else
        {
            agent.anchor.localScale = new Vector3(1, 1, 1);
            agent.anchor.localPosition = new Vector3(0, 0, 0);
        }

        EndAction();
    }

    private Vector3 CalcPivotPosition(float multiplier)
    {
        var reel = agent.reel;
        float x = (reel.cellSize.x + reel.spacing.x) * (multiplier - 1) / 2;
        float y = (reel.cellSize.y + reel.spacing.y) * (multiplier - 1) / 2;
        return new Vector3(x, y, agent.anchor.localPosition.z);
    }
}

}
