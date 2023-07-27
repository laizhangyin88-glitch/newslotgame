using System;
using System.Collections.Generic;
using GameStudio.Slot.EDM.Feature;
using GameStudio.Slot.EDM.Utility;
using SlotMaker;
using UnityEngine;
namespace GameStudio.Slot.EDM.SymbolBehaviour
{
    public class EDMJackpotBehaviour : SlotMaker.SymbolBehaviour
    {
        public const int STOP = 0;
        public const int OPEN = 1;
        public const int OPEN_LOCK = 2;
        public const int INACTIVE = 3;
        public const int UPGRDE = 4;


        public override void OnEntry()
        {
            animator.SetBool("Text", false);
            if (symbol.symbolInfo.customData == null) symbol.symbolInfo.customData = new Dictionary<string, object>();
            if (symbol.symbolInfo.customData.ContainsKey("value"))
                symbol.symbolInfo.customData["value"] = 0;
            else
                symbol.symbolInfo.customData.Add("value", (object)Convert.ToInt64(0));
        }

        public override void OnPrepareStop()
        {

        }

        public override void OnSkip()
        {
            PlayAnimation("Idle");
            GetCachedObject(STOP).SetActive(false);
            GetCachedObject(OPEN).SetActive(false);
            GetCachedObject(INACTIVE).SetActive(false);
            GetCachedObject(OPEN_LOCK).SetActive(false);
            if (symbol.symbolIndex != 17)
                GetCachedObject(UPGRDE).SetActive(false);
        }

        public override void OnStopEffect()
        {
            int rowCount = BlackboardUtils.FindVariable<int>("./customData/currentRowCount").value;
            bool isRespin = BlackboardUtils.FindVariable<bool>("./customData/isRespin").value;
            if ((isRespin == true && 5 - symbol.row < rowCount) || isRespin == false)
            {
                PlayAnimation("InActive");
                GetCachedObject(STOP).SetActive(true);
                EDMJackpotStopController stopController = GetCachedObject(STOP).GetComponent<EDMJackpotStopController>();
                stopController.Initialize();
                GetCachedObject(OPEN).SetActive(false);
                GetCachedObject(INACTIVE).SetActive(false);
                GetCachedObject(OPEN_LOCK).SetActive(false);
                if (isRespin == true && 5 - symbol.row < rowCount)
                {
                    ContentEvent.SendEvent("EDM_ON_RESET_SPIN_COUNT");
                    stopController.SetValueTextActive(true);
                }
                else stopController.SetValueTextActive(false);
            }
            if (symbol.symbolIndex == 15)
                EDMUtility.PlaySound("Low Jackpot Chest Land");
            else if (symbol.symbolIndex == 16)
                EDMUtility.PlaySound("Mid Jackpot Chest Land");
            else if (symbol.symbolIndex == 17)
                EDMUtility.PlaySound("High Jackpot Chest Land");

        }

        public void StopEffectWithExpand()
        {
            int rowCount = BlackboardUtils.FindVariable<int>("./customData/currentRowCount").value;
            bool isRespin = BlackboardUtils.FindVariable<bool>("./customData/isRespin").value;
            if ((isRespin == true && 5 - symbol.row < rowCount) || isRespin == false)
            {
                PlayAnimation("InActive");
                GetCachedObject(STOP).SetActive(true);
                EDMJackpotStopController stopController = GetCachedObject(STOP).GetComponent<EDMJackpotStopController>();
                stopController.Initialize();
                GetCachedObject(OPEN).SetActive(false);
                GetCachedObject(INACTIVE).SetActive(false);
                GetCachedObject(OPEN_LOCK).SetActive(false);
                if (isRespin == true && 5 - symbol.row < rowCount)
                {
                    ContentEvent.SendEvent("EDM_ON_RESET_SPIN_COUNT");
                    stopController.SetValueTextActive(true);
                }
                else stopController.SetValueTextActive(false);
            }

        }
        public override void OnWin()
        {
        }

