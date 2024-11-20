using UnityEngine;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{
    [Category("★ BagelCode/Popup")]
    public class OpenPopupSceneAsync : ActionTask
    {
        public BBParameter<string> bundleName;
        public BBParameter<string> assetName;
        public BBParameter<bool> combineApplicationType;
        public BBParameter<Transform> parent;
        public BBParameter<string> parentName;
        public BBParameter<bool> setActive;

        [BlackboardOnly]
        public BBParameter<GameObject> saveAs;

        protected AssetBundleLoadAssetOperation loadSceneInfoOperation = null;
        protected SceneLoadOperation sceneLoadOperation = null;

        protected override string info
        {
            get
            {
                return string.Format("{0} = LoadSceneAsync({1}), OpenPopup and SetActive {2}", saveAs, assetName, setActive.value);
            }
        }

        protected override void OnExecute()
        {
            loadSceneInfoOperation = null;
            sceneLoadOperation = null;
        }

        protected override void OnUpdate()
        {
            if (loadSceneInfoOperation == null)
                loadSceneInfoOperation = AssetBundleManager.LoadAssetAsync<SceneInfoObject>(GetBundleName(), assetName.value);

            if (loadSceneInfoOperation.IsDone())
            {
                if (sceneLoadOperation == null)
                {
                    var _parent = parent.value;
                    if (!string.IsNullOrEmpty(parentName.value))
                    {
                        if (_parent == null)
                            _parent = GameObject.Find(parentName.value).transform;
                        else
                            _parent = _parent.Find(parentName.value);
                    }

                    var sceneInfo = loadSceneInfoOperation.GetAsset<SceneInfoObject>().GetSceneInfo();

                    sceneLoadOperation = SceneManager.LoadSceneAsync(_parent, sceneInfo, !setActive.value);
                }

                if (sceneLoadOperation.IsDone())
                {
                    GameObject popup = sceneLoadOperation.GetScene();
                    PopupManager.Instance.Open(popup);

                    if (!saveAs.isNone)
                        saveAs.value = popup;

                    EndAction();
                }
            }
        }

        protected string GetBundleName()
        {
            return combineApplicationType.value ? ApplicationSettings.MakeApplicationBundleName(bundleName.value) : bundleName.value;
        }
    }
}
