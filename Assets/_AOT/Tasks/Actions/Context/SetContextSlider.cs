using UnityEngine;
using System;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

    [Category("★ SlotMaker/Context")]
    public class SetContextSlider : ActionTask<ContextElement>
    {
        public BBParameter<float> max;
        [SerializeField] protected bool isImmediate;
        [SerializeField] protected BBParameter<float> velocity;
        ContextSlider property;

        protected override string info
        {
            get { return string.Format("{0}.slider value to {1} by speed {2}", agentInfo, max, velocity); }
        }

        protected override void OnUpdate()
        {
            float currentValue = property.GetFloatProperty();

            if (currentValue >= max.value)
                EndAction(true);

            property.SetFloatProperty(Math.Min(currentValue + velocity.value, max.value));
        }

        protected override void OnExecute()
        {
            property = agent as ContextSlider;

            if (property == null)
            {
                Debug.LogError("[Context] " + agent.ContextName + " is not IContextFloatProperty");
                EndAction(false);
            }
            else if (isImmediate == false && velocity.value <= 0)
            {
                Debug.LogError("[Context] " + "velocity should be positive value");
                EndAction(false);
            }
            else
            {
                if (isImmediate)
                {
                    property.SetFloatProperty(max.value);
                    EndAction(true);
                }
            }
        }

        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
#if UNITY_EDITOR

        protected override void OnTaskInspectorGUI()
        {
            DrawDefaultInspector();

            isImmediate = UnityEditor.EditorGUILayout.Toggle("Is Immediate", isImmediate);
            if (!isImmediate)
                velocity = (BBParameter<float>)NodeCanvas.Editor.BBParameterEditor.ParameterField("Velocity", velocity);
        }

#endif
    }
}
