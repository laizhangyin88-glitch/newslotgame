using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Inbox")]
public class ApplyUsableInboxItemList : ActionTask<Blackboard>  
{
    public BBParameter<List<Blackboard>> itemList;

    protected override string info
    {
        get { return string.Format("Apply inbox item list{0}", itemList); }
    }

    protected override void OnExecute()
    {
        BlackboardQueryUtils.SetUsableInboxItemList(itemList.value);
        EndAction();
    }
}

}
