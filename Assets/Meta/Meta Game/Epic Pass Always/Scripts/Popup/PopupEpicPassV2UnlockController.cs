using UnityEngine;
using ParadoxNotion;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode
{
    public class PopupEpicPassV2UnlockController : MonoBehaviour
    {
        private bool isInit = false;

        private ContextElement rootElement;
        private Blackboard rootBB;

        private ContextElement unlockElement;
        private ContextElement restartElement;

        private void Start()
        {
            InitProperty();
        }

        private void OnDestroy()
        {
            if (MetaSystem.Instance != null)
                MetaSystem.UnSubscribeBackButton(gameObject.GetHashCode());
        }

        public void InitProperty()
        {
            if (isInit) return;

            rootBB = gameObject.GetComponent<Blackboard>();

            bool isUnlock = BlackboardUtils.GetOrCreateVariable<bool>(rootBB, "isUnlock").value;

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(true);

            ContextElement titleUnlockTextElement = ContextUtils.FindElement(rootElement, "Title/Text", ContextSearchingType.FullNameSearch);
            ContextElement infoTextElement = ContextUtils.FindElement(rootElement, "Text Info", ContextSearchingType.ChildrenSearch);
            ContextElement buttonElement = ContextUtils.FindElement(rootElement, "Button Area/Button Collect", ContextSearchingType.FullNameSearch);
            ContextElement closeTextElement = ContextUtils.FindElement(buttonElement, "Text", ContextSearchingType.ChildrenSearch);

            ContextElement iconAreaElement = ContextUtils.FindElement(rootElement, "Icon Area", ContextSearchingType.ChildrenSearch);
            unlockElement = ContextUtils.FindElement(iconAreaElement, "Icon Unlock", ContextSearchingType.ChildrenSearch);
            restartElement = ContextUtils.FindElement(iconAreaElement, "Icon Restart", ContextSearchingType.ChildrenSearch);

            string descText = "";

            if (isUnlock)
            {
                // Unlock
                unlockElement.gameObject.SetActive(true);
                restartElement.gameObject.SetActive(false);

                descText = StringTableUtils.GetString(StringTable.StringTableType.Global, "EPIC_PASS_PURCHASE_RESULT_DESC_TEXT");

                GSManager.Instance.GetHandler(EpicPassUtilsV2.Sounds.EPIC_PASS_UNLOCK).Play();
            }
            else
            {
                // Restart
                unlockElement.gameObject.SetActive(false);
                restartElement.gameObject.SetActive(true);

                descText = StringTableUtils.GetString(StringTable.StringTableType.Global, "EPIC_PASS_RESET_RESULT_DESC_TEXT");
                GSManager.Instance.GetHandler(EpicPassUtilsV2.Sounds.EPIC_PASS_RESTART).Play();
            }

            MetaContextElementUtils.SetTextGlobal(titleUnlockTextElement, "EPIC_PASS_PURCHASE_RESULT_TITLE_TEXT");
            MetaContextElementUtils.SetTextGlobal(closeTextElement, "EPIC_PASS_PURCHASE_RESULT_CLOSE_BUTTON");
            MetaContextElementUtils.SetText(infoTextElement, descText);

            MetaContextElementUtils.SetClickable(buttonElement, OnClickClose);
            // Back Button Event.
            MetaSystem.SubscribeBackButton(gameObject.GetHashCode(), () => { OnClickClose(); });

            isInit = true;
        }

        private void OnClickClose()
        {
            EpicPassUtilsV2.UpdateMetaIcon();

            EpicPassUtilsV2.SendWelcomeVIPLoungePopup(null);

            EventData eventData = new EventData(MetaEventDefine.ON_CLICK_CLOSE_PURCHASE_REWARD_POPUP);
            MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, eventData);

            PopupManager.Instance.Close(gameObject);
            Destroy(gameObject);
        }
    }
}