using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SlotMaker;
using ParadoxNotion;
using ParadoxNotion.Services;
using NodeCanvas.Framework;

namespace BagelCode
{
    [RequireComponent(typeof(MetaUIEventDispatcher))]
    public class MetaUIEventSubscriber : MonoBehaviour
    {
        [System.Serializable]
        public class MetaUISubscribeInfo
        {
            public string eventKey;
            public UnityEvent action;
        }

        private const string ON_META_UI_EVENT = "OnMetaUIEvent";

        public List<MetaUISubscribeInfo> actionList;

        protected virtual void OnEnable()
        {
            SlotMaker.MessageDispatcher.Register(ON_META_UI_EVENT, Dispatch);
        }

        protected virtual void OnDisable()
        {
            SlotMaker.MessageDispatcher.UnRegister(ON_META_UI_EVENT, Dispatch);
        }

        public void Dispatch(EventData eventData)
        {
            foreach(MetaUISubscribeInfo info in actionList)
            {
                if(info.eventKey == eventData.name)
                {
                    info.action.Invoke();
                    break;
                }
            }
        }
    }
}