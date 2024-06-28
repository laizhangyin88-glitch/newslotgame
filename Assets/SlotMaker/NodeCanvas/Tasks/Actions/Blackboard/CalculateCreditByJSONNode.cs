using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SimpleJSON;
using System.Collections.Generic;
using System.Data;

namespace SlotMaker.Tasks.Actions
{

    [Category("★ SlotMaker/Blackboard/Generic")]
    public class CalculateCreditByJSONNode : ActionTask<Blackboard>
    {

        public BBParameter<string> nodePath;
        public BBParameter<bool> isList = false;
        public BBParameter<string> keyPath1;
        public BBParameter<string> keyPath2 = null;

        [BlackboardOnly]
        public BBParameter<long> saveAs;

        protected override string info
        {
            get {

                if(isList.value)
                {

                    if (keyPath2.value != null)
                    {
                        return $"{saveAs} = ({nodePath}as JSON)[{keyPath1}].items sum";
                    }
                    else
                    {
                        return $"{saveAs} = ({nodePath}as JSON)[{keyPath1}].items sum key={keyPath2}";
                    }
                }
                else
                {
                    return $"{saveAs} = ({nodePath}as JSON)[{keyPath1}]";
                }
            }
        }
        protected override void OnExecute()
        {

            var variableA = BlackboardUtils.FindVariable<string>(agent, nodePath.value);
            if (variableA == null || variableA.value == null)
            {
                EndAction(false);
                return;
            }

            string data = variableA.value;
            JSONNode node = JSONNode.Parse(data);

            string[] itemsStrs1 = keyPath1.value.Split('/') ?? new string[] { };

            if (itemsStrs1.Length == 0)
            {
                EndAction(false);
                return;
            }

            JSONNode target = node;
            foreach (string itemStr in itemsStrs1)
            {
                if (target.HasKey(itemStr))
                {
                    target = target[itemStr];
                }
                else
                {
                    EndAction(false);
                    return;
                }
            }

            long RES = 0;
            if (!isList.value)
            {
                saveAs.value = (long)target;
                EndAction();
            }
            else
            {
                if (keyPath2.value == null || keyPath2.value == "")
                {
                    RES = 0;
                    foreach (JSONNode item  in target)
                    {
                        RES += (long)item;
                    }                 
                }
                else
                {
                    string[] itemsStrs2 = keyPath2.value.Split('/') ?? new string[] { };
                    RES = 0;
                    foreach (JSONNode item in target)
                    {
                        JSONNode _target = null;
                        foreach (string itemStr in itemsStrs2)
                        {
                            if (_target.HasKey(itemStr))
                            {
                                _target = _target[itemStr];
                            }
                            else
                            {
                                EndAction(false);
                                return;
                            }
                        }
                        RES += (long)_target;
                    }
                }
            }

            saveAs.value = (long)RES;
            EndAction();
        }
    }
}
