using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Friend")]
public class GetMyOnlineFriendList : ActionTask <Blackboard> 
{
    [BlackboardOnly]
    public BBParameter<List<Blackboard>> saveAs;

    protected override string info 
    {
        get 
        {
            return string.Format("Get My Online Friend List");
        }
    }

    protected override void OnExecute()
    {
        List<Blackboard> friendsList = BlackboardQueryUtils.GetMyOnlineFriendList();
        saveAs.value = friendsList;
        EndAction(true);
    }
}

}
