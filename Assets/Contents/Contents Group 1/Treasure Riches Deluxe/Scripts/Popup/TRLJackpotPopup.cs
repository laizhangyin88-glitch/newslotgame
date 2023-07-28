using System.Collections;
using GameStudio.Slot.TRL.Utillity;
using TMPro;
using UnityEngine;
namespace GameStudio.Slot.TRL.Popup
{
    public class TRLJackpotPopup : TRLPopup
    {
        [SerializeField] private TextMeshProUGUI creditText;
        [SerializeField] private float removeDelay;

        public long earnCredit;
        public long jackpotIndex;

        protected override IEnumerator OnPlayCoroutine()
        {
            TRLUtillity.ChangeSnapShot("Content_Popup");
            if (jackpotIndex == 0) TRLUtillity.PlaySound("Mini Jackpot Popup");
            else if (jackpotIndex == 1) TRLUtillity.PlaySound("Major Jackpot Popup");
            else if (jackpotIndex == 2) TRLUtillity.PlaySound("Grand Jackpot Popup");

            creditText.text = earnCredit.ToString("#,##0");
            yield return new WaitForSeconds(removeDelay);
            TRLUtillity.ChangeSnapShot("Content_Main");
        }
    }
}