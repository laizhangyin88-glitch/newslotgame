using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SimpleJSON;
using System.Collections.Generic;

namespace SlotMaker.Tasks.Actions
{

   /* [Category("★ SlotMaker/Blackboard/Generic")]
    public class GetJSONNodeValue<T> : ActionTask<Blackboard>
    {

        public BBParameter<string> nodePath;
        public BBParameter<string> keyPath;

        [BlackboardOnly]
        public BBParameter<T> saveAs;

        protected override string info
        {
            //get { return string.Format("{0} = {1}", saveAs, valueA); }
            get { return saveAs + " = " + nodePath + $"[{keyPath}]" ; }
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

            string[] itemsStrs = keyPath.value.Split('/') ?? new string[] { };

            if (itemsStrs.Length == 0)
            {
                EndAction(false);
                return;
            }

            JSONNode target = node;
            foreach (string itemStr in itemsStrs)
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
            

            //if (T == typeof(int))
            if(typeof(T).Equals(typeof(int)))
            {
                int res = target;
                saveAs.value = (T)res;
            }else if (typeof(T).Equals(typeof(string)))
            {
        
                saveAs.value = (string)target["1"] ;

                =  target.ToString as T
            }


            EndAction();
        }
    }*/

    [Category("★ SlotMaker/Blackboard/Generic")]
    public class GetJSONNodeValueString : ActionTask<Blackboard>
    {

        public BBParameter<string> nodePath;
        public BBParameter<string> keyPath;
        public BBParameter<string> defaultValue;
        [BlackboardOnly]
        public BBParameter<string> saveAs;

        protected override string info
        {
            //get { return string.Format("{0} = {1}", saveAs, valueA); }
            // get { return saveAs + " = " + nodePath + $"[{keyPath}] as string"; }
            get { return saveAs + $" = ({nodePath}as JSON)[{keyPath}]"; }
        }


        void _OnErr()
        {
            if (defaultValue.value != null && defaultValue.value != "")
            {
                saveAs.value = defaultValue.value;
                EndAction();
            }
            else
            {
                EndAction(false);
            }
        }

        protected override void OnExecute()
        {

            var variableA = BlackboardUtils.FindVariable<string>(agent, nodePath.value);
            if (variableA == null || variableA.value == null)
            {
                _OnErr();
                return;
            }

            string data = variableA.value;
            JSONNode node = JSONNode.Parse(data);

            string[] itemsStrs = keyPath.value.Split('/') ?? new string[] { };

            if (itemsStrs.Length == 0)
            {
                _OnErr();
                return;
            }


            JSONNode target = node;
            foreach (string itemStr in itemsStrs)
            {
                if (target.HasKey(itemStr))
                {
                    target = target[itemStr];
                }
                else{
                    _OnErr();
                    return;
                }
            }

            saveAs.value = (string)target;
            EndAction();
        }
    }

}
