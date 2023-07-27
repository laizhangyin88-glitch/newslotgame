using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using System.Linq;

namespace BagelCode
{
    public abstract class PopupWheelBoosterBaseController : MonoBehaviour
    {
        // Settings
        protected const int WHEEL_SIZE = 12;
        protected const int MIN_ROTATE_COUNT = 5;

        protected ContextElement root;
        protected Blackboard bb;
        protected Animator anim;

        protected Blackboard boostProduct;
        protected Blackboard boostItem;

        protected ContextElement wheelElement;
        protected ContextElement boostButtonElement;

        protected ContextElement resultMultiplierElement;
        protected ContextElement resultSpinCoinElement;

        protected ContextElement[] wheelHighlightElements = new ContextElement[WHEEL_SIZE];
        protected double[] wheelMultiplierList = new double[WHEEL_SIZE];
        protected long[] wheelBetList = new long[WHEEL_SIZE];
        protected long[] wheelCoinList = new long[WHEEL_SIZE];

        protected int targetIndex = 0;
        protected long baseBet = 0;
        protected bool silenceClose = false;
        protected bool isCoin = false;

        // Settings - InitBaseStringKeySetting()
        protected string titleTextKey;  // POPUP_CONTROL_SPIN_BOOSTER_TITLE
        protected string boosterButtonText; // POPUP_CONTROL_SPIN_BOOSTER_BOOST_BUTTON + price

        protected const float DELTA_ANGLE = 360f / WHEEL_SIZE;

        protected const string CLOSE_EVENT = "Close";
        protected const string PURCHASE_EVENT = "OnPurchase";
        protected const string OPEN_PROP_POPUP_EVENT = "OnOpenPropPopup";

        protected const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        protected const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;
        protected const StringTable.StringTableType GLOBAL = StringTable.StringTableType.Global;

        protected abstract void InitBaseStringKeySetting();
        protected abstract void UpdateResultTexts(int idx);
        public abstract void Refresh();

        public virtual void InitProperty()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            boostProduct = bb.GetVariable<Blackboard>("product")?.value;
            boostItem = boostProduct.GetValue<List<Blackboard>>("itemList")[0];

            InitBaseStringKeySetting();
            InitSaleTag();
            InitInboxTag();
            InitButton();
            InitWheel();

            InitText();

            // Game Icon
            MakeSlotThumbnailIcon();

            GetCallerContextID();
        }

        protected virtual void InitSaleTag()
        {
            // Sale Tag
            var saleTagElement = ContextUtils.FindElement(root, "Button Boost/Sale Tag", FULL);
            int salePercent = BlackboardQueryUtils.GetProductSalePercent(boostProduct);
            MetaContextElementUtils.SetActive(saleTagElement, salePercent > 0);
            if (salePercent > 0)
            {
                MetaContextElementUtils.SimpleSetTextGlobal(
                    saleTagElement, "Text", "TEXT_COMMON_PASSIVE_EVENT_PERCENT_SALE", CHILDREN, salePercent);
            }
        }

        protected virtual void InitInboxTag()
        {
            // Inbox Tag
            ContextElement inboxTagElement = ContextUtils.FindElement(root, "Slot Thumbnail/Check", FULL);
            inboxTagElement?.gameObject.SetActive(bb.GetValue<bool>("isInbox"));
        }

        // Button : Boost / Close / Odds
        protected virtual void InitButton()
        {
            // Boost
            boostButtonElement = ContextUtils.FindElement(root, "Button Boost", CHILDREN);
            MetaContextElementUtils.SetActive(boostButtonElement, true);
            MetaContextElementUtils.SetClickable(boostButtonElement, () => EventSender.SendEvent(gameObject, PURCHASE_EVENT));
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Title Text", titleTextKey, CHILDREN);

            // Close
            MetaContextElementUtils.SimpleSetClickable(root, "Button Close", () => EventSender.SendEvent(gameObject, CLOSE_EVENT));
            MetaSystem.SubscribeBackButton(gameObject.GetHashCode(), () => EventSender.SendEvent(gameObject, CLOSE_EVENT));

            // Odds
            var oddsButtonElement = ContextUtils.FindElement(root, "Button Odds", CHILDREN);
            MetaContextElementUtils.SetClickable(oddsButtonElement, () => EventSender.SendEvent(gameObject, OPEN_PROP_POPUP_EVENT));
            MetaContextElementUtils.SimpleSetTextGlobal(oddsButtonElement, "Text", "BUTTON_ODDS", CHILDREN);
        }

