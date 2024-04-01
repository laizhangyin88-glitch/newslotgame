using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BagelCode.ClientModels;
using BSS.Utils;
using NodeCanvas.Framework;
using ParadoxNotion;
using Sirenix.OdinInspector;
using SlotMaker;
using UnityEngine;


namespace BagelCode.Chat
{
    public enum ChannelType
    {
        None = 0,
        Global = 1,
        Club = 2,
        Game = 3,
        Private = 4,
    }
    /// <summary>
    /// Manage chat data received from ChatMessenger by channel type.
    /// </summary>
    public class ChatMetaManager : MonoWeakSingleton<ChatMetaManager>, IChatMessengerSubscriber {

#if UNITY_EDITOR
        [Button]
        private void TestEmoticonSetup()
        {
            int EMOTICON_COUNT = 18;
            emoticonList.Clear();
            for(int i = 0; i < EMOTICON_COUNT; ++i)
            {
                string prefabName = string.Format("Icon Emoticon {0:00}", i + 1);
                emoticonList.Add(AssetBundleManager.LoadAsset<GameObject>(MetaStringDefine.LOBBY_BUNDLE_NAME, prefabName));
            }
        }
#endif

        [ShowInInspector]
        public List<GameObject> emoticonList = new List<GameObject>();

        [ShowInInspector]
        [ReadOnly]
        private List<IChatMetaListener> listeners = new List<IChatMetaListener>();

        [ShowInInspector]
        [ReadOnly]
        public int globalChannelIndex { get; private set; }
        public List<(string channelID, string channelName)> globalChannelInfos { get; private set; } = new List<(string channelId, string channelName)>();
        [ShowInInspector]
        [ReadOnly]
        private Dictionary<ChannelType, string> connectChannels = new Dictionary<ChannelType, string>();
        [ShowInInspector]
        [ReadOnly]
        private Dictionary<ChannelType, int> unreadMessageCounts = new Dictionary<ChannelType, int>();
        [ShowInInspector]
        [ReadOnly]
        private Dictionary<ChannelType, IComparer<ChatUserProfileInfo>> userProfileComparers = new Dictionary<ChannelType, IComparer<ChatUserProfileInfo>>()
        {
            [ChannelType.Global] = new GlobalChatProfileComp(),
            [ChannelType.Club] = new ClubChatProfileComp(),
            [ChannelType.Game] = new GameChatProfileComp()
        };

        private Dictionary<string, MessageDispatcher.EventDelegate> delegates = new Dictionary<string, MessageDispatcher.EventDelegate>();

        private Variable<bool> enableGlobalChatCount = null;

        public delegate void CallbackUnreadMessage();
        private CallbackUnreadMessage unreadMessageCallback;

        private void Clear()
        {
            globalChannelIndex = 0;
            globalChannelInfos.Clear();
            connectChannels.Clear();
            unreadMessageCounts.Clear();
            unreadMessageCallback = null;
            enableGlobalChatCount = null;
        }

        public void AddListener(IChatMetaListener listener)
        {
            listeners.Add(listener);
        }

        public void RemoveListener(IChatMetaListener listener)
        {
            listeners.Remove(listener);
        }

        public void SubscribeMessageChangeListener(CallbackUnreadMessage listener)
        {
            unreadMessageCallback += listener;
        }

        public void UnsubscribeMessageChangeListener(CallbackUnreadMessage listener)
        {
            unreadMessageCallback -= listener;
        }

        public bool IsValidate(ChannelType channelType) {
            switch (channelType) {
                case ChannelType.Global:
                    return true;
                case ChannelType.Club:
                    var clubIDVar = BlackboardUtils.FindVariable<long>("/me/clubId");
                    return clubIDVar != null && clubIDVar.value != 0;
                case ChannelType.Game:
                    var isInGameVar = BlackboardUtils.FindVariable<bool>("/inGame");
                    return isInGameVar != null && isInGameVar.value;
                case ChannelType.Private:
                    return false;
                default:
                    return false;
            }
        }

        public string GetGlobalChannelName(int channelIndex)
        {
            if (channelIndex < 0)
                return globalChannelInfos[globalChannelIndex].channelName;
            return globalChannelInfos[channelIndex].channelName;
        }

