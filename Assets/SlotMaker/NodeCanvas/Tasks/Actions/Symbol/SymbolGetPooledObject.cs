using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Symbol")]
public class SymbolGetPooledObject : ActionTask<BaseSymbol>
{
    public BBParameter<int> poolId;
    public BBParameter<int> cachingId;

    protected override string info { get { return string.Format("Cache({0}) = GetPooledObject({1})", cachingId, poolId); } }

    protected override void OnExecute()
    {
        var symbolController = agent.GetComponent<SymbolController>();
        var go = symbolController.cachingObjects[cachingId.value];

        if (go != null)
        {
            var goID = go.GetComponent<GameObjectId>();
            if (goID != null && goID.id != poolId.value)
            {
                Object.Destroy(goID);
                go.GetComponent<PooledObject>().ReturnToPool();
                go = null;
            }
        }

        if (go == null)
        {
            go = agent.symbolAssets.GetPooledObject(poolId.value);
            go.AddComponent<GameObjectId>().id = poolId.value;
            go.transform.SetParent(agent.anchor, false);
            symbolController.cachingObjects[cachingId.value] = go;
        }
        else
        {
            if (go.activeSelf)
                go.SetActive(false);
            go.SetActive(true);
        }

        EndAction();
    }
}

}
