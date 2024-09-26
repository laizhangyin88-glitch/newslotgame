using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ParadoxNotion;
using NodeCanvas.Framework;
using SlotMaker;
using System.Linq;
using Sirenix.OdinInspector;

namespace BagelCode
{
    public class InGameBetController : EventMonoBehaviour
    {
        private Variable<int> level;
        public int Level
        {
            get { return level.value; }
            set { level.value = value; }
        }

        private Variable<int> autoBetSelectMultiplierLowLevel;
        public int AutoBetSelectMultiplierLowLevel
        {
            get { return autoBetSelectMultiplierLowLevel.value; }
            set { autoBetSelectMultiplierLowLevel.value = value; }
        }

        private Variable<int> autoBetSelectMultiplierHighLevel;
        public int AutoBetSelectMultiplierHighLevel
        {
            get { return autoBetSelectMultiplierHighLevel.value; }
            set { autoBetSelectMultiplierHighLevel.value = value; }
        }

        private Variable<int> autoBetSelectHighLevelStart;
        public int AutoBetSelectHighLevelStart
        {
            get { return autoBetSelectHighLevelStart.value; }
            set { autoBetSelectHighLevelStart.value = value; }
        }

        private Variable<bool> enableGameSpin;
        public bool EnableGameSpin
        {
            get { return enableGameSpin.value; }
            set { enableGameSpin.value = value; }
        }

        private Variable<List<Blackboard>> restrictions;
        public List<Blackboard> Restrictions
        {
            get { return restrictions.value; }
            set { restrictions.value = value; }
        }

        private Variable<List<long>> betList;
        public List<long> BetList
        {
            get { return betList.value; }
            set { betList.value = value; }
        }
        [Button]
        void test_ShowBetList()
        {
            for (int i = 0; i < BetList.Count; i++)
            {
                Debug.Log($"【BetList】{i} = {BetList[i]}");
            }
        }


        private Variable<bool> isExtended;
        public bool IsExtended
        {
            get { return isExtended.value; }
            set { isExtended.value = value; }
        }

        private Variable<int> extendedValue;
        public int ExtendedValue
        {
            get { return extendedValue.value; }
            set { extendedValue.value = value; }
        }

        private Variable<int> levelRestriction;
        public int LevelRestriction
        {
            get { return levelRestriction.value; }
            set { levelRestriction.value = value; }
        }

        private Variable<bool> isEarlyAccess;
        public bool IsEarlyAccess
        {
            get { return isEarlyAccess.value; }
            set { isEarlyAccess.value = value; }
        }

        private Variable<bool> isGameSpin;
        public bool IsGameSpin
        {
            get { return isGameSpin.value; }
            set { isGameSpin.value = value; }
        }

        private Variable<SpinType> spinType;
        public SpinType SpinType
        {
            get { return spinType.value; }
            set
            {
                IsGameSpin = (value != SpinType.None);
                spinType.value = value;
            }
        }

        private Variable<int> gameSpinCount;
        public int GameSpinCount
        {
            get { return gameSpinCount.value; }
            set { gameSpinCount.value = value; }
        }

        private Variable<Dictionary<long, int>> gameSpinDict;
        public Dictionary<long, int> GameSpinDict { get { return gameSpinDict.value; } }

        private Variable<Dictionary<long, int>> bonusSpinDict;
        public Dictionary<long, int> BonusSpinDict { get { return bonusSpinDict.value; } }

        private Variable<long> betCredit;
        public long BetCredit
        {
            get { return betCredit.value; }
            set { betCredit.value = value; }
        }

        private Variable<long> maxBetCredit;
        public long MaxBetCredit
        {
            set { maxBetCredit.value = value; }
        }

        private Variable<int> betIndex;
        public int BetIndex
        {
            get { return betIndex.value; }
            set { betIndex.value = value; }
        }

        private Variable<int> maxBetIndex;
        public int MaxBetIndex
        {
            get { return maxBetIndex.value; }
            set { maxBetIndex.value = value; }
        }

        private Variable<long> extraBetCredit;
        public long ExtraBetCredit
        {
            get { return extraBetCredit.value; }
            set { extraBetCredit.value = value; }
        }

