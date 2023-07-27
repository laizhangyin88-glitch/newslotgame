using System;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using UnityEngine;
using UnityEngine.Events;
using BagelCode.Slots.GDD.Utillity;
using BagelCode.Tasks.Actions.BI;
using TMPro;
using BagelCode.Slots.GDD.Global;

namespace BagelCode.Slots.GDD.Popup
{
    public class GDDFreeGameAdditionalPopup : MonoBehaviour
    {
        //static variable

        [Header("Components")]
        [SerializeField] private Blackboard blackBoard;
        [SerializeField] private Animator popupAnimator;
        [SerializeField] private ContextElement contextElement;
        [SerializeField] private TextMeshProUGUI addedSpinCountText;

        [Space(20)]
        [Header("Variables")]
        [SerializeField] private UnityEvent onDestroyCallback;

        private void Awake()
        {
            if (blackBoard == null) blackBoard = GetComponent<Blackboard>();
            if (contextElement == null) contextElement = GetComponent<ContextElement>();
            if (popupAnimator == null) popupAnimator = GetComponentInChildren<Animator>();
            if (addedSpinCountText == null) addedSpinCountText = transform.Find("Animator/Pay/Pay Txt").GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()
        {

            try
            {
                contextElement.UpdateContext(true);
                GDDUtillity.ChangeSnapShot("Content_Popup");
                GDDUtillity.PlaySound(GDDSound.FreeGameAdditionalPopupAppear);

                ApplyAddedSpinCount();

                var bI_freespin_trigger = new BI_freespin_trigger();
                bI_freespin_trigger.isAdd = new BBParameter<bool>(true);
                bI_freespin_trigger.ExecuteAction(blackBoard);
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);

            }
            finally
            {
                StartCoroutine(DestroyTimer());
            }
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
                MessageDispatcher.Dispatch(SendEvent.ON_CONTENT_UI_EVENT, new EventData(GDDEvent.GDDContentUIEvent.FreeGameAdditionalPopupClose));
                gameObject.SetActive(false);
                GDDUtillity.ChangeSnapShot("Content_Main");
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
        }



        private void ApplyAddedSpinCount()
        {
            int bonusID = GDDUtillity.TryGetGlobalBlackBoardVariable<int>("./bonus/bonusId");
            var spin = GDDUtillity.TryGetGlobalBlackBoardVariable<Blackboard>("./spin");

            if (bonusID == 0 || spin == null) throw new Exception();
            Variable bbvResponse = blackBoard.AddVariable("response", typeof(Blackboard));
            bbvResponse.value = ContentBlackboardUtils.GetBonusResponse(spin, bonusID);

            if (!bbvResponse.isValid() || !bbvResponse.value.isValid()) throw new Exception();
            var response = bbvResponse.value as Blackboard;

            var addedSpinCount = GDDUtillity.TryGetLocalBlackBoardVariable<int>(response, "addedSpinCount");


            addedSpinCountText.text = $"+{addedSpinCount}";
        }
    }
}