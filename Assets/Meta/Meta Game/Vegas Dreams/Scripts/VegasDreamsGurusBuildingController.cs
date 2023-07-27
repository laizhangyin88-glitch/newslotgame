using UnityEngine;
using SlotMaker;
using System.Collections.Generic;
using System;
using BagelCode.ClientModels;
using System.Collections;
using NodeCanvas.Framework;

namespace BagelCode.VegasDreams
{
    public class VegasDreamsGurusBuildingController : EventMonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private ContextElement topArea;
        private ContextElement middleArea;
        private ContextElement bottomArea1;
        private ContextElement bottomArea2;
        private ContextElement bottomArea3;
        private ContextElement cloudArea;

        private List<GameObject> gurusObjects = new List<GameObject>();

        public void InitProperty()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();

            root.UpdateContext(false);

            topArea = ContextUtils.FindElement(root, "Top Area", CHILDREN);
            middleArea = ContextUtils.FindElement(root, "Middle Area", CHILDREN);
            bottomArea1 = ContextUtils.FindElement(root, "Bottom Area 1", CHILDREN);
            bottomArea2 = ContextUtils.FindElement(root, "Bottom Area 2", CHILDREN);
            bottomArea3 = ContextUtils.FindElement(root, "Bottom Area 3", CHILDREN);
            cloudArea = ContextUtils.FindElement(root, "Cloud Area", CHILDREN);

