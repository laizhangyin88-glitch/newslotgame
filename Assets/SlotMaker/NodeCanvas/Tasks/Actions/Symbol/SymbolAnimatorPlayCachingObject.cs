using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Symbol")]
public class SymbolAnimatorPlayCachingObject : ActionTask<BaseSymbol>
{
    public BBParameter<int> cachingId;
    public int stateNameHash;

    protected override string info { get { return string.Format("Cache({0}).Play({1})", cachingId, GlobalSymbolAssets._AnimatorHashToString(stateNameHash)); } }

    protected override void OnExecute()
    {
        var go = agent.GetComponent<SymbolController>().cachingObjects[cachingId.value];
        go.GetComponent<Animator>().Play(stateNameHash, -1, 0f);

        EndAction();
    }
}

}
