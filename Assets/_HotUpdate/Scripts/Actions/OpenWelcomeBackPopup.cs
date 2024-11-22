using UnityEngine;
using NodeCanvas.Framework;
using SlotMaker;
using ParadoxNotion.Design;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Utils")]
    public class OpenWelcomeBackPopup : ActionTask<Blackboard>
    {
        public BBParameter<bool> accountRemovalRequested;
        public BBParameter<int> removalRemainingSec;

        public BBParameter<GameObject> savePopupObj;

        protected override string info
        {
            get
            {
                return string.Format("Open Welcome Back");
            }
        }

        protected override void OnExecute()
        {
            if (accountRemovalRequested != null && accountRemovalRequested.value)
            {
                string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
                string asset = "Popup Delete Account Welcome Back Scene";
                Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

                var popupObj = MetaObjectUtils.MakeScene(bundle, asset, parent);
                var popupBB = popupObj.GetComponent<Blackboard>();

                BlackboardUtils.SetOrCreateValue(popupBB, "removalRemainingSec", removalRemainingSec.value);
                MetaObjectUtils.SetCalleeCaller(popupObj, agent.gameObject);

                MetaPopupUtils.OpenPopup(popupObj);

                savePopupObj.value = popupObj;
            }

            EndAction();
        }
    }
}
