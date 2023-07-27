using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/NativeHelper")]
public class RetentionLocalPush : ActionTask
{
    protected override string info
    {
        get
        { 
            return "After login, delete/set local push for retention"; 
        }
    }

    protected override void OnExecute()
    {   
#if !UNITY_EDITOR
        // Debug.Log("Retention Local Push");

        // var dailybonus = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "values/misc/LOCAL_PUSH/DAILY_BONUS");
        // NativeHelper.Instance.DeleteLocalPush(dailybonus.value.GetValue<int>("ID"));
        // NativeHelper.Instance.SetLocalPush(dailybonus.value.GetValue<int>("ID"), dailybonus.value.GetValue<string>("TITLE"), dailybonus.value.GetValue<string>("TEXT"), dailybonus.value.GetValue<int>("SECONDS"), dailybonus.value.GetValue<string>("TYPE"), true);

        // var retentionList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), "values/misc/LOCAL_PUSH/RETENTION_LIST");
        
        // for (int i = 0; i < retentionList.value.Count; ++i)
        // {
        //     var push = retentionList.value[i];
        //     NativeHelper.Instance.DeleteLocalPush(push.GetValue<int>("ID"));
        //     NativeHelper.Instance.SetLocalPush(push.GetValue<int>("ID"), push.GetValue<string>("TITLE"), push.GetValue<string>("TEXT"), push.GetValue<int>("SECONDS"), push.GetValue<string>("TYPE"), true);
        // }
#endif
        EndAction();
    }
}

}
