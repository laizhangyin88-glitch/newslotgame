using UnityEngine;
using SlotMaker;

namespace BagelCode
{
    public class PopupLevelMultiplierItemController : MonoBehaviour
    {
        private ContextElement rootElement;
        private ContextElement amountTextElement;
        private ContextElement nameTextElement;
        private ContextElement iconAreaElement;

        private GameObject iconObject = null;

        private PopupLevelMultiplierItem itemData;

        public void OnInit(PopupLevelMultiplierItem _itemData)
        {
            rootElement = gameObject.GetComponent<ContextElement>();
            itemData = _itemData;

            rootElement.UpdateContext();

            amountTextElement = ContextUtils.FindElement(rootElement, "Text Amount", ContextSearchingType.ChildrenSearch);
            nameTextElement = ContextUtils.FindElement(rootElement, "Text", ContextSearchingType.ChildrenSearch);
            iconAreaElement = ContextUtils.FindElement(rootElement, "Icon Area", ContextSearchingType.ChildrenSearch);

            if (itemData.IsActive)
            {
                CreateIcon();
                SetMultiplierText();
            }

            rootElement.gameObject.SetActive(itemData.IsActive);
        }

        private void SetMultiplierText()
        {
            double multiplierValue = LevelUtils.GetLevelMultiplierFromPreviousSection(itemData.TypeValue);
            multiplierValue = System.Math.Truncate(multiplierValue * 10) / 10;

            if (amountTextElement != null)
            {
                string baseText = MetaContextElementUtils.GetText(amountTextElement);
                MetaContextElementUtils.SetText(amountTextElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_LEVEL_UP_FULL_LEVEL_MULTIPLIER_AMOUNT", multiplierValue));
            }

            if (nameTextElement != null)
                MetaContextElementUtils.SetText(nameTextElement, StringTableUtils.GetString(StringTable.StringTableType.Global, string.Format("POPUP_LEVEL_UP_FULL_LEVEL_MULTIPLIER_ITEM_{0}", itemData.Index)));
        }

        private void CreateIcon()
        {
            if (iconAreaElement == null || itemData == null) return;
            if (iconObject != null) Destroy(iconObject);

            string rewardName = itemData.TypeValue;
            if (rewardName == "vipDeal")
            {
                bool isVipDealV1Enabled = BlackboardUtils.FindVariable<bool>("/values/misc/ENABLE_VIP_DEAL")?.value ?? false;
                bool isVipDealV2Enabled = VipDealV2.Utils.IsEnabled();
                if (!isVipDealV1Enabled && isVipDealV2Enabled)
                {
                    rewardName = "VipDealV2";
                }
            }

            string itemName = string.Format("LM Shop Reward {0}", rewardName);
            iconObject = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, itemName, iconAreaElement.transform);
        }
    }
}