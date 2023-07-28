using System.Collections;
using System.Collections.Generic;
using BagelCode.Tasks.Actions.Contents;
using GameStudio.Slot.TRL.Popup;
using GameStudio.Slot.TRL.Utillity;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;
namespace GameStudio.Slot.TRL.FeatureAdmin
{
    public class TRLTreasureBonusAdmin : TRLFeatureAdmin
    {
        public override string CONTENT_EVENT_PLAY_FEATURE => "TRLTreasureBonus";
        public override string CONTENT_EVENT_ON_FINISH_FEATURE => "TRLOnFinishTreasureBonus";

        //For SignBoard (Slot Event)
        private const string SLOT_EVENT_SIGNBOARD_BEGIN_TREASUREBONUS = "TRLSignBoardBeginTreasureBonus";
        private const string SLOT_EVENT_SIGNBOARD_TREASUREBONUS_PAY = "TRLSignBoardTreasureBonusPay";
        private const string SLOT_EVENT_SIGNBOARD_TREASUREBONUS_BEGIN_ROUND = "TRLSignBoardTreasureBonusBeginRound";
        private const string SLOT_EVENT_SIGNBOARD_TREASUREBONUS_BEGIN_FINALROUND = "TRLSignBoardTreasureBonusBeginFinalRound";
        private const string SLOT_EVENT_SIGNBOARD_FINISH_TREASUREBONUS = "TRLSignBoardFinishTreasureBonus";

        [SerializeField] private TRLTreasureBonusWindow treasureBonusWindow;
        [SerializeField] private TRLSpotLeftWindow spotLeftWindow;

        [Space(20)]
        [Header("POPUP")]
        [SerializeField] private TRLRoundNumberPopup roundNumberPopup;
        [SerializeField] private TRLRoundPrizePopup roundPrizePopup;
        [SerializeField] private TRLTopWinnerPopup topWinnerPopup;
        [SerializeField] private TRLTreasureBonusResultPopup resultPopup;
        [SerializeField] private TRLTreasureBonusTriggerPopup triggerPopup;

        private long accCredit;

        protected override void RegisterEventDelegates()
        {
        }

