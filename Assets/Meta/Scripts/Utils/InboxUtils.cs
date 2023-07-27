using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using SlotMaker;
using BagelCode.ClientModels;
using BagelCode.OSA_Scroll;
using System.Linq;

using InboxGroup = System.Collections.Generic.List<NodeCanvas.Framework.Blackboard>;

namespace BagelCode
{
    public class InboxUtils
    {
        public static bool InboxAcceptRequest(Blackboard agent, BBParameter<Blackboard> inboxInfo, System.Action onResponse = null)
        {
            Variable<int> inboxId = BlackboardUtils.FindVariable<int>(inboxInfo.value, "id");
            Variable<int> bucksGiftId = BlackboardUtils.FindVariable<int>(inboxInfo.value, "giftId");
            bool isSuccess = true;

            if (inboxId != null)
            {
                BagelCodeClientAPI.InboxAccept(inboxId.value,
                (response) =>
                {
                    var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "inboxResponse");
                    bb = BlackboardUtils.GetOrCreateBlackboard(bb, string.Format("ID_{0}", inboxId.value));
                    ClientAPI2Blackboard.Serialize(bb, response);

                    UpdateEarnValues(bb as Blackboard);
                    isSuccess = ApplyRewardInfo(agent, inboxInfo, bb as Blackboard, response);
                    BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);

                    if (isSuccess)
                    {
                        if (agent != null)
                        {
                            EventSender.SendEvent(agent.gameObject, InboxEvent.ON_SUCCESS_ACCEPT_INBOX);
                        }
                        onResponse?.Invoke();
                    }
                },
                (error) =>
                {
                    Debug.LogWarning("Occur error in InboxAcceptRequest: " + error.errorCode);
                    if (error.errorCode == ClientModels.Error.DAILY_BOOST_ALREADY_EXIST_ERROR)
                    {
                        bool stringError = false;
                        ErrorPopupInfo info = new ErrorPopupInfo();
                        info.type = ErrorPopupType.OK;
                        info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_DAILY_BOOST_ALREADY_EXIST", out stringError);
                        info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);
                        ErrorPopupHandler.Instance.OpenError(info);

                        info.callback1 = delegate
                        {
                            if (agent != null)
                            {
                                EventSender.SendEvent(agent.gameObject, InboxEvent.ON_CANCEL_ACCEPT_INBOX);
                            }
                        };
                        // return true;
                    }
                    else if (error.errorCode == ClientModels.Error.VIP_LOUNGE_NOT_ACTIVE_ERROR)
                    {
                        if (agent != null)
                        {
                            EventSender.SendEvent(agent.gameObject, InboxEvent.ON_FAIL_ACCEPT_INBOX);
                        }
                    }
                    else
                    {
                        GlobalErrorHandler.GlobalError(error);
                    }
                });
            }
            else if (bucksGiftId != null)   // Vegas Bucks - gift accept
            {
                BagelCodeClientAPI.RequestBucksRedeemGift(bucksGiftId.value,
                    (response) =>
                    {
                        var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "inboxResponse");
                        bb = BlackboardUtils.GetOrCreateBlackboard(bb, string.Format("GIFT_ID_{0}", bucksGiftId.value));
                        ClientAPI2Blackboard.Serialize(bb, response);

                        if (agent != null)
                        {
                            EventSender.SendEvent(agent.gameObject, InboxEvent.ON_SUCCESS_ACCEPT_GIFT_BUCKS);
                        }
                        onResponse?.Invoke();
                    },
                    (error) =>
                    {
                        Debug.LogWarning("Occur error in BucksRedeemGiftRequest: " + error.errorCode);
                        GlobalErrorHandler.GlobalError(error);
                    });
            }
            else
            {
                // return false;
                Debug.LogError("No ItemId found." + agent.gameObject.name);
            }

            return isSuccess;
        }

        public static string GetInboxIconPrefabName(InboxCellData.IconType iconType, Blackboard inboxInfo = null)
        {
            switch (iconType)
            {
                case InboxCellData.IconType.CREDIT:
                    return "Inbox Icon Coin";
                case InboxCellData.IconType.RP:
                    return "Inbox Icon VIP Point";
                case InboxCellData.IconType.DAILY_BOOST:
                    return "Inbox Icon Daily Boost";
                case InboxCellData.IconType.PURCHASE_COUPON:
                    return "Badge Event";
                case InboxCellData.IconType.FACEBOOK:
                    return "Inbox Icon Facebook";
                case InboxCellData.IconType.EMAIL:
                    return "Inbox Icon Email";
                case InboxCellData.IconType.FACEBOOK_FRIEND_CONNECT:
                    return "Inbox Icon Facebook Friend Join";
                case InboxCellData.IconType.FACEBOOK_SHARE:
                    return "Inbox Icon Share";
                case InboxCellData.IconType.PURCHASE_RECOVER_CREDIT:
                    return "Inbox Icon Reward Coin";
                case InboxCellData.IconType.PURCHASE_RECOVER_RP:
                    return "Inbox Icon Reward VIP Point";
                case InboxCellData.IconType.TIER_UPGRADE:
                    return "Image Tier";
                case InboxCellData.IconType.DAILY_BONUS_WHEEL_SPIN:
                    return "Inbox Icon Daily Spin";
                case InboxCellData.IconType.TOURNAMENT_WIN:
                    return "Inbox Icon Tournament";
                case InboxCellData.IconType.EXP_MULTIPLY:
                    return "Inbox Icon Exp Boost";
                case InboxCellData.IconType.RANDOM:
                    return "Inbox Icon Mystery Reward";
                case InboxCellData.IconType.NEWS:
                    return "Inbox Icon Notice";
                case InboxCellData.IconType.WEB_IMAGE:
                    return "Inbox Icon Web Image";
                case InboxCellData.IconType.INSTANT_BONUS:
                    return "Inbox Icon Instant Bonus";
                case InboxCellData.IconType.BUY_A_BONUS:
                    return "Inbox Icon Buy A Bonus";
                case InboxCellData.IconType.SUPER_BONUS:
                    return "Inbox Icon Super Bonus";
                case InboxCellData.IconType.GEM:
                    return "Inbox Icon Gem";
                case InboxCellData.IconType.MEGA_WHEEL_SPIN:
                    return "Inbox Icon Mega Wheel";
                case InboxCellData.IconType.SPIN_BOOST:
                    return "Inbox Icon Label";
                case InboxCellData.IconType.HOG_DEAL:
                    return "Inbox Icon Hog Deal";
                case InboxCellData.IconType.FINDER:
                    return "Inbox Icon Finder";
                case InboxCellData.IconType.VIP_INVITE:
                    return "Inbox Icon VIP Invite";
                case InboxCellData.IconType.VIP_EPIC_LOUNGE:
                    return "Inbox Icon VIP Epic Lounge";
                case InboxCellData.IconType.VIP_LOUNGE_OPEN_TICKET:
                    return "Inbox Icon VIP Lounge Ticket";
                case InboxCellData.IconType.VIP_LOUNGE_JACKPOT:
                    return "Inbox Icon VIP Lounge Jackpot";
                case InboxCellData.IconType.BOSSRAIDERS_DEAL:
                    return "Inbox Icon Boss Raiders Deal";
                case InboxCellData.IconType.DEPOT:
                    var depotType = BlackboardUtils.FindValue<DepotType>(inboxInfo, "reward/depotType");
                    return $"Inbox Icon Vegas Dreams Depot {depotType}";
                case InboxCellData.IconType.WILD_PUZZLE:
                    return "Inbox Icon Wild Puzzle";
                case InboxCellData.IconType.LEVEL_UP_EXP_BOOST:
                    return "Inbox Icon Level Up Exp Boost";
                case InboxCellData.IconType.BUCKS_GIFT:
                    return "Inbox Icon Vegas Bucks";
                default:
                    //case InboxCellData.IconType.DEFAULT:
                    return "Inbox Icon Default";
            }
        }

        //

        private static bool ApplyRewardInfo(Blackboard agent, BBParameter<Blackboard> inboxInfo, Blackboard responseBB, InboxAcceptResponse response)
        {
            bool isSuccess = true;

            var inboxType = BlackboardUtils.FindVariable<InboxTypes>(inboxInfo.value, "type");
            if (inboxType.value == InboxTypes.REWARD)
            {
                var inboxRewardType = BlackboardUtils.FindVariable<RewardType>(inboxInfo.value, "reward/type");
                switch (inboxRewardType.value)
                {
                    case RewardType.DAILY_BOOST:
                        {
                            RewardResult reswardResult = (RewardResult)response.inboxResult;
                            if (reswardResult != null)
                            {
                                RewardResultDailyBoost dailyBoostResult = (RewardResultDailyBoost)reswardResult.rewardResult;
                                if (dailyBoostResult != null)
                                {
                                    BlackboardQueryUtils.ApplyDailyBoostInfo(dailyBoostResult.dailyBoost);
                                    BlackboardQueryUtils.UpdateDailyBoostState();
                                }
                            }
                        }
                        break;

                    case RewardType.TIER_UPGRADE:
                        {
                            var tier = BlackboardUtils.FindVariable<int>(null, "/me/tier");
                            var targetTier = BlackboardUtils.FindVariable<int>(inboxInfo.value, "reward/targetTier");
                            if (tier.value < targetTier.value)
                            {
                                var accRp = responseBB.GetValue<Blackboard>("userSyncInfo").GetValue<long>("accRp");
                                BlackboardUtils.FindVariable<long>(null, "/me/accRp").value = accRp;

                                TierUtils.SetTier(targetTier.value);
                            }   
                            else
                            {
                                bool stringError = false;
                                ErrorPopupInfo info = new ErrorPopupInfo();
                                info.type = ErrorPopupType.OK;
                                info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "INBOX_ITEM_TEXT_TIER_UPGRADE_ALERT", out stringError);
                                info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);
                                ErrorPopupHandler.Instance.OpenError(info);
                                info.callback1 = delegate
                                {
                                    if (agent != null)
                                    {
                                        EventSender.SendEvent(agent.gameObject, InboxEvent.ON_FAIL_ACCEPT_INBOX);
                                    }
                                };
                                isSuccess = false;
                            }
                        }
                        break;

                    case RewardType.DAILY_BONUS_WHEEL_SPIN:
                        {
                            BlackboardQueryUtils.SetDailySpinCount(responseBB.GetValue<int>("totalSpinCount"), MetaJackpotType.DAILY_BONUS);
                        }
                        break;
                    case RewardType.GAME_SPIN:
                        {
                            BlackboardQueryUtils.AddGameSpinCount(responseBB.GetValue<int>("gameId"), responseBB.GetValue<int>("addedSpinCount"));
                        }
                        break;
                    case RewardType.GAME_DEAL:
                    case RewardType.GAME_PLAY:
                        {
                            BlackboardQueryUtils.AddGameSpinCount(responseBB.GetValue<int>("gameId"), responseBB.GetValue<int>("addedCount"));
                        }
                        break;
                    case RewardType.INVITE_INSTALL_WITH_TIER:
                        {
                            NativeHelper.Instance.ShareSNSUrl(responseBB.GetValue<string>("snsInviteUrl"));
                        }
                        break;
                    default:
                        break;
                }
            }

            return isSuccess;
        }

        private static void UpdateEarnValues(Blackboard responseBB)
        {
            InboxTypes inboxType = BlackboardUtils.FindVariable<InboxTypes>(responseBB, "inboxType").value;

            long earnCoins = 0;
            long earnRP = 0;

            switch (inboxType)
            {
                case InboxTypes.REWARD:
                    BlackboardQueryUtils.ApplyRewardResult(responseBB, true);
                    break;
                case InboxTypes.MESSAGE:
                    break;
                case InboxTypes.MESSAGE_WARNING:
                    break;
                case InboxTypes.GAME_COMPENSATION:
                    earnCoins = BlackboardUtils.FindVariable<long>(responseBB, "credit").value;
                    break;
                case InboxTypes.FACEBOOK_FRIEND_CONNECT:
                    earnCoins = BlackboardUtils.FindVariable<long>(responseBB, "credit").value;
                    break;
                case InboxTypes.FACEBOOK_SHARE:
                    earnRP = BlackboardUtils.FindVariable<long>(responseBB, "rp").value;
                    break;
                case InboxTypes.TOURNAMENT_WIN:
                    earnCoins = BlackboardUtils.FindVariable<long>(responseBB, "actualWinCredit").value;
                    break;
                case InboxTypes.SOCIAL_CREDIT:
                    earnCoins = BlackboardUtils.FindVariable<long>(responseBB, "credit").value;
                    earnCoins = FreebieLevelUtils.GetLevelMultiplierNumeratorValue(earnCoins, FreebieLevelUtils.FreebieType.FRIENDS_DEAL_BONUS);
                    break;
                default:
                    break;
            }

            BlackboardQueryUtils.AddCoins(earnCoins);
            BlackboardQueryUtils.AddRP(earnRP);
        }

        public static string GetInboxEnterContextID()
        {
            var contentBB = ContentBlackboard.Get();
            if (contentBB != null)
            {
                var inboxEnterContextID = contentBB.GetVariable<string>("inboxEnterContextId");

                if (inboxEnterContextID == null)
                {
                    string newContextId = BiEventUtils.GenerateContextID();
                    inboxEnterContextID = (Variable<string>)contentBB.AddVariable("inboxEnterContextId", newContextId);
                }

                return inboxEnterContextID.value;
            }

            return null;
        }

        public static void RemoveAcceptInboxItem(OSA_InboxItems items, int id)
        {
            items.RemoveItemFromID(id);

            BlackboardQueryUtils.RemoveInboxItem(id);
            BlackboardQueryUtils.UpdateCollectAllCredit();
        }

        public static void RemoveAcceptVegasBucksItem(OSA_InboxItems items, int giftId)
        {
            items.RemoveItemFromGiftId(giftId);

            BlackboardQueryUtils.RemoveInboxVegasBucksItem(giftId);
        }

        public static bool IsGroup(Blackboard inboxInfo)
        {
            // Comparing Unity objects with null and '==' operators is more expensive and may not be accurate than regular comparisons.
            // References about this: https://overworks.github.io/unity/2019/07/16/null-of-unity-object.html
            // if (inboxInfo == null) return false;
            if (inboxInfo is null) return false;

            var groupId = inboxInfo.GetVariable<string>("groupId");

            return groupId != null && !string.IsNullOrEmpty(groupId.value);
        }

        public static string GroupId(Blackboard inboxInfo)
        {
            return inboxInfo.GetVariable<string>("groupId")?.value ?? string.Empty;
        }

        public static InboxCellController FindCellController(int inboxId)
        {
            var cells = GameObject.FindObjectsOfType<InboxCellController>();
            return cells.ToList().FirstOrDefault(c => c.InboxInfo.GetValue<int>("id") == inboxId);
        }

        public static List<InboxGroup> GroupingInboxInfos(List<Blackboard> inboxInfos)
        {
            var groupList = new List<InboxGroup>();
            var ids = new HashSet<string>();
            var indexer = new Dictionary<string, int>();

            foreach (var info in inboxInfos)
            {
                if (info.GetValue<InboxTypes>("type") == InboxTypes.UNKNOWN) continue;

                string groupId = GroupId(info);
                if (string.IsNullOrEmpty(groupId))
                {
                    var group = new InboxGroup();
                    group.Add(info);
                    groupList.Add(group);
                }
                else
                {
                    if (ids.Add(groupId)) // true is group head
                    {
                        var group = new InboxGroup();
                        group.Add(info);
                        indexer[groupId] = groupList.Count();
                        groupList.Add(group);
                    }
                    else
                    {
                        groupList[indexer[groupId]].Add(info);
                    }
                }
            }

            return groupList;
        }
    }
}
