using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

    [Category("★ BagelCode/Inbox")]
    public class RemoveWatchedItemFromItemList: ActionTask <Blackboard> 
    {
        public BBParameter<List<Blackboard>> itemList;

        protected override string info 
        {
            get 
            {
                return string.Format("Remove Watched Item From List {0}", itemList);
            }
        }

        protected override void OnExecute()
        {
            BlackboardQueryUtils.RemoveWatchedInboxItem(itemList.value);
            EndAction(true);
        }
    }

}
