using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions {
    [Category("★ BagelCode")]
    public class InitBundlePrefab : ActionTask
    {
        private bool isInit = false;
        public BBParameter<string> bundleName;
        public BBParameter<string> assetName;
        public BBParameter<string> parentName;
        public BBParameter<bool> combineApplicationType;
        
        protected override string info {
            get {
                return string.Format($"Initialize Bundle Prefab {assetName} on {parentName}");
            }
        }

        protected override void OnExecute()
        {
            if (!isInit)
            {
                MetaObjectUtils.MakePrefab(GetBundleName(), assetName.value, agent.transform, parentName.value);
                isInit = true;
            }

            EndAction();
        }

        private string GetBundleName()
        {
            return combineApplicationType.value ? ApplicationSettings.MakeApplicationBundleName(bundleName.value) : bundleName.value;
        }
    }
}
