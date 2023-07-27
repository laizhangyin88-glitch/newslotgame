using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace BagelCode
{
    public class MetaPickSymbol : BasePickSymbol
    {
        public UnityEvent onReady;
        public UnityEvent onOpen;
        public UnityEvent onOpened;
        public UnityEvent onWin;
        public UnityEvent onDisable;

        protected override void OnReady()
        {
            onReady.Invoke();
        }

        protected override void OnOpen()
        {
            onOpen.Invoke();
        }

        protected override void OnOpened()
        {
            onOpened.Invoke();
        }

        protected override void OnWin()
        {
            if(symbolInfo.isWin)
                onWin.Invoke();
            else
                onDisable.Invoke();
        }

        protected override void OnDisable()
        {
            onDisable.Invoke();
        }
    }
}
