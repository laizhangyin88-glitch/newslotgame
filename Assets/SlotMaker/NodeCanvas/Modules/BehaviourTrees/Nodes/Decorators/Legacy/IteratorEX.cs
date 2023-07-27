using System.Collections;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.BehaviourTrees
{
    [Name("Iterate EX")]
    [Category("Decorators/BagelCode")]
    [Description("[Legacy]Iterate any type of list and execute the child node for each element in the list. Keeps iterating until the Termination Condition is met or the whole list is iterated and return the child node status")]
    [Icon("List")]
    public class IteratorEX : BTDecorator
    {

        public enum TerminationConditions
        {
            None,
            FirstSuccess,
            FirstFailure
        }

        [RequiredField] [BlackboardOnly]
        public BBParameter<IList> targetList;
        public BBParameter<bool> reverseIterator;
        [BlackboardOnly]
        public BBObjectParameter current;
        [BlackboardOnly]
        public BBParameter<int> storeIndex;

        public BBParameter<int> maxIteration = -1;

        public TerminationConditions terminationCondition = TerminationConditions.None;
        public bool resetIndex = true;

        private int currentIndex;
        private bool isFirst = true;

        private IList list
        {
            get {return targetList != null? targetList.value : null;}
        }
        
        protected override Status OnExecute(Component agent, IBlackboard blackboard)
        {
            if (decoratedConnection == null)
            {
                return Status.Resting;
            }

            if (list == null || list.Count == 0)
            {
                return Status.Failure;
            }

            if(isFirst)
            {
                currentIndex = BeginIndex();
                isFirst = false;
            }

            for (int i = currentIndex; IsNext(i); IncreaseIndex(ref i))
            {
                current.value    = list[i];
                storeIndex.value = i;
                status = decoratedConnection.Execute(agent, blackboard);
                
                if (status == Status.Success && terminationCondition == TerminationConditions.FirstSuccess){
                    return Status.Success;
                }
                
                if (status == Status.Failure && terminationCondition == TerminationConditions.FirstFailure){
                    return Status.Failure;
                }

                if (status == Status.Running){
                    currentIndex = i;
                    return Status.Running;
                }

                
                if (IsLast() || IsMaxIteration()){
                    return status;
                }

                decoratedConnection.Reset();
                IncreaseIndex(ref currentIndex);
            }

            return Status.Running;
        }

        private int BeginIndex()
        {
            if(reverseIterator.value && list != null)
                return list.Count-1;

            return 0;
        }

        private bool IsNext(int index)
        {
            if(reverseIterator.value)
                return index >= 0;

            return index < list.Count;
        }

        private bool IsLast()
        {
            if(reverseIterator.value)
                return currentIndex == 0;

            return currentIndex == list.Count-1;
        }

        private bool IsMaxIteration()
        {
            if(reverseIterator.value)
                return list.Count-currentIndex == maxIteration.value-1;

            return currentIndex == maxIteration.value-1;
        }

        private void IncreaseIndex(ref int index)
        {
            if(reverseIterator.value)
                index --;
            else
                index ++;
        }

        protected override void OnReset()
        {
            if (resetIndex)
            {
                currentIndex = BeginIndex();
            }
        }

        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
        #if UNITY_EDITOR

        protected override void OnNodeGUI()
        {

            var leftLabelStyle = new GUIStyle(GUI.skin.GetStyle("label"));
            leftLabelStyle.richText = true;
            leftLabelStyle.alignment = TextAnchor.UpperLeft;

            GUILayout.Label("For Each \t" + current + "\nIn \t" + targetList, leftLabelStyle);
            if (terminationCondition != TerminationConditions.None)
                GUILayout.Label("Exit on " + terminationCondition.ToString());

            if (Application.isPlaying)
                GUILayout.Label("Index: " + currentIndex.ToString() + " / " + (list != null && list.Count != 0? (list.Count -1).ToString() : "?") );
        }

        protected override void OnNodeInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUI.changed)
            {
                var argType = targetList.refType != null? targetList.refType.GetGenericArguments()[0] : null;
                if (current.varType != argType){
                    current.SetType(argType);
                }
            }
        }

        #endif
    }
}
