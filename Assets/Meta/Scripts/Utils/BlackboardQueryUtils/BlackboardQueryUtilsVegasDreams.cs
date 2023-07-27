using SlotMaker;
using BagelCode.ClientModels;
using ParadoxNotion;
using UnityEngine;
using NodeCanvas.Framework;
using System.Collections.Generic;

namespace BagelCode
{
    public static partial class BlackboardQueryUtils
    {
        public static void SetVegasDreamsActive(bool state)
        {
            var activeVar = BlackboardUtils.GetOrCreateVariable<bool>(MainBlackboard.Get(), "buildDreamInfo/active");

            activeVar.value = state;
            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, VegasDreams.VegasDreams.Events.ON_UPDATE_VEGAS_DREAMS_INFO);
        }

        public static bool IsVegasDreamsActive()
        {
            return BlackboardUtils.GetOrCreateVariable<bool>("/buildDreamInfo/active")?.value ?? false;
        }

        public static void UpdateVegasDreamsInfo(BuildDreamInfo vegasDreamsInfo, bool sendEvent = true)
        {
            if (vegasDreamsInfo == null) return;
            
            Blackboard vegasDreamsInfoBB = VegasDreams.VegasDreams.Utils.VegasDreamsInfo;
            ClientAPI2Blackboard.Serialize(vegasDreamsInfoBB, vegasDreamsInfo);

            if (sendEvent)
                EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, VegasDreams.VegasDreams.Events.ON_UPDATE_VEGAS_DREAMS_INFO);
        }

        public static void UpdateVegasDreamsTotalDepotCount(BuildDreamInfo vegasDreamsInfo)
        {
            if (vegasDreamsInfo == null)
            {
                BlackboardUtils.SetOrCreateValue(VegasDreams.VegasDreams.Utils.VegasDreamsInfo, "earnedDepotCount", 0);
                return;
            }

            Blackboard vegasDreamsInfoBB = VegasDreams.VegasDreams.Utils.VegasDreamsInfo;
            var earnedDepotCount = vegasDreamsInfo.totalDepotCount - vegasDreamsInfoBB.GetValue<int>("totalDepotCount");

            BlackboardUtils.SetOrCreateValue(VegasDreams.VegasDreams.Utils.VegasDreamsInfo, "earnedDepotCount", earnedDepotCount);
            UpdateVegasDreamsInfo(vegasDreamsInfo, false);
        }

        public static void UpdateVegasDreamsInfo(Blackboard updateInfoBB, bool sendEvent = true)
        {
            if (updateInfoBB == null) return;
            
            BuildDreamInfo vegasDreamsInfo = new BuildDreamInfo();
            vegasDreamsInfo.active = updateInfoBB.GetValue<bool>("active");
            vegasDreamsInfo.vipLoungeBenefitEndTimestamp = updateInfoBB.GetValue<long>("vipLoungeBenefitEndTimestamp");
            vegasDreamsInfo.seasonThemeId = updateInfoBB.GetValue<int>("seasonThemeId");
            vegasDreamsInfo.totalDepotCount = updateInfoBB.GetValue<int>("totalDepotCount");
            vegasDreamsInfo.seasonEndTimestamp = updateInfoBB.GetValue<long>("seasonEndTimestamp");
            vegasDreamsInfo.hideBadge = updateInfoBB.GetValue<bool>("hideBadge");

            UpdateVegasDreamsInfo(vegasDreamsInfo, sendEvent);
        }

        public static void UpdateDepotCount(Blackboard updateInfoBB)
        {
            // Update Depot Type Count
            var typeName = updateInfoBB.GetValue<DepotType>("depotType").ToString().ToLower();

            if (VegasDreams.VegasDreams.Utils.Depot != null)
                VegasDreams.VegasDreams.Utils.Depot[typeName] = updateInfoBB["totalCount"];

            EventSender.SendGlobalEvent(VegasDreams.VegasDreams.Events.ON_UPDATE_DEPOT);
        }

        public static void UpdateDepotCount(string type, int count)
        {
            if (VegasDreams.VegasDreams.Utils.Depot != null)
                VegasDreams.VegasDreams.Utils.Depot[type] = count;

            EventSender.SendGlobalEvent(VegasDreams.VegasDreams.Events.ON_UPDATE_DEPOT);
        }

        public static void UpdateWildPuzzleCount(Blackboard rewardResultBB)
        {
            var bb = VegasDreams.VegasDreams.Utils.MainSceneBlackboard;
            if (bb == null) return;
            
            bb.SetValue("wildPuzzleCount", rewardResultBB.GetValue<int>("totalCount"));

            EventSender.SendGlobalEvent(VegasDreams.VegasDreams.Events.ON_UPDATE_WILD_PUZZLE_COUNT);
        }

        public static void UpdateBuildingList(Blackboard updatedBuilding)
        {
            var buildingList = VegasDreams.VegasDreams.Utils.BuildingList;
            
            foreach (var building in buildingList)
            {
                var index = updatedBuilding.GetValue<int>("buildingIndex");
                var level = updatedBuilding.GetValue<int>("level");
                var exp = updatedBuilding.GetValue<long>("exp");

                if (building.GetValue<int>("buildingIndex") == index)
                {
                    building.SetValue("level", level);
                    building.SetValue("exp", exp);
                    break;
                }
            }

            EventSender.SendGlobalEvent(VegasDreams.VegasDreams.Events.ON_UPDATE_BUILDING);
        }

        public static void UpdateGurusBuilding(Blackboard updatedGurusBuilding)
        {
            var index = BlackboardUtils.FindVariable<int>(updatedGurusBuilding, "buildingIndex")?.value ?? VegasDreams.VegasDreams.Utils.GurusBuildingIndex;
            if (index != VegasDreams.VegasDreams.Utils.GurusBuildingIndex) return; // check gurus building

            GurusBuilding gurusBuilding = new GurusBuilding();
            gurusBuilding.level = updatedGurusBuilding.GetValue<int>("level");
            gurusBuilding.exp = updatedGurusBuilding.GetValue<long>("exp");
            
            var bb = VegasDreams.VegasDreams.Utils.GurusBuilding;
            if (bb == null)
                bb = BlackboardUtils.GetOrCreateBlackboard(VegasDreams.VegasDreams.Utils.MainSceneBlackboard, "gurusBuilding") as Blackboard;

            ClientAPI2Blackboard.Serialize(bb, gurusBuilding);
            
            EventSender.SendGlobalEvent(VegasDreams.VegasDreams.Events.ON_UPDATE_GURUS_BUILDING);
        }

        public static void UpdateGurusBuildingPreset(Blackboard updatedGurusBuildingPreset)
        {
            if (updatedGurusBuildingPreset == null) return;

            BuildingReward gurusBuildingPreset = new BuildingReward();
            gurusBuildingPreset.level = updatedGurusBuildingPreset.GetValue<int>("level");
            gurusBuildingPreset.exp = updatedGurusBuildingPreset.GetValue<long>("exp");
            gurusBuildingPreset.levelUpReward = updatedGurusBuildingPreset.GetValue<long>("levelUpReward");
            
            var bb = VegasDreams.VegasDreams.Utils.GurusBuildingPreset;
            if (bb == null)
                bb = BlackboardUtils.GetOrCreateBlackboard(VegasDreams.VegasDreams.Utils.MainSceneBlackboard, "gurusBuildingPreset") as Blackboard;

            ClientAPI2Blackboard.Serialize(bb, gurusBuildingPreset);
        }

        public static void UpdateDailyChestResponse(BuildDreamCollectDailyChestResponse response)
        {
            BlackboardQueryUtils.UpdateVIPLoungeInfo(response.vipLoungeInfo);
            BlackboardQueryUtils.UpdateDailyChestList(response.dailyChestList);

            EventSender.SendGlobalEvent(VegasDreams.VegasDreams.Events.ON_UPDATE_DEPOT);
            EventSender.SendGlobalEvent(VegasDreams.VegasDreams.Events.ON_UPDATE_DAILY_CHEST);
        }

        public static void UpdateDailyChestList(List<BuildDreamDailyChestPersonal> dailyChestList)
        {
            var bb = VegasDreams.VegasDreams.Utils.MainSceneBlackboard;

            BlackboardUtils.SetOrCreateList(bb, "dailyChestList", dailyChestList, ClientAPI2Blackboard.Serialize);
        }

        public static void UpdateGurusRanking(GurusRankPercentile gurusRankPercentile, bool forcedUpdate = true)
        {
            if (VegasDreams.VegasDreams.Utils.GurusBuilding == null) return;
            if (VegasDreams.VegasDreams.Utils.GurusRank != VegasDreams.VegasDreams.Utils.NewGurusRank || forcedUpdate)
            {
                var bb = VegasDreams.VegasDreams.Utils.MainSceneBlackboard;

                var gurusRankPercentileBB = BlackboardUtils.GetOrCreateBlackboard(bb, "gurusRankPercentile");

                ClientAPI2Blackboard.Serialize(gurusRankPercentileBB, gurusRankPercentile);
                EventSender.SendGlobalEvent(VegasDreams.VegasDreams.Events.ON_UDATE_GURUS_RANKING);
            }
        }

        public static void UpdateGurusRankingValue(GurusRankListItem item)
        {
            var bb = VegasDreams.VegasDreams.Utils.MainSceneBlackboard;

            GurusRankPercentile rank = new GurusRankPercentile();
            rank.rank = item.rank;
        }

        public static void UpdateExhibitionSeasonList()
        {
            if (VegasDreams.VegasDreams.Utils.ExhibitionSeasonList.Count == 0) return;

            EventSender.SendGlobalEvent(VegasDreams.VegasDreams.Events.ON_UDATE_EXIHIBITION_SEASON_LIST);
        }

        public static void UpdateRequestVegasDreamInfo(BuildDreamInfoResponse response)
        {
            if (response == null)
                return;
            var vipLoungeInfoBB = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "vipLoungeInfo");
            ClientAPI2Blackboard.Serialize(vipLoungeInfoBB, response.vipLoungeInfo);

            var buildDreamInfoBB = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "buildDreamInfo");
            ClientAPI2Blackboard.Serialize(buildDreamInfoBB, response.buildDreamInfo);
        }
    }
}
