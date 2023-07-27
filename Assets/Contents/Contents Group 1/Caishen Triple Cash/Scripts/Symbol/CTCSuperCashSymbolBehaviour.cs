using System.Collections;
using System.Collections.Generic;
using System;
using NodeCanvas.Framework;
using UnityEngine;
using SlotMaker;
using TMPro;

namespace GameStudio.Slot.CTC
{
    public class CTCSuperCashSymbolBehaviour : SymbolBehaviour
    {
        private bool wasHavingCredit;
        private double previousMultiplier;

        public override void OnEntry()
        {
            var symbolCustomData = symbol.symbolInfo.customData;
            wasHavingCredit = false;
            symbol.GetComponentInChildren<TextMeshProUGUI>(true).gameObject.SetActive(false);
            symbol.GetComponentsInChildren<SpriteRenderer>(true)[1].gameObject.SetActive(false);
            if (symbolCustomData == null) return;
        }

    public override void OnWin()
        {
            var winPrefab = GetCachedObject(1);


            winPrefab.SetActive(false);
            winPrefab.SetActive(true);
            PlayAnimation("Invisible");
        }

        public override void OnSkip()
        {
            GetCachedObject(0).SetActive(false);

            PlayAnimation("Idle");
        }

        public override void OnStopEffect()
        {
            var bonusGameResponseList = CTCPostSpinController.BonusGameResponseList;
            int spunCount = BlackboardUtils.FindValue<int>(null, "./bonusSpunCount");
            List<double> thisSpinMutiplierPerReel =
                bonusGameResponseList[bonusGameResponseList.Count - symbol.slotMachine.slotIndex]
                    .GetValue<List<Blackboard>>("cashMultiplierListPerSpin")[spunCount].GetValue<List<double>>("value");

            var payUpReelIndexList = bonusGameResponseList[bonusGameResponseList.Count - symbol.slotMachine.slotIndex]
                    .GetValue<List<Blackboard>>("payUpReelIndexListPerSpin")[spunCount].GetValue<List<int>>("value");
            var thisSpinSymbolPerReel = bonusGameResponseList[bonusGameResponseList.Count - symbol.slotMachine.slotIndex]
                   .GetValue<List<Blackboard>>("symbolPerReelPerSpin")[spunCount].GetValue<List<int>>("value");
            if (IsAbleToCalcSuperCash(symbol.slotMachine, thisSpinMutiplierPerReel, payUpReelIndexList, thisSpinSymbolPerReel) == false) return;
            GSManager.Instance.GetHandler("Super Cash Symbol Land").Play();
            ContentEvent.SendEvent(CTCPostSpinController.ADD_LOCKED_REEL_COUNT_UI_EVENT);

            var stopPrefab = GetCachedObject(0);
            stopPrefab.SetActive(true);

            StartCoroutine(CTCUtill.CallActionAfterDelay(() =>
            {
                symbol.gameObject.SetActive(false);
                var overlaySymbol = symbol.slotMachine.overlay.AddSymbol(symbol.column, 0, symbol.symbolInfo);
                overlaySymbol.Apply();
                overlaySymbol.Play("Win");
            }, 0.5f));
        }

        public static bool IsAbleToCalcSuperCash(BaseSlotMachine slotMachine, List<double> thisSpinMultiplierPerReel, List<int> thisSpinPayupReelList, List<int> thisSpinSymbolPerReel)
        {
            for (int reelIndex = 0; reelIndex < slotMachine.ColumnCount; reelIndex++)
            {
                var lockedSymbol = slotMachine.GetOverlaySymbol(new Cell(reelIndex, 0).GetHashCode());
                if (lockedSymbol == null) continue;
                else if (lockedSymbol.symbolIndex == CTCPostSpinController.CASH_SYMBOL_INDEX && (double)lockedSymbol.symbolInfo.customData["Multiplier"] > 0) return true;
            }

            foreach (var multiplier in thisSpinMultiplierPerReel)
            {
                if (multiplier > 0) return true;
            }

            foreach (var payupReel in thisSpinPayupReelList)
            {
                var lockedSymbol = slotMachine.GetOverlaySymbol(new Cell(payupReel, 0).GetHashCode());
                var isLockedCashSymbol = lockedSymbol != null ? lockedSymbol.symbolIndex == CTCPostSpinController.CASH_SYMBOL_INDEX : false;

                if (isLockedCashSymbol) return true;
                else if (thisSpinSymbolPerReel[payupReel] == CTCPostSpinController.CASH_SYMBOL_INDEX) return true;
            }

            return false;
        }

        public override void OnPrepareStop() { }

        public void RefreshMultiplier()
        {
            var symbolCustomData = symbol.symbolInfo.customData;

            double multiplier = (double)symbolCustomData["Multiplier"];

            var betCredit = BlackboardUtils.FindValue<long>(null, "./betCredit");
            var text = symbol.GetComponentInChildren<TextMeshProUGUI>(true);
            text.gameObject.SetActive(true);
            text.text = FormatUtility.SimpleNumberFormat(Convert.ToInt64(Convert.ToDouble(betCredit) * multiplier));


            var winPrefab = GetCachedObject(1);
            var winPrefabBB = winPrefab.GetComponent<Blackboard>();
            var winAnimator = winPrefab.GetComponentInChildren<Animator>();

            winPrefab.SetActive(false);
            winPrefab.SetActive(true);
            winPrefabBB.GetValue<TextMeshProUGUI>("creditText").text = text.text;

            winAnimator.SetTrigger("IncreaseCredit");
            double addedMultiplier = multiplier - previousMultiplier;
            winPrefabBB.GetValue<TextMeshProUGUI>("increaseCreditText").text = FormatUtility.SimpleNumberFormat(Convert.ToInt64(Convert.ToDouble(betCredit) * addedMultiplier));

            if (!wasHavingCredit)
            {
                wasHavingCredit = true;
                winAnimator.SetTrigger("AddCredit");
            }
            else
            {
                winAnimator.SetTrigger("AlreadyHasCredit");
            }
            previousMultiplier = multiplier;
        }


        public void Dim()
        {
            GetCachedObject(0).SetActive(false);
            GetCachedObject(1).SetActive(false);
            PlayAnimation("Idle");

            symbol.GetComponentInChildren<SpriteRenderer>(true).sprite = GlobalSymbolAssets.Instance.GetSprite(symbol.symbolIndex, 1);
        }
    }
}