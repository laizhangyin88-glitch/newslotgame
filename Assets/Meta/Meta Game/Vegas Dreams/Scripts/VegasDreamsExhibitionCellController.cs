using System.Collections.Generic;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;
using System.Collections;
using BagelCode.ClientModels;
using System.Linq;
using ParadoxNotion.Services;
using BagelCode.OSA_Scroll;

using static BagelCode.InboxEvent;

namespace BagelCode.VegasDreams
{
    public class VegasDreamsExhibitionCellController : EventMonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;
        private Animator anim;

        private ContextElement objectAreaElement;
        private GameObject currentObject;

        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;
        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;

        private const StringTable.StringTableType GLOBAL = StringTable.StringTableType.Global;

        private bool isInit = false;

        public void InitProperty()
        {
            if (isInit) return;

            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            objectAreaElement = ContextUtils.FindElement(root, "Room Area", CHILDREN);
            // GameObject caller = ownerInboxController.gameObject;
            // bb.AddVariable("caller", caller);

            isInit = true;
        }

        public void UpdateView(ExhibitionModel model)
        {
            InitProperty();

            bb.SetValue("building", model.building);
            bb.SetValue("buildingPreset", model.buildingPreset);

            var buildingName = model.buildingPreset.GetValue<string>("name");
            var buildingIndex = model.building.GetValue<int>("buildingIndex");
            var themeId = model.themeId;

            MetaContextElementUtils.SimpleSetText(root, "Text Building Name", buildingName);

            Destroy(currentObject);

            currentObject = MetaObjectUtils.MakePrefab(
                $"mgvegasdreamstheme{themeId}obj",
                $"Theme {themeId} Building {buildingIndex}",
                objectAreaElement.transform
            );
            var objBB = currentObject.GetComponent<Blackboard>();
            objBB.SetValue("index", buildingIndex - 1);
            objBB.SetValue("isExhibition", true);
            objBB.SetValue("building", model.building);
            objBB.SetValue("buildingPreset", model.buildingPreset);
        }
    }
}
