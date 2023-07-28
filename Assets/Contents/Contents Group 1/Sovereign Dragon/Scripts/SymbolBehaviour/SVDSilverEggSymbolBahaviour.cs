using System.Collections;
using NodeCanvas.Framework;
using SlotMaker;
using SlotMaker.Tasks.Actions;
using UnityEngine;

namespace GameStudio.Slot.SVD
{
    public class SVDSilverEggSymbolBahaviour : SVDCommonSymbolBehaviour
    {
        private const string ContextAnimatorName = "_WinAnimatorContext";

        private static readonly int EggStateHash = Animator.StringToHash("Egg");
        private static readonly int TitleStateHash = Animator.StringToHash("Title");
        private static readonly int StartFlowType = Animator.StringToHash("startFlowType");
        private static readonly int FlyHash = Animator.StringToHash("Fly");

        public override void OnEntry()
        {
            base.OnEntry();
            var symbolBB = GetSymbolBB();

            new SymbolSetSprite
            {
                spriteRenderer = GetBaseImage(symbolBB),
                spriteId = 0
            }.ExecuteAction(symbolBB, symbolBB);

            SetVariable("_finishedFly",false,GetSymbolBB());
            SetBaseImageSortingOrder(-1422705371, 3);

            new SymbolSetActiveCachingObject
            {
                cachingId = 0,
                setTo = SymbolSetActiveCachingObject.SetActiveMode.Deactivate
            }.ExecuteAction(symbolBB, symbolBB);

            SimpleSetBaseActive(true);

            BlackboardUtils.GetOrCreateVariable<int>(symbolBB, "_row").value =
                BlackboardUtils.GetOrCreateVariable<int>(symbolBB, "row").value;

            BlackboardUtils.GetOrCreateVariable<int>(symbolBB, "_column").value =
                BlackboardUtils.GetOrCreateVariable<int>(symbolBB, "column").value;

            animator.SetInteger(EggStateHash, 4);
            BlackboardUtils.GetOrCreateVariable<bool>(symbolBB, "shouldFly").value = false;
        }

        public override void OnSkip()
        {
            var symbolBB = GetSymbolBB();
            new SymbolSetSprite
            {
                spriteRenderer = GetBaseImage(symbolBB),
                spriteId = 0
            }.ExecuteAction(symbolBB, symbolBB);
            DeactivateCachingObject();
        }

        private void DeactivateCachingObject()
        {
            var symbolBB = GetSymbolBB();

            new SymbolSetActiveCachingObject
            {
                cachingId = 0,
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
        }

        #region Custom States

        public void Drop()
        {
            var symbolBB = GetSymbolBB();
            animator.SetInteger(EggStateHash, -1);

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
        }

        public void DropSymbolWin()
        {
            animator.SetInteger(TitleStateHash, 0);
            animator.SetInteger(EggStateHash, -1);

            var symbolBB = GetSymbolBB();
            new SymbolSetSprite
            {
                spriteRenderer = GetBaseImage(symbolBB),
                spriteId = 1
            }.ExecuteAction(symbolBB, symbolBB);

            BlackboardUtils.GetOrCreateVariable<bool>(symbolBB, "shouldFly").value = false;

            ClearCachedObjects();

            new SymbolGetGameObject { prefabId = 1, cachingId = 0 }.ExecuteAction(symbolBB, symbolBB);
            new UpdateContext { isForceUpdate = true }.ExecuteAction(symbolBB, symbolBB);

            BlackboardUtils.GetOrCreateVariable<ContextElement>(symbolBB, "_WinAnimatorContext").value =
                ContextUtils.FindElement(symbolBB.GetComponent<ContextElement>(), "Re-Spin Silver Win", ContextSearchingType.FullNameSearch);

            var winAnimator =
                BlackboardUtils.GetOrCreateVariable<ContextElement>(symbolBB, "_WinAnimatorContext").value;
            ((ContextAnimator) winAnimator).animator.SetInteger(StartFlowType, 1);

            BlackboardUtils.GetOrCreateVariable<ContextElement>(symbolBB, "_flyFXContextCompositor").value =
                ContextUtils.FindElement(winAnimator, "Fly FX anchor", ContextSearchingType.FullNameSearch);

            BlackboardUtils.GetOrCreateVariable<Animator>(symbolBB, "winAnimator").value =
                winAnimator.transform.GetComponent<Animator>();

            var compositorBB =
                BlackboardUtils.GetOrCreateVariable<ContextElement>(symbolBB, "_flyFXContextCompositor")
                    .value.GetComponent<Blackboard>();

            BlackboardUtils.GetOrCreateVariable<Transform>(compositorBB, "from").value =
                BlackboardUtils.GetOrCreateVariable<Transform>(symbolBB, "symbolTransform").value;

            BlackboardUtils.GetOrCreateVariable<Transform>(compositorBB, "to").value =
                BlackboardUtils.GetOrCreateVariable<RectTransform>(symbolBB, "_panelTransform").value;

            BlackboardUtils.GetOrCreateVariable<bool>(symbolBB, "shouldFly").value = true;


            if (GetVariable<bool>("./customData/canFlySymbol", symbolBB))
            {
                if (GetVariable<bool>("shouldFly", GetSymbolBB()))
                {
                    OnFly();
                }
            }

            //StartCoroutine(Fly(symbolBB));
        }

        private IEnumerator Fly(Blackboard symbolBB)
        {
            yield return new WaitUntil(() =>
                BlackboardUtils.FindVariable<bool>(symbolBB, "./customData/canFlySymbol").value);


            BlackboardUtils.GetOrCreateVariable<Animator>(symbolBB, "winAnimator").value.SetTrigger(FlyHash);
            BlackboardUtils.GetOrCreateVariable<bool>(symbolBB, "shouldFly").value = false;
        }
        public void RetryFly()
        {
            if (GetVariable<bool>("shouldFly", GetSymbolBB()))
            {
                GetContextAnimator(GetSymbolBB(), ContextAnimatorName).Play("Fly");
                OnFly();
            }
        }
        private void OnFly()
        {
            SetVariable("_finishedFly",true,GetSymbolBB());
            BlackboardUtils.GetOrCreateVariable<Animator>(GetSymbolBB(), "winAnimator").value.SetTrigger(FlyHash);
            BlackboardUtils.GetOrCreateVariable<bool>(GetSymbolBB(), "shouldFly").value = false;
        }

        public void DropResult()
        {
            DeactivateCachingObject();
        }

        public void SetBlankState()
        {
            var symbolBB = GetSymbolBB();
            new SymbolSetSprite
            {
                spriteRenderer = GetBaseImage(symbolBB),
                spriteId = 1
            }.ExecuteAction(symbolBB, symbolBB);
            animator.SetInteger(EggStateHash, -1);
        }

        private T GetVariable<T>(string name, IBlackboard agent)
        {
            return BlackboardUtils.GetOrCreateVariable<T>(agent, name).value;
        }
        private Animator GetContextAnimator(Blackboard symbolBB, string animatorName)
        {
            return ((ContextAnimator) GetVariable<ContextElement>(animatorName, symbolBB)).animator;
        }
        private void SetVariable<T>(string name, T value, Blackboard agent)
        {
            BlackboardUtils.GetOrCreateVariable<T>(agent, name).value = value;
        }
        #endregion
    }
}
