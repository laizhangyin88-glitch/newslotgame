using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class PopupCoinBoosterTotalResultController : MonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        public void InitProperty()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();

            root.UpdateContext(false);

            // Title
            string titleText = bb.GetValue<string>("titleKey");
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Title Area Full/Text", titleText, FULL);

            // Set Item
            var useItemResultList = BlackboardUtils.FindVariable<List<Blackboard>>("/purchaseResponse/itemUseResultList")?.value;
            var useItem = useItemResultList[0];

            long earnCoin = useItem.GetVariable<long>("earnCredit")?.value ?? 0L;
            long prevEarnCoin = useItem.GetVariable<long>("origEarnCredit")?.value ?? 0L;
            long totalEarnCoin = earnCoin + prevEarnCoin;
            long earnRp = useItem.GetVariable<long>("earnRp")?.value ?? 0;

            double multiplier = useItem.GetVariable<double>("multiplier")?.value ?? 1.0;
            ItemType itemType = useItem.GetVariable<ItemType>("itemType").value;

            long multiplierNumerator = 100L;
            var multiplierNumberatorVariable = useItem.GetVariable<long>("multiplierNumerator");
            if (multiplierNumberatorVariable != null)
            {
                multiplierNumerator = multiplierNumberatorVariable.value;
                multiplier = NumberUtils.GetMultiplierFromNumerator(multiplierNumerator);
            }

            bool isFromSpinDeal = bb.GetVariable<bool>("isFromSpinDeal")?.value ?? false;
            if(isFromSpinDeal)
            {
                long baseBet = bb.GetValue<long>("baseBet");
                long totalBet = NumberUtils.GetMultiplierNumeratorValue(baseBet, multiplierNumerator);

                if (totalEarnCoin > 0L)
                {
                    MetaContextElementUtils.SimpleSetTextGlobal(root, "Total Result/Text Coin", "POPUP_CONTROL_SPIN_BOOSTER_RESULT_COIN_SPIN", FULL, totalEarnCoin, totalBet);
                    MetaContextElementUtils.SimpleSetTextGlobal(root, "Purchased Coins/Text Title", "POPUP_CONTROL_SPIN_BOOSTER_RESULT_COINS_TITLE", FULL);
                }
                else
                {
                    MetaContextElementUtils.SimpleSetTextGlobal(root, "Total Result/Text Coin", "POPUP_CONTROL_SPIN_BOOSTER_RESULT_SPIN", FULL, totalBet);
                    MetaContextElementUtils.SimpleSetTextGlobal(root, "Purchased Coins/Text Title", "POPUP_CONTROL_SPIN_BOOSTER_RESULT_BET_TITLE", FULL);
                    MetaContextElementUtils.SimpleSetTextGlobal(root, "Purchased Coins/Text Coin", "POPUP_RESULT_COIN_MULTIPLIER_BET_TEXT", FULL, totalBet);
                }

                MetaContextElementUtils.SimpleSetTextGlobal(root, "Coin Booster/Text Title", "POPUP_CONTROL_SPIN_BOOSTER_RESULT_SPINS_TITLE", FULL);
            }
            else
            {
                MetaContextElementUtils.SimpleSetTextGlobal(root, "Total Result/Text Coin", "POPUP_RESULT_COIN_MULTIPLIER_TOTAL_TEXT", FULL, totalEarnCoin);
                MetaContextElementUtils.SimpleSetTextGlobal(root, "Purchased Coins/Text Title", "POPUP_COIN_MULTIPLIER_COINS_TITLE", FULL);
                MetaContextElementUtils.SimpleSetTextGlobal(root, "Coin Booster/Text Title", "POPUP_COIN_MULTIPLIER_BOOSTER_TITLE", FULL);
            }

            if (totalEarnCoin > 0L)
                MetaContextElementUtils.SimpleSetTextGlobal(root, "Purchased Coins/Text Coin", "POPUP_RESULT_COIN_MULTIPLIER_COIN_TEXT", FULL, prevEarnCoin);
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Coin Booster/Text Multiplier", "POPUP_RESULT_COIN_MULTIPLIER_TEXT", FULL, multiplier);
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Coin Booster Total Result Vip Point/Text Vip Point", "POPUP_RESULT_COIN_MULTIPLIER_RP_TEXT", FULL, earnRp);

            MetaContextElementUtils.SimpleSetClickable(root, "Button Green", () => EventSender.SendEvent(gameObject, "OnClose"));

            MetaContextElementUtils.SimpleSetTextGlobal(root, "Button Green/Text", "BUTTON_COLLECT", FULL);

            // Play Sound
            GSManager.Instance.GetHandler("UI_Coin_Booster_Result").Play();
        }
    }
}
