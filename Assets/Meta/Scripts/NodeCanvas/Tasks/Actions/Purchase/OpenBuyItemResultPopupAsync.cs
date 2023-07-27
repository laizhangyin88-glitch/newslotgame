using System.Collections;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using BagelCode.ClientModels;
using ParadoxNotion;
using BagelCode;

namespace SlotMaker.Tasks.Actions
{
    [Category("★ SlotMaker/Blackboard")]
    public class OpenBuyItemResultPopupAsync : ActionTask<Blackboard>
    {
        [BlackboardOnly]
        public BBParameter<GameObject> saveAsResultPopup;

        [BlackboardOnly]
        public BBParameter<int> saveAsRefillSpinCount;

        private Variable<ItemType> itemType;
        private Blackboard resultBB;

        private const string POPUP_THANKS_FOR_PURCHASE = "POPUP_THANKS_FOR_PURCHASE";
        private const string ON_EARLY_ACCESS_EVENT = "OnEarlyAccessAvailable";

        protected override string info { get { return "Buy Item Result Process Async"; } }

        protected override void OnExecute()
        {
            if (ApplicationSettings.LogTest())
                Debug.Log("Buy Item Result Process Async");

            BuyItemResultProcess();
        }

        private void BuyItemResultProcess()
        {
            itemType = BlackboardUtils.GetOrCreateVariable<ItemType>(agent, "_itemType");

            switch(itemType.value)
            {
                case ItemType.DAILY_BOOST:
                case ItemType.CREDIT_POT_OF_GOLD:
                case ItemType.DAILY_BONUS_WHEEL:
                case ItemType.DAILY_MEGA_WHEEL:
                case ItemType.CREDIT:
                case ItemType.GEM:
                case ItemType.CREDIT_WHEEL:
                case ItemType.PIGGY_BANK:
                case ItemType.EPIC_PASS:
                case ItemType.EPIC_PASS_V2:
                case ItemType.HIDDEN_UNIVERSE_FINDER:
                    StartCoroutine(OpenBuyResultPopupCoroutine());
                    break;
                case ItemType.EARLY_ACCESS:
                    BuyEarlyAccessResult();
                    break;
                case ItemType.CREDIT_MULTIPLIER_WHEEL:
                case ItemType.POG_BOOSTER:
                case ItemType.GEM_BOOSTER:
                case ItemType.SPIN_BOOST:
                case ItemType.TICKETED_BONUS_TICKET:
                case ItemType.TICKETED_BONUS_BOOSTER:
                    PostPurchase();
                    break;
                default:
                    Debug.LogError(string.Format("BuyItemResultProcess failure. {0} is undefined item type.", itemType.value));
                    PostPurchase();
                    break;
            }
        }

        private IEnumerator OpenBuyResultPopupCoroutine()
        {
            string bundle = GetBundleName();
            string asset = GetAssetName();
            Transform parent = GetPopupRoot();

            if (string.IsNullOrEmpty(bundle) || string.IsNullOrEmpty(asset))
            {
                EndAction(false);
                yield break;
            }

            // Make Popup
            yield return StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(bundle, asset, parent,
                (SceneLoadOperation sceneLoadOperation) => saveAsResultPopup.value = sceneLoadOperation.GetScene()));

            // Post Process
            resultBB = saveAsResultPopup.value.GetComponent<Blackboard>();
            switch (itemType.value)
            {
                case ItemType.DAILY_BOOST:
                case ItemType.CREDIT_POT_OF_GOLD:
                    {
                        SetTitleKey();
                        SetUseItemBB();

                        MetaPopupUtils.OpenPopup(saveAsResultPopup.value);
                    }
                    break;
                case ItemType.DAILY_BONUS_WHEEL:
                case ItemType.DAILY_MEGA_WHEEL:
                    {
                        SetTitleKey();
                        SetUseItemBB();
                        SetCaller();

                        MetaPopupUtils.OpenPopup(saveAsResultPopup.value);
                    }
                    break;
                case ItemType.CREDIT:
                case ItemType.GEM:
                case ItemType.CREDIT_WHEEL:
                    {
                        SetTitleKey();
                        SetUseItemBB();
                        SetProduct();
                        SetLevelMultiplierValue();

                        MetaPopupUtils.OpenPopup(saveAsResultPopup.value);
                    }
                    break;
                case ItemType.PIGGY_BANK:
                    {
                        saveAsResultPopup.value.SetActive(false);
                        SetTitleKey();
                        SetUseItemBB();
                    }
                    break;
                case ItemType.EPIC_PASS:
                case ItemType.EPIC_PASS_V2:
                    {
                        SetIsUnlockEpicPass();

                        MetaPopupUtils.OpenPopup(saveAsResultPopup.value);
                    }
                    break;
                case ItemType.HIDDEN_UNIVERSE_FINDER:
                    {
                        var product = BlackboardUtils.GetOrCreateVariable<Blackboard>(agent, "product").value;
                        var item = BlackboardQueryUtils.GetItemFromProduct(product, ItemType.HIDDEN_UNIVERSE_FINDER);
                        int finder = item.GetValue<int>("finder");

                        BlackboardUtils.SetOrCreateValue(resultBB, "earnFinder", finder);
                        BlackboardUtils.SetOrCreateValue(resultBB, "isPurchaseResult", true);
                        BlackboardUtils.SetOrCreateValue(resultBB, "caller", agent.gameObject);

                        MetaPopupUtils.OpenPopup(saveAsResultPopup.value);
                    }
                    break;
            }