        public string GetGlobalChannelID(int channelIndex)
        {
            if (globalChannelInfos.IsValidIndex(channelIndex))
                return globalChannelInfos[channelIndex].channelID;
            else return string.Empty;
        }

        public string GetChannelID(ChannelType channelType)
        {
            switch (channelType)
            {
                case ChannelType.Global:
                    return GetGlobalChannelID(globalChannelIndex);
                case ChannelType.Club:
                    if (!IsValidate(ChannelType.Club)) return "";
                    long clubID = BlackboardUtils.FindValue<long>("/me/clubId");
                    return $"club:{clubID}";
                case ChannelType.Game:
                    if (!IsValidate(ChannelType.Game)) return "";
                    var roomBB = ContentBlackboard.Get().GetValue<Blackboard>("room");
                    var roomID = roomBB.GetValue<string>("roomChatChannelId");
                    return roomID;
                case ChannelType.Private:
                    return "";
                default:
                    return "";
            }
        }

        public ChannelType GetChannelType(string channelID) {
            if (globalChannelInfos.Any(x => x.channelID == channelID)) return ChannelType.Global;
            else if (channelID.StartsWith("club:", StringComparison.CurrentCulture)) return ChannelType.Club;
            else if (IsValidate(ChannelType.Game) && GetChannelID(ChannelType.Game) == channelID) return ChannelType.Game;
            return ChannelType.None;
        }

        public string GetDefaultGlobalChannelID()
        {
            return BlackboardUtils.FindValue<string>("/globalChatChannelId");
        }

        public string GetConnectedChannelID(ChannelType channelType)
        {
            if (!connectChannels.ContainsKey(channelType)) return "";
            return connectChannels[channelType];
        }

        public void ConfirmReadMessage(ChannelType channelType)
        {
            unreadMessageCounts[channelType] = 0;

            if (unreadMessageCallback != null)
                unreadMessageCallback.Invoke();
        }

        public int GetUnreadMessageCount(ChannelType channelType)
        {
            if (!unreadMessageCounts.ContainsKey(channelType)) return 0;
            return unreadMessageCounts[channelType];
        }

        public void ChangeGlobalChannel(int globalIndex)
        {

#if NEW_NET
            Debug.Log("TOP【remove rpc】: /v0/chat/subscribe");
            return;
#endif

            string preGlobalChannelID = GetConnectedChannelID(ChannelType.Global);
            ChatMessenger.Instance.DisconnectChannel(preGlobalChannelID);
            ChatMessenger.Instance.Unsubscribe(preGlobalChannelID, this);
            // globalChannelIndex = globalIndex;
            // string globalChannelID = GetChannelID(ChannelType.Global);
            string globalChannelID = GetGlobalChannelID(globalIndex);
            ChatMessenger.Instance.Subscribe(globalChannelID, this);
            CoroutineUtility.ExcuteAfterCondition(this,
                () => ChatMessenger.Instance.IsConnected(globalChannelID),//Conditon
                () =>
                {
                    // UpdateWelcomeChatPoll(globalChannelID);
                    // Debug.LogError("Change Global Channel");
                    globalChannelIndex = globalIndex;
                    listeners.ForEach(x => x.OnGlobalChannelChanged(preGlobalChannelID, globalChannelID));
                }
            );
        }

        public IEnumerable<ChannelType> GetValidateChannelTypes()
        {
            return Enum.GetNames(typeof(ChannelType)).Select(x => (ChannelType)Enum.Parse(typeof(ChannelType), x)).Where(x => IsValidate(x));
        }

        public IComparer<ChatUserProfileInfo> GetProfileComparer(ChannelType channelType)
        {
            if (!userProfileComparers.ContainsKey(channelType)) return userProfileComparers[ChannelType.Global];
            return userProfileComparers[channelType];
        }



        #region Event Callback
        //System Delegate
        private Dictionary<string, MessageDispatcher.EventDelegate> systemDelegates = new Dictionary<string, MessageDispatcher.EventDelegate>();
        //Meta Delegate
        private Dictionary<string, MessageDispatcher.EventDelegate> metaUIDelegates = new Dictionary<string, MessageDispatcher.EventDelegate>();

