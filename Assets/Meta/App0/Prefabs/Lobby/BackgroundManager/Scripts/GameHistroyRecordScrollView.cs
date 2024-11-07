using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameHistroyRecordParams : BaseParamsWithPrefab
{
    public string sn;
}

public class GameHistroyRecordItemData
{
    public int sn;
    public long bet;
    public long total_win;
    public long end_cent;
    public long init_cent;
    public long start_time;
    public long game_time;
    public int win_line_count;
    public long total_bet;
    public int game_id;
}

public class GameHistroyRecordScrollView : OSA<GameHistroyRecordParams, GameHistroyRecordItem>
{
    public List<GameHistroyRecordItemData> itemDataList;
    protected override GameHistroyRecordItem CreateViewsHolder(int itemIndex)
    {
        var instance = new GameHistroyRecordItem();
        instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
        return instance;
    }

    protected override void UpdateViewsHolder(GameHistroyRecordItem newOrRecycled)
    {
        GameHistroyRecordItemData data = this.itemDataList[newOrRecycled.ItemIndex];
        newOrRecycled.UpdateView(data);
    }
}
