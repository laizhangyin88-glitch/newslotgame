using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using BagelCode.OSA_Scroll;

namespace BagelCode
{
    public class PopupLevelUpFullController : MonoBehaviour
    {
        // access blackboard key
        public enum LevelMultiplierTable
        {
            coin = 0,
            pog,
            dailyBoost,
            wheel,
            vipDeal,
            spinDeal,
            earlyAccess,
            ticketedBonus,
            scratcher,
            hogDeal,
            bossRaidersDeal,
            MAX
        }

        private ContextElement rootElement;
        private Blackboard rootBB;
        private Animator rootAnimator;

        private ContextElement shopMultiplierAreaElement;
        private ContextElement buttonShopElement;

        private List<PopupLevelMultiplierItem> levelMultiplieritems = new List<PopupLevelMultiplierItem>();
        private OSA_LevelUpFullShopMultiplier osaController;

        private string BIContextId = "";
        private string upgradeShopList = "";

        public void OnInit()
        {
            rootElement = gameObject.GetComponent<ContextElement>();
            rootBB = gameObject.GetComponent<Blackboard>();
            rootAnimator = gameObject.GetComponent<Animator>();

            InitProperty();
        }

        private void InitProperty()
        {
            buttonShopElement = ContextUtils.FindElement(rootElement, "Button Shop", ContextSearchingType.ChildrenSearch);
            shopMultiplierAreaElement = ContextUtils.FindElement(rootElement, "Shop Multiplier Area", ContextSearchingType.ChildrenSearch);
            InitLevelMultiplierElement();

            MetaContextElementUtils.SimpleSetTextGlobal(buttonShopElement, "Text", "POPUP_LEVEL_UP_FULL_GO_TO_SHOP", ContextSearchingType.ChildrenSearch);

            MetaContextElementUtils.SetClickable(
                buttonShopElement,
                "OnShop",
                rootElement,
                null
            );
            SetActiveButtonShop(false);
        }

        private void InitLevelMultiplierElement()
        {
            if (levelMultiplieritems == null) levelMultiplieritems = new List<PopupLevelMultiplierItem>();
            levelMultiplieritems.Clear();

            for (int i = 0; i < (int)LevelMultiplierTable.MAX; ++i)
                levelMultiplieritems.Add(new PopupLevelMultiplierItem(i));
            osaController = shopMultiplierAreaElement.GetComponent<OSA_LevelUpFullShopMultiplier>();
        }

        public void SetActiveButtonShop(bool isActive)
        {
            if (buttonShopElement != null)
                buttonShopElement.gameObject.SetActive(isActive);
        }
        // Show Level Multiply : [Go To Shop]
        public void SetActiveLevelMultiplier()
        {
            rootAnimator.SetTrigger("isShopMulti");

            LevelUtils.UpdatePrefsLevelMultiplier();

            BlackboardUtils.SetOrCreateValue<bool>(MainBlackboard.Get(), "ShopCoinLevelMultiplyFlipEnabled", true);

            BI_LevelUpShopUpgrade("trigger");
        }

        public bool CheckLevelMultiplier()
        {
            bool isActive = false;
            List<PopupLevelMultiplierItem> osaItems = new List<PopupLevelMultiplierItem>();
            Dictionary<string, Dictionary<string, double>> shopList = new Dictionary<string, Dictionary<string, double>>();

            if (levelMultiplieritems != null && levelMultiplieritems.Count > 0)
            {
                for (int i = 0; i < (int)LevelMultiplierTable.MAX; ++i)
                {
                    string typeValue = ((LevelMultiplierTable)i).ToString();
                    levelMultiplieritems[i].SetData(LevelUtils.CheckLevelMultiplier(typeValue), typeValue);

                    if (levelMultiplieritems[i].IsActive)
                    {
                        isActive = true;
                        osaItems.Add(levelMultiplieritems[i]);
                        // AE data
                        Dictionary<string, double> item = new Dictionary<string, double>();
                        double typeValueMultiplier = NumberUtils.GetMultiplierFromNumerator(LevelUtils.GetLevelMultiplierNumerator(typeValue));
                        item.Add("level_multiplier", System.Math.Truncate(typeValueMultiplier * 10) / 10);
                        item.Add("rate_of_increase", System.Math.Truncate(LevelUtils.GetLevelMultiplierFromPreviousSection(typeValue) * 10) / 10);
                        shopList.Add(typeValue, item);
                    }
                }
            }

            if (isActive)
            {
                upgradeShopList = SlotMaker.Json.SlotSimpleJson.SerializeObject(shopList);
                if (osaItems.Count > 3)
                    osaController.SetItems(osaItems);
                else
                {
                    osaController.enabled = false;
                    ContextElement contentsElement = ContextUtils.FindElement(shopMultiplierAreaElement, "Contents", ContextSearchingType.ChildrenSearch);
                    for (int i = 0; i < osaItems.Count; ++i)
                    {
                        GameObject obj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "LM Shop Reward Item Cell", contentsElement.transform, null, "Item Cell");
                        PopupLevelMultiplierItemController itemController = obj?.GetComponent<PopupLevelMultiplierItemController>() ?? null;
                        itemController?.OnInit(osaItems[i]);
                    }
                }
            }

