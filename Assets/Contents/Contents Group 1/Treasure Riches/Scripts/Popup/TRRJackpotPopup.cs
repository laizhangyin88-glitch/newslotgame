using System.Collections;
using BagelCode.Slots.TRR.Utillity;
using TMPro;
using UnityEngine;
namespace BagelCode.Slots.TRR.Popup
{
    public class TRRJackpotPopup : TRRPopup
    {
        [SerializeField] private TextMeshProUGUI creditText;
        [SerializeField] private float removeDelay;

        public long earnCredit;
        public long jackpotIndex;

        protected override IEnumerator OnPlayCoroutine()
        {
            TRRUtillity.ChangeSnapShot("Content_Popup");
            if (jackpotIndex == 0) TRRUtillity.PlaySound("Mini Jackpot Popup");
            else if (jackpotIndex == 1) TRRUtillity.PlaySound("Major Jackpot Popup");
            else if (jackpotIndex == 2) TRRUtillity.PlaySound("Grand Jackpot Popup");

            creditText.text = earnCredit.ToString("#,##0");
            yield return new WaitForSeconds(removeDelay);
            TRRUtillity.ChangeSnapShot("Content_Main");
        }
    }
}