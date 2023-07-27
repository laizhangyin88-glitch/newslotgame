using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Chat
{
    public class PopupBuyGlobalChatController : MonoBehaviour
    {
        private int selectIndex = 1;
        private int speakerAmount => selectIndex * speakerInterval;
        private int speakerPrice => selectIndex * gemInterval;

        private ContextElement mContextElement => GetComponent<ContextElement>();
        private ContextButton closeButtonContext => mContextElement.Find("Button Close").GetComponent<ContextButton>();
        private ContextButton minusButtonContext => mContextElement.Find("Button Minus").GetComponent<ContextButton>();
        private ContextButton plusButtonContext => mContextElement.Find("Button Plus").GetComponent<ContextButton>();
        private ContextButton buyButtonContext => mContextElement.Find("Button Buy").GetComponent<ContextButton>();
        private IContextText titleContext => ContextUtils.FindElement(mContextElement, "Title Area/Text", ContextSearchingType.FullNameSearch).GetComponent<IContextText>();
        private IContextText amountTextContext => mContextElement.Find("Text Bet").GetComponent<IContextText>();
        private ContextTextMeshProUGUI buyButtonTextContext => ContextUtils.FindElement(mContextElement, "Button Buy/Text", ContextSearchingType.FullNameSearch).GetComponent<ContextTextMeshProUGUI>();

        private int speakerInterval => BlackboardUtils.FindValue<int>("/values/misc/SPEAKER/SALES_COUNT_INTERVAL");
        private int minSaleCount => BlackboardUtils.FindValue<int>("/values/misc/SPEAKER/MIN_SALES_COUNT");
        private int maxSaleCount => BlackboardUtils.FindValue<int>("/values/misc/SPEAKER/MAX_SALES_COUNT");
        private int gemInterval => BlackboardUtils.FindValue<int>("/values/misc/SPEAKER/GEM_VALUE_FOR_SALES_COUNT_INTERVAL");

        // Start is called before the first frame update
        private void Start()
        {
            mContextElement.UpdateContext();
            titleContext.SetGlobalText("POPUP_BUY_GLOBAL_CHAT_TITLE");

            closeButtonContext.button.onClick.AddListener(
            () =>
            {
                PopupManager.Instance.Close(gameObject);
                Destroy(gameObject);
            });

            buyButtonContext.button.onClick.AddListener(
            () =>
            {
                var meBB = MainBlackboard.Get().GetVariable<Blackboard>("me");
                long gem = meBB.value.GetValue<long>("gem");
                if (gem < speakerPrice)
                {
                    var shopObj = MetaObjectUtils.MakeScene("Shop Scene", PopupManager.Instance.transform.Find("Area"));
                    shopObj.SetActive(false);
                    shopObj.GetComponent<Blackboard>().SetValue("initTabIndex", 1);
                    PopupManager.Instance.Open(shopObj);
                    shopObj.SetActive(true);
                    return;
                }

                // Open Loading Popup
                GameObject loadingObj = MetaPopupUtils.OpenLoadingPopup();

                var bb = GetComponent<Blackboard>();
                if(bb == null)
                    bb = gameObject.AddComponent<Blackboard>();

                var AEProductBB = ProductUtils.MakeSpeakerProduct(bb, speakerAmount, speakerPrice);

                buyButtonContext.button.interactable = false;

                BagelCodeClientAPI.BuySpeakerWithGem(speakerAmount, BiEventUtils.chatEnterContextID,
                    (res) =>
                    {
                        BiEventUtils.GemTransaction(AEProductBB, BiEventUtils.chatEnterContextID, true, false);
                        BiEventUtils.ItemAcquired(AEProductBB, BiEventUtils.chatEnterContextID, false);

                        BlackboardQueryUtils.UpdateUserSyncInfo(res.userSyncInfo, res.serverTime);
                        BlackboardQueryUtils.ApplyUserSyncInfo();
                        PopupManager.Instance.Close(gameObject);
                        Destroy(gameObject);

                        // Close Loading Popup
                        MetaPopupUtils.ClosePopup(loadingObj);
                    },
                    (error) =>
                    {
                        BiEventUtils.GemTransaction(AEProductBB, BiEventUtils.chatEnterContextID, false, false);

                        PopupManager.Instance.Close(gameObject);
                        Destroy(gameObject);

                        // Close Loading Popup
                        MetaPopupUtils.ClosePopup(loadingObj);
                    }
                );
            });


            plusButtonContext.button.onClick.AddListener(PlusBet);
            minusButtonContext.button.onClick.AddListener(MinusBet);



            UpdateAmount();
        }

        public void PlusBet()
        {
            if ((selectIndex + 1) * speakerInterval > maxSaleCount) return;
            selectIndex++;
            UpdateAmount();
        }

        public void MinusBet()
        {
            if ((selectIndex - 1) * speakerInterval < minSaleCount) return;
            selectIndex--;
            UpdateAmount();
        }

        private void UpdateAmount()
        {
            amountTextContext.SetGlobalText("POPUP_BUY_GLOBAL_CHAT_AMOUNT_TEXT", speakerAmount);
            minusButtonContext.button.interactable = ((selectIndex - 1) * speakerInterval >= minSaleCount);
            plusButtonContext.button.interactable = ((selectIndex + 1) * speakerInterval <= maxSaleCount);
            buyButtonTextContext.SetGlobalText("POPUP_BUY_GLOBAL_CHAT_BUY_BUTTON", speakerPrice);
        }
    }
}
