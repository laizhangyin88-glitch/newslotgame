using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode;

public class LobbyJackpotManager : MonoSingleton<LobbyJackpotManager>
{
    private List<WinResult> _jackpotList = new List<WinResult>();

    public bool IsWinLobbyJackpot => _jackpotList.Count > 0;

    public void AddJackpot(WinResult jackpotData)
    {
        Debug.Log($"==@AddJackpot{jackpotData}");
        _jackpotList.Add(jackpotData);
    }

    public WinResult GetNextJackpot()
    {
        if (IsWinLobbyJackpot == false)
            return null;

        WinResult ret = _jackpotList[0];
        _jackpotList.RemoveAt(0);
        return ret;
    }

    public void OpenJackpotPop(string popLayer = "Area")
    {
        bool isOpen = PopupManager.Instance.IsOpen("Popup Win Lobby Jackpot");
        if (isOpen)
            return;//存在窗口时不让其打开，因为彩金数据一个窗口就可以处理完

        var sceneInfo = AssetBundleManager.LoadAsset<SceneInfoObject>(MetaStringDefine.LOBBY_BUNDLE_NAME, "Popup Win Lobby Jackpot Scene").GetSceneInfo();
        var parent = GameObject.Find($"Popup Manager/{popLayer}");
        var sceneObj = SceneManager.LoadScene(parent.transform, sceneInfo);
        PopupManager.Instance.Open(sceneObj);
        sceneObj.SetActive(true);
    }
}
