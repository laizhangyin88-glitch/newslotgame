using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Scene")]
public class GetPooledObject : ActionTask<Blackboard>
{
	public BBParameter<ObjectPool> pool;
    public BBParameter<Transform> parent;
    public BBParameter<bool> activeSelf = false;

    [BlackboardOnly]
    public BBParameter<GameObject> saveAs;

    protected override string info
    {
        get { return string.Format("{0} = GetPooledObject({1})", saveAs, pool); }
    }

    protected override void OnExecute()
    {
		var po = pool.value.GetObject(activeSelf.value);
		var go = po.gameObject;
		go.transform.SetParent(parent.value, false);
		saveAs.value = go;

        EndAction();
    }
}

}
