using System;
using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Club")]

public class GetClubNewsfeedType : ActionTask<Blackboard>
{
    public BBParameter<int> tabIndex;
    public BBParameter<ClubFeedFilterType> saveAsFilterType;
    public BBParameter<bool> isEnableRequest;

    protected override string info
    {
        get
        {
            return string.Format("{0} = Get Club Newsfeed Filter Type({1})", saveAsFilterType, tabIndex);
        }
    }

    protected override void OnExecute()
    {
        switch(tabIndex.value)
        {
            case 0:
                saveAsFilterType.value = ClubFeedFilterType.ALL;
                break;
            case 1:
                saveAsFilterType.value = ClubFeedFilterType.MESSAGES;
                break;
            case 2:
                saveAsFilterType.value = ClubFeedFilterType.EPICS;
                break;
            case 3:
                // if(isEnableRequest.value)
                    saveAsFilterType.value = ClubFeedFilterType.REQUESTS;
                // else
                //     saveAsFilterType.value = ClubFeedFilterType.MESSAGES;
                break;
            default:
                saveAsFilterType.value = ClubFeedFilterType.ALL;
                break;
        }

        EndAction();
    }
}

}
