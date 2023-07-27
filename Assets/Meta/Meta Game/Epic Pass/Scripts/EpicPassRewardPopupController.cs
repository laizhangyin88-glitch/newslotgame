using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ParadoxNotion;
using NodeCanvas.Framework;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class EpicPassRewardPopupController : MonoBehaviour
    {
        private bool isInit = false;

        private ContextElement rootElement;
        private ContextElement titleUnlockTextElement;
        private ContextElement titleLevelUpTextElement;
        private ContextElement infoTextElement;
        private ContextElement closeTextElement;
        private ContextElement iconAreaElement;
        private ContextElement webImageElement;
        private ContextElement buttonElement;
        private ContextElement iconUnlock;
        private ContextElement iconRestart;

        private const string EPIC_PASS_PURCHASE_RESULT_TITLE_TEXT = "EPIC_PASS_PURCHASE_RESULT_TITLE_TEXT";
        private const string EPIC_PASS_PURCHASE_RESULT_DESC_TEXT = "EPIC_PASS_PURCHASE_RESULT_DESC_TEXT";
        private const string EPIC_PASS_PURCHASE_RESULT_CLOSE_BUTTON = "EPIC_PASS_PURCHASE_RESULT_CLOSE_BUTTON";
        private const string EPIC_PASS_LEVEL_UP_TITLE_TEXT = "EPIC_PASS_LEVEL_UP_TITLE_TEXT";
        private const string EPIC_PASS_LEVEL_UP_CLOSE_BUTTON = "EPIC_PASS_LEVEL_UP_CLOSE_BUTTON";
        private const string EPIC_PASS_RESET_RESULT_DESC_TEXT = "EPIC_PASS_RESET_RESULT_DESC_TEXT";

        private const string ICON_IMAGE_URL_KEY = "iconSmallImageUrl";

        //

        private void Start()
        {
            InitProperty();
        }

        private void OnDestroy()
        {
            if(MetaSystem.Instance != null)
                MetaSystem.UnSubscribeBackButton(gameObject.GetHashCode());
        }

        public void InitProperty()
        {
            if (isInit) return;

            var currentBB = gameObject.GetComponent<Blackboard>();
            var metaEnterInfoBB = EpicPassUtils.EpicPassInfo;

            bool isUnlock = BlackboardUtils.GetOrCreateVariable<bool>(currentBB, "isUnlock").value;
            bool isLevelUP = !isUnlock;

            string prefabBundleName = BlackboardQueryUtils.GetMetaBundleName(EventInfoType.SEASON_PASS);
            string imagePrefabName = (isUnlock || EpicPassUtils.IsReset) ? "Epic Pass Icon Epic Pass Big" : "Epic Pass Reward Web Image";
            string iconAreaName = (isUnlock || EpicPassUtils.IsReset) ? "Icon Reward Area" : "Icon Reward Area/Reward Web Image Area";
            string iconImageUrl = metaEnterInfoBB == null ? "" : BlackboardUtils.GetOrCreateVariable<string>(metaEnterInfoBB, ICON_IMAGE_URL_KEY).value;

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(true);

            titleUnlockTextElement = ContextUtils.FindElement(rootElement, "Title/Text", ContextSearchingType.FullNameSearch);
            titleLevelUpTextElement = ContextUtils.FindElement(rootElement, "Title/Text Level Up", ContextSearchingType.FullNameSearch);
            infoTextElement = ContextUtils.FindElement(rootElement, "Text Info", ContextSearchingType.ChildrenSearch);
            iconAreaElement = ContextUtils.FindElement(rootElement, iconAreaName, ContextSearchingType.FullNameSearch);
            buttonElement = ContextUtils.FindElement(rootElement, "Button Area/Button Purchase", ContextSearchingType.FullNameSearch);
            closeTextElement = ContextUtils.FindElement(buttonElement, "Text", ContextSearchingType.ChildrenSearch);

            GameObject imageGO = MetaObjectUtils.MakePrefab(prefabBundleName, imagePrefabName, iconAreaElement.transform);

            if (EpicPassUtils.IsReset)
            {
                MetaContextElementUtils.SetActive(titleUnlockTextElement, true);
                MetaContextElementUtils.SetActive(titleLevelUpTextElement, false);
                MetaContextElementUtils.SetActive(infoTextElement, true);

                MetaContextElementUtils.SetTextGlobal(titleUnlockTextElement, EPIC_PASS_PURCHASE_RESULT_TITLE_TEXT);
                MetaContextElementUtils.SetTextGlobal(infoTextElement, EPIC_PASS_RESET_RESULT_DESC_TEXT);
                MetaContextElementUtils.SetTextGlobal(closeTextElement, EPIC_PASS_PURCHASE_RESULT_CLOSE_BUTTON);
            }
            else
            {
                MetaContextElementUtils.SetActive(titleUnlockTextElement, isUnlock);
                MetaContextElementUtils.SetActive(titleLevelUpTextElement, isLevelUP);
                MetaContextElementUtils.SetActive(infoTextElement, isUnlock);

                if (isUnlock)
                {
                    MetaContextElementUtils.SetTextGlobal(titleUnlockTextElement, EPIC_PASS_PURCHASE_RESULT_TITLE_TEXT);
                    MetaContextElementUtils.SetTextGlobal(infoTextElement, EPIC_PASS_PURCHASE_RESULT_DESC_TEXT);
                    MetaContextElementUtils.SetTextGlobal(closeTextElement, EPIC_PASS_PURCHASE_RESULT_CLOSE_BUTTON);

                    GSManager.Instance.GetHandler(EpicPassUtils.Sounds.EPIC_PASS_UNLOCK).Play();
                }

                if (isLevelUP)
                {
                    iconAreaElement.UpdateContext(true);
                    webImageElement = ContextUtils.FindElement(iconAreaElement, "Image", ContextSearchingType.ChildrenSearch);
                    MetaContextElementUtils.SetWebImage(webImageElement, iconImageUrl);

                    MetaContextElementUtils.SetTextGlobal(titleLevelUpTextElement, EPIC_PASS_LEVEL_UP_TITLE_TEXT, EpicPassUtils.Level);
                    MetaContextElementUtils.SetTextGlobal(closeTextElement, EPIC_PASS_LEVEL_UP_CLOSE_BUTTON);

                    GSManager.Instance.GetHandler(EpicPassUtils.Sounds.EPIC_PASS_LEVEL_UP_POPUP).Play();
                }
            }

            MetaContextElementUtils.SetClickable(
                buttonElement,
                OnClickClose
            );

            // Back Button Event.
            MetaSystem.SubscribeBackButton(gameObject.GetHashCode(), () => { OnClickClose(); });

            isInit = true;
        }

        //

        private void OnClickClose()
        {
            EventData eventData = new EventData(MetaEventDefine.ON_CLICK_CLOSE_PURCHASE_REWARD_POPUP);
            MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, eventData);

            PopupManager.Instance.Close(gameObject);
            Destroy(gameObject);
        }
    }
}
