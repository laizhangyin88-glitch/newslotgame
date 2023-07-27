using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using ParadoxNotion;
using ParadoxNotion.Services;
using NodeCanvas.Framework;
using SlotMaker;
using SlotMaker.Json;
using BagelCode.ClientModels;
using Sirenix.OdinInspector;
using BSS.Utils;
using Action = System.Action;

namespace BagelCode.Chat
{
    /// <summary>
    /// Chatting Logic Controller (Attached Chatting GameObject)
    ///
    /// </summary>
    public class ChattingController : EventMonoBehaviour, IChatMetaListener
    {
        public static ChannelType LastChannelType
        {
            get
            {
                if (!ChatMetaManager.Instance.IsValidate(_lastChannelType))
                {
                    return ChannelType.Global;
                }
                return _lastChannelType;
            }

            set
            {
                _lastChannelType = value;
            }
        }

        private static ChannelType _lastChannelType;

        public ContextElement rootElement;

        [ReadOnly]
        [ShowInInspector]
        public ChannelType CurrentChannelType { get; private set; } = ChannelType.None;
        public string CurrentChannelID => ChatMetaManager.Instance.GetChannelID(CurrentChannelType);

        [ShowInInspector]
        [ReadOnly]
        public List<ChatMessageData> ChatDataList { get; private set; } = new List<ChatMessageData>();
        [ShowInInspector]
        [ReadOnly]
        public List<ChatUserProfileInfo> UserProfiles { get; private set; } = new List<ChatUserProfileInfo>();

        [ReadOnly]
        [ShowInInspector]
        public Dictionary<ChannelType, ChannelChatBase> channelChats = new Dictionary<ChannelType, ChannelChatBase>();


        [ReadOnly]
        [ShowInInspector]
        public ChannelChatBase CurrentChannelChat => channelChats.ContainsKey(CurrentChannelType) ? channelChats[CurrentChannelType] : null;
        public ChattingArea chattingArea;
        public ChattingProfileArea profileArea;

        public int maxDataCount = 200;
        public int refreshDataCount = 100;

        public bool isInit = false;
        public bool IsOpen { get; private set; }
        public bool IsTouchKeyboarding { get; private set; }
        public event Action<ChannelType> OnChannelChanged;

        private const StringTable.StringTableType GLOBAL = StringTable.StringTableType.Global;
        private const string ON_META_UI_EVENT = "OnMetaUIEvent";

        //Collecting Game
        private GameObject collectData;

        public IEnumerator OpenAsync(ChannelType _channelType)
        {
            MetaSystem.SubscribeBackButton(this.GetHashCode(), () => { this.Close(); });
            MessageDispatcher.Dispatch(ON_META_UI_EVENT, new EventData<bool>("OnChatToggle", true));
            var animator = GetComponent<Animator>();
            animator.SetBool("IsOpen", true);
            IsOpen = true;
            ChatMetaManager.Instance.ConfirmReadMessage(_channelType);

            // yield return new WaitForSeconds(0.2f);
            yield return new WaitUntil(() => isInit);

            yield return StartCoroutine(ChangeChannelTypeAsync(_channelType));
        }

        public void Close()
        {
            MetaSystem.UnSubscribeBackButton(this.GetHashCode());
            MessageDispatcher.Dispatch(ON_META_UI_EVENT, new EventData<bool>("OnChatToggle", false));
            IsOpen = false;
            GetComponent<Animator>().SetBool("IsOpen", false);
        }

        public void PopupInfo(int pageCount, string dotPrefabFormat, string infoPrefabFormat, string infoTextFormat)
        {
            GameObject saveAs = null;
            Transform parent = GameObject.Find("Popup Manager/Area").transform;
            MonoManager.current.StartCoroutine(SceneUtils.LoadSceneAsync(MetaStringDefine.LOBBY_BUNDLE_NAME, "Popup Information Scene", parent, true,
                (result) =>
                {
                    PopupManager.Instance.Open(result);
                    saveAs = result;
                    var bb = saveAs.GetComponent<Blackboard>();
                    if (bb != null)
                    {
                        BlackboardUtils.SetOrCreateValue<int>(bb, "pageCount", pageCount);
                        BlackboardUtils.SetOrCreateValue<string>(bb, "bundle", MetaStringDefine.LOBBY_BUNDLE_NAME);
                        BlackboardUtils.SetOrCreateValue<string>(bb, "dotPrefabFormat", dotPrefabFormat);
                        BlackboardUtils.SetOrCreateValue<string>(bb, "infoPrefabFormat", infoPrefabFormat);
                        BlackboardUtils.SetOrCreateValue<string>(bb, "infoTextFormat", infoTextFormat);

                        var variable = BlackboardUtils.GetOrCreateVariable<GameObject>(bb, "caller");
                        variable.value = saveAs.gameObject;
                    }

                    saveAs.gameObject.SetActive(true);
                }
            ));

            BiEventUtils.ClickReportInformation();
        }

