using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Symbol")]
public class SymbolSetSpriteWithRowCount : ActionTask<BaseSymbol>
{
    public BBParameter<SpriteRenderer> spriteRenderer;
    public BBParameter<int> spriteId;

    protected override string info { get { return string.Format("{0}.sprite = {1} + (rowCount - 1)", spriteRenderer, spriteId); } }

    protected override void OnExecute()
    {
        var info = agent.symbolInfo;

        spriteRenderer.value.sprite = agent.symbolAssets.GetSprite(agent.symbolInfo.symbol, spriteId.value + (info.link.rowCount -1));

        EndAction();
    }
}

}
