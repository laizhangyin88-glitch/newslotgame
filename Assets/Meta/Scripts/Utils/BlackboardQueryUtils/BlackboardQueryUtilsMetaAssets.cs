using System.Collections.Generic;
using SlotMaker;

namespace BagelCode
{
    public static partial class BlackboardQueryUtils
    {
        private const string USE_META_ASSET_BUNDLES = "usingMetaAssetBundles";

        public static void AddUsingMetaAssetBundle(string bundleName)
        {
            var bundleList = BlackboardUtils.GetOrCreateVariable<List<string>>(MainBlackboard.Get(), USE_META_ASSET_BUNDLES);

            if (bundleList.value == null)
                bundleList.value = new List<string>();

            if (!bundleList.value.Contains(bundleName))
                bundleList.value.Add(bundleName);
        }

        public static void AddUsingMetaAssetBundles(List<string> bundles)
        {
            for (int i = 0; i < bundles.Count; ++i)
                AddUsingMetaAssetBundle(bundles[i]);
        }

        public static List<string> GetUsingMetaAssetBundles()
        {
            var bundleList = BlackboardUtils.FindVariable<List<string>>(MainBlackboard.Get(), USE_META_ASSET_BUNDLES);

            if (bundleList != null)
                return bundleList.value;

            return null;
        }

        public static void ClearUsingMetaAssetBundles()
        {
            var bundleList = BlackboardUtils.FindVariable<List<string>>(MainBlackboard.Get(), USE_META_ASSET_BUNDLES);

            if (bundleList != null && bundleList.value != null)
                bundleList.value.Clear();
        }

        public static WeightProgress GetWeightProgress()
        {
            var weightProgress = BlackboardUtils.GetOrCreateVariable<object>("/loadingProgress")?.value;
            if (weightProgress == null) weightProgress = new WeightProgress();
            return weightProgress as WeightProgress;
        }
    }
}
