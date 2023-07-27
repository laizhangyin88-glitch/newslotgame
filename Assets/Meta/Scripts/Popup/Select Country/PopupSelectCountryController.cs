using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.UI;
using SlotMaker;
using BagelCode.ClientModels;
using BagelCode.OSA_Scroll;

namespace BagelCode
{
    public class PopupSelectCountryController : MonoBehaviour
    {
        public List<string> countryCodeList;
        public int selectCountryIndex;
        public string selectCountryCode;

        public OSA_SelectCountry osaController;

        private Blackboard rootBB;
        private Animator rootAnimator;
        private ContextElement rootElement;

        private ContextElement osaElement;
        private ContextElement closeButtonElement;

        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

        private bool isInit = false;

        private const string ON_CLOSE = "OnClose";
        private const string ON_CHANGE_SELECT_COUNTRY = "OnChangeSelectCountry";

        private void OnEnable()
        {
            MessageDispatcher.Register(MetaEventDefine.ON_META_UI_EVENT, OnMetaUIEvent);
        }

        private void OnDisable()
        {
            MessageDispatcher.UnRegister(MetaEventDefine.ON_META_UI_EVENT, OnMetaUIEvent);
        }

        private void InitProperty()
        {
            if(isInit) return;

            rootAnimator = gameObject.GetComponent<Animator>();
            rootBB = gameObject.GetComponent<Blackboard>();
            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            MetaContextElementUtils.SimpleSetText(rootElement, "Title Area/Text", StringTableUtils.GetString(tableType, "POPUP_SELECT_COUNTRY_TITLE"), ContextSearchingType.FullNameSearch);
            osaElement = ContextUtils.FindElement(rootElement, "Select Country Contents Area", ContextSearchingType.ChildrenSearch);
            osaController = osaElement.GetComponent<OSA_SelectCountry>();

            closeButtonElement = ContextUtils.FindElement(rootElement, "Button Close", ContextSearchingType.ChildrenSearch);

            MetaContextElementUtils.SetClickable(
                closeButtonElement,
                ON_CLOSE,
                rootElement,
                null
            );

            isInit = true;
        }

        public void InitCountryList(string initCountryCode)
        {
            InitProperty();

            countryCodeList = CountryUtils.GetCountryCodeList();

            selectCountryCode = initCountryCode;
            if(!string.IsNullOrEmpty(selectCountryCode))
                selectCountryIndex = countryCodeList.FindIndex(x => x == selectCountryCode);
            else
                selectCountryIndex = -1;

            osaController.InitList(countryCodeList, selectCountryIndex);
        }

        private void OnMetaUIEvent(EventData eventData)
        {
            if (ON_CHANGE_SELECT_COUNTRY == eventData.name)
            {
                osaController.UpdateSelectCountry((int)eventData.value);
            }
        }
    }
}
