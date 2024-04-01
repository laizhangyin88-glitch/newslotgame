using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode.ClientModels;
using Sirenix.OdinInspector;
using System.Linq;

namespace BagelCode.Chat
{
    /*
     * Connect Flow
     * 1.TryConnect()
     * 2.CoTryConnect() ->(Fail: Repeat retry while succeed)
     * 3.RequestRecent() ->(Fail: Repeat retry while succeed)
     * 4.Status=ChannelStatus.Connect
     */
    [System.Serializable]
    public class ChatMessengerChannel
    {
        public ChatMessengerChannel(string newChannelID)
        {
            channelID = newChannelID;
            status = ChannelStatus.DISCONNECTED;
        }

        [ShowInInspector]
        public ChannelStatus status { get; private set; } = ChannelStatus.DISCONNECTED;
        [ShowInInspector]
        public string channelID { get; private set; }
        [ShowInInspector]
        internal Dictionary<long, ChatMessageData> chatDataDict = new Dictionary<long, ChatMessageData>();
        [ShowInInspector]
        private List<IChatMessengerSubscriber> subscribers = new List<IChatMessengerSubscriber>();//Subscribe GameObjects
        
        private Coroutine tryConnectEnumrator = null;
        private Coroutine tryDisConnectEnumrator = null;
        private Coroutine tryGetRecentDisConnectEnumrator = null;

        private float reconnectDelay = 2f;
        private bool isRequestRecent;

        public void StopAllManualCoroutines()
        {
            if(tryConnectEnumrator != null)
                ChatMessenger.Instance.StopCoroutine(tryConnectEnumrator);
            if(tryDisConnectEnumrator != null)
                ChatMessenger.Instance.StopCoroutine(tryDisConnectEnumrator);
            if(tryGetRecentDisConnectEnumrator != null)
                ChatMessenger.Instance.StopCoroutine(tryGetRecentDisConnectEnumrator);
        }

        public void TryConnect(System.Action successAct=null)
        {
            if (status != ChannelStatus.DISCONNECTED) return;
            StopAllManualCoroutines();
            tryConnectEnumrator = ChatMessenger.Instance.StartCoroutine(CoTryConnect(0f, successAct));
        }

        public void AddSubscriber(IChatMessengerSubscriber subscriber)
        {
            if (!subscribers.Exists(x => x == subscriber))
            {
                subscribers.Add(subscriber);
            }

            if (status == ChannelStatus.CONNECTED)
            {
                subscriber.OnConnectedChannel(channelID);
            }
        }

        public void RemoveSubscriber(IChatMessengerSubscriber subscriber)
        {
            if (!subscribers.Contains(subscriber)) return;
            subscribers.Remove(subscriber);
        }

        public void ClearSubscribers()
        {
            subscribers.Clear();
        }

        public IEnumerable<IChatMessengerSubscriber> GetSubscribers()
        {
            return subscribers;
        }

        public void TryDisconnect(System.Action successAct = null)
        {
            StopAllManualCoroutines();
            tryDisConnectEnumrator = ChatMessenger.Instance.StartCoroutine(CoTryDisconnect(0f, successAct));
        }

        // public IEnumerable<ChatMessageData> GetChatDataList()
        // {
        //     // return chatDataDict.Values.Where(x => ChatMessenger.Instance.strategy.IsVisible(x)).OrderBy(x => x, ChatMessenger.Instance.strategy.GetChatComparer());
        //     // return chatDataDict.Values.OrderBy(x => x, ChatMessenger.Instance.strategy.GetChatComparer());
        //     return chatDataDict.Values;
        // }

        public List<ChatMessageData> GetChatDataList()
        {
            // return chatDataDict.Values.Where(x => ChatMessenger.Instance.strategy.IsVisible(x)).OrderBy(x => x, ChatMessenger.Instance.strategy.GetChatComparer()).ToList();
            return chatDataDict.Values.OrderBy(x => x, ChatMessenger.Instance.strategy.GetChatComparer()).ToList();
            // return chatDataDict.Values.ToList();
        }

