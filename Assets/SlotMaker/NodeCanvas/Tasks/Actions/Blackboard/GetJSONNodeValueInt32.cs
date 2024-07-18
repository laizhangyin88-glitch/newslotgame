using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SimpleJSON;


namespace SlotMaker.Tasks.Actions
{

    [Category("★ SlotMaker/Blackboard/Generic")]
    public class GetJSONNodeValueInt32 : ActionTask<Blackboard>
    {

        public BBParameter<string> nodePath;
        public BBParameter<string> keyPath;
        public BBParameter<string> defaultValue;

        [BlackboardOnly]
        public BBParameter<int> saveAs;


        void _OnErr()
        {
            if (defaultValue.value != null && defaultValue.value != "")
            {
                saveAs.value = int.Parse(defaultValue.value);
                EndAction();
            }
            else
            {
                EndAction(false);
            }
        }


        protected override string info
        {
            //get { return string.Format("{0} = {1}", saveAs, valueA); }
            //get { return saveAs + " = " + nodePath + $"[{keyPath}] as int"; }
            get { return saveAs + $" = ({nodePath}as JSON)[{keyPath}]"; }
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

            saveAs.value = (int)target;
            EndAction();
        }
    }
}
