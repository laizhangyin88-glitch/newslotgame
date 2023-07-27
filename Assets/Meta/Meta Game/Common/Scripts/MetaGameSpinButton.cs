using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace BagelCode.MetaGame
{
    public class MetaGameSpinButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private ContextElement rootElement;
        public ContextElement callerElement;

        public float autoSpinDelay;
        public Button button;
        public Animator animator;
        public TextMeshProUGUI partySpinText;

        public string spinSound = "";
        public string autoSpinSound = "";   // "UI_Spin_Auto";

        public UnityBoolEvent onUpdateAutoSpin;

        private Variable<bool> autoSpin = new Variable<bool>();
        public bool AutoSpin
        {
            get { return autoSpin.value; }
            set { autoSpin.value = value; }
        }

        private enum AutoSpinProgress
        {
            DoNothing,
            Trying,
            Done
        };
        private AutoSpinProgress autoSpinProgress = AutoSpinProgress.DoNothing;

        private static readonly string ON_SPINBUTTON_EVENT = "OnClickStartSpin";

        private readonly string ANI_IS_STOP = "IsStop";
        private readonly string ANI_IS_AUTO = "IsAuto";

        private void Awake()
        {
            AutoSpin = false;
        }

        private void Start()
        {
            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext();
        }

        public void SpinSlotMachine()
        {
            //if (!AutoSpin)
            //    button.interactable = false;

            animator.SetBool(ANI_IS_STOP, true);
        }

        public void StopSlotMachine()
        {
            //button.interactable = true;
        }

        public void StoppedSlotMachine()
        {
            animator.SetBool(ANI_IS_STOP, false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!button.IsActive() || !button.IsInteractable())
                return;

            if (!AutoSpin)
            {
                autoSpinProgress = AutoSpinProgress.Trying;
                StartCoroutine("WaitForAutoSpinDelay");
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (autoSpinProgress == AutoSpinProgress.Trying)
                StopCoroutine("WaitForAutoSpinDelay");

            switch (autoSpinProgress)
            {
                case AutoSpinProgress.DoNothing:
                    AutoSpin = false;
                    break;
                case AutoSpinProgress.Trying:
                    Spin();
                    break;
                case AutoSpinProgress.Done:
                    break;
            }
            autoSpinProgress = AutoSpinProgress.DoNothing;
        }

        private void Spin()
        {
            if (!string.IsNullOrEmpty(spinSound))
                GSManager.Instance.GetHandler(spinSound).Play();
            if (callerElement != null)
                MetaContextElementUtils.SendEvent(callerElement, ON_SPINBUTTON_EVENT, null, null);
        }

        private IEnumerator WaitForAutoSpinDelay()
        {
            yield return new WaitForSeconds(autoSpinDelay);

            AutoSpin = true;
            if (!string.IsNullOrEmpty(autoSpinSound))
                GSManager.Instance.GetHandler(autoSpinSound).Play();
            if (callerElement != null)
                MetaContextElementUtils.SendEvent(callerElement, ON_SPINBUTTON_EVENT, null, null);

            autoSpinProgress = AutoSpinProgress.Done;
        }

        private void UpdateAutoSpin(string name, object value)
        {
            animator.SetBool(ANI_IS_AUTO, AutoSpin);
            if (onUpdateAutoSpin != null)
                onUpdateAutoSpin.Invoke((bool)value);
        }

        private void OnEnable()
        {
            autoSpin.onValueChanged += UpdateAutoSpin;
        }

        private void OnDisable()
        {
            autoSpin.onValueChanged -= UpdateAutoSpin;
        }
    }
}