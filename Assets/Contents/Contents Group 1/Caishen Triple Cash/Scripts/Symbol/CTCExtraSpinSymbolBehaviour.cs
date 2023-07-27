using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using SlotMaker;

namespace GameStudio.Slot.CTC {
    public class CTCExtraSpinSymbolBehaviour : SymbolBehaviour
    {
        private int spinCount;
        public override void OnEntry()
        {
            Sprite sprite;
            if (symbol.symbolInfo.customData == null)
            {
                sprite = GlobalSymbolAssets.Instance.GetSprite(symbol.symbolIndex, Random.Range(0, 3));
                animator.GetComponentInChildren<SpriteRenderer>().sprite = sprite;
                return;
            }

            spinCount = (int)symbol.symbolInfo.customData["ExtraSpinCount"];
            sprite = GlobalSymbolAssets.Instance.GetSprite(symbol.symbolIndex,
                spinCount - 1);
            animator.GetComponentInChildren<SpriteRenderer>().sprite = sprite;
        }

        public override void OnWin()
        {
            GetCachedObject(0).SetActive(false);
            var winPrefab = GetCachedObject(1);
            winPrefab.SetActive(true);
            winPrefab.GetComponentInChildren<Animator>().SetInteger("Spin Count", spinCount);
            PlayAnimation("Invisible");

        }
        public override void OnSkip() {
            GetCachedObject(0).SetActive(false);
            GetCachedObject(1).SetActive(false);
            PlayAnimation("Idle");
        }

        public override void OnStopEffect() {
            GSManager.Instance.GetHandler("Free Spin Symbol Land").Play();
            var stopPefab = GetCachedObject(0);
            stopPefab.SetActive(true);
            stopPefab.GetComponentInChildren<Animator>().SetInteger("Spin Count", spinCount); PlayAnimation("Idle"); }
        public override void OnPrepareStop() { }
    }
}