        public IEnumerator ChangeChannelTypeAsync(ChannelType _channelType)
        {
            if (!ChatMetaManager.Instance.IsValidate(_channelType)) yield break;

            CurrentChannelType = _channelType;
            _lastChannelType = CurrentChannelType;

            //Loading
            if (!ChatMessenger.Instance.IsConnected(CurrentChannelID))
            {
                SetLoadingSpinner(true);
                yield return new WaitUntil(() => ChatMessenger.Instance.IsConnected(CurrentChannelID));
                SetLoadingSpinner(false);
            }
            //BI Event
            BI_client_chat_enter();

            UpdateProfilesAsync();
            //Chatting Area SetUp

            ReceiveCurrentChatDataList();

            // data clear.
            if (ChatDataList.Count > maxDataCount)
            {
                int removeCount = ChatDataList.Count;
                removeCount -= refreshDataCount;

                ChatMessenger.Instance.RemoveChatPolls(CurrentChannelID, removeCount);
                ReceiveCurrentChatDataList();
            }

            OnChannelChanged?.Invoke(CurrentChannelType);//(Refresh Include)
            chattingArea.ScrollToLastAndRefresh();


            ChatMetaManager.Instance.ConfirmReadMessage(_channelType);
        }

        public void Refresh()
        {
            chattingArea.Refresh();
        }

        public void ReceiveCurrentChatDataList()
        {
            // Debug.LogError(string.Format("ReceiveCurrentChatDataList 1 : {0}", Time.realtimeSinceStartup));
            ChatDataList.Clear();
            ChatDataList = ChatMessenger.Instance.GetChatDataList(CurrentChannelID);
            // Debug.LogError(string.Format("ReceiveCurrentChatDataList 2 : {0}", Time.realtimeSinceStartup));
        }

        public void UpdateProfilesAsync()
        {
            UserProfiles.Clear();
            profileArea.Refresh();

            string captureChannelID = CurrentChannelID;
            if (CurrentChannelType == ChannelType.Club)
            {
                var clubID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubId");
                if (clubID == null)
                {
                    profileArea.Refresh();
                    return;
                }

                BagelCodeClientAPI.SilenceClubInfoRequest(clubID.value, true, 0,
                    (response) =>
                    {
                        if (captureChannelID != CurrentChannelID) return;

                        var meClubID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubId");
                        if (meClubID.value > 0 && meClubID.value == response.clubInfo.id)
                        {
                            if (response.myClubInfo == null)
                            {
                                bool stringError = false;
                                ErrorPopupInfo info = new ErrorPopupInfo();
                                info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CLUB_REMOVED_ALERT", out stringError);
                                info.type = ErrorPopupType.OK;
                                info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OK", out stringError);

                                ErrorPopupHandler.Instance.OpenError(info);
                                BlackboardQueryUtils.SetMyClubId(0);
                            }
                            else
                            {
                                UserProfiles.Clear();

                                for (int i = 0; i < response.clubMemberList.Count; ++i)
                                {
                                    UserProfiles.Add(new ChatUserProfileInfo(response.clubMemberList[i], i));
                                }

                                RefreshUserProfiles();
                                UserProfiles.Sort(ChatMetaManager.Instance.GetProfileComparer(CurrentChannelType));
                            }

                            profileArea.Refresh();
                        }
                    },
                    null
                );
            }
            else
            {
                ChatMessenger.Instance.GetUserProfilesAsync(CurrentChannelID,
                    (response) =>
                    {
                        if (captureChannelID != CurrentChannelID) return;

                        UserProfiles.Clear();

                        foreach (UserProfile profile in response.profileList)
                        {
                            UserProfiles.Add(new ChatUserProfileInfo(profile));
                        }

                        RefreshUserProfiles();

                        UserProfiles.Sort(ChatMetaManager.Instance.GetProfileComparer(CurrentChannelType));
                        profileArea.Refresh();
                    },
                    null
                );
            }
        }

