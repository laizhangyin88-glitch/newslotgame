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
    [Serializable]
    public abstract class LobbyMetaIcon
    {
        protected GameObject iconObject;
        protected ContextElement scrollContextElement;
        protected ContextElement badgeAreaElement;
        protected List<EventInfoType> checkEventInfoTypeList;

        protected string bundleName;
        protected string sharedBundleName;
        protected string iconAssetName;

        protected EventInfoType eventType;
        protected int eventId = 0;
        protected long endTimestamp;

        protected bool enableShare;
        [SerializeField]protected string eventName;

        protected List<string> bundleList;

        protected const string LOBBY_BUTTON_ASSET_NAME = "Lobby Button Scene";
        protected const string INGAME_BUTTON_ASSET_NAME = "In Game Button Scene";

        protected const string ON_PASSIVE_EVENT = "OnPassiveEvent";
        protected const string ON_REFRESH_PASSIVE = "RefreshPassive";
        protected const string ON_START_PASSIVE = "StartPassive";
        protected const string ON_REFRESH_META_GAME = "OnRefreshMetaGame";
        protected const string ON_REFRESH_ICON_CHECK = "OnRefreshIconCheck";

        public abstract void OnStart();
        public abstract void OnEventRecv(string eventName, EventData eventData);

        protected abstract bool CheckActiveEvent();

        public virtual void OnInit(ContextElement _scrollContextElement)
        {
            scrollContextElement = _scrollContextElement;
            OnInitCheckEventInfoTypeList();
        }

        protected virtual void OnInitCheckEventInfoTypeList()
        {
            if (checkEventInfoTypeList == null)
                checkEventInfoTypeList = new List<EventInfoType>();
            if (checkEventInfoTypeList.Count > 0)
                checkEventInfoTypeList.Clear();
        }

        protected void DestroyIconObject()
        {
            if (iconObject != null)
            {
                GameObject.Destroy(iconObject);
                iconObject = null;
            }
        }

        protected bool ChackEventInfoType(EventInfoType type)
        {
            if (checkEventInfoTypeList != null && checkEventInfoTypeList.Count > 0)
            {
                return checkEventInfoTypeList.Contains(type);
            }
            return false;
        }

        protected void MakeEventButton(string assetName)
        {
            DestroyIconObject();

            if (CheckActiveEvent())
            {
                if (scrollContextElement != null)
                    iconObject = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, assetName, scrollContextElement.transform, "", assetName);
            }
        }

        public bool IsActive()
        {
            return iconObject != null && iconObject.activeSelf;
        }
    }
}
