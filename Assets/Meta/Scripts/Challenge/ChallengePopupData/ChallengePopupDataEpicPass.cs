using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BagelCode.OSA_Scroll;
using NodeCanvas.Framework;
using SlotMaker;
using BagelCode.ClientModels;
using BagelCode.EpicPass;

namespace BagelCode
{
    public class ChallengePopupDataEpicPass : ChallengePopupData
    {
        private ContextElement selectBaseElement;
        private ContextElement tabIconWebImageElement;

        private ChallengeEpicPassController epicPassController = null;
        // Epic Pass tab control
        public ChallengePopupDataEpicPass(GameObject _owner)
            : base(_owner) { }

        public override void InitProperty()
        {
            isMainTab = true;
            selectBaseElement = ContextUtils.FindElement(root, "Left Tab Base/Tab Epic Pass/Base Select", FULL);
            tabIconWebImageElement = ContextUtils.FindElement(root, "Left Tab Base/Tab Epic Pass/Epic Pass Always Icon Web Image/Image", FULL);

            UpdateTabWebImage();

            ContextElement epicpassElement = ContextUtils.FindElement(root, "Epic Pass", CHILDREN);
            Blackboard epicpassBB = epicpassElement.GetComponent<Blackboard>();

            string contextId = rootBB.GetVariable<string>("_biContextID")?.value ?? "";
            if (string.IsNullOrEmpty(contextId))
                contextId = BiEventUtils.GenerateContextID();
            BlackboardUtils.SetOrCreateValue(epicpassBB, "_biContextID", contextId);

            epicPassController = epicpassElement.GetComponent<ChallengeEpicPassController>();
            epicPassController.OnInit(rootAnim);
            epicPassController.SetBadgeUpdateEventCallback(UpdateBadge);
        }

        public override bool IsComplete(MetaChallengeType tabType)
        {
            return false;
        }

        public override void OnUpdateChallengeInfo()
        {
            UpdateChallengeInfo();
            UpdateBadge();
        }

        public override void UpdateChallengeInfo()
        {
            isUpdated = true;
        }

        public override void OnSelectTab(MetaChallengeType tabType)
        {
            if (tabType != MetaChallengeType.EPIC_PASS)
            {
                selectBaseElement?.gameObject.SetActive(false);
                return;
            }
            rootAnim.SetBool("EpicPass Loading", true);
            ChangeTab(tabType);
            selectBaseElement?.gameObject.SetActive(true);
            UpdatePopupData();
        }

        public override void UpdatePopupData()
        {
            BI_ClientChallengeEnter(null, "season_pass", isAuto ? "meta_game" : "manual");
            epicPassController.UpdateEpicPassTab();
        }

        public override void UpdateBadge()
        {
            bool isUnclaimedReward = EpicPassUtilsV2.UnclaimedRewardCount > 0;
            badgeAnimatorDict[MetaChallengeType.EPIC_PASS]?.SetInteger("value", isUnclaimedReward ? 1 : 0);
            isBadge = isUnclaimedReward;
        }

        private void UpdateTabWebImage()
        {
            string tabIconWebImageUrl = EpicPassUtilsV2.TabIconImageUrl;
            if (!string.IsNullOrEmpty(tabIconWebImageUrl))
            {
                // todo : web download change
                if (!MetaContextElementUtils.SetWebImage(tabIconWebImageElement, tabIconWebImageUrl))
                {
                    IContextImage imageElement = tabIconWebImageElement as IContextImage;
                    imageElement.SetHash(tabIconWebImageUrl.GetHashCode().ToString());

                    WebImageDownloader.Instance.LoadWebImage(
                        tabIconWebImageUrl,
                        CacheType.FileCache,
                        true,
                        null,
                        delegate (Sprite img)
                        {
                            if (img != null)
                            {
                                if (imageElement.CheckHash(tabIconWebImageUrl.GetHashCode().ToString()))
                                {
                                    imageElement.SetSprite(img);
                                }
                            }
                        },
                        null,
                        null
                    );
                }
            }
        }
    }
}
