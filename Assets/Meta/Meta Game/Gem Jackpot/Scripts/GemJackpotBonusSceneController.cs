using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion;

namespace BagelCode.GemJackpot
{
    public class GemJackpotBonusSceneController : MonoBehaviour
    {
        public Animator rootAnimator;

        private bool isInit = false;

        private void Awake()
        {
            OnInit();
        }

        public void OnInit()
        {
            if (isInit) return;

            rootAnimator = GetComponentInChildren<Animator>();

            isInit = true;
        }

        public void OnSpin()
        {
            if (rootAnimator != null)
            {
                // Check Animator state
                if (!rootAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base.Idle"))
                    return;
                // Animator trigger
                rootAnimator.SetTrigger("Spin");
                // Bonus slot machine spin
                MetaSlotMachineSendEvent sendEvent = GetComponent<MetaSlotMachineSendEvent>();
                sendEvent.DispatchMetaSlotMachineSpinButtonEvent("");
                // Play sound
                GSManager.Instance.GetHandler("Meta_Gemjackpot_Jackpotspinclick").Play();

                SendBIJackpotBonusSpin();
            }
        }

        public void OnOutro()
        {
            if (rootAnimator != null)
                rootAnimator.SetTrigger("Outro");
        }

        protected void SendBIJackpotBonusSpin()
        {
            Analytics.CustomEvent("client_click_gem_jackpot_bonus_spin", new Dictionary<string, object>
            {
                { "slot_enter_context_id", GemJackpotUtils.BISlotEnterContextID }
            });
        }
    }
}