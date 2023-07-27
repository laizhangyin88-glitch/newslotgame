using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using SlotMaker;
using TMPro;

namespace GameStudio.Slot.FSF
{
    public class FSFDefaultSymbolBehaviour : SymbolBehaviour
    {
        public override void OnEntry() { }
        public override void OnWin() { GetCachedObject(0)?.SetActive(true); PlayAnimation("Invisible"); }
        public override void OnSkip() { GetCachedObject(0)?.SetActive(false); PlayAnimation("Idle"); }
        public override void OnStopEffect() { GSManager.Instance.GetHandler("Symbol In").Play(); }
        public override void OnPrepareStop() { }
    }
}