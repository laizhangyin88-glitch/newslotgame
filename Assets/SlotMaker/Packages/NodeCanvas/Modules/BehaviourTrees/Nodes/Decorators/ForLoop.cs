using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using NodeCanvas.BehaviourTrees;

namespace SlotMaker.Slots.BehaviourTrees
{
    [Name("For Loop")]
    [Category("✶ Slots/Decorators")]
    [Icon("List")]
    public class ForLoop : BTDecorator
    {
        public BBParameter<int> storeIndex;
        public BBParameter<int> maxIteration;

        public enum TerminationConditions
        {
            None,
            FirstSuccess,
            FirstFailure
        }
        public TerminationConditions terminationCondition = TerminationConditions.None;
        
        public BBParameter<bool> inverse;
        public bool resetIndex;

        private int currentIndex;

        protected override Status OnExecute(Component agent, IBlackboard blackboard)
        {
            if (decoratedConnection == null)
                return Status.Optional;
            
            for (int i = currentIndex; i < maxIteration.value; ++i)
            {
                storeIndex.value = inverse.value ? (maxIteration.value - i) - 1 : i;
                status = decoratedConnection.Execute(agent, blackboard);

                if (status == Status.Success && terminationCondition == TerminationConditions.FirstSuccess)
                    return Status.Success;

                if (status == Status.Failure && terminationCondition == TerminationConditions.FirstFailure)
                    return Status.Failure;
                
                if (status == Status.Running)
                {
                    currentIndex = i;
                    return Status.Running;
                }

                if (currentIndex == maxIteration.value - 1)
                {
                    if (resetIndex)
                        currentIndex = 0;
                    return status;
                }

                decoratedConnection.Reset();
                ++currentIndex;
            }

            return Status.Optional;
        }

        protected override void OnReset()
        {
            if (resetIndex)
                currentIndex = 0;
        }

#if UNITY_EDITOR
        protected override void OnNodeGUI()
        {
            var leftLabelStyle = new GUIStyle(GUI.skin.GetStyle("label"));
            leftLabelStyle.richText = true;
            leftLabelStyle.alignment = TextAnchor.UpperLeft;

            if (!inverse.value)
                GUILayout.Label(string.Format("For ({0} < {1})", storeIndex, maxIteration));
            else
                GUILayout.Label(string.Format("(Inverse) For ({0} >= 0)", storeIndex));

            if (terminationCondition != TerminationConditions.None)
                GUILayout.Label("Exit on " + terminationCondition.ToString());
        }
#endif
    }
}