        public ChatMessageData GetChatData(long chatDataID)
        {
            if (!chatDataDict.ContainsKey(chatDataID)) return null;
            return chatDataDict[chatDataID];
        }

        public long GetLastReceivedChatPollID()
        {
            if (chatDataDict.Values.Count() == 0) return -1;
            return chatDataDict.Values.Max(x => x.chatPoll.id);
        }

        public long GetGlobalTopbarChatID()
        {
            if (chatDataDict.Values.Count() == 0) return -1;
            var topbarValues = chatDataDict.Values.Where(x => x.chatPoll.IsGlobalTopbarType());
            if(topbarValues == null || topbarValues.Count() == 0) return -1;
            return topbarValues.Max(x => x.chatPoll.id);
        }

        public long GetFirstReceivedChatPollID()
        {
            if (chatDataDict.Values.Count() == 0) return -1;
            return chatDataDict.Values.Min(x => x.chatPoll.id);
        }

        private List<ChatMessageData> AddChatPolls(List<ChatPoll> pollList)
        {
            List<ChatMessageData> dataList = new List<ChatMessageData>();
            foreach(ChatPoll poll in pollList)
            {
                if(!chatDataDict.ContainsKey(poll.id) && ChatMessenger.Instance.strategy.IsVisible(poll))
                {
                    var data = MakeChatData(poll);
                    data.SetSuccessData();
                    dataList.Add(data);
                }
            }

            return dataList;
        }

        public void OnAddChatPolls(List<ChatPoll> pollList)
        {
            // Debug.LogError("OnAddChatPolls");
            List<ChatMessageData> dataList = AddChatPolls(pollList);

            if(dataList.Count > 0)
            {
                foreach (var subscriber in subscribers)
                {
                    subscriber.OnAddChatMessages(channelID, dataList);
                }
            }
        }

        public void OnAddRecentPolls(List<ChatPoll> pollList)
        {
            List<ChatMessageData> dataList = AddChatPolls(pollList);

            if(dataList.Count > 0)
            {
                foreach (var subscriber in subscribers)
                {
                    subscriber.OnAddRecentChatMessages(channelID, dataList);
                }
            }
        }

        public void OnRemoveChatPoll(long removeID)
        {
            if(chatDataDict.ContainsKey(removeID))
            {
                // Debug.LogError( string.Format("OnRemove : {0}", removeID) );
                chatDataDict.Remove(removeID);
            }
        }

        public void OnRemoveChatPolls(int count)
        {
            List<ChatMessageData> list = GetChatDataList();
            for(int i=0; i < list.Count; ++i)
            {
                if(i < count)
                {
                    OnRemoveChatPoll(list[i].chatPoll.id);
                }
            }
        }

        public void OnRemoveRequestChatPolls(List<ChatPoll> pollList)
        {
            List<ChatMessageData> dataList = AddChatPolls(pollList);

            if(dataList.Count > 0)
            {
                foreach (var subscriber in subscribers)
                {
                    subscriber.OnRemoveRequestChatMessages(channelID, dataList);
                }
            }
        }

        // Resend
        // public void OnRemapChatData(ChatMessageData data)
        // {
        //     chatDataDict.Remove(data.requestID);
        //     if(!chatDataDict.ContainsKey(data.successID))
        //         chatDataDict.Add(data.successID, data);
        // }

        // internal void OnAddChatMessages(List<ChatMessageData> data)
        // {
            // Debug.LogError("OnAddChatMessages");
            // foreach (var it in data)
            // {
            //     chatDataDict[it.chatPoll.id] = it;
            // }

            // foreach (var subscriber in subscribers)
            // {
            //     subscriber.OnAddChatMessages(channelID, data);
            // }
        // }

        public ChatMessageData MakeChatData(ChatPoll chatPoll)
        {
            if(chatDataDict.ContainsKey(chatPoll.id)) return null;

            var chatData = new ChatMessageData();
            chatData.InitData(chatPoll);
            chatData.ownerChannel = this;

            chatDataDict.Add(chatPoll.id, chatData);

            return chatData;
        }

