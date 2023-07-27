using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Services;

namespace BagelCode
{
    public class PopupVIPLoungeRewardController : MonoBehaviour
    {
        private ContextElement rootElement;
        private Blackboard rootBB;
        private Animator rootAnimator;

        private ContextElement anchorElement;
        private ContextElement textElement;

        private Transform targetTransform;

        private bool isInit = false;

        public float anchorMovementTime = 0.2f;

        private void Start()
        {
            OnInit();
        }

        private void OnInit()
        {
            if (isInit) return;

            InitProperty();
            SetTextProgress();

            SetAnimatorTrigger("isActive");

            isInit = true;
        }

        private void InitProperty()
        {
            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);
            rootBB = gameObject.GetComponent<Blackboard>();
            rootAnimator = gameObject.GetComponent<Animator>();

            anchorElement = ContextUtils.FindElement(rootElement, "Anchor", ContextSearchingType.ChildrenSearch);
            textElement = ContextUtils.FindElement(anchorElement, "Text Progress", ContextSearchingType.ChildrenSearch);

            targetTransform = BlackboardUtils.FindVariable<Transform>(rootBB, "endTransform")?.value ?? null;
        }

        private void SetTextProgress()
        {
            // todo : get lounge reward percent - NumberUtils.GetPercentFromNumerator or NumberUtils.GetAdditionalPercent & EN_Global stringKey Change
            long percent = BlackboardQueryUtils.GetVIPLoungeClubVegasRewardAdditionalPercent();
            if (textElement != null)
                MetaContextElementUtils.SetText(textElement, string.Format("+ {0}%", percent));
        }

        public void SetAnimatorTrigger(string key)
        {
            rootAnimator?.SetTrigger(key);
        }

        public void MoveAnchor()
        {
            //Debug.Log("MoveAnchor : " + targetTransform.name);
            if (targetTransform == null)
                return;

            Vector3 from = anchorElement.transform.position;
            Vector3 to = targetTransform.position;

            AsyncActionUtils.ApplyMovement(
                this,
                anchorElement.transform,
                from,
                to,
                anchorMovementTime,
                TweenUtils.VectorTweenCollectMove,
                0f,
                null
            );
        }

        public void ArriveAnchor()
        {
            var caller = BlackboardUtils.FindVariable<GameObject>(rootBB, "caller");
            if (caller != null && caller.value != null)
                EventSender.SendEvent(caller.value, MessageRouter.ON_CUSTOM_EVENT, new EventData("OnCalleeCallback"));
        }
    }
}