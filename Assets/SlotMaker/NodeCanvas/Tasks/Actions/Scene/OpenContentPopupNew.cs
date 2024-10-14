using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SlotMaker.Tasks.Actions
{
    public class OpenContentPopupNew : ActionTask<Transform>
    {
        public BBParameter<string> assetName;
        public BBParameter<string> uniqueName;
        public BBParameter<string> assetBundleName;

        public bool isGlobalPopup = false;


        protected override string info
        {
            get { return string.Format("OpenContentPopup({0})", assetName); }
        }

        protected override void OnExecute()
        {
            var prefab = AssetBundleManager.LoadAsset<GameObject>(assetBundleName.value, assetName.value);
            GameObject go = GameObject.Instantiate(prefab) as GameObject;
            go.name = uniqueName.value;

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

            EndAction();
        }
    }
}