        private void OnFinishedLogin(EventData eventData)
        {

#if NEW_NET
            Debug.Log("TOP【remove rpc】: /v0/chat/subscribe");
            return;
#endif
            if (ApplicationSettings.LogTest())
                Debug.Log("ChatMetaManager Init");

            //Global Channel Infos Init
            var infoBBList = BlackboardUtils.FindValue<List<Blackboard>>("/globalChatInfoList");
            globalChannelInfos = infoBBList.Select(x => (x.GetValue<string>("channelId"), x.GetValue<string>("channelName"))).ToList();

            enableGlobalChatCount = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "userOptions/globalChat");

            //Global Chat Channel Connect
            ChatMessenger.Instance.Subscribe(GetDefaultGlobalChannelID(), this);
            // UpdateWelcomeChatPoll(GetDefaultGlobalChannelID());

            //Club Chat Channel Connect 
            var clubIDVariable = BlackboardUtils.FindVariable<long>("/me/clubId");
            if (IsValidate(ChannelType.Club))
            {
                ChatMessenger.Instance.Subscribe(GetChannelID(ChannelType.Club), this);
            }

            clubIDVariable.onValueChanged += (key, val) =>
            {
                var clubID = (long)val;
                if (clubID == 0)
                {
                    if (connectChannels.ContainsKey(ChannelType.Club))
                    {
                        string preClubChannelID = connectChannels[ChannelType.Club];
                        ChatMessenger.Instance.Unsubscribe(preClubChannelID, this);
                        ChatMessenger.Instance.DisconnectChannel(preClubChannelID);
                        listeners.ForEach(x => x.OnClubLeaved(preClubChannelID));
                    }
                }
                else
                {
                    ChatMessenger.Instance.Subscribe(GetChannelID(ChannelType.Club), this);
                }
            };

            //Game Chat Channel Connect 
            var inGameVariable = BlackboardUtils.FindVariable<bool>("/inGame");
            inGameVariable.onValueChanged += (key, val) =>
            {
                var inGame = (bool)val;
                if (!inGame)
                {
                    if (connectChannels.ContainsKey(ChannelType.Game))
                    {
                        string preGameChannelID = connectChannels[ChannelType.Game];
                        ChatMessenger.Instance.Unsubscribe(preGameChannelID, this);
                        ChatMessenger.Instance.DisconnectChannel(preGameChannelID);
                    }
                }
                else
                {
                    string channelID = GetChannelID(ChannelType.Game);
                    ChatMessenger.Instance.Subscribe(GetChannelID(ChannelType.Game), this);
                }
            };
        }

        private void UpdateWelcomeChatPoll(string channelID)
        {
            // ChatMessenger.Instance.MakeWelcomChatMessageData(channelID);
        }

        private void Awake()
        {
            systemDelegates[SystemEventDefine.ON_FINISHED_LOGIN_EVENT] = OnFinishedLogin;
        }

