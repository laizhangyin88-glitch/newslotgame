using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using SlotMaker;
using BagelCode.ClientModels;
using Sirenix.OdinInspector;
using System;
using System.Linq;

namespace BagelCode.Chat
{
    public enum ChannelStatus
    {
        DISCONNECTED = 0,
        ONGOING,
        CONNECTED
    };

    public class ChatMessenger : MonoWeakSingleton<ChatMessenger>
    {
        [Button]
        private void BlockUser(string userId)
        {
            var eventData = new EventData<string>("OnBlock", userId);
            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, eventData);
        }

        [ReadOnly]
        [ShowInInspector]
        internal List<ChatMessengerChannel> channelList = new List<ChatMessengerChannel>();

        //Requesting Message
        private static long CHATPOLL_UNIQUE_ID = 0;
        [ReadOnly]
        [ShowInInspector]
        private long lastReceivedID = -1;


        /// <summary>
        /// All Request ChatPoll Table
        /// Key: RequestId , Value: RequestInfo
        /// </summary>
        // [ReadOnly]
        // [ShowInInspector]
        // private Dictionary<long, RequestChatPoll> requestTable = new Dictionary<long, RequestChatPoll>();
        // /// <summary>
        // /// Request ChatPoll Table (is succeed).
        // /// Key: ChatPollId , Value: RequestInfo
        // /// </summary>
        // [ReadOnly]
        // [ShowInInspector]
        // private Dictionary<long, RequestChatPoll> successRequestTable= new Dictionary<long, RequestChatPoll>();


        // [ReadOnly]
        // [ShowInInspector]
        // private Dictionary<long, string> requestContextIdTable = new Dictionary<long, string>();

        /// <summary>
        /// Report Button Activable Table
        /// First(long): chatPollId, Second(bool): isReported
        /// </summary>
        [ReadOnly]
        [ShowInInspector]
        private Dictionary<long, bool> reportableTable = new Dictionary<long, bool>();
        /// <summary>
        /// First(long): chatPollId, Second(bool): isReported
        /// </summary>
        [ReadOnly]
        [ShowInInspector]
        private Dictionary<long, bool> reportTable = new Dictionary<long, bool>();


        [ReadOnly]
        [ShowInInspector]
        private List<string> muteUsers = new List<string>();

        public IChatStrategy strategy { get; private set; } = new ChatStrategyMeta();

        //System Delegate
        private Dictionary<string, MessageDispatcher.EventDelegate> delegates = new Dictionary<string, MessageDispatcher.EventDelegate>();
        private const string ON_SYSTEM_EVENT = "OnSystemEvent";
        private const string ON_SYSTEM_RESET_EVENT = "SystemReset";
        private const string ON_FINISHED_LOGIN_EVENT = "FinishedLogin";


        public bool IsConnected(string channelID)
        {
            var chatChannel = channelList.Find(x => x.channelID == channelID);
            if (chatChannel == null) return false;
            return chatChannel.status == ChannelStatus.CONNECTED;
        }

        // public IEnumerable<ChatMessageData> GetChatDataList(string channelID)
        // {
        //     var chatChannel = channelList.Find(x => x.channelID == channelID);
        //     if (chatChannel == null) return Enumerable.Empty<ChatMessageData>();
        //     return chatChannel.GetChatDataList();
        // }

        public ChatMessengerChannel GetChatMessengerChannel(string channelID)
        {
            return channelList.Find(x => x.channelID == channelID);
        }

        public List<ChatMessageData> GetChatDataList(string channelID)
        {
            var chatChannel = channelList.Find(x => x.channelID == channelID);
            if (chatChannel == null) return new List<ChatMessageData>();

            var ignoreList = BlackboardQueryUtils.GetIgnoreUserIdAll();
            var chatDataList = chatChannel.GetChatDataList();
            chatDataList.RemoveAll(c => ignoreList.Contains(c.chatPoll.userId));

            return chatDataList;
        }

        public int GetChatDataIndex(string channelID, long findID)
        {
            var list = GetChatDataList(channelID);
            return list.FindIndex(c => c.chatPoll.id == findID);
        }

