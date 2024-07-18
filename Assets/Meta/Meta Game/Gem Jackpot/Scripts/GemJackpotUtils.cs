using BagelCode.ClientModels;
using NodeCanvas.Framework;
using SlotMaker;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class GemJackpotUtils
    {
        private static int maxFreeSpinCount = 0;
        private static string slotEnterContextID = "";
        public static long eventRewardMultiply;
        public static long eventSpinGemSale;

        private const string GEM_JACKPOT_GAME_INFO = "metaGameEnterInfo";
        private const string GEM_JACKPOT_MAIN_INFO = "gemJackpotEnterInfo";
        private const string GEM_JACKPOT_LAST_ADS_TIMESTAMP = "GEM_JACKPOT_LAST_ADS_TIMESTAMP";

        private static Blackboard gemJackpotInfo = null;

        public static Blackboard GemJackpotInfo
        {
            get
            {
                if (gemJackpotInfo == null)
                {
                    var info = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), GEM_JACKPOT_GAME_INFO);
                    if (info != null)
                    {
                        info = BlackboardUtils.FindVariable<Blackboard>(info.value, GEM_JACKPOT_MAIN_INFO);
                        if (info != null) gemJackpotInfo = info.value;
                    }
                }
                return gemJackpotInfo;
            }
        }

        public static Blackboard OrigJackpotInfo
        {
            get
            {
                try
                { return BlackboardUtils.FindValue<List<Blackboard>>(GemJackpotInfo, "jackpotList")[0]; }
                catch
                { return null; }
            }
        }

        public static Blackboard JackpotInfo
        {
            get
            {
                try
                { return BlackboardUtils.FindValue<List<Blackboard>>(GemJackpotInfo, "jackpotList")[1]; }
                catch
                { return null; }
            }
            set
            {
                List<Blackboard> jackpotList = BlackboardUtils.FindValue<List<Blackboard>>(GemJackpotInfo, "jackpotList");
                if (jackpotList != null)
                {
                    if (jackpotList.Count == 1)
                    {
                        BlackboardUtils.AddToBlackboardList(GemJackpotInfo, "jackpotList", value);
                    }
                    else if (jackpotList.Count > 1)
                    {
                        //BlackboardUtils.SetOrCreateValue(jackpotList[1], "JackpotInfo", value);
                        jackpotList[1] = value;
                        jackpotList[1].transform.parent = jackpotList[0].transform.parent;
                    }
                }
            }
        }

        public static int MaxFreeSpinCount
        {
            get { return maxFreeSpinCount; }
            set { maxFreeSpinCount = value; }
        }

        public static int FreeSpinCount
        {
            get
            {
                if (GemJackpotInfo != null)
                    return BlackboardUtils.FindValue<int>(GemJackpotInfo, "freeSpinCount");
                return 0;
            }
            set
            {
                if (GemJackpotInfo != null) BlackboardUtils.SetOrCreateValue(GemJackpotInfo, "freeSpinCount", value);
            }
        }

        public static int NextProgress
        {
            get
            {
                if (GemJackpotInfo != null)
                    return BlackboardUtils.FindValue<int>(GemJackpotInfo, "nextProgress");
                return 0;
            }
            set { if (GemJackpotInfo != null) BlackboardUtils.SetOrCreateValue(GemJackpotInfo, "nextProgress", value); }
        }

        public static int PrevProgress
        {
            get
            {
                if (GemJackpotInfo != null)
                    return BlackboardUtils.FindValue<int>(GemJackpotInfo, "prevProgress");
                return 0;
            }
            set { if (GemJackpotInfo != null) BlackboardUtils.SetOrCreateValue(GemJackpotInfo, "prevProgress", value); }
        }

        public static long LastVideoAdsClaimTimestamp
        {
            get
            {
                if (GemJackpotInfo != null)
                    return BlackboardUtils.FindValue<long>(GemJackpotInfo, "lastVideoAdsClaimTimestamp");
                return TimeUtils.GetTimeStamp();
            }
        }

        public static long CoolTime
        {
            get
            {
                if (GemJackpotInfo != null)
                    return BlackboardUtils.FindValue<long>(GemJackpotInfo, "gemJackpotCooltime");
                return 0L;
            }
        }

        public static long GemForSpin
        {
            get
            {
                if (GemJackpotInfo != null)
                    return BlackboardUtils.FindValue<long>(GemJackpotInfo, "gemForSpin");
                return 0L;
            }
        }

        public static long GemForSpinSale
        {
            get { return NumberUtils.GetSaleNumeratorValue(GemForSpin, EventSpinGemSale); }
        }

        public static List<Blackboard> JackpotReelRewardList
        {
            get
            {
                if (GemJackpotInfo != null)
                    return BlackboardUtils.FindValue<List<Blackboard>>(GemJackpotInfo, "jackpotReelRewardList");
                return new List<Blackboard>();
            }
        }

        public static long GrandJackpotRewardAmount
        {
            get
            {
                if (GemJackpotInfo != null)
                    return BlackboardUtils.FindValue<long>(GemJackpotInfo, "grandJackpotRewardAmount");
                return 0L;
            }
        }

        public static int JackpotInfoIndex
        {
            get
            {
                if (GemJackpotInfo != null)
                {
                    if (BlackboardUtils.FindValue<Blackboard>(GemJackpotInfo, "jackpotInfo") != null)
                        return BlackboardUtils.FindValue<int>(GemJackpotInfo, "jackpotInfo/index");
                    else
                        return -1;
                }
                return -1;
            }
        }

        public static List<long> SlotReelRewardList
        {
            get
            {
                if (GemJackpotInfo != null)
                    return BlackboardUtils.FindValue<List<long>>(GemJackpotInfo, "slotReelRewardList");
                return new List<long>();
            }
        }

        public static bool IsClickedNoDealButton
        {
            get
            {
                if (GemJackpotInfo != null)
                    return BlackboardUtils.GetOrCreateVariable<bool>(GemJackpotInfo, "isClickedNoDeal").value;
                return false;
            }
            set { BlackboardUtils.SetOrCreateValue<bool>(GemJackpotInfo, "isClickedNoDeal", value); }
        }

        public static string BISlotEnterContextID
        {
            get { return slotEnterContextID; }
            set { slotEnterContextID = value; }
        }

        public static List<SlotMaker.TestSuite.DebugSpin> GetGemJackpotDebugSpinList()
        {
            List<SlotMaker.TestSuite.DebugSpin> ret = null;
            if (GemJackpotInfo != null)
            {
                try
                { ret = BlackboardUtils.FindValue<List<SlotMaker.TestSuite.DebugSpin>>(GemJackpotInfo, "debugSpins/list"); }
                catch
                { ret = null; }
            }
            return ret;
        }

        public static long EventRewardMultiply
        {
            get
            {
                if (eventRewardMultiply == 0L)
                {
                    EventInfo info = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.GEM_JACKPOT_REWARD_MULTIPLY);
                    eventRewardMultiply = (info != null) ? PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(info) : NumberUtils.GetGlobalDenominator();
                }
                return eventRewardMultiply;
            }
            set { eventRewardMultiply = value; }
        }

        public static long EventSpinGemSale
        {
            get
            {
                if (eventSpinGemSale == 0L)
                {
                    EventInfo info = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.GEM_JACKPOT_SPIN_GEM_SALE);
                    eventSpinGemSale = (info != null) ? PassiveEventManager.Instance.GetEventInfoViewPercent(info) : 100L;
                }
                return eventSpinGemSale;
            }
            set { eventSpinGemSale = value; }
        }

        public static List<int> SlotReelSetResultIndexList
        {
            get
            {
                if (GemJackpotInfo != null) return BlackboardUtils.FindValue<List<int>>(GemJackpotInfo, "slotReelSetResultIndexList");
                return new List<int>();
            }
        }

        public static List<int> InitialSlotReelSetIndexList
        {
            get
            {
                if (GemJackpotInfo != null)
                {
                    List<int> startIndex = BlackboardUtils.FindValue<List<int>>(GemJackpotInfo, "initialSlotReelSetIndexList");
                    for (int i = 0; i < startIndex.Count; ++i)
                        startIndex[i] = startIndex[i] - 1;
                    return startIndex;
                }
                return new List<int>();
            }
        }

        public static bool EnabledGemDisplay
        {
            get
            {
                if (GemJackpotInfo != null) return BlackboardUtils.FindValue<bool>(GemJackpotInfo, "enabledGemJackpotGemDisplay");
                return true;
            }
        }

        public static long GrandJackpotMultiplyNumerator
        {
            get
            {
                if (GemJackpotInfo != null) return BlackboardUtils.FindValue<long>(GemJackpotInfo, "grandJackpotMultiplyNumerator");
                return NumberUtils.GetGlobalDenominator();
            }
        }

        public static long PrevGrandJackpotMultiplyNumerator
        {
            get
            {
                if (GemJackpotInfo != null) return BlackboardUtils.FindValue<long>(GemJackpotInfo, "prevGrandJackpotMultiplyNumerator");
                return GrandJackpotMultiplyNumerator;
            }
            set { BlackboardUtils.SetOrCreateValue<long>(GemJackpotInfo, "prevGrandJackpotMultiplyNumerator", value); }
        }

        public static double FPP
        {
            get
            {
                if (GemJackpotInfo != null) return BlackboardUtils.FindValue<double>(GemJackpotInfo, "favoritePurchasePrice");
                return 0;
            }
        }

        public static long LastAdsTimestamp
        {
            get { return PlayerPrefsUtils.GetInt64(GEM_JACKPOT_LAST_ADS_TIMESTAMP, LastVideoAdsClaimTimestamp); }
            set { PlayerPrefsUtils.SetInt64(GEM_JACKPOT_LAST_ADS_TIMESTAMP, value); }
        }

        public static bool IsShowCloseAds
        {
            get
            {
                if (GemJackpotInfo != null)
                {
                    var isShowCloseAds = BlackboardUtils.FindVariable<bool>(GemJackpotInfo, "isShowCloseAds");
                    if (isShowCloseAds != null)
                        return isShowCloseAds.value;
                    else
                        return LastAdsTimestamp < LastVideoAdsClaimTimestamp;
                }
                return false;
            }
            set { if (GemJackpotInfo != null) BlackboardUtils.SetOrCreateValue(GemJackpotInfo, "isShowCloseAds", value); }
        }

        public static void Init()
        {
            EventRewardMultiply = 0L;
            eventSpinGemSale = 0L;
        }

        public static void CreateGemJackpotReelStripsBB()
        {
            // game reel sequence
            var checkBB = GemJackpotInfo.GetVariable<List<Blackboard>>("reelSetList");
            if (checkBB != null && checkBB.value.Count > 0)
            {
                BlackboardUtils.DestroyBlackboardList(GemJackpotInfo, "reelSetList");
            }

            Dictionary<int, List<int>> slotReelBalance = BlackboardUtils.FindValue<Dictionary<int, List<int>>>(GemJackpotInfo, "slotReelBalance");
            List<int> jackpotReelSetList = BlackboardUtils.FindValue<List<int>>(GemJackpotInfo, "jackpotReelSetList");

            var reelSequenceListBB = BlackboardUtils.CreateBlackboard("reelSequenceList");
            BlackboardUtils.GetOrCreateBlackboardList(reelSequenceListBB, "reelSequenceList");
            for (int i = 0; i < 5; ++i)
            {
                var gameReelListBB = BlackboardUtils.CreateBlackboard("gameReel");
                BlackboardUtils.SetOrCreateValue(gameReelListBB, "value", slotReelBalance[i]);
                BlackboardUtils.AddToBlackboardList(reelSequenceListBB, "reelSequenceList", gameReelListBB);
            }
            BlackboardUtils.SetOrCreateValue(GemJackpotInfo, "gameType", GameType.SLOT_MACHINE);
            BlackboardUtils.AddToBlackboardList(GemJackpotInfo, "reelSetList", reelSequenceListBB);

            // jackpot reel sequence
            var jackpotReelSequenceListBB = BlackboardUtils.CreateBlackboard("reelSequenceList");
            BlackboardUtils.GetOrCreateBlackboardList(jackpotReelSequenceListBB, "reelSequenceList");
            var jackpotGameReelListBB = BlackboardUtils.CreateBlackboard("gameReel");
            BlackboardUtils.SetOrCreateValue(jackpotGameReelListBB, "value", jackpotReelSetList);
            BlackboardUtils.AddToBlackboardList(jackpotReelSequenceListBB, "reelSequenceList", jackpotGameReelListBB);
            BlackboardUtils.AddToBlackboardList(GemJackpotInfo, "reelSetList", jackpotReelSequenceListBB);
        }

        public static bool CheckGemJackpotProgress()
        {
            try
            { return PrevProgress != NextProgress; }
            catch
            { return false; }
        }

        public static bool CheckGemJackpotBonus()
        {
            if (NextProgress >= 25)
                return true;

            return JackpotInfoIndex > -1;
        }

        public static bool CheckGemJackpotTrySpin()
        {
            if (FreeSpinCount > 0)
                return true;
            if (GemForSpinSale <= BlackboardUtils.FindValue<long>(null, "/me/gem"))
                return true;
            return false;
        }

        public static long GetRewardCredit()
        {
            if (JackpotInfoIndex < 0)
                return NumberUtils.GetMultiplierNumeratorValue(SlotReelRewardList[NextProgress], EventRewardMultiply);
            else
                return NumberUtils.GetMultiplierNumeratorValue(BlackboardUtils.FindValue<long>(GemJackpotInfo, "jackpotInfo/credit"), EventRewardMultiply);
        }

        public static GameObject GetSlotMachine()
        {
            if (GemJackpotInfo != null)
            {
                GameObject slotMachine = BlackboardUtils.FindValue<GameObject>(GemJackpotInfo, "slotMachine");
                if (slotMachine != null)
                    return slotMachine;
            }
            return null;
        }

        public static void StartSpin()
        {
            int spinCount = FreeSpinCount;
            if (spinCount > 0) FreeSpinCount = spinCount - 1;
        }

        public static bool CheckConsecutiveBonusSymbol(int reelIndex)
        {
            bool isConsecutiveBonus = false;
            List<int> slotReelSetResultList = SlotReelSetResultIndexList;
            if (slotReelSetResultList != null && reelIndex < slotReelSetResultList.Count)
            {
                var strips = MetaSlotMachineGlobalReelStrips.Instance.GetReelStrips();
                for (int i = 0; i <= reelIndex; ++i)
                {
                    var strip = strips.GetReelStrip(i);
                    int idx = slotReelSetResultList[i];
                    // bonus symbol == 3
                    if (strip.GetSymbol(idx).symbol != 3)
                        return isConsecutiveBonus;
                }
                isConsecutiveBonus = true;
            }
            return isConsecutiveBonus;
        }

        public static void UpdateCloseAdsTimestamp(long lastTimestamp)
        {
            long nowLastVideoAds = LastVideoAdsClaimTimestamp;
            if (lastTimestamp < nowLastVideoAds) IsShowCloseAds = true;
        }

        public static void UpdateJackpotInfo(long grandJackpotMultiplyNumerator)
        {
            List<Blackboard> jackpotList = BlackboardUtils.FindValue<List<Blackboard>>(GemJackpotInfo, "jackpotList");

            if (jackpotList == null || jackpotList.Count < 1 || OrigJackpotInfo is null) return;

            var jackpotInfo = BlackboardUtils.CreateBlackboard("JackpotInfo");

            Blackboard origJackpotInfo = OrigJackpotInfo;

            BlackboardUtils.SetOrCreateValue(jackpotInfo, "current", NumberUtils.GetMultiplierNumeratorValue(BlackboardUtils.FindValue<long>(origJackpotInfo, "current"), grandJackpotMultiplyNumerator));
            BlackboardUtils.SetOrCreateValue(jackpotInfo, "prev", NumberUtils.GetMultiplierNumeratorValue(BlackboardUtils.FindValue<long>(origJackpotInfo, "prev"), grandJackpotMultiplyNumerator));
            BlackboardUtils.SetOrCreateValue(jackpotInfo, "deltaMs", BlackboardUtils.FindValue<int>(origJackpotInfo, "deltaMs"));
            BlackboardUtils.SetOrCreateValue(jackpotInfo, "min", BlackboardUtils.FindValue<long>(origJackpotInfo, "min"));
            BlackboardUtils.SetOrCreateValue(jackpotInfo, "max", BlackboardUtils.FindValue<long>(origJackpotInfo, "max"));
            BlackboardUtils.SetOrCreateValue(jackpotInfo, "alarm", BlackboardUtils.FindValue<bool>(origJackpotInfo, "alarm"));

            JackpotInfo = (Blackboard)jackpotInfo;
        }

        public static bool CheckUnlockedLevel()
        {
            return MetaGameUtils.IsMetaGameLevelLocked();
        }

        // Screen scale (iPad)
        public static void CheckScreenScale(ContextElement element, Vector3 changeVector)
        {
#if UNITY_IOS && (!UNITY_IPHONE || UNITY_EDITOR)
#if !UNITY_EDITOR
            bool deviceIsIpad = UnityEngine.iOS.Device.generation.ToString().Contains("iPad");
            if (deviceIsIpad)
#endif
            {
                if (element != null)
                {
                    CanvasScaler canvasScaler = element.transform.root.GetComponent<CanvasScaler>();
                    if (canvasScaler != null)
                    {
                        element.transform.localScale = changeVector;
                    }
                }
            }
#endif
        }
    }
}
