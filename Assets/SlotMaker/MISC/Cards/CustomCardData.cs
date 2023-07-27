using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using SlotMaker.Json;

namespace SlotMaker.Cards
{
    public class CustomCardData : MonoWeakSingleton<CustomCardData>
    {
        public CardAssets cardAssets;
        public ScriptableObject payTable;
        public Blackboard gsHandler;

        void Awake()
        {
            BlackboardUtils.SetOrCreateValue<Blackboard>(ContentBlackboard.Get(), "customData", GetComponent<Blackboard>());
            GSManager.Instance.handlers.Add(gsHandler);
        }

        protected override void OnDestroy()
        {
            var variables = gsHandler.variables;
            foreach (var pair in variables)
            {
                ((GSHandler)pair.Value.value).Clear();
            }

            base.OnDestroy();
        }
    }
}
