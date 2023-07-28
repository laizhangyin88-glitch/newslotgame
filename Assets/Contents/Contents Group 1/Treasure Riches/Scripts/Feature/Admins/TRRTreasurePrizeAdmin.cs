using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BagelCode.Slots.TRR.Utillity;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using UnityEngine;
namespace BagelCode.Slots.TRR.FeatureAdmin
{
    public class TRRTreasurePrizeAdmin : TRRFeatureAdmin
    {
        public override string CONTENT_EVENT_PLAY_FEATURE => "TRRTreasurePrizeBonus";
        public override string CONTENT_EVENT_ON_FINISH_FEATURE => "TRROnFinishTreasurePrizeBonus";
        public const string CONTENT_EVENT_ON_FINISH_PROFILE_LOAD = "TRROnFinishProfileLoad";

        public TRRScatterMover scatterMover;
        public TRRTreasurePrizePopup treasurePrizePopup;
        public TRRSpotLeftWindow spotLeftWindow;
        public GameObject spotLeftButton;

        protected override void RegisterEventDelegates()
        {
            AddEventDelegate("TRRUpdateSpotLeftBySpinDataWithoutTreasurePrize", UpdateSpotLeftBySpinDataWithoutTreasurePrize);
            AddEventDelegate("TRRUpdateSpotLeftBySpinData", UpdateSpotLeftBySpinData);
            AddEventDelegate("TRRUpdateSpotLeftByBonusData", UpdateSpotLeftByBonusData);
            AddEventDelegate("TRRUpdateSpotLeftByBonusDataWithoutTreasurePrize", UpdateSpotLeftByBonusDataWithoutTreasurePrize);
            AddEventDelegate("TRRInitalizeSpotLeft", InitalizeSpotLeft);
            AddEventDelegate("TRRApplySpotLeft", ApplySpotLeft);
            AddEventDelegate("TRREnableSpotLeftButton", OnEnableSpotLeftButton);
            AddEventDelegate("TRRDisableSpotLeftButton", OnDisableSpotLeftButton);
            AddEventDelegate("TRRRequestUpdateContentsStoreOnSpin", RequestUpdateContentsStoreOnSpin);
        }

        private void OnEnableSpotLeftButton(EventData eventData)
        {
            spotLeftWindow.SetSpotLeftButtonActive(true);
        }

        private void OnDisableSpotLeftButton(EventData eventData)
        {
            spotLeftWindow.CloseWindow();
            spotLeftWindow.SetSpotLeftButtonActive(false);
        }

        protected override IEnumerator OnFeaturePlayCoroutine()
        {
            SlotMachine slotMachine = TRRUtillity.TryGetGlobalBlackBoardVariable<GameObject>("./slotMachine").GetComponent<SlotMachine>();
            List<int> stackedColumns = TRRUtillity.TryGetGlobalBlackBoardVariable<List<int>>("./bonus/response/scatterStackColumns");
            long totalBetCredit = TRRUtillity.TryGetGlobalBlackBoardVariable<long>("./betCredit");
            float collectMultiplier = TRRUtillity.TryGetGlobalBlackBoardVariable<int>("./bonus/response/collectMultiplier");
            long collectAmount = TRRUtillity.TryGetGlobalBlackBoardVariable<long>("./bonus/response/collectAmount");
            int spotColumn = TRRUtillity.TryGetGlobalBlackBoardVariable<int>("./bonus/response/boardColumn");
            int spotRow = TRRUtillity.TryGetGlobalBlackBoardVariable<int>("./bonus/response/boardRow");

            yield return new WaitForSeconds(0.75f);
            SetActiveReelSymbols(slotMachine, false); // 릴에 있는 심볼들을 끄기
            yield return scatterMover.MoveStackedScattersCoroutine(stackedColumns);
            yield return new WaitForSeconds(1.25f);
            //  yield return scatterMover.WinAnimationCoroutine();
            scatterMover.DisableScatterAnimators();

            spotLeftButton.gameObject.SetActive(false);
            yield return ShowCustomPopupCoroutine(treasurePrizePopup, () =>
            {
                treasurePrizePopup.totalBet = totalBetCredit;
                treasurePrizePopup.collectMultiplier = collectMultiplier;
                treasurePrizePopup.collectAmount = collectAmount;
                treasurePrizePopup.spotIndex = spotRow * 8 + spotColumn;
                treasurePrizePopup.stackedColumnCount = stackedColumns.Count;
            });
            spotLeftButton.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.25f);

            spotLeftWindow.OpenWindow();
            yield return new WaitForSeconds(2f);
            yield return spotLeftWindow.TreasurePrizeCollect();
            TRRUtillity.PlaySound("Vox Spot End");
            SetActiveReelSymbols(slotMachine, true);
            spotLeftWindow.CloseWindow();
            yield return new WaitForSeconds(2f);
        }

        private void UpdateSpotLeftBySpinDataWithoutTreasurePrize(EventData eventData)
        {
            IBlackboard bb = ContentBlackboard.Get();
            var spin = bb.GetVariable<Blackboard>("spin").value;
            var response = ContentBlackboardUtils.GetBonusResponse(spin, 19901);
            if (response != null)
            {
                var collectData = TRRUtillity.TryGetGlobalBlackBoardVariable<List<Blackboard>>("./spin/response/communityCollectData");

                int row = TRRUtillity.TryGetLocalBlackBoardVariable<int>(response, "boardRow");
                int column = TRRUtillity.TryGetLocalBlackBoardVariable<int>(response, "boardColumn");

                List<Blackboard> rowSpots = TRRUtillity.TryGetLocalBlackBoardVariable<List<Blackboard>>(collectData[row], "value");
                Blackboard colmnSpots = rowSpots[column];
                TRRUtillity.TrySetLocalBlackBoardVariable<bool>(colmnSpots, "collected", false);
            }

            spotLeftWindow.UpdateSpotLeft(TRRUtillity.TryGetGlobalBlackBoardVariable<List<Blackboard>>("./spin/response/communityCollectData"));
        }

