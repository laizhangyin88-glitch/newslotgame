using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SlotMaker
{
    public interface ISlotMachine : IGrid, ISkippable
    {
        void Shuffle();
        void Shuffle(List<int> indices);
        void SetStripIndices(List<int> indices, int offset);

        BaseSlotMachine Save();
        void Load(BaseSlotMachine model);

        void Clean();
        void Clear();
        void ClearSymbols();

        List<BaseReel> GetReels();
        bool HasReel(int reelIndex);
        BaseReel GetReel(int reelIndex);
        BaseReel FindReel(int column, int row);
        BaseReel CreateReel();
        BaseReel CreateReel(BaseReel src);
        BaseReel CreateReel(int beginColumn, int beginRow, int endColumn, int endRow, int expandTopCount);
        BaseReel CreateReel(int reelIndex, int beginColumn, int beginRow, int endColumn, int endRow, int expandTopCount);
        BaseReel InsertReel(int reelIndex, int beginColumn, int beginRow, int endColumn, int endRow, int expandTopCount);
        void MergeReel(int begin, int end);
        void SplitPerSymbolReel(int reelIndex);

        void InitializeSymbols();
        BaseSymbol CreateSymbol();
        BaseSymbol GetSymbol(int reelIndex, int row);
        List<BaseSymbol> GetSymbols();

        void Visit(Action<BaseSymbol> visitor);

        BaseSlotMachineOverlay CreateOverlay();
        void ClearOverlaySymbols();
        void AddOverlaySymbol(BaseSymbol symbol);
        BaseSymbol GetOverlaySymbol(int index);
        bool HasOverlaySymbol(int index);
    }

    public interface IReel : IGrid, ISkippable
    {
        void Shuffle();
        void Shuffle(int stripIndex);
        void SwapIndex();
        void SetReelStripIndex(int index);

        List<BaseSymbol> GetSymbols();
        BaseSymbol       GetSymbol(int column, int row);

        void Clear();
        void ClearSymbols();
        void Initialize(BaseSlotMachine slotMachine, BaseReel src);
        void Initialize(BaseSlotMachine slotMachine, int reelIndex, int beginColumn, int beginRow, int endColumn, int endRow, int expandTopCount);
        void CopySymbols(BaseReel src);

        bool ContainsSymbol(int column, int row);
        void ClearExpandTop();
        void ExpandTop(int count);
        void ContractTop(int count);
        void Insert(int row, int count, SymbolInfo newSymbol);
        void Replace(int row, int count, SymbolInfo newSymbol);
        void Remove(int row, int count);
        void Merge(BaseReel src);

        void PushFrontSymbol();
        void PushFrontSymbols(int count);
        void PopFrontSymbols(int count);
        void PopBackSymbols(int count);
        void InsertSymbols(int row, int count, SymbolInfo newSymbol);
        void ReplaceSymbols(int row, int count, SymbolInfo newSymbol);
        void RemoveSymbols(int row, int count);
        void RemoveOutBoundSymbols();

        void Play(string animationName);
        void Visit(Action<BaseSymbol> visitor);
    }

    public interface ISymbol : ISkippable
    {
        RectTransform   rectTransform { get; }
        RectTransform   anchor        { get; }
        BaseSlotMachine slotMachine   { get; set; }
        BaseReel        reel          { get; set; }

        void Clear();
        void Initialize();
        void Initialize(BaseReel reel, BaseSymbol src);
        void Initialize(BaseReel reel, SymbolInfo newSymbol);
        void Initialize(BaseReel reel, int column, int row);
        void Initialize(BaseReel reel, int column, int row, int stripIndex);
        void Change(SymbolInfo newSymbol);
        void Apply();
        void Restore(BaseSymbol src);
        void Play(string animationName);
    }

    public interface ISlotMachineMovement
    {
        int spinningCount { get; }
        int movementType { get; set; }

        bool IsSpinning();
        bool IsStopped();

        void OnSpinReel();
        void OnPrepareStoppedReel(int reelIndex);
        void OnStoppedReel(int reelIndex);
    }

    public interface IReelMovement
    {
        bool IsSpinning();
        bool IsStopping();
        bool IsPrepareStopped();
        bool IsStopped();

        float GetVelocity();

        void Spin();
        void Stop();
        void ForceStop();

        void Play(int actionIndex);
        void Play(string actionName);

        void OnSpinSymbol();
        void OnPrepareStoppedSymbol(int row);
        void OnStoppedSymbol(int row);
    }

    public interface ISlotMachineOverlay
    {
        int symbolCount { get; }

        void Clear();

        BaseSymbol AddSymbol(BaseSymbol newSymbol);
        BaseSymbol AddSymbol(int reelIndex, int row, SymbolInfo newSymbol);
    }
}
