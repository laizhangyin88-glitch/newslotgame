using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Scene")]
public class GetPooledObject : ActionTask<Blackboard>
{
    public BBParameter<string> poolName;
    public BBParameter<Transform> parent;
    public BBParameter<string> parentName;
    public BBParameter<bool> activeSelf = false;

    [BlackboardOnly]
    public BBParameter<GameObject> saveAs;

    protected override string info
    {
        get { return string.Format("{0} = GetPooledObject({1})", saveAs, poolName); }
    }

    protected override void OnExecute()
    {
        var pool = BlackboardUtils.FindVariable<GameObject>(agent, poolName.value).value.GetComponent<ObjectPool>();
        var po = pool.GetObject(activeSelf.value);
        var go = po.gameObject;

        var _parent = parent.value;
        if (!string.IsNullOrEmpty(parentName.value))
        {
            if (_parent == null)
                _parent = GameObject.Find(parentName.value).transform;
            else
                _parent = _parent.Find(parentName.value);
        }
        if (_parent != null)
            go.transform.SetParent(_parent, false);

        saveAs.value = go;
        EndAction();
    }
}

}
