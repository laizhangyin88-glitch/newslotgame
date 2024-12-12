using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode;
using System;

public class LobbyJackpotManager : MonoSingleton<LobbyJackpotManager>
{
    private List<WinResult> _jackpotList = new List<WinResult>();

    /// <summary>
    /// 指示当前用户是否有未显示的彩金数据
    /// </summary>
    public bool IsWinLobbyJackpot => _jackpotList.Count > 0;

    /// <summary>
    /// 添加彩金数据
    /// </summary>
    /// <remarks>
    /// 这些彩金数据应该是当前用户的彩金数据，PopupWinLobbyJackpot弹窗会依次对这些彩金数据做显示
    /// </remarks>
    /// <param name="jackpotData"></param>
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

    public string GetNumStr(int value)
    {
        return string.Format("{0:N2}", value);
    }
}

public class WinResult
{
    public string user_id;
    public string nick_name;
    public int single_reward;
    public int bonus_id;

    public override string ToString()
    {
        return $"user_id : {user_id}; nick_name : {nick_name}; single_reward : {single_reward}; bonus_id : {bonus_id};";
    }
}
