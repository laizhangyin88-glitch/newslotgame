using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode
{
    [RequireComponent(typeof(Blackboard))]
    public class MetaCustomSoundData : MonoBehaviour
    {
        public Blackboard gsHandler;

        private void Awake()
        {
            GSManager.Instance.handlers.Add(gsHandler);
        }

        private void OnDestroy()
        {
            var variables = gsHandler.variables;
            foreach (var pair in variables)
            {
                ((GSHandler)pair.Value.value).Clear();
            }

            if(GSManager.Instance == null) return;

            GSManager.Instance.handlers.Remove(gsHandler);
        }
    }
}