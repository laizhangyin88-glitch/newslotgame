using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Symbol")]
public class SymbolSetSortingOrder : ActionTask
{
    public BBParameter<SpriteRenderer> spriteRenderer;
    public int sortingLayerId;
    public int sortingOrder;

    protected override string info { get { return string.Format("{0}.SetSortingOrder({1}, {2})", spriteRenderer, SortingLayer.IDToName(sortingLayerId), sortingOrder); } }

    protected override void OnExecute()
    {
        spriteRenderer.value.sortingLayerID = sortingLayerId;
        spriteRenderer.value.sortingOrder = sortingOrder;

        EndAction();
    }
}

}