        public void RefreshUserProfiles()
        {
            var ignoreList = BlackboardQueryUtils.GetIgnoreUserIdAll();

            foreach (string userId in ignoreList)
                UserProfiles.RemoveAll(u => u.userId == userId);
        }

        public void RequestRecentAsync()
        {
            int count = ChatDataList.Count;
            string captureChannelID = CurrentChannelID;
            SetLoadingSpinner(true);
            ChatMessenger.Instance.RequestRecentAsync(CurrentChannelID, ChatDataList.GetMinID(), (_channelID, chatData) =>
            {
                if (captureChannelID != _channelID) return;
                SetLoadingSpinner(false);
            });
        }

        public void SendChatMessage(string message)
        {
            if (!IsSpeakerEnough()) return;

            var reqChatData = ChatMessenger.Instance.MakeChatMessageData(CurrentChannelID);
            if (reqChatData == null || reqChatData.chatPoll == null)
            {
                Debug.LogError("ChattingController.SendChatMessage failure. reqChatData is invalid.");
                return;
            }

            if (string.IsNullOrEmpty(message))
                message = StringTable.BadWordFilter(message);

            reqChatData.chatPoll.ApplyData(message);

            SendChatMessageBase(reqChatData);
        }

        public void SendChatMessageEmoji(EmojiChat emojiChat)
        {
            if (!IsSpeakerEnough()) return;

            var reqChatData = ChatMessenger.Instance.MakeChatMessageData(CurrentChannelID);
            if (reqChatData == null || reqChatData.chatPoll == null)
            {
                Debug.LogError("ChattingController.SendChatMessageEmoji failure. reqChatData is invalid.");
                return;
            }

            reqChatData.chatPoll.ApplyData(emojiChat);

            MessageDispatcher.Dispatch("OnMetaUIEvent", new EventData<float>("OnPostChatMessage", Time.realtimeSinceStartup));

            SendChatMessageBase(reqChatData);
        }

        public void SendChatMessageInstant(InstantChat instantChat)
        {
            if (!IsSpeakerEnough()) return;

            var reqChatData = ChatMessenger.Instance.MakeChatMessageData(CurrentChannelID);
            if (reqChatData == null || reqChatData.chatPoll == null)
            {
                Debug.LogError("ChattingController.SendChatMessageInstant failure. reqChatData is invalid.");
                return;
            }

            reqChatData.chatPoll.ApplyData(instantChat);

            MessageDispatcher.Dispatch("OnMetaUIEvent", new EventData<float>("OnPostChatMessage", Time.realtimeSinceStartup));

            SendChatMessageBase(reqChatData);
        }

        public void SendChatMessageClubPr()
        {
            if (!IsSpeakerEnough()) return;

            var reqChatData = ChatMessenger.Instance.MakeChatMessageData(CurrentChannelID);
            if (reqChatData == null || reqChatData.chatPoll == null)
            {
                Debug.LogError("ChattingController.SendChatMessageClubPr failure. reqChatData is invalid.");
                return;
            }

            var clubId = BlackboardUtils.FindVariable<long>(null, "/clubId").value;
            var clubName = BlackboardUtils.FindVariable<string>(null, "/clubInfo/name").value;
            var clubSymbol = BlackboardUtils.FindVariable<string>(null, "/clubInfo/symbol").value;
            reqChatData.chatPoll.ApplyData(clubId, clubName, clubSymbol);

            MessageDispatcher.Dispatch("OnMetaUIEvent", new EventData<float>("OnPostChatMessage", Time.realtimeSinceStartup));

            SendChatMessageBase(reqChatData);
        }

        public void ResendChatPoll(ChatMessageData chatData)
        {
            if (!IsSpeakerEnough()) return;

            RemoveChatPoll(chatData);

            chatData = ChatMessenger.Instance.ResendChatMessageData(chatData);
            SendChatMessageBase(chatData, true);
        }

