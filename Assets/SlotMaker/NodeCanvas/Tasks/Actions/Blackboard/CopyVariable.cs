using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{
    [Category("★ SlotMaker/Blackboard")]
    public class CopyVariable : ActionTask<Blackboard>
    {
        public BBParameter<string> valueA;
        
        public BBParameter<string> valueB;

        protected override string info
        {
            get { return string.Format("{0} = {1}", valueB, valueA); }
        }

        protected override void OnExecute()
        {
            var variableA = BlackboardUtils.FindVariable(agent, valueA.value);
            var varType = variableA.varType;
            
            string targetName = null;
            var targetBB = BlackboardUtils.FindBlackboard(agent, valueB.value, ref targetName);

            if (varType == typeof(Blackboard))
            {
                BlackboardUtils.CopyBlackboard(variableA.value as Blackboard, BlackboardUtils.GetOrCreateBlackboard(targetBB, targetName));
            }
            else if (varType == typeof(List<Blackboard>))
            {
                BlackboardUtils.SetOrCreateList<Blackboard>(targetBB, targetName, variableA.value as List<Blackboard>, 
                    (_dst, _src) => { BlackboardUtils.CopyBlackboard(_src, _dst); });
            }
            else
            {
                BlackboardUtils.SetOrCreateValue(targetBB, targetName, variableA.value, varType);
            }

            EndAction();
        }
    }
}
