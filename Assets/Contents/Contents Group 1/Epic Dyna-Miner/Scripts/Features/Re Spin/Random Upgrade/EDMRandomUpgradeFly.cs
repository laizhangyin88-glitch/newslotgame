using System.Collections;
using BagelCode;
using GameStudio.Slot.EDM.Utility;
using SlotMaker;
using UnityEngine;
namespace GameStudio.Slot.EDM.Feature
{
    public class EDMRandomUpgradeFly : MonoBehaviour
    {
        public PooledObject pooledObject;
        public DirectionalWeightPositionController positionController;
        EDMRandomUpgradeFlyData currentFlyingData;
        BaseSymbol targetSymbol;
        BaseSymbol startSymbol;


        public void Initialize(SlotMachine slotMachine, EDMRandomUpgradeFlyData flyingData)
        {
            currentFlyingData = flyingData;

            startSymbol = EDMUtility.GetSymbolFromSpotSlotMachine(slotMachine, flyingData.startCell);
            targetSymbol = EDMUtility.GetSymbolFromSpotSlotMachine(slotMachine, flyingData.targetCell);

            transform.position = startSymbol.transform.position;
            positionController.from = startSymbol.transform;
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

        public void ReturnToPool()
        {
            pooledObject.ReturnToPool();
        }

        private IEnumerator UpgradeJackpotCoroutine()
        {
            int count = targetSymbol.GetComponentInChildren<EDMJackpotStopController>(true).leftSpinCount;
            yield return null;
            targetSymbol.Play("Upgrade");
            yield return new WaitForSeconds(1f);
            targetSymbol.symbolIndex++;

            targetSymbol.symbolMask = (int)ContentCustomData.GetSlotData(targetSymbol.slotMachine.slotIndex).symbolMask.GetMask(targetSymbol.symbolIndex);

            targetSymbol.Change(targetSymbol.symbolInfo);
            targetSymbol.Apply();
            targetSymbol.Play("UpgradeFinish");
            targetSymbol.GetComponentInChildren<EDMJackpotStopController>().SetSpinLeftCount(count);
        }
    }

}