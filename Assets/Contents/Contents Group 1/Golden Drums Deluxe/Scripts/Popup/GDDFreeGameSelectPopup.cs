using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using UnityEngine;
using UnityEngine.Events;
using BagelCode.Slots.GDD.Utillity;
using BagelCode.Slots.GDD.Global;
using UnityEngine.UI;
using BagelCode.Tasks.Actions.ClientAPI;
using BagelCode.Tasks.Actions.BI;

namespace BagelCode.Slots.GDD.Popup
{
    public class GDDFreeGameSelectPopup : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Animator popupAnimator;
        [SerializeField] private Animator mysteryAnimator;
        [SerializeField] private List<Button> selectButtonList = new List<Button>();
        [SerializeField] private ContextElement contextElement;

        [Space(20)]
        [Header("Variables")]
        [SerializeField] private UnityEvent onDestroyCallback;

        private int selectIndex;
        private void Awake()
        {
            if (popupAnimator == null) popupAnimator = transform.Find("Animator").GetComponent<Animator>();
            if (mysteryAnimator == null) mysteryAnimator = transform.Find("Animator/Select Popup Mystery Pick/Animator").GetComponent<Animator>();
            if (contextElement == null) contextElement = GetComponent<ContextElement>();
        }
        private void OnEnable()
        {
            try
            {
                contextElement.UpdateContext(true);
                StartCoroutine(Sequence());
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
        }

        IEnumerator Sequence()
        {
            MessageDispatcher.Dispatch("OnSoundEvent", new EventData("StopBgm"));
            yield return null;
            GDDUtillity.ChangeSnapShot("Content_Main");
            GDDUtillity.PlaySound(GDDSound.FreeGameSelectBGM);

            void OnClickButton(int index)
            {
                selectIndex = index;
                if (index == 4) popupAnimator.SetTrigger($"Select Mystery");
                else popupAnimator.SetTrigger($"Select Lv{index + 1}");
                selectButtonList.ForEach(each => each.onClick.RemoveAllListeners());
                StartCoroutine(OnClickSequence());
            }
            selectButtonList[0].onClick.AddListener(() => OnClickButton(0));
            selectButtonList[1].onClick.AddListener(() => OnClickButton(1));
            selectButtonList[2].onClick.AddListener(() => OnClickButton(2));
            selectButtonList[3].onClick.AddListener(() => OnClickButton(3));
            selectButtonList[4].onClick.AddListener(() => OnClickButton(4));

        }
        IEnumerator OnClickSequence()
        {
            GDDUtillity.PlaySound(GDDSound.Button);

            List<Blackboard> selectOptionList = GDDUtillity.TryGetGlobalBlackBoardVariable<List<Blackboard>>("./customData/selectOptionList");
            Blackboard selectOptionInfo = selectOptionList[selectIndex];
            int bonusID = GDDUtillity.TryGetGlobalBlackBoardVariable<int>("./bonus/bonusId");
            ClaimBonus claimBonus = new ClaimBonus();
            claimBonus.bonusId = bonusID;
            claimBonus.selectedIndex = selectIndex;
            claimBonus.ExecuteAction(this);
            yield return new WaitForSeconds(0.1f);

            int rowIndex = GDDUtillity.TryGetLocalBlackBoardVariable<int>(selectOptionInfo, "rowIndex");
            int countIndex = GDDUtillity.TryGetLocalBlackBoardVariable<int>(selectOptionInfo, "countIndex");
            int count = GDDUtillity.TryGetLocalBlackBoardVariable<int>(selectOptionInfo, "count");
            GDDUtillity.TrySetGlobalBlackBoardVariable<int>("./bonus/totalSpinCount", count);


            if (selectIndex < 4)
            {
                GDDUtillity.PlaySound(GDDSound.FreeGameStart);
            }
            else if (selectIndex == 4)
            {
                popupAnimator.SetTrigger("Question Mark 01 Appear");
                mysteryAnimator.SetInteger("RowIndex", rowIndex);
                mysteryAnimator.SetInteger("FreeGameCountIndex", countIndex);
                yield return new WaitForSeconds(1.4f);
                mysteryAnimator.SetTrigger("RowSelect");
                popupAnimator.SetTrigger("Question Mark 01 Diappear");
                yield return new WaitForSeconds(4.5f);
                mysteryAnimator.SetTrigger("FreeGameCountSelect");
                popupAnimator.SetTrigger("Question Mark 02 Disappear");
                yield return new WaitForSeconds(4.5f);
                GDDUtillity.PlaySound(GDDSound.FreeGameStart);
            }

            BI_freespin_trigger bI_Freespin_Trigger = new BI_freespin_trigger();
            bI_Freespin_Trigger.isAdd = false;
            bI_Freespin_Trigger.ExecuteAction(GetComponent<Blackboard>());

            popupAnimator.SetTrigger("Disappear");
            int row = GDDUtillity.TryGetLocalBlackBoardVariable<int>(selectOptionInfo, "row");

            GDDUtillity.TrySetGlobalBlackBoardVariable<int>("./customData/freeGameRow", row);
            GDDUtillity.TrySetGlobalBlackBoardVariable<int>("./customData/freeGameCount", count);
            GDDUtillity.TrySetGlobalBlackBoardVariable<int>("./game/reelSetIndex/nextIndex", 0);

            yield return new WaitForSeconds(1.6f);

            MessageDispatcher.Dispatch(SendEvent.ON_CONTENT_UI_EVENT, new EventData(GDDEvent.GDDContentUIEvent.FreeGameSelectPopupClose));
            GDDUtillity.StopSound(GDDSound.FreeGameSelectBGM);
            GDDUtillity.ChangeSnapShot("Content_Main");
            gameObject.SetActive(false);
        }
    }
}