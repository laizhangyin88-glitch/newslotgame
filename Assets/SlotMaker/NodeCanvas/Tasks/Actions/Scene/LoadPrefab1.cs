using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Scene")]
public class LoadPrefab1 : ActionTask
{
    public BBParameter<string> bundleName;
    public BBParameter<string> assetName;
    public BBParameter<bool> combineApplicationType;
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
        if(prefab != null)
        {
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
        }
        
        EndAction();
    }

    protected string GetBundleName()
    {
        return combineApplicationType.value ? ApplicationSettings.MakeApplicationBundleName(bundleName.value) : bundleName.value;
    }
}

}
