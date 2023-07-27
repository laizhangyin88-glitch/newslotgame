using System;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion;
using SlotMaker;
using UnityEngine;
using UnityEngine.Events;
using BagelCode.Slots.GDD.Utillity;
using BagelCode.Slots.GDD.Global;

namespace BagelCode.Slots.GDD.Popup
{
    public class GDDRemoveRoyalPopup : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Animator popupAnimator;
        [SerializeField] private ContextElement contextElement;

        [Space(20)]
        [Header("Variables")]
        [SerializeField] private UnityEvent onDestroyCallback;

        private void Awake()
        {
            if (popupAnimator == null) popupAnimator = GetComponentInChildren<Animator>();
            if (contextElement == null) contextElement = GetComponent<ContextElement>();

        }
        private void OnEnable()
        {
            try
            {

                contextElement.UpdateContext(true);
                //   GDDUtillity.ChangeSnapShot("Content_Popup");
                GDDUtillity.PlaySound(GDDSound.RemoveRoyalPopupAppear);

                //Get BlackBoard Variable From Bonus Result.
                //RemoveSymbolList is contains symbol index that to remove symobl in reel strip.
                List<int> removeSymbolList = GDDUtillity.TryGetGlobalBlackBoardVariable<List<int>>("./bonus/response/removeSymbolList");

                //Remove Symbol Feature is remove ranged in 1 to 3 symbols only.
                //So if count of remvoeSymbolList is greater than 3 or less than 1, It is Exception. 
                if (removeSymbolList.Count <= 0 || removeSymbolList.Count >= 7)
                    throw new Exception($"GDD :: MysteryPickPopup :: Start :: removeSymbolList have abnormal count of elements\nit have {removeSymbolList.Count} elements.");

                //GDD Row is ranged only 3 to 6.
                int currentRow = GDDUtillity.TryGetGlobalBlackBoardVariable<int>("./customData/currentRow");
                if (currentRow < 3 || currentRow > 6)
                    throw new Exception($"GDD :: MysteryPickPopup :: Start :: current row is abnormal value \nit need ranged only 3 to 6, but it's {currentRow}.");
                removeSymbolList.Sort();
                ChangeAnimatorParams(removeSymbolList, currentRow);
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

        IEnumerator DestroyTimer()
        {
            while (popupAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99f)
            {
                yield return null;
            }
            if (onDestroyCallback != null) onDestroyCallback.Invoke();
            MessageDispatcher.Dispatch(SendEvent.ON_CONTENT_UI_EVENT, new EventData(GDDEvent.GDDContentUIEvent.RemoveRoyalPopupClose));

            gameObject.SetActive(false);
            // GDDUtillity.ChangeSnapShot("Content_Main");
        }




        private void ChangeAnimatorParams(List<int> removeSymbolIndexList, int currentRow)
        {
            popupAnimator.SetInteger("Position", removeSymbolIndexList.Count - 1);
            popupAnimator.SetInteger("Popup Position", currentRow - 3);
            popupAnimator.SetInteger("SymbolIndex_1", -1);
            popupAnimator.SetInteger("SymbolIndex_2", -2);
            popupAnimator.SetInteger("SymbolIndex_3", -1);
            popupAnimator.SetInteger("SymbolIndex_4", -1);
            popupAnimator.SetInteger("SymbolIndex_5", -1);
            popupAnimator.SetInteger("SymbolIndex_6", -1);
            if (removeSymbolIndexList.Count == 1)
            {
                popupAnimator.SetInteger("SymbolIndex_1", removeSymbolIndexList[0] - 8);
            }
            else if (removeSymbolIndexList.Count == 2)
            {
                popupAnimator.SetInteger("SymbolIndex_1", removeSymbolIndexList[0] - 8);
                popupAnimator.SetInteger("SymbolIndex_2", removeSymbolIndexList[1] - 8);
            }
            else if (removeSymbolIndexList.Count == 3)
            {
                popupAnimator.SetInteger("SymbolIndex_1", removeSymbolIndexList[0] - 8);
                popupAnimator.SetInteger("SymbolIndex_2", removeSymbolIndexList[1] - 8);
                popupAnimator.SetInteger("SymbolIndex_3", removeSymbolIndexList[2] - 8);
            }
            else if (removeSymbolIndexList.Count == 4)
            {
                popupAnimator.SetInteger("SymbolIndex_1", removeSymbolIndexList[0] - 8);
                popupAnimator.SetInteger("SymbolIndex_2", removeSymbolIndexList[1] - 8);
                popupAnimator.SetInteger("SymbolIndex_3", removeSymbolIndexList[2] - 8);
                popupAnimator.SetInteger("SymbolIndex_4", removeSymbolIndexList[3] - 8);
            }
            else if (removeSymbolIndexList.Count == 5)
            {
                popupAnimator.SetInteger("SymbolIndex_1", removeSymbolIndexList[0] - 8);
                popupAnimator.SetInteger("SymbolIndex_2", removeSymbolIndexList[1] - 8);
                popupAnimator.SetInteger("SymbolIndex_3", removeSymbolIndexList[2] - 8);
                popupAnimator.SetInteger("SymbolIndex_4", removeSymbolIndexList[3] - 8);
                popupAnimator.SetInteger("SymbolIndex_5", removeSymbolIndexList[4] - 8);
            }
            else if (removeSymbolIndexList.Count == 6)
            {
                popupAnimator.SetInteger("SymbolIndex_1", removeSymbolIndexList[0] - 8);
                popupAnimator.SetInteger("SymbolIndex_2", removeSymbolIndexList[1] - 8);
                popupAnimator.SetInteger("SymbolIndex_3", removeSymbolIndexList[2] - 8);
                popupAnimator.SetInteger("SymbolIndex_4", removeSymbolIndexList[3] - 8);
                popupAnimator.SetInteger("SymbolIndex_5", removeSymbolIndexList[4] - 8);
                popupAnimator.SetInteger("SymbolIndex_6", removeSymbolIndexList[5] - 8);
            }
        }
    }
}