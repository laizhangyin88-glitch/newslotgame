using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion;
using ParadoxNotion.Design;

using SlotMaker;

namespace SlotMaker.Tasks.Condition
{

    [Category("★ BagelCode/Contents")]
    public class CheckSymbolWin : ConditionTask
    {
        public enum SymbolWinProperty
        {
            EarnCredit,
            SymbolIndex,
            HitCount,
            WayCount,
        }

        public BBParameter<SymbolWin> symbolWin;
        public SymbolWinProperty symbolWinProperty;
        public CompareMethod checkType = CompareMethod.GreaterThan;
        public BBObjectParameter checkValue;

        protected override string info
        {
            get
            {
                if (checkValue.isNone || checkValue.isNull)
                    return string.Empty;

                switch (symbolWinProperty)
                {
                    case SymbolWinProperty.EarnCredit:
                        return $"SymbolWin.earnCredit {OperationUtils.GetCompareString(checkType)} {checkValue}";
                    case SymbolWinProperty.SymbolIndex:
                        return $"SymbolWin.symbolIndex {OperationUtils.GetCompareString(checkType)} {checkValue}";
                    case SymbolWinProperty.HitCount:
                        return $"SymbolWin.hitCount {OperationUtils.GetCompareString(checkType)} {checkValue}";
                    case SymbolWinProperty.WayCount:
                        return $"SymbolWin.wayCount {OperationUtils.GetCompareString(checkType)} {checkValue}";
                    default:
                        return string.Empty;
                }
            }
        }

        protected override bool OnCheck()
        {
            if (symbolWin.isNone || symbolWin.isNull)
            {
                return false;
            }

            switch (symbolWinProperty)
            {
                case SymbolWinProperty.EarnCredit:
                    return OperationUtils.Compare(symbolWin.value.earnCredit, (long)checkValue.value, checkType);
                case SymbolWinProperty.SymbolIndex:
                    return OperationUtils.Compare(symbolWin.value.symbolIndex, (int)checkValue.value, checkType);
                case SymbolWinProperty.HitCount:
                    return OperationUtils.Compare(symbolWin.value.hitCount, (int)checkValue.value, checkType);
                case SymbolWinProperty.WayCount:
                    if (!symbolWin.value.wayCount.HasValue)
                    {
                        return false;
                    }
                    return OperationUtils.Compare(symbolWin.value.wayCount.Value, (int)checkValue.value, checkType);
                default:
                    return false;
            }
        }

        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
#if UNITY_EDITOR

        protected override void OnTaskInspectorGUI()
        {
            DrawDefaultInspector();

            switch (symbolWinProperty)
            {
                case SymbolWinProperty.EarnCredit:
                    checkValue.SetType(typeof(long));
                    break;
                case SymbolWinProperty.SymbolIndex:
                case SymbolWinProperty.HitCount:
                case SymbolWinProperty.WayCount:
                    checkValue.SetType(typeof(int));
                    break;
            }
        }

#endif
    }
}