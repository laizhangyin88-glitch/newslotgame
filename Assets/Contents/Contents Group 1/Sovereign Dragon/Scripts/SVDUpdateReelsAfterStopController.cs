using NodeCanvas.Framework;
using SlotMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameStudio.Slot.SVD
{
    public class SVDUpdateReelsAfterStopController : FeatureController
    {
        protected override string ON_FEATURE_BEGIN_EVENT => "StartUpdateReelsAfterStopSM";
        protected override string ON_FEATURE_END_EVENT => "UpdateReelsAfterStopSMDone";

        [SerializeField] private Blackboard mainFSMBB;

        private int symbolIterator;
        private bool haveSomeDropSymbol;

        private const int ColumnCount = 5;
        private const int RowCount = 7;

        protected override void OnStart()
        {
            base.OnStart();
            haveSomeDropSymbol = false;
            symbolIterator = 0;
        }

        enum EggType
        {
            Gold = 11,
            Silver = 12,
        }

        protected override IEnumerator OnPlayCoroutine()
        {
            string targetName = null;
            var stepInfoBeforeDrop = BlackboardUtils.FindBlackboard(mainFSMBB, "_stepInfoBeforeDrop", ref targetName);
            haveSomeDropSymbol = false;
            var symbolTypesList = BlackboardUtils.FindVariable<List<int>>(stepInfoBeforeDrop, "_symbolTypesList").value;
            var lockedSymbolsList = BlackboardUtils.FindVariable<List<bool>>(mainFSMBB, "_lockedSymbolsList").value;
            var slotMachineExpand = BlackboardUtils.FindVariable<GameObject>(mainFSMBB, "_slotMachineExpand");
            var sm = slotMachineExpand.value.GetComponent<BaseSlotMachine>();

            for (int rowIndex = 0; rowIndex < RowCount; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < ColumnCount; columnIndex++, symbolIterator++)
                {
                    int position = RowCount * columnIndex + rowIndex;

                    if (CanDropSymbol(symbolTypesList, lockedSymbolsList, position))
                    {
                        haveSomeDropSymbol = true;
                        PerformDrop(sm, rowIndex, position, symbolTypesList[position]);
                    }
                }
            }

            if (haveSomeDropSymbol)
            {
                yield return new WaitForSeconds(1.5f);
            }
        }

        private bool CanDropSymbol(List<int> symbolTypesList, List<bool> lockedSymbolsList, int position)
        {
            int symbolType = symbolTypesList[position];
            bool isSymbolAllowed = symbolType == (int)EggType.Gold || symbolType == (int)EggType.Silver;

            bool isLockedPosition = lockedSymbolsList[position];

            return !isLockedPosition && isSymbolAllowed;
        }

        private void PerformDrop(BaseSlotMachine sm, int rowIndex, int position, int symbolType)
        {
            var dropSymbolCell = new Cell(symbolIterator, rowIndex);

            var eggSymbol = sm.GetSymbol(dropSymbolCell.column, dropSymbolCell.row).gameObject;
            var eggSymbolPosition = eggSymbol.GetComponent<Transform>();

            var dropSymbolsPool = BlackboardUtils.FindVariable<GameObject>(mainFSMBB, "./DropSymbolsPool").value;
            var dropSymbolsPoolTransform = dropSymbolsPool.GetComponent<Transform>();
            GameObject dropSymbolGO = GetPooledObject(dropSymbolsPool, dropSymbolsPoolTransform);

            SetupDropGO(dropSymbolGO, position, symbolType, eggSymbolPosition);

            var listFlatGameObj = BlackboardUtils.FindVariable<List<GameObject>>(mainFSMBB, "listFlatGameObj");
            listFlatGameObj.value[position] = dropSymbolGO;

            sm.GetSymbol(dropSymbolCell.column, dropSymbolCell.row).Play("Drop");
        }

        private void SetupDropGO(GameObject dropSymbolGO, int position, int symbolType, Transform eggSymbolPosition)
        {
            dropSymbolGO.GetComponent<Transform>().position = eggSymbolPosition.position;
            var dropSymbolBB = dropSymbolGO.GetComponent<Blackboard>();

            dropSymbolBB.SetValue("symbolType", symbolType);
            if (symbolType == (int)EggType.Gold)
            {
                var originalJackpotIndexesWithoutEligible = BlackboardUtils.FindVariable<List<int>>(mainFSMBB, "_originalJackpotIndexesWithoutEligible").value;
                int originalJackpotIndexWithoutEligible = originalJackpotIndexesWithoutEligible[position];

                if (originalJackpotIndexWithoutEligible >= 0)
                {
                    dropSymbolBB.SetValue("isJackpot", true);
                    dropSymbolBB.SetValue("jpIndex", originalJackpotIndexWithoutEligible);
                }
                else
                {
                    var symbolValuesBeforeDropIndexesList = BlackboardUtils.FindVariable<List<int>>(mainFSMBB, "_symbolValuesBeforeDropIndexesList").value;
                    int valueIndex = symbolValuesBeforeDropIndexesList[position];

                    dropSymbolBB.SetValue("coinIndex", valueIndex);
                    dropSymbolBB.SetValue("CoinValueIndex", valueIndex);
                    dropSymbolBB.SetValue("isJackpot", false);

                    var betCredit = Convert.ToDouble(BlackboardUtils.FindVariable<long>(mainFSMBB, "_betCredit").value);
                    dropSymbolBB.SetValue("_Bet", betCredit);
                }
            }
            else
            {
                dropSymbolBB.SetValue("isJackpot", false);
            }

            //dropSymbolBB.SetValue("from", eggSymbolPosition);
            //dropSymbolBB.SetValue("to", eggSymbolPosition);
            dropSymbolGO.SetActive(true);
        }

        private GameObject GetPooledObject(GameObject poolGO, Transform parent)
        {
            var pool = poolGO.GetComponent<ObjectPool>();
            var pooledObject = pool.GetObject(false);
            var pooledGO = pooledObject.gameObject;

            if (!string.IsNullOrEmpty(""))
            {
                if (parent == null)
                    parent = GameObject.Find("").transform;
                else
                    parent = parent.Find("");
            }
            if (parent != null)
            {
                pooledGO.transform.SetParent(parent, false);
            }

            return pooledGO;
        }

    }
}
