using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

namespace SlotMaker
{

public class SendCustomEvent : MonoBehaviour
{
    public GraphOwner owner;
    public bool sendGlobal;
    
    public string eventName;

    public void SendEvent(string message)
    {
        if (sendGlobal)
            GraphOwner.SendGlobalEvent<string>(eventName, message);
        else 
            owner.SendEvent<string>(eventName, message);
    }
}

}