        private Variable<List<Blackboard>> extraBetRatioList;
        public List<Blackboard> ExtraBetRatioList
        {
            get { return extraBetRatioList.value; }
            set { extraBetRatioList.value = value; }
        }

        private Variable<int> extraBetRatioIndex;
        public int ExtraBetRatioIndex
        {
            get { return extraBetRatioIndex.value; }
            set { extraBetRatioIndex.value = value; }
        }

        private Variable<long> totalBetCredit;
        public long TotalBetCredit
        {
            get { return totalBetCredit.value; }
            set { totalBetCredit.value = value; }
        }

        private Variable<bool> autoSpin;
        public bool AutoSpin
        {
            get { return autoSpin.value; }
            set { autoSpin.value = value; }
        }

        private Variable<bool> useForcedExtraBetRatio;
        public bool UseForcedExtraBetRatio
        {
            get { return useForcedExtraBetRatio.value; }
            set { useForcedExtraBetRatio.value = value; }
        }

        private Variable<List<int>> forcedExtraBetRatioIndexList;
        public List<int> ForcedExtraBetRatioIndexList
        {
            get { return forcedExtraBetRatioIndexList.value; }
            set { forcedExtraBetRatioIndexList.value = value; }
        }

        // Ticketed Bonus extraBetCredit checking status - 2022.01.28 Jungsik Choi
        private enum TicketedStatus
        {
            START,          // Init
            CHECK_TICKET,   // First Check
            END             // Ticket Claim or Non Ticket
        }
        private TicketedStatus ticketedStatus = TicketedStatus.START;

        protected override void Awake()
        {
            base.Awake();

            level = BlackboardUtils.FindVariable<int>("/me/level");
            autoBetSelectMultiplierLowLevel = BlackboardUtils.FindVariable<int>("/values/misc/AUTO_BET_SELECT_MULTIPLIER_LOW_LEVEL");
            autoBetSelectMultiplierHighLevel = BlackboardUtils.FindVariable<int>("/values/misc/AUTO_BET_SELECT_MULTIPLIER_HIGH_LEVEL");
            autoBetSelectHighLevelStart = BlackboardUtils.FindVariable<int>("/values/misc/AUTO_BET_SELECT_HIGH_LEVEL_START");
            enableGameSpin = BlackboardUtils.FindVariable<bool>("/values/misc/ENABLE_GAME_SPIN");
            betList = BlackboardUtils.FindVariable<List<long>>("./betList");
            restrictions = BlackboardUtils.FindVariable<List<Blackboard>>("./restrictionList");
            gameSpinDict = BlackboardUtils.FindVariable<Dictionary<long, int>>("./gameSpinCountPerBet");
            bonusSpinDict = BlackboardUtils.FindVariable<Dictionary<long, int>>("./bonusSpinCountPerBet");
            isEarlyAccess = BlackboardUtils.FindVariable<bool>("./isEarlyAccess");
            levelRestriction = BlackboardUtils.FindVariable<int>("./levelRestriction");
            isGameSpin = BlackboardUtils.FindVariable<bool>("./isGameSpin");
            spinType = BlackboardUtils.FindVariable<SpinType>("./spinType");
            gameSpinCount = BlackboardUtils.FindVariable<int>("./gameSpinCount");
            betCredit = BlackboardUtils.FindVariable<long>("./betCredit");
            maxBetCredit = BlackboardUtils.FindVariable<long>("./maxBetCredit");
            betIndex = BlackboardUtils.FindVariable<int>("./betIndex");
            maxBetIndex = BlackboardUtils.FindVariable<int>("./maxBetIndex");
            extraBetCredit = BlackboardUtils.FindVariable<long>("./extraBetCredit");
            extraBetRatioIndex = BlackboardUtils.FindVariable<int>("./extraBetRatioIndex");
            extraBetRatioList = BlackboardUtils.FindVariable<List<Blackboard>>("./game/extraBetRatioList");
            totalBetCredit = BlackboardUtils.FindVariable<long>("./totalBetCredit");
            autoSpin = BlackboardUtils.FindVariable<bool>("./autoSpin");
            useForcedExtraBetRatio = BlackboardUtils.FindVariable<bool>("./useForcedExtraBetRatio");
            forcedExtraBetRatioIndexList = BlackboardUtils.FindVariable<List<int>>("./forcedExtraBetRatioIndexList");

            isExtended = BlackboardUtils.GetOrCreateVariable<bool>("/isExtendedBetIndex");
            extendedValue = BlackboardUtils.GetOrCreateVariable<int>("/extendedValue");

            ExtendedValue = VipLounge.VipLounge.Utils.GetExtendedValue();
        }

