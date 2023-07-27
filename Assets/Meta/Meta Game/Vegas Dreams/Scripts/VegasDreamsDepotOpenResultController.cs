using UnityEngine;
using SlotMaker;
using System.Collections.Generic;
using System;
using BagelCode.ClientModels;
using System.Collections;
using NodeCanvas.Framework;

namespace BagelCode.VegasDreams
{
    public class VegasDreamsDepotOpenResultController : EventMonoBehaviour
    {
        private ContextElement root;
        private Animator anim;
        private Blackboard bb;

        private bool isInit = false;
        private long totalRewardCredit = 0;

        private const float delay = 0.25f;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private List<Animator> depots = new List<Animator>();
        private List<Animator> cells = new List<Animator>();
        
        private ContextElement topHorizontalArea;
        private ContextElement bottomHorizontalArea;

        public void InitProperty()
        {
            if (isInit) return;

            root = GetComponent<ContextElement>();
            anim = GetComponent<Animator>();
            bb = GetComponent<Blackboard>();

            root.UpdateContext(false);

            GSManager.Instance.GetHandler("UI_Purchase_Complete_Appear").Play();

            InitContents();
            MakeObjectCell();

            anim.SetBool("Active", true);
            StartCoroutine(SequenceAppearCells());

            isInit = true;
        }

        private void InitContents()
        {
            // Horizontal Area
            topHorizontalArea = ContextUtils.FindElement(root, "Horizontal Group 1", CHILDREN);
            bottomHorizontalArea = ContextUtils.FindElement(root, "Horizontal Group 2", CHILDREN);

            // Close Button
            var closeButtonElement = ContextUtils.FindElement(root, "Button Close", CHILDREN);
            MetaContextElementUtils.SetClickable(closeButtonElement,
                () => EventSender.SendEvent(gameObject, VegasDreams.Events.ON_CLOSE));
        }

        private void MakeObjectCell()
        {
            var resultList = VegasDreams.Utils.DepotOpenResultList;

            if (resultList.Count <= 2) // no need split logic
            {
                MetaContextElementUtils.SetActive(bottomHorizontalArea, false);

                foreach (var result in resultList)
                {
                    MakeObjectCellPrefab(result);
                }
            }
            else
            {
                var pivot = resultList.Count / 2;

                for (int i = 0; i < pivot; i++)
                {
                    MakeObjectCellPrefab(resultList[i]);
                }

                for (int i = pivot; i < resultList.Count; i++)
                {
                    MakeObjectCellPrefab(resultList[i], false);
                }
            }

            // Total Reward Coin Text
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Text Total Reward", "COMMA_STYLE_COIN", CHILDREN, totalRewardCredit);
            BlackboardQueryUtils.AddCoins(totalRewardCredit);
        }

        private void MakeObjectCellPrefab(Blackboard result, bool top = true)
        {
            var building = result.GetValue<Blackboard>("updatedBuilding");
            var index = building.GetValue<int>("buildingIndex");
            var level = building.GetValue<int>("level");
            var exp = building.GetValue<long>("exp");

            var earnedExp = result.GetValue<long>("earnedExp");
            var earnedCredit = result.GetValue<long>("earnedCredit");

            totalRewardCredit += earnedCredit;

            var cell = MetaObjectUtils.MakePrefab(
                VegasDreams.Defines.CONTENTS_BUNDLE,
                "Object Cell",
                top ? topHorizontalArea.transform : bottomHorizontalArea.transform
            );

            cells.Add(cell.GetComponent<Animator>());
            
            var cellElement = cell.GetComponent<ContextElement>();
            cellElement.UpdateContext(false);

            var objectArea = ContextUtils.FindElement(cellElement, "Object Area", CHILDREN);

            if (index != VegasDreams.Utils.GurusBuildingIndex)
            {
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
                gaugeBB.SetValue("earnedCredit", earnedCredit);

                // Exp Set
                MetaContextElementUtils.SimpleSetText(cellElement, "Text Building Exp", $"+{earnedExp}", CHILDREN);

                // Level Up Effect
                MetaContextElementUtils.SimpleSetActive(cellElement, "FX Area", earnedCredit > 0, CHILDREN);
            }
            else
            {
                var obj = MetaObjectUtils.MakePrefab(
                    VegasDreams.Utils.GetContentsSeasonBundle(),
                    $"Vegas Dreams Theme {VegasDreams.Utils.SeasonThemeId} Gurus Building Place",
                    objectArea.transform
                );

                var gaugeArea = ContextUtils.FindElement(cellElement, "Gauge Area", CHILDREN);
                var gauge = MetaObjectUtils.MakePrefab(
                    VegasDreams.Defines.CONTENTS_BUNDLE,
                    "Gauge Rarity Gurus",
                    gaugeArea.transform
                );
                var gaugeBB = gauge.GetComponent<Blackboard>();
                gaugeBB.SetValue("earnedCredit", earnedCredit);

                // Exp Set
                MetaContextElementUtils.SimpleSetText(cellElement, "Text Building Exp", $"+{earnedExp}", CHILDREN);

                // Level Up Effect
                MetaContextElementUtils.SimpleSetActive(cellElement, "FX Area", earnedCredit > 0, CHILDREN);
            }
        }

        private IEnumerator SequenceAppearCells()
        {
            foreach (var cell in cells)
            {
                cell.SetTrigger("isAppear");
                yield return new WaitForSeconds(delay);
            }

            if (totalRewardCredit == 0) anim.SetTrigger("Appear");
            else anim.SetTrigger("AppearReward");
        }

        public void Close(bool instantly)
        {
            MetaSystem.UnSubscribeBackButton(gameObject.GetHashCode());
            if (instantly)
            {
                Destroy(gameObject);
            }
            else
            {
                anim.SetTrigger("Close");
            }
        }
    }
}
