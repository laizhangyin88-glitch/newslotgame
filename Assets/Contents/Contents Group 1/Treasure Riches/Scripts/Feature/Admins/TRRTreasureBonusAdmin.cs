using System.Collections;
using System.Collections.Generic;
using BagelCode.Slots.TRR.Popup;
using BagelCode.Slots.TRR.Utillity;
using BagelCode.Tasks.Actions.Contents;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;
namespace BagelCode.Slots.TRR.FeatureAdmin
{
    public class TRRTreasureBonusAdmin : TRRFeatureAdmin
    {
        public override string CONTENT_EVENT_PLAY_FEATURE => "TRRTreasureBonus";
        public override string CONTENT_EVENT_ON_FINISH_FEATURE => "TRROnFinishTreasureBonus";

        //For SignBoard (Slot Event)
        private const string SLOT_EVENT_SIGNBOARD_BEGIN_TREASUREBONUS = "TRRSignBoardBeginTreasureBonus";
        private const string SLOT_EVENT_SIGNBOARD_TREASUREBONUS_PAY = "TRRSignBoardTreasureBonusPay";
        private const string SLOT_EVENT_SIGNBOARD_TREASUREBONUS_BEGIN_ROUND = "TRRSignBoardTreasureBonusBeginRound";
        private const string SLOT_EVENT_SIGNBOARD_TREASUREBONUS_BEGIN_FINALROUND = "TRRSignBoardTreasureBonusBeginFinalRound";
        private const string SLOT_EVENT_SIGNBOARD_FINISH_TREASUREBONUS = "TRRSignBoardFinishTreasureBonus";

        [SerializeField] private TRRTreasureBonusWindow treasureBonusWindow;
        [SerializeField] private TRRSpotLeftWindow spotLeftWindow;

        [Space(20)]
        [Header("POPUP")]
        [SerializeField] private TRRRoundNumberPopup roundNumberPopup;
        [SerializeField] private TRRRoundPrizePopup roundPrizePopup;
        [SerializeField] private TRRTopWinnerPopup topWinnerPopup;
        [SerializeField] private TRRTreasureBonusResultPopup resultPopup;
        [SerializeField] private TRRTreasureBonusTriggerPopup triggerPopup;

        private long accCredit;

        protected override void RegisterEventDelegates()
        {
        }

