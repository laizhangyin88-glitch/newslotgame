using System.Collections;
using UnityEngine;
using ParadoxNotion;

namespace SlotMaker
{
    public class DispatchMessage : MonoBehaviour
    {
        public string eventName;

        public void Dispatch(string message)
        {
            MessageDispatcher.Dispatch(eventName, new EventData(message));
        }
    }
}
