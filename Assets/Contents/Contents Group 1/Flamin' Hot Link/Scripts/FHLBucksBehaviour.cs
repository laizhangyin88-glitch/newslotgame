using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using ParadoxNotion;

namespace GameStudio.Slot.FHL
{
    public class FHLBucksBehaviour : SymbolBehaviour
    {
        public FHLSymbolCoinController symbolCoinController;
        private const int Win = 0;
        private bool changeToBlack = false;
        public override void OnPrepareStop()
        {
            GetCachedObject(1).GetComponentInChildren<Animator>().SetTrigger("Disappear");
            bool isRespin = BlackboardUtils.FindVariable<bool>(null, "./customData/isRespin").value;
            int rowCount = isRespin ? 8 : 4;
            int reelIndex = symbol.reel.reelIndex;
            int rowIndex = reelIndex % rowCount;
            if (symbol.row != rowIndex)
            {
                DefaultSymbolEventHandler defaultEventHandler = eventHandler as DefaultSymbolEventHandler;
                defaultEventHandler.symbolPresets[symbol.symbolIndex].value[0].spriteRenderer.sprite = symbol.symbolAssets.GetSprite(symbol.symbolIndex, 1);
                GetComponent<FHLSymbolCoinController>().coinText.gameObject.SetActive(false);
                changeToBlack = true;
            }
        }

        public override void OnSkip()
        {
            var isDeactivateCache = BlackboardUtils.FindVariable<bool>(null, "./customData/isDeactivateCache");
            if (isDeactivateCache.value)
            {
                DisableAllCachedObjects();
                animator.gameObject.SetActive(true);
                if (!changeToBlack) GetComponent<FHLSymbolCoinController>().coinText.gameObject.SetActive(true);
            }
        }

        public override void OnStopEffect()
        {
            GetComponent<FHLSymbolCoinController>().coinText.gameObject.SetActive(false);
            GameObject fireBall = GetCachedObject(0);
            // IContextText symbolCreditText = BlackboardUtils.FindVariable<ContextElement>(symbol.GetComponent<Blackboard>(), "creditText").value as IContextText;
            IContextText symbolCreditText = symbolCoinController.coinText as IContextText;
            IContextText creditText = BlackboardUtils.FindVariable<ContextElement>(fireBall.GetComponent<Blackboard>(), "creditText").value as IContextText;
            creditText.SetText(symbolCreditText.GetText());
            animator.gameObject.SetActive(false);
            var e = new EventData("Shake");
			MessageDispatcher.Dispatch("OnContentUIDetailEvent", e);
            GSManager.Instance.GetHandler("Bucks Symbol Land").Play();
        }

        public override void OnWin()
        {
            GetCachedObject(0).GetComponentInChildren<Animator>().SetTrigger("Win");
        }

        public override void OnEntry()
        {
            animator.gameObject.SetActive(true);
            changeToBlack = false;
            bool _isRespinIntro = BlackboardUtils.FindVariable<bool>(null, "./customData/_isRespinIntro").value;
            if (!_isRespinIntro)
            {
                GetCachedObject(1);
            }
            symbolCoinController.Apply(symbol);
        }

        public void ActivateFireBallSymbol()
        {
            symbolCoinController.Apply(symbol);
            GameObject fireBall = GetCachedObject(0);
            IContextText symbolCreditText = symbolCoinController.coinText as IContextText;
            IContextText creditText = BlackboardUtils.FindVariable<ContextElement>(fireBall.GetComponent<Blackboard>(), "creditText").value as IContextText;
            creditText.SetText(symbolCreditText.GetText());
            animator.gameObject.SetActive(false);
        }

        public void LockSymbol()
        {
            GetCachedObject(0).GetComponentInChildren<Animator>().SetTrigger("Fix");
        }

        public void Trigger()
        {
            GetCachedObject(0).GetComponentInChildren<Animator>().SetTrigger("Trigger");
        }
    }
}
