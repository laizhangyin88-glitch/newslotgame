using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using TMPro;
using NodeCanvas.Framework;

namespace GameStudio.Slot.FSF
{
    public class FSFJackpotSymbolBehaviour : SymbolBehaviour
    {
        public override void OnEntry() { }
        public override void OnWin()
        {
            var winPrefab = GetCachedObject(1);
            GetCachedObject(1).SetActive(true);
            winPrefab.GetComponentInChildren<Animator>(true).SetBool("IsJackpot", true);
            GetCachedObject(0).SetActive(false);
            PlayAnimation("Invisible");

            var symbolJackpotRenderer = symbol.GetComponent<FSFSymbolEventHandler>().jackpotRenderer;
            // symbolJackpotRenderer.gameObject.SetActive(false);
            var prefabJackpotRenderer = winPrefab.GetComponent<Blackboard>().GetValue<SpriteRenderer>("jackpotRenderer");
            prefabJackpotRenderer.gameObject.SetActive(true);
            prefabJackpotRenderer.sprite = symbolJackpotRenderer.sprite;
        }
        public override void OnSkip() { GetCachedObject(1)?.SetActive(false); GetCachedObject(0)?.SetActive(false); PlayAnimation("Idle"); symbol.GetComponent<FSFSymbolEventHandler>().creditText.gameObject.SetActive(false); }
        public override void OnStopEffect()
        {
            var stopPrefab = GetCachedObject(0);
            stopPrefab.GetComponentInChildren<Animator>(true).SetBool("IsJackpot", true);
            PlayAnimation("Invisible");
            var symbolJackpotRenderer = symbol.GetComponent<FSFSymbolEventHandler>().jackpotRenderer;
            // symbolJackpotRenderer.gameObject.SetActive(false);
            var prefabJackpotRenderer = stopPrefab.GetComponent<Blackboard>().GetValue<SpriteRenderer>("jackpotRenderer");
            prefabJackpotRenderer.gameObject.SetActive(true);
            prefabJackpotRenderer.sprite = symbolJackpotRenderer.sprite;
        }
        public override void OnPrepareStop() { }
    }
}
