using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Notice")]
public class GetPopupNoticeList : ActionTask<Blackboard>  
{
    [BlackboardOnly]
    public BBParameter<List<Blackboard>> saveAs;

    protected override void OnExecute()
    {
        saveAs.value = BlackboardQueryUtils.GetPopupNoticeList();
        EndAction();
    }
}

}