            return isActive;
        }

        public int SetupReward()
        {
            // Calc Data
            Variable<int> targetLevel = BlackboardUtils.FindVariable<int>(null, "/userSyncInfo/level");
            Variable<int> beforeLevel = BlackboardUtils.FindVariable<int>(null, "/me/beforeLevel");

            int startLevel = beforeLevel.value;
            int intervalLevel = targetLevel.value - beforeLevel.value;
            int beforeTier = TierUtils.GetMeTier();

            MetaContextElementUtils.SimpleSetText(rootElement, "Level", targetLevel.value.ToString(), ContextSearchingType.ChildrenSearch);

            long creditBonus = LevelUtils.GetLevelCreditBonus(beforeLevel.value);
            long rpBonus = LevelUtils.GetLevelRpBonus(beforeLevel.value);
            long gemBonus = LevelUtils.GetLevelGemBonus(beforeLevel.value);
            ++beforeLevel.value;

            long timeBonus = TierUtils.GetTimeBonusCoins(beforeTier);
            int specialBonusLevelFactor = BlackboardUtils.FindVariable<int>(null, "/values/timeBonus/SPECIAL_BONUS_LEVEL_FACTOR")?.value ?? 0;
            if (specialBonusLevelFactor != 0)
                timeBonus += (BlackboardUtils.FindValue<int>(null, "/values/timeBonus/BONUS_LEVEL_MULTIPLIER") * (beforeLevel.value - 1))
                                + ((beforeLevel.value / specialBonusLevelFactor) * BlackboardUtils.FindValue<int>(null, "/values/timeBonus/SPECIAL_BONUS_CREDIT"));

            // Set Data
            BlackboardUtils.FindVariable<int>(null, "/me/level").value = targetLevel.value;     // BlackboardQueryUtils.LevelUp() ?
            //beforeLevel = BlackboardUtils.FindVariable<int>(null, "/me/beforeLevel");           // ??
            BlackboardUtils.FindVariable<int>(null, "/me/beforeLevel").value = targetLevel.value;
            BlackboardQueryUtils.AddCoins(creditBonus);
            BlackboardQueryUtils.AddRP(rpBonus);
            BlackboardQueryUtils.AddGems(gemBonus);

            ContextElement rpElement = ContextUtils.FindElement(rootElement, "Vip Point", ContextSearchingType.ChildrenSearch);
            ContextElement gemElement = ContextUtils.FindElement(rootElement, "Gem", ContextSearchingType.ChildrenSearch);

            MetaContextElementUtils.SimpleSetTextGlobal(rootElement, "Coin/Text Amount", "TEXT_COIN_BONUS", ContextSearchingType.FullNameSearch, creditBonus);
            MetaContextElementUtils.SimpleSetTextGlobal(rpElement, "Text Amount", "TEXT_VIP_BONUS", ContextSearchingType.ChildrenSearch, rpBonus);
            MetaContextElementUtils.SimpleSetTextGlobal(rootElement, "Time Bonus/Text Amount", "TEXT_TIME_BONUS_UP", ContextSearchingType.FullNameSearch, timeBonus);
            MetaContextElementUtils.SimpleSetTextGlobal(gemElement, "Text Amount", "TEXT_GEM_BONUS", ContextSearchingType.ChildrenSearch, gemBonus);

            rpElement.gameObject.SetActive(gemBonus == 0);
            gemElement.gameObject.SetActive(gemBonus > 0);

            return beforeLevel.value;
        }

        public void SetShopBiContextId()
        {
            BlackboardQueryUtils.SetShopBiContextId(BIContextId);
        }

        public void CheckButtonClickClose(bool isClicked)
        {
            if (isClicked)
                BI_LevelUpShopUpgrade("ok");
        }

        public void BI_LevelUpShopUpgrade(string triggerType)
        {
            if (string.IsNullOrEmpty(BIContextId))
                BIContextId = BiEventUtils.GenerateContextID();

            Dictionary<string, object> customData = new Dictionary<string, object>();
            customData["target_level"] = BlackboardQueryUtils.GetMyLevel();
            customData["type"] = triggerType;
            customData["shop_list"] = upgradeShopList;
            customData["context_id"] = BIContextId;


            Analytics.CustomEvent("client_level_up_shop_upgraded", customData);
        }
    }
}
