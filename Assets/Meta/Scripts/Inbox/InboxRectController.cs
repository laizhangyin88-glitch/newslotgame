using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;
using static BagelCode.InboxEvent;

namespace BagelCode
{
    public class InboxRectController : MonoBehaviour
    {
        public InboxController ownerInboxController;

        private OSA_Scroll.OSA_InboxItems osaItems;

        private ContextElement root;
        private Blackboard rootBB;

        private bool isInit = false;

        // Context
        private ContextElement viewportElement;
        private ContextElement collectButtonElement;
        private ContextElement closeButtonElement;

        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;
        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;

        //

        public void OnStartAccept()
        {
            if (ApplicationSettings.LogTest())
                Debug.Log("Start Accept");

            // Block back button
            MetaSystem.SubscribeBackButton(gameObject.GetHashCode(), () => { });

            SetRectInteraction(false);
            MetaSystem.BackupUserSyncInfo();
            InboxController.IsAcceptable = false;
        }

        public void OnFinishAccept()
        {
            if (ApplicationSettings.LogTest())
                Debug.Log("Finish Accept");

            MetaSystem.UnSubscribeBackButton(gameObject.GetHashCode());

            SetRectInteraction(true);
            EventSender.SendGlobalEvent(UPDATE_NAVI_CREDIT);
            EventSender.SendEvent(ownerInboxController.gameObject, REFRESH_INBOX_ITEM);
            InboxController.IsAcceptable = true;
        }

        public void OnFinishBannerAccept()
        {
            SetRectInteraction(true);
            EventSender.SendEvent(ownerInboxController.gameObject, REFRESH_INBOX_ITEM);
            InboxController.IsAcceptable = true;
        }

        private void Start()
        {
            InitProperty();

            SetRectInteraction(true);
        }

        //

        private void InitProperty()
        {
            if (isInit) return;

            root = GetComponent<ContextElement>();
            rootBB = GetComponent<Blackboard>();
            osaItems = GetComponent<OSA_Scroll.OSA_InboxItems>();

            GameObject caller = GameObject.FindObjectOfType<InboxController>().gameObject;

            ContextElement callerRoot = caller.GetComponent<ContextElement>();
            ownerInboxController = caller.GetComponent<InboxController>();

            rootBB.AddVariable("caller", caller);

            root.UpdateContext(false);

            viewportElement = ContextUtils.FindElement(root, "Viewport", CHILDREN);
            collectButtonElement = ContextUtils.FindElement(callerRoot, "Button Area/Button Collect All", FULL);
            closeButtonElement = ContextUtils.FindElement(callerRoot, "Button Close", CHILDREN);

            isInit = true;
        }

        private void SetRectInteraction(bool doActive)
        {
            osaItems.enabled = doActive;

            var viewportCanvas = viewportElement.GetComponent<UnityEngine.CanvasGroup>();
            viewportCanvas.interactable = doActive;

            var collectButtonSelectable = collectButtonElement.GetComponent<UnityEngine.UI.Selectable>();
            collectButtonSelectable.interactable = doActive;

            var closeButtonSelectable = closeButtonElement.GetComponent<UnityEngine.UI.Selectable>();
            closeButtonSelectable.interactable = doActive;
        }
    }
}
