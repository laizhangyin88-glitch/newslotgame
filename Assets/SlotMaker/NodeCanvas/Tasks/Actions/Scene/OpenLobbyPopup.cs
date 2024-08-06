using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace SlotMaker.Tasks.Actions
{

    [Category("★ BagelCode/Popup")]
    [Description("大厅加载弹窗方法")]
    public class OpenLobbyPopup : ActionTask<Transform>
    {
        [Description("界面资源所属ab包名")]
        public BBParameter<string> bundleName;
        public BBParameter<string> assetName;
        public BBParameter<bool> combineApplicationType;

        public bool isGlobalPopup = false;

        [BlackboardOnly]
        public BBParameter<GameObject> saveAs;

        protected override string info
        {
            get { return string.Format("{0}OpenContentPopup({1})", (!saveAs.isNone ? (saveAs.ToString() + " = ") : ""), assetName); }
        }

        protected override void OnExecute()
        {
            var prefab = AssetBundleManager.LoadAsset<GameObject>(GetBundleName(), assetName.value);
            GameObject go = GameObject.Instantiate(prefab) as GameObject;

            if (isGlobalPopup)
                go.transform.SetParent(PopupManager.Instance.transform, false);
            else
                go.transform.SetParent(PopupManager.Instance.contents, false);
            PopupManager.Instance.Open(go);

            var bb = go.GetComponent<Blackboard>();
            if (bb != null)
            {
                var variable = BlackboardUtils.GetOrCreateVariable<GameObject>(bb, "caller");
                variable.value = agent.gameObject;
            }

            if (!saveAs.isNone)
                saveAs.value = go;

            EndAction();
        }

        protected string GetBundleName()
        {
            return combineApplicationType.value ? ApplicationSettings.MakeApplicationBundleName(bundleName.value) : bundleName.value;
        }
    }

}
