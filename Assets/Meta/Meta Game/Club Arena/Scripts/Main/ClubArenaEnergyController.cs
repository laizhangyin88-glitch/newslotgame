using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using NodeCanvas.Framework;
using BagelCode.ClientModels;

namespace BagelCode.ClubArena
{
    public class ClubArenaEnergyController : MonoBehaviour
    {
        private Animator rootAnimator;

        private ContextElement textElement;
        private ContextElement appearEnergyTextElement;

        private const int ProgressEnergyMax = 50;
        private const int ProgressEnergyRed = 10;

        private bool isInit = false;

        public void OnInit(ContextElement rootElement)
        {
            if (isInit) return;

            ContextElement anchorElement = ContextUtils.FindElement(rootElement, "Progress Anchor", ContextSearchingType.ChildrenSearch);
            rootAnimator = anchorElement.GetComponent<Animator>();

            textElement = ContextUtils.FindElement(anchorElement, "Text Energy", ContextSearchingType.ChildrenSearch);
            appearEnergyTextElement = ContextUtils.FindElement(anchorElement, "Text Appear Energy", ContextSearchingType.ChildrenSearch);

            isInit = true;
        }

        public void SetAppearEnergy(long energy = 0)
        {
            if (appearEnergyTextElement != null)
                MetaContextElementUtils.SetTextGlobal(appearEnergyTextElement, "CLUB_ARENA_APPEAR_ENERGY_TEXT", energy);
            if (rootAnimator != null)
                rootAnimator.SetTrigger("energyGet");
        }

        public void SetEnergy(long energy)
        {
            MetaContextElementUtils.SetTextGlobal(textElement, "CLUB_ARENA_WHEEL_ENERGY", energy);
        }
    }
}