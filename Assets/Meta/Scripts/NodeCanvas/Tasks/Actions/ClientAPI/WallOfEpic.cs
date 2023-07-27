using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class WallOfEpic : ActionTask
{
    protected override string info { get { return "Request WallOfEpic"; } }

    protected override void OnExecute()
    {
        BagelCodeClientAPI.WallOfEpicList(
        (response) =>
        {
            BlackboardUtils.DestroyBlackboardList(MainBlackboard.Get(), "wallOfEpic");
            var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "wallOfEpic");

            ClientAPI2Blackboard.Serialize(bb, response);

            if(response.recordList != null && response.recordList.Count > 0)
            {
                PlayerPrefs.SetInt("PREVIEW_WALL_OF_EPIC_ID", response.recordList[0].id);
            }

            EndAction(true);
        },
        (error) =>
        {
            GlobalErrorHandler.GlobalError(error);
        });
    }
}

}
