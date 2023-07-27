using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;

namespace GameStudio.Slot.HOC
{
    public class HOCHighSymbolBehaviour : SymbolBehaviour
    {
        private bool isWild;
        public override void OnEntry()
        {
            isWild = false;
        }

        public void TurnToWild()
        {
            animator.Play("Invisible");
            GetCachedObject(0).SetActive(true);
            GetCachedObject(1).SetActive(false);

            var wildSprite = symbol.symbolAssets.GetSprite(symbol.symbolIndex, 1);
            animator.GetComponentInChildren<SpriteRenderer>(true).sprite = wildSprite;
            isWild = true;
        }


        public override void OnSkip()
        {
            animator.Play("Idle");
            GetCachedObject(0).SetActive(false);
            GetCachedObject(1).SetActive(false);
        }

        public override void OnStopEffect()
        {
            animator.Play("Invisible");
            GetCachedObject(0).SetActive(true);
        }

        public override void OnPrepareStop()
        {
        }

        public override void OnWin()
        {
            animator.Play("Invisible");
            if (isWild)
            {
                GetCachedObject(1).SetActive(false);
                var winprefab = GetCachedObject(0);
                winprefab.SetActive(false);
                winprefab.SetActive(true);
                winprefab.GetComponentInChildren<Animator>().SetTrigger("Win");
            }
            else
            {
                GetCachedObject(0).SetActive(false);
                GetCachedObject(1).SetActive(true);
            }
        }
    }
}