        public void RemoveChatPoll(ChatMessageData chatData)
        {
            int removeIndex = ChatMessenger.Instance.GetChatRequestDataIndex(CurrentChannelID, chatData.chatPoll.requestId);
            if (removeIndex > -1)
            {
                // Debug.LogError(string.Format("Remove index = {0}, Request ID = {1}", removeIndex, chatData.chatPoll.requestId));
                ChatMessenger.Instance.RemoveChatPoll(CurrentChannelID, chatData.chatPoll.requestId);
                ReceiveCurrentChatDataList();
                chattingArea.RemoveItem(removeIndex);
            }
        }

        public void SetLoadingSpinner(bool onOff)
        {
            var loadingAnchor = rootElement.Find("Loading Anchor");
            loadingAnchor.transform.GetChild(0).gameObject.SetActive(onOff);
        }

        public void SetTouchKeyboarding()
        {
            if (!TouchScreenKeyboard.isSupported) return;
            IsTouchKeyboarding = true;
            if (ChatDataList.Count > 0)
            {
                var lastChatData = ChatDataList[ChatDataList.Count - 1];
                ChatDataList.Clear();
                ChatDataList.Add(lastChatData);
                chattingArea.ScrollToLastAndRefresh();
            }
            StartCoroutine(WaitTouchKeyboardEnd());
        }

        private IEnumerator WaitTouchKeyboardEnd()
        {
            yield return new WaitForSeconds(0.1f);
#if UNITY_ANDROID || UNITY_IOS
            yield return new WaitUntil(() => !TouchScreenKeyboard.visible);
#endif
            IsTouchKeyboarding = false;

            ReceiveCurrentChatDataList();
            chattingArea.ScrollToLastAndRefresh();
        }

        private void SendChatMessageBase(ChatMessageData chatData, bool isResend = false)
        {
            bool isGlobal = CurrentChannelType == ChannelType.Global;
            // long reqId = chatData.chatPoll.id;

            // MessageDispatcher.Dispatch("OnMetaUIEvent", new EventData<float>("OnPostChatMessage", Time.realtimeSinceStartup));

            chatData.Post();

            // int prevPollCount = chatDataList.Count;
            if (isGlobal)
            {
                if (!isResend)
                {
                    BlackboardQueryUtils.SpendGlobalChatSpeaker();
                }

                ChatMessenger.Instance.SendChatMessageInGlobal(CurrentChannelID, chatData,
                    (success) =>
                    {
                        BI_client_chat(chatData, success);

                        if (!success)
                        {
                            if (!IsTouchKeyboarding)
                            {
                                int removeIndex = ChatMessenger.Instance.GetChatRequestDataIndex(CurrentChannelID, chatData.chatPoll.requestId);
                                if (removeIndex > -1)
                                {
                                    // Debug.LogError(string.Format("Post Failed Remove index = {0}, Request ID = {1}", removeIndex, chatData.chatPoll.requestId));
                                    // ChatMessenger.Instance.RemoveChatPoll(curChannelID, chatData.chatPoll.requestId);
                                    // ReceiveCurrentChatDataList();
                                    chattingArea.RemoveItem(removeIndex);
                                }

                                chatData.SendFailed();

                                ReceiveCurrentChatDataList();
                                // chattingArea.Refresh();

                                int insertIndex = ChatMessenger.Instance.GetChatRequestDataIndex(CurrentChannelID, chatData.chatPoll.requestId);
                                // Debug.LogError(string.Format("insert index {0}, {1}", insertIndex, chatData.chatPoll.requestId));
                                chattingArea.InsertMyChatItem(insertIndex);
                            }
                            else
                            {
                                chatData.SendFailed();
                            }
                        }
                        else
                        {
                            chatData.SendSuccess();
                        }
                    }
                );
            }
            else
            {
                ChatMessenger.Instance.SendChatMessage(CurrentChannelID, chatData,
                    (success) =>
                    {
                        BI_client_chat(chatData, success);

                        if (!success)
                        {
                            if (!IsTouchKeyboarding)
                            {
                                int removeIndex = ChatMessenger.Instance.GetChatRequestDataIndex(CurrentChannelID, chatData.chatPoll.requestId);
                                if (removeIndex > -1)
                                {
                                    // Debug.LogError(string.Format("Post Failed Remove index = {0}, Request ID = {1}", removeIndex, chatData.chatPoll.requestId));
                                    // ChatMessenger.Instance.RemoveChatPoll(curChannelID, chatData.chatPoll.requestId);
                                    // ReceiveCurrentChatDataList();
                                    chattingArea.RemoveItem(removeIndex);
                                }

                                chatData.SendFailed();

                                ReceiveCurrentChatDataList();
                                // chattingArea.Refresh();

                                int insertIndex = ChatMessenger.Instance.GetChatRequestDataIndex(CurrentChannelID, chatData.chatPoll.requestId);
                                // Debug.LogError(string.Format("insert index {0}, {1}", insertIndex, chatData.chatPoll.requestId));
                                chattingArea.InsertMyChatItem(insertIndex);
                            }
                            else
                            {
                                chatData.SendFailed();
                            }
                        }
                        else
                        {
                            chatData.SendSuccess();
                        }
                    }
                );
            }

            if (IsTouchKeyboarding)
            {
                ChatDataList.Clear();
                ChatDataList.Add(chatData);
                chattingArea.ScrollToLastAndRefresh();
            }
            else
            {
                ReceiveCurrentChatDataList();

                // for(int i=0; i < chatDataList.Count; ++i)
                // {
                //     Debug.LogError( string.Format("{4}, id = {0}, reqid = {1}, response {2}, failed {3}", chatDataList[i].chatPoll.id, chatDataList[i].chatPoll.requestId, chatDataList[i].isResponse, chatDataList[i].isSuccess, i) );
                // }

                int insertIndex = ChatMessenger.Instance.GetChatRequestDataIndex(CurrentChannelID, chatData.chatPoll.requestId);
                // Debug.LogError(string.Format("insert index {0}, {1}", insertIndex, chatData.chatPoll.requestId));
                chattingArea.InsertMyChatItem(insertIndex);
            }
        }