        private void Start()
        {
            delegates[SystemEventDefine.ON_SYSTEM_RESET_EVENT] = OnSystemReset;
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

        private void OnSystemReset(EventData eventData)
        {
            Clear();
        }

        private void OnSystemEvent(EventData eventData)
        {
            MessageDispatcher.EventDelegate del;
            if (systemDelegates.TryGetValue(eventData.name, out del))
                del.Invoke(eventData);

            if (delegates.TryGetValue(eventData.name, out del))
                del.Invoke(eventData);
        }

        private void OnMetaUIEvent(EventData eventData)
        {
            MessageDispatcher.EventDelegate del;
            if (metaUIDelegates.TryGetValue(eventData.name, out del))
                del.Invoke(eventData);

            if (delegates.TryGetValue(eventData.name, out del))
                del.Invoke(eventData);
        }

        public void OnConnectedChannel(string channelID)
        {
            var type = GetChannelType(channelID);
            if (type != ChannelType.None)
            {
                connectChannels[type] = channelID;
                unreadMessageCounts[type] = 0;

                if (unreadMessageCallback != null)
                    unreadMessageCallback.Invoke();
            }
        }

        public void OnDisconnectedChannel(string channelID)
        {
            foreach (var it in connectChannels)
            {
                if (it.Value == channelID)
                {
                    unreadMessageCounts[it.Key] = 0;
                    connectChannels.Remove(it.Key);

                    if (unreadMessageCallback != null)
                        unreadMessageCallback.Invoke();
                    return;
                }
            }
        }


        public void OnAddChatMessages(string channelID, List<ChatMessageData> chatDataList)
        {
            string meID = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "me/userId").value;
            //Unread Message Count Add
            // Debug.LogError(connectChannels.Count);
            // Debug.LogError(channelID);
            // Debug.LogError(chatDataList.Count);
            var ignoreList = BlackboardQueryUtils.GetIgnoreUserIdAll();
            foreach (var it in connectChannels)
            {
                if (it.Value == channelID)
                {
                    if (it.Key == ChannelType.Global && enableGlobalChatCount != null && enableGlobalChatCount.value == false)
                    {
                        break;
                    }

                    foreach (ChatMessageData chatA in chatDataList)
                    {
                        if (chatA.chatPoll.userId != meID &&
                            !ignoreList.Contains(chatA.chatPoll.userId))
                        {
                            unreadMessageCounts[it.Key] += chatDataList.Count;
                        }
                    }

                    if (unreadMessageCallback != null)
                        unreadMessageCallback.Invoke();

                    break;
                }
            }

            var type = GetChannelType(channelID);
            if (type == ChannelType.None) return;
            listeners.ForEach(x => x.OnAddChatMessages(type, channelID, chatDataList));
            // Send system chat - BI Send
            BI_client_system_chat(type, chatDataList);
        }

        public void OnAddRecentChatMessages(string channelID, List<ChatMessageData> chatDataList)
        {
            var type = GetChannelType(channelID);
            if (type == ChannelType.None) return;

            listeners.ForEach(x => x.OnAddRecentChatMessages(type, channelID, chatDataList));
        }

        public void OnRemoveRequestChatMessages(string channelID, List<ChatMessageData> chatDataList)
        {
            var type = GetChannelType(channelID);
            if (type == ChannelType.None) return;

            listeners.ForEach(x => x.OnRemoveRequestChatMessages(type, channelID, chatDataList));
        }

        #endregion


        #region ChatUserProfileInfo Comparer Class (Because sort)
        public class GlobalChatProfileComp : IComparer<ChatUserProfileInfo>
        {
            public int Compare(ChatUserProfileInfo profileA, ChatUserProfileInfo profileB)
            {
                if (profileA.userId == profileB.userId) return 0;
                else if (profileA.userId == BlackboardQueryUtils.GetMyUserId()) return -1;
                else if (profileB.userId == BlackboardQueryUtils.GetMyUserId()) return 1;
                else
                {
                    return string.Compare(profileA.userId, profileB.userId, StringComparison.Ordinal);
                }
            }
        }

        public class ClubChatProfileComp : IComparer<ChatUserProfileInfo>
        {
            public int Compare(ChatUserProfileInfo profileA, ChatUserProfileInfo profileB)
            {
                if (profileA.isOnline != profileB.isOnline)
                {
                    if (profileA.isOnline) return -1;
                    else return 1;
                }
                else if (profileA.clubListIndex != profileB.clubListIndex)
                {
                    if (profileA.clubListIndex < profileB.clubListIndex) return -1;
                    if (profileA.clubListIndex > profileB.clubListIndex) return 1;
                }

                return 0;
            }
        }

