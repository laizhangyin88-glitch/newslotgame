using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Tasks.Actions.Contents
{

    [Category("★ BagelCode/lobby")]
    public class ShowLastJackpot : ActionTask
    {
        protected override string info => "展示断线前彩金";

        protected override void OnExecute()
        {
            bool isWinLobbyJackpot = LobbyJackpotManager.Instance.IsWinLobbyJackpot;

            //if (BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "isWinLobbyJackpot") != null)
            //    isWinLobbyJackpot = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "isWinLobbyJackpot").value;

            if (isWinLobbyJackpot)
                StartCoroutine(ShowPopupWinLobbyJackpot());
            else
                EndAction();
        }

        private IEnumerator ShowPopupWinLobbyJackpot()
        {
            LobbyJackpotManager.Instance.OpenJackpotPop("Overlay");
            yield return new WaitForSeconds(5f);
            EndAction();
        }
    }
}
