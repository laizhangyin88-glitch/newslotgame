using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NodeCanvas;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;

namespace BagelCode
{
    public class MetaPickMachine : BasePickMachine
    {
        public UnityEvent onSelectPick;
        public UnityEvent onEndPick;

        protected override void OnPick(BasePickSymbol symbol)
        {
            onSelectPick.Invoke();
        }

        protected override void OnEndPick()
        {
            onEndPick.Invoke();
        }
    }
}
