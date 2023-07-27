using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Symbol")]
public class SymbolSetSpriteWithAssetGroup : ActionTask<BaseSymbol>
{
    public BBParameter<SpriteRenderer> spriteRenderer;
    public List<int> spriteIds;

    private int spriteId;

    protected override string info { get { return string.Format("{0}.sprite = AssetGroup({1})", spriteRenderer, spriteId); } }

    protected override void OnExecute()
    {
        spriteId = spriteIds[agent.symbolAssets.assetGroupId];
        spriteRenderer.value.sprite = agent.symbolAssets.GetSprite(agent.symbolInfo.symbol, spriteId);

        EndAction();
    }
}

}
