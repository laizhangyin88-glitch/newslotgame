using System.Collections;
using BagelCode.Slots.TRR.Popup;
using BagelCode.Slots.TRR.Utillity;
using ParadoxNotion;
using SlotMaker;
using UnityEngine;
namespace BagelCode.Slots.TRR.FeatureAdmin
{
    public class TRRJackpotAdmin : TRRFeatureAdmin
    {
        public override string CONTENT_EVENT_PLAY_FEATURE => "TRRJackpotBonus";
        public override string CONTENT_EVENT_ON_FINISH_FEATURE => "TRROnFinishJackpotBonus";

        public const string SignBoardBeginJackpotBonus = "TRRSignBoardFinishJackpotBonus";

        public TRRJackpotPopup miniJackpotPopup;
        public TRRJackpotPopup majorJackpotPopup;
        public TRRJackpotPopup grandJackpotPopup;

        private TRRJackpotPopup currentActivePopup;

        protected override void RegisterEventDelegates()
        {
        }

        protected override IEnumerator OnFeaturePlayCoroutine()
        {
            yield return new WaitForSeconds(2f);
            MessageDispatcher.Dispatch(SendEvent.ON_SLOT_EVENT, new EventData(SignBoardBeginJackpotBonus));
            int jackpotIndex = TRRUtillity.TryGetGlobalBlackBoardVariable<int>("./bonus/response/jackpotIndex");
            long jackpotEarnCredit = TRRUtillity.TryGetGlobalBlackBoardVariable<long>("./bonus/response/earnCredit");

            if (jackpotIndex == 0) currentActivePopup = miniJackpotPopup;
            else if (jackpotIndex == 1) currentActivePopup = majorJackpotPopup;
            else if (jackpotIndex == 2) currentActivePopup = grandJackpotPopup;

            yield return ShowDefualtPopupCoroutine(currentActivePopup, () =>
            {
                currentActivePopup.jackpotIndex = jackpotIndex;
                currentActivePopup.earnCredit = jackpotEarnCredit;
            });
            yield return new WaitForSeconds(1f);
        }
    }
}