            OnUpdateGurusBuilding();
        }

        public void OnUpdateGurusBuilding()
        {
            if (VegasDreams.Utils.GurusBuilding == null) return;

            var level = VegasDreams.Utils.GurusBuilding.GetValue<int>("level");
            ReplaceObject(level);
        }

        public void OnUpdateLevelUpGurusBuilding()
        {
            var resultList = VegasDreams.Utils.DepotOpenResultList;
            var building = resultList[0]?.GetValue<Blackboard>("updatedBuilding");
            if (resultList.Count == 1 && building?.GetValue<int>("buildingIndex") == VegasDreams.Utils.GurusBuildingIndex)
            {
                var level = building.GetValue<int>("level");
                ReplaceObject(level);
            }
        }

        private void ReplaceObject(int level)
        {
            gurusObjects.ForEach((obj) => Destroy(obj));
            gurusObjects.Clear();

            switch (level)
            {
                case 0:
                    {
                        gurusObjects.Add(MetaObjectUtils.MakePrefab(VegasDreams.Utils.GetContentsSeasonBundle(),
                            $"Vegas Dreams Theme {VegasDreams.Utils.SeasonThemeId} Gurus Building Locked", middleArea.transform));
                    }
                    break;
                case 1:
                    {
                        gurusObjects.Add(MetaObjectUtils.MakePrefab(VegasDreams.Utils.GetContentsSeasonBundle(),
                            $"Vegas Dreams Theme {VegasDreams.Utils.SeasonThemeId} Gurus Building Top", topArea.transform));
                        gurusObjects.Add(MetaObjectUtils.MakePrefab(VegasDreams.Utils.GetContentsSeasonBundle(),
                            $"Vegas Dreams Theme {VegasDreams.Utils.SeasonThemeId} Gurus Building Middle", middleArea.transform));
                        gurusObjects.Add(MetaObjectUtils.MakePrefab(VegasDreams.Utils.GetContentsSeasonBundle(),
                            $"Vegas Dreams Theme {VegasDreams.Utils.SeasonThemeId} Gurus Building Bottom", bottomArea1.transform)); 
                    }
                    break;
                case 2:
                    {
                        gurusObjects.Add(MetaObjectUtils.MakePrefab(VegasDreams.Utils.GetContentsSeasonBundle(),
                            $"Vegas Dreams Theme {VegasDreams.Utils.SeasonThemeId} Gurus Building Top", topArea.transform));
                        gurusObjects.Add(MetaObjectUtils.MakePrefab(VegasDreams.Utils.GetContentsSeasonBundle(),
                            $"Vegas Dreams Theme {VegasDreams.Utils.SeasonThemeId} Gurus Building Middle", middleArea.transform));
                        gurusObjects.Add(MetaObjectUtils.MakePrefab(VegasDreams.Utils.GetContentsSeasonBundle(),
                            $"Vegas Dreams Theme {VegasDreams.Utils.SeasonThemeId} Gurus Building Middle", bottomArea1.transform));
                        gurusObjects.Add(MetaObjectUtils.MakePrefab(VegasDreams.Utils.GetContentsSeasonBundle(),
                            $"Vegas Dreams Theme {VegasDreams.Utils.SeasonThemeId} Gurus Building Bottom", bottomArea2.transform));
                    }
                    break;
                case 3:
                    {
                        gurusObjects.Add(MetaObjectUtils.MakePrefab(VegasDreams.Utils.GetContentsSeasonBundle(),
                            $"Vegas Dreams Theme {VegasDreams.Utils.SeasonThemeId} Gurus Building Top", topArea.transform));
                        gurusObjects.Add(MetaObjectUtils.MakePrefab(VegasDreams.Utils.GetContentsSeasonBundle(),
                            $"Vegas Dreams Theme {VegasDreams.Utils.SeasonThemeId} Gurus Building Middle", middleArea.transform));
                        gurusObjects.Add(MetaObjectUtils.MakePrefab(VegasDreams.Utils.GetContentsSeasonBundle(),
                            $"Vegas Dreams Theme {VegasDreams.Utils.SeasonThemeId} Gurus Building Middle", bottomArea1.transform));
                        gurusObjects.Add(MetaObjectUtils.MakePrefab(VegasDreams.Utils.GetContentsSeasonBundle(),
                            $"Vegas Dreams Theme {VegasDreams.Utils.SeasonThemeId} Gurus Building Middle", bottomArea2.transform));
                        gurusObjects.Add(MetaObjectUtils.MakePrefab(VegasDreams.Utils.GetContentsSeasonBundle(),
                            $"Vegas Dreams Theme {VegasDreams.Utils.SeasonThemeId} Gurus Building Bottom", bottomArea3.transform));
                    }
                    break;
                default:
                    {
                        gurusObjects.Add(MetaObjectUtils.MakePrefab(VegasDreams.Utils.GetContentsSeasonBundle(),
                            $"Vegas Dreams Theme {VegasDreams.Utils.SeasonThemeId} Gurus Building Top", topArea.transform));
                        gurusObjects.Add(MetaObjectUtils.MakePrefab(VegasDreams.Utils.GetContentsSeasonBundle(),
                            $"Vegas Dreams Theme {VegasDreams.Utils.SeasonThemeId} Gurus Building Middle", middleArea.transform));
                        gurusObjects.Add(MetaObjectUtils.MakePrefab(VegasDreams.Utils.GetContentsSeasonBundle(),
                            $"Vegas Dreams Theme {VegasDreams.Utils.SeasonThemeId} Gurus Building Middle", bottomArea1.transform));
                        gurusObjects.Add(MetaObjectUtils.MakePrefab(VegasDreams.Utils.GetContentsSeasonBundle(),
                            $"Vegas Dreams Theme {VegasDreams.Utils.SeasonThemeId} Gurus Building Middle", bottomArea2.transform));
                        gurusObjects.Add(MetaObjectUtils.MakePrefab(VegasDreams.Utils.GetContentsSeasonBundle(),
                            $"Vegas Dreams Theme {VegasDreams.Utils.SeasonThemeId} Gurus Building Middle", bottomArea3.transform));
                        gurusObjects.Add(MetaObjectUtils.MakePrefab(VegasDreams.Defines.CONTENTS_BUNDLE,
                            "Vegas Dreams Gurus Cloud", cloudArea.transform));
                            //TODO : Cloud Make
                    }
                    break;
            }
        }
    }
}