            PostPurchase();
        }

        private void BuyEarlyAccessResult()
        {
            // TODO : refillcount refactoring.
            var grade = BlackboardUtils.GetOrCreateVariable<int>(agent, "_useItem/grade").value;
            saveAsRefillSpinCount.value = BlackboardQueryUtils.RefillEarlyAccessBonusSpins(grade);
            ///////////////////////////////

            Graph.SendGlobalEvent(new EventData(ON_EARLY_ACCESS_EVENT), this);

            PostPurchase();
        }

        private void PostPurchase()
        {
            if (saveAsResultPopup.value != null)
            {
                // Check Tier Shop
                bool isShop = BlackboardUtils.GetOrCreateVariable<bool>(agent, "silentTierShop")?.value ?? false;
                BlackboardUtils.SetOrCreateValue(resultBB, "silentTierShop", isShop);

                var caller = BlackboardUtils.GetOrCreateVariable<GameObject>(agent, "caller")?.value;
                if (caller != null)
                    MetaObjectUtils.SetCalleeCaller(saveAsResultPopup.value, caller);
            }

            EndAction(true);
        }

        private string GetAssetName()
        {
            switch(itemType.value)
            {
                case ItemType.CREDIT:
                    return "Popup Shop Coin Purchase Scene";
                case ItemType.GEM:
                    return "Popup Shop Gem Purchase Scene";
                case ItemType.DAILY_BOOST:
                    return "Popup Daily Boost Purchase Scene";
                case ItemType.DAILY_BONUS_WHEEL:
                    return "Popup Daily Bonus Purchase Result Scene";
                case ItemType.PIGGY_BANK:
                    return "Popup Congratulations Pot Of Gold Scene";
                case ItemType.CREDIT_POT_OF_GOLD:
                    return "Popup Congratulations Coin Pot Of Gold Scene";
                case ItemType.CREDIT_WHEEL:
                    return "Popup Coin Wheel Scene";
                case ItemType.DAILY_MEGA_WHEEL:
                    return "Popup Daily Mega Bonus Purchase Result Scene";
                case ItemType.EPIC_PASS:
                    return "Popup Epic Pass Reward Scene";
                case ItemType.EPIC_PASS_V2:
                    return "Popup Epic Pass Always Unlock Scene";
                case ItemType.HIDDEN_UNIVERSE_FINDER:
                    return "Popup Hidden Objects Finder Received Scene";
                default:
                    return null;
            }
        }

        private string GetBundleName()
        {
            if (itemType.value == ItemType.EPIC_PASS)
            {
                string bundleName = BlackboardQueryUtils.GetMetaBundleName(EventInfoType.SEASON_PASS);

#if USE_ASSETBUNDLE
                // Check asset bundle.
                var bundle = AssetBundleManager.GetLoadedAssetBundle(bundleName);
                if(bundle == null)
                {
                    EndAction(false);
                    return null;
                }
#endif
                return bundleName;
            }
            else if(itemType.value == ItemType.HIDDEN_UNIVERSE_FINDER)
            {
                string bundleName = BagelCode.HiddenObjects.HiddenObjects.Defines.COMMON_BUNDLE;

#if USE_ASSETBUNDLE
                // Check asset bundle.
                var bundle = AssetBundleManager.GetLoadedAssetBundle(bundleName);
                if(bundle == null)
                {
                    EndAction(false);
                    return null;
                }
#endif
                return bundleName;
            }

            return MetaStringDefine.LOBBY_BUNDLE_NAME;
        }

        private Transform GetPopupRoot()
        {
            return MetaPopupUtils.PopupManagerAreaTransform;
        }

        private void SetTitleKey()
        {
            BlackboardUtils.SetOrCreateValue(resultBB, "titleKey", POPUP_THANKS_FOR_PURCHASE);
        }

        private void SetCaller()
        {
            var caller = BlackboardUtils.GetOrCreateVariable<GameObject>(agent, "caller").value;
            BlackboardUtils.SetOrCreateValue(resultBB, "caller", caller);
        }

        private void SetProduct()
        {
            var product = BlackboardUtils.GetOrCreateVariable<Blackboard>(agent, "product").value;
            BlackboardUtils.SetOrCreateValue(resultBB, "product", product);
        }

        private void SetUseItemBB()
        {
            var useItem = BlackboardUtils.GetOrCreateVariable<Blackboard>(agent, "_useItem").value;
            BlackboardUtils.SetOrCreateValue(resultBB, "itemUseResult", useItem);
        }

        private void SetIsUnlockEpicPass()
        {
            BlackboardUtils.SetOrCreateValue(resultBB, "isUnlock", true);
        }

        private void SetLevelMultiplierValue()
        {
            var levelMultiplierType = BlackboardUtils.FindVariable<string>(agent, "_levelMultiplierType");
            string lmTypeValue = levelMultiplierType == null || string.IsNullOrEmpty(levelMultiplierType.value) ? "coin" : levelMultiplierType.value;
            BlackboardUtils.SetOrCreateValue(resultBB, "_levelMultiplierType", lmTypeValue);
        }
    }
}
