using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Chat
{
    [RequireComponent(typeof(ContextElement))]
    public class ChatBadge : MonoBehaviour
    {
        public List<ChannelType> listenChannelTypes = new List<ChannelType>();
        private ContextElement contextElement;

        private bool isInit = false;

        private void Start()
        {
            contextElement = GetComponent<ContextElement>();
            isInit = true;
            
            RefreshCount();
        }

        private void OnEnable()
        {
            if(ChatMetaManager.Instance != null)
                ChatMetaManager.Instance.SubscribeMessageChangeListener(RefreshCount);

            RefreshCount();
        }

        private void OnDisable()
        {
            if(ChatMetaManager.Instance != null)
                ChatMetaManager.Instance.UnsubscribeMessageChangeListener(RefreshCount);
        }

        private void RefreshCount()
        {
            if(!isInit) return;

            int sum = 0;
            foreach (var type in listenChannelTypes)
            {
                if (type == ChannelType.None) continue;
                if (ChatMetaManager.Instance!=null && !ChatMetaManager.Instance.IsValidate(type)) continue;

                sum += ChatMetaManager.Instance.GetUnreadMessageCount(type);
            }

            MetaObjectUtils.UpdateBadge(contextElement, false, sum);
        }
    }
}
