using UnityEngine;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/IAM")]
    public class LoadIAM : ActionTask<Blackboard>
    {
        public BBParameter<string> bundleName;
        public BBParameter<string> parentName;
        public BBParameter<bool> combineApplicationType;

        [BlackboardOnly]
        public BBParameter<GameObject> saveAs;

        protected override string info
        {
            get
            {
                return string.Format("Load InAppMessage Object");
            }
        }

        protected override void OnExecute()
        {
            Blackboard iamInfo = BlackboardUtils.FindVariable<Blackboard>(agent, "_iamInfo").value;

            long endTimestamp = IAMUtils.GetEndTimestamp(iamInfo);
            if (endTimestamp == -1L)
            {
                saveAs.value = null;
                EndAction();
                return;
            }

            if (endTimestamp > 0L)
            {
                IAMUtils.MakeDeal(iamInfo);
            }

            Transform parentTransform = GameObject.Find(parentName.value).transform;
            string bundleName = MetaObjectUtils.GetBundleName(combineApplicationType.value, this.bundleName.value);
            GameObject IAMObject = MetaObjectUtils.MakePrefab(bundleName, "IAM Base", parentTransform);
            IAMObject.name = "IAM Base";



            var iamController = IAMObject.GetComponent<InAppMessage.InAppMessageBase>();
            iamController.LoadIAM(bundleName, iamInfo, endTimestamp);

            // IAMUtils.MakeComponents(bundleName, IAMObject, iamInfo, endTimestamp);

            // IAMUtils.MakeComponents(bundleName, IAMObject, iamInfo, endTimestamp);
            // IAMUtils.MakeCloseButton(bundleName, IAMObject, iamInfo);

            // IAMObject.GetComponent<Animator>().SetTrigger("Active");

            saveAs.value = IAMObject;
            EndAction();
        }
    }
}
