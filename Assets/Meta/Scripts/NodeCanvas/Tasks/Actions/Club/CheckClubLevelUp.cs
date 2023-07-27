using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Conditions
{
	[Category("★ BagelCode/Club")]
	public class CheckClubLevelUp : ConditionTask<Blackboard>
	{
		public BBParameter<string> clubInfo;

		protected override string info
		{
			get { return "Check Club Level Up"; }
		}

		protected override bool OnCheck()
		{
			var clubInfoBB = BlackboardUtils.FindVariable<Blackboard>(agent, clubInfo.value);
			if (clubInfoBB != null && clubInfoBB.value != null)
			{
                long clubId = clubInfoBB.value.GetValue<long>("id");

				int clubLevel = clubInfoBB.value.GetValue<int>("level");
                int lastLevel = ClubUtils.GetClubPlayerPrefs(ClubDefine.PLAYER_PREFS_CLUB_LEVEL, clubId);

				if (lastLevel != 0 && clubLevel > 1)
				{
					return clubLevel > lastLevel;
				}
			}
			return false;
		}
	}
}
