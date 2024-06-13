using ParadoxNotion;
using ParadoxNotion.Services;
using UnityEngine;

namespace SlotMaker
{
    [RequireComponent(typeof(MessageRouter))]
    public class MetaUIEventDispatcher : MonoBehaviour
    {
        private const string ON_META_UI_EVENT = "OnMetaUIEvent";

        private MessageRouter _router = null;

        protected MessageRouter router
        { get { return _router ?? (_router = GetComponent<MessageRouter>()); } }

        private void OnEnable()
        {
            MessageDispatcher.Register(ON_META_UI_EVENT, Dispatch);
        }

        private void OnDisable()
        {
            MessageDispatcher.UnRegister(ON_META_UI_EVENT, Dispatch);
        }

        public void Dispatch(EventData eventData)
        {
            router.Dispatch(ON_META_UI_EVENT, eventData);
        }
    }
}
