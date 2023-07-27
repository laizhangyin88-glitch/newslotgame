using System.Collections.Generic;
using System.Linq;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode
{
    public static partial class BlackboardQueryUtils
    {
        public static bool IsHiddenObjectsActive() // check active
        {
            return BlackboardUtils.GetOrCreateVariable<bool>("/hiddenUniverseInfo/active")?.value ?? false;
        }

        public static bool IsHiddenObjectsActiveForUser() // check active for user (condition of unlock level, etc)
        {
            return BlackboardUtils.GetOrCreateVariable<bool>("/hiddenUniverseInfo/activeForUser")?.value ?? false;
        }

        public static void UpdateHiddenUniverseFinder(int earnFinder)
        {
            HiddenObjects.HiddenObjects.Utils.AddFinderCount(earnFinder, false);
            BlackboardUtils.SetOrCreateValue(HiddenObjects.HiddenObjects.Utils.HiddenObjectsInfo, "earnFinderFromSpin", earnFinder);
        }

        public static bool IsHiddenObjectsEnabled() // check active & active for user
        {
            return IsHiddenObjectsActiveForUser() && IsHiddenObjectsActive();
        }

        public static bool IsFinderShopEnabled()
        {
            bool isShopActive = HiddenObjects.HiddenObjects.Utils.HiddenObjectsInfo.GetValue<bool>("hiddenUniverseShopActive");
            var finderShopProductGroupList = GetShopProductGroups(ShopType.HIDDEN_UNIVERSE);

            return isShopActive && finderShopProductGroupList.Count > 0;
        }

        public static bool IsFinderBundleShopEnabled(out bool isShowCoin)
        {
            isShowCoin = true;

            bool isShopActive = HiddenObjects.HiddenObjects.Utils.HiddenObjectsInfo.GetValue<bool>("hiddenUniverseBundleShopActive");
            if (!isShopActive) return false;

            var coinBundleShopProductGroups = GetShopProductGroups(ShopType.COIN_WITH_HIDDEN_UNIVERSE);
            var gemBundleShopProductGroups = GetShopProductGroups(ShopType.COIN_WITH_HIDDEN_UNIVERSE);

            isShowCoin = coinBundleShopProductGroups != null;

            return coinBundleShopProductGroups != null || gemBundleShopProductGroups != null;
        }

        public static ObjectInfo DeserializeObjectInfo(Blackboard bb)
        {
            if (bb is null) return null;

            var info = new ObjectInfo();
            info.chapter = bb.GetValue<int>("chapter");
            info.stage = bb.GetValue<int>("stage");
            info.index = bb.GetValue<int>("index");
            info.score = bb.GetValue<long>("score");
            info.name = bb.GetValue<string>("name");
            return info;
        }

        public static HiddenUniverseLeaderboard DeserializeHiddenUniverseLeaderboard(Blackboard bb)
        {
            if (bb is null) return null;

            var leaderboard = new HiddenUniverseLeaderboard();
            leaderboard.prevMyRank = DeserializeHiddenUniverseRank(bb.GetValue<Blackboard>("prevMyRank"));
            leaderboard.nowMyRank = DeserializeHiddenUniverseRank(bb.GetValue<Blackboard>("nowMyRank"));
            leaderboard.firstRank = DeserializeHiddenUniverseRank(bb.GetValue<Blackboard>("firstRank"));
            leaderboard.shownRankList = bb.GetValue<List<Blackboard>>("shownRankList").
                Select(r => DeserializeHiddenUniverseRank(r)).ToList();

            return leaderboard;
        }

        public static HiddenUniverseRank DeserializeHiddenUniverseRank(Blackboard bb)
        {
            if (bb is null) return null;

            var rank = new HiddenUniverseRank();
            rank.rank = bb.GetValue<int>("rank");
            rank.userId = bb.GetValue<string>("userId");
            rank.userName = bb.GetValue<string>("userName");
            rank.profileUrl = bb.GetValue<string>("profileUrl");
            rank.score = bb.GetValue<long>("score");
            rank.tier = bb.GetValue<int>("tier");
            return rank;
        }

        public static HiddenUniverseStageInfo DeserializeHiddenUniverseStageInfo(Blackboard bb)
        {
            if (bb is null) return null;

            var stageInfo = new HiddenUniverseStageInfo();
            stageInfo.chapter = bb.GetValue<int>("chapter");
            stageInfo.stage = bb.GetValue<int>("stage");
            stageInfo.completedStarCount = bb.GetValue<int>("completedStarCount");
            stageInfo.ongoingStarPercentile = bb.GetValue<int>("ongoingStarPercentile");
            stageInfo.needFinderCount = bb.GetValue<int>("needFinderCount");
            return stageInfo;
        }

        public static HiddenUniverseChapterInfo DeserializeHiddenUniverseChapterRewardInfo(
            Blackboard bb)
        {
            if (bb == null || bb == null) return default;

            var chapterRewardInfo = new HiddenUniverseChapterInfo();
            chapterRewardInfo.isRewarded = bb.GetValue<bool>("isRewarded");
            chapterRewardInfo.completeRewardCoin = bb.GetValue<long>("completeRewardCoin");
            chapterRewardInfo.completeRewardGem = bb.GetValue<long>("completeRewardGem");
            chapterRewardInfo.unlockLevel = bb.GetValue<int>("unlockLevel");
            return chapterRewardInfo;
        }
    }
}
