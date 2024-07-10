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
    private List<string> urls;

    protected override void Start()
    {
        base.Start();
        urls = MainBlackboard.Get().GetValue<List<string>>("slotADurls");
        ResetItems(urls.Count);
        currentIndex = 0;
    }

    protected override void Update()
    {
        base.Update();
        time += UnityEngine.Time.deltaTime;
        if (time > 5)
        {
            time = 0;
            currentIndex = currentIndex + 1 >= urls.Count ? 0 : currentIndex + 1;
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
        //newOrRecycled.image.sprite = sprites[newOrRecycled.ItemIndex];
        newOrRecycled.webImageController.SetWebImage(urls[newOrRecycled.ItemIndex], false);
    }

    public void OnClickAD()
    {
        Application.OpenURL(urls[currentIndex]);
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
