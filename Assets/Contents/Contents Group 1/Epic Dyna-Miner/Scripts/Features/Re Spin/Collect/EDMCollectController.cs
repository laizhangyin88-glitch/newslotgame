using System.Collections;
using System.Collections.Generic;
using GameStudio.Slot.EDM.Utility;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;

namespace GameStudio.Slot.EDM.Feature
{
    public struct EDMCollectData
    {
        public Vector2Int startCell;
        public List<Vector2Int> collectTargetCellList;
        public List<long> collectAmountList;
        public long totalCollectAmount;
    }

    public class EDMCollectController : MonoBehaviour
    {
        public EDMRespinAdmin respinAdmin;
        public ObjectPool creditFlyingPool;
        public Transform creditFlyingPooledObjectParent;

        private bool triggeredCollect = false;
        private Blackboard currentCollectData = null;
        Dictionary<Vector2Int, int> collectCount = new Dictionary<Vector2Int, int>();
        private void OnEnable()
        {
            collectCount.Clear();
        }
        public IEnumerator CollectCoroutine(Blackboard collectData)
        {

            currentCollectData = collectData;
            triggeredCollect = false;

            Blackboard cellBB = collectData.GetVariable<Blackboard>("cell").value;
            Vector2Int cell = EDMUtility.ConvertCellBBToVector2Int(cellBB);

            BaseSymbol symbol = EDMUtility.GetSymbolFromSpotSlotMachine(respinAdmin.bonusSlotMachine, cell);
            symbol.Play("Active");
            if (symbol.symbolIndex == 13)
            {
                EDMUtility.PlaySound($"Collect vx {UnityEngine.Random.Range(1, 3)}");
            }
            else if (symbol.symbolIndex == 14 && collectCount.ContainsKey(cell) == false)
            {
                EDMUtility.PlaySound("Collect every spin vx");

            }
            if (collectCount.ContainsKey(cell))
                collectCount[cell]++;
            else
                collectCount.Add(cell, 1);

            yield return null;
            long creditAmount = collectData.GetVariable<long>("totalCollectAmount").value;
            long prevAmount = 0;
            Variable<long> varPrevAmount = collectData.GetVariable<long>("previousCollectAmount");
            if (varPrevAmount != null) prevAmount = varPrevAmount.value;
            EDMCollectAnimationController animController = symbol.GetComponentInChildren<EDMCollectAnimationController>();
            animController.Initialize(this, prevAmount);

            yield return new WaitUntil(() => triggeredCollect == true);
            yield return StartCoroutine(CollectFlyCoroutine());
            symbol.Play("InActive");
        }

        public IEnumerator CollectFlyCoroutine()
        {
            List<Blackboard> listCells = currentCollectData.GetVariable<List<Blackboard>>("collectTargetCellList").value;
            List<long> listValues = currentCollectData.GetVariable<List<long>>("collectAmountList").value;
            Blackboard cellBB = currentCollectData.GetVariable<Blackboard>("cell").value;
            Vector2Int cell = EDMUtility.ConvertCellBBToVector2Int(cellBB);
            BaseSymbol symbol = EDMUtility.GetSymbolFromSpotSlotMachine(respinAdmin.bonusSlotMachine, cell);

            List<EDMCreditFly> creditFlyList = new List<EDMCreditFly>();
            for (int i = 0; i < listCells.Count; i++)
            {
                EDMCreditFly creditFly = creditFlyingPool.GetObject().GetComponent<EDMCreditFly>();
                creditFly.transform.SetParent(creditFlyingPooledObjectParent);
                creditFly.transform.localPosition = Vector3.zero;
                creditFly.Initialize(respinAdmin.bonusSlotMachine, EDMUtility.ConvertCellBBToVector2Int(listCells[i]), cell, listValues[i]);
                yield return new WaitForSeconds(0.075f);
            }
            symbol.Play("UpdateValue");

            yield return new WaitUntil(() => creditFlyingPooledObjectParent.childCount == 0);
        }
        public void OnCollect()
        {
            triggeredCollect = true;
        }

    }
}