using UnityEngine;
using UnityEngine.Audio;
using System;
using System.Collections;
using NodeCanvas.Framework;
using Sirenix.OdinInspector;

namespace SlotMaker
{
    public class RegisterGSHandler : MonoBehaviour 
    {
        public Blackboard gsHandler;

        [InlineEditor]
        public GSHandlerList source;

        private void Awake()
        {
            if (GSManager.Instance == null) return;

            if (source)
            {
                foreach (var handler in source.handlers)
                {
                    var variable = gsHandler.AddVariable(handler.handlerId, typeof(GSHandler));
                    variable.value = handler;
                }
            }

            GSManager.Instance.handlers.Add(gsHandler);
        }

        ////////////////////////////////////////////////////////////////////////////
        /// EDITOR
        ////////////////////////////////////////////////////////////////////////////
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (gsHandler == null) gsHandler = gameObject.GetOrAddComponent<Blackboard>();
        }
#endif
    }
}