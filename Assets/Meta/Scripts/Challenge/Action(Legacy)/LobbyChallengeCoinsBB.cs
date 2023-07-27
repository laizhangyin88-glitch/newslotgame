using UnityEngine;
using System;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Challenge")]

public class LobbyChallengeCoinsBB : ActionTask<Blackboard>
{
    public BBParameter<long> saveNewCoins;
    public BBParameter<long> saveRewardCoins;

    protected override string info
    {
        get { return "Get Lobby Challenge Coin BB"; }
    }

    protected override void OnExecute()
    {
        saveRewardCoins.value = 0;

        long eventMultiplierNumerator = NumberUtils.GetGlobalDenominator();
        EventInfo eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.CHALLENGE_CLAIM_MULTIPLY);

        if(eventInfo != null)
            eventMultiplierNumerator = PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(eventInfo);

        var challengeInfoList = FindVariableWithException<List<Blackboard>>(MainBlackboard.Get(), "challengeInfoList");

        int timeZoneOffset = TimeUtils.GetTimeZoneOffset();
        long currentTimestamp = BagelCode.TimeUtils.GetTimeStamp();

        for(int i=0; i<challengeInfoList.value.Count; ++i)
        {
            if(challengeInfoList.value[i] == null)
            {
                throw new Exception(string.Format("NullReferenceException in LobbyChallengeCoinsBB: ChallengeInfoList index {0} is null", i));
            }

            var cType = FindVariableWithException<ChallengeType>(challengeInfoList.value[i], "challengeType");
            bool isDone = FindVariableWithException<bool>(challengeInfoList.value[i], "done").value;
            bool isClaimed = FindVariableWithException<bool>(challengeInfoList.value[i], "claimed").value;

            int completeCount = FindVariableWithException<int>(challengeInfoList.value[i], "challengeProgress").value;
            int minCount = BlackboardQueryUtils.GetChallengeMinCount(cType.value);

            long startTimestamp = FindVariableWithException<long>(challengeInfoList.value[i], "startTimestamp").value;
            DateTime startTimeDate = TimeUtils.ParseTimestampToDateTime( startTimestamp);
            startTimeDate = startTimeDate.AddHours(-timeZoneOffset);
            startTimestamp = (long)((startTimeDate - TimeUtils.Jan1St1970).TotalMilliseconds);

            if(currentTimestamp >= startTimestamp)
            {
                var rewardList = BlackboardUtils.FindVariable<List<Blackboard>>(challengeInfoList.value[i], "rewardList");
                Variable<long> currentRewardCoins = null;

                for (int j = 0; j < rewardList.value.Count; j++)
                {
                    if (currentRewardCoins == null)
                        currentRewardCoins = BlackboardUtils.FindVariable<long>(rewardList.value[j], "credit");
                }

                if(completeCount >= minCount && !isClaimed)
                {
                    if(currentRewardCoins != null && currentRewardCoins.value > saveRewardCoins.value)
                    {
                        saveRewardCoins.value = NumberUtils.GetMultiplierNumeratorValue(currentRewardCoins.value, eventMultiplierNumerator);
                    }
                }

                if(cType.value == ChallengeType.DAILY && !isDone && !isClaimed && completeCount == 0)
                {
                    saveNewCoins.value = NumberUtils.GetMultiplierNumeratorValue(currentRewardCoins.value, eventMultiplierNumerator);
                }
            }
        }

        EndAction();
    }

    private Variable<T> FindVariableWithException<T>(IBlackboard bb, string name)
    {
        var variable = BlackboardUtils.FindVariable<T>(bb, name);

        if(variable == null || variable.value == null)
        {
            throw new Exception(string.Format("NullReferenceException in LobbyChallengeCoinsBB: variable {0} is null", name));
        }

        return variable;
    }
}

}
