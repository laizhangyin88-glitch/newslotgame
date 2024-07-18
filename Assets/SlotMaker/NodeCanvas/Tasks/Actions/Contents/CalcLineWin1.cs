using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using ParadoxNotion;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

    [Category("★ BagelCode/Contents")]
    public class CalcLineWin1 : ActionTask<Blackboard>
    {
        public BBParameter<int> slotIndex = 0;
        public BBParameter<int> column;
        public BBParameter<int> row;
        public BBParameter<int> columnOffset = 0;
        public BBParameter<int> rowOffset = 0;
        public BBParameter<long> betCredit;
        public BBParameter<long> baseWager;
        public BBParameter<List<Blackboard>> payTable;
        public BBParameter<List<Blackboard>> payLine;
        public BBParameter<long> multiplier;
        public OperationMethod MultiplierOperation = OperationMethod.Multiply;
        public BBParameter<bool> bidirectional;
        public BBParameter<bool> excludeMaxLine = false;

        [SerializeField] protected bool checkAll = true;
        [SerializeField] protected List<bool> masks;

        public SymbolAttribute ignoreSymbolAttribute = (SymbolAttribute)0;

        public BBParameter<List<SymbolWin>> saveAs;

        protected List<SymbolWin> winList;

        private const int wildSymbolIndex = 0;

        protected long betPerLine { get { return betCredit.value / baseWager.value; } }

        protected int GetDirectionalColumn(int column, int direction)
        {
            return direction == 1 ? column : (this.column.value - 1) - column;
        }

        protected SymbolInfo GetSymbolInfo(int column, int row)
        {
            return ContentCustomData.GetSlotData(slotIndex.value).deck.GetSymbol(column, row);
        }

        protected bool IsHit(ref SymbolInfo winSymbolInfo, SymbolInfo symbolInfo)
        {
            if (SymbolMask.HasReject(symbolInfo))
            {
                return false;
            }
            else if (winSymbolInfo.Equals(symbolInfo) || SymbolMask.HasWild(winSymbolInfo))
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

        public List<int> GetPayLine(int lineIndex)
        {
            return payLine.value[lineIndex].GetValue<List<int>>("value");
        }

        private bool IsBidirectionalCalcSymbol(int symbol)
        {
            if (checkAll || masks[symbol])
            {
                return true;
            }
            return false;
        }

        protected long CalcOneLine(int lineIndex, int direction)
        {
            var symbolWin = new SymbolWin();

            int hitCount = 0;
            int wildHitCount = 0;
            int winSymbol = -1;
            SymbolInfo winSymbolInfo = null;
            long lineMultiplier = 1L;
            List<int> line = GetPayLine(lineIndex);

            for (int column = 0; column < this.column.value; ++column)
            {
                int dColumn = GetDirectionalColumn(column, direction);
                int row = line[dColumn] + rowOffset.value;
                var symbolInfo = GetSymbolInfo(dColumn + columnOffset.value, row);
                int symbolMultiplier = symbolInfo.multiplier;


                if ((int)ignoreSymbolAttribute != 0 &&
                    SymbolMask.HasAttribute(symbolInfo, ignoreSymbolAttribute))
                {
                    break;
                }

                if (column == 0)
                {
                    winSymbolInfo = symbolInfo;
                    ++hitCount;
                }
                else if (IsHit(ref winSymbolInfo, symbolInfo))
                {
                    ++hitCount;
                }
                else
                {
                    break;
                }
                
                if (MultiplierOperation == OperationMethod.Add)
                    symbolMultiplier = symbolMultiplier <= 1 ? 0 : symbolMultiplier;

                // multiplier끼리 더하거나 곱할 수 있습니다.
                lineMultiplier = (long)OperationTools.Operate(lineMultiplier, (long)symbolMultiplier, MultiplierOperation);

                if (SymbolMask.HasWild(symbolInfo) && SymbolMask.HasWild(winSymbolInfo))
                    ++wildHitCount;

                // TODO
                // cells 는 실제 히트된 cell 만 포함하고 있어야 합니다.
                // 현재 버전에서는 hit 되지 않은 cell 도 포함되고 있습니다.
                symbolWin.cells.Add(new Cell(dColumn + columnOffset.value, row));
            }
            if (winSymbolInfo == null)
                return 0L;

            winSymbol = winSymbolInfo.symbol;

            if (direction == -1 && !IsBidirectionalCalcSymbol(winSymbol))
            {
                return 0L;
            }

            long earnCredit = FindEarnCredit(winSymbol, hitCount);
            if (wildHitCount > 0)
            {
                long wildEarnCredit = FindEarnCredit(wildSymbolIndex, wildHitCount);
                if (wildEarnCredit > earnCredit)
                {
                    int count = hitCount - wildHitCount;
                    for (int column = 0; column < count; ++column)
                        symbolWin.cells.RemoveAt(wildHitCount);

                    winSymbol = wildSymbolIndex;
                    hitCount = wildHitCount;
                    earnCredit = wildEarnCredit;
                }
            }

            if (hitCount == this.column.value && direction == -1 && excludeMaxLine.value == true)
                return 0L;

            if (earnCredit > 0L)
            {
                if (MultiplierOperation == OperationMethod.Add && lineMultiplier != 1L)
                    --lineMultiplier;

                symbolWin.symbolIndex = winSymbol; //winSymbolInfo.symbol;
                symbolWin.direction = direction;
                symbolWin.lineIndex = lineIndex + 1;
                symbolWin.hitCount = hitCount;
                symbolWin.multiplier = multiplier.value * lineMultiplier;
                symbolWin.earnCredit = earnCredit * betPerLine * symbolWin.multiplier;
                winList.Add(symbolWin);

                ContentCustomData.GetSlotData(slotIndex.value).deck.UpdateHitMap(symbolWin.cells);
            }

            return symbolWin.earnCredit;
        }

        protected override void OnExecute()
        {
            long totalEarnCredit = 0L;
            winList = new List<SymbolWin>();
            for (int i = 0; i < payLine.value.Count; ++i)
            {
                totalEarnCredit += CalcOneLine(i, 1);
                if (bidirectional.value)
                    totalEarnCredit += CalcOneLine(i, -1);
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
