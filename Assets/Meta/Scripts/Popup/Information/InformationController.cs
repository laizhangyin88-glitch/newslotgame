using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Services;

namespace BagelCode
{
    public class InformationController : MonoBehaviour
    {
        private ContextElement rootElement;
        private Animator rootAnimator;
        private Blackboard rootBB;

        private ContextElement dotsAreaElement;
        private ContextElement leftButtonElement;
        private ContextElement rightButtonElement;
        private ContextElement closeButtonElement;
        private ContextElement imageAreaElement;
        private ContextElement infoTextElement;

        private GameObject prevPageObj;
        private PIDButton leftPIDButton;
        private PIDButton rightPIDButton;

        private List<string> infoTextList;
        private List<object[]> infoDataList;

        private int idx;
        private int pageCount;

        private string bundleName;
        private string infoPrefabFormat;

        private bool isInit = false;

        //private const string ON_SELECT_RIGHT_INFO_EVENT = "OnSelectRightInfo";
        //private const string ON_SELECT_LEFT_INFO_EVENT = "OnSelectLeftInfo";
        //private const string ON_CLOSE_EVENT = "OnClose";

        private void Start()
        {
            if (isInit) return;
            OnInit();
            isInit = true;
        }

        private void OnDestroy()
        {
            MetaSystem.UnSubscribeBackButton(this.GetHashCode());
        }

        public void OnInit()
        {
            InitProperty();
            InitData();

            MakeDots();
            MakeInfoText();
            InitEvent();

            UpdateInformation();
        }

        private void InitProperty()
        {
            rootElement = gameObject.GetComponent<ContextElement>();
            rootAnimator = gameObject.GetComponent<Animator>();
            rootBB = gameObject.GetComponent<Blackboard>();
            rootElement.UpdateContext(false);

            dotsAreaElement = ContextUtils.FindElement(rootElement, "Dots Area", ContextSearchingType.ChildrenSearch);
            leftButtonElement = ContextUtils.FindElement(rootElement, "Arrow Left", ContextSearchingType.ChildrenSearch);
            rightButtonElement = ContextUtils.FindElement(rootElement, "Arrow Right", ContextSearchingType.ChildrenSearch);
            closeButtonElement = ContextUtils.FindElement(rootElement, "Close", ContextSearchingType.ChildrenSearch);

            imageAreaElement = ContextUtils.FindElement(rootElement, "Image Area", ContextSearchingType.ChildrenSearch);
            infoTextElement = ContextUtils.FindElement(rootElement, "Text", ContextSearchingType.ChildrenSearch);

            leftPIDButton = leftButtonElement.GetComponent<PIDButton>();
            rightPIDButton = rightButtonElement.GetComponent<PIDButton>();
        }

        private void InitData()
        {
            idx = 0;
            pageCount = BlackboardUtils.FindValue<int>(rootBB, "pageCount");
            infoTextList = new List<string>();

            bundleName = BlackboardUtils.FindValue<string>(rootBB, "bundle");
            infoPrefabFormat = BlackboardUtils.FindValue<string>(rootBB, "infoPrefabFormat");

            infoDataList = BlackboardUtils.GetOrCreateVariable<List<object[]>>(rootBB, "datasList").value ?? new List<object[]>();
        }

        private void InitEvent()
        {
            MetaSystem.SubscribeBackButton(this.GetHashCode(), OnClose);

            MetaContextElementUtils.SetClickable(rightButtonElement, OnSelectRightInfo);
            MetaContextElementUtils.SetClickable(leftButtonElement, OnSelectLeftInfo);
            MetaContextElementUtils.SetClickable(closeButtonElement, OnClose);

            leftButtonElement.gameObject.SetActive(pageCount > 1);
            rightButtonElement.gameObject.SetActive(pageCount > 1);

            rootAnimator.SetBool("Active", true);
        }

        private void MakeDots()
        {
            if (pageCount > 1)
            {
                string dotPrefabFormat = BlackboardUtils.FindValue<string>(rootBB, "dotPrefabFormat");
                for (int i = 0; i < pageCount; ++i)
                    MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, dotPrefabFormat, dotsAreaElement.transform, "", string.Format("Dot {0}", i));

                dotsAreaElement.UpdateContext(true);
                MetaContextElementUtils.SetIntProperty(dotsAreaElement, idx);
            }
            else
                dotsAreaElement.gameObject.SetActive(false);
        }

        private void MakeInfoText()
        {
            Variable<List<object[]>> argsList = BlackboardUtils.GetOrCreateVariable<List<object[]>>(rootBB, "argsList");
#if UNITY_EDITOR
            if (argsList == null)
                Debug.LogError("Use action to SetInformationPopupData !!!!!!!!!!!!!!!!!!");
#endif
            StringTable.StringTableType tableType = StringTable.StringTableType.Global;
            string infoTextFormat = BlackboardUtils.FindValue<string>(rootBB, "infoTextFormat");
            for (int i = 0; i < pageCount; ++i)
            {
                if (argsList != null && argsList.value != null && argsList.value.Count > i && argsList.value[i] != null)
                    infoTextList.Add(StringTableUtils.GetString(tableType, string.Format(infoTextFormat, i), argsList.value[i]));
                else
                    infoTextList.Add(StringTableUtils.GetString(tableType, string.Format(infoTextFormat, i)));
            }
        }

        private void UpdateInformation()
        {
            if (prevPageObj != null)
                Destroy(prevPageObj);

            leftPIDButton.interactable = idx > 0;
            rightPIDButton.interactable = idx < pageCount - 1;

            prevPageObj = MetaObjectUtils.MakePrefab(bundleName, string.Format(infoPrefabFormat, idx), imageAreaElement.transform, "", string.Format("Page {0:00}", idx));

            InitPrefabData();
            MetaContextElementUtils.SetText(infoTextElement, infoTextList[idx]);
            if (pageCount > 1)
                MetaContextElementUtils.SetIntProperty(dotsAreaElement, idx);
        }

        private void InitPrefabData()
        {
            if (prevPageObj == null)
                return;

            if (infoDataList != null && infoDataList.Count > idx)
            {
                InformationDataBase dataBase = prevPageObj.GetComponent<InformationDataBase>();
                if (dataBase != null)
                    dataBase.SetInformationData(infoDataList[idx]);
            }

            IInformationPageSetter iPageSetter = prevPageObj.GetComponent<IInformationPageSetter>();
            if (iPageSetter != null)
                iPageSetter.SetInformationPage();
        }

        private void OnClose()
        {
            Variable<GameObject> caller = BlackboardUtils.FindVariable<GameObject>(rootBB, "caller");
            if (caller != null && caller.value != null)
            {
                MessageRouter router = Common.GetOrAddMessageRouter(caller.value);
                router.Dispatch(MessageRouter.ON_CUSTOM_EVENT, new EventData("OnCalleeCallback"), caller.value);
            }

            PopupManager.Instance.Close(gameObject);
            rootAnimator.SetTrigger("Close");
        }

        private void OnSelectRightInfo()
        {
            ++idx;
            if (idx > pageCount - 1)
                idx = pageCount - 1;
            rootAnimator.SetTrigger("PageChangeRight");

            UpdateInformation();
        }

        private void OnSelectLeftInfo()
        {
            --idx;
            if (idx < 0)
                idx = 0;
            rootAnimator.SetTrigger("PageChangeLeft");

            UpdateInformation();
        }
    }
}