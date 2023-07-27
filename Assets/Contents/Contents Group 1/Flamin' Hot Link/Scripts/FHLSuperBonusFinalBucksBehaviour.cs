using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using ParadoxNotion;

namespace GameStudio.Slot.FHL
{
    public class FHLSuperBonusFinalBucksBehaviour : SymbolBehaviour
    {
        public FHLSymbolCoinController symbolCoinController;
        private const int Win = 0;
        private int coinGroupIndex = -1;
        public override void OnPrepareStop()
        {
        }

        public override void OnSkip()
        {
            DisableAllCachedObjects();
            animator.gameObject.SetActive(true);
        }

        public override void OnStopEffect()
        {
            int index = symbol.column * 8 + symbol.row;
            bool isActiveSpot = BlackboardUtils.FindVariable<List<bool>>(null, "./bonus/response/isActiveSpotList").value[index];
            if (isActiveSpot)
            {
                MessageDispatcher.Dispatch("OnContentUIDetailEvent", new EventData("Shake"));
                long beforeCredit = BlackboardUtils.FindVariable<List<long>>(null, "./bonus/response/finalBeforeCreditList").value[index];
                int multiplier = BlackboardUtils.FindVariable<List<int>>(null, "./bonus/response/multiplierList").value[index];
                long finalCredit = beforeCredit * multiplier;
                GetComponent<FHLCreditRolling>().beforeCredit = beforeCredit;
                GetComponent<FHLCreditRolling>().finalCredit = finalCredit;
            }
            // if (!isActiveSpot)
            // {
            //     // DefaultSymbolEventHandler defaultEventHandler = eventHandler as DefaultSymbolEventHandler;
            //     // defaultEventHandler.symbolPresets[symbol.symbolIndex].value[0].spriteRenderer.sprite = symbol.symbolAssets.GetSprite(symbol.symbolIndex, 1);
            // }
            // else
            // {
            //     // GetComponent<FHLSymbolCoinController>().coinText.gameObject.SetActive(false);
            //     // GameObject fireBall = GetCachedObject(0);
            //     // IContextText symbolCreditText = symbolCoinController.coinText as IContextText;
            //     // IContextText creditText = BlackboardUtils.FindVariable<ContextElement>(fireBall.GetComponent<Blackboard>(), "creditText").value as IContextText;
            //     // creditText.SetText(symbolCreditText.GetText());
            //     // animator.gameObject.SetActive(false);
            //     // MessageDispatcher.Dispatch("OnContentUIDetailEvent", new EventData("Shake"));
            //     // GSManager.Instance.GetHandler("Bucks Symbol Land").Play();

            //     // long beforeCredit = BlackboardUtils.FindVariable<List<long>>(null, "./bonus/response/finalBeforeCreditList").value[index];
            //     // int multiplier = BlackboardUtils.FindVariable<List<int>>(null, "./bonus/response/multiplierList").value[index];
            //     // long finalCredit = beforeCredit * multiplier;
            //     // BlackboardUtils.SetOrCreateValue<long>(fireBall.GetComponent<Blackboard>(), "beforeCredit", beforeCredit);
            //     // BlackboardUtils.SetOrCreateValue<long>(fireBall.GetComponent<Blackboard>(), "finalCredit", finalCredit);
            // }
        }

        public override void OnWin()
        {
            // GetCachedObject(0).GetComponentInChildren<Animator>().SetTrigger("Win");
        }

        public override void OnEntry()
        {
            symbolCoinController.Apply(symbol);
            int coinIndex = symbolCoinController.coinIndex;
            coinGroupIndex = CalCoinGroupIndex(coinIndex);
            animator.gameObject.SetActive(true);
            DefaultSymbolEventHandler defaultEventHandler = eventHandler as DefaultSymbolEventHandler;
            defaultEventHandler.symbolPresets[symbol.symbolIndex].value[0].spriteRenderer.sprite = symbol.symbolAssets.GetSprite(symbol.symbolIndex, coinGroupIndex);
        }

        public void MultiplyCredit()
        {
            int reelIndex = symbol.column * 8 + symbol.row;
            List<GameObject> frames = BlackboardUtils.FindVariable<List<GameObject>>(symbol.reel.slotMachine.GetComponent<Blackboard>(), "frames").value;
            GameObject frame = frames[reelIndex];
            BlackboardUtils.GetOrCreateVariable<Animator>(frame.GetComponent<Blackboard>(), "animator").value.SetTrigger("Apply");
            GetComponent<FHLCreditRolling>().OnCreditRolling();
        }

        private int CalCoinGroupIndex(int coinIndex)
        {
            if (coinIndex < 6)
            {
                return 0;
            }
            return 1;
        }
    }
}