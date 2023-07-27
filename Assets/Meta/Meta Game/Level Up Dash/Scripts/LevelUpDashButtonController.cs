using SlotMaker;
using System.Collections;
using UnityEngine;

namespace BagelCode.LevelUpDash
{
    public class LevelUpDashButtonController : MetaGameEventButtonController
    {
        private ContextElement tagElement;

        public override void InitProperty()
        {
            base.InitProperty();

            tagElement = ContextUtils.FindElement(root, "Event Timer Area/Event Tag Without Text", ContextSearchingType.FullNameSearch);

            MetaContextElementUtils.SetClickable(root, () => StartCoroutine(OpenLevelDashCoroutine()), false);

            SetTimer();
        }

        private IEnumerator OpenLevelDashCoroutine()
        {
            yield return new WaitForEndOfFrame(); // skip for context id generated
            EventSender.SendGlobalMetaEvent(LevelUpDash.Events.OPEN_LEVEL_UP_DASH);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            SetTimer();
        }

        private void SetTimer()
        {
            if (tagElement != null)
            {
                var eventController = tagElement.GetComponent<EventTagController>();
                eventController.Initialize(
                    LevelUpDash.Utils.EndTimestamp,
                    "TIME_FORMAT_HHMMSS_TOTALHOUR",
                    "",
                    "Ended",
                    true,
                    gameObject,
                    null);
            }
        }
    }
}
