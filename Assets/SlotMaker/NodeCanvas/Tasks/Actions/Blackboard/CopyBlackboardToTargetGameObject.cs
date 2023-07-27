using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace SlotMaker.Tasks.Actions
{
    [Category("★ SlotMaker/Blackboard")]
    public class CopyBlackboardToTargetGameObject : ActionTask<Blackboard>
    {
        public BBParameter<IBlackboard> source;
        public BBParameter<Transform> targetTransform;
        public BBParameter<string> targetName;
        public BBParameter<Blackboard> saveAs;
        public BBParameter<GameObject> createdGO;

        protected override string info
        {
            get
            {
                string targetParentName = targetTransform.value != null ? targetTransform.value.gameObject.name : "target";
                return string.Format("copy {0} Blackboard to {1}\\{2}", source.name, targetParentName, targetName.value);
            }
        }

        protected override void OnExecute()
        {
            var createdBB = BlackboardUtils.CreateBlackboard(targetName.value) as Blackboard;
            createdBB.transform.parent = targetTransform.value;

            BlackboardUtils.CopyBlackboard(source.value, createdBB);
            saveAs.value = createdBB;
            createdGO.value = createdBB.gameObject;

            EndAction();
        }
    }
}
