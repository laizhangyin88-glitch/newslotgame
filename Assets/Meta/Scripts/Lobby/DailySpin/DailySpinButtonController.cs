using UnityEngine;
using UnityEngine.EventSystems;
using NodeCanvas.Framework;
using SlotMaker;
using System.Collections;

namespace BagelCode
{
    public class DailySpinButtonController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public GraphOwner owner;
        public Blackboard ownerBB;

        public bool isAuto = false;

        private ContextElement root;
        private Animator anim;
        private PIDButton button;

        private ContextElement coverElement;

        private float autoSpinProgress = 0f;
        private bool isTryAutoSpin = false;

        private const float AUTO_SPIN_DELAY = 0.65f;

        private const string SPIN_SOUND = "UI_Spin_Start";
        private const string AUTO_SPIN_SOUND = "UI_Spin_Auto";

        private const string ON_DAILY_SPIN = "OnDailySpin";
        private const string ON_END_SPIN = "OnEndSpin";

        //

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!button.interactable) return;

            if (!isAuto)
            {
                isTryAutoSpin = true;
                autoSpinProgress = 0f;
            }
            else
            {
                Stop();
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!button.interactable) return;

            isAuto = ownerBB.GetVariable<bool>("isAuto")?.value ?? false;
            if (!isAuto)
            {
                Stop();
                Spin();
            }
        }

        public void SetIsAuto(bool _isAuto)
        {
            ownerBB.AddVariable("isAuto", _isAuto);
            isAuto = _isAuto;
        }

        public void ForceStopAuto()
        {
            if (anim != null &&
                (anim.GetCurrentAnimatorStateInfo(0).IsName("Normal To Auto") ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("Auto Spin")))
            {
                coverElement.gameObject.SetActive(false);

                button.interactable = true;

                isTryAutoSpin = false;
                autoSpinProgress = 0f;

                ownerBB.AddVariable("isAuto", false);

                anim.SetTrigger("IsForceStop");
            }
        }

        //

        private void Start()
        {
            InitProperty();
        }

        private void InitProperty()
        {
            root = GetComponent<ContextElement>();
            button = GetComponent<PIDButton>();

            root.UpdateContext(false);

            coverElement = ContextUtils.FindElement(root, "Anchor/Cover", ContextSearchingType.FullNameSearch);
            anim = ContextUtils.FindElement(root, "Anchor", ContextSearchingType.ChildrenSearch).GetComponent<Animator>();

            coverElement.gameObject.SetActive(false);
        }

        private void Spin()
        {
            coverElement.gameObject.SetActive(true);

            owner.SendEvent(ON_DAILY_SPIN);
            GSManager.Instance.GetHandler(SPIN_SOUND).Play();
        }

        private void AutoSpin()
        {
            coverElement.gameObject.SetActive(false);

            anim.ResetTrigger("IsStop");
            anim.SetTrigger("IsAuto");

            button.interactable = true;

            ownerBB.AddVariable("isAuto", true);

            owner.SendEvent(ON_DAILY_SPIN);
            GSManager.Instance.GetHandler(AUTO_SPIN_SOUND).Play();
        }

        private void Stop()
        {
            coverElement.gameObject.SetActive(true);

            button.interactable = false;

            isTryAutoSpin = false;
            autoSpinProgress = 0f;

            ownerBB.AddVariable("isAuto", false);

            anim.SetTrigger("IsStop");
        }

        private void FixedUpdate()
        {
            isAuto = ownerBB.GetVariable<bool>("isAuto")?.value ?? false;

            if (isTryAutoSpin)
            {
                autoSpinProgress += Time.deltaTime;
                if (autoSpinProgress >= AUTO_SPIN_DELAY)
                {
                    AutoSpin();
                    isTryAutoSpin = false;
                }
            }
        }
    }
}