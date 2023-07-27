using System;
using UnityEngine;
using SlotMaker;
using TMPro;

namespace GameStudio.Slot.CTC
{
    public class CTCScatterBehaviour : SymbolBehaviour
    {
        private readonly double[] multiplierArray = { 0.1, 0.2, 0.5, 1, 2, 3, 5, 10, 20, 50, 100 };
        private readonly int[] multiplierWeightList = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 };
        private readonly int weightTotal = 66;
        private const int MINI_JACKPOT_MULTIPLIER_INDEX = 7;

        private int fakeJackpotIndex = -1;

        public override void OnEntry()
        {
            fakeJackpotIndex = -1;
            var symbolCustomData = symbol.symbolInfo.customData;
            SpriteRenderer jackpotRenderer = symbol.GetComponentsInChildren<SpriteRenderer>(true)[1];
            TextMeshProUGUI text = symbol.GetComponentInChildren<TextMeshProUGUI>(true);

            if (symbolCustomData == null)
            {
                int seed = UnityEngine.Random.Range(0, weightTotal);
                int i = 0;
                int sum = 0;
                for (; i < multiplierWeightList.Length; i++)
                {
                    sum += multiplierWeightList[i];
                    if (seed <= sum) break;
                }

                if (i >= MINI_JACKPOT_MULTIPLIER_INDEX)
                {
                    symbol.GetComponentInChildren<TextMeshProUGUI>(true).gameObject.SetActive(false);
                    jackpotRenderer.sprite = GlobalSymbolAssets.Instance.GetSprite(symbol.symbolIndex, i - MINI_JACKPOT_MULTIPLIER_INDEX + 1);
                    jackpotRenderer.gameObject.SetActive(true);
                    text.gameObject.SetActive(false);
                    fakeJackpotIndex = i - MINI_JACKPOT_MULTIPLIER_INDEX;
                }
                else
                {
                    double fakeMultiplier = multiplierArray[i];
                    var betCredit = BlackboardUtils.FindValue<long>(null, "./betCredit");
                    text.gameObject.SetActive(true);
                    jackpotRenderer.gameObject.SetActive(false);
                    text.text = FormatUtility.SimpleNumberFormat(Convert.ToInt64(Convert.ToDouble(betCredit) * fakeMultiplier));
                }

                return;
            }

            double multiplier = (double)symbolCustomData["Multiplier"];
            int jackpotIndex = (int)symbolCustomData["JackpotIndex"];

            if (jackpotIndex >= 0)
            {
                symbol.GetComponentInChildren<TextMeshProUGUI>(true).gameObject.SetActive(false);
                jackpotRenderer.sprite = GlobalSymbolAssets.Instance.GetSprite(symbol.symbolIndex, jackpotIndex + 1);
                jackpotRenderer.gameObject.SetActive(true);
            }
            else
            {
                jackpotRenderer.gameObject.SetActive(false);
            }

            if (multiplier > 0)
            {
                var betCredit = BlackboardUtils.FindValue<long>(null, "./betCredit");
                text.gameObject.SetActive(true);
                text.text = FormatUtility.SimpleNumberFormat(Convert.ToInt64(Convert.ToDouble(betCredit) * multiplier));
            }
            else
            {
                text.gameObject.SetActive(false);
            }
        }
        public override void OnWin()
        {
            GetCachedObject(0).SetActive(false);
            var winPrefab = GetCachedObject(1);
            winPrefab.SetActive(true);
            SpriteRenderer jackpotRenderer = symbol.GetComponentsInChildren<SpriteRenderer>(true)[1];
            bool isJackpot = jackpotRenderer.gameObject.activeSelf;
            winPrefab.GetComponentInChildren<Animator>().SetInteger("JackpotIndex", isJackpot ? 0 : -1);

            if (isJackpot)
            {
                var prefabJackpotRenderer = winPrefab.GetComponentsInChildren<SpriteRenderer>(true)[1];
                prefabJackpotRenderer.sprite = symbol.GetComponentsInChildren<SpriteRenderer>(true)[1].sprite;
            }
            else
            {
                var prefabCredit = winPrefab.GetComponentInChildren<TextMeshProUGUI>(true);
                prefabCredit.text = symbol.GetComponentInChildren<TextMeshProUGUI>(true).text;
            }

            symbol.GetComponentsInChildren<SpriteRenderer>(true)[0].sprite = GlobalSymbolAssets.Instance.GetSprite(symbol.symbolIndex, 5);
        }

        public void HideRocket()
        {
            symbol.GetComponentsInChildren<SpriteRenderer>(true)[0].sprite = GlobalSymbolAssets.Instance.GetSprite(symbol.symbolIndex, 5);
        }

        public override void OnSkip()
        {
            GetCachedObject(0).SetActive(false);
            GetCachedObject(1).SetActive(false);
            PlayAnimation("Idle");
        }

        public override void OnStopEffect()
        {
            var stopPrefab = GetCachedObject(0);
            stopPrefab.SetActive(true);
            stopPrefab.GetComponentInChildren<TextMeshProUGUI>(true).text =
                symbol.GetComponentInChildren<TextMeshProUGUI>(true).text;
            SpriteRenderer jackpotRenderer = symbol.GetComponentsInChildren<SpriteRenderer>(true)[1];
            SpriteRenderer prefabJackpotRenderer = stopPrefab.GetComponentsInChildren<SpriteRenderer>(true)[3];
            prefabJackpotRenderer.sprite = jackpotRenderer.sprite;

            if (symbol.symbolInfo.customData != null)
            {
                stopPrefab.GetComponentInChildren<Animator>().SetInteger("JackpotIndex", (int)symbol.symbolInfo.customData["JackpotIndex"]);
            }
            else
            {
                stopPrefab.GetComponentInChildren<Animator>().SetInteger("JackpotIndex", fakeJackpotIndex);
            }

            BlackboardUtils.GetOrCreateVariable<BaseSymbol>(null, "./customData/_currentStoppingScatter").value = symbol;
            ContentEvent.SendEvent(CTCPotController.COLLECT_POT_EVENT);
        }
        public override void OnPrepareStop() { }
    }
}