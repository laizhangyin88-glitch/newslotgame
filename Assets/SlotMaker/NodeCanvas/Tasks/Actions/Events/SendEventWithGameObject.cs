using UnityEngine;
using ParadoxNotion;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    public class SendEventWithGameObject : MonoBehaviour
    {
        public bool sendGlobal;
        public string eventName;
        [SerializeField] private EventType _eventType;

        private enum EventType
        {
            OnContentEvent,
            OnContentUIEvent,
            OnContentUIDetailEvent,
            OnSoundEvent,
            OnCreditEvent,
            OnSlotEvent,
            OnSlotDetailEvent,
            OnSpinButtonEvent,
            OnWinEvent,
            OnMetaUIEvent,
        }

        public void DispatchEvent(GameObject gameObjectToPass)
        {
            var eventData = new EventData<GameObject>(eventName, gameObjectToPass);

            if (sendGlobal)
            {
                MessageDispatcher.Dispatch(_eventType.ToString(), eventData);
            }
            else
            {
                GetComponent<ContentEventDispatcher>().Dispatch(eventData);
            }
        }
    }
}
