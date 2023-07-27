using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;

namespace BagelCode {

    public static partial class BlackboardQueryUtils
    {
        public static void UpdateUserSyncInfo(UserSyncInfo userSyncInfo, long serverTimestamp)
        {
            if(userSyncInfo != null)
            {
                var userSyncInfoBB = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "userSyncInfo");

                var timestamp = userSyncInfoBB.GetVariable<long>("serverTime");

                if(timestamp == null || timestamp.value < serverTimestamp)
                {
                    ClientAPI2Blackboard.Serialize(userSyncInfoBB, userSyncInfo);
                    BlackboardUtils.SetOrCreateValue( userSyncInfoBB, "serverTime", serverTimestamp);
                    BlackboardUtils.SetOrCreateValue( userSyncInfoBB, "isApplied", false);
                }
            }
        }

        public static void ApplyUserSyncInfo(bool isApply = true)
        {
            var mainBB = MainBlackboard.Get();
            var userSyncInfoBB = mainBB.GetVariable<Blackboard>("userSyncInfo");

            if(userSyncInfoBB != null && userSyncInfoBB.value != null)
            {
                if(isApply)
                {
                    var isApplied = BlackboardUtils.GetOrCreateVariable<bool>( userSyncInfoBB.value, "isApplied");
                    if( !isApplied.value )
                    {
                        var timestamp = userSyncInfoBB.value.GetVariable<long>("serverTime");
                        if(timestamp != null && timestamp.value > 0)
                        {
                            var meBB = mainBB.GetVariable<Blackboard>("me");

                            meBB.value.SetValue("credit",           userSyncInfoBB.value.GetValue<long>("credit"));
                            meBB.value.SetValue("requiredExp",      userSyncInfoBB.value.GetValue<long>("requiredExp"));
                            meBB.value.SetValue("requiredExpMax",   userSyncInfoBB.value.GetValue<long>("requiredExpMax"));
                            meBB.value.SetValue("level",            userSyncInfoBB.value.GetValue<int>("level"));
                            meBB.value.SetValue("rp",               userSyncInfoBB.value.GetValue<long>("rp"));
                            meBB.value.SetValue("gem",              userSyncInfoBB.value.GetValue<long>("gem"));

                            UpdateGlobalChatSpeaker(userSyncInfoBB.value.GetValue<int>("speaker"), timestamp.value);

                            long accRP = userSyncInfoBB.value.GetValue<long>("accRp");
                            meBB.value.SetValue("accRp",            accRP);
                            meBB.value.SetValue("tier",             TierUtils.GetTier(accRP));
                            meBB.value.SetValue("piggyCredit",      userSyncInfoBB.value.GetValue<long>("piggyCredit"));

                        }
                        MessageDispatcher.Dispatch("OnCreditEvent", new EventData<bool>("UpdateNaviCredit", true));
                        MessageDispatcher.Dispatch("OnCreditEvent", new EventData<bool>("UpdateNaviGem", true));
                    }

                    isApplied.value = true;
                }
                // BlackboardUtils.DestroyBlackboard(MainBlackboard.Get(), "userSyncInfo");
            }
        }
        public static void UpdateUserPurchase()
        {
            var mainBB = MainBlackboard.Get();
            var meBB = mainBB.GetVariable<Blackboard>("me");
            var purchaseCount = BlackboardUtils.FindVariable<int>(mainBB, "purchaseResponse/purchaseCount");
            var lifetimeSpend = BlackboardUtils.FindVariable<double>(mainBB, "purchaseResponse/lifetimeSpend");

            meBB.value.SetValue("purchaseCount", purchaseCount.value);
            meBB.value.SetValue("lifetimeSpend", lifetimeSpend.value);
        }
    }

}

