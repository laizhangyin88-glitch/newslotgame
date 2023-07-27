using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode.ClientModels;
using ParadoxNotion;
using ParadoxNotion.Services;
using NodeCanvas.Framework;

namespace BagelCode
{
    public class LobbyVipDealIcon : LobbyMetaIcon
    {
        private bool isVipDealActive = false;

        public override void OnStart()
        {
            if (iconObject == null)
                CheckVipDealEvent();
        }

        public override void OnEventRecv(string eventName, EventData eventData)
        {

        }

        protected override bool CheckActiveEvent()
        {
            return !BlackboardQueryUtils.IsVipDealTierLock() && BlackboardQueryUtils.GetActiveVipDealInfo() != null;
        }

        private void CheckVipDealEvent()
        {
            if (!BlackboardQueryUtils.IsVipDealTierLock())
            {
                var vipDealInfo = BlackboardQueryUtils.GetActiveVipDealInfo();
                isVipDealActive = vipDealInfo != null;
            }
            else
                isVipDealActive = false;

            if (isVipDealActive)
                MakeEventButton("Lobby Button VIP Deal Area");
        }
    }
}