        // public void RemoveChatData(long id)
        // {
        //     chatDataDict.Remove(id);
        // }

        // public void UpdateChatPoll(ChatPoll chatPoll)
        // {
        //     if(chatDataDict.ContainsKey(chatPoll.id))
        //     {
        //         chatDataDict[chatPoll.id].chatPoll = chatPoll;
        //     }
        // }

        public bool IsUserFirstChatPoll(ChatMessageData chatData)
        {
            var chatDataList = GetChatDataList().ToList();

            if (chatData.chatPoll.id < 0)
            {
                if (chatDataList.Count == 0) return true;
                return chatDataList[chatDataList.Count-1].chatPoll.userId != chatData.chatPoll.userId;
            }

            var index = chatDataList.FindIndex(x => x.chatPoll.id == chatData.chatPoll.id);

            if (index <= 0) return true;
            return (chatDataList[index - 1].chatPoll.userId != chatData.chatPoll.userId);
        }

        private IEnumerator CoTryConnect(float delay, System.Action successAct)
        {
#if NEW_NET0
            Debug.Log("【remove rpc】: /v0/chat/subscribe");
            yield return null;
#else
            status = ChannelStatus.ONGOING;
            yield return new WaitForSeconds(delay);
            BagelCodeClientAPI.RequestSubscribeChat(channelID,
                (response) =>
                {
                    RequestRecent(GetFirstReceivedChatPollID(),(channelID,chatData) =>
                        {
                            foreach (var subscriber in subscribers)
                            {
                                subscriber.OnConnectedChannel(channelID);
                            }
                            successAct?.Invoke();
                        }
                    );
                },
                (error) =>
                {
                    //Reconnect
                    tryConnectEnumrator = ChatMessenger.Instance.StartCoroutine(CoTryConnect(reconnectDelay, successAct));
                }
            );

#endif
        }

        public void RequestRecent(long receiveID,System.Action<string,List<ChatPoll>> successAct)
        {

#if NEW_NET
            Debug.Log("【remove rpc】：/v0/chat/recent");
            return;
#endif
            if (isRequestRecent) return;
            StopAllManualCoroutines();
            tryGetRecentDisConnectEnumrator = ChatMessenger.Instance.StartCoroutine(CoRequestRecent(receiveID, 0f, successAct));
        }

        private IEnumerator CoRequestRecent(long receiveID, float delay, System.Action<string, List<ChatPoll>> successAct)
        {
            isRequestRecent = true;
            status = ChannelStatus.ONGOING;

            yield return new WaitForSeconds(delay);

            BagelCodeClientAPI.GetRecentChatMessages(channelID, receiveID == -1 ? 0 : receiveID,
            (response) =>
            {

                string oldJson = JsonUtility.ToJson(response);
                Debug.Log($"@A ChatPollResponse = {oldJson}");

                //Success
                isRequestRecent = false;
                status = ChannelStatus.CONNECTED;

                OnAddRecentPolls(response.dataList);
                
                successAct?.Invoke(channelID, response.dataList);
            },
            (error) =>
            {
                //Retry Recent Messages
                tryGetRecentDisConnectEnumrator = ChatMessenger.Instance.StartCoroutine(CoRequestRecent(receiveID, reconnectDelay, successAct));
            });
        }

        private IEnumerator CoTryDisconnect(float delay, System.Action successAct)
        {
            status = ChannelStatus.ONGOING;

            yield return new WaitForSeconds(delay);

            BagelCodeClientAPI.RequestUnsubscribeChat(channelID,
            (response) =>
            {
                status = ChannelStatus.DISCONNECTED;
                foreach(var subscriber in subscribers)
                {
                    subscriber.OnDisconnectedChannel(channelID);
                }

                successAct?.Invoke();
            },
            (error) =>
            {
                //Retry
                tryDisConnectEnumrator = ChatMessenger.Instance.StartCoroutine(CoTryDisconnect(reconnectDelay, successAct));
            });
        }
    }
}
