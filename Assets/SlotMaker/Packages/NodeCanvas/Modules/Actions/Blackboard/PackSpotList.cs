using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker.IoC;

namespace SlotMaker.Slots.Tasks.Actions.Blackboards
{
    [Category("✶ Slots/Blackboard")]
    public class PackSpotList : ActionTask<Blackboard>
    {
        public BBParameter<string> target;
        public ListType targetType = ListType.SingleList;
        public BBParameter<int> totalColumn;
        public BBParameter<int> totalRow;
        public MajorOrder majorOrderFrom = MajorOrder.ColumnMajor;
        public MajorOrder majorOrderTo = MajorOrder.ColumnMajor;
        public MajorSortOrder sortMode = MajorSortOrder.DoNothing;
        
        public BBParameter<List<int>> saveAs;

        protected override string info
        {
            get { return string.Format("({0}) {1}  = Pack( ({2} {3} ).Sort({4})", majorOrderTo, saveAs, majorOrderFrom, target, sortMode); }
        }

        protected override void OnExecute()
        {
            List<Cell> source = new List<Cell>();
            if (targetType == ListType.SingleList)
            {
                var list = BlackboardUtils.FindValue<List<int>>(agent, target.value);
                foreach (var item in list)
                {
                    source.Add(Cell.UnPack(totalColumn.value, totalRow.value, item, majorOrderFrom));
                }
            }
            else if (targetType == ListType.MultiList)
            {
                var bbList = BlackboardUtils.FindValue<List<Blackboard>>(agent, target.value);
                for (int i = 0, count = bbList.Count; i < count; ++i)
                {
                    var list = bbList[i].GetValue<List<int>>("value");
                    foreach (var item in list)
                    {
                        source.Add(Cell.UnPack(i, item, majorOrderFrom));
                    }
                }
            }

            if (sortMode != MajorSortOrder.DoNothing)
                source.Sort(MajorSortOrderCellComparer.Get(sortMode));
            
            var result = new List<int>();
            foreach (var cell in source)
            {
                result.Add(Cell.Pack(totalColumn.value, totalRow.value, cell, majorOrderTo));
            }
            saveAs.value = result;

            EndAction();
        }
    }
}