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
    public class LobbyOtherMetaGameIcon : LobbyMetaIcon
    {
        public override void OnStart()
        {
            CheckMetaEvent();
        }

        public override void OnEventRecv(string eventName, EventData eventData)
        {
            if (iconObject != null)
            {
                // On event state
                bool isNext = false;
                if (eventName.Equals(ON_PASSIVE_EVENT) && eventData.name.Equals(ON_START_PASSIVE))
                    isNext = ChackEventInfoType(((EventData<EventInfoType>)eventData).value);
                else if (eventName.Equals(ON_PASSIVE_EVENT) && eventData.name.Equals(ON_REFRESH_PASSIVE))
                    isNext = true;
                else if (eventName.Equals(MetaEventDefine.ON_META_UI_EVENT) && eventData.name.Equals(ON_REFRESH_META_GAME))
                    isNext = true;

                if (isNext)
                {
                    DestroyIconObject();
                    OnStart();
                }
            }
            else
            {
                // Non event state
                if (eventName.Equals(ON_PASSIVE_EVENT) && eventData.name.Equals(ON_START_PASSIVE))
                {
                    if (ChackEventInfoType(((EventData<EventInfoType>)eventData).value))
                        OnStart();
                }
            }
        }

        protected override bool CheckActiveEvent()
        {
            return BlackboardQueryUtils.GetOtherMetaGameEventInfo() != null;
        }

        protected override void OnInitCheckEventInfoTypeList()
        {
            base.OnInitCheckEventInfoTypeList();

            checkEventInfoTypeList.Add(EventInfoType.GEM_JACKPOT);
        }

        private void CheckMetaEvent()
        {
            EventInfo eventInfo = BlackboardQueryUtils.GetOtherMetaGameEventInfo();
            bool isLockedFeature = BlackboardQueryUtils.IsLockedFeature(LockedFeatureType.META_GAME);

            if (eventInfo != null && (!isLockedFeature || MetaGameUtils.IsMetaEventLevelLock()))
            {
                bundleName = BlackboardQueryUtils.GetMetaBundleName(eventInfo);
                sharedBundleName = BlackboardQueryUtils.GetSharedMetaBundleName(eventInfo, false);
                iconAssetName = LOBBY_BUTTON_ASSET_NAME;

                eventType = eventInfo.type;
                eventId = eventInfo.id;
                endTimestamp = eventInfo.endTimestamp;

                enableShare = BlackboardQueryUtils.IsShareEnabled(eventInfo);
                eventName = BlackboardQueryUtils.GetOtherMetaGameEventName().ToUpper();

                bundleList = new List<string>();
                bundleList.Add(bundleName);
                if (!string.IsNullOrEmpty(sharedBundleName))
                    bundleList.Add(sharedBundleName);
            }
            else
            {
                bundleName = "";
                sharedBundleName = "";
                iconAssetName = "";

                eventType = EventInfoType.UNKNOWN;
                eventId = 0;
                endTimestamp = 0;

                enableShare = false;
                eventName = "";

                bundleList = null;
            }

            if (eventId != 0)
            {
                if (iconObject == null)
                    MakeEventButton("Other Event Button");
            }
            else
                DestroyIconObject();
        }
    }
}
