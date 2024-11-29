using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Inbox")]
public class GetInboxItemFromType : ActionTask<Blackboard>  
{
    public InboxTypes type;
    public BBParameter<List<Blackboard>> saveAs;

    protected override string info
    {
        get { return string.Format("{0} = get first type {1}", saveAs, type); }
    }

    protected override void OnExecute()
    {
        saveAs.value = BlackboardQueryUtils.GetBlackboardInboxInfoFromType(type);
        EndAction();
    }
}

}
