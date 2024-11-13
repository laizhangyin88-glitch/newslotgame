using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/ClientAPI")]
    
    public class RequestClubFeedCollectAll : ActionTask
    {
        public BBParameter<Blackboard> clubNewsFeedResponse;
        public BBParameter<ClubFeedFilterType> filterType;
        
        protected override string info
        { 
            get 
            { 
                return string.Format("Request Club Feed Collect All {0}", filterType);
            } 
        }

        protected override void OnExecute()
        {
            BagelCodeClientAPI.ClubFeedCollectAllRequest(filterType.value, GetMaxFeedId(), 
                (response) =>
                {
                    BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
                    BlackboardQueryUtils.ApplyUserSyncInfo();
                    
                    EndAction(true);
                },
                (error) =>
                {
                    Debug.Log("Error : " + error.errorCode.ToString());
                    GlobalErrorHandler.GlobalError(error);
                });

        }

        private long GetMaxFeedId()
        {
            List<Blackboard> feedList = clubNewsFeedResponse.value.GetVariable<List<Blackboard>>("feedList").value;

            long maxId = 0;
            for (int i = 0; i < feedList.Count; i++)
            {
                long id = feedList[i].GetValue<long>("id");
                if (id > maxId)
                {
                    maxId = id;
                }
            }

            return maxId;

        }
    }
}