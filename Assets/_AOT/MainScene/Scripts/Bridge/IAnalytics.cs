using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
	public interface IAnalytics
	{
		void CustomEvent(string eventName, Dictionary<string, object> eventData);
		void buyin(int gameId, string productId);
		void spin(int gameId, long betCredit, long earnCredit, bool freeSpin, bool autoSpin);
		void spin_funnel(int gameId);
		void bonus(int gameId, long betCredit, int bonusId, long earnCredit);
		void freespin_trigger(int gameId, long betCredit, int bonusId, int totalSpinCount, int spinCount, int addedSpinCount);
		void jackpot(int gameId, long betCredit, long earnCredit, int jackpotType);
		void big_win(int gameId, long betCredit, long earnCredit, int bigWinType, bool freeSpin);
		void big_win_gamble(int gameId, long betCredit, long earnCredit, int bigWinType, bool freeSpin);
		void keno_pick(int gameId, long baseBet, long extraBet, int ticketCount, int pickType, string pickedNumber, int markedCount);
		void keno_click_auto_change(int gameId, long baseBet, long extraBet, int ticketCount, bool isAutoQuickPick);
	}
}