using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NodeCanvas.Framework;
using SlotMaker;

namespace GameStudio.Slot.HOC
{
    [Serializable]
    public class HOCFeatureUtils
    {
        public const int WILD_SYMBOL_INDEX = 0;
        public const int HIGH_SYMBOL_INDEX = 1;
        public const int BLANK_SYMBOL_INDEX = 13;
        public const int MULTIPLIER_WILD_SYMBOL_INDEX = 14;

        // return wild stacking start row index
        public static int StackWild(BaseSlotMachine slotMachine, int column)
        {
            var deck = ContentCustomData.Instance.slotDataList[slotMachine.slotIndex].deck;
            int wildStartIndex = -1;

            for (int rowIndex = 0; rowIndex < slotMachine.RowCount; rowIndex++)
                if (deck.GetSymbol(column, rowIndex).mask == SymbolAttribute.Wild)
                {
                    wildStartIndex = rowIndex;
                    break;
                }

            if (wildStartIndex == -1) return -1;

            int stackingCount = slotMachine.RowCount - wildStartIndex;
            for (int rowIndex = wildStartIndex; rowIndex < slotMachine.RowCount; rowIndex++)
            {
                var deckSymbol = deck.GetSymbol(column, rowIndex);
                deckSymbol.link.rowCount = stackingCount;
                deckSymbol.link.rowOffset = wildStartIndex - rowIndex;
                deck.deck[column][rowIndex].symbol = WILD_SYMBOL_INDEX;
                deck.deck[column][rowIndex].mask = SymbolAttribute.Wild;

                var slotSymbol = slotMachine.GetSymbol(column, rowIndex);
                slotSymbol.symbolInfo.link.rowCount = stackingCount;
                slotSymbol.symbolInfo.link.rowOffset = wildStartIndex - rowIndex;

                if (rowIndex == wildStartIndex) slotSymbol.Play("Expand");
            }

            return wildStartIndex;
        }


        public static void TurnHighToWildOfColumn(BaseSlotMachine slotMachine, int column)
        {
            var deck = ContentCustomData.Instance.slotDataList[slotMachine.slotIndex].deck;
            var row = slotMachine.RowCount;

            for (int rowIndex = 0; rowIndex < row; rowIndex++)
            {
                if (deck.deck[column][rowIndex].symbol == HIGH_SYMBOL_INDEX)
                {
                    deck.deck[column][rowIndex].symbol = WILD_SYMBOL_INDEX;
                    deck.deck[column][rowIndex].mask = SymbolAttribute.Wild;
                    slotMachine.GetSymbol(column, rowIndex).Play("TurnToWild");
                }
            }
        }

        public static IEnumerator TurnHighToWildOfColumn(BaseSlotMachine slotMachine, int column, float waitPerSymbol)
        {
            var delayForTurning = new WaitForSeconds(waitPerSymbol);

            var deck = ContentCustomData.Instance.slotDataList[slotMachine.slotIndex].deck;
            var row = slotMachine.RowCount;

            var turningTargetRowList = new List<int>();
            for (int rowIndex = row - 1; rowIndex >= 0; rowIndex--)
            {
                if (deck.deck[column][rowIndex].symbol == HIGH_SYMBOL_INDEX)
                {
                    deck.deck[column][rowIndex].symbol = WILD_SYMBOL_INDEX;
                    deck.deck[column][rowIndex].mask = SymbolAttribute.Wild;
                    turningTargetRowList.Add(rowIndex);
                }
            }

            foreach (int targetRow in turningTargetRowList)
            {
                slotMachine.GetSymbol(column, targetRow).Play("TurnToWild");
                yield return delayForTurning;
            }
        }

