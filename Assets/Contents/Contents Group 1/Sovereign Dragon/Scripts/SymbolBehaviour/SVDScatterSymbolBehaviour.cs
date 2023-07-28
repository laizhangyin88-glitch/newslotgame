using SlotMaker.Tasks.Actions;
using UnityEngine;

namespace GameStudio.Slot.SVD
{
    public class SVDScatterSymbolBehaviour : SVDCommonSymbolBehaviour
    {
        private static readonly int EggStateHash = Animator.StringToHash("Egg");
        private static readonly int TitleStateHash = Animator.StringToHash("Title");

        public override void OnEntry()
        {
            base.OnEntry();
            SimpleSetBaseActive(true);

            var symbolBB = GetSymbolBB();
            new SymbolSetSprite
            {
                spriteRenderer = GetBaseImage(symbolBB),
                spriteId = 0
            }.ExecuteAction(symbolBB, symbolBB);

            new SymbolSetActiveCachingObject
            {
                cachingId = 0,
                setTo = SymbolSetActiveCachingObject.SetActiveMode.Deactivate
            }.ExecuteAction(symbolBB, symbolBB);

            ClearSymbolGraphics();
        }

        public override void OnSkip()
        {
            var symbolBB = GetSymbolBB();
            new SymbolSetSprite
            {
                spriteRenderer = GetBaseImage(symbolBB),
                spriteId = 0
            }.ExecuteAction(symbolBB, symbolBB);

            new SymbolSetActiveCachingObject
            {
                cachingId = 0,
                setTo = SymbolSetActiveCachingObject.SetActiveMode.Deactivate
            }.ExecuteAction(symbolBB, symbolBB);

            new SymbolSetActiveCachingObject
            {
                cachingId = 1,
                setTo = SymbolSetActiveCachingObject.SetActiveMode.Deactivate
            }.ExecuteAction(symbolBB, symbolBB);
        }

        public override void OnStopEffect()
        {
            var symbolBB = GetSymbolBB();

            new SymbolSetSprite
            {
                spriteRenderer = GetBaseImage(symbolBB),
                spriteId = 1
            }.ExecuteAction(symbolBB, symbolBB);

            new SymbolGetGameObject { prefabId = 0, cachingId = 0 }.ExecuteAction(symbolBB, symbolBB);
        }

        public override void OnPrepareStop()
        {
        }

        public override void OnWin()
        {
            var symbolBB = GetSymbolBB();

            new SymbolSetSprite
            {
                spriteRenderer = GetBaseImage(symbolBB),
                spriteId = 1
            }.ExecuteAction(symbolBB, symbolBB);

            new SymbolSetActiveCachingObject
            {
                cachingId = 0,
                setTo = SymbolSetActiveCachingObject.SetActiveMode.Deactivate
            }.ExecuteAction(symbolBB, symbolBB);

            new SymbolGetGameObject { prefabId = 1, cachingId = 0 }.ExecuteAction(symbolBB, symbolBB);
        }

        private void ClearSymbolGraphics()
        {
            animator.SetInteger(EggStateHash, -1);
            animator.SetInteger(TitleStateHash, 0);
        }
    }
}
