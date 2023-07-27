using System.Collections;
using System.Collections.Generic;
using GameStudio.Slot.EDM.Utility;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using UnityEngine;
namespace GameStudio.Slot.EDM.Feature
{
    public enum EDMUpgradeCellType { CREDIT = 0, JACKPOT = 1 }


    public struct EDMRandomUpgradeFlyData
    {
        public EDMUpgradeCellType type;
        public Vector2Int startCell;
        public Vector2Int targetCell;
        public long targetCreditUpgradeAmount;
    }

    public class EDMRandomUpgradeController : FeatureModule
    {
        public EDMRespinAdmin respinAdmoin;
        public ObjectPool upgradeFlyingPool;
        private SlotMachine bonusSlotMachine;
        public Transform upgradeFlyingPooledObjectParent;

        public List<EDMRandomUpgradeFlyData> flyingDataQueue = new List<EDMRandomUpgradeFlyData>();

        private void Awake()
        {
            RegisterEvent("EDM_BEGIN_FLY_RANDOM_UPGRADE", BeginFlyUpgrade);
            RegisterEvent("EDM_END_FLY_RANDOM_UPGRADE", EndFlyUpgrade);
        }

        public IEnumerator UpgradeCoroutine(SlotMachine slotMachine, List<Blackboard> upgradeDataList)
        {
            bonusSlotMachine = slotMachine;

            for (int i = 0; i < upgradeDataList.Count; i++)
            {
                Blackboard cellBB = upgradeDataList[i].GetVariable<Blackboard>("cell").value;
                Vector2Int startCell = EDMUtility.ConvertCellBBToVector2Int(cellBB);

                EDMUpgradeCellType upgradeType = EDMUpgradeCellType.CREDIT;
                if (upgradeDataList[i].GetVariable<Blackboard>("targetCreditUpgradeCell") == null)
                    upgradeType = EDMUpgradeCellType.JACKPOT;


                Blackboard targetCellBB = upgradeType == EDMUpgradeCellType.CREDIT ?
                upgradeDataList[i].GetVariable<Blackboard>("targetCreditUpgradeCell").value :
                upgradeDataList[i].GetVariable<Blackboard>("targetJackpotUpgradeCell").value;
                Vector2Int targetCell = EDMUtility.ConvertCellBBToVector2Int(targetCellBB);


                BaseSymbol symbol = EDMUtility.GetSymbolFromSpotSlotMachine(slotMachine, startCell);
                symbol.Play("Skip");
                yield return null;
                symbol.Play("Active");

                flyingDataQueue.Add(new EDMRandomUpgradeFlyData()
                {
                    type = upgradeType,
                    startCell = startCell,
                    targetCell = targetCell,
                    targetCreditUpgradeAmount = upgradeType == EDMUpgradeCellType.CREDIT ?
                    upgradeDataList[i].GetVariable<long>("targetCreditUpgradeAmount").value : -1
                });

                yield return new WaitForSeconds(1f);
            }
            yield return new WaitUntil(() => upgradeFlyingPooledObjectParent.childCount > 0);
            yield return new WaitUntil(() => upgradeFlyingPooledObjectParent.childCount == 0);

            for (int i = 0; i < upgradeDataList.Count; i++)
            {
                Blackboard cellBB = upgradeDataList[i].GetVariable<Blackboard>("cell").value;
                Vector2Int startCell = EDMUtility.ConvertCellBBToVector2Int(cellBB);
                BaseSymbol symbol = EDMUtility.GetSymbolFromSpotSlotMachine(slotMachine, startCell);
                symbol.Play("ActiveDisappear");
            }

            yield return new WaitForSeconds(1f);
        }

        public IEnumerator UpgradeEverySpinCoroutine(SlotMachine slotMachine, List<Blackboard> upgradeDataList)
        {
            bonusSlotMachine = slotMachine;

            for (int i = 0; i < upgradeDataList.Count; i++)
            {
                Blackboard cellBB = upgradeDataList[i].GetVariable<Blackboard>("cell").value;
                Vector2Int startCell = EDMUtility.ConvertCellBBToVector2Int(cellBB);
                BaseSymbol featureSymbol = EDMUtility.GetSymbolFromSpotSlotMachine(slotMachine, startCell);

                featureSymbol.Play("Skip");
                yield return null;

                List<Blackboard> upgradeCreditCellList = upgradeDataList[i].GetVariable<List<Blackboard>>("targetCreditUpgradeCell").value;
                if (upgradeCreditCellList.Count > 0)
                {
                    List<long> upgradeCreditAmountList = upgradeDataList[i].GetVariable<List<long>>("targetCreditUpgradeAmount").value;


                    for (int j = 0; j < upgradeCreditCellList.Count; j++)
                    {
                        Blackboard creditCellBB = upgradeCreditCellList[j];
                        long creditAmount = upgradeCreditAmountList[j];
                        flyingDataQueue.Add(new EDMRandomUpgradeFlyData()
                        {
                            type = EDMUpgradeCellType.CREDIT,
                            startCell = startCell,
                            targetCell = EDMUtility.ConvertCellBBToVector2Int(creditCellBB),
                            targetCreditUpgradeAmount = creditAmount,
                        });
                    }
                }
                List<Blackboard> upgradeChestCellList = upgradeDataList[i].GetVariable<List<Blackboard>>("targetJackpotUpgradeCell").value;
                if (upgradeChestCellList.Count > 0)
                {
                    for (int j = 0; j < upgradeChestCellList.Count; j++)
                    {
                        Blackboard chestCellBB = upgradeChestCellList[j];
                        flyingDataQueue.Add(new EDMRandomUpgradeFlyData()
                        {
                            type = EDMUpgradeCellType.JACKPOT,
                            startCell = startCell,
                            targetCell = EDMUtility.ConvertCellBBToVector2Int(chestCellBB),
                            targetCreditUpgradeAmount = 0,
                        });
                    }
                }
                while (flyingDataQueue.Count > 0)
                {
                    featureSymbol.Play("Active");
                    yield return new WaitUntil(() => upgradeFlyingPooledObjectParent.childCount > 0);
                    yield return new WaitUntil(() => upgradeFlyingPooledObjectParent.childCount == 0);
                }
            }
        }


        private void BeginFlyUpgrade(EventData eventData)
        {
            while (flyingDataQueue.Count > 0)
            {
                EDMRandomUpgradeFly flyingInstance = upgradeFlyingPool.GetObject().GetComponentInChildren<EDMRandomUpgradeFly>();

                EDMRandomUpgradeFlyData flyingData = flyingDataQueue[0];
                flyingInstance.Initialize(bonusSlotMachine, flyingData);
                flyingInstance.transform.parent.SetParent(upgradeFlyingPooledObjectParent);
                flyingDataQueue.RemoveAt(0);
            }
        }

        private void EndFlyUpgrade(EventData eventData)
        {
            EDMRandomUpgradeFlyData flyingData = flyingDataQueue[0];
        }
    }
}