        public virtual void InitGame()
        {
            UpdateMaxBetIndex();
            ChooseBetIndex();
        }

        public virtual void EnterTurn()
        {
            if (IsGameSpin)
            {
                // Exception.. 
                if (SpinType != SpinType.BuyABonus)
                    --GameSpinCount;
            }
        }

        public virtual void ExitTurn()
        {
            UpdateMaxBetIndex();

            if (IsGameSpin)
            {
                if (SpinType == SpinType.BuyABonus)
                {
                    MessageDispatcher.Dispatch("OnCreditEvent", new EventData<int>("UpdateBetIndex", BetIndex));
                    // Exception.. 
                    AutoSpin = false;
                    // ChooseBetIndex();
                    MessageDispatcher.Dispatch("OnMetaUIEvent", new EventData(MetaEventDefine.ACTIVE_META_UI));
                }
                else if (GameSpinCount == 0)
                {
                    AutoSpin = false;
                    ChooseBetIndex();
                    MessageDispatcher.Dispatch("OnMetaUIEvent", new EventData(MetaEventDefine.ACTIVE_META_UI));
                }
            }

            UpdateLevelRestriction(BetCredit);
        }

        public virtual void BroadcastUpdateBetCreditEvent(long bet)
        {
            MessageDispatcher.Dispatch("OnCreditEvent", new EventData<long>("UpdateBetCredit", bet));
        }

        public void UpdateBetIndex(int index)
        {
            if (index < 0) index = 0;
            if (index >= BetList.Count) index = BetList.Count - 1;

            UpdateGameSpin(BetList[index]);

            if (!IsGameSpin)
            {
                BetIndex = Mathf.Clamp(index, 0, MaxBetIndex);
            }
            else
            {
                BetIndex = index;
            }

            IsExtended = ExtendedValue > 0 && ExtendedValue > MaxBetIndex - BetIndex;

            BroadcastUpdateBetCreditEvent(BetList[BetIndex]);
        }

        public void UpdateExtraBetRatioIndex(int index)
        {
            ExtraBetRatioIndex = Mathf.Clamp(index, 0, ExtraBetRatioList.Count);

            BroadcastUpdateBetCreditEvent(BetList[BetIndex]);
        }

        public virtual void UpdateBetCredit(long credit)
        {
            BetCredit = credit;
            ExtraBetCredit = GetExtraBetCredit(credit);
            TotalBetCredit = BetCredit + ExtraBetCredit;

            if (BetList.Contains(credit))
            {
                PlayerPrefs.SetString("LAST_BET_CREDIT", BetCredit.ToString());
                PlayerPrefs.SetString("LAST_EXTRA_BET_CREDIT", ExtraBetCredit.ToString());
            }

            MessageDispatcher.Dispatch("OnCreditEvent", new EventData<long>("UpdatedTotalBetCredit", TotalBetCredit));

            UpdateLevelRestriction(BetCredit);
        }

        public void BuyABonus(long credit)
        {
            GameSpinCount = 1;
            SpinType = SpinType.BuyABonus;

            BroadcastUpdateBetCreditEvent(credit);
            MessageDispatcher.Dispatch("OnSpinButtonEvent", new EventData("OnSpinButtonEvent"));
        }

        public void BetMax()
        {
            MessageDispatcher.Dispatch("OnCreditEvent", new EventData<int>("UpdateBetIndex", MaxBetIndex));
        }

