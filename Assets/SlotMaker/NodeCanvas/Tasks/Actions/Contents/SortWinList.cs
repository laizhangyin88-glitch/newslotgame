using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Design;
using ParadoxNotion;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

    [Category("★ BagelCode/Contents")]
    public class SortWinList : ActionTask<Blackboard>
    {
        public enum OrderType
        {
            Ascending,
            Descending
        }
        public enum SortType
        {
            EarnCredit,
            SymbolIndex
        }
        public BBParameter<List<SymbolWin>> winList;
        public SortType sortType;
        public OrderType orderType;

        // parameters to  sort by Symbol Index
        [SerializeField] protected string symbolIndexPriorityListName;
        private List<int> symbolIndexPriorityList;


        protected override void OnExecute()
        {
            switch (sortType)
            {
                case SortType.EarnCredit:
                    winList.value.Sort(CompareByEarnCredit);
                    break;
                case SortType.SymbolIndex:
                    if (!string.IsNullOrEmpty(symbolIndexPriorityListName))
                        symbolIndexPriorityList = BlackboardUtils.FindVariable<List<int>>(agent, symbolIndexPriorityListName).value;
                    winList.value.Sort(CompareBySymbolIndex);
                    break;
            }

            EndAction();
        }

        private int CompareByEarnCredit(SymbolWin a, SymbolWin b)
        {
            int ret = a.CompareTo(b);
            if (orderType == OrderType.Ascending)
                ret = -ret;
            return ret;
        }

        private int CompareBySymbolIndex(SymbolWin a, SymbolWin b)
        {
            int ret = 0;
            if (symbolIndexPriorityList != null)
            {
                ret = symbolIndexPriorityList[b.symbolIndex] - symbolIndexPriorityList[a.symbolIndex];
                if (ret != 0)
                    return ret;
            }

            ret = a.symbolIndex - b.symbolIndex;
            if (orderType == OrderType.Descending)
                ret = -ret;
            return ret != 0 ? ret : a.CompareTo(b);
        }

        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
#if UNITY_EDITOR
        protected override void OnTaskInspectorGUI()
        {
            DrawDefaultInspector();

            if (sortType == SortType.SymbolIndex)
            {
                symbolIndexPriorityListName = (string)EditorUtils.ReflectedFieldInspector("symbolIndexPriorityList", symbolIndexPriorityListName, typeof(string));
            }
        }
#endif
    }

}
