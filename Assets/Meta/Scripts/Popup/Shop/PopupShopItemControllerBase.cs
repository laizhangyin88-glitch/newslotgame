using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;
using SlotMaker;
using BagelCode.ClientModels;
using BagelCode.Scratcher;

namespace BagelCode
{
    public class PopupShopItemControllerBase : MonoBehaviour
    {
        protected virtual GameObject MakeRewardIcon(Blackboard rewardInfo, Transform root, string parentName, int idx = 0)
        {
            string assetName = "";

            var rewardType = rewardInfo.GetValue<RewardType>("type");
            switch(rewardType)
            {
                // case RewardType.CREDIT:
                // case RewardType.RP:
                case RewardType.DAILY_BONUS_WHEEL_SPIN:
                    assetName = "Shop Rewards Item Daily Wheel";
                    break;
                case RewardType.PURCHASE_COUPON:
                    assetName = "Shop Rewards Item Coupon";
                    break;
                case RewardType.GAME_SPIN:
                case RewardType.GAME_DEAL:
                case RewardType.GAME_PLAY:
                    assetName = "Shop Rewards Game Spin";
                    break;
                // case RewardType.DAILY_BOOST:
                // case RewardType.TIER_UPGRADE:
                // case RewardType.PROGRAMMED_WIN:
                // case RewardType.EXP_MULTIPLY:
                case RewardType.RANDOM:
                    assetName = "Shop Rewards Item Random";
                    break;
                case RewardType.SOCIAL_CREDIT:
                    assetName = "Shop Rewards Item Gift Friends";
                    break;
                case RewardType.TICKETED_BONUS_TICKET:
                case RewardType.TICKETED_BONUS_TICKET_FOR_BOOST:
                    {
                    var tag = BlackboardUtils.FindVariable<BonusTag>(rewardInfo, "tag");
                    if (tag != null)
                    {
                        if (tag.value == BonusTag.BUY_A_BONUS)
                            assetName = "Shop Rewards Buy A Bonus Ticket";
                        else if (tag.value == BonusTag.SUPER_BONUS)
                            assetName = "Shop Rewards Super Bonus Ticket";
                        else if (tag.value == BonusTag.INSTANT_BONUS)
                            assetName = "Shop Rewards Instant Bonus Ticket";
                    }
                    break;
                }
                // case RewardType.SCRATCHER:
                case RewardType.COLLECTING_GAME_PACK:
                    assetName = "Shop Rewards Item Chest";
                    break;
                // case RewardType.CREDIT_WITH_MULTIPLIER:
                // case RewardType.CLUB_CREDIT:
                case RewardType.BOSS_RAIDERS_ENERGY:
                case RewardType.CLUB_ARENA_ENERGY:
                    assetName = "Shop Rewards Item Energy";
                    break;
                case RewardType.HIDDEN_UNIVERSE_FINDER:
                    assetName = "Shop Rewards Item Finder";
                    break;
                case RewardType.DEPOT:
                    var depotType = rewardInfo.GetValue<DepotType>("depotType");
                    assetName = $"Shop Rewards Item Depot {depotType}";
                    break;
                case RewardType.WILD_PUZZLE:
                    assetName = "Shop Rewards Item Wild Puzzle";
                    break;
            }

            return MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, assetName, root, parentName);
        }

        protected virtual void SetIconValues(GameObject targetObj, Blackboard rewardInfoForProductBB)
        {
            var rewardType = rewardInfoForProductBB.GetValue<RewardType>("type");

            switch(rewardType)
            {
                // case RewardType.CREDIT:
                // case RewardType.RP:
                // case RewardType.DAILY_BONUS_WHEEL_SPIN:
                // case RewardType.PURCHASE_COUPON:
                case RewardType.GAME_SPIN:
                case RewardType.GAME_DEAL:
                case RewardType.GAME_PLAY:
                    {
                        // Slot Thumbnail Area
                        // gameId
                        var gameID = rewardInfoForProductBB.GetValue<int>("gameId");
                        MetaIconUtils.MakeSlotThumbnailIconObject(gameID, targetObj.transform, "Slot Thumbnail Area");
                    }
                    break;
                // case RewardType.DAILY_BOOST:
                // case RewardType.TIER_UPGRADE:
                // case RewardType.PROGRAMMED_WIN:
                // case RewardType.EXP_MULTIPLY:
                // case RewardType.RANDOM:
                // case RewardType.SOCIAL_CREDIT:
                case RewardType.TICKETED_BONUS_TICKET:
                case RewardType.TICKETED_BONUS_TICKET_FOR_BOOST:
                    {
                        // Slot Thumbnail Area
                        // gameId
                        var gameID = rewardInfoForProductBB.GetValue<int>("gameId");
                        MetaIconUtils.MakeSlotThumbnailIconObject(gameID, targetObj.transform, "Slot Thumbnail Area");
                    }
                    break;
                // case RewardType.SCRATCHER:
                case RewardType.COLLECTING_GAME_PACK:
                    {
                        var chestId = rewardInfoForProductBB.GetValue<int>("packId");
                        Sprite chestImage = CollectingGameChestData.Instance.chestAssets.assets[BlackboardQueryUtils.GetChestIndex(chestId)];
                        targetObj.transform.GetComponentInChildren<ContextImage>().SetSprite(chestImage);
                    }
                    break;
                // case RewardType.CREDIT_WITH_MULTIPLIER:
                // case RewardType.CLUB_CREDIT:
            }
        }

        protected virtual void SetQuantity(GameObject targetObj, long quantity)
        {
            if(quantity <= 0) return;

            var badgeArea = targetObj.transform.Find("Badge Area");

            if(badgeArea != null)
            {
                var badgeObject = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Shop Rewards Item Badge", badgeArea, null);

                var badgeElement = badgeObject.GetComponent<ContextElement>();
                badgeElement.UpdateContext(false);
                MetaContextElementUtils.SimpleSetTextGlobal(badgeElement, "Text", "SIMPLE_NUMBER", ContextSearchingType.ChildrenSearch, quantity);

                MetaContextElementUtils.SimpleSetActive(badgeElement, "Base"     , quantity < 100);
                MetaContextElementUtils.SimpleSetActive(badgeElement, "Base Long", quantity >= 100);
            }
        }
    }

}

