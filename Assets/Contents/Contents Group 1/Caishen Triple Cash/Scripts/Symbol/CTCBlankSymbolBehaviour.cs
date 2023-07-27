using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using SlotMaker;
namespace GameStudio.Slot.CTC
{
    public class CTCBlankSymbolBehaviour : SymbolBehaviour
    {
        private readonly int[] weightPerSpriteIndex = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        private readonly int weightTotal = 55;


        public override void OnEntry()
        {
            int seed = Random.Range(0, weightTotal);
            int i = 0;
            int sum = 0;
            for (; i < weightPerSpriteIndex.Length; i++)
            {
                sum += weightPerSpriteIndex[i];
                if (seed <= sum) break;
            }

            var sprite = GlobalSymbolAssets.Instance.GetSprite(symbol.symbolIndex, i);
            animator.GetComponentInChildren<SpriteRenderer>().sprite = sprite;

            symbol.GetComponentInChildren<TextMeshProUGUI>(true).gameObject.SetActive(false);
            symbol.GetComponentsInChildren<SpriteRenderer>(true)[1].gameObject.SetActive(false);
        }
        public override void OnWin() { /* GetCachedObject(0)?.SetActive(true);*/ PlayAnimation("Invisible"); }
        public override void OnSkip() { /* GetCachedObject(0)?.SetActive(false);*/ PlayAnimation("Idle"); }
        public override void OnStopEffect() { }
        public override void OnPrepareStop() { }
    }
}