        public static IEnumerator CopyStackedWild(BaseSlotMachine previousSlot, BaseSlotMachine targetSlot, int column,
            int stackingStartIndex, GameObject flyingObject, float flyingTime)
        {
            var targetDeck = ContentCustomData.Instance.slotDataList[targetSlot.slotIndex].deck;

            int stackingCount = targetSlot.RowCount - stackingStartIndex;
            for (int rowIndex = stackingStartIndex; rowIndex < targetSlot.RowCount; rowIndex++)
            {
                var deckSymbol = targetDeck.deck[column][rowIndex];
                deckSymbol.symbol = WILD_SYMBOL_INDEX;
                deckSymbol.mask = SymbolAttribute.Wild;
                deckSymbol.link.rowCount = stackingCount;
                deckSymbol.link.rowOffset = stackingStartIndex - rowIndex;
            }

            for (int rowIndex = stackingStartIndex; rowIndex < targetSlot.RowCount; rowIndex++)
                if (rowIndex == stackingStartIndex)
                {
                    GameObject flyingStartAnchor = new GameObject("Flying Start Anchor", typeof(RectTransform));
                    GameObject flyingDestAnchor = new GameObject("Flying Dest Anchor", typeof(RectTransform));
                    flyingStartAnchor.transform.SetParent(previousSlot.reels[column].transform);
                    flyingDestAnchor.transform.SetParent(targetSlot.reels[column].transform);

                    Vector3 startPosition = previousSlot.reels[column].CalcSymbolPosition(0, 0, 0, rowIndex);
                    Vector3 destPosition = targetSlot.reels[column].CalcSymbolPosition(0, 0, 0, rowIndex);

                    flyingStartAnchor.GetComponent<RectTransform>().anchoredPosition3D = startPosition;
                    flyingDestAnchor.GetComponent<RectTransform>().anchoredPosition3D = destPosition;

                    var positionController = flyingObject.GetComponentInChildren<DirectionalWeightPositionController>();
                    positionController.from = flyingStartAnchor.transform;
                    positionController.to = flyingDestAnchor.transform;
                    flyingObject.SetActive(true);

                    yield return new WaitForSeconds(flyingTime);

                    flyingStartAnchor.DestroyThis();
                    flyingDestAnchor.DestroyThis();
                    flyingObject.GetComponent<PooledObject>().ReturnToPool();

                    var symbolInfo = new SymbolInfo
                    {
                        symbol = WILD_SYMBOL_INDEX,
                        mask = SymbolAttribute.Wild,
                    };
                    symbolInfo.link.rowCount = stackingCount;
                    symbolInfo.link.rowOffset = stackingStartIndex - rowIndex;

                    var overlaySymbol = targetSlot.overlay.AddSymbol(column, rowIndex, symbolInfo);
                    overlaySymbol.symbolIndex = WILD_SYMBOL_INDEX;
                    overlaySymbol.Apply();
                    overlaySymbol.Play("Skip");
                }
                else
                {
                    var symbolInfo = new SymbolInfo
                    {
                        symbol = BLANK_SYMBOL_INDEX,
                        mask = SymbolAttribute.Blank,
                    };
                    symbolInfo.link.rowCount = stackingCount;
                    symbolInfo.link.rowOffset = stackingStartIndex - rowIndex;

                    targetSlot.overlay.RemoveSymbol(column, rowIndex);
                    var overlaySymbol = targetSlot.overlay.AddSymbol(column, rowIndex, symbolInfo);
                    overlaySymbol.symbolIndex = BLANK_SYMBOL_INDEX;
                    overlaySymbol.Apply();
                }
        }

        public static int GetRowIndexOfSymbolAtReel(BaseSlotMachine slotMachine, int column, int symbolIndex)
        {
            var deck = ContentCustomData.Instance.slotDataList[slotMachine.slotIndex].deck;
            for (int rowIndex = 0; rowIndex < slotMachine.RowCount; rowIndex++)
                if (deck.deck[column][rowIndex].symbol == symbolIndex)
                    return rowIndex;

            return -1;
        }


        public static int GetRowIndexOfSymbolAtReelInverseOrder(BaseSlotMachine slotMachine, int column,
            int symbolIndex)
        {
            var deck = ContentCustomData.Instance.slotDataList[slotMachine.slotIndex].deck;
            for (int rowIndex = slotMachine.RowCount - 1; rowIndex >= 0; rowIndex--)
                if (deck.deck[column][rowIndex].symbol == symbolIndex)
                    return rowIndex;

            return -1;
        }

        public static int GetRowIndexOfSymbolAtReel(BaseSlotMachine slotMachine, int column, SymbolAttribute symbolAttribute)
        {
            var deck = ContentCustomData.Instance.slotDataList[slotMachine.slotIndex].deck;
            for (int rowIndex = 0; rowIndex < slotMachine.RowCount; rowIndex++)
                if (SymbolMask.HasAttribute(deck.deck[column][rowIndex], symbolAttribute))
                    return rowIndex;

            return -1;
        }

