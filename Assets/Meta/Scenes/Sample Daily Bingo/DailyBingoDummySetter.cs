using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;



public class DailyBingoDummySetter : MonoBehaviour
{
#if UNITY_EDITOR
    public void InitDayilBingo()
    {
            // Daily Bingo Dummy Data. ///////////////////////////
            DailyBingoInfo dummyInfo = new DailyBingoInfo();
            dummyInfo.board = new DailyBingoBoard();
            dummyInfo.board.bingoRewardList = new List<RewardInfo>();
            dummyInfo.rewardList = new List<RewardResult>();

            dummyInfo.board.lastCollectIndex = 4;
            dummyInfo.board.creditMultiplier = 1000;
            dummyInfo.board.daysLeft = 5;
            dummyInfo.board.backgroundUrl = "http://v3.bagelcode.com:20001/assets/SLOTS1/images/loyalty_stamp/tinify/Bingo Background.png";
            dummyInfo.board.totalWorth = 1000000;

            // Cells
            for(int i=0; i<25; ++i)
            {
                DailyBingoBoardCell boardCellInfo = new DailyBingoBoardCell();

                if(i == 12 || i == 8)
                {
                    boardCellInfo.free = true;
                    boardCellInfo.highlight = false;
                    boardCellInfo.credit = 0;
                }
                else if(i == 24)
                {
                    boardCellInfo.free = false;
                    boardCellInfo.highlight = false;
                    boardCellInfo.credit = 100;
                }
                else if(i%2 == 0)
                {
                    boardCellInfo.free = false;
                    boardCellInfo.highlight = true;
                    boardCellInfo.credit = 20;
                }
                else
                {
                    boardCellInfo.free = false;
                    boardCellInfo.highlight = false;
                    boardCellInfo.credit = 10;
                }

                dummyInfo.board.cells.Add(boardCellInfo);
            }

            // Rewards
            RewardInfoValueCredit creditInfo = new RewardInfoValueCredit();
            creditInfo.credit = 20000;

            RewardInfoValueDailyBonusWheelSpin rewardDailyBonusWheel = new RewardInfoValueDailyBonusWheelSpin();
            rewardDailyBonusWheel.spinCount = 10;

            RewardInfoValueDailyBoost rewardDailyBoost = new RewardInfoValueDailyBoost();
            rewardDailyBoost.baseCreditPerDay = 10000;
            rewardDailyBoost.totalDayCount = 5;

            RewardInfoValueGameSpin rewardGameSpin = new RewardInfoValueGameSpin();
            rewardGameSpin.gameId = 2;
            rewardGameSpin.spinCount = 10;
            rewardGameSpin.bet = 500;

            RewardInfoValueRp rewardRP = new RewardInfoValueRp();
            rewardRP.rp = 20000;

            RewardInfoValueTierUpgrade rewardTierUpgrade = new RewardInfoValueTierUpgrade();
            rewardTierUpgrade.targetTier = 11;
            

            RewardInfo creditRewardInfo = new RewardInfo();
            creditRewardInfo.type = RewardType.CREDIT;
            creditRewardInfo.value = creditInfo;

            RewardInfo rewardRPInfo = new RewardInfo();
            rewardRPInfo.type = RewardType.RP;
            rewardRPInfo.value = rewardRP;

            RewardInfo rewardDailyBonusWheelInfo = new RewardInfo();
            rewardDailyBonusWheelInfo.type = RewardType.DAILY_BONUS_WHEEL_SPIN;
            rewardDailyBonusWheelInfo.value = rewardDailyBonusWheel;

            RewardInfo rewardGameSpinInfo = new RewardInfo();
            rewardGameSpinInfo.type = RewardType.GAME_SPIN;
            rewardGameSpinInfo.value = rewardGameSpin;

            RewardInfo rewardDailyBoostInfo = new RewardInfo();
            rewardDailyBoostInfo.type = RewardType.DAILY_BOOST;
            rewardDailyBoostInfo.value = rewardDailyBoost;

            RewardInfo rewardTierUpInfo = new RewardInfo();
            rewardTierUpInfo.type = RewardType.TIER_UPGRADE;
            rewardTierUpInfo.value = rewardTierUpgrade;

            dummyInfo.board.bingoRewardList.Add(creditRewardInfo);
            dummyInfo.board.bingoRewardList.Add(rewardRPInfo);
            dummyInfo.board.bingoRewardList.Add(rewardDailyBonusWheelInfo);
            dummyInfo.board.bingoRewardList.Add(rewardGameSpinInfo);
            dummyInfo.board.bingoRewardList.Add(rewardDailyBoostInfo);
            dummyInfo.board.bingoRewardList.Add(rewardTierUpInfo);
            dummyInfo.board.bingoRewardList.Add(creditRewardInfo);
            dummyInfo.board.bingoRewardList.Add(rewardRPInfo);
            dummyInfo.board.bingoRewardList.Add(rewardDailyBoostInfo);
            dummyInfo.board.bingoRewardList.Add(rewardDailyBonusWheelInfo);
            dummyInfo.board.bingoRewardList.Add(rewardGameSpinInfo);
            dummyInfo.board.bingoRewardList.Add(rewardTierUpInfo);

            // Color Settings
            dummyInfo.board.colorSetting = new DailyBingoColorSetting();
            dummyInfo.board.colorSetting.todayTextColor = "ff00ff";
            dummyInfo.board.colorSetting.cellTextColor = "0000ff";
            dummyInfo.board.colorSetting.cellHighlightTextColor = "00ffff";
            dummyInfo.board.colorSetting.cellOverlayColor = "000000";
            dummyInfo.board.colorSetting.stampTextColor = "333333";
            dummyInfo.board.colorSetting.highlightStrokeColor = "88ff88";
            dummyInfo.board.colorSetting.descriptionTextColor = "222222";
            dummyInfo.board.colorSetting.daysLeftTextColor = "ff0000";
            dummyInfo.board.colorSetting.totalWinTextColor = "00ffff";
            dummyInfo.board.colorSetting.rewardTextColor = "00ffff";

            // response.dailyBingoInfo = dummyInfo;
            RewardResultCredit rewardResultCredit = new RewardResultCredit();
            rewardResultCredit.credit = 1200;

            RewardResult rewardResultInfo = new RewardResult();
            rewardResultInfo.rewardType = RewardType.CREDIT;
            rewardResultInfo.rewardResult = rewardResultCredit;

            dummyInfo.rewardList.Add(rewardResultInfo);
            // dummyInfo.rewardList.Clear();

            var dailyBingoBB = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "dailyBingoInfo");
            ClientAPI2Blackboard.Serialize(dailyBingoBB, dummyInfo);

            //////////////////////////////////////////////////////
    }
#endif
}


