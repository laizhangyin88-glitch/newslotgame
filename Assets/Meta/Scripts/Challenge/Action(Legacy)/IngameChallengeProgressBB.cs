using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Challenge")]
    public class IngameChallengeProgressBB : ActionTask<Blackboard>
    {
        public BBParameter<ContextElement> challengeTypeSpriteElement;
        public BBParameter<ContextElement> challengeTypeCoverSpriteElement;

        public BBParameter<string> saveProgressText;
        public BBParameter<float> saveProgress;
        public BBParameter<ChallengeType> saveChallengeType;
        public BBParameter<int> saveBadgeCount;
        public BBParameter<bool> isNewChallenge;
        public BBParameter<string> newChallengeText;

        private const string INGAME_CHALLENGE_DAILY = "In Game Challenge Gauge Daily";
        private const string INGAME_CHALLENGE_EXPERT = "In Game Challenge Gauge Expert";
        private const string INGAME_CHALLENGE_MASTER = "In Game Challenge Gauge Master";

        protected override string info
        {
            get { return "Get Ingame Challenge BB"; }
        }

        protected override void OnExecute()
        {
            saveProgressText.value = "0/0";
            saveProgress.value = 0f;
            saveChallengeType.value = ChallengeType.UNKNOWN;
            saveBadgeCount.value = 0;
            // challengeState.value = 0;

            bool isError = false;

            // Update Challenge Button (Daily/Expert/Master)
            var challengeInfoList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), "challengeInfoList");

            long currentTimestamp = BagelCode.TimeUtils.GetTimeStamp();
            isNewChallenge.value = false;

            for (int i = 0; i < challengeInfoList.value.Count; ++i)
            {
                var cType = BlackboardUtils.FindVariable<ChallengeType>(challengeInfoList.value[i], "challengeType");
                bool isDone = BlackboardUtils.FindVariable<bool>(challengeInfoList.value[i], "done").value;
                bool isClaimed = BlackboardUtils.FindVariable<bool>(challengeInfoList.value[i], "claimed").value;

                int completeCount = BlackboardUtils.FindVariable<int>(challengeInfoList.value[i], "challengeProgress").value;
                int minCount = BlackboardQueryUtils.GetChallengeMinCount(cType.value);

                long startTimestamp = BlackboardUtils.FindVariable<long>(challengeInfoList.value[i], "startTimestamp").value;
                startTimestamp = TimeUtils.ApplyTimeZoneOffset(startTimestamp);

                if (currentTimestamp >= startTimestamp)
                {
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
                        saveProgress.value = (float)completeCount / (float)minCount;
                        saveProgressText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "A_PER_B", out isError, completeCount, minCount);

                        // Set Sprite
                        if (challengeTypeSpriteElement.value != null)
                        {
                            switch (cType.value)
                            {
                                case ChallengeType.DAILY:
                                    MetaContextElementUtils.SetSprite(challengeTypeSpriteElement.value, MetaStringDefine.LOBBY_BUNDLE_NAME, INGAME_CHALLENGE_DAILY);
                                    MetaContextElementUtils.SetSprite(challengeTypeCoverSpriteElement.value, MetaStringDefine.LOBBY_BUNDLE_NAME, INGAME_CHALLENGE_DAILY);
                                    break;
                                case ChallengeType.EXPERT:
                                    MetaContextElementUtils.SetSprite(challengeTypeSpriteElement.value, MetaStringDefine.LOBBY_BUNDLE_NAME, INGAME_CHALLENGE_EXPERT);
                                    MetaContextElementUtils.SetSprite(challengeTypeCoverSpriteElement.value, MetaStringDefine.LOBBY_BUNDLE_NAME, INGAME_CHALLENGE_EXPERT);
                                    break;
                                case ChallengeType.MASTER:
                                    MetaContextElementUtils.SetSprite(challengeTypeSpriteElement.value, MetaStringDefine.LOBBY_BUNDLE_NAME, INGAME_CHALLENGE_MASTER);
                                    MetaContextElementUtils.SetSprite(challengeTypeCoverSpriteElement.value, MetaStringDefine.LOBBY_BUNDLE_NAME, INGAME_CHALLENGE_MASTER);
                                    break;
                            }
                        }
                    }
                }
                // Check New Challenge
                long endTimestamp = BlackboardUtils.FindVariable<long>(challengeInfoList.value[i], "endTimestamp").value;
                endTimestamp = TimeUtils.ApplyTimeZoneOffset(endTimestamp);

                ChallengeUtils.PersonalChallengeTypeToMetaChallengeType(cType.value, out MetaChallengeType type);
                long checkTime = ChallengeUtils.GetChallengeCheckTime(type);

                // Time Check : (Check Time <= Start Time && Current TIme >= Start Time) || (Check Time <= End Time && Current Time >= End Time)
                if ((checkTime <= startTimestamp && currentTimestamp >= startTimestamp) || (checkTime <= endTimestamp && currentTimestamp >= endTimestamp))
                {
                    isNewChallenge.value = true;
                    newChallengeText.value = "N";
                }
            }

            EndAction();
        }
    }
}