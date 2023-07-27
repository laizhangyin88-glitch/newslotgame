using UnityEngine;
using SlotMaker;
using TMPro;
namespace GameStudio.Slot.CTC
{
    public class CTCDefaultSymbolBehaviour : SymbolBehaviour
    {
        public override void OnEntry()
        {
            symbol.GetComponentInChildren<TextMeshProUGUI>(true).gameObject.SetActive(false);
            symbol.GetComponentsInChildren<SpriteRenderer>(true)[1].gameObject.SetActive(false);
        }
        public override void OnWin() { PlayAnimation("Win"); symbol.GetComponentInChildren<SpriteRenderer>().sortingLayerName = "Foreground"; }
        public override void OnSkip() { PlayAnimation("Idle"); symbol.GetComponentInChildren<SpriteRenderer>().sortingLayerName = "Base"; }
        public override void OnStopEffect() { }
        public override void OnPrepareStop() { }
    }
}