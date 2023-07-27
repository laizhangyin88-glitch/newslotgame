using UnityEngine;
using SlotMaker;

namespace BagelCode.HiddenObjects
{
    public class HiddenObjectsPopupPauseController : MonoBehaviour
    {
        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private ContextElement root;
        private Animator anim;

        private void Start()
        {
            root = GetComponent<ContextElement>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            anim.SetBool("Active", true);

            // Enable Blur
            BlurManager.SetBlur(true);

            // Title
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Title Area/Text", "HIDDEN_OBJECTS_POPUP_PAUSE_TITLE", FULL);

            // Text
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Center Text Anchor/Text", "HIDDEN_OBJECTS_POPUP_PAUSE_TEXT", FULL);

            // Quit
            var quitButtonElement = ContextUtils.FindElement(root, "Button Quit", CHILDREN);
            MetaContextElementUtils.SetClickable(quitButtonElement, Quit);
            MetaContextElementUtils.SimpleSetTextGlobal(quitButtonElement, "Text", "BUTTON_QUIT", CHILDREN);

            // Resume
            var resumeButtonElement = ContextUtils.FindElement(root, "Button Resume", CHILDREN);
            MetaContextElementUtils.SetClickable(resumeButtonElement, Resume);
            MetaContextElementUtils.SimpleSetTextGlobal(resumeButtonElement, "Text", "BUTTON_RESUME", CHILDREN);
        }

        private void Quit()
        {
            EventSender.SendCalleeCallback(gameObject, HiddenObjects.Events.ON_QUIT);
            anim.SetTrigger("Close");
            MetaPopupUtils.ClosePopup(gameObject);
        }

        private void Resume()
        {
            EventSender.SendCalleeCallback(gameObject, HiddenObjects.Events.ON_RESUME);
            anim.SetTrigger("Close");
            MetaPopupUtils.ClosePopup(gameObject);
        }

        private void OnDestroy()
        {
            BlurManager.SetBlur(false);
        }
    }
}
