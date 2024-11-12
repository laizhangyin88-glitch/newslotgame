using System.Collections;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Task.Action
{

[Category("★ BagelCode/TimeUtils")]
public class GetLastLoginDayCount : ActionTask
{
    public BBParameter<string> lastLoginTimestamp;

    [BlackboardOnly]
    public BBParameter<int> saveAs;

    protected override string info
    {
        get { return string.Format("Get last login day count"); }
    }

    protected override void OnExecute()
    {
        Blackboard bb = agent.GetComponent<Blackboard>();
        var timestamp = BlackboardUtils.FindVariable(bb, lastLoginTimestamp.value);
        if (timestamp == null || timestamp.value == null)
        {
            EndAction(false);
            return;
        }

        saveAs.value = BagelCode.TimeUtils.GetLastLoginDayCount(System.Convert.ToInt64(timestamp.value));
        EndAction();
    }
}

}
