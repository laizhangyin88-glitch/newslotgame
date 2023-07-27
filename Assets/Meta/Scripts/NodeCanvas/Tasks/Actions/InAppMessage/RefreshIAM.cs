using System;
using UnityEngine;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/IAM")]
public class RefreshIAM : ActionTask<Blackboard>
{
    public BBParameter<string> bundleName;
    public BBParameter<bool> combineApplicationType;

    [BlackboardOnly]
    public BBParameter<bool> success;

    protected override string info
    {
        get
        {
            return string.Format("Refresh InAppMessage Object");
        }
    }

    protected override void OnExecute()
    {
        Blackboard iamInfo = BlackboardUtils.FindVariable<Blackboard>(agent, "_iamInfo").value;

        var IAMObject = agent.GetVariable<GameObject>("_iam");
        if (IAMObject == null || IAMObject.value == null)
        {
            success.value = false;
            EndAction();
            return;
        }

        long endTimestamp = IAMUtils.GetEndTimestamp(iamInfo);
        if(endTimestamp == -1L)
        {
            success.value = false;
            EndAction();
            return;
        }

        if (endTimestamp > 0L)
        {
            IAMUtils.MakeDeal(iamInfo);
        }

        string bundleName = MetaObjectUtils.GetBundleName(combineApplicationType.value, this.bundleName.value);

        IAMUtils.ClearComponents(IAMObject.value);

        var iamController = IAMObject.value.GetComponent<InAppMessage.InAppMessageBase>();
        iamController.LoadIAM(bundleName, iamInfo, endTimestamp);

        // IAMUtils.MakeComponents(bundleName, IAMObject.value, iamInfo, endTimestamp);

        success.value = true;
        EndAction();
    }
}

}
