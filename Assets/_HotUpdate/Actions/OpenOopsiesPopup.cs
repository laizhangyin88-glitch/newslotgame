using UnityEngine;
using NodeCanvas.Framework;
using SlotMaker;
using ParadoxNotion.Design;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Utils")]
    public class OpenOopsiesPopup : ActionTask<Blackboard>
    {
        public BBParameter<string> titleKey;
        public BBParameter<string> textKey;

        public BBParameter<GameObject> savePopupObj;

        protected override string info
        {
            get
            {
                return string.Format("Open Oopsies Popup");
            }
        }

        protected override void OnExecute()
        {
            if (titleKey != null && textKey != null)
            {
                string title = StringTableUtils.GetString(StringTable.StringTableType.Global, titleKey.value);
                string text = StringTableUtils.GetString(StringTable.StringTableType.Global, textKey.value);

                string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
                string asset = "Popup Compensation Scene";
                Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

                var popupObj = MetaObjectUtils.MakeScene(bundle, asset, parent);
                var popupBB = popupObj.GetComponent<Blackboard>();

                BlackboardUtils.SetOrCreateValue(popupBB, "_title", title);
                BlackboardUtils.SetOrCreateValue(popupBB, "_content", text);
                MetaObjectUtils.SetCalleeCaller(popupObj, agent.gameObject);

                MetaPopupUtils.OpenPopup(popupObj);

                savePopupObj.value = popupObj;
            }

            EndAction();
        }
    }
}
