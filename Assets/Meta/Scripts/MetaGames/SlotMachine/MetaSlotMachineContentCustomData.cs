using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode
{
    [RequireComponent(typeof(Blackboard))]
    public class MetaSlotMachineContentCustomData : MonoWeakSingleton<MetaSlotMachineContentCustomData>
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
            var cb = GemJackpotUtils.GemJackpotInfo;
            BlackboardUtils.SetOrCreateValue<bool>(cb, "initializedContent", false);
            BlackboardUtils.SetOrCreateValue<Blackboard>(cb, "customData", GetComponent<Blackboard>());

            GSManager.Instance.handlers.Add(gsHandler);
        }

        protected override void OnDestroy()
        {
            var variables = gsHandler.variables;
            foreach (var pair in variables)
            {
                ((GSHandler)pair.Value.value).Clear();
            }

            if (GSManager.Instance != null)
                GSManager.Instance.handlers.Remove(gsHandler);

            base.OnDestroy();
        }

        public void InitCustomData(Blackboard bb = null)
        {
            if (bb == null)
                bb = GemJackpotUtils.GemJackpotInfo;

            if (bb != null)
            {
                BlackboardUtils.SetOrCreateValue(bb, "initializedContent", false);
                BlackboardUtils.SetOrCreateValue(bb, "customData", GetComponent<Blackboard>());
            }
            //GSManager.Instance.handlers.Add(gsHandler);
        }
    }
}