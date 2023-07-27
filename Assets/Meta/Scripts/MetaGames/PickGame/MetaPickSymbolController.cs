using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;

namespace BagelCode
{
    public class MetaPickSymbolController : MonoBehaviour
    {
        public MetaPickSymbol symbol;
        public FSMOwner owner;

        private void Awake()
        {
            if(symbol == null)
                symbol = GetComponent<MetaPickSymbol>();
                
            if(owner == null)
                owner = GetComponent<FSMOwner>();
        }

        public void EnterState(string stateName)
        {
            owner.StopBehaviour();
            owner.StartBehaviour();
            owner.TriggerState(stateName);
        }
    }
}

