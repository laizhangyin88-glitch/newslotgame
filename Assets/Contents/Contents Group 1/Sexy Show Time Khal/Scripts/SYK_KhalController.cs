using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SlotMaker;
using ParadoxNotion;
using TMPro;

namespace GS.Slot.SYK
{
    public class SYK_KhalController : MonoBehaviour
    {
        [SerializeField]
        private Animator khalAnimator;
        [SerializeField]
        private List<int> clothOffMultiplierList;
        public float randomAnimationProbablity = 0.5f;

        private int currentMultiplier;
        private int currnetClothOffIndex = -1;

        public const string KHAL_WIN_ANIM_TRIGGER_EVENT = "KhalSetTriggerWin";
        public const string KHAL_INS_TRIGGER_EVENT = "KhalINSTrigger";
        public const string ON_JACKPOT_WIN = "OnJackpotWin";
        public const string SPIN_SLOT_EVENT = "SpinSlotMachine";
        public const string ON_SLOT_EVENT = "OnSlotEvent";
        public const string ON_SLOT_DETAL_EVENT = "OnSlotDetailEvent";
        public const string ON_BEGIN_EXPECTATION_EVENT = "BeginExpectation";
        public const string ON_END_EXPECTATION_EVENT = "EndExpectation";
        public const string BACK_TO_BASE_IDLE_EVENT = "KhalBackToBaseIdle";

        public const string IS_ON_CLOTH_OFF_VARIABLE_PATH = "./isOnClothOff";
        private const int MULTPLIER_UP_SOUND_COUNT = 6;

        private bool isOnFS = false;

        private void OnEnable()
        {
            currentMultiplier = 1;
            currnetClothOffIndex = -1;

            BlackboardUtils.GetOrCreateVariable<bool>(null, IS_ON_CLOTH_OFF_VARIABLE_PATH).value = false;
            MessageDispatcher.Register(ON_SLOT_EVENT, RandomAnimation);
            MessageDispatcher.Register(SYK_FSMultiplierController.CONTENT_UI_EVENT, ContentUIDelegator);
            MessageDispatcher.Register(ON_SLOT_DETAL_EVENT, SlotDetailEventDelegator);
        }

        private void OnDisable()
        {
            MessageDispatcher.UnRegister(ON_SLOT_EVENT, RandomAnimation);
            MessageDispatcher.UnRegister(SYK_FSMultiplierController.CONTENT_UI_EVENT, ContentUIDelegator);
            MessageDispatcher.UnRegister(ON_SLOT_DETAL_EVENT, SlotDetailEventDelegator);
        }

        private void RandomAnimation(EventData eventData)
        {
            if (eventData.name == SPIN_SLOT_EVENT && eventData.id == 0)
            {
                var randomAnimProb = Random.Range(0, 1f);
                if (randomAnimProb <= randomAnimationProbablity) khalAnimator.SetTrigger("Random");
            }
        }

        private void SlotDetailEventDelegator(EventData eventData)
        {
            if(eventData.name == ON_BEGIN_EXPECTATION_EVENT)
                khalAnimator.SetBool("Expectation", true);
            else if (eventData.name == ON_END_EXPECTATION_EVENT)
                khalAnimator.SetBool("Expectation", false);
        }

        private void ContentUIDelegator(EventData eventData)
        {
            if (eventData.name == KHAL_WIN_ANIM_TRIGGER_EVENT)
            {
                khalAnimator.SetTrigger("Win");

                if (isOnFS)
                {
                    khalAnimator.SetTrigger("End FreeSpin");
                    currentMultiplier = 1;
                    currnetClothOffIndex = -1;
                    isOnFS = false;
                }
                else isOnFS = true;
            }
            else if (eventData.name == KHAL_INS_TRIGGER_EVENT)
            {
                khalAnimator.SetTrigger("Force Start");
                isOnFS = true;
            }
            else if (eventData.name == BACK_TO_BASE_IDLE_EVENT)
            {
                khalAnimator.SetTrigger("Back To Base Idle");
                isOnFS = false;
            }
            else if (eventData.name == ON_JACKPOT_WIN)
            {
                khalAnimator.SetTrigger("Win");
                GSManager.Instance.GetHandler("Jackpot Trigger").Play();
            }
            else if (eventData.name == SYK_FSMultiplierController.MULTIPLIER_UP_EVENT)
            {
                currentMultiplier++;
                khalAnimator.SetTrigger("Multiplier Up");

                int nextClothOffStage = currnetClothOffIndex + 1;
                if (nextClothOffStage < clothOffMultiplierList.Count)
                {
                    bool isReachNextMultiplier = currentMultiplier >= clothOffMultiplierList[nextClothOffStage];
                    if (isReachNextMultiplier)
                    {
                        BlackboardUtils.GetOrCreateVariable<bool>(null, IS_ON_CLOTH_OFF_VARIABLE_PATH).value = true;
                        StartCoroutine(SYK_FSMultiplierController.CallActionAfterDelay(() =>
                        {
                            khalAnimator.SetTrigger("Cloth Off");
                            currnetClothOffIndex++;
                        }, 0.1f));

                        StartCoroutine(SYK_FSMultiplierController.CallActionAfterDelay(() =>
                        {
                            BlackboardUtils.GetOrCreateVariable<bool>(null, IS_ON_CLOTH_OFF_VARIABLE_PATH).value = false;
                        }, 7f));
                    }
                    else
                    {
                        StartCoroutine(SYK_FSMultiplierController.CallActionAfterDelay(() =>
                        {
                            int soundIndex = (currentMultiplier - 1) % MULTPLIER_UP_SOUND_COUNT;
                            soundIndex = soundIndex == 0 ? soundIndex + 1 : soundIndex;
                            string soundName = string.Format("Vox Multiplier Up {0}", soundIndex);
                            GSManager.Instance.GetHandler(soundName).Play();
                        }, 4.2f));
                    }

                } else
                {
                    StartCoroutine(SYK_FSMultiplierController.CallActionAfterDelay(() =>
                    {
                        int soundIndex = (currentMultiplier - 1) % MULTPLIER_UP_SOUND_COUNT;
                        soundIndex = soundIndex == 0 ? soundIndex + 1 : soundIndex;
                        string soundName = string.Format("Vox Multiplier Up {0}", soundIndex);
                        GSManager.Instance.GetHandler(soundName).Play();
                    }, 4.2f));
                }
            }
        }
    }
}