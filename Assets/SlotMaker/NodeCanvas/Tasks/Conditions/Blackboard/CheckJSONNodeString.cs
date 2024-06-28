using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SimpleJSON;

namespace SlotMaker.Tasks.Condition
{

    [Category("★ SlotMaker/Blackboard")]
    public class CheckJSONNodeString : ConditionTask<Blackboard>
    {
        public BBParameter<string> nodePath;
        public BBParameter<string> keyPath; //"xxx/xxx/xxx"
        public BBParameter<string> valueB;

        protected override string info
        {
            get { return  $"({nodePath} as JSON)[{keyPath}]" + " == " + valueB; }
        }

        protected override bool OnCheck()
        {
            var variableA = BlackboardUtils.FindVariable<string>(agent, nodePath.value);
            if (variableA == null || variableA.value == null)
                return false;

            string data = variableA.value;
            JSONNode node = JSONNode.Parse(data);

            string[] itemsStrs = keyPath.value.Split('/') ?? new string[] { };

            if (itemsStrs.Length == 0)
                return false;

            JSONNode target = node;
            foreach (string itemStr in itemsStrs)
            {
                if (target.HasKey(itemStr))
                {
                    target = target[itemStr];
                }
                else
                {
                    return false;
                }
            }
            string resStr = (string)target;
            bool res = string.Equals(resStr, valueB.value);
            return res;
        }
    }

}
