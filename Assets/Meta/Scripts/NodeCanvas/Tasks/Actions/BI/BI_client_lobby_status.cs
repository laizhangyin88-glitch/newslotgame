using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Tasks.Actions.BI
{

    [Category("★ BagelCode/BI")]
    public class BI_client_lobby_status : ActionTask<Blackboard>
    {
        // protected override string info
        // {
        //     get
        //     {
        //         return string.Format("BI_client_lobby_status");
        //     }
        // }

        protected override void OnExecute()
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();

            BiEventUtils.AppendLobbyStatusData(customData);

            var favoriteIndex = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "favoriteSlotUiIndex");
            customData["favorite_slot_ui_index"] = (long)favoriteIndex.value;

            var lobbyBackgroundImageUrl = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "lobbyBackgroundImageUrl");
            customData["lobby_background_image_url"] = lobbyBackgroundImageUrl.value;
    
            Analytics.CustomEvent("client_lobby_status", customData);
            EndAction();
        }
    }

}