        protected virtual void UpdateMaxBetIndex()
        {
            int remainingExtended = ExtendedValue;

            MaxBetIndex = 0;
            for (int i = 0; i < BetList.Count; ++i)
            {
                if (Level < GetBetToLevelRestriction(BetList[i]))
                {
                    if (remainingExtended-- <= 0)
                        break;
                }

                MaxBetIndex = i;
            }

            MaxBetCredit = BetList[MaxBetIndex];

            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, "UpdateButtonState");
        }

        protected virtual void UpdateLevelRestriction(long bet)
        {
            LevelRestriction = 0;
            if (ExtendedValue == 0 && BetList.Contains(bet))
            {
                int currentBetIndex = BetList.IndexOf(bet);
                if (currentBetIndex < BetList.Count - 1)
                {
                    if (currentBetIndex >= MaxBetIndex)
                    {
                        int levelRestriction = GetBetToLevelRestriction(BetList[currentBetIndex + 1]);
                        if (Level < levelRestriction)
                        {
                            LevelRestriction = levelRestriction;
                        }
                    }
                }
            }

            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, "UpdateLevelRestriction"); // for anim refresh
        }

        private void ChooseBetIndex()
        {
            int choosedBetIndex = ChooseGameSpinBetIndex();
            if (choosedBetIndex < 0)
            {
                long userCredit = BlackboardUtils.FindVariable<long>("/me/credit").value;

                bool isHighLevel = (Level >= AutoBetSelectHighLevelStart);
                //choosedBetIndex = ChooseBetIndex(userCredit, userCredit / (long)(isHighLevel ? AutoBetSelectMultiplierHighLevel : AutoBetSelectMultiplierLowLevel));
                
                if (isHighLevel)
                {
                    long lastBetCredit = System.Convert.ToInt64(PlayerPrefs.GetString("LAST_BET_CREDIT", "0"));
                    int lastBetIndex = ChooseBetIndex(userCredit, lastBetCredit);
                    if (choosedBetIndex < lastBetIndex)
                        choosedBetIndex = lastBetIndex;
                }
                choosedBetIndex = 0;
            }

            MessageDispatcher.Dispatch("OnCreditEvent", new EventData<int>("UpdateBetIndex", choosedBetIndex));
        }

        private void UpdateGameSpin(long betCredit)
        {
            bool chooseGameSpin = false;

            if (IsEarlyAccess && BonusSpinDict.ContainsKey(betCredit))
            {
                int spinCount = BonusSpinDict[betCredit];
                if (spinCount > 0)
                {
                    GameSpinCount = spinCount;
                    SpinType = SpinType.BonusSpin;
                    chooseGameSpin = true;
                }
            }

            if (chooseGameSpin == false && EnableGameSpin && GameSpinDict.ContainsKey(betCredit))
            {
                int spinCount = GameSpinDict[betCredit];

                if (spinCount > 0)
                {
                    GameSpinCount = spinCount;
                    SpinType = SpinType.GameSpin;
                    chooseGameSpin = true;
                }
            }

            if (chooseGameSpin == false)
                SpinType = SpinType.None;
        }

        private int ChooseGameSpinBetIndex()
        {
            int chooseBetIndex = -1;

            if (IsEarlyAccess)
            {
                long choosedBetCredit = GetMaxGameSpinBet(BonusSpinDict);
                if (choosedBetCredit > 0)
                {
                    GameSpinCount = BonusSpinDict[choosedBetCredit];
                    SpinType = SpinType.BonusSpin;
                    chooseBetIndex = BetList.IndexOf(choosedBetCredit);
                }
            }

            if (chooseBetIndex == -1 && EnableGameSpin)
            {
                long choosedBetCredit = GetMaxGameSpinBet(GameSpinDict);
                if (choosedBetCredit > 0)
                {
                    GameSpinCount = GameSpinDict[choosedBetCredit];
                    SpinType = SpinType.GameSpin;
                    chooseBetIndex = BetList.IndexOf(choosedBetCredit);
                }
            }

            return chooseBetIndex;
        }

        private int ChooseBetIndex(long userCredit, long desiredCredit)
        {
            int choosedIndex = 0;
            int lastIndex = BetList.Count - 1;
            for (int i = 0; i < BetList.Count; ++i)
            {
                if (Level < GetBetToLevelRestriction(BetList[i]))
                    break;

                if ((desiredCredit < BetList[i]) && (i != lastIndex))
                    break;

                if (userCredit < desiredCredit)
                    break;

                choosedIndex = i;
            }

            return choosedIndex;
        }

        private long GetMaxGameSpinBet(Dictionary<long, int> target)
        {
            long maxGameSpinBet = 0L;
            foreach (KeyValuePair<long, int> data in target)
            {
                if (data.Value > 0 && maxGameSpinBet < data.Key && BetList.Contains(data.Key))
                    maxGameSpinBet = data.Key;
            }

            return maxGameSpinBet;
        }

        protected int GetBetToLevelRestriction(long betCredit)
        {
            int minLevelRestriction = 0;
            for (int i = 0; i < Restrictions.Count; ++i)
            {
                var minBet = Restrictions[i].GetValue<long>("minBet");
                if (betCredit >= minBet)
                    minLevelRestriction = Restrictions[i].GetValue<int>("minLevel");
                else
                    break;
            }

            return minLevelRestriction;
        }

        protected void OverrideExtraBetRatioIndex()
        {
            if (UseForcedExtraBetRatio)
            {
                int index = Mathf.Min(ForcedExtraBetRatioIndexList.Count - 1, BetIndex);
                ExtraBetRatioIndex = ForcedExtraBetRatioIndexList[index];
            }
        }

        protected long GetExtraBetCredit(long credit)
        {
            OverrideExtraBetRatioIndex();

            if (ticketedStatus == TicketedStatus.START)
                ticketedStatus = IsTicketBonusInBlackboard() ? TicketedStatus.CHECK_TICKET : TicketedStatus.END;
            else if (ticketedStatus == TicketedStatus.CHECK_TICKET)
            {
                ticketedStatus = TicketedStatus.END;
                if (IsTicketBonusInBlackboard())
                {
                    var ticketClaimBB = BlackboardUtils.FindVariable<Blackboard>(null, "./ticketClaim");
                    if (ticketClaimBB == null || ticketClaimBB.value == null)
                        return GetTicketBonusExtraBet(credit);
                }
            }

            var extraBetNumerator = ExtraBetRatioList[ExtraBetRatioIndex].GetValue<int>("numerator");
            var extraBetDenominator = ExtraBetRatioList[ExtraBetRatioIndex].GetValue<int>("denominator");

            return credit * extraBetNumerator / extraBetDenominator;
        }
        // check [Ticketed Bonus Ticket] extraBet - 2022.01.28 Jungsik Choi
        private bool IsTicketBonusInBlackboard()
        {
            var ticketBonusBB = BlackboardUtils.FindVariable<Blackboard>(null, "./ticketBonus");
            return ticketBonusBB != null && ticketBonusBB.value != null;
        }

        private long GetTicketBonusExtraBet(long credit)
        {
            var prepareDataBB = BlackboardUtils.FindVariable<Blackboard>(null, "./ticketBonus/prepareData");
            var extraBetNumerator = ExtraBetRatioList[ExtraBetRatioIndex].GetValue<int>("numerator");
            var extraBetDenominator = ExtraBetRatioList[ExtraBetRatioIndex].GetValue<int>("denominator");
            long extraBet = credit * extraBetNumerator / extraBetDenominator;

            if (prepareDataBB != null && prepareDataBB.value != null && BlackboardUtils.FindValue<long>(prepareDataBB.value, "baseBet") == credit)
            {
                extraBet = BlackboardUtils.FindValue<long>(prepareDataBB.value, "extraBet");
                for (int i = 0; i < ExtraBetRatioList.Count; ++i)
                {
                    extraBetNumerator = ExtraBetRatioList[i].GetValue<int>("numerator");
                    extraBetDenominator = ExtraBetRatioList[i].GetValue<int>("denominator");
                    if (credit * extraBetNumerator / extraBetDenominator == extraBet)
                    {
                        ExtraBetRatioIndex = i;
                        break;
                    }
                }
            }

            return extraBet;
        }
    }
}
