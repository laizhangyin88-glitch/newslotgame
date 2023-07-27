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
    public class LobbyMetaGameIcon : LobbyMetaIcon
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
            return BlackboardQueryUtils.GetMetaGameEventInfo() != null;
        }

        protected override void OnInitCheckEventInfoTypeList()
        {
            base.OnInitCheckEventInfoTypeList();

            checkEventInfoTypeList.Add(EventInfoType.LUCKY_FIVE);
            checkEventInfoTypeList.Add(EventInfoType.COLLECTING_GAME);
            checkEventInfoTypeList.Add(EventInfoType.SEASON_PASS);
            checkEventInfoTypeList.Add(EventInfoType.BOSS_RAIDERS);
            checkEventInfoTypeList.Add(EventInfoType.CLUB_ARENA);
        }

        private void CheckMetaEvent()
        {
            EventInfo eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo();
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
                eventName = BlackboardQueryUtils.GetMetaGameEventName().ToUpper();

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
                    MakeEventButton("Event Button");
            }
            else
                DestroyIconObject();
        }
    }
}
