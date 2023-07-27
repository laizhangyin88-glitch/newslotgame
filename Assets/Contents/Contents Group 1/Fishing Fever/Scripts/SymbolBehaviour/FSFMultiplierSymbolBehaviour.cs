using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using TMPro;
using NodeCanvas.Framework;

namespace GameStudio.Slot.FSF
{
    public class FSFMultiplierSymbolBehaviour : SymbolBehaviour
    {

        private GameObject cachedMultiplierIncreasePrefab;
        public const int MULTIPLY_INCREASE_FRAME = 50;

        public override void OnEntry()
        {
        }

        public override void OnWin()
        {
            if(symbol.symbolIndex == 15) GSManager.Instance.GetHandler("Big Fish Win").Play();
            else GSManager.Instance.GetHandler("Small Fish Win").Play();

            var winPrefab = GetCachedObject(1);
            winPrefab.SetActive(false);
            winPrefab.SetActive(true);
            var animator = winPrefab.GetComponentInChildren<Animator>();
            animator.SetBool("IsJackpot", false);
            animator.SetBool("IsMultiplierAnim", false);
            GetCachedObject(0).SetActive(false);
            PlayAnimation("Invisible");

            var originCreditText =   symbol.GetComponent<FSFSymbolEventHandler>().creditText;
            originCreditText.gameObject.SetActive(false);
            var valueText = winPrefab.GetComponent<Blackboard>().GetValue<TextMeshProUGUI>("multiplierText");
            valueText.text = originCreditText.text;
            valueText.gameObject.SetActive(true);
        }

        public void OnMultiply()
        {
            if (symbol.symbolIndex == 15) GSManager.Instance.GetHandler("Big Fish Win").Play();
            else GSManager.Instance.GetHandler("Small Fish Win").Play();

            var winPrefab = GetCachedObject(1);
            var animator = winPrefab.GetComponentInChildren<Animator>();
            cachedMultiplierIncreasePrefab = winPrefab;
            GetCachedObject(1).SetActive(true);
            animator.SetBool("IsJackpot", false);
            animator.SetBool("IsMultiplierAnim", true);
            GetCachedObject(0).SetActive(false);
            PlayAnimation("Invisible");
        }

        public override void OnSkip() { GetCachedObject(1)?.SetActive(false); GetCachedObject(0)?.SetActive(false); PlayAnimation("Idle"); symbol.GetComponent<FSFSymbolEventHandler>().creditText.gameObject.SetActive(true); }
        public override void OnStopEffect()
        {
            GSManager.Instance.GetHandler("Symbol In").Play();
            var stopPrefab = GetCachedObject(0);
            stopPrefab.GetComponentInChildren<Animator>().SetBool("IsJackpot", false);
            PlayAnimation("Invisible");
            var originCreditText = symbol.GetComponent<FSFSymbolEventHandler>().creditText;
            originCreditText.gameObject.SetActive(false);
            var valueText = stopPrefab.GetComponent<Blackboard>().GetValue<TextMeshProUGUI>("multiplierText");
            valueText.text = originCreditText.text;
        }
        public override void OnPrepareStop() { }
    }
}
