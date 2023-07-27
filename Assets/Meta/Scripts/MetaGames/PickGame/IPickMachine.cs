using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public interface IPickMachine
    {
        void Initialize();
        void InitDeck(int resultIndex, int withoutMinKind);
        void BeginPick();
        void EndPick();
        void OpenWithoutSymbols();
        void Pick(BasePickSymbol symbol);
        void Win();
        void OpenRemainSymbols();
        bool IsEnablePick();
        void Clear();
    }

    public interface IPickSymbol
    {
        void Initialize(BasePickMachine pickMachine);
        void Ready();
        void Pick();
        void Open(PickSymbolInfo symbolInfo);
        void Opened(PickSymbolInfo symbolInfo);
        void Win();
        void Disable(PickSymbolInfo symbolInfo);
    }

}