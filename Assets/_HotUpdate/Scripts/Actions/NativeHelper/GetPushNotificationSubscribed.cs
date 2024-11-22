using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/NativeHelper")]
public class GetPushNotificationSubscribed : ActionTask
{
    public BBParameter<bool> saveAs;

    protected override string info
    {
        get
        {
            return string.Format("{0} = Get PushNotification Subscribed", saveAs);
        }
    }

    protected override void OnExecute()
    {
        saveAs.value = NativeHelper.Instance.GetPushNotificationSubscribed();
        
        EndAction();
    }
}

}
