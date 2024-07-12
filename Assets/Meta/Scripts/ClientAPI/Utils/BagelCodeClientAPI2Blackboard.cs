using UnityEngine;
using NodeCanvas.Framework;
using System.Collections.Generic;
using BagelCode.ClientModels;
using SlotMaker;

namespace BagelCode
{
public static partial class ClientAPI2Blackboard
{
    public static void Serialize(IBlackboard bb, AccountRemovalInfo accountRemovalInfo)
    {
        if (accountRemovalInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "accountRemovalRequested", accountRemovalInfo.accountRemovalRequested);
        BlackboardUtils.SetOrCreateValue(bb, "removalRemainingSec", accountRemovalInfo.removalRemainingSec);
    }

    public static void Serialize(IBlackboard bb, AccountRemovalSendVerificationCodeRequest accountRemovalSendVerificationCodeRequest)
    {
        if (accountRemovalSendVerificationCodeRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", accountRemovalSendVerificationCodeRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", accountRemovalSendVerificationCodeRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "email", accountRemovalSendVerificationCodeRequest.email);
    }

    public static void Serialize(IBlackboard bb, AccountRemovalSendVerificationCodeResponse accountRemovalSendVerificationCodeResponse)
    {
        if (accountRemovalSendVerificationCodeResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", accountRemovalSendVerificationCodeResponse.error);
        if (accountRemovalSendVerificationCodeResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), accountRemovalSendVerificationCodeResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", accountRemovalSendVerificationCodeResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "verificationCode", accountRemovalSendVerificationCodeResponse.verificationCode);
    }

    public static void Serialize(IBlackboard bb, AccountRemovalWithdrawRequest accountRemovalWithdrawRequest)
    {
        if (accountRemovalWithdrawRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", accountRemovalWithdrawRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", accountRemovalWithdrawRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "actionType", accountRemovalWithdrawRequest.actionType);
    }

    public static void Serialize(IBlackboard bb, Action action)
    {
        if (action == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "type", action.type);
        if (action.data != null)
        {
            switch (action.type)
            {
            case ActionType.NONE:
                Serialize(bb, (ActionDataNone)action.data);
                break;

            case ActionType.OPEN_LINK:
                Serialize(bb, (ActionDataOpenLink)action.data);
                break;

            case ActionType.OPEN_POPUP:
                Serialize(bb, (ActionDataOpenPopup)action.data);
                break;

            case ActionType.FACEBOOK_CONNECT:
                Serialize(bb, (ActionDataFacebookConnect)action.data);
                break;

            case ActionType.FACEBOOK_INVITE:
                Serialize(bb, (ActionDataFacebookInvite)action.data);
                break;

            case ActionType.PURCHASE_PRODUCT:
                Serialize(bb, (ActionDataPurhcaseProduct)action.data);
                break;

            case ActionType.ENTER_GAME:
                Serialize(bb, (ActionDataEnterGame)action.data);
                break;

            case ActionType.ENTER_GAME_FAVORITE:
                Serialize(bb, (ActionDataEnterGameFavorite)action.data);
                break;

            case ActionType.OPEN_PROFILE_POPUP:
                Serialize(bb, (ActionDataOpenProfilePopup)action.data);
                break;

            case ActionType.OPEN_INVITE_POPUP:
                Serialize(bb, (ActionDataOpenInvitePopup)action.data);
                break;

            case ActionType.COUPON_REDEEM:
                Serialize(bb, (ActionDataCouponRedeem)action.data);
                break;

            case ActionType.TRIGGER_IAM:
                Serialize(bb, (ActionDataTriggerIam)action.data);
                break;

            case ActionType.CHANGE_SCENE:
                Serialize(bb, (ActionDataChangeScene)action.data);
                break;

            case ActionType.OPEN_NOTICE_ACTION_POPUP:
                Serialize(bb, (ActionDataOpenNoticeActionPopup)action.data);
                break;

            case ActionType.REGISTER_USER_GROUP:
                Serialize(bb, (ActionDataRegisterUserGroup)action.data);
                break;

            case ActionType.CLAIM_VIDEO_ADS_ONLY_IAM:
                Serialize(bb, (ActionDataClaimVideoAdsOnlyIam)action.data);
                break;

            case ActionType.USE_FCFS_TICKET:
                Serialize(bb, (ActionDataUseFcfsTicket)action.data);
                break;

            case ActionType.ENABLE_STATUS_MATCH:
                Serialize(bb, (ActionDataEnableStatusMatch)action.data);
                break;

            case ActionType.TIER_MATCH:
                Serialize(bb, (ActionDataTierMatch)action.data);
                break;

            case ActionType.SURVEY:
                Serialize(bb, (ActionDataSurvey)action.data);
                break;

            case ActionType.REDEEM_ACTION_REWARD:
                Serialize(bb, (ActionDataRedeemActionReward)action.data);
                break;

            case ActionType.PURCHASE_PRODUCT_WITH_GEM:
                Serialize(bb, (ActionDataPurchaseProductWithGem)action.data);
                break;

            case ActionType.BUY_BONUS:
                Serialize(bb, (ActionDataBuyBonus)action.data);
                break;

            case ActionType.PUSH_OFF:
                Serialize(bb, (ActionDataPushOff)action.data);
                break;

            case ActionType.OPEN_APPLICATION_SETTINGS:
                Serialize(bb, (ActionDataOpenApplicationSettings)action.data);
                break;

            case ActionType.OPEN_IDFA_ALLOW_POPUP:
                Serialize(bb, (ActionDataOpenIdfaAllowPopup)action.data);
                break;

            case ActionType.SNS_INVITE:
                Serialize(bb, (ActionDataSnsInvite)action.data);
                break;

            case ActionType.FB_MESSAGE_INVITE:
                Serialize(bb, (ActionDataFbMessageInvite)action.data);
                break;

            case ActionType.PIN_TO_TASKBAR:
                Serialize(bb, (ActionDataPinToTaskbar)action.data);
                break;

            case ActionType.VIP_SURVEY:
                Serialize(bb, (ActionDataVipSurvey)action.data);
                break;

            case ActionType.SNS_INVITE_WITH_TIER:
                Serialize(bb, (ActionDataSnsInviteWithTier)action.data);
                break;
            }
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "data");
        }
    }

    public static void Serialize(IBlackboard bb, ActionDataBuyBonus actionDataBuyBonus)
    {
        if (actionDataBuyBonus == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", actionDataBuyBonus.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "ticketedBonusId", actionDataBuyBonus.ticketedBonusId);
        BlackboardUtils.SetOrCreateValue(bb, "bet", actionDataBuyBonus.bet);
        BlackboardUtils.SetOrCreateValue(bb, "extraBet", actionDataBuyBonus.extraBet);
    }

    public static void Serialize(IBlackboard bb, ActionDataChangeScene actionDataChangeScene)
    {
        if (actionDataChangeScene == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "sceneType", actionDataChangeScene.sceneType);
    }

    public static void Serialize(IBlackboard bb, ActionDataClaimVideoAdsOnlyIam actionDataClaimVideoAdsOnlyIam)
    {
        if (actionDataClaimVideoAdsOnlyIam == null) { return; }
    }

    public static void Serialize(IBlackboard bb, ActionDataCouponRedeem actionDataCouponRedeem)
    {
        if (actionDataCouponRedeem == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "code", actionDataCouponRedeem.code);
    }

    public static void Serialize(IBlackboard bb, ActionDataEnableStatusMatch actionDataEnableStatusMatch)
    {
        if (actionDataEnableStatusMatch == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "deeplinkId", actionDataEnableStatusMatch.deeplinkId);
    }

    public static void Serialize(IBlackboard bb, ActionDataEnterGame actionDataEnterGame)
    {
        if (actionDataEnterGame == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", actionDataEnterGame.gameId);
    }

    public static void Serialize(IBlackboard bb, ActionDataEnterGameFavorite actionDataEnterGameFavorite)
    {
        if (actionDataEnterGameFavorite == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", actionDataEnterGameFavorite.gameId);
    }

    public static void Serialize(IBlackboard bb, ActionDataFacebookConnect actionDataFacebookConnect)
    {
        if (actionDataFacebookConnect == null) { return; }
    }

    public static void Serialize(IBlackboard bb, ActionDataFacebookInvite actionDataFacebookInvite)
    {
        if (actionDataFacebookInvite == null) { return; }
    }

    public static void Serialize(IBlackboard bb, ActionDataFbMessageInvite actionDataFbMessageInvite)
    {
        if (actionDataFbMessageInvite == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "inviterUserId", actionDataFbMessageInvite.inviterUserId);
    }

    public static void Serialize(IBlackboard bb, ActionDataNone actionDataNone)
    {
        if (actionDataNone == null) { return; }
    }

    public static void Serialize(IBlackboard bb, ActionDataOpenApplicationSettings actionDataOpenApplicationSettings)
    {
        if (actionDataOpenApplicationSettings == null) { return; }
    }

    public static void Serialize(IBlackboard bb, ActionDataOpenIdfaAllowPopup actionDataOpenIdfaAllowPopup)
    {
        if (actionDataOpenIdfaAllowPopup == null) { return; }
    }

    public static void Serialize(IBlackboard bb, ActionDataOpenInvitePopup actionDataOpenInvitePopup)
    {
        if (actionDataOpenInvitePopup == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "userId", actionDataOpenInvitePopup.userId);
        BlackboardUtils.SetOrCreateValue(bb, "name", actionDataOpenInvitePopup.name);
        BlackboardUtils.SetOrCreateValue(bb, "profileUrl", actionDataOpenInvitePopup.profileUrl);
        BlackboardUtils.SetOrCreateValue(bb, "tier", actionDataOpenInvitePopup.tier);
        BlackboardUtils.SetOrCreateValue(bb, "gameId", actionDataOpenInvitePopup.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "roomId", actionDataOpenInvitePopup.roomId);
        BlackboardUtils.SetOrCreateValue(bb, "profileHighResolutionUrl", actionDataOpenInvitePopup.profileHighResolutionUrl);
    }

    public static void Serialize(IBlackboard bb, ActionDataOpenLink actionDataOpenLink)
    {
        if (actionDataOpenLink == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "url", actionDataOpenLink.url);
    }

    public static void Serialize(IBlackboard bb, ActionDataOpenNoticeActionPopup actionDataOpenNoticeActionPopup)
    {
        if (actionDataOpenNoticeActionPopup == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "id", actionDataOpenNoticeActionPopup.id);
    }

    public static void Serialize(IBlackboard bb, ActionDataOpenPopup actionDataOpenPopup)
    {
        if (actionDataOpenPopup == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "popupType", actionDataOpenPopup.popupType);
    }

    public static void Serialize(IBlackboard bb, ActionDataOpenProfilePopup actionDataOpenProfilePopup)
    {
        if (actionDataOpenProfilePopup == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "targetUserId", actionDataOpenProfilePopup.targetUserId);
    }

    public static void Serialize(IBlackboard bb, ActionDataPinToTaskbar actionDataPinToTaskbar)
    {
        if (actionDataPinToTaskbar == null) { return; }
    }

    public static void Serialize(IBlackboard bb, ActionDataPurchaseProductWithGem actionDataPurchaseProductWithGem)
    {
        if (actionDataPurchaseProductWithGem == null) { return; }
        if (actionDataPurchaseProductWithGem.product != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "product"), actionDataPurchaseProductWithGem.product);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "product");
        }
        BlackboardUtils.SetOrCreateValue(bb, "userGroupId", actionDataPurchaseProductWithGem.userGroupId);
    }

    public static void Serialize(IBlackboard bb, ActionDataPurhcaseProduct actionDataPurhcaseProduct)
    {
        if (actionDataPurhcaseProduct == null) { return; }
        if (actionDataPurhcaseProduct.product != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "product"), actionDataPurhcaseProduct.product);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "product");
        }
        BlackboardUtils.SetOrCreateValue(bb, "userGroupId", actionDataPurhcaseProduct.userGroupId);
    }

    public static void Serialize(IBlackboard bb, ActionDataPushOff actionDataPushOff)
    {
        if (actionDataPushOff == null) { return; }
    }

    public static void Serialize(IBlackboard bb, ActionDataRedeemActionReward actionDataRedeemActionReward)
    {
        if (actionDataRedeemActionReward == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "actionRewardId", actionDataRedeemActionReward.actionRewardId);
    }

    public static void Serialize(IBlackboard bb, ActionDataRegisterUserGroup actionDataRegisterUserGroup)
    {
        if (actionDataRegisterUserGroup == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "id", actionDataRegisterUserGroup.id);
    }

    public static void Serialize(IBlackboard bb, ActionDataSnsInvite actionDataSnsInvite)
    {
        if (actionDataSnsInvite == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "inviterUserId", actionDataSnsInvite.inviterUserId);
    }

    public static void Serialize(IBlackboard bb, ActionDataSnsInviteWithTier actionDataSnsInviteWithTier)
    {
        if (actionDataSnsInviteWithTier == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "inviterUserId", actionDataSnsInviteWithTier.inviterUserId);
        BlackboardUtils.SetOrCreateValue(bb, "inviteInstallWithTierId", actionDataSnsInviteWithTier.inviteInstallWithTierId);
    }

    public static void Serialize(IBlackboard bb, ActionDataSurvey actionDataSurvey)
    {
        if (actionDataSurvey == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "hash", actionDataSurvey.hash);
    }

    public static void Serialize(IBlackboard bb, ActionDataTierMatch actionDataTierMatch)
    {
        if (actionDataTierMatch == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "matchCode", actionDataTierMatch.matchCode);
    }

    public static void Serialize(IBlackboard bb, ActionDataTriggerIam actionDataTriggerIam)
    {
        if (actionDataTriggerIam == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "id", actionDataTriggerIam.id);
    }

    public static void Serialize(IBlackboard bb, ActionDataUseFcfsTicket actionDataUseFcfsTicket)
    {
        if (actionDataUseFcfsTicket == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "fcfsTicketId", actionDataUseFcfsTicket.fcfsTicketId);
    }

    public static void Serialize(IBlackboard bb, ActionDataVipSurvey actionDataVipSurvey)
    {
        if (actionDataVipSurvey == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "surveyUrl", actionDataVipSurvey.surveyUrl);
    }

    public static void Serialize(IBlackboard bb, ActionRewardRedeemRequest actionRewardRedeemRequest)
    {
        if (actionRewardRedeemRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", actionRewardRedeemRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", actionRewardRedeemRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "actionRewardId", actionRewardRedeemRequest.actionRewardId);
        BlackboardUtils.SetOrCreateValue(bb, "currentTime", actionRewardRedeemRequest.currentTime);
        BlackboardUtils.SetOrCreateValue(bb, "timezoneOffset", actionRewardRedeemRequest.timezoneOffset);
    }

    public static void Serialize(IBlackboard bb, ActionRewardRedeemResponse actionRewardRedeemResponse)
    {
        if (actionRewardRedeemResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", actionRewardRedeemResponse.error);
        if (actionRewardRedeemResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), actionRewardRedeemResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", actionRewardRedeemResponse.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "rewardResultList", actionRewardRedeemResponse.rewardResultList, Serialize);
        if (actionRewardRedeemResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), actionRewardRedeemResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
    }

    public static void Serialize(IBlackboard bb, AgeGateRequest ageGateRequest)
    {
        if (ageGateRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", ageGateRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "age", ageGateRequest.age);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", ageGateRequest.ackMask);
    }

    public static void Serialize(IBlackboard bb, AssetBundleBaseUrlRequest assetBundleBaseUrlRequest)
    {
        if (assetBundleBaseUrlRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", assetBundleBaseUrlRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", assetBundleBaseUrlRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "clientOs", assetBundleBaseUrlRequest.clientOs);
        BlackboardUtils.SetOrCreateValue(bb, "clientNumberVersion", assetBundleBaseUrlRequest.clientNumberVersion);
    }

    public static void Serialize(IBlackboard bb, AssetBundleBaseUrlResponse assetBundleBaseUrlResponse)
    {
        if (assetBundleBaseUrlResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", assetBundleBaseUrlResponse.error);
        if (assetBundleBaseUrlResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), assetBundleBaseUrlResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", assetBundleBaseUrlResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "baseUrl", assetBundleBaseUrlResponse.baseUrl);
    }

    public static void Serialize(IBlackboard bb, AssetBundleInfo assetBundleInfo)
    {
        if (assetBundleInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "baseUrl", assetBundleInfo.baseUrl);
        BlackboardUtils.SetOrCreateValue(bb, "downloadThreadCount", assetBundleInfo.downloadThreadCount);
        BlackboardUtils.SetOrCreateValue(bb, "dlcAssetList", assetBundleInfo.dlcAssetList);
    }

    public static void Serialize(IBlackboard bb, BbbRewardInfo bbbRewardInfo)
    {
        if (bbbRewardInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "credit", bbbRewardInfo.credit);
        BlackboardUtils.SetOrCreateValue(bb, "creditLoss", bbbRewardInfo.creditLoss);
        BlackboardUtils.SetOrCreateValue(bb, "triggerType", bbbRewardInfo.triggerType);
        BlackboardUtils.SetOrCreateValue(bb, "bbbId", bbbRewardInfo.bbbId);
    }

    public static void Serialize(IBlackboard bb, BehaviorEventTriggerRequest behaviorEventTriggerRequest)
    {
        if (behaviorEventTriggerRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", behaviorEventTriggerRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", behaviorEventTriggerRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "behaviorKey", behaviorEventTriggerRequest.behaviorKey);
    }

    public static void Serialize(IBlackboard bb, BehaviorEventTriggerResponse behaviorEventTriggerResponse)
    {
        if (behaviorEventTriggerResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", behaviorEventTriggerResponse.error);
        if (behaviorEventTriggerResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), behaviorEventTriggerResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", behaviorEventTriggerResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "needToReloadCampaign", behaviorEventTriggerResponse.needToReloadCampaign);
    }

    public static void Serialize(IBlackboard bb, BigwinRankEntry bigwinRankEntry)
    {
        if (bigwinRankEntry == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "name", bigwinRankEntry.name);
        BlackboardUtils.SetOrCreateValue(bb, "profileUrl", bigwinRankEntry.profileUrl);
        BlackboardUtils.SetOrCreateValue(bb, "tier", bigwinRankEntry.tier);
        BlackboardUtils.SetOrCreateValue(bb, "bigwinCredit", bigwinRankEntry.bigwinCredit);
    }

    public static void Serialize(IBlackboard bb, BigwinRankInfo bigwinRankInfo)
    {
        if (bigwinRankInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", bigwinRankInfo.gameId);
        BlackboardUtils.SetOrCreateList(bb, "rankList", bigwinRankInfo.rankList, Serialize);
    }

    public static void Serialize(IBlackboard bb, Bingo2DArray bingo2DArray)
    {
        if (bingo2DArray == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "row", bingo2DArray.row);
    }

    public static void Serialize(IBlackboard bb, BonusInfo bonusInfo)
    {
        if (bonusInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "isRow", bonusInfo.isRow);
        BlackboardUtils.SetOrCreateValue(bb, "xPos", bonusInfo.xPos);
        BlackboardUtils.SetOrCreateValue(bb, "yPos", bonusInfo.yPos);
        BlackboardUtils.SetOrCreateValue(bb, "prize", bonusInfo.prize);
    }

    public static void Serialize(IBlackboard bb, BonusSpinInfo bonusSpinInfo)
    {
        if (bonusSpinInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", bonusSpinInfo.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "spinCount", bonusSpinInfo.spinCount);
        BlackboardUtils.SetOrCreateValue(bb, "endTimestamp", bonusSpinInfo.endTimestamp);
    }

    public static void Serialize(IBlackboard bb, BossRaidersAdsClaimRequest bossRaidersAdsClaimRequest)
    {
        if (bossRaidersAdsClaimRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", bossRaidersAdsClaimRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", bossRaidersAdsClaimRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "eventId", bossRaidersAdsClaimRequest.eventId);
        BlackboardUtils.SetOrCreateValue(bb, "contextId", bossRaidersAdsClaimRequest.contextId);
    }

    public static void Serialize(IBlackboard bb, BossRaidersAdsClaimResponseV1 bossRaidersAdsClaimResponseV1)
    {
        if (bossRaidersAdsClaimResponseV1 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", bossRaidersAdsClaimResponseV1.error);
        if (bossRaidersAdsClaimResponseV1.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), bossRaidersAdsClaimResponseV1.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", bossRaidersAdsClaimResponseV1.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "bossRaidersCooltime", bossRaidersAdsClaimResponseV1.bossRaidersCooltime);
        BlackboardUtils.SetOrCreateValue(bb, "lastVideoAdsClaimTimestamp", bossRaidersAdsClaimResponseV1.lastVideoAdsClaimTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "energy", bossRaidersAdsClaimResponseV1.energy);
    }

    public static void Serialize(IBlackboard bb, BossRaidersAttackWheelCandidateInfo bossRaidersAttackWheelCandidateInfo)
    {
        if (bossRaidersAttackWheelCandidateInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "hitType", bossRaidersAttackWheelCandidateInfo.hitType);
        BlackboardUtils.SetOrCreateValue(bb, "baseAttackHp", bossRaidersAttackWheelCandidateInfo.baseAttackHp);
    }

    public static void Serialize(IBlackboard bb, BossRaidersAttackWheelResultInfo bossRaidersAttackWheelResultInfo)
    {
        if (bossRaidersAttackWheelResultInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "hitType", bossRaidersAttackWheelResultInfo.hitType);
        BlackboardUtils.SetOrCreateValue(bb, "hp", bossRaidersAttackWheelResultInfo.hp);
        BlackboardUtils.SetOrCreateValue(bb, "attack", bossRaidersAttackWheelResultInfo.attack);
        if (bossRaidersAttackWheelResultInfo.nextRoundBalanceInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "nextRoundBalanceInfo"), bossRaidersAttackWheelResultInfo.nextRoundBalanceInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "nextRoundBalanceInfo");
        }
        if (bossRaidersAttackWheelResultInfo.nextRoundBossInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "nextRoundBossInfo"), bossRaidersAttackWheelResultInfo.nextRoundBossInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "nextRoundBossInfo");
        }
    }

    public static void Serialize(IBlackboard bb, BossRaidersBonusInfo bossRaidersBonusInfo)
    {
        if (bossRaidersBonusInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "type", bossRaidersBonusInfo.type);
        BlackboardUtils.SetOrCreateValue(bb, "amount", bossRaidersBonusInfo.amount);
    }

    public static void Serialize(IBlackboard bb, BossRaidersBonusWheelCandidateInfo bossRaidersBonusWheelCandidateInfo)
    {
        if (bossRaidersBonusWheelCandidateInfo == null) { return; }
    }

    public static void Serialize(IBlackboard bb, BossRaidersBonusWheelResultInfo bossRaidersBonusWheelResultInfo)
    {
        if (bossRaidersBonusWheelResultInfo == null) { return; }
        BlackboardUtils.SetOrCreateList(bb, "bonusInfoList", bossRaidersBonusWheelResultInfo.bonusInfoList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "bonusIndex", bossRaidersBonusWheelResultInfo.bonusIndex);
    }

    public static void Serialize(IBlackboard bb, BossRaidersClubRankInfo bossRaidersClubRankInfo)
    {
        if (bossRaidersClubRankInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "clubId", bossRaidersClubRankInfo.clubId);
        BlackboardUtils.SetOrCreateValue(bb, "clubName", bossRaidersClubRankInfo.clubName);
        BlackboardUtils.SetOrCreateValue(bb, "clubSymbol", bossRaidersClubRankInfo.clubSymbol);
        BlackboardUtils.SetOrCreateValue(bb, "totalCompletedRound", bossRaidersClubRankInfo.totalCompletedRound);
        BlackboardUtils.SetOrCreateValue(bb, "rank", bossRaidersClubRankInfo.rank);
    }

    public static void Serialize(IBlackboard bb, BossRaidersContributionInfo bossRaidersContributionInfo)
    {
        if (bossRaidersContributionInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "userId", bossRaidersContributionInfo.userId);
        BlackboardUtils.SetOrCreateValue(bb, "tier", bossRaidersContributionInfo.tier);
        BlackboardUtils.SetOrCreateValue(bb, "userName", bossRaidersContributionInfo.userName);
        BlackboardUtils.SetOrCreateValue(bb, "profileUrl", bossRaidersContributionInfo.profileUrl);
        BlackboardUtils.SetOrCreateValue(bb, "totalAttack", bossRaidersContributionInfo.totalAttack);
    }

    public static void Serialize(IBlackboard bb, BossRaidersDealAttackWheelResultInfo bossRaidersDealAttackWheelResultInfo)
    {
        if (bossRaidersDealAttackWheelResultInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "hitType", bossRaidersDealAttackWheelResultInfo.hitType);
        BlackboardUtils.SetOrCreateValue(bb, "spinCount", bossRaidersDealAttackWheelResultInfo.spinCount);
        BlackboardUtils.SetOrCreateValue(bb, "remainingHp", bossRaidersDealAttackWheelResultInfo.remainingHp);
        BlackboardUtils.SetOrCreateValue(bb, "earnCredit", bossRaidersDealAttackWheelResultInfo.earnCredit);
        BlackboardUtils.SetOrCreateValue(bb, "totalCredit", bossRaidersDealAttackWheelResultInfo.totalCredit);
        if (bossRaidersDealAttackWheelResultInfo.nextRoundBalanceInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "nextRoundBalanceInfo"), bossRaidersDealAttackWheelResultInfo.nextRoundBalanceInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "nextRoundBalanceInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "nextRoundBossIndex", bossRaidersDealAttackWheelResultInfo.nextRoundBossIndex);
    }

    public static void Serialize(IBlackboard bb, BossRaidersDealBonusInfo bossRaidersDealBonusInfo)
    {
        if (bossRaidersDealBonusInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "type", bossRaidersDealBonusInfo.type);
        BlackboardUtils.SetOrCreateValue(bb, "amount", bossRaidersDealBonusInfo.amount);
    }

    public static void Serialize(IBlackboard bb, BossRaidersDealBonusWheelResultInfo bossRaidersDealBonusWheelResultInfo)
    {
        if (bossRaidersDealBonusWheelResultInfo == null) { return; }
        BlackboardUtils.SetOrCreateList(bb, "bonusInfoList", bossRaidersDealBonusWheelResultInfo.bonusInfoList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "bonusIndex", bossRaidersDealBonusWheelResultInfo.bonusIndex);
        BlackboardUtils.SetOrCreateValue(bb, "spinCount", bossRaidersDealBonusWheelResultInfo.spinCount);
        BlackboardUtils.SetOrCreateValue(bb, "totalCredit", bossRaidersDealBonusWheelResultInfo.totalCredit);
        BlackboardUtils.SetOrCreateValue(bb, "totalGem", bossRaidersDealBonusWheelResultInfo.totalGem);
    }

    public static void Serialize(IBlackboard bb, BossRaidersDealDebugSpinRequest bossRaidersDealDebugSpinRequest)
    {
        if (bossRaidersDealDebugSpinRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", bossRaidersDealDebugSpinRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", bossRaidersDealDebugSpinRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "dealUuid", bossRaidersDealDebugSpinRequest.dealUuid);
        BlackboardUtils.SetOrCreateValue(bb, "isAutoSpin", bossRaidersDealDebugSpinRequest.isAutoSpin);
        BlackboardUtils.SetOrCreateValue(bb, "debugSpinIndex", bossRaidersDealDebugSpinRequest.debugSpinIndex);
    }

    public static void Serialize(IBlackboard bb, BossRaidersDealEndInfo bossRaidersDealEndInfo)
    {
        if (bossRaidersDealEndInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "totalEarnCredit", bossRaidersDealEndInfo.totalEarnCredit);
        BlackboardUtils.SetOrCreateValue(bb, "totalEarnGem", bossRaidersDealEndInfo.totalEarnGem);
        BlackboardUtils.SetOrCreateValue(bb, "jackpotType", bossRaidersDealEndInfo.jackpotType);
    }

    public static void Serialize(IBlackboard bb, BossRaidersDealEnterRequest bossRaidersDealEnterRequest)
    {
        if (bossRaidersDealEnterRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", bossRaidersDealEnterRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", bossRaidersDealEnterRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "dealUuid", bossRaidersDealEnterRequest.dealUuid);
    }

    public static void Serialize(IBlackboard bb, BossRaidersDealEnterResponse bossRaidersDealEnterResponse)
    {
        if (bossRaidersDealEnterResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", bossRaidersDealEnterResponse.error);
        if (bossRaidersDealEnterResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), bossRaidersDealEnterResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", bossRaidersDealEnterResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "themeId", bossRaidersDealEnterResponse.themeId);
        BlackboardUtils.SetOrCreateValue(bb, "bossIndex", bossRaidersDealEnterResponse.bossIndex);
        if (bossRaidersDealEnterResponse.roundBalanceInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "roundBalanceInfo"), bossRaidersDealEnterResponse.roundBalanceInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "roundBalanceInfo");
        }
        BlackboardUtils.SetOrCreateList(bb, "wheelCandidateList", bossRaidersDealEnterResponse.wheelCandidateList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "spinCount", bossRaidersDealEnterResponse.spinCount);
        BlackboardUtils.SetOrCreateValue(bb, "remainingHp", bossRaidersDealEnterResponse.remainingHp);
        BlackboardUtils.SetOrCreateValue(bb, "credit", bossRaidersDealEnterResponse.credit);
        BlackboardUtils.SetOrCreateValue(bb, "gem", bossRaidersDealEnterResponse.gem);
        BlackboardUtils.SetOrCreateValue(bb, "expectedDamage", bossRaidersDealEnterResponse.expectedDamage);
    }

    public static void Serialize(IBlackboard bb, BossRaidersDealRoundInfo bossRaidersDealRoundInfo)
    {
        if (bossRaidersDealRoundInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "round", bossRaidersDealRoundInfo.round);
        BlackboardUtils.SetOrCreateValue(bb, "hpCoin", bossRaidersDealRoundInfo.hpCoin);
        BlackboardUtils.SetOrCreateValue(bb, "clearBonus", bossRaidersDealRoundInfo.clearBonus);
    }

    public static void Serialize(IBlackboard bb, BossRaidersDealSpinRequest bossRaidersDealSpinRequest)
    {
        if (bossRaidersDealSpinRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", bossRaidersDealSpinRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", bossRaidersDealSpinRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "dealUuid", bossRaidersDealSpinRequest.dealUuid);
        BlackboardUtils.SetOrCreateValue(bb, "isAutoSpin", bossRaidersDealSpinRequest.isAutoSpin);
    }

    public static void Serialize(IBlackboard bb, BossRaidersDealSpinResponse bossRaidersDealSpinResponse)
    {
        if (bossRaidersDealSpinResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", bossRaidersDealSpinResponse.error);
        if (bossRaidersDealSpinResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), bossRaidersDealSpinResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", bossRaidersDealSpinResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "wheelResultIndex", bossRaidersDealSpinResponse.wheelResultIndex);
        if (bossRaidersDealSpinResponse.wheelResultInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "wheelResultInfo"), bossRaidersDealSpinResponse.wheelResultInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "wheelResultInfo");
        }
        if (bossRaidersDealSpinResponse.endInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "endInfo"), bossRaidersDealSpinResponse.endInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "endInfo");
        }
        if (bossRaidersDealSpinResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), bossRaidersDealSpinResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
    }

    public static void Serialize(IBlackboard bb, BossRaidersDealWheelResultInfo bossRaidersDealWheelResultInfo)
    {
        if (bossRaidersDealWheelResultInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "type", bossRaidersDealWheelResultInfo.type);
        if (bossRaidersDealWheelResultInfo.info != null)
        {
            switch (bossRaidersDealWheelResultInfo.type)
            {
            case BossRaidersWheelType.ATTACK:
                Serialize(bb, (BossRaidersDealAttackWheelResultInfo)bossRaidersDealWheelResultInfo.info);
                break;

            case BossRaidersWheelType.BONUS:
                Serialize(bb, (BossRaidersDealBonusWheelResultInfo)bossRaidersDealWheelResultInfo.info);
                break;
            }
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "info");
        }
    }

    public static void Serialize(IBlackboard bb, BossRaidersDebugSpinRequest bossRaidersDebugSpinRequest)
    {
        if (bossRaidersDebugSpinRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", bossRaidersDebugSpinRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", bossRaidersDebugSpinRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "eventId", bossRaidersDebugSpinRequest.eventId);
        BlackboardUtils.SetOrCreateValue(bb, "maxBetMultiplyNumerator", bossRaidersDebugSpinRequest.maxBetMultiplyNumerator);
        BlackboardUtils.SetOrCreateValue(bb, "debugSpinIndex", bossRaidersDebugSpinRequest.debugSpinIndex);
        BlackboardUtils.SetOrCreateValue(bb, "isAutoSpin", bossRaidersDebugSpinRequest.isAutoSpin);
    }

    public static void Serialize(IBlackboard bb, BossRaidersEnergyBundleInfoV1 bossRaidersEnergyBundleInfoV1)
    {
        if (bossRaidersEnergyBundleInfoV1 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "bet", bossRaidersEnergyBundleInfoV1.bet);
        BlackboardUtils.SetOrCreateValue(bb, "gaugeType", bossRaidersEnergyBundleInfoV1.gaugeType);
        BlackboardUtils.SetOrCreateValue(bb, "totalEnergyBundleEarning", bossRaidersEnergyBundleInfoV1.totalEnergyBundleEarning);
    }

    public static void Serialize(IBlackboard bb, BossRaidersEnterInfoV1 bossRaidersEnterInfoV1)
    {
        if (bossRaidersEnterInfoV1 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "energy", bossRaidersEnterInfoV1.energy);
        BlackboardUtils.SetOrCreateValue(bb, "energyForSpin", bossRaidersEnterInfoV1.energyForSpin);
        BlackboardUtils.SetOrCreateList(bb, "energyBundleInfoList", bossRaidersEnterInfoV1.energyBundleInfoList, Serialize);
    }

    public static void Serialize(IBlackboard bb, BossRaidersEnterRequest bossRaidersEnterRequest)
    {
        if (bossRaidersEnterRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", bossRaidersEnterRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", bossRaidersEnterRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "eventId", bossRaidersEnterRequest.eventId);
    }

    public static void Serialize(IBlackboard bb, BossRaidersEnterResponseV1 bossRaidersEnterResponseV1)
    {
        if (bossRaidersEnterResponseV1 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", bossRaidersEnterResponseV1.error);
        if (bossRaidersEnterResponseV1.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), bossRaidersEnterResponseV1.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", bossRaidersEnterResponseV1.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "energy", bossRaidersEnterResponseV1.energy);
        BlackboardUtils.SetOrCreateValue(bb, "energyForSpin", bossRaidersEnterResponseV1.energyForSpin);
        if (bossRaidersEnterResponseV1.roundBalanceInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "roundBalanceInfo"), bossRaidersEnterResponseV1.roundBalanceInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "roundBalanceInfo");
        }
        if (bossRaidersEnterResponseV1.roundBossInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "roundBossInfo"), bossRaidersEnterResponseV1.roundBossInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "roundBossInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "clubRank", bossRaidersEnterResponseV1.clubRank);
        BlackboardUtils.SetOrCreateValue(bb, "percentile", bossRaidersEnterResponseV1.percentile);
        BlackboardUtils.SetOrCreateList(bb, "wheelCandidateList", bossRaidersEnterResponseV1.wheelCandidateList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "clubRankInfoList", bossRaidersEnterResponseV1.clubRankInfoList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "contributionInfoList", bossRaidersEnterResponseV1.contributionInfoList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "maxBetMultiplyNumeratorByEnergyList", bossRaidersEnterResponseV1.maxBetMultiplyNumeratorByEnergyList);
        BlackboardUtils.SetOrCreateValue(bb, "finalRewardRankPayTypeInfo", bossRaidersEnterResponseV1.finalRewardRankPayTypeInfo);
        BlackboardUtils.SetOrCreateValue(bb, "finalRewardPercentilePayTypeInfo", bossRaidersEnterResponseV1.finalRewardPercentilePayTypeInfo);
        BlackboardUtils.SetOrCreateValue(bb, "bossRaidersCooltime", bossRaidersEnterResponseV1.bossRaidersCooltime);
        BlackboardUtils.SetOrCreateValue(bb, "lastVideoAdsClaimTimestamp", bossRaidersEnterResponseV1.lastVideoAdsClaimTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "updatedTotalCompletedRound", bossRaidersEnterResponseV1.updatedTotalCompletedRound);
        BlackboardUtils.SetOrCreateValue(bb, "defaultBetMultiplyNumeratorByEnergy", bossRaidersEnterResponseV1.defaultBetMultiplyNumeratorByEnergy);
        if (bossRaidersEnterResponseV1.clubInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "clubInfo"), bossRaidersEnterResponseV1.clubInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "clubInfo");
        }
    }

    public static void Serialize(IBlackboard bb, BossRaidersInfoRequest bossRaidersInfoRequest)
    {
        if (bossRaidersInfoRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", bossRaidersInfoRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", bossRaidersInfoRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "eventId", bossRaidersInfoRequest.eventId);
    }

    public static void Serialize(IBlackboard bb, BossRaidersInfoResponse bossRaidersInfoResponse)
    {
        if (bossRaidersInfoResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", bossRaidersInfoResponse.error);
        if (bossRaidersInfoResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), bossRaidersInfoResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", bossRaidersInfoResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "clubRank", bossRaidersInfoResponse.clubRank);
        BlackboardUtils.SetOrCreateValue(bb, "percentile", bossRaidersInfoResponse.percentile);
        BlackboardUtils.SetOrCreateList(bb, "clubRankInfoList", bossRaidersInfoResponse.clubRankInfoList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "contributionInfoList", bossRaidersInfoResponse.contributionInfoList, Serialize);
    }

    public static void Serialize(IBlackboard bb, BossRaidersRankingPopupInfo bossRaidersRankingPopupInfo)
    {
        if (bossRaidersRankingPopupInfo == null) { return; }
        BlackboardUtils.SetOrCreateList(bb, "clubRanking", bossRaidersRankingPopupInfo.clubRanking, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "backgroundImageUrl", bossRaidersRankingPopupInfo.backgroundImageUrl);
        BlackboardUtils.SetOrCreateValue(bb, "themeId", bossRaidersRankingPopupInfo.themeId);
    }

    public static void Serialize(IBlackboard bb, BossRaidersRewardPopupInfo bossRaidersRewardPopupInfo)
    {
        if (bossRaidersRewardPopupInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "totalAttack", bossRaidersRewardPopupInfo.totalAttack);
        BlackboardUtils.SetOrCreateValue(bb, "gem", bossRaidersRewardPopupInfo.gem);
        BlackboardUtils.SetOrCreateValue(bb, "rank", bossRaidersRewardPopupInfo.rank);
        BlackboardUtils.SetOrCreateValue(bb, "percentile", bossRaidersRewardPopupInfo.percentile);
        BlackboardUtils.SetOrCreateValue(bb, "backgroundImageUrl", bossRaidersRewardPopupInfo.backgroundImageUrl);
        BlackboardUtils.SetOrCreateValue(bb, "eventId", bossRaidersRewardPopupInfo.eventId);
        BlackboardUtils.SetOrCreateValue(bb, "themeId", bossRaidersRewardPopupInfo.themeId);
    }

    public static void Serialize(IBlackboard bb, BossRaidersRoundBalanceInfo bossRaidersRoundBalanceInfo)
    {
        if (bossRaidersRoundBalanceInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "round", bossRaidersRoundBalanceInfo.round);
        BlackboardUtils.SetOrCreateValue(bb, "hp", bossRaidersRoundBalanceInfo.hp);
        BlackboardUtils.SetOrCreateValue(bb, "leaguePoint", bossRaidersRoundBalanceInfo.leaguePoint);
        if (bossRaidersRoundBalanceInfo.reward != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "reward"), bossRaidersRoundBalanceInfo.reward);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "reward");
        }
    }

    public static void Serialize(IBlackboard bb, BossRaidersRoundBossInfo bossRaidersRoundBossInfo)
    {
        if (bossRaidersRoundBossInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "hp", bossRaidersRoundBossInfo.hp);
        if (bossRaidersRoundBossInfo.variationInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "variationInfo"), bossRaidersRoundBossInfo.variationInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "variationInfo");
        }
    }

    public static void Serialize(IBlackboard bb, BossRaidersRoundBossVariationInfo bossRaidersRoundBossVariationInfo)
    {
        if (bossRaidersRoundBossVariationInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "type", bossRaidersRoundBossVariationInfo.type);
        BlackboardUtils.SetOrCreateValue(bb, "color", bossRaidersRoundBossVariationInfo.color);
        BlackboardUtils.SetOrCreateValue(bb, "scale", bossRaidersRoundBossVariationInfo.scale);
        BlackboardUtils.SetOrCreateValue(bb, "themeId", bossRaidersRoundBossVariationInfo.themeId);
        BlackboardUtils.SetOrCreateValue(bb, "bossIndex", bossRaidersRoundBossVariationInfo.bossIndex);
    }

    public static void Serialize(IBlackboard bb, BossRaidersSpinRequest bossRaidersSpinRequest)
    {
        if (bossRaidersSpinRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", bossRaidersSpinRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", bossRaidersSpinRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "eventId", bossRaidersSpinRequest.eventId);
        BlackboardUtils.SetOrCreateValue(bb, "maxBetMultiplyNumerator", bossRaidersSpinRequest.maxBetMultiplyNumerator);
        BlackboardUtils.SetOrCreateValue(bb, "isAutoSpin", bossRaidersSpinRequest.isAutoSpin);
    }

    public static void Serialize(IBlackboard bb, BossRaidersSpinResponseV1 bossRaidersSpinResponseV1)
    {
        if (bossRaidersSpinResponseV1 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", bossRaidersSpinResponseV1.error);
        if (bossRaidersSpinResponseV1.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), bossRaidersSpinResponseV1.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", bossRaidersSpinResponseV1.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "wheelResultIndex", bossRaidersSpinResponseV1.wheelResultIndex);
        if (bossRaidersSpinResponseV1.wheelResultInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "wheelResultInfo"), bossRaidersSpinResponseV1.wheelResultInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "wheelResultInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "energy", bossRaidersSpinResponseV1.energy);
        if (bossRaidersSpinResponseV1.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), bossRaidersSpinResponseV1.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
    }

    public static void Serialize(IBlackboard bb, BossRaidersUpdateInfoV1 bossRaidersUpdateInfoV1)
    {
        if (bossRaidersUpdateInfoV1 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "energy", bossRaidersUpdateInfoV1.energy);
        BlackboardUtils.SetOrCreateValue(bb, "energyEarning", bossRaidersUpdateInfoV1.energyEarning);
    }

    public static void Serialize(IBlackboard bb, BossRaidersWheelCandidate bossRaidersWheelCandidate)
    {
        if (bossRaidersWheelCandidate == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "type", bossRaidersWheelCandidate.type);
        if (bossRaidersWheelCandidate.info != null)
        {
            switch (bossRaidersWheelCandidate.type)
            {
            case BossRaidersWheelType.ATTACK:
                Serialize(bb, (BossRaidersAttackWheelCandidateInfo)bossRaidersWheelCandidate.info);
                break;

            case BossRaidersWheelType.BONUS:
                Serialize(bb, (BossRaidersBonusWheelCandidateInfo)bossRaidersWheelCandidate.info);
                break;
            }
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "info");
        }
    }

    public static void Serialize(IBlackboard bb, BossRaidersWheelResultInfo bossRaidersWheelResultInfo)
    {
        if (bossRaidersWheelResultInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "type", bossRaidersWheelResultInfo.type);
        if (bossRaidersWheelResultInfo.info != null)
        {
            switch (bossRaidersWheelResultInfo.type)
            {
            case BossRaidersWheelType.ATTACK:
                Serialize(bb, (BossRaidersAttackWheelResultInfo)bossRaidersWheelResultInfo.info);
                break;

            case BossRaidersWheelType.BONUS:
                Serialize(bb, (BossRaidersBonusWheelResultInfo)bossRaidersWheelResultInfo.info);
                break;
            }
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "info");
        }
    }

    public static void Serialize(IBlackboard bb, BottomIconInfo bottomIconInfo)
    {
        if (bottomIconInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "type", bottomIconInfo.type);
        BlackboardUtils.SetOrCreateValue(bb, "priority", bottomIconInfo.priority);
    }

    public static void Serialize(IBlackboard bb, BucksAmountResponse bucksAmountResponse)
    {
        if (bucksAmountResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", bucksAmountResponse.error);
        if (bucksAmountResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), bucksAmountResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", bucksAmountResponse.serverTime);
        if (bucksAmountResponse.userBucks != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userBucks"), bucksAmountResponse.userBucks);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userBucks");
        }
    }

    public static void Serialize(IBlackboard bb, BucksGift bucksGift)
    {
        if (bucksGift == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "giftId", bucksGift.giftId);
        BlackboardUtils.SetOrCreateValue(bb, "paidBucks", bucksGift.paidBucks);
        BlackboardUtils.SetOrCreateValue(bb, "bonusBucks", bucksGift.bonusBucks);
        BlackboardUtils.SetOrCreateValue(bb, "freeBucks", bucksGift.freeBucks);
        BlackboardUtils.SetOrCreateValue(bb, "title", bucksGift.title);
        BlackboardUtils.SetOrCreateValue(bb, "message", bucksGift.message);
        BlackboardUtils.SetOrCreateValue(bb, "createTimestamp", bucksGift.createTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "expireTimestamp", bucksGift.expireTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "type", bucksGift.type);
        BlackboardUtils.SetOrCreateValue(bb, "rewardCauseType", bucksGift.rewardCauseType);
        if (bucksGift.reward != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "reward"), bucksGift.reward);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "reward");
        }
    }

    public static void Serialize(IBlackboard bb, BucksRedeemGiftRequest bucksRedeemGiftRequest)
    {
        if (bucksRedeemGiftRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", bucksRedeemGiftRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", bucksRedeemGiftRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "giftId", bucksRedeemGiftRequest.giftId);
    }

    public static void Serialize(IBlackboard bb, BuildDreamCollectDailyChestRequest buildDreamCollectDailyChestRequest)
    {
        if (buildDreamCollectDailyChestRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", buildDreamCollectDailyChestRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", buildDreamCollectDailyChestRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "buildingIndex", buildDreamCollectDailyChestRequest.buildingIndex);
    }

    public static void Serialize(IBlackboard bb, BuildDreamCollectDailyChestResponse buildDreamCollectDailyChestResponse)
    {
        if (buildDreamCollectDailyChestResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", buildDreamCollectDailyChestResponse.error);
        if (buildDreamCollectDailyChestResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), buildDreamCollectDailyChestResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", buildDreamCollectDailyChestResponse.serverTime);
        if (buildDreamCollectDailyChestResponse.earnedDepot != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "earnedDepot"), buildDreamCollectDailyChestResponse.earnedDepot);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "earnedDepot");
        }
        BlackboardUtils.SetOrCreateValue(bb, "earnedCredit", buildDreamCollectDailyChestResponse.earnedCredit);
        BlackboardUtils.SetOrCreateValue(bb, "earnedWildPuzzleCount", buildDreamCollectDailyChestResponse.earnedWildPuzzleCount);
        BlackboardUtils.SetOrCreateValue(bb, "earnedLoungePoint", buildDreamCollectDailyChestResponse.earnedLoungePoint);
        BlackboardUtils.SetOrCreateValue(bb, "newWildPuzzleCount", buildDreamCollectDailyChestResponse.newWildPuzzleCount);
        if (buildDreamCollectDailyChestResponse.depot != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "depot"), buildDreamCollectDailyChestResponse.depot);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "depot");
        }
        BlackboardUtils.SetOrCreateList(bb, "dailyChestList", buildDreamCollectDailyChestResponse.dailyChestList, Serialize);
        if (buildDreamCollectDailyChestResponse.vipLoungeInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "vipLoungeInfo"), buildDreamCollectDailyChestResponse.vipLoungeInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "vipLoungeInfo");
        }
        if (buildDreamCollectDailyChestResponse.buildDreamInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "buildDreamInfo"), buildDreamCollectDailyChestResponse.buildDreamInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "buildDreamInfo");
        }
    }

    public static void Serialize(IBlackboard bb, BuildDreamCollectFreeDepotResponse buildDreamCollectFreeDepotResponse)
    {
        if (buildDreamCollectFreeDepotResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", buildDreamCollectFreeDepotResponse.error);
        if (buildDreamCollectFreeDepotResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), buildDreamCollectFreeDepotResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", buildDreamCollectFreeDepotResponse.serverTime);
        if (buildDreamCollectFreeDepotResponse.earnedDepot != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "earnedDepot"), buildDreamCollectFreeDepotResponse.earnedDepot);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "earnedDepot");
        }
        if (buildDreamCollectFreeDepotResponse.depot != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "depot"), buildDreamCollectFreeDepotResponse.depot);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "depot");
        }
        BlackboardUtils.SetOrCreateValue(bb, "freeDepotLastCollectTimestamp", buildDreamCollectFreeDepotResponse.freeDepotLastCollectTimestamp);
        if (buildDreamCollectFreeDepotResponse.buildDreamInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "buildDreamInfo"), buildDreamCollectFreeDepotResponse.buildDreamInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "buildDreamInfo");
        }
    }

    public static void Serialize(IBlackboard bb, BuildDreamCollectWildPuzzleRequest buildDreamCollectWildPuzzleRequest)
    {
        if (buildDreamCollectWildPuzzleRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", buildDreamCollectWildPuzzleRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", buildDreamCollectWildPuzzleRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "targetBuildingIndex", buildDreamCollectWildPuzzleRequest.targetBuildingIndex);
    }

    public static void Serialize(IBlackboard bb, BuildDreamCollectWildPuzzleResponse buildDreamCollectWildPuzzleResponse)
    {
        if (buildDreamCollectWildPuzzleResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", buildDreamCollectWildPuzzleResponse.error);
        if (buildDreamCollectWildPuzzleResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), buildDreamCollectWildPuzzleResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", buildDreamCollectWildPuzzleResponse.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "depotOpenResultList", buildDreamCollectWildPuzzleResponse.depotOpenResultList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "wildPuzzleCount", buildDreamCollectWildPuzzleResponse.wildPuzzleCount);
        BlackboardUtils.SetOrCreateValue(bb, "finalRewardCredit", buildDreamCollectWildPuzzleResponse.finalRewardCredit);
        if (buildDreamCollectWildPuzzleResponse.buildDreamInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "buildDreamInfo"), buildDreamCollectWildPuzzleResponse.buildDreamInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "buildDreamInfo");
        }
        BlackboardUtils.SetOrCreateList(bb, "dailyChestList", buildDreamCollectWildPuzzleResponse.dailyChestList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "exhibitionSeasonList", buildDreamCollectWildPuzzleResponse.exhibitionSeasonList, Serialize);
        if (buildDreamCollectWildPuzzleResponse.createdGurusBuilding != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "createdGurusBuilding"), buildDreamCollectWildPuzzleResponse.createdGurusBuilding);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "createdGurusBuilding");
        }
        if (buildDreamCollectWildPuzzleResponse.newGurusRankPercentile != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "newGurusRankPercentile"), buildDreamCollectWildPuzzleResponse.newGurusRankPercentile);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "newGurusRankPercentile");
        }
        if (buildDreamCollectWildPuzzleResponse.newGurusBuildingPreset != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "newGurusBuildingPreset"), buildDreamCollectWildPuzzleResponse.newGurusBuildingPreset);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "newGurusBuildingPreset");
        }
        BlackboardUtils.SetOrCreateValue(bb, "finalRewardGem", buildDreamCollectWildPuzzleResponse.finalRewardGem);
    }

    public static void Serialize(IBlackboard bb, BuildDreamConstants buildDreamConstants)
    {
        if (buildDreamConstants == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "SEASON_FINAL_REWARD_CREDIT", buildDreamConstants.SEASON_FINAL_REWARD_CREDIT);
        BlackboardUtils.SetOrCreateValue(bb, "WILD_DEPOT_EXP", buildDreamConstants.WILD_DEPOT_EXP);
        BlackboardUtils.SetOrCreateValue(bb, "WILD_PUZZLE_COUNT_MAX", buildDreamConstants.WILD_PUZZLE_COUNT_MAX);
        BlackboardUtils.SetOrCreateValue(bb, "FREE_DEPOT_COOLTIME_MILLISEC", buildDreamConstants.FREE_DEPOT_COOLTIME_MILLISEC);
        BlackboardUtils.SetOrCreateValue(bb, "DAILY_CHEST_COOLTIME_MILLISEC", buildDreamConstants.DAILY_CHEST_COOLTIME_MILLISEC);
        BlackboardUtils.SetOrCreateValue(bb, "DAILY_CHEST_UNLOCK_BUILDING_LEVEL", buildDreamConstants.DAILY_CHEST_UNLOCK_BUILDING_LEVEL);
        BlackboardUtils.SetOrCreateValue(bb, "BUILDING_LEVEL_MAX", buildDreamConstants.BUILDING_LEVEL_MAX);
        BlackboardUtils.SetOrCreateValue(bb, "GURUS_BUILDING_INDEX", buildDreamConstants.GURUS_BUILDING_INDEX);
        BlackboardUtils.SetOrCreateValue(bb, "SEASON_FINAL_REWARD_GEM", buildDreamConstants.SEASON_FINAL_REWARD_GEM);
    }

    public static void Serialize(IBlackboard bb, BuildDreamDailyChestPersonal buildDreamDailyChestPersonal)
    {
        if (buildDreamDailyChestPersonal == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "buildingIndex", buildDreamDailyChestPersonal.buildingIndex);
        BlackboardUtils.SetOrCreateValue(bb, "lastCollectTimestamp", buildDreamDailyChestPersonal.lastCollectTimestamp);
    }

    public static void Serialize(IBlackboard bb, BuildDreamEnterResponse buildDreamEnterResponse)
    {
        if (buildDreamEnterResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", buildDreamEnterResponse.error);
        if (buildDreamEnterResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), buildDreamEnterResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", buildDreamEnterResponse.serverTime);
        if (buildDreamEnterResponse.buildDreamInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "buildDreamInfo"), buildDreamEnterResponse.buildDreamInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "buildDreamInfo");
        }
        if (buildDreamEnterResponse.buildDreamConstants != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "buildDreamConstants"), buildDreamEnterResponse.buildDreamConstants);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "buildDreamConstants");
        }
        if (buildDreamEnterResponse.buildDreamSeason != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "buildDreamSeason"), buildDreamEnterResponse.buildDreamSeason);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "buildDreamSeason");
        }
        BlackboardUtils.SetOrCreateList(bb, "buildingPresetList", buildDreamEnterResponse.buildingPresetList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "buildingList", buildDreamEnterResponse.buildingList, Serialize);
        if (buildDreamEnterResponse.depot != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "depot"), buildDreamEnterResponse.depot);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "depot");
        }
        BlackboardUtils.SetOrCreateValue(bb, "wildPuzzleCount", buildDreamEnterResponse.wildPuzzleCount);
        BlackboardUtils.SetOrCreateList(bb, "dailyChestList", buildDreamEnterResponse.dailyChestList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "freeDepotLastCollectTimestamp", buildDreamEnterResponse.freeDepotLastCollectTimestamp);
        BlackboardUtils.SetOrCreateList(bb, "exhibitionSeasonList", buildDreamEnterResponse.exhibitionSeasonList, Serialize);
        if (buildDreamEnterResponse.gurusBuilding != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "gurusBuilding"), buildDreamEnterResponse.gurusBuilding);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "gurusBuilding");
        }
        if (buildDreamEnterResponse.gurusBuildingPreset != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "gurusBuildingPreset"), buildDreamEnterResponse.gurusBuildingPreset);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "gurusBuildingPreset");
        }
        if (buildDreamEnterResponse.gurusFinalRewardPreset != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "gurusFinalRewardPreset"), buildDreamEnterResponse.gurusFinalRewardPreset);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "gurusFinalRewardPreset");
        }
        if (buildDreamEnterResponse.gurusRankPercentile != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "gurusRankPercentile"), buildDreamEnterResponse.gurusRankPercentile);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "gurusRankPercentile");
        }
        BlackboardUtils.SetOrCreateValue(bb, "depotBundleShopActive", buildDreamEnterResponse.depotBundleShopActive);
    }

    public static void Serialize(IBlackboard bb, BuildDreamExhibitionRequest buildDreamExhibitionRequest)
    {
        if (buildDreamExhibitionRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", buildDreamExhibitionRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", buildDreamExhibitionRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "seasonId", buildDreamExhibitionRequest.seasonId);
    }

    public static void Serialize(IBlackboard bb, BuildDreamExhibitionResponse buildDreamExhibitionResponse)
    {
        if (buildDreamExhibitionResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", buildDreamExhibitionResponse.error);
        if (buildDreamExhibitionResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), buildDreamExhibitionResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", buildDreamExhibitionResponse.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "buildingList", buildDreamExhibitionResponse.buildingList, Serialize);
        if (buildDreamExhibitionResponse.season != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "season"), buildDreamExhibitionResponse.season);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "season");
        }
    }

    public static void Serialize(IBlackboard bb, BuildDreamGurusRankListRequest buildDreamGurusRankListRequest)
    {
        if (buildDreamGurusRankListRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", buildDreamGurusRankListRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", buildDreamGurusRankListRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "referenceUserId", buildDreamGurusRankListRequest.referenceUserId);
        BlackboardUtils.SetOrCreateValue(bb, "aboveCount", buildDreamGurusRankListRequest.aboveCount);
        BlackboardUtils.SetOrCreateValue(bb, "belowCount", buildDreamGurusRankListRequest.belowCount);
        BlackboardUtils.SetOrCreateValue(bb, "matchTotalCount", buildDreamGurusRankListRequest.matchTotalCount);
    }

    public static void Serialize(IBlackboard bb, BuildDreamGurusRankListResponse buildDreamGurusRankListResponse)
    {
        if (buildDreamGurusRankListResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", buildDreamGurusRankListResponse.error);
        if (buildDreamGurusRankListResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), buildDreamGurusRankListResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", buildDreamGurusRankListResponse.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "gurusRankList", buildDreamGurusRankListResponse.gurusRankList, Serialize);
    }

    public static void Serialize(IBlackboard bb, BuildDreamInfo buildDreamInfo)
    {
        if (buildDreamInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "active", buildDreamInfo.active);
        BlackboardUtils.SetOrCreateValue(bb, "vipLoungeBenefitEndTimestamp", buildDreamInfo.vipLoungeBenefitEndTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "seasonThemeId", buildDreamInfo.seasonThemeId);
        BlackboardUtils.SetOrCreateValue(bb, "totalDepotCount", buildDreamInfo.totalDepotCount);
        BlackboardUtils.SetOrCreateValue(bb, "seasonEndTimestamp", buildDreamInfo.seasonEndTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "hideBadge", buildDreamInfo.hideBadge);
        BlackboardUtils.SetOrCreateValue(bb, "seasonId", buildDreamInfo.seasonId);
    }

    public static void Serialize(IBlackboard bb, BuildDreamInfoResponse buildDreamInfoResponse)
    {
        if (buildDreamInfoResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", buildDreamInfoResponse.error);
        if (buildDreamInfoResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), buildDreamInfoResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", buildDreamInfoResponse.serverTime);
        if (buildDreamInfoResponse.vipLoungeInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "vipLoungeInfo"), buildDreamInfoResponse.vipLoungeInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "vipLoungeInfo");
        }
        if (buildDreamInfoResponse.buildDreamInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "buildDreamInfo"), buildDreamInfoResponse.buildDreamInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "buildDreamInfo");
        }
    }

    public static void Serialize(IBlackboard bb, BuildDreamOpenDepotRequest buildDreamOpenDepotRequest)
    {
        if (buildDreamOpenDepotRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", buildDreamOpenDepotRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", buildDreamOpenDepotRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "depotType", buildDreamOpenDepotRequest.depotType);
    }

    public static void Serialize(IBlackboard bb, BuildDreamOpenDepotResponse buildDreamOpenDepotResponse)
    {
        if (buildDreamOpenDepotResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", buildDreamOpenDepotResponse.error);
        if (buildDreamOpenDepotResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), buildDreamOpenDepotResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", buildDreamOpenDepotResponse.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "depotOpenResultList", buildDreamOpenDepotResponse.depotOpenResultList, Serialize);
        if (buildDreamOpenDepotResponse.depot != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "depot"), buildDreamOpenDepotResponse.depot);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "depot");
        }
        BlackboardUtils.SetOrCreateValue(bb, "finalRewardCredit", buildDreamOpenDepotResponse.finalRewardCredit);
        if (buildDreamOpenDepotResponse.buildDreamInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "buildDreamInfo"), buildDreamOpenDepotResponse.buildDreamInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "buildDreamInfo");
        }
        BlackboardUtils.SetOrCreateList(bb, "dailyChestList", buildDreamOpenDepotResponse.dailyChestList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "exhibitionSeasonList", buildDreamOpenDepotResponse.exhibitionSeasonList, Serialize);
        if (buildDreamOpenDepotResponse.createdGurusBuilding != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "createdGurusBuilding"), buildDreamOpenDepotResponse.createdGurusBuilding);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "createdGurusBuilding");
        }
        if (buildDreamOpenDepotResponse.newGurusRankPercentile != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "newGurusRankPercentile"), buildDreamOpenDepotResponse.newGurusRankPercentile);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "newGurusRankPercentile");
        }
        if (buildDreamOpenDepotResponse.newGurusBuildingPreset != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "newGurusBuildingPreset"), buildDreamOpenDepotResponse.newGurusBuildingPreset);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "newGurusBuildingPreset");
        }
        if (buildDreamOpenDepotResponse.usedDepot != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "usedDepot"), buildDreamOpenDepotResponse.usedDepot);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "usedDepot");
        }
        BlackboardUtils.SetOrCreateValue(bb, "finalRewardGem", buildDreamOpenDepotResponse.finalRewardGem);
    }

    public static void Serialize(IBlackboard bb, BuildDreamSeasonForDropdown buildDreamSeasonForDropdown)
    {
        if (buildDreamSeasonForDropdown == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "id", buildDreamSeasonForDropdown.id);
        BlackboardUtils.SetOrCreateValue(bb, "themeId", buildDreamSeasonForDropdown.themeId);
        BlackboardUtils.SetOrCreateValue(bb, "name", buildDreamSeasonForDropdown.name);
    }

    public static void Serialize(IBlackboard bb, BuildDreamSeasonPreset buildDreamSeasonPreset)
    {
        if (buildDreamSeasonPreset == null) { return; }
        BlackboardUtils.SetOrCreateList(bb, "buildingPresetList", buildDreamSeasonPreset.buildingPresetList, Serialize);
        if (buildDreamSeasonPreset.gurusPreset != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "gurusPreset"), buildDreamSeasonPreset.gurusPreset);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "gurusPreset");
        }
    }

    public static void Serialize(IBlackboard bb, BuildDreamSeasonTheme buildDreamSeasonTheme)
    {
        if (buildDreamSeasonTheme == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "name", buildDreamSeasonTheme.name);
        if (buildDreamSeasonTheme.preset != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "preset"), buildDreamSeasonTheme.preset);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "preset");
        }
    }

    public static void Serialize(IBlackboard bb, BuildingPreset buildingPreset)
    {
        if (buildingPreset == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "buildingIndex", buildingPreset.buildingIndex);
        BlackboardUtils.SetOrCreateValue(bb, "rank", buildingPreset.rank);
        BlackboardUtils.SetOrCreateList(bb, "levelList", buildingPreset.levelList, Serialize);
    }

    public static void Serialize(IBlackboard bb, BuildingReward buildingReward)
    {
        if (buildingReward == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "level", buildingReward.level);
        BlackboardUtils.SetOrCreateValue(bb, "exp", buildingReward.exp);
        BlackboardUtils.SetOrCreateValue(bb, "levelUpReward", buildingReward.levelUpReward);
    }

    public static void Serialize(IBlackboard bb, BuildingSeasonPreset buildingSeasonPreset)
    {
        if (buildingSeasonPreset == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "buildingIndex", buildingSeasonPreset.buildingIndex);
        BlackboardUtils.SetOrCreateValue(bb, "name", buildingSeasonPreset.name);
        BlackboardUtils.SetOrCreateValue(bb, "objectIndexList", buildingSeasonPreset.objectIndexList);
    }

    public static void Serialize(IBlackboard bb, BuildingValues buildingValues)
    {
        if (buildingValues == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "buildingIndex", buildingValues.buildingIndex);
        BlackboardUtils.SetOrCreateValue(bb, "level", buildingValues.level);
        BlackboardUtils.SetOrCreateValue(bb, "exp", buildingValues.exp);
    }

    public static void Serialize(IBlackboard bb, CSCBonusInfo cSCBonusInfo)
    {
        if (cSCBonusInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "symbolMultiplier", cSCBonusInfo.symbolMultiplier);
    }

    public static void Serialize(IBlackboard bb, CSCJackpot cSCJackpot)
    {
        if (cSCJackpot == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "isJackpot", cSCJackpot.isJackpot);
        BlackboardUtils.SetOrCreateValue(bb, "jackpotIndex", cSCJackpot.jackpotIndex);
        BlackboardUtils.SetOrCreateValue(bb, "jackpotAwardAmount", cSCJackpot.jackpotAwardAmount);
    }

    public static void Serialize(IBlackboard bb, CampaignListResponse campaignListResponse)
    {
        if (campaignListResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", campaignListResponse.error);
        if (campaignListResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), campaignListResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", campaignListResponse.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "inAppMessageList", campaignListResponse.inAppMessageList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "slotBannerGroupList", campaignListResponse.slotBannerGroupList, Serialize);
    }

    public static void Serialize(IBlackboard bb, CategoryInfo categoryInfo)
    {
        if (categoryInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "categoryType", categoryInfo.categoryType);
        BlackboardUtils.SetOrCreateValue(bb, "gameIdList", categoryInfo.gameIdList);
    }

    public static void Serialize(IBlackboard bb, ChallengeClaimAllDoneRequest challengeClaimAllDoneRequest)
    {
        if (challengeClaimAllDoneRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", challengeClaimAllDoneRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", challengeClaimAllDoneRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "challengeType", challengeClaimAllDoneRequest.challengeType);
        BlackboardUtils.SetOrCreateValue(bb, "currentTime", challengeClaimAllDoneRequest.currentTime);
        BlackboardUtils.SetOrCreateValue(bb, "timezoneOffset", challengeClaimAllDoneRequest.timezoneOffset);
        BlackboardUtils.SetOrCreateValue(bb, "challengeClaimMultiplyEventId", challengeClaimAllDoneRequest.challengeClaimMultiplyEventId);
    }

    public static void Serialize(IBlackboard bb, ChallengeClaimAllDoneResponseV1 challengeClaimAllDoneResponseV1)
    {
        if (challengeClaimAllDoneResponseV1 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", challengeClaimAllDoneResponseV1.error);
        if (challengeClaimAllDoneResponseV1.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), challengeClaimAllDoneResponseV1.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", challengeClaimAllDoneResponseV1.serverTime);
        if (challengeClaimAllDoneResponseV1.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), challengeClaimAllDoneResponseV1.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
        if (challengeClaimAllDoneResponseV1.nextChallengeInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "nextChallengeInfo"), challengeClaimAllDoneResponseV1.nextChallengeInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "nextChallengeInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "multiplier", challengeClaimAllDoneResponseV1.multiplier);
        BlackboardUtils.SetOrCreateList(bb, "rewardResultList", challengeClaimAllDoneResponseV1.rewardResultList, Serialize);
    }

    public static void Serialize(IBlackboard bb, ChallengeClaimVideoAdsRequest challengeClaimVideoAdsRequest)
    {
        if (challengeClaimVideoAdsRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", challengeClaimVideoAdsRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", challengeClaimVideoAdsRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "challengeType", challengeClaimVideoAdsRequest.challengeType);
    }

    public static void Serialize(IBlackboard bb, ChallengeInfoResponseV1 challengeInfoResponseV1)
    {
        if (challengeInfoResponseV1 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", challengeInfoResponseV1.error);
        if (challengeInfoResponseV1.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), challengeInfoResponseV1.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", challengeInfoResponseV1.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "challengeInfoList", challengeInfoResponseV1.challengeInfoList, Serialize);
        if (challengeInfoResponseV1.clubChallengeInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "clubChallengeInfo"), challengeInfoResponseV1.clubChallengeInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "clubChallengeInfo");
        }
        BlackboardUtils.SetOrCreateList(bb, "topContributionList", challengeInfoResponseV1.topContributionList, Serialize);
        if (challengeInfoResponseV1.clubEventChallengeInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "clubEventChallengeInfo"), challengeInfoResponseV1.clubEventChallengeInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "clubEventChallengeInfo");
        }
        BlackboardUtils.SetOrCreateList(bb, "clubEventChallengeMissionContributionList", challengeInfoResponseV1.clubEventChallengeMissionContributionList, Serialize);
    }

    public static void Serialize(IBlackboard bb, ChallengeInfoSimple challengeInfoSimple)
    {
        if (challengeInfoSimple == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "challengeType", challengeInfoSimple.challengeType);
        BlackboardUtils.SetOrCreateValue(bb, "challengeProgress", challengeInfoSimple.challengeProgress);
        BlackboardUtils.SetOrCreateValue(bb, "done", challengeInfoSimple.done);
        BlackboardUtils.SetOrCreateValue(bb, "claimed", challengeInfoSimple.claimed);
        BlackboardUtils.SetOrCreateValue(bb, "startTimestamp", challengeInfoSimple.startTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "endTimestamp", challengeInfoSimple.endTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "challengeId", challengeInfoSimple.challengeId);
        BlackboardUtils.SetOrCreateList(bb, "rewardList", challengeInfoSimple.rewardList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "isCompleteToUnlock", challengeInfoSimple.isCompleteToUnlock);
        BlackboardUtils.SetOrCreateValue(bb, "maxChallengeProgress", challengeInfoSimple.maxChallengeProgress);
    }

    public static void Serialize(IBlackboard bb, ChallengeInfoV1 challengeInfoV1)
    {
        if (challengeInfoV1 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "challengeType", challengeInfoV1.challengeType);
        BlackboardUtils.SetOrCreateValue(bb, "startTimestamp", challengeInfoV1.startTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "endTimestamp", challengeInfoV1.endTimestamp);
        BlackboardUtils.SetOrCreateList(bb, "missionList", challengeInfoV1.missionList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "done", challengeInfoV1.done);
        BlackboardUtils.SetOrCreateValue(bb, "claimed", challengeInfoV1.claimed);
        BlackboardUtils.SetOrCreateValue(bb, "challengeProgress", challengeInfoV1.challengeProgress);
        BlackboardUtils.SetOrCreateValue(bb, "challengeId", challengeInfoV1.challengeId);
        BlackboardUtils.SetOrCreateList(bb, "rewardList", challengeInfoV1.rewardList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "lastRewardResultList", challengeInfoV1.lastRewardResultList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "isCompleteToUnlock", challengeInfoV1.isCompleteToUnlock);
    }

    public static void Serialize(IBlackboard bb, ChallengeMissionV1 challengeMissionV1)
    {
        if (challengeMissionV1 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "missionType", challengeMissionV1.missionType);
        if (challengeMissionV1.info != null)
        {
            switch (challengeMissionV1.missionType)
            {
            case ChallengeMissionType.WIN_ANY:
                Serialize(bb, (ChallengeMissionValueEmpty)challengeMissionV1.info);
                break;

            case ChallengeMissionType.SPIN_ANY:
                Serialize(bb, (ChallengeMissionValueEmpty)challengeMissionV1.info);
                break;

            case ChallengeMissionType.WIN_BIG_WIN_ANY:
                Serialize(bb, (ChallengeMissionValueWinBigWinAny)challengeMissionV1.info);
                break;

            case ChallengeMissionType.ENTER_FREE_SPIN_ANY:
                Serialize(bb, (ChallengeMissionValueEmpty)challengeMissionV1.info);
                break;

            case ChallengeMissionType.WIN_CREDIT_MORE_THAN_ANY:
                Serialize(bb, (ChallengeMissionValueWinCreditMoreThanAny)challengeMissionV1.info);
                break;

            case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_ANY:
                Serialize(bb, (ChallengeMissionValueEmpty)challengeMissionV1.info);
                break;

            case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BIG_WIN_ANY:
                Serialize(bb, (ChallengeMissionValueAchieveTotalWinCreditByBigWinAny)challengeMissionV1.info);
                break;

            case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_FREE_SPIN_ANY:
                Serialize(bb, (ChallengeMissionValueEmpty)challengeMissionV1.info);
                break;

            case ChallengeMissionType.ACHIEVE_TOTAL_WAGER_CREDIT_ANY:
                Serialize(bb, (ChallengeMissionValueEmpty)challengeMissionV1.info);
                break;

            case ChallengeMissionType.COLLECT_TIMEBONUS:
                Serialize(bb, (ChallengeMissionValueEmpty)challengeMissionV1.info);
                break;

            case ChallengeMissionType.COLLECT_LUCKY_SPINS:
                Serialize(bb, (ChallengeMissionValueEmpty)challengeMissionV1.info);
                break;

            case ChallengeMissionType.PURCHASE_ANY:
                Serialize(bb, (ChallengeMissionValueEmpty)challengeMissionV1.info);
                break;

            case ChallengeMissionType.PURCHASE_PRICE_MORE_THAN:
                Serialize(bb, (ChallengeMissionValuePurchasePriceMoreThan)challengeMissionV1.info);
                break;

            case ChallengeMissionType.ACHIEVE_TOTAL_PURCHASE_PRICE:
                Serialize(bb, (ChallengeMissionValueEmpty)challengeMissionV1.info);
                break;

            case ChallengeMissionType.USE_GEM_MORE_THAN:
                Serialize(bb, (ChallengeMissionValueEmpty)challengeMissionV1.info);
                break;

            case ChallengeMissionType.WATCH_VIDEO_ADS:
                Serialize(bb, (ChallengeMissionValueWatchVideoAds)challengeMissionV1.info);
                break;

            case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_ANY_WITH_BET_LIMIT:
                Serialize(bb, (ChallengeMissionValueBetLimit)challengeMissionV1.info);
                break;

            case ChallengeMissionType.ACHIEVE_TOTAL_WAGER_CREDIT_ANY_WITH_BET_LIMIT:
                Serialize(bb, (ChallengeMissionValueBetLimit)challengeMissionV1.info);
                break;

            case ChallengeMissionType.SPIN_WITH_BET_LIMIT:
                Serialize(bb, (ChallengeMissionValueBetLimit)challengeMissionV1.info);
                break;

            case ChallengeMissionType.CONNECT_FACEBOOK:
                Serialize(bb, (ChallengeMissionValueEmpty)challengeMissionV1.info);
                break;

            case ChallengeMissionType.ADD_FRIENDS:
                Serialize(bb, (ChallengeMissionValueEmpty)challengeMissionV1.info);
                break;

            case ChallengeMissionType.JOIN_CLUB:
                Serialize(bb, (ChallengeMissionValueEmpty)challengeMissionV1.info);
                break;

            case ChallengeMissionType.CONNECT_EMAIL:
                Serialize(bb, (ChallengeMissionValueEmpty)challengeMissionV1.info);
                break;

            case ChallengeMissionType.VIP_CLUB:
                Serialize(bb, (ChallengeMissionValueEmpty)challengeMissionV1.info);
                break;

            case ChallengeMissionType.WIN_TARGETED:
                Serialize(bb, (ChallengeMissionValueWinTargeted)challengeMissionV1.info);
                break;

            case ChallengeMissionType.SPIN_TARGETED:
                Serialize(bb, (ChallengeMissionValueSpinTargeted)challengeMissionV1.info);
                break;

            case ChallengeMissionType.WIN_BIG_WIN_TARGETED:
                Serialize(bb, (ChallengeMissionValueWinBigWinTargeted)challengeMissionV1.info);
                break;

            case ChallengeMissionType.ENTER_FREE_SPIN_TARGETED:
                Serialize(bb, (ChallengeMissionValueEnterFreeSpinTargeted)challengeMissionV1.info);
                break;

            case ChallengeMissionType.WIN_CREDIT_MORE_THAN_TARGETED:
                Serialize(bb, (ChallengeMissionValueWinCreditMoreThanTargeted)challengeMissionV1.info);
                break;

            case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_TARGETED:
                Serialize(bb, (ChallengeMissionValueAchieveTotalWinCreditTargeted)challengeMissionV1.info);
                break;

            case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BIG_WIN_TARGETED:
                Serialize(bb, (ChallengeMissionValueAchieveTotalWinCreditByBigWinTargeted)challengeMissionV1.info);
                break;

            case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_FREE_SPIN_TARGETED:
                Serialize(bb, (ChallengeMissionValueAchieveTotalWinCreditByFreeSpinTargeted)challengeMissionV1.info);
                break;

            case ChallengeMissionType.LUCKY_FIVE_COLLECT_CARDS:
                Serialize(bb, (ChallengeMissionValueEmpty)challengeMissionV1.info);
                break;

            case ChallengeMissionType.LUCKY_FIVE_COLLECT_WILD_CARDS:
                Serialize(bb, (ChallengeMissionValueEmpty)challengeMissionV1.info);
                break;

            case ChallengeMissionType.COLLECTING_GAME_COLLECT_CHESTS:
                Serialize(bb, (ChallengeMissionValueCollectingGameCollectChests)challengeMissionV1.info);
                break;

            case ChallengeMissionType.COLLECTING_GAME_SCRATCH_SCRATCHER:
                Serialize(bb, (ChallengeMissionValueCollectingGameScratchScratcher)challengeMissionV1.info);
                break;

            case ChallengeMissionType.HIDDEN_UNIVERSE_COLLECT_FINDERS:
                Serialize(bb, (ChallengeMissionValueEmpty)challengeMissionV1.info);
                break;

            case ChallengeMissionType.GEM_JACKPOT_SPIN:
                Serialize(bb, (ChallengeMissionValueEmpty)challengeMissionV1.info);
                break;
            }
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "info");
        }
        BlackboardUtils.SetOrCreateValue(bb, "completeCount", challengeMissionV1.completeCount);
        if (challengeMissionV1.reward != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "reward"), challengeMissionV1.reward);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "reward");
        }
        BlackboardUtils.SetOrCreateValue(bb, "progress", challengeMissionV1.progress);
        BlackboardUtils.SetOrCreateValue(bb, "done", challengeMissionV1.done);
        BlackboardUtils.SetOrCreateValue(bb, "id", challengeMissionV1.id);
    }

    public static void Serialize(IBlackboard bb, ChallengeMissionValueAchieveTotalWinCreditByBigWinAny challengeMissionValueAchieveTotalWinCreditByBigWinAny)
    {
        if (challengeMissionValueAchieveTotalWinCreditByBigWinAny == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "winType", challengeMissionValueAchieveTotalWinCreditByBigWinAny.winType);
    }

    public static void Serialize(IBlackboard bb, ChallengeMissionValueAchieveTotalWinCreditByBigWinTargeted challengeMissionValueAchieveTotalWinCreditByBigWinTargeted)
    {
        if (challengeMissionValueAchieveTotalWinCreditByBigWinTargeted == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", challengeMissionValueAchieveTotalWinCreditByBigWinTargeted.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "winType", challengeMissionValueAchieveTotalWinCreditByBigWinTargeted.winType);
    }

    public static void Serialize(IBlackboard bb, ChallengeMissionValueAchieveTotalWinCreditByFreeSpinTargeted challengeMissionValueAchieveTotalWinCreditByFreeSpinTargeted)
    {
        if (challengeMissionValueAchieveTotalWinCreditByFreeSpinTargeted == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", challengeMissionValueAchieveTotalWinCreditByFreeSpinTargeted.gameId);
    }

    public static void Serialize(IBlackboard bb, ChallengeMissionValueAchieveTotalWinCreditTargeted challengeMissionValueAchieveTotalWinCreditTargeted)
    {
        if (challengeMissionValueAchieveTotalWinCreditTargeted == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", challengeMissionValueAchieveTotalWinCreditTargeted.gameId);
    }

    public static void Serialize(IBlackboard bb, ChallengeMissionValueBetLimit challengeMissionValueBetLimit)
    {
        if (challengeMissionValueBetLimit == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "betLimit", challengeMissionValueBetLimit.betLimit);
    }

    public static void Serialize(IBlackboard bb, ChallengeMissionValueCollectingGameCollectChests challengeMissionValueCollectingGameCollectChests)
    {
        if (challengeMissionValueCollectingGameCollectChests == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "collectingGameId", challengeMissionValueCollectingGameCollectChests.collectingGameId);
    }

    public static void Serialize(IBlackboard bb, ChallengeMissionValueCollectingGameScratchScratcher challengeMissionValueCollectingGameScratchScratcher)
    {
        if (challengeMissionValueCollectingGameScratchScratcher == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "collectingGameId", challengeMissionValueCollectingGameScratchScratcher.collectingGameId);
    }

    public static void Serialize(IBlackboard bb, ChallengeMissionValueEmpty challengeMissionValueEmpty)
    {
        if (challengeMissionValueEmpty == null) { return; }
    }

    public static void Serialize(IBlackboard bb, ChallengeMissionValueEnterFreeSpinTargeted challengeMissionValueEnterFreeSpinTargeted)
    {
        if (challengeMissionValueEnterFreeSpinTargeted == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", challengeMissionValueEnterFreeSpinTargeted.gameId);
    }

    public static void Serialize(IBlackboard bb, ChallengeMissionValuePurchasePriceMoreThan challengeMissionValuePurchasePriceMoreThan)
    {
        if (challengeMissionValuePurchasePriceMoreThan == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "targetPrice", challengeMissionValuePurchasePriceMoreThan.targetPrice);
    }

    public static void Serialize(IBlackboard bb, ChallengeMissionValueSpinTargeted challengeMissionValueSpinTargeted)
    {
        if (challengeMissionValueSpinTargeted == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", challengeMissionValueSpinTargeted.gameId);
    }

    public static void Serialize(IBlackboard bb, ChallengeMissionValueWatchVideoAds challengeMissionValueWatchVideoAds)
    {
        if (challengeMissionValueWatchVideoAds == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "consecutiveCount", challengeMissionValueWatchVideoAds.consecutiveCount);
    }

    public static void Serialize(IBlackboard bb, ChallengeMissionValueWinBigWinAny challengeMissionValueWinBigWinAny)
    {
        if (challengeMissionValueWinBigWinAny == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "winType", challengeMissionValueWinBigWinAny.winType);
    }

    public static void Serialize(IBlackboard bb, ChallengeMissionValueWinBigWinTargeted challengeMissionValueWinBigWinTargeted)
    {
        if (challengeMissionValueWinBigWinTargeted == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", challengeMissionValueWinBigWinTargeted.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "winType", challengeMissionValueWinBigWinTargeted.winType);
    }

    public static void Serialize(IBlackboard bb, ChallengeMissionValueWinCreditMoreThanAny challengeMissionValueWinCreditMoreThanAny)
    {
        if (challengeMissionValueWinCreditMoreThanAny == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "targetCredit", challengeMissionValueWinCreditMoreThanAny.targetCredit);
    }

    public static void Serialize(IBlackboard bb, ChallengeMissionValueWinCreditMoreThanTargeted challengeMissionValueWinCreditMoreThanTargeted)
    {
        if (challengeMissionValueWinCreditMoreThanTargeted == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", challengeMissionValueWinCreditMoreThanTargeted.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "targetCredit", challengeMissionValueWinCreditMoreThanTargeted.targetCredit);
    }

    public static void Serialize(IBlackboard bb, ChallengeMissionValueWinTargeted challengeMissionValueWinTargeted)
    {
        if (challengeMissionValueWinTargeted == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", challengeMissionValueWinTargeted.gameId);
    }

    public static void Serialize(IBlackboard bb, ChatBanInfo chatBanInfo)
    {
        if (chatBanInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "isPermanent", chatBanInfo.isPermanent);
        BlackboardUtils.SetOrCreateValue(bb, "banEndTimestamp", chatBanInfo.banEndTimestamp);
    }

    public static void Serialize(IBlackboard bb, ChatDataBossRaidersBossKill chatDataBossRaidersBossKill)
    {
        if (chatDataBossRaidersBossKill == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "round", chatDataBossRaidersBossKill.round);
        BlackboardUtils.SetOrCreateValue(bb, "themeId", chatDataBossRaidersBossKill.themeId);
    }

    public static void Serialize(IBlackboard bb, ChatDataBossRaidersLeadingAttacker chatDataBossRaidersLeadingAttacker)
    {
        if (chatDataBossRaidersLeadingAttacker == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "clubMemberRank", chatDataBossRaidersLeadingAttacker.clubMemberRank);
        BlackboardUtils.SetOrCreateValue(bb, "themeId", chatDataBossRaidersLeadingAttacker.themeId);
    }

    public static void Serialize(IBlackboard bb, ChatDataBossRaidersRankingUp chatDataBossRaidersRankingUp)
    {
        if (chatDataBossRaidersRankingUp == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "clubRank", chatDataBossRaidersRankingUp.clubRank);
        BlackboardUtils.SetOrCreateValue(bb, "themeId", chatDataBossRaidersRankingUp.themeId);
    }

    public static void Serialize(IBlackboard bb, ChatDataClubArenaClubRankingUp chatDataClubArenaClubRankingUp)
    {
        if (chatDataClubArenaClubRankingUp == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "clubRank", chatDataClubArenaClubRankingUp.clubRank);
    }

    public static void Serialize(IBlackboard bb, ChatDataClubArenaLeadingClubMember chatDataClubArenaLeadingClubMember)
    {
        if (chatDataClubArenaLeadingClubMember == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "clubMemberRank", chatDataClubArenaLeadingClubMember.clubMemberRank);
    }

    public static void Serialize(IBlackboard bb, ChatDataClubPr chatDataClubPr)
    {
        if (chatDataClubPr == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "clubId", chatDataClubPr.clubId);
        BlackboardUtils.SetOrCreateValue(bb, "clubName", chatDataClubPr.clubName);
        BlackboardUtils.SetOrCreateValue(bb, "clubSymbol", chatDataClubPr.clubSymbol);
    }

    public static void Serialize(IBlackboard bb, ChatDataEmoji chatDataEmoji)
    {
        if (chatDataEmoji == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "emoji", chatDataEmoji.emoji);
    }

    public static void Serialize(IBlackboard bb, ChatDataInstant chatDataInstant)
    {
        if (chatDataInstant == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "instant", chatDataInstant.instant);
    }

    public static void Serialize(IBlackboard bb, ChatDataMessage chatDataMessage)
    {
        if (chatDataMessage == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "message", chatDataMessage.message);
    }

    public static void Serialize(IBlackboard bb, ChatInitTokenRequest chatInitTokenRequest)
    {
        if (chatInitTokenRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "userId", chatInitTokenRequest.userId);
    }

    public static void Serialize(IBlackboard bb, ChatInitTokenResponse chatInitTokenResponse)
    {
        if (chatInitTokenResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", chatInitTokenResponse.error);
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", chatInitTokenResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "token", chatInitTokenResponse.token);
    }

    public static void Serialize(IBlackboard bb, ChatMuteRequest chatMuteRequest)
    {
        if (chatMuteRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", chatMuteRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", chatMuteRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "targetUserId", chatMuteRequest.targetUserId);
        BlackboardUtils.SetOrCreateValue(bb, "mute", chatMuteRequest.mute);
    }

    public static void Serialize(IBlackboard bb, ChatMuteResponse chatMuteResponse)
    {
        if (chatMuteResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", chatMuteResponse.error);
        if (chatMuteResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), chatMuteResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", chatMuteResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "muteUserIdList", chatMuteResponse.muteUserIdList);
    }

    public static void Serialize(IBlackboard bb, ChatPoll chatPoll)
    {
        if (chatPoll == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "id", chatPoll.id);
        BlackboardUtils.SetOrCreateValue(bb, "channelId", chatPoll.channelId);
        BlackboardUtils.SetOrCreateValue(bb, "userId", chatPoll.userId);
        if (chatPoll.profile != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "profile"), chatPoll.profile);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "profile");
        }
        BlackboardUtils.SetOrCreateValue(bb, "type", chatPoll.type);
        if (chatPoll.data != null)
        {
            switch (chatPoll.type)
            {
            case ChatType.MESSAGE:
                Serialize(bb, (ChatDataMessage)chatPoll.data);
                break;

            case ChatType.INSTANT:
                Serialize(bb, (ChatDataInstant)chatPoll.data);
                break;

            case ChatType.EMOJI:
                Serialize(bb, (ChatDataEmoji)chatPoll.data);
                break;

            case ChatType.CLUB_PR:
                Serialize(bb, (ChatDataClubPr)chatPoll.data);
                break;

            case ChatType.BOSS_RAIDERS_BOSS_KILL:
                Serialize(bb, (ChatDataBossRaidersBossKill)chatPoll.data);
                break;

            case ChatType.BOSS_RAIDERS_RANKING_UP:
                Serialize(bb, (ChatDataBossRaidersRankingUp)chatPoll.data);
                break;

            case ChatType.BOSS_RAIDERS_LEADING_ATTACKER:
                Serialize(bb, (ChatDataBossRaidersLeadingAttacker)chatPoll.data);
                break;

            case ChatType.CLUB_ARENA_CLUB_RANKING_UP:
                Serialize(bb, (ChatDataClubArenaClubRankingUp)chatPoll.data);
                break;

            case ChatType.CLUB_ARENA_LEADING_CLUB_MEMBER:
                Serialize(bb, (ChatDataClubArenaLeadingClubMember)chatPoll.data);
                break;
            }
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "data");
        }
        BlackboardUtils.SetOrCreateValue(bb, "requestId", chatPoll.requestId);
    }

    public static void Serialize(IBlackboard bb, ChatPollRequest chatPollRequest)
    {
        if (chatPollRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "lastReceivedId", chatPollRequest.lastReceivedId);
    }

    public static void Serialize(IBlackboard bb, ChatPollResponse chatPollResponse)
    {
        if (chatPollResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", chatPollResponse.error);
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", chatPollResponse.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "dataList", chatPollResponse.dataList, Serialize);
    }

    public static void Serialize(IBlackboard bb, ChatPostMessageRequestV1 chatPostMessageRequestV1)
    {
        if (chatPostMessageRequestV1 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "channelId", chatPostMessageRequestV1.channelId);
        BlackboardUtils.SetOrCreateValue(bb, "userId", chatPostMessageRequestV1.userId);
        BlackboardUtils.SetOrCreateValue(bb, "type", chatPostMessageRequestV1.type);
        if (chatPostMessageRequestV1.data != null)
        {
            switch (chatPostMessageRequestV1.type)
            {
            case ChatType.MESSAGE:
                Serialize(bb, (ChatDataMessage)chatPostMessageRequestV1.data);
                break;

            case ChatType.INSTANT:
                Serialize(bb, (ChatDataInstant)chatPostMessageRequestV1.data);
                break;

            case ChatType.EMOJI:
                Serialize(bb, (ChatDataEmoji)chatPostMessageRequestV1.data);
                break;

            case ChatType.CLUB_PR:
                Serialize(bb, (ChatDataClubPr)chatPostMessageRequestV1.data);
                break;

            case ChatType.BOSS_RAIDERS_BOSS_KILL:
                Serialize(bb, (ChatDataBossRaidersBossKill)chatPostMessageRequestV1.data);
                break;

            case ChatType.BOSS_RAIDERS_RANKING_UP:
                Serialize(bb, (ChatDataBossRaidersRankingUp)chatPostMessageRequestV1.data);
                break;

            case ChatType.BOSS_RAIDERS_LEADING_ATTACKER:
                Serialize(bb, (ChatDataBossRaidersLeadingAttacker)chatPostMessageRequestV1.data);
                break;

            case ChatType.CLUB_ARENA_CLUB_RANKING_UP:
                Serialize(bb, (ChatDataClubArenaClubRankingUp)chatPostMessageRequestV1.data);
                break;

            case ChatType.CLUB_ARENA_LEADING_CLUB_MEMBER:
                Serialize(bb, (ChatDataClubArenaLeadingClubMember)chatPostMessageRequestV1.data);
                break;
            }
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "data");
        }
        BlackboardUtils.SetOrCreateValue(bb, "contextId", chatPostMessageRequestV1.contextId);
        BlackboardUtils.SetOrCreateValue(bb, "requestId", chatPostMessageRequestV1.requestId);
    }

    public static void Serialize(IBlackboard bb, ChatPostMessageResponse chatPostMessageResponse)
    {
        if (chatPostMessageResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", chatPostMessageResponse.error);
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", chatPostMessageResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "messageId", chatPostMessageResponse.messageId);
    }

    public static void Serialize(IBlackboard bb, ChatPostSystemMessageRequestV1 chatPostSystemMessageRequestV1)
    {
        if (chatPostSystemMessageRequestV1 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "channelId", chatPostSystemMessageRequestV1.channelId);
        BlackboardUtils.SetOrCreateValue(bb, "userId", chatPostSystemMessageRequestV1.userId);
        BlackboardUtils.SetOrCreateValue(bb, "type", chatPostSystemMessageRequestV1.type);
        if (chatPostSystemMessageRequestV1.data != null)
        {
            switch (chatPostSystemMessageRequestV1.type)
            {
            case ChatType.MESSAGE:
                Serialize(bb, (ChatDataMessage)chatPostSystemMessageRequestV1.data);
                break;

            case ChatType.INSTANT:
                Serialize(bb, (ChatDataInstant)chatPostSystemMessageRequestV1.data);
                break;

            case ChatType.EMOJI:
                Serialize(bb, (ChatDataEmoji)chatPostSystemMessageRequestV1.data);
                break;

            case ChatType.CLUB_PR:
                Serialize(bb, (ChatDataClubPr)chatPostSystemMessageRequestV1.data);
                break;

            case ChatType.BOSS_RAIDERS_BOSS_KILL:
                Serialize(bb, (ChatDataBossRaidersBossKill)chatPostSystemMessageRequestV1.data);
                break;

            case ChatType.BOSS_RAIDERS_RANKING_UP:
                Serialize(bb, (ChatDataBossRaidersRankingUp)chatPostSystemMessageRequestV1.data);
                break;

            case ChatType.BOSS_RAIDERS_LEADING_ATTACKER:
                Serialize(bb, (ChatDataBossRaidersLeadingAttacker)chatPostSystemMessageRequestV1.data);
                break;

            case ChatType.CLUB_ARENA_CLUB_RANKING_UP:
                Serialize(bb, (ChatDataClubArenaClubRankingUp)chatPostSystemMessageRequestV1.data);
                break;

            case ChatType.CLUB_ARENA_LEADING_CLUB_MEMBER:
                Serialize(bb, (ChatDataClubArenaLeadingClubMember)chatPostSystemMessageRequestV1.data);
                break;
            }
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "data");
        }
        BlackboardUtils.SetOrCreateValue(bb, "requestId", chatPostSystemMessageRequestV1.requestId);
    }

    public static void Serialize(IBlackboard bb, ChatRecentRequest chatRecentRequest)
    {
        if (chatRecentRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "channelId", chatRecentRequest.channelId);
        BlackboardUtils.SetOrCreateValue(bb, "lastMessageId", chatRecentRequest.lastMessageId);
    }

    public static void Serialize(IBlackboard bb, ChatReportRequestV1 chatReportRequestV1)
    {
        if (chatReportRequestV1 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", chatReportRequestV1.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", chatReportRequestV1.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "targetUserId", chatReportRequestV1.targetUserId);
        BlackboardUtils.SetOrCreateValue(bb, "channelId", chatReportRequestV1.channelId);
        BlackboardUtils.SetOrCreateValue(bb, "type", chatReportRequestV1.type);
        if (chatReportRequestV1.data != null)
        {
            switch (chatReportRequestV1.type)
            {
            case ChatType.MESSAGE:
                Serialize(bb, (ChatDataMessage)chatReportRequestV1.data);
                break;

            case ChatType.INSTANT:
                Serialize(bb, (ChatDataInstant)chatReportRequestV1.data);
                break;

            case ChatType.EMOJI:
                Serialize(bb, (ChatDataEmoji)chatReportRequestV1.data);
                break;

            case ChatType.CLUB_PR:
                Serialize(bb, (ChatDataClubPr)chatReportRequestV1.data);
                break;

            case ChatType.BOSS_RAIDERS_BOSS_KILL:
                Serialize(bb, (ChatDataBossRaidersBossKill)chatReportRequestV1.data);
                break;

            case ChatType.BOSS_RAIDERS_RANKING_UP:
                Serialize(bb, (ChatDataBossRaidersRankingUp)chatReportRequestV1.data);
                break;

            case ChatType.BOSS_RAIDERS_LEADING_ATTACKER:
                Serialize(bb, (ChatDataBossRaidersLeadingAttacker)chatReportRequestV1.data);
                break;

            case ChatType.CLUB_ARENA_CLUB_RANKING_UP:
                Serialize(bb, (ChatDataClubArenaClubRankingUp)chatReportRequestV1.data);
                break;

            case ChatType.CLUB_ARENA_LEADING_CLUB_MEMBER:
                Serialize(bb, (ChatDataClubArenaLeadingClubMember)chatReportRequestV1.data);
                break;
            }
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "data");
        }
    }

    public static void Serialize(IBlackboard bb, ChatReportResponse chatReportResponse)
    {
        if (chatReportResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", chatReportResponse.error);
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", chatReportResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "muteUserIdList", chatReportResponse.muteUserIdList);
    }

    public static void Serialize(IBlackboard bb, ChatSimpleResponse chatSimpleResponse)
    {
        if (chatSimpleResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", chatSimpleResponse.error);
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", chatSimpleResponse.serverTime);
    }

    public static void Serialize(IBlackboard bb, ChatSubscribeRequest chatSubscribeRequest)
    {
        if (chatSubscribeRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "channelId", chatSubscribeRequest.channelId);
    }

    public static void Serialize(IBlackboard bb, ChatSubscriberListRequest chatSubscriberListRequest)
    {
        if (chatSubscriberListRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "channelId", chatSubscriberListRequest.channelId);
    }

    public static void Serialize(IBlackboard bb, ChatSubscriberListResponse chatSubscriberListResponse)
    {
        if (chatSubscriberListResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", chatSubscriberListResponse.error);
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", chatSubscriberListResponse.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "profileList", chatSubscriberListResponse.profileList, Serialize);
    }

    public static void Serialize(IBlackboard bb, ChatUnsubscribeAllRequest chatUnsubscribeAllRequest)
    {
        if (chatUnsubscribeAllRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "userId", chatUnsubscribeAllRequest.userId);
    }

    public static void Serialize(IBlackboard bb, ChatUnsubscribeRequest chatUnsubscribeRequest)
    {
        if (chatUnsubscribeRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "channelId", chatUnsubscribeRequest.channelId);
    }

    public static void Serialize(IBlackboard bb, Club club)
    {
        if (club == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "MAX_MEMBERS", club.MAX_MEMBERS);
        BlackboardUtils.SetOrCreateValue(bb, "DAILY_MAX_DONATION_COUNT", club.DAILY_MAX_DONATION_COUNT);
        BlackboardUtils.SetOrCreateValue(bb, "FIRST_JOIN_REWARD", club.FIRST_JOIN_REWARD);
        BlackboardUtils.SetOrCreateValue(bb, "CREATION_COST", club.CREATION_COST);
        if (club.LEVEL != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "LEVEL"), club.LEVEL);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "LEVEL");
        }
        BlackboardUtils.SetOrCreateValue(bb, "DONATE_UNIT_CREDIT_LIST", club.DONATE_UNIT_CREDIT_LIST);
        BlackboardUtils.SetOrCreateValue(bb, "WATCHER_DAYS", club.WATCHER_DAYS);
        BlackboardUtils.SetOrCreateValue(bb, "WATCHER_DEFAULT_INDEX", club.WATCHER_DEFAULT_INDEX);
        BlackboardUtils.SetOrCreateValue(bb, "LEAGUE_POINT_BOOST", club.LEAGUE_POINT_BOOST);
        if (club.LEAGUE != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "LEAGUE"), club.LEAGUE);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "LEAGUE");
        }
        BlackboardUtils.SetOrCreateValue(bb, "MIN_PLAYER_LEVEL", club.MIN_PLAYER_LEVEL);
        BlackboardUtils.SetOrCreateValue(bb, "CLUB_NAME_MAX_LENGTH", club.CLUB_NAME_MAX_LENGTH);
        BlackboardUtils.SetOrCreateValue(bb, "CLUB_MOTD_MAX_LENGTH", club.CLUB_MOTD_MAX_LENGTH);
        BlackboardUtils.SetOrCreateValue(bb, "FEED_MESSAGE_MAX_LENGTH", club.FEED_MESSAGE_MAX_LENGTH);
        BlackboardUtils.SetOrCreateValue(bb, "CLUB_NOTICE_MAX_LENGTH", club.CLUB_NOTICE_MAX_LENGTH);
        BlackboardUtils.SetOrCreateValue(bb, "COLEADER_LIMIT", club.COLEADER_LIMIT);
        BlackboardUtils.SetOrCreateValue(bb, "SEARCH_NAME_MIN_LENGTH", club.SEARCH_NAME_MIN_LENGTH);
    }

    public static void Serialize(IBlackboard bb, ClubArenaAdsClaimRequest clubArenaAdsClaimRequest)
    {
        if (clubArenaAdsClaimRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", clubArenaAdsClaimRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", clubArenaAdsClaimRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "eventId", clubArenaAdsClaimRequest.eventId);
        BlackboardUtils.SetOrCreateValue(bb, "contextId", clubArenaAdsClaimRequest.contextId);
    }

    public static void Serialize(IBlackboard bb, ClubArenaAdsClaimResponse clubArenaAdsClaimResponse)
    {
        if (clubArenaAdsClaimResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", clubArenaAdsClaimResponse.error);
        if (clubArenaAdsClaimResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), clubArenaAdsClaimResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", clubArenaAdsClaimResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "cooltime", clubArenaAdsClaimResponse.cooltime);
        BlackboardUtils.SetOrCreateValue(bb, "lastVideoAdsClaimTimestamp", clubArenaAdsClaimResponse.lastVideoAdsClaimTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "energy", clubArenaAdsClaimResponse.energy);
    }

    public static void Serialize(IBlackboard bb, ClubArenaAttackRequest clubArenaAttackRequest)
    {
        if (clubArenaAttackRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", clubArenaAttackRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", clubArenaAttackRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "eventId", clubArenaAttackRequest.eventId);
        BlackboardUtils.SetOrCreateValue(bb, "isSteal", clubArenaAttackRequest.isSteal);
        BlackboardUtils.SetOrCreateValue(bb, "attack", clubArenaAttackRequest.attack);
        BlackboardUtils.SetOrCreateValue(bb, "multiplier", clubArenaAttackRequest.multiplier);
        BlackboardUtils.SetOrCreateValue(bb, "opponentUserId", clubArenaAttackRequest.opponentUserId);
        BlackboardUtils.SetOrCreateValue(bb, "clientOpponentPoint", clubArenaAttackRequest.clientOpponentPoint);
        BlackboardUtils.SetOrCreateValue(bb, "isAutoSpin", clubArenaAttackRequest.isAutoSpin);
        BlackboardUtils.SetOrCreateValue(bb, "enterContextId", clubArenaAttackRequest.enterContextId);
        BlackboardUtils.SetOrCreateValue(bb, "matchContextId", clubArenaAttackRequest.matchContextId);
    }

    public static void Serialize(IBlackboard bb, ClubArenaAttackResponse clubArenaAttackResponse)
    {
        if (clubArenaAttackResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", clubArenaAttackResponse.error);
        if (clubArenaAttackResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), clubArenaAttackResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", clubArenaAttackResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "addedPoint", clubArenaAttackResponse.addedPoint);
        BlackboardUtils.SetOrCreateValue(bb, "opponentHasShield", clubArenaAttackResponse.opponentHasShield);
        if (clubArenaAttackResponse.newOpponentState != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "newOpponentState"), clubArenaAttackResponse.newOpponentState);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "newOpponentState");
        }
    }

    public static void Serialize(IBlackboard bb, ClubArenaBonusInfo clubArenaBonusInfo)
    {
        if (clubArenaBonusInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "bonusType", clubArenaBonusInfo.bonusType);
        BlackboardUtils.SetOrCreateValue(bb, "amount", clubArenaBonusInfo.amount);
    }

    public static void Serialize(IBlackboard bb, ClubArenaChestImageChangePoint clubArenaChestImageChangePoint)
    {
        if (clubArenaChestImageChangePoint == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "middle", clubArenaChestImageChangePoint.middle);
        BlackboardUtils.SetOrCreateValue(bb, "high", clubArenaChestImageChangePoint.high);
    }

    public static void Serialize(IBlackboard bb, ClubArenaClubRanking clubArenaClubRanking)
    {
        if (clubArenaClubRanking == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "clubId", clubArenaClubRanking.clubId);
        BlackboardUtils.SetOrCreateValue(bb, "clubName", clubArenaClubRanking.clubName);
        BlackboardUtils.SetOrCreateValue(bb, "clubSymbol", clubArenaClubRanking.clubSymbol);
        BlackboardUtils.SetOrCreateValue(bb, "ranking", clubArenaClubRanking.ranking);
        BlackboardUtils.SetOrCreateValue(bb, "totalPoint", clubArenaClubRanking.totalPoint);
    }

    public static void Serialize(IBlackboard bb, ClubArenaContribution clubArenaContribution)
    {
        if (clubArenaContribution == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "userId", clubArenaContribution.userId);
        BlackboardUtils.SetOrCreateValue(bb, "name", clubArenaContribution.name);
        BlackboardUtils.SetOrCreateValue(bb, "profileUrl", clubArenaContribution.profileUrl);
        BlackboardUtils.SetOrCreateValue(bb, "tier", clubArenaContribution.tier);
        BlackboardUtils.SetOrCreateValue(bb, "point", clubArenaContribution.point);
    }

    public static void Serialize(IBlackboard bb, ClubArenaDebugSpinRequest clubArenaDebugSpinRequest)
    {
        if (clubArenaDebugSpinRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", clubArenaDebugSpinRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", clubArenaDebugSpinRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "eventId", clubArenaDebugSpinRequest.eventId);
        BlackboardUtils.SetOrCreateValue(bb, "betMultiplyNumerator", clubArenaDebugSpinRequest.betMultiplyNumerator);
        BlackboardUtils.SetOrCreateValue(bb, "isAutoSpin", clubArenaDebugSpinRequest.isAutoSpin);
        BlackboardUtils.SetOrCreateValue(bb, "clientShield", clubArenaDebugSpinRequest.clientShield);
        BlackboardUtils.SetOrCreateValue(bb, "matchContextId", clubArenaDebugSpinRequest.matchContextId);
        BlackboardUtils.SetOrCreateValue(bb, "debugSpinResult", clubArenaDebugSpinRequest.debugSpinResult);
    }

    public static void Serialize(IBlackboard bb, ClubArenaEnergyBundleInfo clubArenaEnergyBundleInfo)
    {
        if (clubArenaEnergyBundleInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "bet", clubArenaEnergyBundleInfo.bet);
        BlackboardUtils.SetOrCreateValue(bb, "bundleType", clubArenaEnergyBundleInfo.bundleType);
        BlackboardUtils.SetOrCreateValue(bb, "maximumEnergy", clubArenaEnergyBundleInfo.maximumEnergy);
    }

    public static void Serialize(IBlackboard bb, ClubArenaEnergyUpdateInfo clubArenaEnergyUpdateInfo)
    {
        if (clubArenaEnergyUpdateInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "energy", clubArenaEnergyUpdateInfo.energy);
        BlackboardUtils.SetOrCreateValue(bb, "energyEarning", clubArenaEnergyUpdateInfo.energyEarning);
    }

    public static void Serialize(IBlackboard bb, ClubArenaEnterInfo clubArenaEnterInfo)
    {
        if (clubArenaEnterInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "energy", clubArenaEnterInfo.energy);
        BlackboardUtils.SetOrCreateValue(bb, "energyForSpin", clubArenaEnterInfo.energyForSpin);
        BlackboardUtils.SetOrCreateList(bb, "energyBundleInfoList", clubArenaEnterInfo.energyBundleInfoList, Serialize);
    }

    public static void Serialize(IBlackboard bb, ClubArenaEnterRequest clubArenaEnterRequest)
    {
        if (clubArenaEnterRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", clubArenaEnterRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", clubArenaEnterRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "eventId", clubArenaEnterRequest.eventId);
        BlackboardUtils.SetOrCreateValue(bb, "isFirstEnterAfterLogin", clubArenaEnterRequest.isFirstEnterAfterLogin);
        BlackboardUtils.SetOrCreateValue(bb, "targetOpponentUserId", clubArenaEnterRequest.targetOpponentUserId);
        BlackboardUtils.SetOrCreateValue(bb, "enterContextId", clubArenaEnterRequest.enterContextId);
    }

    public static void Serialize(IBlackboard bb, ClubArenaEnterResponse clubArenaEnterResponse)
    {
        if (clubArenaEnterResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", clubArenaEnterResponse.error);
        if (clubArenaEnterResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), clubArenaEnterResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", clubArenaEnterResponse.serverTime);
        if (clubArenaEnterResponse.myState != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "myState"), clubArenaEnterResponse.myState);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "myState");
        }
        if (clubArenaEnterResponse.opponentState != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "opponentState"), clubArenaEnterResponse.opponentState);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "opponentState");
        }
        BlackboardUtils.SetOrCreateList(bb, "revengeList", clubArenaEnterResponse.revengeList, Serialize);
        if (clubArenaEnterResponse.pointTaken != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "pointTaken"), clubArenaEnterResponse.pointTaken);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "pointTaken");
        }
        BlackboardUtils.SetOrCreateList(bb, "clubMemberContribution", clubArenaEnterResponse.clubMemberContribution, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "clubRankingList", clubArenaEnterResponse.clubRankingList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "clubRank", clubArenaEnterResponse.clubRank);
        BlackboardUtils.SetOrCreateValue(bb, "percentile", clubArenaEnterResponse.percentile);
        BlackboardUtils.SetOrCreateValue(bb, "lastVideoAdsClaimTimestamp", clubArenaEnterResponse.lastVideoAdsClaimTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "videoAdsCooltime", clubArenaEnterResponse.videoAdsCooltime);
        BlackboardUtils.SetOrCreateValue(bb, "maximumShield", clubArenaEnterResponse.maximumShield);
        BlackboardUtils.SetOrCreateValue(bb, "stealPercentage", clubArenaEnterResponse.stealPercentage);
        BlackboardUtils.SetOrCreateValue(bb, "shieldAttackPoint", clubArenaEnterResponse.shieldAttackPoint);
        BlackboardUtils.SetOrCreateValue(bb, "finalRewardRank", clubArenaEnterResponse.finalRewardRank);
        BlackboardUtils.SetOrCreateValue(bb, "finalRewardPercentile", clubArenaEnterResponse.finalRewardPercentile);
        if (clubArenaEnterResponse.chestImageChangePoint != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "chestImageChangePoint"), clubArenaEnterResponse.chestImageChangePoint);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "chestImageChangePoint");
        }
        BlackboardUtils.SetOrCreateValue(bb, "defaultBetMultiplyNumeratorByEnergy", clubArenaEnterResponse.defaultBetMultiplyNumeratorByEnergy);
        BlackboardUtils.SetOrCreateValue(bb, "maxBetMultiplyNumeratorByEnergyList", clubArenaEnterResponse.maxBetMultiplyNumeratorByEnergyList);
        BlackboardUtils.SetOrCreateList(bb, "wheelCandidateList", clubArenaEnterResponse.wheelCandidateList, Serialize);
        if (clubArenaEnterResponse.clubInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "clubInfo"), clubArenaEnterResponse.clubInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "clubInfo");
        }
    }

    public static void Serialize(IBlackboard bb, ClubArenaHelpRequest clubArenaHelpRequest)
    {
        if (clubArenaHelpRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", clubArenaHelpRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", clubArenaHelpRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "eventId", clubArenaHelpRequest.eventId);
        BlackboardUtils.SetOrCreateValue(bb, "mostTakenUserId", clubArenaHelpRequest.mostTakenUserId);
    }

    public static void Serialize(IBlackboard bb, ClubArenaPersonal clubArenaPersonal)
    {
        if (clubArenaPersonal == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "userId", clubArenaPersonal.userId);
        BlackboardUtils.SetOrCreateValue(bb, "eventId", clubArenaPersonal.eventId);
        BlackboardUtils.SetOrCreateValue(bb, "point", clubArenaPersonal.point);
        BlackboardUtils.SetOrCreateValue(bb, "energy", clubArenaPersonal.energy);
        BlackboardUtils.SetOrCreateValue(bb, "shield", clubArenaPersonal.shield);
        BlackboardUtils.SetOrCreateValue(bb, "lastEnterTimestamp", clubArenaPersonal.lastEnterTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "opponentUserId", clubArenaPersonal.opponentUserId);
        BlackboardUtils.SetOrCreateValue(bb, "lastMatchingTimestamp", clubArenaPersonal.lastMatchingTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "leftHelpPopupCount", clubArenaPersonal.leftHelpPopupCount);
        BlackboardUtils.SetOrCreateValue(bb, "lastPushTimestamp", clubArenaPersonal.lastPushTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "leftPushCount", clubArenaPersonal.leftPushCount);
    }

    public static void Serialize(IBlackboard bb, ClubArenaPersonalSimpleWithProfile clubArenaPersonalSimpleWithProfile)
    {
        if (clubArenaPersonalSimpleWithProfile == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "userId", clubArenaPersonalSimpleWithProfile.userId);
        BlackboardUtils.SetOrCreateValue(bb, "point", clubArenaPersonalSimpleWithProfile.point);
        BlackboardUtils.SetOrCreateValue(bb, "energy", clubArenaPersonalSimpleWithProfile.energy);
        BlackboardUtils.SetOrCreateValue(bb, "shield", clubArenaPersonalSimpleWithProfile.shield);
        BlackboardUtils.SetOrCreateValue(bb, "name", clubArenaPersonalSimpleWithProfile.name);
        BlackboardUtils.SetOrCreateValue(bb, "profileUrl", clubArenaPersonalSimpleWithProfile.profileUrl);
        BlackboardUtils.SetOrCreateValue(bb, "clubId", clubArenaPersonalSimpleWithProfile.clubId);
        BlackboardUtils.SetOrCreateValue(bb, "tier", clubArenaPersonalSimpleWithProfile.tier);
        BlackboardUtils.SetOrCreateValue(bb, "matchContextId", clubArenaPersonalSimpleWithProfile.matchContextId);
    }

    public static void Serialize(IBlackboard bb, ClubArenaPointTaken clubArenaPointTaken)
    {
        if (clubArenaPointTaken == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "takenAmountSum", clubArenaPointTaken.takenAmountSum);
        BlackboardUtils.SetOrCreateValue(bb, "takenUserCount", clubArenaPointTaken.takenUserCount);
        BlackboardUtils.SetOrCreateValue(bb, "mostTakenUserId", clubArenaPointTaken.mostTakenUserId);
        BlackboardUtils.SetOrCreateValue(bb, "mostTakenUserName", clubArenaPointTaken.mostTakenUserName);
        BlackboardUtils.SetOrCreateValue(bb, "mostTakenUserProfileUrl", clubArenaPointTaken.mostTakenUserProfileUrl);
        BlackboardUtils.SetOrCreateValue(bb, "mostTakenAmount", clubArenaPointTaken.mostTakenAmount);
    }

    public static void Serialize(IBlackboard bb, ClubArenaRankingPopupInfo clubArenaRankingPopupInfo)
    {
        if (clubArenaRankingPopupInfo == null) { return; }
        BlackboardUtils.SetOrCreateList(bb, "clubRanking", clubArenaRankingPopupInfo.clubRanking, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "backgroundImageUrl", clubArenaRankingPopupInfo.backgroundImageUrl);
    }

    public static void Serialize(IBlackboard bb, ClubArenaRevengeEntity clubArenaRevengeEntity)
    {
        if (clubArenaRevengeEntity == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "userId", clubArenaRevengeEntity.userId);
        BlackboardUtils.SetOrCreateValue(bb, "name", clubArenaRevengeEntity.name);
        BlackboardUtils.SetOrCreateValue(bb, "profileUrl", clubArenaRevengeEntity.profileUrl);
        BlackboardUtils.SetOrCreateValue(bb, "takenPoint", clubArenaRevengeEntity.takenPoint);
        BlackboardUtils.SetOrCreateValue(bb, "takenTime", clubArenaRevengeEntity.takenTime);
        BlackboardUtils.SetOrCreateValue(bb, "isTakenBySteal", clubArenaRevengeEntity.isTakenBySteal);
    }

    public static void Serialize(IBlackboard bb, ClubArenaRevengeRequest clubArenaRevengeRequest)
    {
        if (clubArenaRevengeRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", clubArenaRevengeRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", clubArenaRevengeRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "eventId", clubArenaRevengeRequest.eventId);
        BlackboardUtils.SetOrCreateValue(bb, "opponentUserId", clubArenaRevengeRequest.opponentUserId);
        BlackboardUtils.SetOrCreateValue(bb, "takenTime", clubArenaRevengeRequest.takenTime);
        BlackboardUtils.SetOrCreateValue(bb, "enterContextId", clubArenaRevengeRequest.enterContextId);
        BlackboardUtils.SetOrCreateValue(bb, "revengeContextId", clubArenaRevengeRequest.revengeContextId);
    }

    public static void Serialize(IBlackboard bb, ClubArenaRevengeResponse clubArenaRevengeResponse)
    {
        if (clubArenaRevengeResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", clubArenaRevengeResponse.error);
        if (clubArenaRevengeResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), clubArenaRevengeResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", clubArenaRevengeResponse.serverTime);
        if (clubArenaRevengeResponse.opponentState != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "opponentState"), clubArenaRevengeResponse.opponentState);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "opponentState");
        }
    }

    public static void Serialize(IBlackboard bb, ClubArenaRewardPopupInfo clubArenaRewardPopupInfo)
    {
        if (clubArenaRewardPopupInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "point", clubArenaRewardPopupInfo.point);
        BlackboardUtils.SetOrCreateValue(bb, "gem", clubArenaRewardPopupInfo.gem);
        BlackboardUtils.SetOrCreateValue(bb, "rank", clubArenaRewardPopupInfo.rank);
        BlackboardUtils.SetOrCreateValue(bb, "percentile", clubArenaRewardPopupInfo.percentile);
        BlackboardUtils.SetOrCreateValue(bb, "backgroundImageUrl", clubArenaRewardPopupInfo.backgroundImageUrl);
        BlackboardUtils.SetOrCreateValue(bb, "eventId", clubArenaRewardPopupInfo.eventId);
    }

    public static void Serialize(IBlackboard bb, ClubArenaSpinRequest clubArenaSpinRequest)
    {
        if (clubArenaSpinRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", clubArenaSpinRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", clubArenaSpinRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "eventId", clubArenaSpinRequest.eventId);
        BlackboardUtils.SetOrCreateValue(bb, "betMultiplyNumerator", clubArenaSpinRequest.betMultiplyNumerator);
        BlackboardUtils.SetOrCreateValue(bb, "isAutoSpin", clubArenaSpinRequest.isAutoSpin);
        BlackboardUtils.SetOrCreateValue(bb, "clientShield", clubArenaSpinRequest.clientShield);
        BlackboardUtils.SetOrCreateValue(bb, "matchContextId", clubArenaSpinRequest.matchContextId);
    }

    public static void Serialize(IBlackboard bb, ClubArenaSpinResponse clubArenaSpinResponse)
    {
        if (clubArenaSpinResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", clubArenaSpinResponse.error);
        if (clubArenaSpinResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), clubArenaSpinResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", clubArenaSpinResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "wheelResultIndex", clubArenaSpinResponse.wheelResultIndex);
        if (clubArenaSpinResponse.result != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "result"), clubArenaSpinResponse.result);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "result");
        }
        BlackboardUtils.SetOrCreateValue(bb, "energy", clubArenaSpinResponse.energy);
        if (clubArenaSpinResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), clubArenaSpinResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
    }

    public static void Serialize(IBlackboard bb, ClubArenaSpinResult clubArenaSpinResult)
    {
        if (clubArenaSpinResult == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "type", clubArenaSpinResult.type);
        if (clubArenaSpinResult.result != null)
        {
            switch (clubArenaSpinResult.type)
            {
            case ClubArenaSpinResultType.POINT_10:
                Serialize(bb, (ClubArenaSpinResultPoint)clubArenaSpinResult.result);
                break;

            case ClubArenaSpinResultType.POINT_50:
                Serialize(bb, (ClubArenaSpinResultPoint)clubArenaSpinResult.result);
                break;

            case ClubArenaSpinResultType.ATTACK_10:
                Serialize(bb, (ClubArenaSpinResultAttack)clubArenaSpinResult.result);
                break;

            case ClubArenaSpinResultType.ATTACK_20:
                Serialize(bb, (ClubArenaSpinResultAttack)clubArenaSpinResult.result);
                break;

            case ClubArenaSpinResultType.ATTACK_50:
                Serialize(bb, (ClubArenaSpinResultAttack)clubArenaSpinResult.result);
                break;

            case ClubArenaSpinResultType.ATTACK_100:
                Serialize(bb, (ClubArenaSpinResultAttack)clubArenaSpinResult.result);
                break;

            case ClubArenaSpinResultType.SHIELD:
                Serialize(bb, (ClubArenaSpinResultShield)clubArenaSpinResult.result);
                break;

            case ClubArenaSpinResultType.STEAL:
                Serialize(bb, (ClubArenaSpinResultSteal)clubArenaSpinResult.result);
                break;

            case ClubArenaSpinResultType.BONUS:
                Serialize(bb, (ClubArenaSpinResultBonus)clubArenaSpinResult.result);
                break;
            }
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "result");
        }
    }

    public static void Serialize(IBlackboard bb, ClubArenaSpinResultAttack clubArenaSpinResultAttack)
    {
        if (clubArenaSpinResultAttack == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "attack", clubArenaSpinResultAttack.attack);
    }

    public static void Serialize(IBlackboard bb, ClubArenaSpinResultBonus clubArenaSpinResultBonus)
    {
        if (clubArenaSpinResultBonus == null) { return; }
        BlackboardUtils.SetOrCreateList(bb, "bonusInfoList", clubArenaSpinResultBonus.bonusInfoList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "bonusIndex", clubArenaSpinResultBonus.bonusIndex);
    }

    public static void Serialize(IBlackboard bb, ClubArenaSpinResultPoint clubArenaSpinResultPoint)
    {
        if (clubArenaSpinResultPoint == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "point", clubArenaSpinResultPoint.point);
    }

    public static void Serialize(IBlackboard bb, ClubArenaSpinResultShield clubArenaSpinResultShield)
    {
        if (clubArenaSpinResultShield == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "shield", clubArenaSpinResultShield.shield);
        BlackboardUtils.SetOrCreateValue(bb, "energy", clubArenaSpinResultShield.energy);
    }

    public static void Serialize(IBlackboard bb, ClubArenaSpinResultSteal clubArenaSpinResultSteal)
    {
        if (clubArenaSpinResultSteal == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "percentage", clubArenaSpinResultSteal.percentage);
    }

    public static void Serialize(IBlackboard bb, ClubArenaWheelCandidate clubArenaWheelCandidate)
    {
        if (clubArenaWheelCandidate == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "type", clubArenaWheelCandidate.type);
        if (clubArenaWheelCandidate.info != null)
        {
            switch (clubArenaWheelCandidate.type)
            {
            case ClubArenaSpinResultType.POINT_10:
                Serialize(bb, (ClubArenaWheelCandidateInfoPoint)clubArenaWheelCandidate.info);
                break;

            case ClubArenaSpinResultType.POINT_50:
                Serialize(bb, (ClubArenaWheelCandidateInfoPoint)clubArenaWheelCandidate.info);
                break;

            case ClubArenaSpinResultType.ATTACK_10:
                Serialize(bb, (ClubArenaWheelCandidateInfoAttack)clubArenaWheelCandidate.info);
                break;

            case ClubArenaSpinResultType.ATTACK_20:
                Serialize(bb, (ClubArenaWheelCandidateInfoAttack)clubArenaWheelCandidate.info);
                break;

            case ClubArenaSpinResultType.ATTACK_50:
                Serialize(bb, (ClubArenaWheelCandidateInfoAttack)clubArenaWheelCandidate.info);
                break;

            case ClubArenaSpinResultType.ATTACK_100:
                Serialize(bb, (ClubArenaWheelCandidateInfoAttack)clubArenaWheelCandidate.info);
                break;

            case ClubArenaSpinResultType.SHIELD:
                Serialize(bb, (ClubArenaWheelCandidateInfoShield)clubArenaWheelCandidate.info);
                break;

            case ClubArenaSpinResultType.STEAL:
                Serialize(bb, (ClubArenaWheelCandidateInfoSteal)clubArenaWheelCandidate.info);
                break;

            case ClubArenaSpinResultType.BONUS:
                Serialize(bb, (ClubArenaWheelCandidateInfoBonus)clubArenaWheelCandidate.info);
                break;
            }
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "info");
        }
    }

    public static void Serialize(IBlackboard bb, ClubArenaWheelCandidateInfoAttack clubArenaWheelCandidateInfoAttack)
    {
        if (clubArenaWheelCandidateInfoAttack == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "attack", clubArenaWheelCandidateInfoAttack.attack);
    }

    public static void Serialize(IBlackboard bb, ClubArenaWheelCandidateInfoBonus clubArenaWheelCandidateInfoBonus)
    {
        if (clubArenaWheelCandidateInfoBonus == null) { return; }
    }

    public static void Serialize(IBlackboard bb, ClubArenaWheelCandidateInfoPoint clubArenaWheelCandidateInfoPoint)
    {
        if (clubArenaWheelCandidateInfoPoint == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "point", clubArenaWheelCandidateInfoPoint.point);
    }

    public static void Serialize(IBlackboard bb, ClubArenaWheelCandidateInfoShield clubArenaWheelCandidateInfoShield)
    {
        if (clubArenaWheelCandidateInfoShield == null) { return; }
    }

    public static void Serialize(IBlackboard bb, ClubArenaWheelCandidateInfoSteal clubArenaWheelCandidateInfoSteal)
    {
        if (clubArenaWheelCandidateInfoSteal == null) { return; }
    }

    public static void Serialize(IBlackboard bb, ClubBadgeInfo clubBadgeInfo)
    {
        if (clubBadgeInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "latestFeedRecordId", clubBadgeInfo.latestFeedRecordId);
        BlackboardUtils.SetOrCreateValue(bb, "newStuffExists", clubBadgeInfo.newStuffExists);
        BlackboardUtils.SetOrCreateValue(bb, "latestRewardFeedRecordId", clubBadgeInfo.latestRewardFeedRecordId);
        BlackboardUtils.SetOrCreateValue(bb, "leagueEndTimestamp", clubBadgeInfo.leagueEndTimestamp);
    }

    public static void Serialize(IBlackboard bb, ClubChallengeInfo clubChallengeInfo)
    {
        if (clubChallengeInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "startTimestamp", clubChallengeInfo.startTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "endTimestamp", clubChallengeInfo.endTimestamp);
        BlackboardUtils.SetOrCreateList(bb, "missionList", clubChallengeInfo.missionList, Serialize);
        if (clubChallengeInfo.reward != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "reward"), clubChallengeInfo.reward);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "reward");
        }
        BlackboardUtils.SetOrCreateValue(bb, "leaguePoint", clubChallengeInfo.leaguePoint);
        BlackboardUtils.SetOrCreateValue(bb, "done", clubChallengeInfo.done);
        BlackboardUtils.SetOrCreateValue(bb, "challengeId", clubChallengeInfo.challengeId);
        BlackboardUtils.SetOrCreateValue(bb, "date", clubChallengeInfo.date);
        BlackboardUtils.SetOrCreateValue(bb, "stage", clubChallengeInfo.stage);
        if (clubChallengeInfo.stagePreset != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "stagePreset"), clubChallengeInfo.stagePreset);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "stagePreset");
        }
        if (clubChallengeInfo.totalReward != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "totalReward"), clubChallengeInfo.totalReward);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "totalReward");
        }
        BlackboardUtils.SetOrCreateValue(bb, "isCompleteToUnlock", clubChallengeInfo.isCompleteToUnlock);
        BlackboardUtils.SetOrCreateList(bb, "rewardList", clubChallengeInfo.rewardList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "type", clubChallengeInfo.type);
        BlackboardUtils.SetOrCreateValue(bb, "maxStage", clubChallengeInfo.maxStage);
    }

    public static void Serialize(IBlackboard bb, ClubChallengeInfoMissionRequest clubChallengeInfoMissionRequest)
    {
        if (clubChallengeInfoMissionRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", clubChallengeInfoMissionRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", clubChallengeInfoMissionRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "date", clubChallengeInfoMissionRequest.date);
        BlackboardUtils.SetOrCreateValue(bb, "missionId", clubChallengeInfoMissionRequest.missionId);
    }

    public static void Serialize(IBlackboard bb, ClubChallengeInfoMissionResponse clubChallengeInfoMissionResponse)
    {
        if (clubChallengeInfoMissionResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", clubChallengeInfoMissionResponse.error);
        if (clubChallengeInfoMissionResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), clubChallengeInfoMissionResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", clubChallengeInfoMissionResponse.serverTime);
        if (clubChallengeInfoMissionResponse.missionInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "missionInfo"), clubChallengeInfoMissionResponse.missionInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "missionInfo");
        }
        BlackboardUtils.SetOrCreateList(bb, "missionContributionList", clubChallengeInfoMissionResponse.missionContributionList, Serialize);
    }

    public static void Serialize(IBlackboard bb, ClubChallengeInfoResponse clubChallengeInfoResponse)
    {
        if (clubChallengeInfoResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", clubChallengeInfoResponse.error);
        if (clubChallengeInfoResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), clubChallengeInfoResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", clubChallengeInfoResponse.serverTime);
        if (clubChallengeInfoResponse.clubChallengeInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "clubChallengeInfo"), clubChallengeInfoResponse.clubChallengeInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "clubChallengeInfo");
        }
        BlackboardUtils.SetOrCreateList(bb, "topContributionList", clubChallengeInfoResponse.topContributionList, Serialize);
        if (clubChallengeInfoResponse.clubEventChallengeInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "clubEventChallengeInfo"), clubChallengeInfoResponse.clubEventChallengeInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "clubEventChallengeInfo");
        }
        BlackboardUtils.SetOrCreateList(bb, "clubEventChallengeMissionContributionList", clubChallengeInfoResponse.clubEventChallengeMissionContributionList, Serialize);
    }

    public static void Serialize(IBlackboard bb, ClubChallengeInfoSimple clubChallengeInfoSimple)
    {
        if (clubChallengeInfoSimple == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "startTimestamp", clubChallengeInfoSimple.startTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "endTimestamp", clubChallengeInfoSimple.endTimestamp);
        if (clubChallengeInfoSimple.reward != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "reward"), clubChallengeInfoSimple.reward);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "reward");
        }
        BlackboardUtils.SetOrCreateValue(bb, "maxChallengeProgress", clubChallengeInfoSimple.maxChallengeProgress);
        BlackboardUtils.SetOrCreateValue(bb, "challengeProgress", clubChallengeInfoSimple.challengeProgress);
        BlackboardUtils.SetOrCreateValue(bb, "challengeId", clubChallengeInfoSimple.challengeId);
        BlackboardUtils.SetOrCreateValue(bb, "date", clubChallengeInfoSimple.date);
        BlackboardUtils.SetOrCreateValue(bb, "leaguePoint", clubChallengeInfoSimple.leaguePoint);
        BlackboardUtils.SetOrCreateValue(bb, "stage", clubChallengeInfoSimple.stage);
        BlackboardUtils.SetOrCreateValue(bb, "isCompleteToUnlock", clubChallengeInfoSimple.isCompleteToUnlock);
        BlackboardUtils.SetOrCreateList(bb, "rewardList", clubChallengeInfoSimple.rewardList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "type", clubChallengeInfoSimple.type);
        BlackboardUtils.SetOrCreateValue(bb, "maxStage", clubChallengeInfoSimple.maxStage);
    }

    public static void Serialize(IBlackboard bb, ClubChallengeInfoStagePreset clubChallengeInfoStagePreset)
    {
        if (clubChallengeInfoStagePreset == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "coinRewardRatioNumeratorList", clubChallengeInfoStagePreset.coinRewardRatioNumeratorList);
    }

    public static void Serialize(IBlackboard bb, ClubChallengeMission clubChallengeMission)
    {
        if (clubChallengeMission == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "missionId", clubChallengeMission.missionId);
        BlackboardUtils.SetOrCreateValue(bb, "missionType", clubChallengeMission.missionType);
        if (clubChallengeMission.info != null)
        {
            switch (clubChallengeMission.missionType)
            {
            case ClubChallengeMissionType.WIN_ANY:
                Serialize(bb, (ClubChallengeMissionValueEmpty)clubChallengeMission.info);
                break;

            case ClubChallengeMissionType.SPIN_ANY:
                Serialize(bb, (ClubChallengeMissionValueEmpty)clubChallengeMission.info);
                break;

            case ClubChallengeMissionType.WIN_BIG_WIN_ANY:
                Serialize(bb, (ClubChallengeMissionValueWinBigWinAny)clubChallengeMission.info);
                break;

            case ClubChallengeMissionType.ENTER_FREE_SPIN_ANY:
                Serialize(bb, (ClubChallengeMissionValueEmpty)clubChallengeMission.info);
                break;

            case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_ANY:
                Serialize(bb, (ClubChallengeMissionValueEmpty)clubChallengeMission.info);
                break;

            case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BIG_WIN_ANY:
                Serialize(bb, (ClubChallengeMissionValueAchieveTotalWinCreditByBigWinAny)clubChallengeMission.info);
                break;

            case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_FREE_SPIN_ANY:
                Serialize(bb, (ClubChallengeMissionValueEmpty)clubChallengeMission.info);
                break;

            case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_ANY:
                Serialize(bb, (ClubChallengeMissionValueEmpty)clubChallengeMission.info);
                break;

            case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_BY_BIG_WIN_ANY:
                Serialize(bb, (ClubChallengeMissionValueAchieveTotalLpByBigWinAny)clubChallengeMission.info);
                break;

            case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_BY_FREE_SPIN_ANY:
                Serialize(bb, (ClubChallengeMissionValueEmpty)clubChallengeMission.info);
                break;

            case ClubChallengeMissionType.WIN_TARGETED:
                Serialize(bb, (ClubChallengeMissionValueWinTargeted)clubChallengeMission.info);
                break;

            case ClubChallengeMissionType.SPIN_TARGETED:
                Serialize(bb, (ClubChallengeMissionValueSpinTargeted)clubChallengeMission.info);
                break;

            case ClubChallengeMissionType.WIN_BIG_WIN_TARGETED:
                Serialize(bb, (ClubChallengeMissionValueWinBigWinTargeted)clubChallengeMission.info);
                break;

            case ClubChallengeMissionType.WIN_BONUS_GAME_TARGETED:
                Serialize(bb, (ClubChallengeMissionValueWinBonusGameTargeted)clubChallengeMission.info);
                break;

            case ClubChallengeMissionType.ENTER_FREE_SPIN_TARGETED:
                Serialize(bb, (ClubChallengeMissionValueEnterFreeSpinTargeted)clubChallengeMission.info);
                break;

            case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_TARGETED:
                Serialize(bb, (ClubChallengeMissionValueAchieveTotalWinCreditTargeted)clubChallengeMission.info);
                break;

            case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BONUS_TARGETED:
                Serialize(bb, (ClubChallengeMissionValueAchieveTotalWinCreditByBonusTargeted)clubChallengeMission.info);
                break;

            case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BIG_WIN_TARGETED:
                Serialize(bb, (ClubChallengeMissionValueAchieveTotalWinCreditByBigWinTargeted)clubChallengeMission.info);
                break;

            case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_FREE_SPIN_TARGETED:
                Serialize(bb, (ClubChallengeMissionValueAchieveTotalWinCreditByFreeSpinTargeted)clubChallengeMission.info);
                break;

            case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_TARGETED:
                Serialize(bb, (ClubChallengeMissionValueAchieveTotalLpTargeted)clubChallengeMission.info);
                break;

            case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_BY_BIG_WIN_TARGETED:
                Serialize(bb, (ClubChallengeMissionValueAchieveTotalLpByBigWinTargeted)clubChallengeMission.info);
                break;
            }
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "info");
        }
        BlackboardUtils.SetOrCreateValue(bb, "completeCount", clubChallengeMission.completeCount);
        if (clubChallengeMission.reward != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "reward"), clubChallengeMission.reward);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "reward");
        }
        BlackboardUtils.SetOrCreateValue(bb, "leaguePoint", clubChallengeMission.leaguePoint);
        BlackboardUtils.SetOrCreateValue(bb, "progress", clubChallengeMission.progress);
        BlackboardUtils.SetOrCreateValue(bb, "done", clubChallengeMission.done);
        BlackboardUtils.SetOrCreateValue(bb, "completedStage", clubChallengeMission.completedStage);
    }

    public static void Serialize(IBlackboard bb, ClubChallengeMissionValueAchieveTotalLpByBigWinAny clubChallengeMissionValueAchieveTotalLpByBigWinAny)
    {
        if (clubChallengeMissionValueAchieveTotalLpByBigWinAny == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "winType", clubChallengeMissionValueAchieveTotalLpByBigWinAny.winType);
    }

    public static void Serialize(IBlackboard bb, ClubChallengeMissionValueAchieveTotalLpByBigWinTargeted clubChallengeMissionValueAchieveTotalLpByBigWinTargeted)
    {
        if (clubChallengeMissionValueAchieveTotalLpByBigWinTargeted == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", clubChallengeMissionValueAchieveTotalLpByBigWinTargeted.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "winType", clubChallengeMissionValueAchieveTotalLpByBigWinTargeted.winType);
    }

    public static void Serialize(IBlackboard bb, ClubChallengeMissionValueAchieveTotalLpTargeted clubChallengeMissionValueAchieveTotalLpTargeted)
    {
        if (clubChallengeMissionValueAchieveTotalLpTargeted == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", clubChallengeMissionValueAchieveTotalLpTargeted.gameId);
    }

    public static void Serialize(IBlackboard bb, ClubChallengeMissionValueAchieveTotalWinCreditByBigWinAny clubChallengeMissionValueAchieveTotalWinCreditByBigWinAny)
    {
        if (clubChallengeMissionValueAchieveTotalWinCreditByBigWinAny == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "winType", clubChallengeMissionValueAchieveTotalWinCreditByBigWinAny.winType);
    }

    public static void Serialize(IBlackboard bb, ClubChallengeMissionValueAchieveTotalWinCreditByBigWinTargeted clubChallengeMissionValueAchieveTotalWinCreditByBigWinTargeted)
    {
        if (clubChallengeMissionValueAchieveTotalWinCreditByBigWinTargeted == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", clubChallengeMissionValueAchieveTotalWinCreditByBigWinTargeted.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "winType", clubChallengeMissionValueAchieveTotalWinCreditByBigWinTargeted.winType);
    }

    public static void Serialize(IBlackboard bb, ClubChallengeMissionValueAchieveTotalWinCreditByBonusTargeted clubChallengeMissionValueAchieveTotalWinCreditByBonusTargeted)
    {
        if (clubChallengeMissionValueAchieveTotalWinCreditByBonusTargeted == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", clubChallengeMissionValueAchieveTotalWinCreditByBonusTargeted.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "bonusId", clubChallengeMissionValueAchieveTotalWinCreditByBonusTargeted.bonusId);
        BlackboardUtils.SetOrCreateValue(bb, "bonusName", clubChallengeMissionValueAchieveTotalWinCreditByBonusTargeted.bonusName);
    }

    public static void Serialize(IBlackboard bb, ClubChallengeMissionValueAchieveTotalWinCreditByFreeSpinTargeted clubChallengeMissionValueAchieveTotalWinCreditByFreeSpinTargeted)
    {
        if (clubChallengeMissionValueAchieveTotalWinCreditByFreeSpinTargeted == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", clubChallengeMissionValueAchieveTotalWinCreditByFreeSpinTargeted.gameId);
    }

    public static void Serialize(IBlackboard bb, ClubChallengeMissionValueAchieveTotalWinCreditTargeted clubChallengeMissionValueAchieveTotalWinCreditTargeted)
    {
        if (clubChallengeMissionValueAchieveTotalWinCreditTargeted == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", clubChallengeMissionValueAchieveTotalWinCreditTargeted.gameId);
    }

    public static void Serialize(IBlackboard bb, ClubChallengeMissionValueEmpty clubChallengeMissionValueEmpty)
    {
        if (clubChallengeMissionValueEmpty == null) { return; }
    }

    public static void Serialize(IBlackboard bb, ClubChallengeMissionValueEnterFreeSpinTargeted clubChallengeMissionValueEnterFreeSpinTargeted)
    {
        if (clubChallengeMissionValueEnterFreeSpinTargeted == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", clubChallengeMissionValueEnterFreeSpinTargeted.gameId);
    }

    public static void Serialize(IBlackboard bb, ClubChallengeMissionValueSpinTargeted clubChallengeMissionValueSpinTargeted)
    {
        if (clubChallengeMissionValueSpinTargeted == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", clubChallengeMissionValueSpinTargeted.gameId);
    }

    public static void Serialize(IBlackboard bb, ClubChallengeMissionValueWinBigWinAny clubChallengeMissionValueWinBigWinAny)
    {
        if (clubChallengeMissionValueWinBigWinAny == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "winType", clubChallengeMissionValueWinBigWinAny.winType);
    }

    public static void Serialize(IBlackboard bb, ClubChallengeMissionValueWinBigWinTargeted clubChallengeMissionValueWinBigWinTargeted)
    {
        if (clubChallengeMissionValueWinBigWinTargeted == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", clubChallengeMissionValueWinBigWinTargeted.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "winType", clubChallengeMissionValueWinBigWinTargeted.winType);
    }

    public static void Serialize(IBlackboard bb, ClubChallengeMissionValueWinBonusGameTargeted clubChallengeMissionValueWinBonusGameTargeted)
    {
        if (clubChallengeMissionValueWinBonusGameTargeted == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", clubChallengeMissionValueWinBonusGameTargeted.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "bonusId", clubChallengeMissionValueWinBonusGameTargeted.bonusId);
        BlackboardUtils.SetOrCreateValue(bb, "bonusName", clubChallengeMissionValueWinBonusGameTargeted.bonusName);
    }

    public static void Serialize(IBlackboard bb, ClubChallengeMissionValueWinTargeted clubChallengeMissionValueWinTargeted)
    {
        if (clubChallengeMissionValueWinTargeted == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", clubChallengeMissionValueWinTargeted.gameId);
    }

    public static void Serialize(IBlackboard bb, ClubCreateRequest clubCreateRequest)
    {
        if (clubCreateRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", clubCreateRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", clubCreateRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "name", clubCreateRequest.name);
        BlackboardUtils.SetOrCreateValue(bb, "symbol", clubCreateRequest.symbol);
        BlackboardUtils.SetOrCreateValue(bb, "motd", clubCreateRequest.motd);
        BlackboardUtils.SetOrCreateValue(bb, "watcher", clubCreateRequest.watcher);
        BlackboardUtils.SetOrCreateValue(bb, "minPlayerLevel", clubCreateRequest.minPlayerLevel);
        BlackboardUtils.SetOrCreateValue(bb, "joinType", clubCreateRequest.joinType);
    }

    public static void Serialize(IBlackboard bb, ClubCreateResponse clubCreateResponse)
    {
        if (clubCreateResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", clubCreateResponse.error);
        if (clubCreateResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), clubCreateResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", clubCreateResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "clubId", clubCreateResponse.clubId);
        if (clubCreateResponse.joinReward != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "joinReward"), clubCreateResponse.joinReward);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "joinReward");
        }
        if (clubCreateResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), clubCreateResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
        if (clubCreateResponse.clubInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "clubInfo"), clubCreateResponse.clubInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "clubInfo");
        }
    }

    public static void Serialize(IBlackboard bb, ClubDonateRequest clubDonateRequest)
    {
        if (clubDonateRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", clubDonateRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", clubDonateRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "clubId", clubDonateRequest.clubId);
        BlackboardUtils.SetOrCreateValue(bb, "donateLevel", clubDonateRequest.donateLevel);
        BlackboardUtils.SetOrCreateValue(bb, "timezoneOffset", clubDonateRequest.timezoneOffset);
    }

    public static void Serialize(IBlackboard bb, ClubDonateResponse clubDonateResponse)
    {
        if (clubDonateResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", clubDonateResponse.error);
        if (clubDonateResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), clubDonateResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", clubDonateResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "remainedCount", clubDonateResponse.remainedCount);
        if (clubDonateResponse.clubInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "clubInfo"), clubDonateResponse.clubInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "clubInfo");
        }
        if (clubDonateResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), clubDonateResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
        if (clubDonateResponse.myClubInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "myClubInfo"), clubDonateResponse.myClubInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "myClubInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "donationResetTimestamp", clubDonateResponse.donationResetTimestamp);
    }

    public static void Serialize(IBlackboard bb, ClubFeed clubFeed)
    {
        if (clubFeed == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "id", clubFeed.id);
        BlackboardUtils.SetOrCreateValue(bb, "createdTimestamp", clubFeed.createdTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "type", clubFeed.type);
        if (clubFeed.content != null)
        {
            switch (clubFeed.type)
            {
            case ClubFeedType.LOG:
                Serialize(bb, (ClubFeedContentLog)clubFeed.content);
                break;

            case ClubFeedType.MESSAGE:
                Serialize(bb, (ClubFeedContentMessage)clubFeed.content);
                break;

            case ClubFeedType.WIN_BONUS:
                Serialize(bb, (ClubFeedContentWinBonus)clubFeed.content);
                break;

            case ClubFeedType.CLUB_OFFER_BONUS:
                Serialize(bb, (ClubFeedContentClubOfferBonus)clubFeed.content);
                break;

            case ClubFeedType.CLUB_MISSION_COMPLETE:
                Serialize(bb, (ClubFeedContentClubMissionComplete)clubFeed.content);
                break;

            case ClubFeedType.CLUB_CHALLENGE_COMPLETE:
                Serialize(bb, (ClubFeedContentClubChallengeComplete)clubFeed.content);
                break;

            case ClubFeedType.CLUB_LEAGUE_REWARD:
                Serialize(bb, (ClubFeedContentClubLeagueReward)clubFeed.content);
                break;

            case ClubFeedType.CLUB_TIER_INFO:
                Serialize(bb, (ClubFeedContentClubTierInfo)clubFeed.content);
                break;

            case ClubFeedType.COLEADER_PROMOTE:
                Serialize(bb, (ClubFeedContentColeaderPromote)clubFeed.content);
                break;

            case ClubFeedType.COLEADER_DEMOTE:
                Serialize(bb, (ClubFeedContentColeaderDemote)clubFeed.content);
                break;

            case ClubFeedType.JOIN:
                Serialize(bb, (ClubFeedContentJoin)clubFeed.content);
                break;

            case ClubFeedType.LEVEL_UP:
                Serialize(bb, (ClubFeedContentLevelUp)clubFeed.content);
                break;

            case ClubFeedType.LEADER_CHANGE:
                Serialize(bb, (ClubFeedContentLeaderChange)clubFeed.content);
                break;

            case ClubFeedType.COLLECTING_GAME_REQUEST:
                Serialize(bb, (ClubFeedContentCollectingGameRequest)clubFeed.content);
                break;

            case ClubFeedType.BOSS_RAIDERS_ROUND_COMPLETE:
                Serialize(bb, (ClubFeedBossRaidersRoundComplete)clubFeed.content);
                break;

            case ClubFeedType.BOSS_RAIDERS_END_REWARD:
                Serialize(bb, (ClubFeedBossRaidersEndReward)clubFeed.content);
                break;

            case ClubFeedType.CLUB_ARENA_HELP:
                Serialize(bb, (ClubFeedClubArenaHelp)clubFeed.content);
                break;

            case ClubFeedType.CLUB_ARENA_END_REWARD:
                Serialize(bb, (ClubFeedClubArenaEndReward)clubFeed.content);
                break;

            case ClubFeedType.LEADER_PUSH:
                Serialize(bb, (ClubFeedLeaderPush)clubFeed.content);
                break;

            case ClubFeedType.HIDDEN_UNIVERSE_FINDER_REQUEST:
                Serialize(bb, (ClubFeedHiddenUniverseFinderRequest)clubFeed.content);
                break;
            }
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "content");
        }
        BlackboardUtils.SetOrCreateValue(bb, "like", clubFeed.like);
        BlackboardUtils.SetOrCreateValue(bb, "userLiked", clubFeed.userLiked);
    }

    public static void Serialize(IBlackboard bb, ClubFeedBossRaidersEndReward clubFeedBossRaidersEndReward)
    {
        if (clubFeedBossRaidersEndReward == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "percentile", clubFeedBossRaidersEndReward.percentile);
        if (clubFeedBossRaidersEndReward.reward != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "reward"), clubFeedBossRaidersEndReward.reward);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "reward");
        }
        BlackboardUtils.SetOrCreateValue(bb, "rank", clubFeedBossRaidersEndReward.rank);
        BlackboardUtils.SetOrCreateValue(bb, "themeId", clubFeedBossRaidersEndReward.themeId);
    }

    public static void Serialize(IBlackboard bb, ClubFeedBossRaidersRoundComplete clubFeedBossRaidersRoundComplete)
    {
        if (clubFeedBossRaidersRoundComplete == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "leaguePoint", clubFeedBossRaidersRoundComplete.leaguePoint);
        if (clubFeedBossRaidersRoundComplete.reward != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "reward"), clubFeedBossRaidersRoundComplete.reward);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "reward");
        }
        BlackboardUtils.SetOrCreateValue(bb, "themeId", clubFeedBossRaidersRoundComplete.themeId);
    }

    public static void Serialize(IBlackboard bb, ClubFeedClaimAllRequest clubFeedClaimAllRequest)
    {
        if (clubFeedClaimAllRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", clubFeedClaimAllRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", clubFeedClaimAllRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "maxFeedId", clubFeedClaimAllRequest.maxFeedId);
        BlackboardUtils.SetOrCreateValue(bb, "filterType", clubFeedClaimAllRequest.filterType);
    }

    public static void Serialize(IBlackboard bb, ClubFeedClaimAllResponse clubFeedClaimAllResponse)
    {
        if (clubFeedClaimAllResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", clubFeedClaimAllResponse.error);
        if (clubFeedClaimAllResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), clubFeedClaimAllResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", clubFeedClaimAllResponse.serverTime);
        if (clubFeedClaimAllResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), clubFeedClaimAllResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
    }

    public static void Serialize(IBlackboard bb, ClubFeedClaimRequest clubFeedClaimRequest)
    {
        if (clubFeedClaimRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", clubFeedClaimRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", clubFeedClaimRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "feedId", clubFeedClaimRequest.feedId);
    }

    public static void Serialize(IBlackboard bb, ClubFeedClaimResponse clubFeedClaimResponse)
    {
        if (clubFeedClaimResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", clubFeedClaimResponse.error);
        if (clubFeedClaimResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), clubFeedClaimResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", clubFeedClaimResponse.serverTime);
        if (clubFeedClaimResponse.claimReward != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "claimReward"), clubFeedClaimResponse.claimReward);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "claimReward");
        }
        BlackboardUtils.SetOrCreateValue(bb, "analyticContextId", clubFeedClaimResponse.analyticContextId);
    }

    public static void Serialize(IBlackboard bb, ClubFeedClubArenaEndReward clubFeedClubArenaEndReward)
    {
        if (clubFeedClubArenaEndReward == null) { return; }
        if (clubFeedClubArenaEndReward.reward != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "reward"), clubFeedClubArenaEndReward.reward);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "reward");
        }
        BlackboardUtils.SetOrCreateValue(bb, "rank", clubFeedClubArenaEndReward.rank);
        BlackboardUtils.SetOrCreateValue(bb, "percentile", clubFeedClubArenaEndReward.percentile);
    }

    public static void Serialize(IBlackboard bb, ClubFeedClubArenaHelp clubFeedClubArenaHelp)
    {
        if (clubFeedClubArenaHelp == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "eventId", clubFeedClubArenaHelp.eventId);
        BlackboardUtils.SetOrCreateValue(bb, "userId", clubFeedClubArenaHelp.userId);
        BlackboardUtils.SetOrCreateValue(bb, "name", clubFeedClubArenaHelp.name);
        BlackboardUtils.SetOrCreateValue(bb, "profileUrl", clubFeedClubArenaHelp.profileUrl);
        BlackboardUtils.SetOrCreateValue(bb, "opponentUserId", clubFeedClubArenaHelp.opponentUserId);
        BlackboardUtils.SetOrCreateValue(bb, "opponentName", clubFeedClubArenaHelp.opponentName);
        if (clubFeedClubArenaHelp.reward != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "reward"), clubFeedClubArenaHelp.reward);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "reward");
        }
    }

    public static void Serialize(IBlackboard bb, ClubFeedContentClubChallengeComplete clubFeedContentClubChallengeComplete)
    {
        if (clubFeedContentClubChallengeComplete == null) { return; }
        if (clubFeedContentClubChallengeComplete.reward != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "reward"), clubFeedContentClubChallengeComplete.reward);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "reward");
        }
        BlackboardUtils.SetOrCreateValue(bb, "leaguePoint", clubFeedContentClubChallengeComplete.leaguePoint);
    }

    public static void Serialize(IBlackboard bb, ClubFeedContentClubLeagueReward clubFeedContentClubLeagueReward)
    {
        if (clubFeedContentClubLeagueReward == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "rank", clubFeedContentClubLeagueReward.rank);
        if (clubFeedContentClubLeagueReward.reward != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "reward"), clubFeedContentClubLeagueReward.reward);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "reward");
        }
    }

    public static void Serialize(IBlackboard bb, ClubFeedContentClubMissionComplete clubFeedContentClubMissionComplete)
    {
        if (clubFeedContentClubMissionComplete == null) { return; }
        if (clubFeedContentClubMissionComplete.mission != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "mission"), clubFeedContentClubMissionComplete.mission);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "mission");
        }
        if (clubFeedContentClubMissionComplete.reward != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "reward"), clubFeedContentClubMissionComplete.reward);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "reward");
        }
        BlackboardUtils.SetOrCreateValue(bb, "leaguePoint", clubFeedContentClubMissionComplete.leaguePoint);
    }

    public static void Serialize(IBlackboard bb, ClubFeedContentClubOfferBonus clubFeedContentClubOfferBonus)
    {
        if (clubFeedContentClubOfferBonus == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "name", clubFeedContentClubOfferBonus.name);
        BlackboardUtils.SetOrCreateValue(bb, "userId", clubFeedContentClubOfferBonus.userId);
        BlackboardUtils.SetOrCreateValue(bb, "profileUrl", clubFeedContentClubOfferBonus.profileUrl);
        BlackboardUtils.SetOrCreateValue(bb, "tier", clubFeedContentClubOfferBonus.tier);
        if (clubFeedContentClubOfferBonus.reward != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "reward"), clubFeedContentClubOfferBonus.reward);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "reward");
        }
    }

    public static void Serialize(IBlackboard bb, ClubFeedContentClubTierInfo clubFeedContentClubTierInfo)
    {
        if (clubFeedContentClubTierInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "leagueTier", clubFeedContentClubTierInfo.leagueTier);
        BlackboardUtils.SetOrCreateValue(bb, "tierChange", clubFeedContentClubTierInfo.tierChange);
    }

    public static void Serialize(IBlackboard bb, ClubFeedContentColeaderDemote clubFeedContentColeaderDemote)
    {
        if (clubFeedContentColeaderDemote == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "name", clubFeedContentColeaderDemote.name);
        BlackboardUtils.SetOrCreateValue(bb, "userId", clubFeedContentColeaderDemote.userId);
    }

    public static void Serialize(IBlackboard bb, ClubFeedContentColeaderPromote clubFeedContentColeaderPromote)
    {
        if (clubFeedContentColeaderPromote == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "name", clubFeedContentColeaderPromote.name);
        BlackboardUtils.SetOrCreateValue(bb, "userId", clubFeedContentColeaderPromote.userId);
    }

    public static void Serialize(IBlackboard bb, ClubFeedContentCollectingGameRequest clubFeedContentCollectingGameRequest)
    {
        if (clubFeedContentCollectingGameRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "name", clubFeedContentCollectingGameRequest.name);
        BlackboardUtils.SetOrCreateValue(bb, "userId", clubFeedContentCollectingGameRequest.userId);
        BlackboardUtils.SetOrCreateValue(bb, "profileUrl", clubFeedContentCollectingGameRequest.profileUrl);
        BlackboardUtils.SetOrCreateValue(bb, "eventId", clubFeedContentCollectingGameRequest.eventId);
        BlackboardUtils.SetOrCreateValue(bb, "pieceId", clubFeedContentCollectingGameRequest.pieceId);
        BlackboardUtils.SetOrCreateValue(bb, "capacity", clubFeedContentCollectingGameRequest.capacity);
        BlackboardUtils.SetOrCreateValue(bb, "possessions", clubFeedContentCollectingGameRequest.possessions);
        BlackboardUtils.SetOrCreateValue(bb, "requirement", clubFeedContentCollectingGameRequest.requirement);
        BlackboardUtils.SetOrCreateValue(bb, "tier", clubFeedContentCollectingGameRequest.tier);
        BlackboardUtils.SetOrCreateValue(bb, "rarity", clubFeedContentCollectingGameRequest.rarity);
    }

    public static void Serialize(IBlackboard bb, ClubFeedContentJoin clubFeedContentJoin)
    {
        if (clubFeedContentJoin == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "name", clubFeedContentJoin.name);
        BlackboardUtils.SetOrCreateValue(bb, "userId", clubFeedContentJoin.userId);
    }

    public static void Serialize(IBlackboard bb, ClubFeedContentLeaderChange clubFeedContentLeaderChange)
    {
        if (clubFeedContentLeaderChange == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "name", clubFeedContentLeaderChange.name);
        BlackboardUtils.SetOrCreateValue(bb, "userId", clubFeedContentLeaderChange.userId);
    }

    public static void Serialize(IBlackboard bb, ClubFeedContentLevelUp clubFeedContentLevelUp)
    {
        if (clubFeedContentLevelUp == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "level", clubFeedContentLevelUp.level);
    }

    public static void Serialize(IBlackboard bb, ClubFeedContentLog clubFeedContentLog)
    {
        if (clubFeedContentLog == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "name", clubFeedContentLog.name);
        BlackboardUtils.SetOrCreateValue(bb, "userId", clubFeedContentLog.userId);
        BlackboardUtils.SetOrCreateValue(bb, "logType", clubFeedContentLog.logType);
        BlackboardUtils.SetOrCreateValue(bb, "level", clubFeedContentLog.level);
    }

    public static void Serialize(IBlackboard bb, ClubFeedContentMessage clubFeedContentMessage)
    {
        if (clubFeedContentMessage == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "name", clubFeedContentMessage.name);
        BlackboardUtils.SetOrCreateValue(bb, "userId", clubFeedContentMessage.userId);
        BlackboardUtils.SetOrCreateValue(bb, "profileUrl", clubFeedContentMessage.profileUrl);
        BlackboardUtils.SetOrCreateValue(bb, "tier", clubFeedContentMessage.tier);
        BlackboardUtils.SetOrCreateValue(bb, "message", clubFeedContentMessage.message);
    }

    public static void Serialize(IBlackboard bb, ClubFeedContentWinBonus clubFeedContentWinBonus)
    {
        if (clubFeedContentWinBonus == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "name", clubFeedContentWinBonus.name);
        BlackboardUtils.SetOrCreateValue(bb, "userId", clubFeedContentWinBonus.userId);
        BlackboardUtils.SetOrCreateValue(bb, "profileUrl", clubFeedContentWinBonus.profileUrl);
        BlackboardUtils.SetOrCreateValue(bb, "tier", clubFeedContentWinBonus.tier);
        if (clubFeedContentWinBonus.record != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "record"), clubFeedContentWinBonus.record);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "record");
        }
        BlackboardUtils.SetOrCreateValue(bb, "winBonus", clubFeedContentWinBonus.winBonus);
    }

    public static void Serialize(IBlackboard bb, ClubFeedGiftRequest clubFeedGiftRequest)
    {
        if (clubFeedGiftRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", clubFeedGiftRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", clubFeedGiftRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "feedId", clubFeedGiftRequest.feedId);
        BlackboardUtils.SetOrCreateValue(bb, "metaGameEventId", clubFeedGiftRequest.metaGameEventId);
        BlackboardUtils.SetOrCreateValue(bb, "requestType", clubFeedGiftRequest.requestType);
    }

    public static void Serialize(IBlackboard bb, ClubFeedGiftResponse clubFeedGiftResponse)
    {
        if (clubFeedGiftResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", clubFeedGiftResponse.error);
        if (clubFeedGiftResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), clubFeedGiftResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", clubFeedGiftResponse.serverTime);
    }

    public static void Serialize(IBlackboard bb, ClubFeedHiddenUniverseFinderRequest clubFeedHiddenUniverseFinderRequest)
    {
        if (clubFeedHiddenUniverseFinderRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "name", clubFeedHiddenUniverseFinderRequest.name);
        BlackboardUtils.SetOrCreateValue(bb, "userId", clubFeedHiddenUniverseFinderRequest.userId);
        BlackboardUtils.SetOrCreateValue(bb, "profileUrl", clubFeedHiddenUniverseFinderRequest.profileUrl);
        BlackboardUtils.SetOrCreateValue(bb, "capacity", clubFeedHiddenUniverseFinderRequest.capacity);
        BlackboardUtils.SetOrCreateValue(bb, "requirement", clubFeedHiddenUniverseFinderRequest.requirement);
        BlackboardUtils.SetOrCreateValue(bb, "tier", clubFeedHiddenUniverseFinderRequest.tier);
    }

    public static void Serialize(IBlackboard bb, ClubFeedLeaderPush clubFeedLeaderPush)
    {
        if (clubFeedLeaderPush == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "message", clubFeedLeaderPush.message);
    }

    public static void Serialize(IBlackboard bb, ClubFeedListRequest clubFeedListRequest)
    {
        if (clubFeedListRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", clubFeedListRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", clubFeedListRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "fromFeedId", clubFeedListRequest.fromFeedId);
        BlackboardUtils.SetOrCreateValue(bb, "numFeed", clubFeedListRequest.numFeed);
        BlackboardUtils.SetOrCreateValue(bb, "metaGameEventId", clubFeedListRequest.metaGameEventId);
        BlackboardUtils.SetOrCreateValue(bb, "filterType", clubFeedListRequest.filterType);
    }

    public static void Serialize(IBlackboard bb, ClubFeedListResponse clubFeedListResponse)
    {
        if (clubFeedListResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", clubFeedListResponse.error);
        if (clubFeedListResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), clubFeedListResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", clubFeedListResponse.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "feedList", clubFeedListResponse.feedList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "collectAllEnabled", clubFeedListResponse.collectAllEnabled);
        BlackboardUtils.SetOrCreateValue(bb, "collectAllCredit", clubFeedListResponse.collectAllCredit);
        BlackboardUtils.SetOrCreateValue(bb, "metaGameRequestResetTimestamp", clubFeedListResponse.metaGameRequestResetTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "nextExists", clubFeedListResponse.nextExists);
    }

    public static void Serialize(IBlackboard bb, ClubFeedMessageRequest clubFeedMessageRequest)
    {
        if (clubFeedMessageRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", clubFeedMessageRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", clubFeedMessageRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "message", clubFeedMessageRequest.message);
    }

    public static void Serialize(IBlackboard bb, ClubFeedMessageResponse clubFeedMessageResponse)
    {
        if (clubFeedMessageResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", clubFeedMessageResponse.error);
        if (clubFeedMessageResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), clubFeedMessageResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", clubFeedMessageResponse.serverTime);
    }

    public static void Serialize(IBlackboard bb, ClubInfo clubInfo)
    {
        if (clubInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "id", clubInfo.id);
        BlackboardUtils.SetOrCreateValue(bb, "name", clubInfo.name);
        BlackboardUtils.SetOrCreateValue(bb, "symbol", clubInfo.symbol);
        BlackboardUtils.SetOrCreateValue(bb, "motd", clubInfo.motd);
        BlackboardUtils.SetOrCreateValue(bb, "level", clubInfo.level);
        BlackboardUtils.SetOrCreateValue(bb, "exp", clubInfo.exp);
        BlackboardUtils.SetOrCreateValue(bb, "baseExp", clubInfo.baseExp);
        BlackboardUtils.SetOrCreateValue(bb, "requiredExpMax", clubInfo.requiredExpMax);
        BlackboardUtils.SetOrCreateValue(bb, "notice", clubInfo.notice);
        BlackboardUtils.SetOrCreateValue(bb, "watcher", clubInfo.watcher);
        BlackboardUtils.SetOrCreateValue(bb, "totalContribution", clubInfo.totalContribution);
        BlackboardUtils.SetOrCreateValue(bb, "minPlayerLevel", clubInfo.minPlayerLevel);
        BlackboardUtils.SetOrCreateValue(bb, "joinType", clubInfo.joinType);
    }

    public static void Serialize(IBlackboard bb, ClubInfoRequest clubInfoRequest)
    {
        if (clubInfoRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", clubInfoRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", clubInfoRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "clubId", clubInfoRequest.clubId);
        BlackboardUtils.SetOrCreateValue(bb, "timezoneOffset", clubInfoRequest.timezoneOffset);
        BlackboardUtils.SetOrCreateValue(bb, "isPopup", clubInfoRequest.isPopup);
        BlackboardUtils.SetOrCreateValue(bb, "metaGameEventId", clubInfoRequest.metaGameEventId);
    }

    public static void Serialize(IBlackboard bb, ClubInfoResponse clubInfoResponse)
    {
        if (clubInfoResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", clubInfoResponse.error);
        if (clubInfoResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), clubInfoResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", clubInfoResponse.serverTime);
        if (clubInfoResponse.clubInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "clubInfo"), clubInfoResponse.clubInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "clubInfo");
        }
        BlackboardUtils.SetOrCreateList(bb, "clubMemberList", clubInfoResponse.clubMemberList, Serialize);
        if (clubInfoResponse.myClubInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "myClubInfo"), clubInfoResponse.myClubInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "myClubInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "donationResetTimestamp", clubInfoResponse.donationResetTimestamp);
        if (clubInfoResponse.tierInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "tierInfo"), clubInfoResponse.tierInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "tierInfo");
        }
        if (clubInfoResponse.leagueRewardPopup != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "leagueRewardPopup"), clubInfoResponse.leagueRewardPopup);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "leagueRewardPopup");
        }
        BlackboardUtils.SetOrCreateValue(bb, "numClubRequest", clubInfoResponse.numClubRequest);
        if (clubInfoResponse.lpBoostInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "lpBoostInfo"), clubInfoResponse.lpBoostInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "lpBoostInfo");
        }
        if (clubInfoResponse.leagueInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "leagueInfo"), clubInfoResponse.leagueInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "leagueInfo");
        }
        if (clubInfoResponse.leaderPushInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "leaderPushInfo"), clubInfoResponse.leaderPushInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "leaderPushInfo");
        }
        if (clubInfoResponse.clubArenaRewardPopup != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "clubArenaRewardPopup"), clubInfoResponse.clubArenaRewardPopup);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "clubArenaRewardPopup");
        }
        BlackboardUtils.SetOrCreateValue(bb, "lastClubLeaveTimestamp", clubInfoResponse.lastClubLeaveTimestamp);
        if (clubInfoResponse.bossRaidersRewardPopup != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "bossRaidersRewardPopup"), clubInfoResponse.bossRaidersRewardPopup);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "bossRaidersRewardPopup");
        }
    }

    public static void Serialize(IBlackboard bb, ClubJoinRequest clubJoinRequest)
    {
        if (clubJoinRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", clubJoinRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", clubJoinRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "clubId", clubJoinRequest.clubId);
        BlackboardUtils.SetOrCreateValue(bb, "joinUiType", clubJoinRequest.joinUiType);
    }

    public static void Serialize(IBlackboard bb, ClubJoinResponse clubJoinResponse)
    {
        if (clubJoinResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", clubJoinResponse.error);
        if (clubJoinResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), clubJoinResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", clubJoinResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "clubId", clubJoinResponse.clubId);
        if (clubJoinResponse.joinReward != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "joinReward"), clubJoinResponse.joinReward);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "joinReward");
        }
        if (clubJoinResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), clubJoinResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
        BlackboardUtils.SetOrCreateList(bb, "clubOnlineList", clubJoinResponse.clubOnlineList, Serialize);
        if (clubJoinResponse.userClubStateInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userClubStateInfo"), clubJoinResponse.userClubStateInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userClubStateInfo");
        }
        if (clubJoinResponse.clubInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "clubInfo"), clubJoinResponse.clubInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "clubInfo");
        }
    }

    public static void Serialize(IBlackboard bb, ClubLeaderPushInfo clubLeaderPushInfo)
    {
        if (clubLeaderPushInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "messageList", clubLeaderPushInfo.messageList);
        BlackboardUtils.SetOrCreateValue(bb, "lastPushTimestamp", clubLeaderPushInfo.lastPushTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "cooltimeMs", clubLeaderPushInfo.cooltimeMs);
    }

    public static void Serialize(IBlackboard bb, ClubLeaderPushRequest clubLeaderPushRequest)
    {
        if (clubLeaderPushRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", clubLeaderPushRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", clubLeaderPushRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "messageIndex", clubLeaderPushRequest.messageIndex);
        BlackboardUtils.SetOrCreateValue(bb, "contextId", clubLeaderPushRequest.contextId);
    }

    public static void Serialize(IBlackboard bb, ClubLeague clubLeague)
    {
        if (clubLeague == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "MAX_TIER", clubLeague.MAX_TIER);
        BlackboardUtils.SetOrCreateValue(bb, "LEAGUE_POINT_UNIT_WIN_CREDIT", clubLeague.LEAGUE_POINT_UNIT_WIN_CREDIT);
    }

    public static void Serialize(IBlackboard bb, ClubLeagueInfo clubLeagueInfo)
    {
        if (clubLeagueInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "rank", clubLeagueInfo.rank);
        BlackboardUtils.SetOrCreateValue(bb, "tierChange", clubLeagueInfo.tierChange);
        BlackboardUtils.SetOrCreateValue(bb, "unitNum", clubLeagueInfo.unitNum);
    }

    public static void Serialize(IBlackboard bb, ClubLeagueRankingListPrefetchResponse clubLeagueRankingListPrefetchResponse)
    {
        if (clubLeagueRankingListPrefetchResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", clubLeagueRankingListPrefetchResponse.error);
        if (clubLeagueRankingListPrefetchResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), clubLeagueRankingListPrefetchResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", clubLeagueRankingListPrefetchResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "userNotInClub", clubLeagueRankingListPrefetchResponse.userNotInClub);
        BlackboardUtils.SetOrCreateValue(bb, "listNotReady", clubLeagueRankingListPrefetchResponse.listNotReady);
        BlackboardUtils.SetOrCreateValue(bb, "clubNotInList", clubLeagueRankingListPrefetchResponse.clubNotInList);
        BlackboardUtils.SetOrCreateValue(bb, "maxClubLeagueTier", clubLeagueRankingListPrefetchResponse.maxClubLeagueTier);
        BlackboardUtils.SetOrCreateValue(bb, "userClubLeagueTier", clubLeagueRankingListPrefetchResponse.userClubLeagueTier);
        BlackboardUtils.SetOrCreateValue(bb, "userClubLeagueIndex", clubLeagueRankingListPrefetchResponse.userClubLeagueIndex);
        BlackboardUtils.SetOrCreateValue(bb, "lastUpdatedTimestamp", clubLeagueRankingListPrefetchResponse.lastUpdatedTimestamp);
        if (clubLeagueRankingListPrefetchResponse.clubInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "clubInfo"), clubLeagueRankingListPrefetchResponse.clubInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "clubInfo");
        }
    }

    public static void Serialize(IBlackboard bb, ClubLeagueRankingListRequest clubLeagueRankingListRequest)
    {
        if (clubLeagueRankingListRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", clubLeagueRankingListRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", clubLeagueRankingListRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "tier", clubLeagueRankingListRequest.tier);
        BlackboardUtils.SetOrCreateValue(bb, "startIndex", clubLeagueRankingListRequest.startIndex);
        BlackboardUtils.SetOrCreateValue(bb, "count", clubLeagueRankingListRequest.count);
    }

    public static void Serialize(IBlackboard bb, ClubLeagueRankingListResponse clubLeagueRankingListResponse)
    {
        if (clubLeagueRankingListResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", clubLeagueRankingListResponse.error);
        if (clubLeagueRankingListResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), clubLeagueRankingListResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", clubLeagueRankingListResponse.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "clubList", clubLeagueRankingListResponse.clubList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "lastUpdatedTimestamp", clubLeagueRankingListResponse.lastUpdatedTimestamp);
    }

    public static void Serialize(IBlackboard bb, ClubLeagueResponse clubLeagueResponse)
    {
        if (clubLeagueResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", clubLeagueResponse.error);
        if (clubLeagueResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), clubLeagueResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", clubLeagueResponse.serverTime);
        if (clubLeagueResponse.tierInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "tierInfo"), clubLeagueResponse.tierInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "tierInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "endTimestamp", clubLeagueResponse.endTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "rewardCreditList", clubLeagueResponse.rewardCreditList);
        BlackboardUtils.SetOrCreateList(bb, "clubList", clubLeagueResponse.clubList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "breakTime", clubLeagueResponse.breakTime);
        BlackboardUtils.SetOrCreateValue(bb, "indexPromote", clubLeagueResponse.indexPromote);
        BlackboardUtils.SetOrCreateValue(bb, "indexDemote", clubLeagueResponse.indexDemote);
        BlackboardUtils.SetOrCreateValue(bb, "maxOpenedLeagueTier", clubLeagueResponse.maxOpenedLeagueTier);
        BlackboardUtils.SetOrCreateValue(bb, "indexRewardChange", clubLeagueResponse.indexRewardChange);
        BlackboardUtils.SetOrCreateValue(bb, "unitMaxNum", clubLeagueResponse.unitMaxNum);
    }

    public static void Serialize(IBlackboard bb, ClubLeagueRewardPopupInfo clubLeagueRewardPopupInfo)
    {
        if (clubLeagueRewardPopupInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "leagueTier", clubLeagueRewardPopupInfo.leagueTier);
        BlackboardUtils.SetOrCreateValue(bb, "rank", clubLeagueRewardPopupInfo.rank);
        BlackboardUtils.SetOrCreateValue(bb, "tierChange", clubLeagueRewardPopupInfo.tierChange);
        BlackboardUtils.SetOrCreateValue(bb, "clubId", clubLeagueRewardPopupInfo.clubId);
        BlackboardUtils.SetOrCreateValue(bb, "rewardExpireTimestamp", clubLeagueRewardPopupInfo.rewardExpireTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "rewardCredit", clubLeagueRewardPopupInfo.rewardCredit);
    }

    public static void Serialize(IBlackboard bb, ClubLeagueTierStatus clubLeagueTierStatus)
    {
        if (clubLeagueTierStatus == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "leagueTier", clubLeagueTierStatus.leagueTier);
        BlackboardUtils.SetOrCreateValue(bb, "clubs", clubLeagueTierStatus.clubs);
        BlackboardUtils.SetOrCreateValue(bb, "firstPlaceReward", clubLeagueTierStatus.firstPlaceReward);
        BlackboardUtils.SetOrCreateValue(bb, "opened", clubLeagueTierStatus.opened);
    }

    public static void Serialize(IBlackboard bb, ClubLeagueTiersResponse clubLeagueTiersResponse)
    {
        if (clubLeagueTiersResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", clubLeagueTiersResponse.error);
        if (clubLeagueTiersResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), clubLeagueTiersResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", clubLeagueTiersResponse.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "tierList", clubLeagueTiersResponse.tierList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "maxOpenedLeagueTier", clubLeagueTiersResponse.maxOpenedLeagueTier);
    }

    public static void Serialize(IBlackboard bb, ClubLeagueTopResponse clubLeagueTopResponse)
    {
        if (clubLeagueTopResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", clubLeagueTopResponse.error);
        if (clubLeagueTopResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), clubLeagueTopResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", clubLeagueTopResponse.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "topClubList", clubLeagueTopResponse.topClubList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "currentMaxLeagueTier", clubLeagueTopResponse.currentMaxLeagueTier);
    }

    public static void Serialize(IBlackboard bb, ClubLeaveRequest clubLeaveRequest)
    {
        if (clubLeaveRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", clubLeaveRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", clubLeaveRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "clubId", clubLeaveRequest.clubId);
        BlackboardUtils.SetOrCreateValue(bb, "leaveUiType", clubLeaveRequest.leaveUiType);
    }

    public static void Serialize(IBlackboard bb, ClubLeaveResponse clubLeaveResponse)
    {
        if (clubLeaveResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", clubLeaveResponse.error);
        if (clubLeaveResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), clubLeaveResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", clubLeaveResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "clubId", clubLeaveResponse.clubId);
        BlackboardUtils.SetOrCreateValue(bb, "lastClubLeaveTimestamp", clubLeaveResponse.lastClubLeaveTimestamp);
    }

    public static void Serialize(IBlackboard bb, ClubLevel clubLevel)
    {
        if (clubLevel == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "MAX_LEVEL", clubLevel.MAX_LEVEL);
        BlackboardUtils.SetOrCreateValue(bb, "REQUIRED_EXP_TABLE", clubLevel.REQUIRED_EXP_TABLE);
    }

    public static void Serialize(IBlackboard bb, ClubListInfo clubListInfo)
    {
        if (clubListInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "id", clubListInfo.id);
        BlackboardUtils.SetOrCreateValue(bb, "name", clubListInfo.name);
        BlackboardUtils.SetOrCreateValue(bb, "symbol", clubListInfo.symbol);
        BlackboardUtils.SetOrCreateValue(bb, "motd", clubListInfo.motd);
        BlackboardUtils.SetOrCreateValue(bb, "members", clubListInfo.members);
        BlackboardUtils.SetOrCreateValue(bb, "level", clubListInfo.level);
        BlackboardUtils.SetOrCreateValue(bb, "friends", clubListInfo.friends);
        BlackboardUtils.SetOrCreateValue(bb, "leagueTier", clubListInfo.leagueTier);
        BlackboardUtils.SetOrCreateValue(bb, "leaguePoint", clubListInfo.leaguePoint);
        BlackboardUtils.SetOrCreateValue(bb, "minPlayerLevel", clubListInfo.minPlayerLevel);
        BlackboardUtils.SetOrCreateValue(bb, "joinType", clubListInfo.joinType);
        BlackboardUtils.SetOrCreateValue(bb, "rank", clubListInfo.rank);
    }

    public static void Serialize(IBlackboard bb, ClubListRequest clubListRequest)
    {
        if (clubListRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", clubListRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", clubListRequest.ackMask);
    }

    public static void Serialize(IBlackboard bb, ClubListResponse clubListResponse)
    {
        if (clubListResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", clubListResponse.error);
        if (clubListResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), clubListResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", clubListResponse.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "clubList", clubListResponse.clubList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "invitedClubList", clubListResponse.invitedClubList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "showDeclinePopup", clubListResponse.showDeclinePopup);
        BlackboardUtils.SetOrCreateValue(bb, "analyticContextId", clubListResponse.analyticContextId);
    }

    public static void Serialize(IBlackboard bb, ClubMember clubMember)
    {
        if (clubMember == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "userId", clubMember.userId);
        BlackboardUtils.SetOrCreateValue(bb, "clubId", clubMember.clubId);
        BlackboardUtils.SetOrCreateValue(bb, "authority", clubMember.authority);
        BlackboardUtils.SetOrCreateValue(bb, "donation", clubMember.donation);
        BlackboardUtils.SetOrCreateValue(bb, "dailyDonation", clubMember.dailyDonation);
        BlackboardUtils.SetOrCreateValue(bb, "dailyDonationCount", clubMember.dailyDonationCount);
        BlackboardUtils.SetOrCreateValue(bb, "dailyDonationTimestamp", clubMember.dailyDonationTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "contribution", clubMember.contribution);
        BlackboardUtils.SetOrCreateValue(bb, "joinedTimestamp", clubMember.joinedTimestamp);
    }

    public static void Serialize(IBlackboard bb, ClubMemberAcceptAllResponse clubMemberAcceptAllResponse)
    {
        if (clubMemberAcceptAllResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", clubMemberAcceptAllResponse.error);
        if (clubMemberAcceptAllResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), clubMemberAcceptAllResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", clubMemberAcceptAllResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "success", clubMemberAcceptAllResponse.success);
        BlackboardUtils.SetOrCreateValue(bb, "vacancies", clubMemberAcceptAllResponse.vacancies);
    }

    public static void Serialize(IBlackboard bb, ClubMemberAllRequest clubMemberAllRequest)
    {
        if (clubMemberAllRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", clubMemberAllRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", clubMemberAllRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "clubId", clubMemberAllRequest.clubId);
    }

    public static void Serialize(IBlackboard bb, ClubMemberDemoteResponse clubMemberDemoteResponse)
    {
        if (clubMemberDemoteResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", clubMemberDemoteResponse.error);
        if (clubMemberDemoteResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), clubMemberDemoteResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", clubMemberDemoteResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "clubId", clubMemberDemoteResponse.clubId);
        BlackboardUtils.SetOrCreateValue(bb, "targetUserId", clubMemberDemoteResponse.targetUserId);
    }

    public static void Serialize(IBlackboard bb, ClubMemberListInfo clubMemberListInfo)
    {
        if (clubMemberListInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "name", clubMemberListInfo.name);
        BlackboardUtils.SetOrCreateValue(bb, "userId", clubMemberListInfo.userId);
        BlackboardUtils.SetOrCreateValue(bb, "profileUrl", clubMemberListInfo.profileUrl);
        BlackboardUtils.SetOrCreateValue(bb, "reportCount", clubMemberListInfo.reportCount);
        BlackboardUtils.SetOrCreateValue(bb, "tier", clubMemberListInfo.tier);
        BlackboardUtils.SetOrCreateValue(bb, "authority", clubMemberListInfo.authority);
        BlackboardUtils.SetOrCreateValue(bb, "donation", clubMemberListInfo.donation);
        BlackboardUtils.SetOrCreateValue(bb, "contribution", clubMemberListInfo.contribution);
        BlackboardUtils.SetOrCreateValue(bb, "lastOnlineTimestamp", clubMemberListInfo.lastOnlineTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "isOnline", clubMemberListInfo.isOnline);
        BlackboardUtils.SetOrCreateValue(bb, "leaguePoint", clubMemberListInfo.leaguePoint);
        BlackboardUtils.SetOrCreateValue(bb, "clubGiving", clubMemberListInfo.clubGiving);
        BlackboardUtils.SetOrCreateValue(bb, "countrySelected", clubMemberListInfo.countrySelected);
    }

    public static void Serialize(IBlackboard bb, ClubMemberPromoteResponse clubMemberPromoteResponse)
    {
        if (clubMemberPromoteResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", clubMemberPromoteResponse.error);
        if (clubMemberPromoteResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), clubMemberPromoteResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", clubMemberPromoteResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "clubId", clubMemberPromoteResponse.clubId);
        BlackboardUtils.SetOrCreateValue(bb, "targetUserId", clubMemberPromoteResponse.targetUserId);
    }

    public static void Serialize(IBlackboard bb, ClubMemberRemoveResponse clubMemberRemoveResponse)
    {
        if (clubMemberRemoveResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", clubMemberRemoveResponse.error);
        if (clubMemberRemoveResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), clubMemberRemoveResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", clubMemberRemoveResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "clubId", clubMemberRemoveResponse.clubId);
        BlackboardUtils.SetOrCreateValue(bb, "targetUserId", clubMemberRemoveResponse.targetUserId);
    }

    public static void Serialize(IBlackboard bb, ClubMemberRequest clubMemberRequest)
    {
        if (clubMemberRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", clubMemberRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", clubMemberRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "clubId", clubMemberRequest.clubId);
        BlackboardUtils.SetOrCreateValue(bb, "targetUserId", clubMemberRequest.targetUserId);
    }

    public static void Serialize(IBlackboard bb, ClubMemberRequestsRequest clubMemberRequestsRequest)
    {
        if (clubMemberRequestsRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", clubMemberRequestsRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", clubMemberRequestsRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "clubId", clubMemberRequestsRequest.clubId);
    }

    public static void Serialize(IBlackboard bb, ClubMemberRequestsResponse clubMemberRequestsResponse)
    {
        if (clubMemberRequestsResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", clubMemberRequestsResponse.error);
        if (clubMemberRequestsResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), clubMemberRequestsResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", clubMemberRequestsResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "clubId", clubMemberRequestsResponse.clubId);
        BlackboardUtils.SetOrCreateList(bb, "clubRequests", clubMemberRequestsResponse.clubRequests, Serialize);
    }

    public static void Serialize(IBlackboard bb, ClubMissionContribution clubMissionContribution)
    {
        if (clubMissionContribution == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "userId", clubMissionContribution.userId);
        BlackboardUtils.SetOrCreateValue(bb, "name", clubMissionContribution.name);
        BlackboardUtils.SetOrCreateValue(bb, "profileUrl", clubMissionContribution.profileUrl);
        BlackboardUtils.SetOrCreateValue(bb, "tier", clubMissionContribution.tier);
        BlackboardUtils.SetOrCreateValue(bb, "count", clubMissionContribution.count);
    }

    public static void Serialize(IBlackboard bb, ClubMissionContributionList clubMissionContributionList)
    {
        if (clubMissionContributionList == null) { return; }
        BlackboardUtils.SetOrCreateList(bb, "contributionList", clubMissionContributionList.contributionList, Serialize);
    }

    public static void Serialize(IBlackboard bb, ClubRequest clubRequest)
    {
        if (clubRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "userId", clubRequest.userId);
        BlackboardUtils.SetOrCreateValue(bb, "name", clubRequest.name);
        BlackboardUtils.SetOrCreateValue(bb, "profileUrl", clubRequest.profileUrl);
        BlackboardUtils.SetOrCreateValue(bb, "level", clubRequest.level);
        BlackboardUtils.SetOrCreateValue(bb, "tier", clubRequest.tier);
        BlackboardUtils.SetOrCreateValue(bb, "credit", clubRequest.credit);
    }

    public static void Serialize(IBlackboard bb, ClubSearchFilterRequest clubSearchFilterRequest)
    {
        if (clubSearchFilterRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", clubSearchFilterRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", clubSearchFilterRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "minClubLevel", clubSearchFilterRequest.minClubLevel);
        BlackboardUtils.SetOrCreateValue(bb, "minPlayerLevel", clubSearchFilterRequest.minPlayerLevel);
        BlackboardUtils.SetOrCreateValue(bb, "clubJoinSearchType", clubSearchFilterRequest.clubJoinSearchType);
        BlackboardUtils.SetOrCreateValue(bb, "availableForMe", clubSearchFilterRequest.availableForMe);
        BlackboardUtils.SetOrCreateValue(bb, "friendsInside", clubSearchFilterRequest.friendsInside);
    }

    public static void Serialize(IBlackboard bb, ClubSearchListResponse clubSearchListResponse)
    {
        if (clubSearchListResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", clubSearchListResponse.error);
        if (clubSearchListResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), clubSearchListResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", clubSearchListResponse.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "clubList", clubSearchListResponse.clubList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "analyticContextId", clubSearchListResponse.analyticContextId);
    }

    public static void Serialize(IBlackboard bb, ClubSearchNameRequest clubSearchNameRequest)
    {
        if (clubSearchNameRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", clubSearchNameRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", clubSearchNameRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "searchName", clubSearchNameRequest.searchName);
    }

    public static void Serialize(IBlackboard bb, ClubShareInfo clubShareInfo)
    {
        if (clubShareInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "type", clubShareInfo.type);
        if (clubShareInfo.info != null)
        {
            switch (clubShareInfo.type)
            {
            case EventInfoType.COLLECTING_GAME:
                Serialize(bb, (ClubShareInfoCollectingGame)clubShareInfo.info);
                break;
            }
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "info");
        }
    }

    public static void Serialize(IBlackboard bb, ClubShareInfoCollectingGame clubShareInfoCollectingGame)
    {
        if (clubShareInfoCollectingGame == null) { return; }
        BlackboardUtils.SetOrCreateList(bb, "pieceList", clubShareInfoCollectingGame.pieceList, Serialize);
    }

    public static void Serialize(IBlackboard bb, ClubShareInfoRequest clubShareInfoRequest)
    {
        if (clubShareInfoRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", clubShareInfoRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", clubShareInfoRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "clubId", clubShareInfoRequest.clubId);
        BlackboardUtils.SetOrCreateValue(bb, "metaGameEventId", clubShareInfoRequest.metaGameEventId);
    }

    public static void Serialize(IBlackboard bb, ClubShareInfoResponse clubShareInfoResponse)
    {
        if (clubShareInfoResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", clubShareInfoResponse.error);
        if (clubShareInfoResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), clubShareInfoResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", clubShareInfoResponse.serverTime);
        if (clubShareInfoResponse.clubShareInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "clubShareInfo"), clubShareInfoResponse.clubShareInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "clubShareInfo");
        }
    }

    public static void Serialize(IBlackboard bb, ClubShareRequestRequest clubShareRequestRequest)
    {
        if (clubShareRequestRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", clubShareRequestRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", clubShareRequestRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "clubId", clubShareRequestRequest.clubId);
        BlackboardUtils.SetOrCreateValue(bb, "metaGameEventId", clubShareRequestRequest.metaGameEventId);
        BlackboardUtils.SetOrCreateValue(bb, "pieceId", clubShareRequestRequest.pieceId);
        BlackboardUtils.SetOrCreateValue(bb, "requestType", clubShareRequestRequest.requestType);
    }

    public static void Serialize(IBlackboard bb, ClubShareRequestResponse clubShareRequestResponse)
    {
        if (clubShareRequestResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", clubShareRequestResponse.error);
        if (clubShareRequestResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), clubShareRequestResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", clubShareRequestResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "metaGameRequestResetTimestamp", clubShareRequestResponse.metaGameRequestResetTimestamp);
    }

    public static void Serialize(IBlackboard bb, ClubStatusResponse clubStatusResponse)
    {
        if (clubStatusResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", clubStatusResponse.error);
        if (clubStatusResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), clubStatusResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", clubStatusResponse.serverTime);
        if (clubStatusResponse.userClubStateInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userClubStateInfo"), clubStatusResponse.userClubStateInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userClubStateInfo");
        }
    }

    public static void Serialize(IBlackboard bb, ClubTierInfo clubTierInfo)
    {
        if (clubTierInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "clubId", clubTierInfo.clubId);
        BlackboardUtils.SetOrCreateValue(bb, "week", clubTierInfo.week);
        BlackboardUtils.SetOrCreateValue(bb, "leaguePoint", clubTierInfo.leaguePoint);
        BlackboardUtils.SetOrCreateValue(bb, "leagueTier", clubTierInfo.leagueTier);
    }

    public static void Serialize(IBlackboard bb, ClubUpdateNoticeRequest clubUpdateNoticeRequest)
    {
        if (clubUpdateNoticeRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", clubUpdateNoticeRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", clubUpdateNoticeRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "clubId", clubUpdateNoticeRequest.clubId);
        BlackboardUtils.SetOrCreateValue(bb, "notice", clubUpdateNoticeRequest.notice);
    }

    public static void Serialize(IBlackboard bb, ClubUpdateRequest clubUpdateRequest)
    {
        if (clubUpdateRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", clubUpdateRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", clubUpdateRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "clubId", clubUpdateRequest.clubId);
        BlackboardUtils.SetOrCreateValue(bb, "symbol", clubUpdateRequest.symbol);
        BlackboardUtils.SetOrCreateValue(bb, "motd", clubUpdateRequest.motd);
        BlackboardUtils.SetOrCreateValue(bb, "watcher", clubUpdateRequest.watcher);
        BlackboardUtils.SetOrCreateValue(bb, "minPlayerLevel", clubUpdateRequest.minPlayerLevel);
        BlackboardUtils.SetOrCreateValue(bb, "joinType", clubUpdateRequest.joinType);
    }

    public static void Serialize(IBlackboard bb, ClubUpdateResponse clubUpdateResponse)
    {
        if (clubUpdateResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", clubUpdateResponse.error);
        if (clubUpdateResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), clubUpdateResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", clubUpdateResponse.serverTime);
        if (clubUpdateResponse.clubInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "clubInfo"), clubUpdateResponse.clubInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "clubInfo");
        }
    }

    public static void Serialize(IBlackboard bb, CollectingGameEnterInfo collectingGameEnterInfo)
    {
        if (collectingGameEnterInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "totalPossessions", collectingGameEnterInfo.totalPossessions);
        BlackboardUtils.SetOrCreateList(bb, "gaugeLevelList", collectingGameEnterInfo.gaugeLevelList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "completeScratchers", collectingGameEnterInfo.completeScratchers);
    }

    public static void Serialize(IBlackboard bb, CollectingGameGaugeInfo collectingGameGaugeInfo)
    {
        if (collectingGameGaugeInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "bet", collectingGameGaugeInfo.bet);
        BlackboardUtils.SetOrCreateValue(bb, "gaugeLevel", collectingGameGaugeInfo.gaugeLevel);
    }

    public static void Serialize(IBlackboard bb, CollectingGameInfoRequest collectingGameInfoRequest)
    {
        if (collectingGameInfoRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", collectingGameInfoRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", collectingGameInfoRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "eventId", collectingGameInfoRequest.eventId);
    }

    public static void Serialize(IBlackboard bb, CollectingGameInfoResponseV1 collectingGameInfoResponseV1)
    {
        if (collectingGameInfoResponseV1 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", collectingGameInfoResponseV1.error);
        if (collectingGameInfoResponseV1.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), collectingGameInfoResponseV1.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", collectingGameInfoResponseV1.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "scratcherList", collectingGameInfoResponseV1.scratcherList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "packList", collectingGameInfoResponseV1.packList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "lastFreePackCollectTimestamp", collectingGameInfoResponseV1.lastFreePackCollectTimestamp);
        BlackboardUtils.SetOrCreateList(bb, "sharePieceList", collectingGameInfoResponseV1.sharePieceList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "metaGameRequestResetTimestamp", collectingGameInfoResponseV1.metaGameRequestResetTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "lastFreeChestVideoAdsClaimTimestamp", collectingGameInfoResponseV1.lastFreeChestVideoAdsClaimTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "freeChestCooltime", collectingGameInfoResponseV1.freeChestCooltime);
    }

    public static void Serialize(IBlackboard bb, CollectingGamePackFreeRequest collectingGamePackFreeRequest)
    {
        if (collectingGamePackFreeRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", collectingGamePackFreeRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", collectingGamePackFreeRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "eventId", collectingGamePackFreeRequest.eventId);
    }

    public static void Serialize(IBlackboard bb, CollectingGamePackFreeResponseV1 collectingGamePackFreeResponseV1)
    {
        if (collectingGamePackFreeResponseV1 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", collectingGamePackFreeResponseV1.error);
        if (collectingGamePackFreeResponseV1.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), collectingGamePackFreeResponseV1.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", collectingGamePackFreeResponseV1.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "packList", collectingGamePackFreeResponseV1.packList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "lastFreePackCollectTimestamp", collectingGamePackFreeResponseV1.lastFreePackCollectTimestamp);
        if (collectingGamePackFreeResponseV1.freePackInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "freePackInfo"), collectingGamePackFreeResponseV1.freePackInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "freePackInfo");
        }
    }

    public static void Serialize(IBlackboard bb, CollectingGamePackFreeVideoAdsRequest collectingGamePackFreeVideoAdsRequest)
    {
        if (collectingGamePackFreeVideoAdsRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", collectingGamePackFreeVideoAdsRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", collectingGamePackFreeVideoAdsRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "eventId", collectingGamePackFreeVideoAdsRequest.eventId);
    }

    public static void Serialize(IBlackboard bb, CollectingGamePackFreeVideoAdsResponse collectingGamePackFreeVideoAdsResponse)
    {
        if (collectingGamePackFreeVideoAdsResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", collectingGamePackFreeVideoAdsResponse.error);
        if (collectingGamePackFreeVideoAdsResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), collectingGamePackFreeVideoAdsResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", collectingGamePackFreeVideoAdsResponse.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "packList", collectingGamePackFreeVideoAdsResponse.packList, Serialize);
        if (collectingGamePackFreeVideoAdsResponse.freePackInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "freePackInfo"), collectingGamePackFreeVideoAdsResponse.freePackInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "freePackInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "lastFreeChestVideoAdsClaimTimestamp", collectingGamePackFreeVideoAdsResponse.lastFreeChestVideoAdsClaimTimestamp);
    }

    public static void Serialize(IBlackboard bb, CollectingGamePackOpenInfo collectingGamePackOpenInfo)
    {
        if (collectingGamePackOpenInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "pieceId", collectingGamePackOpenInfo.pieceId);
        BlackboardUtils.SetOrCreateValue(bb, "rarity", collectingGamePackOpenInfo.rarity);
        BlackboardUtils.SetOrCreateValue(bb, "count", collectingGamePackOpenInfo.count);
    }

    public static void Serialize(IBlackboard bb, CollectingGamePackOpenRequest collectingGamePackOpenRequest)
    {
        if (collectingGamePackOpenRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", collectingGamePackOpenRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", collectingGamePackOpenRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "eventId", collectingGamePackOpenRequest.eventId);
        BlackboardUtils.SetOrCreateValue(bb, "packId", collectingGamePackOpenRequest.packId);
    }

    public static void Serialize(IBlackboard bb, CollectingGamePackOpenResponse collectingGamePackOpenResponse)
    {
        if (collectingGamePackOpenResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", collectingGamePackOpenResponse.error);
        if (collectingGamePackOpenResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), collectingGamePackOpenResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", collectingGamePackOpenResponse.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "scratcherList", collectingGamePackOpenResponse.scratcherList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "packList", collectingGamePackOpenResponse.packList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "packOpenResultList", collectingGamePackOpenResponse.packOpenResultList, Serialize);
    }

    public static void Serialize(IBlackboard bb, CollectingGamePackUpdateInfo collectingGamePackUpdateInfo)
    {
        if (collectingGamePackUpdateInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "packId", collectingGamePackUpdateInfo.packId);
        BlackboardUtils.SetOrCreateValue(bb, "totalPossessions", collectingGamePackUpdateInfo.totalPossessions);
        BlackboardUtils.SetOrCreateValue(bb, "completeScratchers", collectingGamePackUpdateInfo.completeScratchers);
    }

    public static void Serialize(IBlackboard bb, CollectingGameScratcherInfo collectingGameScratcherInfo)
    {
        if (collectingGameScratcherInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "scratcherId", collectingGameScratcherInfo.scratcherId);
        BlackboardUtils.SetOrCreateList(bb, "pieceList", collectingGameScratcherInfo.pieceList, Serialize);
        if (collectingGameScratcherInfo.reward != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "reward"), collectingGameScratcherInfo.reward);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "reward");
        }
        BlackboardUtils.SetOrCreateValue(bb, "scratcherSimpleImageUrl", collectingGameScratcherInfo.scratcherSimpleImageUrl);
    }

    public static void Serialize(IBlackboard bb, CollectingGameScratcherRedeemRequest collectingGameScratcherRedeemRequest)
    {
        if (collectingGameScratcherRedeemRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", collectingGameScratcherRedeemRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", collectingGameScratcherRedeemRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "eventId", collectingGameScratcherRedeemRequest.eventId);
        BlackboardUtils.SetOrCreateValue(bb, "scratcherId", collectingGameScratcherRedeemRequest.scratcherId);
    }

    public static void Serialize(IBlackboard bb, CollectingGameScratcherRedeemResponse collectingGameScratcherRedeemResponse)
    {
        if (collectingGameScratcherRedeemResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", collectingGameScratcherRedeemResponse.error);
        if (collectingGameScratcherRedeemResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), collectingGameScratcherRedeemResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", collectingGameScratcherRedeemResponse.serverTime);
        if (collectingGameScratcherRedeemResponse.updatedScratcherInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "updatedScratcherInfo"), collectingGameScratcherRedeemResponse.updatedScratcherInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "updatedScratcherInfo");
        }
        if (collectingGameScratcherRedeemResponse.rewardResult != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "rewardResult"), collectingGameScratcherRedeemResponse.rewardResult);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "rewardResult");
        }
        if (collectingGameScratcherRedeemResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), collectingGameScratcherRedeemResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
    }

    public static void Serialize(IBlackboard bb, CommonResponse commonResponse)
    {
        if (commonResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "ongoingEventTimestamp", commonResponse.ongoingEventTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "ongoingEventIdList", commonResponse.ongoingEventIdList);
        BlackboardUtils.SetOrCreateValue(bb, "MOG", commonResponse.MOG);
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", commonResponse.blockseq);
    }

    public static void Serialize(IBlackboard bb, ConsentRequest consentRequest)
    {
        if (consentRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", consentRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", consentRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "consent", consentRequest.consent);
        BlackboardUtils.SetOrCreateValue(bb, "version", consentRequest.version);
    }

    public static void Serialize(IBlackboard bb, CountryInfo countryInfo)
    {
        if (countryInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "country", countryInfo.country);
        BlackboardUtils.SetOrCreateValue(bb, "countryName", countryInfo.countryName);
    }

    public static void Serialize(IBlackboard bb, Coupon coupon)
    {
        if (coupon == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "code", coupon.code);
        BlackboardUtils.SetOrCreateValue(bb, "resultImageUrl", coupon.resultImageUrl);
        BlackboardUtils.SetOrCreateValue(bb, "type", coupon.type);
    }

    public static void Serialize(IBlackboard bb, CouponRequest couponRequest)
    {
        if (couponRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", couponRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", couponRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "code", couponRequest.code);
        BlackboardUtils.SetOrCreateValue(bb, "currentTime", couponRequest.currentTime);
        BlackboardUtils.SetOrCreateValue(bb, "timezoneOffset", couponRequest.timezoneOffset);
    }

    public static void Serialize(IBlackboard bb, CouponResponse couponResponse)
    {
        if (couponResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", couponResponse.error);
        if (couponResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), couponResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", couponResponse.serverTime);
        if (couponResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), couponResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
        BlackboardUtils.SetOrCreateList(bb, "rewardResultList", couponResponse.rewardResultList, Serialize);
        if (couponResponse.coupon != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "coupon"), couponResponse.coupon);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "coupon");
        }
    }

    public static void Serialize(IBlackboard bb, DailyBingoBoard dailyBingoBoard)
    {
        if (dailyBingoBoard == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "lastCollectIndex", dailyBingoBoard.lastCollectIndex);
        BlackboardUtils.SetOrCreateList(bb, "cells", dailyBingoBoard.cells, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "bingoRewardList", dailyBingoBoard.bingoRewardList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "creditMultiplier", dailyBingoBoard.creditMultiplier);
        BlackboardUtils.SetOrCreateValue(bb, "daysLeft", dailyBingoBoard.daysLeft);
        if (dailyBingoBoard.colorSetting != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "colorSetting"), dailyBingoBoard.colorSetting);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "colorSetting");
        }
        BlackboardUtils.SetOrCreateValue(bb, "backgroundUrl", dailyBingoBoard.backgroundUrl);
        BlackboardUtils.SetOrCreateValue(bb, "totalWorth", dailyBingoBoard.totalWorth);
    }

    public static void Serialize(IBlackboard bb, DailyBingoBoardCell dailyBingoBoardCell)
    {
        if (dailyBingoBoardCell == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "credit", dailyBingoBoardCell.credit);
        BlackboardUtils.SetOrCreateValue(bb, "free", dailyBingoBoardCell.free);
        BlackboardUtils.SetOrCreateValue(bb, "highlight", dailyBingoBoardCell.highlight);
    }

    public static void Serialize(IBlackboard bb, DailyBingoColorSetting dailyBingoColorSetting)
    {
        if (dailyBingoColorSetting == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "todayTextColor", dailyBingoColorSetting.todayTextColor);
        BlackboardUtils.SetOrCreateValue(bb, "cellTextColor", dailyBingoColorSetting.cellTextColor);
        BlackboardUtils.SetOrCreateValue(bb, "cellHighlightTextColor", dailyBingoColorSetting.cellHighlightTextColor);
        BlackboardUtils.SetOrCreateValue(bb, "cellOverlayColor", dailyBingoColorSetting.cellOverlayColor);
        BlackboardUtils.SetOrCreateValue(bb, "stampTextColor", dailyBingoColorSetting.stampTextColor);
        BlackboardUtils.SetOrCreateValue(bb, "highlightStrokeColor", dailyBingoColorSetting.highlightStrokeColor);
        BlackboardUtils.SetOrCreateValue(bb, "descriptionTextColor", dailyBingoColorSetting.descriptionTextColor);
        BlackboardUtils.SetOrCreateValue(bb, "daysLeftTextColor", dailyBingoColorSetting.daysLeftTextColor);
        BlackboardUtils.SetOrCreateValue(bb, "totalWinTextColor", dailyBingoColorSetting.totalWinTextColor);
        BlackboardUtils.SetOrCreateValue(bb, "rewardTextColor", dailyBingoColorSetting.rewardTextColor);
    }

    public static void Serialize(IBlackboard bb, DailyBingoInfo dailyBingoInfo)
    {
        if (dailyBingoInfo == null) { return; }
        if (dailyBingoInfo.board != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "board"), dailyBingoInfo.board);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "board");
        }
        BlackboardUtils.SetOrCreateList(bb, "rewardList", dailyBingoInfo.rewardList, Serialize);
    }

    public static void Serialize(IBlackboard bb, DailyBonus dailyBonus)
    {
        if (dailyBonus == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "consecutiveCredit", dailyBonus.consecutiveCredit);
        BlackboardUtils.SetOrCreateValue(bb, "consecutiveMax", dailyBonus.consecutiveMax);
        BlackboardUtils.SetOrCreateValue(bb, "friendBonusCredit", dailyBonus.friendBonusCredit);
        BlackboardUtils.SetOrCreateValue(bb, "friendBonusMaxCount", dailyBonus.friendBonusMaxCount);
        BlackboardUtils.SetOrCreateValue(bb, "wheelBaseCreditList", dailyBonus.wheelBaseCreditList);
        BlackboardUtils.SetOrCreateValue(bb, "wheelBaseCreditProbability", dailyBonus.wheelBaseCreditProbability);
    }

    public static void Serialize(IBlackboard bb, DailyBonusResult dailyBonusResult)
    {
        if (dailyBonusResult == null) { return; }
        if (dailyBonusResult.wheelBonus != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "wheelBonus"), dailyBonusResult.wheelBonus);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "wheelBonus");
        }
        if (dailyBonusResult.extraBonus != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "extraBonus"), dailyBonusResult.extraBonus);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "extraBonus");
        }
        BlackboardUtils.SetOrCreateValue(bb, "earnCredit", dailyBonusResult.earnCredit);
        BlackboardUtils.SetOrCreateValue(bb, "multiplier", dailyBonusResult.multiplier);
        BlackboardUtils.SetOrCreateList(bb, "jackpotList", dailyBonusResult.jackpotList, Serialize);
    }

    public static void Serialize(IBlackboard bb, DailyBonusWheelSpinRequest dailyBonusWheelSpinRequest)
    {
        if (dailyBonusWheelSpinRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", dailyBonusWheelSpinRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "currentTime", dailyBonusWheelSpinRequest.currentTime);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", dailyBonusWheelSpinRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "dailyWheelEventId", dailyBonusWheelSpinRequest.dailyWheelEventId);
        BlackboardUtils.SetOrCreateValue(bb, "timezoneOffset", dailyBonusWheelSpinRequest.timezoneOffset);
        BlackboardUtils.SetOrCreateValue(bb, "isAutoSpin", dailyBonusWheelSpinRequest.isAutoSpin);
    }

    public static void Serialize(IBlackboard bb, DailyBonusWheelSpinResponse dailyBonusWheelSpinResponse)
    {
        if (dailyBonusWheelSpinResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", dailyBonusWheelSpinResponse.error);
        if (dailyBonusWheelSpinResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), dailyBonusWheelSpinResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", dailyBonusWheelSpinResponse.serverTime);
        if (dailyBonusWheelSpinResponse.wheelBonus != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "wheelBonus"), dailyBonusWheelSpinResponse.wheelBonus);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "wheelBonus");
        }
        BlackboardUtils.SetOrCreateValue(bb, "earnCredit", dailyBonusWheelSpinResponse.earnCredit);
        BlackboardUtils.SetOrCreateValue(bb, "multiplier", dailyBonusWheelSpinResponse.multiplier);
        if (dailyBonusWheelSpinResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), dailyBonusWheelSpinResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
        BlackboardUtils.SetOrCreateList(bb, "jackpotList", dailyBonusWheelSpinResponse.jackpotList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "remainSpinCount", dailyBonusWheelSpinResponse.remainSpinCount);
    }

    public static void Serialize(IBlackboard bb, DailyBoost dailyBoost)
    {
        if (dailyBoost == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "credit", dailyBoost.credit);
        BlackboardUtils.SetOrCreateValue(bb, "start", dailyBoost.start);
        BlackboardUtils.SetOrCreateValue(bb, "end", dailyBoost.end);
        BlackboardUtils.SetOrCreateValue(bb, "lastCollect", dailyBoost.lastCollect);
        BlackboardUtils.SetOrCreateValue(bb, "totalCount", dailyBoost.totalCount);
        BlackboardUtils.SetOrCreateValue(bb, "collectCount", dailyBoost.collectCount);
        BlackboardUtils.SetOrCreateValue(bb, "rp", dailyBoost.rp);
        BlackboardUtils.SetOrCreateValue(bb, "gem", dailyBoost.gem);
        BlackboardUtils.SetOrCreateValue(bb, "applyLevelMultiplier", dailyBoost.applyLevelMultiplier);
    }

    public static void Serialize(IBlackboard bb, DailyBoostCollectRequest dailyBoostCollectRequest)
    {
        if (dailyBoostCollectRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", dailyBoostCollectRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "currentTime", dailyBoostCollectRequest.currentTime);
        BlackboardUtils.SetOrCreateValue(bb, "timezoneOffset", dailyBoostCollectRequest.timezoneOffset);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", dailyBoostCollectRequest.ackMask);
    }

    public static void Serialize(IBlackboard bb, DailyBoostCollectResponse dailyBoostCollectResponse)
    {
        if (dailyBoostCollectResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", dailyBoostCollectResponse.error);
        if (dailyBoostCollectResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), dailyBoostCollectResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", dailyBoostCollectResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "earnCredit", dailyBoostCollectResponse.earnCredit);
        if (dailyBoostCollectResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), dailyBoostCollectResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
        if (dailyBoostCollectResponse.dailyBoost != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "dailyBoost"), dailyBoostCollectResponse.dailyBoost);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "dailyBoost");
        }
        BlackboardUtils.SetOrCreateValue(bb, "earnGem", dailyBoostCollectResponse.earnGem);
    }

    public static void Serialize(IBlackboard bb, DailyDeliveryInfo dailyDeliveryInfo)
    {
        if (dailyDeliveryInfo == null) { return; }
        BlackboardUtils.SetOrCreateList(bb, "rewardList", dailyDeliveryInfo.rewardList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "claimedRewardIdxList", dailyDeliveryInfo.claimedRewardIdxList);
        BlackboardUtils.SetOrCreateValue(bb, "nextClaimRewardIdx", dailyDeliveryInfo.nextClaimRewardIdx);
        BlackboardUtils.SetOrCreateValue(bb, "isLastReward", dailyDeliveryInfo.isLastReward);
        BlackboardUtils.SetOrCreateValue(bb, "nextClaimTimestamp", dailyDeliveryInfo.nextClaimTimestamp);
    }

    public static void Serialize(IBlackboard bb, DailyMegaWheel dailyMegaWheel)
    {
        if (dailyMegaWheel == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "wheelBaseCreditList", dailyMegaWheel.wheelBaseCreditList);
        BlackboardUtils.SetOrCreateValue(bb, "wheelBaseCreditProbability", dailyMegaWheel.wheelBaseCreditProbability);
        BlackboardUtils.SetOrCreateValue(bb, "jackpotAmount", dailyMegaWheel.jackpotAmount);
    }

    public static void Serialize(IBlackboard bb, DailyMegaWheelBonus dailyMegaWheelBonus)
    {
        if (dailyMegaWheelBonus == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "rewardType", dailyMegaWheelBonus.rewardType);
        BlackboardUtils.SetOrCreateValue(bb, "wheelIndex", dailyMegaWheelBonus.wheelIndex);
        BlackboardUtils.SetOrCreateValue(bb, "jackpotIndex", dailyMegaWheelBonus.jackpotIndex);
        BlackboardUtils.SetOrCreateValue(bb, "baseCredit", dailyMegaWheelBonus.baseCredit);
    }

    public static void Serialize(IBlackboard bb, DailyMegaWheelSpinRequest dailyMegaWheelSpinRequest)
    {
        if (dailyMegaWheelSpinRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", dailyMegaWheelSpinRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", dailyMegaWheelSpinRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "dailyWheelEventId", dailyMegaWheelSpinRequest.dailyWheelEventId);
    }

    public static void Serialize(IBlackboard bb, DailyMegaWheelSpinResponse dailyMegaWheelSpinResponse)
    {
        if (dailyMegaWheelSpinResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", dailyMegaWheelSpinResponse.error);
        if (dailyMegaWheelSpinResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), dailyMegaWheelSpinResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", dailyMegaWheelSpinResponse.serverTime);
        if (dailyMegaWheelSpinResponse.wheelBonus != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "wheelBonus"), dailyMegaWheelSpinResponse.wheelBonus);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "wheelBonus");
        }
        BlackboardUtils.SetOrCreateValue(bb, "earnCredit", dailyMegaWheelSpinResponse.earnCredit);
        BlackboardUtils.SetOrCreateValue(bb, "multiplier", dailyMegaWheelSpinResponse.multiplier);
        if (dailyMegaWheelSpinResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), dailyMegaWheelSpinResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "remainSpinCount", dailyMegaWheelSpinResponse.remainSpinCount);
    }

    public static void Serialize(IBlackboard bb, DailySpin dailySpin)
    {
        if (dailySpin == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "purchasedWheelBaseCreditList", dailySpin.purchasedWheelBaseCreditList);
        BlackboardUtils.SetOrCreateValue(bb, "purchasedWheelBaseCreditProbability", dailySpin.purchasedWheelBaseCreditProbability);
    }

    public static void Serialize(IBlackboard bb, DailyWheelBonus dailyWheelBonus)
    {
        if (dailyWheelBonus == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "rewardType", dailyWheelBonus.rewardType);
        BlackboardUtils.SetOrCreateValue(bb, "rewardIndex", dailyWheelBonus.rewardIndex);
        BlackboardUtils.SetOrCreateValue(bb, "baseCredit", dailyWheelBonus.baseCredit);
        BlackboardUtils.SetOrCreateValue(bb, "spinCount", dailyWheelBonus.spinCount);
    }

    public static void Serialize(IBlackboard bb, DebugPushOnesignalRequest debugPushOnesignalRequest)
    {
        if (debugPushOnesignalRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", debugPushOnesignalRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", debugPushOnesignalRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "targetUserId", debugPushOnesignalRequest.targetUserId);
        BlackboardUtils.SetOrCreateValue(bb, "message", debugPushOnesignalRequest.message);
        BlackboardUtils.SetOrCreateValue(bb, "title", debugPushOnesignalRequest.title);
        BlackboardUtils.SetOrCreateValue(bb, "encodedCustomData", debugPushOnesignalRequest.encodedCustomData);
    }

    public static void Serialize(IBlackboard bb, DeepLinkData deepLinkData)
    {
        if (deepLinkData == null) { return; }
        if (deepLinkData.action != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "action"), deepLinkData.action);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "action");
        }
        BlackboardUtils.SetOrCreateValue(bb, "type", deepLinkData.type);
        BlackboardUtils.SetOrCreateValue(bb, "ts", deepLinkData.ts);
        BlackboardUtils.SetOrCreateValue(bb, "comment", deepLinkData.comment);
        BlackboardUtils.SetOrCreateValue(bb, "templateId", deepLinkData.templateId);
    }

    public static void Serialize(IBlackboard bb, DepotOpenResult depotOpenResult)
    {
        if (depotOpenResult == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "earnedExp", depotOpenResult.earnedExp);
        BlackboardUtils.SetOrCreateValue(bb, "earnedCredit", depotOpenResult.earnedCredit);
        if (depotOpenResult.updatedBuilding != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "updatedBuilding"), depotOpenResult.updatedBuilding);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "updatedBuilding");
        }
    }

    public static void Serialize(IBlackboard bb, DepotPersonal depotPersonal)
    {
        if (depotPersonal == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "common", depotPersonal.common);
        BlackboardUtils.SetOrCreateValue(bb, "rare", depotPersonal.rare);
        BlackboardUtils.SetOrCreateValue(bb, "epic", depotPersonal.epic);
        BlackboardUtils.SetOrCreateValue(bb, "legendary", depotPersonal.legendary);
    }

    public static void Serialize(IBlackboard bb, DeviceAdjustData deviceAdjustData)
    {
        if (deviceAdjustData == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "deviceId", deviceAdjustData.deviceId);
        BlackboardUtils.SetOrCreateValue(bb, "adid", deviceAdjustData.adid);
        BlackboardUtils.SetOrCreateValue(bb, "trackerToken", deviceAdjustData.trackerToken);
        BlackboardUtils.SetOrCreateValue(bb, "trackerName", deviceAdjustData.trackerName);
        BlackboardUtils.SetOrCreateValue(bb, "network", deviceAdjustData.network);
        BlackboardUtils.SetOrCreateValue(bb, "campaign", deviceAdjustData.campaign);
        BlackboardUtils.SetOrCreateValue(bb, "adgroup", deviceAdjustData.adgroup);
        BlackboardUtils.SetOrCreateValue(bb, "creative", deviceAdjustData.creative);
        BlackboardUtils.SetOrCreateValue(bb, "clickLabel", deviceAdjustData.clickLabel);
        BlackboardUtils.SetOrCreateValue(bb, "createTimestamp", deviceAdjustData.createTimestamp);
    }

    public static void Serialize(IBlackboard bb, DoNotDisturbSetting doNotDisturbSetting)
    {
        if (doNotDisturbSetting == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "HOUR_LIST", doNotDisturbSetting.HOUR_LIST);
        BlackboardUtils.SetOrCreateValue(bb, "ALTERNATIVE_HOUR", doNotDisturbSetting.ALTERNATIVE_HOUR);
    }

    public static void Serialize(IBlackboard bb, EarlyAccess earlyAccess)
    {
        if (earlyAccess == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "isAvailable", earlyAccess.isAvailable);
        BlackboardUtils.SetOrCreateValue(bb, "grade", earlyAccess.grade);
    }

    public static void Serialize(IBlackboard bb, EarlyAccessInfo earlyAccessInfo)
    {
        if (earlyAccessInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "bonusSpinCount", earlyAccessInfo.bonusSpinCount);
    }

    public static void Serialize(IBlackboard bb, EpicAlbumCollectRewardRequest epicAlbumCollectRewardRequest)
    {
        if (epicAlbumCollectRewardRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", epicAlbumCollectRewardRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", epicAlbumCollectRewardRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "category", epicAlbumCollectRewardRequest.category);
        BlackboardUtils.SetOrCreateValue(bb, "rewardStage", epicAlbumCollectRewardRequest.rewardStage);
    }

    public static void Serialize(IBlackboard bb, EpicAlbumCollectRewardResponse epicAlbumCollectRewardResponse)
    {
        if (epicAlbumCollectRewardResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", epicAlbumCollectRewardResponse.error);
        if (epicAlbumCollectRewardResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), epicAlbumCollectRewardResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", epicAlbumCollectRewardResponse.serverTime);
        if (epicAlbumCollectRewardResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), epicAlbumCollectRewardResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "rewardStage", epicAlbumCollectRewardResponse.rewardStage);
        BlackboardUtils.SetOrCreateValue(bb, "nextRequiredStarCount", epicAlbumCollectRewardResponse.nextRequiredStarCount);
        BlackboardUtils.SetOrCreateList(bb, "rewardResultList", epicAlbumCollectRewardResponse.rewardResultList, Serialize);
    }

    public static void Serialize(IBlackboard bb, EpicAlbumEnterResponse epicAlbumEnterResponse)
    {
        if (epicAlbumEnterResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", epicAlbumEnterResponse.error);
        if (epicAlbumEnterResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), epicAlbumEnterResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", epicAlbumEnterResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "albumEntryLimit", epicAlbumEnterResponse.albumEntryLimit);
        BlackboardUtils.SetOrCreateList(bb, "epicAlbumInfoList", epicAlbumEnterResponse.epicAlbumInfoList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "finalRewardGem", epicAlbumEnterResponse.finalRewardGem);
    }

    public static void Serialize(IBlackboard bb, EpicAlbumInfo epicAlbumInfo)
    {
        if (epicAlbumInfo == null) { return; }
        if (epicAlbumInfo.categoryInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "categoryInfo"), epicAlbumInfo.categoryInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "categoryInfo");
        }
        BlackboardUtils.SetOrCreateList(bb, "woeInfoList", epicAlbumInfo.woeInfoList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "totalStarCount", epicAlbumInfo.totalStarCount);
        BlackboardUtils.SetOrCreateValue(bb, "collectedRewardStage", epicAlbumInfo.collectedRewardStage);
        BlackboardUtils.SetOrCreateValue(bb, "nextRequiredStarCount", epicAlbumInfo.nextRequiredStarCount);
    }

    public static void Serialize(IBlackboard bb, ErrorDetailInfoBannedUser errorDetailInfoBannedUser)
    {
        if (errorDetailInfoBannedUser == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "isPermanent", errorDetailInfoBannedUser.isPermanent);
        BlackboardUtils.SetOrCreateValue(bb, "banEndTimestamp", errorDetailInfoBannedUser.banEndTimestamp);
    }

    public static void Serialize(IBlackboard bb, ErrorDetailInfoDeprecatedVersion errorDetailInfoDeprecatedVersion)
    {
        if (errorDetailInfoDeprecatedVersion == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "rewardCredit", errorDetailInfoDeprecatedVersion.rewardCredit);
        BlackboardUtils.SetOrCreateValue(bb, "appDownloadUrl", errorDetailInfoDeprecatedVersion.appDownloadUrl);
        BlackboardUtils.SetOrCreateValue(bb, "minClientNumberVersion", errorDetailInfoDeprecatedVersion.minClientNumberVersion);
    }

    public static void Serialize(IBlackboard bb, ErrorOnlyResponse errorOnlyResponse)
    {
        if (errorOnlyResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", errorOnlyResponse.error);
    }

    public static void Serialize(IBlackboard bb, ErrorResponse errorResponse)
    {
        if (errorResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", errorResponse.error);
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", errorResponse.serverTime);
        if (errorResponse.errorDetailInfo != null)
        {
            switch (errorResponse.error)
            {
            case Error.DEPRECATED_VERSION_ERROR:
                Serialize(bb, (ErrorDetailInfoDeprecatedVersion)errorResponse.errorDetailInfo);
                break;

            case Error.BANNED_USER_ERROR:
                Serialize(bb, (ErrorDetailInfoBannedUser)errorResponse.errorDetailInfo);
                break;
            }
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "errorDetailInfo");
        }
    }

    public static void Serialize(IBlackboard bb, EventDataBonusMultiply eventDataBonusMultiply)
    {
        if (eventDataBonusMultiply == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", eventDataBonusMultiply.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "multiplierNumerator", eventDataBonusMultiply.multiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, EventDataBonusSale eventDataBonusSale)
    {
        if (eventDataBonusSale == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", eventDataBonusSale.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "saleNumerator", eventDataBonusSale.saleNumerator);
    }

    public static void Serialize(IBlackboard bb, EventDataBossRaiders eventDataBossRaiders)
    {
        if (eventDataBossRaiders == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "themeId", eventDataBossRaiders.themeId);
    }

    public static void Serialize(IBlackboard bb, EventDataBuildDreamSeason eventDataBuildDreamSeason)
    {
        if (eventDataBuildDreamSeason == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "seasonThemeId", eventDataBuildDreamSeason.seasonThemeId);
    }

    public static void Serialize(IBlackboard bb, EventDataChallengeClaimMultiply eventDataChallengeClaimMultiply)
    {
        if (eventDataChallengeClaimMultiply == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "multiplierNumerator", eventDataChallengeClaimMultiply.multiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, EventDataClubEventChallenge eventDataClubEventChallenge)
    {
        if (eventDataClubEventChallenge == null) { return; }
    }

    public static void Serialize(IBlackboard bb, EventDataCollectingGame eventDataCollectingGame)
    {
        if (eventDataCollectingGame == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "collectingGameId", eventDataCollectingGame.collectingGameId);
        BlackboardUtils.SetOrCreateValue(bb, "collectingGameName", eventDataCollectingGame.collectingGameName);
    }

    public static void Serialize(IBlackboard bb, EventDataCreditMultiplierWheel eventDataCreditMultiplierWheel)
    {
        if (eventDataCreditMultiplierWheel == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "multiplierNumerator", eventDataCreditMultiplierWheel.multiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, EventDataCreditMultiplierWheelMultiply eventDataCreditMultiplierWheelMultiply)
    {
        if (eventDataCreditMultiplierWheelMultiply == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "multiplierNumerator", eventDataCreditMultiplierWheelMultiply.multiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, EventDataDailyWheelMultiply eventDataDailyWheelMultiply)
    {
        if (eventDataDailyWheelMultiply == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "multiplierNumerator", eventDataDailyWheelMultiply.multiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, EventDataDailyWheelMultiplyFreebie eventDataDailyWheelMultiplyFreebie)
    {
        if (eventDataDailyWheelMultiplyFreebie == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "multiplierNumerator", eventDataDailyWheelMultiplyFreebie.multiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, EventDataDiscountMultiply eventDataDiscountMultiply)
    {
        if (eventDataDiscountMultiply == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "multiplierNumerator", eventDataDiscountMultiply.multiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, EventDataDropRateMultiply eventDataDropRateMultiply)
    {
        if (eventDataDropRateMultiply == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "multiplierNumerator", eventDataDropRateMultiply.multiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, EventDataEmpty eventDataEmpty)
    {
        if (eventDataEmpty == null) { return; }
    }

    public static void Serialize(IBlackboard bb, EventDataExpMultiply eventDataExpMultiply)
    {
        if (eventDataExpMultiply == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "multiplierNumerator", eventDataExpMultiply.multiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, EventDataExpMultiplyExtendable eventDataExpMultiplyExtendable)
    {
        if (eventDataExpMultiplyExtendable == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "multiplierNumerator", eventDataExpMultiplyExtendable.multiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, EventDataGemBooster eventDataGemBooster)
    {
        if (eventDataGemBooster == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "multiplierNumerator", eventDataGemBooster.multiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, EventDataGemBoosterMultiply eventDataGemBoosterMultiply)
    {
        if (eventDataGemBoosterMultiply == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "multiplierNumerator", eventDataGemBoosterMultiply.multiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, EventDataJackpotRewardMultiply eventDataJackpotRewardMultiply)
    {
        if (eventDataJackpotRewardMultiply == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "multiplierNumerator", eventDataJackpotRewardMultiply.multiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, EventDataJackpotSpinGemSale eventDataJackpotSpinGemSale)
    {
        if (eventDataJackpotSpinGemSale == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "saleNumerator", eventDataJackpotSpinGemSale.saleNumerator);
    }

    public static void Serialize(IBlackboard bb, EventDataPersonalEventChallenge eventDataPersonalEventChallenge)
    {
        if (eventDataPersonalEventChallenge == null) { return; }
    }

    public static void Serialize(IBlackboard bb, EventDataPiggyBankMultiply eventDataPiggyBankMultiply)
    {
        if (eventDataPiggyBankMultiply == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "multiplierNumerator", eventDataPiggyBankMultiply.multiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, EventDataPiggyBankSale eventDataPiggyBankSale)
    {
        if (eventDataPiggyBankSale == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "productIndex", eventDataPiggyBankSale.productIndex);
    }

    public static void Serialize(IBlackboard bb, EventDataPogBoosterMinMultiplier eventDataPogBoosterMinMultiplier)
    {
        if (eventDataPogBoosterMinMultiplier == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "multiplierNumerator", eventDataPogBoosterMinMultiplier.multiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, EventDataPogBoosterMultiply eventDataPogBoosterMultiply)
    {
        if (eventDataPogBoosterMultiply == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "multiplierNumerator", eventDataPogBoosterMultiply.multiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, EventDataPogBoosterWithout eventDataPogBoosterWithout)
    {
        if (eventDataPogBoosterWithout == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "minMultiplierIndex", eventDataPogBoosterWithout.minMultiplierIndex);
    }

    public static void Serialize(IBlackboard bb, EventDataSeasonPass eventDataSeasonPass)
    {
        if (eventDataSeasonPass == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "seasonPassSettingId", eventDataSeasonPass.seasonPassSettingId);
        BlackboardUtils.SetOrCreateValue(bb, "seasonPassSettingName", eventDataSeasonPass.seasonPassSettingName);
    }

    public static void Serialize(IBlackboard bb, EventDataSeasonPassV2 eventDataSeasonPassV2)
    {
        if (eventDataSeasonPassV2 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "seasonPassSettingV2Id", eventDataSeasonPassV2.seasonPassSettingV2Id);
        BlackboardUtils.SetOrCreateValue(bb, "seasonPassSettingV2Name", eventDataSeasonPassV2.seasonPassSettingV2Name);
    }

    public static void Serialize(IBlackboard bb, EventDataShopMultiplyPassiveEvent eventDataShopMultiplyPassiveEvent)
    {
        if (eventDataShopMultiplyPassiveEvent == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "multiplierNumerator", eventDataShopMultiplyPassiveEvent.multiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, EventDataTierUpShopMultiply eventDataTierUpShopMultiply)
    {
        if (eventDataTierUpShopMultiply == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "multiplierNumerator", eventDataTierUpShopMultiply.multiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, EventDataTimeBonus eventDataTimeBonus)
    {
        if (eventDataTimeBonus == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "multiplierNumerator", eventDataTimeBonus.multiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, EventDataTimeBonusCooltime eventDataTimeBonusCooltime)
    {
        if (eventDataTimeBonusCooltime == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "cooltimeMin", eventDataTimeBonusCooltime.cooltimeMin);
    }

    public static void Serialize(IBlackboard bb, EventDataVoucherShopMultiply eventDataVoucherShopMultiply)
    {
        if (eventDataVoucherShopMultiply == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "multiplierNumerator", eventDataVoucherShopMultiply.multiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, EventInfo eventInfo)
    {
        if (eventInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "id", eventInfo.id);
        BlackboardUtils.SetOrCreateValue(bb, "type", eventInfo.type);
        BlackboardUtils.SetOrCreateValue(bb, "startTimestamp", eventInfo.startTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "endTimestamp", eventInfo.endTimestamp);
        if (eventInfo.constraints != null)
        {
            switch (eventInfo.type)
            {
            case EventInfoType.EXP_MULTIPLY:
                Serialize(bb, (EventDataExpMultiply)eventInfo.constraints);
                break;

            case EventInfoType.TIME_BONUS:
                Serialize(bb, (EventDataTimeBonus)eventInfo.constraints);
                break;

            case EventInfoType.CREDIT_MULTIPLIER_WHEEL:
                Serialize(bb, (EventDataCreditMultiplierWheel)eventInfo.constraints);
                break;

            case EventInfoType.PAYER_RP_MULTIPLY:
                Serialize(bb, (EventDataEmpty)eventInfo.constraints);
                break;

            case EventInfoType.DAILY_WHEEL_MULTIPLY:
                Serialize(bb, (EventDataDailyWheelMultiply)eventInfo.constraints);
                break;

            case EventInfoType.PIGGY_BANK_MULTIPLY:
                Serialize(bb, (EventDataPiggyBankMultiply)eventInfo.constraints);
                break;

            case EventInfoType.PIGGY_BANK_SALE:
                Serialize(bb, (EventDataPiggyBankSale)eventInfo.constraints);
                break;

            case EventInfoType.CHALLENGE_CLAIM_MULTIPLY:
                Serialize(bb, (EventDataChallengeClaimMultiply)eventInfo.constraints);
                break;

            case EventInfoType.FREE_COIN_BOOSTER:
                Serialize(bb, (EventDataEmpty)eventInfo.constraints);
                break;

            case EventInfoType.LUCKY_FIVE:
                Serialize(bb, (EventDataEmpty)eventInfo.constraints);
                break;

            case EventInfoType.TIME_BONUS_COOLTIME:
                Serialize(bb, (EventDataTimeBonusCooltime)eventInfo.constraints);
                break;

            case EventInfoType.LP_BOOST:
                Serialize(bb, (EventDataEmpty)eventInfo.constraints);
                break;

            case EventInfoType.CREDIT_MULTIPLIER_WHEEL_MULTIPLY:
                Serialize(bb, (EventDataCreditMultiplierWheelMultiply)eventInfo.constraints);
                break;

            case EventInfoType.COLLECTING_GAME:
                Serialize(bb, (EventDataCollectingGame)eventInfo.constraints);
                break;

            case EventInfoType.POG_BOOSTER_MULTIPLY:
                Serialize(bb, (EventDataPogBoosterMultiply)eventInfo.constraints);
                break;

            case EventInfoType.POG_BOOSTER_MIN_MULTIPLIER:
                Serialize(bb, (EventDataPogBoosterMinMultiplier)eventInfo.constraints);
                break;

            case EventInfoType.POG_BOOSTER_FREE:
                Serialize(bb, (EventDataEmpty)eventInfo.constraints);
                break;

            case EventInfoType.POG_BOOSTER_WITHOUT:
                Serialize(bb, (EventDataPogBoosterWithout)eventInfo.constraints);
                break;

            case EventInfoType.ALL_GAME_OPEN:
                Serialize(bb, (EventDataEmpty)eventInfo.constraints);
                break;

            case EventInfoType.CRAZY_CLUB_CHALLENGE:
                Serialize(bb, (EventDataEmpty)eventInfo.constraints);
                break;

            case EventInfoType.CRAZY_CHALLENGE:
                Serialize(bb, (EventDataEmpty)eventInfo.constraints);
                break;

            case EventInfoType.BONUS_SALE:
                Serialize(bb, (EventDataBonusSale)eventInfo.constraints);
                break;

            case EventInfoType.BONUS_MULTIPLY:
                Serialize(bb, (EventDataBonusMultiply)eventInfo.constraints);
                break;

            case EventInfoType.COIN_SHOP_EVENT_MULTIPLY:
                Serialize(bb, (EventDataShopMultiplyPassiveEvent)eventInfo.constraints);
                break;

            case EventInfoType.GEM_SHOP_EVENT_MULTIPLY:
                Serialize(bb, (EventDataShopMultiplyPassiveEvent)eventInfo.constraints);
                break;

            case EventInfoType.GEM_BAB_SHOP_EVENT_MULTIPLY:
                Serialize(bb, (EventDataShopMultiplyPassiveEvent)eventInfo.constraints);
                break;

            case EventInfoType.GEM_BOOSTER:
                Serialize(bb, (EventDataGemBooster)eventInfo.constraints);
                break;

            case EventInfoType.FREE_GEM_BOOSTER:
                Serialize(bb, (EventDataEmpty)eventInfo.constraints);
                break;

            case EventInfoType.GEM_BOOSTER_MULTIPLY:
                Serialize(bb, (EventDataGemBoosterMultiply)eventInfo.constraints);
                break;

            case EventInfoType.COIN_SHOP_DISCOUNT_EVENT_MULTIPLY:
                Serialize(bb, (EventDataDiscountMultiply)eventInfo.constraints);
                break;

            case EventInfoType.GEM_SHOP_DISCOUNT_EVENT_MULTIPLY:
                Serialize(bb, (EventDataDiscountMultiply)eventInfo.constraints);
                break;

            case EventInfoType.POG_DISCOUNT_EVENT_MULTIPLY:
                Serialize(bb, (EventDataDiscountMultiply)eventInfo.constraints);
                break;

            case EventInfoType.GEM_BAB_SHOP_DISCOUNT_EVENT_MULTIPLY:
                Serialize(bb, (EventDataDiscountMultiply)eventInfo.constraints);
                break;

            case EventInfoType.GEM_BAB_PROMOTION_SHOP_EVENT_MULTIPLY:
                Serialize(bb, (EventDataShopMultiplyPassiveEvent)eventInfo.constraints);
                break;

            case EventInfoType.GEM_BAB_PROMOTION_SHOP_DISCOUNT_EVENT_MULTIPLY:
                Serialize(bb, (EventDataDiscountMultiply)eventInfo.constraints);
                break;

            case EventInfoType.COLLECTING_GAME_CHEST_DROP_RATE_MULTIPLY:
                Serialize(bb, (EventDataDropRateMultiply)eventInfo.constraints);
                break;

            case EventInfoType.VOUCHER_SHOP_EVENT_MULTIPLY:
                Serialize(bb, (EventDataVoucherShopMultiply)eventInfo.constraints);
                break;

            case EventInfoType.TIER_UP_SHOP_EVENT_MULTIPLY:
                Serialize(bb, (EventDataTierUpShopMultiply)eventInfo.constraints);
                break;

            case EventInfoType.SEASON_PASS:
                Serialize(bb, (EventDataSeasonPass)eventInfo.constraints);
                break;

            case EventInfoType.GEM_JACKPOT:
                Serialize(bb, (EventDataEmpty)eventInfo.constraints);
                break;

            case EventInfoType.GEM_JACKPOT_REWARD_MULTIPLY:
                Serialize(bb, (EventDataJackpotRewardMultiply)eventInfo.constraints);
                break;

            case EventInfoType.GEM_JACKPOT_SPIN_GEM_SALE:
                Serialize(bb, (EventDataJackpotSpinGemSale)eventInfo.constraints);
                break;

            case EventInfoType.DAILY_WHEEL_WEDGE:
                Serialize(bb, (EventDataEmpty)eventInfo.constraints);
                break;

            case EventInfoType.PERSONAL_EVENT_CHALLENGE:
                Serialize(bb, (EventDataPersonalEventChallenge)eventInfo.constraints);
                break;

            case EventInfoType.CLUB_EVENT_CHALLENGE:
                Serialize(bb, (EventDataClubEventChallenge)eventInfo.constraints);
                break;

            case EventInfoType.BOSS_RAIDERS:
                Serialize(bb, (EventDataBossRaiders)eventInfo.constraints);
                break;

            case EventInfoType.CLUB_ARENA:
                Serialize(bb, (EventDataEmpty)eventInfo.constraints);
                break;

            case EventInfoType.VIP_LOUNGE:
                Serialize(bb, (EventDataEmpty)eventInfo.constraints);
                break;

            case EventInfoType.DAILY_WHEEL_MULTIPLY_FREEBIE:
                Serialize(bb, (EventDataDailyWheelMultiplyFreebie)eventInfo.constraints);
                break;

            case EventInfoType.LEVEL_UP_DASH:
                Serialize(bb, (EventDataEmpty)eventInfo.constraints);
                break;

            case EventInfoType.LEVEL_UP_DASH_MISSION:
                Serialize(bb, (EventDataEmpty)eventInfo.constraints);
                break;

            case EventInfoType.BUILD_DREAM_SEASON:
                Serialize(bb, (EventDataBuildDreamSeason)eventInfo.constraints);
                break;

            case EventInfoType.SEASON_PASS_V2:
                Serialize(bb, (EventDataSeasonPassV2)eventInfo.constraints);
                break;

            case EventInfoType.EXP_MULTIPLY_EXTENDABLE:
                Serialize(bb, (EventDataExpMultiplyExtendable)eventInfo.constraints);
                break;
            }
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "constraints");
        }
        BlackboardUtils.SetOrCreateValue(bb, "isPersonal", eventInfo.isPersonal);
        BlackboardUtils.SetOrCreateValue(bb, "hideBadge", eventInfo.hideBadge);
    }

    public static void Serialize(IBlackboard bb, EventRequest eventRequest)
    {
        if (eventRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", eventRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", eventRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "eventIdList", eventRequest.eventIdList);
    }

    public static void Serialize(IBlackboard bb, EventResponse eventResponse)
    {
        if (eventResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", eventResponse.error);
        if (eventResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), eventResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", eventResponse.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "ongoingEventList", eventResponse.ongoingEventList, Serialize);
    }

    public static void Serialize(IBlackboard bb, ExpiryInfo expiryInfo)
    {
        if (expiryInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "type", expiryInfo.type);
        BlackboardUtils.SetOrCreateValue(bb, "value", expiryInfo.value);
    }

    public static void Serialize(IBlackboard bb, ExtraBonus extraBonus)
    {
        if (extraBonus == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "friendCount", extraBonus.friendCount);
        BlackboardUtils.SetOrCreateValue(bb, "friendBonusCredit", extraBonus.friendBonusCredit);
        BlackboardUtils.SetOrCreateValue(bb, "consecutiveCount", extraBonus.consecutiveCount);
        BlackboardUtils.SetOrCreateValue(bb, "consecutiveCredit", extraBonus.consecutiveCredit);
    }

    public static void Serialize(IBlackboard bb, FacebookShareImageUrl facebookShareImageUrl)
    {
        if (facebookShareImageUrl == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "TIER_UP", facebookShareImageUrl.TIER_UP);
        BlackboardUtils.SetOrCreateValue(bb, "LEVEL_UP", facebookShareImageUrl.LEVEL_UP);
        BlackboardUtils.SetOrCreateValue(bb, "SUPER_MEGA_WIN", facebookShareImageUrl.SUPER_MEGA_WIN);
        BlackboardUtils.SetOrCreateValue(bb, "EPIC_WIN", facebookShareImageUrl.EPIC_WIN);
        BlackboardUtils.SetOrCreateValue(bb, "MEGA_JACKPOT", facebookShareImageUrl.MEGA_JACKPOT);
        BlackboardUtils.SetOrCreateValue(bb, "GRAND_JACKPOT", facebookShareImageUrl.GRAND_JACKPOT);
        BlackboardUtils.SetOrCreateValue(bb, "DAILY_WHEEL_JACKPOT", facebookShareImageUrl.DAILY_WHEEL_JACKPOT);
        BlackboardUtils.SetOrCreateValue(bb, "JGQ_JACKPOT", facebookShareImageUrl.JGQ_JACKPOT);
        BlackboardUtils.SetOrCreateValue(bb, "WLD_JACKPOT", facebookShareImageUrl.WLD_JACKPOT);
        BlackboardUtils.SetOrCreateValue(bb, "BNB_JACKPOT", facebookShareImageUrl.BNB_JACKPOT);
        BlackboardUtils.SetOrCreateValue(bb, "SRS_JACKPOT", facebookShareImageUrl.SRS_JACKPOT);
        BlackboardUtils.SetOrCreateValue(bb, "MJF_JACKPOT", facebookShareImageUrl.MJF_JACKPOT);
        BlackboardUtils.SetOrCreateValue(bb, "TRD_JACKPOT", facebookShareImageUrl.TRD_JACKPOT);
        BlackboardUtils.SetOrCreateValue(bb, "ATC_JACKPOT", facebookShareImageUrl.ATC_JACKPOT);
        BlackboardUtils.SetOrCreateValue(bb, "DGR_JACKPOT", facebookShareImageUrl.DGR_JACKPOT);
        BlackboardUtils.SetOrCreateValue(bb, "RWS_JACKPOT", facebookShareImageUrl.RWS_JACKPOT);
        BlackboardUtils.SetOrCreateValue(bb, "MYD_JACKPOT", facebookShareImageUrl.MYD_JACKPOT);
        BlackboardUtils.SetOrCreateValue(bb, "MAR_JACKPOT", facebookShareImageUrl.MAR_JACKPOT);
        BlackboardUtils.SetOrCreateValue(bb, "LGS_JACKPOT", facebookShareImageUrl.LGS_JACKPOT);
        BlackboardUtils.SetOrCreateValue(bb, "GCB_JACKPOT", facebookShareImageUrl.GCB_JACKPOT);
        BlackboardUtils.SetOrCreateValue(bb, "CSC_JACKPOT", facebookShareImageUrl.CSC_JACKPOT);
        BlackboardUtils.SetOrCreateValue(bb, "RHR_JACKPOT", facebookShareImageUrl.RHR_JACKPOT);
        BlackboardUtils.SetOrCreateValue(bb, "LPS_JACKPOT", facebookShareImageUrl.LPS_JACKPOT);
        BlackboardUtils.SetOrCreateValue(bb, "ROG_JACKPOT", facebookShareImageUrl.ROG_JACKPOT);
        BlackboardUtils.SetOrCreateValue(bb, "DNW_JACKPOT", facebookShareImageUrl.DNW_JACKPOT);
    }

    public static void Serialize(IBlackboard bb, FcfsTicketAddResponse fcfsTicketAddResponse)
    {
        if (fcfsTicketAddResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", fcfsTicketAddResponse.error);
        if (fcfsTicketAddResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), fcfsTicketAddResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", fcfsTicketAddResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "encodedAction", fcfsTicketAddResponse.encodedAction);
    }

    public static void Serialize(IBlackboard bb, FcfsTicketUseRequest fcfsTicketUseRequest)
    {
        if (fcfsTicketUseRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", fcfsTicketUseRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", fcfsTicketUseRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "fcfsTicketId", fcfsTicketUseRequest.fcfsTicketId);
        BlackboardUtils.SetOrCreateValue(bb, "deeplinkUrl", fcfsTicketUseRequest.deeplinkUrl);
    }

    public static void Serialize(IBlackboard bb, FcfsTicketUseResponse fcfsTicketUseResponse)
    {
        if (fcfsTicketUseResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", fcfsTicketUseResponse.error);
        if (fcfsTicketUseResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), fcfsTicketUseResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", fcfsTicketUseResponse.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "rewardResultList", fcfsTicketUseResponse.rewardResultList, Serialize);
    }

    public static void Serialize(IBlackboard bb, FreeVipDeal freeVipDeal)
    {
        if (freeVipDeal == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "isFree", freeVipDeal.isFree);
        BlackboardUtils.SetOrCreateValue(bb, "creditAmount", freeVipDeal.creditAmount);
        BlackboardUtils.SetOrCreateValue(bb, "multiplierNumerator", freeVipDeal.multiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, FriendAcceptAllResponse friendAcceptAllResponse)
    {
        if (friendAcceptAllResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", friendAcceptAllResponse.error);
        if (friendAcceptAllResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), friendAcceptAllResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", friendAcceptAllResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "acceptedUserIdList", friendAcceptAllResponse.acceptedUserIdList);
        BlackboardUtils.SetOrCreateValue(bb, "isNormalFriendCountMax", friendAcceptAllResponse.isNormalFriendCountMax);
    }

    public static void Serialize(IBlackboard bb, FriendAcceptRequest friendAcceptRequest)
    {
        if (friendAcceptRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", friendAcceptRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "targetUserId", friendAcceptRequest.targetUserId);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", friendAcceptRequest.ackMask);
    }

    public static void Serialize(IBlackboard bb, FriendAcceptResponse friendAcceptResponse)
    {
        if (friendAcceptResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", friendAcceptResponse.error);
        if (friendAcceptResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), friendAcceptResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", friendAcceptResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "acceptedUserId", friendAcceptResponse.acceptedUserId);
    }

    public static void Serialize(IBlackboard bb, FriendAddByCodeRequest friendAddByCodeRequest)
    {
        if (friendAddByCodeRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", friendAddByCodeRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "targetFriendCode", friendAddByCodeRequest.targetFriendCode);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", friendAddByCodeRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "type", friendAddByCodeRequest.type);
    }

    public static void Serialize(IBlackboard bb, FriendAddByCodeResponse friendAddByCodeResponse)
    {
        if (friendAddByCodeResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", friendAddByCodeResponse.error);
        if (friendAddByCodeResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), friendAddByCodeResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", friendAddByCodeResponse.serverTime);
        if (friendAddByCodeResponse.friendInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "friendInfo"), friendAddByCodeResponse.friendInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "friendInfo");
        }
    }

    public static void Serialize(IBlackboard bb, FriendAddEncourageListRequest friendAddEncourageListRequest)
    {
        if (friendAddEncourageListRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", friendAddEncourageListRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "targetUserIdList", friendAddEncourageListRequest.targetUserIdList);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", friendAddEncourageListRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "type", friendAddEncourageListRequest.type);
    }

    public static void Serialize(IBlackboard bb, FriendAddEncourageListResponse friendAddEncourageListResponse)
    {
        if (friendAddEncourageListResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", friendAddEncourageListResponse.error);
        if (friendAddEncourageListResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), friendAddEncourageListResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", friendAddEncourageListResponse.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "friendInfoList", friendAddEncourageListResponse.friendInfoList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "earnCredit", friendAddEncourageListResponse.earnCredit);
        if (friendAddEncourageListResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), friendAddEncourageListResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
    }

    public static void Serialize(IBlackboard bb, FriendAddListRequest friendAddListRequest)
    {
        if (friendAddListRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", friendAddListRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", friendAddListRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "targetUserIdList", friendAddListRequest.targetUserIdList);
        BlackboardUtils.SetOrCreateValue(bb, "type", friendAddListRequest.type);
    }

    public static void Serialize(IBlackboard bb, FriendAddListResponse friendAddListResponse)
    {
        if (friendAddListResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", friendAddListResponse.error);
        if (friendAddListResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), friendAddListResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", friendAddListResponse.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "friendInfoList", friendAddListResponse.friendInfoList, Serialize);
    }

    public static void Serialize(IBlackboard bb, FriendAddRequest friendAddRequest)
    {
        if (friendAddRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", friendAddRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "targetUserId", friendAddRequest.targetUserId);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", friendAddRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "type", friendAddRequest.type);
    }

    public static void Serialize(IBlackboard bb, FriendAddResponse friendAddResponse)
    {
        if (friendAddResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", friendAddResponse.error);
        if (friendAddResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), friendAddResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", friendAddResponse.serverTime);
        if (friendAddResponse.friendInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "friendInfo"), friendAddResponse.friendInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "friendInfo");
        }
    }

    public static void Serialize(IBlackboard bb, FriendGiftCollectResponse friendGiftCollectResponse)
    {
        if (friendGiftCollectResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", friendGiftCollectResponse.error);
        if (friendGiftCollectResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), friendGiftCollectResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", friendGiftCollectResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "earnCredit", friendGiftCollectResponse.earnCredit);
        if (friendGiftCollectResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), friendGiftCollectResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
    }

    public static void Serialize(IBlackboard bb, FriendGiftSendResponse friendGiftSendResponse)
    {
        if (friendGiftSendResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", friendGiftSendResponse.error);
        if (friendGiftSendResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), friendGiftSendResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", friendGiftSendResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "lastSendGiftTimestamp", friendGiftSendResponse.lastSendGiftTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "receiverUserIdList", friendGiftSendResponse.receiverUserIdList);
    }

    public static void Serialize(IBlackboard bb, FriendInfo friendInfo)
    {
        if (friendInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "userId", friendInfo.userId);
        BlackboardUtils.SetOrCreateValue(bb, "name", friendInfo.name);
        BlackboardUtils.SetOrCreateValue(bb, "profileUrl", friendInfo.profileUrl);
        BlackboardUtils.SetOrCreateValue(bb, "tier", friendInfo.tier);
        BlackboardUtils.SetOrCreateValue(bb, "reportCount", friendInfo.reportCount);
        BlackboardUtils.SetOrCreateValue(bb, "lastLoginTimestamp", friendInfo.lastLoginTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "type", friendInfo.type);
        BlackboardUtils.SetOrCreateValue(bb, "receivedGiftCount", friendInfo.receivedGiftCount);
        BlackboardUtils.SetOrCreateValue(bb, "lastSendGiftTimestamp", friendInfo.lastSendGiftTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "accepted", friendInfo.accepted);
        BlackboardUtils.SetOrCreateValue(bb, "level", friendInfo.level);
        BlackboardUtils.SetOrCreateValue(bb, "lastOnlineTimestamp", friendInfo.lastOnlineTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "profileHighResolutionUrl", friendInfo.profileHighResolutionUrl);
        BlackboardUtils.SetOrCreateValue(bb, "clubId", friendInfo.clubId);
    }

    public static void Serialize(IBlackboard bb, FriendInviteResponse friendInviteResponse)
    {
        if (friendInviteResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", friendInviteResponse.error);
        if (friendInviteResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), friendInviteResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", friendInviteResponse.serverTime);
    }

    public static void Serialize(IBlackboard bb, FriendListResponse friendListResponse)
    {
        if (friendListResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", friendListResponse.error);
        if (friendListResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), friendListResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", friendListResponse.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "friendInfoList", friendListResponse.friendInfoList, Serialize);
    }

    public static void Serialize(IBlackboard bb, FriendOnlineListResponse friendOnlineListResponse)
    {
        if (friendOnlineListResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", friendOnlineListResponse.error);
        if (friendOnlineListResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), friendOnlineListResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", friendOnlineListResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "onlineFriendUserIdList", friendOnlineListResponse.onlineFriendUserIdList);
    }

    public static void Serialize(IBlackboard bb, FriendRejectRequest friendRejectRequest)
    {
        if (friendRejectRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", friendRejectRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "targetUserId", friendRejectRequest.targetUserId);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", friendRejectRequest.ackMask);
    }

    public static void Serialize(IBlackboard bb, FriendRemoveRequest friendRemoveRequest)
    {
        if (friendRemoveRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", friendRemoveRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "targetUserId", friendRemoveRequest.targetUserId);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", friendRemoveRequest.ackMask);
    }

    public static void Serialize(IBlackboard bb, GambleDealRequestV2 gambleDealRequestV2)
    {
        if (gambleDealRequestV2 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", gambleDealRequestV2.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", gambleDealRequestV2.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "contents", gambleDealRequestV2.contents);
    }

    public static void Serialize(IBlackboard bb, GambleDealResponseV2 gambleDealResponseV2)
    {
        if (gambleDealResponseV2 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", gambleDealResponseV2.error);
        if (gambleDealResponseV2.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), gambleDealResponseV2.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", gambleDealResponseV2.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "contents", gambleDealResponseV2.contents);
    }

    public static void Serialize(IBlackboard bb, GambleStartRequestV2 gambleStartRequestV2)
    {
        if (gambleStartRequestV2 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", gambleStartRequestV2.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", gambleStartRequestV2.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "contents", gambleStartRequestV2.contents);
    }

    public static void Serialize(IBlackboard bb, GambleStartResponseV2 gambleStartResponseV2)
    {
        if (gambleStartResponseV2 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", gambleStartResponseV2.error);
        if (gambleStartResponseV2.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), gambleStartResponseV2.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", gambleStartResponseV2.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "contents", gambleStartResponseV2.contents);
    }

    public static void Serialize(IBlackboard bb, GambleTakeRequest gambleTakeRequest)
    {
        if (gambleTakeRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", gambleTakeRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", gambleTakeRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "contents", gambleTakeRequest.contents);
    }

    public static void Serialize(IBlackboard bb, GambleTakeResponseV2 gambleTakeResponseV2)
    {
        if (gambleTakeResponseV2 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", gambleTakeResponseV2.error);
        if (gambleTakeResponseV2.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), gambleTakeResponseV2.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", gambleTakeResponseV2.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "contents", gambleTakeResponseV2.contents);
        if (gambleTakeResponseV2.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), gambleTakeResponseV2.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
    }

    public static void Serialize(IBlackboard bb, GameInfo gameInfo)
    {
        if (gameInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "enumId", gameInfo.enumId);
        BlackboardUtils.SetOrCreateValue(bb, "gameTitle", gameInfo.gameTitle);
        BlackboardUtils.SetOrCreateValue(bb, "gameId", gameInfo.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "minBet", gameInfo.minBet);
        BlackboardUtils.SetOrCreateValue(bb, "maxBet", gameInfo.maxBet);
        BlackboardUtils.SetOrCreateValue(bb, "minLevel", gameInfo.minLevel);
        BlackboardUtils.SetOrCreateValue(bb, "useCustomScene", gameInfo.useCustomScene);
        BlackboardUtils.SetOrCreateValue(bb, "gameType", gameInfo.gameType);
        BlackboardUtils.SetOrCreateValue(bb, "unlockStatus", gameInfo.unlockStatus);
        BlackboardUtils.SetOrCreateValue(bb, "orientation", gameInfo.orientation);
        BlackboardUtils.SetOrCreateValue(bb, "shortImageUrl", gameInfo.shortImageUrl);
        BlackboardUtils.SetOrCreateValue(bb, "longImageUrl", gameInfo.longImageUrl);
        BlackboardUtils.SetOrCreateValue(bb, "minClientVersion", gameInfo.minClientVersion);
        BlackboardUtils.SetOrCreateValue(bb, "gameFilter", gameInfo.gameFilter);
        BlackboardUtils.SetOrCreateValue(bb, "gameOrder", gameInfo.gameOrder);
        BlackboardUtils.SetOrCreateValue(bb, "isLong", gameInfo.isLong);
    }

    public static void Serialize(IBlackboard bb, GamePlayRestriction gamePlayRestriction)
    {
        if (gamePlayRestriction == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "minBet", gamePlayRestriction.minBet);
        BlackboardUtils.SetOrCreateValue(bb, "minLevel", gamePlayRestriction.minLevel);
    }

    public static void Serialize(IBlackboard bb, GameSpinBonus gameSpinBonus)
    {
        if (gameSpinBonus == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "type", gameSpinBonus.type);
    }

    public static void Serialize(IBlackboard bb, GameSpinCollectCoinResponse gameSpinCollectCoinResponse)
    {
        if (gameSpinCollectCoinResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", gameSpinCollectCoinResponse.error);
        if (gameSpinCollectCoinResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), gameSpinCollectCoinResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", gameSpinCollectCoinResponse.serverTime);
        if (gameSpinCollectCoinResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), gameSpinCollectCoinResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
    }

    public static void Serialize(IBlackboard bb, GameSpinCollectFreeResponse gameSpinCollectFreeResponse)
    {
        if (gameSpinCollectFreeResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", gameSpinCollectFreeResponse.error);
        if (gameSpinCollectFreeResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), gameSpinCollectFreeResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", gameSpinCollectFreeResponse.serverTime);
        if (gameSpinCollectFreeResponse.gameSpinResult != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "gameSpinResult"), gameSpinCollectFreeResponse.gameSpinResult);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "gameSpinResult");
        }
        BlackboardUtils.SetOrCreateValue(bb, "lastCollectTimestamp", gameSpinCollectFreeResponse.lastCollectTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "timebonusConsecutive", gameSpinCollectFreeResponse.timebonusConsecutive);
        BlackboardUtils.SetOrCreateValue(bb, "timeBonusCooltime", gameSpinCollectFreeResponse.timeBonusCooltime);
        BlackboardUtils.SetOrCreateValue(bb, "collectCreditEnabled", gameSpinCollectFreeResponse.collectCreditEnabled);
        BlackboardUtils.SetOrCreateValue(bb, "earnLoungePoint", gameSpinCollectFreeResponse.earnLoungePoint);
        if (gameSpinCollectFreeResponse.vipLoungeInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "vipLoungeInfo"), gameSpinCollectFreeResponse.vipLoungeInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "vipLoungeInfo");
        }
    }

    public static void Serialize(IBlackboard bb, GameSpinConfigure gameSpinConfigure)
    {
        if (gameSpinConfigure == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "consecutiveMax", gameSpinConfigure.consecutiveMax);
        BlackboardUtils.SetOrCreateValue(bb, "betLevelIndex", gameSpinConfigure.betLevelIndex);
        BlackboardUtils.SetOrCreateValue(bb, "bonusReelUnlockSecAfterPurchase", gameSpinConfigure.bonusReelUnlockSecAfterPurchase);
        BlackboardUtils.SetOrCreateValue(bb, "highlightSpinCountThreshold", gameSpinConfigure.highlightSpinCountThreshold);
    }

    public static void Serialize(IBlackboard bb, GameSpinInfoForLobby gameSpinInfoForLobby)
    {
        if (gameSpinInfoForLobby == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", gameSpinInfoForLobby.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "totalSpinCountOfGame", gameSpinInfoForLobby.totalSpinCountOfGame);
    }

    public static void Serialize(IBlackboard bb, GameSpinReelSet gameSpinReelSet)
    {
        if (gameSpinReelSet == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameIdReel", gameSpinReelSet.gameIdReel);
        BlackboardUtils.SetOrCreateValue(bb, "spinCountReel", gameSpinReelSet.spinCountReel);
        BlackboardUtils.SetOrCreateValue(bb, "betScaleReel", gameSpinReelSet.betScaleReel);
    }

    public static void Serialize(IBlackboard bb, GameSpinResult gameSpinResult)
    {
        if (gameSpinResult == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameIdReelIndex", gameSpinResult.gameIdReelIndex);
        BlackboardUtils.SetOrCreateValue(bb, "spinCountReelIndex", gameSpinResult.spinCountReelIndex);
        BlackboardUtils.SetOrCreateValue(bb, "gameId", gameSpinResult.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "bonusReelIndex", gameSpinResult.bonusReelIndex);
        BlackboardUtils.SetOrCreateValue(bb, "bonusCredit", gameSpinResult.bonusCredit);
        BlackboardUtils.SetOrCreateValue(bb, "bonusSpinAgain", gameSpinResult.bonusSpinAgain);
        BlackboardUtils.SetOrCreateValue(bb, "congratulationEffect", gameSpinResult.congratulationEffect);
        BlackboardUtils.SetOrCreateValue(bb, "bet", gameSpinResult.bet);
        BlackboardUtils.SetOrCreateValue(bb, "betScaleReelIndex", gameSpinResult.betScaleReelIndex);
        BlackboardUtils.SetOrCreateValue(bb, "addedSpinCount", gameSpinResult.addedSpinCount);
        BlackboardUtils.SetOrCreateValue(bb, "totalSpinCountOfBet", gameSpinResult.totalSpinCountOfBet);
        BlackboardUtils.SetOrCreateValue(bb, "betScale", gameSpinResult.betScale);
        BlackboardUtils.SetOrCreateValue(bb, "collectCoinCredit", gameSpinResult.collectCoinCredit);
    }

    public static void Serialize(IBlackboard bb, GameSpinSetting gameSpinSetting)
    {
        if (gameSpinSetting == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "lastPurchaseTimestamp", gameSpinSetting.lastPurchaseTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "bonusReelUnlockSecAfterPurchase", gameSpinSetting.bonusReelUnlockSecAfterPurchase);
    }

    public static void Serialize(IBlackboard bb, GemBuySpeakerRequest gemBuySpeakerRequest)
    {
        if (gemBuySpeakerRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", gemBuySpeakerRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", gemBuySpeakerRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "speaker", gemBuySpeakerRequest.speaker);
        BlackboardUtils.SetOrCreateValue(bb, "contextId", gemBuySpeakerRequest.contextId);
    }

    public static void Serialize(IBlackboard bb, GemBuySpeakerResponse gemBuySpeakerResponse)
    {
        if (gemBuySpeakerResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", gemBuySpeakerResponse.error);
        if (gemBuySpeakerResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), gemBuySpeakerResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", gemBuySpeakerResponse.serverTime);
        if (gemBuySpeakerResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), gemBuySpeakerResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
    }

    public static void Serialize(IBlackboard bb, GemJackpotCollectRewardResponse gemJackpotCollectRewardResponse)
    {
        if (gemJackpotCollectRewardResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", gemJackpotCollectRewardResponse.error);
        if (gemJackpotCollectRewardResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), gemJackpotCollectRewardResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", gemJackpotCollectRewardResponse.serverTime);
        if (gemJackpotCollectRewardResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), gemJackpotCollectRewardResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "grandJackpotMultiplyNumerator", gemJackpotCollectRewardResponse.grandJackpotMultiplyNumerator);
    }

    public static void Serialize(IBlackboard bb, GemJackpotDebugSpinRequest gemJackpotDebugSpinRequest)
    {
        if (gemJackpotDebugSpinRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", gemJackpotDebugSpinRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", gemJackpotDebugSpinRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "debugSpinIndex", gemJackpotDebugSpinRequest.debugSpinIndex);
        BlackboardUtils.SetOrCreateValue(bb, "slotEnterContextId", gemJackpotDebugSpinRequest.slotEnterContextId);
    }

    public static void Serialize(IBlackboard bb, GemJackpotEnterRequest gemJackpotEnterRequest)
    {
        if (gemJackpotEnterRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", gemJackpotEnterRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", gemJackpotEnterRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "eventId", gemJackpotEnterRequest.eventId);
    }

    public static void Serialize(IBlackboard bb, GemJackpotEnterResponse gemJackpotEnterResponse)
    {
        if (gemJackpotEnterResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", gemJackpotEnterResponse.error);
        if (gemJackpotEnterResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), gemJackpotEnterResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", gemJackpotEnterResponse.serverTime);
        if (gemJackpotEnterResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), gemJackpotEnterResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "gemForSpin", gemJackpotEnterResponse.gemForSpin);
        BlackboardUtils.SetOrCreateValue(bb, "slotReelRewardList", gemJackpotEnterResponse.slotReelRewardList);
        BlackboardUtils.SetOrCreateList(bb, "jackpotReelRewardList", gemJackpotEnterResponse.jackpotReelRewardList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "jackpotList", gemJackpotEnterResponse.jackpotList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "grandJackpotMultiplyNumerator", gemJackpotEnterResponse.grandJackpotMultiplyNumerator);
        BlackboardUtils.SetOrCreateValue(bb, "freeSpinCount", gemJackpotEnterResponse.freeSpinCount);
        BlackboardUtils.SetOrCreateValue(bb, "initialSlotReelSetIndexList", gemJackpotEnterResponse.initialSlotReelSetIndexList);
        BlackboardUtils.SetOrCreateValue(bb, "jackpotReelSetList", gemJackpotEnterResponse.jackpotReelSetList);
        BlackboardUtils.SetOrCreateValue(bb, "enabledGemJackpotGemDisplay", gemJackpotEnterResponse.enabledGemJackpotGemDisplay);
        BlackboardUtils.SetOrCreateValue(bb, "slotReelBalance", gemJackpotEnterResponse.slotReelBalance);
        BlackboardUtils.SetOrCreateValue(bb, "favoritePurchasePrice", gemJackpotEnterResponse.favoritePurchasePrice);
    }

    public static void Serialize(IBlackboard bb, GemJackpotInfo gemJackpotInfo)
    {
        if (gemJackpotInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "lastVideoAdsClaimTimestamp", gemJackpotInfo.lastVideoAdsClaimTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "gemJackpotCooltime", gemJackpotInfo.gemJackpotCooltime);
        BlackboardUtils.SetOrCreateList(bb, "jackpotList", gemJackpotInfo.jackpotList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "grandJackpotMultiplyNumerator", gemJackpotInfo.grandJackpotMultiplyNumerator);
    }

    public static void Serialize(IBlackboard bb, GemJackpotJackpotInfo gemJackpotJackpotInfo)
    {
        if (gemJackpotJackpotInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "index", gemJackpotJackpotInfo.index);
        BlackboardUtils.SetOrCreateValue(bb, "credit", gemJackpotJackpotInfo.credit);
    }

    public static void Serialize(IBlackboard bb, GemJackpotJackpotRewardInfo gemJackpotJackpotRewardInfo)
    {
        if (gemJackpotJackpotRewardInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "type", gemJackpotJackpotRewardInfo.type);
        BlackboardUtils.SetOrCreateValue(bb, "credit", gemJackpotJackpotRewardInfo.credit);
    }

    public static void Serialize(IBlackboard bb, GemJackpotSpinRequest gemJackpotSpinRequest)
    {
        if (gemJackpotSpinRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", gemJackpotSpinRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", gemJackpotSpinRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "slotEnterContextId", gemJackpotSpinRequest.slotEnterContextId);
    }

    public static void Serialize(IBlackboard bb, GemJackpotSpinResponse gemJackpotSpinResponse)
    {
        if (gemJackpotSpinResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", gemJackpotSpinResponse.error);
        if (gemJackpotSpinResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), gemJackpotSpinResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", gemJackpotSpinResponse.serverTime);
        if (gemJackpotSpinResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), gemJackpotSpinResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "nextProgress", gemJackpotSpinResponse.nextProgress);
        BlackboardUtils.SetOrCreateValue(bb, "slotReelSetResultIndexList", gemJackpotSpinResponse.slotReelSetResultIndexList);
        if (gemJackpotSpinResponse.jackpotInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "jackpotInfo"), gemJackpotSpinResponse.jackpotInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "jackpotInfo");
        }
        BlackboardUtils.SetOrCreateList(bb, "jackpotList", gemJackpotSpinResponse.jackpotList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "grandJackpotMultiplyNumerator", gemJackpotSpinResponse.grandJackpotMultiplyNumerator);
        BlackboardUtils.SetOrCreateValue(bb, "grandJackpotRewardAmount", gemJackpotSpinResponse.grandJackpotRewardAmount);
    }

    public static void Serialize(IBlackboard bb, GemJackpotVideoAdsClaimRequest gemJackpotVideoAdsClaimRequest)
    {
        if (gemJackpotVideoAdsClaimRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", gemJackpotVideoAdsClaimRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", gemJackpotVideoAdsClaimRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "eventId", gemJackpotVideoAdsClaimRequest.eventId);
    }

    public static void Serialize(IBlackboard bb, GemJackpotVideoAdsClaimResponse gemJackpotVideoAdsClaimResponse)
    {
        if (gemJackpotVideoAdsClaimResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", gemJackpotVideoAdsClaimResponse.error);
        if (gemJackpotVideoAdsClaimResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), gemJackpotVideoAdsClaimResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", gemJackpotVideoAdsClaimResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "gemJackpotCooltime", gemJackpotVideoAdsClaimResponse.gemJackpotCooltime);
        BlackboardUtils.SetOrCreateValue(bb, "lastVideoAdsClaimTimestamp", gemJackpotVideoAdsClaimResponse.lastVideoAdsClaimTimestamp);
    }

    public static void Serialize(IBlackboard bb, GemUnlockGameRequest gemUnlockGameRequest)
    {
        if (gemUnlockGameRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", gemUnlockGameRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "gameId", gemUnlockGameRequest.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", gemUnlockGameRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "contextId", gemUnlockGameRequest.contextId);
    }

    public static void Serialize(IBlackboard bb, GemUnlockGameResponse gemUnlockGameResponse)
    {
        if (gemUnlockGameResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", gemUnlockGameResponse.error);
        if (gemUnlockGameResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), gemUnlockGameResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", gemUnlockGameResponse.serverTime);
        if (gemUnlockGameResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), gemUnlockGameResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
        if (gemUnlockGameResponse.unlockedGameInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "unlockedGameInfo"), gemUnlockGameResponse.unlockedGameInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "unlockedGameInfo");
        }
        if (gemUnlockGameResponse.unlockedSlot != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "unlockedSlot"), gemUnlockGameResponse.unlockedSlot);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "unlockedSlot");
        }
    }

    public static void Serialize(IBlackboard bb, GemUseBonusRequest gemUseBonusRequest)
    {
        if (gemUseBonusRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", gemUseBonusRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", gemUseBonusRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "gameId", gemUseBonusRequest.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "ticketedBonusId", gemUseBonusRequest.ticketedBonusId);
        BlackboardUtils.SetOrCreateValue(bb, "bet", gemUseBonusRequest.bet);
        BlackboardUtils.SetOrCreateValue(bb, "extraBet", gemUseBonusRequest.extraBet);
        BlackboardUtils.SetOrCreateValue(bb, "bonusSaleEventId", gemUseBonusRequest.bonusSaleEventId);
        BlackboardUtils.SetOrCreateValue(bb, "bonusMultiplyEventId", gemUseBonusRequest.bonusMultiplyEventId);
        BlackboardUtils.SetOrCreateValue(bb, "iamId", gemUseBonusRequest.iamId);
        BlackboardUtils.SetOrCreateValue(bb, "iamTriggerType", gemUseBonusRequest.iamTriggerType);
        BlackboardUtils.SetOrCreateValue(bb, "contextId", gemUseBonusRequest.contextId);
    }

    public static void Serialize(IBlackboard bb, GemUseBonusResponse gemUseBonusResponse)
    {
        if (gemUseBonusResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", gemUseBonusResponse.error);
        if (gemUseBonusResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), gemUseBonusResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", gemUseBonusResponse.serverTime);
        if (gemUseBonusResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), gemUseBonusResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", gemUseBonusResponse.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "ticketId", gemUseBonusResponse.ticketId);
    }

    public static void Serialize(IBlackboard bb, GlobalChatInfo globalChatInfo)
    {
        if (globalChatInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "channelId", globalChatInfo.channelId);
        BlackboardUtils.SetOrCreateValue(bb, "channelName", globalChatInfo.channelName);
    }

    public static void Serialize(IBlackboard bb, GlobalChatPostMessageRequestV1 globalChatPostMessageRequestV1)
    {
        if (globalChatPostMessageRequestV1 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", globalChatPostMessageRequestV1.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", globalChatPostMessageRequestV1.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "channelId", globalChatPostMessageRequestV1.channelId);
        BlackboardUtils.SetOrCreateValue(bb, "type", globalChatPostMessageRequestV1.type);
        if (globalChatPostMessageRequestV1.data != null)
        {
            switch (globalChatPostMessageRequestV1.type)
            {
            case ChatType.MESSAGE:
                Serialize(bb, (ChatDataMessage)globalChatPostMessageRequestV1.data);
                break;

            case ChatType.INSTANT:
                Serialize(bb, (ChatDataInstant)globalChatPostMessageRequestV1.data);
                break;

            case ChatType.EMOJI:
                Serialize(bb, (ChatDataEmoji)globalChatPostMessageRequestV1.data);
                break;

            case ChatType.CLUB_PR:
                Serialize(bb, (ChatDataClubPr)globalChatPostMessageRequestV1.data);
                break;

            case ChatType.BOSS_RAIDERS_BOSS_KILL:
                Serialize(bb, (ChatDataBossRaidersBossKill)globalChatPostMessageRequestV1.data);
                break;

            case ChatType.BOSS_RAIDERS_RANKING_UP:
                Serialize(bb, (ChatDataBossRaidersRankingUp)globalChatPostMessageRequestV1.data);
                break;

            case ChatType.BOSS_RAIDERS_LEADING_ATTACKER:
                Serialize(bb, (ChatDataBossRaidersLeadingAttacker)globalChatPostMessageRequestV1.data);
                break;

            case ChatType.CLUB_ARENA_CLUB_RANKING_UP:
                Serialize(bb, (ChatDataClubArenaClubRankingUp)globalChatPostMessageRequestV1.data);
                break;

            case ChatType.CLUB_ARENA_LEADING_CLUB_MEMBER:
                Serialize(bb, (ChatDataClubArenaLeadingClubMember)globalChatPostMessageRequestV1.data);
                break;
            }
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "data");
        }
        BlackboardUtils.SetOrCreateValue(bb, "contextId", globalChatPostMessageRequestV1.contextId);
        BlackboardUtils.SetOrCreateValue(bb, "requestId", globalChatPostMessageRequestV1.requestId);
    }

    public static void Serialize(IBlackboard bb, GlobalChatPostMessageResponse globalChatPostMessageResponse)
    {
        if (globalChatPostMessageResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", globalChatPostMessageResponse.error);
        if (globalChatPostMessageResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), globalChatPostMessageResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", globalChatPostMessageResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "messageId", globalChatPostMessageResponse.messageId);
        BlackboardUtils.SetOrCreateValue(bb, "speaker", globalChatPostMessageResponse.speaker);
        BlackboardUtils.SetOrCreateValue(bb, "lastSpeakerRefillTimestamp", globalChatPostMessageResponse.lastSpeakerRefillTimestamp);
    }

    public static void Serialize(IBlackboard bb, GurusBuilding gurusBuilding)
    {
        if (gurusBuilding == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "level", gurusBuilding.level);
        BlackboardUtils.SetOrCreateValue(bb, "exp", gurusBuilding.exp);
    }

    public static void Serialize(IBlackboard bb, GurusFinalReward gurusFinalReward)
    {
        if (gurusFinalReward == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "seasonThemeId", gurusFinalReward.seasonThemeId);
        BlackboardUtils.SetOrCreateValue(bb, "seasonName", gurusFinalReward.seasonName);
        BlackboardUtils.SetOrCreateValue(bb, "rank", gurusFinalReward.rank);
        BlackboardUtils.SetOrCreateValue(bb, "level", gurusFinalReward.level);
        BlackboardUtils.SetOrCreateValue(bb, "rewardGem", gurusFinalReward.rewardGem);
    }

    public static void Serialize(IBlackboard bb, GurusFinalRewardPreset gurusFinalRewardPreset)
    {
        if (gurusFinalRewardPreset == null) { return; }
        BlackboardUtils.SetOrCreateList(bb, "rewardByRankList", gurusFinalRewardPreset.rewardByRankList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "rewardByPercentileList", gurusFinalRewardPreset.rewardByPercentileList, Serialize);
    }

    public static void Serialize(IBlackboard bb, GurusRankListItem gurusRankListItem)
    {
        if (gurusRankListItem == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "rank", gurusRankListItem.rank);
        BlackboardUtils.SetOrCreateValue(bb, "level", gurusRankListItem.level);
        if (gurusRankListItem.profile != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "profile"), gurusRankListItem.profile);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "profile");
        }
    }

    public static void Serialize(IBlackboard bb, GurusRankPercentile gurusRankPercentile)
    {
        if (gurusRankPercentile == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "rank", gurusRankPercentile.rank);
        BlackboardUtils.SetOrCreateValue(bb, "percentile", gurusRankPercentile.percentile);
    }

    public static void Serialize(IBlackboard bb, GurusRewardByPercentile gurusRewardByPercentile)
    {
        if (gurusRewardByPercentile == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "percentile", gurusRewardByPercentile.percentile);
        BlackboardUtils.SetOrCreateValue(bb, "gem", gurusRewardByPercentile.gem);
    }

    public static void Serialize(IBlackboard bb, GurusRewardByRank gurusRewardByRank)
    {
        if (gurusRewardByRank == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "rank", gurusRewardByRank.rank);
        BlackboardUtils.SetOrCreateValue(bb, "gem", gurusRewardByRank.gem);
    }

    public static void Serialize(IBlackboard bb, GurusSeasonPreset gurusSeasonPreset)
    {
        if (gurusSeasonPreset == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "name", gurusSeasonPreset.name);
    }

    public static void Serialize(IBlackboard bb, HiddenUniverseChapterInfo hiddenUniverseChapterInfo)
    {
        if (hiddenUniverseChapterInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "chapter", hiddenUniverseChapterInfo.chapter);
        BlackboardUtils.SetOrCreateValue(bb, "unlockLevel", hiddenUniverseChapterInfo.unlockLevel);
        BlackboardUtils.SetOrCreateValue(bb, "isRewarded", hiddenUniverseChapterInfo.isRewarded);
        BlackboardUtils.SetOrCreateValue(bb, "completeRewardCoin", hiddenUniverseChapterInfo.completeRewardCoin);
        BlackboardUtils.SetOrCreateValue(bb, "completeRewardGem", hiddenUniverseChapterInfo.completeRewardGem);
    }

    public static void Serialize(IBlackboard bb, HiddenUniverseChapterUnlock hiddenUniverseChapterUnlock)
    {
        if (hiddenUniverseChapterUnlock == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "chapter", hiddenUniverseChapterUnlock.chapter);
        BlackboardUtils.SetOrCreateValue(bb, "unlock", hiddenUniverseChapterUnlock.unlock);
    }

    public static void Serialize(IBlackboard bb, HiddenUniverseChapterUnlockLevel hiddenUniverseChapterUnlockLevel)
    {
        if (hiddenUniverseChapterUnlockLevel == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "chapter", hiddenUniverseChapterUnlockLevel.chapter);
        BlackboardUtils.SetOrCreateValue(bb, "level", hiddenUniverseChapterUnlockLevel.level);
    }

    public static void Serialize(IBlackboard bb, HiddenUniverseClaimChapterRewardRequest hiddenUniverseClaimChapterRewardRequest)
    {
        if (hiddenUniverseClaimChapterRewardRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", hiddenUniverseClaimChapterRewardRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", hiddenUniverseClaimChapterRewardRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "chapter", hiddenUniverseClaimChapterRewardRequest.chapter);
    }

    public static void Serialize(IBlackboard bb, HiddenUniverseClaimChapterRewardResponse hiddenUniverseClaimChapterRewardResponse)
    {
        if (hiddenUniverseClaimChapterRewardResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", hiddenUniverseClaimChapterRewardResponse.error);
        if (hiddenUniverseClaimChapterRewardResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), hiddenUniverseClaimChapterRewardResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", hiddenUniverseClaimChapterRewardResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "earnCredit", hiddenUniverseClaimChapterRewardResponse.earnCredit);
        BlackboardUtils.SetOrCreateValue(bb, "earnGem", hiddenUniverseClaimChapterRewardResponse.earnGem);
        if (hiddenUniverseClaimChapterRewardResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), hiddenUniverseClaimChapterRewardResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
        BlackboardUtils.SetOrCreateList(bb, "rewardedChapterInfoList", hiddenUniverseClaimChapterRewardResponse.rewardedChapterInfoList, Serialize);
    }

    public static void Serialize(IBlackboard bb, HiddenUniverseClearContent hiddenUniverseClearContent)
    {
        if (hiddenUniverseClearContent == null) { return; }
    }

    public static void Serialize(IBlackboard bb, HiddenUniverseDebugPlayClearRequest hiddenUniverseDebugPlayClearRequest)
    {
        if (hiddenUniverseDebugPlayClearRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", hiddenUniverseDebugPlayClearRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", hiddenUniverseDebugPlayClearRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "chapter", hiddenUniverseDebugPlayClearRequest.chapter);
        BlackboardUtils.SetOrCreateValue(bb, "stage", hiddenUniverseDebugPlayClearRequest.stage);
        BlackboardUtils.SetOrCreateValue(bb, "token", hiddenUniverseDebugPlayClearRequest.token);
        BlackboardUtils.SetOrCreateList(bb, "playLog", hiddenUniverseDebugPlayClearRequest.playLog, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "allObject", hiddenUniverseDebugPlayClearRequest.allObject);
        BlackboardUtils.SetOrCreateValue(bb, "ignoreLock", hiddenUniverseDebugPlayClearRequest.ignoreLock);
        BlackboardUtils.SetOrCreateValue(bb, "pauseCount", hiddenUniverseDebugPlayClearRequest.pauseCount);
    }

    public static void Serialize(IBlackboard bb, HiddenUniverseDebugPlayEnterRequest hiddenUniverseDebugPlayEnterRequest)
    {
        if (hiddenUniverseDebugPlayEnterRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", hiddenUniverseDebugPlayEnterRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", hiddenUniverseDebugPlayEnterRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "chapter", hiddenUniverseDebugPlayEnterRequest.chapter);
        BlackboardUtils.SetOrCreateValue(bb, "stage", hiddenUniverseDebugPlayEnterRequest.stage);
        BlackboardUtils.SetOrCreateValue(bb, "allObject", hiddenUniverseDebugPlayEnterRequest.allObject);
        BlackboardUtils.SetOrCreateValue(bb, "ignoreLock", hiddenUniverseDebugPlayEnterRequest.ignoreLock);
        BlackboardUtils.SetOrCreateValue(bb, "type", hiddenUniverseDebugPlayEnterRequest.type);
    }

    public static void Serialize(IBlackboard bb, HiddenUniverseEnterResponse hiddenUniverseEnterResponse)
    {
        if (hiddenUniverseEnterResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", hiddenUniverseEnterResponse.error);
        if (hiddenUniverseEnterResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), hiddenUniverseEnterResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", hiddenUniverseEnterResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "maxFinder", hiddenUniverseEnterResponse.maxFinder);
        BlackboardUtils.SetOrCreateValue(bb, "finder", hiddenUniverseEnterResponse.finder);
        BlackboardUtils.SetOrCreateValue(bb, "giftedFinder", hiddenUniverseEnterResponse.giftedFinder);
        BlackboardUtils.SetOrCreateValue(bb, "lastFinderRequestTimestamp", hiddenUniverseEnterResponse.lastFinderRequestTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "finderRequestCooltime", hiddenUniverseEnterResponse.finderRequestCooltime);
        BlackboardUtils.SetOrCreateValue(bb, "lastVideoAdsClaimTimestamp", hiddenUniverseEnterResponse.lastVideoAdsClaimTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "videoAdsCooltime", hiddenUniverseEnterResponse.videoAdsCooltime);
        BlackboardUtils.SetOrCreateValue(bb, "videoAdsFinderCount", hiddenUniverseEnterResponse.videoAdsFinderCount);
        BlackboardUtils.SetOrCreateValue(bb, "neededStarForStageUnlock", hiddenUniverseEnterResponse.neededStarForStageUnlock);
        BlackboardUtils.SetOrCreateValue(bb, "neededStarForChapterUnlock", hiddenUniverseEnterResponse.neededStarForChapterUnlock);
        BlackboardUtils.SetOrCreateList(bb, "chapterUnlockList", hiddenUniverseEnterResponse.chapterUnlockList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "stageUnlockList", hiddenUniverseEnterResponse.stageUnlockList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "stageInfoList", hiddenUniverseEnterResponse.stageInfoList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "hiddenUniverseShopActive", hiddenUniverseEnterResponse.hiddenUniverseShopActive);
        BlackboardUtils.SetOrCreateValue(bb, "hiddenUniverseBundleShopActive", hiddenUniverseEnterResponse.hiddenUniverseBundleShopActive);
        BlackboardUtils.SetOrCreateValue(bb, "hiddenUniverseSymbolList", hiddenUniverseEnterResponse.hiddenUniverseSymbolList);
        BlackboardUtils.SetOrCreateList(bb, "chapterInfoList", hiddenUniverseEnterResponse.chapterInfoList, Serialize);
    }

    public static void Serialize(IBlackboard bb, HiddenUniverseFindContent hiddenUniverseFindContent)
    {
        if (hiddenUniverseFindContent == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "objectIndex", hiddenUniverseFindContent.objectIndex);
        BlackboardUtils.SetOrCreateValue(bb, "score", hiddenUniverseFindContent.score);
    }

    public static void Serialize(IBlackboard bb, HiddenUniverseGiftFinderRequest hiddenUniverseGiftFinderRequest)
    {
        if (hiddenUniverseGiftFinderRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", hiddenUniverseGiftFinderRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", hiddenUniverseGiftFinderRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "clubId", hiddenUniverseGiftFinderRequest.clubId);
        BlackboardUtils.SetOrCreateValue(bb, "clubFeedId", hiddenUniverseGiftFinderRequest.clubFeedId);
    }

    public static void Serialize(IBlackboard bb, HiddenUniverseGiftFinderResponse hiddenUniverseGiftFinderResponse)
    {
        if (hiddenUniverseGiftFinderResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", hiddenUniverseGiftFinderResponse.error);
        if (hiddenUniverseGiftFinderResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), hiddenUniverseGiftFinderResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", hiddenUniverseGiftFinderResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "finder", hiddenUniverseGiftFinderResponse.finder);
        if (hiddenUniverseGiftFinderResponse.clubFeed != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "clubFeed"), hiddenUniverseGiftFinderResponse.clubFeed);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "clubFeed");
        }
    }

    public static void Serialize(IBlackboard bb, HiddenUniverseHintContent hiddenUniverseHintContent)
    {
        if (hiddenUniverseHintContent == null) { return; }
    }

    public static void Serialize(IBlackboard bb, HiddenUniverseInfo hiddenUniverseInfo)
    {
        if (hiddenUniverseInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "active", hiddenUniverseInfo.active);
        BlackboardUtils.SetOrCreateValue(bb, "activeForUser", hiddenUniverseInfo.activeForUser);
        BlackboardUtils.SetOrCreateValue(bb, "maxFinder", hiddenUniverseInfo.maxFinder);
        BlackboardUtils.SetOrCreateValue(bb, "finder", hiddenUniverseInfo.finder);
        BlackboardUtils.SetOrCreateValue(bb, "requiredFinderForLastStage", hiddenUniverseInfo.requiredFinderForLastStage);
    }

    public static void Serialize(IBlackboard bb, HiddenUniverseLeaderboard hiddenUniverseLeaderboard)
    {
        if (hiddenUniverseLeaderboard == null) { return; }
        if (hiddenUniverseLeaderboard.prevMyRank != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "prevMyRank"), hiddenUniverseLeaderboard.prevMyRank);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "prevMyRank");
        }
        if (hiddenUniverseLeaderboard.nowMyRank != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "nowMyRank"), hiddenUniverseLeaderboard.nowMyRank);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "nowMyRank");
        }
        if (hiddenUniverseLeaderboard.firstRank != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "firstRank"), hiddenUniverseLeaderboard.firstRank);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "firstRank");
        }
        BlackboardUtils.SetOrCreateList(bb, "shownRankList", hiddenUniverseLeaderboard.shownRankList, Serialize);
    }

    public static void Serialize(IBlackboard bb, HiddenUniversePlayClearRequest hiddenUniversePlayClearRequest)
    {
        if (hiddenUniversePlayClearRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", hiddenUniversePlayClearRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", hiddenUniversePlayClearRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "chapter", hiddenUniversePlayClearRequest.chapter);
        BlackboardUtils.SetOrCreateValue(bb, "stage", hiddenUniversePlayClearRequest.stage);
        BlackboardUtils.SetOrCreateValue(bb, "token", hiddenUniversePlayClearRequest.token);
        BlackboardUtils.SetOrCreateList(bb, "playLog", hiddenUniversePlayClearRequest.playLog, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "pauseCount", hiddenUniversePlayClearRequest.pauseCount);
    }

    public static void Serialize(IBlackboard bb, HiddenUniversePlayClearResponse hiddenUniversePlayClearResponse)
    {
        if (hiddenUniversePlayClearResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", hiddenUniversePlayClearResponse.error);
        if (hiddenUniversePlayClearResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), hiddenUniversePlayClearResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", hiddenUniversePlayClearResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "maxFinder", hiddenUniversePlayClearResponse.maxFinder);
        BlackboardUtils.SetOrCreateValue(bb, "finder", hiddenUniversePlayClearResponse.finder);
        BlackboardUtils.SetOrCreateValue(bb, "totalScore", hiddenUniversePlayClearResponse.totalScore);
        BlackboardUtils.SetOrCreateValue(bb, "baseScore", hiddenUniversePlayClearResponse.baseScore);
        BlackboardUtils.SetOrCreateValue(bb, "accuracyBonusScore", hiddenUniversePlayClearResponse.accuracyBonusScore);
        BlackboardUtils.SetOrCreateValue(bb, "timeBonusScore", hiddenUniversePlayClearResponse.timeBonusScore);
        BlackboardUtils.SetOrCreateValue(bb, "hintBonusScore", hiddenUniversePlayClearResponse.hintBonusScore);
        if (hiddenUniversePlayClearResponse.leaderboard != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "leaderboard"), hiddenUniversePlayClearResponse.leaderboard);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "leaderboard");
        }
        if (hiddenUniversePlayClearResponse.stageInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "stageInfo"), hiddenUniversePlayClearResponse.stageInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "stageInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "earnCredit", hiddenUniversePlayClearResponse.earnCredit);
        BlackboardUtils.SetOrCreateValue(bb, "earnGem", hiddenUniversePlayClearResponse.earnGem);
        BlackboardUtils.SetOrCreateValue(bb, "isHighScore", hiddenUniversePlayClearResponse.isHighScore);
        BlackboardUtils.SetOrCreateValue(bb, "isPerfect", hiddenUniversePlayClearResponse.isPerfect);
        if (hiddenUniversePlayClearResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), hiddenUniversePlayClearResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
    }

    public static void Serialize(IBlackboard bb, HiddenUniversePlayEnterRequest hiddenUniversePlayEnterRequest)
    {
        if (hiddenUniversePlayEnterRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", hiddenUniversePlayEnterRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", hiddenUniversePlayEnterRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "chapter", hiddenUniversePlayEnterRequest.chapter);
        BlackboardUtils.SetOrCreateValue(bb, "stage", hiddenUniversePlayEnterRequest.stage);
        BlackboardUtils.SetOrCreateValue(bb, "type", hiddenUniversePlayEnterRequest.type);
    }

    public static void Serialize(IBlackboard bb, HiddenUniversePlayEnterResponse hiddenUniversePlayEnterResponse)
    {
        if (hiddenUniversePlayEnterResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", hiddenUniversePlayEnterResponse.error);
        if (hiddenUniversePlayEnterResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), hiddenUniversePlayEnterResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", hiddenUniversePlayEnterResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "token", hiddenUniversePlayEnterResponse.token);
        BlackboardUtils.SetOrCreateValue(bb, "totalObjectCount", hiddenUniversePlayEnterResponse.totalObjectCount);
        BlackboardUtils.SetOrCreateList(bb, "objectToFindList", hiddenUniversePlayEnterResponse.objectToFindList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "wrongObjectIndexList", hiddenUniversePlayEnterResponse.wrongObjectIndexList);
        BlackboardUtils.SetOrCreateValue(bb, "comboDuration", hiddenUniversePlayEnterResponse.comboDuration);
        BlackboardUtils.SetOrCreateValue(bb, "maxCombo", hiddenUniversePlayEnterResponse.maxCombo);
        BlackboardUtils.SetOrCreateValue(bb, "comboMultiplierList", hiddenUniversePlayEnterResponse.comboMultiplierList);
        BlackboardUtils.SetOrCreateValue(bb, "hintCooltime", hiddenUniversePlayEnterResponse.hintCooltime);
        BlackboardUtils.SetOrCreateValue(bb, "penaltyTriggerCount", hiddenUniversePlayEnterResponse.penaltyTriggerCount);
        BlackboardUtils.SetOrCreateValue(bb, "penaltyTriggerDuration", hiddenUniversePlayEnterResponse.penaltyTriggerDuration);
        BlackboardUtils.SetOrCreateValue(bb, "penaltyDuration", hiddenUniversePlayEnterResponse.penaltyDuration);
        BlackboardUtils.SetOrCreateValue(bb, "timeLimit", hiddenUniversePlayEnterResponse.timeLimit);
    }

    public static void Serialize(IBlackboard bb, HiddenUniversePlayLeaveRequest hiddenUniversePlayLeaveRequest)
    {
        if (hiddenUniversePlayLeaveRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", hiddenUniversePlayLeaveRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", hiddenUniversePlayLeaveRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "chapter", hiddenUniversePlayLeaveRequest.chapter);
        BlackboardUtils.SetOrCreateValue(bb, "stage", hiddenUniversePlayLeaveRequest.stage);
    }

    public static void Serialize(IBlackboard bb, HiddenUniversePlayLeaveResponse hiddenUniversePlayLeaveResponse)
    {
        if (hiddenUniversePlayLeaveResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", hiddenUniversePlayLeaveResponse.error);
        if (hiddenUniversePlayLeaveResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), hiddenUniversePlayLeaveResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", hiddenUniversePlayLeaveResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "maxFinder", hiddenUniversePlayLeaveResponse.maxFinder);
        BlackboardUtils.SetOrCreateValue(bb, "finder", hiddenUniversePlayLeaveResponse.finder);
    }

    public static void Serialize(IBlackboard bb, HiddenUniversePlayLog hiddenUniversePlayLog)
    {
        if (hiddenUniversePlayLog == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "type", hiddenUniversePlayLog.type);
        BlackboardUtils.SetOrCreateValue(bb, "time", hiddenUniversePlayLog.time);
        if (hiddenUniversePlayLog.content != null)
        {
            switch (hiddenUniversePlayLog.type)
            {
            case HiddenUniversePlayTypes.FIND:
                Serialize(bb, (HiddenUniverseFindContent)hiddenUniversePlayLog.content);
                break;

            case HiddenUniversePlayTypes.WRONG:
                Serialize(bb, (HiddenUniverseWrongContent)hiddenUniversePlayLog.content);
                break;

            case HiddenUniversePlayTypes.HINT:
                Serialize(bb, (HiddenUniverseHintContent)hiddenUniversePlayLog.content);
                break;

            case HiddenUniversePlayTypes.CLEAR:
                Serialize(bb, (HiddenUniverseClearContent)hiddenUniversePlayLog.content);
                break;

            case HiddenUniversePlayTypes.TIME_UP:
                Serialize(bb, (HiddenUniverseTimeUpContent)hiddenUniversePlayLog.content);
                break;
            }
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "content");
        }
    }

    public static void Serialize(IBlackboard bb, HiddenUniverseRank hiddenUniverseRank)
    {
        if (hiddenUniverseRank == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "rank", hiddenUniverseRank.rank);
        BlackboardUtils.SetOrCreateValue(bb, "userId", hiddenUniverseRank.userId);
        BlackboardUtils.SetOrCreateValue(bb, "userName", hiddenUniverseRank.userName);
        BlackboardUtils.SetOrCreateValue(bb, "profileUrl", hiddenUniverseRank.profileUrl);
        BlackboardUtils.SetOrCreateValue(bb, "score", hiddenUniverseRank.score);
        BlackboardUtils.SetOrCreateValue(bb, "tier", hiddenUniverseRank.tier);
    }

    public static void Serialize(IBlackboard bb, HiddenUniverseRequestFinderRequest hiddenUniverseRequestFinderRequest)
    {
        if (hiddenUniverseRequestFinderRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", hiddenUniverseRequestFinderRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", hiddenUniverseRequestFinderRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "clubId", hiddenUniverseRequestFinderRequest.clubId);
    }

    public static void Serialize(IBlackboard bb, HiddenUniverseRequestFinderResponse hiddenUniverseRequestFinderResponse)
    {
        if (hiddenUniverseRequestFinderResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", hiddenUniverseRequestFinderResponse.error);
        if (hiddenUniverseRequestFinderResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), hiddenUniverseRequestFinderResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", hiddenUniverseRequestFinderResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "hiddenUniverseFinderRequestCooltime", hiddenUniverseRequestFinderResponse.hiddenUniverseFinderRequestCooltime);
        BlackboardUtils.SetOrCreateValue(bb, "lastFinderRequestTimestamp", hiddenUniverseRequestFinderResponse.lastFinderRequestTimestamp);
    }

    public static void Serialize(IBlackboard bb, HiddenUniverseRewardedChapterInfo hiddenUniverseRewardedChapterInfo)
    {
        if (hiddenUniverseRewardedChapterInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "chapter", hiddenUniverseRewardedChapterInfo.chapter);
        BlackboardUtils.SetOrCreateValue(bb, "isRewarded", hiddenUniverseRewardedChapterInfo.isRewarded);
    }

    public static void Serialize(IBlackboard bb, HiddenUniverseStageInfo hiddenUniverseStageInfo)
    {
        if (hiddenUniverseStageInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "chapter", hiddenUniverseStageInfo.chapter);
        BlackboardUtils.SetOrCreateValue(bb, "stage", hiddenUniverseStageInfo.stage);
        BlackboardUtils.SetOrCreateValue(bb, "completedStarCount", hiddenUniverseStageInfo.completedStarCount);
        BlackboardUtils.SetOrCreateValue(bb, "ongoingStarPercentile", hiddenUniverseStageInfo.ongoingStarPercentile);
        BlackboardUtils.SetOrCreateValue(bb, "needFinderCount", hiddenUniverseStageInfo.needFinderCount);
    }

    public static void Serialize(IBlackboard bb, HiddenUniverseStageUnlock hiddenUniverseStageUnlock)
    {
        if (hiddenUniverseStageUnlock == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "chapter", hiddenUniverseStageUnlock.chapter);
        BlackboardUtils.SetOrCreateValue(bb, "stage", hiddenUniverseStageUnlock.stage);
        BlackboardUtils.SetOrCreateValue(bb, "unlock", hiddenUniverseStageUnlock.unlock);
    }

    public static void Serialize(IBlackboard bb, HiddenUniverseTimeUpContent hiddenUniverseTimeUpContent)
    {
        if (hiddenUniverseTimeUpContent == null) { return; }
    }

    public static void Serialize(IBlackboard bb, HiddenUniverseVideoAdsClaimRequest hiddenUniverseVideoAdsClaimRequest)
    {
        if (hiddenUniverseVideoAdsClaimRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", hiddenUniverseVideoAdsClaimRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", hiddenUniverseVideoAdsClaimRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "contextId", hiddenUniverseVideoAdsClaimRequest.contextId);
    }

    public static void Serialize(IBlackboard bb, HiddenUniverseVideoAdsClaimResponse hiddenUniverseVideoAdsClaimResponse)
    {
        if (hiddenUniverseVideoAdsClaimResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", hiddenUniverseVideoAdsClaimResponse.error);
        if (hiddenUniverseVideoAdsClaimResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), hiddenUniverseVideoAdsClaimResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", hiddenUniverseVideoAdsClaimResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "hiddenUniverseVideoAdsCooltime", hiddenUniverseVideoAdsClaimResponse.hiddenUniverseVideoAdsCooltime);
        BlackboardUtils.SetOrCreateValue(bb, "lastVideoAdsClaimTimestamp", hiddenUniverseVideoAdsClaimResponse.lastVideoAdsClaimTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "finder", hiddenUniverseVideoAdsClaimResponse.finder);
    }

    public static void Serialize(IBlackboard bb, HiddenUniverseWrongContent hiddenUniverseWrongContent)
    {
        if (hiddenUniverseWrongContent == null) { return; }
    }

    public static void Serialize(IBlackboard bb, HogDealClearContent hogDealClearContent)
    {
        if (hogDealClearContent == null) { return; }
    }

    public static void Serialize(IBlackboard bb, HogDealCollectRequest hogDealCollectRequest)
    {
        if (hogDealCollectRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", hogDealCollectRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", hogDealCollectRequest.ackMask);
        BlackboardUtils.SetOrCreateList(bb, "playLog", hogDealCollectRequest.playLog, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "contextId", hogDealCollectRequest.contextId);
    }

    public static void Serialize(IBlackboard bb, HogDealCollectResponse hogDealCollectResponse)
    {
        if (hogDealCollectResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", hogDealCollectResponse.error);
        if (hogDealCollectResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), hogDealCollectResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", hogDealCollectResponse.serverTime);
        if (hogDealCollectResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), hogDealCollectResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "earnCredit", hogDealCollectResponse.earnCredit);
        BlackboardUtils.SetOrCreateValue(bb, "jackpotType", hogDealCollectResponse.jackpotType);
    }

    public static void Serialize(IBlackboard bb, HogDealFindContent hogDealFindContent)
    {
        if (hogDealFindContent == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "objectIndex", hogDealFindContent.objectIndex);
        BlackboardUtils.SetOrCreateValue(bb, "score", hogDealFindContent.score);
    }

    public static void Serialize(IBlackboard bb, HogDealHintContent hogDealHintContent)
    {
        if (hogDealHintContent == null) { return; }
    }

    public static void Serialize(IBlackboard bb, HogDealPlayLog hogDealPlayLog)
    {
        if (hogDealPlayLog == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "type", hogDealPlayLog.type);
        BlackboardUtils.SetOrCreateValue(bb, "time", hogDealPlayLog.time);
        if (hogDealPlayLog.content != null)
        {
            switch (hogDealPlayLog.type)
            {
            case HogDealPlayTypes.FIND:
                Serialize(bb, (HogDealFindContent)hogDealPlayLog.content);
                break;

            case HogDealPlayTypes.WRONG:
                Serialize(bb, (HogDealWrongContent)hogDealPlayLog.content);
                break;

            case HogDealPlayTypes.HINT:
                Serialize(bb, (HogDealHintContent)hogDealPlayLog.content);
                break;

            case HogDealPlayTypes.CLEAR:
                Serialize(bb, (HogDealClearContent)hogDealPlayLog.content);
                break;
            }
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "content");
        }
    }

    public static void Serialize(IBlackboard bb, HogDealWrongContent hogDealWrongContent)
    {
        if (hogDealWrongContent == null) { return; }
    }

    public static void Serialize(IBlackboard bb, IamExtraData iamExtraData)
    {
        if (iamExtraData == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "bossRaidersDealBaseAttackHp", iamExtraData.bossRaidersDealBaseAttackHp);
    }

    public static void Serialize(IBlackboard bb, InAppMessageAllInTrigger inAppMessageAllInTrigger)
    {
        if (inAppMessageAllInTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageAllInWithPushOffTrigger inAppMessageAllInWithPushOffTrigger)
    {
        if (inAppMessageAllInWithPushOffTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageAnyPurchaseClickGetButtonTrigger inAppMessageAnyPurchaseClickGetButtonTrigger)
    {
        if (inAppMessageAnyPurchaseClickGetButtonTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageBonusEventBadge inAppMessageBonusEventBadge)
    {
        if (inAppMessageBonusEventBadge == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageBuyBonusPurchasePopup inAppMessageBuyBonusPurchasePopup)
    {
        if (inAppMessageBuyBonusPurchasePopup == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", inAppMessageBuyBonusPurchasePopup.gameId);
    }

    public static void Serialize(IBlackboard bb, InAppMessageClaimVideoAdsRequest inAppMessageClaimVideoAdsRequest)
    {
        if (inAppMessageClaimVideoAdsRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", inAppMessageClaimVideoAdsRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", inAppMessageClaimVideoAdsRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "iamId", inAppMessageClaimVideoAdsRequest.iamId);
    }

    public static void Serialize(IBlackboard bb, InAppMessageClaimVideoAdsResponse inAppMessageClaimVideoAdsResponse)
    {
        if (inAppMessageClaimVideoAdsResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", inAppMessageClaimVideoAdsResponse.error);
        if (inAppMessageClaimVideoAdsResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), inAppMessageClaimVideoAdsResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", inAppMessageClaimVideoAdsResponse.serverTime);
        if (inAppMessageClaimVideoAdsResponse.rewardResult != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "rewardResult"), inAppMessageClaimVideoAdsResponse.rewardResult);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "rewardResult");
        }
    }

    public static void Serialize(IBlackboard bb, InAppMessageCloseClub inAppMessageCloseClub)
    {
        if (inAppMessageCloseClub == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageCloseDailySpinTrigger inAppMessageCloseDailySpinTrigger)
    {
        if (inAppMessageCloseDailySpinTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageCloseEarningPointTrigger inAppMessageCloseEarningPointTrigger)
    {
        if (inAppMessageCloseEarningPointTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageCloseFriend inAppMessageCloseFriend)
    {
        if (inAppMessageCloseFriend == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageCloseHiddenUniverseShopTrigger inAppMessageCloseHiddenUniverseShopTrigger)
    {
        if (inAppMessageCloseHiddenUniverseShopTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageCloseInboxTrigger inAppMessageCloseInboxTrigger)
    {
        if (inAppMessageCloseInboxTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageCloseInhouseAdsTrigger inAppMessageCloseInhouseAdsTrigger)
    {
        if (inAppMessageCloseInhouseAdsTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageCloseLevelUpDashMain inAppMessageCloseLevelUpDashMain)
    {
        if (inAppMessageCloseLevelUpDashMain == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageClosePotOfGoldTrigger inAppMessageClosePotOfGoldTrigger)
    {
        if (inAppMessageClosePotOfGoldTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageCloseShopTrigger inAppMessageCloseShopTrigger)
    {
        if (inAppMessageCloseShopTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageCloseVideoAdsTrigger inAppMessageCloseVideoAdsTrigger)
    {
        if (inAppMessageCloseVideoAdsTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageCloseVipLoungeTrigger inAppMessageCloseVipLoungeTrigger)
    {
        if (inAppMessageCloseVipLoungeTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageClubDealPopup inAppMessageClubDealPopup)
    {
        if (inAppMessageClubDealPopup == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageCoinPurchasePopup inAppMessageCoinPurchasePopup)
    {
        if (inAppMessageCoinPurchasePopup == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "maxPurchaseCount", inAppMessageCoinPurchasePopup.maxPurchaseCount);
        BlackboardUtils.SetOrCreateValue(bb, "nextSequentialIamId", inAppMessageCoinPurchasePopup.nextSequentialIamId);
        BlackboardUtils.SetOrCreateDict(bb, "productInfoDict", inAppMessageCoinPurchasePopup.productInfoDict, Serialize);
    }

    public static void Serialize(IBlackboard bb, InAppMessageCollectTimedBonusTrigger inAppMessageCollectTimedBonusTrigger)
    {
        if (inAppMessageCollectTimedBonusTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageComponent inAppMessageComponent)
    {
        if (inAppMessageComponent == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "type", inAppMessageComponent.type);
        BlackboardUtils.SetOrCreateValue(bb, "posX", inAppMessageComponent.posX);
        BlackboardUtils.SetOrCreateValue(bb, "posY", inAppMessageComponent.posY);
        if (inAppMessageComponent.data != null)
        {
            switch (inAppMessageComponent.type)
            {
            case InAppMessageComponentType.BACKGROUND_WEB_IMAGE:
                Serialize(bb, (InAppMessageComponentBackgroundWebImage)inAppMessageComponent.data);
                break;

            case InAppMessageComponentType.TEXT:
                Serialize(bb, (InAppMessageComponentText)inAppMessageComponent.data);
                break;

            case InAppMessageComponentType.TIMER:
                Serialize(bb, (InAppMessageComponentTimer)inAppMessageComponent.data);
                break;

            case InAppMessageComponentType.SLOT_THUMBNAIL:
                Serialize(bb, (InAppMessageComponentSlotThumbnail)inAppMessageComponent.data);
                break;

            case InAppMessageComponentType.SCROLL_TEXT_BOX:
                Serialize(bb, (InAppMessageComponentScrollTextBox)inAppMessageComponent.data);
                break;

            case InAppMessageComponentType.CLICK_ACTION:
                Serialize(bb, (InAppMessageComponentClickAction)inAppMessageComponent.data);
                break;

            case InAppMessageComponentType.BUY_BUTTON:
                Serialize(bb, (InAppMessageComponentBuyButton)inAppMessageComponent.data);
                break;

            case InAppMessageComponentType.BUTTON:
                Serialize(bb, (InAppMessageComponentButton)inAppMessageComponent.data);
                break;

            case InAppMessageComponentType.TIER_ICON:
                Serialize(bb, (InAppMessageComponentTierIcon)inAppMessageComponent.data);
                break;

            case InAppMessageComponentType.SINGLE_BONUS_BUY_BUTTON:
                Serialize(bb, (InAppMessageComponentSingleBonusBuyButton)inAppMessageComponent.data);
                break;

            case InAppMessageComponentType.RANGE_BONUS_BUY_BUTTON:
                Serialize(bb, (InAppMessageComponentRangeBonusBuyButton)inAppMessageComponent.data);
                break;

            case InAppMessageComponentType.BONUS_EVENT_BADGE:
                Serialize(bb, (InAppMessageBonusEventBadge)inAppMessageComponent.data);
                break;

            case InAppMessageComponentType.SINGLE_RANGE_BONUS_BUY_BUTTON:
                Serialize(bb, (InAppMessageComponentRangeBonusBuyButton)inAppMessageComponent.data);
                break;

            case InAppMessageComponentType.EPIC_PASS_BUY_BUTTON:
                Serialize(bb, (InAppMessageComponentEpicPassBuyButton)inAppMessageComponent.data);
                break;

            case InAppMessageComponentType.LEVEL_MULTIPLIER_TEXT:
                Serialize(bb, (InAppMessageComponentLevelMultiplierText)inAppMessageComponent.data);
                break;

            case InAppMessageComponentType.LEVEL_UP_DASH_MISSION_LEFT_TIME:
                Serialize(bb, (InAppMessageComponentLevelUpDashMissionLeftTime)inAppMessageComponent.data);
                break;

            case InAppMessageComponentType.RED_RIBBON_TAG:
                Serialize(bb, (InAppMessageComponentRedRibbonTag)inAppMessageComponent.data);
                break;
            }
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "data");
        }
        BlackboardUtils.SetOrCreateValue(bb, "rotation", inAppMessageComponent.rotation);
    }

    public static void Serialize(IBlackboard bb, InAppMessageComponentBackgroundWebImage inAppMessageComponentBackgroundWebImage)
    {
        if (inAppMessageComponentBackgroundWebImage == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "imageUrl", inAppMessageComponentBackgroundWebImage.imageUrl);
    }

    public static void Serialize(IBlackboard bb, InAppMessageComponentButton inAppMessageComponentButton)
    {
        if (inAppMessageComponentButton == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "width", inAppMessageComponentButton.width);
        BlackboardUtils.SetOrCreateValue(bb, "height", inAppMessageComponentButton.height);
        if (inAppMessageComponentButton.action != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "action"), inAppMessageComponentButton.action);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "action");
        }
        BlackboardUtils.SetOrCreateValue(bb, "closeWhenSucceeded", inAppMessageComponentButton.closeWhenSucceeded);
        BlackboardUtils.SetOrCreateValue(bb, "text", inAppMessageComponentButton.text);
        BlackboardUtils.SetOrCreateValue(bb, "isTagged", inAppMessageComponentButton.isTagged);
        BlackboardUtils.SetOrCreateValue(bb, "tag", inAppMessageComponentButton.tag);
        BlackboardUtils.SetOrCreateValue(bb, "useAnimation", inAppMessageComponentButton.useAnimation);
    }

    public static void Serialize(IBlackboard bb, InAppMessageComponentBuyButton inAppMessageComponentBuyButton)
    {
        if (inAppMessageComponentBuyButton == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "width", inAppMessageComponentBuyButton.width);
        BlackboardUtils.SetOrCreateValue(bb, "height", inAppMessageComponentBuyButton.height);
        if (inAppMessageComponentBuyButton.product != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "product"), inAppMessageComponentBuyButton.product);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "product");
        }
        BlackboardUtils.SetOrCreateValue(bb, "closeWhenSucceeded", inAppMessageComponentBuyButton.closeWhenSucceeded);
        BlackboardUtils.SetOrCreateValue(bb, "userGroupId", inAppMessageComponentBuyButton.userGroupId);
        BlackboardUtils.SetOrCreateValue(bb, "isTagged", inAppMessageComponentBuyButton.isTagged);
        BlackboardUtils.SetOrCreateValue(bb, "tag", inAppMessageComponentBuyButton.tag);
        BlackboardUtils.SetOrCreateValue(bb, "useAnimation", inAppMessageComponentBuyButton.useAnimation);
    }

    public static void Serialize(IBlackboard bb, InAppMessageComponentClickAction inAppMessageComponentClickAction)
    {
        if (inAppMessageComponentClickAction == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "width", inAppMessageComponentClickAction.width);
        BlackboardUtils.SetOrCreateValue(bb, "height", inAppMessageComponentClickAction.height);
        if (inAppMessageComponentClickAction.action != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "action"), inAppMessageComponentClickAction.action);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "action");
        }
        BlackboardUtils.SetOrCreateValue(bb, "closeWhenSucceeded", inAppMessageComponentClickAction.closeWhenSucceeded);
    }

    public static void Serialize(IBlackboard bb, InAppMessageComponentEpicPassBuyButton inAppMessageComponentEpicPassBuyButton)
    {
        if (inAppMessageComponentEpicPassBuyButton == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "width", inAppMessageComponentEpicPassBuyButton.width);
        BlackboardUtils.SetOrCreateValue(bb, "height", inAppMessageComponentEpicPassBuyButton.height);
        BlackboardUtils.SetOrCreateValue(bb, "text", inAppMessageComponentEpicPassBuyButton.text);
    }

    public static void Serialize(IBlackboard bb, InAppMessageComponentLevelMultiplierText inAppMessageComponentLevelMultiplierText)
    {
        if (inAppMessageComponentLevelMultiplierText == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "showLevelMultiplierOnText", inAppMessageComponentLevelMultiplierText.showLevelMultiplierOnText);
        BlackboardUtils.SetOrCreateValue(bb, "showLevelMultiplierOffText", inAppMessageComponentLevelMultiplierText.showLevelMultiplierOffText);
        if (inAppMessageComponentLevelMultiplierText.product != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "product"), inAppMessageComponentLevelMultiplierText.product);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "product");
        }
        BlackboardUtils.SetOrCreateValue(bb, "showOutline", inAppMessageComponentLevelMultiplierText.showOutline);
    }

    public static void Serialize(IBlackboard bb, InAppMessageComponentLevelUpDashMissionLeftTime inAppMessageComponentLevelUpDashMissionLeftTime)
    {
        if (inAppMessageComponentLevelUpDashMissionLeftTime == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageComponentRangeBonusBuyButton inAppMessageComponentRangeBonusBuyButton)
    {
        if (inAppMessageComponentRangeBonusBuyButton == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", inAppMessageComponentRangeBonusBuyButton.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "ticketedBonusId", inAppMessageComponentRangeBonusBuyButton.ticketedBonusId);
        BlackboardUtils.SetOrCreateValue(bb, "minGem", inAppMessageComponentRangeBonusBuyButton.minGem);
        BlackboardUtils.SetOrCreateValue(bb, "maxGem", inAppMessageComponentRangeBonusBuyButton.maxGem);
        BlackboardUtils.SetOrCreateValue(bb, "closeWhenSucceeded", inAppMessageComponentRangeBonusBuyButton.closeWhenSucceeded);
        BlackboardUtils.SetOrCreateValue(bb, "useAnimation", inAppMessageComponentRangeBonusBuyButton.useAnimation);
    }

    public static void Serialize(IBlackboard bb, InAppMessageComponentRedRibbonTag inAppMessageComponentRedRibbonTag)
    {
        if (inAppMessageComponentRedRibbonTag == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "tagSize", inAppMessageComponentRedRibbonTag.tagSize);
        BlackboardUtils.SetOrCreateValue(bb, "text", inAppMessageComponentRedRibbonTag.text);
        if (inAppMessageComponentRedRibbonTag.product != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "product"), inAppMessageComponentRedRibbonTag.product);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "product");
        }
    }

    public static void Serialize(IBlackboard bb, InAppMessageComponentScrollTextBox inAppMessageComponentScrollTextBox)
    {
        if (inAppMessageComponentScrollTextBox == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "width", inAppMessageComponentScrollTextBox.width);
        BlackboardUtils.SetOrCreateValue(bb, "height", inAppMessageComponentScrollTextBox.height);
        BlackboardUtils.SetOrCreateValue(bb, "text", inAppMessageComponentScrollTextBox.text);
    }

    public static void Serialize(IBlackboard bb, InAppMessageComponentSingleBonusBuyButton inAppMessageComponentSingleBonusBuyButton)
    {
        if (inAppMessageComponentSingleBonusBuyButton == null) { return; }
        if (inAppMessageComponentSingleBonusBuyButton.action != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "action"), inAppMessageComponentSingleBonusBuyButton.action);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "action");
        }
        BlackboardUtils.SetOrCreateValue(bb, "closeWhenSucceeded", inAppMessageComponentSingleBonusBuyButton.closeWhenSucceeded);
        BlackboardUtils.SetOrCreateValue(bb, "useAnimation", inAppMessageComponentSingleBonusBuyButton.useAnimation);
    }

    public static void Serialize(IBlackboard bb, InAppMessageComponentSlotThumbnail inAppMessageComponentSlotThumbnail)
    {
        if (inAppMessageComponentSlotThumbnail == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "width", inAppMessageComponentSlotThumbnail.width);
        BlackboardUtils.SetOrCreateValue(bb, "height", inAppMessageComponentSlotThumbnail.height);
        BlackboardUtils.SetOrCreateValue(bb, "gameId", inAppMessageComponentSlotThumbnail.gameId);
    }

    public static void Serialize(IBlackboard bb, InAppMessageComponentText inAppMessageComponentText)
    {
        if (inAppMessageComponentText == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "text", inAppMessageComponentText.text);
        if (inAppMessageComponentText.product != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "product"), inAppMessageComponentText.product);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "product");
        }
        BlackboardUtils.SetOrCreateValue(bb, "showOutline", inAppMessageComponentText.showOutline);
    }

    public static void Serialize(IBlackboard bb, InAppMessageComponentTierIcon inAppMessageComponentTierIcon)
    {
        if (inAppMessageComponentTierIcon == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageComponentTimer inAppMessageComponentTimer)
    {
        if (inAppMessageComponentTimer == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageDailyBoostPurchasePopup inAppMessageDailyBoostPurchasePopup)
    {
        if (inAppMessageDailyBoostPurchasePopup == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageEarlyAccessLobbyEnterTrigger inAppMessageEarlyAccessLobbyEnterTrigger)
    {
        if (inAppMessageEarlyAccessLobbyEnterTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageEarlyAccessNonSubscriberSlotEnterTrigger inAppMessageEarlyAccessNonSubscriberSlotEnterTrigger)
    {
        if (inAppMessageEarlyAccessNonSubscriberSlotEnterTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageEarlyAccessPurchasePopup inAppMessageEarlyAccessPurchasePopup)
    {
        if (inAppMessageEarlyAccessPurchasePopup == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageEnterClubFromLobbyTrigger inAppMessageEnterClubFromLobbyTrigger)
    {
        if (inAppMessageEnterClubFromLobbyTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageEnterClubFromLobbyWithLeaderPushUnlockedTrigger inAppMessageEnterClubFromLobbyWithLeaderPushUnlockedTrigger)
    {
        if (inAppMessageEnterClubFromLobbyWithLeaderPushUnlockedTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageEnterEarningPointTrigger inAppMessageEnterEarningPointTrigger)
    {
        if (inAppMessageEnterEarningPointTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageEnterGameFromLobbyTrigger inAppMessageEnterGameFromLobbyTrigger)
    {
        if (inAppMessageEnterGameFromLobbyTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageEnterHiddenUniverseShopTrigger inAppMessageEnterHiddenUniverseShopTrigger)
    {
        if (inAppMessageEnterHiddenUniverseShopTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageEnterHiddenUniverseTrigger inAppMessageEnterHiddenUniverseTrigger)
    {
        if (inAppMessageEnterHiddenUniverseTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageEnterInboxTrigger inAppMessageEnterInboxTrigger)
    {
        if (inAppMessageEnterInboxTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageEnterLobbyFromGameTrigger inAppMessageEnterLobbyFromGameTrigger)
    {
        if (inAppMessageEnterLobbyFromGameTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageEnterLobbyFromLoginTrigger inAppMessageEnterLobbyFromLoginTrigger)
    {
        if (inAppMessageEnterLobbyFromLoginTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageEnterVipLoungeTrigger inAppMessageEnterVipLoungeTrigger)
    {
        if (inAppMessageEnterVipLoungeTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageExitHiddenUniverseTrigger inAppMessageExitHiddenUniverseTrigger)
    {
        if (inAppMessageExitHiddenUniverseTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageGemChestShopTrigger inAppMessageGemChestShopTrigger)
    {
        if (inAppMessageGemChestShopTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageIDFAPopup inAppMessageIDFAPopup)
    {
        if (inAppMessageIDFAPopup == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageInHouseAdsForFreeDepotTrigger inAppMessageInHouseAdsForFreeDepotTrigger)
    {
        if (inAppMessageInHouseAdsForFreeDepotTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageInfo inAppMessageInfo)
    {
        if (inAppMessageInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "type", inAppMessageInfo.type);
        BlackboardUtils.SetOrCreateList(bb, "componentList", inAppMessageInfo.componentList, Serialize);
        if (inAppMessageInfo.data != null)
        {
            switch (inAppMessageInfo.type)
            {
            case InAppMessageType.POPUP:
                Serialize(bb, (InAppMessagePopup)inAppMessageInfo.data);
                break;

            case InAppMessageType.COIN_PURCHASE_POPUP:
                Serialize(bb, (InAppMessageCoinPurchasePopup)inAppMessageInfo.data);
                break;

            case InAppMessageType.DAILY_BOOST_PURCHASE_POPUP:
                Serialize(bb, (InAppMessageDailyBoostPurchasePopup)inAppMessageInfo.data);
                break;

            case InAppMessageType.EARLY_ACCESS_PURCHASE_POPUP:
                Serialize(bb, (InAppMessageEarlyAccessPurchasePopup)inAppMessageInfo.data);
                break;

            case InAppMessageType.INSTANT_BONUS_PURCHASE_POPUP:
                Serialize(bb, (InAppMessageInstantBonusPurchasePopup)inAppMessageInfo.data);
                break;

            case InAppMessageType.VIDEO_ADS_POPUP:
                Serialize(bb, (InAppMessageVideoAdsPopup)inAppMessageInfo.data);
                break;

            case InAppMessageType.STATUS_MATCH_POPUP:
                Serialize(bb, (InAppMessageStatusMatchPopup)inAppMessageInfo.data);
                break;

            case InAppMessageType.SURVEY_POPUP:
                Serialize(bb, (InAppMessageSurveyPopup)inAppMessageInfo.data);
                break;

            case InAppMessageType.CLUB_DEAL_POPUP:
                Serialize(bb, (InAppMessageClubDealPopup)inAppMessageInfo.data);
                break;

            case InAppMessageType.BUY_A_BONUS_PURCHASE_POPUP:
                Serialize(bb, (InAppMessageBuyBonusPurchasePopup)inAppMessageInfo.data);
                break;

            case InAppMessageType.SUPER_BONUS_PURCHASE_POPUP:
                Serialize(bb, (InAppMessageSuperBonusPurchasePopup)inAppMessageInfo.data);
                break;

            case InAppMessageType.EPIC_PASS_PURCHASE_POPUP:
                Serialize(bb, (InAppMessageSeasonPassPurchasePopup)inAppMessageInfo.data);
                break;

            case InAppMessageType.IDFA_POPUP:
                Serialize(bb, (InAppMessageIDFAPopup)inAppMessageInfo.data);
                break;

            case InAppMessageType.TERMS_OF_USE_POPUP:
                Serialize(bb, (InAppMessageTermsOfUsePopup)inAppMessageInfo.data);
                break;

            case InAppMessageType.INVISIBLE_ACTION_TRIGGER:
                Serialize(bb, (InAppMessageInvisibleActionTrigger)inAppMessageInfo.data);
                break;

            case InAppMessageType.PIN_TO_TASKBAR_POPUP:
                Serialize(bb, (InAppMessagePinToTaskbarPopup)inAppMessageInfo.data);
                break;
            }
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "data");
        }
        BlackboardUtils.SetOrCreateValue(bb, "cooltimeSec", inAppMessageInfo.cooltimeSec);
        BlackboardUtils.SetOrCreateValue(bb, "useUserTimer", inAppMessageInfo.useUserTimer);
        BlackboardUtils.SetOrCreateValue(bb, "userTimerMin", inAppMessageInfo.userTimerMin);
        BlackboardUtils.SetOrCreateValue(bb, "endTimestamp", inAppMessageInfo.endTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "id", inAppMessageInfo.id);
        BlackboardUtils.SetOrCreateValue(bb, "userTimerRegenMin", inAppMessageInfo.userTimerRegenMin);
        BlackboardUtils.SetOrCreateValue(bb, "startTimestamp", inAppMessageInfo.startTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "locktimeSec", inAppMessageInfo.locktimeSec);
        BlackboardUtils.SetOrCreateValue(bb, "name", inAppMessageInfo.name);
        BlackboardUtils.SetOrCreateValue(bb, "useDeal", inAppMessageInfo.useDeal);
        BlackboardUtils.SetOrCreateValue(bb, "maxExposureCount", inAppMessageInfo.maxExposureCount);
        BlackboardUtils.SetOrCreateValue(bb, "behaviorKeyViewFromLobby", inAppMessageInfo.behaviorKeyViewFromLobby);
        BlackboardUtils.SetOrCreateValue(bb, "behaviorKeyViewFromAll", inAppMessageInfo.behaviorKeyViewFromAll);
        BlackboardUtils.SetOrCreateValue(bb, "promotedGameId", inAppMessageInfo.promotedGameId);
        BlackboardUtils.SetOrCreateValue(bb, "priority", inAppMessageInfo.priority);
        BlackboardUtils.SetOrCreateList(bb, "triggerV2List", inAppMessageInfo.triggerV2List, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "useAreYouSurePopup", inAppMessageInfo.useAreYouSurePopup);
    }

    public static void Serialize(IBlackboard bb, InAppMessageInfoRequest inAppMessageInfoRequest)
    {
        if (inAppMessageInfoRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", inAppMessageInfoRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", inAppMessageInfoRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "iamId", inAppMessageInfoRequest.iamId);
    }

    public static void Serialize(IBlackboard bb, InAppMessageInfoResponse inAppMessageInfoResponse)
    {
        if (inAppMessageInfoResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", inAppMessageInfoResponse.error);
        if (inAppMessageInfoResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), inAppMessageInfoResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", inAppMessageInfoResponse.serverTime);
        if (inAppMessageInfoResponse.inAppMessage != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "inAppMessage"), inAppMessageInfoResponse.inAppMessage);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "inAppMessage");
        }
    }

    public static void Serialize(IBlackboard bb, InAppMessageInhouseAdsForBossRaidersTrigger inAppMessageInhouseAdsForBossRaidersTrigger)
    {
        if (inAppMessageInhouseAdsForBossRaidersTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageInhouseAdsForClubArenaTrigger inAppMessageInhouseAdsForClubArenaTrigger)
    {
        if (inAppMessageInhouseAdsForClubArenaTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageInhouseAdsForCollectingGameTrigger inAppMessageInhouseAdsForCollectingGameTrigger)
    {
        if (inAppMessageInhouseAdsForCollectingGameTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageInhouseAdsForGemJackpotTrigger inAppMessageInhouseAdsForGemJackpotTrigger)
    {
        if (inAppMessageInhouseAdsForGemJackpotTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageInhouseAdsForHiddenUniverseTrigger inAppMessageInhouseAdsForHiddenUniverseTrigger)
    {
        if (inAppMessageInhouseAdsForHiddenUniverseTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageInhouseAdsForInboxTrigger inAppMessageInhouseAdsForInboxTrigger)
    {
        if (inAppMessageInhouseAdsForInboxTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageInhouseAdsForSeasonPassTrigger inAppMessageInhouseAdsForSeasonPassTrigger)
    {
        if (inAppMessageInhouseAdsForSeasonPassTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageInhouseAdsForShopTrigger inAppMessageInhouseAdsForShopTrigger)
    {
        if (inAppMessageInhouseAdsForShopTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageInhouseAdsForTimeCollectTrigger inAppMessageInhouseAdsForTimeCollectTrigger)
    {
        if (inAppMessageInhouseAdsForTimeCollectTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageInhouseAdsWithPushOffTrigger inAppMessageInhouseAdsWithPushOffTrigger)
    {
        if (inAppMessageInhouseAdsWithPushOffTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageInstantBonusPurchasePopup inAppMessageInstantBonusPurchasePopup)
    {
        if (inAppMessageInstantBonusPurchasePopup == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", inAppMessageInstantBonusPurchasePopup.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "isSale", inAppMessageInstantBonusPurchasePopup.isSale);
        BlackboardUtils.SetOrCreateValue(bb, "salePercentage", inAppMessageInstantBonusPurchasePopup.salePercentage);
    }

    public static void Serialize(IBlackboard bb, InAppMessageInvisibleActionTrigger inAppMessageInvisibleActionTrigger)
    {
        if (inAppMessageInvisibleActionTrigger == null) { return; }
        if (inAppMessageInvisibleActionTrigger.action != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "action"), inAppMessageInvisibleActionTrigger.action);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "action");
        }
    }

    public static void Serialize(IBlackboard bb, InAppMessageLimitedProductInfo inAppMessageLimitedProductInfo)
    {
        if (inAppMessageLimitedProductInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "purchaseLimit", inAppMessageLimitedProductInfo.purchaseLimit);
        BlackboardUtils.SetOrCreateValue(bb, "purchasedCount", inAppMessageLimitedProductInfo.purchasedCount);
    }

    public static void Serialize(IBlackboard bb, InAppMessageLobbyAttFromLoginTrigger inAppMessageLobbyAttFromLoginTrigger)
    {
        if (inAppMessageLobbyAttFromLoginTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessagePinToTaskbarPopup inAppMessagePinToTaskbarPopup)
    {
        if (inAppMessagePinToTaskbarPopup == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessagePopup inAppMessagePopup)
    {
        if (inAppMessagePopup == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageSeasonPassPurchasePopup inAppMessageSeasonPassPurchasePopup)
    {
        if (inAppMessageSeasonPassPurchasePopup == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageSetPushOffAtSettingsTrigger inAppMessageSetPushOffAtSettingsTrigger)
    {
        if (inAppMessageSetPushOffAtSettingsTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageSetPushOnAtDeviceToGetRewardTrigger inAppMessageSetPushOnAtDeviceToGetRewardTrigger)
    {
        if (inAppMessageSetPushOnAtDeviceToGetRewardTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageSetPushOnAtSettingsToGetRewardTrigger inAppMessageSetPushOnAtSettingsToGetRewardTrigger)
    {
        if (inAppMessageSetPushOnAtSettingsToGetRewardTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageSetPushOnToGetRewardTrigger inAppMessageSetPushOnToGetRewardTrigger)
    {
        if (inAppMessageSetPushOnToGetRewardTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageStatusMatchPopup inAppMessageStatusMatchPopup)
    {
        if (inAppMessageStatusMatchPopup == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageSuperBonusPurchasePopup inAppMessageSuperBonusPurchasePopup)
    {
        if (inAppMessageSuperBonusPurchasePopup == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", inAppMessageSuperBonusPurchasePopup.gameId);
    }

    public static void Serialize(IBlackboard bb, InAppMessageSurveyPopup inAppMessageSurveyPopup)
    {
        if (inAppMessageSurveyPopup == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageTermsOfUsePopup inAppMessageTermsOfUsePopup)
    {
        if (inAppMessageTermsOfUsePopup == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageTierUpTrigger inAppMessageTierUpTrigger)
    {
        if (inAppMessageTierUpTrigger == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "tier", inAppMessageTierUpTrigger.tier);
    }

    public static void Serialize(IBlackboard bb, InAppMessageTimebonusWithPushOffTrigger inAppMessageTimebonusWithPushOffTrigger)
    {
        if (inAppMessageTimebonusWithPushOffTrigger == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InAppMessageTrigger inAppMessageTrigger)
    {
        if (inAppMessageTrigger == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "type", inAppMessageTrigger.type);
        if (inAppMessageTrigger.data != null)
        {
            switch (inAppMessageTrigger.type)
            {
            case InAppMessageTriggerType.ENTER_LOBBY_FROM_LOGIN:
                Serialize(bb, (InAppMessageEnterLobbyFromLoginTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.ENTER_LOBBY_FROM_GAME:
                Serialize(bb, (InAppMessageEnterLobbyFromGameTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.CLOSE_SHOP:
                Serialize(bb, (InAppMessageCloseShopTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.COLLECT_TIMED_BONUS:
                Serialize(bb, (InAppMessageCollectTimedBonusTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.ALL_IN:
                Serialize(bb, (InAppMessageAllInTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.EARLY_ACCESS_LOBBY_ENTER:
                Serialize(bb, (InAppMessageEarlyAccessLobbyEnterTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.EARLY_ACCESS_NON_SUBSCRIBER_SLOT_ENTER:
                Serialize(bb, (InAppMessageEarlyAccessNonSubscriberSlotEnterTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.INHOUSE_ADS_FOR_TIME_COLLECT:
                Serialize(bb, (InAppMessageInhouseAdsForTimeCollectTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.ENTER_CLUB_FROM_LOBBY:
                Serialize(bb, (InAppMessageEnterClubFromLobbyTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.ENTER_INBOX:
                Serialize(bb, (InAppMessageEnterInboxTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.CLOSE_INBOX:
                Serialize(bb, (InAppMessageCloseInboxTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.CLOSE_POT_OF_GOLD:
                Serialize(bb, (InAppMessageClosePotOfGoldTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.CLOSE_DAILY_SPIN:
                Serialize(bb, (InAppMessageCloseDailySpinTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.ENTER_GAME_FROM_LOBBY:
                Serialize(bb, (InAppMessageEnterGameFromLobbyTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.INHOUSE_ADS_FOR_SHOP:
                Serialize(bb, (InAppMessageInhouseAdsForShopTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.INHOUSE_ADS_FOR_COLLECTING_GAME:
                Serialize(bb, (InAppMessageInhouseAdsForCollectingGameTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.TIMEBONUS_WITH_PUSH_OFF:
                Serialize(bb, (InAppMessageTimebonusWithPushOffTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.INHOUSE_ADS_WITH_PUSH_OFF:
                Serialize(bb, (InAppMessageInhouseAdsWithPushOffTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.ALL_IN_WITH_PUSH_OFF:
                Serialize(bb, (InAppMessageAllInWithPushOffTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.SET_PUSH_OFF_AT_SETTINGS:
                Serialize(bb, (InAppMessageSetPushOffAtSettingsTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.INHOUSE_ADS_FOR_SEASON_PASS:
                Serialize(bb, (InAppMessageInhouseAdsForSeasonPassTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.INHOUSE_ADS_FOR_GEM_JACKPOT:
                Serialize(bb, (InAppMessageInhouseAdsForGemJackpotTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.CLOSE_VIDEO_ADS:
                Serialize(bb, (InAppMessageCloseVideoAdsTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.CLOSE_INHOUSE_ADS:
                Serialize(bb, (InAppMessageCloseInhouseAdsTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.INHOUSE_ADS_FOR_BOSS_RAIDERS:
                Serialize(bb, (InAppMessageInhouseAdsForBossRaidersTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.ENTER_CLUB_FROM_LOBBY_WITH_LEADER_PUSH_UNLOCKED:
                Serialize(bb, (InAppMessageEnterClubFromLobbyWithLeaderPushUnlockedTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.SET_PUSH_ON_AT_SETTINGS_TO_GET_REWARD:
                Serialize(bb, (InAppMessageSetPushOnAtSettingsToGetRewardTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.SET_PUSH_ON_AT_DEVICE_TO_GET_REWARD:
                Serialize(bb, (InAppMessageSetPushOnAtDeviceToGetRewardTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.SET_PUSH_ON_TO_GET_REWARD:
                Serialize(bb, (InAppMessageSetPushOnToGetRewardTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.GEM_CHEST_SHOP:
                Serialize(bb, (InAppMessageGemChestShopTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.INHOUSE_ADS_FOR_CLUB_ARENA:
                Serialize(bb, (InAppMessageInhouseAdsForClubArenaTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.LOBBY_ATT_FROM_LOGIN:
                Serialize(bb, (InAppMessageLobbyAttFromLoginTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.INHOUSE_ADS_FOR_HIDDEN_UNIVERSE:
                Serialize(bb, (InAppMessageInhouseAdsForHiddenUniverseTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.INHOUSE_ADS_FOR_INBOX:
                Serialize(bb, (InAppMessageInhouseAdsForInboxTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.ENTER_HIDDEN_UNIVERSE:
                Serialize(bb, (InAppMessageEnterHiddenUniverseTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.EXIT_HIDDEN_UNIVERSE:
                Serialize(bb, (InAppMessageExitHiddenUniverseTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.ENTER_HIDDEN_UNIVERSE_SHOP:
                Serialize(bb, (InAppMessageEnterHiddenUniverseShopTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.CLOSE_HIDDEN_UNIVERSE_SHOP:
                Serialize(bb, (InAppMessageCloseHiddenUniverseShopTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.TIER_UP:
                Serialize(bb, (InAppMessageTierUpTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.ENTER_VIP_LOUNGE:
                Serialize(bb, (InAppMessageEnterVipLoungeTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.CLOSE_VIP_LOUNGE:
                Serialize(bb, (InAppMessageCloseVipLoungeTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.INHOUSE_ADS_FOR_FREE_DEPOT:
                Serialize(bb, (InAppMessageInHouseAdsForFreeDepotTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.ANY_PURCHASE_CLICK_GET_BUTTON:
                Serialize(bb, (InAppMessageAnyPurchaseClickGetButtonTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.CLOSE_LEVEL_UP_DASH_MAIN:
                Serialize(bb, (InAppMessageCloseLevelUpDashMain)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.ENTER_EARNING_POINT:
                Serialize(bb, (InAppMessageEnterEarningPointTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.CLOSE_EARNING_POINT:
                Serialize(bb, (InAppMessageCloseEarningPointTrigger)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.CLOSE_CLUB:
                Serialize(bb, (InAppMessageCloseClub)inAppMessageTrigger.data);
                break;

            case InAppMessageTriggerType.CLOSE_FRIEND:
                Serialize(bb, (InAppMessageCloseFriend)inAppMessageTrigger.data);
                break;
            }
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "data");
        }
    }

    public static void Serialize(IBlackboard bb, InAppMessageVideoAdsPopup inAppMessageVideoAdsPopup)
    {
        if (inAppMessageVideoAdsPopup == null) { return; }
        if (inAppMessageVideoAdsPopup.reward != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "reward"), inAppMessageVideoAdsPopup.reward);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "reward");
        }
    }

    public static void Serialize(IBlackboard bb, InboxAcceptAllRequest inboxAcceptAllRequest)
    {
        if (inboxAcceptAllRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", inboxAcceptAllRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", inboxAcceptAllRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "currentTime", inboxAcceptAllRequest.currentTime);
        BlackboardUtils.SetOrCreateValue(bb, "timezoneOffset", inboxAcceptAllRequest.timezoneOffset);
        BlackboardUtils.SetOrCreateValue(bb, "maxInboxId", inboxAcceptAllRequest.maxInboxId);
    }

    public static void Serialize(IBlackboard bb, InboxAcceptAllResponse inboxAcceptAllResponse)
    {
        if (inboxAcceptAllResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", inboxAcceptAllResponse.error);
        if (inboxAcceptAllResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), inboxAcceptAllResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", inboxAcceptAllResponse.serverTime);
        if (inboxAcceptAllResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), inboxAcceptAllResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
    }

    public static void Serialize(IBlackboard bb, InboxAcceptRequest inboxAcceptRequest)
    {
        if (inboxAcceptRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", inboxAcceptRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "inboxId", inboxAcceptRequest.inboxId);
        BlackboardUtils.SetOrCreateValue(bb, "currentTime", inboxAcceptRequest.currentTime);
        BlackboardUtils.SetOrCreateValue(bb, "timezoneOffset", inboxAcceptRequest.timezoneOffset);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", inboxAcceptRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "contextId", inboxAcceptRequest.contextId);
    }

    public static void Serialize(IBlackboard bb, InboxAcceptResponse inboxAcceptResponse)
    {
        if (inboxAcceptResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", inboxAcceptResponse.error);
        if (inboxAcceptResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), inboxAcceptResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", inboxAcceptResponse.serverTime);
        if (inboxAcceptResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), inboxAcceptResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "inboxType", inboxAcceptResponse.inboxType);
        if (inboxAcceptResponse.inboxResult != null)
        {
            switch (inboxAcceptResponse.inboxType)
            {
            case InboxTypes.REWARD:
                Serialize(bb, (RewardResult)inboxAcceptResponse.inboxResult);
                break;

            case InboxTypes.MESSAGE:
                Serialize(bb, (InboxResultMessage)inboxAcceptResponse.inboxResult);
                break;

            case InboxTypes.MESSAGE_WARNING:
                Serialize(bb, (InboxResultMessageWarning)inboxAcceptResponse.inboxResult);
                break;

            case InboxTypes.GAME_COMPENSATION:
                Serialize(bb, (InboxResultGameCompensation)inboxAcceptResponse.inboxResult);
                break;

            case InboxTypes.FACEBOOK_FRIEND_CONNECT:
                Serialize(bb, (InboxResultFacebookFriendConnect)inboxAcceptResponse.inboxResult);
                break;

            case InboxTypes.FACEBOOK_SHARE:
                Serialize(bb, (InboxResultFacebookShare)inboxAcceptResponse.inboxResult);
                break;

            case InboxTypes.TOURNAMENT_WIN:
                Serialize(bb, (InboxResultTournamentWin)inboxAcceptResponse.inboxResult);
                break;

            case InboxTypes.TICKETED_BONUS_TICKET:
                Serialize(bb, (InboxResultTicktedBonusTicket)inboxAcceptResponse.inboxResult);
                break;

            case InboxTypes.SOCIAL_CREDIT:
                Serialize(bb, (InboxResultSocialCredit)inboxAcceptResponse.inboxResult);
                break;

            case InboxTypes.SPIN_DEAL:
                Serialize(bb, (InboxResultSpinDeal)inboxAcceptResponse.inboxResult);
                break;

            case InboxTypes.HOG_DEAL:
                Serialize(bb, (InboxResultHogDeal)inboxAcceptResponse.inboxResult);
                break;

            case InboxTypes.SPIN_DEAL_V2:
                Serialize(bb, (InboxResultSpinDealV2)inboxAcceptResponse.inboxResult);
                break;

            case InboxTypes.TICKETED_BONUS_TICKET_FOR_BOOST:
                Serialize(bb, (InboxResultTicketedBonusTicketForBoost)inboxAcceptResponse.inboxResult);
                break;

            case InboxTypes.BOSS_RAIDERS_DEAL:
                Serialize(bb, (InboxResultBossRaidersDeal)inboxAcceptResponse.inboxResult);
                break;
            }
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "inboxResult");
        }
        BlackboardUtils.SetOrCreateValue(bb, "inboxCount", inboxAcceptResponse.inboxCount);
    }

    public static void Serialize(IBlackboard bb, InboxBannerInfo inboxBannerInfo)
    {
        if (inboxBannerInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "id", inboxBannerInfo.id);
        BlackboardUtils.SetOrCreateValue(bb, "title", inboxBannerInfo.title);
        BlackboardUtils.SetOrCreateValue(bb, "message", inboxBannerInfo.message);
        BlackboardUtils.SetOrCreateValue(bb, "createTimestamp", inboxBannerInfo.createTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "expireTimestamp", inboxBannerInfo.expireTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "inAppMessageId", inboxBannerInfo.inAppMessageId);
        BlackboardUtils.SetOrCreateValue(bb, "inboxBannerType", inboxBannerInfo.inboxBannerType);
        BlackboardUtils.SetOrCreateValue(bb, "rewardCauseType", inboxBannerInfo.rewardCauseType);
        BlackboardUtils.SetOrCreateValue(bb, "imageUrl", inboxBannerInfo.imageUrl);
        BlackboardUtils.SetOrCreateValue(bb, "gameId", inboxBannerInfo.gameId);
    }

    public static void Serialize(IBlackboard bb, InboxExtraBossRaidersDeal inboxExtraBossRaidersDeal)
    {
        if (inboxExtraBossRaidersDeal == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "themeId", inboxExtraBossRaidersDeal.themeId);
        BlackboardUtils.SetOrCreateValue(bb, "spinPerDeal", inboxExtraBossRaidersDeal.spinPerDeal);
        BlackboardUtils.SetOrCreateValue(bb, "groupId", inboxExtraBossRaidersDeal.groupId);
    }

    public static void Serialize(IBlackboard bb, InboxExtraDataFacebookShare inboxExtraDataFacebookShare)
    {
        if (inboxExtraDataFacebookShare == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "credit", inboxExtraDataFacebookShare.credit);
        BlackboardUtils.SetOrCreateValue(bb, "rp", inboxExtraDataFacebookShare.rp);
    }

    public static void Serialize(IBlackboard bb, InboxExtraDataFriendConnect inboxExtraDataFriendConnect)
    {
        if (inboxExtraDataFriendConnect == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "credit", inboxExtraDataFriendConnect.credit);
        BlackboardUtils.SetOrCreateValue(bb, "userName", inboxExtraDataFriendConnect.userName);
    }

    public static void Serialize(IBlackboard bb, InboxExtraDataGameCompensation inboxExtraDataGameCompensation)
    {
        if (inboxExtraDataGameCompensation == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", inboxExtraDataGameCompensation.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "credit", inboxExtraDataGameCompensation.credit);
        BlackboardUtils.SetOrCreateValue(bb, "compensationType", inboxExtraDataGameCompensation.compensationType);
    }

    public static void Serialize(IBlackboard bb, InboxExtraDataMessage inboxExtraDataMessage)
    {
        if (inboxExtraDataMessage == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InboxExtraDataMessageWarning inboxExtraDataMessageWarning)
    {
        if (inboxExtraDataMessageWarning == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InboxExtraDataReward inboxExtraDataReward)
    {
        if (inboxExtraDataReward == null) { return; }
        if (inboxExtraDataReward.reward != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "reward"), inboxExtraDataReward.reward);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "reward");
        }
        BlackboardUtils.SetOrCreateValue(bb, "isDailyDelivery", inboxExtraDataReward.isDailyDelivery);
        BlackboardUtils.SetOrCreateValue(bb, "imageUrl", inboxExtraDataReward.imageUrl);
        BlackboardUtils.SetOrCreateValue(bb, "groupId", inboxExtraDataReward.groupId);
    }

    public static void Serialize(IBlackboard bb, InboxExtraHogDeal inboxExtraHogDeal)
    {
        if (inboxExtraHogDeal == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "hogDealPresetId", inboxExtraHogDeal.hogDealPresetId);
        BlackboardUtils.SetOrCreateValue(bb, "groupId", inboxExtraHogDeal.groupId);
        BlackboardUtils.SetOrCreateValue(bb, "levelMultiplierNumerator", inboxExtraHogDeal.levelMultiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, InboxExtraSocialCredit inboxExtraSocialCredit)
    {
        if (inboxExtraSocialCredit == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "credit", inboxExtraSocialCredit.credit);
        BlackboardUtils.SetOrCreateValue(bb, "profileUrl", inboxExtraSocialCredit.profileUrl);
        BlackboardUtils.SetOrCreateValue(bb, "applyTierMultiplier", inboxExtraSocialCredit.applyTierMultiplier);
    }

    public static void Serialize(IBlackboard bb, InboxExtraSpinDeal inboxExtraSpinDeal)
    {
        if (inboxExtraSpinDeal == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", inboxExtraSpinDeal.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "spinDealId", inboxExtraSpinDeal.spinDealId);
        BlackboardUtils.SetOrCreateValue(bb, "baseBet", inboxExtraSpinDeal.baseBet);
        BlackboardUtils.SetOrCreateValue(bb, "spinCount", inboxExtraSpinDeal.spinCount);
        BlackboardUtils.SetOrCreateValue(bb, "totalBet", inboxExtraSpinDeal.totalBet);
        BlackboardUtils.SetOrCreateValue(bb, "isBoosted", inboxExtraSpinDeal.isBoosted);
        BlackboardUtils.SetOrCreateValue(bb, "multiplier", inboxExtraSpinDeal.multiplier);
    }

    public static void Serialize(IBlackboard bb, InboxExtraSpinDealV2 inboxExtraSpinDealV2)
    {
        if (inboxExtraSpinDealV2 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", inboxExtraSpinDealV2.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "spinDealId", inboxExtraSpinDealV2.spinDealId);
        BlackboardUtils.SetOrCreateValue(bb, "baseBet", inboxExtraSpinDealV2.baseBet);
        BlackboardUtils.SetOrCreateValue(bb, "spinCount", inboxExtraSpinDealV2.spinCount);
        BlackboardUtils.SetOrCreateValue(bb, "totalBet", inboxExtraSpinDealV2.totalBet);
        BlackboardUtils.SetOrCreateValue(bb, "isBoosted", inboxExtraSpinDealV2.isBoosted);
        BlackboardUtils.SetOrCreateValue(bb, "multiplier", inboxExtraSpinDealV2.multiplier);
    }

    public static void Serialize(IBlackboard bb, InboxExtraTicketedBonusTicketForBoost inboxExtraTicketedBonusTicketForBoost)
    {
        if (inboxExtraTicketedBonusTicketForBoost == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "ticketId", inboxExtraTicketedBonusTicketForBoost.ticketId);
        if (inboxExtraTicketedBonusTicketForBoost.reward != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "reward"), inboxExtraTicketedBonusTicketForBoost.reward);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "reward");
        }
    }

    public static void Serialize(IBlackboard bb, InboxExtraTicktedBonusTicket inboxExtraTicktedBonusTicket)
    {
        if (inboxExtraTicktedBonusTicket == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", inboxExtraTicktedBonusTicket.gameId);
    }

    public static void Serialize(IBlackboard bb, InboxExtraTournamentWin inboxExtraTournamentWin)
    {
        if (inboxExtraTournamentWin == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "basicWinCredit", inboxExtraTournamentWin.basicWinCredit);
        BlackboardUtils.SetOrCreateValue(bb, "actualWinCredit", inboxExtraTournamentWin.actualWinCredit);
        BlackboardUtils.SetOrCreateValue(bb, "serialWinCount", inboxExtraTournamentWin.serialWinCount);
        BlackboardUtils.SetOrCreateValue(bb, "rank", inboxExtraTournamentWin.rank);
        BlackboardUtils.SetOrCreateValue(bb, "serialWinBonus", inboxExtraTournamentWin.serialWinBonus);
        BlackboardUtils.SetOrCreateValue(bb, "tournamentId", inboxExtraTournamentWin.tournamentId);
    }

    public static void Serialize(IBlackboard bb, InboxInfo inboxInfo)
    {
        if (inboxInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "id", inboxInfo.id);
        BlackboardUtils.SetOrCreateValue(bb, "title", inboxInfo.title);
        BlackboardUtils.SetOrCreateValue(bb, "message", inboxInfo.message);
        BlackboardUtils.SetOrCreateValue(bb, "type", inboxInfo.type);
        BlackboardUtils.SetOrCreateValue(bb, "createTimestamp", inboxInfo.createTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "expireTimestamp", inboxInfo.expireTimestamp);
        if (inboxInfo.extraData != null)
        {
            switch (inboxInfo.type)
            {
            case InboxTypes.REWARD:
                Serialize(bb, (InboxExtraDataReward)inboxInfo.extraData);
                break;

            case InboxTypes.MESSAGE:
                Serialize(bb, (InboxExtraDataMessage)inboxInfo.extraData);
                break;

            case InboxTypes.MESSAGE_WARNING:
                Serialize(bb, (InboxExtraDataMessageWarning)inboxInfo.extraData);
                break;

            case InboxTypes.GAME_COMPENSATION:
                Serialize(bb, (InboxExtraDataGameCompensation)inboxInfo.extraData);
                break;

            case InboxTypes.FACEBOOK_FRIEND_CONNECT:
                Serialize(bb, (InboxExtraDataFriendConnect)inboxInfo.extraData);
                break;

            case InboxTypes.FACEBOOK_SHARE:
                Serialize(bb, (InboxExtraDataFacebookShare)inboxInfo.extraData);
                break;

            case InboxTypes.TOURNAMENT_WIN:
                Serialize(bb, (InboxExtraTournamentWin)inboxInfo.extraData);
                break;

            case InboxTypes.TICKETED_BONUS_TICKET:
                Serialize(bb, (InboxExtraTicktedBonusTicket)inboxInfo.extraData);
                break;

            case InboxTypes.SOCIAL_CREDIT:
                Serialize(bb, (InboxExtraSocialCredit)inboxInfo.extraData);
                break;

            case InboxTypes.SPIN_DEAL:
                Serialize(bb, (InboxExtraSpinDeal)inboxInfo.extraData);
                break;

            case InboxTypes.HOG_DEAL:
                Serialize(bb, (InboxExtraHogDeal)inboxInfo.extraData);
                break;

            case InboxTypes.SPIN_DEAL_V2:
                Serialize(bb, (InboxExtraSpinDealV2)inboxInfo.extraData);
                break;

            case InboxTypes.TICKETED_BONUS_TICKET_FOR_BOOST:
                Serialize(bb, (InboxExtraTicketedBonusTicketForBoost)inboxInfo.extraData);
                break;

            case InboxTypes.BOSS_RAIDERS_DEAL:
                Serialize(bb, (InboxExtraBossRaidersDeal)inboxInfo.extraData);
                break;
            }
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "extraData");
        }
        BlackboardUtils.SetOrCreateValue(bb, "rewardCauseType", inboxInfo.rewardCauseType);
        BlackboardUtils.SetOrCreateValue(bb, "eventId", inboxInfo.eventId);
        BlackboardUtils.SetOrCreateValue(bb, "couponCode", inboxInfo.couponCode);
    }

    public static void Serialize(IBlackboard bb, InboxListResponse inboxListResponse)
    {
        if (inboxListResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", inboxListResponse.error);
        if (inboxListResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), inboxListResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", inboxListResponse.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "inboxList", inboxListResponse.inboxList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "collectAllEnabled", inboxListResponse.collectAllEnabled);
        BlackboardUtils.SetOrCreateList(bb, "inboxBannerList", inboxListResponse.inboxBannerList, Serialize);
        if (inboxListResponse.userBucksInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userBucksInfo"), inboxListResponse.userBucksInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userBucksInfo");
        }
    }

    public static void Serialize(IBlackboard bb, InboxResultBossRaidersDeal inboxResultBossRaidersDeal)
    {
        if (inboxResultBossRaidersDeal == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "dealUuid", inboxResultBossRaidersDeal.dealUuid);
        BlackboardUtils.SetOrCreateValue(bb, "themeId", inboxResultBossRaidersDeal.themeId);
        BlackboardUtils.SetOrCreateValue(bb, "spinCount", inboxResultBossRaidersDeal.spinCount);
    }

    public static void Serialize(IBlackboard bb, InboxResultFacebookFriendConnect inboxResultFacebookFriendConnect)
    {
        if (inboxResultFacebookFriendConnect == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "credit", inboxResultFacebookFriendConnect.credit);
    }

    public static void Serialize(IBlackboard bb, InboxResultFacebookShare inboxResultFacebookShare)
    {
        if (inboxResultFacebookShare == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "credit", inboxResultFacebookShare.credit);
        BlackboardUtils.SetOrCreateValue(bb, "rp", inboxResultFacebookShare.rp);
    }

    public static void Serialize(IBlackboard bb, InboxResultGameCompensation inboxResultGameCompensation)
    {
        if (inboxResultGameCompensation == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "credit", inboxResultGameCompensation.credit);
    }

    public static void Serialize(IBlackboard bb, InboxResultHogDeal inboxResultHogDeal)
    {
        if (inboxResultHogDeal == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "symbol", inboxResultHogDeal.symbol);
        BlackboardUtils.SetOrCreateValue(bb, "stage", inboxResultHogDeal.stage);
        BlackboardUtils.SetOrCreateList(bb, "comboMultiplier", inboxResultHogDeal.comboMultiplier, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "prizeList", inboxResultHogDeal.prizeList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "objectList", inboxResultHogDeal.objectList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "hintCooltime", inboxResultHogDeal.hintCooltime);
        BlackboardUtils.SetOrCreateValue(bb, "comboDuration", inboxResultHogDeal.comboDuration);
    }

    public static void Serialize(IBlackboard bb, InboxResultHogDealComboMultiplier inboxResultHogDealComboMultiplier)
    {
        if (inboxResultHogDealComboMultiplier == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "comboCount", inboxResultHogDealComboMultiplier.comboCount);
        BlackboardUtils.SetOrCreateValue(bb, "multiplierNumerator", inboxResultHogDealComboMultiplier.multiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, InboxResultHogDealObject inboxResultHogDealObject)
    {
        if (inboxResultHogDealObject == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "index", inboxResultHogDealObject.index);
        BlackboardUtils.SetOrCreateValue(bb, "name", inboxResultHogDealObject.name);
    }

    public static void Serialize(IBlackboard bb, InboxResultHogDealPrize inboxResultHogDealPrize)
    {
        if (inboxResultHogDealPrize == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "type", inboxResultHogDealPrize.type);
        BlackboardUtils.SetOrCreateValue(bb, "credit", inboxResultHogDealPrize.credit);
    }

    public static void Serialize(IBlackboard bb, InboxResultMessage inboxResultMessage)
    {
        if (inboxResultMessage == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InboxResultMessageWarning inboxResultMessageWarning)
    {
        if (inboxResultMessageWarning == null) { return; }
    }

    public static void Serialize(IBlackboard bb, InboxResultSocialCredit inboxResultSocialCredit)
    {
        if (inboxResultSocialCredit == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "credit", inboxResultSocialCredit.credit);
        BlackboardUtils.SetOrCreateValue(bb, "profileUrl", inboxResultSocialCredit.profileUrl);
        BlackboardUtils.SetOrCreateValue(bb, "applyTierMultiplier", inboxResultSocialCredit.applyTierMultiplier);
    }

    public static void Serialize(IBlackboard bb, InboxResultSpinDeal inboxResultSpinDeal)
    {
        if (inboxResultSpinDeal == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", inboxResultSpinDeal.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "spinDealId", inboxResultSpinDeal.spinDealId);
        BlackboardUtils.SetOrCreateValue(bb, "betList", inboxResultSpinDeal.betList);
        BlackboardUtils.SetOrCreateValue(bb, "spinCountList", inboxResultSpinDeal.spinCountList);
    }

    public static void Serialize(IBlackboard bb, InboxResultSpinDealV2 inboxResultSpinDealV2)
    {
        if (inboxResultSpinDealV2 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", inboxResultSpinDealV2.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "spinDealId", inboxResultSpinDealV2.spinDealId);
        BlackboardUtils.SetOrCreateValue(bb, "betList", inboxResultSpinDealV2.betList);
        BlackboardUtils.SetOrCreateValue(bb, "spinCountList", inboxResultSpinDealV2.spinCountList);
        BlackboardUtils.SetOrCreateValue(bb, "extraSpinCountList", inboxResultSpinDealV2.extraSpinCountList);
        BlackboardUtils.SetOrCreateValue(bb, "moreTagList", inboxResultSpinDealV2.moreTagList);
    }

    public static void Serialize(IBlackboard bb, InboxResultTicketedBonusTicketForBoost inboxResultTicketedBonusTicketForBoost)
    {
        if (inboxResultTicketedBonusTicketForBoost == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", inboxResultTicketedBonusTicketForBoost.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "ticketId", inboxResultTicketedBonusTicketForBoost.ticketId);
    }

    public static void Serialize(IBlackboard bb, InboxResultTicktedBonusTicket inboxResultTicktedBonusTicket)
    {
        if (inboxResultTicktedBonusTicket == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", inboxResultTicktedBonusTicket.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "ticketId", inboxResultTicktedBonusTicket.ticketId);
    }

    public static void Serialize(IBlackboard bb, InboxResultTournamentWin inboxResultTournamentWin)
    {
        if (inboxResultTournamentWin == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "basicWinCredit", inboxResultTournamentWin.basicWinCredit);
        BlackboardUtils.SetOrCreateValue(bb, "actualWinCredit", inboxResultTournamentWin.actualWinCredit);
        BlackboardUtils.SetOrCreateValue(bb, "serialWinCount", inboxResultTournamentWin.serialWinCount);
        BlackboardUtils.SetOrCreateValue(bb, "rank", inboxResultTournamentWin.rank);
        BlackboardUtils.SetOrCreateValue(bb, "serialWinBonus", inboxResultTournamentWin.serialWinBonus);
        BlackboardUtils.SetOrCreateValue(bb, "tournamentId", inboxResultTournamentWin.tournamentId);
    }

    public static void Serialize(IBlackboard bb, InviteInstallInfo inviteInstallInfo)
    {
        if (inviteInstallInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "snsUrl", inviteInstallInfo.snsUrl);
        BlackboardUtils.SetOrCreateValue(bb, "snsRewardCredit", inviteInstallInfo.snsRewardCredit);
        BlackboardUtils.SetOrCreateValue(bb, "fbMessageUrl", inviteInstallInfo.fbMessageUrl);
        BlackboardUtils.SetOrCreateValue(bb, "fbMessageRewardCredit", inviteInstallInfo.fbMessageRewardCredit);
        BlackboardUtils.SetOrCreateValue(bb, "shareMessage", inviteInstallInfo.shareMessage);
    }

    public static void Serialize(IBlackboard bb, InviteInstallRequest inviteInstallRequest)
    {
        if (inviteInstallRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", inviteInstallRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", inviteInstallRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "inviterUserId", inviteInstallRequest.inviterUserId);
        BlackboardUtils.SetOrCreateValue(bb, "inviteType", inviteInstallRequest.inviteType);
        BlackboardUtils.SetOrCreateValue(bb, "contextId", inviteInstallRequest.contextId);
    }

    public static void Serialize(IBlackboard bb, InviteInstallWithTierRequest inviteInstallWithTierRequest)
    {
        if (inviteInstallWithTierRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", inviteInstallWithTierRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", inviteInstallWithTierRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "inviterUserId", inviteInstallWithTierRequest.inviterUserId);
        BlackboardUtils.SetOrCreateValue(bb, "inviteType", inviteInstallWithTierRequest.inviteType);
        BlackboardUtils.SetOrCreateValue(bb, "contextId", inviteInstallWithTierRequest.contextId);
        BlackboardUtils.SetOrCreateValue(bb, "inviteInstallWithTierId", inviteInstallWithTierRequest.inviteInstallWithTierId);
    }

    public static void Serialize(IBlackboard bb, InviteInstallWithTierResponse inviteInstallWithTierResponse)
    {
        if (inviteInstallWithTierResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", inviteInstallWithTierResponse.error);
        if (inviteInstallWithTierResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), inviteInstallWithTierResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", inviteInstallWithTierResponse.serverTime);
        if (inviteInstallWithTierResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), inviteInstallWithTierResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
    }

    public static void Serialize(IBlackboard bb, Item item)
    {
        if (item == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "id", item.id);
        BlackboardUtils.SetOrCreateValue(bb, "name", item.name);
        BlackboardUtils.SetOrCreateValue(bb, "itemType", item.itemType);
        if (item.itemDetailInfo != null)
        {
            switch (item.itemType)
            {
            case ItemType.CREDIT:
                Serialize(bb, (ItemDetailInfoCredit)item.itemDetailInfo);
                break;

            case ItemType.DAILY_BOOST:
                Serialize(bb, (ItemDetailInfoDailyBoost)item.itemDetailInfo);
                break;

            case ItemType.TIER_BOOST:
                Serialize(bb, (ItemDetailInfoTierBoost)item.itemDetailInfo);
                break;

            case ItemType.CREDIT_POT_OF_GOLD:
                Serialize(bb, (ItemDetailInfoCreditPotOfGold)item.itemDetailInfo);
                break;

            case ItemType.CREDIT_MULTIPLIER_WHEEL:
                Serialize(bb, (ItemDetailInfoCreditMultiplierWheel)item.itemDetailInfo);
                break;

            case ItemType.DAILY_BONUS_WHEEL:
                Serialize(bb, (ItemDetailInfoDailyBonusWheel)item.itemDetailInfo);
                break;

            case ItemType.PIGGY_BANK:
                Serialize(bb, (ItemDetailInfoPiggyBank)item.itemDetailInfo);
                break;

            case ItemType.EARLY_ACCESS:
                Serialize(bb, (ItemDetailInfoEarlyAccess)item.itemDetailInfo);
                break;

            case ItemType.CREDIT_WHEEL:
                Serialize(bb, (ItemDetailInfoCreditWheel)item.itemDetailInfo);
                break;

            case ItemType.TICKETED_BONUS_TICKET:
                Serialize(bb, (ItemDetailInfoTicketedBonusTicket)item.itemDetailInfo);
                break;

            case ItemType.DAILY_MEGA_WHEEL:
                Serialize(bb, (ItemDetailInfoDailyMegaWheel)item.itemDetailInfo);
                break;

            case ItemType.POG_BOOSTER:
                Serialize(bb, (ItemDetailInfoPogBooster)item.itemDetailInfo);
                break;

            case ItemType.GEM:
                Serialize(bb, (ItemDetailInfoGem)item.itemDetailInfo);
                break;

            case ItemType.GEM_BOOSTER:
                Serialize(bb, (ItemDetailInfoGemBooster)item.itemDetailInfo);
                break;

            case ItemType.EPIC_PASS:
                Serialize(bb, (ItemDetailInfoEpicPass)item.itemDetailInfo);
                break;

            case ItemType.SPIN_BOOST:
                Serialize(bb, (ItemDetailInfoSpinBoost)item.itemDetailInfo);
                break;

            case ItemType.HIDDEN_UNIVERSE_FINDER:
                Serialize(bb, (ItemDetailInfoHiddenUniverseFinder)item.itemDetailInfo);
                break;

            case ItemType.TICKETED_BONUS_BOOSTER:
                Serialize(bb, (ItemDetailInfoTicketedBonusBooster)item.itemDetailInfo);
                break;

            case ItemType.EPIC_PASS_V2:
                Serialize(bb, (ItemDetailInfoEpicPass)item.itemDetailInfo);
                break;
            }
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "itemDetailInfo");
        }
    }

    public static void Serialize(IBlackboard bb, ItemDetailInfoCredit itemDetailInfoCredit)
    {
        if (itemDetailInfoCredit == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "baseCredit", itemDetailInfoCredit.baseCredit);
        BlackboardUtils.SetOrCreateValue(bb, "rp", itemDetailInfoCredit.rp);
        BlackboardUtils.SetOrCreateValue(bb, "additionalCreditMultiplierNumerator", itemDetailInfoCredit.additionalCreditMultiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, ItemDetailInfoCreditMultiplierWheel itemDetailInfoCreditMultiplierWheel)
    {
        if (itemDetailInfoCreditMultiplierWheel == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "rp", itemDetailInfoCreditMultiplierWheel.rp);
        BlackboardUtils.SetOrCreateList(bb, "setting", itemDetailInfoCreditMultiplierWheel.setting, Serialize);
    }

    public static void Serialize(IBlackboard bb, ItemDetailInfoCreditMultiplierWheelSetting itemDetailInfoCreditMultiplierWheelSetting)
    {
        if (itemDetailInfoCreditMultiplierWheelSetting == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "weight", itemDetailInfoCreditMultiplierWheelSetting.weight);
        BlackboardUtils.SetOrCreateValue(bb, "multiplierNumerator", itemDetailInfoCreditMultiplierWheelSetting.multiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, ItemDetailInfoCreditPotOfGold itemDetailInfoCreditPotOfGold)
    {
        if (itemDetailInfoCreditPotOfGold == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "baseCredit", itemDetailInfoCreditPotOfGold.baseCredit);
        BlackboardUtils.SetOrCreateValue(bb, "rp", itemDetailInfoCreditPotOfGold.rp);
    }

    public static void Serialize(IBlackboard bb, ItemDetailInfoCreditWheel itemDetailInfoCreditWheel)
    {
        if (itemDetailInfoCreditWheel == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "rp", itemDetailInfoCreditWheel.rp);
        BlackboardUtils.SetOrCreateList(bb, "setting", itemDetailInfoCreditWheel.setting, Serialize);
    }

    public static void Serialize(IBlackboard bb, ItemDetailInfoCreditWheelSetting itemDetailInfoCreditWheelSetting)
    {
        if (itemDetailInfoCreditWheelSetting == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "credit", itemDetailInfoCreditWheelSetting.credit);
        BlackboardUtils.SetOrCreateValue(bb, "weight", itemDetailInfoCreditWheelSetting.weight);
    }

    public static void Serialize(IBlackboard bb, ItemDetailInfoDailyBonusWheel itemDetailInfoDailyBonusWheel)
    {
        if (itemDetailInfoDailyBonusWheel == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "spinCount", itemDetailInfoDailyBonusWheel.spinCount);
        BlackboardUtils.SetOrCreateValue(bb, "rp", itemDetailInfoDailyBonusWheel.rp);
    }

    public static void Serialize(IBlackboard bb, ItemDetailInfoDailyBoost itemDetailInfoDailyBoost)
    {
        if (itemDetailInfoDailyBoost == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "baseCreditPerDay", itemDetailInfoDailyBoost.baseCreditPerDay);
        BlackboardUtils.SetOrCreateValue(bb, "rp", itemDetailInfoDailyBoost.rp);
        BlackboardUtils.SetOrCreateValue(bb, "totalDayCount", itemDetailInfoDailyBoost.totalDayCount);
        BlackboardUtils.SetOrCreateValue(bb, "baseGemPerDay", itemDetailInfoDailyBoost.baseGemPerDay);
    }

    public static void Serialize(IBlackboard bb, ItemDetailInfoDailyMegaWheel itemDetailInfoDailyMegaWheel)
    {
        if (itemDetailInfoDailyMegaWheel == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "rp", itemDetailInfoDailyMegaWheel.rp);
        BlackboardUtils.SetOrCreateValue(bb, "spinCount", itemDetailInfoDailyMegaWheel.spinCount);
    }

    public static void Serialize(IBlackboard bb, ItemDetailInfoEarlyAccess itemDetailInfoEarlyAccess)
    {
        if (itemDetailInfoEarlyAccess == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "grade", itemDetailInfoEarlyAccess.grade);
    }

    public static void Serialize(IBlackboard bb, ItemDetailInfoEpicPass itemDetailInfoEpicPass)
    {
        if (itemDetailInfoEpicPass == null) { return; }
    }

    public static void Serialize(IBlackboard bb, ItemDetailInfoGem itemDetailInfoGem)
    {
        if (itemDetailInfoGem == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gem", itemDetailInfoGem.gem);
        BlackboardUtils.SetOrCreateValue(bb, "rp", itemDetailInfoGem.rp);
    }

    public static void Serialize(IBlackboard bb, ItemDetailInfoGemBooster itemDetailInfoGemBooster)
    {
        if (itemDetailInfoGemBooster == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "rp", itemDetailInfoGemBooster.rp);
        BlackboardUtils.SetOrCreateList(bb, "setting", itemDetailInfoGemBooster.setting, Serialize);
    }

    public static void Serialize(IBlackboard bb, ItemDetailInfoGemBoosterSetting itemDetailInfoGemBoosterSetting)
    {
        if (itemDetailInfoGemBoosterSetting == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "weight", itemDetailInfoGemBoosterSetting.weight);
        BlackboardUtils.SetOrCreateValue(bb, "multiplierNumerator", itemDetailInfoGemBoosterSetting.multiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, ItemDetailInfoHiddenUniverseFinder itemDetailInfoHiddenUniverseFinder)
    {
        if (itemDetailInfoHiddenUniverseFinder == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "finder", itemDetailInfoHiddenUniverseFinder.finder);
    }

    public static void Serialize(IBlackboard bb, ItemDetailInfoPiggyBank itemDetailInfoPiggyBank)
    {
        if (itemDetailInfoPiggyBank == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "minCredit", itemDetailInfoPiggyBank.minCredit);
        BlackboardUtils.SetOrCreateValue(bb, "maxCredit", itemDetailInfoPiggyBank.maxCredit);
        BlackboardUtils.SetOrCreateList(bb, "contributionList", itemDetailInfoPiggyBank.contributionList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "rp", itemDetailInfoPiggyBank.rp);
    }

    public static void Serialize(IBlackboard bb, ItemDetailInfoPogBooster itemDetailInfoPogBooster)
    {
        if (itemDetailInfoPogBooster == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "rp", itemDetailInfoPogBooster.rp);
        BlackboardUtils.SetOrCreateList(bb, "setting", itemDetailInfoPogBooster.setting, Serialize);
    }

    public static void Serialize(IBlackboard bb, ItemDetailInfoPogBoosterSetting itemDetailInfoPogBoosterSetting)
    {
        if (itemDetailInfoPogBoosterSetting == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "weight", itemDetailInfoPogBoosterSetting.weight);
        BlackboardUtils.SetOrCreateValue(bb, "multiplierNumerator", itemDetailInfoPogBoosterSetting.multiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, ItemDetailInfoSpinBoost itemDetailInfoSpinBoost)
    {
        if (itemDetailInfoSpinBoost == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "rp", itemDetailInfoSpinBoost.rp);
        BlackboardUtils.SetOrCreateList(bb, "setting", itemDetailInfoSpinBoost.setting, Serialize);
    }

    public static void Serialize(IBlackboard bb, ItemDetailInfoSpinBoostSetting itemDetailInfoSpinBoostSetting)
    {
        if (itemDetailInfoSpinBoostSetting == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "multiplierNumerator", itemDetailInfoSpinBoostSetting.multiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, ItemDetailInfoTicketedBonusBooster itemDetailInfoTicketedBonusBooster)
    {
        if (itemDetailInfoTicketedBonusBooster == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "rp", itemDetailInfoTicketedBonusBooster.rp);
        BlackboardUtils.SetOrCreateList(bb, "setting", itemDetailInfoTicketedBonusBooster.setting, Serialize);
    }

    public static void Serialize(IBlackboard bb, ItemDetailInfoTicketedBonusBoosterSetting itemDetailInfoTicketedBonusBoosterSetting)
    {
        if (itemDetailInfoTicketedBonusBoosterSetting == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "multiplierNumerator", itemDetailInfoTicketedBonusBoosterSetting.multiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, ItemDetailInfoTicketedBonusTicket itemDetailInfoTicketedBonusTicket)
    {
        if (itemDetailInfoTicketedBonusTicket == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "bet", itemDetailInfoTicketedBonusTicket.bet);
        BlackboardUtils.SetOrCreateValue(bb, "extraBet", itemDetailInfoTicketedBonusTicket.extraBet);
        if (itemDetailInfoTicketedBonusTicket.extraData != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "extraData"), itemDetailInfoTicketedBonusTicket.extraData);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "extraData");
        }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", itemDetailInfoTicketedBonusTicket.gameId);
    }

    public static void Serialize(IBlackboard bb, ItemDetailInfoTicketedBonusTicketExtraData itemDetailInfoTicketedBonusTicketExtraData)
    {
        if (itemDetailInfoTicketedBonusTicketExtraData == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "freeSpinCount", itemDetailInfoTicketedBonusTicketExtraData.freeSpinCount);
    }

    public static void Serialize(IBlackboard bb, ItemDetailInfoTierBoost itemDetailInfoTierBoost)
    {
        if (itemDetailInfoTierBoost == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "baseCredit", itemDetailInfoTierBoost.baseCredit);
        BlackboardUtils.SetOrCreateValue(bb, "tier", itemDetailInfoTierBoost.tier);
        BlackboardUtils.SetOrCreateValue(bb, "needTier", itemDetailInfoTierBoost.needTier);
        BlackboardUtils.SetOrCreateValue(bb, "totalDayCount", itemDetailInfoTierBoost.totalDayCount);
    }

    public static void Serialize(IBlackboard bb, ItemResultCredit itemResultCredit)
    {
        if (itemResultCredit == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "earnCredit", itemResultCredit.earnCredit);
        BlackboardUtils.SetOrCreateValue(bb, "earnRp", itemResultCredit.earnRp);
        BlackboardUtils.SetOrCreateValue(bb, "rpMultiplier", itemResultCredit.rpMultiplier);
        BlackboardUtils.SetOrCreateValue(bb, "origEarnCredit", itemResultCredit.origEarnCredit);
        BlackboardUtils.SetOrCreateValue(bb, "eventMultiplierNumerator", itemResultCredit.eventMultiplierNumerator);
        BlackboardUtils.SetOrCreateValue(bb, "applyTierMultiplier", itemResultCredit.applyTierMultiplier);
        BlackboardUtils.SetOrCreateValue(bb, "levelMultiplierNumerator", itemResultCredit.levelMultiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, ItemResultCreditMultiplierWheel itemResultCreditMultiplierWheel)
    {
        if (itemResultCreditMultiplierWheel == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "earnCredit", itemResultCreditMultiplierWheel.earnCredit);
        BlackboardUtils.SetOrCreateValue(bb, "earnRp", itemResultCreditMultiplierWheel.earnRp);
        BlackboardUtils.SetOrCreateValue(bb, "rpMultiplier", itemResultCreditMultiplierWheel.rpMultiplier);
        BlackboardUtils.SetOrCreateValue(bb, "multiplierIndex", itemResultCreditMultiplierWheel.multiplierIndex);
        BlackboardUtils.SetOrCreateValue(bb, "multiplier", itemResultCreditMultiplierWheel.multiplier);
        BlackboardUtils.SetOrCreateValue(bb, "origEarnCredit", itemResultCreditMultiplierWheel.origEarnCredit);
        BlackboardUtils.SetOrCreateValue(bb, "origEarnRp", itemResultCreditMultiplierWheel.origEarnRp);
        BlackboardUtils.SetOrCreateValue(bb, "wheelSpinCount", itemResultCreditMultiplierWheel.wheelSpinCount);
    }

    public static void Serialize(IBlackboard bb, ItemResultCreditPotOfGold itemResultCreditPotOfGold)
    {
        if (itemResultCreditPotOfGold == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "earnCredit", itemResultCreditPotOfGold.earnCredit);
        BlackboardUtils.SetOrCreateValue(bb, "earnRp", itemResultCreditPotOfGold.earnRp);
        BlackboardUtils.SetOrCreateValue(bb, "rpMultiplier", itemResultCreditPotOfGold.rpMultiplier);
        BlackboardUtils.SetOrCreateValue(bb, "levelMultiplierNumerator", itemResultCreditPotOfGold.levelMultiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, ItemResultCreditWheel itemResultCreditWheel)
    {
        if (itemResultCreditWheel == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "index", itemResultCreditWheel.index);
        BlackboardUtils.SetOrCreateValue(bb, "rpMultiplier", itemResultCreditWheel.rpMultiplier);
        BlackboardUtils.SetOrCreateValue(bb, "earnCredit", itemResultCreditWheel.earnCredit);
        BlackboardUtils.SetOrCreateValue(bb, "earnRp", itemResultCreditWheel.earnRp);
        BlackboardUtils.SetOrCreateValue(bb, "origEarnCredit", itemResultCreditWheel.origEarnCredit);
        BlackboardUtils.SetOrCreateValue(bb, "origEarnRp", itemResultCreditWheel.origEarnRp);
        BlackboardUtils.SetOrCreateValue(bb, "levelMultiplierNumerator", itemResultCreditWheel.levelMultiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, ItemResultDailyBonusWheel itemResultDailyBonusWheel)
    {
        if (itemResultDailyBonusWheel == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "addedSpinCount", itemResultDailyBonusWheel.addedSpinCount);
        BlackboardUtils.SetOrCreateValue(bb, "totalSpinCount", itemResultDailyBonusWheel.totalSpinCount);
        BlackboardUtils.SetOrCreateValue(bb, "earnRp", itemResultDailyBonusWheel.earnRp);
        BlackboardUtils.SetOrCreateValue(bb, "rpMultiplier", itemResultDailyBonusWheel.rpMultiplier);
    }

    public static void Serialize(IBlackboard bb, ItemResultDailyBoost itemResultDailyBoost)
    {
        if (itemResultDailyBoost == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "earnCredit", itemResultDailyBoost.earnCredit);
        BlackboardUtils.SetOrCreateValue(bb, "earnRp", itemResultDailyBoost.earnRp);
        BlackboardUtils.SetOrCreateValue(bb, "rpMultiplier", itemResultDailyBoost.rpMultiplier);
        if (itemResultDailyBoost.dailyBoost != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "dailyBoost"), itemResultDailyBoost.dailyBoost);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "dailyBoost");
        }
        BlackboardUtils.SetOrCreateValue(bb, "levelMultiplierNumerator", itemResultDailyBoost.levelMultiplierNumerator);
        BlackboardUtils.SetOrCreateValue(bb, "earnGem", itemResultDailyBoost.earnGem);
    }

    public static void Serialize(IBlackboard bb, ItemResultDailyMegaWheel itemResultDailyMegaWheel)
    {
        if (itemResultDailyMegaWheel == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "earnRp", itemResultDailyMegaWheel.earnRp);
        BlackboardUtils.SetOrCreateValue(bb, "rpMultiplier", itemResultDailyMegaWheel.rpMultiplier);
        BlackboardUtils.SetOrCreateValue(bb, "addedSpinCount", itemResultDailyMegaWheel.addedSpinCount);
        BlackboardUtils.SetOrCreateValue(bb, "totalSpinCount", itemResultDailyMegaWheel.totalSpinCount);
    }

    public static void Serialize(IBlackboard bb, ItemResultEarlyAccess itemResultEarlyAccess)
    {
        if (itemResultEarlyAccess == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "grade", itemResultEarlyAccess.grade);
    }

    public static void Serialize(IBlackboard bb, ItemResultEpicPass itemResultEpicPass)
    {
        if (itemResultEpicPass == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "unclaimedRewardCount", itemResultEpicPass.unclaimedRewardCount);
    }

    public static void Serialize(IBlackboard bb, ItemResultGem itemResultGem)
    {
        if (itemResultGem == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "earnGem", itemResultGem.earnGem);
        BlackboardUtils.SetOrCreateValue(bb, "earnRp", itemResultGem.earnRp);
        BlackboardUtils.SetOrCreateValue(bb, "rpMultiplier", itemResultGem.rpMultiplier);
        BlackboardUtils.SetOrCreateValue(bb, "eventMultiplierNumerator", itemResultGem.eventMultiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, ItemResultGemBooster itemResultGemBooster)
    {
        if (itemResultGemBooster == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "earnGem", itemResultGemBooster.earnGem);
        BlackboardUtils.SetOrCreateValue(bb, "earnRp", itemResultGemBooster.earnRp);
        BlackboardUtils.SetOrCreateValue(bb, "rpMultiplier", itemResultGemBooster.rpMultiplier);
        BlackboardUtils.SetOrCreateValue(bb, "multiplierIndex", itemResultGemBooster.multiplierIndex);
        BlackboardUtils.SetOrCreateValue(bb, "multiplier", itemResultGemBooster.multiplier);
        BlackboardUtils.SetOrCreateValue(bb, "origEarnGem", itemResultGemBooster.origEarnGem);
        BlackboardUtils.SetOrCreateValue(bb, "origEarnRp", itemResultGemBooster.origEarnRp);
        BlackboardUtils.SetOrCreateValue(bb, "wheelSpinCount", itemResultGemBooster.wheelSpinCount);
    }

    public static void Serialize(IBlackboard bb, ItemResultHiddenUniverseFinder itemResultHiddenUniverseFinder)
    {
        if (itemResultHiddenUniverseFinder == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "earnFinder", itemResultHiddenUniverseFinder.earnFinder);
        BlackboardUtils.SetOrCreateValue(bb, "totalFinder", itemResultHiddenUniverseFinder.totalFinder);
    }

    public static void Serialize(IBlackboard bb, ItemResultPiggyBank itemResultPiggyBank)
    {
        if (itemResultPiggyBank == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "earnCredit", itemResultPiggyBank.earnCredit);
        BlackboardUtils.SetOrCreateValue(bb, "newPiggyLevel", itemResultPiggyBank.newPiggyLevel);
        BlackboardUtils.SetOrCreateValue(bb, "earnRp", itemResultPiggyBank.earnRp);
        BlackboardUtils.SetOrCreateValue(bb, "rpMultiplier", itemResultPiggyBank.rpMultiplier);
        BlackboardUtils.SetOrCreateList(bb, "newPiggyProductList", itemResultPiggyBank.newPiggyProductList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "multiplierNumerator", itemResultPiggyBank.multiplierNumerator);
        BlackboardUtils.SetOrCreateValue(bb, "levelMultiplierNumerator", itemResultPiggyBank.levelMultiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, ItemResultPogBooster itemResultPogBooster)
    {
        if (itemResultPogBooster == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "earnCredit", itemResultPogBooster.earnCredit);
        BlackboardUtils.SetOrCreateValue(bb, "earnRp", itemResultPogBooster.earnRp);
        BlackboardUtils.SetOrCreateValue(bb, "rpMultiplier", itemResultPogBooster.rpMultiplier);
        BlackboardUtils.SetOrCreateValue(bb, "multiplierIndex", itemResultPogBooster.multiplierIndex);
        BlackboardUtils.SetOrCreateValue(bb, "multiplier", itemResultPogBooster.multiplier);
        BlackboardUtils.SetOrCreateValue(bb, "origEarnCredit", itemResultPogBooster.origEarnCredit);
        BlackboardUtils.SetOrCreateValue(bb, "origEarnRp", itemResultPogBooster.origEarnRp);
        BlackboardUtils.SetOrCreateValue(bb, "boostCount", itemResultPogBooster.boostCount);
    }

    public static void Serialize(IBlackboard bb, ItemResultSpinBoost itemResultSpinBoost)
    {
        if (itemResultSpinBoost == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "earnCredit", itemResultSpinBoost.earnCredit);
        BlackboardUtils.SetOrCreateValue(bb, "earnRp", itemResultSpinBoost.earnRp);
        BlackboardUtils.SetOrCreateValue(bb, "rpMultiplier", itemResultSpinBoost.rpMultiplier);
        BlackboardUtils.SetOrCreateValue(bb, "origEarnCredit", itemResultSpinBoost.origEarnCredit);
        BlackboardUtils.SetOrCreateValue(bb, "origEarnRp", itemResultSpinBoost.origEarnRp);
        BlackboardUtils.SetOrCreateValue(bb, "multiplier", itemResultSpinBoost.multiplier);
        BlackboardUtils.SetOrCreateValue(bb, "multiplierIndex", itemResultSpinBoost.multiplierIndex);
        BlackboardUtils.SetOrCreateValue(bb, "multiplierNumerator", itemResultSpinBoost.multiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, ItemResultTicketedBonusBooster itemResultTicketedBonusBooster)
    {
        if (itemResultTicketedBonusBooster == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "earnCredit", itemResultTicketedBonusBooster.earnCredit);
        BlackboardUtils.SetOrCreateValue(bb, "earnRp", itemResultTicketedBonusBooster.earnRp);
        BlackboardUtils.SetOrCreateValue(bb, "rpMultiplier", itemResultTicketedBonusBooster.rpMultiplier);
        BlackboardUtils.SetOrCreateValue(bb, "origEarnCredit", itemResultTicketedBonusBooster.origEarnCredit);
        BlackboardUtils.SetOrCreateValue(bb, "origEarnRp", itemResultTicketedBonusBooster.origEarnRp);
        BlackboardUtils.SetOrCreateValue(bb, "multiplierIndex", itemResultTicketedBonusBooster.multiplierIndex);
        BlackboardUtils.SetOrCreateValue(bb, "multiplierNumerator", itemResultTicketedBonusBooster.multiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, ItemResultTicketedBonusTicket itemResultTicketedBonusTicket)
    {
        if (itemResultTicketedBonusTicket == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "ticketId", itemResultTicketedBonusTicket.ticketId);
    }

    public static void Serialize(IBlackboard bb, ItemResultTierBoost itemResultTierBoost)
    {
        if (itemResultTierBoost == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "earnCredit", itemResultTierBoost.earnCredit);
        if (itemResultTierBoost.tierBoost != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "tierBoost"), itemResultTierBoost.tierBoost);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "tierBoost");
        }
    }

    public static void Serialize(IBlackboard bb, ItemUseResult itemUseResult)
    {
        if (itemUseResult == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "itemId", itemUseResult.itemId);
        BlackboardUtils.SetOrCreateValue(bb, "itemType", itemUseResult.itemType);
        if (itemUseResult.result != null)
        {
            switch (itemUseResult.itemType)
            {
            case ItemType.CREDIT:
                Serialize(bb, (ItemResultCredit)itemUseResult.result);
                break;

            case ItemType.DAILY_BOOST:
                Serialize(bb, (ItemResultDailyBoost)itemUseResult.result);
                break;

            case ItemType.TIER_BOOST:
                Serialize(bb, (ItemResultTierBoost)itemUseResult.result);
                break;

            case ItemType.CREDIT_POT_OF_GOLD:
                Serialize(bb, (ItemResultCreditPotOfGold)itemUseResult.result);
                break;

            case ItemType.CREDIT_MULTIPLIER_WHEEL:
                Serialize(bb, (ItemResultCreditMultiplierWheel)itemUseResult.result);
                break;

            case ItemType.DAILY_BONUS_WHEEL:
                Serialize(bb, (ItemResultDailyBonusWheel)itemUseResult.result);
                break;

            case ItemType.PIGGY_BANK:
                Serialize(bb, (ItemResultPiggyBank)itemUseResult.result);
                break;

            case ItemType.EARLY_ACCESS:
                Serialize(bb, (ItemResultEarlyAccess)itemUseResult.result);
                break;

            case ItemType.CREDIT_WHEEL:
                Serialize(bb, (ItemResultCreditWheel)itemUseResult.result);
                break;

            case ItemType.TICKETED_BONUS_TICKET:
                Serialize(bb, (ItemResultTicketedBonusTicket)itemUseResult.result);
                break;

            case ItemType.DAILY_MEGA_WHEEL:
                Serialize(bb, (ItemResultDailyMegaWheel)itemUseResult.result);
                break;

            case ItemType.POG_BOOSTER:
                Serialize(bb, (ItemResultPogBooster)itemUseResult.result);
                break;

            case ItemType.GEM:
                Serialize(bb, (ItemResultGem)itemUseResult.result);
                break;

            case ItemType.GEM_BOOSTER:
                Serialize(bb, (ItemResultGemBooster)itemUseResult.result);
                break;

            case ItemType.EPIC_PASS:
                Serialize(bb, (ItemResultEpicPass)itemUseResult.result);
                break;

            case ItemType.SPIN_BOOST:
                Serialize(bb, (ItemResultSpinBoost)itemUseResult.result);
                break;

            case ItemType.HIDDEN_UNIVERSE_FINDER:
                Serialize(bb, (ItemResultHiddenUniverseFinder)itemUseResult.result);
                break;

            case ItemType.TICKETED_BONUS_BOOSTER:
                Serialize(bb, (ItemResultTicketedBonusBooster)itemUseResult.result);
                break;

            case ItemType.EPIC_PASS_V2:
                Serialize(bb, (ItemResultEpicPass)itemUseResult.result);
                break;
            }
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "result");
        }
    }

    public static void Serialize(IBlackboard bb, JackpotInfo jackpotInfo)
    {
        if (jackpotInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "current", jackpotInfo.current);
        BlackboardUtils.SetOrCreateValue(bb, "prev", jackpotInfo.prev);
        BlackboardUtils.SetOrCreateValue(bb, "deltaMs", jackpotInfo.deltaMs);
        BlackboardUtils.SetOrCreateValue(bb, "min", jackpotInfo.min);
        BlackboardUtils.SetOrCreateValue(bb, "max", jackpotInfo.max);
        BlackboardUtils.SetOrCreateValue(bb, "alarm", jackpotInfo.alarm);
    }

    public static void Serialize(IBlackboard bb, JackpotInfoForLobbyV5 jackpotInfoForLobbyV5)
    {
        if (jackpotInfoForLobbyV5 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", jackpotInfoForLobbyV5.gameId);
        BlackboardUtils.SetOrCreateList(bb, "jackpots", jackpotInfoForLobbyV5.jackpots, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "jackpotAssetType", jackpotInfoForLobbyV5.jackpotAssetType);
    }

    public static void Serialize(IBlackboard bb, KenoClaimBonusRequest kenoClaimBonusRequest)
    {
        if (kenoClaimBonusRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", kenoClaimBonusRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", kenoClaimBonusRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "contents", kenoClaimBonusRequest.contents);
        BlackboardUtils.SetOrCreateValue(bb, "gameId", kenoClaimBonusRequest.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "isGamePlay", kenoClaimBonusRequest.isGamePlay);
        BlackboardUtils.SetOrCreateValue(bb, "isBonusPlay", kenoClaimBonusRequest.isBonusPlay);
        BlackboardUtils.SetOrCreateValue(bb, "metaGameEventId", kenoClaimBonusRequest.metaGameEventId);
        BlackboardUtils.SetOrCreateValue(bb, "seasonPassEventId", kenoClaimBonusRequest.seasonPassEventId);
    }

    public static void Serialize(IBlackboard bb, KenoClaimBonusResponse kenoClaimBonusResponse)
    {
        if (kenoClaimBonusResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", kenoClaimBonusResponse.error);
        if (kenoClaimBonusResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), kenoClaimBonusResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", kenoClaimBonusResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "contents", kenoClaimBonusResponse.contents);
        if (kenoClaimBonusResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), kenoClaimBonusResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
    }

    public static void Serialize(IBlackboard bb, KenoPlayRequest kenoPlayRequest)
    {
        if (kenoPlayRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", kenoPlayRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", kenoPlayRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "contents", kenoPlayRequest.contents);
        BlackboardUtils.SetOrCreateValue(bb, "gameId", kenoPlayRequest.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "isGamePlay", kenoPlayRequest.isGamePlay);
        BlackboardUtils.SetOrCreateValue(bb, "isAutoPlay", kenoPlayRequest.isAutoPlay);
        BlackboardUtils.SetOrCreateValue(bb, "isBonusPlay", kenoPlayRequest.isBonusPlay);
        BlackboardUtils.SetOrCreateValue(bb, "metaGameEventId", kenoPlayRequest.metaGameEventId);
        BlackboardUtils.SetOrCreateValue(bb, "collectingGameChestDropRateMultiplyEventId", kenoPlayRequest.collectingGameChestDropRateMultiplyEventId);
        BlackboardUtils.SetOrCreateValue(bb, "isAutoChange", kenoPlayRequest.isAutoChange);
        BlackboardUtils.SetOrCreateValue(bb, "expEventIdList", kenoPlayRequest.expEventIdList);
        BlackboardUtils.SetOrCreateValue(bb, "isHighRollerBet", kenoPlayRequest.isHighRollerBet);
        BlackboardUtils.SetOrCreateValue(bb, "seasonPassEventId", kenoPlayRequest.seasonPassEventId);
    }

    public static void Serialize(IBlackboard bb, KenoPlayResponseV1 kenoPlayResponseV1)
    {
        if (kenoPlayResponseV1 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", kenoPlayResponseV1.error);
        if (kenoPlayResponseV1.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), kenoPlayResponseV1.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", kenoPlayResponseV1.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "contents", kenoPlayResponseV1.contents);
        if (kenoPlayResponseV1.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), kenoPlayResponseV1.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "nextMysteryGiftLevel", kenoPlayResponseV1.nextMysteryGiftLevel);
        if (kenoPlayResponseV1.tournamentInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "tournamentInfo"), kenoPlayResponseV1.tournamentInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "tournamentInfo");
        }
        if (kenoPlayResponseV1.mysteryGiftInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "mysteryGiftInfo"), kenoPlayResponseV1.mysteryGiftInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "mysteryGiftInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "gamePlayCount", kenoPlayResponseV1.gamePlayCount);
        BlackboardUtils.SetOrCreateValue(bb, "bonusPlayCount", kenoPlayResponseV1.bonusPlayCount);
        BlackboardUtils.SetOrCreateValue(bb, "featureUnlockList", kenoPlayResponseV1.featureUnlockList);
        if (kenoPlayResponseV1.metaGameInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "metaGameInfo"), kenoPlayResponseV1.metaGameInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "metaGameInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "earnFinder", kenoPlayResponseV1.earnFinder);
        if (kenoPlayResponseV1.vipLoungeCompositeInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "vipLoungeCompositeInfo"), kenoPlayResponseV1.vipLoungeCompositeInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "vipLoungeCompositeInfo");
        }
        if (kenoPlayResponseV1.levelUpDashInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "levelUpDashInfo"), kenoPlayResponseV1.levelUpDashInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "levelUpDashInfo");
        }
    }

    public static void Serialize(IBlackboard bb, LPBoostInfo lPBoostInfo)
    {
        if (lPBoostInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "isActive", lPBoostInfo.isActive);
        BlackboardUtils.SetOrCreateValue(bb, "isGlobalEnabled", lPBoostInfo.isGlobalEnabled);
        BlackboardUtils.SetOrCreateValue(bb, "eventPercent", lPBoostInfo.eventPercent);
        BlackboardUtils.SetOrCreateValue(bb, "isTierBoostEnabled", lPBoostInfo.isTierBoostEnabled);
        BlackboardUtils.SetOrCreateValue(bb, "tierBoostPercent", lPBoostInfo.tierBoostPercent);
        BlackboardUtils.SetOrCreateValue(bb, "isPurchaseBoostEnabled", lPBoostInfo.isPurchaseBoostEnabled);
        BlackboardUtils.SetOrCreateValue(bb, "purchasePercent", lPBoostInfo.purchasePercent);
        BlackboardUtils.SetOrCreateValue(bb, "totalPercent", lPBoostInfo.totalPercent);
        BlackboardUtils.SetOrCreateValue(bb, "purchasePercentMax", lPBoostInfo.purchasePercentMax);
    }

    public static void Serialize(IBlackboard bb, LadderRow ladderRow)
    {
        if (ladderRow == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "bars", ladderRow.bars);
    }

    public static void Serialize(IBlackboard bb, LeaderboardEntry leaderboardEntry)
    {
        if (leaderboardEntry == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "userId", leaderboardEntry.userId);
        BlackboardUtils.SetOrCreateValue(bb, "name", leaderboardEntry.name);
        BlackboardUtils.SetOrCreateValue(bb, "profileUrl", leaderboardEntry.profileUrl);
        BlackboardUtils.SetOrCreateValue(bb, "tier", leaderboardEntry.tier);
        BlackboardUtils.SetOrCreateValue(bb, "reportCount", leaderboardEntry.reportCount);
        BlackboardUtils.SetOrCreateValue(bb, "maxWin", leaderboardEntry.maxWin);
        BlackboardUtils.SetOrCreateValue(bb, "totalWin", leaderboardEntry.totalWin);
        BlackboardUtils.SetOrCreateValue(bb, "coin", leaderboardEntry.coin);
    }

    public static void Serialize(IBlackboard bb, LeaderboardRequest leaderboardRequest)
    {
        if (leaderboardRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", leaderboardRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "periodType", leaderboardRequest.periodType);
        BlackboardUtils.SetOrCreateValue(bb, "sortType", leaderboardRequest.sortType);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", leaderboardRequest.ackMask);
    }

    public static void Serialize(IBlackboard bb, LeaderboardResponse leaderboardResponse)
    {
        if (leaderboardResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", leaderboardResponse.error);
        if (leaderboardResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), leaderboardResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", leaderboardResponse.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "userList", leaderboardResponse.userList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "userRank", leaderboardResponse.userRank);
        BlackboardUtils.SetOrCreateValue(bb, "userValue", leaderboardResponse.userValue);
        BlackboardUtils.SetOrCreateValue(bb, "expireTimestamp", leaderboardResponse.expireTimestamp);
    }

    public static void Serialize(IBlackboard bb, Level level)
    {
        if (level == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "EXP_PER_CREDIT", level.EXP_PER_CREDIT);
        BlackboardUtils.SetOrCreateValue(bb, "MAX_LEVEL", level.MAX_LEVEL);
        BlackboardUtils.SetOrCreateValue(bb, "REQUIRED_EXP_TABLE", level.REQUIRED_EXP_TABLE);
        BlackboardUtils.SetOrCreateValue(bb, "CREDIT_BONUS", level.CREDIT_BONUS);
        BlackboardUtils.SetOrCreateValue(bb, "RP_BONUS", level.RP_BONUS);
        BlackboardUtils.SetOrCreateValue(bb, "EXP_PER_SPIN", level.EXP_PER_SPIN);
        BlackboardUtils.SetOrCreateValue(bb, "MAX_EXP_CAP", level.MAX_EXP_CAP);
        BlackboardUtils.SetOrCreateList(bb, "LEVEL_MULTIPLIER_NUMERATOR_TABLE", level.LEVEL_MULTIPLIER_NUMERATOR_TABLE, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "GEM_BONUS", level.GEM_BONUS);
        BlackboardUtils.SetOrCreateList(bb, "LEVEL_MULTIPLIER_NEW_NUMERATOR_TABLE", level.LEVEL_MULTIPLIER_NEW_NUMERATOR_TABLE, Serialize);
    }

    public static void Serialize(IBlackboard bb, LevelMultiplierNewNumeratorInfo levelMultiplierNewNumeratorInfo)
    {
        if (levelMultiplierNewNumeratorInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "level", levelMultiplierNewNumeratorInfo.level);
        BlackboardUtils.SetOrCreateValue(bb, "promotionFormat", levelMultiplierNewNumeratorInfo.promotionFormat);
        BlackboardUtils.SetOrCreateValue(bb, "coin", levelMultiplierNewNumeratorInfo.coin);
        BlackboardUtils.SetOrCreateValue(bb, "pog", levelMultiplierNewNumeratorInfo.pog);
        BlackboardUtils.SetOrCreateValue(bb, "dailyBoost", levelMultiplierNewNumeratorInfo.dailyBoost);
        BlackboardUtils.SetOrCreateValue(bb, "wheel", levelMultiplierNewNumeratorInfo.wheel);
        BlackboardUtils.SetOrCreateValue(bb, "earlyAccess", levelMultiplierNewNumeratorInfo.earlyAccess);
        BlackboardUtils.SetOrCreateValue(bb, "ticketedBonus", levelMultiplierNewNumeratorInfo.ticketedBonus);
        BlackboardUtils.SetOrCreateValue(bb, "spinDeal", levelMultiplierNewNumeratorInfo.spinDeal);
        BlackboardUtils.SetOrCreateValue(bb, "scratcher", levelMultiplierNewNumeratorInfo.scratcher);
        BlackboardUtils.SetOrCreateValue(bb, "hogDeal", levelMultiplierNewNumeratorInfo.hogDeal);
        BlackboardUtils.SetOrCreateValue(bb, "bossRaidersDeal", levelMultiplierNewNumeratorInfo.bossRaidersDeal);
        BlackboardUtils.SetOrCreateValue(bb, "timeBonus", levelMultiplierNewNumeratorInfo.timeBonus);
        BlackboardUtils.SetOrCreateValue(bb, "luckySpin", levelMultiplierNewNumeratorInfo.luckySpin);
        BlackboardUtils.SetOrCreateValue(bb, "dailySpin", levelMultiplierNewNumeratorInfo.dailySpin);
        BlackboardUtils.SetOrCreateValue(bb, "vipBonus", levelMultiplierNewNumeratorInfo.vipBonus);
        BlackboardUtils.SetOrCreateValue(bb, "epicWinShare", levelMultiplierNewNumeratorInfo.epicWinShare);
        BlackboardUtils.SetOrCreateValue(bb, "welcomeBackBonus", levelMultiplierNewNumeratorInfo.welcomeBackBonus);
        BlackboardUtils.SetOrCreateValue(bb, "friendsBonus", levelMultiplierNewNumeratorInfo.friendsBonus);
        BlackboardUtils.SetOrCreateValue(bb, "friendsDealBonus", levelMultiplierNewNumeratorInfo.friendsDealBonus);
        BlackboardUtils.SetOrCreateValue(bb, "clubDealBonus", levelMultiplierNewNumeratorInfo.clubDealBonus);
        BlackboardUtils.SetOrCreateValue(bb, "bbb", levelMultiplierNewNumeratorInfo.bbb);
        BlackboardUtils.SetOrCreateValue(bb, "leaguePoint", levelMultiplierNewNumeratorInfo.leaguePoint);
        BlackboardUtils.SetOrCreateValue(bb, "clubReward", levelMultiplierNewNumeratorInfo.clubReward);
        BlackboardUtils.SetOrCreateValue(bb, "maxExpCap", levelMultiplierNewNumeratorInfo.maxExpCap);
        BlackboardUtils.SetOrCreateValue(bb, "vipDeal", levelMultiplierNewNumeratorInfo.vipDeal);
        BlackboardUtils.SetOrCreateValue(bb, "vipDealFreebie", levelMultiplierNewNumeratorInfo.vipDealFreebie);
    }

    public static void Serialize(IBlackboard bb, LevelMultiplierNumeratorInfo levelMultiplierNumeratorInfo)
    {
        if (levelMultiplierNumeratorInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "level", levelMultiplierNumeratorInfo.level);
        BlackboardUtils.SetOrCreateValue(bb, "shop", levelMultiplierNumeratorInfo.shop);
        BlackboardUtils.SetOrCreateValue(bb, "timeBonus", levelMultiplierNumeratorInfo.timeBonus);
        BlackboardUtils.SetOrCreateValue(bb, "luckySpin", levelMultiplierNumeratorInfo.luckySpin);
        BlackboardUtils.SetOrCreateValue(bb, "dailySpin", levelMultiplierNumeratorInfo.dailySpin);
        BlackboardUtils.SetOrCreateValue(bb, "vipBonus", levelMultiplierNumeratorInfo.vipBonus);
        BlackboardUtils.SetOrCreateValue(bb, "epicWinShare", levelMultiplierNumeratorInfo.epicWinShare);
        BlackboardUtils.SetOrCreateValue(bb, "welcomeBackBonus", levelMultiplierNumeratorInfo.welcomeBackBonus);
        BlackboardUtils.SetOrCreateValue(bb, "friendsBonus", levelMultiplierNumeratorInfo.friendsBonus);
        BlackboardUtils.SetOrCreateValue(bb, "friendsDealBonus", levelMultiplierNumeratorInfo.friendsDealBonus);
        BlackboardUtils.SetOrCreateValue(bb, "clubDealBonus", levelMultiplierNumeratorInfo.clubDealBonus);
        BlackboardUtils.SetOrCreateValue(bb, "leaguePoint", levelMultiplierNumeratorInfo.leaguePoint);
        BlackboardUtils.SetOrCreateValue(bb, "clubReward", levelMultiplierNumeratorInfo.clubReward);
        BlackboardUtils.SetOrCreateValue(bb, "maxExpCap", levelMultiplierNumeratorInfo.maxExpCap);
        BlackboardUtils.SetOrCreateValue(bb, "promotionFormat", levelMultiplierNumeratorInfo.promotionFormat);
        BlackboardUtils.SetOrCreateValue(bb, "earlyAccess", levelMultiplierNumeratorInfo.earlyAccess);
        BlackboardUtils.SetOrCreateValue(bb, "bbb", levelMultiplierNumeratorInfo.bbb);
    }

    public static void Serialize(IBlackboard bb, LevelUpDashAnyPurchaseBoosterPersonal levelUpDashAnyPurchaseBoosterPersonal)
    {
        if (levelUpDashAnyPurchaseBoosterPersonal == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "expMultiplyNumerator", levelUpDashAnyPurchaseBoosterPersonal.expMultiplyNumerator);
        BlackboardUtils.SetOrCreateValue(bb, "endTimestamp", levelUpDashAnyPurchaseBoosterPersonal.endTimestamp);
    }

    public static void Serialize(IBlackboard bb, LevelUpDashInfoOnLogin levelUpDashInfoOnLogin)
    {
        if (levelUpDashInfoOnLogin == null) { return; }
        if (levelUpDashInfoOnLogin.missionExtendedPersonal != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "missionExtendedPersonal"), levelUpDashInfoOnLogin.missionExtendedPersonal);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "missionExtendedPersonal");
        }
        if (levelUpDashInfoOnLogin.anyPurchaseBoosterPersonal != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "anyPurchaseBoosterPersonal"), levelUpDashInfoOnLogin.anyPurchaseBoosterPersonal);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "anyPurchaseBoosterPersonal");
        }
    }

    public static void Serialize(IBlackboard bb, LevelUpDashInfoOnSpin levelUpDashInfoOnSpin)
    {
        if (levelUpDashInfoOnSpin == null) { return; }
        if (levelUpDashInfoOnSpin.missionExtendedPersonal != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "missionExtendedPersonal"), levelUpDashInfoOnSpin.missionExtendedPersonal);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "missionExtendedPersonal");
        }
        if (levelUpDashInfoOnSpin.anyPurchaseBoosterPersonal != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "anyPurchaseBoosterPersonal"), levelUpDashInfoOnSpin.anyPurchaseBoosterPersonal);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "anyPurchaseBoosterPersonal");
        }
        BlackboardUtils.SetOrCreateList(bb, "missionStageRewardResultList", levelUpDashInfoOnSpin.missionStageRewardResultList, Serialize);
    }

    public static void Serialize(IBlackboard bb, LevelUpDashMissionExtendedPersonal levelUpDashMissionExtendedPersonal)
    {
        if (levelUpDashMissionExtendedPersonal == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "id", levelUpDashMissionExtendedPersonal.id);
        BlackboardUtils.SetOrCreateValue(bb, "eventId", levelUpDashMissionExtendedPersonal.eventId);
        BlackboardUtils.SetOrCreateValue(bb, "userId", levelUpDashMissionExtendedPersonal.userId);
        BlackboardUtils.SetOrCreateList(bb, "stageList", levelUpDashMissionExtendedPersonal.stageList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "stageClearCount", levelUpDashMissionExtendedPersonal.stageClearCount);
        BlackboardUtils.SetOrCreateValue(bb, "missionStartLevel", levelUpDashMissionExtendedPersonal.missionStartLevel);
        BlackboardUtils.SetOrCreateValue(bb, "missionEndTimestamp", levelUpDashMissionExtendedPersonal.missionEndTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "isAnyPurchaseBoosterEnabled", levelUpDashMissionExtendedPersonal.isAnyPurchaseBoosterEnabled);
    }

    public static void Serialize(IBlackboard bb, LevelUpDashMissionStage levelUpDashMissionStage)
    {
        if (levelUpDashMissionStage == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "targetLevel", levelUpDashMissionStage.targetLevel);
        BlackboardUtils.SetOrCreateList(bb, "rewardBundle", levelUpDashMissionStage.rewardBundle, Serialize);
    }

    public static void Serialize(IBlackboard bb, LobbyBGM lobbyBGM)
    {
        if (lobbyBGM == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "audioUrl", lobbyBGM.audioUrl);
        BlackboardUtils.SetOrCreateValue(bb, "isActive", lobbyBGM.isActive);
    }

    public static void Serialize(IBlackboard bb, LobbyRequest lobbyRequest)
    {
        if (lobbyRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", lobbyRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "currentTime", lobbyRequest.currentTime);
        BlackboardUtils.SetOrCreateValue(bb, "timezoneOffset", lobbyRequest.timezoneOffset);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", lobbyRequest.ackMask);
    }

    public static void Serialize(IBlackboard bb, LobbyResponseV6 lobbyResponseV6)
    {
        if (lobbyResponseV6 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", lobbyResponseV6.error);
        if (lobbyResponseV6.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), lobbyResponseV6.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }

        BlackboardUtils.SetOrCreateValue(bb, "serverTime", lobbyResponseV6.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "slotListId", lobbyResponseV6.slotListId);
        BlackboardUtils.SetOrCreateList(bb, "slotList", lobbyResponseV6.slotList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "vipSlotListId", lobbyResponseV6.vipSlotListId);
        BlackboardUtils.SetOrCreateList(bb, "vipSlotList", lobbyResponseV6.vipSlotList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "inboxList", lobbyResponseV6.inboxList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "celebrityList", lobbyResponseV6.celebrityList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "friendList", lobbyResponseV6.friendList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "onlineFriendUserIdList", lobbyResponseV6.onlineFriendUserIdList);
        BlackboardUtils.SetOrCreateValue(bb, "lastCollectTimestamp", lobbyResponseV6.lastCollectTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "timeBonusCooltime", lobbyResponseV6.timeBonusCooltime);
        BlackboardUtils.SetOrCreateList(bb, "friendRecommendationList", lobbyResponseV6.friendRecommendationList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "gameInfoList", lobbyResponseV6.gameInfoList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "bigwinRankList", lobbyResponseV6.bigwinRankList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "nextMysteryGiftLevel", lobbyResponseV6.nextMysteryGiftLevel);
        BlackboardUtils.SetOrCreateList(bb, "friendEncourageList", lobbyResponseV6.friendEncourageList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "friendEncourageRewardCredit", lobbyResponseV6.friendEncourageRewardCredit);
        BlackboardUtils.SetOrCreateList(bb, "shopList", lobbyResponseV6.shopList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "lastWallOfEpicRecordId", lobbyResponseV6.lastWallOfEpicRecordId);
        BlackboardUtils.SetOrCreateList(bb, "jackpotInfoForLobbyList", lobbyResponseV6.jackpotInfoForLobbyList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "bonusSpinList", lobbyResponseV6.bonusSpinList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "totalGameSpinCountList", lobbyResponseV6.totalGameSpinCountList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "favoriteSlotIdList", lobbyResponseV6.favoriteSlotIdList);
        BlackboardUtils.SetOrCreateValue(bb, "favoriteSlotUiIndex", lobbyResponseV6.favoriteSlotUiIndex);
        if (lobbyResponseV6.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), lobbyResponseV6.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "clubId", lobbyResponseV6.clubId);
        BlackboardUtils.SetOrCreateValue(bb, "kickedFromClub", lobbyResponseV6.kickedFromClub);
        BlackboardUtils.SetOrCreateList(bb, "clubOnlineList", lobbyResponseV6.clubOnlineList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "dailyBonusSpinCount", lobbyResponseV6.dailyBonusSpinCount);
        BlackboardUtils.SetOrCreateValue(bb, "clubLeagueOpened", lobbyResponseV6.clubLeagueOpened);
        BlackboardUtils.SetOrCreateValue(bb, "dailyMegaWheelSpinCount", lobbyResponseV6.dailyMegaWheelSpinCount);
        BlackboardUtils.SetOrCreateDict(bb, "metaJackpotInfoDict", lobbyResponseV6.metaJackpotInfoDict, BlackboardUtils.WrapAnonymousList<JackpotInfo>(Serialize));
        if (lobbyResponseV6.clubBadgeInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "clubBadgeInfo"), lobbyResponseV6.clubBadgeInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "clubBadgeInfo");
        }
        if (lobbyResponseV6.userClubStateInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userClubStateInfo"), lobbyResponseV6.userClubStateInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userClubStateInfo");
        }
        BlackboardUtils.SetOrCreateList(bb, "inAppMessageList", lobbyResponseV6.inAppMessageList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "lobbyBackgroundImageUrl", lobbyResponseV6.lobbyBackgroundImageUrl);
        if (lobbyResponseV6.vipDealInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "vipDealInfo"), lobbyResponseV6.vipDealInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "vipDealInfo");
        }
        BlackboardUtils.SetOrCreateList(bb, "inboxBannerList", lobbyResponseV6.inboxBannerList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "slotBannerGroupList", lobbyResponseV6.slotBannerGroupList, Serialize);
        if (lobbyResponseV6.metaGameEnterInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "metaGameEnterInfo"), lobbyResponseV6.metaGameEnterInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "metaGameEnterInfo");
        }
        BlackboardUtils.SetOrCreateList(bb, "woeInfoList", lobbyResponseV6.woeInfoList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "epicAlbumGameIdList", lobbyResponseV6.epicAlbumGameIdList);
        BlackboardUtils.SetOrCreateValue(bb, "clubChatChannelId", lobbyResponseV6.clubChatChannelId);
        BlackboardUtils.SetOrCreateValue(bb, "clubAuthority", lobbyResponseV6.clubAuthority);
        if (lobbyResponseV6.clubInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "clubInfo"), lobbyResponseV6.clubInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "clubInfo");
        }
        BlackboardUtils.SetOrCreateList(bb, "noticeList", lobbyResponseV6.noticeList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "enabledEpicAlbumRewardCount", lobbyResponseV6.enabledEpicAlbumRewardCount);
        BlackboardUtils.SetOrCreateValue(bb, "vipDailyBonusCooltime", lobbyResponseV6.vipDailyBonusCooltime);
        BlackboardUtils.SetOrCreateValue(bb, "lastShopBonusVideoAdsClaimTimestamp", lobbyResponseV6.lastShopBonusVideoAdsClaimTimestamp);
        if (lobbyResponseV6.lobbyBgm != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "lobbyBgm"), lobbyResponseV6.lobbyBgm);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "lobbyBgm");
        }
        BlackboardUtils.SetOrCreateValue(bb, "dailySpinPricePointsIndex", lobbyResponseV6.dailySpinPricePointsIndex);
        BlackboardUtils.SetOrCreateValue(bb, "lastClubLeaveTimestamp", lobbyResponseV6.lastClubLeaveTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "winxNumeratorDict", lobbyResponseV6.winxNumeratorDict);
        if (lobbyResponseV6.clubArenaRankingPopupInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "clubArenaRankingPopupInfo"), lobbyResponseV6.clubArenaRankingPopupInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "clubArenaRankingPopupInfo");
        }
        BlackboardUtils.SetOrCreateList(bb, "challengeFriendRecommendationList", lobbyResponseV6.challengeFriendRecommendationList, Serialize);
        if (lobbyResponseV6.bossRaidersRankingPopupInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "bossRaidersRankingPopupInfo"), lobbyResponseV6.bossRaidersRankingPopupInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "bossRaidersRankingPopupInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "clubLeagueCurrentWeek", lobbyResponseV6.clubLeagueCurrentWeek);
        BlackboardUtils.SetOrCreateValue(bb, "clubLeagueWeeklyPopupRevealTimestamp", lobbyResponseV6.clubLeagueWeeklyPopupRevealTimestamp);
        if (lobbyResponseV6.hiddenUniverseInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "hiddenUniverseInfo"), lobbyResponseV6.hiddenUniverseInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "hiddenUniverseInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "baseWagerDict", lobbyResponseV6.baseWagerDict);
        BlackboardUtils.SetOrCreateValue(bb, "showLevelMultiplierText", lobbyResponseV6.showLevelMultiplierText);
        BlackboardUtils.SetOrCreateValue(bb, "earlyAccessTotalBetInfo", lobbyResponseV6.earlyAccessTotalBetInfo);
        if (lobbyResponseV6.vipLoungeInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "vipLoungeInfo"), lobbyResponseV6.vipLoungeInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "vipLoungeInfo");
        }
        if (lobbyResponseV6.chatBanInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "chatBanInfo"), lobbyResponseV6.chatBanInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "chatBanInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "bossRaidersDealRecoveryUuid", lobbyResponseV6.bossRaidersDealRecoveryUuid);
        BlackboardUtils.SetOrCreateValue(bb, "blockedUserIdList", lobbyResponseV6.blockedUserIdList);
        if (lobbyResponseV6.iamExtraData != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "iamExtraData"), lobbyResponseV6.iamExtraData);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "iamExtraData");
        }
        if (lobbyResponseV6.buildDreamInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "buildDreamInfo"), lobbyResponseV6.buildDreamInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "buildDreamInfo");
        }
        if (lobbyResponseV6.gurusFinalReward != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "gurusFinalReward"), lobbyResponseV6.gurusFinalReward);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "gurusFinalReward");
        }
        BlackboardUtils.SetOrCreateValue(bb, "loungeJackpotCompensation", lobbyResponseV6.loungeJackpotCompensation);
        BlackboardUtils.SetOrCreateList(bb, "bottomIconList", lobbyResponseV6.bottomIconList, Serialize);
        if (lobbyResponseV6.seasonPassEnterInfoV2 != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "seasonPassEnterInfoV2"), lobbyResponseV6.seasonPassEnterInfoV2);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "seasonPassEnterInfoV2");
        }
        if (lobbyResponseV6.userBucksInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userBucksInfo"), lobbyResponseV6.userBucksInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userBucksInfo");
        }
        if (lobbyResponseV6.vipDealInfoV2 != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "vipDealInfoV2"), lobbyResponseV6.vipDealInfoV2);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "vipDealInfoV2");
        }
        BlackboardUtils.SetOrCreateValue(bb, "vipDealV2FreebieCompensation", lobbyResponseV6.vipDealV2FreebieCompensation);
    }

    public static void Serialize(IBlackboard bb, LocalPush localPush)
    {
        if (localPush == null) { return; }
        if (localPush.DO_NOT_DISTURB_SETTING != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "DO_NOT_DISTURB_SETTING"), localPush.DO_NOT_DISTURB_SETTING);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "DO_NOT_DISTURB_SETTING");
        }
        if (localPush.TIME_BONUS != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "TIME_BONUS"), localPush.TIME_BONUS);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "TIME_BONUS");
        }
        if (localPush.DAILY_BONUS != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "DAILY_BONUS"), localPush.DAILY_BONUS);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "DAILY_BONUS");
        }
        BlackboardUtils.SetOrCreateList(bb, "RETENTION_LIST", localPush.RETENTION_LIST, Serialize);
        if (localPush.LUCKY_SPIN_BONUS != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "LUCKY_SPIN_BONUS"), localPush.LUCKY_SPIN_BONUS);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "LUCKY_SPIN_BONUS");
        }
    }

    public static void Serialize(IBlackboard bb, LoginFacebookRequest loginFacebookRequest)
    {
        if (loginFacebookRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", loginFacebookRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "clientOs", loginFacebookRequest.clientOs);
        BlackboardUtils.SetOrCreateValue(bb, "clientNumberVersion", loginFacebookRequest.clientNumberVersion);
        BlackboardUtils.SetOrCreateValue(bb, "accessToken", loginFacebookRequest.accessToken);
        BlackboardUtils.SetOrCreateValue(bb, "loginTime", loginFacebookRequest.loginTime);
        BlackboardUtils.SetOrCreateValue(bb, "timezoneOffset", loginFacebookRequest.timezoneOffset);
        BlackboardUtils.SetOrCreateValue(bb, "language", loginFacebookRequest.language);
        BlackboardUtils.SetOrCreateValue(bb, "isServerMaintenanceIgnore", loginFacebookRequest.isServerMaintenanceIgnore);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", loginFacebookRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "queryString", loginFacebookRequest.queryString);
        BlackboardUtils.SetOrCreateValue(bb, "contextId", loginFacebookRequest.contextId);
        BlackboardUtils.SetOrCreateValue(bb, "deviceName", loginFacebookRequest.deviceName);
        BlackboardUtils.SetOrCreateValue(bb, "osVersion", loginFacebookRequest.osVersion);
        BlackboardUtils.SetOrCreateValue(bb, "assetVersion", loginFacebookRequest.assetVersion);
    }

    public static void Serialize(IBlackboard bb, LoginRequest loginRequest)
    {
        if (loginRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", loginRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "clientOs", loginRequest.clientOs);
        BlackboardUtils.SetOrCreateValue(bb, "clientNumberVersion", loginRequest.clientNumberVersion);
        BlackboardUtils.SetOrCreateValue(bb, "deviceId", loginRequest.deviceId);
        BlackboardUtils.SetOrCreateValue(bb, "loginTime", loginRequest.loginTime);
        BlackboardUtils.SetOrCreateValue(bb, "timezoneOffset", loginRequest.timezoneOffset);
        BlackboardUtils.SetOrCreateValue(bb, "language", loginRequest.language);
        BlackboardUtils.SetOrCreateValue(bb, "isServerMaintenanceIgnore", loginRequest.isServerMaintenanceIgnore);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", loginRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "deviceName", loginRequest.deviceName);
        BlackboardUtils.SetOrCreateValue(bb, "assetVersion", loginRequest.assetVersion);
        BlackboardUtils.SetOrCreateValue(bb, "clickPn", loginRequest.clickPn);
        BlackboardUtils.SetOrCreateValue(bb, "devicePushSetting", loginRequest.devicePushSetting);
    }

    public static void Serialize(IBlackboard bb, LoginResponse loginResponse)
    {
        if (loginResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", loginResponse.error);
        if (loginResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), loginResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", loginResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "serverNumberVersion", loginResponse.serverNumberVersion);
        BlackboardUtils.SetOrCreateValue(bb, "serverStringVersion", loginResponse.serverStringVersion);
        BlackboardUtils.SetOrCreateValue(bb, "appDownloadUrl", loginResponse.appDownloadUrl);
        BlackboardUtils.SetOrCreateValue(bb, "appReviewUrl", loginResponse.appReviewUrl);
        BlackboardUtils.SetOrCreateValue(bb, "recentClientNumberVersion", loginResponse.recentClientNumberVersion);
        BlackboardUtils.SetOrCreateValue(bb, "minClientNumberVersion", loginResponse.minClientNumberVersion);
        if (loginResponse.me != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "me"), loginResponse.me);
        }
        else
        {
            Debug.LogError("@ loginResponse.me  is null");
            BlackboardUtils.DestroyBlackboard(bb, "me");
        }
        if (loginResponse.dailyBonusResult != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "dailyBonusResult"), loginResponse.dailyBonusResult);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "dailyBonusResult");
        }
        if (loginResponse.dailyBoost != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "dailyBoost"), loginResponse.dailyBoost);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "dailyBoost");
        }
        if (loginResponse.values != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "values"), loginResponse.values);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "values");
        }
        if (loginResponse.ssoAccountInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "ssoAccountInfo"), loginResponse.ssoAccountInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "ssoAccountInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "distanceFromLastLoginTimestamp", loginResponse.distanceFromLastLoginTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "ongoingEventTimestamp", loginResponse.ongoingEventTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "pushEnabled", loginResponse.pushEnabled);
        if (loginResponse.deviceAdjustData != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "deviceAdjustData"), loginResponse.deviceAdjustData);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "deviceAdjustData");
        }
        BlackboardUtils.SetOrCreateValue(bb, "appName", loginResponse.appName);
        BlackboardUtils.SetOrCreateValue(bb, "deviceFirstInstalled", loginResponse.deviceFirstInstalled);
        BlackboardUtils.SetOrCreateList(bb, "ongoingEventList", loginResponse.ongoingEventList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "versionUpdateRewardCredit", loginResponse.versionUpdateRewardCredit);
        BlackboardUtils.SetOrCreateValue(bb, "isAllInBonusAvailable", loginResponse.isAllInBonusAvailable);
        if (loginResponse.gameSpinReel != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "gameSpinReel"), loginResponse.gameSpinReel);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "gameSpinReel");
        }
        if (loginResponse.videoAdsPlacementNames != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "videoAdsPlacementNames"), loginResponse.videoAdsPlacementNames);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "videoAdsPlacementNames");
        }
        if (loginResponse.dailyBingoInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "dailyBingoInfo"), loginResponse.dailyBingoInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "dailyBingoInfo");
        }
        BlackboardUtils.SetOrCreateList(bb, "challengeInfoList", loginResponse.challengeInfoList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "clientString", loginResponse.clientString);
        if (loginResponse.welcomeBackRewardInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "welcomeBackRewardInfo"), loginResponse.welcomeBackRewardInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "welcomeBackRewardInfo");
        }
        if (loginResponse.bbbRewardInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "bbbRewardInfo"), loginResponse.bbbRewardInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "bbbRewardInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "creditBeforeReward", loginResponse.creditBeforeReward);
        BlackboardUtils.SetOrCreateValue(bb, "clientIp", loginResponse.clientIp);
        BlackboardUtils.SetOrCreateValue(bb, "clientCountry", loginResponse.clientCountry);
        BlackboardUtils.SetOrCreateValue(bb, "facebookLongLivedAccessToken", loginResponse.facebookLongLivedAccessToken);
        if (loginResponse.earlyAccess != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "earlyAccess"), loginResponse.earlyAccess);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "earlyAccess");
        }
        BlackboardUtils.SetOrCreateList(bb, "piggyBankProductList", loginResponse.piggyBankProductList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "hasPreBbbReward", loginResponse.hasPreBbbReward);
        BlackboardUtils.SetOrCreateValue(bb, "lockedFeatureList", loginResponse.lockedFeatureList);
        BlackboardUtils.SetOrCreateValue(bb, "isPurchaseProhibitedRegion", loginResponse.isPurchaseProhibitedRegion);
        BlackboardUtils.SetOrCreateValue(bb, "showConsentPopup", loginResponse.showConsentPopup);
        BlackboardUtils.SetOrCreateValue(bb, "policyUrl", loginResponse.policyUrl);
        BlackboardUtils.SetOrCreateValue(bb, "showStatusMatchMenu", loginResponse.showStatusMatchMenu);
        BlackboardUtils.SetOrCreateValue(bb, "statusMatchResult", loginResponse.statusMatchResult);
        BlackboardUtils.SetOrCreateValue(bb, "disableStatusMatchDeeplink", loginResponse.disableStatusMatchDeeplink);
        if (loginResponse.userOptions != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userOptions"), loginResponse.userOptions);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userOptions");
        }
        if (loginResponse.tutorialInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "tutorialInfo"), loginResponse.tutorialInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "tutorialInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "isEu", loginResponse.isEu);
        BlackboardUtils.SetOrCreateValue(bb, "chatToken", loginResponse.chatToken);
        BlackboardUtils.SetOrCreateValue(bb, "globalChatChannelId", loginResponse.globalChatChannelId);
        BlackboardUtils.SetOrCreateList(bb, "globalChatInfoList", loginResponse.globalChatInfoList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "muteUserIdList", loginResponse.muteUserIdList);
        BlackboardUtils.SetOrCreateValue(bb, "instantMessageCooltime", loginResponse.instantMessageCooltime);
        BlackboardUtils.SetOrCreateList(bb, "countryInfoList", loginResponse.countryInfoList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "dailyBonusNeedMorePopupEnabled", loginResponse.dailyBonusNeedMorePopupEnabled);
        BlackboardUtils.SetOrCreateValue(bb, "enterGameId", loginResponse.enterGameId);
        BlackboardUtils.SetOrCreateList(bb, "clubChallengeInfoList", loginResponse.clubChallengeInfoList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "adjustConversionValueTargetRevenue", loginResponse.adjustConversionValueTargetRevenue);
        BlackboardUtils.SetOrCreateValue(bb, "termsOfUseUrl", loginResponse.termsOfUseUrl);
        BlackboardUtils.SetOrCreateValue(bb, "clientState", loginResponse.clientState);
        BlackboardUtils.SetOrCreateValue(bb, "clientCity", loginResponse.clientCity);
        BlackboardUtils.SetOrCreateValue(bb, "termsOfUseType", loginResponse.termsOfUseType);
        if (loginResponse.metaGameCompensation != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "metaGameCompensation"), loginResponse.metaGameCompensation);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "metaGameCompensation");
        }
        if (loginResponse.inviteInstallInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "inviteInstallInfo"), loginResponse.inviteInstallInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "inviteInstallInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "isAccountRemovalPermitted", loginResponse.isAccountRemovalPermitted);
        if (loginResponse.accountRemovalInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "accountRemovalInfo"), loginResponse.accountRemovalInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "accountRemovalInfo");
        }
        if (loginResponse.levelUpDashInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "levelUpDashInfo"), loginResponse.levelUpDashInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "levelUpDashInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "isPipModeEnabled", loginResponse.isPipModeEnabled);
        BlackboardUtils.SetOrCreateValue(bb, "isPipTriggerButtonEnabled", loginResponse.isPipTriggerButtonEnabled);
    }

    public static void Serialize(IBlackboard bb, LoungeJackpotCollectResponse loungeJackpotCollectResponse)
    {
        if (loungeJackpotCollectResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", loungeJackpotCollectResponse.error);
        if (loungeJackpotCollectResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), loungeJackpotCollectResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", loungeJackpotCollectResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "winCredit", loungeJackpotCollectResponse.winCredit);
        if (loungeJackpotCollectResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), loungeJackpotCollectResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
    }

    public static void Serialize(IBlackboard bb, LoungeJackpotDebugSpinRequest loungeJackpotDebugSpinRequest)
    {
        if (loungeJackpotDebugSpinRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", loungeJackpotDebugSpinRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", loungeJackpotDebugSpinRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "debugWinType", loungeJackpotDebugSpinRequest.debugWinType);
        BlackboardUtils.SetOrCreateValue(bb, "contextId", loungeJackpotDebugSpinRequest.contextId);
    }

    public static void Serialize(IBlackboard bb, LoungeJackpotInfo loungeJackpotInfo)
    {
        if (loungeJackpotInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "baseWinCredit", loungeJackpotInfo.baseWinCredit);
        BlackboardUtils.SetOrCreateValue(bb, "grandJackpotCommunityCredit", loungeJackpotInfo.grandJackpotCommunityCredit);
        BlackboardUtils.SetOrCreateList(bb, "jackpotTableList", loungeJackpotInfo.jackpotTableList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "wheelPreset", loungeJackpotInfo.wheelPreset, Serialize);
    }

    public static void Serialize(IBlackboard bb, LoungeJackpotInfoResponse loungeJackpotInfoResponse)
    {
        if (loungeJackpotInfoResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", loungeJackpotInfoResponse.error);
        if (loungeJackpotInfoResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), loungeJackpotInfoResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", loungeJackpotInfoResponse.serverTime);
        if (loungeJackpotInfoResponse.loungeJackpotInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "loungeJackpotInfo"), loungeJackpotInfoResponse.loungeJackpotInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "loungeJackpotInfo");
        }
    }

    public static void Serialize(IBlackboard bb, LoungeJackpotPresentationValues loungeJackpotPresentationValues)
    {
        if (loungeJackpotPresentationValues == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "INCREASE_ANIMATION_RESET_TIME_MILLISEC", loungeJackpotPresentationValues.INCREASE_ANIMATION_RESET_TIME_MILLISEC);
        BlackboardUtils.SetOrCreateValue(bb, "INCREASE_START_PERCENT", loungeJackpotPresentationValues.INCREASE_START_PERCENT);
        BlackboardUtils.SetOrCreateValue(bb, "INCREASE_CREDIT_PER_SECOND", loungeJackpotPresentationValues.INCREASE_CREDIT_PER_SECOND);
        BlackboardUtils.SetOrCreateValue(bb, "AFTER_ANIMATION_REFRESH_TIME_MILLISEC", loungeJackpotPresentationValues.AFTER_ANIMATION_REFRESH_TIME_MILLISEC);
    }

    public static void Serialize(IBlackboard bb, LoungeJackpotSpinRequest loungeJackpotSpinRequest)
    {
        if (loungeJackpotSpinRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", loungeJackpotSpinRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", loungeJackpotSpinRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "contextId", loungeJackpotSpinRequest.contextId);
    }

    public static void Serialize(IBlackboard bb, LoungeJackpotSpinResponse loungeJackpotSpinResponse)
    {
        if (loungeJackpotSpinResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", loungeJackpotSpinResponse.error);
        if (loungeJackpotSpinResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), loungeJackpotSpinResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", loungeJackpotSpinResponse.serverTime);
        if (loungeJackpotSpinResponse.vipLoungeInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "vipLoungeInfo"), loungeJackpotSpinResponse.vipLoungeInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "vipLoungeInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "winCredit", loungeJackpotSpinResponse.winCredit);
        BlackboardUtils.SetOrCreateValue(bb, "wheelIndex", loungeJackpotSpinResponse.wheelIndex);
        if (loungeJackpotSpinResponse.loungeJackpotInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "loungeJackpotInfo"), loungeJackpotSpinResponse.loungeJackpotInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "loungeJackpotInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "usedExcessLoungePoint", loungeJackpotSpinResponse.usedExcessLoungePoint);
    }

    public static void Serialize(IBlackboard bb, LoungeJackpotWheelItem loungeJackpotWheelItem)
    {
        if (loungeJackpotWheelItem == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "winType", loungeJackpotWheelItem.winType);
        BlackboardUtils.SetOrCreateValue(bb, "minMultiplierNumerator", loungeJackpotWheelItem.minMultiplierNumerator);
        BlackboardUtils.SetOrCreateValue(bb, "minValue", loungeJackpotWheelItem.minValue);
        BlackboardUtils.SetOrCreateValue(bb, "maxMultiplierNumerator", loungeJackpotWheelItem.maxMultiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, LuckyFiveCardInfo luckyFiveCardInfo)
    {
        if (luckyFiveCardInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "number", luckyFiveCardInfo.number);
        BlackboardUtils.SetOrCreateValue(bb, "bet", luckyFiveCardInfo.bet);
    }

    public static void Serialize(IBlackboard bb, LuckyFiveClaimRequest luckyFiveClaimRequest)
    {
        if (luckyFiveClaimRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", luckyFiveClaimRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", luckyFiveClaimRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "eventId", luckyFiveClaimRequest.eventId);
    }

    public static void Serialize(IBlackboard bb, LuckyFiveClaimResponse luckyFiveClaimResponse)
    {
        if (luckyFiveClaimResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", luckyFiveClaimResponse.error);
        if (luckyFiveClaimResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), luckyFiveClaimResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", luckyFiveClaimResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "earnCredit", luckyFiveClaimResponse.earnCredit);
        if (luckyFiveClaimResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), luckyFiveClaimResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
    }

    public static void Serialize(IBlackboard bb, LuckyFiveEnterInfo luckyFiveEnterInfo)
    {
        if (luckyFiveEnterInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "cardList", luckyFiveEnterInfo.cardList);
        BlackboardUtils.SetOrCreateValue(bb, "hitList", luckyFiveEnterInfo.hitList);
        BlackboardUtils.SetOrCreateValue(bb, "isClaimable", luckyFiveEnterInfo.isClaimable);
        BlackboardUtils.SetOrCreateValue(bb, "eligibleMinBet", luckyFiveEnterInfo.eligibleMinBet);
    }

    public static void Serialize(IBlackboard bb, LuckyFiveInfoRequest luckyFiveInfoRequest)
    {
        if (luckyFiveInfoRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", luckyFiveInfoRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", luckyFiveInfoRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "eventId", luckyFiveInfoRequest.eventId);
    }

    public static void Serialize(IBlackboard bb, LuckyFiveInfoResponse luckyFiveInfoResponse)
    {
        if (luckyFiveInfoResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", luckyFiveInfoResponse.error);
        if (luckyFiveInfoResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), luckyFiveInfoResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", luckyFiveInfoResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "paytable", luckyFiveInfoResponse.paytable);
        BlackboardUtils.SetOrCreateValue(bb, "unclaimedWin", luckyFiveInfoResponse.unclaimedWin);
        BlackboardUtils.SetOrCreateValue(bb, "totalWin", luckyFiveInfoResponse.totalWin);
        BlackboardUtils.SetOrCreateList(bb, "recentCardList", luckyFiveInfoResponse.recentCardList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "averageBet", luckyFiveInfoResponse.averageBet);
        BlackboardUtils.SetOrCreateValue(bb, "hitList", luckyFiveInfoResponse.hitList);
        BlackboardUtils.SetOrCreateValue(bb, "earnCredit", luckyFiveInfoResponse.earnCredit);
        BlackboardUtils.SetOrCreateValue(bb, "rule", luckyFiveInfoResponse.rule);
    }

    public static void Serialize(IBlackboard bb, LuckyFiveWinInfo luckyFiveWinInfo)
    {
        if (luckyFiveWinInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "averageBet", luckyFiveWinInfo.averageBet);
        BlackboardUtils.SetOrCreateValue(bb, "earnCredit", luckyFiveWinInfo.earnCredit);
        BlackboardUtils.SetOrCreateValue(bb, "rule", luckyFiveWinInfo.rule);
        BlackboardUtils.SetOrCreateValue(bb, "cardList", luckyFiveWinInfo.cardList);
        BlackboardUtils.SetOrCreateValue(bb, "hitList", luckyFiveWinInfo.hitList);
        BlackboardUtils.SetOrCreateValue(bb, "isClaimable", luckyFiveWinInfo.isClaimable);
    }

    public static void Serialize(IBlackboard bb, LuckyFiveWinListRequest luckyFiveWinListRequest)
    {
        if (luckyFiveWinListRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", luckyFiveWinListRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", luckyFiveWinListRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "eventId", luckyFiveWinListRequest.eventId);
    }

    public static void Serialize(IBlackboard bb, LuckyFiveWinListResponse luckyFiveWinListResponse)
    {
        if (luckyFiveWinListResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", luckyFiveWinListResponse.error);
        if (luckyFiveWinListResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), luckyFiveWinListResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", luckyFiveWinListResponse.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "recordList", luckyFiveWinListResponse.recordList, Serialize);
    }

    public static void Serialize(IBlackboard bb, LuckyFiveWinRecord luckyFiveWinRecord)
    {
        if (luckyFiveWinRecord == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "bet", luckyFiveWinRecord.bet);
        BlackboardUtils.SetOrCreateValue(bb, "win", luckyFiveWinRecord.win);
        BlackboardUtils.SetOrCreateValue(bb, "rule", luckyFiveWinRecord.rule);
        BlackboardUtils.SetOrCreateValue(bb, "pay", luckyFiveWinRecord.pay);
        BlackboardUtils.SetOrCreateValue(bb, "state", luckyFiveWinRecord.state);
    }

    public static void Serialize(IBlackboard bb, MetaEligibleBetThreshold metaEligibleBetThreshold)
    {
        if (metaEligibleBetThreshold == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "hiddenUniverseBetList", metaEligibleBetThreshold.hiddenUniverseBetList);
        if (metaEligibleBetThreshold.vipLounge != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "vipLounge"), metaEligibleBetThreshold.vipLounge);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "vipLounge");
        }
        BlackboardUtils.SetOrCreateValue(bb, "epicPassV2BetAmount", metaEligibleBetThreshold.epicPassV2BetAmount);
    }

    public static void Serialize(IBlackboard bb, MetaGameCompensation metaGameCompensation)
    {
        if (metaGameCompensation == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "hogDeal", metaGameCompensation.hogDeal);
    }

    public static void Serialize(IBlackboard bb, MetaGameEnterInfoRequest metaGameEnterInfoRequest)
    {
        if (metaGameEnterInfoRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", metaGameEnterInfoRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", metaGameEnterInfoRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "gameId", metaGameEnterInfoRequest.gameId);
    }

    public static void Serialize(IBlackboard bb, MetaGameEnterInfoResponseV2 metaGameEnterInfoResponseV2)
    {
        if (metaGameEnterInfoResponseV2 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", metaGameEnterInfoResponseV2.error);
        if (metaGameEnterInfoResponseV2.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), metaGameEnterInfoResponseV2.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", metaGameEnterInfoResponseV2.serverTime);
        if (metaGameEnterInfoResponseV2.metaGameEnterInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "metaGameEnterInfo"), metaGameEnterInfoResponseV2.metaGameEnterInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "metaGameEnterInfo");
        }
    }

    public static void Serialize(IBlackboard bb, MetaGameEnterInfoV1 metaGameEnterInfoV1)
    {
        if (metaGameEnterInfoV1 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "type", metaGameEnterInfoV1.type);
        if (metaGameEnterInfoV1.info != null)
        {
            switch (metaGameEnterInfoV1.type)
            {
            case EventInfoType.LUCKY_FIVE:
                Serialize(bb, (LuckyFiveEnterInfo)metaGameEnterInfoV1.info);
                break;

            case EventInfoType.COLLECTING_GAME:
                Serialize(bb, (CollectingGameEnterInfo)metaGameEnterInfoV1.info);
                break;

            case EventInfoType.SEASON_PASS:
                Serialize(bb, (SeasonPassEnterInfo)metaGameEnterInfoV1.info);
                break;

            case EventInfoType.BOSS_RAIDERS:
                Serialize(bb, (BossRaidersEnterInfoV1)metaGameEnterInfoV1.info);
                break;

            case EventInfoType.CLUB_ARENA:
                Serialize(bb, (ClubArenaEnterInfo)metaGameEnterInfoV1.info);
                break;
            }
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "info");
        }
        if (metaGameEnterInfoV1.gemJackpotEnterInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "gemJackpotEnterInfo"), metaGameEnterInfoV1.gemJackpotEnterInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "gemJackpotEnterInfo");
        }
    }

    public static void Serialize(IBlackboard bb, MetaGameInfoV1 metaGameInfoV1)
    {
        if (metaGameInfoV1 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "type", metaGameInfoV1.type);
        if (metaGameInfoV1.info != null)
        {
            switch (metaGameInfoV1.type)
            {
            case EventInfoType.LUCKY_FIVE:
                Serialize(bb, (LuckyFiveWinInfo)metaGameInfoV1.info);
                break;

            case EventInfoType.COLLECTING_GAME:
                Serialize(bb, (CollectingGamePackUpdateInfo)metaGameInfoV1.info);
                break;

            case EventInfoType.SEASON_PASS:
                Serialize(bb, (SeasonPassPointUpdateInfo)metaGameInfoV1.info);
                break;

            case EventInfoType.GEM_JACKPOT:
                Serialize(bb, (GemJackpotInfo)metaGameInfoV1.info);
                break;

            case EventInfoType.BOSS_RAIDERS:
                Serialize(bb, (BossRaidersUpdateInfoV1)metaGameInfoV1.info);
                break;

            case EventInfoType.CLUB_ARENA:
                Serialize(bb, (ClubArenaEnergyUpdateInfo)metaGameInfoV1.info);
                break;
            }
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "info");
        }
    }

    public static void Serialize(IBlackboard bb, MetaJackpotInfo metaJackpotInfo)
    {
        if (metaJackpotInfo == null) { return; }
        BlackboardUtils.SetOrCreateList(bb, "jackpotList", metaJackpotInfo.jackpotList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "spinCount", metaJackpotInfo.spinCount);
    }

    public static void Serialize(IBlackboard bb, MetaJackpotRequest metaJackpotRequest)
    {
        if (metaJackpotRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", metaJackpotRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "jackpotType", metaJackpotRequest.jackpotType);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", metaJackpotRequest.ackMask);
    }

    public static void Serialize(IBlackboard bb, MetaJackpotResponseV1 metaJackpotResponseV1)
    {
        if (metaJackpotResponseV1 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", metaJackpotResponseV1.error);
        if (metaJackpotResponseV1.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), metaJackpotResponseV1.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", metaJackpotResponseV1.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "jackpotList", metaJackpotResponseV1.jackpotList, Serialize);
    }

    public static void Serialize(IBlackboard bb, MetaUpdateInfo metaUpdateInfo)
    {
        if (metaUpdateInfo == null) { return; }
        if (metaUpdateInfo.tournamentInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "tournamentInfo"), metaUpdateInfo.tournamentInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "tournamentInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "nextMysteryGiftLevel", metaUpdateInfo.nextMysteryGiftLevel);
        if (metaUpdateInfo.mysteryGiftInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "mysteryGiftInfo"), metaUpdateInfo.mysteryGiftInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "mysteryGiftInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "featureUnlockList", metaUpdateInfo.featureUnlockList);
        BlackboardUtils.SetOrCreateValue(bb, "gameDealCount", metaUpdateInfo.gameDealCount);
        BlackboardUtils.SetOrCreateValue(bb, "bonusDealCount", metaUpdateInfo.bonusDealCount);
    }

    public static void Serialize(IBlackboard bb, Misc misc)
    {
        if (misc == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "ROOM_PLAYERS_TIMEOUT_MS", misc.ROOM_PLAYERS_TIMEOUT_MS);
        BlackboardUtils.SetOrCreateValue(bb, "FRIEND_COUNT_MAX", misc.FRIEND_COUNT_MAX);
        BlackboardUtils.SetOrCreateValue(bb, "CREDIT_GIFT_AMOUNT", misc.CREDIT_GIFT_AMOUNT);
        BlackboardUtils.SetOrCreateValue(bb, "CREDIT_GIFT_AMOUNT_FB", misc.CREDIT_GIFT_AMOUNT_FB);
        if (misc.LOCAL_PUSH != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "LOCAL_PUSH"), misc.LOCAL_PUSH);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "LOCAL_PUSH");
        }
        BlackboardUtils.SetOrCreateValue(bb, "REPORT_THRESHOLD", misc.REPORT_THRESHOLD);
        BlackboardUtils.SetOrCreateValue(bb, "FACEBOOK_PAGE_URL", misc.FACEBOOK_PAGE_URL);
        BlackboardUtils.SetOrCreateValue(bb, "AGE_GATE_THRESHOLD", misc.AGE_GATE_THRESHOLD);
        if (misc.FACEBOOK_SHARE_IMAGE_URL != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "FACEBOOK_SHARE_IMAGE_URL"), misc.FACEBOOK_SHARE_IMAGE_URL);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "FACEBOOK_SHARE_IMAGE_URL");
        }
        BlackboardUtils.SetOrCreateValue(bb, "FRIEND_INVITE_RESTRICTION_SEC", misc.FRIEND_INVITE_RESTRICTION_SEC);
        BlackboardUtils.SetOrCreateValue(bb, "PIGGY_BANK_INFO_APPEAR_SECTION_LIST", misc.PIGGY_BANK_INFO_APPEAR_SECTION_LIST);
        BlackboardUtils.SetOrCreateValue(bb, "PIGGY_BANK_APPEAR_COIN_COUNT", misc.PIGGY_BANK_APPEAR_COIN_COUNT);
        BlackboardUtils.SetOrCreateValue(bb, "AGE_GATE_OCCUR", misc.AGE_GATE_OCCUR);
        BlackboardUtils.SetOrCreateValue(bb, "SHOW_DAILY_BOOST_ITEM", misc.SHOW_DAILY_BOOST_ITEM);
        BlackboardUtils.SetOrCreateValue(bb, "PIGGY_BANK_ENABLED", misc.PIGGY_BANK_ENABLED);
        BlackboardUtils.SetOrCreateValue(bb, "ENABLE_GAME_SPIN", misc.ENABLE_GAME_SPIN);
        BlackboardUtils.SetOrCreateValue(bb, "FACEBOOK_APP_LINK_URL", misc.FACEBOOK_APP_LINK_URL);
        BlackboardUtils.SetOrCreateValue(bb, "FRIEND_GIFT_SEND_INTERVAL_SEC", misc.FRIEND_GIFT_SEND_INTERVAL_SEC);
        BlackboardUtils.SetOrCreateValue(bb, "FUNNEL_SPIN_COUNT_LIST", misc.FUNNEL_SPIN_COUNT_LIST);
        BlackboardUtils.SetOrCreateValue(bb, "TUTORIAL_ENABLED", misc.TUTORIAL_ENABLED);
        BlackboardUtils.SetOrCreateValue(bb, "SUPPORT_PAGE_URL", misc.SUPPORT_PAGE_URL);
        BlackboardUtils.SetOrCreateValue(bb, "LOBBY_PLAYERS_TIMEOUT_MS", misc.LOBBY_PLAYERS_TIMEOUT_MS);
        BlackboardUtils.SetOrCreateValue(bb, "VERSION_UPDATE_REWARD_CREDIT", misc.VERSION_UPDATE_REWARD_CREDIT);
        BlackboardUtils.SetOrCreateValue(bb, "NUDGE_COOLTIME_MS", misc.NUDGE_COOLTIME_MS);
        BlackboardUtils.SetOrCreateValue(bb, "SHOW_FACEBOOK_CONNECT_BUTTON_ON_FIRST_LOGIN", misc.SHOW_FACEBOOK_CONNECT_BUTTON_ON_FIRST_LOGIN);
        BlackboardUtils.SetOrCreateValue(bb, "ENABLE_LUCKY_SPIN", misc.ENABLE_LUCKY_SPIN);
        BlackboardUtils.SetOrCreateValue(bb, "ENABLE_TOURNAMENT", misc.ENABLE_TOURNAMENT);
        BlackboardUtils.SetOrCreateValue(bb, "VIDEO_ADS_ENABLED", misc.VIDEO_ADS_ENABLED);
        BlackboardUtils.SetOrCreateValue(bb, "ENABLE_WALL_OF_EPIC", misc.ENABLE_WALL_OF_EPIC);
        BlackboardUtils.SetOrCreateValue(bb, "CLIENT_ASSET_DOWNLOAD_RETRY_ENABLED", misc.CLIENT_ASSET_DOWNLOAD_RETRY_ENABLED);
        BlackboardUtils.SetOrCreateValue(bb, "CLIENT_ASSET_DOWNLOAD_RETRY_TIMEOUT_SEC", misc.CLIENT_ASSET_DOWNLOAD_RETRY_TIMEOUT_SEC);
        BlackboardUtils.SetOrCreateValue(bb, "ENABLE_CLIENT_STORAGE_CHECK", misc.ENABLE_CLIENT_STORAGE_CHECK);
        BlackboardUtils.SetOrCreateValue(bb, "CLIENT_STORAGE_CHECK_MIN_MB", misc.CLIENT_STORAGE_CHECK_MIN_MB);
        BlackboardUtils.SetOrCreateValue(bb, "HIDE_PROFILE_REPORT_THRESHOLD", misc.HIDE_PROFILE_REPORT_THRESHOLD);
        BlackboardUtils.SetOrCreateValue(bb, "CHALLENGE_MIN_COUNT", misc.CHALLENGE_MIN_COUNT);
        BlackboardUtils.SetOrCreateValue(bb, "PERSONAL_PASSIVE_EVENT_ID_OFFSET", misc.PERSONAL_PASSIVE_EVENT_ID_OFFSET);
        BlackboardUtils.SetOrCreateValue(bb, "INHOUSE_ADS_ENABLED", misc.INHOUSE_ADS_ENABLED);
        BlackboardUtils.SetOrCreateValue(bb, "FACEBOOK_GAMEREQUEST_TITLE", misc.FACEBOOK_GAMEREQUEST_TITLE);
        BlackboardUtils.SetOrCreateValue(bb, "FACEBOOK_GAMEREQUEST_MESSAGE", misc.FACEBOOK_GAMEREQUEST_MESSAGE);
        BlackboardUtils.SetOrCreateValue(bb, "PROBS_ENABLE", misc.PROBS_ENABLE);
        BlackboardUtils.SetOrCreateValue(bb, "AUTO_BET_SELECT_HIGH_LEVEL_START", misc.AUTO_BET_SELECT_HIGH_LEVEL_START);
        BlackboardUtils.SetOrCreateValue(bb, "AUTO_BET_SELECT_MULTIPLIER_LOW_LEVEL", misc.AUTO_BET_SELECT_MULTIPLIER_LOW_LEVEL);
        BlackboardUtils.SetOrCreateValue(bb, "AUTO_BET_SELECT_MULTIPLIER_HIGH_LEVEL", misc.AUTO_BET_SELECT_MULTIPLIER_HIGH_LEVEL);
        BlackboardUtils.SetOrCreateValue(bb, "POLICY_VERSION", misc.POLICY_VERSION);
        BlackboardUtils.SetOrCreateValue(bb, "MAX_NOTICE_EXPOSURE_COUNT", misc.MAX_NOTICE_EXPOSURE_COUNT);
        BlackboardUtils.SetOrCreateValue(bb, "STATUS_MATCH_MENU_ENABLED", misc.STATUS_MATCH_MENU_ENABLED);
        BlackboardUtils.SetOrCreateValue(bb, "ENABLE_CLUB_RESTRICTION", misc.ENABLE_CLUB_RESTRICTION);
        BlackboardUtils.SetOrCreateValue(bb, "FACEBOOK_SHARE_REWARD_MAX_CREDIT", misc.FACEBOOK_SHARE_REWARD_MAX_CREDIT);
        BlackboardUtils.SetOrCreateValue(bb, "ENABLE_VIP_DEAL", misc.ENABLE_VIP_DEAL);
        BlackboardUtils.SetOrCreateValue(bb, "UNLOCK_GAME_GEM_COST", misc.UNLOCK_GAME_GEM_COST);
        BlackboardUtils.SetOrCreateValue(bb, "GLOBAL_DENOMINATOR", misc.GLOBAL_DENOMINATOR);
        BlackboardUtils.SetOrCreateValue(bb, "ENABLE_EPIC_ALBUM", misc.ENABLE_EPIC_ALBUM);
        BlackboardUtils.SetOrCreateValue(bb, "GEM_VALUE_FOR_COIN", misc.GEM_VALUE_FOR_COIN);
        BlackboardUtils.SetOrCreateValue(bb, "GEM_VALUE_FOR_COIN_DISCOUNT_NUMERATOR", misc.GEM_VALUE_FOR_COIN_DISCOUNT_NUMERATOR);
        BlackboardUtils.SetOrCreateValue(bb, "ENABLE_SHOW_PERCENTAGE_COIN_SHOP", misc.ENABLE_SHOW_PERCENTAGE_COIN_SHOP);
        BlackboardUtils.SetOrCreateValue(bb, "ENABLE_SHOW_PERCENTAGE_GEM_SHOP", misc.ENABLE_SHOW_PERCENTAGE_GEM_SHOP);
        BlackboardUtils.SetOrCreateValue(bb, "ENABLE_SHOW_PERCENTAGE_POG", misc.ENABLE_SHOW_PERCENTAGE_POG);
        BlackboardUtils.SetOrCreateValue(bb, "ENABLE_SHOW_PERCENTAGE_WHEEL", misc.ENABLE_SHOW_PERCENTAGE_WHEEL);
        if (misc.SPEAKER != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "SPEAKER"), misc.SPEAKER);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "SPEAKER");
        }
        BlackboardUtils.SetOrCreateValue(bb, "PORTRAIT_POW_NUMBER", misc.PORTRAIT_POW_NUMBER);
        BlackboardUtils.SetOrCreateValue(bb, "ENABLE_SHOW_PERCENTAGE_BUY_A_BONUS", misc.ENABLE_SHOW_PERCENTAGE_BUY_A_BONUS);
        BlackboardUtils.SetOrCreateValue(bb, "INFLATION_NUMERATOR_SHOP", misc.INFLATION_NUMERATOR_SHOP);
        BlackboardUtils.SetOrCreateValue(bb, "INFLATION_NUMERATOR_WHEEL", misc.INFLATION_NUMERATOR_WHEEL);
        BlackboardUtils.SetOrCreateValue(bb, "INFLATION_NUMERATOR_POG", misc.INFLATION_NUMERATOR_POG);
        BlackboardUtils.SetOrCreateValue(bb, "ENABLE_BAB_ONE_CLICK_PURCHASE", misc.ENABLE_BAB_ONE_CLICK_PURCHASE);
        BlackboardUtils.SetOrCreateValue(bb, "ENABLE_SHOW_PERCENTAGE_GEM_BAB_PROMOTION_SHOP", misc.ENABLE_SHOW_PERCENTAGE_GEM_BAB_PROMOTION_SHOP);
        BlackboardUtils.SetOrCreateValue(bb, "CLIENT_ANALYTIC_EVENT_SENDER_AWS_ACCESS_KEY", misc.CLIENT_ANALYTIC_EVENT_SENDER_AWS_ACCESS_KEY);
        BlackboardUtils.SetOrCreateValue(bb, "CLIENT_ANALYTIC_EVENT_SENDER_AWS_SECRET_KEY", misc.CLIENT_ANALYTIC_EVENT_SENDER_AWS_SECRET_KEY);
        BlackboardUtils.SetOrCreateValue(bb, "CLIENT_ANALYTIC_EVENT_SENDER_AWS_HOST", misc.CLIENT_ANALYTIC_EVENT_SENDER_AWS_HOST);
        BlackboardUtils.SetOrCreateValue(bb, "CLIENT_ANALYTIC_EVENT_SENDER_APP_NAME", misc.CLIENT_ANALYTIC_EVENT_SENDER_APP_NAME);
        BlackboardUtils.SetOrCreateValue(bb, "CLIENT_ANALYTIC_EVENT_SENDER_ENV", misc.CLIENT_ANALYTIC_EVENT_SENDER_ENV);
        if (misc.TUTORIAL_TIMER_SEC != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "TUTORIAL_TIMER_SEC"), misc.TUTORIAL_TIMER_SEC);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "TUTORIAL_TIMER_SEC");
        }
        BlackboardUtils.SetOrCreateValue(bb, "WELCOME_KUDO_COOLTIME_MS", misc.WELCOME_KUDO_COOLTIME_MS);
        BlackboardUtils.SetOrCreateValue(bb, "ENABLE_SHOW_PERCENTAGE_GEM_JACKPOT", misc.ENABLE_SHOW_PERCENTAGE_GEM_JACKPOT);
        BlackboardUtils.SetOrCreateValue(bb, "TERMS_OF_USE_VERSION", misc.TERMS_OF_USE_VERSION);
        BlackboardUtils.SetOrCreateValue(bb, "ENABLE_TERMS_OF_USE", misc.ENABLE_TERMS_OF_USE);
        BlackboardUtils.SetOrCreateValue(bb, "ENABLE_LEAGUE_RANK_POPUP", misc.ENABLE_LEAGUE_RANK_POPUP);
        BlackboardUtils.SetOrCreateValue(bb, "SHOP_COIN_ICON_SECTION_LIST", misc.SHOP_COIN_ICON_SECTION_LIST);
        BlackboardUtils.SetOrCreateValue(bb, "SHOP_GEM_ICON_SECTION_LIST", misc.SHOP_GEM_ICON_SECTION_LIST);
        BlackboardUtils.SetOrCreateValue(bb, "ENABLE_INVITE_INSTALL", misc.ENABLE_INVITE_INSTALL);
        BlackboardUtils.SetOrCreateValue(bb, "CLUB_REJOIN_COOLTIME_MS", misc.CLUB_REJOIN_COOLTIME_MS);
        BlackboardUtils.SetOrCreateValue(bb, "ENABLE_NATIVE_APP_TRACKING_TRANSPARENCY", misc.ENABLE_NATIVE_APP_TRACKING_TRANSPARENCY);
        BlackboardUtils.SetOrCreateValue(bb, "CLUB_ARENA_RANK_POPUP_COOLTIME_MS", misc.CLUB_ARENA_RANK_POPUP_COOLTIME_MS);
        BlackboardUtils.SetOrCreateValue(bb, "BOSS_RAIDERS_RANK_POPUP_COOLTIME_MS", misc.BOSS_RAIDERS_RANK_POPUP_COOLTIME_MS);
        BlackboardUtils.SetOrCreateValue(bb, "ENABLE_META_EVENT_LEVEL_LOCK", misc.ENABLE_META_EVENT_LEVEL_LOCK);
        BlackboardUtils.SetOrCreateValue(bb, "ENABLE_NEED_UPDATE_META_ICON", misc.ENABLE_NEED_UPDATE_META_ICON);
        if (misc.VIP_LOUNGE_MISC != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "VIP_LOUNGE_MISC"), misc.VIP_LOUNGE_MISC);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "VIP_LOUNGE_MISC");
        }
        BlackboardUtils.SetOrCreateValue(bb, "ENABLE_SEASON_PASS_V2", misc.ENABLE_SEASON_PASS_V2);
        BlackboardUtils.SetOrCreateValue(bb, "ENABLE_BUCKS_SHOP_UI", misc.ENABLE_BUCKS_SHOP_UI);
        BlackboardUtils.SetOrCreateValue(bb, "ENABLE_VIP_DEAL_V2", misc.ENABLE_VIP_DEAL_V2);
        BlackboardUtils.SetOrCreateValue(bb, "ENABLE_VIP_DEAL_V2_FREEBIE_TIER_MULTI", misc.ENABLE_VIP_DEAL_V2_FREEBIE_TIER_MULTI);
    }

    public static void Serialize(IBlackboard bb, MysteryGiftInfo mysteryGiftInfo)
    {
        if (mysteryGiftInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "level", mysteryGiftInfo.level);
        BlackboardUtils.SetOrCreateList(bb, "rewardResultList", mysteryGiftInfo.rewardResultList, Serialize);
    }

    public static void Serialize(IBlackboard bb, Notice notice)
    {
        if (notice == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "id", notice.id);
        BlackboardUtils.SetOrCreateValue(bb, "title", notice.title);
        BlackboardUtils.SetOrCreateValue(bb, "message", notice.message);
        BlackboardUtils.SetOrCreateValue(bb, "clientOsSet", notice.clientOsSet);
        BlackboardUtils.SetOrCreateValue(bb, "createTimestamp", notice.createTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "type", notice.type);
        if (notice.constraints != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "constraints"), notice.constraints);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "constraints");
        }
        BlackboardUtils.SetOrCreateValue(bb, "priority", notice.priority);
        BlackboardUtils.SetOrCreateValue(bb, "imageUrl", notice.imageUrl);
        BlackboardUtils.SetOrCreateValue(bb, "startTimestamp", notice.startTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "endTimestamp", notice.endTimestamp);
        if (notice.action != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "action"), notice.action);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "action");
        }
        BlackboardUtils.SetOrCreateValue(bb, "abTestTag", notice.abTestTag);
    }

    public static void Serialize(IBlackboard bb, NoticeConstraints noticeConstraints)
    {
        if (noticeConstraints == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "showEndTimer", noticeConstraints.showEndTimer);
        BlackboardUtils.SetOrCreateValue(bb, "useUserTimer", noticeConstraints.useUserTimer);
        BlackboardUtils.SetOrCreateValue(bb, "userTimer", noticeConstraints.userTimer);
        BlackboardUtils.SetOrCreateValue(bb, "cooltimeSec", noticeConstraints.cooltimeSec);
        BlackboardUtils.SetOrCreateValue(bb, "timerPosX", noticeConstraints.timerPosX);
        BlackboardUtils.SetOrCreateValue(bb, "timerPosY", noticeConstraints.timerPosY);
        BlackboardUtils.SetOrCreateValue(bb, "maxExposureCount", noticeConstraints.maxExposureCount);
    }

    public static void Serialize(IBlackboard bb, ObjectInfo objectInfo)
    {
        if (objectInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "chapter", objectInfo.chapter);
        BlackboardUtils.SetOrCreateValue(bb, "stage", objectInfo.stage);
        BlackboardUtils.SetOrCreateValue(bb, "index", objectInfo.index);
        BlackboardUtils.SetOrCreateValue(bb, "score", objectInfo.score);
        BlackboardUtils.SetOrCreateValue(bb, "name", objectInfo.name);
    }

    public static void Serialize(IBlackboard bb, PaidVipDeal paidVipDeal)
    {
        if (paidVipDeal == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "dealUuid", paidVipDeal.dealUuid);
        BlackboardUtils.SetOrCreateValue(bb, "productId", paidVipDeal.productId);
        BlackboardUtils.SetOrCreateValue(bb, "isSoldOut", paidVipDeal.isSoldOut);
        BlackboardUtils.SetOrCreateValue(bb, "multiplierNumerator", paidVipDeal.multiplierNumerator);
        if (paidVipDeal.product != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "product"), paidVipDeal.product);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "product");
        }
    }

    public static void Serialize(IBlackboard bb, PiggyBankContribution piggyBankContribution)
    {
        if (piggyBankContribution == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "intervalEnd", piggyBankContribution.intervalEnd);
        BlackboardUtils.SetOrCreateValue(bb, "contribution", piggyBankContribution.contribution);
    }

    public static void Serialize(IBlackboard bb, PollDataBonusTrigger pollDataBonusTrigger)
    {
        if (pollDataBonusTrigger == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "userId", pollDataBonusTrigger.userId);
        BlackboardUtils.SetOrCreateValue(bb, "bonusId", pollDataBonusTrigger.bonusId);
        BlackboardUtils.SetOrCreateValue(bb, "userName", pollDataBonusTrigger.userName);
    }

    public static void Serialize(IBlackboard bb, PollDataBossRaidersEnd pollDataBossRaidersEnd)
    {
        if (pollDataBossRaidersEnd == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "themeId", pollDataBossRaidersEnd.themeId);
    }

    public static void Serialize(IBlackboard bb, PollDataBossRaidersRoundComplete pollDataBossRaidersRoundComplete)
    {
        if (pollDataBossRaidersRoundComplete == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "themeId", pollDataBossRaidersRoundComplete.themeId);
    }

    public static void Serialize(IBlackboard bb, PollDataChallengeMissionCompleteV1 pollDataChallengeMissionCompleteV1)
    {
        if (pollDataChallengeMissionCompleteV1 == null) { return; }
        if (pollDataChallengeMissionCompleteV1.rewardResult != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "rewardResult"), pollDataChallengeMissionCompleteV1.rewardResult);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "rewardResult");
        }
        if (pollDataChallengeMissionCompleteV1.mission != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "mission"), pollDataChallengeMissionCompleteV1.mission);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "mission");
        }
        if (pollDataChallengeMissionCompleteV1.challenge != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "challenge"), pollDataChallengeMissionCompleteV1.challenge);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "challenge");
        }
    }

    public static void Serialize(IBlackboard bb, PollDataClubArenaEnd pollDataClubArenaEnd)
    {
        if (pollDataClubArenaEnd == null) { return; }
    }

    public static void Serialize(IBlackboard bb, PollDataClubArenaOneDayLeft pollDataClubArenaOneDayLeft)
    {
        if (pollDataClubArenaOneDayLeft == null) { return; }
    }

    public static void Serialize(IBlackboard bb, PollDataClubArenaRankingUp pollDataClubArenaRankingUp)
    {
        if (pollDataClubArenaRankingUp == null) { return; }
    }

    public static void Serialize(IBlackboard bb, PollDataClubChallengeMissionComplete pollDataClubChallengeMissionComplete)
    {
        if (pollDataClubChallengeMissionComplete == null) { return; }
        if (pollDataClubChallengeMissionComplete.rewardResult != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "rewardResult"), pollDataClubChallengeMissionComplete.rewardResult);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "rewardResult");
        }
        if (pollDataClubChallengeMissionComplete.mission != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "mission"), pollDataClubChallengeMissionComplete.mission);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "mission");
        }
        if (pollDataClubChallengeMissionComplete.challenge != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "challenge"), pollDataClubChallengeMissionComplete.challenge);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "challenge");
        }
    }

    public static void Serialize(IBlackboard bb, PollDataClubColeaderChange pollDataClubColeaderChange)
    {
        if (pollDataClubColeaderChange == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "userId", pollDataClubColeaderChange.userId);
        BlackboardUtils.SetOrCreateValue(bb, "promote", pollDataClubColeaderChange.promote);
        BlackboardUtils.SetOrCreateValue(bb, "clubSymbol", pollDataClubColeaderChange.clubSymbol);
        BlackboardUtils.SetOrCreateValue(bb, "clubLeagueTier", pollDataClubColeaderChange.clubLeagueTier);
    }

    public static void Serialize(IBlackboard bb, PollDataClubDeal pollDataClubDeal)
    {
        if (pollDataClubDeal == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "name", pollDataClubDeal.name);
        BlackboardUtils.SetOrCreateValue(bb, "profileUrl", pollDataClubDeal.profileUrl);
        BlackboardUtils.SetOrCreateValue(bb, "clubSymbol", pollDataClubDeal.clubSymbol);
        BlackboardUtils.SetOrCreateValue(bb, "earnCredit", pollDataClubDeal.earnCredit);
        BlackboardUtils.SetOrCreateValue(bb, "tier", pollDataClubDeal.tier);
        BlackboardUtils.SetOrCreateValue(bb, "userId", pollDataClubDeal.userId);
        BlackboardUtils.SetOrCreateValue(bb, "kudoId", pollDataClubDeal.kudoId);
        BlackboardUtils.SetOrCreateValue(bb, "clubLeagueTier", pollDataClubDeal.clubLeagueTier);
    }

    public static void Serialize(IBlackboard bb, PollDataClubInvite pollDataClubInvite)
    {
        if (pollDataClubInvite == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "userId", pollDataClubInvite.userId);
        BlackboardUtils.SetOrCreateValue(bb, "name", pollDataClubInvite.name);
        BlackboardUtils.SetOrCreateValue(bb, "clubLeagueTier", pollDataClubInvite.clubLeagueTier);
        BlackboardUtils.SetOrCreateValue(bb, "clubName", pollDataClubInvite.clubName);
        BlackboardUtils.SetOrCreateValue(bb, "clubSymbol", pollDataClubInvite.clubSymbol);
    }

    public static void Serialize(IBlackboard bb, PollDataClubMemberLogin pollDataClubMemberLogin)
    {
        if (pollDataClubMemberLogin == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "userId", pollDataClubMemberLogin.userId);
        BlackboardUtils.SetOrCreateValue(bb, "name", pollDataClubMemberLogin.name);
    }

    public static void Serialize(IBlackboard bb, PollDataClubRequest pollDataClubRequest)
    {
        if (pollDataClubRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "userId", pollDataClubRequest.userId);
        BlackboardUtils.SetOrCreateValue(bb, "profileUrl", pollDataClubRequest.profileUrl);
        BlackboardUtils.SetOrCreateValue(bb, "tier", pollDataClubRequest.tier);
    }

    public static void Serialize(IBlackboard bb, PollDataClubRequestAccepted pollDataClubRequestAccepted)
    {
        if (pollDataClubRequestAccepted == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "clubId", pollDataClubRequestAccepted.clubId);
        BlackboardUtils.SetOrCreateValue(bb, "clubSymbol", pollDataClubRequestAccepted.clubSymbol);
        BlackboardUtils.SetOrCreateValue(bb, "clubLeagueTier", pollDataClubRequestAccepted.clubLeagueTier);
    }

    public static void Serialize(IBlackboard bb, PollDataClubShare pollDataClubShare)
    {
        if (pollDataClubShare == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "name", pollDataClubShare.name);
        BlackboardUtils.SetOrCreateValue(bb, "profileUrl", pollDataClubShare.profileUrl);
        BlackboardUtils.SetOrCreateValue(bb, "eventType", pollDataClubShare.eventType);
        BlackboardUtils.SetOrCreateValue(bb, "kudoId", pollDataClubShare.kudoId);
        BlackboardUtils.SetOrCreateValue(bb, "tier", pollDataClubShare.tier);
        BlackboardUtils.SetOrCreateValue(bb, "clubSymbol", pollDataClubShare.clubSymbol);
        BlackboardUtils.SetOrCreateValue(bb, "eventPresetId", pollDataClubShare.eventPresetId);
        BlackboardUtils.SetOrCreateValue(bb, "userId", pollDataClubShare.userId);
        BlackboardUtils.SetOrCreateValue(bb, "clubLeagueTier", pollDataClubShare.clubLeagueTier);
    }

    public static void Serialize(IBlackboard bb, PollDataCommunityGameTicketEarn pollDataCommunityGameTicketEarn)
    {
        if (pollDataCommunityGameTicketEarn == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "userId", pollDataCommunityGameTicketEarn.userId);
        BlackboardUtils.SetOrCreateValue(bb, "ticketCount", pollDataCommunityGameTicketEarn.ticketCount);
    }

    public static void Serialize(IBlackboard bb, PollDataCustomUserInGameAction pollDataCustomUserInGameAction)
    {
        if (pollDataCustomUserInGameAction == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "userId", pollDataCustomUserInGameAction.userId);
        BlackboardUtils.SetOrCreateValue(bb, "customUserInGameActionType", pollDataCustomUserInGameAction.customUserInGameActionType);
    }

    public static void Serialize(IBlackboard bb, PollDataFirstGlobalChat pollDataFirstGlobalChat)
    {
        if (pollDataFirstGlobalChat == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "userId", pollDataFirstGlobalChat.userId);
        BlackboardUtils.SetOrCreateValue(bb, "name", pollDataFirstGlobalChat.name);
        BlackboardUtils.SetOrCreateValue(bb, "profileUrl", pollDataFirstGlobalChat.profileUrl);
        BlackboardUtils.SetOrCreateValue(bb, "kudoId", pollDataFirstGlobalChat.kudoId);
        BlackboardUtils.SetOrCreateValue(bb, "tier", pollDataFirstGlobalChat.tier);
    }

    public static void Serialize(IBlackboard bb, PollDataFriendConnect pollDataFriendConnect)
    {
        if (pollDataFriendConnect == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "userId", pollDataFriendConnect.userId);
        BlackboardUtils.SetOrCreateValue(bb, "name", pollDataFriendConnect.name);
    }

    public static void Serialize(IBlackboard bb, PollDataFriendInvite pollDataFriendInvite)
    {
        if (pollDataFriendInvite == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "userId", pollDataFriendInvite.userId);
        BlackboardUtils.SetOrCreateValue(bb, "profileUrl", pollDataFriendInvite.profileUrl);
        BlackboardUtils.SetOrCreateValue(bb, "name", pollDataFriendInvite.name);
        BlackboardUtils.SetOrCreateValue(bb, "tier", pollDataFriendInvite.tier);
        BlackboardUtils.SetOrCreateValue(bb, "gameId", pollDataFriendInvite.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "roomId", pollDataFriendInvite.roomId);
        BlackboardUtils.SetOrCreateValue(bb, "reportCount", pollDataFriendInvite.reportCount);
        BlackboardUtils.SetOrCreateValue(bb, "profileHighResolutionUrl", pollDataFriendInvite.profileHighResolutionUrl);
        BlackboardUtils.SetOrCreateValue(bb, "inviteType", pollDataFriendInvite.inviteType);
    }

    public static void Serialize(IBlackboard bb, PollDataHiddenUniverseFinderGift pollDataHiddenUniverseFinderGift)
    {
        if (pollDataHiddenUniverseFinderGift == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "name", pollDataHiddenUniverseFinderGift.name);
        BlackboardUtils.SetOrCreateValue(bb, "profileUrl", pollDataHiddenUniverseFinderGift.profileUrl);
        BlackboardUtils.SetOrCreateValue(bb, "tier", pollDataHiddenUniverseFinderGift.tier);
        BlackboardUtils.SetOrCreateValue(bb, "clubSymbol", pollDataHiddenUniverseFinderGift.clubSymbol);
        BlackboardUtils.SetOrCreateValue(bb, "userId", pollDataHiddenUniverseFinderGift.userId);
        BlackboardUtils.SetOrCreateValue(bb, "clubLeagueTier", pollDataHiddenUniverseFinderGift.clubLeagueTier);
        BlackboardUtils.SetOrCreateValue(bb, "kudoId", pollDataHiddenUniverseFinderGift.kudoId);
    }

    public static void Serialize(IBlackboard bb, PollDataJackpotWin pollDataJackpotWin)
    {
        if (pollDataJackpotWin == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "userId", pollDataJackpotWin.userId);
        BlackboardUtils.SetOrCreateValue(bb, "profileUrl", pollDataJackpotWin.profileUrl);
        BlackboardUtils.SetOrCreateValue(bb, "name", pollDataJackpotWin.name);
        BlackboardUtils.SetOrCreateValue(bb, "tier", pollDataJackpotWin.tier);
        BlackboardUtils.SetOrCreateValue(bb, "winCredit", pollDataJackpotWin.winCredit);
        BlackboardUtils.SetOrCreateValue(bb, "gameId", pollDataJackpotWin.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "jackpotKudoType", pollDataJackpotWin.jackpotKudoType);
        BlackboardUtils.SetOrCreateValue(bb, "kudoId", pollDataJackpotWin.kudoId);
    }

    public static void Serialize(IBlackboard bb, PollDataKudoReceive pollDataKudoReceive)
    {
        if (pollDataKudoReceive == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "userId", pollDataKudoReceive.userId);
        BlackboardUtils.SetOrCreateValue(bb, "name", pollDataKudoReceive.name);
        BlackboardUtils.SetOrCreateValue(bb, "tier", pollDataKudoReceive.tier);
        BlackboardUtils.SetOrCreateValue(bb, "profileUrl", pollDataKudoReceive.profileUrl);
        BlackboardUtils.SetOrCreateValue(bb, "kudoId", pollDataKudoReceive.kudoId);
        BlackboardUtils.SetOrCreateValue(bb, "kudoLikeType", pollDataKudoReceive.kudoLikeType);
    }

    public static void Serialize(IBlackboard bb, PollDataLevelUp pollDataLevelUp)
    {
        if (pollDataLevelUp == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "userId", pollDataLevelUp.userId);
        BlackboardUtils.SetOrCreateValue(bb, "level", pollDataLevelUp.level);
    }

    public static void Serialize(IBlackboard bb, PollDataLike pollDataLike)
    {
        if (pollDataLike == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "userId", pollDataLike.userId);
        BlackboardUtils.SetOrCreateValue(bb, "targetUserId", pollDataLike.targetUserId);
    }

    public static void Serialize(IBlackboard bb, PollDataMetaJackpotWin pollDataMetaJackpotWin)
    {
        if (pollDataMetaJackpotWin == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "userId", pollDataMetaJackpotWin.userId);
        BlackboardUtils.SetOrCreateValue(bb, "profileUrl", pollDataMetaJackpotWin.profileUrl);
        BlackboardUtils.SetOrCreateValue(bb, "name", pollDataMetaJackpotWin.name);
        BlackboardUtils.SetOrCreateValue(bb, "tier", pollDataMetaJackpotWin.tier);
        BlackboardUtils.SetOrCreateValue(bb, "winCredit", pollDataMetaJackpotWin.winCredit);
        BlackboardUtils.SetOrCreateValue(bb, "type", pollDataMetaJackpotWin.type);
        BlackboardUtils.SetOrCreateValue(bb, "jackpotKudoType", pollDataMetaJackpotWin.jackpotKudoType);
        BlackboardUtils.SetOrCreateValue(bb, "kudoId", pollDataMetaJackpotWin.kudoId);
    }

    public static void Serialize(IBlackboard bb, PollDataNotice pollDataNotice)
    {
        if (pollDataNotice == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "message", pollDataNotice.message);
        BlackboardUtils.SetOrCreateValue(bb, "isHighlighted", pollDataNotice.isHighlighted);
        if (pollDataNotice.action != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "action"), pollDataNotice.action);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "action");
        }
    }

    public static void Serialize(IBlackboard bb, PollDataNoticeImage pollDataNoticeImage)
    {
        if (pollDataNoticeImage == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "imageUrl", pollDataNoticeImage.imageUrl);
        if (pollDataNoticeImage.action != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "action"), pollDataNoticeImage.action);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "action");
        }
    }

    public static void Serialize(IBlackboard bb, PollDataProgrammedKudo pollDataProgrammedKudo)
    {
        if (pollDataProgrammedKudo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "winType", pollDataProgrammedKudo.winType);
        BlackboardUtils.SetOrCreateValue(bb, "profileUrlList", pollDataProgrammedKudo.profileUrlList);
        BlackboardUtils.SetOrCreateValue(bb, "likeCount", pollDataProgrammedKudo.likeCount);
    }

    public static void Serialize(IBlackboard bb, PollDataSocialCredit pollDataSocialCredit)
    {
        if (pollDataSocialCredit == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "name", pollDataSocialCredit.name);
        BlackboardUtils.SetOrCreateValue(bb, "profileUrl", pollDataSocialCredit.profileUrl);
        BlackboardUtils.SetOrCreateValue(bb, "earnCredit", pollDataSocialCredit.earnCredit);
        BlackboardUtils.SetOrCreateValue(bb, "tier", pollDataSocialCredit.tier);
        BlackboardUtils.SetOrCreateValue(bb, "userId", pollDataSocialCredit.userId);
        BlackboardUtils.SetOrCreateValue(bb, "kudoId", pollDataSocialCredit.kudoId);
    }

    public static void Serialize(IBlackboard bb, PollDataTierUp pollDataTierUp)
    {
        if (pollDataTierUp == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "userId", pollDataTierUp.userId);
        BlackboardUtils.SetOrCreateValue(bb, "tier", pollDataTierUp.tier);
    }

    public static void Serialize(IBlackboard bb, PollDataTournamentEnd pollDataTournamentEnd)
    {
        if (pollDataTournamentEnd == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "rank", pollDataTournamentEnd.rank);
        BlackboardUtils.SetOrCreateValue(bb, "winCredit", pollDataTournamentEnd.winCredit);
        BlackboardUtils.SetOrCreateValue(bb, "tournamentId", pollDataTournamentEnd.tournamentId);
        BlackboardUtils.SetOrCreateValue(bb, "serialWinCount", pollDataTournamentEnd.serialWinCount);
        BlackboardUtils.SetOrCreateValue(bb, "serialWinBonus", pollDataTournamentEnd.serialWinBonus);
    }

    public static void Serialize(IBlackboard bb, PollDataTournamentStart pollDataTournamentStart)
    {
        if (pollDataTournamentStart == null) { return; }
    }

    public static void Serialize(IBlackboard bb, PollDataTournamentTopRanker pollDataTournamentTopRanker)
    {
        if (pollDataTournamentTopRanker == null) { return; }
        if (pollDataTournamentTopRanker.profile != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "profile"), pollDataTournamentTopRanker.profile);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "profile");
        }
        BlackboardUtils.SetOrCreateValue(bb, "winCredit", pollDataTournamentTopRanker.winCredit);
        BlackboardUtils.SetOrCreateValue(bb, "kudoId", pollDataTournamentTopRanker.kudoId);
        BlackboardUtils.SetOrCreateValue(bb, "userId", pollDataTournamentTopRanker.userId);
    }

    public static void Serialize(IBlackboard bb, PollDataWin pollDataWin)
    {
        if (pollDataWin == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "userId", pollDataWin.userId);
        BlackboardUtils.SetOrCreateValue(bb, "winType", pollDataWin.winType);
        BlackboardUtils.SetOrCreateValue(bb, "winCredit", pollDataWin.winCredit);
    }

    public static void Serialize(IBlackboard bb, PollRequest pollRequest)
    {
        if (pollRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", pollRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "lastReceivedId", pollRequest.lastReceivedId);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", pollRequest.ackMask);
    }

    public static void Serialize(IBlackboard bb, PollResponseV1 pollResponseV1)
    {
        if (pollResponseV1 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", pollResponseV1.error);
        if (pollResponseV1.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), pollResponseV1.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", pollResponseV1.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "data", pollResponseV1.data, Serialize);
    }

    public static void Serialize(IBlackboard bb, PollV1 pollV1)
    {
        if (pollV1 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "__event__", pollV1.__event__);
        if (pollV1.data != null)
        {
            switch (pollV1.__event__)
            {
            case PollType.KUDO_RECEIVE:
                Serialize(bb, (PollDataKudoReceive)pollV1.data);
                break;

            case PollType.LEVEL_UP:
                Serialize(bb, (PollDataLevelUp)pollV1.data);
                break;

            case PollType.TIER_UP:
                Serialize(bb, (PollDataTierUp)pollV1.data);
                break;

            case PollType.LIKE:
                Serialize(bb, (PollDataLike)pollV1.data);
                break;

            case PollType.WIN:
                Serialize(bb, (PollDataWin)pollV1.data);
                break;

            case PollType.JACKPOT_WIN:
                Serialize(bb, (PollDataJackpotWin)pollV1.data);
                break;

            case PollType.META_JACKPOT_WIN:
                Serialize(bb, (PollDataMetaJackpotWin)pollV1.data);
                break;

            case PollType.FRIEND_INVITE:
                Serialize(bb, (PollDataFriendInvite)pollV1.data);
                break;

            case PollType.FRIEND_CONNECT:
                Serialize(bb, (PollDataFriendConnect)pollV1.data);
                break;

            case PollType.NOTICE:
                Serialize(bb, (PollDataNotice)pollV1.data);
                break;

            case PollType.NOTICE_IMAGE:
                Serialize(bb, (PollDataNoticeImage)pollV1.data);
                break;

            case PollType.BONUS_TRIGGER:
                Serialize(bb, (PollDataBonusTrigger)pollV1.data);
                break;

            case PollType.TOURNAMENT_START:
                Serialize(bb, (PollDataTournamentStart)pollV1.data);
                break;

            case PollType.TOURNAMENT_END:
                Serialize(bb, (PollDataTournamentEnd)pollV1.data);
                break;

            case PollType.TOURNAMENT_TOP_RANKER:
                Serialize(bb, (PollDataTournamentTopRanker)pollV1.data);
                break;

            case PollType.CHALLENGE_MISSION_COMPLETE:
                Serialize(bb, (PollDataChallengeMissionCompleteV1)pollV1.data);
                break;

            case PollType.COMMUNITY_GAME_TICKET_EARN:
                Serialize(bb, (PollDataCommunityGameTicketEarn)pollV1.data);
                break;

            case PollType.CLUB_CHALLENGE_MISSION_COMPLETE:
                Serialize(bb, (PollDataClubChallengeMissionComplete)pollV1.data);
                break;

            case PollType.CLUB_MEMBER_LOGIN:
                Serialize(bb, (PollDataClubMemberLogin)pollV1.data);
                break;

            case PollType.CLUB_COLEADER_CHANGE:
                Serialize(bb, (PollDataClubColeaderChange)pollV1.data);
                break;

            case PollType.CLUB_REQUEST:
                Serialize(bb, (PollDataClubRequest)pollV1.data);
                break;

            case PollType.CLUB_REQUEST_ACCEPTED:
                Serialize(bb, (PollDataClubRequestAccepted)pollV1.data);
                break;

            case PollType.CUSTOM_USER_IN_GAME_ACTION:
                Serialize(bb, (PollDataCustomUserInGameAction)pollV1.data);
                break;

            case PollType.SOCIAL_CREDIT:
                Serialize(bb, (PollDataSocialCredit)pollV1.data);
                break;

            case PollType.CLUB_DEAL:
                Serialize(bb, (PollDataClubDeal)pollV1.data);
                break;

            case PollType.CLUB_SHARE:
                Serialize(bb, (PollDataClubShare)pollV1.data);
                break;

            case PollType.CLUB_INVITE:
                Serialize(bb, (PollDataClubInvite)pollV1.data);
                break;

            case PollType.FIRST_GLOBAL_CHAT:
                Serialize(bb, (PollDataFirstGlobalChat)pollV1.data);
                break;

            case PollType.BOSS_RAIDERS_ROUND_COMPLETE:
                Serialize(bb, (PollDataBossRaidersRoundComplete)pollV1.data);
                break;

            case PollType.BOSS_RAIDERS_END:
                Serialize(bb, (PollDataBossRaidersEnd)pollV1.data);
                break;

            case PollType.CLUB_ARENA_RANKING_UP:
                Serialize(bb, (PollDataClubArenaRankingUp)pollV1.data);
                break;

            case PollType.CLUB_ARENA_ONE_DAY_LEFT:
                Serialize(bb, (PollDataClubArenaOneDayLeft)pollV1.data);
                break;

            case PollType.CLUB_ARENA_END:
                Serialize(bb, (PollDataClubArenaEnd)pollV1.data);
                break;

            case PollType.HIDDEN_UNIVERSE_FINDER_GIFT:
                Serialize(bb, (PollDataHiddenUniverseFinderGift)pollV1.data);
                break;

            case PollType.PROGRAMMED_KUDO:
                Serialize(bb, (PollDataProgrammedKudo)pollV1.data);
                break;
            }
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "data");
        }
        BlackboardUtils.SetOrCreateValue(bb, "id", pollV1.id);
    }

    public static void Serialize(IBlackboard bb, PreBbbClaimResponse preBbbClaimResponse)
    {
        if (preBbbClaimResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", preBbbClaimResponse.error);
        if (preBbbClaimResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), preBbbClaimResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", preBbbClaimResponse.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "rewardResultList", preBbbClaimResponse.rewardResultList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "preBbbId", preBbbClaimResponse.preBbbId);
    }

    public static void Serialize(IBlackboard bb, PrevAmount prevAmount)
    {
        if (prevAmount == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "amount", prevAmount.amount);
        BlackboardUtils.SetOrCreateValue(bb, "timeDelta", prevAmount.timeDelta);
    }

    public static void Serialize(IBlackboard bb, ProbMap probMap)
    {
        if (probMap == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "value", probMap.value);
        BlackboardUtils.SetOrCreateValue(bb, "prob", probMap.prob);
    }

    public static void Serialize(IBlackboard bb, ProbsInfo probsInfo)
    {
        if (probsInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "message", probsInfo.message);
        BlackboardUtils.SetOrCreateList(bb, "probList", probsInfo.probList, Serialize);
    }

    public static void Serialize(IBlackboard bb, ProbsRequest probsRequest)
    {
        if (probsRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", probsRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "probType", probsRequest.probType);
        BlackboardUtils.SetOrCreateValue(bb, "eventId", probsRequest.eventId);
        BlackboardUtils.SetOrCreateValue(bb, "productId", probsRequest.productId);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", probsRequest.ackMask);
    }

    public static void Serialize(IBlackboard bb, ProbsResponse probsResponse)
    {
        if (probsResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", probsResponse.error);
        if (probsResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), probsResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", probsResponse.serverTime);
        if (probsResponse.probsInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "probsInfo"), probsResponse.probsInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "probsInfo");
        }
    }

    public static void Serialize(IBlackboard bb, Product product)
    {
        if (product == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "id", product.id);
        BlackboardUtils.SetOrCreateValue(bb, "name", product.name);
        BlackboardUtils.SetOrCreateValue(bb, "price", product.price);
        BlackboardUtils.SetOrCreateValue(bb, "originalPrice", product.originalPrice);
        BlackboardUtils.SetOrCreateValue(bb, "skuAndroid", product.skuAndroid);
        BlackboardUtils.SetOrCreateValue(bb, "skuIos", product.skuIos);
        BlackboardUtils.SetOrCreateList(bb, "itemList", product.itemList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "tagType", product.tagType);
        BlackboardUtils.SetOrCreateValue(bb, "skuAmazon", product.skuAmazon);
        BlackboardUtils.SetOrCreateValue(bb, "skuWindows", product.skuWindows);
        BlackboardUtils.SetOrCreateValue(bb, "skuGameroom", product.skuGameroom);
        BlackboardUtils.SetOrCreateValue(bb, "skuCanvas", product.skuCanvas);
        BlackboardUtils.SetOrCreateValue(bb, "isSubscription", product.isSubscription);
        BlackboardUtils.SetOrCreateValue(bb, "offerFreeTrial", product.offerFreeTrial);
        BlackboardUtils.SetOrCreateList(bb, "rewardList", product.rewardList, Serialize);
        if (product.rewardListVisualizeInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "rewardListVisualizeInfo"), product.rewardListVisualizeInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "rewardListVisualizeInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "eventMultiplierNumerator", product.eventMultiplierNumerator);
        BlackboardUtils.SetOrCreateValue(bb, "gemPrice", product.gemPrice);
        BlackboardUtils.SetOrCreateValue(bb, "msStoreId", product.msStoreId);
        BlackboardUtils.SetOrCreateValue(bb, "enableBucksPurchase", product.enableBucksPurchase);
    }

    public static void Serialize(IBlackboard bb, ProductGroup productGroup)
    {
        if (productGroup == null) { return; }
        BlackboardUtils.SetOrCreateList(bb, "productList", productGroup.productList, Serialize);
    }

    public static void Serialize(IBlackboard bb, PurchaseCompleteRequestV2 purchaseCompleteRequestV2)
    {
        if (purchaseCompleteRequestV2 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", purchaseCompleteRequestV2.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", purchaseCompleteRequestV2.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "purchaseProgressId", purchaseCompleteRequestV2.purchaseProgressId);
        BlackboardUtils.SetOrCreateValue(bb, "receipt", purchaseCompleteRequestV2.receipt);
        BlackboardUtils.SetOrCreateValue(bb, "signature", purchaseCompleteRequestV2.signature);
        BlackboardUtils.SetOrCreateValue(bb, "deviceType", purchaseCompleteRequestV2.deviceType);
        BlackboardUtils.SetOrCreateValue(bb, "currentTime", purchaseCompleteRequestV2.currentTime);
        BlackboardUtils.SetOrCreateValue(bb, "timezoneOffset", purchaseCompleteRequestV2.timezoneOffset);
        BlackboardUtils.SetOrCreateValue(bb, "contextId", purchaseCompleteRequestV2.contextId);
        BlackboardUtils.SetOrCreateValue(bb, "currencyCode", purchaseCompleteRequestV2.currencyCode);
        BlackboardUtils.SetOrCreateValue(bb, "localPrice", purchaseCompleteRequestV2.localPrice);
        BlackboardUtils.SetOrCreateValue(bb, "useBucks", purchaseCompleteRequestV2.useBucks);
    }

    public static void Serialize(IBlackboard bb, PurchaseCompleteResponseV2 purchaseCompleteResponseV2)
    {
        if (purchaseCompleteResponseV2 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", purchaseCompleteResponseV2.error);
        if (purchaseCompleteResponseV2.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), purchaseCompleteResponseV2.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", purchaseCompleteResponseV2.serverTime);
        if (purchaseCompleteResponseV2.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), purchaseCompleteResponseV2.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "purchaseId", purchaseCompleteResponseV2.purchaseId);
        BlackboardUtils.SetOrCreateList(bb, "itemUseResultList", purchaseCompleteResponseV2.itemUseResultList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "purchaseCount", purchaseCompleteResponseV2.purchaseCount);
        BlackboardUtils.SetOrCreateValue(bb, "lifetimeSpend", purchaseCompleteResponseV2.lifetimeSpend);
        BlackboardUtils.SetOrCreateValue(bb, "distanceFromLastPurchaseTimestamp", purchaseCompleteResponseV2.distanceFromLastPurchaseTimestamp);
        BlackboardUtils.SetOrCreateList(bb, "rewardResultList", purchaseCompleteResponseV2.rewardResultList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "hasPreBbbReward", purchaseCompleteResponseV2.hasPreBbbReward);
        BlackboardUtils.SetOrCreateValue(bb, "needToReloadCampaign", purchaseCompleteResponseV2.needToReloadCampaign);
        BlackboardUtils.SetOrCreateValue(bb, "earnLoungePoint", purchaseCompleteResponseV2.earnLoungePoint);
        if (purchaseCompleteResponseV2.vipLoungeInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "vipLoungeInfo"), purchaseCompleteResponseV2.vipLoungeInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "vipLoungeInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "anyPurchaseBoosterEndTimestamp", purchaseCompleteResponseV2.anyPurchaseBoosterEndTimestamp);
        if (purchaseCompleteResponseV2.userBucks != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userBucks"), purchaseCompleteResponseV2.userBucks);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userBucks");
        }
    }

    public static void Serialize(IBlackboard bb, PurchaseCreateProgressRequest purchaseCreateProgressRequest)
    {
        if (purchaseCreateProgressRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", purchaseCreateProgressRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", purchaseCreateProgressRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "productId", purchaseCreateProgressRequest.productId);
        BlackboardUtils.SetOrCreateValue(bb, "targetPurchaseId", purchaseCreateProgressRequest.targetPurchaseId);
        BlackboardUtils.SetOrCreateValue(bb, "piggyBankEventId", purchaseCreateProgressRequest.piggyBankEventId);
        BlackboardUtils.SetOrCreateValue(bb, "pogBoosterEventId", purchaseCreateProgressRequest.pogBoosterEventId);
        BlackboardUtils.SetOrCreateValue(bb, "cmwBonusEventId", purchaseCreateProgressRequest.cmwBonusEventId);
        BlackboardUtils.SetOrCreateValue(bb, "cmwMultiplyEventId", purchaseCreateProgressRequest.cmwMultiplyEventId);
        BlackboardUtils.SetOrCreateValue(bb, "metaGameEventId", purchaseCreateProgressRequest.metaGameEventId);
        BlackboardUtils.SetOrCreateValue(bb, "vipDealUuid", purchaseCreateProgressRequest.vipDealUuid);
        BlackboardUtils.SetOrCreateValue(bb, "vipDealInfoId", purchaseCreateProgressRequest.vipDealInfoId);
        BlackboardUtils.SetOrCreateValue(bb, "userGroupId", purchaseCreateProgressRequest.userGroupId);
        BlackboardUtils.SetOrCreateValue(bb, "iamId", purchaseCreateProgressRequest.iamId);
        BlackboardUtils.SetOrCreateValue(bb, "iamTriggerType", purchaseCreateProgressRequest.iamTriggerType);
        BlackboardUtils.SetOrCreateValue(bb, "coinShopMultiplyEventId", purchaseCreateProgressRequest.coinShopMultiplyEventId);
        BlackboardUtils.SetOrCreateValue(bb, "gemShopMultiplyEventId", purchaseCreateProgressRequest.gemShopMultiplyEventId);
        BlackboardUtils.SetOrCreateValue(bb, "dailyWheelEventId", purchaseCreateProgressRequest.dailyWheelEventId);
        BlackboardUtils.SetOrCreateValue(bb, "gemBabShopMultiplyEventId", purchaseCreateProgressRequest.gemBabShopMultiplyEventId);
        BlackboardUtils.SetOrCreateValue(bb, "gemBoosterBonusEventId", purchaseCreateProgressRequest.gemBoosterBonusEventId);
        BlackboardUtils.SetOrCreateValue(bb, "gemBoosterMultiplyEventId", purchaseCreateProgressRequest.gemBoosterMultiplyEventId);
        BlackboardUtils.SetOrCreateValue(bb, "gemBabPromotionShopMultiplyEventId", purchaseCreateProgressRequest.gemBabPromotionShopMultiplyEventId);
        BlackboardUtils.SetOrCreateValue(bb, "shopEventFlag", purchaseCreateProgressRequest.shopEventFlag);
        BlackboardUtils.SetOrCreateValue(bb, "voucherShopMultiplyEventId", purchaseCreateProgressRequest.voucherShopMultiplyEventId);
        BlackboardUtils.SetOrCreateValue(bb, "tierUpShopMultiplyEventId", purchaseCreateProgressRequest.tierUpShopMultiplyEventId);
        BlackboardUtils.SetOrCreateValue(bb, "spinDealId", purchaseCreateProgressRequest.spinDealId);
        BlackboardUtils.SetOrCreateValue(bb, "ticketIdList", purchaseCreateProgressRequest.ticketIdList);
        BlackboardUtils.SetOrCreateValue(bb, "seasonPassEventId", purchaseCreateProgressRequest.seasonPassEventId);
    }

    public static void Serialize(IBlackboard bb, PurchaseCreateProgressResponse purchaseCreateProgressResponse)
    {
        if (purchaseCreateProgressResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", purchaseCreateProgressResponse.error);
        if (purchaseCreateProgressResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), purchaseCreateProgressResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", purchaseCreateProgressResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "purchaseProgressId", purchaseCreateProgressResponse.purchaseProgressId);
        BlackboardUtils.SetOrCreateValue(bb, "hasSufficientBucks", purchaseCreateProgressResponse.hasSufficientBucks);
    }

    public static void Serialize(IBlackboard bb, PurchaseGemPaymentRequest purchaseGemPaymentRequest)
    {
        if (purchaseGemPaymentRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", purchaseGemPaymentRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", purchaseGemPaymentRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "productId", purchaseGemPaymentRequest.productId);
        BlackboardUtils.SetOrCreateValue(bb, "currentTime", purchaseGemPaymentRequest.currentTime);
        BlackboardUtils.SetOrCreateValue(bb, "timezoneOffset", purchaseGemPaymentRequest.timezoneOffset);
        BlackboardUtils.SetOrCreateValue(bb, "iamId", purchaseGemPaymentRequest.iamId);
        BlackboardUtils.SetOrCreateValue(bb, "metaGameEventId", purchaseGemPaymentRequest.metaGameEventId);
        BlackboardUtils.SetOrCreateValue(bb, "userGroupId", purchaseGemPaymentRequest.userGroupId);
        BlackboardUtils.SetOrCreateValue(bb, "iamTriggerType", purchaseGemPaymentRequest.iamTriggerType);
        BlackboardUtils.SetOrCreateValue(bb, "contextId", purchaseGemPaymentRequest.contextId);
        BlackboardUtils.SetOrCreateValue(bb, "seasonPassEventId", purchaseGemPaymentRequest.seasonPassEventId);
    }

    public static void Serialize(IBlackboard bb, PurchaseGemPaymentResponse purchaseGemPaymentResponse)
    {
        if (purchaseGemPaymentResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", purchaseGemPaymentResponse.error);
        if (purchaseGemPaymentResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), purchaseGemPaymentResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", purchaseGemPaymentResponse.serverTime);
        if (purchaseGemPaymentResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), purchaseGemPaymentResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
        BlackboardUtils.SetOrCreateList(bb, "itemUseResultList", purchaseGemPaymentResponse.itemUseResultList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "rewardResultList", purchaseGemPaymentResponse.rewardResultList, Serialize);
    }

    public static void Serialize(IBlackboard bb, PurchaseGetAzureAdCollectionsTokenResponse purchaseGetAzureAdCollectionsTokenResponse)
    {
        if (purchaseGetAzureAdCollectionsTokenResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", purchaseGetAzureAdCollectionsTokenResponse.error);
        if (purchaseGetAzureAdCollectionsTokenResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), purchaseGetAzureAdCollectionsTokenResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", purchaseGetAzureAdCollectionsTokenResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "azureAdCollectionsToken", purchaseGetAzureAdCollectionsTokenResponse.azureAdCollectionsToken);
    }

    public static void Serialize(IBlackboard bb, PurchaseRecoverRequestV2 purchaseRecoverRequestV2)
    {
        if (purchaseRecoverRequestV2 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", purchaseRecoverRequestV2.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", purchaseRecoverRequestV2.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "sku", purchaseRecoverRequestV2.sku);
        BlackboardUtils.SetOrCreateValue(bb, "receipt", purchaseRecoverRequestV2.receipt);
        BlackboardUtils.SetOrCreateValue(bb, "signature", purchaseRecoverRequestV2.signature);
        BlackboardUtils.SetOrCreateValue(bb, "deviceType", purchaseRecoverRequestV2.deviceType);
        BlackboardUtils.SetOrCreateValue(bb, "currentTime", purchaseRecoverRequestV2.currentTime);
        BlackboardUtils.SetOrCreateValue(bb, "timezoneOffset", purchaseRecoverRequestV2.timezoneOffset);
        BlackboardUtils.SetOrCreateValue(bb, "currencyCode", purchaseRecoverRequestV2.currencyCode);
        BlackboardUtils.SetOrCreateValue(bb, "localPrice", purchaseRecoverRequestV2.localPrice);
    }

    public static void Serialize(IBlackboard bb, PurchaseRecoverRequestV3 purchaseRecoverRequestV3)
    {
        if (purchaseRecoverRequestV3 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", purchaseRecoverRequestV3.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", purchaseRecoverRequestV3.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "sku", purchaseRecoverRequestV3.sku);
        BlackboardUtils.SetOrCreateValue(bb, "receipt", purchaseRecoverRequestV3.receipt);
        BlackboardUtils.SetOrCreateValue(bb, "signature", purchaseRecoverRequestV3.signature);
        BlackboardUtils.SetOrCreateValue(bb, "deviceType", purchaseRecoverRequestV3.deviceType);
        BlackboardUtils.SetOrCreateValue(bb, "currentTime", purchaseRecoverRequestV3.currentTime);
        BlackboardUtils.SetOrCreateValue(bb, "timezoneOffset", purchaseRecoverRequestV3.timezoneOffset);
        BlackboardUtils.SetOrCreateValue(bb, "currencyCode", purchaseRecoverRequestV3.currencyCode);
        BlackboardUtils.SetOrCreateValue(bb, "localPrice", purchaseRecoverRequestV3.localPrice);
        BlackboardUtils.SetOrCreateValue(bb, "purchaseProgressId", purchaseRecoverRequestV3.purchaseProgressId);
    }

    public static void Serialize(IBlackboard bb, PurchaseRecoverResponseV2 purchaseRecoverResponseV2)
    {
        if (purchaseRecoverResponseV2 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", purchaseRecoverResponseV2.error);
        if (purchaseRecoverResponseV2.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), purchaseRecoverResponseV2.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", purchaseRecoverResponseV2.serverTime);
        if (purchaseRecoverResponseV2.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), purchaseRecoverResponseV2.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "purchaseId", purchaseRecoverResponseV2.purchaseId);
        BlackboardUtils.SetOrCreateList(bb, "itemUseResultList", purchaseRecoverResponseV2.itemUseResultList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "purchaseCount", purchaseRecoverResponseV2.purchaseCount);
        BlackboardUtils.SetOrCreateValue(bb, "lifetimeSpend", purchaseRecoverResponseV2.lifetimeSpend);
        BlackboardUtils.SetOrCreateValue(bb, "distanceFromLastPurchaseTimestamp", purchaseRecoverResponseV2.distanceFromLastPurchaseTimestamp);
        BlackboardUtils.SetOrCreateList(bb, "rewardResultList", purchaseRecoverResponseV2.rewardResultList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "hasPreBbbReward", purchaseRecoverResponseV2.hasPreBbbReward);
        BlackboardUtils.SetOrCreateList(bb, "purchaseRecoverInboxList", purchaseRecoverResponseV2.purchaseRecoverInboxList, Serialize);
    }

    public static void Serialize(IBlackboard bb, PurchaseRecoverResponseV3 purchaseRecoverResponseV3)
    {
        if (purchaseRecoverResponseV3 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", purchaseRecoverResponseV3.error);
        if (purchaseRecoverResponseV3.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), purchaseRecoverResponseV3.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", purchaseRecoverResponseV3.serverTime);
        if (purchaseRecoverResponseV3.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), purchaseRecoverResponseV3.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "purchaseId", purchaseRecoverResponseV3.purchaseId);
        BlackboardUtils.SetOrCreateList(bb, "itemUseResultList", purchaseRecoverResponseV3.itemUseResultList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "purchaseCount", purchaseRecoverResponseV3.purchaseCount);
        BlackboardUtils.SetOrCreateValue(bb, "lifetimeSpend", purchaseRecoverResponseV3.lifetimeSpend);
        BlackboardUtils.SetOrCreateValue(bb, "distanceFromLastPurchaseTimestamp", purchaseRecoverResponseV3.distanceFromLastPurchaseTimestamp);
        BlackboardUtils.SetOrCreateList(bb, "rewardResultList", purchaseRecoverResponseV3.rewardResultList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "hasPreBbbReward", purchaseRecoverResponseV3.hasPreBbbReward);
        BlackboardUtils.SetOrCreateList(bb, "purchaseRecoverInboxList", purchaseRecoverResponseV3.purchaseRecoverInboxList, Serialize);
    }

    public static void Serialize(IBlackboard bb, PurchaseRedeemBonusRequest purchaseRedeemBonusRequest)
    {
        if (purchaseRedeemBonusRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", purchaseRedeemBonusRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", purchaseRedeemBonusRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "currentTime", purchaseRedeemBonusRequest.currentTime);
        BlackboardUtils.SetOrCreateValue(bb, "timezoneOffset", purchaseRedeemBonusRequest.timezoneOffset);
        BlackboardUtils.SetOrCreateValue(bb, "productId", purchaseRedeemBonusRequest.productId);
    }

    public static void Serialize(IBlackboard bb, PurchaseRedeemBonusResponse purchaseRedeemBonusResponse)
    {
        if (purchaseRedeemBonusResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", purchaseRedeemBonusResponse.error);
        if (purchaseRedeemBonusResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), purchaseRedeemBonusResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", purchaseRedeemBonusResponse.serverTime);
        if (purchaseRedeemBonusResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), purchaseRedeemBonusResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
        BlackboardUtils.SetOrCreateList(bb, "itemUseResultList", purchaseRedeemBonusResponse.itemUseResultList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "rewardResultList", purchaseRedeemBonusResponse.rewardResultList, Serialize);
    }

    public static void Serialize(IBlackboard bb, PurchaseRedeemFreeCoinBoosterRequest purchaseRedeemFreeCoinBoosterRequest)
    {
        if (purchaseRedeemFreeCoinBoosterRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", purchaseRedeemFreeCoinBoosterRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", purchaseRedeemFreeCoinBoosterRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "productId", purchaseRedeemFreeCoinBoosterRequest.productId);
        BlackboardUtils.SetOrCreateValue(bb, "targetPurchaseId", purchaseRedeemFreeCoinBoosterRequest.targetPurchaseId);
        BlackboardUtils.SetOrCreateValue(bb, "freeCoinBoosterEventId", purchaseRedeemFreeCoinBoosterRequest.freeCoinBoosterEventId);
    }

    public static void Serialize(IBlackboard bb, PurchaseRedeemFreeCoinBoosterResponse purchaseRedeemFreeCoinBoosterResponse)
    {
        if (purchaseRedeemFreeCoinBoosterResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", purchaseRedeemFreeCoinBoosterResponse.error);
        if (purchaseRedeemFreeCoinBoosterResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), purchaseRedeemFreeCoinBoosterResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", purchaseRedeemFreeCoinBoosterResponse.serverTime);
        if (purchaseRedeemFreeCoinBoosterResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), purchaseRedeemFreeCoinBoosterResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
        BlackboardUtils.SetOrCreateList(bb, "itemUseResultList", purchaseRedeemFreeCoinBoosterResponse.itemUseResultList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "rewardResultList", purchaseRedeemFreeCoinBoosterResponse.rewardResultList, Serialize);
    }

    public static void Serialize(IBlackboard bb, PurchaseRedeemFreeDailyBonusRequest purchaseRedeemFreeDailyBonusRequest)
    {
        if (purchaseRedeemFreeDailyBonusRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", purchaseRedeemFreeDailyBonusRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", purchaseRedeemFreeDailyBonusRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "productId", purchaseRedeemFreeDailyBonusRequest.productId);
    }

    public static void Serialize(IBlackboard bb, PurchaseRedeemFreeDailyBonusResponse purchaseRedeemFreeDailyBonusResponse)
    {
        if (purchaseRedeemFreeDailyBonusResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", purchaseRedeemFreeDailyBonusResponse.error);
        if (purchaseRedeemFreeDailyBonusResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), purchaseRedeemFreeDailyBonusResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", purchaseRedeemFreeDailyBonusResponse.serverTime);
        if (purchaseRedeemFreeDailyBonusResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), purchaseRedeemFreeDailyBonusResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
        BlackboardUtils.SetOrCreateList(bb, "itemUseResultList", purchaseRedeemFreeDailyBonusResponse.itemUseResultList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "rewardResultList", purchaseRedeemFreeDailyBonusResponse.rewardResultList, Serialize);
    }

    public static void Serialize(IBlackboard bb, PurchaseRedeemFreeGemBoosterRequest purchaseRedeemFreeGemBoosterRequest)
    {
        if (purchaseRedeemFreeGemBoosterRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", purchaseRedeemFreeGemBoosterRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", purchaseRedeemFreeGemBoosterRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "productId", purchaseRedeemFreeGemBoosterRequest.productId);
        BlackboardUtils.SetOrCreateValue(bb, "targetPurchaseId", purchaseRedeemFreeGemBoosterRequest.targetPurchaseId);
        BlackboardUtils.SetOrCreateValue(bb, "freeGemBoosterEventId", purchaseRedeemFreeGemBoosterRequest.freeGemBoosterEventId);
    }

    public static void Serialize(IBlackboard bb, PurchaseRedeemFreeGemBoosterResponse purchaseRedeemFreeGemBoosterResponse)
    {
        if (purchaseRedeemFreeGemBoosterResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", purchaseRedeemFreeGemBoosterResponse.error);
        if (purchaseRedeemFreeGemBoosterResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), purchaseRedeemFreeGemBoosterResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", purchaseRedeemFreeGemBoosterResponse.serverTime);
        if (purchaseRedeemFreeGemBoosterResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), purchaseRedeemFreeGemBoosterResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
        BlackboardUtils.SetOrCreateList(bb, "itemUseResultList", purchaseRedeemFreeGemBoosterResponse.itemUseResultList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "rewardResultList", purchaseRedeemFreeGemBoosterResponse.rewardResultList, Serialize);
    }

    public static void Serialize(IBlackboard bb, PurchaseRedeemFreePogBoosterRequest purchaseRedeemFreePogBoosterRequest)
    {
        if (purchaseRedeemFreePogBoosterRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", purchaseRedeemFreePogBoosterRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", purchaseRedeemFreePogBoosterRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "productId", purchaseRedeemFreePogBoosterRequest.productId);
        BlackboardUtils.SetOrCreateValue(bb, "targetPurchaseId", purchaseRedeemFreePogBoosterRequest.targetPurchaseId);
        BlackboardUtils.SetOrCreateValue(bb, "freePogBoosterEventId", purchaseRedeemFreePogBoosterRequest.freePogBoosterEventId);
    }

    public static void Serialize(IBlackboard bb, PurchaseRedeemFreePogBoosterResponse purchaseRedeemFreePogBoosterResponse)
    {
        if (purchaseRedeemFreePogBoosterResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", purchaseRedeemFreePogBoosterResponse.error);
        if (purchaseRedeemFreePogBoosterResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), purchaseRedeemFreePogBoosterResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", purchaseRedeemFreePogBoosterResponse.serverTime);
        if (purchaseRedeemFreePogBoosterResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), purchaseRedeemFreePogBoosterResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
        BlackboardUtils.SetOrCreateList(bb, "itemUseResultList", purchaseRedeemFreePogBoosterResponse.itemUseResultList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "rewardResultList", purchaseRedeemFreePogBoosterResponse.rewardResultList, Serialize);
    }

    public static void Serialize(IBlackboard bb, PurchaseRedeemVipDealRequest purchaseRedeemVipDealRequest)
    {
        if (purchaseRedeemVipDealRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", purchaseRedeemVipDealRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", purchaseRedeemVipDealRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "dealUuid", purchaseRedeemVipDealRequest.dealUuid);
        BlackboardUtils.SetOrCreateValue(bb, "vipDealInfoId", purchaseRedeemVipDealRequest.vipDealInfoId);
    }

    public static void Serialize(IBlackboard bb, PurchaseRedeemVipDealResponse purchaseRedeemVipDealResponse)
    {
        if (purchaseRedeemVipDealResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", purchaseRedeemVipDealResponse.error);
        if (purchaseRedeemVipDealResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), purchaseRedeemVipDealResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", purchaseRedeemVipDealResponse.serverTime);
        if (purchaseRedeemVipDealResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), purchaseRedeemVipDealResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
        BlackboardUtils.SetOrCreateList(bb, "itemUseResultList", purchaseRedeemVipDealResponse.itemUseResultList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "rewardResultList", purchaseRedeemVipDealResponse.rewardResultList, Serialize);
    }

    public static void Serialize(IBlackboard bb, PurchaseUpdateProgressRequest purchaseUpdateProgressRequest)
    {
        if (purchaseUpdateProgressRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", purchaseUpdateProgressRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", purchaseUpdateProgressRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "purchaseProgressId", purchaseUpdateProgressRequest.purchaseProgressId);
        BlackboardUtils.SetOrCreateValue(bb, "updatedState", purchaseUpdateProgressRequest.updatedState);
        BlackboardUtils.SetOrCreateValue(bb, "isRecover", purchaseUpdateProgressRequest.isRecover);
        BlackboardUtils.SetOrCreateValue(bb, "errorMessage", purchaseUpdateProgressRequest.errorMessage);
    }

    public static void Serialize(IBlackboard bb, PurchaseUpdateProgressResponse purchaseUpdateProgressResponse)
    {
        if (purchaseUpdateProgressResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", purchaseUpdateProgressResponse.error);
        if (purchaseUpdateProgressResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), purchaseUpdateProgressResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", purchaseUpdateProgressResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "success", purchaseUpdateProgressResponse.success);
    }

    public static void Serialize(IBlackboard bb, PushMessage pushMessage)
    {
        if (pushMessage == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "ID", pushMessage.ID);
        BlackboardUtils.SetOrCreateValue(bb, "SENDER", pushMessage.SENDER);
        BlackboardUtils.SetOrCreateValue(bb, "TITLE", pushMessage.TITLE);
        BlackboardUtils.SetOrCreateValue(bb, "TEXT", pushMessage.TEXT);
        BlackboardUtils.SetOrCreateValue(bb, "SECONDS", pushMessage.SECONDS);
        BlackboardUtils.SetOrCreateValue(bb, "TYPE", pushMessage.TYPE);
    }

    public static void Serialize(IBlackboard bb, PushRegisterRequestV1 pushRegisterRequestV1)
    {
        if (pushRegisterRequestV1 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", pushRegisterRequestV1.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "deviceId", pushRegisterRequestV1.deviceId);
        BlackboardUtils.SetOrCreateValue(bb, "onesignalId", pushRegisterRequestV1.onesignalId);
        BlackboardUtils.SetOrCreateValue(bb, "clientOs", pushRegisterRequestV1.clientOs);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", pushRegisterRequestV1.ackMask);
    }

    public static void Serialize(IBlackboard bb, RateRequest rateRequest)
    {
        if (rateRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", rateRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "rating", rateRequest.rating);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", rateRequest.ackMask);
    }

    public static void Serialize(IBlackboard bb, RateResponse rateResponse)
    {
        if (rateResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", rateResponse.error);
        if (rateResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), rateResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", rateResponse.serverTime);
        if (rateResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), rateResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "lastRatedClientNumberVersion", rateResponse.lastRatedClientNumberVersion);
        BlackboardUtils.SetOrCreateValue(bb, "earnCredit", rateResponse.earnCredit);
    }

    public static void Serialize(IBlackboard bb, Record record)
    {
        if (record == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "maxWin", record.maxWin);
        BlackboardUtils.SetOrCreateValue(bb, "totalWin", record.totalWin);
        BlackboardUtils.SetOrCreateValue(bb, "gameId", record.gameId);
    }

    public static void Serialize(IBlackboard bb, RegisterUserGroupRequest registerUserGroupRequest)
    {
        if (registerUserGroupRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", registerUserGroupRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", registerUserGroupRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "userGroupId", registerUserGroupRequest.userGroupId);
    }

    public static void Serialize(IBlackboard bb, ReportAdjustRequest reportAdjustRequest)
    {
        if (reportAdjustRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", reportAdjustRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", reportAdjustRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "deviceId", reportAdjustRequest.deviceId);
        BlackboardUtils.SetOrCreateValue(bb, "adid", reportAdjustRequest.adid);
        BlackboardUtils.SetOrCreateValue(bb, "trackerToken", reportAdjustRequest.trackerToken);
        BlackboardUtils.SetOrCreateValue(bb, "trackerName", reportAdjustRequest.trackerName);
        BlackboardUtils.SetOrCreateValue(bb, "network", reportAdjustRequest.network);
        BlackboardUtils.SetOrCreateValue(bb, "campaign", reportAdjustRequest.campaign);
        BlackboardUtils.SetOrCreateValue(bb, "adgroup", reportAdjustRequest.adgroup);
        BlackboardUtils.SetOrCreateValue(bb, "creative", reportAdjustRequest.creative);
        BlackboardUtils.SetOrCreateValue(bb, "clickLabel", reportAdjustRequest.clickLabel);
    }

    public static void Serialize(IBlackboard bb, ReportDealEndResponseV2 reportDealEndResponseV2)
    {
        if (reportDealEndResponseV2 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", reportDealEndResponseV2.error);
        if (reportDealEndResponseV2.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), reportDealEndResponseV2.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", reportDealEndResponseV2.serverTime);
        if (reportDealEndResponseV2.metaGameInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "metaGameInfo"), reportDealEndResponseV2.metaGameInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "metaGameInfo");
        }
    }

    public static void Serialize(IBlackboard bb, ReportDealEndResponseV3 reportDealEndResponseV3)
    {
        if (reportDealEndResponseV3 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", reportDealEndResponseV3.error);
        if (reportDealEndResponseV3.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), reportDealEndResponseV3.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", reportDealEndResponseV3.serverTime);
        if (reportDealEndResponseV3.metaGameInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "metaGameInfo"), reportDealEndResponseV3.metaGameInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "metaGameInfo");
        }
        if (reportDealEndResponseV3.seasonPassUpdateInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "seasonPassUpdateInfo"), reportDealEndResponseV3.seasonPassUpdateInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "seasonPassUpdateInfo");
        }
    }

    public static void Serialize(IBlackboard bb, ReportTurnEndRequestV1 reportTurnEndRequestV1)
    {
        if (reportTurnEndRequestV1 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", reportTurnEndRequestV1.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", reportTurnEndRequestV1.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "spinBlockseq", reportTurnEndRequestV1.spinBlockseq);
        BlackboardUtils.SetOrCreateValue(bb, "metaGameEventId", reportTurnEndRequestV1.metaGameEventId);
    }

    public static void Serialize(IBlackboard bb, ReportTurnEndRequestV3 reportTurnEndRequestV3)
    {
        if (reportTurnEndRequestV3 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", reportTurnEndRequestV3.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", reportTurnEndRequestV3.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "spinBlockseq", reportTurnEndRequestV3.spinBlockseq);
        BlackboardUtils.SetOrCreateValue(bb, "metaGameEventId", reportTurnEndRequestV3.metaGameEventId);
        BlackboardUtils.SetOrCreateValue(bb, "seasonPassEventId", reportTurnEndRequestV3.seasonPassEventId);
    }

    public static void Serialize(IBlackboard bb, ReportTurnEndResponseV2 reportTurnEndResponseV2)
    {
        if (reportTurnEndResponseV2 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", reportTurnEndResponseV2.error);
        if (reportTurnEndResponseV2.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), reportTurnEndResponseV2.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", reportTurnEndResponseV2.serverTime);
        if (reportTurnEndResponseV2.metaGameInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "metaGameInfo"), reportTurnEndResponseV2.metaGameInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "metaGameInfo");
        }
    }

    public static void Serialize(IBlackboard bb, ReportTurnEndResponseV3 reportTurnEndResponseV3)
    {
        if (reportTurnEndResponseV3 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", reportTurnEndResponseV3.error);
        if (reportTurnEndResponseV3.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), reportTurnEndResponseV3.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", reportTurnEndResponseV3.serverTime);
        if (reportTurnEndResponseV3.metaGameInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "metaGameInfo"), reportTurnEndResponseV3.metaGameInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "metaGameInfo");
        }
        if (reportTurnEndResponseV3.seasonPassUpdateInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "seasonPassUpdateInfo"), reportTurnEndResponseV3.seasonPassUpdateInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "seasonPassUpdateInfo");
        }
    }

    public static void Serialize(IBlackboard bb, Reward reward)
    {
        if (reward == null) { return; }
        if (reward.FACEBOOK_FRIEND_CONNECT != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "FACEBOOK_FRIEND_CONNECT"), reward.FACEBOOK_FRIEND_CONNECT);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "FACEBOOK_FRIEND_CONNECT");
        }
        if (reward.FACEBOOK_SHARE != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "FACEBOOK_SHARE"), reward.FACEBOOK_SHARE);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "FACEBOOK_SHARE");
        }
        if (reward.FACEBOOK_CONNECT != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "FACEBOOK_CONNECT"), reward.FACEBOOK_CONNECT);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "FACEBOOK_CONNECT");
        }
        if (reward.SEND_KUDO != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "SEND_KUDO"), reward.SEND_KUDO);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "SEND_KUDO");
        }
        if (reward.PROFILE_UPLOAD != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "PROFILE_UPLOAD"), reward.PROFILE_UPLOAD);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "PROFILE_UPLOAD");
        }
        if (reward.RATE != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "RATE"), reward.RATE);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "RATE");
        }
        if (reward.EMAIL_CONNECT != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "EMAIL_CONNECT"), reward.EMAIL_CONNECT);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "EMAIL_CONNECT");
        }
        if (reward.APPLE_CONNECT != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "APPLE_CONNECT"), reward.APPLE_CONNECT);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "APPLE_CONNECT");
        }
    }

    public static void Serialize(IBlackboard bb, RewardInfo rewardInfo)
    {
        if (rewardInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "type", rewardInfo.type);
        if (rewardInfo.value != null)
        {
            switch (rewardInfo.type)
            {
            case RewardType.CREDIT:
                Serialize(bb, (RewardInfoValueCredit)rewardInfo.value);
                break;

            case RewardType.RP:
                Serialize(bb, (RewardInfoValueRp)rewardInfo.value);
                break;

            case RewardType.DAILY_BONUS_WHEEL_SPIN:
                Serialize(bb, (RewardInfoValueDailyBonusWheelSpin)rewardInfo.value);
                break;

            case RewardType.PURCHASE_COUPON:
                Serialize(bb, (RewardInfoValuePurchaseCoupon)rewardInfo.value);
                break;

            case RewardType.GAME_SPIN:
                Serialize(bb, (RewardInfoValueGameSpin)rewardInfo.value);
                break;

            case RewardType.DAILY_BOOST:
                Serialize(bb, (RewardInfoValueDailyBoost)rewardInfo.value);
                break;

            case RewardType.TIER_UPGRADE:
                Serialize(bb, (RewardInfoValueTierUpgrade)rewardInfo.value);
                break;

            case RewardType.PROGRAMMED_WIN:
                Serialize(bb, (RewardInfoValueProgrammedWin)rewardInfo.value);
                break;

            case RewardType.EXP_MULTIPLY:
                Serialize(bb, (RewardInfoValueExpMultiply)rewardInfo.value);
                break;

            case RewardType.RANDOM:
                Serialize(bb, (RewardInfoValueRandom)rewardInfo.value);
                break;

            case RewardType.SOCIAL_CREDIT:
                Serialize(bb, (RewardInfoValueSocialCredit)rewardInfo.value);
                break;

            case RewardType.GAME_DEAL:
                Serialize(bb, (RewardInfoValueGameDeal)rewardInfo.value);
                break;

            case RewardType.TICKETED_BONUS_TICKET:
                Serialize(bb, (RewardInfoValueTicketedBonusTicket)rewardInfo.value);
                break;

            case RewardType.SCRATCHER:
                Serialize(bb, (RewardInfoValueScratcher)rewardInfo.value);
                break;

            case RewardType.COLLECTING_GAME_PACK:
                Serialize(bb, (RewardInfoCollectingGamePack)rewardInfo.value);
                break;

            case RewardType.CREDIT_WITH_MULTIPLIER:
                Serialize(bb, (RewardInfoValueCreditWithMultiplier)rewardInfo.value);
                break;

            case RewardType.CLUB_CREDIT:
                Serialize(bb, (RewardInfoValueClubCredit)rewardInfo.value);
                break;

            case RewardType.GEM:
                Serialize(bb, (RewardInfoValueGem)rewardInfo.value);
                break;

            case RewardType.SCRATCHER_FOR_INBOX:
                Serialize(bb, (RewardInfoValueScratcherForInbox)rewardInfo.value);
                break;

            case RewardType.MEGA_WHEEL_SPIN:
                Serialize(bb, (RewardInfoValueMegaWheelSpin)rewardInfo.value);
                break;

            case RewardType.DAILY_DELIVERY:
                Serialize(bb, (RewardInfoValueDailyDelivery)rewardInfo.value);
                break;

            case RewardType.SEASON_PASS_POINT:
                Serialize(bb, (RewardInfoValueSeasonPassPoint)rewardInfo.value);
                break;

            case RewardType.BOSS_RAIDERS_ENERGY:
                Serialize(bb, (RewardInfoValueBossRaidersEnergy)rewardInfo.value);
                break;

            case RewardType.CLUB_ARENA_ENERGY:
                Serialize(bb, (RewardInfoValueClubArenaEnergy)rewardInfo.value);
                break;

            case RewardType.GAME_PLAY:
                Serialize(bb, (RewardInfoValueGamePlay)rewardInfo.value);
                break;

            case RewardType.SPIN_DEAL:
                Serialize(bb, (RewardInfoValueSpinDeal)rewardInfo.value);
                break;

            case RewardType.HIDDEN_UNIVERSE_FINDER:
                Serialize(bb, (RewardInfoValueHiddenUniverseFinder)rewardInfo.value);
                break;

            case RewardType.PL:
                Serialize(bb, (RewardInfoValueProgrammedLoss)rewardInfo.value);
                break;

            case RewardType.HOG_DEAL:
                Serialize(bb, (RewardInfoValueHogDeal)rewardInfo.value);
                break;

            case RewardType.TICKETED_BONUS_TICKET_FOR_BOOST:
                Serialize(bb, (RewardInfoValueTicketedBonusTicket)rewardInfo.value);
                break;

            case RewardType.VIP_LOUNGE_OPEN_TICKET:
                Serialize(bb, (RewardInfoValueVipLoungeOpenTicket)rewardInfo.value);
                break;

            case RewardType.BOSS_RAIDERS_DEAL:
                Serialize(bb, (RewardInfoValueBossRaidersDeal)rewardInfo.value);
                break;

            case RewardType.BOSS_RAIDERS_DEAL_SPIN:
                Serialize(bb, (RewardInfoValueBossRaidersDealSpin)rewardInfo.value);
                break;

            case RewardType.INVITE_INSTALL_WITH_TIER:
                Serialize(bb, (RewardInfoValueInviteInstallWithTier)rewardInfo.value);
                break;

            case RewardType.DEPOT:
                Serialize(bb, (RewardInfoValueDepot)rewardInfo.value);
                break;

            case RewardType.WILD_PUZZLE:
                Serialize(bb, (RewardInfoValueWildPuzzle)rewardInfo.value);
                break;

            case RewardType.VIP_LOUNGE_POINT:
                Serialize(bb, (RewardInfoValueVipLoungePoint)rewardInfo.value);
                break;

            case RewardType.EXP_MULTIPLY_EXTENDABLE:
                Serialize(bb, (RewardInfoValueExpMultiplyExtendable)rewardInfo.value);
                break;

            case RewardType.BUCKS_GIFT:
                Serialize(bb, (RewardInfoValueBucksGift)rewardInfo.value);
                break;
            }
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "value");
        }
        BlackboardUtils.SetOrCreateValue(bb, "inboxInfo", rewardInfo.inboxInfo);
    }

    public static void Serialize(IBlackboard bb, RewardInfoCollectingGamePack rewardInfoCollectingGamePack)
    {
        if (rewardInfoCollectingGamePack == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "packId", rewardInfoCollectingGamePack.packId);
        BlackboardUtils.SetOrCreateValue(bb, "collectingGameId", rewardInfoCollectingGamePack.collectingGameId);
        BlackboardUtils.SetOrCreateValue(bb, "count", rewardInfoCollectingGamePack.count);
    }

    public static void Serialize(IBlackboard bb, RewardInfoForProduct rewardInfoForProduct)
    {
        if (rewardInfoForProduct == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "type", rewardInfoForProduct.type);
        if (rewardInfoForProduct.value != null)
        {
            switch (rewardInfoForProduct.type)
            {
            case RewardType.CREDIT:
                Serialize(bb, (RewardInfoValueCredit)rewardInfoForProduct.value);
                break;

            case RewardType.RP:
                Serialize(bb, (RewardInfoValueRp)rewardInfoForProduct.value);
                break;

            case RewardType.DAILY_BONUS_WHEEL_SPIN:
                Serialize(bb, (RewardInfoValueDailyBonusWheelSpin)rewardInfoForProduct.value);
                break;

            case RewardType.GAME_SPIN:
                Serialize(bb, (RewardInfoValueGameSpin)rewardInfoForProduct.value);
                break;

            case RewardType.DAILY_BOOST:
                Serialize(bb, (RewardInfoValueDailyBoost)rewardInfoForProduct.value);
                break;

            case RewardType.TIER_UPGRADE:
                Serialize(bb, (RewardInfoValueTierUpgrade)rewardInfoForProduct.value);
                break;

            case RewardType.PROGRAMMED_WIN:
                Serialize(bb, (RewardInfoValueProgrammedWin)rewardInfoForProduct.value);
                break;

            case RewardType.RANDOM:
                Serialize(bb, (RewardInfoValueRandom)rewardInfoForProduct.value);
                break;

            case RewardType.SOCIAL_CREDIT:
                Serialize(bb, (RewardInfoValueSocialCredit)rewardInfoForProduct.value);
                break;

            case RewardType.GAME_DEAL:
                Serialize(bb, (RewardInfoValueGameDeal)rewardInfoForProduct.value);
                break;

            case RewardType.TICKETED_BONUS_TICKET:
                Serialize(bb, (RewardInfoValueTicketedBonusTicket)rewardInfoForProduct.value);
                break;

            case RewardType.SCRATCHER:
                Serialize(bb, (RewardInfoValueScratcher)rewardInfoForProduct.value);
                break;

            case RewardType.COLLECTING_GAME_PACK:
                Serialize(bb, (RewardInfoCollectingGamePack)rewardInfoForProduct.value);
                break;

            case RewardType.CREDIT_WITH_MULTIPLIER:
                Serialize(bb, (RewardInfoValueCreditWithMultiplier)rewardInfoForProduct.value);
                break;

            case RewardType.CLUB_CREDIT:
                Serialize(bb, (RewardInfoValueClubCredit)rewardInfoForProduct.value);
                break;

            case RewardType.GEM:
                Serialize(bb, (RewardInfoValueGem)rewardInfoForProduct.value);
                break;

            case RewardType.SCRATCHER_FOR_INBOX:
                Serialize(bb, (RewardInfoValueScratcherForInbox)rewardInfoForProduct.value);
                break;

            case RewardType.MEGA_WHEEL_SPIN:
                Serialize(bb, (RewardInfoValueMegaWheelSpin)rewardInfoForProduct.value);
                break;

            case RewardType.DAILY_DELIVERY:
                Serialize(bb, (RewardInfoValueDailyDelivery)rewardInfoForProduct.value);
                break;

            case RewardType.SEASON_PASS_POINT:
                Serialize(bb, (RewardInfoValueSeasonPassPoint)rewardInfoForProduct.value);
                break;

            case RewardType.BOSS_RAIDERS_ENERGY:
                Serialize(bb, (RewardInfoValueBossRaidersEnergy)rewardInfoForProduct.value);
                break;

            case RewardType.CLUB_ARENA_ENERGY:
                Serialize(bb, (RewardInfoValueClubArenaEnergy)rewardInfoForProduct.value);
                break;

            case RewardType.GAME_PLAY:
                Serialize(bb, (RewardInfoValueGamePlay)rewardInfoForProduct.value);
                break;

            case RewardType.SPIN_DEAL:
                Serialize(bb, (RewardInfoValueSpinDeal)rewardInfoForProduct.value);
                break;

            case RewardType.HIDDEN_UNIVERSE_FINDER:
                Serialize(bb, (RewardInfoValueHiddenUniverseFinder)rewardInfoForProduct.value);
                break;

            case RewardType.PL:
                Serialize(bb, (RewardInfoValueProgrammedLoss)rewardInfoForProduct.value);
                break;

            case RewardType.HOG_DEAL:
                Serialize(bb, (RewardInfoValueHogDeal)rewardInfoForProduct.value);
                break;

            case RewardType.TICKETED_BONUS_TICKET_FOR_BOOST:
                Serialize(bb, (RewardInfoValueTicketedBonusTicket)rewardInfoForProduct.value);
                break;

            case RewardType.VIP_LOUNGE_OPEN_TICKET:
                Serialize(bb, (RewardInfoValueVipLoungeOpenTicket)rewardInfoForProduct.value);
                break;

            case RewardType.BOSS_RAIDERS_DEAL:
                Serialize(bb, (RewardInfoValueBossRaidersDeal)rewardInfoForProduct.value);
                break;

            case RewardType.BOSS_RAIDERS_DEAL_SPIN:
                Serialize(bb, (RewardInfoValueBossRaidersDealSpin)rewardInfoForProduct.value);
                break;

            case RewardType.INVITE_INSTALL_WITH_TIER:
                Serialize(bb, (RewardInfoValueInviteInstallWithTier)rewardInfoForProduct.value);
                break;

            case RewardType.DEPOT:
                Serialize(bb, (RewardInfoValueDepot)rewardInfoForProduct.value);
                break;

            case RewardType.WILD_PUZZLE:
                Serialize(bb, (RewardInfoValueWildPuzzle)rewardInfoForProduct.value);
                break;

            case RewardType.VIP_LOUNGE_POINT:
                Serialize(bb, (RewardInfoValueVipLoungePoint)rewardInfoForProduct.value);
                break;

            case RewardType.BUCKS_GIFT:
                Serialize(bb, (RewardInfoValueBucksGift)rewardInfoForProduct.value);
                break;
            }
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "value");
        }
    }

    public static void Serialize(IBlackboard bb, RewardInfoValueBossRaidersDeal rewardInfoValueBossRaidersDeal)
    {
        if (rewardInfoValueBossRaidersDeal == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "themeId", rewardInfoValueBossRaidersDeal.themeId);
        BlackboardUtils.SetOrCreateValue(bb, "multiplier", rewardInfoValueBossRaidersDeal.multiplier);
        BlackboardUtils.SetOrCreateValue(bb, "dealCount", rewardInfoValueBossRaidersDeal.dealCount);
        BlackboardUtils.SetOrCreateValue(bb, "spinPerDeal", rewardInfoValueBossRaidersDeal.spinPerDeal);
    }

    public static void Serialize(IBlackboard bb, RewardInfoValueBossRaidersDealSpin rewardInfoValueBossRaidersDealSpin)
    {
        if (rewardInfoValueBossRaidersDealSpin == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "dealUuid", rewardInfoValueBossRaidersDealSpin.dealUuid);
        BlackboardUtils.SetOrCreateValue(bb, "spinCount", rewardInfoValueBossRaidersDealSpin.spinCount);
    }

    public static void Serialize(IBlackboard bb, RewardInfoValueBossRaidersEnergy rewardInfoValueBossRaidersEnergy)
    {
        if (rewardInfoValueBossRaidersEnergy == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "energy", rewardInfoValueBossRaidersEnergy.energy);
    }

    public static void Serialize(IBlackboard bb, RewardInfoValueBucksGift rewardInfoValueBucksGift)
    {
        if (rewardInfoValueBucksGift == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "paidBucks", rewardInfoValueBucksGift.paidBucks);
        BlackboardUtils.SetOrCreateValue(bb, "bonusBucks", rewardInfoValueBucksGift.bonusBucks);
        BlackboardUtils.SetOrCreateValue(bb, "freeBucks", rewardInfoValueBucksGift.freeBucks);
    }

    public static void Serialize(IBlackboard bb, RewardInfoValueClubArenaEnergy rewardInfoValueClubArenaEnergy)
    {
        if (rewardInfoValueClubArenaEnergy == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "energy", rewardInfoValueClubArenaEnergy.energy);
    }

    public static void Serialize(IBlackboard bb, RewardInfoValueClubCredit rewardInfoValueClubCredit)
    {
        if (rewardInfoValueClubCredit == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "memberCredit", rewardInfoValueClubCredit.memberCredit);
    }

    public static void Serialize(IBlackboard bb, RewardInfoValueCredit rewardInfoValueCredit)
    {
        if (rewardInfoValueCredit == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "credit", rewardInfoValueCredit.credit);
        BlackboardUtils.SetOrCreateValue(bb, "applyTierMultiplier", rewardInfoValueCredit.applyTierMultiplier);
        BlackboardUtils.SetOrCreateValue(bb, "viewAd", rewardInfoValueCredit.viewAd);
        BlackboardUtils.SetOrCreateValue(bb, "applyLevelMultiplier", rewardInfoValueCredit.applyLevelMultiplier);
    }

    public static void Serialize(IBlackboard bb, RewardInfoValueCreditWithMultiplier rewardInfoValueCreditWithMultiplier)
    {
        if (rewardInfoValueCreditWithMultiplier == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "credit", rewardInfoValueCreditWithMultiplier.credit);
        BlackboardUtils.SetOrCreateValue(bb, "applyTierMultiplier", rewardInfoValueCreditWithMultiplier.applyTierMultiplier);
        BlackboardUtils.SetOrCreateValue(bb, "multiplierNumerator", rewardInfoValueCreditWithMultiplier.multiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, RewardInfoValueDailyBonusWheelSpin rewardInfoValueDailyBonusWheelSpin)
    {
        if (rewardInfoValueDailyBonusWheelSpin == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "spinCount", rewardInfoValueDailyBonusWheelSpin.spinCount);
        BlackboardUtils.SetOrCreateValue(bb, "viewAd", rewardInfoValueDailyBonusWheelSpin.viewAd);
    }

    public static void Serialize(IBlackboard bb, RewardInfoValueDailyBoost rewardInfoValueDailyBoost)
    {
        if (rewardInfoValueDailyBoost == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "baseCreditPerDay", rewardInfoValueDailyBoost.baseCreditPerDay);
        BlackboardUtils.SetOrCreateValue(bb, "totalDayCount", rewardInfoValueDailyBoost.totalDayCount);
        BlackboardUtils.SetOrCreateValue(bb, "baseGemPerDay", rewardInfoValueDailyBoost.baseGemPerDay);
        BlackboardUtils.SetOrCreateValue(bb, "viewAd", rewardInfoValueDailyBoost.viewAd);
        BlackboardUtils.SetOrCreateValue(bb, "applyLevelMultiplier", rewardInfoValueDailyBoost.applyLevelMultiplier);
    }

    public static void Serialize(IBlackboard bb, RewardInfoValueDailyDelivery rewardInfoValueDailyDelivery)
    {
        if (rewardInfoValueDailyDelivery == null) { return; }
    }

    public static void Serialize(IBlackboard bb, RewardInfoValueDepot rewardInfoValueDepot)
    {
        if (rewardInfoValueDepot == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "depotType", rewardInfoValueDepot.depotType);
        BlackboardUtils.SetOrCreateValue(bb, "count", rewardInfoValueDepot.count);
    }

    public static void Serialize(IBlackboard bb, RewardInfoValueExpMultiply rewardInfoValueExpMultiply)
    {
        if (rewardInfoValueExpMultiply == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "durationSec", rewardInfoValueExpMultiply.durationSec);
        BlackboardUtils.SetOrCreateValue(bb, "multiplierNumerator", rewardInfoValueExpMultiply.multiplierNumerator);
        BlackboardUtils.SetOrCreateValue(bb, "viewAd", rewardInfoValueExpMultiply.viewAd);
    }

    public static void Serialize(IBlackboard bb, RewardInfoValueExpMultiplyExtendable rewardInfoValueExpMultiplyExtendable)
    {
        if (rewardInfoValueExpMultiplyExtendable == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "durationSec", rewardInfoValueExpMultiplyExtendable.durationSec);
        BlackboardUtils.SetOrCreateValue(bb, "multiplierNumerator", rewardInfoValueExpMultiplyExtendable.multiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, RewardInfoValueGameDeal rewardInfoValueGameDeal)
    {
        if (rewardInfoValueGameDeal == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", rewardInfoValueGameDeal.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "count", rewardInfoValueGameDeal.count);
        BlackboardUtils.SetOrCreateValue(bb, "betPerHand", rewardInfoValueGameDeal.betPerHand);
        BlackboardUtils.SetOrCreateValue(bb, "handCount", rewardInfoValueGameDeal.handCount);
        BlackboardUtils.SetOrCreateValue(bb, "viewAd", rewardInfoValueGameDeal.viewAd);
    }

    public static void Serialize(IBlackboard bb, RewardInfoValueGamePlay rewardInfoValueGamePlay)
    {
        if (rewardInfoValueGamePlay == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", rewardInfoValueGamePlay.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "count", rewardInfoValueGamePlay.count);
        BlackboardUtils.SetOrCreateValue(bb, "betPerTicket", rewardInfoValueGamePlay.betPerTicket);
        BlackboardUtils.SetOrCreateValue(bb, "ticketCount", rewardInfoValueGamePlay.ticketCount);
        BlackboardUtils.SetOrCreateValue(bb, "viewAd", rewardInfoValueGamePlay.viewAd);
    }

    public static void Serialize(IBlackboard bb, RewardInfoValueGameSpin rewardInfoValueGameSpin)
    {
        if (rewardInfoValueGameSpin == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", rewardInfoValueGameSpin.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "spinCount", rewardInfoValueGameSpin.spinCount);
        BlackboardUtils.SetOrCreateValue(bb, "bet", rewardInfoValueGameSpin.bet);
        BlackboardUtils.SetOrCreateValue(bb, "totalBet", rewardInfoValueGameSpin.totalBet);
        BlackboardUtils.SetOrCreateValue(bb, "viewAd", rewardInfoValueGameSpin.viewAd);
        BlackboardUtils.SetOrCreateValue(bb, "applyLevelMultiplier", rewardInfoValueGameSpin.applyLevelMultiplier);
        BlackboardUtils.SetOrCreateValue(bb, "applyTierMultiplier", rewardInfoValueGameSpin.applyTierMultiplier);
    }

    public static void Serialize(IBlackboard bb, RewardInfoValueGem rewardInfoValueGem)
    {
        if (rewardInfoValueGem == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gem", rewardInfoValueGem.gem);
        BlackboardUtils.SetOrCreateValue(bb, "viewAd", rewardInfoValueGem.viewAd);
    }

    public static void Serialize(IBlackboard bb, RewardInfoValueHiddenUniverseFinder rewardInfoValueHiddenUniverseFinder)
    {
        if (rewardInfoValueHiddenUniverseFinder == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "finder", rewardInfoValueHiddenUniverseFinder.finder);
    }

    public static void Serialize(IBlackboard bb, RewardInfoValueHogDeal rewardInfoValueHogDeal)
    {
        if (rewardInfoValueHogDeal == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "hogDealPresetId", rewardInfoValueHogDeal.hogDealPresetId);
        BlackboardUtils.SetOrCreateValue(bb, "multiplier", rewardInfoValueHogDeal.multiplier);
        BlackboardUtils.SetOrCreateValue(bb, "count", rewardInfoValueHogDeal.count);
        BlackboardUtils.SetOrCreateValue(bb, "maxWinCredit", rewardInfoValueHogDeal.maxWinCredit);
    }

    public static void Serialize(IBlackboard bb, RewardInfoValueInviteInstallWithTier rewardInfoValueInviteInstallWithTier)
    {
        if (rewardInfoValueInviteInstallWithTier == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "targetTier", rewardInfoValueInviteInstallWithTier.targetTier);
        BlackboardUtils.SetOrCreateValue(bb, "snsRewardCredit", rewardInfoValueInviteInstallWithTier.snsRewardCredit);
        BlackboardUtils.SetOrCreateValue(bb, "fbMessageRewardCredit", rewardInfoValueInviteInstallWithTier.fbMessageRewardCredit);
    }

    public static void Serialize(IBlackboard bb, RewardInfoValueMegaWheelSpin rewardInfoValueMegaWheelSpin)
    {
        if (rewardInfoValueMegaWheelSpin == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "spinCount", rewardInfoValueMegaWheelSpin.spinCount);
    }

    public static void Serialize(IBlackboard bb, RewardInfoValueProgrammedLoss rewardInfoValueProgrammedLoss)
    {
        if (rewardInfoValueProgrammedLoss == null) { return; }
    }

    public static void Serialize(IBlackboard bb, RewardInfoValueProgrammedWin rewardInfoValueProgrammedWin)
    {
        if (rewardInfoValueProgrammedWin == null) { return; }
    }

    public static void Serialize(IBlackboard bb, RewardInfoValuePurchaseCoupon rewardInfoValuePurchaseCoupon)
    {
        if (rewardInfoValuePurchaseCoupon == null) { return; }
        if (rewardInfoValuePurchaseCoupon.product != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "product"), rewardInfoValuePurchaseCoupon.product);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "product");
        }
        BlackboardUtils.SetOrCreateValue(bb, "applyDiscount", rewardInfoValuePurchaseCoupon.applyDiscount);
        BlackboardUtils.SetOrCreateValue(bb, "viewAd", rewardInfoValuePurchaseCoupon.viewAd);
    }

    public static void Serialize(IBlackboard bb, RewardInfoValueRandom rewardInfoValueRandom)
    {
        if (rewardInfoValueRandom == null) { return; }
    }

    public static void Serialize(IBlackboard bb, RewardInfoValueRp rewardInfoValueRp)
    {
        if (rewardInfoValueRp == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "rp", rewardInfoValueRp.rp);
        BlackboardUtils.SetOrCreateValue(bb, "viewAd", rewardInfoValueRp.viewAd);
    }

    public static void Serialize(IBlackboard bb, RewardInfoValueScratcher rewardInfoValueScratcher)
    {
        if (rewardInfoValueScratcher == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "scratcherRule", rewardInfoValueScratcher.scratcherRule);
        BlackboardUtils.SetOrCreateValue(bb, "backgroundImageUrl", rewardInfoValueScratcher.backgroundImageUrl);
        BlackboardUtils.SetOrCreateValue(bb, "sampleImageUrl", rewardInfoValueScratcher.sampleImageUrl);
        BlackboardUtils.SetOrCreateValue(bb, "inboxImageUrl", rewardInfoValueScratcher.inboxImageUrl);
        BlackboardUtils.SetOrCreateValue(bb, "scratcherPresetId", rewardInfoValueScratcher.scratcherPresetId);
        BlackboardUtils.SetOrCreateValue(bb, "rewardImageUrl", rewardInfoValueScratcher.rewardImageUrl);
        BlackboardUtils.SetOrCreateValue(bb, "scratcherName", rewardInfoValueScratcher.scratcherName);
        BlackboardUtils.SetOrCreateValue(bb, "rewardName", rewardInfoValueScratcher.rewardName);
        BlackboardUtils.SetOrCreateValue(bb, "maxWinCredit", rewardInfoValueScratcher.maxWinCredit);
        BlackboardUtils.SetOrCreateValue(bb, "version", rewardInfoValueScratcher.version);
        BlackboardUtils.SetOrCreateValue(bb, "tierMultiplierNumerator", rewardInfoValueScratcher.tierMultiplierNumerator);
        BlackboardUtils.SetOrCreateValue(bb, "applyLevelMultiplier", rewardInfoValueScratcher.applyLevelMultiplier);
        BlackboardUtils.SetOrCreateValue(bb, "scratcherMultiplierNumerator", rewardInfoValueScratcher.scratcherMultiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, RewardInfoValueScratcherForInbox rewardInfoValueScratcherForInbox)
    {
        if (rewardInfoValueScratcherForInbox == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "scratcherPresetId", rewardInfoValueScratcherForInbox.scratcherPresetId);
        BlackboardUtils.SetOrCreateValue(bb, "rewardImageUrl", rewardInfoValueScratcherForInbox.rewardImageUrl);
        BlackboardUtils.SetOrCreateValue(bb, "count", rewardInfoValueScratcherForInbox.count);
        BlackboardUtils.SetOrCreateValue(bb, "scratcherName", rewardInfoValueScratcherForInbox.scratcherName);
        BlackboardUtils.SetOrCreateValue(bb, "rewardName", rewardInfoValueScratcherForInbox.rewardName);
        BlackboardUtils.SetOrCreateValue(bb, "maxWinCredit", rewardInfoValueScratcherForInbox.maxWinCredit);
        BlackboardUtils.SetOrCreateValue(bb, "version", rewardInfoValueScratcherForInbox.version);
        BlackboardUtils.SetOrCreateValue(bb, "viewAd", rewardInfoValueScratcherForInbox.viewAd);
        BlackboardUtils.SetOrCreateValue(bb, "applyTierMultiplier", rewardInfoValueScratcherForInbox.applyTierMultiplier);
    }

    public static void Serialize(IBlackboard bb, RewardInfoValueSeasonPassPoint rewardInfoValueSeasonPassPoint)
    {
        if (rewardInfoValueSeasonPassPoint == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "point", rewardInfoValueSeasonPassPoint.point);
    }

    public static void Serialize(IBlackboard bb, RewardInfoValueSocialCredit rewardInfoValueSocialCredit)
    {
        if (rewardInfoValueSocialCredit == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "credit", rewardInfoValueSocialCredit.credit);
    }

    public static void Serialize(IBlackboard bb, RewardInfoValueSpinDeal rewardInfoValueSpinDeal)
    {
        if (rewardInfoValueSpinDeal == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", rewardInfoValueSpinDeal.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "baseBet", rewardInfoValueSpinDeal.baseBet);
        BlackboardUtils.SetOrCreateValue(bb, "spinCount", rewardInfoValueSpinDeal.spinCount);
        BlackboardUtils.SetOrCreateValue(bb, "totalBet", rewardInfoValueSpinDeal.totalBet);
    }

    public static void Serialize(IBlackboard bb, RewardInfoValueTicketedBonusTicket rewardInfoValueTicketedBonusTicket)
    {
        if (rewardInfoValueTicketedBonusTicket == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", rewardInfoValueTicketedBonusTicket.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "baseBet", rewardInfoValueTicketedBonusTicket.baseBet);
        BlackboardUtils.SetOrCreateValue(bb, "extraBet", rewardInfoValueTicketedBonusTicket.extraBet);
        BlackboardUtils.SetOrCreateValue(bb, "applyTierMultiplier", rewardInfoValueTicketedBonusTicket.applyTierMultiplier);
        BlackboardUtils.SetOrCreateValue(bb, "gameCount", rewardInfoValueTicketedBonusTicket.gameCount);
        BlackboardUtils.SetOrCreateValue(bb, "tag", rewardInfoValueTicketedBonusTicket.tag);
        BlackboardUtils.SetOrCreateValue(bb, "viewAd", rewardInfoValueTicketedBonusTicket.viewAd);
        BlackboardUtils.SetOrCreateValue(bb, "applyLevelMultiplier", rewardInfoValueTicketedBonusTicket.applyLevelMultiplier);
    }

    public static void Serialize(IBlackboard bb, RewardInfoValueTierUpgrade rewardInfoValueTierUpgrade)
    {
        if (rewardInfoValueTierUpgrade == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "targetTier", rewardInfoValueTierUpgrade.targetTier);
        BlackboardUtils.SetOrCreateValue(bb, "viewAd", rewardInfoValueTierUpgrade.viewAd);
    }

    public static void Serialize(IBlackboard bb, RewardInfoValueVipLoungeOpenTicket rewardInfoValueVipLoungeOpenTicket)
    {
        if (rewardInfoValueVipLoungeOpenTicket == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "openDays", rewardInfoValueVipLoungeOpenTicket.openDays);
    }

    public static void Serialize(IBlackboard bb, RewardInfoValueVipLoungePoint rewardInfoValueVipLoungePoint)
    {
        if (rewardInfoValueVipLoungePoint == null) { return; }
    }

    public static void Serialize(IBlackboard bb, RewardInfoValueWildPuzzle rewardInfoValueWildPuzzle)
    {
        if (rewardInfoValueWildPuzzle == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "count", rewardInfoValueWildPuzzle.count);
    }

    public static void Serialize(IBlackboard bb, RewardInfoWithIsClaimed rewardInfoWithIsClaimed)
    {
        if (rewardInfoWithIsClaimed == null) { return; }
        if (rewardInfoWithIsClaimed.reward != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "reward"), rewardInfoWithIsClaimed.reward);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "reward");
        }
        BlackboardUtils.SetOrCreateValue(bb, "isClaimed", rewardInfoWithIsClaimed.isClaimed);
    }

    public static void Serialize(IBlackboard bb, RewardInfoWithIsClaimedV2 rewardInfoWithIsClaimedV2)
    {
        if (rewardInfoWithIsClaimedV2 == null) { return; }
        BlackboardUtils.SetOrCreateList(bb, "rewardList", rewardInfoWithIsClaimedV2.rewardList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "isClaimed", rewardInfoWithIsClaimedV2.isClaimed);
    }

    public static void Serialize(IBlackboard bb, RewardListVisualizeInfo rewardListVisualizeInfo)
    {
        if (rewardListVisualizeInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "useRewardPackageIcon", rewardListVisualizeInfo.useRewardPackageIcon);
        BlackboardUtils.SetOrCreateValue(bb, "rewardPackageIconUrl", rewardListVisualizeInfo.rewardPackageIconUrl);
        BlackboardUtils.SetOrCreateList(bb, "rewardViewInfoList", rewardListVisualizeInfo.rewardViewInfoList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "rewardPackageIconSize", rewardListVisualizeInfo.rewardPackageIconSize);
    }

    public static void Serialize(IBlackboard bb, RewardResult rewardResult)
    {
        if (rewardResult == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "rewardType", rewardResult.rewardType);
        if (rewardResult.rewardResult != null)
        {
            switch (rewardResult.rewardType)
            {
            case RewardType.CREDIT:
                Serialize(bb, (RewardResultCredit)rewardResult.rewardResult);
                break;

            case RewardType.RP:
                Serialize(bb, (RewardResultRp)rewardResult.rewardResult);
                break;

            case RewardType.DAILY_BONUS_WHEEL_SPIN:
                Serialize(bb, (RewardResultDailyBonusWheelSpin)rewardResult.rewardResult);
                break;

            case RewardType.PURCHASE_COUPON:
                Serialize(bb, (RewardResultPurchaseCoupon)rewardResult.rewardResult);
                break;

            case RewardType.GAME_SPIN:
                Serialize(bb, (RewardResultGameSpin)rewardResult.rewardResult);
                break;

            case RewardType.DAILY_BOOST:
                Serialize(bb, (RewardResultDailyBoost)rewardResult.rewardResult);
                break;

            case RewardType.TIER_UPGRADE:
                Serialize(bb, (RewardResultTierUpgrade)rewardResult.rewardResult);
                break;

            case RewardType.PROGRAMMED_WIN:
                Serialize(bb, (RewardResultProgrammedWin)rewardResult.rewardResult);
                break;

            case RewardType.EXP_MULTIPLY:
                Serialize(bb, (RewardResultExpMultiply)rewardResult.rewardResult);
                break;

            case RewardType.RANDOM:
                Serialize(bb, (RewardResultRandom)rewardResult.rewardResult);
                break;

            case RewardType.SOCIAL_CREDIT:
                Serialize(bb, (RewardResultSocialCredit)rewardResult.rewardResult);
                break;

            case RewardType.GAME_DEAL:
                Serialize(bb, (RewardResultGameDeal)rewardResult.rewardResult);
                break;

            case RewardType.TICKETED_BONUS_TICKET:
                Serialize(bb, (RewardResultTicketedBonusTicket)rewardResult.rewardResult);
                break;

            case RewardType.SCRATCHER:
                Serialize(bb, (RewardResultScratcher)rewardResult.rewardResult);
                break;

            case RewardType.COLLECTING_GAME_PACK:
                Serialize(bb, (RewardResultCollectingGamePack)rewardResult.rewardResult);
                break;

            case RewardType.CREDIT_WITH_MULTIPLIER:
                Serialize(bb, (RewardResultCreditWithMultiplier)rewardResult.rewardResult);
                break;

            case RewardType.CLUB_CREDIT:
                Serialize(bb, (RewardResultClubCredit)rewardResult.rewardResult);
                break;

            case RewardType.GEM:
                Serialize(bb, (RewardResultGem)rewardResult.rewardResult);
                break;

            case RewardType.SCRATCHER_FOR_INBOX:
                Serialize(bb, (RewardResultScratcherForInbox)rewardResult.rewardResult);
                break;

            case RewardType.MEGA_WHEEL_SPIN:
                Serialize(bb, (RewardResultMegaWheelSpin)rewardResult.rewardResult);
                break;

            case RewardType.DAILY_DELIVERY:
                Serialize(bb, (RewardResultDailyDelivery)rewardResult.rewardResult);
                break;

            case RewardType.SEASON_PASS_POINT:
                Serialize(bb, (RewardResultSeasonPassPoint)rewardResult.rewardResult);
                break;

            case RewardType.BOSS_RAIDERS_ENERGY:
                Serialize(bb, (RewardResultBossRaidersEnergy)rewardResult.rewardResult);
                break;

            case RewardType.CLUB_ARENA_ENERGY:
                Serialize(bb, (RewardResultClubArenaEnergy)rewardResult.rewardResult);
                break;

            case RewardType.GAME_PLAY:
                Serialize(bb, (RewardResultGamePlay)rewardResult.rewardResult);
                break;

            case RewardType.SPIN_DEAL:
                Serialize(bb, (RewardResultSpinDeal)rewardResult.rewardResult);
                break;

            case RewardType.HIDDEN_UNIVERSE_FINDER:
                Serialize(bb, (RewardResultHiddenUniverseFinder)rewardResult.rewardResult);
                break;

            case RewardType.PL:
                Serialize(bb, (RewardResultProgrammedLoss)rewardResult.rewardResult);
                break;

            case RewardType.HOG_DEAL:
                Serialize(bb, (RewardResultHogDeal)rewardResult.rewardResult);
                break;

            case RewardType.TICKETED_BONUS_TICKET_FOR_BOOST:
                Serialize(bb, (RewardResultTicketedBonusTicket)rewardResult.rewardResult);
                break;

            case RewardType.VIP_LOUNGE_OPEN_TICKET:
                Serialize(bb, (RewardResultVipLoungeOpenTicket)rewardResult.rewardResult);
                break;

            case RewardType.BOSS_RAIDERS_DEAL:
                Serialize(bb, (RewardResultBossRaidersDeal)rewardResult.rewardResult);
                break;

            case RewardType.BOSS_RAIDERS_DEAL_SPIN:
                Serialize(bb, (RewardResultBossRaidersDealSpin)rewardResult.rewardResult);
                break;

            case RewardType.INVITE_INSTALL_WITH_TIER:
                Serialize(bb, (RewardResultInviteInstallWithTier)rewardResult.rewardResult);
                break;

            case RewardType.DEPOT:
                Serialize(bb, (RewardResultDepot)rewardResult.rewardResult);
                break;

            case RewardType.WILD_PUZZLE:
                Serialize(bb, (RewardResultWildPuzzle)rewardResult.rewardResult);
                break;

            case RewardType.VIP_LOUNGE_POINT:
                Serialize(bb, (RewardResultVipLoungePoint)rewardResult.rewardResult);
                break;

            case RewardType.EXP_MULTIPLY_EXTENDABLE:
                Serialize(bb, (RewardResultExpMultiplyExtendable)rewardResult.rewardResult);
                break;

            case RewardType.BUCKS_GIFT:
                Serialize(bb, (RewardResultBucksGift)rewardResult.rewardResult);
                break;
            }
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "rewardResult");
        }
        BlackboardUtils.SetOrCreateValue(bb, "isFromRandom", rewardResult.isFromRandom);
        BlackboardUtils.SetOrCreateValue(bb, "isFromBingo", rewardResult.isFromBingo);
        BlackboardUtils.SetOrCreateValue(bb, "isSentToInbox", rewardResult.isSentToInbox);
        if (rewardResult.rewardInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "rewardInfo"), rewardResult.rewardInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "rewardInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "title", rewardResult.title);
        if (rewardResult.dailyDeliveryInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "dailyDeliveryInfo"), rewardResult.dailyDeliveryInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "dailyDeliveryInfo");
        }
    }

    public static void Serialize(IBlackboard bb, RewardResultBossRaidersDeal rewardResultBossRaidersDeal)
    {
        if (rewardResultBossRaidersDeal == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "dealCount", rewardResultBossRaidersDeal.dealCount);
        BlackboardUtils.SetOrCreateValue(bb, "spinPerDeal", rewardResultBossRaidersDeal.spinPerDeal);
    }

    public static void Serialize(IBlackboard bb, RewardResultBossRaidersDealSpin rewardResultBossRaidersDealSpin)
    {
        if (rewardResultBossRaidersDealSpin == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "totalSpinCount", rewardResultBossRaidersDealSpin.totalSpinCount);
    }

    public static void Serialize(IBlackboard bb, RewardResultBossRaidersEnergy rewardResultBossRaidersEnergy)
    {
        if (rewardResultBossRaidersEnergy == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "energy", rewardResultBossRaidersEnergy.energy);
    }

    public static void Serialize(IBlackboard bb, RewardResultBucksGift rewardResultBucksGift)
    {
        if (rewardResultBucksGift == null) { return; }
    }

    public static void Serialize(IBlackboard bb, RewardResultClubArenaEnergy rewardResultClubArenaEnergy)
    {
        if (rewardResultClubArenaEnergy == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "energy", rewardResultClubArenaEnergy.energy);
    }

    public static void Serialize(IBlackboard bb, RewardResultClubCredit rewardResultClubCredit)
    {
        if (rewardResultClubCredit == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "memberCredit", rewardResultClubCredit.memberCredit);
        BlackboardUtils.SetOrCreateList(bb, "clubMemberProfileList", rewardResultClubCredit.clubMemberProfileList, Serialize);
    }

    public static void Serialize(IBlackboard bb, RewardResultCollectingGamePack rewardResultCollectingGamePack)
    {
        if (rewardResultCollectingGamePack == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "rewardImageUrl", rewardResultCollectingGamePack.rewardImageUrl);
    }

    public static void Serialize(IBlackboard bb, RewardResultCredit rewardResultCredit)
    {
        if (rewardResultCredit == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "credit", rewardResultCredit.credit);
    }

    public static void Serialize(IBlackboard bb, RewardResultCreditWithMultiplier rewardResultCreditWithMultiplier)
    {
        if (rewardResultCreditWithMultiplier == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "credit", rewardResultCreditWithMultiplier.credit);
        BlackboardUtils.SetOrCreateValue(bb, "multiplierNumerator", rewardResultCreditWithMultiplier.multiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, RewardResultDailyBonusWheelSpin rewardResultDailyBonusWheelSpin)
    {
        if (rewardResultDailyBonusWheelSpin == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "addedSpinCount", rewardResultDailyBonusWheelSpin.addedSpinCount);
        BlackboardUtils.SetOrCreateValue(bb, "totalSpinCount", rewardResultDailyBonusWheelSpin.totalSpinCount);
    }

    public static void Serialize(IBlackboard bb, RewardResultDailyBoost rewardResultDailyBoost)
    {
        if (rewardResultDailyBoost == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "earnCredit", rewardResultDailyBoost.earnCredit);
        if (rewardResultDailyBoost.dailyBoost != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "dailyBoost"), rewardResultDailyBoost.dailyBoost);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "dailyBoost");
        }
        BlackboardUtils.SetOrCreateValue(bb, "earnGem", rewardResultDailyBoost.earnGem);
    }

    public static void Serialize(IBlackboard bb, RewardResultDailyDelivery rewardResultDailyDelivery)
    {
        if (rewardResultDailyDelivery == null) { return; }
    }

    public static void Serialize(IBlackboard bb, RewardResultDepot rewardResultDepot)
    {
        if (rewardResultDepot == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "depotType", rewardResultDepot.depotType);
        BlackboardUtils.SetOrCreateValue(bb, "addedCount", rewardResultDepot.addedCount);
        BlackboardUtils.SetOrCreateValue(bb, "totalCount", rewardResultDepot.totalCount);
        if (rewardResultDepot.buildDreamInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "buildDreamInfo"), rewardResultDepot.buildDreamInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "buildDreamInfo");
        }
    }

    public static void Serialize(IBlackboard bb, RewardResultExpMultiply rewardResultExpMultiply)
    {
        if (rewardResultExpMultiply == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "durationSec", rewardResultExpMultiply.durationSec);
        BlackboardUtils.SetOrCreateValue(bb, "multiplierNumerator", rewardResultExpMultiply.multiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, RewardResultExpMultiplyExtendable rewardResultExpMultiplyExtendable)
    {
        if (rewardResultExpMultiplyExtendable == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "durationSec", rewardResultExpMultiplyExtendable.durationSec);
        BlackboardUtils.SetOrCreateValue(bb, "multiplierNumerator", rewardResultExpMultiplyExtendable.multiplierNumerator);
        BlackboardUtils.SetOrCreateValue(bb, "isExtended", rewardResultExpMultiplyExtendable.isExtended);
    }

    public static void Serialize(IBlackboard bb, RewardResultGameDeal rewardResultGameDeal)
    {
        if (rewardResultGameDeal == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", rewardResultGameDeal.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "addedCount", rewardResultGameDeal.addedCount);
        BlackboardUtils.SetOrCreateValue(bb, "totalCount", rewardResultGameDeal.totalCount);
        BlackboardUtils.SetOrCreateValue(bb, "betPerHand", rewardResultGameDeal.betPerHand);
        BlackboardUtils.SetOrCreateValue(bb, "handCount", rewardResultGameDeal.handCount);
    }

    public static void Serialize(IBlackboard bb, RewardResultGamePlay rewardResultGamePlay)
    {
        if (rewardResultGamePlay == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", rewardResultGamePlay.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "addedCount", rewardResultGamePlay.addedCount);
        BlackboardUtils.SetOrCreateValue(bb, "totalCount", rewardResultGamePlay.totalCount);
        BlackboardUtils.SetOrCreateValue(bb, "betPerTicket", rewardResultGamePlay.betPerTicket);
        BlackboardUtils.SetOrCreateValue(bb, "ticketCount", rewardResultGamePlay.ticketCount);
    }

    public static void Serialize(IBlackboard bb, RewardResultGameSpin rewardResultGameSpin)
    {
        if (rewardResultGameSpin == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", rewardResultGameSpin.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "addedSpinCount", rewardResultGameSpin.addedSpinCount);
        BlackboardUtils.SetOrCreateValue(bb, "totalSpinCount", rewardResultGameSpin.totalSpinCount);
        BlackboardUtils.SetOrCreateValue(bb, "bet", rewardResultGameSpin.bet);
        BlackboardUtils.SetOrCreateValue(bb, "totalBet", rewardResultGameSpin.totalBet);
    }

    public static void Serialize(IBlackboard bb, RewardResultGem rewardResultGem)
    {
        if (rewardResultGem == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gem", rewardResultGem.gem);
    }

    public static void Serialize(IBlackboard bb, RewardResultHiddenUniverseFinder rewardResultHiddenUniverseFinder)
    {
        if (rewardResultHiddenUniverseFinder == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "earnFinder", rewardResultHiddenUniverseFinder.earnFinder);
        BlackboardUtils.SetOrCreateValue(bb, "totalFinder", rewardResultHiddenUniverseFinder.totalFinder);
    }

    public static void Serialize(IBlackboard bb, RewardResultHogDeal rewardResultHogDeal)
    {
        if (rewardResultHogDeal == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "count", rewardResultHogDeal.count);
    }

    public static void Serialize(IBlackboard bb, RewardResultInviteInstallWithTier rewardResultInviteInstallWithTier)
    {
        if (rewardResultInviteInstallWithTier == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "snsInviteUrl", rewardResultInviteInstallWithTier.snsInviteUrl);
    }

    public static void Serialize(IBlackboard bb, RewardResultMegaWheelSpin rewardResultMegaWheelSpin)
    {
        if (rewardResultMegaWheelSpin == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "addedSpinCount", rewardResultMegaWheelSpin.addedSpinCount);
        BlackboardUtils.SetOrCreateValue(bb, "totalSpinCount", rewardResultMegaWheelSpin.totalSpinCount);
    }

    public static void Serialize(IBlackboard bb, RewardResultProgrammedLoss rewardResultProgrammedLoss)
    {
        if (rewardResultProgrammedLoss == null) { return; }
    }

    public static void Serialize(IBlackboard bb, RewardResultProgrammedWin rewardResultProgrammedWin)
    {
        if (rewardResultProgrammedWin == null) { return; }
    }

    public static void Serialize(IBlackboard bb, RewardResultPurchaseCoupon rewardResultPurchaseCoupon)
    {
        if (rewardResultPurchaseCoupon == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "success", rewardResultPurchaseCoupon.success);
    }

    public static void Serialize(IBlackboard bb, RewardResultRandom rewardResultRandom)
    {
        if (rewardResultRandom == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "count", rewardResultRandom.count);
    }

    public static void Serialize(IBlackboard bb, RewardResultRp rewardResultRp)
    {
        if (rewardResultRp == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "rp", rewardResultRp.rp);
    }

    public static void Serialize(IBlackboard bb, RewardResultScratcher rewardResultScratcher)
    {
        if (rewardResultScratcher == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "credit", rewardResultScratcher.credit);
        BlackboardUtils.SetOrCreateValue(bb, "scratcherRule", rewardResultScratcher.scratcherRule);
        if (rewardResultScratcher.scratcherInfo != null)
        {
            switch (rewardResultScratcher.scratcherRule)
            {
            case RewardScratcherRule.INSTANT_CASH_DEFAULT:
                Serialize(bb, (ScratcherInfoInstantCashDefault)rewardResultScratcher.scratcherInfo);
                break;

            case RewardScratcherRule.INSTANT_CASH_WIN_IT_ALL:
                Serialize(bb, (ScratcherInfoInstantCashWinItAll)rewardResultScratcher.scratcherInfo);
                break;

            case RewardScratcherRule.WINNING_NUMBERS_DEFAULT:
                Serialize(bb, (ScratcherInfoWinningNumbersDefault)rewardResultScratcher.scratcherInfo);
                break;

            case RewardScratcherRule.WINNING_NUMBERS_MULTIPLIER:
                Serialize(bb, (ScratcherInfoWinningNumbersMultiplier)rewardResultScratcher.scratcherInfo);
                break;

            case RewardScratcherRule.DICE:
                Serialize(bb, (ScratcherInfoDice)rewardResultScratcher.scratcherInfo);
                break;

            case RewardScratcherRule.POKER_DEFAULT:
                Serialize(bb, (ScratcherInfoPokerDefault)rewardResultScratcher.scratcherInfo);
                break;

            case RewardScratcherRule.POKER_MULTIPLIER:
                Serialize(bb, (ScratcherInfoPokerMultiplier)rewardResultScratcher.scratcherInfo);
                break;

            case RewardScratcherRule.MATCH_THREE_ROW:
                Serialize(bb, (ScratcherInfoMatchThreeRow)rewardResultScratcher.scratcherInfo);
                break;

            case RewardScratcherRule.MATCH_THREE_PRIZE:
                Serialize(bb, (ScratcherInfoMatchThreePrize)rewardResultScratcher.scratcherInfo);
                break;

            case RewardScratcherRule.WINNING_NUMBERS_DYNAMIC_ROWS:
                Serialize(bb, (ScratcherInfoWinningNumbersDynamicRows)rewardResultScratcher.scratcherInfo);
                break;

            case RewardScratcherRule.WINNING_NUMBERS_GLOBAL_MULTIPLIER:
                Serialize(bb, (ScratcherInfoWinningNumbersGlobalMultiplier)rewardResultScratcher.scratcherInfo);
                break;

            case RewardScratcherRule.BINGO_DEFAULT:
                Serialize(bb, (ScratcherInfoBingoDefault)rewardResultScratcher.scratcherInfo);
                break;

            case RewardScratcherRule.BINGO_MULTIPLIER:
                Serialize(bb, (ScratcherInfoBingoMultiplier)rewardResultScratcher.scratcherInfo);
                break;

            case RewardScratcherRule.KENO_STEPPED:
                Serialize(bb, (ScratcherInfoKenoStepped)rewardResultScratcher.scratcherInfo);
                break;

            case RewardScratcherRule.KENO_STEPPED_MULTIPLIER:
                Serialize(bb, (ScratcherInfoKenoSteppedMultiplier)rewardResultScratcher.scratcherInfo);
                break;

            case RewardScratcherRule.KENO_CUBE_SMALL:
                Serialize(bb, (ScratcherInfoKenoCubeSmall)rewardResultScratcher.scratcherInfo);
                break;

            case RewardScratcherRule.KENO_CUBE_BIG:
                Serialize(bb, (ScratcherInfoKenoCubeBig)rewardResultScratcher.scratcherInfo);
                break;

            case RewardScratcherRule.KENO_GENERAL:
                Serialize(bb, (ScratcherInfoKenoGeneral)rewardResultScratcher.scratcherInfo);
                break;

            case RewardScratcherRule.LADDER:
                Serialize(bb, (ScratcherInfoLadder)rewardResultScratcher.scratcherInfo);
                break;

            case RewardScratcherRule.LADDER_BONUS:
                Serialize(bb, (ScratcherInfoLadderBonus)rewardResultScratcher.scratcherInfo);
                break;

            case RewardScratcherRule.LADDER_MULTIPLIER:
                Serialize(bb, (ScratcherInfoLadderMultiplier)rewardResultScratcher.scratcherInfo);
                break;
            }
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "scratcherInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "backgroundImageUrl", rewardResultScratcher.backgroundImageUrl);
        BlackboardUtils.SetOrCreateValue(bb, "sampleImageUrl", rewardResultScratcher.sampleImageUrl);
        BlackboardUtils.SetOrCreateValue(bb, "coverColor", rewardResultScratcher.coverColor);
        BlackboardUtils.SetOrCreateValue(bb, "coverText", rewardResultScratcher.coverText);
        BlackboardUtils.SetOrCreateList(bb, "symbolList", rewardResultScratcher.symbolList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "bigWinType", rewardResultScratcher.bigWinType);
        BlackboardUtils.SetOrCreateValue(bb, "scratcherPresetId", rewardResultScratcher.scratcherPresetId);
        BlackboardUtils.SetOrCreateValue(bb, "rewardImageUrl", rewardResultScratcher.rewardImageUrl);
        BlackboardUtils.SetOrCreateValue(bb, "pokerWinTypeList", rewardResultScratcher.pokerWinTypeList);
        BlackboardUtils.SetOrCreateValue(bb, "pokerWinList", rewardResultScratcher.pokerWinList);
        BlackboardUtils.SetOrCreateValue(bb, "scratcherName", rewardResultScratcher.scratcherName);
        BlackboardUtils.SetOrCreateValue(bb, "rewardName", rewardResultScratcher.rewardName);
        BlackboardUtils.SetOrCreateList(bb, "bingoCallersNumberList", rewardResultScratcher.bingoCallersNumberList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "bingoWinPatternIndexList", rewardResultScratcher.bingoWinPatternIndexList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "version", rewardResultScratcher.version);
    }

    public static void Serialize(IBlackboard bb, RewardResultScratcherForInbox rewardResultScratcherForInbox)
    {
        if (rewardResultScratcherForInbox == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "scratcherPresetId", rewardResultScratcherForInbox.scratcherPresetId);
        BlackboardUtils.SetOrCreateValue(bb, "rewardImageUrl", rewardResultScratcherForInbox.rewardImageUrl);
        BlackboardUtils.SetOrCreateValue(bb, "count", rewardResultScratcherForInbox.count);
        BlackboardUtils.SetOrCreateValue(bb, "scratcherName", rewardResultScratcherForInbox.scratcherName);
        BlackboardUtils.SetOrCreateValue(bb, "rewardName", rewardResultScratcherForInbox.rewardName);
    }

    public static void Serialize(IBlackboard bb, RewardResultSeasonPassPoint rewardResultSeasonPassPoint)
    {
        if (rewardResultSeasonPassPoint == null) { return; }
        if (rewardResultSeasonPassPoint.originalUpdateInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "originalUpdateInfo"), rewardResultSeasonPassPoint.originalUpdateInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "originalUpdateInfo");
        }
        if (rewardResultSeasonPassPoint.updateInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "updateInfo"), rewardResultSeasonPassPoint.updateInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "updateInfo");
        }
    }

    public static void Serialize(IBlackboard bb, RewardResultSocialCredit rewardResultSocialCredit)
    {
        if (rewardResultSocialCredit == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "credit", rewardResultSocialCredit.credit);
        BlackboardUtils.SetOrCreateList(bb, "friendList", rewardResultSocialCredit.friendList, Serialize);
    }

    public static void Serialize(IBlackboard bb, RewardResultSpinDeal rewardResultSpinDeal)
    {
        if (rewardResultSpinDeal == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", rewardResultSpinDeal.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "baseBet", rewardResultSpinDeal.baseBet);
        BlackboardUtils.SetOrCreateValue(bb, "spinCount", rewardResultSpinDeal.spinCount);
        BlackboardUtils.SetOrCreateValue(bb, "totalBet", rewardResultSpinDeal.totalBet);
        BlackboardUtils.SetOrCreateValue(bb, "spinDealId", rewardResultSpinDeal.spinDealId);
    }

    public static void Serialize(IBlackboard bb, RewardResultTicketedBonusTicket rewardResultTicketedBonusTicket)
    {
        if (rewardResultTicketedBonusTicket == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", rewardResultTicketedBonusTicket.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "ticketId", rewardResultTicketedBonusTicket.ticketId);
        BlackboardUtils.SetOrCreateValue(bb, "tag", rewardResultTicketedBonusTicket.tag);
    }

    public static void Serialize(IBlackboard bb, RewardResultTierUpgrade rewardResultTierUpgrade)
    {
        if (rewardResultTierUpgrade == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "originalTier", rewardResultTierUpgrade.originalTier);
        BlackboardUtils.SetOrCreateValue(bb, "upgradedTier", rewardResultTierUpgrade.upgradedTier);
    }

    public static void Serialize(IBlackboard bb, RewardResultVipLoungeOpenTicket rewardResultVipLoungeOpenTicket)
    {
        if (rewardResultVipLoungeOpenTicket == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "addedDays", rewardResultVipLoungeOpenTicket.addedDays);
        if (rewardResultVipLoungeOpenTicket.vipLoungeInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "vipLoungeInfo"), rewardResultVipLoungeOpenTicket.vipLoungeInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "vipLoungeInfo");
        }
    }

    public static void Serialize(IBlackboard bb, RewardResultVipLoungePoint rewardResultVipLoungePoint)
    {
        if (rewardResultVipLoungePoint == null) { return; }
    }

    public static void Serialize(IBlackboard bb, RewardResultWildPuzzle rewardResultWildPuzzle)
    {
        if (rewardResultWildPuzzle == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "addedCount", rewardResultWildPuzzle.addedCount);
        BlackboardUtils.SetOrCreateValue(bb, "totalCount", rewardResultWildPuzzle.totalCount);
    }

    public static void Serialize(IBlackboard bb, RewardViewInfo rewardViewInfo)
    {
        if (rewardViewInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "rewardIndex", rewardViewInfo.rewardIndex);
        BlackboardUtils.SetOrCreateValue(bb, "quantity", rewardViewInfo.quantity);
    }

    public static void Serialize(IBlackboard bb, Room room)
    {
        if (room == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "roomId", room.roomId);
        BlackboardUtils.SetOrCreateList(bb, "players", room.players, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "roomChatChannelId", room.roomChatChannelId);
    }

    public static void Serialize(IBlackboard bb, RoomBoastBigwinRequest roomBoastBigwinRequest)
    {
        if (roomBoastBigwinRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", roomBoastBigwinRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", roomBoastBigwinRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "winCredit", roomBoastBigwinRequest.winCredit);
        BlackboardUtils.SetOrCreateValue(bb, "betCredit", roomBoastBigwinRequest.betCredit);
        BlackboardUtils.SetOrCreateValue(bb, "roomId", roomBoastBigwinRequest.roomId);
    }

    public static void Serialize(IBlackboard bb, RoomEnterRandomRequestV2 roomEnterRandomRequestV2)
    {
        if (roomEnterRandomRequestV2 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", roomEnterRandomRequestV2.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", roomEnterRandomRequestV2.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "contents", roomEnterRandomRequestV2.contents);
        BlackboardUtils.SetOrCreateValue(bb, "isEarlyAccess", roomEnterRandomRequestV2.isEarlyAccess);
        BlackboardUtils.SetOrCreateValue(bb, "gameId", roomEnterRandomRequestV2.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "slotEnterContextId", roomEnterRandomRequestV2.slotEnterContextId);
    }

    public static void Serialize(IBlackboard bb, RoomEnterResponseV3 roomEnterResponseV3)
    {
        if (roomEnterResponseV3 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", roomEnterResponseV3.error);
        if (roomEnterResponseV3.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), roomEnterResponseV3.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", roomEnterResponseV3.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "contents", roomEnterResponseV3.contents);
        if (roomEnterResponseV3.room != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "room"), roomEnterResponseV3.room);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "room");
        }
        BlackboardUtils.SetOrCreateValue(bb, "nextMysteryGiftLevel", roomEnterResponseV3.nextMysteryGiftLevel);
        if (roomEnterResponseV3.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), roomEnterResponseV3.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
        if (roomEnterResponseV3.tournamentInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "tournamentInfo"), roomEnterResponseV3.tournamentInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "tournamentInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "betList", roomEnterResponseV3.betList);
        BlackboardUtils.SetOrCreateList(bb, "restrictionList", roomEnterResponseV3.restrictionList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "gameSpinCountPerBet", roomEnterResponseV3.gameSpinCountPerBet);
        BlackboardUtils.SetOrCreateValue(bb, "bonusSpinCountPerBet", roomEnterResponseV3.bonusSpinCountPerBet);
        if (roomEnterResponseV3.metaGameEnterInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "metaGameEnterInfo"), roomEnterResponseV3.metaGameEnterInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "metaGameEnterInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "useForcedExtraBetRatio", roomEnterResponseV3.useForcedExtraBetRatio);
        BlackboardUtils.SetOrCreateValue(bb, "forcedExtraBetRatioIndexList", roomEnterResponseV3.forcedExtraBetRatioIndexList);
        BlackboardUtils.SetOrCreateValue(bb, "slotEnterContextId", roomEnterResponseV3.slotEnterContextId);
        BlackboardUtils.SetOrCreateValue(bb, "lpEligibleBet", roomEnterResponseV3.lpEligibleBet);
        if (roomEnterResponseV3.metaEligibleBetThreshold != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "metaEligibleBetThreshold"), roomEnterResponseV3.metaEligibleBetThreshold);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "metaEligibleBetThreshold");
        }
        BlackboardUtils.SetOrCreateValue(bb, "defaultBabExtraBetIndex", roomEnterResponseV3.defaultBabExtraBetIndex);
        BlackboardUtils.SetOrCreateValue(bb, "vipLoungeExtendedBetIndex", roomEnterResponseV3.vipLoungeExtendedBetIndex);
        BlackboardUtils.SetOrCreateValue(bb, "jackpotTypeList", roomEnterResponseV3.jackpotTypeList);
        if (roomEnterResponseV3.seasonPassEnterInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "seasonPassEnterInfo"), roomEnterResponseV3.seasonPassEnterInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "seasonPassEnterInfo");
        }
    }

    public static void Serialize(IBlackboard bb, RoomEnterTargetRequestV2 roomEnterTargetRequestV2)
    {
        if (roomEnterTargetRequestV2 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", roomEnterTargetRequestV2.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", roomEnterTargetRequestV2.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "contents", roomEnterTargetRequestV2.contents);
        BlackboardUtils.SetOrCreateValue(bb, "targetRoomId", roomEnterTargetRequestV2.targetRoomId);
        BlackboardUtils.SetOrCreateValue(bb, "isEarlyAccess", roomEnterTargetRequestV2.isEarlyAccess);
        BlackboardUtils.SetOrCreateValue(bb, "slotEnterContextId", roomEnterTargetRequestV2.slotEnterContextId);
    }

    public static void Serialize(IBlackboard bb, RoomMetaInfoRequest roomMetaInfoRequest)
    {
        if (roomMetaInfoRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", roomMetaInfoRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", roomMetaInfoRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "roomId", roomMetaInfoRequest.roomId);
    }

    public static void Serialize(IBlackboard bb, RoomMetaInfoResponse roomMetaInfoResponse)
    {
        if (roomMetaInfoResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", roomMetaInfoResponse.error);
        if (roomMetaInfoResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), roomMetaInfoResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", roomMetaInfoResponse.serverTime);
        if (roomMetaInfoResponse.room != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "room"), roomMetaInfoResponse.room);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "room");
        }
        if (roomMetaInfoResponse.tournamentInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "tournamentInfo"), roomMetaInfoResponse.tournamentInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "tournamentInfo");
        }
    }

    public static void Serialize(IBlackboard bb, RoomStoreInfoResponse roomStoreInfoResponse)
    {
        if (roomStoreInfoResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", roomStoreInfoResponse.error);
        if (roomStoreInfoResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), roomStoreInfoResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", roomStoreInfoResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "contents", roomStoreInfoResponse.contents);
    }

    public static void Serialize(IBlackboard bb, ScratcherInfoBingoDefault scratcherInfoBingoDefault)
    {
        if (scratcherInfoBingoDefault == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "numScratchArea", scratcherInfoBingoDefault.numScratchArea);
        BlackboardUtils.SetOrCreateValue(bb, "numBingoCard", scratcherInfoBingoDefault.numBingoCard);
        BlackboardUtils.SetOrCreateValue(bb, "linePrizeList", scratcherInfoBingoDefault.linePrizeList);
        BlackboardUtils.SetOrCreateValue(bb, "fourCornersPrizeList", scratcherInfoBingoDefault.fourCornersPrizeList);
        BlackboardUtils.SetOrCreateValue(bb, "xPrizeList", scratcherInfoBingoDefault.xPrizeList);
    }

    public static void Serialize(IBlackboard bb, ScratcherInfoBingoMultiplier scratcherInfoBingoMultiplier)
    {
        if (scratcherInfoBingoMultiplier == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "numScratchArea", scratcherInfoBingoMultiplier.numScratchArea);
        BlackboardUtils.SetOrCreateValue(bb, "numBingoCard", scratcherInfoBingoMultiplier.numBingoCard);
        BlackboardUtils.SetOrCreateValue(bb, "linePrizeList", scratcherInfoBingoMultiplier.linePrizeList);
        BlackboardUtils.SetOrCreateValue(bb, "fourCornersPrizeList", scratcherInfoBingoMultiplier.fourCornersPrizeList);
        BlackboardUtils.SetOrCreateValue(bb, "xPrizeList", scratcherInfoBingoMultiplier.xPrizeList);
    }

    public static void Serialize(IBlackboard bb, ScratcherInfoDice scratcherInfoDice)
    {
        if (scratcherInfoDice == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "instantWinCredit", scratcherInfoDice.instantWinCredit);
        BlackboardUtils.SetOrCreateValue(bb, "instantWinSymbol", scratcherInfoDice.instantWinSymbol);
        BlackboardUtils.SetOrCreateValue(bb, "numInstantWin", scratcherInfoDice.numInstantWin);
        BlackboardUtils.SetOrCreateValue(bb, "numDiceTry", scratcherInfoDice.numDiceTry);
        BlackboardUtils.SetOrCreateValue(bb, "diceHitSum", scratcherInfoDice.diceHitSum);
        BlackboardUtils.SetOrCreateValue(bb, "diceTwiceSum", scratcherInfoDice.diceTwiceSum);
    }

    public static void Serialize(IBlackboard bb, ScratcherInfoInstantCashDefault scratcherInfoInstantCashDefault)
    {
        if (scratcherInfoInstantCashDefault == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "hitSymbol", scratcherInfoInstantCashDefault.hitSymbol);
        BlackboardUtils.SetOrCreateValue(bb, "numScratchArea", scratcherInfoInstantCashDefault.numScratchArea);
    }

    public static void Serialize(IBlackboard bb, ScratcherInfoInstantCashWinItAll scratcherInfoInstantCashWinItAll)
    {
        if (scratcherInfoInstantCashWinItAll == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "hitSymbol", scratcherInfoInstantCashWinItAll.hitSymbol);
        BlackboardUtils.SetOrCreateValue(bb, "winItAllSymbol", scratcherInfoInstantCashWinItAll.winItAllSymbol);
        BlackboardUtils.SetOrCreateValue(bb, "numScratchArea", scratcherInfoInstantCashWinItAll.numScratchArea);
    }

    public static void Serialize(IBlackboard bb, ScratcherInfoKenoCubeBig scratcherInfoKenoCubeBig)
    {
        if (scratcherInfoKenoCubeBig == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "numberCount", scratcherInfoKenoCubeBig.numberCount);
        BlackboardUtils.SetOrCreateList(bb, "winningNumbersAroundCube", scratcherInfoKenoCubeBig.winningNumbersAroundCube, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "prizeList", scratcherInfoKenoCubeBig.prizeList);
    }

    public static void Serialize(IBlackboard bb, ScratcherInfoKenoCubeSmall scratcherInfoKenoCubeSmall)
    {
        if (scratcherInfoKenoCubeSmall == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "numberCount", scratcherInfoKenoCubeSmall.numberCount);
        BlackboardUtils.SetOrCreateList(bb, "winningNumbersAroundCube", scratcherInfoKenoCubeSmall.winningNumbersAroundCube, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "prizeList", scratcherInfoKenoCubeSmall.prizeList);
    }

    public static void Serialize(IBlackboard bb, ScratcherInfoKenoGeneral scratcherInfoKenoGeneral)
    {
        if (scratcherInfoKenoGeneral == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "columnCount", scratcherInfoKenoGeneral.columnCount);
        BlackboardUtils.SetOrCreateValue(bb, "columnLength", scratcherInfoKenoGeneral.columnLength);
        BlackboardUtils.SetOrCreateValue(bb, "prizeList", scratcherInfoKenoGeneral.prizeList);
    }

    public static void Serialize(IBlackboard bb, ScratcherInfoKenoStepped scratcherInfoKenoStepped)
    {
        if (scratcherInfoKenoStepped == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "rowCount", scratcherInfoKenoStepped.rowCount);
        BlackboardUtils.SetOrCreateValue(bb, "prizeList", scratcherInfoKenoStepped.prizeList);
    }

    public static void Serialize(IBlackboard bb, ScratcherInfoKenoSteppedMultiplier scratcherInfoKenoSteppedMultiplier)
    {
        if (scratcherInfoKenoSteppedMultiplier == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "rowCount", scratcherInfoKenoSteppedMultiplier.rowCount);
        BlackboardUtils.SetOrCreateValue(bb, "prizeList", scratcherInfoKenoSteppedMultiplier.prizeList);
    }

    public static void Serialize(IBlackboard bb, ScratcherInfoLadder scratcherInfoLadder)
    {
        if (scratcherInfoLadder == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "startPositionList", scratcherInfoLadder.startPositionList);
        BlackboardUtils.SetOrCreateValue(bb, "prizePositionList", scratcherInfoLadder.prizePositionList);
        BlackboardUtils.SetOrCreateList(bb, "ladder_2dArray", scratcherInfoLadder.ladder_2dArray, Serialize);
    }

    public static void Serialize(IBlackboard bb, ScratcherInfoLadderBonus scratcherInfoLadderBonus)
    {
        if (scratcherInfoLadderBonus == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "startPositionList", scratcherInfoLadderBonus.startPositionList);
        BlackboardUtils.SetOrCreateValue(bb, "prizePositionList", scratcherInfoLadderBonus.prizePositionList);
        BlackboardUtils.SetOrCreateList(bb, "ladder_2dArray", scratcherInfoLadderBonus.ladder_2dArray, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "numInstantWin", scratcherInfoLadderBonus.numInstantWin);
        BlackboardUtils.SetOrCreateList(bb, "bonusWinInfo", scratcherInfoLadderBonus.bonusWinInfo, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "bonusLoseInfo", scratcherInfoLadderBonus.bonusLoseInfo, Serialize);
    }

    public static void Serialize(IBlackboard bb, ScratcherInfoLadderMultiplier scratcherInfoLadderMultiplier)
    {
        if (scratcherInfoLadderMultiplier == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "startPositionList", scratcherInfoLadderMultiplier.startPositionList);
        BlackboardUtils.SetOrCreateValue(bb, "prizePositionList", scratcherInfoLadderMultiplier.prizePositionList);
        BlackboardUtils.SetOrCreateList(bb, "ladder_2dArray", scratcherInfoLadderMultiplier.ladder_2dArray, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "winningNumbersMultiplier", scratcherInfoLadderMultiplier.winningNumbersMultiplier);
    }

    public static void Serialize(IBlackboard bb, ScratcherInfoMatchThreePrize scratcherInfoMatchThreePrize)
    {
        if (scratcherInfoMatchThreePrize == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "numScratchArea", scratcherInfoMatchThreePrize.numScratchArea);
    }

    public static void Serialize(IBlackboard bb, ScratcherInfoMatchThreeRow scratcherInfoMatchThreeRow)
    {
        if (scratcherInfoMatchThreeRow == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "numRow", scratcherInfoMatchThreeRow.numRow);
    }

    public static void Serialize(IBlackboard bb, ScratcherInfoPokerDefault scratcherInfoPokerDefault)
    {
        if (scratcherInfoPokerDefault == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "numScratchArea", scratcherInfoPokerDefault.numScratchArea);
    }

    public static void Serialize(IBlackboard bb, ScratcherInfoPokerMultiplier scratcherInfoPokerMultiplier)
    {
        if (scratcherInfoPokerMultiplier == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "numScratchArea", scratcherInfoPokerMultiplier.numScratchArea);
    }

    public static void Serialize(IBlackboard bb, ScratcherInfoWinningNumbersDefault scratcherInfoWinningNumbersDefault)
    {
        if (scratcherInfoWinningNumbersDefault == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "winningNumbers", scratcherInfoWinningNumbersDefault.winningNumbers);
        BlackboardUtils.SetOrCreateValue(bb, "myNumbers", scratcherInfoWinningNumbersDefault.myNumbers);
    }

    public static void Serialize(IBlackboard bb, ScratcherInfoWinningNumbersDynamicRows scratcherInfoWinningNumbersDynamicRows)
    {
        if (scratcherInfoWinningNumbersDynamicRows == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "winningNumbers", scratcherInfoWinningNumbersDynamicRows.winningNumbers);
        BlackboardUtils.SetOrCreateValue(bb, "myNumbers", scratcherInfoWinningNumbersDynamicRows.myNumbers);
        BlackboardUtils.SetOrCreateValue(bb, "rowNumList", scratcherInfoWinningNumbersDynamicRows.rowNumList);
    }

    public static void Serialize(IBlackboard bb, ScratcherInfoWinningNumbersGlobalMultiplier scratcherInfoWinningNumbersGlobalMultiplier)
    {
        if (scratcherInfoWinningNumbersGlobalMultiplier == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "winningNumbers", scratcherInfoWinningNumbersGlobalMultiplier.winningNumbers);
        BlackboardUtils.SetOrCreateValue(bb, "myNumbers", scratcherInfoWinningNumbersGlobalMultiplier.myNumbers);
    }

    public static void Serialize(IBlackboard bb, ScratcherInfoWinningNumbersMultiplier scratcherInfoWinningNumbersMultiplier)
    {
        if (scratcherInfoWinningNumbersMultiplier == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "winningNumbers", scratcherInfoWinningNumbersMultiplier.winningNumbers);
        BlackboardUtils.SetOrCreateValue(bb, "myNumbers", scratcherInfoWinningNumbersMultiplier.myNumbers);
        BlackboardUtils.SetOrCreateValue(bb, "winningNumbersMultiplierSymbolList", scratcherInfoWinningNumbersMultiplier.winningNumbersMultiplierSymbolList);
        BlackboardUtils.SetOrCreateValue(bb, "winningNumbersMultiplierList", scratcherInfoWinningNumbersMultiplier.winningNumbersMultiplierList);
    }

    public static void Serialize(IBlackboard bb, ScratcherPackInfo scratcherPackInfo)
    {
        if (scratcherPackInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "packId", scratcherPackInfo.packId);
        BlackboardUtils.SetOrCreateValue(bb, "collectingGameId", scratcherPackInfo.collectingGameId);
        BlackboardUtils.SetOrCreateValue(bb, "possessions", scratcherPackInfo.possessions);
        BlackboardUtils.SetOrCreateValue(bb, "packImageId", scratcherPackInfo.packImageId);
    }

    public static void Serialize(IBlackboard bb, ScratcherPieceInfo scratcherPieceInfo)
    {
        if (scratcherPieceInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "pieceId", scratcherPieceInfo.pieceId);
        BlackboardUtils.SetOrCreateValue(bb, "rarity", scratcherPieceInfo.rarity);
        BlackboardUtils.SetOrCreateValue(bb, "requirement", scratcherPieceInfo.requirement);
        BlackboardUtils.SetOrCreateValue(bb, "possessions", scratcherPieceInfo.possessions);
        BlackboardUtils.SetOrCreateValue(bb, "shareCapacity", scratcherPieceInfo.shareCapacity);
    }

    public static void Serialize(IBlackboard bb, ScratcherSymbol scratcherSymbol)
    {
        if (scratcherSymbol == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "symbolType", scratcherSymbol.symbolType);
        BlackboardUtils.SetOrCreateValue(bb, "symbolId", scratcherSymbol.symbolId);
        BlackboardUtils.SetOrCreateValue(bb, "prize", scratcherSymbol.prize);
        BlackboardUtils.SetOrCreateValue(bb, "isHighWin", scratcherSymbol.isHighWin);
        BlackboardUtils.SetOrCreateValue(bb, "isHit", scratcherSymbol.isHit);
        BlackboardUtils.SetOrCreateValue(bb, "coverId", scratcherSymbol.coverId);
    }

    public static void Serialize(IBlackboard bb, SeasonPassAdsClaimRequest seasonPassAdsClaimRequest)
    {
        if (seasonPassAdsClaimRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", seasonPassAdsClaimRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", seasonPassAdsClaimRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "eventId", seasonPassAdsClaimRequest.eventId);
    }

    public static void Serialize(IBlackboard bb, SeasonPassAdsClaimResponse seasonPassAdsClaimResponse)
    {
        if (seasonPassAdsClaimResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", seasonPassAdsClaimResponse.error);
        if (seasonPassAdsClaimResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), seasonPassAdsClaimResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", seasonPassAdsClaimResponse.serverTime);
        if (seasonPassAdsClaimResponse.updateInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "updateInfo"), seasonPassAdsClaimResponse.updateInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "updateInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "adsFreePoint", seasonPassAdsClaimResponse.adsFreePoint);
        BlackboardUtils.SetOrCreateValue(bb, "lastAdsClaimTimestamp", seasonPassAdsClaimResponse.lastAdsClaimTimestamp);
    }

    public static void Serialize(IBlackboard bb, SeasonPassAdsClaimResponseV2 seasonPassAdsClaimResponseV2)
    {
        if (seasonPassAdsClaimResponseV2 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", seasonPassAdsClaimResponseV2.error);
        if (seasonPassAdsClaimResponseV2.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), seasonPassAdsClaimResponseV2.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", seasonPassAdsClaimResponseV2.serverTime);
        if (seasonPassAdsClaimResponseV2.updateInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "updateInfo"), seasonPassAdsClaimResponseV2.updateInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "updateInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "adsFreePoint", seasonPassAdsClaimResponseV2.adsFreePoint);
        BlackboardUtils.SetOrCreateValue(bb, "lastAdsClaimTimestamp", seasonPassAdsClaimResponseV2.lastAdsClaimTimestamp);
    }

    public static void Serialize(IBlackboard bb, SeasonPassAdsViewRequest seasonPassAdsViewRequest)
    {
        if (seasonPassAdsViewRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", seasonPassAdsViewRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", seasonPassAdsViewRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "eventId", seasonPassAdsViewRequest.eventId);
    }

    public static void Serialize(IBlackboard bb, SeasonPassAdsViewResponse seasonPassAdsViewResponse)
    {
        if (seasonPassAdsViewResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", seasonPassAdsViewResponse.error);
        if (seasonPassAdsViewResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), seasonPassAdsViewResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", seasonPassAdsViewResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "lastAdsViewTimestamp", seasonPassAdsViewResponse.lastAdsViewTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "nextAdsResetTimestamp", seasonPassAdsViewResponse.nextAdsResetTimestamp);
    }

    public static void Serialize(IBlackboard bb, SeasonPassEligibleBetBigWinPointInfo seasonPassEligibleBetBigWinPointInfo)
    {
        if (seasonPassEligibleBetBigWinPointInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "winType", seasonPassEligibleBetBigWinPointInfo.winType);
        BlackboardUtils.SetOrCreateValue(bb, "point", seasonPassEligibleBetBigWinPointInfo.point);
    }

    public static void Serialize(IBlackboard bb, SeasonPassEligibleBetInfo seasonPassEligibleBetInfo)
    {
        if (seasonPassEligibleBetInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "totalBet", seasonPassEligibleBetInfo.totalBet);
        BlackboardUtils.SetOrCreateValue(bb, "point", seasonPassEligibleBetInfo.point);
        BlackboardUtils.SetOrCreateList(bb, "bigWinPointInfo", seasonPassEligibleBetInfo.bigWinPointInfo, Serialize);
    }

    public static void Serialize(IBlackboard bb, SeasonPassEnterInfo seasonPassEnterInfo)
    {
        if (seasonPassEnterInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "iconBigImageUrl", seasonPassEnterInfo.iconBigImageUrl);
        BlackboardUtils.SetOrCreateValue(bb, "pointIconImageUrl", seasonPassEnterInfo.pointIconImageUrl);
        BlackboardUtils.SetOrCreateValue(bb, "backgroundImageUrl", seasonPassEnterInfo.backgroundImageUrl);
        BlackboardUtils.SetOrCreateValue(bb, "level", seasonPassEnterInfo.level);
        BlackboardUtils.SetOrCreateValue(bb, "point", seasonPassEnterInfo.point);
        BlackboardUtils.SetOrCreateValue(bb, "requiredPoint", seasonPassEnterInfo.requiredPoint);
        BlackboardUtils.SetOrCreateValue(bb, "requiredPointMax", seasonPassEnterInfo.requiredPointMax);
        if (seasonPassEnterInfo.nextReward != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "nextReward"), seasonPassEnterInfo.nextReward);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "nextReward");
        }
        BlackboardUtils.SetOrCreateValue(bb, "unclaimedRewardCount", seasonPassEnterInfo.unclaimedRewardCount);
        BlackboardUtils.SetOrCreateList(bb, "eligibleBetInfoList", seasonPassEnterInfo.eligibleBetInfoList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "maxLevel", seasonPassEnterInfo.maxLevel);
    }

    public static void Serialize(IBlackboard bb, SeasonPassEnterInfoRequestV2 seasonPassEnterInfoRequestV2)
    {
        if (seasonPassEnterInfoRequestV2 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", seasonPassEnterInfoRequestV2.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", seasonPassEnterInfoRequestV2.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "gameId", seasonPassEnterInfoRequestV2.gameId);
    }

    public static void Serialize(IBlackboard bb, SeasonPassEnterInfoResponseV2 seasonPassEnterInfoResponseV2)
    {
        if (seasonPassEnterInfoResponseV2 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", seasonPassEnterInfoResponseV2.error);
        if (seasonPassEnterInfoResponseV2.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), seasonPassEnterInfoResponseV2.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", seasonPassEnterInfoResponseV2.serverTime);
        if (seasonPassEnterInfoResponseV2.seasonPassEnterInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "seasonPassEnterInfo"), seasonPassEnterInfoResponseV2.seasonPassEnterInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "seasonPassEnterInfo");
        }
    }

    public static void Serialize(IBlackboard bb, SeasonPassEnterInfoV2 seasonPassEnterInfoV2)
    {
        if (seasonPassEnterInfoV2 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "pointIconImageUrl", seasonPassEnterInfoV2.pointIconImageUrl);
        BlackboardUtils.SetOrCreateValue(bb, "backgroundImageUrl", seasonPassEnterInfoV2.backgroundImageUrl);
        BlackboardUtils.SetOrCreateValue(bb, "level", seasonPassEnterInfoV2.level);
        BlackboardUtils.SetOrCreateValue(bb, "point", seasonPassEnterInfoV2.point);
        BlackboardUtils.SetOrCreateValue(bb, "maxLevel", seasonPassEnterInfoV2.maxLevel);
        BlackboardUtils.SetOrCreateValue(bb, "requiredPoint", seasonPassEnterInfoV2.requiredPoint);
        BlackboardUtils.SetOrCreateValue(bb, "requiredPointMax", seasonPassEnterInfoV2.requiredPointMax);
        BlackboardUtils.SetOrCreateValue(bb, "unclaimedRewardCount", seasonPassEnterInfoV2.unclaimedRewardCount);
        BlackboardUtils.SetOrCreateList(bb, "gaugeLevelList", seasonPassEnterInfoV2.gaugeLevelList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "nextRewardList", seasonPassEnterInfoV2.nextRewardList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "tabIconImageUrl", seasonPassEnterInfoV2.tabIconImageUrl);
        BlackboardUtils.SetOrCreateValue(bb, "bigIconImageUrl", seasonPassEnterInfoV2.bigIconImageUrl);
    }

    public static void Serialize(IBlackboard bb, SeasonPassGaugeInfo seasonPassGaugeInfo)
    {
        if (seasonPassGaugeInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "totalBet", seasonPassGaugeInfo.totalBet);
        BlackboardUtils.SetOrCreateValue(bb, "gaugeLevel", seasonPassGaugeInfo.gaugeLevel);
    }

    public static void Serialize(IBlackboard bb, SeasonPassInfoRequest seasonPassInfoRequest)
    {
        if (seasonPassInfoRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", seasonPassInfoRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", seasonPassInfoRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "eventId", seasonPassInfoRequest.eventId);
    }

    public static void Serialize(IBlackboard bb, SeasonPassInfoResponse seasonPassInfoResponse)
    {
        if (seasonPassInfoResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", seasonPassInfoResponse.error);
        if (seasonPassInfoResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), seasonPassInfoResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", seasonPassInfoResponse.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "rewardInfoList", seasonPassInfoResponse.rewardInfoList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "iconBigImageUrl", seasonPassInfoResponse.iconBigImageUrl);
        BlackboardUtils.SetOrCreateValue(bb, "iconSmallImageUrl", seasonPassInfoResponse.iconSmallImageUrl);
        BlackboardUtils.SetOrCreateValue(bb, "pointIconImageUrl", seasonPassInfoResponse.pointIconImageUrl);
        BlackboardUtils.SetOrCreateValue(bb, "titleImageUrl", seasonPassInfoResponse.titleImageUrl);
        BlackboardUtils.SetOrCreateValue(bb, "backgroundImageUrl", seasonPassInfoResponse.backgroundImageUrl);
        BlackboardUtils.SetOrCreateValue(bb, "decorationLeftImageUrl", seasonPassInfoResponse.decorationLeftImageUrl);
        BlackboardUtils.SetOrCreateValue(bb, "decorationRightImageUrl", seasonPassInfoResponse.decorationRightImageUrl);
        BlackboardUtils.SetOrCreateValue(bb, "level", seasonPassInfoResponse.level);
        BlackboardUtils.SetOrCreateValue(bb, "point", seasonPassInfoResponse.point);
        BlackboardUtils.SetOrCreateValue(bb, "requiredPoint", seasonPassInfoResponse.requiredPoint);
        BlackboardUtils.SetOrCreateValue(bb, "requiredPointMax", seasonPassInfoResponse.requiredPointMax);
        BlackboardUtils.SetOrCreateValue(bb, "isPaid", seasonPassInfoResponse.isPaid);
        if (seasonPassInfoResponse.epicPassProduct != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "epicPassProduct"), seasonPassInfoResponse.epicPassProduct);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "epicPassProduct");
        }
        BlackboardUtils.SetOrCreateValue(bb, "adsFreePoint", seasonPassInfoResponse.adsFreePoint);
        BlackboardUtils.SetOrCreateValue(bb, "lastAdsViewTimestamp", seasonPassInfoResponse.lastAdsViewTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "lastAdsClaimTimestamp", seasonPassInfoResponse.lastAdsClaimTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "nextAdsResetTimestamp", seasonPassInfoResponse.nextAdsResetTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "maxLevel", seasonPassInfoResponse.maxLevel);
        BlackboardUtils.SetOrCreateValue(bb, "inboxImageUrl", seasonPassInfoResponse.inboxImageUrl);
        BlackboardUtils.SetOrCreateValue(bb, "resetGemPrice", seasonPassInfoResponse.resetGemPrice);
        BlackboardUtils.SetOrCreateList(bb, "recommendedGemProductList", seasonPassInfoResponse.recommendedGemProductList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "collectAllButtonDisplayed", seasonPassInfoResponse.collectAllButtonDisplayed);
        BlackboardUtils.SetOrCreateValue(bb, "collectAllEnabled", seasonPassInfoResponse.collectAllEnabled);
    }

    public static void Serialize(IBlackboard bb, SeasonPassInfoResponseV2 seasonPassInfoResponseV2)
    {
        if (seasonPassInfoResponseV2 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", seasonPassInfoResponseV2.error);
        if (seasonPassInfoResponseV2.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), seasonPassInfoResponseV2.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", seasonPassInfoResponseV2.serverTime);
        if (seasonPassInfoResponseV2.seasonPassInfoV2 != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "seasonPassInfoV2"), seasonPassInfoResponseV2.seasonPassInfoV2);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "seasonPassInfoV2");
        }
    }

    public static void Serialize(IBlackboard bb, SeasonPassInfoV2 seasonPassInfoV2)
    {
        if (seasonPassInfoV2 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "backgroundImageUrl", seasonPassInfoV2.backgroundImageUrl);
        BlackboardUtils.SetOrCreateValue(bb, "pointIconImageUrl", seasonPassInfoV2.pointIconImageUrl);
        BlackboardUtils.SetOrCreateValue(bb, "tabIconImageUrl", seasonPassInfoV2.tabIconImageUrl);
        BlackboardUtils.SetOrCreateValue(bb, "level", seasonPassInfoV2.level);
        BlackboardUtils.SetOrCreateValue(bb, "maxLevel", seasonPassInfoV2.maxLevel);
        BlackboardUtils.SetOrCreateValue(bb, "point", seasonPassInfoV2.point);
        BlackboardUtils.SetOrCreateValue(bb, "requiredPoint", seasonPassInfoV2.requiredPoint);
        BlackboardUtils.SetOrCreateValue(bb, "requiredPointMax", seasonPassInfoV2.requiredPointMax);
        BlackboardUtils.SetOrCreateValue(bb, "adsFreePoint", seasonPassInfoV2.adsFreePoint);
        BlackboardUtils.SetOrCreateValue(bb, "lastAdsViewTimestamp", seasonPassInfoV2.lastAdsViewTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "lastAdsClaimTimestamp", seasonPassInfoV2.lastAdsClaimTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "nextAdsResetTimestamp", seasonPassInfoV2.nextAdsResetTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "resetGemPrice", seasonPassInfoV2.resetGemPrice);
        BlackboardUtils.SetOrCreateList(bb, "recommendedGemProductList", seasonPassInfoV2.recommendedGemProductList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "collectAllButtonDisplayed", seasonPassInfoV2.collectAllButtonDisplayed);
        BlackboardUtils.SetOrCreateValue(bb, "collectAllEnabled", seasonPassInfoV2.collectAllEnabled);
        BlackboardUtils.SetOrCreateValue(bb, "isPaid", seasonPassInfoV2.isPaid);
        if (seasonPassInfoV2.epicPassProduct != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "epicPassProduct"), seasonPassInfoV2.epicPassProduct);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "epicPassProduct");
        }
        BlackboardUtils.SetOrCreateList(bb, "rewardInfoList", seasonPassInfoV2.rewardInfoList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "isEpicPassEnabled", seasonPassInfoV2.isEpicPassEnabled);
        BlackboardUtils.SetOrCreateValue(bb, "bigIconImageUrl", seasonPassInfoV2.bigIconImageUrl);
    }

    public static void Serialize(IBlackboard bb, SeasonPassPointUpdateInfo seasonPassPointUpdateInfo)
    {
        if (seasonPassPointUpdateInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "earnPoint", seasonPassPointUpdateInfo.earnPoint);
        BlackboardUtils.SetOrCreateValue(bb, "level", seasonPassPointUpdateInfo.level);
        BlackboardUtils.SetOrCreateValue(bb, "point", seasonPassPointUpdateInfo.point);
        BlackboardUtils.SetOrCreateValue(bb, "requiredPoint", seasonPassPointUpdateInfo.requiredPoint);
        BlackboardUtils.SetOrCreateValue(bb, "requiredPointMax", seasonPassPointUpdateInfo.requiredPointMax);
        if (seasonPassPointUpdateInfo.nextReward != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "nextReward"), seasonPassPointUpdateInfo.nextReward);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "nextReward");
        }
        BlackboardUtils.SetOrCreateValue(bb, "unclaimedRewardCount", seasonPassPointUpdateInfo.unclaimedRewardCount);
        BlackboardUtils.SetOrCreateValue(bb, "skippedRequiredPointMaxList", seasonPassPointUpdateInfo.skippedRequiredPointMaxList);
    }

    public static void Serialize(IBlackboard bb, SeasonPassPointUpdateInfoV2 seasonPassPointUpdateInfoV2)
    {
        if (seasonPassPointUpdateInfoV2 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "level", seasonPassPointUpdateInfoV2.level);
        BlackboardUtils.SetOrCreateValue(bb, "earnPoint", seasonPassPointUpdateInfoV2.earnPoint);
        BlackboardUtils.SetOrCreateValue(bb, "point", seasonPassPointUpdateInfoV2.point);
        BlackboardUtils.SetOrCreateValue(bb, "requiredPoint", seasonPassPointUpdateInfoV2.requiredPoint);
        BlackboardUtils.SetOrCreateValue(bb, "requiredPointMax", seasonPassPointUpdateInfoV2.requiredPointMax);
        BlackboardUtils.SetOrCreateValue(bb, "skippedRequiredPointMaxList", seasonPassPointUpdateInfoV2.skippedRequiredPointMaxList);
        BlackboardUtils.SetOrCreateValue(bb, "unclaimedRewardCount", seasonPassPointUpdateInfoV2.unclaimedRewardCount);
        BlackboardUtils.SetOrCreateList(bb, "nextRewardList", seasonPassPointUpdateInfoV2.nextRewardList, Serialize);
    }

    public static void Serialize(IBlackboard bb, SeasonPassResetRequest seasonPassResetRequest)
    {
        if (seasonPassResetRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", seasonPassResetRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", seasonPassResetRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "eventId", seasonPassResetRequest.eventId);
        BlackboardUtils.SetOrCreateValue(bb, "contextId", seasonPassResetRequest.contextId);
    }

    public static void Serialize(IBlackboard bb, SeasonPassResetResponse seasonPassResetResponse)
    {
        if (seasonPassResetResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", seasonPassResetResponse.error);
        if (seasonPassResetResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), seasonPassResetResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", seasonPassResetResponse.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "rewardResultList", seasonPassResetResponse.rewardResultList, Serialize);
        if (seasonPassResetResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), seasonPassResetResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
    }

    public static void Serialize(IBlackboard bb, SeasonPassResetResponseV2 seasonPassResetResponseV2)
    {
        if (seasonPassResetResponseV2 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", seasonPassResetResponseV2.error);
        if (seasonPassResetResponseV2.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), seasonPassResetResponseV2.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", seasonPassResetResponseV2.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "freeRewardResultList", seasonPassResetResponseV2.freeRewardResultList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "paidRewardResultList", seasonPassResetResponseV2.paidRewardResultList, Serialize);
        if (seasonPassResetResponseV2.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), seasonPassResetResponseV2.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
        BlackboardUtils.SetOrCreateList(bb, "nextRewardList", seasonPassResetResponseV2.nextRewardList, Serialize);
    }

    public static void Serialize(IBlackboard bb, SeasonPassRewardClaimRequest seasonPassRewardClaimRequest)
    {
        if (seasonPassRewardClaimRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", seasonPassRewardClaimRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", seasonPassRewardClaimRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "eventId", seasonPassRewardClaimRequest.eventId);
        BlackboardUtils.SetOrCreateValue(bb, "level", seasonPassRewardClaimRequest.level);
        BlackboardUtils.SetOrCreateValue(bb, "isPaidReward", seasonPassRewardClaimRequest.isPaidReward);
    }

    public static void Serialize(IBlackboard bb, SeasonPassRewardClaimResponse seasonPassRewardClaimResponse)
    {
        if (seasonPassRewardClaimResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", seasonPassRewardClaimResponse.error);
        if (seasonPassRewardClaimResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), seasonPassRewardClaimResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", seasonPassRewardClaimResponse.serverTime);
        if (seasonPassRewardClaimResponse.rewardResult != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "rewardResult"), seasonPassRewardClaimResponse.rewardResult);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "rewardResult");
        }
        BlackboardUtils.SetOrCreateList(bb, "rewardInfoList", seasonPassRewardClaimResponse.rewardInfoList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "unclaimedRewardCount", seasonPassRewardClaimResponse.unclaimedRewardCount);
    }

    public static void Serialize(IBlackboard bb, SeasonPassRewardClaimResponseV2 seasonPassRewardClaimResponseV2)
    {
        if (seasonPassRewardClaimResponseV2 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", seasonPassRewardClaimResponseV2.error);
        if (seasonPassRewardClaimResponseV2.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), seasonPassRewardClaimResponseV2.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", seasonPassRewardClaimResponseV2.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "rewardResultList", seasonPassRewardClaimResponseV2.rewardResultList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "rewardInfoList", seasonPassRewardClaimResponseV2.rewardInfoList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "unclaimedRewardCount", seasonPassRewardClaimResponseV2.unclaimedRewardCount);
        BlackboardUtils.SetOrCreateValue(bb, "isPaidReward", seasonPassRewardClaimResponseV2.isPaidReward);
        BlackboardUtils.SetOrCreateList(bb, "availableRewardOnPurchaseList", seasonPassRewardClaimResponseV2.availableRewardOnPurchaseList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "isPaid", seasonPassRewardClaimResponseV2.isPaid);
    }

    public static void Serialize(IBlackboard bb, SeasonPassRewardCollectAllRequest seasonPassRewardCollectAllRequest)
    {
        if (seasonPassRewardCollectAllRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", seasonPassRewardCollectAllRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", seasonPassRewardCollectAllRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "eventId", seasonPassRewardCollectAllRequest.eventId);
        BlackboardUtils.SetOrCreateValue(bb, "contextId", seasonPassRewardCollectAllRequest.contextId);
    }

    public static void Serialize(IBlackboard bb, SeasonPassRewardCollectAllResponse seasonPassRewardCollectAllResponse)
    {
        if (seasonPassRewardCollectAllResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", seasonPassRewardCollectAllResponse.error);
        if (seasonPassRewardCollectAllResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), seasonPassRewardCollectAllResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", seasonPassRewardCollectAllResponse.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "rewardResultList", seasonPassRewardCollectAllResponse.rewardResultList, Serialize);
    }

    public static void Serialize(IBlackboard bb, SeasonPassRewardCollectAllResponseV2 seasonPassRewardCollectAllResponseV2)
    {
        if (seasonPassRewardCollectAllResponseV2 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", seasonPassRewardCollectAllResponseV2.error);
        if (seasonPassRewardCollectAllResponseV2.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), seasonPassRewardCollectAllResponseV2.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", seasonPassRewardCollectAllResponseV2.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "freeRewardResultList", seasonPassRewardCollectAllResponseV2.freeRewardResultList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "paidRewardResultList", seasonPassRewardCollectAllResponseV2.paidRewardResultList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "availableRewardOnPurchaseList", seasonPassRewardCollectAllResponseV2.availableRewardOnPurchaseList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "isPaid", seasonPassRewardCollectAllResponseV2.isPaid);
    }

    public static void Serialize(IBlackboard bb, SeasonPassRewardInfo seasonPassRewardInfo)
    {
        if (seasonPassRewardInfo == null) { return; }
        if (seasonPassRewardInfo.freeReward != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "freeReward"), seasonPassRewardInfo.freeReward);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "freeReward");
        }
        if (seasonPassRewardInfo.paidReward != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "paidReward"), seasonPassRewardInfo.paidReward);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "paidReward");
        }
    }

    public static void Serialize(IBlackboard bb, SeasonPassRewardInfoV2 seasonPassRewardInfoV2)
    {
        if (seasonPassRewardInfoV2 == null) { return; }
        if (seasonPassRewardInfoV2.free != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "free"), seasonPassRewardInfoV2.free);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "free");
        }
        if (seasonPassRewardInfoV2.paid != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "paid"), seasonPassRewardInfoV2.paid);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "paid");
        }
    }

    public static void Serialize(IBlackboard bb, Shop shop)
    {
        if (shop == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "id", shop.id);
        BlackboardUtils.SetOrCreateValue(bb, "type", shop.type);
        BlackboardUtils.SetOrCreateValue(bb, "name", shop.name);
        BlackboardUtils.SetOrCreateValue(bb, "isDefault", shop.isDefault);
        BlackboardUtils.SetOrCreateValue(bb, "startTimestamp", shop.startTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "endTimestamp", shop.endTimestamp);
        BlackboardUtils.SetOrCreateList(bb, "productGroupList", shop.productGroupList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "imageUrl", shop.imageUrl);
    }

    public static void Serialize(IBlackboard bb, SimpleRequest simpleRequest)
    {
        if (simpleRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", simpleRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", simpleRequest.ackMask);
    }

    public static void Serialize(IBlackboard bb, SimpleResponse simpleResponse)
    {
        if (simpleResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", simpleResponse.error);
        if (simpleResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), simpleResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", simpleResponse.serverTime);
    }

    public static void Serialize(IBlackboard bb, SimulatorComponentInfo simulatorComponentInfo)
    {
        if (simulatorComponentInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "id", simulatorComponentInfo.id);
        BlackboardUtils.SetOrCreateValue(bb, "name", simulatorComponentInfo.name);
        BlackboardUtils.SetOrCreateValue(bb, "type", simulatorComponentInfo.type);
    }

    public static void Serialize(IBlackboard bb, Slot slot)
    {
        if (slot == null) { return; }
        if (slot.flags != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "flags"), slot.flags);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "flags");
        }
        BlackboardUtils.SetOrCreateValue(bb, "gameId", slot.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "comingSoonText", slot.comingSoonText);
    }

    public static void Serialize(IBlackboard bb, SlotBanner slotBanner)
    {
        if (slotBanner == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "slotBannerType", slotBanner.slotBannerType);
        BlackboardUtils.SetOrCreateList(bb, "noticeList", slotBanner.noticeList, Serialize);
    }

    public static void Serialize(IBlackboard bb, SlotBannerGroup slotBannerGroup)
    {
        if (slotBannerGroup == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "id", slotBannerGroup.id);
        BlackboardUtils.SetOrCreateValue(bb, "priority", slotBannerGroup.priority);
        BlackboardUtils.SetOrCreateValue(bb, "slotBannerGroupType", slotBannerGroup.slotBannerGroupType);
        BlackboardUtils.SetOrCreateList(bb, "slotBannerList", slotBannerGroup.slotBannerList, Serialize);
    }

    public static void Serialize(IBlackboard bb, SlotBannerNotice slotBannerNotice)
    {
        if (slotBannerNotice == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "id", slotBannerNotice.id);
        BlackboardUtils.SetOrCreateValue(bb, "priority", slotBannerNotice.priority);
        BlackboardUtils.SetOrCreateValue(bb, "comment", slotBannerNotice.comment);
        BlackboardUtils.SetOrCreateValue(bb, "imageUrl", slotBannerNotice.imageUrl);
        BlackboardUtils.SetOrCreateValue(bb, "startTimestamp", slotBannerNotice.startTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "endTimestamp", slotBannerNotice.endTimestamp);
        if (slotBannerNotice.action != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "action"), slotBannerNotice.action);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "action");
        }
        if (slotBannerNotice.constraints != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "constraints"), slotBannerNotice.constraints);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "constraints");
        }
        BlackboardUtils.SetOrCreateList(bb, "componentList", slotBannerNotice.componentList, Serialize);
    }

    public static void Serialize(IBlackboard bb, SlotBannerNoticeConstraints slotBannerNoticeConstraints)
    {
        if (slotBannerNoticeConstraints == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "showEndTimer", slotBannerNoticeConstraints.showEndTimer);
        BlackboardUtils.SetOrCreateValue(bb, "useUserTimer", slotBannerNoticeConstraints.useUserTimer);
        BlackboardUtils.SetOrCreateValue(bb, "userTimerMin", slotBannerNoticeConstraints.userTimerMin);
        BlackboardUtils.SetOrCreateValue(bb, "cooltimeSec", slotBannerNoticeConstraints.cooltimeSec);
    }

    public static void Serialize(IBlackboard bb, SlotClaimBonusRequestV3 slotClaimBonusRequestV3)
    {
        if (slotClaimBonusRequestV3 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", slotClaimBonusRequestV3.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", slotClaimBonusRequestV3.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "contents", slotClaimBonusRequestV3.contents);
        BlackboardUtils.SetOrCreateValue(bb, "gameId", slotClaimBonusRequestV3.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "isGameSpin", slotClaimBonusRequestV3.isGameSpin);
        BlackboardUtils.SetOrCreateValue(bb, "isBonusSpin", slotClaimBonusRequestV3.isBonusSpin);
        BlackboardUtils.SetOrCreateValue(bb, "metaGameEventId", slotClaimBonusRequestV3.metaGameEventId);
        BlackboardUtils.SetOrCreateValue(bb, "seasonPassEventId", slotClaimBonusRequestV3.seasonPassEventId);
    }

    public static void Serialize(IBlackboard bb, SlotClaimBonusResponseV3 slotClaimBonusResponseV3)
    {
        if (slotClaimBonusResponseV3 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", slotClaimBonusResponseV3.error);
        if (slotClaimBonusResponseV3.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), slotClaimBonusResponseV3.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", slotClaimBonusResponseV3.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "contents", slotClaimBonusResponseV3.contents);
        if (slotClaimBonusResponseV3.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), slotClaimBonusResponseV3.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
    }

    public static void Serialize(IBlackboard bb, SlotFlags slotFlags)
    {
        if (slotFlags == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "isLong", slotFlags.isLong);
        BlackboardUtils.SetOrCreateValue(bb, "tag", slotFlags.tag);
        BlackboardUtils.SetOrCreateValue(bb, "status", slotFlags.status);
        BlackboardUtils.SetOrCreateValue(bb, "isAnimated", slotFlags.isAnimated);
    }

    public static void Serialize(IBlackboard bb, SlotListBannerComponent slotListBannerComponent)
    {
        if (slotListBannerComponent == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "type", slotListBannerComponent.type);
        BlackboardUtils.SetOrCreateValue(bb, "posX", slotListBannerComponent.posX);
        BlackboardUtils.SetOrCreateValue(bb, "posY", slotListBannerComponent.posY);
        if (slotListBannerComponent.data != null)
        {
            switch (slotListBannerComponent.type)
            {
            case SlotListBannerComponentType.TEXT:
                Serialize(bb, (SlotListBannerComponentText)slotListBannerComponent.data);
                break;

            case SlotListBannerComponentType.LEVEL_MULTIPLIER_TEXT_ON:
                Serialize(bb, (SlotListBannerComponentText)slotListBannerComponent.data);
                break;

            case SlotListBannerComponentType.LEVEL_MULTIPLIER_TEXT_OFF:
                Serialize(bb, (SlotListBannerComponentText)slotListBannerComponent.data);
                break;

            case SlotListBannerComponentType.RED_RIBBON_TAG:
                Serialize(bb, (SlotListBannerComponentTag)slotListBannerComponent.data);
                break;
            }
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "data");
        }
        BlackboardUtils.SetOrCreateValue(bb, "rotation", slotListBannerComponent.rotation);
    }

    public static void Serialize(IBlackboard bb, SlotListBannerComponentTag slotListBannerComponentTag)
    {
        if (slotListBannerComponentTag == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "text", slotListBannerComponentTag.text);
        BlackboardUtils.SetOrCreateValue(bb, "tagSize", slotListBannerComponentTag.tagSize);
        if (slotListBannerComponentTag.product != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "product"), slotListBannerComponentTag.product);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "product");
        }
    }

    public static void Serialize(IBlackboard bb, SlotListBannerComponentText slotListBannerComponentText)
    {
        if (slotListBannerComponentText == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "text", slotListBannerComponentText.text);
        if (slotListBannerComponentText.product != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "product"), slotListBannerComponentText.product);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "product");
        }
        BlackboardUtils.SetOrCreateValue(bb, "showOutline", slotListBannerComponentText.showOutline);
    }

    public static void Serialize(IBlackboard bb, SlotSpinRequestV2 slotSpinRequestV2)
    {
        if (slotSpinRequestV2 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", slotSpinRequestV2.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", slotSpinRequestV2.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "contents", slotSpinRequestV2.contents);
        BlackboardUtils.SetOrCreateValue(bb, "gameId", slotSpinRequestV2.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "isGameSpin", slotSpinRequestV2.isGameSpin);
        BlackboardUtils.SetOrCreateValue(bb, "roomId", slotSpinRequestV2.roomId);
        BlackboardUtils.SetOrCreateValue(bb, "isBonusSpin", slotSpinRequestV2.isBonusSpin);
        BlackboardUtils.SetOrCreateValue(bb, "isAutoSpin", slotSpinRequestV2.isAutoSpin);
        BlackboardUtils.SetOrCreateValue(bb, "metaGameEventId", slotSpinRequestV2.metaGameEventId);
        BlackboardUtils.SetOrCreateValue(bb, "collectingGameChestDropRateMultiplyEventId", slotSpinRequestV2.collectingGameChestDropRateMultiplyEventId);
        BlackboardUtils.SetOrCreateValue(bb, "expEventIdList", slotSpinRequestV2.expEventIdList);
        BlackboardUtils.SetOrCreateValue(bb, "isHighRollerBet", slotSpinRequestV2.isHighRollerBet);
        BlackboardUtils.SetOrCreateValue(bb, "seasonPassEventId", slotSpinRequestV2.seasonPassEventId);
    }

    public static void Serialize(IBlackboard bb, SlotSpinResponseV3 slotSpinResponseV3)
    {
        if (slotSpinResponseV3 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", slotSpinResponseV3.error);
        if (slotSpinResponseV3.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), slotSpinResponseV3.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", slotSpinResponseV3.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "contents", slotSpinResponseV3.contents);
        if (slotSpinResponseV3.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), slotSpinResponseV3.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "nextMysteryGiftLevel", slotSpinResponseV3.nextMysteryGiftLevel);
        if (slotSpinResponseV3.tournamentInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "tournamentInfo"), slotSpinResponseV3.tournamentInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "tournamentInfo");
        }
        if (slotSpinResponseV3.mysteryGiftInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "mysteryGiftInfo"), slotSpinResponseV3.mysteryGiftInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "mysteryGiftInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "gameSpinCount", slotSpinResponseV3.gameSpinCount);
        BlackboardUtils.SetOrCreateValue(bb, "bonusSpinCount", slotSpinResponseV3.bonusSpinCount);
        BlackboardUtils.SetOrCreateValue(bb, "featureUnlockList", slotSpinResponseV3.featureUnlockList);
        if (slotSpinResponseV3.metaGameInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "metaGameInfo"), slotSpinResponseV3.metaGameInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "metaGameInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "earnFinder", slotSpinResponseV3.earnFinder);
        if (slotSpinResponseV3.vipLoungeCompositeInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "vipLoungeCompositeInfo"), slotSpinResponseV3.vipLoungeCompositeInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "vipLoungeCompositeInfo");
        }
        if (slotSpinResponseV3.levelUpDashInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "levelUpDashInfo"), slotSpinResponseV3.levelUpDashInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "levelUpDashInfo");
        }
    }

    public static void Serialize(IBlackboard bb, SocialReward socialReward)
    {
        if (socialReward == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "credit", socialReward.credit);
        BlackboardUtils.SetOrCreateValue(bb, "rp", socialReward.rp);
    }

    public static void Serialize(IBlackboard bb, Speaker speaker)
    {
        if (speaker == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "MIN_SALES_COUNT", speaker.MIN_SALES_COUNT);
        BlackboardUtils.SetOrCreateValue(bb, "MAX_SALES_COUNT", speaker.MAX_SALES_COUNT);
        BlackboardUtils.SetOrCreateValue(bb, "SALES_COUNT_INTERVAL", speaker.SALES_COUNT_INTERVAL);
        BlackboardUtils.SetOrCreateValue(bb, "GEM_VALUE_FOR_SALES_COUNT_INTERVAL", speaker.GEM_VALUE_FOR_SALES_COUNT_INTERVAL);
    }

    public static void Serialize(IBlackboard bb, SpinDealUseRequest spinDealUseRequest)
    {
        if (spinDealUseRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", spinDealUseRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", spinDealUseRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "spinDealId", spinDealUseRequest.spinDealId);
        BlackboardUtils.SetOrCreateValue(bb, "betSpinIndex", spinDealUseRequest.betSpinIndex);
        BlackboardUtils.SetOrCreateValue(bb, "contextId", spinDealUseRequest.contextId);
    }

    public static void Serialize(IBlackboard bb, SpinDealUseResponse spinDealUseResponse)
    {
        if (spinDealUseResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", spinDealUseResponse.error);
        if (spinDealUseResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), spinDealUseResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", spinDealUseResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "baseBet", spinDealUseResponse.baseBet);
        BlackboardUtils.SetOrCreateValue(bb, "spinCount", spinDealUseResponse.spinCount);
        BlackboardUtils.SetOrCreateValue(bb, "betIndex", spinDealUseResponse.betIndex);
        BlackboardUtils.SetOrCreateValue(bb, "spinCountIndex", spinDealUseResponse.spinCountIndex);
        if (spinDealUseResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), spinDealUseResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
    }

    public static void Serialize(IBlackboard bb, SpinDealUseV2Request spinDealUseV2Request)
    {
        if (spinDealUseV2Request == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", spinDealUseV2Request.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", spinDealUseV2Request.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "spinDealId", spinDealUseV2Request.spinDealId);
        BlackboardUtils.SetOrCreateValue(bb, "betSpinIndex", spinDealUseV2Request.betSpinIndex);
        BlackboardUtils.SetOrCreateValue(bb, "contextId", spinDealUseV2Request.contextId);
    }

    public static void Serialize(IBlackboard bb, SpinDealUseV2Response spinDealUseV2Response)
    {
        if (spinDealUseV2Response == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", spinDealUseV2Response.error);
        if (spinDealUseV2Response.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), spinDealUseV2Response.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", spinDealUseV2Response.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "baseBet", spinDealUseV2Response.baseBet);
        BlackboardUtils.SetOrCreateValue(bb, "spinCount", spinDealUseV2Response.spinCount);
        if (spinDealUseV2Response.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), spinDealUseV2Response.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
    }

    public static void Serialize(IBlackboard bb, SsoAccountInfo ssoAccountInfo)
    {
        if (ssoAccountInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "facebookId", ssoAccountInfo.facebookId);
        BlackboardUtils.SetOrCreateValue(bb, "email", ssoAccountInfo.email);
        BlackboardUtils.SetOrCreateValue(bb, "appleId", ssoAccountInfo.appleId);
    }

    public static void Serialize(IBlackboard bb, SsoAssociatedUser ssoAssociatedUser)
    {
        if (ssoAssociatedUser == null) { return; }
        if (ssoAssociatedUser.user != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "user"), ssoAssociatedUser.user);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "user");
        }
        BlackboardUtils.SetOrCreateValue(bb, "normalFriendCount", ssoAssociatedUser.normalFriendCount);
        BlackboardUtils.SetOrCreateValue(bb, "facebookFriendCount", ssoAssociatedUser.facebookFriendCount);
        if (ssoAssociatedUser.accountRemovalInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "accountRemovalInfo"), ssoAssociatedUser.accountRemovalInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "accountRemovalInfo");
        }
    }

    public static void Serialize(IBlackboard bb, SsoConnectAppleRequest ssoConnectAppleRequest)
    {
        if (ssoConnectAppleRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", ssoConnectAppleRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", ssoConnectAppleRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "email", ssoConnectAppleRequest.email);
        BlackboardUtils.SetOrCreateValue(bb, "firstName", ssoConnectAppleRequest.firstName);
        BlackboardUtils.SetOrCreateValue(bb, "lastName", ssoConnectAppleRequest.lastName);
        BlackboardUtils.SetOrCreateValue(bb, "idToken", ssoConnectAppleRequest.idToken);
        BlackboardUtils.SetOrCreateValue(bb, "contextId", ssoConnectAppleRequest.contextId);
        BlackboardUtils.SetOrCreateValue(bb, "isFirstLogin", ssoConnectAppleRequest.isFirstLogin);
    }

    public static void Serialize(IBlackboard bb, SsoConnectEmailRequestV1 ssoConnectEmailRequestV1)
    {
        if (ssoConnectEmailRequestV1 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", ssoConnectEmailRequestV1.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "email", ssoConnectEmailRequestV1.email);
        BlackboardUtils.SetOrCreateValue(bb, "validationCode", ssoConnectEmailRequestV1.validationCode);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", ssoConnectEmailRequestV1.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "contextId", ssoConnectEmailRequestV1.contextId);
        BlackboardUtils.SetOrCreateValue(bb, "isFirstLogin", ssoConnectEmailRequestV1.isFirstLogin);
    }

    public static void Serialize(IBlackboard bb, SsoConnectFacebookRequestV2 ssoConnectFacebookRequestV2)
    {
        if (ssoConnectFacebookRequestV2 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", ssoConnectFacebookRequestV2.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "facebookId", ssoConnectFacebookRequestV2.facebookId);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", ssoConnectFacebookRequestV2.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "accessToken", ssoConnectFacebookRequestV2.accessToken);
        BlackboardUtils.SetOrCreateValue(bb, "contextId", ssoConnectFacebookRequestV2.contextId);
        BlackboardUtils.SetOrCreateValue(bb, "isFirstLogin", ssoConnectFacebookRequestV2.isFirstLogin);
    }

    public static void Serialize(IBlackboard bb, SsoConnectResponse ssoConnectResponse)
    {
        if (ssoConnectResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", ssoConnectResponse.error);
        if (ssoConnectResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), ssoConnectResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", ssoConnectResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "successSignIn", ssoConnectResponse.successSignIn);
        if (ssoConnectResponse.associatedUser != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "associatedUser"), ssoConnectResponse.associatedUser);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "associatedUser");
        }
        if (ssoConnectResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), ssoConnectResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
        if (ssoConnectResponse.ssoAccountInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "ssoAccountInfo"), ssoConnectResponse.ssoAccountInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "ssoAccountInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "earnCredit", ssoConnectResponse.earnCredit);
        if (ssoConnectResponse.updatedUserInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "updatedUserInfo"), ssoConnectResponse.updatedUserInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "updatedUserInfo");
        }
    }

    public static void Serialize(IBlackboard bb, SsoConnectUpdatedUserInfo ssoConnectUpdatedUserInfo)
    {
        if (ssoConnectUpdatedUserInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "name", ssoConnectUpdatedUserInfo.name);
        BlackboardUtils.SetOrCreateValue(bb, "profileUrl", ssoConnectUpdatedUserInfo.profileUrl);
        BlackboardUtils.SetOrCreateValue(bb, "gender", ssoConnectUpdatedUserInfo.gender);
        BlackboardUtils.SetOrCreateValue(bb, "profileImageUploadCount", ssoConnectUpdatedUserInfo.profileImageUploadCount);
    }

    public static void Serialize(IBlackboard bb, SsoSwitchAccountAppleRequest ssoSwitchAccountAppleRequest)
    {
        if (ssoSwitchAccountAppleRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", ssoSwitchAccountAppleRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", ssoSwitchAccountAppleRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "idToken", ssoSwitchAccountAppleRequest.idToken);
    }

    public static void Serialize(IBlackboard bb, SsoSwitchAccountEmailRequest ssoSwitchAccountEmailRequest)
    {
        if (ssoSwitchAccountEmailRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", ssoSwitchAccountEmailRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "email", ssoSwitchAccountEmailRequest.email);
        BlackboardUtils.SetOrCreateValue(bb, "validationCode", ssoSwitchAccountEmailRequest.validationCode);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", ssoSwitchAccountEmailRequest.ackMask);
    }

    public static void Serialize(IBlackboard bb, SsoSwitchAccountFacebookRequest ssoSwitchAccountFacebookRequest)
    {
        if (ssoSwitchAccountFacebookRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", ssoSwitchAccountFacebookRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "facebookId", ssoSwitchAccountFacebookRequest.facebookId);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", ssoSwitchAccountFacebookRequest.ackMask);
    }

    public static void Serialize(IBlackboard bb, SsoUserInfo ssoUserInfo)
    {
        if (ssoUserInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "name", ssoUserInfo.name);
        BlackboardUtils.SetOrCreateValue(bb, "profileUrl", ssoUserInfo.profileUrl);
        BlackboardUtils.SetOrCreateValue(bb, "profileHighResolutionUrl", ssoUserInfo.profileHighResolutionUrl);
        BlackboardUtils.SetOrCreateValue(bb, "credit", ssoUserInfo.credit);
        BlackboardUtils.SetOrCreateValue(bb, "level", ssoUserInfo.level);
        BlackboardUtils.SetOrCreateValue(bb, "tier", ssoUserInfo.tier);
    }

    public static void Serialize(IBlackboard bb, SsoValidateEmailDebugResponse ssoValidateEmailDebugResponse)
    {
        if (ssoValidateEmailDebugResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", ssoValidateEmailDebugResponse.error);
        if (ssoValidateEmailDebugResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), ssoValidateEmailDebugResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", ssoValidateEmailDebugResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "validationCode", ssoValidateEmailDebugResponse.validationCode);
    }

    public static void Serialize(IBlackboard bb, SsoValidateEmailRequestV1 ssoValidateEmailRequestV1)
    {
        if (ssoValidateEmailRequestV1 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", ssoValidateEmailRequestV1.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", ssoValidateEmailRequestV1.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "email", ssoValidateEmailRequestV1.email);
        BlackboardUtils.SetOrCreateValue(bb, "errorIfExists", ssoValidateEmailRequestV1.errorIfExists);
    }

    public static void Serialize(IBlackboard bb, StatusMatchEnableRequest statusMatchEnableRequest)
    {
        if (statusMatchEnableRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", statusMatchEnableRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", statusMatchEnableRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "deeplinkId", statusMatchEnableRequest.deeplinkId);
    }

    public static void Serialize(IBlackboard bb, StatusMatchEnableResponse statusMatchEnableResponse)
    {
        if (statusMatchEnableResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", statusMatchEnableResponse.error);
        if (statusMatchEnableResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), statusMatchEnableResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", statusMatchEnableResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "success", statusMatchEnableResponse.success);
    }

    public static void Serialize(IBlackboard bb, StatusMatchRequestRequest statusMatchRequestRequest)
    {
        if (statusMatchRequestRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", statusMatchRequestRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", statusMatchRequestRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "email", statusMatchRequestRequest.email);
        BlackboardUtils.SetOrCreateValue(bb, "vipProgram", statusMatchRequestRequest.vipProgram);
        BlackboardUtils.SetOrCreateValue(bb, "vipProgramApp", statusMatchRequestRequest.vipProgramApp);
        BlackboardUtils.SetOrCreateValue(bb, "vipProgramStatus", statusMatchRequestRequest.vipProgramStatus);
        BlackboardUtils.SetOrCreateValue(bb, "otherVipProgramApp", statusMatchRequestRequest.otherVipProgramApp);
        BlackboardUtils.SetOrCreateValue(bb, "otherVipProgramStatus", statusMatchRequestRequest.otherVipProgramStatus);
        BlackboardUtils.SetOrCreateValue(bb, "vipProgramUserId", statusMatchRequestRequest.vipProgramUserId);
    }

    public static void Serialize(IBlackboard bb, SuperSimulatorInfoRequest superSimulatorInfoRequest)
    {
        if (superSimulatorInfoRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", superSimulatorInfoRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", superSimulatorInfoRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "componentId", superSimulatorInfoRequest.componentId);
    }

    public static void Serialize(IBlackboard bb, SuperSimulatorInfoResponse superSimulatorInfoResponse)
    {
        if (superSimulatorInfoResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", superSimulatorInfoResponse.error);
        if (superSimulatorInfoResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), superSimulatorInfoResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", superSimulatorInfoResponse.serverTime);
        if (superSimulatorInfoResponse.iamComponent != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "iamComponent"), superSimulatorInfoResponse.iamComponent);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "iamComponent");
        }
        if (superSimulatorInfoResponse.slbComponent != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "slbComponent"), superSimulatorInfoResponse.slbComponent);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "slbComponent");
        }
        BlackboardUtils.SetOrCreateValue(bb, "revision", superSimulatorInfoResponse.revision);
    }

    public static void Serialize(IBlackboard bb, SuperSimulatorListResponse superSimulatorListResponse)
    {
        if (superSimulatorListResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", superSimulatorListResponse.error);
        if (superSimulatorListResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), superSimulatorListResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", superSimulatorListResponse.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "componentList", superSimulatorListResponse.componentList, Serialize);
    }

    public static void Serialize(IBlackboard bb, SurveyRequest surveyRequest)
    {
        if (surveyRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", surveyRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", surveyRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "surveyHash", surveyRequest.surveyHash);
    }

    public static void Serialize(IBlackboard bb, SurveyResponse surveyResponse)
    {
        if (surveyResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", surveyResponse.error);
        if (surveyResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), surveyResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", surveyResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "rewardCredit", surveyResponse.rewardCredit);
        if (surveyResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), surveyResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
    }

    public static void Serialize(IBlackboard bb, SystemOptionsUpdateRequest systemOptionsUpdateRequest)
    {
        if (systemOptionsUpdateRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", systemOptionsUpdateRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", systemOptionsUpdateRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "pushNotification", systemOptionsUpdateRequest.pushNotification);
        BlackboardUtils.SetOrCreateValue(bb, "kudoJackpot", systemOptionsUpdateRequest.kudoJackpot);
        BlackboardUtils.SetOrCreateValue(bb, "kudoTournament", systemOptionsUpdateRequest.kudoTournament);
        BlackboardUtils.SetOrCreateValue(bb, "globalChat", systemOptionsUpdateRequest.globalChat);
        BlackboardUtils.SetOrCreateValue(bb, "kudoNewUserWelcome", systemOptionsUpdateRequest.kudoNewUserWelcome);
        BlackboardUtils.SetOrCreateValue(bb, "enablePipMode", systemOptionsUpdateRequest.enablePipMode);
    }

    public static void Serialize(IBlackboard bb, TermsOfUseRequest termsOfUseRequest)
    {
        if (termsOfUseRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", termsOfUseRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", termsOfUseRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "consent", termsOfUseRequest.consent);
        BlackboardUtils.SetOrCreateValue(bb, "version", termsOfUseRequest.version);
        BlackboardUtils.SetOrCreateValue(bb, "contextId", termsOfUseRequest.contextId);
        BlackboardUtils.SetOrCreateValue(bb, "termsOfUseType", termsOfUseRequest.termsOfUseType);
    }

    public static void Serialize(IBlackboard bb, TicketedBonusClaimRequest ticketedBonusClaimRequest)
    {
        if (ticketedBonusClaimRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", ticketedBonusClaimRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", ticketedBonusClaimRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "ticketId", ticketedBonusClaimRequest.ticketId);
    }

    public static void Serialize(IBlackboard bb, TicketedBonusClaimResponse ticketedBonusClaimResponse)
    {
        if (ticketedBonusClaimResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", ticketedBonusClaimResponse.error);
        if (ticketedBonusClaimResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), ticketedBonusClaimResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", ticketedBonusClaimResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "earnCredit", ticketedBonusClaimResponse.earnCredit);
        BlackboardUtils.SetOrCreateValue(bb, "tierMultiplier", ticketedBonusClaimResponse.tierMultiplier);
        BlackboardUtils.SetOrCreateValue(bb, "totalEarnCredit", ticketedBonusClaimResponse.totalEarnCredit);
        if (ticketedBonusClaimResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), ticketedBonusClaimResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "insBbbCredit", ticketedBonusClaimResponse.insBbbCredit);
        BlackboardUtils.SetOrCreateValue(bb, "applyTierMultiplier", ticketedBonusClaimResponse.applyTierMultiplier);
    }

    public static void Serialize(IBlackboard bb, TicketedBonusUseRequest ticketedBonusUseRequest)
    {
        if (ticketedBonusUseRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", ticketedBonusUseRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", ticketedBonusUseRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "ticketId", ticketedBonusUseRequest.ticketId);
        BlackboardUtils.SetOrCreateValue(bb, "contents", ticketedBonusUseRequest.contents);
    }

    public static void Serialize(IBlackboard bb, TicketedBonusUseResponseV1 ticketedBonusUseResponseV1)
    {
        if (ticketedBonusUseResponseV1 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", ticketedBonusUseResponseV1.error);
        if (ticketedBonusUseResponseV1.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), ticketedBonusUseResponseV1.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", ticketedBonusUseResponseV1.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "contents", ticketedBonusUseResponseV1.contents);
        if (ticketedBonusUseResponseV1.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), ticketedBonusUseResponseV1.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
    }

    public static void Serialize(IBlackboard bb, Tier tier)
    {
        if (tier == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "MAX_TIER", tier.MAX_TIER);
        BlackboardUtils.SetOrCreateValue(bb, "RP_TABLE", tier.RP_TABLE);
        BlackboardUtils.SetOrCreateValue(bb, "TITLE_TABLE", tier.TITLE_TABLE);
        BlackboardUtils.SetOrCreateValue(bb, "VIP_DAILY_BONUS_CREDIT", tier.VIP_DAILY_BONUS_CREDIT);
        BlackboardUtils.SetOrCreateValue(bb, "VIP_DEAL_MIN_TIER", tier.VIP_DEAL_MIN_TIER);
        BlackboardUtils.SetOrCreateValue(bb, "TIER_MULTIPLIER_NUMERATOR_TABLE", tier.TIER_MULTIPLIER_NUMERATOR_TABLE);
        BlackboardUtils.SetOrCreateValue(bb, "VIP_DEAL_V2_MIN_TIER", tier.VIP_DEAL_V2_MIN_TIER);
    }

    public static void Serialize(IBlackboard bb, TierBoost tierBoost)
    {
        if (tierBoost == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "tier", tierBoost.tier);
        BlackboardUtils.SetOrCreateValue(bb, "start", tierBoost.start);
        BlackboardUtils.SetOrCreateValue(bb, "end", tierBoost.end);
        BlackboardUtils.SetOrCreateValue(bb, "totalDays", tierBoost.totalDays);
    }

    public static void Serialize(IBlackboard bb, TierMatchOfferClaimRequest tierMatchOfferClaimRequest)
    {
        if (tierMatchOfferClaimRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", tierMatchOfferClaimRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", tierMatchOfferClaimRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "matchCode", tierMatchOfferClaimRequest.matchCode);
    }

    public static void Serialize(IBlackboard bb, TierMatchOfferClaimResponse tierMatchOfferClaimResponse)
    {
        if (tierMatchOfferClaimResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", tierMatchOfferClaimResponse.error);
        if (tierMatchOfferClaimResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), tierMatchOfferClaimResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", tierMatchOfferClaimResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "currentTier", tierMatchOfferClaimResponse.currentTier);
        BlackboardUtils.SetOrCreateValue(bb, "isTierIncreased", tierMatchOfferClaimResponse.isTierIncreased);
        BlackboardUtils.SetOrCreateValue(bb, "freebieAmount", tierMatchOfferClaimResponse.freebieAmount);
        if (tierMatchOfferClaimResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), tierMatchOfferClaimResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
        BlackboardUtils.SetOrCreateList(bb, "inboxList", tierMatchOfferClaimResponse.inboxList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "inboxBannerList", tierMatchOfferClaimResponse.inboxBannerList, Serialize);
    }

    public static void Serialize(IBlackboard bb, TimeBonus timeBonus)
    {
        if (timeBonus == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "BASE_CREDIT", timeBonus.BASE_CREDIT);
        BlackboardUtils.SetOrCreateValue(bb, "BONUS_LEVEL_MULTIPLIER", timeBonus.BONUS_LEVEL_MULTIPLIER);
        BlackboardUtils.SetOrCreateValue(bb, "SPECIAL_BONUS_LEVEL_FACTOR", timeBonus.SPECIAL_BONUS_LEVEL_FACTOR);
        BlackboardUtils.SetOrCreateValue(bb, "SPECIAL_BONUS_CREDIT", timeBonus.SPECIAL_BONUS_CREDIT);
        BlackboardUtils.SetOrCreateValue(bb, "COOLTIME", timeBonus.COOLTIME);
        BlackboardUtils.SetOrCreateValue(bb, "COOLTIME_EPSILON", timeBonus.COOLTIME_EPSILON);
    }

    public static void Serialize(IBlackboard bb, TimeBonusCollectRequest timeBonusCollectRequest)
    {
        if (timeBonusCollectRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", timeBonusCollectRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", timeBonusCollectRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "timeBonusEventId", timeBonusCollectRequest.timeBonusEventId);
    }

    public static void Serialize(IBlackboard bb, TimeBonusCollectResponse timeBonusCollectResponse)
    {
        if (timeBonusCollectResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", timeBonusCollectResponse.error);
        if (timeBonusCollectResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), timeBonusCollectResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", timeBonusCollectResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "earnCredit", timeBonusCollectResponse.earnCredit);
        BlackboardUtils.SetOrCreateValue(bb, "multiplier", timeBonusCollectResponse.multiplier);
        BlackboardUtils.SetOrCreateValue(bb, "lastCollectTimestamp", timeBonusCollectResponse.lastCollectTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "timeBonusCooltime", timeBonusCollectResponse.timeBonusCooltime);
        if (timeBonusCollectResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), timeBonusCollectResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "timebonusConsecutive", timeBonusCollectResponse.timebonusConsecutive);
        BlackboardUtils.SetOrCreateValue(bb, "earnLoungePoint", timeBonusCollectResponse.earnLoungePoint);
        if (timeBonusCollectResponse.vipLoungeInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "vipLoungeInfo"), timeBonusCollectResponse.vipLoungeInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "vipLoungeInfo");
        }
    }

    public static void Serialize(IBlackboard bb, TournamentInfo tournamentInfo)
    {
        if (tournamentInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "tournamentId", tournamentInfo.tournamentId);
        BlackboardUtils.SetOrCreateValue(bb, "status", tournamentInfo.status);
        BlackboardUtils.SetOrCreateValue(bb, "userRank", tournamentInfo.userRank);
        BlackboardUtils.SetOrCreateValue(bb, "endByTimestamp", tournamentInfo.endByTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "breakUntilTimestamp", tournamentInfo.breakUntilTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "serialWinCount", tournamentInfo.serialWinCount);
        BlackboardUtils.SetOrCreateValue(bb, "serialWinBonus", tournamentInfo.serialWinBonus);
        BlackboardUtils.SetOrCreateList(bb, "prizeList", tournamentInfo.prizeList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "serialWinBonusList", tournamentInfo.serialWinBonusList);
        BlackboardUtils.SetOrCreateValue(bb, "maxRankToAdvance", tournamentInfo.maxRankToAdvance);
        BlackboardUtils.SetOrCreateValue(bb, "startTimestamp", tournamentInfo.startTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "periodMin", tournamentInfo.periodMin);
        BlackboardUtils.SetOrCreateValue(bb, "participantsCount", tournamentInfo.participantsCount);
    }

    public static void Serialize(IBlackboard bb, TournamentInfoForMetaInfo tournamentInfoForMetaInfo)
    {
        if (tournamentInfoForMetaInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "tournamentId", tournamentInfoForMetaInfo.tournamentId);
        BlackboardUtils.SetOrCreateValue(bb, "status", tournamentInfoForMetaInfo.status);
        BlackboardUtils.SetOrCreateValue(bb, "endByTimestamp", tournamentInfoForMetaInfo.endByTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "breakUntilTimestamp", tournamentInfoForMetaInfo.breakUntilTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "serialWinCount", tournamentInfoForMetaInfo.serialWinCount);
        BlackboardUtils.SetOrCreateValue(bb, "serialWinBonus", tournamentInfoForMetaInfo.serialWinBonus);
        BlackboardUtils.SetOrCreateValue(bb, "baseTotalPrize", tournamentInfoForMetaInfo.baseTotalPrize);
        BlackboardUtils.SetOrCreateList(bb, "topRankList", tournamentInfoForMetaInfo.topRankList, Serialize);
        BlackboardUtils.SetOrCreateList(bb, "myRankList", tournamentInfoForMetaInfo.myRankList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "revision", tournamentInfoForMetaInfo.revision);
        BlackboardUtils.SetOrCreateValue(bb, "maxRankToAdvance", tournamentInfoForMetaInfo.maxRankToAdvance);
        BlackboardUtils.SetOrCreateValue(bb, "startTimestamp", tournamentInfoForMetaInfo.startTimestamp);
        if (tournamentInfoForMetaInfo.prevTournamentResult != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "prevTournamentResult"), tournamentInfoForMetaInfo.prevTournamentResult);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "prevTournamentResult");
        }
    }

    public static void Serialize(IBlackboard bb, TournamentInfoRequest tournamentInfoRequest)
    {
        if (tournamentInfoRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", tournamentInfoRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", tournamentInfoRequest.ackMask);
    }

    public static void Serialize(IBlackboard bb, TournamentInfoResponse tournamentInfoResponse)
    {
        if (tournamentInfoResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", tournamentInfoResponse.error);
        if (tournamentInfoResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), tournamentInfoResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", tournamentInfoResponse.serverTime);
        if (tournamentInfoResponse.info != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "info"), tournamentInfoResponse.info);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "info");
        }
    }

    public static void Serialize(IBlackboard bb, TournamentPrize tournamentPrize)
    {
        if (tournamentPrize == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "fromRank", tournamentPrize.fromRank);
        BlackboardUtils.SetOrCreateValue(bb, "toRank", tournamentPrize.toRank);
        BlackboardUtils.SetOrCreateValue(bb, "actualPrize", tournamentPrize.actualPrize);
    }

    public static void Serialize(IBlackboard bb, TournamentRankInfo tournamentRankInfo)
    {
        if (tournamentRankInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "score", tournamentRankInfo.score);
        BlackboardUtils.SetOrCreateValue(bb, "rank", tournamentRankInfo.rank);
        if (tournamentRankInfo.profile != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "profile"), tournamentRankInfo.profile);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "profile");
        }
    }

    public static void Serialize(IBlackboard bb, TournamentResultInfo tournamentResultInfo)
    {
        if (tournamentResultInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "rank", tournamentResultInfo.rank);
        BlackboardUtils.SetOrCreateValue(bb, "baseWinCredit", tournamentResultInfo.baseWinCredit);
        BlackboardUtils.SetOrCreateValue(bb, "tournamentSerialWinCount", tournamentResultInfo.tournamentSerialWinCount);
        BlackboardUtils.SetOrCreateValue(bb, "serialWinBonus", tournamentResultInfo.serialWinBonus);
        BlackboardUtils.SetOrCreateValue(bb, "actualWinCredit", tournamentResultInfo.actualWinCredit);
    }

    public static void Serialize(IBlackboard bb, TutorialInfo tutorialInfo)
    {
        if (tutorialInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "stage", tutorialInfo.stage);
        BlackboardUtils.SetOrCreateValue(bb, "count", tutorialInfo.count);
    }

    public static void Serialize(IBlackboard bb, TutorialTimerSec tutorialTimerSec)
    {
        if (tutorialTimerSec == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "SPIN_SEVEN_TIMES", tutorialTimerSec.SPIN_SEVEN_TIMES);
        BlackboardUtils.SetOrCreateValue(bb, "REACH_LEVEL_FOUR", tutorialTimerSec.REACH_LEVEL_FOUR);
        BlackboardUtils.SetOrCreateValue(bb, "TWO_MORE_LEVEL", tutorialTimerSec.TWO_MORE_LEVEL);
    }

    public static void Serialize(IBlackboard bb, TutorialUpdateRequest tutorialUpdateRequest)
    {
        if (tutorialUpdateRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", tutorialUpdateRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", tutorialUpdateRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "stage", tutorialUpdateRequest.stage);
        BlackboardUtils.SetOrCreateValue(bb, "count", tutorialUpdateRequest.count);
    }

    public static void Serialize(IBlackboard bb, TutorialUpdateResponse tutorialUpdateResponse)
    {
        if (tutorialUpdateResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", tutorialUpdateResponse.error);
        if (tutorialUpdateResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), tutorialUpdateResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", tutorialUpdateResponse.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "rewardResultList", tutorialUpdateResponse.rewardResultList, Serialize);
    }

    public static void Serialize(IBlackboard bb, UploadImageRequest uploadImageRequest)
    {
        if (uploadImageRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", uploadImageRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", uploadImageRequest.ackMask);
    }

    public static void Serialize(IBlackboard bb, UploadImageResponse uploadImageResponse)
    {
        if (uploadImageResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", uploadImageResponse.error);
        if (uploadImageResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), uploadImageResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", uploadImageResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "screenshotUrl", uploadImageResponse.screenshotUrl);
    }

    public static void Serialize(IBlackboard bb, UserBlockRequest userBlockRequest)
    {
        if (userBlockRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", userBlockRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", userBlockRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "targetUserId", userBlockRequest.targetUserId);
        BlackboardUtils.SetOrCreateValue(bb, "blockType", userBlockRequest.blockType);
    }

    public static void Serialize(IBlackboard bb, UserBucks userBucks)
    {
        if (userBucks == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "paidBucks", userBucks.paidBucks);
        BlackboardUtils.SetOrCreateValue(bb, "bonusBucks", userBucks.bonusBucks);
        BlackboardUtils.SetOrCreateValue(bb, "freeBucks", userBucks.freeBucks);
    }

    public static void Serialize(IBlackboard bb, UserBucksInfo userBucksInfo)
    {
        if (userBucksInfo == null) { return; }
        if (userBucksInfo.bucks != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "bucks"), userBucksInfo.bucks);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "bucks");
        }
        BlackboardUtils.SetOrCreateList(bb, "giftList", userBucksInfo.giftList, Serialize);
    }

    public static void Serialize(IBlackboard bb, UserClubStateInfo userClubStateInfo)
    {
        if (userClubStateInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "userClubState", userClubStateInfo.userClubState);
        BlackboardUtils.SetOrCreateValue(bb, "requestedClubId", userClubStateInfo.requestedClubId);
    }

    public static void Serialize(IBlackboard bb, UserEditMeRequestV1 userEditMeRequestV1)
    {
        if (userEditMeRequestV1 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", userEditMeRequestV1.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", userEditMeRequestV1.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "name", userEditMeRequestV1.name);
        BlackboardUtils.SetOrCreateValue(bb, "gender", userEditMeRequestV1.gender);
        BlackboardUtils.SetOrCreateValue(bb, "age", userEditMeRequestV1.age);
        BlackboardUtils.SetOrCreateValue(bb, "message", userEditMeRequestV1.message);
        BlackboardUtils.SetOrCreateValue(bb, "country", userEditMeRequestV1.country);
    }

    public static void Serialize(IBlackboard bb, UserEditMeResponse userEditMeResponse)
    {
        if (userEditMeResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", userEditMeResponse.error);
        if (userEditMeResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), userEditMeResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", userEditMeResponse.serverTime);
        if (userEditMeResponse.me != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "me"), userEditMeResponse.me);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "me");
        }
    }

    public static void Serialize(IBlackboard bb, UserInfo userInfo)
    {
        if (userInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "userId", userInfo.userId);
        BlackboardUtils.SetOrCreateValue(bb, "name", userInfo.name);
        BlackboardUtils.SetOrCreateValue(bb, "profileUrl", userInfo.profileUrl);
        BlackboardUtils.SetOrCreateValue(bb, "credit", userInfo.credit);
        BlackboardUtils.SetOrCreateValue(bb, "level", userInfo.level);
        BlackboardUtils.SetOrCreateValue(bb, "tier", userInfo.tier);
        BlackboardUtils.SetOrCreateValue(bb, "facebookId", userInfo.facebookId);
        BlackboardUtils.SetOrCreateValue(bb, "reportCount", userInfo.reportCount);
        BlackboardUtils.SetOrCreateValue(bb, "lastLoginTimestamp", userInfo.lastLoginTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "rp", userInfo.rp);
        BlackboardUtils.SetOrCreateValue(bb, "accRp", userInfo.accRp);
        BlackboardUtils.SetOrCreateValue(bb, "age", userInfo.age);
        BlackboardUtils.SetOrCreateValue(bb, "location", userInfo.location);
        BlackboardUtils.SetOrCreateValue(bb, "message", userInfo.message);
        BlackboardUtils.SetOrCreateValue(bb, "like", userInfo.like);
        BlackboardUtils.SetOrCreateValue(bb, "playingRoomId", userInfo.playingRoomId);
        BlackboardUtils.SetOrCreateValue(bb, "gender", userInfo.gender);
        BlackboardUtils.SetOrCreateValue(bb, "lastSendGiftTimestamp", userInfo.lastSendGiftTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "profileHighResolutionUrl", userInfo.profileHighResolutionUrl);
        BlackboardUtils.SetOrCreateValue(bb, "lastOnlineTimestamp", userInfo.lastOnlineTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "timezoneOffset", userInfo.timezoneOffset);
        BlackboardUtils.SetOrCreateValue(bb, "maxTournamentSerialWinCount", userInfo.maxTournamentSerialWinCount);
        BlackboardUtils.SetOrCreateValue(bb, "tournamentChampionCount", userInfo.tournamentChampionCount);
        BlackboardUtils.SetOrCreateValue(bb, "clubId", userInfo.clubId);
        BlackboardUtils.SetOrCreateValue(bb, "gem", userInfo.gem);
        BlackboardUtils.SetOrCreateValue(bb, "countrySelected", userInfo.countrySelected);
    }

    public static void Serialize(IBlackboard bb, UserInfoMe userInfoMe)
    {
        if (userInfoMe == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "userId", userInfoMe.userId);
        BlackboardUtils.SetOrCreateValue(bb, "name", userInfoMe.name);
        BlackboardUtils.SetOrCreateValue(bb, "profileUrl", userInfoMe.profileUrl);
        BlackboardUtils.SetOrCreateValue(bb, "credit", userInfoMe.credit);
        BlackboardUtils.SetOrCreateValue(bb, "level", userInfoMe.level);
        BlackboardUtils.SetOrCreateValue(bb, "tier", userInfoMe.tier);
        BlackboardUtils.SetOrCreateValue(bb, "origTier", userInfoMe.origTier);
        BlackboardUtils.SetOrCreateValue(bb, "facebookId", userInfoMe.facebookId);
        BlackboardUtils.SetOrCreateValue(bb, "reportCount", userInfoMe.reportCount);
        BlackboardUtils.SetOrCreateValue(bb, "requiredExp", userInfoMe.requiredExp);
        BlackboardUtils.SetOrCreateValue(bb, "requiredExpMax", userInfoMe.requiredExpMax);
        BlackboardUtils.SetOrCreateValue(bb, "rp", userInfoMe.rp);
        BlackboardUtils.SetOrCreateValue(bb, "accRp", userInfoMe.accRp);
        BlackboardUtils.SetOrCreateValue(bb, "gender", userInfoMe.gender);
        BlackboardUtils.SetOrCreateValue(bb, "age", userInfoMe.age);
        BlackboardUtils.SetOrCreateValue(bb, "location", userInfoMe.location);
        BlackboardUtils.SetOrCreateValue(bb, "message", userInfoMe.message);
        BlackboardUtils.SetOrCreateValue(bb, "like", userInfoMe.like);
        BlackboardUtils.SetOrCreateValue(bb, "friendCode", userInfoMe.friendCode);
        BlackboardUtils.SetOrCreateValue(bb, "deviceId", userInfoMe.deviceId);
        BlackboardUtils.SetOrCreateValue(bb, "disapproveCount", userInfoMe.disapproveCount);
        BlackboardUtils.SetOrCreateValue(bb, "isBanned", userInfoMe.isBanned);
        BlackboardUtils.SetOrCreateValue(bb, "dailybonusTimestamp", userInfoMe.dailybonusTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "dailybonusConsecutive", userInfoMe.dailybonusConsecutive);
        BlackboardUtils.SetOrCreateValue(bb, "registerTimestamp", userInfoMe.registerTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "loginCount", userInfoMe.loginCount);
        BlackboardUtils.SetOrCreateValue(bb, "timebonusConsecutive", userInfoMe.timebonusConsecutive);
        BlackboardUtils.SetOrCreateValue(bb, "piggyCredit", userInfoMe.piggyCredit);
        BlackboardUtils.SetOrCreateValue(bb, "piggyLevel", userInfoMe.piggyLevel);
        BlackboardUtils.SetOrCreateValue(bb, "stampDeckId", userInfoMe.stampDeckId);
        BlackboardUtils.SetOrCreateValue(bb, "profileHighResolutionUrl", userInfoMe.profileHighResolutionUrl);
        BlackboardUtils.SetOrCreateValue(bb, "vipDailybonusTimestamp", userInfoMe.vipDailybonusTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "totalBetCredit", userInfoMe.totalBetCredit);
        BlackboardUtils.SetOrCreateValue(bb, "totalWinCredit", userInfoMe.totalWinCredit);
        BlackboardUtils.SetOrCreateValue(bb, "metaEarnCredit", userInfoMe.metaEarnCredit);
        BlackboardUtils.SetOrCreateValue(bb, "purchaseEarnCredit", userInfoMe.purchaseEarnCredit);
        BlackboardUtils.SetOrCreateValue(bb, "totalSpinCount", userInfoMe.totalSpinCount);
        BlackboardUtils.SetOrCreateValue(bb, "firstPurchaseId", userInfoMe.firstPurchaseId);
        BlackboardUtils.SetOrCreateValue(bb, "lastPurchaseId", userInfoMe.lastPurchaseId);
        BlackboardUtils.SetOrCreateValue(bb, "purchaseCount", userInfoMe.purchaseCount);
        BlackboardUtils.SetOrCreateValue(bb, "lifetimeSpend", userInfoMe.lifetimeSpend);
        BlackboardUtils.SetOrCreateValue(bb, "firstPlayedGameId", userInfoMe.firstPlayedGameId);
        BlackboardUtils.SetOrCreateValue(bb, "lastRatedClientNumberVersion", userInfoMe.lastRatedClientNumberVersion);
        BlackboardUtils.SetOrCreateValue(bb, "lastPurchaseTimestamp", userInfoMe.lastPurchaseTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "sessionTotalLength", userInfoMe.sessionTotalLength);
        BlackboardUtils.SetOrCreateValue(bb, "profileImageUploadCount", userInfoMe.profileImageUploadCount);
        BlackboardUtils.SetOrCreateValue(bb, "maxTournamentSerialWinCount", userInfoMe.maxTournamentSerialWinCount);
        BlackboardUtils.SetOrCreateValue(bb, "lastVideoAdsClaimTimestamp", userInfoMe.lastVideoAdsClaimTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "firstClubJoined", userInfoMe.firstClubJoined);
        BlackboardUtils.SetOrCreateValue(bb, "clubId", userInfoMe.clubId);
        BlackboardUtils.SetOrCreateValue(bb, "gem", userInfoMe.gem);
        BlackboardUtils.SetOrCreateValue(bb, "speaker", userInfoMe.speaker);
        BlackboardUtils.SetOrCreateValue(bb, "lastSpeakerRefillTimestamp", userInfoMe.lastSpeakerRefillTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "countrySelected", userInfoMe.countrySelected);
    }

    public static void Serialize(IBlackboard bb, UserInfoProfileResponse userInfoProfileResponse)
    {
        if (userInfoProfileResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", userInfoProfileResponse.error);
        if (userInfoProfileResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), userInfoProfileResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", userInfoProfileResponse.serverTime);
        if (userInfoProfileResponse.user != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "user"), userInfoProfileResponse.user);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "user");
        }
        BlackboardUtils.SetOrCreateValue(bb, "normalFriendCount", userInfoProfileResponse.normalFriendCount);
        BlackboardUtils.SetOrCreateValue(bb, "facebookFriendCount", userInfoProfileResponse.facebookFriendCount);
        BlackboardUtils.SetOrCreateList(bb, "recordList", userInfoProfileResponse.recordList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "userState", userInfoProfileResponse.userState);
        if (userInfoProfileResponse.clubInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "clubInfo"), userInfoProfileResponse.clubInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "clubInfo");
        }
        if (userInfoProfileResponse.clubTierInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "clubTierInfo"), userInfoProfileResponse.clubTierInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "clubTierInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "isBlocked", userInfoProfileResponse.isBlocked);
    }

    public static void Serialize(IBlackboard bb, UserInfoRoom userInfoRoom)
    {
        if (userInfoRoom == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "userId", userInfoRoom.userId);
        BlackboardUtils.SetOrCreateValue(bb, "name", userInfoRoom.name);
        BlackboardUtils.SetOrCreateValue(bb, "profileUrl", userInfoRoom.profileUrl);
        BlackboardUtils.SetOrCreateValue(bb, "credit", userInfoRoom.credit);
        BlackboardUtils.SetOrCreateValue(bb, "tier", userInfoRoom.tier);
        BlackboardUtils.SetOrCreateValue(bb, "reportCount", userInfoRoom.reportCount);
        BlackboardUtils.SetOrCreateValue(bb, "clubId", userInfoRoom.clubId);
        BlackboardUtils.SetOrCreateValue(bb, "countrySelected", userInfoRoom.countrySelected);
        BlackboardUtils.SetOrCreateValue(bb, "clubLeagueTier", userInfoRoom.clubLeagueTier);
    }

    public static void Serialize(IBlackboard bb, UserInfoSimple userInfoSimple)
    {
        if (userInfoSimple == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "userId", userInfoSimple.userId);
        BlackboardUtils.SetOrCreateValue(bb, "name", userInfoSimple.name);
        BlackboardUtils.SetOrCreateValue(bb, "profileUrl", userInfoSimple.profileUrl);
        BlackboardUtils.SetOrCreateValue(bb, "credit", userInfoSimple.credit);
        BlackboardUtils.SetOrCreateValue(bb, "tier", userInfoSimple.tier);
        BlackboardUtils.SetOrCreateValue(bb, "reportCount", userInfoSimple.reportCount);
    }

    public static void Serialize(IBlackboard bb, UserInfoSimpleRequest userInfoSimpleRequest)
    {
        if (userInfoSimpleRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", userInfoSimpleRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "targetUserId", userInfoSimpleRequest.targetUserId);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", userInfoSimpleRequest.ackMask);
    }

    public static void Serialize(IBlackboard bb, UserInvitableListResponse userInvitableListResponse)
    {
        if (userInvitableListResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", userInvitableListResponse.error);
        if (userInvitableListResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), userInvitableListResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", userInvitableListResponse.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "friendList", userInvitableListResponse.friendList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "friendOnlineList", userInvitableListResponse.friendOnlineList);
        BlackboardUtils.SetOrCreateList(bb, "clubMemberList", userInvitableListResponse.clubMemberList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "clubOnlineList", userInvitableListResponse.clubOnlineList);
    }

    public static void Serialize(IBlackboard bb, UserInviteRequest userInviteRequest)
    {
        if (userInviteRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", userInviteRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "targetUserIdList", userInviteRequest.targetUserIdList);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", userInviteRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "inviteType", userInviteRequest.inviteType);
    }

    public static void Serialize(IBlackboard bb, UserKudoRequest userKudoRequest)
    {
        if (userKudoRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", userKudoRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "targetUserId", userKudoRequest.targetUserId);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", userKudoRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "kudoId", userKudoRequest.kudoId);
        BlackboardUtils.SetOrCreateValue(bb, "kudoLikeType", userKudoRequest.kudoLikeType);
        BlackboardUtils.SetOrCreateValue(bb, "kudoType", userKudoRequest.kudoType);
    }

    public static void Serialize(IBlackboard bb, UserKudoResponse userKudoResponse)
    {
        if (userKudoResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", userKudoResponse.error);
        if (userKudoResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), userKudoResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", userKudoResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "earnRp", userKudoResponse.earnRp);
        if (userKudoResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), userKudoResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
    }

    public static void Serialize(IBlackboard bb, UserOptions userOptions)
    {
        if (userOptions == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "pushNotification", userOptions.pushNotification);
        BlackboardUtils.SetOrCreateValue(bb, "kudoJackpot", userOptions.kudoJackpot);
        BlackboardUtils.SetOrCreateValue(bb, "kudoTournament", userOptions.kudoTournament);
        BlackboardUtils.SetOrCreateValue(bb, "globalChat", userOptions.globalChat);
        BlackboardUtils.SetOrCreateValue(bb, "kudoNewUserWelcome", userOptions.kudoNewUserWelcome);
        BlackboardUtils.SetOrCreateValue(bb, "enablePipMode", userOptions.enablePipMode);
    }

    public static void Serialize(IBlackboard bb, UserProfile userProfile)
    {
        if (userProfile == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "userId", userProfile.userId);
        BlackboardUtils.SetOrCreateValue(bb, "name", userProfile.name);
        BlackboardUtils.SetOrCreateValue(bb, "profileUrl", userProfile.profileUrl);
        BlackboardUtils.SetOrCreateValue(bb, "tier", userProfile.tier);
        BlackboardUtils.SetOrCreateValue(bb, "reportCount", userProfile.reportCount);
        BlackboardUtils.SetOrCreateValue(bb, "profileHighResolutionUrl", userProfile.profileHighResolutionUrl);
        BlackboardUtils.SetOrCreateValue(bb, "clubId", userProfile.clubId);
        BlackboardUtils.SetOrCreateValue(bb, "lastOnlineTimestamp", userProfile.lastOnlineTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "level", userProfile.level);
        BlackboardUtils.SetOrCreateValue(bb, "countrySelected", userProfile.countrySelected);
    }

    public static void Serialize(IBlackboard bb, UserProfileListRequest userProfileListRequest)
    {
        if (userProfileListRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", userProfileListRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", userProfileListRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "userIdList", userProfileListRequest.userIdList);
    }

    public static void Serialize(IBlackboard bb, UserProfileListResponse userProfileListResponse)
    {
        if (userProfileListResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", userProfileListResponse.error);
        if (userProfileListResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), userProfileListResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", userProfileListResponse.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "userProfileList", userProfileListResponse.userProfileList, Serialize);
    }

    public static void Serialize(IBlackboard bb, UserProfileUploadRequest userProfileUploadRequest)
    {
        if (userProfileUploadRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", userProfileUploadRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", userProfileUploadRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "isRewardNeeded", userProfileUploadRequest.isRewardNeeded);
    }

    public static void Serialize(IBlackboard bb, UserProfileUploadResponse userProfileUploadResponse)
    {
        if (userProfileUploadResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", userProfileUploadResponse.error);
        if (userProfileUploadResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), userProfileUploadResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", userProfileUploadResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "profileUrl", userProfileUploadResponse.profileUrl);
        BlackboardUtils.SetOrCreateValue(bb, "profileHighResolutionUrl", userProfileUploadResponse.profileHighResolutionUrl);
        BlackboardUtils.SetOrCreateValue(bb, "profileImageUploadCount", userProfileUploadResponse.profileImageUploadCount);
        BlackboardUtils.SetOrCreateValue(bb, "rewardCredit", userProfileUploadResponse.rewardCredit);
    }

    public static void Serialize(IBlackboard bb, UserSyncInfo userSyncInfo)
    {
        if (userSyncInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "credit", userSyncInfo.credit);
        BlackboardUtils.SetOrCreateValue(bb, "requiredExp", userSyncInfo.requiredExp);
        BlackboardUtils.SetOrCreateValue(bb, "requiredExpMax", userSyncInfo.requiredExpMax);
        BlackboardUtils.SetOrCreateValue(bb, "level", userSyncInfo.level);
        BlackboardUtils.SetOrCreateValue(bb, "rp", userSyncInfo.rp);
        BlackboardUtils.SetOrCreateValue(bb, "accRp", userSyncInfo.accRp);
        BlackboardUtils.SetOrCreateValue(bb, "piggyCredit", userSyncInfo.piggyCredit);
        BlackboardUtils.SetOrCreateValue(bb, "totalSpinCount", userSyncInfo.totalSpinCount);
        BlackboardUtils.SetOrCreateValue(bb, "gem", userSyncInfo.gem);
        BlackboardUtils.SetOrCreateValue(bb, "speaker", userSyncInfo.speaker);
    }

    public static void Serialize(IBlackboard bb, UserUnblockRequest userUnblockRequest)
    {
        if (userUnblockRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", userUnblockRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", userUnblockRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "targetUserId", userUnblockRequest.targetUserId);
    }

    public static void Serialize(IBlackboard bb, Values values)
    {
        if (values == null) { return; }
        if (values.misc != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "misc"), values.misc);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "misc");
        }
        if (values.tier != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "tier"), values.tier);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "tier");
        }
        if (values.level != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "level"), values.level);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "level");
        }
        if (values.timeBonus != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "timeBonus"), values.timeBonus);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "timeBonus");
        }
        if (values.gameSpin != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "gameSpin"), values.gameSpin);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "gameSpin");
        }
        if (values.reward != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "reward"), values.reward);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "reward");
        }
        if (values.dailyBonus != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "dailyBonus"), values.dailyBonus);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "dailyBonus");
        }
        if (values.assetBundleInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "assetBundleInfo"), values.assetBundleInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "assetBundleInfo");
        }
        BlackboardUtils.SetOrCreateList(bb, "earlyAccess", values.earlyAccess, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "featureUnlockLevel", values.featureUnlockLevel);
        if (values.club != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "club"), values.club);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "club");
        }
        if (values.dailyMegaWheel != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "dailyMegaWheel"), values.dailyMegaWheel);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "dailyMegaWheel");
        }
        if (values.dailySpin != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "dailySpin"), values.dailySpin);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "dailySpin");
        }
    }

    public static void Serialize(IBlackboard bb, VideoAdsClaimResponse videoAdsClaimResponse)
    {
        if (videoAdsClaimResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", videoAdsClaimResponse.error);
        if (videoAdsClaimResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), videoAdsClaimResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", videoAdsClaimResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "lastCollectTimestamp", videoAdsClaimResponse.lastCollectTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "timebonusConsecutive", videoAdsClaimResponse.timebonusConsecutive);
        BlackboardUtils.SetOrCreateValue(bb, "timeBonusCooltime", videoAdsClaimResponse.timeBonusCooltime);
        BlackboardUtils.SetOrCreateValue(bb, "lastVideoAdsClaimTimestamp", videoAdsClaimResponse.lastVideoAdsClaimTimestamp);
    }

    public static void Serialize(IBlackboard bb, VideoAdsPlacementNames videoAdsPlacementNames)
    {
        if (videoAdsPlacementNames == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "timebonus", videoAdsPlacementNames.timebonus);
        BlackboardUtils.SetOrCreateValue(bb, "challenge", videoAdsPlacementNames.challenge);
        BlackboardUtils.SetOrCreateValue(bb, "inAppMessage", videoAdsPlacementNames.inAppMessage);
        BlackboardUtils.SetOrCreateValue(bb, "collectingGame", videoAdsPlacementNames.collectingGame);
        BlackboardUtils.SetOrCreateValue(bb, "dailyBonus", videoAdsPlacementNames.dailyBonus);
        BlackboardUtils.SetOrCreateValue(bb, "shopBonus", videoAdsPlacementNames.shopBonus);
        BlackboardUtils.SetOrCreateValue(bb, "seasonPass", videoAdsPlacementNames.seasonPass);
        BlackboardUtils.SetOrCreateValue(bb, "gemJackpot", videoAdsPlacementNames.gemJackpot);
        BlackboardUtils.SetOrCreateValue(bb, "bossRaiders", videoAdsPlacementNames.bossRaiders);
        BlackboardUtils.SetOrCreateValue(bb, "clubArena", videoAdsPlacementNames.clubArena);
        BlackboardUtils.SetOrCreateValue(bb, "hiddenUniverse", videoAdsPlacementNames.hiddenUniverse);
        BlackboardUtils.SetOrCreateValue(bb, "inbox", videoAdsPlacementNames.inbox);
        BlackboardUtils.SetOrCreateValue(bb, "buildDream", videoAdsPlacementNames.buildDream);
    }

    public static void Serialize(IBlackboard bb, VideoPokerClaimBonusRequestV2 videoPokerClaimBonusRequestV2)
    {
        if (videoPokerClaimBonusRequestV2 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", videoPokerClaimBonusRequestV2.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", videoPokerClaimBonusRequestV2.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "contents", videoPokerClaimBonusRequestV2.contents);
        BlackboardUtils.SetOrCreateValue(bb, "gameId", videoPokerClaimBonusRequestV2.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "isBonusDeal", videoPokerClaimBonusRequestV2.isBonusDeal);
        BlackboardUtils.SetOrCreateValue(bb, "isGameDeal", videoPokerClaimBonusRequestV2.isGameDeal);
        BlackboardUtils.SetOrCreateValue(bb, "metaGameEventId", videoPokerClaimBonusRequestV2.metaGameEventId);
        BlackboardUtils.SetOrCreateValue(bb, "seasonPassEventId", videoPokerClaimBonusRequestV2.seasonPassEventId);
    }

    public static void Serialize(IBlackboard bb, VideoPokerClaimBonusResponseV2 videoPokerClaimBonusResponseV2)
    {
        if (videoPokerClaimBonusResponseV2 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", videoPokerClaimBonusResponseV2.error);
        if (videoPokerClaimBonusResponseV2.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), videoPokerClaimBonusResponseV2.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", videoPokerClaimBonusResponseV2.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "contents", videoPokerClaimBonusResponseV2.contents);
        if (videoPokerClaimBonusResponseV2.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), videoPokerClaimBonusResponseV2.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
    }

    public static void Serialize(IBlackboard bb, VideoPokerDealRequestV2 videoPokerDealRequestV2)
    {
        if (videoPokerDealRequestV2 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", videoPokerDealRequestV2.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", videoPokerDealRequestV2.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "contents", videoPokerDealRequestV2.contents);
        BlackboardUtils.SetOrCreateValue(bb, "gameId", videoPokerDealRequestV2.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "isBonusDeal", videoPokerDealRequestV2.isBonusDeal);
        BlackboardUtils.SetOrCreateValue(bb, "isGameDeal", videoPokerDealRequestV2.isGameDeal);
        BlackboardUtils.SetOrCreateValue(bb, "metaGameEventId", videoPokerDealRequestV2.metaGameEventId);
        BlackboardUtils.SetOrCreateValue(bb, "collectingGameChestDropRateMultiplyEventId", videoPokerDealRequestV2.collectingGameChestDropRateMultiplyEventId);
        BlackboardUtils.SetOrCreateValue(bb, "expEventIdList", videoPokerDealRequestV2.expEventIdList);
        BlackboardUtils.SetOrCreateValue(bb, "isHighRollerBet", videoPokerDealRequestV2.isHighRollerBet);
        BlackboardUtils.SetOrCreateValue(bb, "seasonPassEventId", videoPokerDealRequestV2.seasonPassEventId);
    }

    public static void Serialize(IBlackboard bb, VideoPokerDealResponseV3 videoPokerDealResponseV3)
    {
        if (videoPokerDealResponseV3 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", videoPokerDealResponseV3.error);
        if (videoPokerDealResponseV3.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), videoPokerDealResponseV3.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", videoPokerDealResponseV3.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "contents", videoPokerDealResponseV3.contents);
        if (videoPokerDealResponseV3.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), videoPokerDealResponseV3.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
        if (videoPokerDealResponseV3.tournamentInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "tournamentInfo"), videoPokerDealResponseV3.tournamentInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "tournamentInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "nextMysteryGiftLevel", videoPokerDealResponseV3.nextMysteryGiftLevel);
        if (videoPokerDealResponseV3.mysteryGiftInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "mysteryGiftInfo"), videoPokerDealResponseV3.mysteryGiftInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "mysteryGiftInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "featureUnlockList", videoPokerDealResponseV3.featureUnlockList);
        BlackboardUtils.SetOrCreateValue(bb, "gameDealCount", videoPokerDealResponseV3.gameDealCount);
        BlackboardUtils.SetOrCreateValue(bb, "bonusDealCount", videoPokerDealResponseV3.bonusDealCount);
        if (videoPokerDealResponseV3.metaGameInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "metaGameInfo"), videoPokerDealResponseV3.metaGameInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "metaGameInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "earnFinder", videoPokerDealResponseV3.earnFinder);
        if (videoPokerDealResponseV3.vipLoungeCompositeInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "vipLoungeCompositeInfo"), videoPokerDealResponseV3.vipLoungeCompositeInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "vipLoungeCompositeInfo");
        }
        if (videoPokerDealResponseV3.levelUpDashInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "levelUpDashInfo"), videoPokerDealResponseV3.levelUpDashInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "levelUpDashInfo");
        }
    }

    public static void Serialize(IBlackboard bb, VideoPokerDrawRequestV2 videoPokerDrawRequestV2)
    {
        if (videoPokerDrawRequestV2 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", videoPokerDrawRequestV2.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", videoPokerDrawRequestV2.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "contents", videoPokerDrawRequestV2.contents);
        BlackboardUtils.SetOrCreateValue(bb, "gameId", videoPokerDrawRequestV2.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "isBonusDeal", videoPokerDrawRequestV2.isBonusDeal);
        BlackboardUtils.SetOrCreateValue(bb, "isGameDeal", videoPokerDrawRequestV2.isGameDeal);
        BlackboardUtils.SetOrCreateValue(bb, "metaGameEventId", videoPokerDrawRequestV2.metaGameEventId);
        BlackboardUtils.SetOrCreateValue(bb, "seasonPassEventId", videoPokerDrawRequestV2.seasonPassEventId);
    }

    public static void Serialize(IBlackboard bb, VideoPokerDrawResponseV2 videoPokerDrawResponseV2)
    {
        if (videoPokerDrawResponseV2 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", videoPokerDrawResponseV2.error);
        if (videoPokerDrawResponseV2.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), videoPokerDrawResponseV2.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", videoPokerDrawResponseV2.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "contents", videoPokerDrawResponseV2.contents);
        if (videoPokerDrawResponseV2.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), videoPokerDrawResponseV2.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
    }

    public static void Serialize(IBlackboard bb, VideoPokerReportDealEndRequest videoPokerReportDealEndRequest)
    {
        if (videoPokerReportDealEndRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", videoPokerReportDealEndRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", videoPokerReportDealEndRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "startBlockseq", videoPokerReportDealEndRequest.startBlockseq);
    }

    public static void Serialize(IBlackboard bb, VideoPokerReportDealEndRequestV1 videoPokerReportDealEndRequestV1)
    {
        if (videoPokerReportDealEndRequestV1 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", videoPokerReportDealEndRequestV1.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", videoPokerReportDealEndRequestV1.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "startBlockseq", videoPokerReportDealEndRequestV1.startBlockseq);
        BlackboardUtils.SetOrCreateValue(bb, "metaGameEventId", videoPokerReportDealEndRequestV1.metaGameEventId);
    }

    public static void Serialize(IBlackboard bb, VideoPokerReportDealEndRequestV3 videoPokerReportDealEndRequestV3)
    {
        if (videoPokerReportDealEndRequestV3 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", videoPokerReportDealEndRequestV3.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", videoPokerReportDealEndRequestV3.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "startBlockseq", videoPokerReportDealEndRequestV3.startBlockseq);
        BlackboardUtils.SetOrCreateValue(bb, "metaGameEventId", videoPokerReportDealEndRequestV3.metaGameEventId);
        BlackboardUtils.SetOrCreateValue(bb, "seasonPassEventId", videoPokerReportDealEndRequestV3.seasonPassEventId);
    }

    public static void Serialize(IBlackboard bb, VipDailyBonusCollectRequest vipDailyBonusCollectRequest)
    {
        if (vipDailyBonusCollectRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", vipDailyBonusCollectRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "currentTime", vipDailyBonusCollectRequest.currentTime);
        BlackboardUtils.SetOrCreateValue(bb, "timezoneOffset", vipDailyBonusCollectRequest.timezoneOffset);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", vipDailyBonusCollectRequest.ackMask);
    }

    public static void Serialize(IBlackboard bb, VipDailyBonusCollectResponse vipDailyBonusCollectResponse)
    {
        if (vipDailyBonusCollectResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", vipDailyBonusCollectResponse.error);
        if (vipDailyBonusCollectResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), vipDailyBonusCollectResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", vipDailyBonusCollectResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "earnCredit", vipDailyBonusCollectResponse.earnCredit);
        BlackboardUtils.SetOrCreateValue(bb, "vipDailybonusTimestamp", vipDailyBonusCollectResponse.vipDailybonusTimestamp);
        if (vipDailyBonusCollectResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), vipDailyBonusCollectResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "earnLoungePoint", vipDailyBonusCollectResponse.earnLoungePoint);
        if (vipDailyBonusCollectResponse.vipLoungeInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "vipLoungeInfo"), vipDailyBonusCollectResponse.vipLoungeInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "vipLoungeInfo");
        }
    }

    public static void Serialize(IBlackboard bb, VipDailyBonusVideoAdsFreeRequest vipDailyBonusVideoAdsFreeRequest)
    {
        if (vipDailyBonusVideoAdsFreeRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", vipDailyBonusVideoAdsFreeRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", vipDailyBonusVideoAdsFreeRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "currentTime", vipDailyBonusVideoAdsFreeRequest.currentTime);
        BlackboardUtils.SetOrCreateValue(bb, "timezoneOffset", vipDailyBonusVideoAdsFreeRequest.timezoneOffset);
    }

    public static void Serialize(IBlackboard bb, VipDailyBonusVideoAdsFreeResponse vipDailyBonusVideoAdsFreeResponse)
    {
        if (vipDailyBonusVideoAdsFreeResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", vipDailyBonusVideoAdsFreeResponse.error);
        if (vipDailyBonusVideoAdsFreeResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), vipDailyBonusVideoAdsFreeResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", vipDailyBonusVideoAdsFreeResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "earnCredit", vipDailyBonusVideoAdsFreeResponse.earnCredit);
        BlackboardUtils.SetOrCreateValue(bb, "lastShopBonusVideoAdsClaimTimestamp", vipDailyBonusVideoAdsFreeResponse.lastShopBonusVideoAdsClaimTimestamp);
        if (vipDailyBonusVideoAdsFreeResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), vipDailyBonusVideoAdsFreeResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
    }

    public static void Serialize(IBlackboard bb, VipDeal vipDeal)
    {
        if (vipDeal == null) { return; }
        if (vipDeal.product != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "product"), vipDeal.product);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "product");
        }
        BlackboardUtils.SetOrCreateValue(bb, "multiplier", vipDeal.multiplier);
        BlackboardUtils.SetOrCreateValue(bb, "isFree", vipDeal.isFree);
        BlackboardUtils.SetOrCreateValue(bb, "isSoldOut", vipDeal.isSoldOut);
        BlackboardUtils.SetOrCreateValue(bb, "dealUuid", vipDeal.dealUuid);
        BlackboardUtils.SetOrCreateValue(bb, "multiplierNumerator", vipDeal.multiplierNumerator);
    }

    public static void Serialize(IBlackboard bb, VipDealCommonRequestV2 vipDealCommonRequestV2)
    {
        if (vipDealCommonRequestV2 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", vipDealCommonRequestV2.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", vipDealCommonRequestV2.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "vipDealInfoId", vipDealCommonRequestV2.vipDealInfoId);
    }

    public static void Serialize(IBlackboard bb, VipDealEnterRequest vipDealEnterRequest)
    {
        if (vipDealEnterRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", vipDealEnterRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", vipDealEnterRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "vipDealInfoId", vipDealEnterRequest.vipDealInfoId);
    }

    public static void Serialize(IBlackboard bb, VipDealFreebieRedeemResponse vipDealFreebieRedeemResponse)
    {
        if (vipDealFreebieRedeemResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", vipDealFreebieRedeemResponse.error);
        if (vipDealFreebieRedeemResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), vipDealFreebieRedeemResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", vipDealFreebieRedeemResponse.serverTime);
        if (vipDealFreebieRedeemResponse.rewardResult != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "rewardResult"), vipDealFreebieRedeemResponse.rewardResult);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "rewardResult");
        }
        if (vipDealFreebieRedeemResponse.userSyncInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "userSyncInfo"), vipDealFreebieRedeemResponse.userSyncInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "userSyncInfo");
        }
    }

    public static void Serialize(IBlackboard bb, VipDealInfo vipDealInfo)
    {
        if (vipDealInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "startTimestamp", vipDealInfo.startTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "endTimestamp", vipDealInfo.endTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "multiplierList", vipDealInfo.multiplierList);
        BlackboardUtils.SetOrCreateList(bb, "dealList", vipDealInfo.dealList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "isViewed", vipDealInfo.isViewed);
        BlackboardUtils.SetOrCreateValue(bb, "shopId", vipDealInfo.shopId);
        BlackboardUtils.SetOrCreateValue(bb, "vipDealInfoId", vipDealInfo.vipDealInfoId);
        BlackboardUtils.SetOrCreateValue(bb, "multiplierNumeratorList", vipDealInfo.multiplierNumeratorList);
    }

    public static void Serialize(IBlackboard bb, VipDealInfoV2 vipDealInfoV2)
    {
        if (vipDealInfoV2 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "startTimestamp", vipDealInfoV2.startTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "endTimestamp", vipDealInfoV2.endTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "freeMultiplierNumeratorList", vipDealInfoV2.freeMultiplierNumeratorList);
        BlackboardUtils.SetOrCreateList(bb, "freeDealList", vipDealInfoV2.freeDealList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "paidMultiplierNumeratorList", vipDealInfoV2.paidMultiplierNumeratorList);
        BlackboardUtils.SetOrCreateList(bb, "paidDealList", vipDealInfoV2.paidDealList, Serialize);
        BlackboardUtils.SetOrCreateValue(bb, "isViewedFree", vipDealInfoV2.isViewedFree);
        BlackboardUtils.SetOrCreateValue(bb, "isViewedPaid", vipDealInfoV2.isViewedPaid);
        BlackboardUtils.SetOrCreateValue(bb, "shopId", vipDealInfoV2.shopId);
        BlackboardUtils.SetOrCreateValue(bb, "vipDealInfoId", vipDealInfoV2.vipDealInfoId);
        BlackboardUtils.SetOrCreateValue(bb, "isFreebieRedeemed", vipDealInfoV2.isFreebieRedeemed);
    }

    public static void Serialize(IBlackboard bb, VipLoungeBenefitInfo vipLoungeBenefitInfo)
    {
        if (vipLoungeBenefitInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "FRIENDS_BONUS", vipLoungeBenefitInfo.FRIENDS_BONUS);
        BlackboardUtils.SetOrCreateValue(bb, "SHOP_BONUS", vipLoungeBenefitInfo.SHOP_BONUS);
        BlackboardUtils.SetOrCreateValue(bb, "LOBBY_BONUS", vipLoungeBenefitInfo.LOBBY_BONUS);
        BlackboardUtils.SetOrCreateValue(bb, "LP_BOOST", vipLoungeBenefitInfo.LP_BOOST);
    }

    public static void Serialize(IBlackboard bb, VipLoungeBetInfo vipLoungeBetInfo)
    {
        if (vipLoungeBetInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "qualifiedBetAmount", vipLoungeBetInfo.qualifiedBetAmount);
        BlackboardUtils.SetOrCreateValue(bb, "buildDreamBetAmount", vipLoungeBetInfo.buildDreamBetAmount);
    }

    public static void Serialize(IBlackboard bb, VipLoungeChallengeValues vipLoungeChallengeValues)
    {
        if (vipLoungeChallengeValues == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "QUALIFIED_SPINS", vipLoungeChallengeValues.QUALIFIED_SPINS);
        BlackboardUtils.SetOrCreateValue(bb, "REACH_LEVEL", vipLoungeChallengeValues.REACH_LEVEL);
        BlackboardUtils.SetOrCreateValue(bb, "COLLECT_SHOP_BONUS", vipLoungeChallengeValues.COLLECT_SHOP_BONUS);
        BlackboardUtils.SetOrCreateValue(bb, "COLLECT_TIME_BONUS", vipLoungeChallengeValues.COLLECT_TIME_BONUS);
        BlackboardUtils.SetOrCreateValue(bb, "COLLECT_LUCKY_SPIN", vipLoungeChallengeValues.COLLECT_LUCKY_SPIN);
    }

    public static void Serialize(IBlackboard bb, VipLoungeCompositeInfo vipLoungeCompositeInfo)
    {
        if (vipLoungeCompositeInfo == null) { return; }
        if (vipLoungeCompositeInfo.vipLoungeInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "vipLoungeInfo"), vipLoungeCompositeInfo.vipLoungeInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "vipLoungeInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "earnedDepotType", vipLoungeCompositeInfo.earnedDepotType);
        if (vipLoungeCompositeInfo.buildDreamInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "buildDreamInfo"), vipLoungeCompositeInfo.buildDreamInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "buildDreamInfo");
        }
    }

    public static void Serialize(IBlackboard bb, VipLoungeEnterResponse vipLoungeEnterResponse)
    {
        if (vipLoungeEnterResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", vipLoungeEnterResponse.error);
        if (vipLoungeEnterResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), vipLoungeEnterResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", vipLoungeEnterResponse.serverTime);
        if (vipLoungeEnterResponse.vipLoungeInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "vipLoungeInfo"), vipLoungeEnterResponse.vipLoungeInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "vipLoungeInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "cumulatedSpinCount", vipLoungeEnterResponse.cumulatedSpinCount);
        if (vipLoungeEnterResponse.loungeJackpotInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "loungeJackpotInfo"), vipLoungeEnterResponse.loungeJackpotInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "loungeJackpotInfo");
        }
        if (vipLoungeEnterResponse.loungeJackpotPresentationValues != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "loungeJackpotPresentationValues"), vipLoungeEnterResponse.loungeJackpotPresentationValues);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "loungeJackpotPresentationValues");
        }
        BlackboardUtils.SetOrCreateValue(bb, "qualifiedBetAmount", vipLoungeEnterResponse.qualifiedBetAmount);
    }

    public static void Serialize(IBlackboard bb, VipLoungeInfo vipLoungeInfo)
    {
        if (vipLoungeInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "active", vipLoungeInfo.active);
        BlackboardUtils.SetOrCreateValue(bb, "activeForUser", vipLoungeInfo.activeForUser);
        BlackboardUtils.SetOrCreateValue(bb, "loungePoint", vipLoungeInfo.loungePoint);
        BlackboardUtils.SetOrCreateValue(bb, "excessLoungePoint", vipLoungeInfo.excessLoungePoint);
        BlackboardUtils.SetOrCreateValue(bb, "gaugeMax", vipLoungeInfo.gaugeMax);
        BlackboardUtils.SetOrCreateValue(bb, "extendedBetIndex", vipLoungeInfo.extendedBetIndex);
        BlackboardUtils.SetOrCreateValue(bb, "benefitEndTimestamp", vipLoungeInfo.benefitEndTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "badgeCount", vipLoungeInfo.badgeCount);
        if (vipLoungeInfo.benefitInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "benefitInfo"), vipLoungeInfo.benefitInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "benefitInfo");
        }
        BlackboardUtils.SetOrCreateValue(bb, "loungeJackpotActivateTimestamp", vipLoungeInfo.loungeJackpotActivateTimestamp);
    }

    public static void Serialize(IBlackboard bb, VipLoungeMisc vipLoungeMisc)
    {
        if (vipLoungeMisc == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "VIP_LOUNGE_EXP_BOOST_MULTIPLIER_NUMERATOR", vipLoungeMisc.VIP_LOUNGE_EXP_BOOST_MULTIPLIER_NUMERATOR);
        if (vipLoungeMisc.CHALLENGE_VALUES != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "CHALLENGE_VALUES"), vipLoungeMisc.CHALLENGE_VALUES);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "CHALLENGE_VALUES");
        }
        BlackboardUtils.SetOrCreateValue(bb, "CUMULATIVE_SPIN_MAX", vipLoungeMisc.CUMULATIVE_SPIN_MAX);
        BlackboardUtils.SetOrCreateValue(bb, "LOUNGE_OPEN_TIME_MILLISEC", vipLoungeMisc.LOUNGE_OPEN_TIME_MILLISEC);
        BlackboardUtils.SetOrCreateValue(bb, "LOUNGE_JACKPOT_TIMER_MILLISEC", vipLoungeMisc.LOUNGE_JACKPOT_TIMER_MILLISEC);
    }

    public static void Serialize(IBlackboard bb, WallOfEpicInfo wallOfEpicInfo)
    {
        if (wallOfEpicInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "id", wallOfEpicInfo.id);
        BlackboardUtils.SetOrCreateValue(bb, "winCredit", wallOfEpicInfo.winCredit);
        BlackboardUtils.SetOrCreateValue(bb, "reportedTimestamp", wallOfEpicInfo.reportedTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "screenshotUrl", wallOfEpicInfo.screenshotUrl);
        BlackboardUtils.SetOrCreateValue(bb, "gameId", wallOfEpicInfo.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "betCredit", wallOfEpicInfo.betCredit);
        BlackboardUtils.SetOrCreateValue(bb, "epicWinCount", wallOfEpicInfo.epicWinCount);
        BlackboardUtils.SetOrCreateValue(bb, "starCount", wallOfEpicInfo.starCount);
        BlackboardUtils.SetOrCreateValue(bb, "nextRequiredEpicWinCount", wallOfEpicInfo.nextRequiredEpicWinCount);
        BlackboardUtils.SetOrCreateValue(bb, "prevEpicWinCount", wallOfEpicInfo.prevEpicWinCount);
    }

    public static void Serialize(IBlackboard bb, WallOfEpicLikeRequest wallOfEpicLikeRequest)
    {
        if (wallOfEpicLikeRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", wallOfEpicLikeRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", wallOfEpicLikeRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "id", wallOfEpicLikeRequest.id);
    }

    public static void Serialize(IBlackboard bb, WallOfEpicRecentListResponse wallOfEpicRecentListResponse)
    {
        if (wallOfEpicRecentListResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", wallOfEpicRecentListResponse.error);
        if (wallOfEpicRecentListResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), wallOfEpicRecentListResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", wallOfEpicRecentListResponse.serverTime);
        BlackboardUtils.SetOrCreateList(bb, "recordList", wallOfEpicRecentListResponse.recordList, Serialize);
    }

    public static void Serialize(IBlackboard bb, WallOfEpicRecord wallOfEpicRecord)
    {
        if (wallOfEpicRecord == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "id", wallOfEpicRecord.id);
        if (wallOfEpicRecord.profile != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "profile"), wallOfEpicRecord.profile);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "profile");
        }
        BlackboardUtils.SetOrCreateValue(bb, "winCredit", wallOfEpicRecord.winCredit);
        BlackboardUtils.SetOrCreateValue(bb, "like", wallOfEpicRecord.like);
        BlackboardUtils.SetOrCreateValue(bb, "reportedTimestamp", wallOfEpicRecord.reportedTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "screenshotUrl", wallOfEpicRecord.screenshotUrl);
        BlackboardUtils.SetOrCreateValue(bb, "gameId", wallOfEpicRecord.gameId);
        BlackboardUtils.SetOrCreateValue(bb, "winRatio", wallOfEpicRecord.winRatio);
    }

    public static void Serialize(IBlackboard bb, WallOfEpicRegisterRequestV1 wallOfEpicRegisterRequestV1)
    {
        if (wallOfEpicRegisterRequestV1 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", wallOfEpicRegisterRequestV1.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", wallOfEpicRegisterRequestV1.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "winCredit", wallOfEpicRegisterRequestV1.winCredit);
        BlackboardUtils.SetOrCreateValue(bb, "betCredit", wallOfEpicRegisterRequestV1.betCredit);
        BlackboardUtils.SetOrCreateValue(bb, "gameId", wallOfEpicRegisterRequestV1.gameId);
    }

    public static void Serialize(IBlackboard bb, WallOfEpicRegisterResponseV1 wallOfEpicRegisterResponseV1)
    {
        if (wallOfEpicRegisterResponseV1 == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", wallOfEpicRegisterResponseV1.error);
        if (wallOfEpicRegisterResponseV1.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), wallOfEpicRegisterResponseV1.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", wallOfEpicRegisterResponseV1.serverTime);
        if (wallOfEpicRegisterResponseV1.woeInfo != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "woeInfo"), wallOfEpicRegisterResponseV1.woeInfo);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "woeInfo");
        }
        BlackboardUtils.SetOrCreateList(bb, "woeInfoList", wallOfEpicRegisterResponseV1.woeInfoList, Serialize);
    }

    public static void Serialize(IBlackboard bb, WallOfEpicUploadImageRequest wallOfEpicUploadImageRequest)
    {
        if (wallOfEpicUploadImageRequest == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "blockseq", wallOfEpicUploadImageRequest.blockseq);
        BlackboardUtils.SetOrCreateValue(bb, "ackMask", wallOfEpicUploadImageRequest.ackMask);
        BlackboardUtils.SetOrCreateValue(bb, "id", wallOfEpicUploadImageRequest.id);
    }

    public static void Serialize(IBlackboard bb, WallOfEpicUploadImageResponse wallOfEpicUploadImageResponse)
    {
        if (wallOfEpicUploadImageResponse == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "error", wallOfEpicUploadImageResponse.error);
        if (wallOfEpicUploadImageResponse.common != null)
        {
            Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), wallOfEpicUploadImageResponse.common);
        }
        else
        {
            BlackboardUtils.DestroyBlackboard(bb, "common");
        }
        BlackboardUtils.SetOrCreateValue(bb, "serverTime", wallOfEpicUploadImageResponse.serverTime);
        BlackboardUtils.SetOrCreateValue(bb, "screenshotUrl", wallOfEpicUploadImageResponse.screenshotUrl);
    }

    public static void Serialize(IBlackboard bb, WelcomeBackRewardInfo welcomeBackRewardInfo)
    {
        if (welcomeBackRewardInfo == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "prevLoginTimestamp", welcomeBackRewardInfo.prevLoginTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "currentLoginTimestamp", welcomeBackRewardInfo.currentLoginTimestamp);
        BlackboardUtils.SetOrCreateValue(bb, "earnCredit", welcomeBackRewardInfo.earnCredit);
        BlackboardUtils.SetOrCreateValue(bb, "totalEarnCredit", welcomeBackRewardInfo.totalEarnCredit);
    }

    public static void Serialize(IBlackboard bb, numbersAroundCube numbersAroundCube)
    {
        if (numbersAroundCube == null) { return; }
        BlackboardUtils.SetOrCreateValue(bb, "numbers", numbersAroundCube.numbers);
    }
}
}
