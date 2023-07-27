using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker.IoC
{
    public class DynamicComponent : MonoBehaviour
    {
        public Component source;

        [InlineEditor]
        public VariableComponent target;

        private void Awake()
        {
            target.value = source;
        }

        private void OnDestroy()
        {
            target.value = null;
        }
    }
}