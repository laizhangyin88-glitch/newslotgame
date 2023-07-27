using System.Collections;
using NodeCanvas.Framework;
using UnityEngine;

namespace BagelCode.Scratcher
{
    public class ScratcherActionPlayGroup : ScratcherPlayGroup
    {
        public System.Action action;
        public float playTime;

        public ScratcherActionPlayGroup(System.Action _action, float _playTime)
        {
            action = _action;
            playTime = _playTime;
        }

        public override IEnumerator Play(ScratcherPlayController controller)
        {
            action?.Invoke();

            yield return new WaitForSeconds(playTime);

            SendEvent(instantOpenDelay, controller);
        }
    }
}