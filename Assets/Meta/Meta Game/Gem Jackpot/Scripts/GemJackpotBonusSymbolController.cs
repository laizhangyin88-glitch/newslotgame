using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using TMPro;

namespace BagelCode.GemJackpot
{
    public class GemJackpotBonusSymbolController : MonoBehaviour
    {
        public Animator animator = null;
        public ContextElement textElement = null;

        public void SetText(string text)
        {
            ContextUtils.SetText(textElement, text);
        }

        public void SetAnimationTrigger(string trigger)
        {
            if (animator != null)
                animator.SetTrigger(trigger);
        }
    }
}