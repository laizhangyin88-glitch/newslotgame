using UnityEngine;
using SlotMaker;
using ParadoxNotion;
using BagelCode.ClientModels;
using NodeCanvas.Framework;

namespace BagelCode
{
    public class ProfileBlockPopupController : MonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;

        private ContextElement selectDeviceElement;
        private ContextElement selectAccountElement;

        private ContextElement deviceCheckImageElement;
        private ContextElement accountCheckImageElement;

        private bool isBlockDevice = false;

        private ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private void Start()
        {
            InitProperty();
        }

        private void InitProperty()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();

            root.UpdateContext(false);

            // Title
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Title Area/Text", "POPUP_BLOCK_TITLE", FULL);

            // Contents
            var contentsAreaElement = ContextUtils.FindElement(root, "Contents Friend Request Block", CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(contentsAreaElement, "Text", "POPUP_BLOCK_TEXT", CHILDREN);

            MetaContextElementUtils.SimpleSetTextGlobal(contentsAreaElement, "Radio Button 1/Text", "POPUP_BLOCK_TOGGLE_USER", FULL);
            MetaContextElementUtils.SimpleSetTextGlobal(contentsAreaElement, "Radio Button 2/Text", "POPUP_BLOCK_TOGGLE_ACCOUNT", FULL);

            selectDeviceElement = ContextUtils.FindElement(contentsAreaElement, "Radio Button 1/Toggle", FULL);
            selectAccountElement = ContextUtils.FindElement(contentsAreaElement, "Radio Button 2/Toggle", FULL);

            deviceCheckImageElement = ContextUtils.FindElement(selectDeviceElement, "On", CHILDREN);
            accountCheckImageElement = ContextUtils.FindElement(selectAccountElement, "On", CHILDREN);

            MetaContextElementUtils.SetClickable(selectDeviceElement, OnToggleDevice);
            MetaContextElementUtils.SetClickable(selectAccountElement, OnToggleAccount);

            // Cancel
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Button Block/Text", "BUTTON_PROFILE_BLOCK_YES", FULL);
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Button Cancel/Text", "BUTTON_PROFILE_BLOCK_NO", FULL);

            MetaContextElementUtils.SimpleSetClickable(root, "Button Close", OnCancel);
            MetaContextElementUtils.SimpleSetClickable(root, "Button Cancel", OnCancel);
            MetaSystem.SubscribeBackButton(gameObject.GetHashCode(), OnCancel);

            // Block
            MetaContextElementUtils.SimpleSetClickable(root, "Button Block", OnBlock);

            // Default
            OnToggleDevice();
        }

        private void OnDestroy()
        {
            MetaSystem.UnSubscribeBackButton(gameObject.GetHashCode());
        }

        private void OnCancel()
        {
            EventSender.SendCalleeCallback(gameObject, "OnNo");
            MetaPopupUtils.ClosePopup(gameObject);
        }

        private void OnBlock()
        {
            var caller = BlackboardUtils.GetOrCreateVariable<GameObject>(bb, "caller")?.value;
            if(caller != null)
            {
                var eventData = new EventData<BlockType>("OnYes",
                    isBlockDevice ? BlockType.DEVICE : BlockType.USER);
                EventSender.SendEvent(caller, eventData);
            }

            MetaPopupUtils.ClosePopup(gameObject);
        }

        private void OnToggleDevice()
        {
            if (isBlockDevice) return;
            isBlockDevice = true;

            MetaContextElementUtils.SetActive(deviceCheckImageElement, true);
            MetaContextElementUtils.SetActive(accountCheckImageElement, false);
        }

        private void OnToggleAccount()
        {
            if (!isBlockDevice) return;
            isBlockDevice = false;

            MetaContextElementUtils.SetActive(deviceCheckImageElement, false);
            MetaContextElementUtils.SetActive(accountCheckImageElement, true);
        }
    }
}
