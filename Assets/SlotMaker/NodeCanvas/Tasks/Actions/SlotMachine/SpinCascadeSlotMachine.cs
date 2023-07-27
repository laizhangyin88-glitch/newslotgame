using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/SlotMachine")]
public class SpinCascadeSlotMachine : ActionTask<Transform>
{
    public BBParameter<List<float>> stopDelays;
    public BBParameter<bool> enableForceSkip;
    private Coroutine coroutine;
    private bool skip;

    private string ON_SPINBUTTON_EVENT = "OnSpinButtonEvent";

    protected override void OnExecute()
    {
        var reels = agent.GetComponent<BaseSlotMachine>().GetReels();
        int reelCount = reels.Count;
        for (int i = 0; i < reelCount; ++i)
        {
            if (reels[i].beginRow != 0)
                reels[i].movement.Spin();
        }
        skip = false;

        MessageDispatcher.Register(ON_SPINBUTTON_EVENT, OnSpinButton);
        coroutine = StartCoroutine(StopCo());
    }

    protected override void OnStop()
    {
        MessageDispatcher.UnRegister(ON_SPINBUTTON_EVENT, OnSpinButton);
    }

    private IEnumerator StopCo()
    {
        var slotMachine = agent.GetComponent<BaseSlotMachine>();
        var reels = slotMachine.GetReels();
        int reelCount = reels.Count;
        int i = 0;
        for (int column = 0; column < reelCount; ++column)
        {
            if (reels[column].movement.IsSpinning())
            {
                reels[column].movement.Stop();

                float elapsedTime = 0f;
                while (!skip && elapsedTime < stopDelays.value[i])
                {
                    yield return new WaitForEndOfFrame();
                    elapsedTime += Time.deltaTime;
                }
                ++i;
            }
        }

        while (slotMachine.movement.IsSpinning())
            yield return null;

        EndAction();
    }

    private void OnSpinButton(EventData eventData)
    {
        if (enableForceSkip.value)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);

            var reels = agent.GetComponent<BaseSlotMachine>().GetReels();
            int reelCount = reels.Count;
            for (int i = 0; i < reelCount; ++i)
            {
                if (!reels[i].movement.IsStopped())
                    reels[i].movement.ForceStop();
            }

            EndAction();
        }
        else 
        {
            skip = true;
        }
        
    }
}

}
