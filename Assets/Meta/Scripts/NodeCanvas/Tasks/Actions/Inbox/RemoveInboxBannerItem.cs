using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

    [Category("★ BagelCode/Inbox")]
    public class RemoveInboxBannerItem : ActionTask <Blackboard> 
    {
        public BBParameter<int> removeInboxID;

        protected override string info 
        {
            get 
            {
                return string.Format("Remove Inbox Banner Item {0}", removeInboxID);
            }
        }

        protected override void OnExecute()
        {
            BlackboardQueryUtils.RemoveInboxBannerItem(removeInboxID.value);
            BlackboardQueryUtils.SetWatchedInboxBannerItem(removeInboxID.value);
            EndAction(true);
        }
    }

}
