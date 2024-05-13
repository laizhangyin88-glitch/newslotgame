using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine.UI;
using System;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class PopupDevButtonScrollerIamPreview : PopupDevButtonScroller
    {
        private Blackboard bb;

        private const float RELOAD_INTERVAL_SEC = 1f;
        private float remainingInterval = 0f;

        private List<SimulatorComponentInfo> iamSlbInfoList = new List<SimulatorComponentInfo>();

        private SimulatorComponentInfo currentIamSlbInfo = null;
        private GameObject popupObj = null;
        private GameObject currentBannerObj = null;

        private Coroutine popupUpdateCoroutine = null;

        private long lastChangedTimestamp = 0;

        protected override void Start()
        {
            StartCoroutine(InitCoroutine());
        }

        private IEnumerator InitCoroutine()
        {
            bb = GetComponent<Blackboard>();

            yield return StartCoroutine(RequestIamSlbListCoroutine());

            base.Start();

            var inputFieldElement = ContextUtils.FindElement(root, "Dev Input", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SimpleSetText(inputFieldElement, "Placeholder", "Enter Search Keyword");

            var inputField = inputFieldElement.GetComponent<InputField>();
            inputField.onValueChanged.AddListener(OnInputChanged);

            Register(EventSender.ON_CALLEE_CALLBACK, OnClosePopup);
        }

        private IEnumerator PopupUpdateCoroutine() // Reload
        {
            if (popupObj == null) yield break;

            // Initialize
            var iamController = popupObj.GetComponent<InAppMessage.InAppMessagePreviewController>();
            var slbController = popupObj.GetComponent<PopupPreviewBannerAreaController>();
            bool isIam = slbController == null;

            // Update
            while (popupObj != null)
            {
                if (remainingInterval <= 0f)
                {
                    remainingInterval = RELOAD_INTERVAL_SEC;

                    long changedTimestamp = lastChangedTimestamp;

                    yield return StartCoroutine(ReuqestIamSlbInfoCoroutine());

                    if (lastChangedTimestamp > changedTimestamp)
                    {
                        if (isIam)
                        {
                            var iamInfo = BlackboardUtils.FindVariable<Blackboard>(bb, "iamInfo")?.value;
                            iamController.ReloadIam(iamInfo);
                        }
                        else
                        {
                            Destroy(currentBannerObj);
                            var slbInfo = BlackboardUtils.FindVariable<Blackboard>(bb, "slbInfo")?.value;
                            currentBannerObj = MakeBannerObject(popupObj, slbInfo);
                        }
                    }
                }

                remainingInterval -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            yield break;
        }

        private void OnClosePopup()
        {
            popupObj = null;
        }

        private void OnInputChanged(string text)
        {
            UpdateLists(text);
        }

        private IEnumerator RequestIamSlbListCoroutine()
        {
            var loadingObj = MetaPopupUtils.OpenLoadingPopup();

            bool isSuccess = false;
            bool isFail = false;
            BagelCodeClientAPI.RequestPreviewIamSlbList(
                (response) =>
                {
                    isSuccess = true;
                    iamSlbInfoList = response.componentList;
                },
                (error) =>
                {
                    isFail = true;
                    Debug.LogError("PopupDevButtonScrollerIamPreview.RequestIamSlbListCoroutine failure.");
                });

            yield return new WaitUntil(() => isSuccess || isFail);

            MetaPopupUtils.ClosePopup(loadingObj);
        }

        private void UpdateLists(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                cellObjList.ForEach(o => o.SetActive(true));
            }

            int cellCount = CellCount();
            for (int i = 0; i < cellCount; ++i)
            {
                string cellText = cellTextList[i];

                bool isContainsSearchText = cellText.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
                cellObjList[i].SetActive(isContainsSearchText);
            }
        }

        private bool IsIam(int i)
        {
            if (iamSlbInfoList.IsValidIndex(i))
                return iamSlbInfoList[i].type == "IN_APP_MESSAGE";
            return true;
        }

        private bool IsIam(SimulatorComponentInfo info)
        {
            return info.type == "IN_APP_MESSAGE";
        }

        protected override int CellCount()
        {
            return iamSlbInfoList.Count;
        }

        protected override string TitleText() => "Select IAM/SLB";

        protected override string CellText(int i) => iamSlbInfoList[i].name;

        protected override void OnClick(int i)
        {
            StartCoroutine(OpenIamSlbPopupCoroutine(i));
        }

        private IEnumerator OpenIamSlbPopupCoroutine(int i)
        {
            currentIamSlbInfo = iamSlbInfoList[i];

            yield return StartCoroutine(ReuqestIamSlbInfoCoroutine());

            if (IsIam(i))
            {
                var iamInfo = BlackboardUtils.FindVariable<Blackboard>(bb, "iamInfo")?.value;
                if (iamInfo == null) yield break;

                string bundle = MetaStringDefine.TEST_SUITE;
                string asset = "IAM Preview Base";
                Transform parent = MetaPopupUtils.PopupManagerAreaTransform;
                popupObj = MetaObjectUtils.MakePrefab(bundle, asset, parent);
                MetaPopupUtils.OpenPopup(popupObj);

                var iamBB = popupObj.GetComponent<Blackboard>();
                BlackboardUtils.SetOrCreateValue(iamBB, "iamCaller", gameObject);
                BlackboardUtils.SetOrCreateValue(iamBB, "_iamInfo", iamInfo);

                var controller = popupObj.GetComponent<InAppMessage.InAppMessageBase>();
                controller.LoadIAM(MetaStringDefine.LOBBY_BUNDLE_NAME, iamInfo, 0L);
            }
            else
            {
                var slbInfo = BlackboardUtils.FindVariable<Blackboard>(bb, "slbInfo")?.value;
                if(slbInfo == null) yield break;

                string bundle = MetaStringDefine.TEST_SUITE;
                string asset = "Popup Preview Banner Area Scene";
                Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

                yield return StartCoroutine(MetaObjectUtils.MakeSceneCoroutine(bundle, asset, parent,
                    (GameObject obj) => popupObj = obj));

                MetaObjectUtils.SetCalleeCaller(popupObj, gameObject);
                MetaPopupUtils.OpenPopup(popupObj);

                currentBannerObj = MakeBannerObject(popupObj, slbInfo);
            }

            if (popupUpdateCoroutine != null)
                StopCoroutine(popupUpdateCoroutine);
            popupUpdateCoroutine = StartCoroutine(PopupUpdateCoroutine());
        }

        private IEnumerator ReuqestIamSlbInfoCoroutine()
        {
            // Request
            bool isSuccess = false;
            bool isFail = false;
            bool isIam = IsIam(currentIamSlbInfo);
            int id = currentIamSlbInfo.id;
            BagelCodeClientAPI.RequestPreviewIamSlbInfo(id,
                (response) =>
                {
                    isSuccess = true;

                    lastChangedTimestamp = response.revision;

                    if (isIam)
                    {
                        var iamInfoBB = BlackboardUtils.GetOrCreateBlackboard(bb, "iamInfo");
                        BlackboardUtils.ClearBlackboard(iamInfoBB);
                        ClientAPI2Blackboard.Serialize(iamInfoBB, response.iamComponent);
                    }
                    else
                    {
                        var slbInfoBB = BlackboardUtils.GetOrCreateBlackboard(bb, "slbInfo");
                        BlackboardUtils.ClearBlackboard(slbInfoBB);
                        ClientAPI2Blackboard.Serialize(slbInfoBB, response.slbComponent);
                    }
                },
                (error) =>
                {
                    isFail = true;
                    Debug.LogError("PopupDevButtonScrollerIamPreview.RequestIamSlbListCoroutine failure.");
                });

            yield return new WaitUntil(() => isSuccess || isFail);
        }

        private GameObject MakeBannerObject(GameObject ownerPopupObj, Blackboard bannerInfo)
        {
            string bundle = MetaStringDefine.TEST_SUITE;
            string asset = "Slot Banner Preview Scene";
            Transform parent = ownerPopupObj.transform;
            var bannerObj = MetaObjectUtils.MakeScene(bundle, asset, parent);

            var controller = bannerObj.GetComponent<LobbyBannerGrorupBase>();
            controller.UpdateBannerInfo(bannerInfo);

            

            return bannerObj;
        }
    }
}
