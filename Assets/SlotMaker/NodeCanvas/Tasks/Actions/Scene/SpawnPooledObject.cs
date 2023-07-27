using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Scene")]
public class SpawnPooledObject : ActionTask<Blackboard>
{
    public BBParameter<ObjectPool> pool;
    public BBParameter<Transform> parent;
	public BBParameter<int> spawnCount;

    [BlackboardOnly]
    public BBParameter<List<GameObject>> saveAs;

    protected override string info
    {
        get { return string.Format("{0} = SpawnPooledObject({1}, {2})", saveAs, pool, spawnCount); }
    }

    protected override void OnExecute()
    {
		saveAs.value = new List<GameObject>();

		for (int i = 0; i < spawnCount.value; ++i)
		{
			var po = pool.value.GetObject();
			var go = po.gameObject;
			go.transform.SetParent(parent.value, false);
			saveAs.value.Add(go);
		}

        EndAction();
    }
}

}
