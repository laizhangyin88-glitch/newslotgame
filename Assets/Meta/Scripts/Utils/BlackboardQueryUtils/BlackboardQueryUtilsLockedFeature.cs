using System.Collections.Generic;
using NodeCanvas.Framework;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode
{
    public static partial class BlackboardQueryUtils
    {
        public static bool IsLockedFeature(LockedFeatureType featureType)
        {
            var lockedFeatureList = BlackboardUtils.FindVariable<List<LockedFeatureType>>(MainBlackboard.Get(), "lockedFeatureList");

            if (lockedFeatureList != null && lockedFeatureList.value.Count > 0)
            {
                return lockedFeatureList.value.Contains(featureType);
            }

            return false;
        }

        public static int GetFeatureMinLevel(LockedFeatureType featureType)
        {
            var featureUnlockLevel = BlackboardUtils.FindVariable<Dictionary<int, int>>(MainBlackboard.Get(), "values/featureUnlockLevel");

            if (featureUnlockLevel != null && featureUnlockLevel.value.ContainsKey((int)featureType))
            {
                return featureUnlockLevel.value[(int)featureType];
            }

            return 0;
        }

        public static void UpdateUnlockFeature(List<LockedFeatureType> featureUnlockList)
        {
            if (featureUnlockList == null || featureUnlockList.Count == 0) return;

            var lockedFeatureList = BlackboardUtils.FindVariable<List<LockedFeatureType>>(MainBlackboard.Get(), "lockedFeatureList");

            if (lockedFeatureList != null && lockedFeatureList.value.Count > 0)
            {
                for (int i = 0; i < featureUnlockList.Count; ++i)
                {
                    if (lockedFeatureList.value.Contains(featureUnlockList[i]))
                    {
                        lockedFeatureList.value.Remove(featureUnlockList[i]);
                    }
                }

            }
        }

        public static bool IsActiveTutorial()
        {
            var tutorialEnabled = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "values/misc/TUTORIAL_ENABLED");
            var tutorialInfo = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "tutorialInfo");

            if (tutorialEnabled != null && tutorialEnabled.value && tutorialInfo != null)
            {
                int stage = tutorialInfo.value.GetValue<int>("stage");
                if (stage == 0 || stage == 1 || stage == 2)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
