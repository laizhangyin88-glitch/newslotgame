using UnityEngine;
using SlotMaker;
using TMPro;

namespace GameStudio.Slot.CTC
{
    public class CTCHighSymbolBehaviour : SymbolBehaviour
    {
        public override void OnEntry()
        {
            symbol.GetComponentInChildren<TextMeshProUGUI>(true).gameObject.SetActive(false);
            symbol.GetComponentsInChildren<SpriteRenderer>(true)[1].gameObject.SetActive(false);
        }
        public override void OnWin() { GetCachedObject(0).SetActive(true); PlayAnimation("Invisible"); }
        public override void OnSkip()
        {
            GetCachedObject(0).SetActive(false); PlayAnimation("Idle");
        }
        public override void OnStopEffect() { }
        public override void OnPrepareStop() { }
    }
}