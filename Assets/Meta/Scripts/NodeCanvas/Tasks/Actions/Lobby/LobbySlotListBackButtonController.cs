using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode.ClientModels;
using ParadoxNotion;
using ParadoxNotion.Services;
using NodeCanvas.Framework;

namespace BagelCode
{
    public class LobbySlotListBackButtonController : MonoBehaviour
    {
        private Animator buttonAnimator;
        private Variable<bool> isShowLobbyBackButton;

        private IEnumerator checkerRoutine;

        private void Awake()
        {
            buttonAnimator = GetComponent<Animator>();
            isShowLobbyBackButton = BlackboardUtils.GetOrCreateVariable<bool>("/isShowLobbyBackButton");
            checkerRoutine = CheckButtonStatus();
        }

        private void OnEnable()
        {
            StartCoroutine(checkerRoutine);
        }

        private void OnDisable()
        {
            StopCoroutine(checkerRoutine);
        }

        IEnumerator CheckButtonStatus()
        {
            if(buttonAnimator == null) yield break;

            while(true)
            {
                buttonAnimator.SetBool("IsOn", isShowLobbyBackButton.value);

                yield return new WaitForSeconds(0.33f);
            }
        }
    }
}
