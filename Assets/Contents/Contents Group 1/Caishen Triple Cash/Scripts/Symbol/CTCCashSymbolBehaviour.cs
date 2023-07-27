using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using TMPro;

namespace GameStudio.Slot.CTC
{
    public class CTCCashSymbolBehaviour : SymbolBehaviour
    {
        private readonly double[] multiplierArray = { 0.1, 0.2, 0.5, 1, 2, 3, 5, 10, 20, 50, 100 };
        private readonly int[] multiplierWeightList = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 };
        private readonly int weightTotal = 66;
        private const int MINI_JACKPOT_MULTIPLIER_INDEX = 7;

        private double previousMultiplier;
        private bool wasHavingCredit;

        public override void OnEntry()
        {
            var symbolCustomData = symbol.symbolInfo.customData;
            SpriteRenderer jackpotRenderer = symbol.GetComponentsInChildren<SpriteRenderer>(true)[1];
            var text = symbol.GetComponentInChildren<TextMeshProUGUI>(true);

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
                }
                else
                {
                    double fakeMultiplier = multiplierArray[i];
                    var betCredit = BlackboardUtils.FindValue<long>(null, "./betCredit");
                    jackpotRenderer.gameObject.SetActive(false);
                    text.gameObject.SetActive(true);
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
                wasHavingCredit = true;
                var betCredit = BlackboardUtils.FindValue<long>(null, "./betCredit");
                text.gameObject.SetActive(true);
                text.text = FormatUtility.SimpleNumberFormat(Convert.ToInt64(Convert.ToDouble(betCredit) * multiplier));
            }
            else
            {
                wasHavingCredit = false;
                symbol.GetComponentInChildren<TextMeshProUGUI>(true).gameObject.SetActive(false);
            }