        public static int GetRowIndexOfSymbolAtReelInverseOrder(BaseSlotMachine slotMachine, int column,
            SymbolAttribute symbolAttribute)
        {
            var deck = ContentCustomData.Instance.slotDataList[slotMachine.slotIndex].deck;
            for (int rowIndex = slotMachine.RowCount - 1; rowIndex >= 0; rowIndex--)
                if (SymbolMask.HasAttribute(deck.deck[column][rowIndex], symbolAttribute))
                    return rowIndex;

            return -1;
        }

        public static List<BaseSymbol> TurnOverlaySymbolToSlotSymbol(BaseSlotMachine slotMachine, int targetSymbolIndex)
        {
            var turnedSymbolList = new List<BaseSymbol>();
            for (int colIndex = 0; colIndex < slotMachine.ColumnCount; colIndex++)
            {
                for (int rowIndex = slotMachine.RowCount - 1; rowIndex >= 0; rowIndex--)
                {
                    var overlaySymbol = slotMachine.GetOverlaySymbol(new Cell(colIndex, rowIndex).GetHashCode());
                    if (overlaySymbol == null) continue;

                    if (overlaySymbol.symbolIndex == targetSymbolIndex)
                    {
                        var symbolInfo = overlaySymbol.symbolInfo;
                        var slotSymbol = slotMachine.GetSymbol(colIndex, rowIndex);
                        slotSymbol.symbolInfo = symbolInfo;
                        slotSymbol.symbolIndex = symbolInfo.symbol;
                        turnedSymbolList.Add(slotSymbol);
                    }
                }
            }

            return turnedSymbolList;
        }

        public static IEnumerator CallActionAfterDelay(Action func, float delay)
        {
            yield return new WaitForSeconds(delay);
            func();
        }

        public static bool IsInRange(int targetRow, int startRow, int endRow)
        {
            return targetRow >= startRow && targetRow <= endRow;
        }

        public static void UpdateStopDelaysPerSlot(List<BaseSlotMachine> slotMachineList, float additionalDelayForWildExpanding)
        {
            for (int i = 0; i < slotMachineList.Count; i++)
            {
                var slotMachine = slotMachineList[i];
                var slotMachineBB = slotMachine.GetComponent<Blackboard>();

                var originStopDelays = slotMachineBB.GetValue<List<float>>("boostStopDelays");
                var newStopDelays = new List<float>(originStopDelays.Count);

                for (int colIndex = 0; colIndex < slotMachine.ColumnCount; colIndex++)
                {
                    int wildStackStartIndex = GetRowIndexOfSymbolAtReel(slotMachine, colIndex,
                        SymbolAttribute.Wild);

                    bool needToWildExpand = wildStackStartIndex > -1;
                    if (needToWildExpand && i > 0)
                    {
                        int wildCopyStartIndex = int.MaxValue;
                        for (int previousSlotIndex = i - 1; previousSlotIndex >= 0; previousSlotIndex--)
                        {
                            int wildCopyStartIndexFromPrevious =     HOCFeatureUtils.GetRowIndexOfSymbolAtReel(slotMachineList[previousSlotIndex],
                                colIndex, WILD_SYMBOL_INDEX);

                            if(wildCopyStartIndexFromPrevious < 0)  wildCopyStartIndexFromPrevious =     HOCFeatureUtils.GetRowIndexOfSymbolAtReel(slotMachineList[previousSlotIndex],
                                colIndex, MULTIPLIER_WILD_SYMBOL_INDEX);

                            wildCopyStartIndex =
                                wildCopyStartIndexFromPrevious > -1 &&
                                wildCopyStartIndexFromPrevious < wildCopyStartIndex
                                    ? wildCopyStartIndexFromPrevious
                                    : wildCopyStartIndex;
                        }

                        needToWildExpand = wildCopyStartIndex > slotMachine.RowCount || (wildStackStartIndex < wildCopyStartIndex && wildCopyStartIndex < slotMachine.RowCount );
                    }

                    int expandCount = slotMachine.RowCount - wildStackStartIndex;
                    var originStopDelay = originStopDelays[colIndex];

                    if (needToWildExpand && colIndex <  slotMachine.ColumnCount -1)
                    {
                        if(expandCount > 1)  newStopDelays.Add(originStopDelay+additionalDelayForWildExpanding);
                        else if (i < slotMachineList.Count - 1)
                            newStopDelays.Add(originStopDelay + additionalDelayForWildExpanding - 1);
                        else newStopDelays.Add(originStopDelay);
                    }
                    else  newStopDelays.Add(originStopDelay);
                }

                BlackboardUtils.SetOrCreateValue<List<float>>(slotMachineBB,"newStopDelays" ,newStopDelays);
            }
        }

