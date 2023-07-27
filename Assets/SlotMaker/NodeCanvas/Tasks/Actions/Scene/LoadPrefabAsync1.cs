using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Scene")]
public class LoadPrefabAsync1 : ActionTask
{
    public BBParameter<string> bundleName;
    public BBParameter<string> assetName;
    public BBParameter<bool> combineApplicationType;
    public BBParameter<Transform> parent;
    public BBParameter<string> parentName;
    public BBParameter<string> uniqueName;

    [BlackboardOnly]
    public BBParameter<GameObject> saveAs;

    protected AssetBundleLoadAssetOperation loadPrefabInfoOperation = null;

    protected override string info
    {
        get 
        { 
            if (parent != null)
                return string.Format("{0} = {1}{2}.LoadPrefabAsync({3})", saveAs, parent, (parentName != null && !string.IsNullOrEmpty(parentName.value)) ? (".Find(" + parentName + ")") : "", assetName); 
            else 
                return string.Format("{0} = GameObject.Find({1}).LoadPrefabAsync({2})", saveAs, parentName, assetName); 
        }
    }

    protected override void OnExecute()
    {
        loadPrefabInfoOperation = null;
    }

    protected override void OnUpdate()
    {
        if (loadPrefabInfoOperation == null)
            loadPrefabInfoOperation = AssetBundleManager.LoadAssetAsync<GameObject>(GetBundleName(), assetName.value);

        if (loadPrefabInfoOperation.IsDone())
        {
            var prefab = loadPrefabInfoOperation.GetAsset<GameObject>();
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
    }

    protected string GetBundleName()
    {
        return combineApplicationType.value ? ApplicationSettings.MakeApplicationBundleName(bundleName.value) : bundleName.value;
    }
}

}
