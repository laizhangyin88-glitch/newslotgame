using System;
using System.Collections.Generic;
using GameStudio.Slot.EDM.Utility;
using SlotMaker;
using TMPro;
using UnityEngine;
namespace GameStudio.Slot.EDM.SymbolBehaviour
{
    public class EDMCreditBehaviour : SlotMaker.SymbolBehaviour
    {
        public int STOP = 0;
        public int COLLECT = 1;
        public int UPGRADE = 2;
        public int WIN = 3;
        public int INACTIVE = 4;

        TextMeshProUGUI valueText;
        List<long> creditValueList = null;

        private void UpdateValueText()
        {
            if (valueText == null)
                valueText = animator.transform.Find("valueText").GetComponent<TextMeshProUGUI>();

            if (creditValueList == null)
                creditValueList = BlackboardUtils.FindVariable<List<long>>("./customData/creditValueList").value;
            long totalBet = BlackboardUtils.FindVariable<long>("./totalBetCredit").value;
            valueText.text = FormatUtility.SimpleNumberFormat(totalBet / 50 * creditValueList[UnityEngine.Random.Range(0, creditValueList.Count)]);
            if (symbol.symbolInfo.customData != null && symbol.symbolInfo.customData.ContainsKey("value"))
            {
                if (Convert.ToInt64(symbol.symbolInfo.customData["value"]) <= 0)
                    valueText.text = FormatUtility.SimpleNumberFormat(totalBet / 50 * creditValueList[UnityEngine.Random.Range(0, creditValueList.Count)]);
                else
                    valueText.text = FormatUtility.SimpleNumberFormat(Convert.ToInt64(symbol.symbolInfo.customData["value"]));
            }
            else
                valueText.text = FormatUtility.SimpleNumberFormat(totalBet / 50 * creditValueList[UnityEngine.Random.Range(0, creditValueList.Count)]);

        }


        public override void OnEntry()
        {
            PlayAnimation("Idle");
            animator.SetBool("Text", true);
            UpdateValueText();
        }

        public override void OnPrepareStop()
        {
        }

        public override void OnSkip()
        {
            PlayAnimation("Idle");
            GetCachedObject(STOP).SetActive(false);
            GetCachedObject(COLLECT).SetActive(false);
            GetCachedObject(UPGRADE).SetActive(false);
            GetCachedObject(WIN).SetActive(false);
            GetCachedObject(INACTIVE).SetActive(false);

        }

        public override void OnStopEffect()
        {
            int rowCount = BlackboardUtils.FindVariable<int>("./customData/currentRowCount").value;
            bool isRespin = BlackboardUtils.FindVariable<bool>("./customData/isRespin").value;
            if (isRespin == true && 5 - symbol.row < rowCount)
            {
                PlayAnimation("InActive");
                GetCachedObject(COLLECT).SetActive(false);
                GetCachedObject(UPGRADE).SetActive(false);
                GetCachedObject(WIN).SetActive(false);
                ContentEvent.SendEvent("EDM_ON_RESET_SPIN_COUNT");
                GetCachedObject(STOP).SetActive(true);
                GetCachedObject(STOP).transform.Find("Animator/Anchor/Counter/Number Text").GetComponent<TextMeshProUGUI>().text = FormatUtility.SimpleNumberFormat(Convert.ToInt64(symbol.symbolInfo.customData["value"]));
                GetCachedObject(INACTIVE).SetActive(false);
            }
            EDMUtility.PlaySound("Gold Land");
        }

        public override void OnWin()
        {
            PlayAnimation("InActive");
            GetCachedObject(STOP).SetActive(false);
            GetCachedObject(COLLECT).SetActive(false);
            GetCachedObject(UPGRADE).SetActive(false);
            GetCachedObject(WIN).SetActive(true);
            GetCachedObject(INACTIVE).SetActive(false);
        }

        public void Upgrade()
        {
            PlayAnimation("InActive");
            GetCachedObject(STOP).SetActive(false);
            GetCachedObject(COLLECT).SetActive(false);
            GetCachedObject(WIN).SetActive(false);
            UpdateValueText();

            GetCachedObject(UPGRADE).SetActive(false);
            GetCachedObject(UPGRADE).SetActive(true);
            GetCachedObject(UPGRADE).GetComponentInChildren<Animator>().SetTrigger("Upgrade");
            GetCachedObject(UPGRADE).transform.Find("Animator/Anchor/Counter/Number Text").GetComponent<TextMeshProUGUI>().text = FormatUtility.SimpleNumberFormat(Convert.ToInt64(symbol.symbolInfo.customData["oldValue"]));
            GetCachedObject(UPGRADE).transform.Find("Animator/Upgrade Number Text").GetComponent<TextMeshProUGUI>().text = FormatUtility.SimpleNumberFormat(Convert.ToInt64(symbol.symbolInfo.customData["value"]) - Convert.ToInt64(symbol.symbolInfo.customData["oldValue"]));
            GetCachedObject(UPGRADE).transform.Find("Animator/New Upgrade Number Text").GetComponent<TextMeshProUGUI>().text = FormatUtility.SimpleNumberFormat(Convert.ToInt64(symbol.symbolInfo.customData["value"]));
            GetCachedObject(INACTIVE).SetActive(false);
        }

        public void Collect()
        {
            PlayAnimation("InActive");
            GetCachedObject(STOP).SetActive(false);
            GetCachedObject(COLLECT).SetActive(true);
            GetCachedObject(COLLECT).transform.Find("Animator/Anchor/Counter/Number Text").GetComponent<TextMeshProUGUI>().text = FormatUtility.SimpleNumberFormat(Convert.ToInt64(symbol.symbolInfo.customData["value"]));
            GetCachedObject(COLLECT).GetComponentInChildren<Animator>().SetTrigger("Collect");
            GetCachedObject(UPGRADE).SetActive(false);
            GetCachedObject(WIN).SetActive(false);
            GetCachedObject(INACTIVE).SetActive(false);
        }
        public void InActiveFinalCollect()
        {
            PlayAnimation("InActive");
            GetCachedObject(STOP).SetActive(false);
            GetCachedObject(COLLECT).SetActive(false);
            GetCachedObject(UPGRADE).SetActive(false);
            GetCachedObject(WIN).SetActive(false);
            GetCachedObject(INACTIVE).SetActive(true);
        }

    }
}