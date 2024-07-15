using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SimpleJSON;

namespace SlotMaker.Tasks.Condition
{
    [Category("★ SlotMaker/Blackboard")]
    public class CheckJSONNodeHasKey : ConditionTask<Blackboard>
    {
        public BBParameter<string> nodePath;
        public BBParameter<string> keyPath; 
        protected override string info
        {
            //get { return nodePath + $" has key [{keyPath}]"; }
            get{ return $"({nodePath} as JSON) has key [{keyPath}]";
        }
    }

        protected override bool OnCheck()
        {
            var variableA = BlackboardUtils.FindVariable<string>(agent, nodePath.value);

            if (variableA == null || variableA.value == null)
            {
                Debug.LogError($"{nodePath.value} is null");
                return false;
            }

            string data = variableA.value;
            JSONNode node = JSONNode.Parse(data);

            string[] itemsStrs = keyPath.value.Split('/') ?? new string[] { };

            if (itemsStrs.Length == 0)
                return false;

            JSONNode target = node;
            for (int i =0; i< itemsStrs.Length; i++)
            {
                string itemStr = itemsStrs[i];
                if (target.HasKey(itemStr))
                {
                    target = target[itemStr];
                }
                else
                {
                    Debug.LogError("false..........................." + keyPath.value);
                    return false;
                }
            }
            Debug.LogError("true..........................." + keyPath.value);   
            return true;
        }
    }

}
