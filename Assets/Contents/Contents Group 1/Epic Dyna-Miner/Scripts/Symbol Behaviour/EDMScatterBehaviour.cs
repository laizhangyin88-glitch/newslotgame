using System;
using System.Collections.Generic;
using SlotMaker;
using TMPro;
using UnityEngine;

namespace GameStudio.Slot.EDM.SymbolBehaviour
{
    public class EDMScatterBehaviour : SlotMaker.SymbolBehaviour
    {
        const int STOP = 0;
        const int WIN = 1;

        TextMeshProUGUI valueText;
        List<long> creditValueList = null;

        public override void OnEntry()
        {
            animator.SetBool("Text", true);

            if (valueText == null)
            {
                valueText = animator.transform.Find("valueText").GetComponent<TextMeshProUGUI>();
            }

            if (creditValueList == null)
            {
                creditValueList = BlackboardUtils.FindVariable<List<long>>("./customData/creditValueList").value;
            }

            long totalBet = BlackboardUtils.FindVariable<long>("./totalBetCredit").value;
            if (symbol.symbolInfo.customData == null)
                symbol.symbolInfo.customData = new Dictionary<string, object>();

            if (symbol.symbolInfo.customData.ContainsKey("value") == false)
                symbol.symbolInfo.customData.Add("value", Convert.ToInt64(0));

            if (symbol.symbolInfo.customData != null && symbol.symbolInfo.customData.ContainsKey("value"))
            {
                if (Convert.ToInt64(symbol.symbolInfo.customData["value"]) == 0)
                    valueText.text = FormatUtility.SimpleNumberFormat(totalBet / 50 * creditValueList[UnityEngine.Random.Range(0, creditValueList.Count)]);
                else
                    valueText.text = FormatUtility.SimpleNumberFormat(Convert.ToInt64(symbol.symbolInfo.customData["value"]));
            }
            else
                valueText.text = FormatUtility.SimpleNumberFormat(totalBet / 50 * creditValueList[UnityEngine.Random.Range(0, creditValueList.Count)]);
        }

        public override void OnPrepareStop()
        {
        }

        public override void OnSkip()
        {
            PlayAnimation("Idle");
            GetCachedObject(STOP).SetActive(false);
            GetCachedObject(WIN).SetActive(false);
        }

        public override void OnStopEffect()
        {
            if (symbol.symbolInfo.customData != null && symbol.symbolInfo.customData.ContainsKey("value") && Convert.ToInt64(symbol.symbolInfo.customData["value"]) > 0 && symbol.row < 3)
            {
                PlayAnimation("InActive");
                GetCachedObject(WIN).SetActive(false);
                GetCachedObject(STOP).SetActive(true);
                GetCachedObject(STOP).transform.Find("Animator/Anchor/Counter/Number Text").GetComponent<TextMeshProUGUI>().text = FormatUtility.SimpleNumberFormat(Convert.ToInt64(symbol.symbolInfo.customData["value"]));
            }
        }

        public override void OnWin()
        {
            PlayAnimation("InActive");
            GetCachedObject(STOP).SetActive(false);
            GetCachedObject(WIN).SetActive(true);
            GetCachedObject(WIN).transform.Find("Animator/Anchor/Counter/Number Text").GetComponent<TextMeshProUGUI>().text = FormatUtility.SimpleNumberFormat(Convert.ToInt64(symbol.symbolInfo.customData["value"]));
        }

        public void BeginExpectation()
        {
            if (symbol.symbolInfo.customData != null && symbol.symbolInfo.customData.ContainsKey("value") && Convert.ToInt64(symbol.symbolInfo.customData["value"]) > 0 && symbol.row < 3)
                GetCachedObject(STOP).GetComponentInChildren<Animator>().SetBool("Expectation", true);
        }

        public void EndExpectation()
        {
            if (symbol.symbolInfo.customData != null && symbol.symbolInfo.customData.ContainsKey("value") && Convert.ToInt64(symbol.symbolInfo.customData["value"]) > 0 && symbol.row < 3)
                GetCachedObject(STOP).GetComponentInChildren<Animator>().SetBool("Expectation", false);
        }
    }
}