            public static void UpdateStopEffectSpots(List<BaseSlotMachine> slotMachineList)
        {
            for (int i = 0; i < slotMachineList.Count; i++)
            {
                var slotMachine = slotMachineList[i];
                var expectationSpots =
                    ContentCustomData.GetSlotData(slotMachine.slotIndex).expectation.expectationSpots;

                for (int colIndex = 0; colIndex < slotMachine.ColumnCount; colIndex++)
                {
                    int wildStackStartIndex = GetRowIndexOfSymbolAtReel(slotMachine,
                        colIndex,
                        SymbolAttribute.Wild);

                    bool needToWildExpand = wildStackStartIndex > -1;
                    if (needToWildExpand && i > 0)
                    {
                        int wildCopyStartIndex = int.MaxValue;
                        for (int previousSlotIndex = i - 1;
                             previousSlotIndex >= 0;
                             previousSlotIndex--)
                        {
                            int wildCopyStartIndexFromPrevious =
                                HOCFeatureUtils.GetRowIndexOfSymbolAtReel(
                                    slotMachineList[previousSlotIndex],
                                    colIndex, WILD_SYMBOL_INDEX);

                            if (wildCopyStartIndexFromPrevious < 0)
                                wildCopyStartIndexFromPrevious =
                                    HOCFeatureUtils.GetRowIndexOfSymbolAtReel(
                                        slotMachineList[previousSlotIndex],
                                        colIndex, MULTIPLIER_WILD_SYMBOL_INDEX);

                            wildCopyStartIndex =
                                wildCopyStartIndexFromPrevious > -1 &&
                                wildCopyStartIndexFromPrevious < wildCopyStartIndex
                                    ? wildCopyStartIndexFromPrevious
                                    : wildCopyStartIndex;
                        }

                        needToWildExpand = wildCopyStartIndex > slotMachine.RowCount ||
                                           (wildStackStartIndex < wildCopyStartIndex &&
                                            wildCopyStartIndex < slotMachine.RowCount);
                    }

                    if (needToWildExpand)
                        expectationSpots[colIndex]
                            .Add(new Cell(colIndex, wildStackStartIndex));
                }
            }
        }
        #region Super Bonus

