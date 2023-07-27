using System.Collections;
using UnityEngine;
using SlotMaker;

namespace GameStudio.Slot.FSF
{
    public class FSFCatController : FeatureController
    {
        [SerializeField]
        private Animator catAnimator;

        protected override string ON_FEATURE_BEGIN_EVENT { get => "OrderCatFishing"; }
        protected override string ON_FEATURE_END_EVENT { get => "OrderCatFishingDone"; }

        public const string ON_SOUND_EVENT = "OnSoundEvent";
        public const string STOP_BGM_EVENT = "StopBgm";

        protected override IEnumerator OnPlayCoroutine()
        {
            catAnimator.SetTrigger("Fishing");
            if (BlackboardUtils.GetOrCreateVariable<bool>(null, "./customData/hadCommunityStarted").value)
                MessageDispatcher.Dispatch(ON_SOUND_EVENT, new ParadoxNotion.EventData(STOP_BGM_EVENT));

            GSManager.Instance.GetHandler("Fishing Cat").Play();
            yield return new WaitForSeconds(2.2f);
        }

        protected override void OnFinish()
        {
        }

        protected override void OnStart()
        {
        }
    }
}