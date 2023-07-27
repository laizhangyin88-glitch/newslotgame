using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using ParadoxNotion;
using ParadoxNotion.Services;

namespace BagelCode
{
    public class LobbyVipDealBehaviour : MonoBehaviour
    {
        private GameObject vipDealObj = null;

        private SimpleReserveTimer _reserveTimer;
        private SimpleReserveTimer reserveTimer
        {
            get
            {
                if (_reserveTimer == null)
                {
                    _reserveTimer = GetComponent<SimpleReserveTimer>();
                    if (_reserveTimer == null)
                        _reserveTimer = gameObject.AddComponent<SimpleReserveTimer>();
                }
                return _reserveTimer;
            }
        }

        private void OnEnable()
        {
            UpdateVariables();
        }

        public void UpdateVariables()
        {
            if(!BlackboardQueryUtils.IsVipDealTierLock())
            {
                var vipDealInfo = BlackboardQueryUtils.GetActiveVipDealInfo();

                if(vipDealInfo != null)
                {
                    var endTimestamp = vipDealInfo.GetValue<long>("endTimestamp");

                    ActivateVipDeal(endTimestamp);
                }
                else
                {
                    DeactivateVipDeal();
                }
            }
            else
            {
                DeactivateVipDeal();
            }
        }

        private void ActivateVipDeal(long endTimestamp)
        {
            // Make Icon
            if(vipDealObj == null)
                vipDealObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Lobby Button VIP Deal", transform);

            vipDealObj.SetActive(true);

            reserveTimer.SetReserveCallback(endTimestamp, () => { OnEndTimer(); });
        }

        private void DeactivateVipDeal()
        {
            if(vipDealObj != null)
                vipDealObj.SetActive(false);

            reserveTimer.Stop();
        }

        private void OnEndTimer()
        {
            UpdateVariables();
        }
    }
}