        public void Open()
        {
            PlayAnimation("InActive");
            GetCachedObject(STOP).SetActive(false);
            GetCachedObject(OPEN).SetActive(true);

            int jackpotIndex = BlackboardUtils.FindVariable<int>("./customData/jackpotIndex").value;
            GetCachedObject(OPEN).GetComponentInChildren<Animator>().SetInteger("jackpotIndex", jackpotIndex);
            EDMJackpotOpenController jackpotOpenController = GetCachedObject(OPEN).GetComponentInChildren<EDMJackpotOpenController>();
            jackpotOpenController.flyingFromTransform = symbol.transform;
            jackpotOpenController.Initalize();
            GetCachedObject(OPEN_LOCK).SetActive(false);
            GetCachedObject(INACTIVE).SetActive(false);
            EDMUtility.PlaySound($"Chest Open vx {UnityEngine.Random.Range(1, 3)}");

            if (symbol.symbolIndex == 15)
                EDMUtility.PlaySound("Low Jackpot Chest Open");
            else if (symbol.symbolIndex == 16)
                EDMUtility.PlaySound("Mid Jackpot Chest Open");
            else if (symbol.symbolIndex == 17)
                EDMUtility.PlaySound("High Jackpot Chest Open");

        }
        public void OpenIdle()
        {
            PlayAnimation("InActive");
            GetCachedObject(STOP).SetActive(false);
            GetCachedObject(OPEN).SetActive(false);
            GetCachedObject(INACTIVE).SetActive(false);
            GetCachedObject(OPEN_LOCK).SetActive(true);
            int jackpotIndex = BlackboardUtils.FindVariable<int>("./customData/jackpotIndex").value;
            GetCachedObject(OPEN_LOCK).GetComponentInChildren<Animator>().SetInteger("jackpotIndex", jackpotIndex);
        }

        public void Upgrade()
        {
            int rowCount = BlackboardUtils.FindVariable<int>("./customData/currentRowCount").value;
            bool isRespin = BlackboardUtils.FindVariable<bool>("./customData/isRespin").value;
            EDMUtility.PlaySound("Chest Upgrade");
            PlayAnimation("InActive");
            int leftSpinCount = GetCachedObject(STOP).GetComponent<EDMJackpotStopController>().leftSpinCount;
            GetCachedObject(STOP).SetActive(false);
            GetCachedObject(UPGRDE).SetActive(true);
            GetCachedObject(UPGRDE).GetComponent<EDMJackpotUpgradeController>().Initialize(leftSpinCount);
            GetCachedObject(UPGRDE).GetComponent<EDMJackpotUpgradeController>().SetValueTextActive(isRespin == true && (rowCount - 4) - (2 - Mathf.Floor(symbol.reel.reelIndex / 5)) >= 0);
            if (isRespin)
                GetCachedObject(UPGRDE).GetComponent<EDMJackpotUpgradeController>().Skip();
            GetCachedObject(OPEN).SetActive(false);
            GetCachedObject(OPEN_LOCK).SetActive(false);
            GetCachedObject(INACTIVE).SetActive(false);
        }


        public void UpgradeFinish()
        {
            this.eventHandler.ClearCachedObjects();
            int rowCount = BlackboardUtils.FindVariable<int>("./customData/currentRowCount").value;
            bool isRespin = BlackboardUtils.FindVariable<bool>("./customData/isRespin").value;
            PlayAnimation("InActive");
            GetCachedObject(STOP).SetActive(true);
            EDMJackpotStopController stopController = GetCachedObject(STOP).GetComponent<EDMJackpotStopController>();
            stopController.SetValueTextActive(isRespin == true && 5 - symbol.row < rowCount);
            stopController.Initialize();
            stopController.ForceIdle();
            if (isRespin)
                stopController.Skip();
            GetCachedObject(OPEN).SetActive(false);
            GetCachedObject(INACTIVE).SetActive(false);
            GetCachedObject(OPEN_LOCK).SetActive(false);
        }

        public void InActiveFinalCollect()
        {
            PlayAnimation("InActive");
            GetCachedObject(STOP).SetActive(false);
            GetCachedObject(OPEN).SetActive(false);
            if (symbol.symbolIndex != 17)
                GetCachedObject(UPGRDE).SetActive(false);
            GetCachedObject(OPEN_LOCK).SetActive(false);
            GetCachedObject(INACTIVE).SetActive(true);
        }
    }
}
