using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using BagelCode.ClientModels;
using SlotMaker;

namespace BagelCode {
    public class ChatSimulator : MonoBehaviour {
        //public string channelID;
        //public string chatMessage;

        //[ButtonGroup(group: "Connect Buttons", order: 1)]
        //[Button(ButtonSizes.Medium, Name = "Connect Channel")]
        //public void ConnectChannel() {
        //    Debug.Log(channelID);
        //    ChattingMessenger.Instance.ConnectChannel(channelID);
        //}

        //[ButtonGroup(group: "Connect Buttons", order: 2)]
        //[Button(ButtonSizes.Medium, Name = "Disconnect Channel")]
        //public void DisconnectChannel() {
        //    Debug.Log(channelID);
        //    ChattingMessenger.Instance.DisconnectChannel(channelID);
        //}

        //[ButtonGroup(group: "Subscription Buttons", order: 1)]
        //[Button(ButtonSizes.Medium, Name = "Subscription")]
        //public void SubscribeChannel() {
        //    Debug.Log(channelID);
        //    ChattingMessenger.Instance.Subscribe(channelID, this);
        //}

        //[ButtonGroup(group: "Subscription Buttons", order: 2)]
        //[Button(ButtonSizes.Medium, Name = "Unsubscription")]
        //public void UnsubscribeChannel() {
        //    ChattingMessenger.Instance.Unsubscribe(channelID, this);
        //}

        //[ButtonGroup(group: "Subscription Buttons", order: 3)]
        //[Button(ButtonSizes.Medium, Name = "Send Message")]
        //public void SendMessage() {
        //    var chatPoll = new ChatPoll();

        //    chatPoll.id = 0;
        //    chatPoll.channelId = channelID;
        //    chatPoll.message = chatMessage;
        //    chatPoll.userId = BlackboardUtils.FindVariable<string>(null, "/me/userId").value;

        //    ChattingMessenger.Instance.SendChatMessage(channelID, chatMessage);
        //}

        //[ButtonGroup(group: "Subscription Buttons", order: 4)]
        //[Button(ButtonSizes.Medium, Name = "Show Recent")]
        //public void RequestRecentMessage() {
        //    BagelCodeClientAPI.GetRecentChatMessages(channelID,
        //    (response) => {
        //        for (int i = 0; i < response.dataList.Count; ++i) {
        //            Debug.Log("Recent Messages : " + response.dataList[i].message);
        //        }
        //    },
        //    (error) => {
        //    });
        //}
        //[PropertyOrder(5)]
        //public string otherUserId;
        //[PropertyOrder(6)]
        //public string otherMessage;
        //[ButtonGroup(order: 7)]
        //[Button(ButtonSizes.Medium, Name = "Send Other Message")]
        //public void SendOtherMessage() {
        //    var chatPoll = new ChatPoll();

        //    chatPoll.id = 0;
        //    chatPoll.channelId = channelID;
        //    chatPoll.message = otherMessage;
        //    chatPoll.userId = otherUserId;

        //    ChattingMessenger.Instance.SendChatMessage(channelID, otherMessage);
        //}



        //public void OnAddChatMessages(List<ChatPoll> chatDataList) {
        //    for (int i = 0; i < chatDataList.Count; ++i) {
        //        Debug.Log("Add Chat Messages : " + chatDataList[i].message);
        //    }
        //}

        //public void OnMessageSend(bool isSuccess) {
        //    Debug.Log(isSuccess);
        //}

        //public void OnEnterChannel(List<ChatPoll> chatDataList) {
        //    for (int i = 0; i < chatDataList.Count; ++i) {
        //        Debug.Log("Init Messages : " + chatDataList[i].message);
        //    }
        //}

        //public void OnDisconnectChannel() {
        //    Debug.Log("Disconnect");
        //}

        public void OnUpdateSubsribers(List<UserProfile> subscriberProfiles) {
            
        }
    }

}
