using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Actions
{
[Category("★ BagelCode/TimeUtils")]
public class WaitTimestamp : ActionTask 
{
    public BBParameter<long> waitTimestamp;
    private long currentTimestamp;
    public CompactStatus finishStatus = CompactStatus.Success;

    protected override string info{
        get {return "Wait until ts " + waitTimestamp;}
    }

    protected override void OnUpdate(){
        if (BagelCode.TimeUtils.GetTimeStamp() >= waitTimestamp.value){
            EndAction(finishStatus == CompactStatus.Success? true : false);
        }
    }
}
}
