using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using UnityEngine;
using NodeCanvas.Framework;
using UnityEngine.UI;

namespace BagelCode
{
    public abstract class TotalResultPopupController : EventMonoBehaviour
    {
        protected ContextElement rootElement;
        protected Blackboard bb;
        protected Animator anim;

        public List<long> prizeList = new List<long>();

        private bool isBetterLuck = false;

        private const string APPEAR_SOUND = "Collecting_Game_Scratcher_Result_Appear";

        protected const StringTable.StringTableType GLOBAL = StringTable.StringTableType.Global;

        protected abstract string GetTitleText();
        protected abstract string GetInfoText();
        protected abstract string GetTypeText(int i);
        protected abstract string GetWinText(int i);
        protected abstract string GetPrizeText(int i);
        protected abstract string GetCollectButtonText();
        protected abstract string GetOkayButtonText();
        protected abstract string GetTotalText();

        protected virtual void OnClose() { }

        protected virtual void GetVariables() { }

        //

        private void Start()
        {
            rootElement = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();

            rootElement.UpdateContext(false);

            GetVariables();

            Init();
        }

        private void Init()
        {
            GSManager.Instance.GetHandler(APPEAR_SOUND).Play();

            StartCoroutine(EnableRect(1f));

            ContextElement buttonElement = ContextUtils.FindElement(rootElement, "Button Yellow", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetClickable(buttonElement, OnClickClose);

            ContextElement titleTextElement = ContextUtils.FindElement(rootElement, "Title Area Full/Text", ContextSearchingType.FullNameSearch);
            ContextElement resultInfoElement = ContextUtils.FindElement(rootElement, "Text Result", ContextSearchingType.ChildrenSearch);

            MetaContextElementUtils.SetText(titleTextElement, GetTitleText());
            MetaContextElementUtils.SetText(resultInfoElement, GetInfoText());

            ContextElement contentsElement = ContextUtils.FindElement(rootElement, "Scroll Rect/Contents", ContextSearchingType.FullNameSearch);

            long totalPrize = 0;
            for (int i = 0; i < prizeList.Count; i++)
            {
                long prizeL = prizeList[i];

                var cell = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Popup Contents Total Result Cell", contentsElement.transform);

                ContextElement cellElement = cell.GetComponent<ContextElement>();
                cellElement.UpdateContext(true);

                MetaContextElementUtils.SimpleSetText(cellElement, "Text Name", GetTypeText(i));
                MetaContextElementUtils.SimpleSetText(cellElement, "Text Win", GetWinText(i));
                MetaContextElementUtils.SimpleSetText(cellElement, "Text Result", GetPrizeText(i));

                totalPrize += prizeL;
            }

            ContextElement buttonTextElement = ContextUtils.FindElement(buttonElement, "Text", ContextSearchingType.ChildrenSearch);

            string buttonText = totalPrize > 0 ? GetCollectButtonText() : GetOkayButtonText();
            MetaContextElementUtils.SetText(buttonTextElement, buttonText);

            // Total
            ContextElement totalResultElement = ContextUtils.FindElement(rootElement, "Total Result", ContextSearchingType.ChildrenSearch);
            ContextElement textElement = ContextUtils.FindElement(totalResultElement, "Text", ContextSearchingType.ChildrenSearch);
            ContextElement textCoinElement = ContextUtils.FindElement(totalResultElement, "Text Coin", ContextSearchingType.ChildrenSearch);

            isBetterLuck = totalPrize == 0;
            totalResultElement.GetComponent<Animator>().enabled = !isBetterLuck;
            MetaContextElementUtils.SetActive(textCoinElement, !isBetterLuck);
            MetaContextElementUtils.SetActive(textElement, !isBetterLuck);

            if (isBetterLuck)
            {
                ContextElement textBetterLuckElement = ContextUtils.FindElement(totalResultElement, "Text Better Luck", ContextSearchingType.ChildrenSearch);
                MetaContextElementUtils.SetTextGlobal(textBetterLuckElement, "COLLECTING_GAME_TOTAL_RESULT_BETTER_LUCK");
                MetaContextElementUtils.SetActive(textBetterLuckElement, true);
            }
            else // Show Total
            {
                MetaContextElementUtils.SetText(textElement, GetTotalText());

                string textCoin = StringTableUtils.GetString(GLOBAL, "COMMA_STYLE_COIN", totalPrize);
                MetaContextElementUtils.SetText(textCoinElement, textCoin);
            }
        }

        private void OnClickClose()
        {
            if (isBetterLuck)
            {
                anim.SetTrigger("CloseBetterLuck");
            }
            else
            {
                anim.SetTrigger("Close");
            }

            OnClose();

            MetaPopupUtils.ClosePopup(gameObject);
        }

        private IEnumerator EnableRect(float delay)
        {
            yield return new WaitForSeconds(delay);

            ContextElement rectElement = ContextUtils.FindElement(rootElement, "Scroll Rect", ContextSearchingType.ChildrenSearch);
            rectElement.GetComponent<ScrollRect>().enabled = true;
        }
    }
}
