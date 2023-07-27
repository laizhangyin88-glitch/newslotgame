using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/ClientAPI")]
    
    public class RequestInboxCollectAll : ActionTask
    {
        protected override string info
        { 
            get 
            { 
                return "Request Inbox Collect All";
            } 
        }

        protected override void OnExecute()
        {
            int maxId = BlackboardQueryUtils.GetMaxInboxId();
            
            BagelCodeClientAPI.InboxCollectAllRequest(maxId, 
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
    }
}