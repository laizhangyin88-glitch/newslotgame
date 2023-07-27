using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace BagelCode.Tasks.Actions 
{

[Category("★ BagelCode/Utils")]
public class UpdateCredit : ActionTask 
{
    public BBParameter<long> currentCredit;
    public BBParameter<long> targetCredit;

    public BBParameter<float> maxAnimationTime;
    public BBParameter<int> minCreditPerSecond;

    public BBParameter<string> updatedCreditEventName;

    protected override void OnUpdate()
    {
        long diffCredit = targetCredit.value - currentCredit.value;
        if (diffCredit == 0)
            return;

        float animationTime = Mathf.Min((float)diffCredit / minCreditPerSecond.value, maxAnimationTime.value);
        if (animationTime == 0f)
        {
            currentCredit.value = targetCredit.value;
            return;
        }

        currentCredit.value += (long)((float)diffCredit * Time.deltaTime / animationTime);
        if (currentCredit.value >= targetCredit.value)
        {
            currentCredit.value = targetCredit.value;
            Graph.SendGlobalEvent(new EventData(updatedCreditEventName.value), agent);
        }
    }
}

}

