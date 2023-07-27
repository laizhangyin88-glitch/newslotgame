using System;
using System.Collections.Generic;
using GameStudio.Slot.EDM.Utility;
using SlotMaker;
using TMPro;
using UnityEngine;
namespace GameStudio.Slot.EDM.SymbolBehaviour
{
    public class EDMCollectBehaviour : SlotMaker.SymbolBehaviour
    {
        public int STOP = 0;
        public int ACTIVE = 1;
        public int INACTIVE = 2;

        TextMeshProUGUI valueText;
        TextMeshProUGUI activeValueText;

        public void UpdateValueText()
        {
            if (valueText == null)
                valueText = animator.transform.Find("valueText").GetComponent<TextMeshProUGUI>();
            if (activeValueText == null)
            {
                activeValueText = GetCachedObject(ACTIVE).transform.Find("Animator/Anchor/Counter/Number Text").GetComponent<TextMeshProUGUI>();
                GetCachedObject(ACTIVE).SetActive(false);
            }


            if (symbol.symbolInfo.customData != null && symbol.symbolInfo.customData.ContainsKey("value"))
            {
                if (activeValueText != null)
                    activeValueText.text = FormatUtility.SimpleNumberFormat(Convert.ToInt64(symbol.symbolInfo.customData["value"]));
                if (valueText != null)
                    valueText.text = FormatUtility.SimpleNumberFormat(Convert.ToInt64(symbol.symbolInfo.customData["value"]));
            }

            else
            {
                if (activeValueText != null)
                    activeValueText.text = FormatUtility.SimpleNumberFormat(0);
                if (valueText != null)
                    valueText.text = activeValueText.text = FormatUtility.SimpleNumberFormat(0);
            }

        }

        public override void OnEntry()
        {
            if (symbol.symbolInfo.customData != null && symbol.symbolInfo.customData.ContainsKey("value"))
                symbol.symbolInfo.customData["value"] = 0;
            animator.SetBool("Text", false);

            UpdateValueText();
        }

        public override void OnPrepareStop()
        {
        }

        public override void OnSkip()
        {
            PlayAnimation("Idle");
            GetCachedObject(STOP).SetActive(false);
            GetCachedObject(ACTIVE).SetActive(false);
            GetCachedObject(INACTIVE).SetActive(false);
        }

        public override void OnStopEffect()
        {
            int rowCount = BlackboardUtils.FindVariable<int>("./customData/currentRowCount").value;
            bool isRespin = BlackboardUtils.FindVariable<bool>("./customData/isRespin").value;
            if (isRespin == true && 5 - symbol.row < rowCount)
            {
                ContentEvent.SendEvent("EDM_ON_RESET_SPIN_COUNT");
                PlayAnimation("InActive");
                GetCachedObject(STOP).SetActive(true);
                GetCachedObject(ACTIVE).SetActive(false);
                GetCachedObject(INACTIVE).SetActive(false);
            }
            EDMUtility.PlaySound("Collect Symbol Land");
        }

        public override void OnWin()
        {
        }

        public void Active()
        {
            PlayAnimation("InActive");
            if (symbol.symbolInfo.customData == null)
                symbol.symbolInfo.customData = new Dictionary<string, object>();

            if (symbol.symbolInfo.customData.ContainsKey("value"))
                symbol.symbolInfo.customData["value"] = 0;
            else
                symbol.symbolInfo.customData.Add("value", 0);

            UpdateValueText();
            GetCachedObject(STOP).SetActive(false);
            GetCachedObject(ACTIVE).SetActive(true);
            GetCachedObject(INACTIVE).SetActive(false);
            GetCachedObject(ACTIVE).GetComponentInChildren<Animator>().SetBool("Active", true);
            GetCachedObject(ACTIVE).GetComponentInChildren<Animator>().SetBool("Collect", true);
        }
        public void InActive()
        {
            GetCachedObject(ACTIVE).GetComponentInChildren<Animator>().SetBool("Active", false);
            GetCachedObject(ACTIVE).GetComponentInChildren<Animator>().SetBool("Collect", false);
        }
        public void InActiveFinalCollect()
        {
            PlayAnimation("InActive");
            GetCachedObject(ACTIVE).SetActive(false);
            GetCachedObject(STOP).SetActive(false);
            GetCachedObject(INACTIVE).SetActive(true);
        }
    }
}