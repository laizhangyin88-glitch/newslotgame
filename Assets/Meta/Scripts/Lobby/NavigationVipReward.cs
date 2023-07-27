using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode.ClientModels;
using ParadoxNotion;
using ParadoxNotion.Services;
using NodeCanvas.Framework;

namespace BagelCode
{
    public class NavigationVipReward : EventMonoBehaviour
    {
        private bool isInit = false;
        private int lastTier = 0;

        public void Update()
        {
            if (isInit == false)
            {
                Initialize();
                isInit = true;
            }

            var tier = BlackboardUtils.FindVariable<int>(null, "/me/tier");
            if (tier != null && tier.value != lastTier)
                UpdateVipIcon();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateVipIcon();
        }

        private void Initialize()
        {
            ContextElement rootElement = GetComponent<ContextElement>();
            rootElement.UpdateContext();

            ContextButton vipRewardButton = ContextUtils.FindElement(rootElement, "Button Tier", ContextSearchingType.ChildrenSearch).GetComponent<ContextButton>();
            vipRewardButton.AddListenerOnClick(
                context =>
                {
                    Dictionary<string, object> customData = new Dictionary<string, object>();
                    Analytics.CustomEvent("client_click_rewards_program", customData);

                    string bundleName = ApplicationSettings.MakeApplicationBundleName("lobby");
                    var sceneInfo = AssetBundleManager.LoadAsset<SceneInfoObject>(bundleName, "VIP Rewards Scene").GetSceneInfo();

                    var go = GameObject.Find("Popup Manager/Area");
                    if (go != null && sceneInfo != null)
                    {
                        var temp = SceneManager.LoadScene(go.transform, sceneInfo);
                        PopupManager.Instance.Open(temp);
                        temp.SetActive(true);
                    }
                }
            );

            UpdateVipIcon();
        }

        private void UpdateVipIcon()
        {
            ContextElement rootElement = GetComponent<ContextElement>();
            ContextElement imageTier = ContextUtils.FindElement(rootElement, "Button Tier/Image Tier", ContextSearchingType.FullNameSearch);

            if (imageTier != null)
            {
                var tier = BlackboardUtils.FindVariable<int>(null, "/me/tier");
                Blackboard bb = imageTier.GetComponent<Blackboard>();

                if (bb != null && tier != null)
                {
                    int tierGroup = TierUtils.GetTierGroup(tier.value);
                    BlackboardUtils.SetOrCreateValue(bb, "Image Tier", tierGroup);

                    Animator animator = imageTier.GetComponent<Animator>();
                    if (animator != null)
                        animator.SetInteger("Tier", tierGroup);

                    lastTier = tier.value;
                }
            }
        }
    }
}
