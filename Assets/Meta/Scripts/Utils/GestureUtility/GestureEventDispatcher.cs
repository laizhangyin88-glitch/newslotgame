using SlotMaker;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Services;

namespace BagelCode
{
    [RequireComponent(typeof(MessageRouter))]
    public class GestureEventDispatcher : MonoBehaviour
    {
        private MessageRouter _router = null;
        protected MessageRouter router { get { return _router ?? (_router = GetComponent<MessageRouter>()); } }

        private void OnEnable()
        {
            MessageDispatcher.Register(GestureManager.ON_GESTURE_EVENT, Dispatch);
        }

        private void OnDisable()
        {
            MessageDispatcher.UnRegister(GestureManager.ON_GESTURE_EVENT, Dispatch);
        }

        public void Dispatch(EventData eventData)
        {
            router.Dispatch(GestureManager.ON_GESTURE_EVENT, eventData);
        }
    }
}
