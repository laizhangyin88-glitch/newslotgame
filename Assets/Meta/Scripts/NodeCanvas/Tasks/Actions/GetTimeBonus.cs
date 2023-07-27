using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Utils")]
    public class GetTimeBonus : ActionTask<Blackboard>
	{
		public BBParameter<string> level;
		public BBParameter<string> tier;
		public BBParameter<long> eventNumerator;

		public BBParameter<long> saveAs;

		protected override string info
		{
			get { return string.Format("{0} = TimeBonus", saveAs); }
		}

		protected override void OnExecute()
		{
			var _tier = BlackboardUtils.FindVariable<int>(agent, tier.value);
			var _level = BlackboardUtils.FindVariable<int>(agent, level.value);
			var _baseCredit = BlackboardUtils.FindVariable<long>(agent, "/values/timeBonus/BASE_CREDIT");
			var _bonusLevelMultiplier = BlackboardUtils.FindVariable<int>(agent, "/values/timeBonus/BONUS_LEVEL_MULTIPLIER");
			var _specialBonusLevelFactor = BlackboardUtils.FindVariable<int>(agent, "/values/timeBonus/SPECIAL_BONUS_LEVEL_FACTOR");
			var _specialBounsCredit = BlackboardUtils.FindVariable<int>(agent, "/values/timeBonus/SPECIAL_BONUS_CREDIT");

			if (_level == null || _baseCredit == null || _bonusLevelMultiplier == null || _specialBonusLevelFactor == null || _specialBounsCredit == null)
			{
				Debug.LogError("TimeBonus Failed");
				EndAction(false);
			}
			else
			{
				long tierBaseCredit = TierUtils.GetTierFractionCoin(_baseCredit.value, _tier.value);
				tierBaseCredit = FreebieLevelUtils.GetLevelMultiplierNumeratorValue(tierBaseCredit, FreebieLevelUtils.FreebieType.TIME_BONUS);
				long tempLong;
				if (_specialBonusLevelFactor.value != 0)
				{
					tempLong = (_bonusLevelMultiplier.value * (_level.value - 1)) + ((_level.value / _specialBonusLevelFactor.value) * _specialBounsCredit.value);
				}
				else
				{
					tempLong = 0;
				}

				saveAs.value = NumberUtils.GetMultiplierNumeratorValue(tierBaseCredit + tempLong, eventNumerator.value);
				saveAs.value = NumberUtils.GetMultiplierNumeratorValue(saveAs.value, BlackboardQueryUtils.GetVIPLoungeClubVegasRewardActiveNumerator("LOBBY_BONUS"));
				EndAction(true);
			}
		}
	}
}