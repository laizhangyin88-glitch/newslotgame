using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace SlotMaker.Tasks.Actions
{

[Category("★ BagelCode/Scene")]
public class LoadContentPrefab : ActionTask
{
    public BBParameter<string> assetName;
    public BBParameter<Transform> parent;
    public BBParameter<string> parentName;
    public BBParameter<string> uniqueName;

    [BlackboardOnly]
    public BBParameter<GameObject> saveAs;

    protected override string info
    {
        get { return string.Format("{0} = LoadPrefab({1})", saveAs, assetName); }
    }

    protected override void OnExecute()
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

        saveAs.value = go;
        EndAction();
    }

    protected string GetBundleName()
    {
        return BlackboardUtils.FindVariable<string>(null, "./game/gameTitle").value;
    }
}

}
