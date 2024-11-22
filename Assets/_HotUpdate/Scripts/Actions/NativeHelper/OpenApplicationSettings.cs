using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/NativeHelper")]
public class OpenApplicationSettings : ActionTask
{
    protected override string info { get { return "Open Application Settings"; } }

    protected override void OnExecute()
    {
        SendDevicePushSetting();
        NativeHelper.Instance.OpenAppSettings();
        
        EndAction();
    }

    protected void SendDevicePushSetting()
    {
        Blackboard rootBB = agent.GetComponent<Blackboard>();
        if (rootBB != null)
        {
            string biContextID = BlackboardUtils.FindValue<string>(rootBB, "_biContextID");
            BlackboardQueryUtils.BI_Device_Push_Setting(biContextID);
        }
    }
}

}
