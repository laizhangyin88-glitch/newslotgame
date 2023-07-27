using UnityEngine;
using UnityEngine.Events;
using SlotMaker;
using BagelCode.MetaGame;

namespace BagelCode.BossRaiders
{
    public abstract class BossRaidersWheelControllerBase : MonoBehaviour
    {
        protected ContextElement callerElement;

        protected ContextElement rootElement;
        protected Animator rootAnimator;

        protected UnityAction ownerAdsTimerCallback;

        protected BossRaidersWheelBase wheelController;
        protected MetaGameSpinButton spinButtonController;

        protected virtual void InitWheelData() { }

        protected abstract void InitProperty();
        protected abstract void InitWheelController();
        protected abstract void InitSpinButtonController();

        public abstract void ChangeWheelData(long changeValue);

        public virtual void OnInit(ContextElement caller)
        {
            callerElement = caller;

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);
            rootAnimator = gameObject.GetComponent<Animator>();

            InitProperty();
            InitWheelController();
            InitSpinButtonController();
            InitWheelData();
        }

        public virtual void SetActiveSpinButton(bool isSpin, bool isReadyToAds) { }
        public virtual void OpenSpeechBalloon() { }
        public virtual void UpdateWheelData(bool isReadyToAds) { }

        public Animator GetAnimator()
        {
            return rootAnimator;
        }

        public ContextElement GetHighlightElement()
        {
            if (wheelController != null)
                return wheelController.GetHighlightElement();
            return null;
        }

        public void SetAdsTimerCallback(UnityAction callback)
        {
            ownerAdsTimerCallback = callback;
        }

        protected void SetTriggerAnimation(string key)
        {
            rootAnimator?.SetTrigger(key);
        }

        public void SetActiveAnimation(string key, bool isActive)
        {
            if (rootAnimator != null)
                rootAnimator.SetBool(key, isActive);
        }
        // Wheel
        protected void SetWheelData(long multi)
        {
            wheelController?.SetWheelData(multi);
        }

        public Animator GetHighlightAnimator(string cellName)
        {
            return wheelController?.GetHighlightAnimator(cellName);
        }
        // Spin Button
        public void SetAutoSpin(bool isAutoSpin)
        {
            if (spinButtonController != null)
                spinButtonController.AutoSpin = isAutoSpin;
        }

        public void SpinWheel(bool isSpin)
        {
            if (spinButtonController != null)
            {
                if (isSpin)
                    spinButtonController.SpinSlotMachine();
                else
                    spinButtonController.StoppedSlotMachine();
            }
        }

        public void SetSpinButtonActiveCover(bool isActive)
        {
            if (spinButtonController != null)
            {
                spinButtonController.GetComponent<PIDButton>()?.disableCover?.SetActive(isActive);
            }
        }
    }
}