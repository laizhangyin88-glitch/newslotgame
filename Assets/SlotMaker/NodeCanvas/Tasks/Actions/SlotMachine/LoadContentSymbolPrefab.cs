using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/SlotMachine")]
public class LoadContentSymbolPrefab : ActionTask
{
    public BBParameter<string> assetName;
    public BBParameter<Transform> parent;
    public BBParameter<string> parentName;
    public BBParameter<string> uniqueName;

    public BBParameter<GameObject> delegator;
    public BBParameter<GameObject> animator;

    protected override string info
    {
        get { return string.Format("{0} = LoadSymbolPrefab({1})", delegator, assetName); }
    }

    protected override void OnExecute()
    {
        if (delegator.value == null)
        {
            var prefab = AssetBundleManager.LoadAsset<GameObject>(GetBundleName(), assetName.value);
            GameObject go = GameObject.Instantiate(prefab) as GameObject;
            go.name = uniqueName.value;

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

            delegator.value = go;
        }
        else
        {
            delegator.value.SetActive(true);
        }

        if (!animator.isNone && animator != null && animator.value != null)
        {
            animator.value.SetActive(false);
        }

        EndAction();
    }

    protected string GetBundleName()
    {
        return BlackboardUtils.FindVariable<string>(null, "./game/gameTitle").value;
    }
}

}
