using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{
    [Category("★ BagelCode/BI")]
    public class BI_friend_suggestion_collect_edit_earn_coin : ActionTask<Blackboard>
    {
        public BBParameter<long> earnCoin;
        public BBParameter<int> requestCount;
        public BBParameter<int> beforeFriendCount;
        public BBParameter<string> type;

        protected override void OnExecute()
        {
            Analytics.CustomEvent("client_friend_suggestion_collect", new Dictionary<string, object>
            {
                { "earn_coin", earnCoin.value },
                { "friend_request_count", requestCount.value },
                { "friend_count", beforeFriendCount.value },
                { "type", type.value }
            });

            EndAction();
        }
    }
}