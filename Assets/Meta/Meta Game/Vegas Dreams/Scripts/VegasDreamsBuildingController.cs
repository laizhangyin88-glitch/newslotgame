using UnityEngine;
using SlotMaker;
using System.Collections.Generic;
using System;
using BagelCode.ClientModels;
using System.Collections;
using NodeCanvas.Framework;

namespace BagelCode.VegasDreams
{
    public class VegasDreamsBuildingController : EventMonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        public void InitProperty()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();

            root.UpdateContext(false);

            bb.AddVariable("index", typeof(int));
            bb.AddVariable("isExhibition", typeof(bool));

            OnUpdateBuilding();
        }

        public void OnUpdateBuilding()
        {
            var isExhibition = bb.GetValue<bool>("isExhibition");

            if (isExhibition)
            {
                var buildingBB = bb.GetValue<Blackboard>("building");
                var buildingPresetBB = bb.GetValue<Blackboard>("buildingPreset");

                var buildingIndex = buildingBB.GetValue<int>("buildingIndex");
                var objectIndexList = buildingPresetBB.GetValue<List<int>>("objectIndexList");
                
                for (int i = 1; i <= VegasDreams.Utils.BuildingMaxLevel; i++)
                {
                    var obj = ContextUtils.FindElement(root, $"Building {buildingIndex} Object {objectIndexList[i - 1]}", CHILDREN);
                    obj.GetComponent<Animator>().SetBool("isOn", false);
                }

                var level = buildingBB.GetValue<int>("level");
                for (int i = 1; i <= level; i++)
                {
                    var obj = ContextUtils.FindElement(root, $"Building {buildingIndex} Object {objectIndexList[i - 1]}", CHILDREN);
                    obj.GetComponent<Animator>().SetBool("isOn", true);
                }
            }
            else
            {
                var index = bb.GetValue<int>("index");
                var level = VegasDreams.Utils.GetBuildingLevel(index);

                for (int i = 1; i <= VegasDreams.Utils.BuildingMaxLevel; i++)
                {
                    var obj = ContextUtils.FindElement(root, $"Building {index + 1} Object {VegasDreams.Utils.GetBuildingObjectIndex(index)[i - 1]}", CHILDREN);
                    obj.GetComponent<Animator>().SetBool("isOn", false);
                }

                for (int i = 1; i <= level; i++)
                {
                    var obj = ContextUtils.FindElement(root, $"Building {index + 1} Object {VegasDreams.Utils.GetBuildingObjectIndex(index)[i - 1]}", CHILDREN);
                    obj.GetComponent<Animator>().SetBool("isOn", true);
                }
            }
        }
    }
}
