using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using BagelCode.ClientModels;
using System.Linq;
using SlotMaker.Json;

namespace BagelCode.VegasDreams
{
    public static class VegasDreamsAnalytics
    {
        public static void click_button_vds(string buttonName, string contextId)
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();
            customData["button_name"] = buttonName;
            customData["context_id"] = contextId;
            Analytics.CustomEvent("client_click_button", customData);
        }

        public static void build_dream_building_information(string contextId)
        {
            var build_reward_star_level = VegasDreams.Utils.BuildingList.Select(x => x.GetValue<int>("level")).ToList();
            var building_name_list = VegasDreams.Utils.SeasonBuildingPresetList.Select(x => x.GetValue<string>("name")).ToList();
            var building_exp_list = VegasDreams.Utils.BuildingList.Select(x => x.GetValue<long>("exp")).ToList();

            var gurusBuilding = VegasDreams.Utils.GurusBuilding;

            Dictionary<string, object> customData = new Dictionary<string, object>();
            customData["context_id"] = contextId;
            customData["build_reward_star_level"] = build_reward_star_level;
            customData["depots"] = GetDepotJson();
            customData["wild_puzzle"] = VegasDreams.Utils.WildPuzzleCount;
            customData["season_id"] = VegasDreams.Utils.SeasonId;
            customData["theme_id"] = VegasDreams.Utils.SeasonThemeId;
            customData["theme_name"] = VegasDreams.Utils.SeasonName;
            customData["is_gurus_building_opened"] = gurusBuilding != null;
            customData["building_name_list"] = building_name_list;
            customData["building_exp_list"] = building_exp_list;
            customData["building_max_exp_list"] = GetBuildingMaxExpList();
            customData["gurus_building_exp"] = gurusBuilding?.GetValue<long>("exp") ?? 0;
            customData["gurus_building_level"] = gurusBuilding?.GetValue<int>("level") ?? 0;
            Analytics.CustomEvent("client_build_dream_building_information", customData);
        }

        public static void build_dream_exhibition_hall(string contextId, List<Blackboard> buildingPresetList, int selectedSeasonNumber)
        {
            var building_name_list = buildingPresetList.Select(x => x.GetValue<string>("name")).ToList();

            Dictionary<string, object> customData = new Dictionary<string, object>();
            customData["context_id"] = contextId;
            customData["building_name_list"] = building_name_list;
            customData["season_id"] = selectedSeasonNumber;
            Analytics.CustomEvent("client_build_dream_exhibition_hall", customData);
        }

        private static Dictionary<string, int> GetDepotJson()
        {
            var depots = VegasDreams.Utils.Depot;

            var data = new Dictionary<string, int>();

            foreach (var depot in depots.variables)
            {
                data[depot.Key] = depots.GetValue<int>(depot.Key);
            }

            return data;
        }

        private static List<long> GetBuildingMaxExpList()
        {
            var data = new List<long>();

            var buildingList = VegasDreams.Utils.BuildingList;

            for (int i = 0; i < buildingList.Count; i++)
            {
                var targetLevel = buildingList[i].GetValue<int>("level");
                var requireExp = VegasDreams.Utils.GetBuildingPresetData(i, targetLevel + 1, "exp");
                data.Add(requireExp);
            }

            return data;
        }
    }
}
