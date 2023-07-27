using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using ParadoxNotion;
using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class PopupControlSpinDealSelectController : MonoBehaviour
    {
        [SerializeField]
        private int gameId;

        private ContextElement root;
        private Blackboard bb;

        private Blackboard inboxResponse;

        private bool isChoice = false;

        private ContextElement[] cellElements = new ContextElement[CELL_COUNT];
        private ContextElement[] spinHighlightTextElements = new ContextElement[CELL_COUNT];
        private ContextElement[] itemHighlightTextElements = new ContextElement[CELL_COUNT];
        private Animator[] cellAnims = new Animator[CELL_COUNT];
        private long[] bets = new long[CELL_COUNT];
        private int[] spins = new int[CELL_COUNT];

        private const int CELL_COUNT = 5;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private const StringTable.StringTableType GLOBAL = StringTable.StringTableType.Global;

        public void InitProperty()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();

            root.UpdateContext(false);

            // Block back button
            MetaSystem.SubscribeBackButton(gameObject.GetHashCode(), () => { });
            
            inboxResponse = bb.GetValue<Blackboard>("inboxResponse");
            isChoice = bb.GetValue<bool>("isChoice");

            gameId = inboxResponse.GetValue<int>("gameId");
            var betList = inboxResponse.GetValue<List<long>>("betList");
            var spinCountList = inboxResponse.GetValue<List<int>>("spinCountList");

            // Send Bi
            Dictionary<string, object> customData = new Dictionary<string, object>();
            customData["game_id"] = gameId;
            customData["original_bet"] = betList[0];
            customData["original_spin_count"] = spinCountList[0];
            customData["context_id"] = InboxUtils.GetInboxEnterContextID();
            Analytics.CustomEvent("client_click_spin_deal_button", customData);
            
            // Title
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Title Text", "POPUP_CONTROL_SPIN_DEAL_SELECT_TITLE", CHILDREN);

            if (isChoice) InitChoice(betList, spinCountList);
            else InitSelect(betList, spinCountList);
        }

        private void InitSelect(List<long> betList, List<int> spinCountList)
        {
            // Cell
            for (int i = 0; i < CELL_COUNT; ++i)
            {
                string elementName = string.Format("Control Spin Deal Select Cell {0}", i + 1);
                cellElements[i] = ContextUtils.FindElement(root, elementName, CHILDREN);
                cellAnims[i] = cellElements[i].GetComponent<Animator>();

                var cellElement = cellElements[i];

                // Play Button
                var playButtonElement = ContextUtils.FindElement(cellElement, "Button Area/Button Play", FULL);
                MetaContextElementUtils.SimpleSetTextGlobal(playButtonElement, "Text", "POPUP_CONTROL_SPIN_DEAL_SELECT_PLAY_BUTTON", CHILDREN);

                int playIdx = i;
                MetaContextElementUtils.SetClickable(playButtonElement,
                    () => EventSender.SendEvent(gameObject, new EventData<int>("OnPlay", playIdx)));

                // Bet, Spin
                string betText;
                string spinText;
                bool isRandom = i == (CELL_COUNT - 1);
                if (isRandom) // Random
                {
                    betText = StringTableUtils.GetString(GLOBAL, "POPUP_CONTROL_SPIN_DEAL_SELECT_RANDOM_BET_TEXT");
                    spinText = StringTableUtils.GetString(GLOBAL, "POPUP_CONTROL_SPIN_DEAL_SELECT_RANDOM_SPIN_TEXT");
                }
                else // Not Random
                {

                    bets[i] = LevelUtils.GetLevelMultiplierNumeratorValue(betList[i], "spinDeal");
                    spins[i] = spinCountList[i];

                    betText = StringTableUtils.GetString(GLOBAL, "POPUP_CONTROL_SPIN_DEAL_SELECT_BET_TEXT", bets[i]);
                    spinText = StringTableUtils.GetString(GLOBAL, "POPUP_CONTROL_SPIN_DEAL_SELECT_SPIN_TEXT", spins[i]);
                }
                MetaContextElementUtils.SimpleSetText(cellElement, "Item Text", betText);
                MetaContextElementUtils.SimpleSetText(cellElement, "Spin Text", spinText);

                itemHighlightTextElements[i] = ContextUtils.FindElement(cellElement, "Item Highlight Text", CHILDREN);
                spinHighlightTextElements[i] = ContextUtils.FindElement(cellElement, "Spin Highlight Text", CHILDREN);
                MetaContextElementUtils.SetText(itemHighlightTextElements[i], betText);
                MetaContextElementUtils.SetText(spinHighlightTextElements[i], spinText);
            }
        }

        private void InitChoice(List<long> betList, List<int> spinCountList)
        {
            var moreTagList = inboxResponse.GetValue<List<int>>("moreTagList");
            var extraSpinCountList = inboxResponse.GetValue<List<int>>("extraSpinCountList");

            for (int i = 0; i < CELL_COUNT; ++i)
            {
                string elementName = string.Format("Choice Spin Deal Select Cell {0}", i + 1);
                cellElements[i] = ContextUtils.FindElement(root, elementName, CHILDREN);
                cellAnims[i] = cellElements[i].GetComponent<Animator>();

                var cellElement = cellElements[i];

                // Play Button
                var playButtonElement = ContextUtils.FindElement(cellElement, "Button Area/Button Play", FULL);
                MetaContextElementUtils.SimpleSetTextGlobal(playButtonElement, "Text", "POPUP_CONTROL_SPIN_DEAL_SELECT_PLAY_BUTTON", CHILDREN);

                int playIdx = i;
                MetaContextElementUtils.SetClickable(playButtonElement,
                    () => EventSender.SendEvent(gameObject, new EventData<int>("OnPlay", playIdx)));

                // Info
                bets[i] = LevelUtils.GetLevelMultiplierNumeratorValue(betList[i], "spinDeal");
                spins[i] = spinCountList[i];

                if(extraSpinCountList[i] > 0)
                {
                    MetaContextElementUtils.SimpleSetTextGlobal(cellElement, "Information Text",
                        "POPUP_CONTROL_SPIN_DEAL_CHOICE_BET_EXTRA_TEXT", CHILDREN,
                        bets[i], spins[i], extraSpinCountList[i]);
                }
                else
                {
                    MetaContextElementUtils.SimpleSetTextGlobal(cellElement, "Information Text",
                        "POPUP_CONTROL_SPIN_DEAL_CHOICE_BET_TEXT", CHILDREN,
                        bets[i], spins[i]);
                }

                // Title
                MetaContextElementUtils.SimpleSetTextGlobal(cellElement, "Title Text", "POPUP_CONTROL_SPIN_DEAL_CHOICE_TITLE", CHILDREN);

                // Total Bet
                long totalBet = bets[i] * (spins[i] + extraSpinCountList[i]);
                MetaContextElementUtils.SimpleSetTextGlobal(cellElement, "Total Bet Text",
                    "POPUP_CONTROL_SPIN_DEAL_CHOICE_TOTAL_BET", CHILDREN, totalBet);

                // More %
                if (i > 0)
                {
                    MetaContextElementUtils.SimpleSetTextGlobal(cellElement, "Bage More Text",
                        "POPUP_CONTROL_SPIN_DEAL_CHOICE_MORE", CHILDREN, moreTagList[i]);
                }
            }
        }

        public IEnumerator PlayChoice(int idx)
        {
            // Play Sound
            GSManager.Instance.GetHandler("UI_Button_Normal").Play();

            // Loading
            GameObject loadingPopupObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenLoadingPopupCoroutine(
                (GameObject popupObj) => loadingPopupObj = popupObj));

            int spinDealId = inboxResponse.GetValue<int>("spinDealId");

            // Request
            bool success = false;
            bool fail = false;
            BagelCodeClientAPI.RequestControlSpinDeal2(
                idx,
                spinDealId,
                InboxUtils.GetInboxEnterContextID(),
                (response) =>
                {
                    ClientAPI2Blackboard.Serialize(bb, response);

                    success = true;
                },
                (error) =>
                {
                    GlobalErrorHandler.GlobalError(error);

                    fail = true;
                });

            yield return new WaitUntil(() => success || fail);

            MetaPopupUtils.ClosePopup(loadingPopupObj);

            // Play
            EnterGame();
        }

        public IEnumerator PlaySelect(int idx)
        {
            // Play Sound
            GSManager.Instance.GetHandler("UI_Button_Normal").Play();

            // Loading
            GameObject loadingPopupObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenLoadingPopupCoroutine(
                (GameObject popupObj) => loadingPopupObj = popupObj));

            int spinDealId = inboxResponse.GetValue<int>("spinDealId");

            // Request
            bool success = false;
            bool fail = false;
            BagelCodeClientAPI.RequestControlSpinDeal(
                (BetSpinIndex)idx,
                spinDealId,
                InboxUtils.GetInboxEnterContextID(),
                (response) =>
                {
                    ClientAPI2Blackboard.Serialize(bb, response);

                    success = true;
                },
                (error) =>
                {
                    GlobalErrorHandler.GlobalError(error);

                    fail = true;
                });

            yield return new WaitUntil(() => success || fail);

            MetaPopupUtils.ClosePopup(loadingPopupObj);

            // Play
            bool isRandom = idx == (CELL_COUNT - 1);
            if (isRandom)
            {
                StartCoroutine(PlayRandomCoroutine());
            }
            else
            {
                EnterGame();
            }
        }

        //

        private IEnumerator PlayRandomCoroutine()
        {
            int targetBetIndex = bb.GetValue<int>("betIndex");
            int targetSpinIndex = bb.GetValue<int>("spinCountIndex");
            int randomIdx = CELL_COUNT - 1;

            // Ready
            foreach (var cellAnim in cellAnims)
                cellAnim.SetTrigger("isReady");

            yield return new WaitForSeconds(1.2f);

            // Swap Value
            MetaContextElementUtils.SimpleSetTextGlobal(cellElements[randomIdx], "Item Text",
                "POPUP_CONTROL_SPIN_DEAL_SELECT_BET_TEXT", CHILDREN, bets[targetBetIndex]);
            MetaContextElementUtils.SimpleSetTextGlobal(cellElements[randomIdx], "Spin Text",
                "POPUP_CONTROL_SPIN_DEAL_SELECT_SPIN_TEXT", CHILDREN, spins[targetSpinIndex]);

            // Highlight Bet
            cellAnims[targetBetIndex].SetTrigger("isHighlightBet");
            GSManager.Instance.GetHandler("UI_Spin_Deal_Select").Play();
            yield return new WaitForSeconds(0.7f);

            // Show Bet
            cellAnims[randomIdx].SetTrigger("isBet");
            GSManager.Instance.GetHandler("UI_Spin_Deal_Hit").Play();
            yield return new WaitForSeconds(1.2f);

            // Highlight Spin
            cellAnims[targetSpinIndex].SetTrigger("isHighlightSpin");
            GSManager.Instance.GetHandler("UI_Spin_Deal_Select").Play();
            yield return new WaitForSeconds(0.7f);

            // Show Spin
            cellAnims[randomIdx].SetTrigger("isSpin");
            GSManager.Instance.GetHandler("UI_Spin_Deal_Hit").Play();
            yield return new WaitForSeconds(1f);

            EnterGame();
        }

        private void EnterGame()
        {
            MetaSystem.UnSubscribeBackButton(gameObject.GetHashCode());

            BlackboardQueryUtils.SetEnterGameInfo(
                gameId,
                "EnterGame",
                "inbox",
                null,
                0,
                null,
                false,
                0,
                null);

            EventSender.SendGlobalEvent(new EventData("OnEnterGame"));
        }
    }
}
