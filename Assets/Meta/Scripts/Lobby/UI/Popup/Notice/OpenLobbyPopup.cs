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
            GameObject go = LoadAndOpenLobbyPupup(bundleName.value, assetName.value, isGlobalPopup, agent.gameObject);

            if (!saveAs.isNone)
                saveAs.value = go;

            EndAction();
        }

        /// <summary>
        /// 加载并打开大厅弹窗
        /// </summary>
        /// <param name="abName">ab名，不包含拼接部分</param>
        /// <param name="assetName">资源名</param>
        /// <param name="isGlobalPopup"></param>
        /// <param name="agent"></param>
        /// <returns></returns>
        public static GameObject LoadAndOpenLobbyPupup(string abName, string assetName, bool isGlobalPopup, GameObject agent, bool isCombineApplicationType = true)
        {
            string abFullName = isCombineApplicationType ? ApplicationSettings.MakeApplicationBundleName(abName) : abName;

            var prefab = AssetBundleManager.LoadAsset<GameObject>(abFullName, assetName);
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
                variable.value = agent;
            }

            return go;
        }

        protected string GetBundleName()
        {
            return combineApplicationType.value ? ApplicationSettings.MakeApplicationBundleName(bundleName.value) : bundleName.value;
        }
    }

}
