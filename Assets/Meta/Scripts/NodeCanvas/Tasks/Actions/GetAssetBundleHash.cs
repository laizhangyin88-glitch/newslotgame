using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Tasks.Actions {

    [Category("★ BagelCode/Utils")]
    public class GetAssetBundleHash : ActionTask
    {
        public BBParameter<string> bundleName;
        public BBParameter<string> saveAs;

        protected override string info
        {
            get { return string.Format("Save Asset Bundle Hash {0} as {1}", bundleName, saveAs); }
        }

        protected override void OnExecute()
        {
#if USE_ASSETBUNDLE
            Hash128 hashCode = AssetBundleManager.Manifest.GetAssetBundleHash(bundleName.value);
            saveAs.value = hashCode.ToString();
#endif
            EndAction();
        }
    }

}
