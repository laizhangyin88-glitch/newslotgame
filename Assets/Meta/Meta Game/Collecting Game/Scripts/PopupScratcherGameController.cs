using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using UnityEngine;

namespace BagelCode
{
    public class PopupScratcherGameController : MonoBehaviour
    {
        private void OnDestroy()
        {
            MetaSystem.UnSubscribeBackButton(gameObject.GetHashCode());
        }

        private void Start()
        {
            MetaSystem.SubscribeBackButton(gameObject.GetHashCode(),
                () => EventSender.SendGlobalEvent("OnScratcherNextStep"));
        }
    }
}