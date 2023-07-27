using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Scene")]
public class ReturnPooledObject : ActionTask<Blackboard>
{
    public BBParameter<List<GameObject>> pooledObjects;

    protected override string info
    {
        get { return string.Format("ReturnPooledObjects {0}", pooledObjects); }
    }

    protected override void OnExecute()
    {
        List<GameObject> objs = pooledObjects.value;
		for (int i = 0; i < objs.Count; ++i)
		{
            objs[i].GetComponent<PooledObject>().ReturnToPool();
		}
        pooledObjects.value.Clear();

        EndAction();
    }
}

}
