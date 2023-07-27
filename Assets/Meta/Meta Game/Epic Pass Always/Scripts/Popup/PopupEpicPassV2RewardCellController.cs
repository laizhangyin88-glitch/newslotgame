using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ParadoxNotion;
using NodeCanvas.Framework;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.EpicPass
{
    public class PopupEpicPassV2RewardCellController : MonoBehaviour
    {
        private ContextElement rootElement;

        private ContextElement rewardItemListElement;
        private ContextElement itemElement;
        private ContextElement lockedElement = null;

        private GameObject webImageObject = null;

        private EpicPassV2RewardCelltemController itemController = null;

        private bool isInit = false;

        public Blackboard rewardInfoBB;
        public bool isPaid = false; // reward free/paid type
        public bool isLocked = false;

        private const string WEB_ICON_IMAGE = "Epic Pass Always Icon Popup Web Image";

        public void OnInit(Blackboard rewardBB, bool _isLocked = false)
        {
            rewardInfoBB = rewardBB;
            isLocked = _isLocked;

            InitProperty();
            UpdateVariables();
        }

        private void InitProperty()
        {
            if (isInit) return;

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(true);

            ContextElement rewardsElement = ContextUtils.FindElement(rootElement, "Epic Pass Rewards", ContextSearchingType.ChildrenSearch);

            lockedElement = ContextUtils.FindElement(rewardsElement, "Locked", ContextSearchingType.ChildrenSearch);

            rewardItemListElement = ContextUtils.FindElement(rewardsElement, "Reward Item List", ContextSearchingType.ChildrenSearch);
            itemElement = ContextUtils.FindElement(rewardItemListElement, "Cell Item 01", ContextSearchingType.ChildrenSearch);
            itemController = itemElement.GetComponent<EpicPassV2RewardCelltemController>();

            isInit = true;
        }

        private void UpdateVariables()
        {
            if (itemController == null)
                return;

            itemController.isPopupReward = true;
            itemController.SetValues(rewardInfoBB);
            itemController.UpdateVariables();

            if (isPaid)
            {
                lockedElement.gameObject.SetActive(!EpicPassUtilsV2.Paid);
            }
            else
            {
                lockedElement.gameObject.SetActive(false);

                if (rewardInfoBB == null) // level up
                {
                    itemElement.gameObject.SetActive(false);
                    CreateWebImage();
                }
                else
                {
                    itemElement.gameObject.SetActive(true);
                    webImageObject?.SetActive(false);
                }
            }
        }

        private void CreateWebImage()
        {
            if (webImageObject == null)
                webImageObject = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, WEB_ICON_IMAGE, rewardItemListElement.transform);

            ContextElement objElement = webImageObject.GetComponent<ContextElement>();
            objElement.UpdateContext();
            ContextElement iconImageElement = ContextUtils.FindElement(objElement, "Image", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetWebImage(iconImageElement, EpicPassUtilsV2.TabIconImageUrl);

            webImageObject.SetActive(true);
        }
    }
}
