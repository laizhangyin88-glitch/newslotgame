using System;
using System.Collections.Generic;
using GameStudio.Slot.EDM.Utility;
using SlotMaker;
using TMPro;
using UnityEngine;
namespace GameStudio.Slot.EDM.Feature
{
    public class EDMCreditFly : MonoBehaviour
    {
        public PooledObject pooledObject;
        public DirectionalWeightPositionController positionController;
        public TextMeshProUGUI valueText;
        private long ownedCredit;

        private BaseSymbol startSymbol;
        private BaseSymbol endSymbol;

        public void Initialize(SlotMachine slotMachine, Vector2Int startPos, Vector2Int targetPos, long credit)
        {
            Vector2Int from = startPos;
            Vector2Int to = targetPos;

            startSymbol = EDMUtility.GetSymbolFromSpotSlotMachine(slotMachine, from);
            endSymbol = EDMUtility.GetSymbolFromSpotSlotMachine(slotMachine, to);
            startSymbol.Play("Collect");
            transform.position = startSymbol.transform.position;

            positionController.from = startSymbol.transform;
            positionController.to = endSymbol.transform;

            ownedCredit = credit;

            valueText.text = FormatUtility.SimpleNumberFormat(credit);
        }

        public void OnArrive()
        {
            if (endSymbol.symbolInfo.customData == null) endSymbol.symbolInfo.customData = new Dictionary<string, object>();
            if (endSymbol.symbolInfo.customData.ContainsKey("value") == false)
                endSymbol.symbolInfo.customData.Add("value", (object)((long)0));

            long currentCredit = Convert.ToInt64(endSymbol.symbolInfo.customData["value"]);
            currentCredit += ownedCredit;
            endSymbol.symbolInfo.customData["value"] = (object)currentCredit;
            endSymbol.GetComponentInChildren<EDMCollectAnimationController>().OnFlyArrive(ownedCredit);
        }
    }

}