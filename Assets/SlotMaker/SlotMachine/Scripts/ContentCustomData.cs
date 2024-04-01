using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion;
using NodeCanvas.Framework;

namespace SlotMaker
{
    [RequireComponent(typeof(Blackboard))]
    public class ContentCustomData : MonoWeakSingleton<ContentCustomData>
    {
        public int symbolCount;

        public SymbolSortingOrder symbolSortingOrder;

        public SymbolSprite symbolSprite;
        public List<string> symbolName;

        public Blackboard gsHandler;

        public List<SlotData> slotDataList;

        public static SlotData GetSlotData(int slotIndex)
        {
            return Instance.slotDataList[slotIndex];
        }

        private void Awake()
        {
            var cb = ContentBlackboard.Get();
            BlackboardUtils.SetOrCreateValue<bool>(cb, "initializedContent", false);
            BlackboardUtils.SetOrCreateValue<Blackboard>(cb, "customData", GetComponent<Blackboard>());
            
            GSManager.Instance.handlers.Add(gsHandler);
        }

        protected override void OnDestroy()
        {
            var variables = gsHandler.variables;
            foreach (var pair in variables)
            {
                //((GSHandler)pair.Value.value).Clear();
                if (pair.Value != null)
                {
                    ((GSHandler)pair.Value.value)?.Clear();
                }
            }

            base.OnDestroy();
        }
    }
}
