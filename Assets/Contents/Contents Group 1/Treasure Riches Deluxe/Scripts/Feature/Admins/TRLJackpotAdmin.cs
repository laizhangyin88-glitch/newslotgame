using System.Collections;
using GameStudio.Slot.TRL.Popup;
using GameStudio.Slot.TRL.Utillity;
using ParadoxNotion;
using SlotMaker;
using UnityEngine;
namespace GameStudio.Slot.TRL.FeatureAdmin
{
    public class TRLJackpotAdmin : TRLFeatureAdmin
    {
        public override string CONTENT_EVENT_PLAY_FEATURE => "TRLJackpotBonus";
        public override string CONTENT_EVENT_ON_FINISH_FEATURE => "TRLOnFinishJackpotBonus";

        public const string SignBoardBeginJackpotBonus = "TRLSignBoardFinishJackpotBonus";

        public TRLJackpotPopup miniJackpotPopup;
        public TRLJackpotPopup majorJackpotPopup;
        public TRLJackpotPopup grandJackpotPopup;

        private TRLJackpotPopup currentActivePopup;

        protected override void RegisterEventDelegates()
        {
        }

        protected override IEnumerator OnFeaturePlayCoroutine()
        {
            yield return new WaitForSeconds(2.5f);
            MessageDispatcher.Dispatch(SendEvent.ON_SLOT_EVENT, new EventData(SignBoardBeginJackpotBonus));
            int jackpotIndex = TRLUtillity.TryGetGlobalBlackBoardVariable<int>("./bonus/response/jackpotIndex");
            long jackpotEarnCredit = TRLUtillity.TryGetGlobalBlackBoardVariable<long>("./bonus/response/earnCredit");

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