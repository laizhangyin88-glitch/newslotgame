using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode.ClientModels;
using NodeCanvas.Framework;

namespace BagelCode
{
    public class LobbyBannerGrorupBase : MonoBehaviour
    {
        protected bool isPreview = false;

        public bool initialized = false;
        public PageScrollRect pageController;

        public int dotCountLimit = 8;

        private List<GameObject> bannerList = new List<GameObject>();
        private List<GameObject> dotList = new List<GameObject>();

        private ContextElement contentsElement;
        private ContextElement dotElement;

        private void InitContext()
        {
            if (initialized) return;

            var rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            contentsElement = ContextUtils.FindElement(rootElement, "Contents", ContextSearchingType.ChildrenSearch);
            dotElement = ContextUtils.FindElement(rootElement, "Dots", ContextSearchingType.ChildrenSearch);

            initialized = true;
        }

        public void UpdateBannerInfo(Blackboard noticeInfo)
        {
            UpdateBannerInfo(new List<Blackboard>() { noticeInfo });
        }

        public virtual void UpdateBannerInfo(List<Blackboard> noticeList)
        {
            InitContext();

            for (int i = 0; i < noticeList.Count; ++i)
            {
                var bannerObject = MetaObjectUtils.MakeScene(MetaStringDefine.LOBBY_BUNDLE_NAME, "Banner Item Scene", contentsElement.transform, null);
                var bannerBB = bannerObject.GetComponent<Blackboard>();

                BlackboardUtils.SetOrCreateValue(bannerBB, "bannerInfo", noticeList[i]);
                BlackboardUtils.SetOrCreateValue(bannerBB, "interactable", !isPreview);

                CreateComponents(noticeList[i], bannerObject.transform);

                bannerList.Add(bannerObject);

                // dot limit
                if (i < dotCountLimit)
                {
                    var dotObject = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Banner Toggle", dotElement.transform, null, "Banner Toggle");
                    dotList.Add(dotObject);
                }
            }

            contentsElement.UpdateContext(true);
            dotElement.UpdateContext(true);

            MetaContextElementUtils.SetIntProperty(dotElement, 0);

            dotElement.gameObject.SetActive(noticeList.Count > 1);

            if (pageController != null)
                pageController.enabled = noticeList.Count > 1;
        }

        public void OnTogglePageChanged(int index)
        {
            // Wrapping code for overflow
            if (index >= dotCountLimit)
                index = dotCountLimit - 1;

            MetaContextElementUtils.SetIntProperty(dotElement, index);
        }

        private void CreateComponents(Blackboard banner, Transform bannerObjectTransform)
        {
            var components = banner.GetVariable<List<Blackboard>>("componentList").value;

            for (int i = 0; i < components.Count; i++)
            {
                CreateComponent(components[i], bannerObjectTransform);
            }
        }

        private void CreateComponent(Blackboard component, Transform bannerObjectTransform)
        {
            var type = component.GetValue<SlotListBannerComponentType>("type");
            var bundleName = MetaStringDefine.LOBBY_BUNDLE_NAME;

            switch (type)
            {
                case SlotListBannerComponentType.TEXT:
                case SlotListBannerComponentType.LEVEL_MULTIPLIER_TEXT_ON:
                case SlotListBannerComponentType.LEVEL_MULTIPLIER_TEXT_OFF:
                    {
                        bool showLevelMultiplierText = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "showLevelMultiplierText")?.value ?? false;

                        if (!(type is SlotListBannerComponentType.TEXT) &&
                            type is SlotListBannerComponentType.LEVEL_MULTIPLIER_TEXT_ON != showLevelMultiplierText) return;

                        string resultText = component.GetValue<string>("text");
                        bool showOutline = component.GetValue<bool>("showOutline");

                        var bannerText = showOutline ? MetaObjectUtils.MakePrefab(bundleName, "Slot Banner Central Outline Text", bannerObjectTransform)
                                                        : MetaObjectUtils.MakePrefab(bundleName, "Slot Banner Central Text", bannerObjectTransform);
                        ContextTextMeshProUGUI textElement = bannerText.GetComponent<ContextTextMeshProUGUI>();

                        resultText = InterpolatedTextUtils.ParseCommonText(resultText);

                        var product = BlackboardUtils.FindVariable<Blackboard>(component, "product");
                        if (product != null)
                        {
                            resultText = ParseSLBText(resultText, product.value);
                        }

                        ParsePosition(bannerText.transform, component);
                        ParseRotation(bannerText.transform, component);

                        textElement.SetText(resultText);
                    }
                    break;
                case SlotListBannerComponentType.RED_RIBBON_TAG:
                    {
                        int size = component.GetValue<int>("tagSize");

                        string asset = (size == 0) ? "IAM Tag Label Small" :
                            (size == 1) ? "IAM Tag Label Normal" : "IAM Tag Label Big";

                        var componentObj = MetaObjectUtils.MakePrefab(bundleName, asset, bannerObjectTransform);

                        ContextElement componentElement = componentObj.GetComponent<ContextElement>();
                        componentElement.UpdateContext(true);

                        string tagText = component.GetValue<string>("text");
                        MetaContextElementUtils.SimpleSetText(componentElement, "Text", tagText);
                    }
                    break;
                default:
                    {
                        Debug.LogError("UNKNOWN SLB COMPONENT");
                    }
                    break;
            }
        }

        private static string ParseSLBText(string text, Blackboard product)
        {
            text = InterpolatedTextUtils.ParseTextOfProduct(text, product);
            text = InterpolatedTextUtils.ParseEconomyMultiplier(text);

            text = InterpolatedTextUtils.ParseCustomText(text);

            return text;
        }

        private void ParsePosition(Transform componentTrasform, Blackboard componentBB)
        {
            int componentPosX = componentBB.GetValue<int>("posX");
            int componentPosY = componentBB.GetValue<int>("posY");

            RectTransform componentRectTransform = componentTrasform.GetComponent<RectTransform>();
            componentRectTransform.anchorMin = new Vector2(0, 1f);
            componentRectTransform.anchorMax = new Vector2(0, 1f);

            componentRectTransform.anchoredPosition = new Vector2(componentPosX, -componentPosY);
        }

        private void ParseRotation(Transform componentTrasform, Blackboard componentBB)
        {
            float rotation = (float)componentBB.GetValue<int>("rotation");
            componentTrasform.rotation = Quaternion.Euler(0, 0, rotation);
        }
    }
}
