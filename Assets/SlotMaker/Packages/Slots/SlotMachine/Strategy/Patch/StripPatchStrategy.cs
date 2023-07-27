using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Slots.Strategy
{
    [CreateAssetMenu(fileName="New Strip Patch Strategy", menuName="SlotMaker2/Slot/Strategy/Strip/Patch")]
    public class StripPatchStrategy : ScriptableObject
    {
        public virtual void CalcFrontPatch(List<SymbolStrip> strips, int srcIndex, int dstIndex, out int srcPatchCount, out int dstPatchCount)
        {
            srcPatchCount = 0;
            foreach (var strip in strips)
            {
                var symbol = strip.GetSymbol(srcIndex);
                srcPatchCount = Mathf.Max(symbol.rowOffset, srcPatchCount);
            }

            dstPatchCount = 0;
            foreach (var strip in strips)
            {
                var symbol = strip.GetSymbol(dstIndex);
                dstPatchCount = Mathf.Max(symbol.rowCount - symbol.rowOffset - 1, dstPatchCount);
            }
        }

        public virtual void CalcBackPatch(List<SymbolStrip> strips, int srcIndex, int dstIndex, out int srcPatchCount, out int dstPatchCount)
        {
            srcPatchCount = 0;
            foreach (var strip in strips)
            {
                var symbol = strip.GetSymbol(srcIndex);
                srcPatchCount = Mathf.Max(symbol.rowCount - symbol.rowOffset - 1, srcPatchCount);
            }

            dstPatchCount = 0;
            foreach (var strip in strips)
            {
                var symbol = strip.GetSymbol(dstIndex);
                dstPatchCount = Mathf.Max(symbol.rowOffset, dstPatchCount);
            }
        }
    }
}