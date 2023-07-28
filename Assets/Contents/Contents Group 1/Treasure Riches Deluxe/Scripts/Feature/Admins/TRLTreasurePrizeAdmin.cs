using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameStudio.Slot.TRL.Utillity;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using UnityEngine;
namespace GameStudio.Slot.TRL.FeatureAdmin
{
    public class TRLTreasurePrizeAdmin : TRLFeatureAdmin
    {
        public override string CONTENT_EVENT_PLAY_FEATURE => "TRLTreasurePrizeBonus";
        public override string CONTENT_EVENT_ON_FINISH_FEATURE => "TRLOnFinishTreasurePrizeBonus";
        public const string CONTENT_EVENT_ON_FINISH_PROFILE_LOAD = "TRLOnFinishProfileLoad";

        public TRLScatterMover scatterMover;
        public TRLTreasurePrizePopup treasurePrizePopup;
        public TRLSpotLeftWindow spotLeftWindow;
        public GameObject spotLeftButton;

        protected override void RegisterEventDelegates()
        {
            AddEventDelegate("TRLUpdateSpotLeftBySpinDataWithoutTreasurePrize", UpdateSpotLeftBySpinDataWithoutTreasurePrize);
            AddEventDelegate("TRLUpdateSpotLeftBySpinData", UpdateSpotLeftBySpinData);
            AddEventDelegate("TRLUpdateSpotLeftByBonusData", UpdateSpotLeftByBonusData);
            AddEventDelegate("TRLUpdateSpotLeftByBonusDataWithoutTreasurePrize", UpdateSpotLeftByBonusDataWithoutTreasurePrize);
            AddEventDelegate("TRLApplySpotLeftData", ApplySpotLeftData);
            AddEventDelegate("TRLInitalizeSpotLeft", InitalizeSpotLeft);
            AddEventDelegate("TRLEnableSpotLeftButton", OnEnableSpotLeftButton);
            AddEventDelegate("TRLDisableSpotLeftButton", OnDisableSpotLeftButton);
            AddEventDelegate("TRLRequestUpdateContentsStoreOnSpin", RequestUpdateContentsStoreOnSpin);
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
            SlotMachine slotMachine = TRLUtillity.TryGetGlobalBlackBoardVariable<GameObject>("./slotMachine").GetComponent<SlotMachine>();
            List<int> stackedColumns = TRLUtillity.TryGetGlobalBlackBoardVariable<List<int>>("./bonus/response/scatterStackColumns");
            long totalBetCredit = TRLUtillity.TryGetGlobalBlackBoardVariable<long>("./betCredit");
            float collectMultiplier = TRLUtillity.TryGetGlobalBlackBoardVariable<int>("./bonus/response/collectMultiplier");
            long collectAmount = TRLUtillity.TryGetGlobalBlackBoardVariable<long>("./bonus/response/collectAmount");
            int spotColumn = TRLUtillity.TryGetGlobalBlackBoardVariable<int>("./bonus/response/boardColumn");
            int spotRow = TRLUtillity.TryGetGlobalBlackBoardVariable<int>("./bonus/response/boardRow");

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
            TRLUtillity.PlaySound("Vox Spot End");
            SetActiveReelSymbols(slotMachine, true);
            spotLeftWindow.CloseWindow();
            yield return new WaitForSeconds(2f);
        }

        private void UpdateSpotLeftBySpinDataWithoutTreasurePrize(EventData eventData)
        {
            IBlackboard bb = ContentBlackboard.Get();
            var spin = bb.GetVariable<Blackboard>("spin").value;
            var response = ContentBlackboardUtils.GetBonusResponse(spin, 20001);
            if (response != null)
            {
                var collectData = TRLUtillity.TryGetGlobalBlackBoardVariable<List<Blackboard>>("./spin/response/communityCollectData");

                int row = TRLUtillity.TryGetLocalBlackBoardVariable<int>(response, "boardRow");
                int column = TRLUtillity.TryGetLocalBlackBoardVariable<int>(response, "boardColumn");

                List<Blackboard> rowSpots = TRLUtillity.TryGetLocalBlackBoardVariable<List<Blackboard>>(collectData[row], "value");
                Blackboard colmnSpots = rowSpots[column];
                TRLUtillity.TrySetLocalBlackBoardVariable<bool>(colmnSpots, "collected", false);
            }

            spotLeftWindow.UpdateSpotLeft(TRLUtillity.TryGetGlobalBlackBoardVariable<List<Blackboard>>("./spin/response/communityCollectData"));
        }