        public class GameChatProfileComp : IComparer<ChatUserProfileInfo>
        {
            public int Compare(ChatUserProfileInfo profileA, ChatUserProfileInfo profileB)
            {
                if (profileA.isMe) return -1;
                else if (profileB.isMe) return 1;

                return 0;
            }
        }
        #endregion
        #region BI Event
        private void BI_client_system_chat(ChannelType channelType, List<ChatMessageData> addChatDataList)
        {
            if (addChatDataList.Count > 0)
            {
                for (int i = 0; i < addChatDataList.Count; ++i)
                {
                    bool isSystemChat = false;
                    string chatMessage = "";
                    string name = "";
                    switch (addChatDataList[i].chatPoll.type)
                    {
                        case ChatType.BOSS_RAIDERS_BOSS_KILL:
                            isSystemChat = true;
                            ChatDataBossRaidersBossKill bossKill = (ChatDataBossRaidersBossKill)addChatDataList[i].chatPoll.data;
                            chatMessage = StringTableUtils.GetString(StringTable.StringTableType.Global, "CHAT_BOSS_RAIDERS_KILL_TEXT", bossKill.round);
                            name = StringTableUtils.GetString(StringTable.StringTableType.Global, "CHAT_BOSS_RAIDERS_NOTICE_TITLE");
                            break;
                        case ChatType.BOSS_RAIDERS_RANKING_UP:
                            ChatDataBossRaidersRankingUp bossRankingUp = (ChatDataBossRaidersRankingUp)addChatDataList[i].chatPoll.data;
                            chatMessage = StringTableUtils.GetString(StringTable.StringTableType.Global, "CHAT_BOSS_RAIDERS_CLUB_RANKING_UP_TEXT", bossRankingUp.clubRank, bossRankingUp.clubRank);
                            name = StringTableUtils.GetString(StringTable.StringTableType.Global, "CHAT_BOSS_RAIDERS_NOTICE_TITLE");
                            isSystemChat = true;
                            break;
                        case ChatType.BOSS_RAIDERS_LEADING_ATTACKER:
                            ChatDataBossRaidersLeadingAttacker bossLeading = (ChatDataBossRaidersLeadingAttacker)addChatDataList[i].chatPoll.data;
                            chatMessage = StringTableUtils.GetString(StringTable.StringTableType.Global, "CHAT_META_GAME_LEADING_ATTACKER_TEXT",
                                bossLeading.clubMemberRank, bossLeading.clubMemberRank, StringTableUtils.GetString(StringTable.StringTableType.Global, "BOSS_RAIDERS_NAME"));
                            name = addChatDataList[i].chatPoll.profile.userId;
                            isSystemChat = true;
                            break;
                        case ChatType.CLUB_ARENA_CLUB_RANKING_UP:
                            ChatDataClubArenaClubRankingUp clubRankingUp = (ChatDataClubArenaClubRankingUp)addChatDataList[i].chatPoll.data;
                            chatMessage = StringTableUtils.GetString(StringTable.StringTableType.Global, "CHAT_CLUB_ARENA_CLUB_RANKING_UP_TEXT",
                                clubRankingUp.clubRank, clubRankingUp.clubRank);
                            name = StringTableUtils.GetString(StringTable.StringTableType.Global, "CHAT_CLUB_ARENA_NOTICE_TITLE");
                            isSystemChat = true;
                            break;
                        case ChatType.CLUB_ARENA_LEADING_CLUB_MEMBER:
                            ChatDataClubArenaLeadingClubMember clubLeading = (ChatDataClubArenaLeadingClubMember)addChatDataList[i].chatPoll.data;
                            chatMessage = StringTableUtils.GetString(StringTable.StringTableType.Global, "CHAT_META_GAME_LEADING_ATTACKER_TEXT",
                                clubLeading.clubMemberRank, clubLeading.clubMemberRank, StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_ARENA_NAME"));
                            name = addChatDataList[i].chatPoll.profile.userId;
                            isSystemChat = true;
                            break;
                    }
                    if (isSystemChat == true)
                    {
                        //Debug.Log("ChatMetaManager -> OnAddChatMessages -> BI_client_system_chat : " + addChatDataList[i].chatPoll.type);
                        string chatType = channelType.ToString().ToLower();
                        string language = null;
                        if (GetChannelType(addChatDataList[i].chatPoll.channelId) == ChannelType.Global)
                        {
                            language = globalChannelInfos[globalChannelIndex].channelName;
                        }
                        name = StringUtility.RemoveTrimAndNewline(StringUtility.UnFormatColorTags(name));
                        chatMessage = StringUtility.RemoveTrimAndNewline(StringUtility.UnFormatColorTags(chatMessage));

                        BiEventUtils.SendSystemChat(chatType, language, GetChannelID(channelType), addChatDataList[i], name, chatMessage);
                    }
                }
            }
        }
        #endregion
    }
}
