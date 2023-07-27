using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using SlotMaker;

namespace BagelCode
{
    [RequireComponent(typeof(PooledObject))]
    public class MetaSlotMachineReel : Reel
	{
        public BaseReelStrip _metaStrip;
        public BaseReelStrip metaStrip
        {
            get { return _metaStrip ?? MetaSlotMachineGlobalReelStrips.Instance.GetReelStrips().GetReelStrip(reelIndex); }
            set { _metaStrip = value; }
        }

        public override void Shuffle()
        {
            index = metaStrip.GetRandomIndex();
        }

        public override void Shuffle(int reelIndex)
        {
            index = metaStrip.CalcIndex(reelIndex);
        }

        public override void PushFrontSymbol()
        {
            index = metaStrip.CalcIndex(--index);

            var frontSymbol = symbols[0];
            var symbol = slotMachine.CreateSymbol();

            symbol.transform.SetParent(symbolsTransform, false);
            symbol.transform.SetAsFirstSibling();
            symbol.Initialize(this, frontSymbol.column, frontSymbol.row - 1, index);
            UpdateFrontSymbolTransform(symbol);
            symbols.Insert(0, symbol);
        }

        public override void PopFrontSymbols(int count)
        {
            index = metaStrip.CalcIndex(index + count);

            for (int i = 0; i < count; ++i)
            {
                var symbol = symbols[0];
                symbol.Clear();
                symbols.RemoveAt(0);
            }
        }

        public override void PushBackSymbol()
        {
            int backIndex = metaStrip.CalcIndex(index + symbols.Count);

            var backSymbol = symbols[symbols.Count - 1];
            var symbol = slotMachine.CreateSymbol();

            symbol.transform.SetParent(symbolsTransform, false);
            symbol.transform.SetAsLastSibling();
            symbol.Initialize(this, backSymbol.column, backSymbol.row + 1, backIndex);
            UpdateBackSymbolTransform(symbol);
            symbols.Add(symbol);
        }
    }
}