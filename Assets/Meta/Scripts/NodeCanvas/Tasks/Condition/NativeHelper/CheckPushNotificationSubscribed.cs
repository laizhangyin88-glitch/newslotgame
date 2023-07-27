using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Condition
{

[Category("★ BagelCode/NativeHelper")]
public class CheckPushNotificationSubscribed : ConditionTask<Blackboard>
{
    protected override string info
    {
        get { return "Check Push Notification Subscribed"; }
    }

    protected override bool OnCheck()
    {
        return NativeHelper.Instance.GetPushNotificationSubscribed();
    }
}

}
