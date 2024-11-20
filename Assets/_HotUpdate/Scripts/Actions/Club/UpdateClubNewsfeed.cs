using System;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
 {
     
     [Category("★ BagelCode/Club")]
     public class UpdateClubNewsfeed : ActionTask<ContextElement>
     {
        public BBParameter<string> mgBundleName;
        public BBParameter<bool> mgEnableShare;
        public BBParameter<EventInfoType> mgEventInfoType;
        public BBParameter<int>  mgEventID;

        public BBParameter<ContextElement> saveAsCollectAllButtonElement;
        public BBParameter<ContextElement> saveAsNewsfeedTabElement;
        public BBParameter<bool> saveAsEnableRequest;

        public BBParameter<GameObject> prevRequestButtonIconObj;

        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

        private ContextElement buttonLayoutAreaElement;
        private ContextElement requestButtonElement;
        private ContextElement requestButtonIconAreaElement;
        private ContextElement requestTabButtonElement;

        private bool isInit = false;
        private GameObject dataObj = null;
         
        protected override string info
        {
            get { return "Update Club Newsfeed"; }
        }

        protected override void OnExecute()
        {
            if(prevRequestButtonIconObj.value != null)
                GameObject.Destroy(prevRequestButtonIconObj.value);

            InitContext();
            UpdateVariables();

            EndAction();
        }

        private void InitContext()
        {
            if(isInit) return;

            var newsFeedElement = ContextUtils.FindElement(agent, "News Feed", ContextSearchingType.ChildrenSearch);

            saveAsCollectAllButtonElement.value = ContextUtils.FindElement(newsFeedElement, "Button Area/Button Collect All", ContextSearchingType.FullNameSearch);
            saveAsNewsfeedTabElement.value = ContextUtils.FindElement(newsFeedElement, "Filter", ContextSearchingType.ChildrenSearch);
            requestTabButtonElement = ContextUtils.FindElement(saveAsNewsfeedTabElement.value, "Tab Requests", ContextSearchingType.ChildrenSearch);
            buttonLayoutAreaElement = ContextUtils.FindElement(agent, "Post Input/Button Layout", ContextSearchingType.FullNameSearch);
            requestButtonElement = ContextUtils.FindElement(agent, "Post Input/Button Layout/Button Request", ContextSearchingType.FullNameSearch);
            requestButtonIconAreaElement = ContextUtils.FindElement(requestButtonElement, "Meta Game Item Area", ContextSearchingType.ChildrenSearch);

            MetaContextElementUtils.SetClickable(
                saveAsCollectAllButtonElement.value,
                "OnClubFeedCollectAll",
                false,
                false,
                SendEvent,
                ownerSystem
            );

            MetaContextElementUtils.SetClickable(
                saveAsNewsfeedTabElement.value,
                "OnChangeNewsfeedTab",
                false,
                false,
                SendEvent,
                ownerSystem
            );

            MetaContextElementUtils.SetClickable(
                requestButtonElement,
                "OnRequestClubShare",
                false,
                false,
                SendEvent,
                ownerSystem
            );

            var collectAllButtonTopText01Element = ContextUtils.FindElement(saveAsCollectAllButtonElement.value, "Text 01", ContextSearchingType.FullNameSearch);
            MetaContextElementUtils.SetText(collectAllButtonTopText01Element, StringTableUtils.GetString(tableType, "BUTTON_COLLECT_ALL_COIN"));
            var collectAllButtonTopText02Element = ContextUtils.FindElement(saveAsCollectAllButtonElement.value, "Text 02", ContextSearchingType.FullNameSearch);
            MetaContextElementUtils.SetText(collectAllButtonTopText02Element, "0");

            var requestButtonActiveTextElement = ContextUtils.FindElement(requestButtonElement, "Active/Text", ContextSearchingType.ChildrenSearch);
            var requestButtonInactiveTextElement = ContextUtils.FindElement(requestButtonElement, "Inactive/Text", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetText(requestButtonActiveTextElement, StringTableUtils.GetString(tableType, "BUTTON_REQUEST_SHARE_ITEM"));
            MetaContextElementUtils.SetText(requestButtonInactiveTextElement, StringTableUtils.GetString(tableType, "BUTTON_REQUEST_SHARE_ITEM"));

            isInit = true;
        }

        private void UpdateVariables()
        {
            saveAsEnableRequest.value = mgEnableShare.value;

            if(mgEnableShare.value)
            {
#if USE_ASSETBUNDLE
                var bundle = AssetBundleManager.GetLoadedAssetBundle(mgBundleName.value);
                saveAsEnableRequest.value = bundle != null;
#endif
                if(saveAsEnableRequest.value)
                {
                    switch(mgEventInfoType.value)
                    {
                        case EventInfoType.COLLECTING_GAME:
                            if(dataObj == null)
                                dataObj = MetaObjectUtils.MakePrefab(mgBundleName.value, "Data", agent.transform, null, null);
                            break;
#if UNITY_EDITOR
                        default:
                            Debug.LogError("Make Data Prefab!!!!!!!!");
                            break;
#endif
                    }

                    if(dataObj == null)
                    {
                        mgEventID.value = 0;
                        saveAsEnableRequest.value = false;
                    }
                }
                else
                {
                    mgEventID.value = 0;
                    saveAsEnableRequest.value = false;
                }
            }

            buttonLayoutAreaElement.gameObject.SetActive(saveAsEnableRequest.value);
            requestTabButtonElement.gameObject.SetActive(saveAsEnableRequest.value);

            if(saveAsEnableRequest.value)
            {
                prevRequestButtonIconObj.value = MetaObjectUtils.MakePrefab(mgBundleName.value, "Share Icon", requestButtonIconAreaElement.transform, null, null);
                MetaContextElementUtils.SetBooleanProperty(requestButtonElement, false);
                MetaContextElementUtils.SimpleSetActive(requestButtonElement, "Loading Area", true);
            }
        }
     }
 }