        #region Callback Function
        public void OnGlobalChannelChanged(string preGlobalId, string curGlobalId)
        {
            SetLoadingSpinner(false);
            UpdateProfilesAsync();
            ReceiveCurrentChatDataList();
            chattingArea.ScrollToLastAndRefresh();
        }


        public void OnAddChatMessages(ChannelType channelType, string channelID, List<ChatMessageData> addChatDataList)
        {
            // Debug.LogError("Controller OnAddChatMeessages");
            if (CurrentChannelType == channelType)
            {
                // Debug.LogError(string.Format("Controller OnAddChatMeessages {0}, {1}", channelType, addChatDataList.Count));

                if (addChatDataList.Count > 0)
                {
                    if (IsTouchKeyboarding)
                    {
                        ChatDataList.Clear();
                        ChatDataList.Add(addChatDataList[addChatDataList.Count - 1]);
                        chattingArea.ScrollToLastAndRefresh();
                    }
                    else
                    {
                        if (addChatDataList.Count > 1)
                        {
                            addChatDataList.Sort(
                                (a, b) =>
                                {
                                    if (a.chatPoll.id < b.chatPoll.id) return -1;
                                    return 1;
                                }
                            );
                        }

                        ReceiveCurrentChatDataList();

                        for (int i = 0; i < addChatDataList.Count; ++i)
                        {
                            if (addChatDataList[i].chatPoll.requestId > 0)
                            {
                                int removeIndex = ChatMessenger.Instance.GetChatRequestDataIndex(CurrentChannelID, addChatDataList[i].chatPoll.requestId);
                                // Debug.LogError(string.Format("Remove index = {0}, Request ID = {1}", removeIndex, addChatDataList[i].chatPoll.requestId));

                                if (removeIndex > -1)
                                {
                                    ChatMessenger.Instance.RemoveChatPoll(CurrentChannelID, addChatDataList[i].chatPoll.requestId);
                                    ReceiveCurrentChatDataList();
                                    chattingArea.RemoveItem(removeIndex);
                                }
                            }

                            // int insertIndex = ChatMessenger.Instance.GetChatPollDataIndex(curChannelID, addChatDataList[i].chatPoll.id);

                            // Debug.LogError(string.Format("Insert index = {0}, Poll ID = {1}", insertIndex, addChatDataList[i].chatPoll.id));

                            // chattingArea.InsertPollChat(insertIndex);
                        }

                        var ignoreList = BlackboardQueryUtils.GetIgnoreUserIdAll();
                        for (int i = 0; i < addChatDataList.Count; ++i)
                        {
                            // if(addChatDataList[i].chatPoll.requestId > 0)
                            // {
                            //     int removeIndex = ChatMessenger.Instance.GetChatRequestDataIndex(curChannelID, addChatDataList[i].chatPoll.requestId);
                            //     Debug.LogError(string.Format("Remove index = {0}, Request ID = {1}", removeIndex, addChatDataList[i].chatPoll.requestId));

                            //     if(removeIndex > -1)
                            //     {
                            //         ChatMessenger.Instance.RemoveChatPoll(curChannelID, addChatDataList[i].chatPoll.requestId);
                            //         ReceiveCurrentChatDataList();
                            //         chattingArea.RemoveItem(removeIndex);
                            //     }
                            // }

                            if(!ignoreList.Contains(addChatDataList[i].chatPoll.userId))
                            {
                                int insertIndex = ChatMessenger.Instance.GetChatPollDataIndex(CurrentChannelID, addChatDataList[i].chatPoll.id);
                                // Debug.LogError(string.Format("Insert index = {0}, Poll ID = {1}", insertIndex, addChatDataList[i].chatPoll.id));

                                if(insertIndex >= 0 && insertIndex < ChatMessenger.Instance.GetChatDataList(CurrentChannelID).Count)
                                {
                                    chattingArea.InsertPollChat(insertIndex);
                                }
                            }
                        }
                    }
                }

                // Check. Success Request Table.
                if (IsOpen)
                    ChatMetaManager.Instance.ConfirmReadMessage(channelType);
            }
            else
            {
                for (int i = 0; i < addChatDataList.Count; ++i)
                {
                    if (addChatDataList[i].chatPoll.requestId > 0)
                    {
                        ChatMessenger.Instance.RemoveChatPoll(ChatMetaManager.Instance.GetChannelID(channelType), addChatDataList[i].chatPoll.requestId);
                    }
                }
            }
        }

