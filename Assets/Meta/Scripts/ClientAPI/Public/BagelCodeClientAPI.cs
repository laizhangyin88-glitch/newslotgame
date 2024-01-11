using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using BagelCode.Internal;
using SlotMaker;
using SlotMaker.Contents;
using BagelCode.ClientModels;
using BagelCode.Task.Actions;
using BagelCode.Protobuf;

namespace BagelCode
{

    public class BagelCodeClientAPI
    {
        public static void Login(Action<LoginResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
#if DEV
            var deviceID = PlayerPrefs.GetString("DEBUG_DEVICE_ID", "");
            if (string.IsNullOrEmpty(deviceID))
                deviceID = NativeHelper.Instance.GetDeviceID();
#endif
            var info = BlackboardUtils.FindVariable<NodeCanvas.Framework.Blackboard>(MainBlackboard.Get(), "/pushInfo");
            var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "userLoginInfo");
            string strContent = "";
            if (BlackboardUtils.FindVariable<string>(bb, "loginInfo") != null)
                strContent = BlackboardUtils.FindVariable<string>(bb, "loginInfo").value;
            LoginRequest request = new LoginRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                clientOs = ApplicationSettings.GetPlatformName().ToUpper(),
                clientNumberVersion = ApplicationSettings.GetClientVersionNumber(),
#if DEV
                deviceId = deviceID,
#else
                deviceId = NativeHelper.Instance.GetDeviceID(),
#endif
                deviceName = ApplicationSettings.GetDeviceModel(),
                assetVersion = ApplicationSettings.Instance.bundleVersion,
                loginTime = TimeUtils.GetTimeStamp(),
                timezoneOffset = TimeUtils.GetTimeZoneOffset(),
                language = ApplicationSettings.GetDeviceLanguage(),
                isServerMaintenanceIgnore = false,
                clickPn = (info != null && info.value != null) ? "click_pn" : "",
                devicePushSetting = strContent
            };

