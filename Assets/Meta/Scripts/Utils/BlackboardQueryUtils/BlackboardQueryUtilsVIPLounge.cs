using SlotMaker;
using BagelCode.ClientModels;
using ParadoxNotion;
using UnityEngine;
using NodeCanvas.Framework;

namespace BagelCode
{
    public static partial class BlackboardQueryUtils
    {
        public static void SetVipLoungeEnabled(bool state)
        {
            var activeVar = BlackboardUtils.GetOrCreateVariable<bool>(VipLounge.VipLounge.Utils.VipLoungeInfo, "active");
            var activeForUserVar = BlackboardUtils.GetOrCreateVariable<bool>(VipLounge.VipLounge.Utils.VipLoungeInfo, "activeForUser");

            activeVar.value = state;
            if (activeForUserVar.value != state)
            {
                activeForUserVar.value = state;
                var eventData = new EventData<bool>(VipLounge.VipLounge.Events.ON_UPDATE_VIP_LOUNGE_INFO, state);
                EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, eventData);
            }
        }

        public static bool IsVipLoungeActive()
        {
            bool isActive = false;
            if (IsActiveMetaGameEvent(EventInfoType.VIP_LOUNGE))
                isActive = true;
            else
            {
                if (GetMetaGameEventInfo(EventInfoType.VIP_LOUNGE, true) != null)
                {
                    isActive = false;
                    SetVipLoungeEnabled(false);
                }
                else
                    isActive = BlackboardUtils.GetOrCreateVariable<bool>("/vipLoungeInfo/active")?.value ?? false;
            }
                
            return isActive;
        }

        public static bool IsVipLoungeActiveForUser()
        {
            return BlackboardUtils.GetOrCreateVariable<bool>("/vipLoungeInfo/activeForUser")?.value ?? false;
        }

        public static bool IsVipLoungeEnabled() // check active & active for user
        {
            return IsVipLoungeActiveForUser() && IsVipLoungeActive();
        }

        public static bool HasVipLoungeBadge()
        {
            return VipLounge.VipLounge.Utils.BadgeCount > 0;
        }

        public static bool IsVipLoungeEndedTimestamp()
        {
            EventInfo eventInfo = GetMetaGameEventInfo(EventInfoType.VIP_LOUNGE, true);
            if (eventInfo != null)
                return TimeUtils.GetTimeStamp() > eventInfo.endTimestamp;
            return false;
        }

        public static bool IsVipLoungeActiveBenefit()
        {
            return IsVipLoungeEnabled() && HasVipLoungeBadge() && !IsVipLoungeEndedTimestamp();
        }

        public static long GetVIPLoungeClubVegasRewardNumerator(string key = "REWARD_INCREASE_PERCENTAGE")
        {
            Blackboard benefitInfoBB = BlackboardUtils.FindVariable<Blackboard>(VipLounge.VipLounge.Utils.VipLoungeInfo, "benefitInfo")?.value ?? null;
            if (benefitInfoBB != null)
            {
                return BlackboardUtils.FindVariable<long>(benefitInfoBB, key)?.value ?? NumberUtils.GetGlobalDenominator();
            }
            return NumberUtils.GetGlobalDenominator();
        }

        public static long GetVIPLoungeClubVegasRewardAdditionalPercent()
        {
            return NumberUtils.GetAdditionalPercent(GetVIPLoungeClubVegasRewardNumerator());
        }

        public static long GetVIPLoungeClubVegasRewardActiveNumerator(string key)
        {
            if (VipLounge.VipLounge.Utils.IsEnded)
                return NumberUtils.GetGlobalDenominator();
            else
                return GetVIPLoungeClubVegasRewardNumerator(key);
        }

        public static void UpdateVIPLoungeInfo(Blackboard updateInfoBB)
        {
            if (updateInfoBB == null) return;

            VipLoungeInfo updateVipLoungeInfo = new VipLoungeInfo();
            updateVipLoungeInfo.active = updateInfoBB.GetValue<bool>("active");
            updateVipLoungeInfo.activeForUser = updateInfoBB.GetValue<bool>("activeForUser");
            updateVipLoungeInfo.loungePoint = updateInfoBB.GetValue<long>("loungePoint");
            updateVipLoungeInfo.excessLoungePoint = updateInfoBB.GetValue<long>("excessLoungePoint");
            updateVipLoungeInfo.gaugeMax = updateInfoBB.GetValue<long>("gaugeMax");
            updateVipLoungeInfo.extendedBetIndex = updateInfoBB.GetValue<int>("extendedBetIndex");
            updateVipLoungeInfo.benefitEndTimestamp = updateInfoBB.GetValue<long>("benefitEndTimestamp");
            updateVipLoungeInfo.badgeCount = updateInfoBB.GetValue<int>("badgeCount");
            if (updateInfoBB.GetVariable<Blackboard>("benefitInfo")?.value ?? null != null)
            {
                updateVipLoungeInfo.benefitInfo = new VipLoungeBenefitInfo();
                updateVipLoungeInfo.benefitInfo.FRIENDS_BONUS = BlackboardUtils.FindValue<long>(updateInfoBB.GetValue<Blackboard>("benefitInfo"), "FRIENDS_BONUS");
                updateVipLoungeInfo.benefitInfo.SHOP_BONUS = BlackboardUtils.FindValue<long>(updateInfoBB.GetValue<Blackboard>("benefitInfo"), "SHOP_BONUS");
                updateVipLoungeInfo.benefitInfo.LOBBY_BONUS = BlackboardUtils.FindValue<long>(updateInfoBB.GetValue<Blackboard>("benefitInfo"), "LOBBY_BONUS");
                updateVipLoungeInfo.benefitInfo.LP_BOOST = BlackboardUtils.FindValue<long>(updateInfoBB.GetValue<Blackboard>("benefitInfo"), "LP_BOOST");
            }
            else
            {
                updateVipLoungeInfo.benefitInfo = null;
            }

            UpdateVIPLoungeInfo(updateVipLoungeInfo);
        }

