using System;
using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/EarlyAccess")]

public class GetEarlyAccessThumbnailIcons : ActionTask<Blackboard>
{
    public BBParameter<List<string>> gameTitleList;

    protected override string info
    {
        get { return "Get Early Access Thumbnail Icons"; }
    }

    protected override void OnExecute()
    {
        gameTitleList.value = new List<string>();

        List<Blackboard> earlyAccessGameInfoList = BlackboardQueryUtils.GetEarlyAccessGameInfoList();
        
        if(earlyAccessGameInfoList != null && earlyAccessGameInfoList.Count >= 3)
        {
            for(int i=0; i<earlyAccessGameInfoList.Count; ++i)
            {
                var gameTitle = BlackboardUtils.FindVariable<string>(earlyAccessGameInfoList[i], "gameTitle");
                gameTitleList.value.Add(gameTitle.value);
            }

        }

        EndAction();
    }
}

}
