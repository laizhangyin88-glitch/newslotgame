using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using TMPro;
using NodeCanvas.Framework;

namespace GameStudio.Slot.FSF
{
    public class FSFHighFishSymbolBehaviour : SymbolBehaviour
    {
        public override void OnEntry() { }
        public override void OnWin()
        {
            var winPrefab = GetCachedObject(1);
            GetCachedObject(1).SetActive(true);
            GetCachedObject(0).SetActive(false);
            PlayAnimation("Invisible");

            winPrefab.GetComponentInChildren<Animator>().SetBool("IsValue", true);
            var originCreditText = symbol.GetComponent<FSFSymbolEventHandler>().creditText;
            originCreditText.gameObject.SetActive(false);
            var valueText = winPrefab.GetComponent<Blackboard>().GetValue<TextMeshProUGUI>("valueText");
            valueText.text = originCreditText.text;
        }
        public override void OnSkip() { GetCachedObject(1)?.SetActive(false); GetCachedObject(0)?.SetActive(false); PlayAnimation("Idle"); symbol.GetComponent<FSFSymbolEventHandler>().creditText.gameObject.SetActive(true); }
        public override void OnStopEffect()
        {
            GSManager.Instance.GetHandler("Symbol In").Play();
            var stopPrefab = GetCachedObject(0);
            PlayAnimation("Invisible");
            stopPrefab.GetComponentInChildren<Animator>().SetBool("IsValue", true);
            var originCreditText = symbol.GetComponent<FSFSymbolEventHandler>().creditText;
            originCreditText.gameObject.SetActive(false);
            var valueText = stopPrefab.GetComponent<Blackboard>().GetValue<TextMeshProUGUI>("valueText");
            valueText.text = originCreditText.text;
        }
        public override void OnPrepareStop() { }
    }
}
