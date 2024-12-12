using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class WifiParams: BaseParamsWithPrefab
{

}

public class WifiListOSA : OSA<WifiParams, WifiItemController>
{
    public List<string> Data = new List<string>();

    protected override void Start()
    {
        base.Start();
    }


    protected override WifiItemController CreateViewsHolder(int itemIndex)
    {
        var instance = new WifiItemController();
        instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
      
        return instance;
    }

    protected override void UpdateViewsHolder(WifiItemController newOrRecycled)
    {
        string childAdapterModel = Data[newOrRecycled.ItemIndex];
        newOrRecycled.UpdateViews(childAdapterModel);
    }
}