        public int GetChatRequestDataIndex(string channelID, long findID)
        {
            var list = GetChatDataList(channelID);

            // for(int i=0; i < list.Count; ++i)
            // {
            //     Debug.LogError( string.Format("{4}, id = {0}, reqid = {1}, response {2}, failed {3}", list[i].chatPoll.id, list[i].chatPoll.requestId, list[i].isResponse, list[i].isSuccess, i) );
            // }

            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].chatPoll.id == findID && list[i].chatPoll.requestId == findID)
                {
                    return i;
                }
            }

            return -1;
        }

        public int GetChatPollDataIndex(string channelID, long findID)
        {
            var list = GetChatDataList(channelID);

            // for(int i=0; i < list.Count; ++i)
            // {
            //     Debug.LogError( string.Format("{4}, id = {0}, reqid = {1}, response {2}, failed {3}", list[i].chatPoll.id, list[i].chatPoll.requestId, list[i].isResponse, list[i].isSuccess, i) );
            // }

            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].chatPoll.id == findID)
                {
                    return i;
                }
            }

            return -1;
        }

        public void RemoveChatPoll(string channelID, long removeID)
        {
            var chatChannel = channelList.Find(x => x.channelID == channelID);
            if (chatChannel != null)
                chatChannel.OnRemoveChatPoll(removeID);
        }

        public void RemoveChatPolls(string channelID, int count)
        {
            // remove first index to count Index
            var chatChannel = channelList.Find(x => x.channelID == channelID);
            if (chatChannel != null)
                chatChannel.OnRemoveChatPolls(count);
        }

        public void UpdateChatPollUniqueID(long id)
        {
            if (CHATPOLL_UNIQUE_ID < id)
                CHATPOLL_UNIQUE_ID = id;
        }

        public void GetUserProfilesAsync(string channelID, System.Action<ChatSubscriberListResponse> OnSuccessAct, HTTPErrorCallback OnErrorAct)
        {
            BagelCodeClientAPI.GetChatSubsribers(channelID, OnSuccessAct, OnErrorAct);
        }

        public void RequestMuteAsync(bool onOff, string userId, System.Action<ChatMuteResponse> OnSuccessAct, HTTPErrorCallback OnErrorAct)
        {
            BagelCodeClientAPI.RequestChatMute(onOff, userId,
                (response) =>
                {
                    if (ApplicationSettings.LogTest())
                    {
                        string onOffStr = onOff ? "On" : "Off";
                        Debug.Log($"Mute {onOffStr} Success");
                    }
                    muteUsers = response.muteUserIdList;
                    OnSuccessAct?.Invoke(response);
                },
                (error) =>
                {
                    if (ApplicationSettings.LogTest())
                        Debug.Log($"Mute Fail [Error Code : {error.errorCode}] \nErrorInfo: {error.errorDetailInfo}");
                }
            );
        }

        public void RequestReportAsync(ChatPoll chatPoll, System.Action<ChatReportResponse> OnSuccessAct, HTTPErrorCallback OnErrorAct)
        {
            BagelCodeClientAPI.ReportChatMessage(chatPoll,
                (response) =>
                {
                    if (ApplicationSettings.LogTest())
                        Debug.Log("Report Success");
                    reportTable[chatPoll.id] = true;
                    muteUsers = response.muteUserIdList;
                    OnSuccessAct?.Invoke(response);
                },
                (error) =>
                {
                    if (ApplicationSettings.LogTest())
                        Debug.Log($"Report Fail [Error Code : {error.errorCode}] \nErrorInfo: {error.errorDetailInfo}");
                }
            );
        }

        public void SetReportable(long chatPollID, bool onOff)
        {
            reportableTable[chatPollID] = onOff;
        }

        // public RequestChatPoll GetRequestInfo(long requestId)
        // {
        //     if (!requestTable.ContainsKey(requestId)) return null;
        //     return requestTable[requestId];
        // }

        // public void RemoveRequestInfo(long requestID)
        // {
        //     if(requestTable.ContainsKey(requestID))
        //         requestTable.Remove(requestID);
        // }

        // public RequestChatPoll GetRequestInfoWithChatPollId(long chatPollId)
        // {
        //     if (!successRequestTable.ContainsKey(chatPollId)) return null;
        //     return successRequestTable[chatPollId];
        // }

        // public void RemoveSuccessRequestInfo(long chatPollId)
        // {
        //     if(successRequestTable.ContainsKey(chatPollId))
        //         successRequestTable.Remove(chatPollId);
        // }

        // public void UpdateChatPoll(string channelID, ChatPoll updateChatPoll)
        // {
        //     var chatChannel = channelList.Find(x => x.channelID == channelID);
        //     if (chatChannel == null) return;
        //     chatChannel.UpdateChatPoll(updateChatPoll);
        // }

        // public void RemoveChatPoll(string channelID,long chatPollID)
        // {
        //     var chatChannel = channelList.Find(x => x.channelID == channelID);
        //     if (chatChannel == null) return;
        //     chatChannel.RemoveChatData(chatPollID);
        // }

        public void RequestRecentAsync(string channelID, long _lastReceiveID, System.Action<string, List<ChatPoll>> OnSuccessAct)
        {
            var chatChannel = channelList.Find(x => x.channelID == channelID);
            if (chatChannel == null) return;
            chatChannel.RequestRecent(_lastReceiveID, OnSuccessAct);
        }

        public long GetLastReceivedChatPollID(string channelID)
        {
            var chatChannel = channelList.Find(x => x.channelID == channelID);
            if (chatChannel == null) return -1;
            return chatChannel.GetLastReceivedChatPollID();
        }

        public long GetGlobalTopbarChatID(string channelID)
        {
            var chatChannel = channelList.Find(x => x.channelID == channelID);
            if (chatChannel == null) return -1;
            return chatChannel.GetGlobalTopbarChatID();
        }

        public bool IsUserFirstChatPoll(ChatMessageData chatData)
        {
            var chatChannel = channelList.Find(x => x.channelID == chatData.chatPoll.channelId);
            if (chatChannel == null) return false;
            return chatChannel.IsUserFirstChatPoll(chatData);
        }

        public ChatMessageData FindChatData(long chatPollID)
        {
            foreach (var channel in channelList)
            {
                var chatData = channel.GetChatData(chatPollID);
                if (chatData != null)
                    return chatData;
            }
            return null;
        }

        public bool IsMute(string userId)
        {
            return muteUsers.Contains(userId);
        }

        public bool IsReported(long chatPollId)
        {
            if (!reportTable.ContainsKey(chatPollId)) return false;
            return reportTable[chatPollId];
        }

        public bool IsReportable(long chatPollId)
        {
            if (!reportableTable.ContainsKey(chatPollId)) return false;
            return reportableTable[chatPollId];
        }

        public void ConnectChannel(string channelID)
        {

            var chatChannel = channelList.Find(x => x.channelID == channelID);
            if (chatChannel == null)
            {
                chatChannel = new ChatMessengerChannel(channelID);
                channelList.Add(chatChannel);
            }
            chatChannel.TryConnect();
        }

        public void DisconnectChannel(string channelID)
        {
            var chatChannel = channelList.Find(x => x.channelID == channelID);
            if (chatChannel == null) return;

            // Debug.LogError(string.Format("Try DisConnect {0}", channelID));

            chatChannel.TryDisconnect(() =>
            {
                // Debug.LogError(string.Format("DisConnect {0}", channelID));
                channelList.Remove(chatChannel);
            });
        }


        public void Subscribe(string channelID, IChatMessengerSubscriber subscriber)
        {
#if NEW_NET0
            Debug.Log("【remove rpc】: /v0/chat/subscribe");
            return;
#endif
            ConnectChannel(channelID);
            var chatChannel = channelList.Find(x => x.channelID == channelID);
            chatChannel.AddSubscriber(subscriber);
        }

        public void Unsubscribe(string channelID, IChatMessengerSubscriber subscriber)
        {
            if (!channelList.Exists(x => x.channelID == channelID)) return;
            var chatChannel = channelList.Find(x => x.channelID == channelID);
            chatChannel.RemoveSubscriber(subscriber);
        }

        public void UnsubscribeAll(IChatMessengerSubscriber subscriber)
        {
            channelList.ForEach(chatChannel =>
            {
                chatChannel.RemoveSubscriber(subscriber);
            });
        }

        public void MakeWelcomChatMessageData(string channelID)
        {
            var channel = channelList.Find(x => x.channelID == channelID);
            if (channel == null) return;

            long requestID = ++CHATPOLL_UNIQUE_ID;

            var systemChatPoll = new ChatPoll();
            systemChatPoll.userId = "";
            systemChatPoll.profile = null;
            systemChatPoll.id = requestID;
            systemChatPoll.requestId = requestID;

            ChatMessageData data = channel.MakeChatData(systemChatPoll);
            data.isResponse = true;
            data.isSuccess = true;
            data.isSystem = true;
        }

        public ChatMessageData MakeChatMessageData(string channelID)
        {
            var channel = channelList.Find(x => x.channelID == channelID);
            if (channel == null) return null;

            long requestID = ++CHATPOLL_UNIQUE_ID;

            var chatPoll = new ChatPoll();
            chatPoll.channelId = channelID;
            chatPoll.userId = BlackboardUtils.FindVariable<string>("/me/userId").value;
            chatPoll.profile = BlackboardQueryUtils.GetMyProfile();
            chatPoll.id = requestID;
            chatPoll.requestId = requestID;

            return channel.MakeChatData(chatPoll);
        }

        public ChatMessageData ResendChatMessageData(ChatMessageData resendData)
        {
            var channel = channelList.Find(x => x.channelID == resendData.chatPoll.channelId);
            if (channel == null) return null;

            long requestID = ++CHATPOLL_UNIQUE_ID;

            // resendData.chatPoll.channelId = channelID;
            // resendData.chatPoll.userId = BlackboardUtils.FindVariable<string>("/me/userId").value;
            // resendData.chatPoll.profile = BlackboardQueryUtils.GetMyProfile();
            resendData.chatPoll.id = requestID;
            resendData.chatPoll.requestId = requestID;

            resendData.Post();

            return channel.MakeChatData(resendData.chatPoll);
        }

        public void SendChatMessage(string channelID, ChatMessageData chatData, System.Action<bool> OnEndAct = null)
        {
            CheckValid(chatData.chatPoll);

            BagelCodeClientAPI.PostChatMessageForChatServer(channelID, chatData.chatPoll.GetChatType(), chatData.chatPoll.requestId, chatData.chatPoll.data, chatData.contextID,
                (response) =>
                {
                    // chatData.SendSuccess();

                    if (OnEndAct != null) OnEndAct.Invoke(true);
                },
                (error) =>
                {
                    if (ApplicationSettings.LogTest())
                        Debug.Log($"SendChatMessage Fail\ncode:{error.errorCode} , Info:{error.errorDetailInfo}");

                    // chatData.SendFailed();

                    // chat banned
                    if (error.errorCode == Error.BANNED_USER_ERROR &&
                        error.errorDetailInfo is ErrorDetailInfoBannedUser errorDetailInfo)
                    {
                        OpenChatBannedPopup(errorDetailInfo);
                    }

                    if (OnEndAct != null) OnEndAct.Invoke(false);
                }
            );
        }

        public void SendChatMessageInGlobal(string channelId, ChatMessageData chatData, System.Action<bool> OnEndAct = null)
        {
            CheckValid(chatData.chatPoll);

            BagelCodeClientAPI.PostChatMessageForLogicServer(channelId, chatData.chatPoll.GetChatType(), chatData.chatPoll.requestId, chatData.chatPoll.data, chatData.contextID,
                (response) =>
                {
                    BlackboardQueryUtils.UpdateGlobalChatSpeaker(response.speaker, response.serverTime);
                    // BlackboardUtils.FindVariable<int>(null, "/me/speaker").value -= response.speaker;
                    // chatData.SendSuccess();

                    if (OnEndAct != null) OnEndAct.Invoke(true);
                },
                (error) =>
                {
                    if (ApplicationSettings.LogTest())
                    {
                        Debug.Log($"SendChatMessage Fail\ncode:{error.errorCode} , Info:{error.errorDetailInfo}");
                        Debug.Log($"channelId:{channelId} , Chat Type:{chatData.chatPoll.GetChatType()} , " +
                            $"Request Id:{chatData.chatPoll.requestId} , Chat Poll:{chatData.chatPoll.data} , " +
                            $"contextId:{chatData.contextID}");
                    }

                    // chatData.SendFailed();

                    // chat banned
                    if (error.errorCode == Error.BANNED_USER_ERROR &&
                        error.errorDetailInfo is ErrorDetailInfoBannedUser errorDetailInfo)
                    {
                        OpenChatBannedPopup(errorDetailInfo);
                    }

                    if (OnEndAct != null) OnEndAct.Invoke(false);
                }
            );
        }

        private void OpenChatBannedPopup(ErrorDetailInfoBannedUser errorDetailInfo)
        {
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;
            GameObject okPopupObj = null;

            StartCoroutine(MetaPopupUtils.OpenOKPopupCoroutine(parent,
                (GameObject obj) =>
                {
                    okPopupObj = obj;

                    if (okPopupObj != null)
                    {
                        bool isPermanent = errorDetailInfo.isPermanent;
                        long endTimestamp = errorDetailInfo.banEndTimestamp;

                        if (isPermanent)
                        {
                            AEUtils.SendAE("client_banned_user_action",
                                ("ban_type", "chat"),
                                ("expire_timestamp", null));
                        }
                        else
                        {
                            AEUtils.SendAE("client_banned_user_action",
                                ("ban_type", "chat"),
                                ("expire_timestamp", endTimestamp));
                        }

                        string contentText = isPermanent ?
                            StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_BANNED_CHAT_PERMANENT") :
                            StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_BANNED_CHAT", endTimestamp);

                        string buttonText = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_CUSTOMER_SUPPORT");

                        MetaPopupUtils.SetCommonPopupData(okPopupObj, transform, contentText, "",
                            "OnCustomerSupport", buttonText, "", "", "OnClose", "OnClose", true, false, true, true, true);

                        MetaPopupUtils.OpenPopup(okPopupObj);
                    }
                }));
        }

        public void OpenCustomerSupport()
        {
            string supportUrl = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_SUPPORT_PAGE_URL");
            Application.OpenURL(supportUrl);
        }

        private void Awake()
        {
            lastReceivedID = -1;
            delegates[ON_SYSTEM_RESET_EVENT] = OnSystemReset;
            delegates[ON_FINISHED_LOGIN_EVENT] = OnFinishedLogin;
        }

        private void OnEnable()
        {
            MessageDispatcher.Register(ON_SYSTEM_EVENT, OnSystemEvent);
        }

        private void OnDisable()
        {
            MessageDispatcher.UnRegister(ON_SYSTEM_EVENT, OnSystemEvent);
        }

        private void RequestChatPollRecursive()
        {
#if NEW_NET
            return;
#endif
            var isAlive = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "sessionAlive");
            if (isAlive.value)
            {
                BagelCodeClientAPI.ChatPollRequest(lastReceivedID,
                (response) =>
                {
                    var sessionAlive = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "sessionAlive");
                    if(sessionAlive.value)
                    {
                        UpdateChatPollUniqueID(response.serverTime);

                        var meID = BlackboardQueryUtils.GetMyUserId();
                        foreach (var chatChannel in channelList)
                        {
                            var channelID = chatChannel.channelID;

                            List<ChatPoll> addChatPolls = new List<ChatPoll>();
                            // List<ChatPoll> removeChatPolls = new List<ChatPoll>();
                            for(int i=0; i < response.dataList.Count; ++i)
                            {
                                if(response.dataList[i].channelId == channelID)
                                {
                                    UpdateChatPollUniqueID(response.dataList[i].id);
                                    addChatPolls.Add(response.dataList[i]);
                                }
                            }

                            // if (removeChatPolls.Count > 0)
                            //     chatChannel.OnRemoveRequestChatPolls(removeChatPolls);

                            if (addChatPolls.Count > 0)
                                chatChannel.OnAddChatPolls(addChatPolls);
                        }
                        if (response.dataList.Count > 0)
                        {
                            lastReceivedID = response.dataList[response.dataList.Count - 1].id;
                        }
                    }

                    Invoke("RequestChatPollRecursive", 0f);
                },
                (error) =>
                {
                    Invoke("RequestChatPollRecursive", 5f);
                });
            }
        }

        private void ResetMessenger()
        {
            lastReceivedID = -1;
            foreach (var it in channelList)
            {
                if (it != null)
                {
                    it.TryDisconnect();
                }
            }
            channelList.Clear();
        }

        private void CheckValid(ChatPoll chatPoll)
        {
            if (chatPoll == null) throw new Exception("ChatPoll is Null!");
            if (chatPoll.data==null) throw new Exception("ChatPoll Data is Null");
            // if (chatPoll.id>=0) throw new Exception("ChatPoll Id is Negative");
            // if (chatPoll.id>= 0) throw new Exception("ChatPoll Id is Negative");
        }

        /*
         * System Event Callback
         */
        private void OnSystemEvent(EventData eventData)
        {
            MessageDispatcher.EventDelegate del;
            if (delegates.TryGetValue(eventData.name, out del))
                del.Invoke(eventData);
        }

        private void OnSystemReset(EventData eventData)
        {
            ResetMessenger();
        }

        private void OnFinishedLogin(EventData eventData)
        {
            muteUsers = BlackboardUtils.FindValue<List<string>>(MainBlackboard.Get(), "muteUserIdList");
            RequestChatPollRecursive();
        }
    }
}
