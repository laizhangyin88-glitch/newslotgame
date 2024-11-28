using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Tasks.Actions.Contents
{

    [Category("★ BagelCode/Contents")]
    public class EndTurn : ActionTask
    {
        protected override void OnExecute()
        {
            Blackboard cb = ContentBlackboard.Get();
            var turn = cb.GetValue<Blackboard>("turn");
            BlackboardUtils.SetOrCreateValue<long>(turn, "endTime", MetaSystem.GetTimeStamp());

            ContentEvent.EndTurn(turn);

            cb.RemoveVariable("current");

            //bool isWinLobbyJackpot = false;
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
            //BlackboardUtils.SetOrCreateValue(MainBlackboard.Get(), "isWinLobbyJackpot", false);
            //var sceneInfo = AssetBundleManager.LoadAsset<SceneInfoObject>(MetaStringDefine.LOBBY_BUNDLE_NAME, "Popup Win Lobby Jackpot Scene").GetSceneInfo();
            //var parent = GameObject.Find("Popup Manager/Area");
            //var sceneObj = SceneManager.LoadScene(parent.transform, sceneInfo);
            //PopupManager.Instance.Open(sceneObj);
            //sceneObj.SetActive(true);
            LobbyJackpotManager.Instance.OpenJackpotPop();
            yield return new WaitForSeconds(5);
            EndAction();
        }
    }
}
