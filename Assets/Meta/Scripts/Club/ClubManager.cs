using System;
using System.Collections.Generic;
using ParadoxNotion;
using SlotMaker;
using Sirenix.OdinInspector;

namespace BagelCode
{
    public class ClubManager : MonoSingleton<ClubManager>
    {
        public event Action<long> OnClubLeaved;
        /// <summary>
        /// Club is joined (include Create,Change) 
        /// </summary>
        public event Action<long> OnClubJoined;
        /// <summary>
        /// Club is created (not include new joined) 
        /// </summary>
        public event Action<long> OnClubCreated;
        /// <summary>
        /// Another club joined. (not include new joined) 
        /// </summary>
        public event Action<long, long> OnClubChanged;
        [ReadOnly]
        [ShowInInspector]
        public long curClubId { get; private set; }

        //System Delegate
        private Dictionary<string, MessageDispatcher.EventDelegate> systemDelegates = new Dictionary<string, MessageDispatcher.EventDelegate>();
        //Meta Delegate
        private Dictionary<string, MessageDispatcher.EventDelegate> metaUIDelegates = new Dictionary<string, MessageDispatcher.EventDelegate>();

        private const string JOIN_SUCCESS_EVENT = "OnJoinClubSuccess";
        private const string CREATE_SUCCESS_EVENT = "OnCreateClubSuccess";

        private void OnFinishedLogin(EventData eventData)
        {
            //Club Chat Channel Connect 
            var clubIdVariable = BlackboardUtils.FindVariable<long>("/me/clubId");
            curClubId = clubIdVariable.value;

            clubIdVariable.onValueChanged += (key, val) =>
            {
                var clubId = (long)val;
                if (clubId == curClubId) return;
                long preClubId = curClubId;
                curClubId = clubId;

                if (curClubId == 0)
                {
                    OnClubLeaved?.Invoke(preClubId);
                }
                else
                {
                    if (preClubId != 0)
                    {
                        OnClubChanged?.Invoke(preClubId, curClubId);
                    }
                    OnClubJoined?.Invoke(curClubId);
                }
            };

        }

        private void OnCreated(EventData eventData)
        {
            long clubId = (long)eventData.value;
            curClubId = clubId;
            OnClubCreated?.Invoke(curClubId);
        }

        private void Awake()
        {
            systemDelegates[SystemEventDefine.ON_FINISHED_LOGIN_EVENT] = OnFinishedLogin;
            metaUIDelegates[CREATE_SUCCESS_EVENT] = OnCreated;
        }
        private void OnEnable()
        {
            MessageDispatcher.Register(MetaEventDefine.ON_META_UI_EVENT, OnMetaUIEvent);
            MessageDispatcher.Register(SystemEventDefine.ON_SYSTEM_EVENT, OnSystemEvent);
        }
        private void OnDisable()
        {
            MessageDispatcher.UnRegister(MetaEventDefine.ON_META_UI_EVENT, OnMetaUIEvent);
            MessageDispatcher.Register(SystemEventDefine.ON_SYSTEM_EVENT, OnSystemEvent);
        }

        private void OnSystemEvent(EventData eventData)
        {
            MessageDispatcher.EventDelegate del;
            if (systemDelegates.TryGetValue(eventData.name, out del))
                del.Invoke(eventData);
        }

        private void OnMetaUIEvent(EventData eventData)
        {
            MessageDispatcher.EventDelegate del;
            if (metaUIDelegates.TryGetValue(eventData.name, out del))
                del.Invoke(eventData);
        }
    }
}