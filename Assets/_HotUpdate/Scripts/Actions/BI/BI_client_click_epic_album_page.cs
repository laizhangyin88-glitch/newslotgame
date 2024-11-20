using System;
using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

    [Category("★ BagelCode/BI")]
    public class BI_client_click_epic_album_page : ActionTask<Blackboard>
    {
        public BBParameter<string> type;
        public BBParameter<int> categoryId;
        public BBParameter<int> gameId;
        public BBParameter<string> categoryBadgeStatus;
        
        protected override void OnExecute()
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();

            customData["type"] = type.value;

            if (type.value.Equals("category_click"))
            {
                customData["category_id"] = categoryId.value;
                //badge_type String  Type of Badge for category when click category.(Default, New, Complete)
                customData["badge_type"] = categoryBadgeStatus.value;
            }
            else if(type.value.Equals("click_arrow"))
            {
                customData["category_id"] = categoryId.value;
            }
            else if (type.value.Equals("click_album"))
            {
                customData["game_id"] = gameId.value;
                
                var woeInfo = BlackboardQueryUtils.GetWOEInfo((CategoryType) categoryId.value, gameId.value);
                customData["is_empty"] = woeInfo == null;
            }
            
            Analytics.CustomEvent("client_click_epic_album_page", customData);

            EndAction();
        }
    }

}
