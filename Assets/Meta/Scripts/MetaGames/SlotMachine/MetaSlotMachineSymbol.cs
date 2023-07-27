using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using SlotMaker;

namespace BagelCode
{
    public class MetaSlotMachineSymbol : Symbol
    {
        public UnitySymbolEvent onStop;

        public override void Initialize()
        {
            int stripIndex = ((MetaSlotMachineReel)reel).metaStrip.CalcIndex(reel.index + physicalRow);
            Change(MetaGameUtils.GetSymbol(slotMachine.slotIndex, reel.reelIndex, ((MetaSlotMachineReel)reel).metaStrip, stripIndex));
            Apply();
        }

        public override void Initialize(BaseReel reel, int column, int row, int stripIndex)
        {
            this.slotMachine = reel.slotMachine;
            this.reel = reel;
            this.column = column;
            this.row = row;

            Change(MetaGameUtils.GetSymbol(slotMachine.slotIndex, reel.reelIndex, ((MetaSlotMachineReel)reel).metaStrip, stripIndex));
            Apply();
        }

        public virtual void Stop()
        {
            OnStop();
        }

        protected virtual void OnStop()
        {
            onStop.Invoke(this);
        }
    }
}