            previousMultiplier = multiplier;
        }
        public override void OnWin()
        {
            GetCachedObject(0).SetActive(false);
            GetCachedObject(1).SetActive(false);
            GetCachedObject(4).SetActive(false);
            symbol.GetComponentInChildren<TextMeshProUGUI>(true).gameObject.SetActive(false);

            var symbolCustomData = symbol.symbolInfo.customData;
            int jackpotIndex = (int)symbolCustomData["JackpotIndex"];

            if (jackpotIndex >= 0)
            {
                wasHavingCredit = false;
                var winPrefab = GetCachedObject(3);

                winPrefab.SetActive(true);
                var winAnimator = winPrefab.GetComponentInChildren<Animator>();
                winAnimator.SetInteger("JackpotIndex", jackpotIndex);
                winPrefab.GetComponent<Blackboard>().GetValue<TextMeshProUGUI>("creditText").gameObject.SetActive(false);
            }
            else
            {
                wasHavingCredit = true;
                var winPrefab = GetCachedObject(2);

                winPrefab.SetActive(true);
                var creditText = winPrefab.GetComponent<Blackboard>().GetValue<TextMeshProUGUI>("creditText");
                creditText.text = symbol.GetComponentInChildren<TextMeshProUGUI>(true).text;
                creditText.gameObject.SetActive(true);
            }

            PlayAnimation("Invisible");
        }
        public override void OnSkip()
        {
            GetCachedObject(0).SetActive(false);
            GetCachedObject(1).SetActive(false);
            //GetCachedObject(2).SetActive(false);
            //GetCachedObject(3).SetActive(false);
            GetCachedObject(4).SetActive(false);
            // PlayAnimation("Idle");
        }

        public override void OnStopEffect()
        {
            ContentEvent.SendEvent(CTCPostSpinController.ADD_LOCKED_REEL_COUNT_UI_EVENT);
            var symbolCustomData = symbol.symbolInfo.customData;
            int jackpotIndex = (int)symbolCustomData["JackpotIndex"];
            symbol.GetComponentInChildren<TextMeshProUGUI>(true).gameObject.SetActive(false);
            PlayAnimation("Invisible");

            if (jackpotIndex >= 0)
            {
                GSManager.Instance.GetHandler("Jackpot Symbol Land").Play();
                var stopPrefab = GetCachedObject(1);

                stopPrefab.SetActive(true);
                stopPrefab.GetComponentInChildren<Animator>().SetInteger("JackpotIndex", jackpotIndex);
            }
            else
            {
                GSManager.Instance.GetHandler("LB Cash Symbol Land").Play();
                var stopPrefab = GetCachedObject(0);

                stopPrefab.SetActive(true);
                stopPrefab.GetComponentInChildren<TextMeshProUGUI>().text =
                    symbol.GetComponentInChildren<TextMeshProUGUI>(true).text;
            }

            StartCoroutine(CTCUtill.CallActionAfterDelay(() =>
            {
                var overlaySymbol = symbol.slotMachine.overlay.AddSymbol(symbol.column, 0, symbol.symbolInfo);
                overlaySymbol.Apply();
                overlaySymbol.Play("Win");
                symbol.gameObject.SetActive(false);
            }, 0.5f));
        }

        public override void OnPrepareStop() { }

        public void RefreshMultiplier()
        {
            symbol.GetComponentInChildren<TextMeshProUGUI>(true).gameObject.SetActive(false);
            var symbolCustomData = symbol.symbolInfo.customData;
            SpriteRenderer jackpotRenderer = symbol.GetComponentsInChildren<SpriteRenderer>(true)[1];

            double multiplier = (double)symbolCustomData["Multiplier"];
            int jackpotIndex = (int)symbolCustomData["JackpotIndex"];

            GameObject winPrefab = jackpotIndex >= 0 ? GetCachedObject(3) : GetCachedObject(2);
            winPrefab.SetActive(true);
            Animator winAniamtor = winPrefab.GetComponentInChildren<Animator>();
            Blackboard winPrefabBB = winPrefab.GetComponent<Blackboard>();

            if (jackpotIndex >= 0)
            {
                winAniamtor.SetInteger("JackpotIndex", jackpotIndex);
                symbol.GetComponentInChildren<TextMeshProUGUI>(true).gameObject.SetActive(false);
                jackpotRenderer.sprite = GlobalSymbolAssets.Instance.GetSprite(symbol.symbolIndex, jackpotIndex + 1);
                animator.SetBool("IsJackpot", true);
            }
            else
            {
                jackpotRenderer.gameObject.SetActive(false);
            }

            if (multiplier > 0)
            {
                animator.SetBool("IsCredit", true);
                var betCredit = BlackboardUtils.FindValue<long>(null, "./betCredit");
                var text = symbol.GetComponentInChildren<TextMeshProUGUI>(true);
                text.text = FormatUtility.SimpleNumberFormat(Convert.ToInt64(Convert.ToDouble(betCredit) * multiplier));

                if (wasHavingCredit)
                {
                    winAniamtor.SetTrigger("IncreaseCredit");
                    double addedMultiplier = multiplier - previousMultiplier;
                    winPrefabBB.GetValue<TextMeshProUGUI>("increaseCreditText").text = FormatUtility.SimpleNumberFormat(Convert.ToInt64(Convert.ToDouble(betCredit) * addedMultiplier));
                    winPrefabBB.GetValue<TextMeshProUGUI>("creditText").text = text.text;
                }
                else
                {
                    winAniamtor.SetTrigger("AddCredit");
                    var prefabCreditText = winPrefabBB.GetValue<TextMeshProUGUI>("creditText");
                    prefabCreditText.gameObject.SetActive(true);
                    prefabCreditText.text = text.text;
                    wasHavingCredit = true; 
                }
            }
            else
            {
                symbol.GetComponentInChildren<TextMeshProUGUI>(true).gameObject.SetActive(false);
            }

            previousMultiplier = multiplier;
        }

        public void Lock()
        {
            GSManager.Instance.GetHandler("LB Cash Symbol Appear").Play();
            PlayAnimation("Invisible");
            var symbolCustomData = symbol.symbolInfo.customData;
            int jackpotIndex = (int)symbolCustomData["JackpotIndex"];
            var prefab = GetCachedObject(4);
            bool isJackpot = jackpotIndex >= 0;
            prefab.SetActive(true);

            var prefabText = prefab.GetComponentInChildren<TextMeshProUGUI>(true);
            var prefabJackpotRenderer = prefab.GetComponentsInChildren<SpriteRenderer>(true)[2];

            if (isJackpot)
            {
                prefabJackpotRenderer.sprite = symbol.GetComponentsInChildren<SpriteRenderer>()[1].sprite;
                prefabJackpotRenderer.gameObject.SetActive(true);
                prefabText.gameObject.SetActive(false);
            }
            else
            {
                prefabText.text =
                    symbol.GetComponentInChildren<TextMeshProUGUI>(true).text;
                prefabText.gameObject.SetActive(true);
                prefabJackpotRenderer.gameObject.SetActive(false);
            }

            Invoke("OnWin", 1.5f);
        }

        public void Dim()
        {
            GetCachedObject(0).SetActive(false);
            GetCachedObject(1).SetActive(false);
            GetCachedObject(2).SetActive(false);
            GetCachedObject(3).SetActive(false);
            GetCachedObject(4).SetActive(false);
            PlayAnimation("Idle");

           var myText = symbol.GetComponentInChildren<TextMeshProUGUI>(true);
           var myJackpotRenderer = symbol.GetComponentsInChildren<SpriteRenderer>(true)[1];
            var overlaidSymbol = symbol.slotMachine.GetSymbol(symbol.column, 0);
            overlaidSymbol.gameObject.SetActive(true);
            overlaidSymbol.Play("Skip");
            overlaidSymbol.GetComponentInChildren<SpriteRenderer>(true).sprite = GlobalSymbolAssets.Instance.GetSprite(symbol.symbolIndex, 5);

            var overlaidText = overlaidSymbol.GetComponentInChildren<TextMeshProUGUI>(true);
            var overlaidJackpotRenderer =overlaidSymbol.GetComponentsInChildren<SpriteRenderer>(true)[1];
            var overlaidAnimator = overlaidSymbol.GetComponentInChildren<Animator>();
            overlaidAnimator.SetBool("IsCredit", animator.GetBool("IsCredit"));
            overlaidAnimator.SetBool("IsJackpot", animator.GetBool("IsJackpot"));
            overlaidText.text = myText.text;
            overlaidJackpotRenderer.sprite = myJackpotRenderer.sprite;
            overlaidText.gameObject.SetActive((double)symbol.symbolInfo.customData["Multiplier"] > 0);
            overlaidJackpotRenderer.gameObject.SetActive((int)symbol.symbolInfo.customData["JackpotIndex"] >= 0);
            symbol.gameObject.SetActive(false);
        }
    }
}