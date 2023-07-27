using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class BasePickSymbol : MonoBehaviour, IPickSymbol
    {
        public int valueIndex;
        public bool isReady = false;
        public BasePickMachine pickMachine;
        public PickSymbolInfo symbolInfo;

        public void Initialize(BasePickMachine pickMachine)
        {
            this.pickMachine = pickMachine;
            isReady = false;
            symbolInfo = null;
        }

        public void Ready()
        {
            isReady = true;

            OnReady();
        }

        public void Pick()
        {
            if(!isReady) return;
            if(pickMachine != null)
                pickMachine.Pick(this);
            OnPick();
        }

        public void Open(PickSymbolInfo symbolInfo)
        {
            this.symbolInfo = symbolInfo;
            valueIndex = symbolInfo.index;
            OnOpen();
        }

        public void Opened(PickSymbolInfo symbolInfo)
        {
            this.symbolInfo = symbolInfo;
            valueIndex = symbolInfo.index;
            OnOpened();
        }

        public void Win()
        {
            OnWin();
        }

        public void Disable(PickSymbolInfo symbolInfo)
        {
            this.symbolInfo = symbolInfo;
            valueIndex = symbolInfo.index;
            OnDisable();
        }

        protected virtual void OnReady(){}
        protected virtual void OnPick(){}
        protected virtual void OnOpen(){}
        protected virtual void OnOpened(){}
        protected virtual void OnWin(){}
        protected virtual void OnDisable(){}
    }
}

