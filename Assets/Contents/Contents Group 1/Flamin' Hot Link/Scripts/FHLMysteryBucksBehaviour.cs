using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using SlotMaker;
using NodeCanvas.Framework;
using ParadoxNotion;

namespace GameStudio.Slot.FHL
{
    public class FHLMysteryBucksBehaviour : SymbolBehaviour
    {
        private const int Win = 0;
        public override void OnPrepareStop()
        {
            GetCachedObject(2).GetComponentInChildren<Animator>().SetTrigger("Disappear");
            bool isRespin = BlackboardUtils.FindVariable<bool>(null, "./customData/isRespin").value;
            int rowCount = isRespin ? 8 : 4;
            int reelIndex = symbol.reel.reelIndex;
            int rowIndex = reelIndex % rowCount;
            if (symbol.row != rowIndex)
            {
                DefaultSymbolEventHandler defaultEventHandler = eventHandler as DefaultSymbolEventHandler;
                defaultEventHandler.symbolPresets[symbol.symbolIndex].value[0].spriteRenderer.sprite = symbol.symbolAssets.GetSprite(symbol.symbolIndex, 1);
            }
        }

        public override void OnSkip()
        {
            PlayAnimation("Idle");
        }

        public override void OnStopEffect()
        {
            GetCachedObject(1);
            var e = new EventData("Shake");
			MessageDispatcher.Dispatch("OnContentUIDetailEvent", e);
            GSManager.Instance.GetHandler("Wheel Symbol Land").Play();
        }

        public override void OnWin()
        {
            GetCachedObject(1).GetComponentInChildren<Animator>().SetTrigger("Win");
        }
        public override void OnEntry()
        {
            animator.gameObject.SetActive(true);
            bool _isRespinIntro = BlackboardUtils.FindVariable<bool>(null, "./customData/_isRespinIntro").value;
            if (!_isRespinIntro)
            {
                GetCachedObject(2);
            }
        }

        public void LockSymbol() 
        {
            GetCachedObject(1).GetComponentInChildren<Animator>().SetTrigger("Fix");
        }

        public void PlayWheel()
        {
            GameObject wheel = BlackboardUtils.FindValue<GameObject>(GetCachedObject(1).GetComponent<Blackboard>(), "wheelGO");
            GameObject wheelCanvas = BlackboardUtils.FindValue<GameObject>(wheel.GetComponent<Blackboard>(), "WheelCanvas");
            wheelCanvas.GetComponent<Canvas>().sortingLayerName = "Foreground";
            int index = symbol.column * 8 + symbol.row;
            List<Blackboard> mysteryBucksResultList = BlackboardUtils.FindValue<List<Blackboard>>(null, "./bonus/response/mysteryBucksResultList");
            for (int i = 0; i < mysteryBucksResultList.Count; i++)
            {
                int row = BlackboardUtils.FindValue<int>(mysteryBucksResultList[i], "row");
                int col = BlackboardUtils.FindValue<int>(mysteryBucksResultList[i], "col");
                int wheelIndex = col * 8 + row;
                if (wheelIndex == index)
                {
                    int prizeIndex = BlackboardUtils.FindValue<int>(mysteryBucksResultList[i], "value");
                    wheel.GetComponent<Blackboard>().SetValue("_prizeIndex", prizeIndex);
                }
            }
            GetCachedObject(1).GetComponent<SortingGroup>().sortingLayerName = "Foreground";
            GetCachedObject(1).GetComponent<SortingGroup>().sortingOrder = 9;
            var e = new EventData("WheelSpinStart");
            wheel.GetComponent<ContentUIDetailEventDispatcher>().Dispatch(e);
            GSManager.Instance.GetHandler("Wheel Symbol On").Play();
        }

        public void ChangeSortingGroup()
        {
            GameObject wheel = BlackboardUtils.FindValue<GameObject>(GetCachedObject(1).GetComponent<Blackboard>(), "wheelGO");
            GameObject wheelCanvas = BlackboardUtils.FindValue<GameObject>(wheel.GetComponent<Blackboard>(), "WheelCanvas");
            wheelCanvas.GetComponent<Canvas>().sortingLayerName = "Midground";
            GetCachedObject(1).GetComponent<SortingGroup>().sortingLayerName = "Base";
            GetCachedObject(1).GetComponent<SortingGroup>().sortingOrder = 20;
        }
    }
}
