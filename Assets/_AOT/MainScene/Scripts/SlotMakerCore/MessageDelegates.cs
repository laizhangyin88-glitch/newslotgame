using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;

namespace SlotMaker
{
    public class MessageDelegates
    {
        private Dictionary<string, MessageDispatcher.EventDelegate> delegates;

        public MessageDelegates(Dictionary<string, MessageDispatcher.EventDelegate> delegates)
        {
            this.delegates = delegates;
        }

        public void Delegate(EventData eventData)
        {
            MessageDispatcher.EventDelegate del;
            if (delegates.TryGetValue(eventData.name, out del))
                del.Invoke(eventData);
        }
    }
}
