using SlotMaker;
using UnityEngine;
using NodeCanvas.Framework;
using BagelCode.ClientModels;

namespace BagelCode.EpicPass
{
    public class EpicPassV2ButtonIconRewardController : MonoBehaviour
    {
        private GameObject rewardIconObj = null;
        private ContextElement iconAreaElement;
        private ContextElement textElement;

        private Blackboard rewardBB;

        private bool isInit = false;

        private const string REWARD_COIN = "Image Reward Coin";
        private const string REWARD_GEM = "Image Reward Gem";
        private const string REWARD_DAILY_SPIN = "Image Reward Daily Spin";
        private const string REWARD_GIFT = "Image Reward Mystery Gift";
        private const string REWARD_VIP_LOUNGE_TICKET = "Image Reward VIP Lounge Ticket";
        private const string REWARD_BAB = "Image Reward Buy A Bonus";
        private const string REWARD_INS = "Image Reward Instant Bonus";
        private const string REWARD_SPB = "Image Reward Super Bonus";
        private const string REWARD_FINDER = "Image Reward Finder";
        private const string REWARD_SCRATCHER = "Image Reward Scratcher";
        private const string REWARD_WILD_PUZZLE = "Image Reward Wild Puzzle";

        public void OnInit(Blackboard _rewardBB)
        {
            rewardBB = _rewardBB;

            InitProperty();
            CreateRewardIcon();

        }

        private void InitProperty()
        {
            if (isInit) return;

            ContextElement rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext();

            iconAreaElement = ContextUtils.FindElement(rootElement, "Icon Area", ContextSearchingType.ChildrenSearch);
            textElement = ContextUtils.FindElement(rootElement, "Text", ContextSearchingType.ChildrenSearch);

            isInit = true;
        }

