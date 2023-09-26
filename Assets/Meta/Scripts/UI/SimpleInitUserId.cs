using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class SimpleInitUserId : MonoBehaviour
    {
        public ContextElement textElement;
        public IContextText property;

        void Start()
        {
            if (textElement != null)
                property = textElement as IContextText;
            string myId = BlackboardUtils.FindVariable<string>(null, "/me/userId")?.value ?? "";
            property.SetText(myId);
        }
    }
}