        protected virtual void InitWheel()
        {
            long coin = bb.GetValue<long>("purchasedCredit");
            baseBet = bb.GetValue<long>("baseBet");
            isCoin = coin > 0L; 
            // Wheel
            List<long> wheelMultiplierNumeratorList = boostItem.GetValue<List<Blackboard>>("setting")
                .Select(s => s.GetValue<long>("multiplierNumerator")).ToList();

            wheelElement = ContextUtils.FindElement(root, "Wheel", CHILDREN);
            var numbersElement = ContextUtils.FindElement(root, "Wheel/Numbers", FULL);
            var highlightElement = ContextUtils.FindElement(root, "Wheel/Highlight", FULL);
            for (int i = 0; i < WHEEL_SIZE; ++i)
            {
                long multiplierNumerator = wheelMultiplierNumeratorList[i];
                double multiplier = NumberUtils.GetMultiplierFromNumerator(multiplierNumerator);

                string indexName = StringTableUtils.GetString(GLOBAL, "TEXT_ZERO_2", (i + 1));
                MetaContextElementUtils.SimpleSetTextGlobal(numbersElement, indexName, "POPUP_CONTROL_SPIN_BOOSTER_PANEL_TEXT", CHILDREN, multiplier);

                wheelHighlightElements[i] = ContextUtils.FindElement(highlightElement, indexName, CHILDREN);

                wheelMultiplierList[i] = multiplier;
                wheelCoinList[i] = (long)(coin * multiplier);
                wheelBetList[i] = (long)(baseBet * multiplier);
            }
        }

