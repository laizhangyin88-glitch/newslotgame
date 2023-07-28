using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;

namespace GameStudio.Slot.NDS
{
    public class NDSSevenBehaviour : SymbolBehaviour
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
            PlayAnimation("Idle");
            ClearCachedObjects();
            animator.gameObject.SetActive(true);
        }

        public override void OnStopEffect()
        {
            GetCachedObject(0);
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
            GetComponent<NDSMultiplierSelector>().Apply(symbol);
        }
    }
}
