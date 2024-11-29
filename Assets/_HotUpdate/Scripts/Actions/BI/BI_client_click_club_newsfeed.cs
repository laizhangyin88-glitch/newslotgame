using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{

    [Category("★ BagelCode/BI")]
    public class BI_client_click_club_newsfeed : ActionTask<Blackboard>
    {
        public BBParameter<string> fromType;

        protected override string info
        {
            get { return string.Format("BI Client Click Newsfeed {0}", fromType); }
        }

        protected override void OnExecute()
        {
            // use ClubContentsControllerNewsFeed.OnTabNewsFeed instead
            Dictionary<string, object> customData = new Dictionary<string, object>();

            customData["type"] = fromType.value;

            Analytics.CustomEvent("client_click_club_newsfeed", customData);

            EndAction();
        }
    }
}
