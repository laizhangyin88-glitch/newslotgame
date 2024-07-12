using BagelCode;
using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;
using SlotMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OSA_SlotAD : OSA<SlotADParams, SlotAD>
{
    private float time = 0;
    private int currentIndex;
    private List<ADSData> adsDatas;
    private float showTime = 5;

    protected override void Start()
    {
        base.Start();
        adsDatas = MainBlackboard.Get().GetValue<List<ADSData>>("slotADurls");
        ResetItems(adsDatas.Count);
        currentIndex = 0;
    }

    protected override void Update()
    {
        base.Update();
        time += UnityEngine.Time.deltaTime;
        if (time > showTime)
        {
            time = 0;
            currentIndex = currentIndex + 1 >= adsDatas.Count ? 0 : currentIndex + 1;
            SmoothScrollTo(currentIndex, 1, .5f, .5f);
        }
    }

    protected override SlotAD CreateViewsHolder(int itemIndex)
    {
        var item = new SlotAD();
        item.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
        return item;
    }

    protected override void UpdateViewsHolder(SlotAD newOrRecycled)
    {
        newOrRecycled.webImageController.SetWebImage(adsDatas[newOrRecycled.ItemIndex].imageUrl, false);
        showTime = adsDatas[newOrRecycled.ItemIndex].showTime == 0 ? 5 : adsDatas[newOrRecycled.ItemIndex].showTime;
    }

    public void OnClickAD()
    {
        Application.OpenURL(adsDatas[currentIndex].linkUrl);
    }
}

[Serializable]
public class SlotADParams : BaseParamsWithPrefab
{
   
}

public class SlotAD : BaseItemViewsHolder
{
    public Image image;
    public WebImageController webImageController;

    public override void CollectViews()
    {
        base.CollectViews();
        image = root.GetComponent<Image>();
        webImageController = root.GetComponent<WebImageController>();
    }
}

public class ADSData
{
    public string imageUrl;
    public string linkUrl;
    public float showTime;
}