        protected virtual void InitText()
        {
            long rp = boostItem.GetValue<long>("rp");
            long coin = bb.GetValue<long>("purchasedCredit");

            // Boost
            MetaContextElementUtils.SimpleSetText(boostButtonElement, "Text", boosterButtonText, CHILDREN);

            var textNormalElement = ContextUtils.FindElement(root, "Text Normal", CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(textNormalElement, "Text Multiplier Up To", "POPUP_CONTROL_SPIN_BOOSTER_UP_TO_TEXT", CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(textNormalElement, "Text Get", "POPUP_CONTROL_SPIN_BOOSTER_GET_TEXT", CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(textNormalElement, "Text Vip Point", "POPUP_CONTROL_SPIN_BOOSTER_RP_TEXT", CHILDREN, rp);

            MetaContextElementUtils.SimpleSetTextGlobal(textNormalElement, "Text Multiplier", "POPUP_CONTROL_SPIN_BOOSTER_MULTIPLIER_TEXT", CHILDREN, wheelMultiplierList[0]);
            if (isCoin)
            {
                MetaContextElementUtils.SimpleSetTextGlobal(root, "Text Purchased", "POPUP_CONTROL_SPIN_BOOSTER_PURCHASED_TEXT", CHILDREN, coin);
                MetaContextElementUtils.SimpleSetTextGlobal(root, "Text Purchased Result", "POPUP_CONTROL_SPIN_BOOSTER_PURCHASED_TEXT", CHILDREN, coin);
                MetaContextElementUtils.SimpleSetTextGlobal(textNormalElement, "Text Coin", "POPUP_CONTROL_SPIN_BOOSTER_COIN_SPIN_TEXT", CHILDREN, wheelCoinList[0], wheelBetList[0]);
            }
            else
            {
                MetaContextElementUtils.SimpleSetTextGlobal(root, "Text Purchased", "POPUP_CONTROL_SPIN_BOOSTER_PURCHASED_SPIN_TEXT", CHILDREN, baseBet);
                MetaContextElementUtils.SimpleSetTextGlobal(root, "Text Purchased Result", "POPUP_CONTROL_SPIN_BOOSTER_PURCHASED_SPIN_TEXT", CHILDREN, baseBet);
                MetaContextElementUtils.SimpleSetTextGlobal(textNormalElement, "Text Coin", "POPUP_CONTROL_SPIN_BOOSTER_SPIN_TEXT", CHILDREN, wheelBetList[0]);
            }

            var textResultElement = ContextUtils.FindElement(root, "Text Result", CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(textResultElement, "Text Multiplier", "POPUP_CONTROL_SPIN_BOOSTER_RESULT_MULTIPLIER_TEXT", CHILDREN);
            resultMultiplierElement = ContextUtils.FindElement(textResultElement, "Text Multiplier Result", CHILDREN);
            resultSpinCoinElement = ContextUtils.FindElement(textResultElement, "Text Coin Result", CHILDREN);
        }

        public virtual void Close()
        {
            if (!silenceClose)
                EventSender.SendCalleeCallback(gameObject);

            anim.SetTrigger("Close");
            MetaSystem.UnSubscribeBackButton(gameObject.GetHashCode());
            MetaPopupUtils.ClosePopup(gameObject);
        }

        public virtual IEnumerator Spin()
        {
            // Disable BackButton
            MetaObjectUtils.EnableBackButton(false);

            // Set Target Index
            var purchaseResponse = BlackboardUtils.FindVariable<Blackboard>("/purchaseResponse")?.value;
            var purchaseResultItem = purchaseResponse.GetValue<List<Blackboard>>("itemUseResultList")[0];
            targetIndex = purchaseResultItem.GetValue<int>("multiplierIndex");

            // Spin
            var rigidbody = wheelElement.GetComponent<Rigidbody>();
            rigidbody.AddRelativeTorque(new Vector3(0f, 0f, -10f), ForceMode.Impulse);

            anim.SetTrigger("Spin");

            // Spin to Target Index
            float targetAngle = DELTA_ANGLE * targetIndex;
            wheelElement.GetComponent<BigWheel>().Simulation(0f, targetAngle, MIN_ROTATE_COUNT);

            // Until Wheel Stop
            ContextElement currentHighlightElement = null;
            Animator currentHighlightAnim = null;
            int lastIndex = 0;

            yield return new WaitForSeconds(0.1f);

            while (rigidbody.angularVelocity.magnitude >= 0.05f)
            {
                // Highlight
                float currentAngle = MetaContextElementUtils.GetFloatProperty(wheelElement);
                int currentIndex = (int)((currentAngle + 366.5f) / 30f) % 12;

                // If Index Changed
                if (currentIndex != lastIndex)
                {
                    // Play Loop Sound
                    GSManager.Instance.GetHandler("UI_Coin_Booster_Wheel_Loop").Play();

                    // Disable Last
                    var lastHighlightElement = wheelHighlightElements[lastIndex];
                    var lastHighlightAnim = lastHighlightElement.GetComponent<Animator>();
                    lastHighlightAnim.SetBool("Highlight", false);
                    lastIndex = currentIndex;

                    // Enable Current
                    currentHighlightElement = wheelHighlightElements[currentIndex];
                    currentHighlightAnim = currentHighlightElement.GetComponent<Animator>();
                    currentHighlightAnim.SetBool("Highlight", true);

                    // Update Text
                    UpdateResultTexts(currentIndex);
                }

                yield return new WaitForEndOfFrame();
            }

            if (currentHighlightAnim != null)
            {
                currentHighlightAnim.SetBool("Highlight", false);
                currentHighlightAnim.SetBool("Win", true);
            }

            // Play Stop Sound
            GSManager.Instance.GetHandler("UI_Coin_Booster_Stop").Play();

            anim.SetTrigger("Stop");

            // Play Win
            yield return new WaitForSeconds(3f);

            // Open Boost Result Popup
            yield return StartCoroutine(OpenBoostResultPopupCoroutine());

            // Enable BackButton
            MetaObjectUtils.EnableBackButton(true);
        }

        private IEnumerator OpenBoostResultPopupCoroutine()
        {
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Popup Coin Booster Total Result Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject resultPopupObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(bundle, asset, parent,
                (SceneLoadOperation sceneLoadOperation) => resultPopupObj = sceneLoadOperation.GetScene()));

            var resultPopupBB = resultPopupObj.GetComponent<Blackboard>();
            resultPopupBB.AddVariable("isRedeem", false);
            resultPopupBB.AddVariable("isFromSpinDeal", true);
            resultPopupBB.AddVariable("baseBet", baseBet);

            var caller = bb.GetVariable<GameObject>("caller")?.value;
            MetaObjectUtils.SetCalleeCaller(resultPopupObj, caller);
            silenceClose = true;

            MetaPopupUtils.OpenPopup(resultPopupObj);
        }

        public IEnumerator OpenProbPopupCoroutine()
        {
            // Send Bi
            string contextId = bb.GetValue<string>("contextId");
            Dictionary<string, object> customData = new Dictionary<string, object>();
            customData["context_id"] = contextId;
            Analytics.CustomEvent("client_click_spin_boost_odds", customData);

            // Make Popup
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Popup Loot Box Info Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject propPopupObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(bundle, asset, parent,
                (SceneLoadOperation sceneLoadOperation) => propPopupObj = sceneLoadOperation.GetScene()));

            var popupBB = propPopupObj.GetComponent<Blackboard>();
            popupBB.AddVariable("probType", ProbType.SPIN_BOOST);
            popupBB.AddVariable("productID", boostProduct.GetValue<int>("id"));
            popupBB.AddVariable("passiveEventID", 0);

            MetaObjectUtils.SetCalleeCaller(propPopupObj, gameObject);

            MetaPopupUtils.OpenPopup(propPopupObj);

            var callbackTrigger = new EventTrigger(gameObject, EventSender.ON_CALLEE_CALLBACK);
            yield return new WaitUntilTrigger(callbackTrigger);
        }

        private void GetCallerContextID()
        {
            // caller: IAM Base
            var caller = bb.GetValue<GameObject>("caller");
            var callerBB = caller.GetComponent<Blackboard>();
            string contextId = callerBB.GetValue<string>("_biContextID");
            bb.AddVariable("contextId", contextId);
        }

        protected void MakeSlotThumbnailIcon()
        {
            int gameId = bb.GetValue<int>("gameId");
            string title = BlackboardQueryUtils.GetGameTitle(gameId);
            var slotThumbnailAreaElement = ContextUtils.FindElement(root, "Slot Thumbnail/Image Area", FULL);

            MetaIconUtils.MakeSlotThumbnailIconObjectFromGameTitle(
                title, slotThumbnailAreaElement.transform, null);
        }
    }
}