        protected override IEnumerator OnFeaturePlayCoroutine()
        {
            spotLeftWindow.SetLeftCount(0);
            yield return new WaitForSeconds(2f);
            TRRUtillity.SendEvent(SendEvent.ON_WIN_EVENT, "SkipWin");
            TRRUtillity.SendEvent(SendEvent.ON_SLOT_EVENT, SLOT_EVENT_SIGNBOARD_BEGIN_TREASUREBONUS);

            //Wait Trigger Popup
            yield return ShowDefualtPopupCoroutine(triggerPopup);
            TRRUtillity.PlayBonusBGM();

            accCredit = 0;

            TRRUtillity.SendEvent(SendEvent.ON_CONTENT_UI_EVENT, "TRRCloseSpotLeftWindow");
            TRRUtillity.SendEvent(SendEvent.ON_CONTENT_UI_EVENT, "TRROpenTreasureBonusWindow");
            yield return new WaitForSeconds(4f);

            yield return new WaitForSeconds(2f);

            Blackboard bonusResult = TRRUtillity.TryGetGlobalBlackBoardVariable<Blackboard>("./bonus/response");

            List<Blackboard> frameMovementData = TRRUtillity.TryGetLocalBlackBoardVariable<List<Blackboard>>(bonusResult, "frameMovementData");
            treasureBonusWindow.visitedIndexList.Clear();
            for (int i = 0; i < 14; i++)
                treasureBonusWindow.visitedIndexList.Add(false);
            for (int i = 0; i < 6; i++)
            {
                treasureBonusWindow.ActiveRoundMultiplier(i + 1);
                if (i == 5) treasureBonusWindow.ActiveRoundMultiplier(7);
                TRRUtillity.TrySetGlobalBlackBoardVariable<int>("./customData/currentRound", i + 1);

                if (i == 5)
                    TRRUtillity.SendEvent(SendEvent.ON_SLOT_EVENT, SLOT_EVENT_SIGNBOARD_TREASUREBONUS_BEGIN_FINALROUND);
                else
                    TRRUtillity.SendEvent<int>(SendEvent.ON_SLOT_EVENT, SLOT_EVENT_SIGNBOARD_TREASUREBONUS_BEGIN_ROUND, i + 1);

                yield return ShowDefualtPopupCoroutine(roundNumberPopup, () =>
                {
                    roundNumberPopup.currentRound = i + 1;
                });

                Blackboard roundData = frameMovementData[i];
                List<int> verticalFrameIndex = TRRUtillity.TryGetLocalBlackBoardVariable<List<int>>(roundData, "verticalFrameIndex");
                List<int> horizontalFrameIndex = TRRUtillity.TryGetLocalBlackBoardVariable<List<int>>(roundData, "horizontalFrameIndex");

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
                    List<int> finalVerticalFrameIndex = TRRUtillity.TryGetLocalBlackBoardVariable<List<int>>(finalRoundData, "verticalFrameIndex");
                    List<int> finalHorizontalFrameIndex = TRRUtillity.TryGetLocalBlackBoardVariable<List<int>>(finalRoundData, "horizontalFrameIndex");
                    foreach (var each in finalVerticalFrameIndex)
                        targets.Add(CalcChooseFrameIndexFromVerticalIndex(each));
                    foreach (var each in finalHorizontalFrameIndex)
                        targets.Add(CalcChooseFrameIndexFromHorizontalIndex(each));
                    treasureBonusWindow.MovementFrame(targets.Count, targets.ToArray(), i + 2, moveCount);
                }


                yield return new WaitUntil(() => treasureBonusWindow.isFrameMoving == false);
                yield return new WaitForSeconds(0.75f);

                TRRUtillity.PlaySound("Multiplier Complete");
                foreach (var each in treasureBonusWindow.currentFrame)
                {
                    each.GetComponent<TRRTreasureBonusChooseFrame>().TriggerWin();
                }
                yield return new WaitForSeconds(1.5f);
                List<long> roundCreditData = TRRUtillity.TryGetLocalBlackBoardVariable<List<long>>(bonusResult, "earnCreditPerRound");
                Blackboard topWinnerData = TRRUtillity.TryGetLocalBlackBoardVariable<Blackboard>(bonusResult, "topWinnerData");

                string topWinnerId = TRRUtillity.TryGetLocalBlackBoardVariable<string>(topWinnerData, "userId");
                long topWinnerEarnCredit = TRRUtillity.TryGetLocalBlackBoardVariable<long>(topWinnerData, "earnCredit");

                //파이널 라운드일 경우
                if (i == 5)
                {
                    TRRUtillity.SendEvent<long>(SendEvent.ON_SLOT_EVENT, SLOT_EVENT_SIGNBOARD_TREASUREBONUS_PAY, roundCreditData[i] + roundCreditData[i + 1]);

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
                    TRRUtillity.SendEvent<long>(SendEvent.ON_SLOT_EVENT, SLOT_EVENT_SIGNBOARD_TREASUREBONUS_PAY, roundCreditData[i]);


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
                    each.GetComponent<TRRTreasureBonusChooseFrame>().InActiveElements();
                yield return new WaitForSeconds(0.5f);
                foreach (var each in treasureBonusWindow.currentFrame)
                    each.SetActive(false);
                treasureBonusWindow.InActiveRoundMultiplier(i + 1);
                if (i == 5) treasureBonusWindow.InActiveRoundMultiplier(7);

                yield return new WaitForSeconds(1.25f);
            }

            long earnCredit = TRRUtillity.TryGetGlobalBlackBoardVariable<long>("./bonus/response/earnCredit");
            TRRUtillity.StopBGM();
            string playerUserID = TRRUtillity.TryGetGlobalBlackBoardVariable<string>("/me/userId");
            TRRUtillity.SendEvent(SendEvent.ON_SLOT_EVENT, SLOT_EVENT_SIGNBOARD_FINISH_TREASUREBONUS);

            AddBonusEarnCredit addBonusEarnCredit = new AddBonusEarnCredit();
            addBonusEarnCredit.earnCredit = "./bonus/response/earnCredit";
            TRRUtillity.ExecuteAction(addBonusEarnCredit, this);
            TRRUtillity.SendEvent<bool>(SendEvent.ON_CREDIT_EVENT, "UpdateTurnCredit", false);
            yield return ShowDefualtPopupCoroutine(resultPopup, () =>
                                   {
                                       resultPopup.credit = earnCredit;
                                       resultPopup.userId = playerUserID;
                                   });
            IBlackboard bb = ContentBlackboard.Get();

            var spin = bb.GetVariable<Blackboard>("spin").value;
            bool checkBonus19901 = ContentBlackboardUtils.GetBonusResponse(spin, 19901);
            bool checkBonus19903 = ContentBlackboardUtils.GetBonusResponse(spin, 19903);
            if (checkBonus19903)
            {
                spotLeftWindow.UpdateSpotLeft(TRRUtillity.TryGetGlobalBlackBoardVariable<List<Blackboard>>("./spin/response/communityCollectData"));
            }
            else
            {
                if (checkBonus19901)
                {
                    var collectData = TRRUtillity.TryGetGlobalBlackBoardVariable<List<Blackboard>>("./spin/response/communityCollectData");
                    var response = ContentBlackboardUtils.GetBonusResponse(spin, 19901);
                    int row = TRRUtillity.TryGetLocalBlackBoardVariable<int>(response, "boardRow");
                    int column = TRRUtillity.TryGetLocalBlackBoardVariable<int>(response, "boardColumn");

                    List<Blackboard> rowSpots = TRRUtillity.TryGetLocalBlackBoardVariable<List<Blackboard>>(collectData[row], "value");
                    Blackboard colmnSpots = rowSpots[column];
                    TRRUtillity.TrySetLocalBlackBoardVariable<bool>(colmnSpots, "collected", false);

                    spotLeftWindow.UpdateSpotLeft(collectData);
                }
                else
                {
                    spotLeftWindow.UpdateSpotLeft(TRRUtillity.TryGetGlobalBlackBoardVariable<List<Blackboard>>("./spin/response/communityCollectData"));
                }
            }
            TRRUtillity.SendEvent(SendEvent.ON_CONTENT_UI_EVENT, "TRRCloseTreasureBonusWindow");
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