using Com.TheFallenGames.OSA.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OSA_JackpotNum : OSA<BaseParams, JackpotNumItem>
{
    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    private List<Sprite> sprites;

    protected override void Start()
    {
        base.Start();

        ResetItems(10);
    }

    protected override JackpotNumItem CreateViewsHolder(int itemIndex)
    {
        var item = new JackpotNumItem();
        item.Init(prefab, itemIndex);
        prefab.SetActive(false);
        return item;
    }

    protected override void UpdateViewsHolder(JackpotNumItem newOrRecycled)
    {
        newOrRecycled.image.sprite = sprites[newOrRecycled.ItemIndex];
    }

    public void TestClick()
    {
        ResetItems(10);
        SmoothScrollTo(0, 0.5f, .5f, .5f);
    }
}

public class JackpotNumItem : BaseItemViewsHolder
{
    public Image image;

    public override void CollectViews()
    {
        base.CollectViews();
        image = root.GetComponent<Image>();
    }
}
