using System.Collections;
using BagelCode.Slots.TRR.Utillity;
using UnityEngine;
namespace BagelCode.Slots.TRR.Popup
{
    public class TRRTreasureBonusTriggerPopup : TRRPopup
    {
        [SerializeField] private float removeDelay;

        protected override IEnumerator OnPlayCoroutine()
        {
            TRRUtillity.ChangeSnapShot("Content_Popup");
            TRRUtillity.PlaySound("Bonus Game Popup");
            yield return new WaitForSeconds(removeDelay);
            TRRUtillity.ChangeSnapShot("Content_Main");
        }

    }
}