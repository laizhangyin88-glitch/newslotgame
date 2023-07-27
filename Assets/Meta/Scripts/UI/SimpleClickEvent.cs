using System.Collections;
using System.Collections.Generic;
using BagelCode;
using SlotMaker;
using UnityEngine;
using static SlotMaker.StringTable;

namespace Meta.Scripts.UI
{
    public class SimpleClickEvent : MonoBehaviour
    {
        public string eventName;
        public bool isGlobal;

        private void Start()
        {
            var context = GetComponent<ContextElement>();
            MetaContextElementUtils.SetClickable(context, gameObject, eventName, isGlobal, true);
        }
    }
}
