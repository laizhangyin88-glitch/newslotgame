using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using System;

namespace SlotMaker.Tasks.Condition
{

    [Category("★ SlotMaker/Blackboard")]
    public class CheckInt32 : ConditionTask<Blackboard>
    {
        public BBParameter<string> valueA;
        public CompareMethod checkType = CompareMethod.EqualTo;
        public BBParameter<int> valueB;

        protected override string info
        {
            get { return valueA + OperationUtils.GetCompareString(checkType) + valueB; }
        }

        protected override bool OnCheck()
        {

            var variableA = BlackboardUtils.FindVariable<int>(agent, valueA.value);

#if UNITY_EDITOR && false
        Debug.Log("@ 问题待解决");

        if (variableA == null)
        {
            Debug.LogError($"valueA.value =  {valueA.value}");
            Debug.LogError($"valueB.value =  {valueB.value}");
            return true;
        }

#endif
            //return OperationUtils.Compare(variableA.value, valueB.value, checkType);
            bool temp = false;
            try
            {
                temp = OperationUtils.Compare(variableA.value, valueB.value, checkType);
            }
            catch (Exception ex)
            {

                Debug.LogError($"info =  {info} ;  agent.gameObject = {agent.gameObject.name}");
                Debug.LogError($"valueA.value =  {(valueA != null ? valueA.value : "")}");
                //Debug.LogError($"variableA.value =  {(variableA != null? variableA.value : - 999)}");
                Debug.LogError($"valueB.value =  {(valueB != null ? valueB.value : -999)}");
                Debug.LogError($"@【报错】: {ex}");
            }
            return temp;  //OperationUtils.Compare(variableA.value, valueB.value, checkType);

        }
    }
}
