using UnityEngine;
using SlotMaker;
using System.Collections;
using NodeCanvas.Framework;

namespace BagelCode.HiddenObjects
{
    public class VipLoungeSpinRewardReceivedController : MonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;
        private Animator anim;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private void Start()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            anim.SetTrigger("Active");

            var dayCount = bb.GetValue<int>("dayCount");
            
            // Title
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Title Area/Text", "VIP_LOUNGE_SPIN_REWARD_RESULT_TITLE", FULL);
            
            // Text
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Item/Text", "VIP_LOUNGE_SPIN_REWARD_PRODUCT_DAY", FULL, dayCount);

            // Make Finder Image
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "VIP Lounge Shop Reward Item";
            Transform parent = ContextUtils.FindElement(root, "Item/Image Area", FULL).transform;
            MetaObjectUtils.MakePrefab(bundle, asset, parent);

            // Collect
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Button Collect/Text", "BUTTON_COLLECT", FULL);
            MetaContextElementUtils.SimpleSetClickable(root, "Button Collect", Close);
        }

        private void Close()
        {
            EventSender.SendCalleeCallback(gameObject);

            anim.SetTrigger("Close");
            PopupManager.Instance.Close(gameObject);
        }
    }
}
