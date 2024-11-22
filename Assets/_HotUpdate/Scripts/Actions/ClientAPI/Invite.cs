using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using SlotMaker.Json;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class Invite : ActionTask<Blackboard>
{
	public BBParameter<string> invitableList;
	public BBParameter<int> type;

	private static string key = "INVITED_USER_ID_DICT";

	protected override string info { get { return "Request Invite"; } }

	protected override void OnExecute()
	{
		var userList = BlackboardUtils.FindVariable<List<string>>(agent, invitableList.value);

		if(userList != null && userList.value.Count != 0)
		{
			InviteType inviteType = GetInviteType(type.value);

			if (inviteType != InviteType.UNKNOWN)
			{
				BagelCodeClientAPI.Invite((InviteType)(type.value + 1), userList.value, (response) => {}, (error) => {} ); // Async
				SaveInvitableList(key, userList.value);

				EndAction(true);
				return;
			}
		}

		EndAction(false);
	}

	private InviteType GetInviteType(int type)
	{
		switch(type)
		{
			case 0:
				return InviteType.FRIEND;
			case 1:
				return InviteType.CLUB;
			default:
				return InviteType.UNKNOWN;
		}
	}

	private void SaveInvitableList(string key, List<string> userList)
	{
		Dictionary<string, long> invitedInfoDict;
		string json = PlayerPrefs.GetString(key, "");
		if (!string.IsNullOrEmpty(json))
		{
	        invitedInfoDict = (Dictionary<string, long>)SlotSimpleJson.DeserializeObject(json, typeof(Dictionary<string, long>));
		}
		else
		{
			invitedInfoDict = new Dictionary<string, long>();
		}

		var restrictionSec = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "/values/misc/FRIEND_INVITE_RESTRICTION_SEC");
		long targetTime = BagelCode.TimeUtils.GetTimeStamp() + ((long)restrictionSec.value * 1000L);
		for(int i = 0; i < userList.Count;i++)
		{
			invitedInfoDict[userList[i]] = targetTime;
		}

		PlayerPrefs.SetString(key, SlotSimpleJson.SerializeObject(invitedInfoDict));
	}
}

}
