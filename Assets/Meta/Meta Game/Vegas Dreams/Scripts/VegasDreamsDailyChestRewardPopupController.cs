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
    public class VegasDreamsDailyChestRewardPopupController : EventMonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;
        private Animator anim;

        private ContextElement contentsAreaElement;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        public void InitProperty()
        {
            root = GetComponent<ContextElement>();
            anim = GetComponent<Animator>();
            bb = GetComponent<Blackboard>();

            root.UpdateContext(false);

            GSManager.Instance.GetHandler("UI_Purchase_Complete_Appear").Play();

            contentsAreaElement = ContextUtils.FindElement(root, "Contents", CHILDREN);

            InitContents();
            MakeEarnPrefabs();
        }

        private void MakeEarnPrefabs()
        {
            // priority
            // credit > vlp > wild puzzle > depots
            
            var earnedCredit = bb.GetValue<long>("earnedCredit");
            if (earnedCredit > 0)
            {
                var itemCell = MetaObjectUtils.MakePrefab(VegasDreams.Defines.CONTENTS_BUNDLE, "Vegas Dreams Daily Chest Rewards Item Cell", contentsAreaElement.transform);
                var itemCellElement = itemCell.GetComponent<ContextElement>();
                itemCellElement.UpdateContext(false);
                var imageAreaElement = ContextUtils.FindElement(itemCellElement, "Image Area", CHILDREN);
                var textElement = ContextUtils.FindElement(itemCellElement, "Text", CHILDREN);

                MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Image Common Reward Coin", imageAreaElement.transform);
                MetaContextElementUtils.SetTextGlobal(textElement, "COMMA_STYLE_COIN", earnedCredit);
                BlackboardQueryUtils.AddCoins(earnedCredit);
            }

            var earnedDepot = bb.GetValue<Blackboard>("earnedDepot");
            var depot = bb.GetValue<Blackboard>("depot");

            foreach (DepotType type in Enum.GetValues(typeof(DepotType)))
            {
                if (type == DepotType.UNKNOWN) continue;
                var typeName = type.ToString().ToLower();

                var earnedDepotType = earnedDepot.GetValue<int>(typeName);

                if (earnedDepotType > 0)
                {
                    var itemCell = MetaObjectUtils.MakePrefab(VegasDreams.Defines.CONTENTS_BUNDLE, "Vegas Dreams Daily Chest Rewards Item Cell", contentsAreaElement.transform);
                    var itemCellElement = itemCell.GetComponent<ContextElement>();
                    itemCellElement.UpdateContext(false);
                    var imageAreaElement = ContextUtils.FindElement(itemCellElement, "Image Area", CHILDREN);
                    var textElement = ContextUtils.FindElement(itemCellElement, "Text", CHILDREN);

                    MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, $"Image Common Reward Depot {type.ToString()}", imageAreaElement.transform);
                    MetaContextElementUtils.SetTextGlobal(textElement, $"VEGAS_DREAMS_COMMON_REWARD_{type.ToString()}", earnedDepotType);
                    BlackboardQueryUtils.UpdateDepotCount(typeName, depot.GetValue<int>(typeName));
                }
            }

            var earnedLoungePoint = bb.GetValue<long>("earnedLoungePoint");
            if (earnedLoungePoint > 0)
            {
                var itemCell = MetaObjectUtils.MakePrefab(VegasDreams.Defines.CONTENTS_BUNDLE, "Vegas Dreams Daily Chest Rewards Item Cell", contentsAreaElement.transform);
                var itemCellElement = itemCell.GetComponent<ContextElement>();
                itemCellElement.UpdateContext(false);
                var imageAreaElement = ContextUtils.FindElement(itemCellElement, "Image Area", CHILDREN);
                var textElement = ContextUtils.FindElement(itemCellElement, "Text", CHILDREN);

                MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Image Common Reward VIP Lounge Point", imageAreaElement.transform);
                BlackboardQueryUtils.UpdateVIPLoungeInfo(bb.GetValue<Blackboard>("vipLoungeInfo"));
                MetaContextElementUtils.SetTextGlobal(textElement, "VIP_LOUNGE_REWARD_VLP", earnedLoungePoint);
            }

            var earnedWildPuzzleCount = bb.GetValue<int>("earnedWildPuzzleCount");
            if (earnedWildPuzzleCount > 0)
            {
                var itemCell = MetaObjectUtils.MakePrefab(VegasDreams.Defines.CONTENTS_BUNDLE, "Vegas Dreams Daily Chest Rewards Item Cell", contentsAreaElement.transform);
                var itemCellElement = itemCell.GetComponent<ContextElement>();
                itemCellElement.UpdateContext(false);
                var imageAreaElement = ContextUtils.FindElement(itemCellElement, "Image Area", CHILDREN);
                var textElement = ContextUtils.FindElement(itemCellElement, "Text", CHILDREN);

                MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Image Common Reward Wild Puzzle", imageAreaElement.transform);
                MetaContextElementUtils.SetTextGlobal(textElement, "VEGAS_DREAMS_COMMON_REWARD_WILD_PUZZLE", earnedWildPuzzleCount);
            }
        }

        private void InitContents()
        {
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Button Collect/Text", "BUTTON_COLLECT", FULL);
            MetaContextElementUtils.SimpleSetClickable(root, "Button Collect", () =>
            {
                GSManager.Instance.GetHandler("UI_Button_Collect").Play();
                EventSender.SendEvent(gameObject, VegasDreams.Events.ON_CLOSE);
            });
        }
    }
}
