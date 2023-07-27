using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using BagelCode.ClientModels;
using System;

namespace BagelCode
{

public class MysteryTest : MonoBehaviour
{
#if UNITY_EDITOR

	private const string ON_SPINBUTTON_EVENT = "OnSpinButtonEvent";
	private const int REWARD_COUNT_LIMIT = 3;

	[Serializable]
	public class RewardInfo
	{
		public string name = "";
		public RewardType rewardType = RewardType.UNKNOWN;
		public long credit = 0L;
		public long rp = 0L;
		public int spinCount = 0;
		public GameTitle gameTitle = GameTitle.UNKNOWN;
		public string bet = "";
		public int totalDay = 0;
		public int targetTier = 0;
	}

	public enum GameTitle
	{
		UNKNOWN = 666,
		JungleQuest = 1,
		BillionaireBeauty = 2,
		WildCity = 3,
		RedWhiteBlueStacked = 4,
		SuperReSpin = 5,
		DragonsRoar = 6,
		MajesticFortune = 7,
		TropicalDreams = 8,
		AztecCharms = 9
	}

	[HideInInspector]
	public int rewardListCount = 0;

	public List<RewardInfo> rewardList = new List<RewardInfo>();
	public int targetLevel = 0;
	public RewardType rewardType = RewardType.UNKNOWN;

	public void AddItem()
	{
		if(rewardListCount >= REWARD_COUNT_LIMIT){ return; }
		UpdateRewardList(rewardList, rewardType);
    }

	public void ResetItem()
	{
		rewardList.Clear();
	}

	public void SpinButton()
	{
		if(rewardListCount == 0){return;}

        var level = BlackboardUtils.FindVariable<int>(null, "/me/level");
        level.value = 0;

		BlackboardUtils.DestroyBlackboard(MainBlackboard.Get(), "mysteryGiftTestInfo");
		var bb = (Blackboard)BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "mysteryGiftTestInfo");
		SerializeMysteryGift(bb, rewardList);

		var e = new EventData(ON_SPINBUTTON_EVENT);
		MessageDispatcher.Dispatch(ON_SPINBUTTON_EVENT, e);
		return;
	}

    private void UpdateRewardList(List<RewardInfo> rewardList, RewardType type)
    {
        RewardInfo rewardInfo = null;
        switch(type)
        {
            case RewardType.CREDIT:
            case RewardType.CREDIT_WITH_MULTIPLIER:
                rewardInfo = new RewardInfo();
                rewardInfo.name = "Credit";
                rewardInfo.rewardType = type;
                rewardInfo.credit = 1000000L;
                break;

            case RewardType.RP:
                rewardInfo = new RewardInfo();
                rewardInfo.name = "RP";
                rewardInfo.rewardType = type;
                rewardInfo.rp = 10000L;
                break;

            case RewardType.DAILY_BONUS_WHEEL_SPIN:
                rewardInfo = new RewardInfo();
                rewardInfo.name = "DailySpin";
                rewardInfo.rewardType = type;
                rewardInfo.spinCount = 10;
                break;

        	case RewardType.GAME_SPIN:
                rewardInfo = new RewardInfo();
                rewardInfo.name = "GameSpin";
                rewardInfo.rewardType = type;
				rewardInfo.gameTitle = GameTitle.JungleQuest;
                rewardInfo.spinCount = 10;
                rewardInfo.bet = "350000";
                break;

        	case RewardType.DAILY_BOOST:
                rewardInfo = new RewardInfo();
                rewardInfo.name = "DailyBoost";
                rewardInfo.rewardType = type;
                rewardInfo.credit = 1000000L;
                rewardInfo.totalDay = 10;
                break;

        	case RewardType.TIER_UPGRADE:
                rewardInfo = new RewardInfo();
                rewardInfo.name = "TierUpgrade";
                rewardInfo.rewardType = type;
                rewardInfo.targetTier = 15;
                break;

            default :
                break;
        }
        if(rewardInfo != null)
        {
	        rewardList.Add(rewardInfo);
        }
    }

    private void SerializeMysteryGift(Blackboard bb, List<RewardInfo> rewardList)
    {
		BlackboardUtils.SetOrCreateValue<int>(bb, "level", targetLevel);

		for(int i = 0; i < rewardList.Count; i++)
		{
			var rewardResultBB = (Blackboard)BlackboardUtils.CreateBlackboard("rewardResult");
			BlackboardUtils.AddToBlackboardList(bb, "rewardResultList", rewardResultBB);

			SerializeRewardResult(rewardResultBB, rewardList[i]);
		}
    }

    private void SerializeRewardResult(Blackboard bb, RewardInfo rewardInfo)
    {
		RewardType rewardType = rewardInfo.rewardType;
		Blackboard inboxBB;
		Blackboard inboxRewardBB;

		BlackboardUtils.SetOrCreateValue<RewardType>(bb, "rewardType", rewardInfo.rewardType);

        switch(rewardType)
        {
            case RewardType.CREDIT:
            case RewardType.CREDIT_WITH_MULTIPLIER:
				BlackboardUtils.SetOrCreateValue<long>(bb, "credit", rewardInfo.credit);
				break;

            case RewardType.RP:
	            inboxBB = (Blackboard)BlackboardUtils.GetOrCreateBlackboard(bb, "inbox");
	            inboxRewardBB = (Blackboard)BlackboardUtils.GetOrCreateBlackboard(inboxBB, "reward");
	            BlackboardUtils.SetOrCreateValue<long>(inboxRewardBB, "rp", rewardInfo.rp);
            	break;

            case RewardType.DAILY_BONUS_WHEEL_SPIN:
				BlackboardUtils.SetOrCreateValue<int>(bb, "addedSpinCount", rewardInfo.spinCount);
				BlackboardUtils.SetOrCreateValue<int>(bb, "totalSpinCount", rewardInfo.spinCount);
            	break;

        	case RewardType.GAME_SPIN:
	            inboxBB = (Blackboard)BlackboardUtils.GetOrCreateBlackboard(bb, "inbox");
	            inboxRewardBB = (Blackboard)BlackboardUtils.GetOrCreateBlackboard(inboxBB, "reward");
	            BlackboardUtils.SetOrCreateValue<int>(inboxRewardBB, "gameId", (int)rewardInfo.gameTitle);
	            BlackboardUtils.SetOrCreateValue<string>(inboxRewardBB, "betZoneId", rewardInfo.bet);
	            BlackboardUtils.SetOrCreateValue<int>(inboxRewardBB, "spinCount", rewardInfo.spinCount);
	            break;

        	case RewardType.DAILY_BOOST:
	            inboxBB = (Blackboard)BlackboardUtils.GetOrCreateBlackboard(bb, "inbox");
	            inboxRewardBB = (Blackboard)BlackboardUtils.GetOrCreateBlackboard(inboxBB, "reward");
	            BlackboardUtils.SetOrCreateValue<long>(inboxRewardBB, "baseCreditPerDay", rewardInfo.credit);
	            BlackboardUtils.SetOrCreateValue<int>(inboxRewardBB, "totalDayCount", rewardInfo.totalDay);
	            break;

        	case RewardType.TIER_UPGRADE:
	            inboxBB = (Blackboard)BlackboardUtils.GetOrCreateBlackboard(bb, "inbox");
	            inboxRewardBB = (Blackboard)BlackboardUtils.GetOrCreateBlackboard(inboxBB, "reward");
	            BlackboardUtils.SetOrCreateValue<int>(inboxRewardBB, "targetTier", (int)rewardInfo.targetTier);
	            break;

            default :
                break;
        }
    }

#endif
}

}
