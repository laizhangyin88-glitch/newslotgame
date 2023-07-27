using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Services;
using NodeCanvas.Framework;

namespace SlotMaker
{
    public class MetaUIEventManager : MonoBehaviour
    {
        
        private const string ON_META_UI_EVENT = "OnMetaUIEvent";

        public List<StringGraphOwnerPairVariable> eventList = new List<StringGraphOwnerPairVariable>();
        private StringBuilder eventNameBuilder = new StringBuilder("OnEnd");
        private const string EVENT_INPUT_PREFIX = "OnBegin";
        private const string EVENT_OUTPUT_PREFIX = "OnEnd";

        private void OnEnable()
        {
            MessageDispatcher.Register(ON_META_UI_EVENT, Dispatch);
        }

        private void OnDisable()
        {
            MessageDispatcher.UnRegister(ON_META_UI_EVENT, Dispatch);
        }
        
        public void Dispatch(EventData eventData) {
            bool isTriggered = false;

            foreach (var e in eventList)
            {
                if (eventData.name.Equals(e.key)) 
                {
                    isTriggered = true;
                    e.value.StartBehaviour();
                }
            }

            if (!isTriggered && eventData.name.Contains(EVENT_INPUT_PREFIX)) 
            {
                eventNameBuilder.Remove(0, eventNameBuilder.Length);
                eventNameBuilder.Append(eventData.name);
                eventNameBuilder.Replace(EVENT_INPUT_PREFIX, EVENT_OUTPUT_PREFIX);

                MessageDispatcher.Dispatch(ON_META_UI_EVENT, new EventData(eventNameBuilder.ToString()));
            }
        }
    }
}