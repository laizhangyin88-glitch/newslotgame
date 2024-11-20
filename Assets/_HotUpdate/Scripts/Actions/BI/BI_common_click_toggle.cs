using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_common_click_toggle : ActionTask<Blackboard>
{
    public BBParameter<string> eventName;
    public BBParameter<string> toggleTypeName;
    public BBParameter<bool> toggle;

    protected override string info
    {
        get { return string.Format("BI Common Click Toggle {0}", eventName); }
    }

    protected override void OnExecute()
    {
        Dictionary<string, object> customData = new Dictionary<string, object>();

        customData[toggleTypeName.value] = toggle.value ? "on" : "off";

        Analytics.CustomEvent(eventName.value, customData);

        EndAction();
    }
}

}
