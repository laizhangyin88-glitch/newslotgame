using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BagelCode.ClientModels;
using SlotMaker;
using NodeCanvas.Framework;
using UnityEngine.UI;
using ParadoxNotion;

namespace BagelCode.VegasDreams
{
    public class VegasDreamsWildPuzzleSelectController : MonoBehaviour
    {
        private ContextElement root;
        private Animator anim;
        private Blackboard bb;

        private bool isInit = false;

        private float timer = 0;

        private ContextElement collectButtonElement;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private List<Animator> cells = new List<Animator>();

        public void InitProperty()
        {
            if (isInit) return;


            root = GetComponent<ContextElement>();
            anim = GetComponent<Animator>();
            bb = GetComponent<Blackboard>();

            root.UpdateContext(false);

            GSManager.Instance.GetHandler(VegasDreams.Defines.GET_WILD_DEPOT).Play();

            InitContents();

            isInit = true;
        }

        public void UpdateSelectObject(int index)
        {
            collectButtonElement.GetComponent<PIDButton>().interactable = true;

            foreach (var cell in cells)
            {
                cell.SetBool("isCheck", false);
            }

            cells[index - 1].SetBool("isCheck", true);
        }

        private void InitContents()
        {
            foreach (var building in VegasDreams.Utils.BuildingList)
            {
                MakeObjectCellPrefab(building);
            }

            var titleTextElement = ContextUtils.FindElement(root, "Anchor/Title Area/Text Title", FULL);
            MetaContextElementUtils.SetTextGlobal(titleTextElement, "VEGAS_DREAMS_WILD_PUZZLE_SELECT_TITLE", VegasDreams.Utils.WildDepotExp);

            var collectButtonTextElement = ContextUtils.FindElement(root, "Anchor/Bottom Area/Button Collect/Text", FULL);
            MetaContextElementUtils.SetTextGlobal(collectButtonTextElement, "VEGAS_DREAMS_WILD_PUZZLE_SELECT_COLLECT_BUTTON");

            collectButtonElement = ContextUtils.FindElement(root, "Anchor/Bottom Area/Button Collect", FULL);
            collectButtonElement.GetComponent<PIDButton>().interactable = false;
            MetaContextElementUtils.SetClickable(collectButtonElement,
                () => EventSender.SendEvent(gameObject, VegasDreams.Events.ON_Collect_WILD_PUZZLE_BUILDING));

            var closeButtonElement = ContextUtils.FindElement(root, "Anchor/Button Close", FULL);
            MetaContextElementUtils.SetClickable(closeButtonElement,
                () => EventSender.SendEvent(gameObject, VegasDreams.Events.ON_CLOSE));
        }

        private void MakeObjectCellPrefab(Blackboard building)
        {
            var index = building.GetValue<int>("buildingIndex");
            var level = building.GetValue<int>("level");
            var exp = building.GetValue<long>("exp");

            var parent = ContextUtils.FindElement(root, $"Anchor/Room Cell {index:00}", FULL);
            var cell = MetaObjectUtils.MakePrefab(
                VegasDreams.Defines.CONTENTS_BUNDLE,
                "Popup Vegas Dreams Wild Puzzle Add Exp Room Cell",
                parent.transform
            );

            cells.Add(cell.GetComponent<Animator>());

            var isMaxLevel = VegasDreams.Utils.GetBuildingLevel(index - 1) == VegasDreams.Defines.MAX_BUILDING_LEVEL;
            cell.GetComponent<PIDButton>().interactable = !isMaxLevel;


            var cellElement = cell.GetComponent<ContextElement>();
            MetaContextElementUtils.SetClickable(cellElement, () =>
            {
                if (isMaxLevel) return;
                
                var eventData = new EventData<int>(VegasDreams.Events.ON_CLICK_WILD_PUZZLE_BUILDING, index);
                EventSender.SendEvent(gameObject, eventData);
            });
            cellElement.UpdateContext(false);
            MetaContextElementUtils.SimpleSetActive(cellElement, "Checkbox Area", !isMaxLevel);
            MetaContextElementUtils.SimpleSetActive(cellElement, "Complete", isMaxLevel);

            var objectArea = ContextUtils.FindElement(cellElement, "Building Area", CHILDREN);
            var obj = MetaObjectUtils.MakePrefab(
                VegasDreams.Utils.GetObjectSeasonBundle(),
                $"Theme {VegasDreams.Utils.SeasonThemeId} Building {index}",
                objectArea.transform
            );
            obj.GetComponent<Blackboard>().SetValue("index", index - 1);

            var gaugeArea = ContextUtils.FindElement(cellElement, "Gauge Area", CHILDREN);
            var gauge = MetaObjectUtils.MakePrefab(
                VegasDreams.Defines.CONTENTS_BUNDLE,
                $"Gauge Rarity {VegasDreams.Utils.GetBuildingRank(index - 1)}",
                gaugeArea.transform
            );
            var gaugeBB = gauge.GetComponent<Blackboard>();
            gaugeBB.SetValue("index", index - 1);
        }
    }
}