        private void UpdateSpotLeftByBonusDataWithoutTreasurePrize(EventData eventData)
        {
            IBlackboard bb = ContentBlackboard.Get();
            var spin = bb.GetVariable<Blackboard>("spin").value;
            var treasurePrizeResponse = ContentBlackboardUtils.GetBonusResponse(spin, 20001);
            var treasureBonusResponse = ContentBlackboardUtils.GetBonusResponse(spin, 20002);
            if (treasurePrizeResponse != null)
            {
                var collectData = TRLUtillity.TryGetLocalBlackBoardVariable<List<Blackboard>>(treasureBonusResponse, "bonusGameData/collectData");

                int row = TRLUtillity.TryGetLocalBlackBoardVariable<int>(treasurePrizeResponse, "boardRow");
                int column = TRLUtillity.TryGetLocalBlackBoardVariable<int>(treasurePrizeResponse, "boardColumn");

                List<Blackboard> rowSpots = TRLUtillity.TryGetLocalBlackBoardVariable<List<Blackboard>>(collectData[row], "value");
                Blackboard colmnSpots = rowSpots[column];
                TRLUtillity.TrySetLocalBlackBoardVariable<bool>(colmnSpots, "collected", false);
                spotLeftWindow.UpdateSpotLeft(collectData);
            }

        }


        private void UpdateSpotLeftBySpinData(EventData eventData)
        {
            spotLeftWindow.UpdateSpotLeft(TRLUtillity.TryGetGlobalBlackBoardVariable<List<Blackboard>>("./spin/response/communityCollectData"));
        }
        private void UpdateSpotLeftByBonusData(EventData eventData)
        {
            IBlackboard bb = ContentBlackboard.Get();
            var spin = bb.GetVariable<Blackboard>("spin").value;
            var response = ContentBlackboardUtils.GetBonusResponse(spin, 20002);
            if (response != null)
            {
                spotLeftWindow.UpdateSpotLeft(TRLUtillity.TryGetLocalBlackBoardVariable<List<Blackboard>>(response, "bonusGameData/collectData"));
            }
        }
        private void ApplySpotLeftData(EventData eventData)
        {
            spotLeftWindow.ApplySpotsLeftData();
        }
        private void InitalizeSpotLeft(EventData eventData)
        {
            StartCoroutine(InitalizeSpotLeftCoroutine());
        }
        IEnumerator InitalizeSpotLeftCoroutine()
        {
            string initalizedContentsStorePath = "./game/contentsStoreInfo/community_game_data/collectData";
            AddLoadTaskUserProfileFromBoardData(TRLUtillity.TryGetGlobalBlackBoardVariable<List<Blackboard>>(initalizedContentsStorePath));

            spotLeftWindow.UpdateSpotLeft(TRLUtillity.TryGetGlobalBlackBoardVariable<List<Blackboard>>(initalizedContentsStorePath));

            TRLUtillity.SendEvent("OnContentEvent", CONTENT_EVENT_ON_FINISH_PROFILE_LOAD);
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
            var response = ContentBlackboardUtils.GetBonusResponse(spin, 20002);
            if (response != null)
            {
                AddLoadTaskUserProfileFromBoardData(
                TRLUtillity.TryGetLocalBlackBoardVariable<List<Blackboard>>(response, "bonusGameData/collectData"));
            }
            AddLoadTaskUserProfileFromBoardData(TRLUtillity.TryGetGlobalBlackBoardVariable<List<Blackboard>>("./spin/response/communityCollectData"));
            yield return new WaitUntil(() => TRLUserProfileManager.Instance.LoadFinish());
            TRLUtillity.SendEvent("OnContentEvent", CONTENT_EVENT_ON_FINISH_PROFILE_LOAD);
        }


        public void AddLoadTaskUserProfileFromBoardData(List<Blackboard> boardData)
        {
            List<string> userIdList = new List<string>();
            //Request 받은 컨텐츠 스토어 데이터를 기반으로 User Profile Image 로딩
            for (int i = 0; i < 3; i++)
            {
                List<Blackboard> rowSpots = TRLUtillity.TryGetLocalBlackBoardVariable<List<Blackboard>>(boardData[i], "value");
                for (int j = 0; j < 8; j++)
                {
                    Blackboard colmnSpots = rowSpots[j];
                    bool collected = TRLUtillity.TryGetLocalBlackBoardVariable<bool>(colmnSpots, "collected");
                    if (collected)
                    {
                        string userID = TRLUtillity.TryGetLocalBlackBoardVariable<string>(colmnSpots, "userId");
                        userIdList.Add(userID);
                    }
                }
            }
            TRLUserProfileManager.Instance.LoadUserProfileCoroutine(userIdList.Distinct().ToList());
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