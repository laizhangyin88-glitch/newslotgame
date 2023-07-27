using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker.Slots
{
    [CreateAssetMenu(fileName = "New Winning Combination", menuName = "SlotMaker2/Math/Winning Combination")]
    public class WinningCombination : VariableLongList
    {
        [Flags]
        public enum CombinationRule
        {
            LeftToRight = (1 << 0),
            RightToLeft = (1 << 1),
            Scatter     = (1 << 2)
        }

        [Flags]
        public enum CombinationMask
        {
            Combination1 = (1 << 0),
            Combination2 = (1 << 1),
            Combination3 = (1 << 2),
            Combination4 = (1 << 3),
            Combination5 = (1 << 4),
            Combination6 = (1 << 5),
            Combination7 = (1 << 6),
            Combination8 = (1 << 7),
            Combination9 = (1 << 8),
            Combination10 = (1 << 9)
        };

        [InlineEditor]
        public SymbolEntity symbol;
        public SymbolEntity.SymbolAttribute any = SymbolEntity.SymbolAttribute.Wild;
        public CombinationRule combinationRule = CombinationRule.LeftToRight;
        public CombinationMask combinationMask = CombinationMask.Combination1;

        public bool HasAttribute(SymbolEntity.SymbolAttribute mask)
        {
            return (any & mask) == mask;
        }

        public bool HasAnyAttribute(SymbolEntity.SymbolAttribute mask)
        {
            return (int)(any & mask) != 0;
        }

        public bool HasWild()
        {
            return HasAnyAttribute(SymbolEntity.SymbolAttribute.Wild);
        }

        public bool HasSeven()
        {
            return HasAttribute(SymbolEntity.SymbolAttribute.Seven);
        }

        public bool HasBar()
        {
            return HasAttribute(SymbolEntity.SymbolAttribute.Bar);
        }

        public bool HasCombinationRule(CombinationRule mask)
        {
            return (combinationRule & mask) == mask;
        }

        public bool HasAnyCombinationRule(CombinationRule mask)
        {
            return (int)(combinationRule & mask) != 0;
        }

        public bool HasCombinationMask(CombinationMask mask)
        {
            return (combinationMask & mask) == mask;
        }

        public bool HasAnyCombinationMask(CombinationMask mask)
        {
            return (int)(combinationMask & mask) != 0;
        }
    }
}