        protected override IEnumerator OnFeaturePlayCoroutine()
        {
            spotLeftWindow.SetLeftCount(0);
            yield return new WaitForSeconds(2f);
            TRLUtillity.SendEvent(SendEvent.ON_WIN_EVENT, "SkipWin");
            TRLUtillity.SendEvent(SendEvent.ON_SLOT_EVENT, SLOT_EVENT_SIGNBOARD_BEGIN_TREASUREBONUS);

            //Wait Trigger Popup
            yield return ShowDefualtPopupCoroutine(triggerPopup);
            TRLUtillity.PlayBonusBGM();

            accCredit = 0;

            TRLUtillity.SendEvent(SendEvent.ON_CONTENT_UI_EVENT, "TRLCloseSpotLeftWindow");
            TRLUtillity.SendEvent(SendEvent.ON_CONTENT_UI_EVENT, "TRLOpenTreasureBonusWindow");
            yield return new WaitForSeconds(4f);

            yield return new WaitForSeconds(2f);

            Blackboard bonusResult = TRLUtillity.TryGetGlobalBlackBoardVariable<Blackboard>("./bonus/response");

            List<Blackboard> frameMovementData = TRLUtillity.TryGetLocalBlackBoardVariable<List<Blackboard>>(bonusResult, "frameMovementData");
            treasureBonusWindow.visitedIndexList.Clear();
            for (int i = 0; i < 14; i++)
                treasureBonusWindow.visitedIndexList.Add(false);
            for (int i = 0; i < 6; i++)
            {
                treasureBonusWindow.ActiveRoundMultiplier(i + 1);
                if (i == 5) treasureBonusWindow.ActiveRoundMultiplier(7);
                TRLUtillity.TrySetGlobalBlackBoardVariable<int>("./customData/currentRound", i + 1);

                if (i == 5)
                    TRLUtillity.SendEvent(SendEvent.ON_SLOT_EVENT, SLOT_EVENT_SIGNBOARD_TREASUREBONUS_BEGIN_FINALROUND);
                else
                    TRLUtillity.SendEvent<int>(SendEvent.ON_SLOT_EVENT, SLOT_EVENT_SIGNBOARD_TREASUREBONUS_BEGIN_ROUND, i + 1);

                yield return ShowDefualtPopupCoroutine(roundNumberPopup, () =>
                {
                    roundNumberPopup.currentRound = i + 1;
                });

                Blackboard roundData = frameMovementData[i];
                List<int> verticalFrameIndex = TRLUtillity.TryGetLocalBlackBoardVariable<List<int>>(roundData, "verticalFrameIndex");
                List<int> horizontalFrameIndex = TRLUtillity.TryGetLocalBlackBoardVariable<List<int>>(roundData, "horizontalFrameIndex");

                List<int> targets = new List<int>();
                foreach (var each in verticalFrameIndex)
                {
                    targets.Add(CalcChooseFrameIndexFromVerticalIndex(each));
                }
                foreach (var each in horizontalFrameIndex)
                {
                    targets.Add(CalcChooseFrameIndexFromHorizontalIndex(each));
                }
                if (i == 5)
                {
                    treasureBonusWindow.visitedIndexList[2] = true;
                    treasureBonusWindow.visitedIndexList[11] = true;
                    treasureBonusWindow.visitedIndexList[1] = true;
                    treasureBonusWindow.visitedIndexList[12] = true;
                    treasureBonusWindow.visitedIndexList[0] = true;
                    treasureBonusWindow.visitedIndexList[13] = true;
                }
                int moveCount = UnityEngine.Random.Range(9, 13);
                treasureBonusWindow.MovementFrame(targets.Count, targets.ToArray(), i + 1, moveCount);

                if (i == 5)
                {
                    targets.Clear();
                    Blackboard finalRoundData = frameMovementData[i + 1];
                    List<int> finalVerticalFrameIndex = TRLUtillity.TryGetLocalBlackBoardVariable<List<int>>(finalRoundData, "verticalFrameIndex");
                    List<int> finalHorizontalFrameIndex = TRLUtillity.TryGetLocalBlackBoardVariable<List<int>>(finalRoundData, "horizontalFrameIndex");
                    foreach (var each in finalVerticalFrameIndex)
                        targets.Add(CalcChooseFrameIndexFromVerticalIndex(each));
                    foreach (var each in finalHorizontalFrameIndex)
                        targets.Add(CalcChooseFrameIndexFromHorizontalIndex(each));
                    treasureBonusWindow.MovementFrame(targets.Count, targets.ToArray(), i + 2, moveCount);
                }


                yield return new WaitUntil(() => treasureBonusWindow.isFrameMoving == false);
                yield return new WaitForSeconds(0.75f);

                TRLUtillity.PlaySound("Multiplier Complete");
                foreach (var each in treasureBonusWindow.currentFrame)
                {
                    each.GetComponent<TRLTreasureBonusChooseFrame>().TriggerWin();
                }
                yield return new WaitForSeconds(1.5f);
                List<long> roundCreditData = TRLUtillity.TryGetLocalBlackBoardVariable<List<long>>(bonusResult, "earnCreditPerRound");
                Blackboard topWinnerData = TRLUtillity.TryGetLocalBlackBoardVariable<Blackboard>(bonusResult, "topWinnerData");

                string topWinnerId = TRLUtillity.TryGetLocalBlackBoardVariable<string>(topWinnerData, "userId");
                long topWinnerEarnCredit = TRLUtillity.TryGetLocalBlackBoardVariable<long>(topWinnerData, "earnCredit");

                //파이널 라운드일 경우
                if (i == 5)
                {
                    TRLUtillity.SendEvent<long>(SendEvent.ON_SLOT_EVENT, SLOT_EVENT_SIGNBOARD_TREASUREBONUS_PAY, roundCreditData[i] + roundCreditData[i + 1]);

                    // 탑 위너가 플레이어일 경우
                    if (roundCreditData[i + 1] > 0)
                    {
                        topWinnerPopup.gameObject.SetActive(true);

                        Coroutine totalWinCreditUpdate = treasureBonusWindow.TotalWinCreditDirectionCoroutine(1.75f, accCredit, accCredit + roundCreditData[i + 1] + roundCreditData[i]);
                        Coroutine topWinnerPopupCoroutine = ShowDefualtPopupCoroutine(topWinnerPopup, () =>
                        {
                            topWinnerPopup.userID = topWinnerId;
                            topWinnerPopup.credit = topWinnerEarnCredit;
                            topWinnerPopup.useCountingDirection = true;
                        });
                        yield return this.WaitAllCoroutine(totalWinCreditUpdate, topWinnerPopupCoroutine);
                        accCredit += roundCreditData[i];
                        topWinnerPopup.gameObject.SetActive(false);
                    }
                    // 탑위너가 플레이어가 아닐 경우
                    else if (roundCreditData[i] > 0)
                    {
                        Coroutine totalWinCreditUpdate = treasureBonusWindow.TotalWinCreditDirectionCoroutine(1.75f, accCredit, accCredit + roundCreditData[i + 1] + roundCreditData[i]);
                        Coroutine roundPrizePopupCoroutine = ShowDefualtPopupCoroutine(roundPrizePopup, () =>
                        {
                            roundPrizePopup.round = i + 1;
                            roundPrizePopup.roundCreditData = roundCreditData;
                        });
                        yield return this.WaitAllCoroutine(totalWinCreditUpdate, roundPrizePopupCoroutine);
                        accCredit += roundCreditData[i];
                        yield return new WaitForSeconds(0.75f);
                        yield return ShowDefualtPopupCoroutine(topWinnerPopup, () =>
                                        {
                                            topWinnerPopup.userID = topWinnerId;
                                            topWinnerPopup.credit = topWinnerEarnCredit;
                                            topWinnerPopup.useCountingDirection = false;
                                        });
                    }
                    else
                    {
                        Coroutine totalWinCreditUpdate = treasureBonusWindow.TotalWinCreditDirectionCoroutine(1.75f, accCredit, accCredit + roundCreditData[i + 1] + roundCreditData[i]);
                        Coroutine topWinnerPopupCoroutine = ShowDefualtPopupCoroutine(topWinnerPopup, () =>
                                        {
                                            topWinnerPopup.userID = topWinnerId;
                                            topWinnerPopup.credit = topWinnerEarnCredit;
                                            topWinnerPopup.useCountingDirection = false;
                                        });
                        yield return this.WaitAllCoroutine(totalWinCreditUpdate, topWinnerPopupCoroutine);
                        accCredit += roundCreditData[i];
                        yield return new WaitForSeconds(0.75f);
                    }
                }
                else if (roundCreditData[i] > 0)
                {
                    TRLUtillity.SendEvent<long>(SendEvent.ON_SLOT_EVENT, SLOT_EVENT_SIGNBOARD_TREASUREBONUS_PAY, roundCreditData[i]);


                    yield return this.WaitAllCoroutine(
                        treasureBonusWindow.TotalWinCreditDirectionCoroutine(1.75f, accCredit, accCredit + roundCreditData[i]),
                        ShowDefualtPopupCoroutine(roundPrizePopup, () =>
                    {
                        roundPrizePopup.round = i + 1;
                        roundPrizePopup.roundCreditData = roundCreditData;
                    }));
                    accCredit += roundCreditData[i];
                }

                yield return new WaitForSeconds(0.5f);
                foreach (var each in treasureBonusWindow.currentFrame)
                    each.GetComponent<TRLTreasureBonusChooseFrame>().InActiveElements();
                yield return new WaitForSeconds(0.5f);
                foreach (var each in treasureBonusWindow.currentFrame)
                    each.SetActive(false);
                treasureBonusWindow.InActiveRoundMultiplier(i + 1);
                if (i == 5) treasureBonusWindow.InActiveRoundMultiplier(7);

                yield return new WaitForSeconds(1.25f);
            }

            long earnCredit = TRLUtillity.TryGetGlobalBlackBoardVariable<long>("./bonus/response/earnCredit");
            TRLUtillity.StopBGM();
            string playerUserID = TRLUtillity.TryGetGlobalBlackBoardVariable<string>("/me/userId");
            TRLUtillity.SendEvent(SendEvent.ON_SLOT_EVENT, SLOT_EVENT_SIGNBOARD_FINISH_TREASUREBONUS);

            AddBonusEarnCredit addBonusEarnCredit = new AddBonusEarnCredit();
            addBonusEarnCredit.earnCredit = "./bonus/response/earnCredit";
            TRLUtillity.ExecuteAction(addBonusEarnCredit, this);
            TRLUtillity.SendEvent<bool>(SendEvent.ON_CREDIT_EVENT, "UpdateTurnCredit", false);

            yield return ShowDefualtPopupCoroutine(resultPopup, () =>
                        {
                            resultPopup.credit = earnCredit;
                            resultPopup.userId = playerUserID;
                        });
            IBlackboard bb = ContentBlackboard.Get();

            var spin = bb.GetVariable<Blackboard>("spin").value;
            bool checkBonus20001 = ContentBlackboardUtils.GetBonusResponse(spin, 20001);
            bool checkBonus20003 = ContentBlackboardUtils.GetBonusResponse(spin, 20003);
            if (checkBonus20003)
            {
                spotLeftWindow.UpdateSpotLeft(TRLUtillity.TryGetGlobalBlackBoardVariable<List<Blackboard>>("./spin/response/communityCollectData"));
            }
            else
            {
                if (checkBonus20001)
                {
                    var collectData = TRLUtillity.TryGetGlobalBlackBoardVariable<List<Blackboard>>("./spin/response/communityCollectData");
                    var response = ContentBlackboardUtils.GetBonusResponse(spin, 20001);
                    int row = TRLUtillity.TryGetLocalBlackBoardVariable<int>(response, "boardRow");
                    int column = TRLUtillity.TryGetLocalBlackBoardVariable<int>(response, "boardColumn");

                    List<Blackboard> rowSpots = TRLUtillity.TryGetLocalBlackBoardVariable<List<Blackboard>>(collectData[row], "value");
                    Blackboard colmnSpots = rowSpots[column];
                    TRLUtillity.TrySetLocalBlackBoardVariable<bool>(colmnSpots, "collected", false);

                    spotLeftWindow.UpdateSpotLeft(collectData);
                }
                else
                {
                    spotLeftWindow.UpdateSpotLeft(TRLUtillity.TryGetGlobalBlackBoardVariable<List<Blackboard>>("./spin/response/communityCollectData"));
                }
            }
            TRLUtillity.SendEvent(SendEvent.ON_CONTENT_UI_EVENT, "TRLCloseTreasureBonusWindow");
        }

        int CalcChooseFrameIndexFromVerticalIndex(int verticalIndex)
        {
            return 3 + verticalIndex;
        }

        int CalcChooseFrameIndexFromHorizontalIndex(int horizontalIndex)
        {
            int direction = UnityEngine.Random.Range(0, 100) >= 50 ? 1 : -1;

            if (direction == -1)
                return 2 - horizontalIndex;

            return 11 + horizontalIndex;
        }
    }
}