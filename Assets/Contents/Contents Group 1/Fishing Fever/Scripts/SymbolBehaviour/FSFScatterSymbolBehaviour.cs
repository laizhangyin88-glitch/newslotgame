using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using TMPro;
using NodeCanvas.Framework;

namespace GameStudio.Slot.FSF
{
    public class FSFScatterSymbolBehaviour : SymbolBehaviour
    {
        public override void OnEntry() { }
        public override void OnWin() { }
        public override void OnSkip() { GetCachedObject(0)?.SetActive(false); PlayAnimation("Idle"); }
        public override void OnStopEffect()
        {
            var stopPrefab = GetCachedObject(0);
            PlayAnimation("Invisible");
            var originCreditText = symbol.GetComponent<FSFSymbolEventHandler>().creditText;
            originCreditText.gameObject.SetActive(false);
        }
        public override void OnPrepareStop() { }
    }
}
