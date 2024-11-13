using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Notice")]
public class GetNoticeFromID : ActionTask<Blackboard>  
{
    public BBParameter<string> findID;

    [BlackboardOnly]
    public BBParameter<Blackboard> saveAs;

    protected override string info
    {
        get { return string.Format("{0} = GetNotice({1})", saveAs, findID); }
    }

    protected override void OnExecute()
    {
        var id = BlackboardUtils.FindVariable<int>(agent, findID.value);

        saveAs.value = BlackboardQueryUtils.GetNoticeFromID(id.value);
        
        EndAction();
    }
}

}
