using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker.Json;

namespace SlotMaker.Tasks.Actions
{
    [Category("★ SlotMaker/Json")]
    public class LoadContentsSchema : ActionTask
    {
        public BBParameter<string> bundleName;
        public BBParameter<string> assetName;

        protected override string info { get { return string.Format("Load {0} in {1}", assetName, bundleName); } }

        protected override void OnExecute()
        {
            var schemaAsset = AssetBundleManager.LoadAsset<TextAsset>(bundleName.value, assetName.value);
            if (schemaAsset != null)
                BlackboardJson.LoadSchema(schemaAsset.text, true);
            
            EndAction();
        }
    }
}