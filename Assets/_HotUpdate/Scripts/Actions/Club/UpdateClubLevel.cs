using System;
using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Club")]

public class UpdateClubLevel : ActionTask<Blackboard>
{
    public BBParameter<string> clubInfo;

    protected override string info
    {
        get { return "Update Club Level"; }
    }

    protected override void OnExecute()
    {
		var clubInfoBB = BlackboardUtils.FindVariable<Blackboard>(agent, clubInfo.value);

		if (clubInfoBB != null && clubInfoBB.value != null)
		{
			string clubLevelKey = string.Format("ClubLevel:{0}", clubInfoBB.value.GetValue<long>("id"));
			int clubLevel = clubInfoBB.value.GetValue<int>("level");

			PlayerPrefs.SetInt(clubLevelKey, clubLevel);
		}
		
        EndAction();
    }
}

}
