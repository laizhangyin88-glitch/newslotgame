using System;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using UnityEngine;
using UnityEngine.Events;
using BagelCode.Slots.GDD.Utillity;
using BagelCode.Slots.GDD.Global;
using TMPro;

namespace BagelCode.Slots.GDD.Popup
{
    public class GDDJackpotResultPopup : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Animator popupAnimator;
        [SerializeField] private ContextElement contextElement;
        [SerializeField] private Blackboard blackboard;
        [SerializeField] private TextMeshProUGUI rewardText;
        [SerializeField] private PIDButton button;

        [Space(20)]
        [Header("Variables")]
        [SerializeField] private UnityEvent onDestroyCallback;

        private string jackpotSound;
        private string jackpotEndSound;

        private bool isSkipCounting = false;

        private void Awake()
        {
            if (blackboard == null) blackboard = GetComponent<Blackboard>();
            if (popupAnimator == null) popupAnimator = GetComponentInChildren<Animator>();
            if (contextElement == null) contextElement = GetComponent<ContextElement>();
            if (rewardText == null) rewardText = transform.Find("Aniamtor/Anchor/Pay/Pay Txt").GetComponent<TextMeshProUGUI>();
            if (button == null) button = transform.Find("Aniamtor/Button").GetComponent<PIDButton>();
        }

        private void OnEnable()
        {
            try
            {
                isSkipCounting = false;
                contextElement.UpdateContext(true);
                int jackpotIndex = GDDUtillity.TryGetLocalBlackBoardVariable<int>(blackboard, "jackpotIndex");
                popupAnimator.SetInteger("Index", jackpotIndex);
                switch (jackpotIndex)
                {
                    case 0:
                        jackpotSound = GDDSound.JackpotResultPopupMiniJackpot;
                        jackpotEndSound = GDDSound.JackpotResultPopupMiniJackpotEnd;
                        break;
                    case 1:
                        jackpotSound = GDDSound.JackpotResultPopupMinorJackpot;
                        jackpotEndSound = GDDSound.JackpotResultPopupMinorJackpotEnd;
                        break;
                    case 2:
                        jackpotSound = GDDSound.JackpotResultPopupMajorJackpot;
                        jackpotEndSound = GDDSound.JackpotResultPopupMajorJackpotEnd;
                        break;
                    case 3:
                        jackpotSound = GDDSound.JackpotResultPopupMegaJackpot;
                        jackpotEndSound = GDDSound.JackpotResultPopupMegaJackpotEnd;
                        break;
                    case 4:
                        jackpotSound = GDDSound.JackpotResultPopupGrandJackpot;
                        jackpotEndSound = GDDSound.JackpotResultPopupGrandJackpotEnd;
                        break;
                    default:
                        throw new Exception();
                }

                GDDUtillity.PlaySound(jackpotSound);
                StartCoroutine(CountingJackpotAmount(jackpotIndex));
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
            finally
            {
            }
        }

        IEnumerator CountingJackpotAmount(int jackpotIndex)
        {
            long jackpotEarnCredit = GDDUtillity.TryGetGlobalBlackBoardVariable<long>("./bonus/response/earnCredit");
            int tickCount = jackpotIndex > 3 ? 123 : 103;
            int currentTickCount = 0;
            long currentCountValue = 0;
            long addAmountPerTick = jackpotEarnCredit / tickCount;
            void OnClick()
            {
                if (currentTickCount > 40)
                    isSkipCounting = true;
            }

            button.onClick.AddListener(OnClick);

            while (currentTickCount < tickCount)
            {
                currentTickCount++;
                currentCountValue = currentTickCount * addAmountPerTick;
                rewardText.text = currentCountValue.ToString("#,##0");
                if (isSkipCounting) break;
                yield return new WaitForSeconds(0.03f);
            }
            button.onClick.RemoveListener(OnClick);
            currentCountValue = jackpotEarnCredit;
            rewardText.text = currentCountValue.ToString("#,##0");

            StartCoroutine(Disappear());
        }

        IEnumerator Disappear()
        {
            GDDUtillity.StopSound(jackpotSound);
            GDDUtillity.PlaySound(jackpotEndSound);
            GDDUtillity.PlaySound(GDDSound.JackpotResultPopupEnd);
            popupAnimator.SetTrigger("Disappear");
            yield return null;
            while (popupAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99f)
            {

                yield return null;
            }
            if (onDestroyCallback != null) onDestroyCallback.Invoke();
            GDDUtillity.ChangeSnapShot("Content_Main");
            MessageDispatcher.Dispatch(SendEvent.ON_CONTENT_UI_EVENT, new EventData(GDDEvent.GDDContentUIEvent.JackpotResultPopupClose));
            gameObject.SetActive(false);
        }


    }
}