        public void OnAddRecentChatMessages(ChannelType channelType, string channelID, List<ChatMessageData> addChatDataList)
        {
            if (CurrentChannelType == channelType)
            {
                if (channelType == ChannelType.Global && CurrentChannelID != channelID) return;

                if (addChatDataList.Count > 0)
                {
                    ReceiveCurrentChatDataList();

                    chattingArea.ScrollToFirstAndRefresh(addChatDataList.Count);
                }
            }
        }

        public void OnRemoveRequestChatMessages(ChannelType channelType, string channelID, List<ChatMessageData> addChatDataList)
        {
            if (CurrentChannelType == channelType)
            {
                // Debug.LogError(string.Format("Controller OnAddChatMeessages {0}, {1}", channelType, addChatDataList.Count));
                // if (isTouchKeyboarding)
                // {
                //     chatDataList.Clear();
                // }

                // if(addChatDataList.Count > 0)
                // {
                //     if(isTouchKeyboarding)
                //     {
                //         chatDataList.Add(addChatDataList[addChatDataList.Count -1]);
                //         chattingArea.ScrollToLastAndRefresh();
                //     }
                //     else
                //     {
                //         ReceiveCurrentChatDataList();
                //         for(int i=0; i < addChatDataList.Count; ++i)
                //         {
                //             // Debug.LogError(addChatDataList[i].GetAssetName());
                //             // Debug.LogError(addChatDataList[i].chatPoll.userId);
                //             // Debug.LogError(addChatDataList[i].chatPoll.type);
                //             Debug.LogError(addChatDataList[i].chatPoll.id);
                //             Debug.LogError(addChatDataList[i].chatPoll.requestId);

                //             chattingArea.InsertOtherChat(-1);
                //         }
                //     }
                // }

                // // Check. Success Request Table.
                // if (isOpen)
                //     ChatMetaManager.Instance.ConfirmReadMessage(channelType);
            }
            else
            {
                // for(int i=0; i < addChatDataList.Count; ++i)
                // {
                //     OnRemoveRequestChatPoll(addChatDataList[i].chatPoll);
                // }
            }
        }

