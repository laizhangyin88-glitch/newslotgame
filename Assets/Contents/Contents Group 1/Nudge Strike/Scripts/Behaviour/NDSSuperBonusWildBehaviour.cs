using System.Diagnostics.SymbolStore;
using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;

namespace GameStudio.Slot.NDS
{
    public class NDSSuperBonusWildBehaviour : SymbolBehaviour
    {
        public NDSMotionBlur motionBlur;
        public override void OnPrepareStop()
        {
        }

        public override void OnSkip()
        {
            DefaultSymbolEventHandler defaultEventHandler = eventHandler as DefaultSymbolEventHandler;
            SpriteRenderer spriteRenderer = defaultEventHandler.symbolPresets[symbol.symbolIndex].value[0].spriteRenderer;
            spriteRenderer.sortingLayerID = SortingLayer.NameToID("Base");
            animator.gameObject.SetActive(true);
            PlayAnimation("Idle");
            ClearCachedObjects();
        }

        public override void OnStopEffect()
        {
            if (1 < symbol.row && symbol.row < 5)
            {
                animator.gameObject.SetActive(false);
                GetCachedObject(0);
            }
        }

        public override void OnWin()
        {
            DefaultSymbolEventHandler defaultEventHandler = eventHandler as DefaultSymbolEventHandler;
            SpriteRenderer spriteRenderer = defaultEventHandler.symbolPresets[symbol.symbolIndex].value[0].spriteRenderer;
            spriteRenderer.sortingLayerID = SortingLayer.NameToID("Midground");
            PlayAnimation("Win");
        }
        public override void OnEntry()
        {
            PlayAnimation("Idle");
            animator.gameObject.SetActive(true);
            motionBlur.symbol = symbol;
            GetComponent<NDSWildMultiplierContorller>().Apply(symbol);
        }
        public void ReadyNudgeDown()
        {
            GetCachedObject(1).SetActive(true);
        }
        public void ReadyNudgeUp()
        {
            GetCachedObject(2).SetActive(true);
        }
    }
}
