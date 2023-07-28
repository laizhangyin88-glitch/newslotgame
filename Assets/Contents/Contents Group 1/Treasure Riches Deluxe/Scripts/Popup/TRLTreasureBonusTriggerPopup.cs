using System.Collections;
using GameStudio.Slot.TRL.Utillity;
using UnityEngine;
namespace GameStudio.Slot.TRL.Popup
{
    public class TRLTreasureBonusTriggerPopup : TRLPopup
    {
        [SerializeField] private float removeDelay;

        protected override IEnumerator OnPlayCoroutine()
        {
            TRLUtillity.ChangeSnapShot("Content_Popup");
            TRLUtillity.PlaySound("Bonus Game Popup");
            yield return new WaitForSeconds(removeDelay);
            TRLUtillity.ChangeSnapShot("Content_Main");
        }

    }
}