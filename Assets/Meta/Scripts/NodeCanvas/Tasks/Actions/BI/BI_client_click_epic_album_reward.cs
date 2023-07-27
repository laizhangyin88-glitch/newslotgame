using System;
using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Tasks.Actions.BI
{

    [Category("★ BagelCode/BI")]
    public class BI_client_click_epic_album_reward : ActionTask<Blackboard>
    {
        public BBParameter<int> categoryId;
        
        public BBParameter<int> currentRewardCount;

        public BBParameter<List<Blackboard>> rewardResultBBList;
        
        protected override void OnExecute()
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();

            customData["category_id"] = categoryId.value;

            customData["nth_reward"] = currentRewardCount.value;
            customData["max_reward_count"] = BlackboardQueryUtils.GetEpicAlbumEntryLimit();

            if(rewardResultBBList != null && rewardResultBBList.value != null && rewardResultBBList.value.Count >0)
                BiEventUtils.AppendCommonRewardEventData(customData, rewardResultBBList.value[0]);

            // foreach(var pair in customData)
            // {
            //     UnityEngine.Debug.LogError(string.Format("{0} : {1}", pair.Key, pair.Value));
            // }
            
            Analytics.CustomEvent("client_click_epic_album_reward", customData);

            EndAction();
        }
    }

}
