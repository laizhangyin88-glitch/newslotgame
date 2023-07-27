using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{
    [Category("★ BagelCode/BI")]
    public class BI_client_click_update_meta_icon : ActionTask
    {
        public BBParameter<bool> isInGame;

        protected override string info
        {
            get
            {
                return string.Format("client_click_update_meta_icon(type : {0})", isInGame.value ? "in_game" : "lobby");
            }
        }

        protected override void OnExecute()
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();

            customData["type"] = isInGame.value ? "in_game" : "lobby";

            Analytics.CustomEvent("client_click_update_meta_icon", customData);

            EndAction();
        }
    }
}