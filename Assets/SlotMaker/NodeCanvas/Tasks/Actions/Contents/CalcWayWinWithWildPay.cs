using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

    [Category("★ BagelCode/Contents")]
    public class CalcWayWinWithWildPay : ActionTask<Blackboard>
    {
        public BBParameter<int> slotIndex = 0;
        public BBParameter<int> column;
        public BBParameter<int> row;
        public BBParameter<long> betCredit;
        public BBParameter<long> baseWager;
        public BBParameter<List<Blackboard>> payTable;
        public BBParameter<long> multiplier;
        public BBParameter<List<long>> symbolMultipliers;
        public BBParameter<bool> bidirectional;
        public BBParameter<bool> excludeMaxWay;
        // mainWildSymbolIndex stands for an index of wild symbol that correspond to the index of wild symbol in paytable
        // with this index, all wild symbols (of possibly more than one index of wild symbol) are calculated together
        public BBParameter<int> mainWildSymbolIndex = 0;

        [SerializeField] protected bool checkAll = true;
        [SerializeField] protected List<bool> masks;

        public BBParameter<List<SymbolWin>> saveAs;

        protected List<SymbolWin> winList;

        private SymbolMask mask;

        protected long betPerWay { get { return betCredit.value / baseWager.value; } }

        protected int GetDirectionalColumn(int column, int direction)
        {
            return direction == 1 ? column : (this.column.value - 1) - column;
        }

        protected SymbolInfo GetSymbolInfo(int column, int row)
        {
            return ContentCustomData.GetSlotData(slotIndex.value).deck.GetSymbol(column, row);
        }

        protected long GetSymbolMultiplier(int symbolIndex)
        {
            if (symbolMultipliers.isNone || symbolMultipliers.isNull)
                return 1L;

            return symbolMultipliers.value[symbolIndex];
        }

        protected bool IsHit(ref SymbolInfo winSymbolInfo, SymbolInfo symbolInfo)
        {
            if (SymbolMask.HasReject(symbolInfo))
            {
                return false;
            }
            else if (winSymbolInfo.Equals(symbolInfo))
            {
                winSymbolInfo = symbolInfo;
                return true;
            }
            else if (SymbolMask.HasWild(symbolInfo))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public long FindEarnCredit(int symbolIndex, int hitCount)
        {
            return payTable.value[symbolIndex].GetValue<List<long>>("value")[hitCount - 1];
        }

        private bool IsBidirectionalCalcSymbol(int symbol)
        {
            if (checkAll || masks[symbol])
            {
                return true;
            }
            return false;
        }

        // this function doesn't consider ways having only wilds to be win ways
        protected long CalcOneWay(int waySymbolIndex, int direction, bool excludeMaxWay)
        {
            var symbolWin = new SymbolWin();

            int hitCount = 0;
            var winSymbolInfo = SlotUtils.CreateSymbolInfo(waySymbolIndex, mask);
            int wayCount = 1;
            long wayMultiplier = 1L;
            int totalHitCount = 0;
            int totalWildHitCount = 0;
            bool isCheckingOnlyWildWin = waySymbolIndex == mainWildSymbolIndex.value;

            for (int column = 0; column < this.column.value; ++column)
            {
                int dColumn = GetDirectionalColumn(column, direction);
                int columnHitCount = 0;

                for (int row = 0; row < this.row.value; ++row)
                {
                    var symbolInfo = GetSymbolInfo(dColumn, row);

                    if (IsHit(ref winSymbolInfo, symbolInfo))
                        ++columnHitCount;
                    else
                        continue;

                    wayMultiplier *= GetSymbolMultiplier(symbolInfo.symbol);
                    wayMultiplier *= symbolInfo.multiplier;

                    if (SymbolMask.HasWild(symbolInfo))
                    {
                        totalWildHitCount++;
                    }

                    symbolWin.cells.Add(new Cell(dColumn, row));
                }

                if (columnHitCount == 0)
                {
                    break;
                }
                else
                {
                    ++hitCount;
                    totalHitCount += columnHitCount;
                    wayCount *= columnHitCount;
                }
            }

            if (direction == -1 && !IsBidirectionalCalcSymbol(winSymbolInfo.symbol))
            {
                return 0L;
            }

            if ((hitCount == 0) || (hitCount == this.column.value && excludeMaxWay))
                return 0L;

            long earnCredit = FindEarnCredit(winSymbolInfo.symbol, hitCount);
            
            if (IsWayWinValid(isCheckingOnlyWildWin, earnCredit, totalHitCount, totalWildHitCount))
            {
                symbolWin.symbolIndex = waySymbolIndex;
                symbolWin.direction = direction;
                symbolWin.hitCount = hitCount;
                symbolWin.wayCount = wayCount;
                symbolWin.multiplier = multiplier.value * wayMultiplier;
                symbolWin.earnCredit = earnCredit * betPerWay * symbolWin.multiplier * wayCount;

                winList.Add(symbolWin);

                ContentCustomData.GetSlotData(slotIndex.value).deck.UpdateHitMap(symbolWin.cells);
            }

            return symbolWin.earnCredit;
        }

        private bool IsWayWinValid(bool isCheckingOnlyWildWin, long earnCredit, int totalHitCount, int totalWildHitCount)
        {
            if (earnCredit == 0L) return false;
            // filter way out that has only wilds when the way checks for the symbol whose index is not "main" wild
            if (!isCheckingOnlyWildWin && totalHitCount == totalWildHitCount) return false;
            return true;
        }

        protected override void OnExecute()
        {
            mask = ContentCustomData.GetSlotData(slotIndex.value).symbolMask;
            long totalEarnCredit = 0L;
            winList = new List<SymbolWin>();
            for (int i = 0; i < payTable.value.Count; ++i)
            {
                totalEarnCredit += CalcOneWay(i, 1, false);
                if (bidirectional.value)
                    totalEarnCredit += CalcOneWay(i, -1, excludeMaxWay.value);
            }

            winList.Sort();

            var spin = BlackboardUtils.FindVariable<Blackboard>(null, "./spin").value;
            BlackboardUtils.SetOrCreateValue<List<SymbolWin>>(spin, "winList", winList);
            ContentBlackboardUtils.AddEarnCredit(spin, totalEarnCredit);

            saveAs.value = winList;

            EndAction();
        }

        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
#if UNITY_EDITOR

        protected override void OnTaskInspectorGUI()
        {
            DrawDefaultInspector();

            if (bidirectional.value)
            {
                UnityEditor.EditorGUILayout.LabelField("Bidirectional", "options");
                checkAll = UnityEditor.EditorGUILayout.Toggle("Check All Symbols", checkAll);

                if (!checkAll)
                    masks = (List<bool>)EditorUtils.ReflectedFieldInspector("Masks", masks, typeof(List<bool>));
            }
        }

#endif

    }

}
