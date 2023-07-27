using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Scene")]
public class LoadPrefabFromStringList : ActionTask<Transform> 
{
    public BBParameter<string> bundleName;
    [RequiredField] [BlackboardOnly]
    public BBParameter<List<string>> targetList;
    public BBParameter<int> index;
    public BBParameter<bool> combineApplicationType;
    public BBParameter<string> parentName;
    public BBParameter<string> uniqueName;

    [BlackboardOnly]
    public BBParameter<GameObject> saveAs;
    
    protected AssetBundleLoadAssetOperation loadPrefabInfoOperation = null;

    protected override string info
    {
        get 
        { 
            string assetName = "";
            if (targetList != null && targetList.value != null && index != null)
                assetName = GetAssetName();
                
            if (string.IsNullOrEmpty(parentName.value))
                return string.Format("{0} = {1}.LoadPrefab({2})", saveAs, agentInfo, assetName); 
            else 
                return string.Format("{0} = Find({1}).LoadPrefab({2})", saveAs, parentName, assetName); 
        }
    }
    
    protected override void OnExecute()
    {
        var assetName = GetAssetName();
        if (string.IsNullOrEmpty(assetName))
        {
            EndAction(false);
            return;
        }
        
        loadPrefabInfoOperation = AssetBundleManager.LoadAssetAsync<GameObject>(GetBundleName(), assetName);
    }
    
    protected override void OnUpdate()
    {
        if (!loadPrefabInfoOperation.IsDone())    
            return;
            
        var prefab = loadPrefabInfoOperation.GetAsset<GameObject>();
        GameObject go = GameObject.Instantiate(prefab) as GameObject;
        go.name = uniqueName.value;

        Transform parent = agent;
        if (!string.IsNullOrEmpty(parentName.value))
        {
            if (parent == null)
                parent = GameObject.Find(parentName.value).transform;
            else 
                parent = parent.Find(parentName.value);
        }
        if (parent != null)
            go.transform.SetParent(parent, false);

        saveAs.value = go;
        EndAction();
    }

    protected string GetBundleName()
    {
        return combineApplicationType.value ? ApplicationSettings.MakeApplicationBundleName(bundleName.value) : bundleName.value;
    }
    
    protected string GetAssetName()
    {
        return targetList.value[index.value];
    }
}

}
