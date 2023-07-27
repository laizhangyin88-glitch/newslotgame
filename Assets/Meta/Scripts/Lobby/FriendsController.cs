using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FriendsController : EventMonoBehaviour
    {
        public void Init()
        {
            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);
            Register(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_BLOCKED_USER_CHANGED, OnUpdatedFriendList);
        }

        private void OnUpdatedFriendList()
        {
            EventSender.SendEvent(gameObject, "RefreshFriend");
        }
    }
}
