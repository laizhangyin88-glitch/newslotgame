using UnityEngine;
using SlotMaker;
using System.Collections.Generic;
using System;
using BagelCode.ClientModels;
using System.Collections;
using NodeCanvas.Framework;
using System.Linq;

namespace BagelCode.VegasDreams
{
    public class VegasDreamsDailyChestPopupController : EventMonoBehaviour
    {
        private int BUILDING_MAX_COUNT = 8;

        private ContextElement root;
        private Blackboard bb;
        private Animator anim;
        private GameObject caller;

        private ContextElement collectAllButton;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private List<Animator> depots = new List<Animator>();

        private string contextId;

        public void InitProperty()
        {
            root = GetComponent<ContextElement>();
            anim = GetComponent<Animator>();
            bb = GetComponent<Blackboard>();
            caller = bb.GetValue<GameObject>("caller");

            root.UpdateContext(false);

            MakeDailyChestCell();
            InitContents();
        }

        private void InitContents()
        {
            collectAllButton = ContextUtils.FindElement(root, "Button Collect", CHILDREN);

            MetaContextElementUtils.SetClickable(collectAllButton, () =>
            {
                VegasDreamsAnalytics.click_button_vds("GIFT_COLLECT_ALL", contextId);
                EventSender.SendEvent(gameObject, VegasDreams.Events.ON_CLICK_COLLECT_ALL_DAILY_CHEST);
                GSManager.Instance.GetHandler("UI_Button_Normal").Play();
            });
            
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Button Collect/Text", "BUTTON_COLLECT_ALL", FULL);

            MetaContextElementUtils.SimpleSetClickable(root, "Button Close", () =>
            {
                EventSender.SendEvent(gameObject, VegasDreams.Events.ON_CLOSE);
            });
        }

        public void UpdateCollectAllButton()
        {
            var chestList = VegasDreams.Utils.DailyChestList;
            if (chestList == null) return;

            var isCollectable = chestList.Any((chest) => 
            {
                var lastCollectTimestamp = chest.GetValue<long>("lastCollectTimestamp");
                var nextCollectTimestamp = lastCollectTimestamp + VegasDreams.Utils.DailyChestCooltime;
                return TimeUtils.GetTimeStamp() > nextCollectTimestamp;
            });
            collectAllButton.GetComponent<PIDButton>().interactable = isCollectable;
        }

        public void CollectDailyChest(int index)
        {
            BagelCodeClientAPI.RequestVegasDreamCollectDailyChest(index,
                (response) =>
                {
                    BlackboardQueryUtils.UpdateDailyChestResponse(response);
                    BlackboardQueryUtils.UpdateVegasDreamsInfo(response.buildDreamInfo);

                    string bundle = VegasDreams.Defines.CONTENTS_BUNDLE;

                    string asset = "Popup Vegas Dreams Daily Chest Total Rewards Scene";
                    Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

                    var popupObj = MetaObjectUtils.MakeScene(bundle, asset, parent);

                    var popupBB = popupObj.GetComponent<Blackboard>();
                    ClientAPI2Blackboard.Serialize(popupBB, response);

                    // string contextID = BlackboardUtils.GetOrCreateVariable<string>(bb, "contextID")?.value;
                    // BlackboardUtils.SetOrCreateValue(popupBB, "contextID", contextID);

                    MetaObjectUtils.SetCalleeCaller(popupObj, gameObject);
                    MetaPopupUtils.OpenPopup(popupObj);
                },
                (error) =>
                {
                    switch (error.errorCode)
                    {
                        case Error.BUILD_DREAM_NOT_ACTIVE_ERROR:
                            VegasDreamsErrorHandler.OpenVegasDreamsEndPopup(() => EventSender.SendEvent(gameObject, VegasDreams.Events.ON_VEGAS_DREAMS_END_CALLBACK));
                            break;
                        default:
                            GlobalErrorHandler.GlobalError(error);
                            break;
                    }
                });
        }

        public void CollectAllDailyChest()
        {
            BagelCodeClientAPI.RequestVegasDreamCollectAllDailyChest(
                (response) => 
                {
                    BlackboardQueryUtils.UpdateDailyChestResponse(response);
                    BlackboardQueryUtils.UpdateVegasDreamsInfo(response.buildDreamInfo);

                    string bundle = VegasDreams.Defines.CONTENTS_BUNDLE;

                    string asset = "Popup Vegas Dreams Daily Chest Total Rewards Scene";
                    Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

                    var popupObj = MetaObjectUtils.MakeScene(bundle, asset, parent);

                    var popupBB = popupObj.GetComponent<Blackboard>();
                    ClientAPI2Blackboard.Serialize(popupBB, response);

                    // string contextID = BlackboardUtils.GetOrCreateVariable<string>(bb, "contextID")?.value;
                    // BlackboardUtils.SetOrCreateValue(popupBB, "contextID", contextID);

                    MetaObjectUtils.SetCalleeCaller(popupObj, caller);
                    MetaPopupUtils.OpenPopup(popupObj);
                },
                (error) =>
                {
                    switch (error.errorCode)
                    {
                        case Error.BUILD_DREAM_NOT_ACTIVE_ERROR:
                            VegasDreamsErrorHandler.OpenVegasDreamsEndPopup(() => EventSender.SendEvent(caller, VegasDreams.Events.ON_VEGAS_DREAMS_END_CALLBACK));
                            break;
                        default:
                            GlobalErrorHandler.GlobalError(error);
                            break;
                    }
                });
        }

        private void MakeDailyChestCell()
        {
            for (int i = 1; i <= BUILDING_MAX_COUNT; i++)
            {
                var cellAreaElement = ContextUtils.FindElement(root, $"Cell Area {i}", CHILDREN);
                var rank = VegasDreams.Utils.GetBuildingRank(i - 1);
                var chestCell = MetaObjectUtils.MakePrefab(VegasDreams.Defines.CONTENTS_BUNDLE, $"Daily Chest Cell {rank}", cellAreaElement.transform);
                var chestCellElement = chestCell.GetComponent<ContextElement>();
                chestCellElement.UpdateContext(false);

                var chestAreaElement = ContextUtils.FindElement(chestCellElement, "Chest Anchor", CHILDREN);
                MetaContextElementUtils.SimpleSetText(chestCellElement, "Text Object Name", VegasDreams.Utils.GetBuildingName(i - 1));

                for (int level = 1; level <= VegasDreams.Utils.GetBuildingLevel(i - 1); level++)
                {
                    var star = ContextUtils.FindElement(chestCellElement, $"Star {level}", CHILDREN);
                    star.GetComponent<Animator>().SetTrigger("isAlreadyGet");
                }

                var dailyChest = MetaObjectUtils.MakePrefab(
                    VegasDreams.Defines.CONTENTS_BUNDLE,
                    $"Chest {rank}",
                    chestAreaElement.transform
                );
                var chestBB = dailyChest.GetComponent<Blackboard>();
                chestBB.SetValue("index", i - 1);
                chestBB.SetValue("isGiftPopup", true);
            }
        }
    }
}