        public static void SetBadgeCount(int count)
        {
            Blackboard vipLoungeInfoBB = VipLounge.VipLounge.Utils.VipLoungeInfo;
            if (vipLoungeInfoBB == null) return;

            BlackboardUtils.SetOrCreateValue(vipLoungeInfoBB, "badgeCount", count);
        }

        public static void UpdateVIPLoungeInfo(VipLoungeInfo vipLoungeInfo)
        {
            if (vipLoungeInfo == null) return;

            bool inVipLoungeInfoEmpty = VipLounge.VipLounge.Utils.IsVipLoungeInfoNull();
            Blackboard vipLoungeInfoBB = VipLounge.VipLounge.Utils.VipLoungeInfo;

            ClientAPI2Blackboard.Serialize(vipLoungeInfoBB, vipLoungeInfo);

            if (inVipLoungeInfoEmpty)
            {
                BlackboardUtils.SetOrCreateValue(vipLoungeInfoBB, VipLounge.VipLounge.Defines.PREV_BADGE_COUNT_KEY, 0);
            }

#if DEV
            int badgeCountPlayerPrefsValue = PlayerPrefs.GetInt(VipLounge.VipLounge.Defines.PLAYER_PREFS_DEBUG_STATIC_BADGE);
            if (PlayerPrefs.GetInt(VipLounge.VipLounge.Defines.PLAYER_PREFS_DEBUG_EARN_BADGE) == 1)
            {
                BlackboardUtils.SetOrCreateValue(vipLoungeInfoBB, "badgeCount", 1);
            }
            else if (badgeCountPlayerPrefsValue > 0)
            {
                BlackboardUtils.SetOrCreateValue(vipLoungeInfoBB, "badgeCount", badgeCountPlayerPrefsValue - 1);
            }
#endif

            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, VipLounge.VipLounge.Events.ON_UPDATE_VIP_LOUNGE_INFO);
        }

        public static void CreateVIPLoungePrevBadgeCount()
        {
            Variable<int> prevBadgeCount = VipLounge.VipLounge.Utils.VipLoungeInfo.GetVariable<int>(VipLounge.VipLounge.Defines.PREV_BADGE_COUNT_KEY);
            if (prevBadgeCount == null)
                BlackboardUtils.SetOrCreateValue(VipLounge.VipLounge.Utils.VipLoungeInfo, VipLounge.VipLounge.Defines.PREV_BADGE_COUNT_KEY, VipLounge.VipLounge.Utils.BadgeCount);
        }

        public static bool IsVipLoungeBadgeEarned(bool updatePrev = true)
        {
#if DEV
            if (PlayerPrefs.GetInt(VipLounge.VipLounge.Defines.PLAYER_PREFS_DEBUG_EARN_BADGE) == 1)
            {
                return true;
            }
#endif

            Blackboard vipLoungeInfoBB = VipLounge.VipLounge.Utils.VipLoungeInfo;
            if (vipLoungeInfoBB == null) return false;

            int prevBadgeCount = VipLounge.VipLounge.Utils.PrevBadgeCount;
            int badgeCount = VipLounge.VipLounge.Utils.BadgeCount;

            if (updatePrev)
                BlackboardUtils.SetOrCreateValue(vipLoungeInfoBB, VipLounge.VipLounge.Defines.PREV_BADGE_COUNT_KEY, badgeCount);

            return prevBadgeCount < badgeCount;
        }

        public static void UpdateResponseVIPLounge(Error error, CommonResponse common, long serverTime, VipLoungeInfo vipLoungeInfo)
        {
            var bb = MainBlackboard.Get();
            BlackboardUtils.SetOrCreateValue(bb, "error", error);
            BlackboardUtils.SetOrCreateValue(bb, "serverTime", serverTime);
            if (common != null)
                ClientAPI2Blackboard.Serialize(BlackboardUtils.GetOrCreateBlackboard(bb, "common"), common);
            if (vipLoungeInfo != null && VipLounge.VipLounge.Utils.VipLoungeInfo != null)
                ClientAPI2Blackboard.Serialize(VipLounge.VipLounge.Utils.VipLoungeInfo, vipLoungeInfo);
        }

        public static string GetTimeStampToTimeString(long timestamp)
        {
            if (timestamp > 0L)
            {
                if (timestamp >= TimeUtils.ONE_DAY_MS)
                {
                    int days = (int)(timestamp / TimeUtils.ONE_DAY_MS);
                    return StringTableUtils.GetString(StringTable.StringTableType.Global, "VIP_LOUNGE_COOLTIME_TO_DAY", days);
                }
                else if (timestamp >= TimeUtils.ONE_HOUR_MS)
                {
                    int hours = (int)(timestamp / TimeUtils.ONE_HOUR_MS);
                    return StringTableUtils.GetString(StringTable.StringTableType.Global, "VIP_LOUNGE_COOLTIME_TO_HOUR", hours);
                }
                else
                {
                    int minutes = (int)(timestamp / TimeUtils.ONE_MIN_MS);
                    return StringTableUtils.GetString(StringTable.StringTableType.Global, "VIP_LOUNGE_COOLTIME_TO_MINUTE", minutes);
                }
            }
            else
            {
                return "";
            }
        }
    }
}
