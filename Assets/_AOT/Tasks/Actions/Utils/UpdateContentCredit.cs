using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Utils")]
public class UpdateContentCredit : ActionTask
{
    public BBParameter<long> unitCredit;
    public BBParameter<long> currentCredit;
    public BBParameter<long> targetCredit;

    public BBParameter<float> minTime;
    public BBParameter<float> maxTime;
    public BBParameter<Vector2> limitTime = new Vector2(0.1f, 20.0f);
    public bool useDebug;

    public BBParameter<string> updatedCreditEventName;

    public BBParameter<float> progress;

    private long oldTargetCredit = 0L;
    private long deltaCredit = 0L;

    private float cA;
    private float cB;
    private const float cMinCreditRate = 1f;
    private const float cMaxCreditRate = 10f;

    protected override string OnInit()
    {
        UpdateCoefficient();
        return null;
    }

    private void UpdateCoefficient()
    {
        // float ca = ((cMaxCreditRate / maxTime.value) - (cMinCreditRate / minTime.value)) / (maxTime.value - minTime.value);
        // float cb = (cMinCreditRate / minTime.value) - ca * minTime.value;
        cA = (maxTime.value - minTime.value) / (cMaxCreditRate - cMinCreditRate);
        cB = minTime.value - cA * cMinCreditRate;
    }

    protected override void OnUpdate()
    {
        long diffCredit = targetCredit.value - currentCredit.value;
        if (diffCredit == 0)
            return;

        if (useDebug)
            UpdateCoefficient();

        if (currentCredit.value == 0L || targetCredit.value != oldTargetCredit)
        {
            float diffCreditRate = (float)diffCredit / (float)unitCredit.value;
            float deltaTime = Mathf.Clamp(cA * diffCreditRate + cB, limitTime.value.x, limitTime.value.y);
            deltaCredit = (long)(diffCreditRate / deltaTime * (float)unitCredit.value);

            if (useDebug)
                Debug.Log("Expected animation time: " + deltaTime);

            oldTargetCredit = targetCredit.value;
        }

        currentCredit.value += (long)Mathf.Max(((float)deltaCredit * Time.deltaTime), 1f);
        if (currentCredit.value >= targetCredit.value)
        {
            currentCredit.value = targetCredit.value;
            progress.value = 1f;

            if (!updatedCreditEventName.isNone && !string.IsNullOrEmpty(updatedCreditEventName.value))
                Graph.SendGlobalEvent(new EventData(updatedCreditEventName.value), agent);
        }
        else
        {
            progress.value = (float)currentCredit.value / (float)targetCredit.value;
        }

    }
}

}
