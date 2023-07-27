using System;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using UnityEngine;
using UnityEngine.Events;
using BagelCode.Slots.GDD.Utillity;
using TMPro;
using BagelCode.Slots.GDD.Global;

namespace BagelCode.Slots.GDD.Popup
{
    public class GDDFreeGameResultPopup : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Blackboard blackBoard;
        [SerializeField] private Animator popupAnimator;
        [SerializeField] private ContextElement contextElement;
        [SerializeField] private PIDButton button;
        [SerializeField] private TextMeshProUGUI text;

        [Space(20)]
        [Header("Variables")]
        [SerializeField] private UnityEvent onDestroyCallback;

        private void Awake()
        {
            if (blackBoard == null) blackBoard = GetComponent<Blackboard>();
            if (popupAnimator == null) popupAnimator = GetComponentInChildren<Animator>();
            if (contextElement == null) contextElement = GetComponent<ContextElement>();
            if (button == null) button = GetComponentInChildren<PIDButton>();
            if (text == null) text = gameObject.transform.Find("Animator/Pay/Pay Txt").GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()
        {

            try
            {
                contextElement.UpdateContext(true);
                button.onClick.AddListener(OnClickButton);
                GDDUtillity.PlaySound(GDDSound.FreeGameResultPopupAppear);
                GDDUtillity.PlaySound(GDDSound.BonusGameResult);

                long totalEarnCredit = GDDUtillity.TryGetGlobalBlackBoardVariable<long>("./bonus/totalEarnCredit");
                text.text = $"{totalEarnCredit.ToString("#,##0")}";
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }

        }

        private void OnClickButton()
        {
            GDDUtillity.PlaySound(GDDSound.Button);
            popupAnimator.SetTrigger("Disappear");
            StartCoroutine(DestroyTimer());
        }

        private IEnumerator DestroyTimer()
        {
            yield return null;
            while (popupAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99f)
            {
                yield return null;
            }
            try
            {
                if (onDestroyCallback != null) onDestroyCallback.Invoke();
                MessageDispatcher.Dispatch(SendEvent.ON_CONTENT_UI_EVENT, new EventData(GDDEvent.GDDContentUIEvent.FreeGameResultPopupClose));
                gameObject.SetActive(false);
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
        }
    }
}