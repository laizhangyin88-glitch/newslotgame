using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SlotMaker;

#if DEV
using TMPro;
#endif

namespace BagelCode
{

public class SlotImageDLCController : MonoBehaviour
{
    public GameObject loadingObject;
    public GameObject backgroundObject;
    public Transform iconArea;
    public GameObject iconObject = null;

    private string bundleName;
    private string assetName;

    private bool isDownloaded = false;
    private bool useBackground = false;

    private List<AssetBundleLoadOperation> loadOperationList = new List<AssetBundleLoadOperation>();
    private bool isError = false;

#if DEV
    private TextMeshProUGUI assetNameText = null;

    public void SetDebugText(string text)
    {
        if( assetNameText == null )
        {
            var go = new GameObject();
            go.transform.parent = transform;
            go.transform.localPosition = new Vector3(10f, 0f, 0f);
            go.transform.localScale = new Vector3(1f, 1f, 1f);
            go.name = "Asset Name";
            assetNameText = go.AddComponent<TextMeshProUGUI>();
            assetNameText.fontSize = 34;
            assetNameText.color = new Color(0, 255, 255);
        }

        assetNameText.text = text + "(Anim)";
        if(isDownloaded)
            assetNameText.gameObject.SetActive(false);
    }
#endif

    public void SetSlotImage(string targetBundleName, string targetAssetName, bool useDefaultIcon)
    {
        bundleName = targetBundleName;
        assetName = targetAssetName;
        useBackground = useDefaultIcon;

        LoadOperation();
    }

    private void LoadOperation()
    {
        ActiveImage(false);
        ActiveLoading(true);
        isDownloaded = false;

        if(isError)
            loadOperationList.Clear();

        AssetBundleManager.AddDLC(bundleName);
        loadOperationList.AddRange(AssetBundleManager.LoadDependencies(bundleName));
        loadOperationList.Add(AssetBundleManager.LoadAssetBundle(bundleName));

        isError = false;
    }

    private void OnEnable()
    {
        if(isDownloaded)
        {
            if(iconObject == null)
                LoadIcon();
        }
        else
        {
            if(isError)
                LoadOperation();
        }
    }

    private void Update()
    {
        if(!isDownloaded && !isError)
        {
            bool allDone = true;
            for (int i = 0; i < loadOperationList.Count; ++i)
            {
                if (!loadOperationList[i].IsDone())
                    allDone = false;
                else
                {
                    string error = loadOperationList[i].GetError();
                    if (!string.IsNullOrEmpty(error))
                    {
                        isError = true;
                        return;
                    }
                }
            }

            isDownloaded = allDone;

            if(isDownloaded)
            {
                BlackboardQueryUtils.AddUsingMetaAssetBundle(bundleName);

                if(gameObject.activeInHierarchy)
                    LoadIcon();
            }
        }
    }

    private void ActiveLoading(bool isActive)
    {
        if( loadingObject != null )
            loadingObject.SetActive(isActive);
    }

    private void ActiveImage(bool isActive)
    {
        if( backgroundObject != null )
            backgroundObject.SetActive(useBackground && !isActive );

        if( iconArea != null )
            iconArea.gameObject.SetActive(isActive);
    }

    private void ActiveException()
    {
        ActiveLoading(false);
        ActiveImage(false);
        if( backgroundObject != null )
            backgroundObject.SetActive(true);
    }

    private void LoadIcon()
    {
        if(gameObject == null || iconArea == null) return;

        // Make Icon
        try
        {
            iconObject = MetaObjectUtils.MakePrefab(bundleName, assetName, iconArea, "");
            ActiveLoading(false);
            ActiveImage(true);
        }
        catch(Exception e)
        {
#if DEV
            if (ApplicationSettings.LogTest())
                Debug.LogError( string.Format("Load Icon {0}, {1}", bundleName, assetName ));
#endif
        }
#if DEV
        if( assetNameText != null )
            assetNameText.gameObject.SetActive(false);
#endif
    }
}

}

