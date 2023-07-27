using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Symbol")]
public class SymbolSetSortingGroupCachingObject : ActionTask<BaseSymbol>
{
    public BBParameter<int> cachingId;
    public int sortingLayerId;
    public int sortingOrder;

    protected override string info { get { return string.Format("Cache({0}).SetSortingGroupOrder({1}, {2})", cachingId, SortingLayer.IDToName(sortingLayerId), sortingOrder); } }

    protected override void OnExecute()
    {
        var go = agent.GetComponent<SymbolController>().cachingObjects[cachingId.value];
        var sortingGroup = go.GetComponent<SortingGroup>();
        sortingGroup.sortingLayerID = sortingLayerId;
        sortingGroup.sortingOrder = sortingOrder;

        EndAction();
    }
}

}
