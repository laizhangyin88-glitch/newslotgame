using NodeCanvas.Framework;
using SlotMaker;
using SlotMaker.Tasks.Actions;

namespace GameStudio.Slot.SVD
{
    public class SVDWildTallSymbolBehaviour : SVDCommonSymbolBehaviour
    {
        private readonly int WinDownStateNameHash = -1898419772;
        private readonly int WinTopStateNameHash = -1885551826;
        private readonly int WinMidDownStateNameHash = -1780499026;
        private readonly int WinMidTopStateNameHash = -58838443;

        public override void OnEntry()
        {
            base.OnEntry();
            var symbolBB = GetSymbolBB();

            // -1422705371 = "Base"
            SetBaseImageSortingOrder(-1422705371, 0);
            SimpleSetBaseActive(false);
            BlackboardUtils.GetOrCreateVariable<bool>(symbolBB, "_needPlaySound").value = true;

            new SymbolSetSprite
            {
                spriteRenderer = GetBaseImage(symbolBB),
                spriteId = BlackboardUtils.FindVariable<bool>(symbolBB, "isPivot").value ? 0 : 1
            }.ExecuteAction(symbolBB, symbolBB);

            SimpleSetBaseActive(true);
        }

        public override void OnSkip()
        {
            var symbolBB = GetSymbolBB();
            SimpleSetBaseActive(true);

            new SymbolSetSprite
            {
                spriteRenderer = GetBaseImage(symbolBB),
                spriteId = 0
            }.ExecuteAction(symbolBB, symbolBB);

            // -1422705371 = "Base"
            SetBaseImageSortingOrder(-1422705371, 0);

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

            new SimpleStopGameSound { id = "Wild Stack Dragon Animation" }.ExecuteAction(symbolBB, symbolBB);
        }

        public override void OnStopEffect()
        {
        }

        public override void OnPrepareStop()
        {
        }

        public override void OnWin()
        {
            var symbolBB = GetSymbolBB();
            SimpleSetBaseActive(false);

            new SymbolSetSprite
            {
                spriteRenderer = GetBaseImage(symbolBB),
                spriteId = 1
            }.ExecuteAction(symbolBB, symbolBB);

            if (!BlackboardUtils.FindVariable<bool>(symbolBB, "isPivot").value)
            {
                return;
            }

            var row = BlackboardUtils.FindVariable<int>(symbolBB, "row").value;

            if (row == 2)
            {
                WinRegularSymbol(symbolBB);
            }
            else
            {
                WinSlicedSymbol(symbolBB, row);
            }
        }

        public void LinkOriginWin()
        {
            var symbolBB = GetSymbolBB();
            new SymbolGetPooledObject { poolId = 1, cachingId = 2 }.ExecuteAction(symbolBB, symbolBB);
        }

        private void WinRegularSymbol(Blackboard symbolBB)
        {
            new SymbolGetGameObject { prefabId = 0, cachingId = 0 }.ExecuteAction(symbolBB, symbolBB);
            PlayWinSound(symbolBB);
        }

        private void WinSlicedSymbol(Blackboard symbolBB, int row)
        {
            new SymbolGetGameObject { prefabId = 1, cachingId = 0 }.ExecuteAction(symbolBB, symbolBB);

            switch (row)
            {
                case 0:
                    PlayCachedWinAnimation(symbolBB, WinDownStateNameHash, true);
                    break;
                case 1:
                    PlayCachedWinAnimation(symbolBB, WinMidDownStateNameHash);
                    PlayWinSound(symbolBB);
                    break;
                case 3:
                    PlayCachedWinAnimation(symbolBB, WinMidTopStateNameHash);
                    PlayWinSound(symbolBB);
                    break;
                case 4:
                    PlayCachedWinAnimation(symbolBB, WinTopStateNameHash, true);
                    break;
            }
        }

        private void PlayCachedWinAnimation(Blackboard symbolBB, int animationHash, bool shouldClearSound = false)
        {
            new SymbolAnimatorPlayCachingObject
            {
                cachingId = 0, stateNameHash = animationHash
            }.ExecuteAction(symbolBB, symbolBB);

            if (shouldClearSound)
            {
                BlackboardUtils.GetOrCreateVariable<bool>(symbolBB, "_needPlaySound").value = false;
            }
        }

        private void PlayWinSound(Blackboard symbolBB)
        {
            if (BlackboardUtils.GetOrCreateVariable<bool>(symbolBB, "_needPlaySound").value)
            {
                new SimplePlayGameSound { id = "Wild Stack Dragon Animation" }.ExecuteAction(symbolBB, symbolBB);
                BlackboardUtils.GetOrCreateVariable<bool>(symbolBB, "_needPlaySound").value = false;
            }
        }
    }
}
