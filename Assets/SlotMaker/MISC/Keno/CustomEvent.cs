using System;
using UnityEngine;
using UnityEngine.Events;

namespace SlotMaker.Keno.Events
{
    [Serializable]
    public class KenoEvent : UnityEvent<KenoInstance, GameObject> {}

    [Serializable]
    public class SpotEvent : UnityEvent<SpotInstance, GameObject> {}

    [Serializable]
    public class BallEvent : UnityEvent<BallInstance, GameObject> {}

    [Serializable]
    public abstract class CustomEvent<T, Event>
        where Event : UnityEvent<T, GameObject>
    {
        public string eventName;
        public Event onEvent;

        public void OnEvent(T eventData, GameObject agent)
        {
            onEvent.Invoke(eventData, agent);
        }
    }

    [Serializable]
    public class CustomSpotEvent : CustomEvent<SpotInstance, SpotEvent> {}

    [Serializable]
    public class CustomBallEvent : CustomEvent<BallInstance, BallEvent> {}
}
