using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace SlotMaker
{
    [RequireComponent(typeof(RectTransform))]
    public class BaseSymbol : MonoBehaviour, ISymbol
    {
        public int column { get; set; }
        public int row { get; set; }

        public override int GetHashCode() { return Cell.GetHashCode(column, row); }

        public SymbolInfo symbolInfo;

        public int symbolIndex { get { return symbolInfo.symbol; } set { symbolInfo.symbol = value; } }

        public int symbolMultiplier { get { return symbolInfo.multiplier; } set { symbolInfo.multiplier = value; } }

        public int subSymbolIndex { get { return symbolInfo.subSymbol.symbol; } set { symbolInfo.subSymbol.symbol = value; } }

        public int symbolMask { get { return (int)symbolInfo.mask; } set { symbolInfo.mask = (SymbolAttribute)value; } }

        public bool isPivot { get { return symbolInfo.link.isPivot; } }

        public bool unitSymbol { get { return symbolInfo.link.unitSymbol; } }

        public int physicalRow { get { return row - reel.beginRow + reel.topBuffer; } }

        public int rowCount { get { return symbolInfo.link.rowCount; } }

        public int columnCount { get { return symbolInfo.link.columnCount; } }

        private RectTransform _rectTransform;
        public RectTransform rectTransform { get { return _rectTransform ?? (_rectTransform = GetComponent<RectTransform>()); } }

        public virtual RectTransform anchor { get { return rectTransform; } }

        public BaseSlotMachine slotMachine { get; set; }
        public BaseReel reel { get; set; }

        public SymbolAssets symbolAssets { get { return (reel != null) ? reel.symbolAssets : GlobalSymbolAssets.Instance; } }

        public virtual void Clear()
        {
            OnClear();
        }

        public virtual void Initialize()
        {
            int stripIndex = reel.strip.CalcIndex(reel.index + physicalRow);
            Change(SlotUtils.GetSymbol(slotMachine.slotIndex, reel.reelIndex, reel.strip, stripIndex));
            Apply();
        }

        public virtual void Initialize(BaseReel reel, BaseSymbol src)
        {
            this.slotMachine = reel.slotMachine;
            this.reel = reel;
            this.column = src.column;
            this.row = src.row;
            this.symbolInfo = (SymbolInfo)src.symbolInfo.Clone();

            Apply();
            Restore(src);
        }

        public virtual void Initialize(BaseReel reel, SymbolInfo newSymbol)
        {
            this.slotMachine = reel.slotMachine;
            this.reel = reel;

            Change((SymbolInfo)newSymbol.Clone());
            Apply();
        }

        public virtual void Initialize(BaseReel reel, int column, int row)
        {
            this.slotMachine = reel.slotMachine;
            this.reel = reel;
            this.column = column;
            this.row = row;

            Initialize();
        }

        public virtual void Initialize(BaseReel reel, int column, int row, int stripIndex)
        {
            this.slotMachine = reel.slotMachine;
            this.reel = reel;
            this.column = column;
            this.row = row;

            Change(SlotUtils.GetSymbol(slotMachine.slotIndex, reel.reelIndex, reel.strip, stripIndex));
            Apply();
        }

        public virtual void Change(SymbolInfo newSymbol)
        {
            symbolInfo = newSymbol;
            OnChange();
        }

        public virtual void Apply()
        {
            OnApply();
        }

        public virtual void Restore(BaseSymbol src)
        {
            OnRestore(src);
        }

        public virtual void Play(string animationName)
        {
            OnPlay(animationName);
        }

        public virtual void Skip()
        {
            OnSkip();
        }

        protected virtual void OnClear() { }
        protected virtual void OnChange() { }
        protected virtual void OnApply() { }
        protected virtual void OnRestore(BaseSymbol src) { }
        protected virtual void OnPlay(string animationName) { }
        protected virtual void OnSkip() { }
    }
}
