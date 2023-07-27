using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

    [Category("★ SlotMaker/Scene")]
    public class LoadScene1 : ActionTask
    {
        public BBParameter<string> bundleName;
        public BBParameter<string> assetName;
        public BBParameter<bool> combineApplicationType;
        public BBParameter<Transform> parent;
        public BBParameter<string> parentName;

        [BlackboardOnly]
        public BBParameter<GameObject> saveAs;

        protected override string info
        {
            get
            {
                return string.Format("{0} = LoadScene({1})", saveAs, assetName);
            }
        }

        protected override void OnExecute()
        {
            var sceneInfo = AssetBundleManager.LoadAsset<SceneInfoObject>(GetBundleName(), assetName.value).GetSceneInfo();

            var _parent = parent.value;
            if (!string.IsNullOrEmpty(parentName.value))
            {
                if (_parent == null)
                    _parent = GameObject.Find(parentName.value).transform;
                else
                    _parent = _parent.Find(parentName.value);
            }

            var temp = SceneManager.LoadScene(_parent, sceneInfo);
            if (!saveAs.isNone)
                saveAs.value = temp;
            EndAction();
        }

        protected string GetBundleName()
        {
            return combineApplicationType.value ? ApplicationSettings.MakeApplicationBundleName(bundleName.value) : bundleName.value;
        }
    }

}