        private void CreateRewardIcon()
        {
            if (rewardIconObj != null)
            {
                Destroy(rewardIconObj);
                rewardIconObj = null;
            }
            if (rewardBB == null)
                return;

            string rewardText = "";

            var rewardType = rewardBB.GetValue<ClientModels.RewardType>("type");
            switch (rewardType)
            {
                case RewardType.CREDIT:
                case RewardType.CREDIT_WITH_MULTIPLIER:
                    {
                        if (rewardIconObj == null)
                            rewardIconObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, REWARD_COIN, iconAreaElement.transform, null, "Reward Icon");

                        var credit = BlackboardUtils.FindVariable<long>(rewardBB, "credit");
                        rewardText = StringTableUtils.GetString(StringTable.StringTableType.Global, "EPIC_PASS_BUTTON_REWARD_COIN_TEXT", credit.value);
                    }
                    break;
                case RewardType.GEM:
                    {
                        if (rewardIconObj == null)
                            rewardIconObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, REWARD_GEM, iconAreaElement.transform, null, "Reward Icon");

                        var gem = BlackboardUtils.FindVariable<long>(rewardBB, "gem");
                        rewardText = StringTableUtils.GetString(StringTable.StringTableType.Global, "EPIC_PASS_BUTTON_REWARD_GEM_TEXT", gem.value);
                    }
                    break;
                case RewardType.DAILY_BONUS_WHEEL_SPIN:
                    {
                        if (rewardIconObj == null)
                            rewardIconObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, REWARD_DAILY_SPIN, iconAreaElement.transform, null, "Reward Icon");
                        var spinCount = BlackboardUtils.FindVariable<int>(rewardBB, "spinCount");
                        rewardText = StringTableUtils.GetString(StringTable.StringTableType.Global, "EPIC_PASS_REWARD_DAILY_SPIN_TEXT", spinCount.value);
                    }
                    break;
                case RewardType.RP:
                case RewardType.GAME_SPIN:
                case RewardType.GAME_DEAL:
                case RewardType.GAME_PLAY:
                case RewardType.RANDOM:
                case RewardType.DAILY_DELIVERY:
                    {
                        if (rewardIconObj == null)
                            rewardIconObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, REWARD_GIFT, iconAreaElement.transform, null, "Reward Icon");
                    }
                    break;
                case RewardType.SCRATCHER:
                case RewardType.SCRATCHER_FOR_INBOX:
                    {
                        if (rewardIconObj == null)
                            rewardIconObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, REWARD_SCRATCHER, iconAreaElement.transform, null, "Reward Icon");
                        var count = BlackboardUtils.FindVariable<int>(rewardBB, "count");
                        rewardText = StringTableUtils.GetString(StringTable.StringTableType.Global, "EPIC_PASS_REWARD_SCRATCHER_TEXT", count.value);
                    }
                    break;
                case RewardType.TICKETED_BONUS_TICKET:
                    {
                        var tag = BlackboardUtils.FindVariable<ClientModels.BonusTag>(rewardBB, "tag");
                        string assetName = REWARD_BAB;
                        switch (tag.value)
                        {
                            case ClientModels.BonusTag.INSTANT_BONUS:
                                assetName = REWARD_INS;
                                break;
                            case ClientModels.BonusTag.BUY_A_BONUS:
                                assetName = REWARD_BAB;
                                break;
                            case ClientModels.BonusTag.SUPER_BONUS:
                                assetName = REWARD_SPB;
                                break;
                        }
                        if (rewardIconObj == null)
                            rewardIconObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, assetName, iconAreaElement.transform, null, "Reward Icon");

                        long totalBet = rewardBB.GetValue<long>("baseBet") + rewardBB.GetValue<long>("extraBet");
                        rewardText = StringTableUtils.GetString(StringTable.StringTableType.Global, "EPIC_PASS_REWARD_TICKETED_BONUS_TICKET_TEXT", totalBet);
                    }
                    break;
                case RewardType.HIDDEN_UNIVERSE_FINDER:
                    {
                        if (rewardIconObj == null)
                            rewardIconObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, REWARD_FINDER, iconAreaElement.transform, null, "Reward Icon");
                        var finder = BlackboardUtils.FindVariable<int>(rewardBB, "finder");
                        rewardText = StringTableUtils.GetString(StringTable.StringTableType.Global, "EPIC_PASS_REWARD_FINDER_TEXT", finder.value);
                    }
                    break;
                case RewardType.VIP_LOUNGE_OPEN_TICKET:
                    {
                        if (rewardIconObj == null)
                            rewardIconObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, REWARD_VIP_LOUNGE_TICKET, iconAreaElement.transform, null, "Reward Icon");

                        var openDays = BlackboardUtils.FindVariable<int>(rewardBB, "openDays");
                        rewardText = StringTableUtils.GetString(StringTable.StringTableType.Global, "EPIC_PASS_REWARD_VIP_LOUNGE_TICKET_TEXT", openDays.value);
                    }
                    break;
                case RewardType.DEPOT:
                    {
                        DepotType type = BlackboardUtils.FindVariable<DepotType>(rewardBB, "depotType")?.value ?? DepotType.UNKNOWN;
                        string assetName = GetDepotText(type);
                        if (rewardIconObj == null && !string.IsNullOrEmpty(assetName))
                            rewardIconObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, assetName, iconAreaElement.transform, null, "Reward Icon");

                        var count = BlackboardUtils.FindVariable<int>(rewardBB, "count");
                        rewardText = StringTableUtils.GetString(StringTable.StringTableType.Global, "EPIC_PASS_REWARD_DEPOT_TEXT", count.value);
                    }
                    break;
                case RewardType.WILD_PUZZLE:
                    {
                        if (rewardIconObj == null)
                            rewardIconObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, REWARD_WILD_PUZZLE, iconAreaElement.transform, null, "Reward Icon");

                        var count = BlackboardUtils.FindVariable<int>(rewardBB, "count");
                        rewardText = StringTableUtils.GetString(StringTable.StringTableType.Global, "EPIC_PASS_REWARD_WILD_PUZZLE_TEXT", count.value);
                    }
                    break;
            }
            SetText(rewardText);
        }

        public ClientModels.RewardType GetRewardType()
        {
            return rewardBB?.GetValue<ClientModels.RewardType>("type") ?? ClientModels.RewardType.UNKNOWN;
        }

        private void SetText(string message)
        {
            if (textElement != null)
                MetaContextElementUtils.SetText(textElement, message);
        }

        private string GetDepotText(DepotType type)
        {
            if (type == DepotType.UNKNOWN)
                return "";

            return "Image Reward Depot " + TextDecoUtils.EnumTypeToText<DepotType>(
                (int)type, TextDecoUtils.TextFormat.PASCAL_CASE, " ");
        }
    }
}