using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Friend")]
public class GetFriendNameById : ActionTask <Blackboard> 
{
    public BBParameter<string> userId;
    public BBParameter<string> saveAs;

    protected override string info 
    {
        get 
        {
            return string.Format("Get Friend name by Id");
        }
    }

    protected override void OnExecute()
    {
        string name = BlackboardQueryUtils.GetFriendNameById(userId.value);
        saveAs.value = name;
        EndAction(true);
    }
}

}
