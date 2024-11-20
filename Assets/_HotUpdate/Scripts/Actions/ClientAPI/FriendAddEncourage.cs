using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;


namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class FriendAddEncourage : ActionTask <Blackboard> 
{
    public BBParameter<List<string>> targetUserIdList;
    public BBParameter<int> savedBeforeFriendCount;

    protected override string info 
    {
        get 
        {
            return "Request Suggested Friends";
        }
    }

    protected override void OnExecute()
    {
        BagelCodeClientAPI.FriendAddEncourage(targetUserIdList.value,
        (response) =>
        {
            savedBeforeFriendCount.value = BlackboardQueryUtils.GetFriendList(true).Count;

            var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "friendAddEncourageListResponse");

            ClientAPI2Blackboard.Serialize(bb, response);
            BlackboardQueryUtils.AddCoins(response.earnCredit);

            List<Blackboard> friendList = BlackboardUtils.GetOrCreateBlackboardList(bb, "friendInfoList");

            for (int i = 0; i < friendList.Count; ++i)
            {
                string userId = friendList[i].GetValue<string>("userId");
                if (BlackboardQueryUtils.IsContainsUser(userId, BlackboardQueryUtils.GetFriendList(false)))
                {
                    BlackboardQueryUtils.UpdateFriendItemAsAccepted(userId);
                }
                else
                {
                    BlackboardUtils.AddToBlackboardList(MainBlackboard.Get(), "friendList", friendList[i]);    
                }
            }

            var friendEncourageList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), "friendEncourageList");
            int friendListCount = friendEncourageList.value.Count;

            for(int i = 0; i < friendListCount; ++i)
            {
                BlackboardUtils.RemoveAtBlackboardList(MainBlackboard.Get(), "friendEncourageList", 0);
            }

            BlackboardQueryUtils.UpdateCelebInfo();

            EndAction(true);
            
        },
        (error) =>
        {
            switch(error.errorCode)
            {
                default:
                    GlobalErrorHandler.GlobalError(error);
                break;
            }
        });
    }
}

}
