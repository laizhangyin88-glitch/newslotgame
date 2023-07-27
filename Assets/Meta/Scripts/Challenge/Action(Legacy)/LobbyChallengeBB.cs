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

    public class LobbyChallengeBB : ActionTask<Blackboard>
    {
        public BBParameter<ContextElement> challengeTypeSpriteElement;
        public BBParameter<ContextElement> challengeTypeCoverSpriteElement;

        public BBParameter<ChallengeType> saveChallengeType;

        public BBParameter<int> saveBadgeCount;
        public BBParameter<string> saveNewID;
        public BBParameter<long> saveRewardCoins;

        private const string prevChallengeKey = "NEW_CHALLENGE_ID";

        private const string LOBBY_CHALLENGE_DAILY = "In Game Challenge Gauge Daily";
        private const string LOBBY_CHALLENGE_EXPERT = "In Game Challenge Gauge Expert";
        private const string LOBBY_CHALLENGE_MASTER = "In Game Challenge Gauge Master";

        protected override string info
        {
            get { return "Get Lobby Challenge Button State BB"; }
        }

        protected override void OnExecute()
        {
            saveChallengeType.value = ChallengeType.UNKNOWN;

            saveBadgeCount.value = 0;
            saveRewardCoins.value = 0;
            saveNewID.value = "";

            if (PlayerPrefs.HasKey(prevChallengeKey))
            {
                saveNewID.value = PlayerPrefs.GetString(prevChallengeKey, "");
            }

            long eventMultiplierNumerator = NumberUtils.GetGlobalDenominator();
            EventInfo eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.CHALLENGE_CLAIM_MULTIPLY);

            if (eventInfo != null)
                eventMultiplierNumerator = PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(eventInfo);

            var challengeInfoList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), "challengeInfoList");

            int timeZoneOffset = TimeUtils.GetTimeZoneOffset();
            long currentTimestamp = BagelCode.TimeUtils.GetTimeStamp();

            for (int i = 0; i < challengeInfoList.value.Count; ++i)
            {
                var cType = BlackboardUtils.FindVariable<ChallengeType>(challengeInfoList.value[i], "challengeType");
                bool isDone = BlackboardUtils.FindVariable<bool>(challengeInfoList.value[i], "done").value;
                bool isClaimed = BlackboardUtils.FindVariable<bool>(challengeInfoList.value[i], "claimed").value;

                int completeCount = BlackboardUtils.FindVariable<int>(challengeInfoList.value[i], "challengeProgress").value;
                int minCount = BlackboardQueryUtils.GetChallengeMinCount(cType.value);

                long startTimestamp = BlackboardUtils.FindVariable<long>(challengeInfoList.value[i], "startTimestamp").value;
                DateTime startTimeDate = TimeUtils.ParseTimestampToDateTime(startTimestamp);
                startTimeDate = startTimeDate.AddHours(-timeZoneOffset);
                startTimestamp = (long)((startTimeDate - TimeUtils.Jan1St1970).TotalMilliseconds);

                if (currentTimestamp >= startTimestamp)
                {
                    if (saveBadgeCount.value == 0 && completeCount < minCount)
                    {
                        saveBadgeCount.value = minCount - completeCount;
                    }

                    var rewardList = BlackboardUtils.FindVariable<List<Blackboard>>(challengeInfoList.value[i], "rewardList");
                    Variable<long> currentRewardCoins = null;

                    for (int j = 0; j < rewardList.value.Count; j++)
                    {
                        if (currentRewardCoins == null)
                            currentRewardCoins = BlackboardUtils.FindVariable<long>(rewardList.value[j], "credit");
                    }



                    if (completeCount >= minCount && !isClaimed)
                    {
                        if (currentRewardCoins != null && currentRewardCoins.value > saveRewardCoins.value)
                        {
                            saveRewardCoins.value = NumberUtils.GetMultiplierNumeratorValue(currentRewardCoins.value, eventMultiplierNumerator);
                        }
                    }

                    if (completeCount >= minCount)
                    {
                        completeCount = minCount;
                        isDone = true;
                    }

                    if (isDone && !isClaimed)
                        ++saveBadgeCount.value;
                }
                else
                {
                    completeCount = minCount;
                    isDone = true;
                }

                if (saveChallengeType.value == ChallengeType.UNKNOWN)
                {
                    if (!isDone || i + 1 == challengeInfoList.value.Count)
                    {
                        saveChallengeType.value = cType.value;

                        if (challengeTypeSpriteElement.value != null)
                        {
                            switch (cType.value)
                            {
                                case ChallengeType.DAILY:
                                    MetaContextElementUtils.SetSprite(challengeTypeSpriteElement.value, MetaStringDefine.LOBBY_BUNDLE_NAME, LOBBY_CHALLENGE_DAILY);
                                    MetaContextElementUtils.SetSprite(challengeTypeCoverSpriteElement.value, MetaStringDefine.LOBBY_BUNDLE_NAME, LOBBY_CHALLENGE_DAILY);
                                    break;
                                case ChallengeType.EXPERT:
                                    MetaContextElementUtils.SetSprite(challengeTypeSpriteElement.value, MetaStringDefine.LOBBY_BUNDLE_NAME, LOBBY_CHALLENGE_EXPERT);
                                    MetaContextElementUtils.SetSprite(challengeTypeCoverSpriteElement.value, MetaStringDefine.LOBBY_BUNDLE_NAME, LOBBY_CHALLENGE_EXPERT);
                                    break;
                                case ChallengeType.MASTER:
                                    MetaContextElementUtils.SetSprite(challengeTypeSpriteElement.value, MetaStringDefine.LOBBY_BUNDLE_NAME, LOBBY_CHALLENGE_MASTER);
                                    MetaContextElementUtils.SetSprite(challengeTypeCoverSpriteElement.value, MetaStringDefine.LOBBY_BUNDLE_NAME, LOBBY_CHALLENGE_MASTER);
                                    break;
                            }
                        }
                    }
                }
            }

            EndAction();
        }
    }
}