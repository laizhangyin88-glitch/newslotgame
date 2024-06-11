using NodeCanvas.Framework;
using SlotMaker;
using SlotMaker.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class V3Analytics : IAnalytics
    {
        public void CustomEvent(string eventName, Dictionary<string, object> eventData)
        {
            var buffer = new Dictionary<string, object>();
            if (!BiEventUtils.AppendCommonEventData(eventName, buffer)) return;

            if (eventData != null)
            {
                // override
                foreach (var pair in eventData)
                {
                    buffer[pair.Key] = pair.Value;
                }
            }

            BiEventSender.SendBiEvent(SlotSimpleJson.SerializeObject(buffer) + '\n');
        }

        public void buyin(int gameId, string productId)
        {
            CustomEvent("client_buyin", new Dictionary<string, object>
            {
                { "game_id", gameId },
                { "bet_zone_id", "free" },
                { "product_id", Convert.ToInt64(productId) }
            });
        }

        public void spin(int gameId, long betCredit, long earnCredit, bool freeSpin, bool autoSpin)
        {
            var customData = new Dictionary<string, object>();

            int totalSpinCount = BlackboardUtils.FindValue<int>("./turn/spin/response/userSyncInfo/totalSpinCount") - 1;
            var spinType = BlackboardUtils.FindValue<SpinType>("./spinType");

            customData["game_id"] = gameId;
            customData["bet_zone_id"] = "free";
            customData["bet"] = betCredit;
            customData["win"] = earnCredit;
            customData["total_spin_count"] = totalSpinCount;
            customData["is_freespin"] = freeSpin;
            customData["is_auto_spin"] = autoSpin;
            customData["type"] = ToString(spinType);
            customData["is_early_access"] = (spinType == SpinType.BonusSpin);

            switch (spinType)
            {
                case SpinType.GameSpin:
                    {
                        var gameSpinDict = BlackboardUtils.FindValue<Dictionary<long, int>>(ContentBlackboard.Get(), "gameSpinCountPerBet");
                        if (gameSpinDict.ContainsKey(betCredit))
                            customData["remain_game_spin_count"] = gameSpinDict[betCredit] + 1;
                    }
                    break;

                case SpinType.BonusSpin:
                    {
                        var bonusSpinDict = BlackboardUtils.FindValue<Dictionary<long, int>>(ContentBlackboard.Get(), "bonusSpinCountPerBet");
                        if (bonusSpinDict.ContainsKey(betCredit))
                            customData["remain_bonus_spin_count"] = bonusSpinDict[betCredit] + 1;
                    }
                    break;
            }

            CustomEvent("client_spin", customData);
        }

        public void spin_funnel(int gameId)
        {
            var me = BlackboardUtils.FindVariable<Blackboard>(null, "/me").value;
            var funnelSpinCountList = BlackboardUtils.FindVariable<List<int>>(MainBlackboard.Get(), "/values/misc/FUNNEL_SPIN_COUNT_LIST");
            var content = ContentBlackboard.Get();
            var game = content.GetValue<Blackboard>("game");
            var totalSpinCount = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "/userSyncInfo/totalSpinCount");

            var loginCount = BlackboardUtils.FindVariable<int>(null, "/me/loginCount");
            long funnel_spin_count = System.Convert.ToInt64(PlayerPrefs.GetString("funnel_spin_count", "0"));

            bool isGameSpin = false;
            if (BlackboardQueryUtils.GetGameSpinEnabled())
            {
                var isGameSpinVar = BlackboardUtils.FindVariable<bool>(null, "./isGameSpin");

                if (isGameSpinVar != null)
                    isGameSpin = isGameSpinVar.value;
            }
            if (isGameSpin)
                return;

            if (loginCount.value == PlayerPrefs.GetInt("login_count", 0))
            {
                funnel_spin_count += 1L;
                PlayerPrefs.SetString("funnel_spin_count", funnel_spin_count.ToString());
            }
            else
            {
                PlayerPrefs.SetString("funnel_spin_count", "1");
                funnel_spin_count = 1L;
                PlayerPrefs.SetInt("login_count", loginCount.value);
            }

            for (int i = 0; i < funnelSpinCountList.value.Count; i++)
            {
                if (totalSpinCount.value == funnelSpinCountList.value[i])
                {
                    string adjustEventId = string.Format("spin_funnel:{0}", BiEventUtils.AddZeroPadding(3, totalSpinCount.value));
                    AdjustManager.Instance.SendEvent(adjustEventId);
                }

                if (funnelSpinCountList.value[i] == Convert.ToInt32(funnel_spin_count))
                {
                    CustomEvent("client_spin_funnel", new Dictionary<string, object>
                    {
                        { "game_id", gameId },
                        { "bet_zone_id", "free" },
                        { "spin_count", funnel_spin_count }
                    });
                }
            }
        }

        public void bonus(int gameId, long betCredit, int bonusId, long earnCredit)
        {
            var customData = new Dictionary<string, object>();

            var spinType = BlackboardUtils.FindValue<SpinType>("./spinType");

            customData["game_id"] = gameId;
            customData["bet_zone_id"] = "free";
            customData["bet"] = betCredit;
            customData["bonus_id"] = bonusId;
            customData["win"] = earnCredit;
            customData["type"] = ToString(spinType);

            CustomEvent("client_bonus", customData);
        }

        public void freespin_trigger(int gameId, long betCredit, int bonusId, int totalSpinCount, int spinCount, int addedSpinCount)
        {
            var customData = new Dictionary<string, object>();

            var spinType = BlackboardUtils.FindValue<SpinType>("./spinType");

            customData["game_id"] = gameId;
            customData["bet_zone_id"] = "free";
            customData["bet"] = betCredit;
            customData["bonus_id"] = bonusId;
            customData["earn_freespin_count"] = addedSpinCount;
            customData["remain_freespin_count"] = totalSpinCount - addedSpinCount - spinCount;
            customData["total_freespin_count"] = totalSpinCount - addedSpinCount;
            customData["type"] = ToString(spinType);

            CustomEvent("client_freespin_trigger", customData);
        }

        public void jackpot(int gameId, long betCredit, long earnCredit, int jackpotType)
        {
            var customData = new Dictionary<string, object>();

            var spinType = BlackboardUtils.FindValue<SpinType>("./spinType");

            customData["game_id"] = gameId;
            customData["bet_zone_id"] = "free";
            customData["bet"] = betCredit;
            customData["win"] = earnCredit;
            customData["jackpot_index"] = jackpotType;
            customData["type"] = ToString(spinType);

            CustomEvent("client_jackpot", customData);
        }

        public void big_win(int gameId, long betCredit, long earnCredit, int bigWinType, bool freeSpin)
        {
            var customData = new Dictionary<string, object>();

            var spinType = BlackboardUtils.FindValue<SpinType>("./spinType");

            customData["game_id"] = gameId;
            customData["bet_zone_id"] = "free";
            customData["bet"] = betCredit;
            customData["win"] = earnCredit;
            customData["type"] = ToString(bigWinType);
            customData["spin_type"] = ToString(spinType);
            customData["is_freespin"] = freeSpin;

            customData["is_gamble"] = false;

            CustomEvent("client_big_win", customData);
        }

        public void big_win_gamble(int gameId, long betCredit, long earnCredit, int bigWinType, bool freeSpin)
        {
            var customData = new Dictionary<string, object>();

            var spinType = BlackboardUtils.FindValue<SpinType>("./spinType");

            customData["game_id"] = gameId;
            customData["bet_zone_id"] = "free";
            customData["bet"] = betCredit;
            customData["win"] = earnCredit;
            customData["type"] = ToString(bigWinType);
            customData["spin_type"] = ToString(spinType);
            customData["is_freespin"] = freeSpin;

            customData["is_gamble"] = true;

            CustomEvent("client_big_win", customData);
        }

        private static readonly string[] KENO_PICK_TYPES = new string[] { "mark", "erase", "quick_pick" };

        public void keno_pick(int gameId, long baseBet, long extraBet, int ticketCount, int pickType, string pickedNumber, int markedCount)
        {
            Analytics.CustomEvent("client_pick", new Dictionary<string, object>
            {
                { "game_id", gameId },
                { "bet_per_ticket", baseBet },
                { "extra_bet_per_ticket", extraBet },
                { "ticket_count", ticketCount },
                { "slot_enter_context_id", BiEventUtils.GetSlotEnterContextID() },
                { "type", KENO_PICK_TYPES[pickType] },
                { "picked_number", pickedNumber },
                { "marked_count", markedCount },
            });
        }

        public void keno_click_auto_change(int gameId, long baseBet, long extraBet, int ticketCount, bool isAutoQuickPick)
        {
            Analytics.CustomEvent("client_click_auto_change", new Dictionary<string, object>
            {
                { "game_id", gameId },
                { "bet_per_ticket", baseBet },
                { "extra_bet_per_ticket", extraBet },
                { "ticket_count", ticketCount },
                { "slot_enter_context_id", BiEventUtils.GetSlotEnterContextID() },
                { "type", isAutoQuickPick },
            });
        }

        private string ToString(SpinType spinType)
        {
            switch (spinType)
            {
                case SpinType.GameSpin:
                    return "game_spin";

                case SpinType.BonusSpin:
                    return "bonus_spin";

                case SpinType.BuyABonus:
                    return "buy_bonus";
            }

            return "default";
        }

        private static readonly string[] BIG_WIN_TYPES = new string[] { "big", "superbig", "mega", "supermega", "epic" };

        private string ToString(int bigWinType)
        {
            return BIG_WIN_TYPES[bigWinType];
        }
    }
}
