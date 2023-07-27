using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.UI;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class PopupSelectCountryCellController : MonoBehaviour
    {
        // private PopupSelectCountryController owner;

        private Animator rootAnimator;
        private ContextElement rootElement;

        private List<ContextElement> baseCellList = new List<ContextElement>();

        private ContextElement countryIconAreaElement;
        private ContextElement countryButtonElement;
        private ContextElement countryIconElement;

        private ContextElement countryNameElement;
        private ContextElement countryNameSelectElement;

        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

        private bool isInit = false;

        private bool isSelect = false;
        private int cellIndex = -1;
        private string countryCode;

        private const string ON_SELECT_COUNTRY = "OnSelectCountry";
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
            // rootBB = gameObject.GetComponent<Blackboard>();
            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            baseCellList = new List<ContextElement>();
            baseCellList.Add( ContextUtils.FindElement(rootElement, "Base Cell 1", ContextSearchingType.ChildrenSearch) );
            baseCellList.Add( ContextUtils.FindElement(rootElement, "Base Cell 2", ContextSearchingType.ChildrenSearch) );
            baseCellList.Add( ContextUtils.FindElement(rootElement, "Base Cell Select", ContextSearchingType.ChildrenSearch) );

            countryIconAreaElement = ContextUtils.FindElement(rootElement, "Icon Country Area", ContextSearchingType.ChildrenSearch);
            var imageIconObj = MetaObjectUtils.MakePrefab("Icon Image", countryIconAreaElement.transform);
            countryIconElement = imageIconObj.GetComponent<ContextElement>();

            countryNameElement = ContextUtils.FindElement(rootElement, "Text Country", ContextSearchingType.ChildrenSearch);
            countryNameSelectElement = ContextUtils.FindElement(rootElement, "Text Country Select", ContextSearchingType.ChildrenSearch);

            IContextClickable clickableElement = rootElement as IContextClickable;
            clickableElement.RemoveAllListener();
            clickableElement.AddListenerOnClick(
                (context) =>
                {
                    OnSelect();
                }
            );

            countryNameElement.gameObject.SetActive(true);
            countryNameSelectElement.gameObject.SetActive(false);

            isInit = true;
        }

        public void Refresh(string code, int index, int selectIndex)
        {
            InitProperty();

            cellIndex = index;
            countryCode = code;

            string countryName = StringTableUtils.GetString(tableType, "POPUP_SELECT_COUNTRY_CELL_TEXT", CountryUtils.GetCountryName(countryCode) );
            MetaContextElementUtils.SetText(countryNameElement, countryName);
            MetaContextElementUtils.SetText(countryNameSelectElement, countryName);

            MetaContextElementUtils.SetContextCountryImage(countryIconElement, countryCode);

            UpdateSelectCell(selectIndex);
        }

        private void UpdateSelectCell(int selectIndex)
        {
            isSelect = cellIndex == selectIndex;
            int cellStyle = isSelect ? 2 : cellIndex%2;

            for(int i=0; i < baseCellList.Count; ++i)
            {
                baseCellList[i].gameObject.SetActive( i == cellStyle );
            }

            countryNameElement.gameObject.SetActive(!isSelect);
            countryNameSelectElement.gameObject.SetActive(isSelect);
        }

        private void OnSelect()
        {
            // if(isSelect)
                MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData<string>(ON_SELECT_COUNTRY, countryCode));
            // else
            //     MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData<int>(ON_CHANGE_SELECT_COUNTRY, cellIndex));
        }

        private void OnMetaUIEvent(EventData eventData)
        {
            if (ON_CHANGE_SELECT_COUNTRY == eventData.name)
            {
                UpdateSelectCell((int)eventData.value);
            }
        }
    }
}
