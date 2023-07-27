using System.Collections;

namespace BagelCode.Scratcher
{
    public class ScratcherAsyncActionPlayGroup : ScratcherPlayGroup
    {
        public IEnumerator asyncAction;

        public ScratcherAsyncActionPlayGroup(IEnumerator _asyncAction)
        {
            asyncAction = _asyncAction;
        }

        public override IEnumerator Play(ScratcherPlayController controller)
        {
            yield return controller.StartCoroutine(asyncAction);

            SendEvent(instantOpenDelay, controller);
        }
    }
}