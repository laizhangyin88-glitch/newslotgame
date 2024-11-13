using System;
using System.Collections.Generic;

namespace SlotMaker
{
    public class InternalEventRouter : Singleton<InternalEventRouter>
    {
        public Dictionary<string, Action<EventRouterData>> events = new Dictionary<string, Action<EventRouterData>>();

        public bool IsEmpty
        {
            get
            {
                return events.Count == 0;
            }
        }

        public void Register(string name, Action<EventRouterData> action)
        {
            if (!events.ContainsKey(name))
                events[name] = action;
            else
                events[name] += action;
        }

        public void Invoke(EventRouterData data)
        {
            if (events.ContainsKey(data.name))
            {
                events[data.name]?.Invoke(data);
            }
        }
    }

    public class EventRouterData
    {
        public string name { get; private set; }
        public string guid { get; private set; }
        public string objName { get; private set; }

        public EventRouterData(string name, IEventRouterDataObject dataObject)
        {
            this.name = name;

            if(dataObject != null)
            {
                this.guid = dataObject.GetGuid();
                this.objName = dataObject.GetObjectName();
            }
        }
    }

    public interface IEventRouterDataObject
    {
        string GetGuid();
        string GetObjectName();
    }
}
