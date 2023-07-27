using System.Collections.Generic;
using SlotMaker;
using NodeCanvas.Framework;
using BagelCode.ClientModels;

namespace BagelCode.BossRaiders.Deal
{
    // Use Blackboard
    public partial class BossRaidersDealSceneController
    {
        private Blackboard dealBB;
        private List<Blackboard> wheelCandidateList;

        private void InitWheelCandidate()
        {
            if (wheelCandidateList != null && wheelCandidateList.Count > 0)
                wheelCandidateList.Clear();
            wheelCandidateList = dealBB.GetValue<List<Blackboard>>("wheelCandidateList");
        }

        private Blackboard GetWheelCandidate(int index)
        {
            if (wheelCandidateList == null || wheelCandidateList.Count == 0)
                InitWheelCandidate();

            if (index < 0 || index >= wheelCandidateList.Count)
                return null;
            return wheelCandidateList[index];
        }

        private Blackboard GetSpinResultInfoBB()
        {
            return dealBB.GetValue<Blackboard>("wheelResultInfo");
        }

        private Blackboard GetRoundBalanceInfoBB()
        {
            return dealBB?.GetValue<Blackboard>("roundBalanceInfo") ?? null;
        }

        private void SetRoundBalanceInfoBB(Blackboard bb)
        {
            Blackboard roundBalanceBB = GetRoundBalanceInfoBB();
            if (roundBalanceBB != null && bb != null)
            {
                BlackboardUtils.SetOrCreateValue(roundBalanceBB, "round", bb.GetValue<int>("round"));
                BlackboardUtils.SetOrCreateValue(roundBalanceBB, "hpCoin", bb.GetValue<long>("hpCoin"));
                BlackboardUtils.SetOrCreateValue(roundBalanceBB, "clearBonus", bb.GetValue<long>("clearBonus"));

                dealBB?.SetValue("remainingHp", bb.GetValue<long>("hpCoin"));
            }
        }

        private int GetCurrentBossRound()
        {
            Blackboard roundBalanceBB = GetRoundBalanceInfoBB();
            if (roundBalanceBB != null)
                return roundBalanceBB.GetValue<int>("round") + 1;
            return 1;
        }

        private long GetCurrentBossHP()
        {
            if (dealBB != null)
                return dealBB.GetValue<long>("remainingHp");
            return 0L;
        }

        private int GetBossIndex()
        {
            if (dealBB != null)
                return dealBB.GetVariable<int>("bossIndex")?.value ?? 0;
            return 0;
        }

        private int GetSpinResultAngle()
        {
            if (dealBB != null)
                return dealBB.GetValue<int>("wheelResultIndex");
            return 1;
        }

        private bool GetSpinResultIsBonus()
        {
            Blackboard resultBB = GetSpinResultInfoBB();
            if (resultBB != null)
                return resultBB.GetValue<BossRaidersWheelType>("type") == BossRaidersWheelType.BONUS;
            return false;
        }

        private BossRaidersHitType GetSpinResultHitType()
        {
            Blackboard resultBB = GetSpinResultInfoBB();
            if (resultBB != null && resultBB.GetValue<BossRaidersWheelType>("type") == BossRaidersWheelType.ATTACK)
                return resultBB.GetValue<BossRaidersHitType>("hitType");
            return BossRaidersHitType.UNKNOWN;
        }

        private long GetExpectedDamage()
        {
            return dealBB?.GetValue<long>("expectedDamage") ?? 0;
        }

        private int CheckBonusResultSpin()
        {
            if (GetSpinResultIsBonus())
            {
                Blackboard resultBB = GetSpinResultInfoBB();
                List<Blackboard> bonusInfoList = resultBB.GetValue<List<Blackboard>>("bonusInfoList");
                int bonusIndex = resultBB.GetValue<int>("bonusIndex");

                if (bonusInfoList != null && bonusInfoList.Count > 0)
                {
                    RewardType rewardType = bonusInfoList[bonusIndex].GetValue<RewardType>("type");
                    if (rewardType == RewardType.BOSS_RAIDERS_DEAL_SPIN)
                        return (int)bonusInfoList[bonusIndex].GetValue<long>("amount");
                }
            }
            return 0;
        }

        private RewardType GetBonusResultType()
        {
            if (GetSpinResultIsBonus())
            {
                Blackboard resultBB = GetSpinResultInfoBB();
                List<Blackboard> bonusInfoList = resultBB.GetValue<List<Blackboard>>("bonusInfoList");
                int bonusIndex = resultBB.GetValue<int>("bonusIndex");

                if (bonusInfoList != null && bonusInfoList.Count > 0)
                    return bonusInfoList[bonusIndex].GetValue<RewardType>("type");
            }

            return RewardType.CREDIT;
        }

        private void UpdateBossData(bool isBossAlive)
        {
            Blackboard resultBB = GetSpinResultInfoBB();
            if (resultBB != null)
            {
                long resultBossHP = resultBB.GetValue<long>("remainingHp");
                if (dealBB != null)
                    BlackboardUtils.SetOrCreateValue(dealBB, "remainingHp", resultBossHP);

                if (resultBossHP == 0)
                {
                    // next boss data update
                    SetRoundBalanceInfoBB(resultBB.GetValue<Blackboard>("nextRoundBalanceInfo"));
                    BlackboardUtils.SetOrCreateValue<int>(dealBB, "bossIndex", resultBB.GetValue<int>("nextRoundBossIndex"));
                }
                // Update top score
                Variable<long> credit = BlackboardUtils.GetOrCreateVariable<long>(dealBB, "credit");
                long earnCredit = resultBB.GetValue<long>("earnCredit");
                credit.value = resultBB.GetValue<long>("totalCredit");
                if (isBossAlive == false)
                    credit.value -= dealBB.GetValue<long>("prevClearBonus");
                BlackboardUtils.SetOrCreateValue(dealBB, "spinCount", resultBB.GetValue<int>("spinCount"));
            }
        }

        private void UpdateBonusReward()
        {
            Blackboard resultBB = GetSpinResultInfoBB();
            if (resultBB != null)
            {
                BlackboardUtils.SetOrCreateValue(dealBB, "credit", resultBB.GetValue<long>("totalCredit"));
                BlackboardUtils.SetOrCreateValue(dealBB, "gem", resultBB.GetValue<long>("totalGem"));
                BlackboardUtils.SetOrCreateValue(dealBB, "spinCount", resultBB.GetValue<int>("spinCount"));
            }
        }

        private void UpdateClearBonus()
        {
            Blackboard resultBB = GetSpinResultInfoBB();
            if (resultBB != null)
            {
                BlackboardUtils.SetOrCreateValue(dealBB, "prevCredit", dealBB.GetValue<long>("credit"));
                Variable<long> credit = BlackboardUtils.GetOrCreateVariable<long>(dealBB, "credit");
                credit.value = resultBB.GetValue<long>("totalCredit");
            }
        }

        private void BackupDealInfo()
        {
            BlackboardUtils.SetOrCreateValue(dealBB, "prevRound", GetCurrentBossRound());
            BlackboardUtils.SetOrCreateValue(dealBB, "prevClearBonus", GetRoundBalanceInfoBB()?.GetValue<long>("clearBonus") ?? 0L);
            BlackboardUtils.SetOrCreateValue(dealBB, "prevCredit", dealBB.GetValue<long>("credit"));
            BlackboardUtils.SetOrCreateValue(dealBB, "prevGem", dealBB.GetValue<long>("gem"));
        }
    }
}