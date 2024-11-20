using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Conditions.Contents
{

[Category("★ BagelCode/IAM")]
public class CheckIAMTrigger : ConditionTask
{
    public BBParameter<BagelCode.ClientModels.InAppMessageTriggerType> triggerType;

    protected override string info
    {
        get { return string.Format("Check IAM Trigger {0}", triggerType); }
    }

    protected override bool OnCheck()
    {
        return BagelCode.IAMRouter.Instance.CheckTriggerIAM(triggerType.value);
    }
}

}
