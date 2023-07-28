using NodeCanvas.Framework;
using SlotMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameStudio.Slot.SVD
{
    public class SVDSortObjectsAfterDropController : FeatureController
    {
        private const float RowOffset = 117f;
        private const int ColumnCount = 5;
        private const int RowCount = 7;

        protected override string ON_FEATURE_BEGIN_EVENT => "StartSortDropGameObjectsAfterDrop";
        protected override string ON_FEATURE_END_EVENT => "SortDropGameObjectsAfterDropDone";

        [SerializeField] private Blackboard mainFSMBB;

        protected override IEnumerator OnPlayCoroutine()
        {
            var listFlatGameObj = BlackboardUtils.FindVariable<List<GameObject>>(mainFSMBB, "listFlatGameObj");
            var wasDrop = BlackboardUtils.FindOrCreateVariable(mainFSMBB, "_wasDrop", typeof(bool));
            var stepIterator = BlackboardUtils.FindVariable<int>(mainFSMBB, "_stepIterator").value;

            wasDrop.value = false;

            for (int rowIndex = RowCount - 1; rowIndex >= 0; rowIndex--)
            {
                for (int columnIndex = 0; columnIndex < ColumnCount; columnIndex++)
                {
                    int oldIndex = RowCount * columnIndex + rowIndex;

                    var winDropGO = listFlatGameObj.value[oldIndex];
                    if (winDropGO == null)
                    {
                        continue;
                    }

                    int blankCount = 0;
                    for (int i = rowIndex + 1; i < RowCount; i++)
                    {
                        int checkIndex = RowCount * columnIndex + i;
                        if (listFlatGameObj.value[checkIndex] == null)
                        {
                            blankCount++;
                        }
                    }

                    int newRowIndex = rowIndex + blankCount;

                    int newIndex = RowCount * columnIndex + newRowIndex;

                    if (rowIndex != newRowIndex)
                    {
                        listFlatGameObj.value[oldIndex] = null;
                        listFlatGameObj.value[newIndex] = winDropGO;
                        wasDrop.value = true;
                    }
                }
            }

            var shouldWaitForInitialDrop = (bool) wasDrop.value && stepIterator == 0;
            if (shouldWaitForInitialDrop)
            {
                yield return new WaitForSeconds(1f);
            }
        }
    }
}
