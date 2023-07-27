using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    public static class Analytics
    {
        private static IAnalytics analytics;

        public static void InitializeAnalytics(IAnalytics customAnalytics)
        {
            analytics = customAnalytics;
        }

        public static void CustomEvent(string eventName, Dictionary<string, object> eventData)
        {
            analytics.CustomEvent(eventName, eventData);
        }

        public static void buyin(int gameId, string productId)
        {
            analytics.buyin(gameId, productId);
        }

        public static void spin(int gameId, long betCredit, long earnCredit, bool freeSpin, bool autoSpin)
        {
            analytics.spin(gameId, betCredit, earnCredit, freeSpin, autoSpin);
        }

        public static void spin_funnel(int gameId)
        {
            analytics.spin_funnel(gameId);
        }

        public static void bonus(int gameId, long betCredit, int bonusId, long earnCredit)
        {
            analytics.bonus(gameId, betCredit, bonusId, earnCredit);
        }

        public static void freespin_trigger(int gameId, long betCredit, int bonusId, int totalSpinCount, int spinCount, int addedSpinCount)
        {
            analytics.freespin_trigger(gameId, betCredit, bonusId, totalSpinCount, spinCount, addedSpinCount);
        }

        public static void jackpot(int gameId, long betCredit, long earnCredit, int jackpotType)
        {
            analytics.jackpot(gameId, betCredit, earnCredit, jackpotType);
        }

        public static void big_win(int gameId, long betCredit, long earnCredit, int bigWinType, bool freeSpin)
        {
            analytics.big_win(gameId, betCredit, earnCredit, bigWinType, freeSpin);
        }

        public static void big_win_gamble(int gameId, long betCredit, long earnCredit, int bigWinType, bool freeSpin)
        {
            analytics.big_win_gamble(gameId, betCredit, earnCredit, bigWinType, freeSpin);
        }

        public static void keno_pick(int gameId, long baseBet, long extraBet, int ticketCount, int pickType, string pickedNumber, int markedCount)
        {
            analytics.keno_pick(gameId, baseBet, extraBet, ticketCount, pickType, pickedNumber, markedCount);
        }

        public static void keno_click_auto_change(int gameId, long baseBet, long extraBet, int ticketCount, bool isAutoQuickPick)
        {
            analytics.keno_click_auto_change(gameId, baseBet, extraBet, ticketCount, isAutoQuickPick);
        }
    }
}
