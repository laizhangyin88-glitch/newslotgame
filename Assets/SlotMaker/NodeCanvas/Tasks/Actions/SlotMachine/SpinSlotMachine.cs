using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace SlotMaker.Tasks.Actions
{
    [Category("★ SlotMaker/SlotMachine")]
    public class SpinSlotMachine : ActionTask<Transform>
    {
        protected override void OnExecute()
        {
            var reels = agent.GetComponent<SlotMachine>().GetReels();
            int reelCount = reels.Count;
            for (int i = 0; i < reelCount; ++i)
            {
                reels[i].movement.Spin();
            }

            EndAction();
        }
    }
}