            BagelCodeHTTP.MakeApiCall("/v0/main/login", null, request,
                LoginResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void LoginFacebook(Action<LoginResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            string url = Application.absoluteURL;
            int iqs = url.IndexOf('?');
            string rawQueryString = "";
            if (iqs >= 0)
                rawQueryString = (iqs < url.Length - 1) ? url.Substring(iqs + 1) : "";

            LoginFacebookRequest request = new LoginFacebookRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                clientOs = ApplicationSettings.GetPlatformName().ToUpper(),
                clientNumberVersion = ApplicationSettings.GetClientVersionNumber(),
                accessToken = SocialManager.Instance.GetFacebookAccessToken(),
                deviceName = ApplicationSettings.GetDeviceModel(),
                loginTime = TimeUtils.GetTimeStamp(),
                timezoneOffset = TimeUtils.GetTimeZoneOffset(),
                language = ApplicationSettings.GetDeviceLanguage(),
                isServerMaintenanceIgnore = false,
                queryString = rawQueryString,
                contextId = NativeHelper.Instance.GetContextId(),
                osVersion = SystemInfo.operatingSystem,
                assetVersion = ApplicationSettings.Instance.bundleVersion
            };

            BagelCodeHTTP.MakeApiCall("/v0/main/login/facebook", null, request,
                LoginResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void Logout(Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits()
            };

            BagelCodeHTTP.MakeApiCall("/v0/main/logout", null, request, SimpleResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void Lobby(Action<LobbyResponseV6> responseCallback, HTTPErrorCallback errorCallback)
        {
            LobbyRequest request = new LobbyRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                currentTime = TimeUtils.GetTimeStamp(),
                timezoneOffset = TimeUtils.GetTimeZoneOffset()
            };

            BagelCodeHTTP.MakeApiCall("/v6/main/lobby", null, request,
                LobbyResponseV6.Deserialize, responseCallback, errorCallback);
        }

        public static void Rate(int rating, Action<RateResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            RateRequest request = new RateRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                rating = rating
            };

            BagelCodeHTTP.MakeApiCall("/v0/main/rate", null, request,
                RateResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void Leaderboard(PeriodTypes reqPriodType, SortTypes reqSortType,
                                        Action<LeaderboardResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            LeaderboardRequest request = new LeaderboardRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                periodType = reqPriodType,
                sortType = reqSortType
            };

            BagelCodeHTTP.MakeApiCall("/v1/main/leaderboard", null, request,
                LeaderboardResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void GetUserProfile(string userId, Action<UserInfoProfileResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            UserInfoSimpleRequest request = new UserInfoSimpleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                targetUserId = userId
            };

            BagelCodeHTTP.MakeApiCall("/v0/user/info/profile", null, request,
                UserInfoProfileResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void GetUserProfileList(List<string> userIdList, Action<UserProfileListResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            UserProfileListRequest request = new UserProfileListRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                userIdList = userIdList
            };

            BagelCodeHTTP.MakeApiCall("/v0/user/profile_list", null, request,
                UserProfileListResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void EditUserProfile(string reqName, Gender reqGender, int reqAge, string reqCountryCode, string reqMessage, bool reqIsRewardNeeded, Action<UserEditMeResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            UserEditMeRequestV1 request = new UserEditMeRequestV1
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                name = reqName,
                gender = reqGender,
                age = reqAge,
                message = reqMessage,
                country = reqCountryCode
            };

            BagelCodeHTTP.MakeApiCall("/v1/user/edit/me", null, request,
                UserEditMeResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void UploadProfile(byte[] reqImageBytes, bool isUpdateProfile, Action<UserProfileUploadResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            UserProfileUploadRequest request = new UserProfileUploadRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                imageHighResolution = reqImageBytes,
                isRewardNeeded = isUpdateProfile
            };

            BagelCodeHTTP.MakeApiCall("/v3/user/profile_upload", null, request,
                UserProfileUploadResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void UserLike(string reqUserId, Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            UserInfoSimpleRequest request = new UserInfoSimpleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                targetUserId = reqUserId
            };

            BagelCodeHTTP.MakeApiCall("/v0/user/like", null, request,
                SimpleResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void Kudo(string reqUserId, int kudoID, KudoLikeType likeType, string biKudoType, Action<UserKudoResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            UserKudoRequest request = new UserKudoRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                targetUserId = reqUserId,
                kudoId = kudoID,
                kudoLikeType = likeType,
                kudoType = biKudoType
            };

            BagelCodeHTTP.MakeApiCall("/v0/user/kudo", null, request,
                UserKudoResponse.Deserialize, responseCallback, errorCallback);
        }

        //public static void UserReport(string reqUserId, BlockType blockType, Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        //{
        //    UserReportRequest request = new UserReportRequest()
        //    {
        //        blockseq = BagelCodeHTTP.FetchBlockSeq(),
        //        ackMask = BagelCodeHTTP.GenerateAckBits(),
        //        blockType = blockType,
        //        targetUserId = reqUserId
        //    };

        //    BagelCodeHTTP.MakeApiCall("/v1/user/report", null, request,
        //        SimpleResponse.Deserialize, responseCallback, errorCallback);
        //}

        public static void UserBlock(string reqUserId, BlockType blockType, Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            UserBlockRequest request = new UserBlockRequest()
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                blockType = blockType,
                targetUserId = reqUserId
            };

            BagelCodeHTTP.MakeApiCall("/v0/user/block", null, request,
                SimpleResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void UserUnblock(string reqUserId, Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            UserUnblockRequest request = new UserUnblockRequest()
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                targetUserId = reqUserId
            };

            BagelCodeHTTP.MakeApiCall("/v0/user/unblock", null, request,
                SimpleResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void FriendAdd(string reqUserId, Action<FriendAddResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            FriendAddRequest request = new FriendAddRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                targetUserId = reqUserId
            };

            BagelCodeHTTP.MakeApiCall("/v0/friend/add", null, request,
                FriendAddResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void FriendCodeAdd(string friendCode, Action<FriendAddByCodeResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            FriendAddByCodeRequest request = new FriendAddByCodeRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                targetFriendCode = friendCode
            };

            BagelCodeHTTP.MakeApiCall("/v0/friend/add_by_code", null, request,
                FriendAddByCodeResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void FriendAddEncourage(List<string> targetUserIdList, Action<FriendAddEncourageListResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            FriendAddEncourageListRequest request = new FriendAddEncourageListRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                targetUserIdList = targetUserIdList,
                ackMask = BagelCodeHTTP.GenerateAckBits()
            };

            BagelCodeHTTP.MakeApiCall("/v0/friend/add_encourage_list", null, request,
                FriendAddEncourageListResponse.Deserialize, responseCallback, errorCallback);

        }

        public static void FriendAddList(List<string> _targetUserIdList, string _type, Action<FriendAddListResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            FriendAddListRequest request = new FriendAddListRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                targetUserIdList = _targetUserIdList,
                type = _type
            };

            BagelCodeHTTP.MakeApiCall("/v0/friend/add_list", null, request, FriendAddListResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void FriendAccept(string userId, Action<FriendAcceptResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            FriendAcceptRequest request = new FriendAcceptRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                targetUserId = userId
            };

            BagelCodeHTTP.MakeApiCall("/v0/friend/accept", null, request,
                FriendAcceptResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void FriendAcceptAll(Action<FriendAcceptAllResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits()
            };

            BagelCodeHTTP.MakeApiCall("/v0/friend/accept/all", null, request,
                FriendAcceptAllResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void FriendRejectAll(Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits()
            };

            BagelCodeHTTP.MakeApiCall("/v0/friend/reject/all", null, request,
                SimpleResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void FriendReject(string userId, Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            FriendRejectRequest request = new FriendRejectRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                targetUserId = userId
            };

            BagelCodeHTTP.MakeApiCall("/v0/friend/reject", null, request,
                SimpleResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void FriendRemove(string userId, Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            FriendRemoveRequest request = new FriendRemoveRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                targetUserId = userId
            };

            BagelCodeHTTP.MakeApiCall("/v0/friend/remove", null, request,
                SimpleResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void FriendGiftCollect(Action<FriendGiftCollectResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest()
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits()
            };

            BagelCodeHTTP.MakeApiCall("/v0/friend/gift/collect", null, request,
                FriendGiftCollectResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void FriendGiftSend(Action<FriendGiftSendResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest()
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits()
            };

            BagelCodeHTTP.MakeApiCall("/v0/friend/gift/send", null, request,
                FriendGiftSendResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void FriendOnlineList(Action<FriendOnlineListResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest()
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits()
            };

            BagelCodeHTTP.MakeApiCall("/v0/friend/online_list", null, request,
                FriendOnlineListResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void InvitableUserList(Action<UserInvitableListResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest()
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits()
            };

            BagelCodeHTTP.MakeApiCall("/v0/user/invitable_list", null, request,
                UserInvitableListResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void Invite(InviteType type, List<string> inviteUserList, Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            UserInviteRequest request = new UserInviteRequest()
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                targetUserIdList = inviteUserList,
                inviteType = type
            };

            BagelCodeHTTP.MakeApiCall("/v0/user/invite", null, request,
                SimpleResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void TimeBonusCollect(int timeBonusEventID, Action<TimeBonusCollectResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            TimeBonusCollectRequest request = new TimeBonusCollectRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                timeBonusEventId = timeBonusEventID
            };

            BagelCodeHTTP.MakeApiCall("/v0/main/time_bonus/collect", null, request,
                    TimeBonusCollectResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void LuckySpinBonus(Action<GameSpinCollectFreeResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits()
            };

            BagelCodeHTTP.MakeApiCall("/v0/main/game_spin/collect_free", null, request,
                    GameSpinCollectFreeResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void LuckySpinCollectCoin(Action<GameSpinCollectCoinResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits()
            };

            BagelCodeHTTP.MakeApiCall("/v0/main/game_spin/collect_coin", null, request,
                GameSpinCollectCoinResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void VideoAdsCliam(Action<VideoAdsClaimResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits()
            };
            // #if UNITY_WEBGL && !UNITY_EDITOR
            // bypass adblock chrome extensions (they filter out video_ads/* path)
            // BagelCodeHTTP.MakeApiCall("/v0/main/video_@ds/claim", null, request,
            // #else
            BagelCodeHTTP.MakeApiCall("/v0/main/video_acls/claim", null, request,
                    // #endif
                    VideoAdsClaimResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void Purchase_CreateProgress(int productID,
                                                    int targetPurchaseID,
                                                    int piggyEventID,
                                                    int pogbEventID,
                                                    int creditMultiplierWheelEventID,
                                                    int creditMultiplierAllWheelEvent,
                                                    int metaGameEventID,
                                                    int coinShopMultiplierEventID,
                                                    int gemShopMultiplierEventID,
                                                    int dailyWheelEventID,
                                                    int gemBabShopMultiplyEventID,
                                                    int gemBabPromotionShopMultiplyEventID,
                                                    int voucherShopMultiplyEventID,
                                                    int gemMultiplierWheelEventID,
                                                    int gemMultiplierAllWheelEvent,
                                                    int tierUpShopMultiplyEventID,
                                                    int spinDealID,
                                                    List<int> ticketIDList,
                                                    int seasonPassEventID,
                                                    bool isShopEvent,
                                                    string vipDealUUID,
                                                    int vipDealInfoID,
                                                    int userGroupID,
                                                    int iamID,
                                                    string iamTriggerType,
                                                    Action<PurchaseCreateProgressResponse> responseCallback,
                                                    HTTPErrorCallback errorCallback)
        {
            PurchaseCreateProgressRequest request = new PurchaseCreateProgressRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                productId = productID,
                targetPurchaseId = targetPurchaseID,
                piggyBankEventId = piggyEventID,
                pogBoosterEventId = pogbEventID,
                cmwBonusEventId = creditMultiplierWheelEventID,
                cmwMultiplyEventId = creditMultiplierAllWheelEvent,
                metaGameEventId = metaGameEventID,
                coinShopMultiplyEventId = coinShopMultiplierEventID,
                gemShopMultiplyEventId = gemShopMultiplierEventID,
                dailyWheelEventId = dailyWheelEventID,
                gemBabShopMultiplyEventId = gemBabShopMultiplyEventID,
                gemBabPromotionShopMultiplyEventId = gemBabPromotionShopMultiplyEventID,
                voucherShopMultiplyEventId = voucherShopMultiplyEventID,
                gemBoosterBonusEventId = gemMultiplierWheelEventID,
                gemBoosterMultiplyEventId = gemMultiplierAllWheelEvent,
                tierUpShopMultiplyEventId = tierUpShopMultiplyEventID,
                spinDealId = spinDealID,
                ticketIdList = ticketIDList,
                seasonPassEventId = seasonPassEventID,
                shopEventFlag = isShopEvent,
                vipDealUuid = vipDealUUID,
                vipDealInfoId = vipDealInfoID,
                userGroupId = userGroupID,
                iamId = iamID,
                iamTriggerType = iamTriggerType,
            };

            BagelCodeHTTP.MakeApiCall("/v0/purchase/create_progress", null, request,
                            PurchaseCreateProgressResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void Purchase_Complete(long progressID,
                                                string purchaseReceipt,
                                                string purchaseSignature,
                                                string purchaseCurrencyCode,
                                                string purchaseLocalPrice,
                                                string contextID,
                                                bool _useBucks,
                                                Action<PurchaseCompleteResponseV2> responseCallback,
                                                HTTPErrorCallback errorCallback)
        {
            PurchaseCompleteRequestV2 request = new PurchaseCompleteRequestV2
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                purchaseProgressId = progressID,
                receipt = purchaseReceipt,
                signature = purchaseSignature,
                currencyCode = purchaseCurrencyCode,
                localPrice = purchaseLocalPrice,
                contextId = contextID,
                deviceType = ApplicationSettings.GetDeviceType(),
                currentTime = TimeUtils.GetTimeStamp(),
                timezoneOffset = TimeUtils.GetTimeZoneOffset(),
                useBucks = _useBucks
            };

            BagelCodeHTTP.MakeApiCall("/v2/purchase/complete", null, request,
                            PurchaseCompleteResponseV2.Deserialize, responseCallback, errorCallback);
        }

        public static void Purchase_Failed(long progressID,
                                            bool recover,
                                            string errorMsg,
                                            Action<PurchaseUpdateProgressResponse> responseCallback,
                                            HTTPErrorCallback errorCallback)
        {
            PurchaseUpdateProgressRequest request = new PurchaseUpdateProgressRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                purchaseProgressId = progressID,
                updatedState = PurchaseProgressState.PAYMENT_FAIL,
                isRecover = recover,
                errorMessage = errorMsg
            };

            BagelCodeHTTP.MakeApiCall("/v0/purchase/update_progress", null, request,
                            PurchaseUpdateProgressResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void Purchase_Recover(string recoverSku,
                                                string purchaseReceipt,
                                                string purchaseSignature,
                                                string purchaseCurrencyCode,
                                                string purchaseLocalPrice,
                                                long purchaseProgressID,
                                                Action<PurchaseRecoverResponseV3> responseCallback,
                                                HTTPErrorCallback errorCallback)
        {
            PurchaseRecoverRequestV3 request = new PurchaseRecoverRequestV3
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                sku = recoverSku,
                receipt = purchaseReceipt,
                signature = purchaseSignature,
                currencyCode = purchaseCurrencyCode,
                localPrice = purchaseLocalPrice,
                deviceType = ApplicationSettings.GetDeviceType(),
                currentTime = TimeUtils.GetTimeStamp(),
                timezoneOffset = TimeUtils.GetTimeZoneOffset(),
                purchaseProgressId = purchaseProgressID
            };

            BagelCodeHTTP.MakeApiCall("/v3/purchase/recover", null, request,
                            PurchaseRecoverResponseV3.Deserialize, responseCallback, errorCallback);
        }

        public static void BuyItemWithGem(int productID, int iamID, int userGroupID, int metaGameEventID, string iamTriggerType, string contextID, Action<PurchaseGemPaymentResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            PurchaseGemPaymentRequest request = new PurchaseGemPaymentRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                productId = productID,
                currentTime = TimeUtils.GetTimeStamp(),
                timezoneOffset = TimeUtils.GetTimeZoneOffset(),
                iamId = iamID,
                userGroupId = userGroupID,
                metaGameEventId = metaGameEventID,
                iamTriggerType = iamTriggerType,
                contextId = contextID
            };

            BagelCodeHTTP.MakeApiCall("/v0/purchase/gem_payment", null, request, PurchaseGemPaymentResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void InboxAccept(int inboxId, Action<InboxAcceptResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            InboxAcceptRequest request = new InboxAcceptRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                inboxId = inboxId,
                currentTime = TimeUtils.GetTimeStamp(),
                timezoneOffset = TimeUtils.GetTimeZoneOffset(),
                contextId = InboxUtils.GetInboxEnterContextID()
            };

            BagelCodeHTTP.MakeApiCall("/v0/inbox/accept", null, request,
                            InboxAcceptResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void GameEnter(int gameID, bool isEarlyAccess, string contextID, Action<RoomEnterResponseV3> responseCallback, HTTPErrorCallback errorCallback)
        {
            RoomEnterRandomRequestV2 request = new RoomEnterRandomRequestV2
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                contents = ContentsSerializer.SerializeEnter(gameID),
                isEarlyAccess = isEarlyAccess,
                gameId = gameID,
                slotEnterContextId = contextID
            };

            BagelCodeHTTP.MakeApiCall("/v3/room/enter/random", null, request,
                            RoomEnterResponseV3.Deserialize, responseCallback, errorCallback);
        }

        public static void GameEnterRoom(int gameID, string targetRoomID, bool isEarlyAccess, string contextID, Action<RoomEnterResponseV3> responseCallback, HTTPErrorCallback errorCallback)
        {
            RoomEnterTargetRequestV2 request = new RoomEnterTargetRequestV2
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                contents = ContentsSerializer.SerializeEnter(gameID),
                targetRoomId = targetRoomID,
                isEarlyAccess = isEarlyAccess,
                slotEnterContextId = contextID
            };

            BagelCodeHTTP.MakeApiCall("/v3/room/enter/target", null, request,
                            RoomEnterResponseV3.Deserialize, responseCallback, errorCallback);
        }

        public static void SlotSpin(long bet, long extraBet, string roomID, bool isGameSpin, bool isBonusSpin, int metaGameEventID, int gameID, int collectingGameChestDropRateMultiplyEventId, object customData, bool isAutoSpin, List<int> expEventIdList, bool isHighRollerBet, int seasonPassEventID,
            Action<SlotSpinResponseV3> responseCallback, HTTPErrorCallback errorCallback)
        {
            SlotSpinRequestV2 request = new SlotSpinRequestV2
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                contents = ContentsSerializer.SerializeSlotSpin(gameID, bet, extraBet, customData),
                gameId = gameID,
                isGameSpin = isGameSpin,
                roomId = roomID,
                isBonusSpin = isBonusSpin,
                isAutoSpin = isAutoSpin,
                metaGameEventId = metaGameEventID,
                collectingGameChestDropRateMultiplyEventId = collectingGameChestDropRateMultiplyEventId,
                expEventIdList = expEventIdList,
                isHighRollerBet = isHighRollerBet,
                seasonPassEventId = seasonPassEventID,
            };
#if DEV
            BagelCodeHTTP.MakeApiCall("/vd3/slot/debug/spin", null,
#else
            BagelCodeHTTP.MakeApiCall("/v3/slot/spin", null,
#endif
                request, SlotSpinResponseV3.Deserialize, responseCallback, errorCallback);
        }

        public static void SlotClaimBonus(int gameID, int metaGameEventID, string uid, bool isGameSpin, bool isBonusSpin, object customData, int seasonPassEventID, Action<SlotClaimBonusResponseV3> responseCallback, HTTPErrorCallback errorCallback)
        {
            SlotClaimBonusRequestV3 request = new SlotClaimBonusRequestV3
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                contents = ContentsSerializer.SerializeClaimBonus(uid, customData),
                gameId = gameID,
                isGameSpin = isGameSpin,
                isBonusSpin = isBonusSpin,
                metaGameEventId = metaGameEventID,
                seasonPassEventId = seasonPassEventID
            };

            BagelCodeHTTP.MakeApiCall("/v3/slot/claim/bonus", null, request,
                SlotClaimBonusResponseV3.Deserialize, responseCallback, errorCallback);
        }

        public static void RoomBigWin(long betCredit, long earnCredit, string roomID,
                                      Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            RoomBoastBigwinRequest request = new RoomBoastBigwinRequest
            {
                betCredit = betCredit,
                winCredit = earnCredit,
                roomId = roomID,
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits()
            };

            BagelCodeHTTP.MakeApiCall("/v0/room/boast/bigwin", null, request,
                SimpleResponse.Deserialize, responseCallback, errorCallback, false);
        }

        public static void RequestContentsStore(Action<RoomStoreInfoResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits()
            };

            BagelCodeHTTP.MakeApiCall("/v0/room/store_info", null, request,
                RoomStoreInfoResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void SlotEndTurn(uint beginBlockSeq, int metaGameEventID, int seasonPassEventID, Action<ReportTurnEndResponseV3> responseCallback, HTTPErrorCallback errorCallback)
        {
            ReportTurnEndRequestV3 request = new ReportTurnEndRequestV3
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                spinBlockseq = beginBlockSeq,
                metaGameEventId = metaGameEventID,
                seasonPassEventId = seasonPassEventID,
            };

            BagelCodeHTTP.MakeApiCall("/v3/report/turn_end", null, request,
                ReportTurnEndResponseV3.Deserialize, responseCallback, errorCallback, false);
        }

        public static void VideoPokerDeal(long betPerHand, int handCount, bool isGameDeal, bool isBonusDeal, int metaGameEventID, int gameID, int collectingGameChestDropRateMultiplyEventId, List<int> expEventIdList, bool isHighRollerBet, object customData, int seasonPassEventID,
            Action<VideoPokerDealResponseV3> responseCallback, HTTPErrorCallback errorCallback)
        {
            VideoPokerDealRequestV2 request = new VideoPokerDealRequestV2
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                contents = ContentsSerializer.SerializeVideoPokerDeal(gameID, betPerHand, handCount, customData),
                gameId = gameID,
                isBonusDeal = isBonusDeal,
                isGameDeal = isGameDeal,
                metaGameEventId = metaGameEventID,
                collectingGameChestDropRateMultiplyEventId = collectingGameChestDropRateMultiplyEventId,
                expEventIdList = expEventIdList,
                isHighRollerBet = isHighRollerBet,
                seasonPassEventId = seasonPassEventID,
            };

#if DEV
            BagelCodeHTTP.MakeApiCall("/vd3/video_poker/deal", null,
#else
            BagelCodeHTTP.MakeApiCall("/v3/video_poker/deal", null,
#endif
                request, VideoPokerDealResponseV3.Deserialize, responseCallback, errorCallback);
        }

        public static void VideoPokerDraw(List<bool> helds, int gameID, bool isBonusDeal, bool isGameDeal, int metaGameEventID, object customData, int seasonPassEventID, Action<VideoPokerDrawResponseV2> responseCallback, HTTPErrorCallback errorCallback)
        {
            VideoPokerDrawRequestV2 request = new VideoPokerDrawRequestV2
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                contents = ContentsSerializer.SerializeVideoPokerDraw(gameID, helds, customData),
                gameId = gameID,
                isBonusDeal = isBonusDeal,
                isGameDeal = isGameDeal,
                metaGameEventId = metaGameEventID,
                seasonPassEventId = seasonPassEventID
            };

            BagelCodeHTTP.MakeApiCall("/v2/video_poker/draw", null, request,
                VideoPokerDrawResponseV2.Deserialize, responseCallback, errorCallback);
        }

        public static void VideoPokerClaimBonus(int gameID, bool isBonusDeal, bool isGameDeal, int metaGameEventID, string uid, object customData, int seasonPassEventID, Action<VideoPokerClaimBonusResponseV2> responseCallback, HTTPErrorCallback errorCallback)
        {
            VideoPokerClaimBonusRequestV2 request = new VideoPokerClaimBonusRequestV2
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                contents = ContentsSerializer.SerializeClaimBonus(uid, customData),
                gameId = gameID,
                isBonusDeal = isBonusDeal,
                isGameDeal = isGameDeal,
                metaGameEventId = metaGameEventID,
                seasonPassEventId = seasonPassEventID
            };

            BagelCodeHTTP.MakeApiCall("/v2/video_poker/claim/bonus", null, request,
                VideoPokerClaimBonusResponseV2.Deserialize, responseCallback, errorCallback);
        }

        public static void VideoPokerEndDeal(uint beginBlockSeq, int metaGameEventID, int seasonPassEventID, Action<ReportDealEndResponseV3> responseCallback, HTTPErrorCallback errorCallback)
        {
            VideoPokerReportDealEndRequestV3 request = new VideoPokerReportDealEndRequestV3
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                startBlockseq = beginBlockSeq,
                metaGameEventId = metaGameEventID,
                seasonPassEventId = seasonPassEventID
            };

            BagelCodeHTTP.MakeApiCall("/v3/video_poker/report/deal_end", null, request,
                ReportDealEndResponseV3.Deserialize, responseCallback, errorCallback, false);
        }

        public static void KenoPlay(long betPerTicket, long extraBetPerTicket, int ticketCount, bool isAutoPick, List<List<int>> pickInfoList, bool isGamePlay, bool isBonusPlay, int metaGameEventID, int gameID, int collectingGameChestDropRateMultiplyEventId, List<int> expEventIdList, bool isHighRollerBet, object customData, int seasonPassEventID,
            Action<KenoPlayResponseV1> responseCallback, HTTPErrorCallback errorCallback)
        {
            KenoPlayRequest request = new KenoPlayRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                contents = ContentsSerializer.SerializeKenoPlay(gameID, betPerTicket, extraBetPerTicket, ticketCount, pickInfoList, customData),
                gameId = gameID,
                isBonusPlay = isBonusPlay,
                isGamePlay  = isGamePlay,
                metaGameEventId = metaGameEventID,
                collectingGameChestDropRateMultiplyEventId = collectingGameChestDropRateMultiplyEventId,
                isAutoChange = isAutoPick,
                expEventIdList = expEventIdList,
                isHighRollerBet = isHighRollerBet,
                seasonPassEventId = seasonPassEventID
            };

    #if DEV
            BagelCodeHTTP.MakeApiCall("/vd1/keno/play", null,
    #else
            BagelCodeHTTP.MakeApiCall("/v1/keno/play", null,
    #endif
                request, KenoPlayResponseV1.Deserialize, responseCallback, errorCallback);
        }

        public static void KenoClaimBonus(int gameID, bool isBonusPlay, bool isGamePlay, int metaGameEventID, string uid, object customData, int seasonPassEventID, Action<KenoClaimBonusResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            KenoClaimBonusRequest request = new KenoClaimBonusRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                contents = ContentsSerializer.SerializeClaimBonus(uid, customData),
                gameId = gameID,
                isBonusPlay = isBonusPlay,
                isGamePlay = isGamePlay,
                metaGameEventId = metaGameEventID,
                seasonPassEventId = seasonPassEventID
            };

            BagelCodeHTTP.MakeApiCall("/v0/keno/claim/bonus", null, request,
                KenoClaimBonusResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void GambleStart(string ticketId, Action<GambleStartResponseV2> responseCallback, HTTPErrorCallback errorCallback)
        {
            GambleStartRequestV2 request = new GambleStartRequestV2
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                contents = ContentsSerializer.SerializeGambleStart(ticketId)
            };

            BagelCodeHTTP.MakeApiCall("/v2/gamble/start", null, request,
                GambleStartResponseV2.Deserialize, responseCallback, errorCallback);
        }

        public static void GambleDeal(object customData, Action<GambleDealResponseV2> responseCallback, HTTPErrorCallback errorCallback)
        {
            GambleDealRequestV2 request = new GambleDealRequestV2
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                contents = ContentsSerializer.SerializeGambleDeal(customData)
            };

            BagelCodeHTTP.MakeApiCall("/v2/gamble/deal", null, request,
                GambleDealResponseV2.Deserialize, responseCallback, errorCallback);
        }

        public static void GambleTake(Action<GambleTakeResponseV2> responseCallback, HTTPErrorCallback errorCallback)
        {
            GambleTakeRequest request = new GambleTakeRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                contents = ContentsSerializer.SerializeGambleTake()
            };

            BagelCodeHTTP.MakeApiCall("/v2/gamble/take", null, request,
                GambleTakeResponseV2.Deserialize, responseCallback, errorCallback);
        }

        public static void OnesignalRegister(string token, Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            PushRegisterRequestV1 request = new PushRegisterRequestV1
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                deviceId = NativeHelper.Instance.GetDeviceID(),
                onesignalId = token,
                clientOs = ApplicationSettings.GetPlatformName().ToUpper()
            };

            BagelCodeHTTP.MakeApiCall("/v1/main/push/register", null, request,
                    SimpleResponse.Deserialize, responseCallback, errorCallback, false);
        }

        public static void Ping(Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits()
            };

            BagelCodeHTTP.MakeApiCall("/v0/main/ping", null, request,
                SimpleResponse.Deserialize, responseCallback, errorCallback, false);
        }

        public static void SsoValidateEmail(string email, bool useValidateEmail, Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SsoValidateEmailRequestV1 request = new SsoValidateEmailRequestV1
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                email = StringUtility.RemoveTrimAndNewline(email),
                errorIfExists = useValidateEmail
            };

            BagelCodeHTTP.MakeApiCall("/v1/sso/validate_email", null, request,
                    SimpleResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void SsoConnectEmail(string email, string validationCode, string contextId, Action<SsoConnectResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SsoConnectEmailRequestV1 request = new SsoConnectEmailRequestV1
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                email = StringUtility.RemoveTrimAndNewline(email),
                validationCode = validationCode,
                contextId = contextId,
            };

            BagelCodeHTTP.MakeApiCall("/v1/sso/connect/email", null, request,
                SsoConnectResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void SsoSwitchAccountEmail(string email, string validationCode, Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SsoSwitchAccountEmailRequest request = new SsoSwitchAccountEmailRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                email = StringUtility.RemoveTrimAndNewline(email),
                validationCode = validationCode,
            };

            BagelCodeHTTP.MakeApiCall("/v0/sso/switch_account/email", null, request,
                SimpleResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void SsoConnectFacebook(string facebookId, string contextId, Action<SsoConnectResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SsoConnectFacebookRequestV2 request = new SsoConnectFacebookRequestV2
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                facebookId = facebookId,
                accessToken = SocialManager.Instance.GetFacebookAccessToken(),
                contextId = contextId
            };

            BagelCodeHTTP.MakeApiCall("/v2/sso/connect/facebook", null, request,
                SsoConnectResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void SsoSwitchAccountFacebook(string facebookId, Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SsoSwitchAccountFacebookRequest request = new SsoSwitchAccountFacebookRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                facebookId = facebookId,
            };

            BagelCodeHTTP.MakeApiCall("/v0/sso/switch_account/facebook", null, request,
                SimpleResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void ShareFacebook(Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits()
            };

            BagelCodeHTTP.MakeApiCall("/v0/sso/boast/facebook", null, request,
                SimpleResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void SsoConnectApple(string userIDKey, string connectIDToken, string connectEmail, string userFirstName, string userLastName, string contextID, Action<SsoConnectResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SsoConnectAppleRequest request = new SsoConnectAppleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                email = StringUtility.RemoveTrimAndNewline(connectEmail),
                firstName = userFirstName,
                lastName = userLastName,
                idToken = connectIDToken,
                contextId = contextID
            };

            BagelCodeHTTP.MakeApiCall("/v0/sso/connect/apple", null, request,
                SsoConnectResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void SsoSwitchAccountApple(string connectIDToken, Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SsoSwitchAccountAppleRequest request = new SsoSwitchAccountAppleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                idToken = connectIDToken
            };

            BagelCodeHTTP.MakeApiCall("/v0/sso/switch_account/apple", null, request,
                SimpleResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void SsoLogout(Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits()
            };

            BagelCodeHTTP.MakeApiCall("/v0/sso/logout", null, request,
                SimpleResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void DailyBoostCollect(Action<DailyBoostCollectResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            DailyBoostCollectRequest request = new DailyBoostCollectRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                currentTime = TimeUtils.GetTimeStamp(),
                timezoneOffset = TimeUtils.GetTimeZoneOffset()
            };

            BagelCodeHTTP.MakeApiCall("/v0/main/daily_boost/collect", null, request,
                    DailyBoostCollectResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void VipDailyBonusCollect(Action<VipDailyBonusCollectResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            VipDailyBonusCollectRequest request = new VipDailyBonusCollectRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                currentTime = TimeUtils.GetTimeStamp(),
                timezoneOffset = TimeUtils.GetTimeZoneOffset()
            };

            BagelCodeHTTP.MakeApiCall("/v0/main/vip_daily_bonus/collect", null, request,
                    VipDailyBonusCollectResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void VideoAdsVipDailyBonusCollect(Action<VipDailyBonusVideoAdsFreeResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            VipDailyBonusVideoAdsFreeRequest request = new VipDailyBonusVideoAdsFreeRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                currentTime = TimeUtils.GetTimeStamp(),
                timezoneOffset = TimeUtils.GetTimeZoneOffset()
            };

            BagelCodeHTTP.MakeApiCall("/v0/main/vip_daily_bonus/video_acls_free", null, request,
                    VipDailyBonusVideoAdsFreeResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RoomMetaInfo(string roomID, Action<RoomMetaInfoResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            RoomMetaInfoRequest request = new RoomMetaInfoRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                roomId = roomID
            };

            BagelCodeHTTP.MakeApiCall("/v0/room/meta_info", null, request,
                    RoomMetaInfoResponse.Deserialize, responseCallback, errorCallback, false);
        }

        public static void Poll(long lastID, Action<PollResponseV1> responseCallback, HTTPErrorCallback errorCallback)
        {
            PollRequest request = new PollRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                lastReceivedId = lastID,
            };

            BagelCodeHTTP.LongPoll("/v1/system/poll", request,
                    PollResponseV1.Deserialize, responseCallback, errorCallback);
        }

        public static void Adjust(Dictionary<string, object> dict, Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ReportAdjustRequest request = new ReportAdjustRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                deviceId = NativeHelper.Instance.GetDeviceID(),
                adid = FindProperty(dict, "adid"),
                trackerToken = FindProperty(dict, "trackerToken"),
                trackerName = FindProperty(dict, "trackerName"),
                network = FindProperty(dict, "network"),
                campaign = FindProperty(dict, "campaign"),
                adgroup = FindProperty(dict, "adgroup"),
                creative = FindProperty(dict, "creative"),
                clickLabel = FindProperty(dict, "clickLabel")
            };

            BagelCodeHTTP.MakeApiCall("/v1/report/adjust", null, request, SimpleResponse.Deserialize, responseCallback, errorCallback, false);
        }

        public static void DailySpin(int eventId, bool isAuto, Action<DailyBonusWheelSpinResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            DailyBonusWheelSpinRequest request = new DailyBonusWheelSpinRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                currentTime = TimeUtils.GetTimeStamp(),
                timezoneOffset = TimeUtils.GetTimeZoneOffset(),
                dailyWheelEventId = eventId,
                isAutoSpin = isAuto
            };

            BagelCodeHTTP.MakeApiCall("/v0/main/daily_bonus_wheel/spin", null, request,
                    DailyBonusWheelSpinResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void DailyMegaSpin(int eventId, Action<DailyMegaWheelSpinResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            DailyMegaWheelSpinRequest request = new DailyMegaWheelSpinRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                dailyWheelEventId = eventId
            };

            BagelCodeHTTP.MakeApiCall("/v0/main/mega_wheel/spin", null, request,
                    DailyMegaWheelSpinResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void PassiveEventInfo(List<int> idList, Action<EventResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            EventRequest request = new EventRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                eventIdList = idList
            };

            BagelCodeHTTP.MakeApiCall("/v0/main/event", null, request, EventResponse.Deserialize, responseCallback, errorCallback, false);
        }

        public static void Nudge(string targetUserId, Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            UserInfoSimpleRequest request = new UserInfoSimpleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                targetUserId = targetUserId
            };

            BagelCodeHTTP.MakeApiCall("/v0/user/nudge", null, request, SimpleResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RedeemBonus(int productID, Action<PurchaseRedeemBonusResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            PurchaseRedeemBonusRequest request = new PurchaseRedeemBonusRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                currentTime = TimeUtils.GetTimeStamp(),
                timezoneOffset = TimeUtils.GetTimeZoneOffset(),
                productId = productID
            };

            BagelCodeHTTP.MakeApiCall("/v0/purchase/redeem_bonus", null, request, PurchaseRedeemBonusResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void TournamentInfo(Action<TournamentInfoResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            TournamentInfoRequest request = new TournamentInfoRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits()
            };

            BagelCodeHTTP.MakeApiCall("/v0/tournament/info", null, request, TournamentInfoResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void InboxList(Action<InboxListResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits()
            };

            BagelCodeHTTP.MakeApiCall("/v0/inbox/list", null, request, InboxListResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void CouponRedeem(string code, Action<CouponResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            CouponRequest request = new CouponRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                code = code,
                currentTime = TimeUtils.GetTimeStamp(),
                timezoneOffset = TimeUtils.GetTimeZoneOffset()
            };

            BagelCodeHTTP.MakeApiCall("/v0/main/coupon", null, request, CouponResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void WallOfEpicRegister(int gameID,
                                            long betCoins,
                                            long winCoins,
                                            Action<WallOfEpicRegisterResponseV1> responseCallback,
                                            HTTPErrorCallback errorCallback)
        {
            WallOfEpicRegisterRequestV1 request = new WallOfEpicRegisterRequestV1
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                winCredit = winCoins,
                betCredit = betCoins,
                gameId = gameID,
            };

            BagelCodeHTTP.MakeApiCall("/v1/wall_of_epic/register", null, request,
                WallOfEpicRegisterResponseV1.Deserialize, responseCallback, errorCallback);
        }

        public static void WallOfEpicUploadImage(int id, byte[] screenshot, Action<WallOfEpicUploadImageResponse> responseCallback,
            HTTPErrorCallback errorCallback)
        {
            WallOfEpicUploadImageRequest request = new WallOfEpicUploadImageRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                id = id,
                screenshot = screenshot
            };

            BagelCodeHTTP.MakeApiCall("/v0/wall_of_epic/upload_image", null, request,
                WallOfEpicUploadImageResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void WallOfEpicList(Action<WallOfEpicRecentListResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits()
            };

            BagelCodeHTTP.MakeApiCall("/v0/wall_of_epic/recent_list", null, request, WallOfEpicRecentListResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void WallOfEpicLike(int liekID, Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            WallOfEpicLikeRequest request = new WallOfEpicLikeRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                id = liekID
            };

            BagelCodeHTTP.MakeApiCall("/v0/wall_of_epic/like", null, request, SimpleResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void MetaJackpotInfo(MetaJackpotType type, Action<MetaJackpotResponseV1> responseCallback, HTTPErrorCallback errorCallback)
        {
            MetaJackpotRequest request = new MetaJackpotRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                jackpotType = type.ToString()
            };

            BagelCodeHTTP.MakeApiCall("/v1/main/meta_jackpot", null, request, MetaJackpotResponseV1.Deserialize, responseCallback, errorCallback, false);
        }

        public static void ChallengeInfo(Action<ChallengeInfoResponseV1> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits()
            };

            BagelCodeHTTP.MakeApiCall("/v1/challenge/info", null, request, ChallengeInfoResponseV1.Deserialize, responseCallback, errorCallback);
        }

        public static void ChallengeClaim(ChallengeType claimChallengeType, int multiplierEventID, Action<ChallengeClaimAllDoneResponseV1> responseCallback, HTTPErrorCallback errorCallback)
        {
            ChallengeClaimAllDoneRequest request = new ChallengeClaimAllDoneRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                challengeType = claimChallengeType,
                currentTime = TimeUtils.GetTimeStamp(),
                timezoneOffset = TimeUtils.GetTimeZoneOffset(),
                challengeClaimMultiplyEventId = multiplierEventID
            };

            BagelCodeHTTP.MakeApiCall("/v1/challenge/claim_all_done", null, request, ChallengeClaimAllDoneResponseV1.Deserialize, responseCallback, errorCallback);
        }

        public static void ChallengeClaimVideoAds(ChallengeType claimChallengeType, Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ChallengeClaimVideoAdsRequest request = new ChallengeClaimVideoAdsRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                challengeType = claimChallengeType
            };

            BagelCodeHTTP.MakeApiCall("/v0/challenge/claim_video_acls", null, request, SimpleResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void PreBBBClaime(Action<PreBbbClaimResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits()
            };

            BagelCodeHTTP.MakeApiCall("/v0/pre_bbb/claim", null, request, PreBbbClaimResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RegisterUserGroup(int groupID, Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            RegisterUserGroupRequest request = new RegisterUserGroupRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                userGroupId = groupID
            };

            BagelCodeHTTP.MakeApiCall("/v0/user/register_user_group", null, request, SimpleResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void TicketedBonusUse(int tiecketID, Action<TicketedBonusUseResponseV1> responseCallback, HTTPErrorCallback errorCallback)
        {
            TicketedBonusUseRequest request = new TicketedBonusUseRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                ticketId = tiecketID,
                contents = ContentsSerializer.SerializeTicketedBonusStart(tiecketID.ToString())
            };

            BagelCodeHTTP.MakeApiCall("/v1/ticketed_bonus/use", null, request, TicketedBonusUseResponseV1.Deserialize, responseCallback, errorCallback);
        }

        public static void TicketedBonusUseWithGem(int gameId, int ticketedBonusId, long bet, long extraBet, int bonusSaleEventId, int bonusMultiplyEventId, int iamId, string iamTriggerType, Action<GemUseBonusResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            GemUseBonusRequest request = new GemUseBonusRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                gameId = gameId,
                ticketedBonusId = ticketedBonusId,
                bet = bet,
                extraBet = extraBet,
                bonusSaleEventId = bonusSaleEventId,
                bonusMultiplyEventId = bonusMultiplyEventId,
                iamId = iamId,
                iamTriggerType = iamTriggerType
            };

            BagelCodeHTTP.MakeApiCall("/v0/gem/use/bonus", null, request, GemUseBonusResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void TicketedBonusClaim(int tiecketID, Action<TicketedBonusClaimResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            TicketedBonusClaimRequest request = new TicketedBonusClaimRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                ticketId = tiecketID
            };

            BagelCodeHTTP.MakeApiCall("/v0/ticketed_bonus/claim", null, request, TicketedBonusClaimResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void IAMVideoAdsCliam(int iamID, Action<InAppMessageClaimVideoAdsResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            InAppMessageClaimVideoAdsRequest request = new InAppMessageClaimVideoAdsRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                iamId = iamID
            };

            BagelCodeHTTP.MakeApiCall("/v0/in_app_message/claim_video_acls", null, request, InAppMessageClaimVideoAdsResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void LootBoxInfo(ProbType probType, int eventID, int productID, Action<ProbsResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ProbsRequest request = new ProbsRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                probType = probType,
                eventId = eventID,
                productId = productID,
                ackMask = BagelCodeHTTP.GenerateAckBits()
            };

            BagelCodeHTTP.MakeApiCall("/v0/main/probs", null, request, ProbsResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void UserClubStatusRequest(Action<ClubStatusResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
            };

            BagelCodeHTTP.MakeApiCall("/v0/club/status", null, request, ClubStatusResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void ClubListRequest(Action<ClubListResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ClubListRequest request = new ClubListRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits()
            };

            BagelCodeHTTP.MakeApiCall("/v0/club/list", null, request, ClubListResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void ClubInfoRequest(long clubID, bool isPopupInfo, int mgEventID, Action<ClubInfoResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ClubInfoRequest request = new ClubInfoRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                clubId = clubID,
                isPopup = isPopupInfo,
                timezoneOffset = TimeUtils.GetTimeZoneOffset(),
                metaGameEventId = mgEventID,
            };

            BagelCodeHTTP.MakeApiCall("/v0/club/info", null, request, ClubInfoResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void SilenceClubInfoRequest(long clubID, bool isPopupInfo, int mgEventID, Action<ClubInfoResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ClubInfoRequest request = new ClubInfoRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                clubId = clubID,
                isPopup = isPopupInfo,
                timezoneOffset = TimeUtils.GetTimeZoneOffset(),
                metaGameEventId = mgEventID,
            };

            BagelCodeHTTP.MakeApiCall("/v0/club/info", null, request, ClubInfoResponse.Deserialize, responseCallback, errorCallback, false);
        }

        public static void CreateClub(int watchRestriction, string clubName, string clubMessage, string clubSymbol, int minLevel, ClubJoinType clubJoinType, Action<ClubCreateResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ClubCreateRequest request = new ClubCreateRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                name = clubName,
                symbol = clubSymbol,
                motd = clubMessage,
                watcher = watchRestriction,
                minPlayerLevel = minLevel,
                joinType = clubJoinType
            };

            BagelCodeHTTP.MakeApiCall("/v0/club/create", null, request, ClubCreateResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void JoinClub(long joinClubID, string fromType, Action<ClubJoinResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ClubJoinRequest request = new ClubJoinRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                clubId = joinClubID,
                joinUiType = fromType
            };

            BagelCodeHTTP.MakeApiCall("/v0/club/join", null, request, ClubJoinResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void LeaveClub(long leaveClubID, Action<ClubLeaveResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ClubLeaveRequest request = new ClubLeaveRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                clubId = leaveClubID
            };

            BagelCodeHTTP.MakeApiCall("/v0/club/leave", null, request, ClubLeaveResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void UpdateClubNotice(long clubID, string notice, Action<ClubUpdateResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ClubUpdateNoticeRequest request = new ClubUpdateNoticeRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                clubId = clubID,
                notice = notice
            };

            BagelCodeHTTP.MakeApiCall("/v0/club/update/notice", null, request, ClubUpdateResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void EditClub(int watchRestriction, long clubID, string editMessage, string editSymbol, int editMinLevel, ClubJoinType editJoinType, Action<ClubUpdateResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ClubUpdateRequest request = new ClubUpdateRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                clubId = clubID,
                symbol = editSymbol,
                motd = editMessage,
                watcher = watchRestriction,
                minPlayerLevel = editMinLevel,
                joinType = editJoinType
            };

            BagelCodeHTTP.MakeApiCall("/v0/club/update", null, request, ClubUpdateResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void DonateClub(long clubID, int userDonateLevel, Action<ClubDonateResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ClubDonateRequest request = new ClubDonateRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                clubId = clubID,
                timezoneOffset = TimeUtils.GetTimeZoneOffset(),
                donateLevel = userDonateLevel
            };

            BagelCodeHTTP.MakeApiCall("/v0/club/donate", null, request, ClubDonateResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void ClubMemberPromote(long clubID, string userID, Action<ClubMemberPromoteResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ClubMemberRequest request = new ClubMemberRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                clubId = clubID,
                targetUserId = userID
            };

            BagelCodeHTTP.MakeApiCall("/v0/club/member/promote", null, request, ClubMemberPromoteResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void ClubMemberDemote(long clubID, string userID, Action<ClubMemberDemoteResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ClubMemberRequest request = new ClubMemberRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                clubId = clubID,
                targetUserId = userID
            };

            BagelCodeHTTP.MakeApiCall("/v0/club/member/demote", null, request, ClubMemberDemoteResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void ClubMemberRemove(long clubID, string userID, Action<ClubMemberRemoveResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ClubMemberRequest request = new ClubMemberRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                clubId = clubID,
                targetUserId = userID
            };

            BagelCodeHTTP.MakeApiCall("/v0/club/member/remove", null, request, ClubMemberRemoveResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void ClubMemberInvite(long clubID, string userID, Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ClubMemberRequest request = new ClubMemberRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                clubId = clubID,
                targetUserId = userID
            };

            BagelCodeHTTP.MakeApiCall("/v0/club/member/invite", null, request, SimpleResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void ClubJoinRequestList(long clubID, Action<ClubMemberRequestsResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ClubMemberRequestsRequest request = new ClubMemberRequestsRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                clubId = clubID
            };

            BagelCodeHTTP.MakeApiCall("/v0/club/member/requests", null, request, ClubMemberRequestsResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void ClubJoinRequestAccept(long clubID, string userID, Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ClubMemberRequest request = new ClubMemberRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                clubId = clubID,
                targetUserId = userID
            };

            BagelCodeHTTP.MakeApiCall("/v0/club/member/accept", null, request, SimpleResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void ClubJoinRequestDecline(long clubID, string userID, Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ClubMemberRequest request = new ClubMemberRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                clubId = clubID,
                targetUserId = userID
            };

            BagelCodeHTTP.MakeApiCall("/v0/club/member/decline", null, request, SimpleResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void ClubJoinRequestAcceptAll(long clubID, Action<ClubMemberAcceptAllResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ClubMemberAllRequest request = new ClubMemberAllRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                clubId = clubID,
            };

            BagelCodeHTTP.MakeApiCall("/v0/club/member/accept/all", null, request, ClubMemberAcceptAllResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void ClubJoinRequestDeclineAll(long clubID, Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ClubMemberAllRequest request = new ClubMemberAllRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                clubId = clubID,
            };

            BagelCodeHTTP.MakeApiCall("/v0/club/member/decline/all", null, request, SimpleResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void ClubSearchFromName(string targetName, Action<ClubSearchListResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ClubSearchNameRequest request = new ClubSearchNameRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                searchName = targetName,
            };

            BagelCodeHTTP.MakeApiCall("/v0/club/search/name", null, request, ClubSearchListResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void ClubSearchFromFilter(int clubLevel, int playerLevel, ClubJoinSearchType searchType, bool isAvailable, bool isFriendsInside, Action<ClubSearchListResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ClubSearchFilterRequest request = new ClubSearchFilterRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                minClubLevel = clubLevel,
                minPlayerLevel = playerLevel,
                clubJoinSearchType = searchType,
                availableForMe = isAvailable,
                friendsInside = isFriendsInside
            };

            BagelCodeHTTP.MakeApiCall("/v0/club/search/filter", null, request, ClubSearchListResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestClubNewsFeedList(ClubFeedFilterType requestFileterType, long feedID, int reqFeedCount, int mgEventID, out long reqBlockSeq, bool isReliability, Action<ClubFeedListResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ClubFeedListRequest request = new ClubFeedListRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                fromFeedId = feedID,
                numFeed = reqFeedCount,
                metaGameEventId = mgEventID,
                filterType = requestFileterType
            };

            reqBlockSeq = (long)request.blockseq;

            BagelCodeHTTP.MakeApiCall("/v0/club/feed/list", null, request, ClubFeedListResponse.Deserialize, responseCallback, errorCallback, isReliability);
        }

        public static void SendNewsFeedClaim(long feedID, Action<ClubFeedClaimResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ClubFeedClaimRequest request = new ClubFeedClaimRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                feedId = feedID
            };

            BagelCodeHTTP.MakeApiCall("/v0/club/feed/like", null, request, ClubFeedClaimResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void SendNewsFeedMessage(string sendMessage, Action<ClubFeedMessageResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ClubFeedMessageRequest request = new ClubFeedMessageRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                message = sendMessage
            };

            BagelCodeHTTP.MakeApiCall("/v0/club/feed/message", null, request, ClubFeedMessageResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestClubChallengeInfo(Action<ClubChallengeInfoResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits()
            };

            BagelCodeHTTP.MakeApiCall("/v0/club/challenge/info", null, request, ClubChallengeInfoResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestClubMissionInfo(int missionDate, string missionID, Action<ClubChallengeInfoMissionResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ClubChallengeInfoMissionRequest request = new ClubChallengeInfoMissionRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                date = missionDate,
                missionId = missionID
            };

            BagelCodeHTTP.MakeApiCall("/v0/club/challenge/info/mission", null, request, ClubChallengeInfoMissionResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestClubLeague(Action<ClubLeagueResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits()
            };

            BagelCodeHTTP.MakeApiCall("/v0/club/league", null, request, ClubLeagueResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestClubAllTiers(Action<ClubLeagueTiersResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits()
            };

            BagelCodeHTTP.MakeApiCall("/v0/club/league/tiers", null, request, ClubLeagueTiersResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestClubTopList(Action<ClubLeagueTopResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits()
            };

            BagelCodeHTTP.MakeApiCall("/v0/club/league/top", null, request, ClubLeagueTopResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestClubLeagueRankingListPrefetch(Action<ClubLeagueRankingListPrefetchResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits()
            };

            BagelCodeHTTP.MakeApiCall("/v0/club/league/list/prefetch", null, request, ClubLeagueRankingListPrefetchResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestClubLeagueRankingList(int _tier, int _startIndex, int _count, Action<ClubLeagueRankingListResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ClubLeagueRankingListRequest request = new ClubLeagueRankingListRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                tier = _tier,
                startIndex = _startIndex,
                count = _count
            };

            BagelCodeHTTP.MakeApiCall("/v0/club/league/list", null, request, ClubLeagueRankingListResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void Consent(int versionData, Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ConsentRequest request = new ConsentRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                consent = 1, // agree
                version = versionData
            };

            BagelCodeHTTP.MakeApiCall("/v0/main/consent", null, request, SimpleResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void TermsOfUse(int versionData, string contextID, TermsOfUseType touType, Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            TermsOfUseRequest request = new TermsOfUseRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                consent = 1, // agree
                version = versionData,
                contextId = contextID,
                termsOfUseType = touType
            };

            BagelCodeHTTP.MakeApiCall("/v0/main/terms_of_use", null, request, SimpleResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RedeemFreeCoinBooster(int productID, int targetID, int eventID, Action<PurchaseRedeemFreeCoinBoosterResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            PurchaseRedeemFreeCoinBoosterRequest request = new PurchaseRedeemFreeCoinBoosterRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                productId = productID,
                targetPurchaseId = targetID,
                freeCoinBoosterEventId = eventID
            };

            BagelCodeHTTP.MakeApiCall("/v0/purchase/redeem_free_coin_booster", null, request, PurchaseRedeemFreeCoinBoosterResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RedeemFreeDailySpin(int productID, Action<PurchaseRedeemFreeDailyBonusResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            PurchaseRedeemFreeDailyBonusRequest request = new PurchaseRedeemFreeDailyBonusRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                productId = productID
            };

            BagelCodeHTTP.MakeApiCall("/v0/purchase/redeem_free_daily_bonus", null, request, PurchaseRedeemFreeDailyBonusResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RedeemFreeGemBooster(int productID, int targetID, int eventID, Action<PurchaseRedeemFreeGemBoosterResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            PurchaseRedeemFreeGemBoosterRequest request = new PurchaseRedeemFreeGemBoosterRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                productId = productID,
                targetPurchaseId = targetID,
                freeGemBoosterEventId = eventID
            };

            BagelCodeHTTP.MakeApiCall("/v0/purchase/redeem_free_gem_booster", null, request, PurchaseRedeemFreeGemBoosterResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void AddFCFSTicket(Action<FcfsTicketAddResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits()
            };

            BagelCodeHTTP.MakeApiCall("/v0/fcfs_ticket/add", null, request, FcfsTicketAddResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void UseFCFSTicket(long fcfsId, string uri, Action<FcfsTicketUseResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            FcfsTicketUseRequest request = new FcfsTicketUseRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                fcfsTicketId = fcfsId,
                deeplinkUrl = uri
            };

            BagelCodeHTTP.MakeApiCall("/v0/fcfs_ticket/use", null, request, FcfsTicketUseResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void StatusMatchEnable(int deeplinkId, Action<StatusMatchEnableResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            StatusMatchEnableRequest request = new StatusMatchEnableRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                deeplinkId = deeplinkId
            };

            BagelCodeHTTP.MakeApiCall("/v0/status_match/enable", null, request, StatusMatchEnableResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void StatusMatchRequest(string email, StatusMatchVipProgram vipProgram, StatusMatchVipProgramApp vipProgramApp, StatusMatchVipProgramStatus vipProgramStatus,
                                              string otherVipProgramApp, string otherVipProgramStatus, string vipProgramUserId, byte[] proofImage,
                                              Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            StatusMatchRequestRequest request = new StatusMatchRequestRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                email = StringUtility.RemoveTrimAndNewline(email),
                vipProgram = vipProgram,
                vipProgramApp = vipProgramApp,
                vipProgramStatus = vipProgramStatus,
                otherVipProgramApp = otherVipProgramApp,
                otherVipProgramStatus = otherVipProgramStatus,
                vipProgramUserId = vipProgramUserId,
                proofImage = proofImage
            };

            BagelCodeHTTP.MakeApiCall("/v0/status_match/request", null, request, SimpleResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void TierMatchOfferClaimRequest(string matchCode, Action<TierMatchOfferClaimResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            TierMatchOfferClaimRequest request = new TierMatchOfferClaimRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                matchCode = matchCode
            };

            BagelCodeHTTP.MakeApiCall("/v0/tier_match_offer/claim", null, request, TierMatchOfferClaimResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void SurveyRequest(string surveyHash, Action<SurveyResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SurveyRequest request = new SurveyRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                surveyHash = surveyHash
            };

            BagelCodeHTTP.MakeApiCall("/v0/main/survey", null, request, SurveyResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RedeemActionRewardRequest(string actionRewardId, Action<ActionRewardRedeemResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ActionRewardRedeemRequest request = new ActionRewardRedeemRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                currentTime = TimeUtils.GetTimeStamp(),
                timezoneOffset = TimeUtils.GetTimeZoneOffset(),
                actionRewardId = actionRewardId
            };

            BagelCodeHTTP.MakeApiCall("/v0/action_reward/redeem", null, request, ActionRewardRedeemResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void LuckyFiveResult(int eventID, Action<LuckyFiveInfoResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            LuckyFiveInfoRequest request = new LuckyFiveInfoRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                eventId = eventID
            };

            BagelCodeHTTP.MakeApiCall("/v0/lucky_five/info", null, request, LuckyFiveInfoResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void LuckyFiveWinList(int eventID, Action<LuckyFiveWinListResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            LuckyFiveWinListRequest request = new LuckyFiveWinListRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                eventId = eventID
            };

            BagelCodeHTTP.MakeApiCall("/v0/lucky_five/win_list", null, request, LuckyFiveWinListResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void LuckyFiveClaim(int eventID, Action<LuckyFiveClaimResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            LuckyFiveClaimRequest request = new LuckyFiveClaimRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                eventId = eventID
            };

            BagelCodeHTTP.MakeApiCall("/v0/lucky_five/claim", null, request, LuckyFiveClaimResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void InboxCollectAllRequest(int maxInboxId, Action<InboxAcceptAllResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            InboxAcceptAllRequest request = new InboxAcceptAllRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                currentTime = TimeUtils.GetTimeStamp(),
                timezoneOffset = TimeUtils.GetTimeZoneOffset(),
                maxInboxId = maxInboxId
            };

            BagelCodeHTTP.MakeApiCall("/v0/inbox/accept/all", null, request, InboxAcceptAllResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void ClubFeedCollectAllRequest(ClubFeedFilterType requestFileterType, long maxFeedId, Action<ClubFeedClaimAllResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ClubFeedClaimAllRequest request = new ClubFeedClaimAllRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                maxFeedId = maxFeedId,
                filterType = requestFileterType
            };

            BagelCodeHTTP.MakeApiCall("/v0/club/feed/like/all", null, request, ClubFeedClaimAllResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void SystemOptionsUpdateRequest(bool pushNotification, bool kudoJackpot, bool kudoTournament, bool globalChatNotification, bool kudoNewUserWelcome, bool enablePipMode, Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SystemOptionsUpdateRequest request = new SystemOptionsUpdateRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                pushNotification = pushNotification,
                kudoJackpot = kudoJackpot,
                kudoTournament = kudoTournament,
                globalChat = globalChatNotification,
                kudoNewUserWelcome = kudoNewUserWelcome,
                enablePipMode = enablePipMode,
            };

            BagelCodeHTTP.MakeApiCall("/v0/system/options/update", null, request, SimpleResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void VipDealSelectComplete(int vipDealInfoID, Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            VipDealEnterRequest request = new VipDealEnterRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                vipDealInfoId = vipDealInfoID
            };

            BagelCodeHTTP.MakeApiCall("/v0/vip_deal/enter", null, request, SimpleResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void VipDealRedeem(string vipDealUUID, int vipDealInfoID, Action<PurchaseRedeemVipDealResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            PurchaseRedeemVipDealRequest request = new PurchaseRedeemVipDealRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                dealUuid = vipDealUUID,
                vipDealInfoId = vipDealInfoID
            };

            BagelCodeHTTP.MakeApiCall("/v0/purchase/redeem_vip_deal", null, request, PurchaseRedeemVipDealResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void VipFreeDealSelectComplete(int vipDealInfoID, Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            VipDealCommonRequestV2 request = new VipDealCommonRequestV2
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                vipDealInfoId = vipDealInfoID
            };

            BagelCodeHTTP.MakeApiCall("/v2/vip_deal/enter", null, request, SimpleResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void VipPayDealSelectComplete(int vipDealInfoID, Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            VipDealCommonRequestV2 request = new VipDealCommonRequestV2
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                vipDealInfoId = vipDealInfoID
            };

            BagelCodeHTTP.MakeApiCall("/v2/vip_deal/update_paid_viewed", null, request, SimpleResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void VipDealV2FreebieRedeem(int vipDealInfoID, Action<VipDealFreebieRedeemResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            VipDealCommonRequestV2 request = new VipDealCommonRequestV2
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                vipDealInfoId = vipDealInfoID
            };

            BagelCodeHTTP.MakeApiCall("/v2/vip_deal/redeem_freebie", null, request, VipDealFreebieRedeemResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void POGBRedeem(int productID, int targetID, int eventID, Action<PurchaseRedeemFreePogBoosterResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            PurchaseRedeemFreePogBoosterRequest request = new PurchaseRedeemFreePogBoosterRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                productId = productID,
                targetPurchaseId = targetID,
                freePogBoosterEventId = eventID
            };

            BagelCodeHTTP.MakeApiCall("/v0/purchase/redeem_free_pog_booster", null, request, PurchaseRedeemFreePogBoosterResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void InAppMessageInfoRequest(int iamId, Action<InAppMessageInfoResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            InAppMessageInfoRequest request = new InAppMessageInfoRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                iamId = iamId
            };

            BagelCodeHTTP.MakeApiCall("/v0/in_app_message/info", null, request, InAppMessageInfoResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void CollectingGameInfoRequest(int eventId, Action<CollectingGameInfoResponseV1> responseCallback, HTTPErrorCallback errorCallback)
        {
            CollectingGameInfoRequest request = new CollectingGameInfoRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                eventId = eventId
            };

            BagelCodeHTTP.MakeApiCall("/v1/collecting_game/info", null, request, CollectingGameInfoResponseV1.Deserialize, responseCallback, errorCallback);
        }

        public static void CollectingGamePackFreeRequest(int eventId, Action<CollectingGamePackFreeResponseV1> responseCallback, HTTPErrorCallback errorCallback)
        {
            CollectingGamePackFreeRequest request = new CollectingGamePackFreeRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                eventId = eventId,
            };

            BagelCodeHTTP.MakeApiCall("/v1/collecting_game/pack/free", null, request, CollectingGamePackFreeResponseV1.Deserialize, responseCallback, errorCallback);
        }

        public static void CollectingGamePackVideoAdsFreeRequest(int eventId, Action<CollectingGamePackFreeVideoAdsResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            CollectingGamePackFreeVideoAdsRequest request = new CollectingGamePackFreeVideoAdsRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                eventId = eventId,
            };

            BagelCodeHTTP.MakeApiCall("/v1/collecting_game/pack/video_acls_free", null, request, CollectingGamePackFreeVideoAdsResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void CollectingGamePackOpenRequest(int eventId, int packId, Action<CollectingGamePackOpenResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            CollectingGamePackOpenRequest request = new CollectingGamePackOpenRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                eventId = eventId,
                packId = packId
            };

            BagelCodeHTTP.MakeApiCall("/v0/collecting_game/pack/open", null, request, CollectingGamePackOpenResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void CollectingGamePackOpenAllRequest(int eventId, Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            CollectingGameInfoRequest request = new CollectingGameInfoRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                eventId = eventId
            };

            BagelCodeHTTP.MakeApiCall("/vd/collecting_game/pack/open/all", null, request, SimpleResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void CollectingGameScratcherRedeemRequest(int eventId, int scratcherId, Action<CollectingGameScratcherRedeemResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            CollectingGameScratcherRedeemRequest request = new CollectingGameScratcherRedeemRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                eventId = eventId,
                scratcherId = scratcherId
            };

            BagelCodeHTTP.MakeApiCall("/v0/collecting_game/scratcher/redeem", null, request, CollectingGameScratcherRedeemResponse.Deserialize, responseCallback, errorCallback);
        }

        // public static void CollectingGameConvertRequest(int eventId, List<CollectingGameConvertInfo> convertPieceList, Action<CollectingGameConvertResponse> responseCallback, HTTPErrorCallback errorCallback)
        // {
        // Legacy
        // CollectingGameConvertRequest request = new CollectingGameConvertRequest
        // {
        //     blockseq = BagelCodeHTTP.FetchBlockSeq(),
        //     ackMask = BagelCodeHTTP.GenerateAckBits(),
        //     eventId = eventId,
        //     convertPieceList = convertPieceList
        // };

        // BagelCodeHTTP.MakeApiCall("/v0/collecting_game/convert", request, CollectingGameConvertResponse.Deserialize, responseCallback, errorCallback);
        // }

        static string FindProperty(Dictionary<string, object> dict, string key, string nullValue = "")
        {
            object value;
            if (dict.TryGetValue(key, out value))
                return (value != null) ? value.ToString() : nullValue;
            return nullValue;
        }

        public static void RequestCampaignList(Action<CampaignListResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits()
            };

            BagelCodeHTTP.MakeApiCall("/v0/main/campaign/list", null, request, CampaignListResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestUnlockGameByGem(int gameID, string contextID, Action<GemUnlockGameResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            GemUnlockGameRequest request = new GemUnlockGameRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                gameId = gameID,
                contextId = contextID,
            };

            BagelCodeHTTP.MakeApiCall("/v0/gem/unlock/game", null, request, GemUnlockGameResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestTutorialUpdate(int stage, long count, Action<TutorialUpdateResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            TutorialUpdateRequest request = new TutorialUpdateRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                stage = stage,
                count = count
            };

            BagelCodeHTTP.MakeApiCall("/v0/tutorial/update", null, request, TutorialUpdateResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestClubShareItemList(long clubID, int mgEventID, Action<ClubShareInfoResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ClubShareInfoRequest request = new ClubShareInfoRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                clubId = clubID,
                metaGameEventId = mgEventID
            };

            BagelCodeHTTP.MakeApiCall("/v0/club/share/info", null, request, ClubShareInfoResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestClubShareItem(long clubID, int mgEventID, int requestItemID, string requestType, Action<ClubShareRequestResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ClubShareRequestRequest request = new ClubShareRequestRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                clubId = clubID,
                metaGameEventId = mgEventID,
                pieceId = requestItemID,
                requestType = requestType
            };

            BagelCodeHTTP.MakeApiCall("/v0/club/share/request", null, request, ClubShareRequestResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void GiftClubFeed(long feedID, int mgEventID, Action<ClubFeedGiftResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ClubFeedGiftRequest request = new ClubFeedGiftRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                feedId = feedID,
                metaGameEventId = mgEventID
            };

            BagelCodeHTTP.MakeApiCall("/v0/club/feed/gift", null, request, ClubFeedGiftResponse.Deserialize, responseCallback, errorCallback);
        }



        public static void GetAzureADCollectionsToken(Action<PurchaseGetAzureAdCollectionsTokenResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits()
            };

            BagelCodeHTTP.MakeApiCall("/v0/purchase/get_azure_acl_collections_token", null, request, PurchaseGetAzureAdCollectionsTokenResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestEpicAlbumEnter(Action<EpicAlbumEnterResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits()
            };

            BagelCodeHTTP.MakeApiCall("/v0/epic_album/enter", null, request, EpicAlbumEnterResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestEpicAlbumCollectReward(int epicAlbumCategory, int epicAlbumRewardStage, Action<EpicAlbumCollectRewardResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            EpicAlbumCollectRewardRequest request = new EpicAlbumCollectRewardRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                category = epicAlbumCategory,
                rewardStage = epicAlbumRewardStage
            };

            BagelCodeHTTP.MakeApiCall("/v0/epic_album/collect_reward", null, request, EpicAlbumCollectRewardResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void UploadImage(byte[] reqImageBytes, Action<UploadImageResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            UploadImageRequest request = new UploadImageRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                screenshot = reqImageBytes
            };

            BagelCodeHTTP.MakeApiCall("/v0/main/upload_image", null, request, UploadImageResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestSubscribeChat(string channelID, Action<ChatSimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ChatSubscribeRequest request = new ChatSubscribeRequest
            {
                channelId = channelID
            };

            BagelCodeHTTP.MakeApiChatCall("/v0/chat/subscribe", BlackboardQueryUtils.GetChatRequestHeaders(),
                request, ChatSimpleResponse.Deserialize, responseCallback, errorCallback, false);
        }

        public static void RequestUnsubscribeChat(string channelID, Action<ChatSimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ChatUnsubscribeRequest request = new ChatUnsubscribeRequest
            {
                channelId = channelID
            };

            BagelCodeHTTP.MakeApiChatCall("/v0/chat/unsubscribe", BlackboardQueryUtils.GetChatRequestHeaders(),
                request, ChatSimpleResponse.Deserialize, responseCallback, errorCallback, false);
        }

        public static void PostChatMessageForChatServer(string channelID, ChatType chatType, long requestID, object chatPollData, string _contextId, Action<ChatPostMessageResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ChatPostMessageRequestV1 request = new ChatPostMessageRequestV1
            {
                channelId = channelID,
                type = chatType,
                data = chatPollData,
                contextId = _contextId,
                requestId = requestID
            };

            BagelCodeHTTP.MakeApiChatCall("/v1/chat/post_message", BlackboardQueryUtils.GetChatRequestHeaders(),
                request, ChatPostMessageResponse.Deserialize, responseCallback, errorCallback, false);
        }

        public static void PostChatMessageForLogicServer(string channelID, ChatType chatType, long requestID, object chatPollData, string _contextId, Action<GlobalChatPostMessageResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            GlobalChatPostMessageRequestV1 request = new GlobalChatPostMessageRequestV1
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                channelId = channelID,
                type = chatType,
                data = chatPollData,
                contextId = _contextId,
                requestId = requestID
            };
            BagelCodeHTTP.MakeApiCall("/v1/chat/global/post_message", BlackboardQueryUtils.GetChatRequestHeaders(),
                request, GlobalChatPostMessageResponse.Deserialize, responseCallback, errorCallback, false);
        }


        public static void GetRecentChatMessages(string channelID, long lastReceiveID, Action<ChatPollResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ChatRecentRequest request = new ChatRecentRequest
            {
                channelId = channelID,
                lastMessageId = lastReceiveID

            };

            BagelCodeHTTP.MakeApiChatCall("/v0/chat/recent", BlackboardQueryUtils.GetChatRequestHeaders(),
                request, ChatPollResponse.Deserialize, responseCallback, errorCallback, false);
        }

        public static void ChatPollRequest(long lastReceivedID, Action<ChatPollResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ChatPollRequest request = new ChatPollRequest
            {
                lastReceivedId = lastReceivedID
            };

            BagelCodeHTTP.ChatPoll("/v0/system/poll", BlackboardQueryUtils.GetChatRequestHeaders(),
                request, ChatPollResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void GetChatSubsribers(string channelID, Action<ChatSubscriberListResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ChatSubscriberListRequest request = new ChatSubscriberListRequest
            {
                channelId = channelID,
            };

            BagelCodeHTTP.MakeApiChatCall("/v0/chat/subscriber/list", BlackboardQueryUtils.GetChatRequestHeaders(),
                request, ChatSubscriberListResponse.Deserialize, responseCallback, errorCallback, false);
        }

        public static void BuySpeakerWithGem(int amount, string contextID, Action<GemBuySpeakerResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            GemBuySpeakerRequest request = new GemBuySpeakerRequest
            {
                speaker = amount,
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                contextId = contextID
            };

            BagelCodeHTTP.MakeApiCall("/v0/gem/buy/speaker", BlackboardQueryUtils.GetChatRequestHeaders(),
                request, GemBuySpeakerResponse.Deserialize, responseCallback, errorCallback, false);
        }

        public static void RequestChatMute(bool muteOnOff, string userId, Action<ChatMuteResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ChatMuteRequest request = new ChatMuteRequest
            {
                mute = muteOnOff,
                targetUserId = userId,
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
            };

            BagelCodeHTTP.MakeApiCall("/v0/chat/mute", BlackboardQueryUtils.GetChatRequestHeaders(),
                request, ChatMuteResponse.Deserialize, responseCallback, errorCallback, false);
        }

        public static void ReportChatMessage(ChatPoll chatPoll, Action<ChatReportResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ChatReportRequestV1 request = new ChatReportRequestV1
            {
                targetUserId = chatPoll.userId,
                type = chatPoll.type,
                data = chatPoll.data,
                channelId = chatPoll.channelId,
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits()
            };

            BagelCodeHTTP.MakeApiCall("/v1/chat/report", BlackboardQueryUtils.GetChatRequestHeaders(),
                request, ChatReportResponse.Deserialize, responseCallback, errorCallback, false);
        }

        public static void SeasonPassInfoRequest(int eventId, Action<SeasonPassInfoResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SeasonPassInfoRequest request = new SeasonPassInfoRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                eventId = eventId
            };

            BagelCodeHTTP.MakeApiCall("/v0/season_pass/info", null, request, SeasonPassInfoResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void SeasonPassInfoRequestV2(int eventId, Action<SeasonPassInfoResponseV2> responseCallback, HTTPErrorCallback errorCallback)
        {
            SeasonPassInfoRequest request = new SeasonPassInfoRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                eventId = eventId
            };

            BagelCodeHTTP.MakeApiCall("/v2/season_pass/info", null, request, SeasonPassInfoResponseV2.Deserialize, responseCallback, errorCallback);
        }

        public static void CollectSeasonPassReward(int eventId, int targetLevel, bool isPaid, Action<SeasonPassRewardClaimResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SeasonPassRewardClaimRequest request = new SeasonPassRewardClaimRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                eventId = eventId,
                level = targetLevel,
                isPaidReward = isPaid
            };

            BagelCodeHTTP.MakeApiCall("/v0/season_pass/reward/claim", null, request, SeasonPassRewardClaimResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void CollectSeasonPassRewardV2(int eventId, int targetLevel, bool isPaid, Action<SeasonPassRewardClaimResponseV2> responseCallback, HTTPErrorCallback errorCallback)
        {
            SeasonPassRewardClaimRequest request = new SeasonPassRewardClaimRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                eventId = eventId,
                level = targetLevel,
                isPaidReward = isPaid
            };

            BagelCodeHTTP.MakeApiCall("/v2/season_pass/reward/claim", null, request, SeasonPassRewardClaimResponseV2.Deserialize, responseCallback, errorCallback);
        }

        public static void CollectAllSeasonPassReward(int eventId, Action<SeasonPassRewardCollectAllResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SeasonPassRewardCollectAllRequest request = new SeasonPassRewardCollectAllRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                eventId = eventId
            };

            BagelCodeHTTP.MakeApiCall("/v0/season_pass/reward/collect/all", null, request, SeasonPassRewardCollectAllResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void CollectAllSeasonPassRewardV2(int eventId, Action<SeasonPassRewardCollectAllResponseV2> responseCallback, HTTPErrorCallback errorCallback)
        {
            SeasonPassRewardCollectAllRequest request = new SeasonPassRewardCollectAllRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                eventId = eventId
            };

            BagelCodeHTTP.MakeApiCall("/v2/season_pass/reward/collect/all", null, request, SeasonPassRewardCollectAllResponseV2.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestSeasonPassAdsView(int eventId, Action<SeasonPassAdsViewResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SeasonPassAdsViewRequest request = new SeasonPassAdsViewRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                eventId = eventId
            };

            BagelCodeHTTP.MakeApiCall("/v0/season_pass/acls/view", null, request, SeasonPassAdsViewResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestSeasonPassAdsViewV2(int eventId, Action<SeasonPassAdsViewResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SeasonPassAdsViewRequest request = new SeasonPassAdsViewRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                eventId = eventId
            };

            BagelCodeHTTP.MakeApiCall("/v2/season_pass/acls/view", null, request, SeasonPassAdsViewResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestSeasonPassAdsClaim(int eventId, Action<SeasonPassAdsClaimResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SeasonPassAdsClaimRequest request = new SeasonPassAdsClaimRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                eventId = eventId
            };

            BagelCodeHTTP.MakeApiCall("/v0/season_pass/acls/claim", null, request, SeasonPassAdsClaimResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestSeasonPassAdsClaimV2(int eventId, Action<SeasonPassAdsClaimResponseV2> responseCallback, HTTPErrorCallback errorCallback)
        {
            SeasonPassAdsClaimRequest request = new SeasonPassAdsClaimRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                eventId = eventId
            };

            BagelCodeHTTP.MakeApiCall("/v2/season_pass/acls/claim", null, request, SeasonPassAdsClaimResponseV2.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestMetaEnterGameInfo(int gameID, Action<MetaGameEnterInfoResponseV2> responseCallback, HTTPErrorCallback errorCallback)
        {
            MetaGameEnterInfoRequest request = new MetaGameEnterInfoRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                gameId = gameID
            };

            BagelCodeHTTP.MakeApiCall("/v2/main/meta_game/enter_info", null, request, MetaGameEnterInfoResponseV2.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestGemJackpotEnter(int eventId, Action<GemJackpotEnterResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            GemJackpotEnterRequest request = new GemJackpotEnterRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                eventId = eventId
            };

            BagelCodeHTTP.MakeApiCall("/v0/gem_jackpot/enter", null, request, GemJackpotEnterResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestGemJackpotVideoAdsClaim(int eventId, Action<GemJackpotVideoAdsClaimResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            GemJackpotVideoAdsClaimRequest request = new GemJackpotVideoAdsClaimRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                eventId = eventId
            };

            // #if UNITY_WEBGL && !UNITY_EDITOR
            // bypass adblock chrome extensions (they filter out video_ads/* path)
            // BagelCodeHTTP.MakeApiCall("/v0/gem_jackpot/video_@ds/claim", null, request, GemJackpotVideoAdsClaimResponse.Deserialize, responseCallback, errorCallback);
            // #else
            BagelCodeHTTP.MakeApiCall("/v0/gem_jackpot/video_acls/claim", null, request, GemJackpotVideoAdsClaimResponse.Deserialize, responseCallback, errorCallback);
            // #endif
        }

        public static void RequestGemJackpotSpin(string _slotEnterContextID, Action<GemJackpotSpinResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            GemJackpotSpinRequest request = new GemJackpotSpinRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                slotEnterContextId = _slotEnterContextID
            };

            BagelCodeHTTP.MakeApiCall("/v0/gem_jackpot/spin", null, request, GemJackpotSpinResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestGemJackpotDebugSpin(int index, string _slotEnterContextID, Action<GemJackpotSpinResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            GemJackpotDebugSpinRequest request = new GemJackpotDebugSpinRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                debugSpinIndex = (GemJackpotDebugSpinType)index,
                slotEnterContextId = _slotEnterContextID
            };
            BagelCodeHTTP.MakeApiCall("/vd/gem_jackpot/spin", null, request, GemJackpotSpinResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestGemJackpotCollectReward(Action<GemJackpotCollectRewardResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits()
            };
            BagelCodeHTTP.MakeApiCall("/v0/gem_jackpot/collect_reward", null, request, GemJackpotCollectRewardResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestBehaviorEventTrigger(string behaviorKey, Action<BehaviorEventTriggerResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            BehaviorEventTriggerRequest request = new BehaviorEventTriggerRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                behaviorKey = behaviorKey
            };

            BagelCodeHTTP.MakeApiCall("/v0/behavior_event/trigger", null, request, BehaviorEventTriggerResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestSeasonPassReset(int eventId, Action<SeasonPassResetResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SeasonPassResetRequest request = new SeasonPassResetRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                eventId = eventId
            };

            BagelCodeHTTP.MakeApiCall("/v0/season_pass/reset", null, request, SeasonPassResetResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestSeasonPassResetV2(int eventId, Action<SeasonPassResetResponseV2> responseCallback, HTTPErrorCallback errorCallback)
        {
            SeasonPassResetRequest request = new SeasonPassResetRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                eventId = eventId
            };

            BagelCodeHTTP.MakeApiCall("/v2/season_pass/reset", null, request, SeasonPassResetResponseV2.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestSeasonPassEnterInfoV2(int gameID, Action<SeasonPassEnterInfoResponseV2> responseCallback, HTTPErrorCallback errorCallback)
        {
            SeasonPassEnterInfoRequestV2 request = new SeasonPassEnterInfoRequestV2
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                gameId = gameID
            };

            BagelCodeHTTP.MakeApiCall("/v2/season_pass/enter_info", null, request, SeasonPassEnterInfoResponseV2.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestBossRaidersEnter(int eventId, Action<BossRaidersEnterResponseV1> responseCallback, HTTPErrorCallback errorCallback)
        {
            BossRaidersEnterRequest request = new BossRaidersEnterRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                eventId = eventId
            };

            BagelCodeHTTP.MakeApiCall("/v1/boss_raiders/enter", null, request, BossRaidersEnterResponseV1.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestBossRaidersSpin(int eventId, long multiplyNumerator, bool isAuto, Action<BossRaidersSpinResponseV1> responseCallback, HTTPErrorCallback errorCallback)
        {
            BossRaidersSpinRequest request = new BossRaidersSpinRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                eventId = eventId,
                maxBetMultiplyNumerator = multiplyNumerator,
                isAutoSpin = isAuto
            };

            BagelCodeHTTP.MakeApiCall("/v1/boss_raiders/spin", null, request, BossRaidersSpinResponseV1.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestBossRaidersDebugSpin(int eventId, long multiplyNumerator, BossRaidersDebugSpinType debugSpinType, bool isAuto, Action<BossRaidersSpinResponseV1> responseCallback, HTTPErrorCallback errorCallback)
        {
            BossRaidersDebugSpinRequest request = new BossRaidersDebugSpinRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                eventId = eventId,
                maxBetMultiplyNumerator = multiplyNumerator,
                debugSpinIndex = debugSpinType,
                isAutoSpin = isAuto
            };

            BagelCodeHTTP.MakeApiCall("/vd1/boss_raiders/debug/spin", null, request, BossRaidersSpinResponseV1.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestBossRaidersVideoAdsClaim(int eventId, string contextId, Action<BossRaidersAdsClaimResponseV1> responseCallback, HTTPErrorCallback errorCallback)
        {
            BossRaidersAdsClaimRequest request = new BossRaidersAdsClaimRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                eventId = eventId,
                contextId = contextId
            };

            BagelCodeHTTP.MakeApiCall("/v1/boss_raiders/video_acls/claim", null, request, BossRaidersAdsClaimResponseV1.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestBossRaidersInfo(int eventId, Action<BossRaidersInfoResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            BossRaidersInfoRequest request = new BossRaidersInfoRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                eventId = eventId
            };

            BagelCodeHTTP.MakeApiCall("/v0/boss_raiders/info", null, request, BossRaidersInfoResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestInviterUser(string contextID, string userID, InviteInstallType type, Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            InviteInstallRequest request = new InviteInstallRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                contextId = contextID,
                inviterUserId = userID,
                inviteType = type
            };

            BagelCodeHTTP.MakeApiCall("/v0/invite_install/add", null, request, SimpleResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestVIPInviteInstallUser(string contextID, string userID, int inviteInstallWithTierId, InviteInstallType type, Action<InviteInstallWithTierResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            InviteInstallWithTierRequest request = new InviteInstallWithTierRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                contextId = contextID,
                inviterUserId = userID,
                inviteType = type,
                inviteInstallWithTierId = inviteInstallWithTierId
            };

            BagelCodeHTTP.MakeApiCall("/v0/invite_install/add_with_tier", null, request, InviteInstallWithTierResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestClubLeaderPush(int idx, string contextId, Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ClubLeaderPushRequest request = new ClubLeaderPushRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                messageIndex = idx,
                contextId = contextId
            };

            BagelCodeHTTP.MakeApiCall("/v0/club/leader_push/send", null, request, SimpleResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestClubArenaEnter(int eventId, bool isFirstEnter, string targetUserId, string contextId, Action<ClubArenaEnterResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ClubArenaEnterRequest request = new ClubArenaEnterRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                eventId = eventId,
                isFirstEnterAfterLogin = isFirstEnter,
                targetOpponentUserId = targetUserId,
                enterContextId = contextId
            };

            BagelCodeHTTP.MakeApiCall("/v0/club_arena/enter", null, request, ClubArenaEnterResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestClubArenaSpin(int eventId, long multiplyNumerator, bool isAuto, long myShield, string _matchContextId, Action<ClubArenaSpinResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ClubArenaSpinRequest request = new ClubArenaSpinRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                eventId = eventId,
                betMultiplyNumerator = multiplyNumerator,
                isAutoSpin = isAuto,
                clientShield = myShield,
                matchContextId = _matchContextId
            };

            BagelCodeHTTP.MakeApiCall("/v0/club_arena/spin", null, request, ClubArenaSpinResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestClubArenaDebugSpin(int eventId, long multiplyNumerator, ClubArenaDebugSpinResultType debugSpinType, bool isAuto, long myShield, string _matchContextId, Action<ClubArenaSpinResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ClubArenaDebugSpinRequest request = new ClubArenaDebugSpinRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                eventId = eventId,
                betMultiplyNumerator = multiplyNumerator,
                isAutoSpin = isAuto,
                debugSpinResult = debugSpinType,
                clientShield = myShield,
                matchContextId = _matchContextId
            };

            BagelCodeHTTP.MakeApiCall("/vd/club_arena/debug/spin", null, request, ClubArenaSpinResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestClubArenaVideoAdsClaim(int eventId, string contextId, Action<ClubArenaAdsClaimResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ClubArenaAdsClaimRequest request = new ClubArenaAdsClaimRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                eventId = eventId,
                contextId = contextId
            };

            BagelCodeHTTP.MakeApiCall("/v0/club_arena/claim_video_acls", null, request, ClubArenaAdsClaimResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestClubArenaRevenge(int eventId, string opponentId, long _takenTime, string contextId, string _revengeContextId, Action<ClubArenaRevengeResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ClubArenaRevengeRequest request = new ClubArenaRevengeRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                eventId = eventId,
                opponentUserId = opponentId,
                takenTime = _takenTime,
                enterContextId = contextId,
                revengeContextId = _revengeContextId
            };

            BagelCodeHTTP.MakeApiCall("/v0/club_arena/revenge", null, request, ClubArenaRevengeResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestClubArenaAttack(int eventId, bool _isSteal, long _attack, long _multiplier, string opponentId, long opponentPoint, string contextId, string _matchContextId, bool isAuto, Action<ClubArenaAttackResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ClubArenaAttackRequest request = new ClubArenaAttackRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                eventId = eventId,
                isSteal = _isSteal,
                attack = _attack,
                multiplier = _multiplier,
                opponentUserId = opponentId,
                clientOpponentPoint = opponentPoint,
                enterContextId = contextId,
                matchContextId = _matchContextId,
                isAutoSpin = isAuto
            };

            BagelCodeHTTP.MakeApiCall("/v0/club_arena/attack", null, request, ClubArenaAttackResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestClubArenaHelp(int eventId, string userId, Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            ClubArenaHelpRequest request = new ClubArenaHelpRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                eventId = eventId,
                mostTakenUserId = userId
            };

            BagelCodeHTTP.MakeApiCall("/v0/club_arena/help", null, request, SimpleResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestControlSpinDeal(BetSpinIndex spinIndex, int spinDealId, string contextId, Action<SpinDealUseResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SpinDealUseRequest request = new SpinDealUseRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                spinDealId = spinDealId,
                betSpinIndex = spinIndex,
                contextId = contextId,
            };

            BagelCodeHTTP.MakeApiCall("/v0/spin_deal/use", null, request, SpinDealUseResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestControlSpinDeal2(int spinIndex, int spinDealId, string contextId, Action<SpinDealUseV2Response> responseCallback, HTTPErrorCallback errorCallback)
        {
            SpinDealUseV2Request request = new SpinDealUseV2Request
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                spinDealId = spinDealId,
                betSpinIndex = spinIndex,
                contextId = contextId,
            };

            BagelCodeHTTP.MakeApiCall("/v1/spin_deal/use", null, request, SpinDealUseV2Response.Deserialize, responseCallback, errorCallback);
        }

        #region Hidden Objects

        public static void RequestHiddenObjectsEnter(Action<HiddenUniverseEnterResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
            };

            BagelCodeHTTP.MakeApiCall("/v0/hidden_universe/enter", null, request, HiddenUniverseEnterResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestHiddenObjectsPlay(int chapter, int stage, string type, Action<HiddenUniversePlayEnterResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
#if DEV
            bool allObject = PlayerPrefs.GetInt(HiddenObjects.HiddenObjects.Defines.PLAYER_PREFS_SHOW_ALL, 0) == 1;
            HiddenUniverseDebugPlayEnterRequest request = new HiddenUniverseDebugPlayEnterRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                chapter = chapter,
                stage = stage,
                type = type,
                allObject = allObject,
                ignoreLock = true,
            };

            BagelCodeHTTP.MakeApiCall("/vd/hidden_universe/play/enter", null, request, HiddenUniversePlayEnterResponse.Deserialize, responseCallback, errorCallback);
#else

            HiddenUniversePlayEnterRequest request = new HiddenUniversePlayEnterRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                chapter = chapter,
                stage = stage,
                type = type,
            };

            BagelCodeHTTP.MakeApiCall("/v0/hidden_universe/play/enter", null, request, HiddenUniversePlayEnterResponse.Deserialize, responseCallback, errorCallback);
#endif
        }

        public static void RequestHiddenObjectsClear(int chapter, int stage, string token, int pauseCount, List<HiddenUniversePlayLog> playLog, Action<HiddenUniversePlayClearResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
#if DEV
            bool allObject = PlayerPrefs.GetInt(HiddenObjects.HiddenObjects.Defines.PLAYER_PREFS_SHOW_ALL, 0) == 1;
            HiddenUniverseDebugPlayClearRequest request = new HiddenUniverseDebugPlayClearRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                chapter = chapter,
                stage = stage,
                token = token,
                playLog = playLog,
                pauseCount = pauseCount,
                allObject = allObject,
                ignoreLock = true,
            };

            BagelCodeHTTP.MakeApiCall("/vd/hidden_universe/play/clear", null, request, HiddenUniversePlayClearResponse.Deserialize, responseCallback, errorCallback);
#else
            HiddenUniversePlayClearRequest request = new HiddenUniversePlayClearRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                chapter = chapter,
                stage = stage,
                token = token,
                playLog = playLog,
                pauseCount = pauseCount,
            };

            BagelCodeHTTP.MakeApiCall("/v0/hidden_universe/play/clear", null, request, HiddenUniversePlayClearResponse.Deserialize, responseCallback, errorCallback);
#endif
        }

        public static void RequestHiddenObjectsVideoAdsClaim(string contextId, Action<HiddenUniverseVideoAdsClaimResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            HiddenUniverseVideoAdsClaimRequest request = new HiddenUniverseVideoAdsClaimRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                contextId = contextId,
            };

            BagelCodeHTTP.MakeApiCall("/v0/hidden_universe/video_acls/claim", null, request, HiddenUniverseVideoAdsClaimResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestHiddenObjectsFinderRequest(long clubId, Action<HiddenUniverseRequestFinderResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            HiddenUniverseRequestFinderRequest request = new HiddenUniverseRequestFinderRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                clubId = clubId,
            };

            BagelCodeHTTP.MakeApiCall("/v0/hidden_universe/request_finder", null, request, HiddenUniverseRequestFinderResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestHiddenObjectsFinderGift(long clubId, long clubFeedId, Action<HiddenUniverseGiftFinderResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            HiddenUniverseGiftFinderRequest request = new HiddenUniverseGiftFinderRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                clubId = clubId,
                clubFeedId = clubFeedId,
            };

            BagelCodeHTTP.MakeApiCall("/v0/hidden_universe/gift_finder", null, request, HiddenUniverseGiftFinderResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestHiddenObjectsLeaveGame(int chapter, int stage, Action<HiddenUniversePlayLeaveResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            HiddenUniversePlayLeaveRequest request = new HiddenUniversePlayLeaveRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                chapter = chapter,
                stage = stage,
            };

            BagelCodeHTTP.MakeApiCall("/v0/hidden_universe/play/leave", null, request, HiddenUniversePlayLeaveResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestHiddenObjectsCollectChapterReward(int chapter, Action<HiddenUniverseClaimChapterRewardResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            HiddenUniverseClaimChapterRewardRequest request = new HiddenUniverseClaimChapterRewardRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                chapter = chapter,
            };

            BagelCodeHTTP.MakeApiCall("/v0/hidden_universe/claim_chapter_reward", null, request, HiddenUniverseClaimChapterRewardResponse.Deserialize, responseCallback, errorCallback);
        }

        #endregion

        public static void RequestHogDealCollect(List<HogDealPlayLog> playLog, string contextId, Action<HogDealCollectResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            HogDealCollectRequest request = new HogDealCollectRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                playLog = playLog,
                contextId = contextId,
            };

            BagelCodeHTTP.MakeApiCall("/v0/hog_deal/collect", null, request, HogDealCollectResponse.Deserialize, responseCallback, errorCallback);
        }
        #region Vip Lounge
        public static void RequestVipLoungeEnter(Action<VipLoungeEnterResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
            };

            BagelCodeHTTP.MakeApiCall("/v0/vip_lounge/enter", null, request, VipLoungeEnterResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestVipLoungeJackpotSpin(string contextID, Action<LoungeJackpotSpinResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            LoungeJackpotSpinRequest request = new LoungeJackpotSpinRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                contextId = contextID,
            };

            BagelCodeHTTP.MakeApiCall("/v0/vip_lounge/lounge_jackpot/spin", null, request, LoungeJackpotSpinResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestVipLoungeJackpotDebugSpin(LoungeJackpotWinType winType, string contextID, Action<LoungeJackpotSpinResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            LoungeJackpotDebugSpinRequest request = new LoungeJackpotDebugSpinRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                debugWinType = winType,
                contextId = contextID,
            };

            BagelCodeHTTP.MakeApiCall("/vd/vip_lounge/lounge_jackpot/debug/spin", null, request, LoungeJackpotSpinResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestVipLoungeJackpotCollect(Action<LoungeJackpotCollectResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
            };

            BagelCodeHTTP.MakeApiCall("/v0/vip_lounge/lounge_jackpot/collect", null, request, LoungeJackpotCollectResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestVipLoungeJackpotInfo(Action<LoungeJackpotInfoResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
            };

            BagelCodeHTTP.MakeApiCall("/v0/vip_lounge/lounge_jackpot/info", null, request, LoungeJackpotInfoResponse.Deserialize, responseCallback, errorCallback);
        }

        #endregion

        #region Vegas Dreams
        public static void RequestVegasDreamInfo(Action<BuildDreamInfoResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
            };

            BagelCodeHTTP.MakeApiCall("/v0/build_dream/info", null, request, BuildDreamInfoResponse.Deserialize, responseCallback, errorCallback, false);
        }

        public static void RequestVegasDreamEnter(Action<BuildDreamEnterResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
            };

            BagelCodeHTTP.MakeApiCall("/v0/build_dream/enter", null, request, BuildDreamEnterResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestVegasDreamCollectFreeDepot(Action<BuildDreamCollectFreeDepotResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
            };

            BagelCodeHTTP.MakeApiCall("/v0/build_dream/collect_free_depot", null, request, BuildDreamCollectFreeDepotResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestVegasDreamOpenDepot(DepotType type, Action<BuildDreamOpenDepotResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            BuildDreamOpenDepotRequest request = new BuildDreamOpenDepotRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                depotType = type,
            };

            BagelCodeHTTP.MakeApiCall("/v0/build_dream/open_depot", null, request, BuildDreamOpenDepotResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestVegasDreamOpenDepotAll(Action<BuildDreamOpenDepotResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
            };

            BagelCodeHTTP.MakeApiCall("/v0/build_dream/open_depot/all", null, request, BuildDreamOpenDepotResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestVegasDreamCollectWildPuzzle(int targetBuildingIndex, Action<BuildDreamCollectWildPuzzleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            BuildDreamCollectWildPuzzleRequest request = new BuildDreamCollectWildPuzzleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                targetBuildingIndex = targetBuildingIndex,
            };

            BagelCodeHTTP.MakeApiCall("/v0/build_dream/collect_wild_puzzle", null, request, BuildDreamCollectWildPuzzleResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestVegasDreamCollectDailyChest(int buildingIndex, Action<BuildDreamCollectDailyChestResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            BuildDreamCollectDailyChestRequest request = new BuildDreamCollectDailyChestRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                buildingIndex = buildingIndex,
            };

            BagelCodeHTTP.MakeApiCall("/v0/build_dream/collect_daily_chest", null, request, BuildDreamCollectDailyChestResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestVegasDreamCollectAllDailyChest(Action<BuildDreamCollectDailyChestResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
            };

            BagelCodeHTTP.MakeApiCall("/v0/build_dream/collect_daily_chest/all", null, request, BuildDreamCollectDailyChestResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestVegasDreamGurusRankingList(Action<BuildDreamGurusRankListResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            BuildDreamGurusRankListRequest request = new BuildDreamGurusRankListRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                referenceUserId = BlackboardQueryUtils.GetMyUserId(),
                aboveCount = 3,
                belowCount = 3,
                matchTotalCount = true,
            };

            BagelCodeHTTP.MakeApiCall("/v0/build_dream/gurus_rank_list", null, request, BuildDreamGurusRankListResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestVegasDreamExhibition(int seasonId, Action<BuildDreamExhibitionResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            BuildDreamExhibitionRequest request = new BuildDreamExhibitionRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                seasonId = seasonId,
            };

            BagelCodeHTTP.MakeApiCall("/v0/build_dream/exhibition", null, request, BuildDreamExhibitionResponse.Deserialize, responseCallback, errorCallback);
        }
        #endregion

        public static void RequestBossRaidersDealEnter(string _dealUuid, Action<BossRaidersDealEnterResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            BossRaidersDealEnterRequest request = new BossRaidersDealEnterRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                dealUuid = _dealUuid,
            };

            BagelCodeHTTP.MakeApiCall("/v0/boss_raiders_deal/enter", null, request, BossRaidersDealEnterResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestBossRaidersDealSpin(string _dealUuid, bool isAuto, Action<BossRaidersDealSpinResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            BossRaidersDealSpinRequest request = new BossRaidersDealSpinRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                dealUuid = _dealUuid,
                isAutoSpin = isAuto,
            };

            BagelCodeHTTP.MakeApiCall("/v0/boss_raiders_deal/spin", null, request, BossRaidersDealSpinResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestBossRaidersDealDebugSpin(string _dealUuid, BossRaidersDebugSpinType debugSpinType, bool isAuto, Action<BossRaidersDealSpinResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            BossRaidersDealDebugSpinRequest request = new BossRaidersDealDebugSpinRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                dealUuid = _dealUuid,
                debugSpinIndex = debugSpinType,
                isAutoSpin = isAuto,
            };

            BagelCodeHTTP.MakeApiCall("/vd/boss_raiders_deal/debug/spin", null, request, BossRaidersDealSpinResponse.Deserialize, responseCallback, errorCallback);
        }

        #region Delete Account
        public static void RequestDeleteAccountSendVerificationCode(string email, Action<AccountRemovalSendVerificationCodeResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            AccountRemovalSendVerificationCodeRequest request = new AccountRemovalSendVerificationCodeRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                email = email,
            };

            BagelCodeHTTP.MakeApiCall("/v0/account_removal/send_verification_code", null, request, AccountRemovalSendVerificationCodeResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestDeleteAccountRequest(Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
            };

            BagelCodeHTTP.MakeApiCall("/v0/account_removal/request", null, request, SimpleResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestDeleteAccountWithdraw(string actionType, Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            AccountRemovalWithdrawRequest request = new AccountRemovalWithdrawRequest
            {
                actionType = actionType,
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
            };

            BagelCodeHTTP.MakeApiCall("/v0/account_removal/withdraw", null, request, SimpleResponse.Deserialize, responseCallback, errorCallback);
        }
        #endregion

        public static void RequestBucksRedeemGift(int giftID, Action<SimpleResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            BucksRedeemGiftRequest request = new BucksRedeemGiftRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                giftId = giftID,
            };
            BagelCodeHTTP.MakeApiCall("/v0/bucks/redeem_gift", null, request, SimpleResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestBucksAmount(Action<BucksAmountResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
            };
            BagelCodeHTTP.MakeApiCall("/v0/bucks/amount", null, request, BucksAmountResponse.Deserialize, responseCallback, errorCallback, false);
        }
        #region IAM Simulator
        public static void RequestPreviewIamSlbList(Action<SuperSimulatorListResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SimpleRequest request = new SimpleRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
            };

            BagelCodeHTTP.MakeApiCall("/v0/super_simulator/list", null, request, SuperSimulatorListResponse.Deserialize, responseCallback, errorCallback);
        }

        public static void RequestPreviewIamSlbInfo(int componentId, Action<SuperSimulatorInfoResponse> responseCallback, HTTPErrorCallback errorCallback)
        {
            SuperSimulatorInfoRequest request = new SuperSimulatorInfoRequest
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                componentId = componentId,
            };

            BagelCodeHTTP.MakeApiCall("/v0/super_simulator/info", null, request, SuperSimulatorInfoResponse.Deserialize, responseCallback, errorCallback);
        }
        #endregion
    }
}
