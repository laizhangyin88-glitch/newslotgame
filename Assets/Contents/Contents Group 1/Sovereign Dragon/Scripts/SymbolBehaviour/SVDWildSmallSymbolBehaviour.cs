using SlotMaker;
using NodeCanvas.Framework;
using SlotMaker.Tasks.Actions;
using UnityEngine;

namespace GameStudio.Slot.SVD
{
    public class SVDWildSmallSymbolBehaviour : SVDCommonSymbolBehaviour
    {
        private const int BaseLayerHash = -1422705371;

        private Blackboard symbolBB;
        public override void OnEntry()
        {
            base.OnEntry();
            symbolBB = symbol.GetComponent<SVDSymbolEventHandler>().GetSymbolBlackboard();

            GetVariable<GameObject>("base", symbolBB).SetActive(true);
            SymbolSetSprite(GetBaseImage(symbolBB), 0, symbolBB);
            SymbolSetSortingOrder(GetBaseImage(symbolBB), BaseLayerHash, 0, symbolBB);
            SymbolSetActiveCachingObjectExecute(0, SymbolSetActiveCachingObject.SetActiveMode.Deactivate, symbolBB);
        }

        public override void OnSkip()
        {
            SymbolSetSprite(GetBaseImage(symbolBB), 0, symbolBB);
            GetVariable<GameObject>("base", symbolBB).SetActive(true);
            SymbolSetSortingOrder(GetBaseImage(symbolBB), BaseLayerHash, 0, symbolBB);
            SymbolSetActiveCachingObjectExecute(0, SymbolSetActiveCachingObject.SetActiveMode.Deactivate, symbolBB);
            SymbolSetActiveCachingObjectExecute(1, SymbolSetActiveCachingObject.SetActiveMode.Deactivate, symbolBB);
        }

        public override void OnStopEffect()
        {
            SymbolSetSprite(GetBaseImage(symbolBB), 1, symbolBB);
            SymbolGetGameObject(0, 0, symbolBB);
            GetVariable<GameObject>("base", symbolBB).SetActive(false);
        }

        public override void OnPrepareStop()
        {
        }

        public override void OnWin()
        {
            SymbolSetSprite(GetBaseImage(symbolBB), 1, symbolBB);
            gameObject.SetActive(false);
            SymbolSetActiveCachingObjectExecute(0, SymbolSetActiveCachingObject.SetActiveMode.Deactivate, symbolBB);
            SymbolGetGameObject(1, 0, symbolBB);
        }

        private T GetVariable<T>(string name, IBlackboard agent)
        {
            return BlackboardUtils.GetOrCreateVariable<T>(agent, name).value;
        }

        private void SymbolSetSprite(SpriteRenderer spriteRenderer, int spriteId, Blackboard agent)
        {
            new SymbolSetSprite
            {
                spriteRenderer = spriteRenderer,
                spriteId = spriteId
            }.ExecuteAction(agent, symbolBB);
        }

        private void SymbolSetActiveCachingObjectExecute(int cachingId, SymbolSetActiveCachingObject.SetActiveMode mode, Blackboard agent)
        {
            new SymbolSetActiveCachingObject
            {
                cachingId = cachingId,
                setTo = mode
            }.ExecuteAction(agent, symbolBB);
        }
        private void SymbolSetSortingOrder(SpriteRenderer spriteRenderer, int sortingLayerId, int sortingOrder, Blackboard agent)
        {
            new SymbolSetSortingOrder
            {
                spriteRenderer = spriteRenderer,
                sortingLayerId = sortingLayerId,
                sortingOrder = sortingOrder
            }.ExecuteAction(agent, symbolBB);
        }
        private void SymbolGetGameObject(int prefabId, int cachingId, Blackboard agent)
        {
            new SymbolGetGameObject
            {
                prefabId = prefabId,
                cachingId = cachingId
            }.ExecuteAction(agent, symbolBB);
        }
    }
}
