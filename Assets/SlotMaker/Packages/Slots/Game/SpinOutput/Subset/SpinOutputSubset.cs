using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using Sirenix.OdinInspector;

namespace SlotMaker.Slots
{
    [Serializable]
    public abstract class SpinOutputSubset : ScriptableObject
    {
        [InlineEditor]
        [PropertyOrder(-100)]
        public SpinOutput source;

        public OverridenSymbolEntity GetSymbol(int x, int y, int z)
        {
            return source.GetSymbol(x, y, z);
        }
    }

    [Serializable]
    public abstract class SpinOutputExpectationSubset : SpinOutputSubset
    {
        public event Action<int> onBeginExpectation;
        public event Action<int> onBeginFirstExpectation;
        public event Action<int> onEndExpectation;
        public event Action<int> onEndLastExpectation;

        public event Action<List<List<Cell3>>> onExpectationSpots;

        public abstract List<bool> GetExpectedReels();
        public abstract List<List<Cell3>> GetExpectedSpots();
        public abstract List<int> GetExpectedValues();

        public virtual void BeginExpectation(int index)
        {
            if (onBeginExpectation != null) onBeginExpectation(index);

            if (onBeginFirstExpectation != null)
            {
                var expectation = GetExpectedReels();
                for (int i = 0; i <= index; ++i)
                {
                    if (expectation[i])
                    {
                        if (i == index) onBeginFirstExpectation(index);
                        break;
                    }
                }
            }
        }

        public virtual void EndExpectation(int index)
        {
            if (onEndExpectation != null) onEndExpectation(index);

            if (onEndLastExpectation != null)
            {
                var expectation = GetExpectedReels();
                for (int i = expectation.Count - 1; i >= index; --i)
                {
                    if (expectation[i])
                    {
                        if (i == index) onEndLastExpectation(index);
                        break;
                    }
                }
            }
        }

        public virtual void ExpectationSpots()
        {
            if (onExpectationSpots != null) onExpectationSpots(GetExpectedSpots());
        }
    }

    [Serializable]
    public class SpinWin
    {
        public long earnCredit;
        public long multiplier = 1L;
        public WinningCombination winningCombination;
        public WinningCombination.CombinationRule combinationRule;
        public List<Cell3> spots = new List<Cell3>();
        public int hitCount;

        public static bool IsHit(OverridenSymbolEntity symbol, ref OverridenSymbolEntity winningSymbol)
        {
            if ((winningSymbol == null) || winningSymbol.Equals(symbol) || winningSymbol.IsWild())
            {
                winningSymbol = symbol;
                return true;
            }
            else if (symbol.IsWild())
            {
                return true;
            }

            return false;
        }

        public static bool IsHit(OverridenSymbolEntity symbol, SymbolEntity winningSymbol, SymbolEntity.SymbolAttribute any)
        {
            return (symbol.value == winningSymbol.value) || symbol.HasAnyAttribute(any);
        }

        public static long OperateMultiplier(long a, long b, OperationMethod om)
        {
            if (om == OperationMethod.Set)
                return b;
            if (om == OperationMethod.Add)
                return (a > 1L ? a : 0) + (b > 1L ? b : 0);
            // if (om == OperationMethod.Subtract)
            //     return a - b;
            if (om == OperationMethod.Multiply)
                return a * b;
            // if (om == OperationMethod.Divide)
            //     return a / b;
            return a;
        }
    }

    [Serializable]
    public class SpinLineWin : SpinWin
    {
        public int lineIndex;
    }

    [Serializable]
    public class SpinWayWin : SpinWin
    {
        public int wayCount;
    }

    [Serializable]
    public abstract class SpinOutputWinningSubset : SpinOutputSubset
    {
        public event Action<SpinOutputWinningSubset> onTotalWin;
        public event Action<SpinWin> onSingleWin;
        public event Action<SpinOutputWinningSubset> onSkipWin;

        [NonSerialized]
        [ShowInInspector]
        public long betCredit;

        [NonSerialized]
        [ShowInInspector]
        public long earnCredit;

        [NonSerialized]
        [ShowInInspector]
        public long multiplier = 1L;

        [NonSerialized]
        public HashSet<Cell3> spots = new HashSet<Cell3>();

        public abstract IEnumerable<SpinWin> GetTotalWin();
        public abstract SpinWin GetSingleWin(int winningIndex);
        
        public abstract int winCount { get; }

        public virtual void TotalWin()
        {
            if (onTotalWin != null) onTotalWin(this);
        }

        public virtual void SingleWin(int winningIndex)
        {
            if (onSingleWin != null) onSingleWin(GetSingleWin(winningIndex));
        }

        public virtual void SkipWin()
        {
            if (onSkipWin != null) onSkipWin(this);
        }
    }
}