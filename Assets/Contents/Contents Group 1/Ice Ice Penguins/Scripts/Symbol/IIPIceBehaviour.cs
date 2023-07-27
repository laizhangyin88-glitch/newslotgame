using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using GameStudio.Slot.IIP.Utility;
using NodeCanvas.Framework;
using SlotMaker;
using TMPro;
using UnityEngine;

namespace GameStudio.Slot.IIP.Symbol
{
    public class IIPIceBehaviour : SymbolBehaviour
    {
        private readonly int StopEffect = 0;

        private long bonusBet = 0;
        private TextMeshProUGUI valueText;

        public override void StartBehaviour(SymbolEventHandler eventHandler)
        {
            base.StartBehaviour(eventHandler);
            if (valueText == null) valueText = animator.transform.Find("Bet Text").GetComponent<TextMeshProUGUI>();
        }

        public override void OnEntry()
        {
            int userIndex = this.symbol.slotMachine.slotIndex - 1;
            //Display bet credit only player's symbol
            if (userIndex == 0)
            {
                animator.SetBool("Text", true);
                animator.Update(Time.deltaTime);

                List<Blackboard> userGameList = BlackboardUtils.FindVariable<List<Blackboard>>("./bonus/response/userGameResultList").value;
                bonusBet = userGameList[userIndex].GetVariable<long>("betMultiplier").value;

                valueText.text = FormatUtility.SimpleNumberFormat(bonusBet);
            }
        }

        public override void OnPrepareStop()
        {
            if (symbol.row == 0)
            {
                PlayAnimation("Deactive");

                animator.SetBool("Text", false);
                animator.Update(Time.deltaTime);

                IIPUtility.PlaySound("Ice Land");

                GetCachedObject(StopEffect).SetActive(true);
                TextMeshProUGUI stopEffectBetCreditText = GetCachedObject(StopEffect).GetComponentInChildren<TextMeshProUGUI>(true);
                int userIndex = this.symbol.slotMachine.slotIndex - 1;
                if (userIndex == 0)
                {
                    stopEffectBetCreditText.gameObject.SetActive(true);
                    stopEffectBetCreditText.text = FormatUtility.SimpleNumberFormat(bonusBet);
                }
                else stopEffectBetCreditText.gameObject.SetActive(false);

            }
        }

        public override void OnSkip()
        {
            PlayAnimation("Idle");
            GetCachedObject(StopEffect).SetActive(false);
        }

        public override void OnStopEffect()
        {
        }

        public override void OnWin()
        {
        }

        public void Hide()
        {
            PlayAnimation("Deactive");
            animator.SetBool("Text", false);
            animator.Update(Time.deltaTime);
            GetCachedObject(StopEffect).SetActive(false);
        }
    }
}
