using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class OSA_JackpotRecord : OSA<BaseParamsWithPrefab, JackpotRecordItemViewsHolder>
{
    [SerializeField]
    private List<Sprite> bgSprites;
    [SerializeField]
    private List<Sprite> titleSprites;


    protected override void Start()
    {

        base.Start();
        ResetItems(6);
    }

    protected override JackpotRecordItemViewsHolder CreateViewsHolder(int itemIndex)
    {
        var instance = new JackpotRecordItemViewsHolder();

        instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);

        return instance;
    }


    protected override void UpdateViewsHolder(JackpotRecordItemViewsHolder newOrRecycled)
    {

    }


}



public class JackpotRecordItemViewsHolder : BaseItemViewsHolder
{
    public Image BG;
    public TextMeshProUGUI rank;
    public TextMeshProUGUI id;
    public Image title;
    public TextMeshProUGUI time;
    public TextMeshProUGUI bounus;

    public override void CollectViews()
    {
        base.CollectViews();
        BG = root.Find("BG").GetComponent<Image>();
        rank = root.Find("Rank").GetComponent<TextMeshProUGUI>();
        id = root.Find("Id").GetComponent<TextMeshProUGUI>();
        title = root.Find("TitleImg").GetComponent<Image>();
        time = root.Find("Time").GetComponent<TextMeshProUGUI>();
        bounus = root.Find("Bounus").GetComponent<TextMeshProUGUI>();
    }
}
