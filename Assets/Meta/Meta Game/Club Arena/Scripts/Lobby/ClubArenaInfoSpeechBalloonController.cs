using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;
using UnityEngine.Events;
using NodeCanvas.Framework;

namespace BagelCode.ClubArena
{
    public class ClubArenaInfoSpeechBalloonController : MonoBehaviour
    {
        private ContextElement rootElement;
        private Animator rootAnimator;

        private float waitTime = 2.0f;
        private float displayTime = 6.0f;
        private float checkTime = 0.0f;

        private bool isAnimActive = false;
        private bool isInit = false;
        private bool isLockedLevel = false;

        public void OnReadyGame()
        {
            isAnimActive = true;
        }

        public void OnEnterTurn()
        {
            Display(false);
        }

        public void UpdateTotalBet(long totalBetCredit)
        {
            Display(false);
        }

        private void InitProperty()
        {
            if (isInit) return;

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);
            rootAnimator = gameObject.GetComponent<Animator>();

            isLockedLevel = MetaGameUtils.IsMetaGameLevelLocked();

            isInit = true;
        }

        private void Start()
        {
            InitProperty();
        }

        private void Update()
        {
            if (isAnimActive && isInit && !isLockedLevel)
            {
                if (checkTime < waitTime)
                    checkTime += Time.deltaTime;
                else if (checkTime < displayTime + waitTime)
                {
                    if (rootAnimator.GetBool("IsActive") == false)
                        Display(true);
                    checkTime += Time.deltaTime;
                }
                else
                    Display(false);
            }
        }

        private void Display(bool isActive)
        {
            isAnimActive = isActive;
            rootAnimator.SetBool("IsActive", isAnimActive);
        }
    }
}
