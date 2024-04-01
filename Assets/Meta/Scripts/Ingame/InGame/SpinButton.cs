using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using ParadoxNotion;
using NodeCanvas.Framework;
using SlotMaker;
using TMPro;
using System;
using SboxSpace;

namespace BagelCode
{
    public class SpinButton : InGameButton, IPointerDownHandler, IPointerUpHandler
    {
        public float autoSpinDelay;
        public Button button;
        public Animator animator;
        public TextMeshProUGUI partySpinText;

        public string spinSound = "UI_Spin_Start";
        public string autoSpinSound = "UI_Spin_Auto";

        private ContextElement root;

        private ContextElement luckySpinElement;
        private ContextElement bonusSpinElement;

        private List<ContextElement> defaultStopElementList = new List<ContextElement>();
        private List<ContextElement> gameSpinStopElementList = new List<ContextElement>();

        private Variable<SpinType> spinType;
        public SpinType SpinType
        {
            get { return spinType.value; }
        }

        private Variable<bool> autoSpin;
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

        private static readonly string ON_SPINBUTTON_EVENT = "OnSpinButtonEvent";

        private int ANI_IS_STOP = Animator.StringToHash("IsStop");
        private int ANI_IS_AUTO = Animator.StringToHash("IsAuto");
        private int ANI_IS_GAME_SPIN = Animator.StringToHash("IsPartySpin");

        public void LeaveGame()
        {
            button.interactable = false;
        }

        public void ReadyGame()
        {
            button.interactable = true;
        }

        public void FailSpin()
        {
            AutoSpin = false;
            GSManager.Instance.GetHandler(spinSound).Play();
        }

        public void SpinSlotMachine()
        {
            if (!AutoSpin)
                button.interactable = false;

            animator.SetBool(ANI_IS_STOP, true);
        }

        public void StopSlotMachine()
        {
            button.interactable = true;
        }

        public void StoppedSlotMachine()
        {
            animator.SetBool(ANI_IS_STOP, false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            PointerDown();
        }

        private void PointerDown()
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
            PointerUp();
            //IOEventCenter.SendEvent(IOCenterEvent.EVENT_KeyStatus, new object[2] { 10, 0 });
        }

        private void PointerUp()
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
            GSManager.Instance.GetHandler(spinSound).Play();
            MessageDispatcher.Dispatch(ON_SPINBUTTON_EVENT, new EventData(ON_SPINBUTTON_EVENT));
        }

        private IEnumerator WaitForAutoSpinDelay()
        {
            yield return new WaitForSeconds(autoSpinDelay);

            AutoSpin = true;
            GSManager.Instance.GetHandler(autoSpinSound).Play();

            MessageDispatcher.Dispatch(ON_SPINBUTTON_EVENT, new EventData(ON_SPINBUTTON_EVENT));

            autoSpinProgress = AutoSpinProgress.Done;
        }

        private void UpdateSpinType(string name, object value)
        {
            if (SpinType == SpinType.None)
            {
                SetState(true);

                return;
            }
            else
            {
                if (SpinType == SpinType.GameSpin || SpinType == SpinType.BonusSpin)
                {
                    luckySpinElement.gameObject.SetActive(SpinType == SpinType.GameSpin);
                    bonusSpinElement.gameObject.SetActive(SpinType == SpinType.BonusSpin);

                    SetState(false);
                }
                else
                {
                    SetState(true);
                }
            }
        }

        private void UpdateAutoSpin(string name, object value)
        {
            animator.SetBool(ANI_IS_AUTO, AutoSpin);

            PIPManager.Instance.SetPipState("AutoSpin", (bool)value);
        }

        protected override void Awake()
        {
            base.Awake();
            spinType = BlackboardUtils.FindVariable<SpinType>("./spinType");
            autoSpin = BlackboardUtils.FindVariable<bool>("./autoSpin");
        }

        private void Start()
        {
            root = GetComponent<ContextElement>();
            root.UpdateContext(true);

            luckySpinElement = ContextUtils.FindElement(root, "Text Lucky Spin", ContextSearchingType.ChildrenSearch);
            bonusSpinElement = ContextUtils.FindElement(root, "Text Bonus Spin", ContextSearchingType.ChildrenSearch);

            defaultStopElementList.Add(ContextUtils.FindElement(root, "Stop Text/Text Stop", ContextSearchingType.FullNameSearch));
            defaultStopElementList.Add(ContextUtils.FindElement(root, "Auto Spin Text/Text Stop", ContextSearchingType.FullNameSearch));

            gameSpinStopElementList.Add(ContextUtils.FindElement(root, "Stop Text/Text Stop Lucky", ContextSearchingType.FullNameSearch));
            gameSpinStopElementList.Add(ContextUtils.FindElement(root, "Auto Spin Text/Text Stop Lucky", ContextSearchingType.FullNameSearch));

            // Set Spin/Play Button
            var spinElement = ContextUtils.FindElement(root, "Text Spin", ContextSearchingType.ChildrenSearch);
            var playElement = ContextUtils.FindElement(root, "Text Play", ContextSearchingType.ChildrenSearch);
            var spinDescElement = ContextUtils.FindElement(root, "Auto Spin Text/Text Spin Description", ContextSearchingType.FullNameSearch);
            var playDescElement = ContextUtils.FindElement(root, "Auto Spin Text/Text Play Description", ContextSearchingType.FullNameSearch);

            var gameType = BlackboardQueryUtils.GetGameType(BlackboardQueryUtils.GetIngameID());
            bool isKeno = gameType == ClientModels.GameType.KENO;

            MetaContextElementUtils.SetActive(spinElement, !isKeno);
            MetaContextElementUtils.SetActive(spinDescElement, !isKeno);
            MetaContextElementUtils.SetActive(playElement, isKeno);
            MetaContextElementUtils.SetActive(playDescElement, isKeno);


            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);
            Register(MetaEventDefine.ON_META_UI_EVENT, MachineEventDefine.ON_KEY_START, OnMachinePoint);
        }

        private void OnMachinePoint(EventData data)
        {
            if ((int)data.value == 1)
                PointerDown();
            else
                PointerUp();
        }

        private void SetState(bool isDefault)
        {
            foreach (var obj in defaultStopElementList)
                obj.gameObject.SetActive(isDefault);

            foreach (var obj in gameSpinStopElementList)
                obj.gameObject.SetActive(!isDefault);

            animator.SetBool(ANI_IS_GAME_SPIN, !isDefault);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            spinType.onValueChanged += UpdateSpinType;
            autoSpin.onValueChanged += UpdateAutoSpin;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            spinType.onValueChanged -= UpdateSpinType;
            autoSpin.onValueChanged -= UpdateAutoSpin;
        }
    }
}