        public void OnClubLeaved(string preClubChannelId)
        {
            if (CurrentChannelType == ChannelType.Club)
            {
                if (IsOpen)
                {
                    StartCoroutine(ChangeChannelTypeAsync(ChannelType.Global));
                }
            }
        }

        #endregion

        #region Initialize
        protected override void Awake()
        {
            base.Awake();

            if (gameObject.GetComponent<MessageRouter>() == null)
                gameObject.AddComponent<MessageRouter>();

            if (gameObject.GetComponent<MetaUIEventDispatcher>() == null)
                gameObject.AddComponent<MetaUIEventDispatcher>();
        }

        private void Start()
        {
            rootElement = GetComponent<ContextElement>();
            rootElement.UpdateContext(true);

            chattingArea = rootElement.FindElement<ChattingArea>("Chatting Area");
            profileArea = rootElement.FindElement<ChattingProfileArea>("Profile Area");

            InitBackButton();
            InitCloseButton();
            InitReportInfoButton();
            ChatMetaManager.Instance.AddListener(this);

            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);
            Register(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_BLOCKED_USER_CHANGED, OnBlockedUserChanged);

            isInit = true;
        }

        private void OnBlockedUserChanged()
        {
            // remove ignore
            RefreshUserProfiles();
            profileArea.Refresh();

            // Refresh Chats
            ReceiveCurrentChatDataList();
            Refresh();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (ChatMetaManager.Instance != null && MetaSystem.Instance != null)
                MetaSystem.UnSubscribeBackButton(this.GetHashCode());
        }

        private void OnDestroy()
        {
            // MetaSystem.UnSubscribeBackButton(this.GetHashCode());

            StopAllCoroutines();

            if (ChatMetaManager.Instance != null)
                ChatMetaManager.Instance.RemoveListener(this);
        }

        private void InitBackButton()
        {
            var bgButton = rootElement.Find("Background Button").GetComponent<ContextButton>();
            bgButton.button.onClick.AddListener(
                () =>
                {
                    Close();
                }
            );
        }

        private void InitCloseButton()
        {
            var closeButton = rootElement.Find("Button Close").GetComponent<ContextButton>();
            closeButton.button.onClick.AddListener(
                () =>
                {
                    Close();
                }
            );
        }

        private void InitReportInfoButton()
        {
            var reportInfoButton = rootElement.Find("Button Report Info").GetComponent<ContextButton>();
            reportInfoButton.button.onClick.AddListener(
                () =>
                {
                    PopupInfo(1, "Information Dots", "Chatting Report Information", "CHATTING_REPORT_INFORMATION_TEXT");
                }
            );
        }
        #endregion



        private bool IsSpeakerEnough()
        {
            bool isGlobal = CurrentChannelType == ChannelType.Global;
            if (!isGlobal) return true;
            int speaker = BlackboardQueryUtils.GetGlobalChatSpeakerCount();
            if (speaker <= 0)
            {
                ChatSpeakerContext.OpenSpeakerPopup();
                return false;
            }
            return true;
        }

        private void BI_client_chat_enter()
        {
            string chatType = CurrentChannelType.ToString().ToLower();
            string language = null;
            if (CurrentChannelType == ChannelType.Global)
            {
                language = ChatMetaManager.Instance.globalChannelInfos[ChatMetaManager.Instance.globalChannelIndex].channelName;
            }

            BiEventUtils.ChatEnter(chatType, language, CurrentChannelID);
        }

        private void BI_client_chat(ChatMessageData chatData, bool isSuccess)
        {
            string chatType = CurrentChannelType.ToString().ToLower();
            string language = null;
            bool isSpeakerUsed = false;
            if (ChatMetaManager.Instance.GetChannelType(chatData.chatPoll.channelId) == ChannelType.Global)
            {
                language = ChatMetaManager.Instance.globalChannelInfos[ChatMetaManager.Instance.globalChannelIndex].channelName;
                isSpeakerUsed = true;
            }

            BiEventUtils.SendChat(chatType, language, CurrentChannelID, chatData, isSpeakerUsed, isSuccess);
        }
    }
}