        private void UpdateSpotLeftByBonusDataWithoutTreasurePrize(EventData eventData)
        {
            IBlackboard bb = ContentBlackboard.Get();
            var spin = bb.GetVariable<Blackboard>("spin").value;
            var treasurePrizeResponse = ContentBlackboardUtils.GetBonusResponse(spin, 19901);
            var treasureBonusResponse = ContentBlackboardUtils.GetBonusResponse(spin, 19902);
            if (treasurePrizeResponse != null)
            {
                var collectData = TRRUtillity.TryGetLocalBlackBoardVariable<List<Blackboard>>(treasureBonusResponse, "bonusGameData/collectData");

                int row = TRRUtillity.TryGetLocalBlackBoardVariable<int>(treasurePrizeResponse, "boardRow");
                int column = TRRUtillity.TryGetLocalBlackBoardVariable<int>(treasurePrizeResponse, "boardColumn");

                List<Blackboard> rowSpots = TRRUtillity.TryGetLocalBlackBoardVariable<List<Blackboard>>(collectData[row], "value");
                Blackboard colmnSpots = rowSpots[column];
                TRRUtillity.TrySetLocalBlackBoardVariable<bool>(colmnSpots, "collected", false);
                spotLeftWindow.UpdateSpotLeft(collectData);
            }

        }
        private void UpdateSpotLeftBySpinData(EventData eventData)
        {
            spotLeftWindow.UpdateSpotLeft(TRRUtillity.TryGetGlobalBlackBoardVariable<List<Blackboard>>("./spin/response/communityCollectData"));
        }
        private void UpdateSpotLeftByBonusData(EventData eventData)
        {
            IBlackboard bb = ContentBlackboard.Get();
            var spin = bb.GetVariable<Blackboard>("spin").value;
            var response = ContentBlackboardUtils.GetBonusResponse(spin, 19902);
            if (response != null)
            {
                spotLeftWindow.UpdateSpotLeft(TRRUtillity.TryGetLocalBlackBoardVariable<List<Blackboard>>(response, "bonusGameData/collectData"));
            }
        }

        private void InitalizeSpotLeft(EventData eventData)
        {
            StartCoroutine(InitalizeSpotLeftCoroutine());
        }

        private void ApplySpotLeft(EventData eventData)
        {
            spotLeftWindow.ApplySpotsLeftData();
        }
        IEnumerator InitalizeSpotLeftCoroutine()
        {
            string initalizedContentsStorePath = "./game/contentsStoreInfo/community_game_data/collectData";
            AddLoadTaskUserProfileFromBoardData(TRRUtillity.TryGetGlobalBlackBoardVariable<List<Blackboard>>(initalizedContentsStorePath));

            spotLeftWindow.UpdateSpotLeft(TRRUtillity.TryGetGlobalBlackBoardVariable<List<Blackboard>>(initalizedContentsStorePath));

            TRRUtillity.SendEvent("OnContentEvent", CONTENT_EVENT_ON_FINISH_PROFILE_LOAD);
            yield return null;
        }


        private void RequestUpdateContentsStoreOnSpin(EventData eventData)
        {
            StartCoroutine(RequestUpdateContentsStoreOnSpinCoroutine());
        }

        IEnumerator RequestUpdateContentsStoreOnSpinCoroutine()
        {
            IBlackboard bb = ContentBlackboard.Get();
            var spin = bb.GetVariable<Blackboard>("spin").value;
            var response = ContentBlackboardUtils.GetBonusResponse(spin, 19902);
            if (response != null)
            {
                AddLoadTaskUserProfileFromBoardData(
                TRRUtillity.TryGetLocalBlackBoardVariable<List<Blackboard>>(response, "bonusGameData/collectData"));
            }
            AddLoadTaskUserProfileFromBoardData(TRRUtillity.TryGetGlobalBlackBoardVariable<List<Blackboard>>("./spin/response/communityCollectData"));
            yield return new WaitUntil(() => TRRProfileManager.Instance.LoadFinish());
            TRRUtillity.SendEvent("OnContentEvent", CONTENT_EVENT_ON_FINISH_PROFILE_LOAD);
        }


        public void AddLoadTaskUserProfileFromBoardData(List<Blackboard> boardData)
        {
            List<string> userIdList = new List<string>();
            //Request 받은 컨텐츠 스토어 데이터를 기반으로 User Profile Image 로딩
            for (int i = 0; i < 3; i++)
            {
                List<Blackboard> rowSpots = TRRUtillity.TryGetLocalBlackBoardVariable<List<Blackboard>>(boardData[i], "value");
                for (int j = 0; j < 8; j++)
                {
                    Blackboard colmnSpots = rowSpots[j];
                    bool collected = TRRUtillity.TryGetLocalBlackBoardVariable<bool>(colmnSpots, "collected");
                    if (collected)
                    {
                        string userID = TRRUtillity.TryGetLocalBlackBoardVariable<string>(colmnSpots, "userId");
                        userIdList.Add(userID);
                    }
                }
            }
            TRRProfileManager.Instance.LoadUserProfileCoroutine(userIdList.Distinct().ToList());
        }


        public void SetActiveReelSymbols(SlotMachine slotMachine, bool setTo)
        {
            for (int i = 0; i < 5; i++)
            {
                foreach (var each in slotMachine.reels[i].symbols)
                {
                    each.gameObject.SetActive(setTo);
                    each.Play("Skip");
                }
            }
        }
    }
}