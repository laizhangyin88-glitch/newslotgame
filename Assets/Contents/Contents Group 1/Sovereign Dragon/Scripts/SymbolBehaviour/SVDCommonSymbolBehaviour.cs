using NodeCanvas.Framework;
using SlotMaker;
using SlotMaker.Tasks.Actions;
using UnityEngine;

namespace GameStudio.Slot.SVD
{
    public abstract class SVDCommonSymbolBehaviour : SymbolBehaviour
    {
        private readonly int TitleAnimationStateHash = Animator.StringToHash("Title");
        private readonly int EggStateHash = Animator.StringToHash("Egg");
        private readonly int JPStateHash = Animator.StringToHash("JP");

        private Blackboard _symbolBB;

        public override void OnEntry()
        {
            animator.SetInteger(TitleAnimationStateHash, 0);
            animator.SetInteger(EggStateHash, -1);
            animator.SetInteger(JPStateHash, -1);
        }

        protected Blackboard GetSymbolBB()
        {
            if (_symbolBB == null)
            {
                _symbolBB = symbol.GetComponent<SVDSymbolEventHandler>().GetSymbolBlackboard();
            }

            return _symbolBB;
        }

        protected void SimpleSetBaseActive(bool isActive)
        {
            BlackboardUtils.FindVariable<GameObject>(GetSymbolBB(), "base").value.SetActive(isActive);
        }

        protected void SetBaseImageSortingOrder(int sortingLayerId, int sortingOrder)
        {
            var symbolBB = GetSymbolBB();

            // -1422705371 = "Base"
            new SymbolSetSortingOrder
            {
                spriteRenderer = GetBaseImage(symbolBB),
                sortingLayerId = sortingLayerId,
                sortingOrder = sortingOrder
            }.ExecuteAction(symbolBB, symbolBB);
        }

        protected SpriteRenderer GetBaseImage(Blackboard symbolBB)
        {
            return BlackboardUtils.FindVariable<SpriteRenderer>(symbolBB, "baseImage").value;
        }
    }
}
