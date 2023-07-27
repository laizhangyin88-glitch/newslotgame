using System.Collections;
using BagelCode;
using GameStudio.Slot.EDM.Utility;
using SlotMaker;
using UnityEngine;
namespace GameStudio.Slot.EDM.Feature
{
    public struct EDMBaseGameRandomUpgradeFlyData
    {
        public EDMUpgradeCellType type;
        public Vector2Int targetCell;
        public long targetCreditUpgradeAmount;
    }

    public class EDMBaseGameRandomUpgradeFly : MonoBehaviour
    {
        public PooledObject pooledObject;
        public DirectionalWeightPositionController positionController;
        EDMBaseGameRandomUpgradeFlyData currentFlyingData;
        BaseSymbol targetSymbol;

        public void Initialize(SlotMachine slotMachine, EDMBaseGameRandomUpgradeFlyData flyingData)
        {
            currentFlyingData = flyingData;
            targetSymbol = EDMUtility.GetSymbolFromSpotSlotMachine(slotMachine, flyingData.targetCell);
            positionController.from = transform;
            positionController.to = targetSymbol.transform;
        }

        public void OnArrive()
        {
            if (currentFlyingData.type == EDMUpgradeCellType.CREDIT)
            {
                targetSymbol.symbolInfo.customData.SetOrAddValue("oldValue", targetSymbol.symbolInfo.customData["value"]);
                targetSymbol.symbolInfo.customData["value"] = currentFlyingData.targetCreditUpgradeAmount;

                targetSymbol.Play("Upgrade");
            }
            else
            {
                StartCoroutine(UpgradeJackpotCoroutine());
            }
        }

        private IEnumerator UpgradeJackpotCoroutine()
        {
            targetSymbol.Play("Upgrade");
            yield return new WaitForSeconds(1f);
            targetSymbol.symbolIndex++;

            targetSymbol.symbolMask = (int)ContentCustomData.GetSlotData(targetSymbol.slotMachine.slotIndex).symbolMask.GetMask(targetSymbol.symbolIndex);

            targetSymbol.Change(targetSymbol.symbolInfo);
            targetSymbol.Apply();
            targetSymbol.Play("UpgradeFinish");
            targetSymbol.GetComponentInChildren<EDMJackpotStopController>().leftSpinCount = 10;
        }
    }

}