        public static IEnumerator CopyStackedWildSB(BaseSlotMachine previousSlot, BaseSlotMachine targetSlot,
            int column, int stackingStartIndex, GameObject flyingObject, float flyingTime, int multiplier, int flyingCount)
        {
            var targetDeck = ContentCustomData.Instance.slotDataList[targetSlot.slotIndex].deck;

            int stackingCount = targetSlot.RowCount - stackingStartIndex;
            int targetSymbolIndex = multiplier > 1 ? MULTIPLIER_WILD_SYMBOL_INDEX : WILD_SYMBOL_INDEX;

            for (int rowIndex = stackingStartIndex; rowIndex < targetSlot.RowCount; rowIndex++)
            {
                var deckSymbol = targetDeck.deck[column][rowIndex];
                deckSymbol.multiplier = multiplier;
                deckSymbol.symbol = targetSymbolIndex;
                deckSymbol.mask = SymbolAttribute.Wild;
                deckSymbol.link.rowCount = stackingCount;
                deckSymbol.link.rowOffset = stackingStartIndex - rowIndex;
            }

            for (int rowIndex = stackingStartIndex; rowIndex < targetSlot.RowCount; rowIndex++)
                if (rowIndex == stackingStartIndex)
                {
                    GameObject flyingStartAnchor = new GameObject("Flying Start Anchor", typeof(RectTransform));
                    GameObject flyingDestAnchor = new GameObject("Flying Dest Anchor", typeof(RectTransform));
                    flyingStartAnchor.transform.SetParent(previousSlot.reels[column].transform);
                    flyingDestAnchor.transform.SetParent(targetSlot.reels[column].transform);

                    Vector3 startPosition = previousSlot.reels[column].CalcSymbolPosition(0, 0, 0, rowIndex);
                    Vector3 destPosition = targetSlot.reels[column].CalcSymbolPosition(0, 0, 0, rowIndex);

                    flyingStartAnchor.GetComponent<RectTransform>().anchoredPosition3D = startPosition;
                    flyingDestAnchor.GetComponent<RectTransform>().anchoredPosition3D = destPosition;

                    var positionController = flyingObject.GetComponentInChildren<DirectionalWeightPositionController>();
                    positionController.from = flyingStartAnchor.transform;
                    positionController.to = flyingDestAnchor.transform;
                    flyingObject.SetActive(true);
                    var flyingAnimator = flyingObject.GetComponentInChildren<Animator>();
                    flyingAnimator.SetInteger("SB Level", BlackboardUtils.FindValue<int>("./bonus/bonusId"));
                    flyingAnimator.SetInteger("Copied Count",flyingCount);

                    yield return new WaitForSeconds(flyingTime);

                    flyingStartAnchor.DestroyThis();
                    flyingDestAnchor.DestroyThis();
                    flyingObject.GetComponent<PooledObject>().ReturnToPool();

                    var symbolInfo = new SymbolInfo
                    {
                        symbol = targetSymbolIndex,
                        mask = SymbolAttribute.Wild,
                    };
                    symbolInfo.link.rowCount = stackingCount;
                    symbolInfo.link.rowOffset = stackingStartIndex - rowIndex;

                    var overlaySymbol = targetSlot.overlay.AddSymbol(column, rowIndex, symbolInfo);
                    overlaySymbol.symbolIndex = targetSymbolIndex;
                    overlaySymbol.Apply();
                    overlaySymbol.Play("Skip");
                }
                else
                {
                    var symbolInfo = new SymbolInfo
                    {
                        symbol = BLANK_SYMBOL_INDEX,
                        mask = SymbolAttribute.Blank,
                    };
                    symbolInfo.link.rowCount = stackingCount;
                    symbolInfo.link.rowOffset = stackingStartIndex - rowIndex;

                    targetSlot.overlay.RemoveSymbol(column, rowIndex);
                    var overlaySymbol = targetSlot.overlay.AddSymbol(column, rowIndex, symbolInfo);
                    overlaySymbol.symbolIndex = BLANK_SYMBOL_INDEX;
                    overlaySymbol.Apply();
                }
        }

        public static int StackWildSB(BaseSlotMachine slotMachine, int column)
        {
            var deck = ContentCustomData.Instance.slotDataList[slotMachine.slotIndex].deck;
            int wildStartIndex = -1;

            for (int rowIndex = 0; rowIndex < slotMachine.RowCount; rowIndex++)
                if (deck.GetSymbol(column, rowIndex).mask == SymbolAttribute.Wild)
                {
                    wildStartIndex = rowIndex;
                    break;
                }

            if (wildStartIndex == -1) return -1;

            int stackingCount = slotMachine.RowCount - wildStartIndex;
            int overlappedMultiplier = 1;
            for (int rowIndex = wildStartIndex; rowIndex < slotMachine.RowCount; rowIndex++)
            {
                var deckSymbol = deck.GetSymbol(column, rowIndex);
                overlappedMultiplier = overlappedMultiplier > deckSymbol.multiplier ? overlappedMultiplier : deckSymbol.multiplier;
            }

            int targetSymbolIndex = overlappedMultiplier > 1 ? MULTIPLIER_WILD_SYMBOL_INDEX : WILD_SYMBOL_INDEX;

            for (int rowIndex = wildStartIndex; rowIndex < slotMachine.RowCount; rowIndex++)
            {
                var deckSymbol = deck.GetSymbol(column, rowIndex);
                deckSymbol.link.rowCount = stackingCount;
                deckSymbol.link.rowOffset = wildStartIndex - rowIndex;
                deckSymbol.multiplier = overlappedMultiplier;
                deck.deck[column][rowIndex].symbol = targetSymbolIndex;
                deck.deck[column][rowIndex].mask = SymbolAttribute.Wild;

                var slotSymbol = slotMachine.GetSymbol(column, rowIndex);
                slotSymbol.symbolInfo.link.rowCount = stackingCount;
                slotSymbol.symbolInfo.link.rowOffset = wildStartIndex - rowIndex;

                if (rowIndex == wildStartIndex) slotSymbol.Play("Expand");
            }

            return wildStartIndex;
        }

        #endregion
    }
}
