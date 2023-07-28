using NodeCanvas.Framework;
using SlotMaker;
using SlotMaker.Tasks.Actions;
using UnityEngine;

namespace GameStudio.Slot.SVD
{
    public class SVDDefaultSymbolBehaviour : SVDCommonSymbolBehaviour
    {
        private const int WinAnimationStateHash = -2004386410;
        private const int IdleAnimationStateHash = 2081823275;

        private readonly int EggStateHash = Animator.StringToHash("Egg");
        private readonly int TitleStateHash = Animator.StringToHash("Title");

        public override void OnEntry()
        {
            base.OnEntry();
            var symbolBB = symbol.GetComponent<SVDSymbolEventHandler>().GetSymbolBlackboard();
            BlackboardUtils.FindVariable<GameObject>(symbolBB, "base").value.SetActive(true);
            var baseImage = GetBaseImage(symbolBB);
            new SymbolSetSprite { spriteRenderer = baseImage, spriteId = 0 }.ExecuteAction(symbolBB, symbolBB);

            // -1422705371 = "Base"
            new SymbolSetSortingOrder
            {
                spriteRenderer = baseImage,
                sortingLayerId = -1422705371,
                sortingOrder = 0
            }.ExecuteAction(symbolBB, symbolBB);

            new SymbolSetActiveCachingObject
            {
                cachingId = 0,
                setTo = SymbolSetActiveCachingObject.SetActiveMode.Deactivate
            }.ExecuteAction(symbolBB, symbolBB);

            ClearSymbolGraphics();
        }

        private void ClearSymbolGraphics()
        {
            animator.SetInteger(EggStateHash, -1);
            animator.SetInteger(TitleStateHash, 0);
        }

        public override void OnSkip()
        {
            var symbolBB = GetSymbolBB();
            new SymbolSetSprite
            {
                spriteRenderer = GetBaseImage(symbolBB), spriteId = 0
            }.ExecuteAction(symbolBB, symbolBB);
            BlackboardUtils.FindVariable<GameObject>(symbolBB, "base").value.SetActive(true);
            PlayAnimation(IdleAnimationStateHash);

            // -1422705371 = "Base"
            new SymbolSetSortingOrder
            {
                spriteRenderer = GetBaseImage(symbolBB),
                sortingLayerId = -1422705371,
                sortingOrder = 0
            }.ExecuteAction(symbolBB, symbolBB);

            var symbolIndex = GetSymbolIndex(symbolBB);

            new SymbolSetActiveCachingObject
            {
                cachingId = IsSymbolHigh(symbolIndex) ? 1 : 0,
                setTo = SymbolSetActiveCachingObject.SetActiveMode.Deactivate
            }.ExecuteAction(symbolBB, symbolBB);
        }

        private int GetSymbolIndex(Blackboard symbolBB)
        {
            return BlackboardUtils.FindVariable<int>(symbolBB, "symbolIndex").value;
        }

        public override void OnStopEffect()
        {
        }

        public override void OnPrepareStop()
        {
        }

        public override void OnWin()
        {
            var symbolBB = symbol.GetComponent<SVDSymbolEventHandler>().GetSymbolBlackboard();
            var symbolIndex = GetSymbolIndex(symbolBB);

            if (IsSymbolHigh(symbolIndex))
            {
                BlackboardUtils.FindVariable<GameObject>(symbolBB, "base").value.SetActive(false);
                new SymbolSetSprite
                {
                    spriteRenderer = GetBaseImage(symbolBB), spriteId = 1
                }.ExecuteAction(symbolBB, symbolBB);
                new SymbolGetGameObject {prefabId = 0, cachingId = 1}.ExecuteAction(symbolBB, symbolBB);
            }
            else
            {
                PlayAnimation(WinAnimationStateHash);

                // 1424790303 = "Foreground"
                new SymbolSetSortingOrder
                {
                    spriteRenderer = GetBaseImage(symbolBB),
                    sortingLayerId = 1424790303,
                    sortingOrder = 0
                }.ExecuteAction(symbolBB, symbolBB);
                new SymbolGetPooledObject {poolId = 0, cachingId = 0}.ExecuteAction(symbolBB, symbolBB);
            }
        }

        private bool IsSymbolHigh(int symbolIndex)
        {
            return symbolIndex == 2;
        }
    }
}
